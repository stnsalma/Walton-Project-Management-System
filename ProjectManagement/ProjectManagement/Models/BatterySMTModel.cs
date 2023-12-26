using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class BatterySMTModel
    {
        public bool? IsActive { get; set; }
        public long Id { get; set; }
        public long PlanId { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int? OrderNumber { get; set; }
        public string PoCategory { get; set; }
        public DateTime? MaterialReceiveStartDateBSmt { get; set; }
        public DateTime? MaterialReceiveEndDateBSmt { get; set; }
        public DateTime? IqcCompleteStartDateBSmt { get; set; }
        public DateTime? IqcCompleteEndDateBSmt { get; set; }
        public DateTime? TrialProductionStartDateBSmt { get; set; }
        public DateTime? TrialProductionEndDateBSmt { get; set; }
        public DateTime? SmtMassProductionStartDateBSmt { get; set; }
        public long? TotalQuantityBSmt { get; set; }
        public DateTime? SmtMassProductionEndDateBSmt { get; set; }
        public string StatusBSmt { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}