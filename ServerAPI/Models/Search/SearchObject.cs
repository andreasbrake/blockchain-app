using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlockchainAppAPI.Models.Search
{
    [Table("SearchObject", Schema="Search")]
    public class SearchObject: AuditBase
    {
        [Key]
        public Guid SearchObjectId { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String CompiledQuery { get; set; }
        public String CompiledCountQuery { get; set; }
        public String _CompiledFieldList { get; set; }
        [NotMapped]
        public List<string> CompiledFieldList { 
            get 
            {
                return JArray.Parse(this._CompiledFieldList).Select(f => f.ToString()).ToList();
            } 
            set
            {
                this._CompiledFieldList = JsonConvert.SerializeObject(JArray.FromObject(value));
            }
        }
        public Boolean IsValid { get; set; }
        public String ModuleName { get; set; }
        public String ObjectName { get; set; }
        

        public List<Selection> Selections { get; set; }
    }
}