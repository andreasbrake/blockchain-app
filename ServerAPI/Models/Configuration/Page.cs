using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlockchainAppAPI.Models.Configuration
{
    [Table("Page", Schema="System")]
    public class Page: AuditBase
    {
        [Key]
        public Guid PageId { get; set; }
        public String Name { get; set; }
        public String Template { get; set; }

        public Guid ModuleId { get; set; }
        public Module Module { get; set; }
        
        public Guid? MainWidgetId { get; set; }
        public Widget MainWidget { get; set; }
    }
}