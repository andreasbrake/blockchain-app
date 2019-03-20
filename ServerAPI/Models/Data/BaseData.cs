using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlockchainAppAPI.Models.Configuration
{
    public class BaseDataModel
    {
        [Key]
        public Guid ObjectId { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime DateModified { get; set; }
        public Guid ModifiedBy { get; set; }
        
        public string _DataBlob { get; set; }
        [NotMapped]
        public JObject DataBlob { 
            get { return _DataBlob == null ? null : JObject.Parse(_DataBlob); }
            set {  _DataBlob = JsonConvert.SerializeObject(value); } 
        }

        public JObject Data {
            get
            {
                JObject baseBlob = this.DataBlob;
                baseBlob["ObjectId"] = this.ObjectId;
                baseBlob["DateCreated"] = this.DateCreated;
                baseBlob["CreatedBy"] = this.CreatedBy;
                baseBlob["DateModified"] = this.DateModified;
                baseBlob["ModifiedBy"] = this.ModifiedBy;
                return baseBlob;
            }
        }
    }
}