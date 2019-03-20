using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BlockchainAppAPI.DataAccess.Configuration;
using BlockchainAppAPI.Logic.Data;
using BlockchainAppAPI.Models.Configuration;
using StackExchange.Redis;

namespace BlockchainAppAPI.Controllers.Configuration
{
    //[Authorize]
    [Route("api/data")]
    public class DataController: Controller
    {
        private readonly string _currentUser;
        private readonly IDataManager _dm;
        private readonly SystemContext _context;
        
        public DataController(IHttpContextAccessor httpContextAccessor, IDataManager dm, SystemContext context)
        {
            _currentUser = httpContextAccessor.CurrentUser();
            _context = context;
            _dm = dm;
        }

        // GET api/data
        [HttpGet]
        [Route("{moduleName}/{objectName}/{objectId}")]
        public async Task GetObjectData(string moduleName, string objectName, string objectId)
        {
            BaseDataModel response = await _dm.LookupObject(moduleName, objectName, objectId);
            
            await _dm.PushToStore($"{moduleName}-{objectName}-{objectId}", response.Data);
        }
        
        // GET api/data
        [HttpPost]
        [Route("{moduleName}/{objectName}/{objectId}")]
        public async Task SetObjectData(string moduleName, string objectName, string objectId, [FromBody]JObject objectUpdates)
        {
            //TODO: Validate object fields exist

            List<KeyValuePair<string, string>> updateLookup = new List<KeyValuePair<string, string>>();
            foreach(KeyValuePair<string, JToken> kv in objectUpdates) {
                updateLookup.Add(
                    new KeyValuePair<string, string>(kv.Key, kv.Value.ToString())
                );
            }
            
            string fieldLookup = $@"
                UPDATE [dbo].[{moduleName}_{objectName}] 
                    SET { 
                        String.Join(
                            ", ", 
                            updateLookup.Select(f => 
                                String.Format(@"{0} = '{1}'", f.Key, f.Value) 
                            )
                        ) 
                    }
                WHERE [{objectName}Id] = @OID";

            _context.Database.ExecuteSqlCommand(fieldLookup, new SqlParameter("OID", objectId));
            
            BaseDataModel response = await _dm.LookupObject(moduleName, objectName, objectId);

            await _dm.PushToStore($"{moduleName}-{objectName}-{objectId}", response.Data);
        }

        
    }
}