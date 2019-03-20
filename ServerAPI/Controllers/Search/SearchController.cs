using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Threading;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using BlockchainAppAPI.Logic.Search;
using BlockchainAppAPI.Models.Search;
using BlockchainAppAPI.DataAccess.Configuration;

namespace BlockchainAppAPI.Controllers.Search
{
    [Route("api/search")]
    public class SearchController : Controller
    {
        private readonly string _currentUser;
        private readonly SystemContext _context;


        public SearchController(IHttpContextAccessor httpContextAccessor, SystemContext context)
        {
            _currentUser = httpContextAccessor.CurrentUser();
            _context = context;
        }

        [HttpGet]
        [Route("")]
        public async Task<JObject> GetSearchObjectResults(string q, int start = 0, int end = 10, bool metadata = false)
        {
            string searchName = "Unnamed-" + Logic.Utility.SearchHash.HashSearch(q);

            SearchObject search = await this._context.SearchObjects.
                Where(s => s.Name == searchName).
                FirstOrDefaultAsync();

            if(search == null) {
                JObject searchQuery = JObject.Parse(q);
                search = new SearchObject() {
                    SearchObjectId = Guid.NewGuid(),
                    Name = searchName,
                    Description = "Unnamed search",
                    ModuleName = searchQuery["moduleName"].ToString(),
                    ObjectName = searchQuery["objectName"].ToString(),
                    IsValid = false,
                    DateCreated = DateTime.UtcNow,
                    CreatedBy = Guid.Empty,
                    DateModified = DateTime.UtcNow,
                    ModifiedBy = Guid.Empty,
                    IsActive = true,
                    Selections = ((JArray)searchQuery["selection"]).Select(s => {
                        JObject selection = (JObject)s;
                        return new Selection(){
                            ObjectFieldName = selection["objectField"].ToString(),
                            Aggregate = selection["aggregate"]?.ToString(),
                            Function = selection["function"]?.ToString(),
                            DateCreated = DateTime.UtcNow,
                            CreatedBy = Guid.Empty,
                            DateModified = DateTime.UtcNow,
                            ModifiedBy = Guid.Empty,
                            IsActive = true,
                        };
                    }).ToList()
                };

                await this._context.SearchObjects.AddAsync(search);
                await this._context.SaveChangesAsync();
            }

            if(!search.IsValid) {
                search = await new SearchCompiler(this._context).Compile(searchName);
                this._context.SearchObjects.Update(search);
                await this._context.SaveChangesAsync();
            }

            int count = -1;

            if(metadata) {
                DataTable dt1 = evaluateSearch(search.CompiledCountQuery, -1, -1);
                count = Int32.Parse(dt1.Rows[0].ItemArray[0].ToString());
            }

            DataTable dt2 = evaluateSearch(search.CompiledQuery, start, end);

            JArray resultSet = JArray.FromObject(
                dt2.Rows.Cast<DataRow>().Select(dr => 
                    JArray.FromObject(dr.ItemArray)
                )
            );

            if(metadata) {
                return JObject.FromObject(new {
                    ResultSize = count,
                    FieldList = search.CompiledFieldList,
                    Name = search.Name,
                    Chunk = resultSet
                });
            }
            else {
                return JObject.FromObject(new {
                    Chunk = resultSet
                });
            }
        }

        // GET api/search?q=q
        [HttpGet]
        [Route("{searchName}")]
        public async Task<JObject> GetSearchResults(string searchName, int start = 0, int end = 10, bool metadata = false)
        {
            SearchObject search = await this._context.SearchObjects.
                Where(s => s.Name == searchName).
                FirstOrDefaultAsync();

            if(search == null) {
                throw new Exception("Could not find search");
            }

            if(!search.IsValid) {
                search = await new SearchCompiler(this._context).Compile(searchName);
                this._context.SearchObjects.Update(search);
                await this._context.SaveChangesAsync();
            }

            int count = -1;

            if(metadata) {
                DataTable dt1 = evaluateSearch(search.CompiledCountQuery, -1, -1);
                count = Int32.Parse(dt1.Rows[0].ItemArray[0].ToString());
            }

            DataTable dt2 = evaluateSearch(search.CompiledQuery, start, end);

            JArray resultSet = JArray.FromObject(
                dt2.Rows.Cast<DataRow>().Select(dr => 
                    JArray.FromObject(dr.ItemArray)
                )
            );

            if(metadata) {
                return JObject.FromObject(new {
                    ResultSize = count,
                    FieldList = search.CompiledFieldList,
                    Chunk = resultSet
                });
            }
            else {
                return JObject.FromObject(new {
                    Chunk = resultSet
                });
            }
        }
        
        [HttpGet]
        [Route("{searchName}/metadata")]
        public async Task<JObject> GetSearchMetadata(string searchName)
        {
            SearchObject search = await this._context.SearchObjects.
                Where(s => s.Name == searchName).
                FirstOrDefaultAsync();

            if(search == null) {
                throw new Exception("Could not find search");
            }

            if(!search.IsValid) {
                search = await new SearchCompiler(this._context).Compile(searchName);
                this._context.SearchObjects.Update(search);
                await this._context.SaveChangesAsync();
            }

            DataTable dt = evaluateSearch(search.CompiledCountQuery, -1, -1);

            int count = Int32.Parse(dt.Rows[0].ItemArray[0].ToString());

            return JObject.FromObject(new {
                ResultSize = count,
                FieldList = search.CompiledFieldList
            });
        }
        private DataTable evaluateSearch(string query, int start, int end) 
        {
            DataTable dt = new DataTable();

            DbConnection conn = this._context.Database.GetDbConnection();
            ConnectionState connectionState = conn.State;
            try
            {
                if (connectionState != ConnectionState.Open) conn.Open();
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;
                    if(start >= 0 && end >= 0) {
                        cmd.Parameters.Add(new SqlParameter("start", start));
                        cmd.Parameters.Add(new SqlParameter("end", end));
                    }
                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connectionState != ConnectionState.Closed) conn.Close();
            }

            return dt;
        }
    }
}