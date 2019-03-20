using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

using BlockchainAppAPI.Models.Configuration;

namespace BlockchainAppAPI.Controllers.Configuration
{
    [Authorize]
    [Route("api/NavMenu")]
    public class NavigationMenuController: Controller
    {
        // GET api/NavMenu
        [HttpGet]
        [Route("")]
        public async Task<JArray> GetNavigationMenu()
        {
            return await Task.FromResult<JArray>(
                JArray.FromObject(new [] {
                    new {
                        path = "/",
                        name = "Home",
                        icon = "home"
                    },
                    new {
                        path = "/view/patent/test page?application=F19F133B-FCA1-4348-84DF-471DC74E1981",
                        name = "Submittal Page",
                        icon = "edit"
                    },
                    new {
                        path = "/demo/reportWizard",
                        name = "Report Wizard",
                        icon = "chart-bar"
                    },
                    new {
                        path = "/chart-builder",
                        name = "Chart Builder",
                        icon = "chart-bar"
                    },
                    new {
                        path = "/demo/search",
                        name = "Search All",
                        icon = "search"
                    }
                })
            );
        }
    }
}