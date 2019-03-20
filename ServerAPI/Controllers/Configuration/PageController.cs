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
    [Authorize]
    [Route("api/page/")]
    public class PageController : Controller
    {
        private readonly string _currentUser;
        private readonly SystemContext _context;

        public PageController(IHttpContextAccessor httpContextAccessor, SystemContext context)
        {
            _currentUser = httpContextAccessor.CurrentUser();
            _context = context;
        }
        
        // GET api/page/{moduleName}
        [HttpGet]
        [Route("{moduleName}")]
        public async Task<JArray> GetAllPagesInModule(string moduleName)
        {
            List<Page> model = await _context.Pages.Where(p => 
                p.Module.Name == moduleName
            ).ToListAsync();

            return JArray.FromObject(model);
        }

        // GET api/page/test
        [HttpGet]
        [Route("{moduleName}/{pageName}")]
        public async Task<JObject> Get(string moduleName, string pageName)
        {
            Page model = await _context.Pages
                .Include(o => o.Module)
                .Include(o => o.MainWidget)
                .Where(p => 
                    p.Module.Name == moduleName && p.Name == pageName
                ).FirstOrDefaultAsync();

            model.Module.Pages = null;
            model.Module.Objects = null;
            model.Module.Widgets = null;
            if(model.MainWidget != null) model.MainWidget.Module = null;

            return model == null ? null : JObject.FromObject(model);
        }

        // PUT api/page/test
        [HttpPut]
        [Route("{moduleName}/{pageName}")]
        public async Task<JObject> Put(string moduleName, string pageName, [FromBody]JObject value)
        {
            Page model = value.ToObject<Page>();

            await this._context.Pages.AddAsync(model);

            await _context.SaveChangesAsync();

            return model == null ? null : JObject.FromObject(model);
        }

        // // POST api/page/test
        [HttpPost]
        [Route("{moduleName}/{pageName}")]
        public async Task<JObject> Post(string moduleName, string pageName, [FromBody]JObject value)
        {
            Page pageUpdate = value.ToObject<Page>();
            
            Page basePage = await _context.Pages.Where(p => 
                p.Module.Name == moduleName && p.Name == pageName
            ).FirstOrDefaultAsync();

            if(basePage != null) {
                basePage.IsActive = pageUpdate.IsActive;
                basePage.DateModified = DateTime.UtcNow;
                basePage.Template = pageUpdate.Template;
                
                _context.Pages.Update(basePage);
            }
            else {
                Module baseModule = await _context.Modules.Where(m => 
                    m.Name == moduleName
                ).FirstOrDefaultAsync();

                if(pageUpdate.Template == null) pageUpdate.Template = "";

                pageUpdate.Module = baseModule;
                pageUpdate.ModuleId = baseModule.ModuleId;

                pageUpdate.DateCreated = DateTime.UtcNow;
                pageUpdate.CreatedBy = Guid.Empty;
                pageUpdate.DateModified = DateTime.UtcNow;
                pageUpdate.ModifiedBy = Guid.Empty;
                pageUpdate.IsActive = false;

                await _context.Pages.AddAsync(pageUpdate);
            }

            await _context.SaveChangesAsync();

            return basePage == null ? null : JObject.FromObject(basePage);
        }


        // DELETE api/page/test
        [HttpDelete]
        [Route("{moduleName}/{pageName}")]
        public async Task Delete(string moduleName, string pageName)
        {
            Page page = await _context.Pages.Where(p => 
                p.Module.Name == moduleName && p.Name == pageName
            ).FirstOrDefaultAsync();

            this._context.Pages.Remove(page);
            
            await _context.SaveChangesAsync();
        }
    }
}
