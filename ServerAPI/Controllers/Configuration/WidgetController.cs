using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using BlockchainAppAPI.DataAccess.Configuration;
using BlockchainAppAPI.Models.Configuration;

namespace BlockchainAppAPI.Controllers.Configuration
{
    //[Authorize]
    [Route("api/widget")]
    public class WidgetController : Controller
    {
        private readonly string _currentUser;
        private readonly SystemContext _context;

        public WidgetController(IHttpContextAccessor httpContextAccessor, SystemContext context)
        {
            _currentUser = httpContextAccessor.CurrentUser();
            _context = context;
        }
        
        // GET api/widget/{moduleName}
        [HttpGet]
        [Route("{moduleName}")]
        public async Task<JArray> GetAllWidgetsInModule(string moduleName)
        {
            List<Widget> model = await _context.Widgets
                .Include(w => w.Children)
                    .ThenInclude(t => t.ChildWidget)
                .Where(p => 
                    p.Module.Name == moduleName
                ).ToListAsync();

            
            model = model.Select(w => {
                JObject props = w.WidgetProperties;
                props["subWidgets"] = JArray.FromObject(w.Children
                    .OrderBy(c => c.Sequence)
                    .Select(c => c.ChildWidget.Name)
                );
                w.WidgetProperties = props;

                w.Children = null;
                w.Parents = null;
                return w;
            }).ToList();

            return JArray.FromObject(model);
        }

        // POST api/widget/{moduleName}/{widgetName}
        [HttpPost]
        [Route("{moduleName}")]
        public async Task<JObject> Post(string moduleName, [FromBody]JObject widgetList)
        {
            List<Widget> widgetUpdate = ((JArray)widgetList["widgets"]).Cast<JObject>()
                .Select(o => o.ToObject<Widget>())
                .Where(w => w.Name != null)
                .ToList();

            Module baseModule = await _context.Modules
                .Where(m => 
                    m.Name == moduleName
                ).FirstOrDefaultAsync();

            List<Widget> baseWidgetList = await _context.Widgets
                .Include(w => w.Children)
                    .ThenInclude(t => t.ChildWidget)
                .Where(p => 
                    p.Module.Name == moduleName
                ).ToListAsync();

            Dictionary<Guid, string> widgetNameLookup = baseWidgetList.Select(w => 
                    new KeyValuePair<Guid, string>(w.WidgetId, w.Name)
                ).ToDictionary(
                    kv => kv.Key, 
                    kv => kv.Value
                );

            Dictionary<string, Guid> baseWidgetGuidLookup = baseWidgetList.Select(w => 
                    new KeyValuePair<string, Guid>(w.Name, w.WidgetId)
                ).ToDictionary(
                    kv => kv.Key, 
                    kv => kv.Value
                );

            List<Widget> newWidgets = widgetUpdate.Where(w => w.WidgetId == default(Guid)).ToList();
            List<Widget> existingWidgets = widgetUpdate.Where(w => w.WidgetId != default(Guid)).ToList();

            Dictionary<string, Guid> transformer = new Dictionary<string, Guid>();
            Dictionary<string, List<Guid>> childList = new Dictionary<string, List<Guid>>();

            for(int i=0; i < newWidgets.Count; i++) {
                Guid newId = Guid.NewGuid();
                transformer.Add(newWidgets[i].Name, newId);
                newWidgets[i].WidgetId = newId;
                newWidgets[i].Name = "NoName-" + newId.ToString();
                widgetNameLookup.Add(newId, newWidgets[i].Name);
            }

            for(int i=0; i < baseWidgetList.Count; i++) {
                Widget updateWidget = existingWidgets.Where(uw => uw.WidgetId == baseWidgetList[i].WidgetId).FirstOrDefault();
                if(updateWidget != null) {
                    JArray subWidgets = (JArray)updateWidget.WidgetProperties["subWidgets"];

                    baseWidgetList[i].DateModified = DateTime.UtcNow;
                    baseWidgetList[i].ModifiedBy = Guid.Empty;
                    baseWidgetList[i].Children = subWidgets.Cast<JToken>().Select((nameToken, sequence) => {
                        string name = nameToken.ToString();
                        return new WidgetTree() {
                            WidgetTreeId = Guid.NewGuid(),
                            DateCreated = DateTime.UtcNow,
                            CreatedBy = Guid.Empty,
                            DateModified = DateTime.UtcNow,
                            ModifiedBy = Guid.Empty,
                            IsActive = false,
                            Sequence = sequence,
                            WidgetId = baseWidgetList[i].WidgetId,
                            ChildWidgetId = transformer.ContainsKey(name) 
                                ? transformer[name] 
                                : baseWidgetGuidLookup[name]
                        };
                    }).ToList();
                    
                    JObject updateProps = updateWidget.WidgetProperties;
                    updateProps["subWidgets"] = JArray.FromObject(baseWidgetList[i].Children
                        .OrderBy(c => 
                            c.Sequence
                        )
                        .Select(c =>
                            widgetNameLookup[c.ChildWidgetId]
                        )
                    );
                    baseWidgetList[i].WidgetProperties = updateProps;
                    
                    _context.Widgets.Update(baseWidgetList[i]);
                }
            }

            for(int i=0; i < newWidgets.Count; i++) {
                JArray subWidgets = (JArray)newWidgets[i].WidgetProperties["subWidgets"];

                if(subWidgets != null) {
                    newWidgets[i].Children = subWidgets.Cast<JToken>().Select((nameToken, sequence) => {
                        string name = nameToken.ToString();
                        return new WidgetTree() {
                            WidgetTreeId = Guid.NewGuid(),
                            DateCreated = DateTime.UtcNow,
                            CreatedBy = Guid.Empty,
                            DateModified = DateTime.UtcNow,
                            ModifiedBy = Guid.Empty,
                            IsActive = false,
                            Sequence = sequence,
                            WidgetId = newWidgets[i].WidgetId,
                            ChildWidgetId = transformer.ContainsKey(name) 
                                ? transformer[name] 
                                : baseWidgetGuidLookup[name]
                        };
                    }).ToList();
                }
                else {
                    newWidgets[i].Children = new List<WidgetTree>();
                }

                newWidgets[i].ModuleId = baseModule.ModuleId;
                newWidgets[i].Template = "";
                newWidgets[i].DateCreated = DateTime.UtcNow;
                newWidgets[i].CreatedBy = Guid.Empty;
                newWidgets[i].DateModified = DateTime.UtcNow;
                newWidgets[i].ModifiedBy = Guid.Empty;
                newWidgets[i].IsActive = false;
                
                JObject updateProps = newWidgets[i].WidgetProperties;
                updateProps["subWidgets"] = JArray.FromObject(newWidgets[i].Children
                    .OrderBy(c => 
                        c.Sequence
                    )
                    .Select(c => 
                        widgetNameLookup[c.ChildWidgetId]
                    )
                );
                newWidgets[i].WidgetProperties = updateProps;
                
                await _context.Widgets.AddAsync(newWidgets[i]);
            }
            
            await _context.SaveChangesAsync();

            // return baseWidget == null ? null : JObject.FromObject(baseWidget);
            return await Task.FromResult<JObject>(null);
        }

