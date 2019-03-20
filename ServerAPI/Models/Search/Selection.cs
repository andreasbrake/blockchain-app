using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlockchainAppAPI.Models.Search
{
    [Table("Selection", Schema="Search")]
    public class Selection: AuditBase
    {
        [Key]
        public Guid SelectionId { get; set; }
        public String ObjectFieldName { get; set; }
        public String Function { get; set; }
        public String Aggregate { get; set; }

        
        public Guid SearchObjectId { get; set; }
        public SearchObject SearchObject { get; set; }
        
    }
}