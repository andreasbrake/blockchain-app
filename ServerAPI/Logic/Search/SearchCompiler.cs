using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using BlockchainAppAPI.DataAccess.Configuration;
using BlockchainAppAPI.Models.Configuration;
using BlockchainAppAPI.Models.Search;

namespace BlockchainAppAPI.Logic.Search
{
    public class SearchCompiler
    {
        private SystemContext _context;

        public SearchCompiler(SystemContext context)
        {
            this._context = context;
        }

        public async Task<SearchObject> Compile(string searchName)
        {
            SearchObject search = await this._context.SearchObjects.
                Include(so => so.Selections).
                Where(so => so.Name == searchName).
                FirstOrDefaultAsync();

            List<string> fieldList = this.compileFieldList(search);

            string selectClause = this.compileSelectClause(search);
            string fromClause = this.compileFromClause(search);
            string whereClause = this.compileWhereClause(search);
            string groupByClause = this.compileGroupByClause(search);
            string orderByClause = this.compileOrderByClause(search);

            search.CompiledQuery = $@"
                SELECT {selectClause}
                FROM {fromClause}
                {(whereClause != null ? $"WHERE {whereClause}" : "")}
                {(groupByClause != null ? $"GROUP BY {groupByClause}" : "")}
                ORDER BY {orderByClause}
                OFFSET @start ROWS
                FETCH NEXT (@end - @start + 1) ROWS ONLY
            ";

            if(groupByClause != null) {
                search.CompiledCountQuery = $@"
                    SELECT COUNT(*)
                    FROM (
                        SELECT {selectClause}
                        FROM {fromClause}
                        {(whereClause != null ? $"WHERE {whereClause}" : "")}
                        GROUP BY {groupByClause}
                    ) t1
                ";
            }
            else {
                search.CompiledCountQuery = $@"
                    SELECT COUNT(*)
                    FROM {fromClause}
                    {(whereClause != null ? $"WHERE {whereClause}" : "")}
                ";
            }


            search.CompiledFieldList = fieldList;

            search.IsValid = true;
            
            return search;
        }

        private List<string> compileFieldList(SearchObject search)
        {
            List<Selection> selections = search.Selections;

            return selections.Select(s => {
                return $"{s.ObjectFieldName}";
            }).ToList();
        }

        private string compileSelectClause(SearchObject search)
        {
            List<Selection> selections = search.Selections;

            return String.Join(
                ",",
                selections.Select(s => {
                    if(s.Aggregate != null && s.Aggregate.Trim() != "") {
                        return $"{s.Aggregate}([{s.ObjectFieldName}]) as [{s.Aggregate}_{s.ObjectFieldName}]"; 
                    }
                    return $"[{s.ObjectFieldName}] as [{s.ObjectFieldName}]";
                }
            ));
        }

        private string compileFromClause(SearchObject search)
        {
            return $"{search.ModuleName}_{search.ObjectName}";
        }

        private string compileWhereClause(SearchObject search)
        {
            return null;
        }

        private string compileGroupByClause(SearchObject search)
        {
            if(!search.Selections.Exists(s => 
                s.Aggregate != null &&
                s.Aggregate.Trim() != ""
            )) {
                return null;
            }

            List<Selection> selections = search.Selections.Where(s => 
                s.Aggregate == null ||
                s.Aggregate.Trim() == ""
            ).ToList();

            return String.Join(
                ",",
                selections.Select(s => {
                    return $"[{s.ObjectFieldName}]";
                }
            ));
        }

        private string compileOrderByClause(SearchObject search)
        {
            return search.Selections.First().ObjectFieldName + " ASC";
        }
    }
}