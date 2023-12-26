using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwQcAllIncentiveListModel
    {
        public long Id { get; set; }
        public string ClaimingField { get; set; }
        public string RegularClaimArea { get; set; }
        public string ProjectType { get; set; }
        public decimal? Timeline { get; set; }
        public decimal? BaseAmount { get; set; }
        public decimal? BaseAmount1 { get; set; }
        public decimal? Percentage { get; set; }
        public bool? IsActive { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}