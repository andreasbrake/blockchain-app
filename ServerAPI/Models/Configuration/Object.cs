using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlockchainAppAPI.Models.Configuration
{
    [Table("Object", Schema="System")]
    public class Object: AuditBase
    {
        [Key]
        public Guid ObjectId { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        
        public Guid ModuleId { get; set; }
        public Module Module { get; set; }

        //[ForeignKey("ParentObject")]
        public Guid? ParentObjectId { get; set; }
        public Models.Configuration.Object ParentObject { get; set; }

        //[ForeignKey("MainObject")]
        public Guid? MainObjectId { get; set; }
        public Models.Configuration.Object MainObject { get; set; }
        
        public Boolean IsBaseObject { get; set; }
        public List<ObjectField> ObjectFields { get; set; }
    }
}