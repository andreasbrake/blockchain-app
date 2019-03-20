using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BlockchainAppAPI.DataAccess.Configuration;
using BlockchainAppAPI.Logic.Configuration;
using BlockchainAppAPI.Models.Configuration;

namespace BlockchainAppAPI.Controllers.Configuration
{
    //[Authorize]
    [Route("api/object/")]
    public class ObjectController : Controller
    {
        private readonly string _currentUser;
        private readonly SystemContext _context;

        public ObjectController(IHttpContextAccessor httpContextAccessor, SystemContext context)
        {
            _currentUser = httpContextAccessor.CurrentUser();
            _context = context;
        }
        
        // GET api/Object/{moduleName}
        [HttpGet]
        [Route("{moduleName}")]
        public async Task<JArray> GetAllObjectsInModule(string moduleName)
        {
            List<Models.Configuration.Object> model = await _context.Objects.Where(p => 
                p.Module.Name == moduleName
            ).ToListAsync();

            return JArray.FromObject(model);
        }

        // GET api/object/{moduleName}/{objectName}
        [HttpGet]
        [Route("{moduleName}/{objectName}")]
        public async Task<JObject> Get(string moduleName, string objectName)
        {
            Models.Configuration.Object model = await _context.Objects
                .Include(o => o.Module)
                .Include(o => o.ParentObject)
                .Include(o => o.MainObject)
                .Include(o => o.ObjectFields)
                .Where(p => 
                    p.Module.Name == moduleName && p.Name == objectName
                ).FirstOrDefaultAsync();
            
            model.Module.Objects = null;
            model.ObjectFields = model.ObjectFields.Select(f => {
                    f.Object = null;
                    return f;
                }).ToList();

            if(model.ParentObject != null) model.ParentObject.Module = null;
            if(model.MainObject != null) model.MainObject.Module = null;

            return model == null ? null : JObject.FromObject(model);
        }

        // PUT api/object/{moduleName}/{objectName}
        [HttpPut]
        [Route("{moduleName}/{objectName}")]
        public async Task<JObject> Put(string moduleName, string objectName, [FromBody]JObject value)
        {
            Models.Configuration.Object model = value.ToObject<Models.Configuration.Object>();

            await this._context.Objects.AddAsync(model);

            await _context.SaveChangesAsync();

            return model == null ? null : JObject.FromObject(model);
        }

        // // POST api/object/{moduleName}/{objectName}
        [HttpPost]
        [Route("{moduleName}/{objectName}")]
        public async Task<JObject> Post(string moduleName, string objectName, [FromBody]JObject value)
        {
            Models.Configuration.Object objectUpdate = value.ToObject<Models.Configuration.Object>();
            
            Models.Configuration.Object baseObject = await _context.Objects.Where(p => 
                p.Module.Name == moduleName && p.Name == objectName
            ).FirstOrDefaultAsync();

            baseObject.Description = objectUpdate.Description;
            baseObject.IsActive = objectUpdate.IsActive;
            baseObject.DateModified = DateTime.UtcNow;
            
            _context.Objects.Update(baseObject);

            await _context.SaveChangesAsync();

            return baseObject == null ? null : JObject.FromObject(baseObject);
        }


        // DELETE api/object/{moduleName}/{objectName}
        [HttpDelete]
        [Route("{moduleName}/{objectName}")]
        public async Task Delete(string moduleName, string objectName)
        {
            Models.Configuration.Object Object = await _context.Objects.Where(p => 
                p.Module.Name == moduleName && p.Name == objectName
            ).FirstOrDefaultAsync();

            this._context.Objects.Remove(Object);
            
            await _context.SaveChangesAsync();
        }
    }
}
