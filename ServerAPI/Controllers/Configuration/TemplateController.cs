using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using BlockchainAppAPI.DataAccess.Configuration;
using BlockchainAppAPI.Logic.Configuration;
using BlockchainAppAPI.Models.Configuration;

namespace BlockchainAppAPI.Controllers.Configuration
{
    [Authorize]
    [Route("api/template/{name}")]
    public class TemplateController : Controller
    {
        private readonly string _currentUser;


        public TemplateController(IHttpContextAccessor httpContextAccessor)
        {
            _currentUser = httpContextAccessor.CurrentUser();
        }

        // GET api/template/test
        // [HttpGet]
        // public async Task<JObject> Get(string name, Guid? objectId)
        // {
        //     CompiledPageModel compiledTemplate = (await _repository.GetTemplate(name));
        //     IEnumerable<string> fieldList = await TemplateRender.GetTemplateFields(compiledTemplate);
        //     IEnumerable<ObjectField> fields = await _systemRepository.ResolveFields(fieldList);



        //     JObject pageReponse = JObject.FromObject(new {
        //         Template = compiledTemplate.CompiledPage,
        //         Fields = fields
        //     });
            
        //     return pageReponse;
        // }

        
        // POST api/template/test
        [HttpPost]
        public async Task<string> Post(string name)
        {
            return await Task.FromResult<string>(null);
            //return await _repository.CompileTemplate(name);
        }
    }
}