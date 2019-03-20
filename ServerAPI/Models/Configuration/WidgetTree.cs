using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlockchainAppAPI.Models.Configuration
{
    [Table("WidgetTree", Schema="System")]
    public class WidgetTree: AuditBase
    {
        [Key]
        public Guid WidgetTreeId { get; set; }
        public int Sequence { get; set; }
        
        public Guid WidgetId { get; set; }
        public Widget Widget { get; set; }
        
        public Guid ChildWidgetId { get; set; }
        public Widget ChildWidget { get; set; }
    }
}