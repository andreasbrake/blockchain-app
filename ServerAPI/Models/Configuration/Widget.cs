using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlockchainAppAPI.Models.Configuration
{
    [Table("Widget", Schema="System")]
    public class Widget: AuditBase
    {
        [Key]
        public Guid WidgetId { get; set; }
        public String Name { get; set; }
        
        public String WidgetType { get; set; }

        public string _WidgetProperties { get; set; }
        [NotMapped]
        public JObject WidgetProperties { 
            get { return _WidgetProperties == null ? null : JObject.Parse(_WidgetProperties); }
            set {  _WidgetProperties = JsonConvert.SerializeObject(value); } 
        }

        public String Template { get; set; }

        public List<WidgetTree> Parents { get; set; }
        public List<WidgetTree> Children { get; set; }

        public Guid ModuleId { get; set; }
        public Module Module { get; set; }
    }
}