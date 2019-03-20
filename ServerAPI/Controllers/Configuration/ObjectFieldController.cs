using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using BlockchainAppAPI.DataAccess.Configuration;
using BlockchainAppAPI.Models.Configuration;

namespace BlockchainAppAPI.Controllers.Configuration
{
    // [Authorize]
    [Route("api/objectField")]
    public class ObjectFieldController : Controller
    {
        private readonly string _currentUser;
        private readonly SystemContext _context;

        public ObjectFieldController(IHttpContextAccessor httpContextAccessor, SystemContext context)
        {
            _currentUser = httpContextAccessor.CurrentUser();
            _context = context;
        }

        // GET api/objectField/{moduleName}/{objectName}
        [HttpGet]
        [Route("{moduleName}/{objectName}")]
        public async Task<JArray> GetAll(string moduleName, string objectName)
        {
            IEnumerable<ObjectField> model = await this._context.ObjectFields.Where(of => 
                of.Object.Name == objectName && of.Object.Module.Name == moduleName
            ).ToListAsync();
            return JArray.FromObject(model);
        }

        // GET api/objectField/{moduleName}/{objectName}/{objectFieldName}
        [HttpGet]
        [Route("{moduleName}/{objectName}/{objectFieldName}")]
        public async Task<JObject> Get(string moduleName, string objectName, string objectFieldName)
        {
            ObjectField model = await _context.ObjectFields.Where(of => 
                of.Name == objectFieldName && of.Object.Name == objectName && of.Object.Module.Name == moduleName
            ).FirstOrDefaultAsync();
            
            return model == null ? null : JObject.FromObject(model);
        }

        // PUT api/object/{moduleName}/{objectName}/{objectFieldName}
        [HttpPut]
        [Route("{moduleName}/{objectName}/{objectFieldName}")]
        public async Task<JObject> Put(string moduleName, string objectName, string objectFieldName, [FromBody]JObject value)
        {
            ObjectField model = value.ToObject<ObjectField>();

            await this._context.ObjectFields.AddAsync(model);

            await _context.SaveChangesAsync();

            return model == null ? null : JObject.FromObject(model);
        }

        // // POST api/object/{moduleName}/{objectName}/{objectFieldName}
        [HttpPost]
        [Route("{moduleName}/{objectName}/{objectFieldName}")]
        public async Task<JObject> Post(string moduleName, string objectName, string objectFieldName, [FromBody]JObject value)
        {
            ObjectField objectFieldUpdate = value.ToObject<ObjectField>();
            
            ObjectField baseObjectField = await _context.ObjectFields.Where(of => 
                of.Name == objectFieldName && of.Object.Name == objectName && of.Object.Module.Name == moduleName
            ).FirstOrDefaultAsync();

            if(baseObjectField != null) {
                baseObjectField.Description = objectFieldUpdate.Description;
                baseObjectField.IsActive = objectFieldUpdate.IsActive;
                baseObjectField.DateModified = DateTime.UtcNow;
                
                _context.ObjectFields.Update(baseObjectField);
            }
            else {
                Models.Configuration.Object baseObject = await _context.Objects.Where(o => 
                    o.Name == objectName && o.Module.Name == moduleName
                ).FirstOrDefaultAsync();

                if(objectFieldUpdate.Description == null) objectFieldUpdate.Description = "";
                if(objectFieldUpdate.DataType == null) objectFieldUpdate.DataType = "[nvarchar](128)";

                objectFieldUpdate.Object = baseObject;
                objectFieldUpdate.ObjectId = baseObject.ObjectId;

                objectFieldUpdate.DateCreated = DateTime.UtcNow;
                objectFieldUpdate.CreatedBy = Guid.Empty;
                objectFieldUpdate.DateModified = DateTime.UtcNow;
                objectFieldUpdate.ModifiedBy = Guid.Empty;
                objectFieldUpdate.IsActive = false;

                await _context.ObjectFields.AddAsync(objectFieldUpdate);
            }

            await _context.SaveChangesAsync();

            return baseObjectField == null ? null : JObject.FromObject(baseObjectField);
        }


        // DELETE api/object/{moduleName}/{objectName}/{objectFieldName}
        [HttpDelete]
        [Route("{objectFieldName}")]
        public async Task Delete(string moduleName, string objectName, string objectFieldName)
        {
            ObjectField objectField = await _context.ObjectFields.Where(of => 
                of.Name == objectFieldName && of.Object.Name == objectName && of.Object.Module.Name == moduleName
            ).FirstOrDefaultAsync();

            this._context.ObjectFields.Remove(objectField);
            
            await _context.SaveChangesAsync();
        }
        
    }
}