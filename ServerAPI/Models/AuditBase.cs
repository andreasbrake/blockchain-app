using System;

namespace BlockchainAppAPI.Models
{
    public class AuditBase
    {
        public DateTime DateCreated { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime DateModified { get; set; }
        public Guid ModifiedBy { get; set; }
        public Boolean IsActive { get; set; }
    }
}