        // GET api/widget/{moduleName}/{widgetName}
        [HttpGet]
        [Route("{moduleName}/{widgetName}")]
        public async Task<JObject> Get(string moduleName, string widgetName)
        {
            Widget model = await _context.Widgets
                .Include(w => w.Children)
                    .ThenInclude(t => t.ChildWidget)
                .Where(p => 
                    p.Module.Name == moduleName && p.Name == widgetName
                ).FirstOrDefaultAsync();
            
            JObject props = model.WidgetProperties;
            props["subWidgets"] = JArray.FromObject(model.Children
                .OrderBy(c => c.Sequence)
                .Select(c => c.ChildWidget.Name)
            );
            model.WidgetProperties = props;

            model.Children = null;
            model.Parents = null;

            return model == null ? null : JObject.FromObject(model);
        }

        // PUT api/widget/{moduleName}/{widgetName}
        [HttpPut]
        [Route("{moduleName}/{widgetName}")]
        public async Task<JObject> Put(string moduleName, string widgetName, [FromBody]JObject value)
        {
            Widget model = value.ToObject<Widget>();

            await this._context.Widgets.AddAsync(model);

            await _context.SaveChangesAsync();

            return model == null ? null : JObject.FromObject(model);
        }

        // // POST api/widget/{moduleName}/{widgetName}
        [HttpPost]
        [Route("{moduleName}/{widgetName}")]
        public async Task<JObject> Post(string moduleName, string widgetName, [FromBody]JObject value)
        {
            Widget widgetUpdate = value.ToObject<Widget>();
            
            Widget baseWidget = await _context.Widgets.Where(p => 
                p.Module.Name == moduleName && p.Name == widgetName
            ).FirstOrDefaultAsync();

            baseWidget.IsActive = widgetUpdate.IsActive;
            baseWidget.DateModified = DateTime.UtcNow;
            baseWidget.Template = widgetUpdate.Template;
            
            _context.Widgets.Update(baseWidget);

            await _context.SaveChangesAsync();

            return baseWidget == null ? null : JObject.FromObject(baseWidget);
        }


        // DELETE api/widget/{moduleName}/{widgetName}
        [HttpDelete]
        [Route("{moduleName}/{widgetName}")]
        public async Task Delete(string moduleName, string widgetName)
        {
            Widget widget = await _context.Widgets.Where(p => 
                p.Module.Name == moduleName && p.Name == widgetName
            ).FirstOrDefaultAsync();

            this._context.Widgets.Remove(widget);
            
            await _context.SaveChangesAsync();
        }
    }
}
