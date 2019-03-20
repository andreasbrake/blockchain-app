using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlockchainAppAPI.Models.Configuration
{
    [Table("ObjectField", Schema="System")]
    public class ObjectField: AuditBase
    {
        [Key]
        public Guid ObjectFieldId { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        [ForeignKey("Object")]
        public Guid ObjectId { get; set; }
        public String DataType { get; set; }
        public Boolean IsBaseIndexed { get; set; }

        public Models.Configuration.Object Object { get; set; }
    }
}