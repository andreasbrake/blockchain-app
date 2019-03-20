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
    [Route("api/module")]
    public class ModuleController : Controller
    {
        private readonly string _currentUser;
        private readonly SystemContext _context;


        public ModuleController(IHttpContextAccessor httpContextAccessor, SystemContext context)
        {
            _currentUser = httpContextAccessor.CurrentUser();
            _context = context;
        }

        // GET api/module/all
        [HttpGet]
        [Route("all")]
        public async Task<JArray> GetAll()
        {
            IEnumerable<Module> model = await this._context.Modules.ToListAsync();
            return JArray.FromObject(model);
        }

        // GET api/module/{moduleName}
        [HttpGet]
        [Route("{moduleName}")]
        public async Task<JObject> Get(string moduleName)
        {
            Module model = await _context.Modules.Where(m => 
                m.Name == moduleName
            ).FirstOrDefaultAsync();
            
            model.Objects = await _context.Objects.Where(o => 
                o.Module.Name == model.Name
            ).ToListAsync();
            model.Objects = model.Objects.Select(o => {
                o.Module = null;
                return o;
            }).ToList();

            model.Pages = await _context.Pages.Where(o => 
                o.Module.Name == model.Name
            ).ToListAsync();
            model.Pages = model.Pages.Select(o => {
                o.Module = null;
                return o;
            }).ToList();

            return model == null ? null : JObject.FromObject(model);
        }

        // PUT api/object/{moduleName}
        [HttpPut]
        [Route("{moduleName}")]
        public async Task<JObject> Put(string moduleName, [FromBody]JObject value)
        {
            Module model = value.ToObject<Module>();

            await this._context.Modules.AddAsync(model);

            await _context.SaveChangesAsync();

            return model == null ? null : JObject.FromObject(model);
        }

        // // POST api/object/{moduleName}
        [HttpPost]
        [Route("{moduleName}")]
        public async Task<JObject> Post(string moduleName, [FromBody]JObject value)
        {
            Module moduleUpdate = value.ToObject<Module>();
            
            Module baseModule = await _context.Modules.Where(m => 
                m.Name == moduleName
            ).FirstOrDefaultAsync();

            baseModule.IsActive = moduleUpdate.IsActive;
            baseModule.Description = moduleUpdate.Description;
            baseModule.DateModified = DateTime.UtcNow;
            
            _context.Modules.Update(baseModule);

            await _context.SaveChangesAsync();

            return baseModule == null ? null : JObject.FromObject(baseModule);
        }


        // DELETE api/object/{moduleName}
        [HttpDelete]
        [Route("{moduleName}")]
        public async Task Delete(string moduleName)
        {
            Module module = await _context.Modules.Where(m => 
                m.Name == moduleName 
            ).FirstOrDefaultAsync();

            this._context.Modules.Remove(module);
            
            await _context.SaveChangesAsync();
        }
        
    }
}