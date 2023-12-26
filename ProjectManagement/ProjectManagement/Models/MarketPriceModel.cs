using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class MarketPriceModel
    {
        public long? MarketPriceId { get; set; }
        public long ProjectMasterId { get; set; }
        public decimal? FinalPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? Multiplier { get; set; }
        public decimal? Mrp { get; set; }
        public bool? IsLocked { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        //Custom
        public string ProjectName { get; set; }
        public string JigsUnitPrice { get; set; }
        public string HandsetProcessCost { get; set; }
        public string ProjectModel { get; set; }
        public decimal? ProjectMasterPrice { get; set; }
    }
}