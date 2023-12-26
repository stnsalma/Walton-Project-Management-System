using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectClosePenaltyModel
    {
        public long ProjectClosePenaltyId { get; set; }
        public long? ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public DateTime? PoDate { get; set; }
        public int? PoCreatedBeforeMonth { get; set; }
        public int? DaysPassedAfterSevenMonth { get; set; }
        public int? Penalty { get; set; }
        public DateTime? IsCompletedDate { get; set; }
        public long? ProjectPurchaseOrderFormId { get; set; }
        public int? OrderNumber { get; set; }
    }
}