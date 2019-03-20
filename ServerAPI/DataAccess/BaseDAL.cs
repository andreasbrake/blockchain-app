using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BlockchainAppAPI.DataAccess
{
    public class BaseDAL
    {
        private string _source { get; set; }
        private string _procedure { get; set; }
        private IEnumerable<DALParameter> _params { get; set; }

        private string _requestLogString
        {
            get
            {
                return "EXEC " + _procedure + " " + String.Join(", ", _params.Select(p => 
                    p.Name + "='" + p.Value.ToString().Replace("'", "''") + "'"
                ));
            }
        }

        public BaseDAL(IConfiguration configuration, string source = null)
        {
            this._source = source ?? (
                configuration.GetConnectionString("ApplicationDB")
                ??
                System.Environment.GetEnvironmentVariable("APPLICATION_DB")   
            );
        }

        public void createRequest(string procedure, params DALParameter[] parameters)
        {
            this._procedure = procedure;
            this._params = parameters;
        }

        public async Task ExecuteNonQuery()
        {
            using(SqlConnection connection = new SqlConnection(_source))
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = this._procedure;
                foreach(SqlParameter p in _params.Select(p => p.Parameter))
                {
                    command.Parameters.Add(p);
                }

                connection.Open();
                await command.ExecuteNonQueryAsync();
                connection.Close();
            }
        }

        public async Task<T> ExecuteScalar<T>()
        {
            object response = default(T);
            using(SqlConnection connection = new SqlConnection(_source))
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = this._procedure;
                foreach(SqlParameter p in _params.Select(p => p.Parameter))
                {
                    command.Parameters.Add(p);
                }

                connection.Open();
                response = await command.ExecuteScalarAsync();
                connection.Close();
            }
            return (T)response;
        }

        #region get results

        public async Task<T> GetObject<T>()
        {
            return (await GetResults(typeof(T)))[0].Cast<T>().FirstOrDefault();
        }

        public async Task<IEnumerable<T>> GetObjectList<T>()
        {
            return (await GetResults(typeof(T)))[0].Cast<T>();
        }


        public async Task<object[][]> GetResultMatrix()
        {
            object[] results = await GetResult(
                (t) => t.Rows.Cast<DataRow>().Select(r => r.ItemArray).ToArray()
            );

            return results.Cast<object[]>().ToArray();
        }
        public async Task<object[]> GetResult(Func<DataTable, IEnumerable<object>> cb)
        {
            return (await GetResults(cb))[0];
        }

        public async Task<object[][]> GetResults(params Type[] ts)
        {
            object[][] results = await GetResults(
                ts.Select(t => 
                    DefaultParse(t)
                ).ToArray()
            );

            return results;
        }

        public async Task<object[]> GetSingleResults(Func<SqlDataReader, object> cbs)
        {
            List<object> output = new List<object>();
            
            using(SqlConnection connection = new SqlConnection(_source))
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = this._procedure;
                foreach(SqlParameter p in _params.Select(p => p.Parameter))
                {
                    command.Parameters.Add(p);
                }

                connection.Open();
                
                using(SqlDataReader dr = await command.ExecuteReaderAsync())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            output.Add(cbs(dr));
                        }
                    }
                }

                connection.Close();
            }
            
            return output.ToArray();
        }

        public async Task<object[][]> GetResults(params Func<DataTable, IEnumerable<object>>[] cbs)
        {
            DataSet ds = new DataSet();
            
            using(SqlConnection connection = new SqlConnection(_source))
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = this._procedure;
                foreach(SqlParameter p in _params.Select(p => p.Parameter))
                {
                    command.Parameters.Add(p);
                }

                connection.Open();
                
                try
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            SqlDataAdapter adp = new SqlDataAdapter(command);
                            adp.Fill(ds);
                        }
                        catch(Exception e)
                        {
                            throw new Exception("Error running command " + this._requestLogString, e);
                        }
                    });
                }
                catch(Exception e) 
                {
                    throw e;
                }

                connection.Close();
            }
            
            return ds.Tables.Cast<DataTable>().Select((dt, i) => 
                cbs[i](dt).ToArray()
            ).ToArray();
        }

        #endregion

        #region support
        private Func<DataTable, IEnumerable<object>> DefaultParse(Type t)
        {
            return dt => {
                Dictionary<PropertyInfo, int> propLookup = dt.Columns.Cast<DataColumn>()
                    .Select(c => c.ToString())
                    .Where(c => t.GetProperties().Where(p => p.Name == c).Count() > 0)
                    .ToDictionary (
                        c => t.GetProperties().Where(p => p.Name == c).First(),
                        c => dt.Columns.IndexOf(c)
                    );

                foreach(PropertyInfo p in t.GetProperties())
                {
                }

                return dt.Rows.Cast<DataRow>().Select(r => {
                    object[] items = r.ItemArray;

                    object entity = Activator.CreateInstance(t);
                    
                    foreach(PropertyInfo p in t.GetProperties())
                    {
                        if(propLookup.ContainsKey(p)) 
                        {
                            p.SetValue(entity, r.ItemArray[propLookup[p]], null);
                        }
                    }
                    
                    return entity;
                });
            };
        }
        
        #endregion
    }

    public class DALParameter
    {
        public string Name { get; set; }
        public SqlDbType? Type { get; set; }
        public object Value { get; set; }
        public SqlParameter Parameter 
        {
            get
            {
                if(Type != null)
                {
                    return new SqlParameter(Name, Type)
                    {
                        Value = Value
                    };
                }
                return new SqlParameter(Name, Value);
            }
        }

        public DALParameter(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }
        public DALParameter(string name, SqlDbType type, object value)
        {
            this.Name = name;
            this.Type = type;
            this.Value = value;
        }
    }
}