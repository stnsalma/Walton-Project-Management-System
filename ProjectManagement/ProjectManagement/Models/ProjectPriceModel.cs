using System;

namespace ProjectManagement.Models
{
    public class ProjectPriceModel
    {
        public long ProjectPriceId { get; set; }
        public long ProjectMasterId { get; set; }
        public decimal Price { get; set; }
        public System.DateTime PriceDate { get; set; }
        public string PriceStage { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        
        // Custom prop
        public bool IsFinalPrice { get; set; }
    }
}