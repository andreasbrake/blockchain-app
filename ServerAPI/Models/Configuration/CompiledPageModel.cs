using System;

namespace BlockchainAppAPI.Models.Configuration
{
    public class CompiledPageModel
    {
        public Guid CompiledPageId { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid PageId { get; set; }
        public String CompiledPage { get; set; }
    }
}