using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlockchainAppAPI.Models.Configuration
{
    [Table("Module", Schema="System")]
    public class Module: AuditBase
    {
        [Key]
        public Guid ModuleId { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        
        public List<Models.Configuration.Object> Objects { get; set; }
        public List<Page> Pages { get; set; }
        public List<Widget> Widgets { get; set; }
    }
}