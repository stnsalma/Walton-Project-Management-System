using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class AllTrialInfoModel
    {
        public long Id { get; set; }
        public Nullable<long> PlanId { get; set; }
        public long? SmtTrialId { get; set; }
        public long? HousingTrialId { get; set; }
        public long? BatteryTrialId { get; set; }
        public long? AssemblyTrialId { get; set; }
        public DateTime? WorkingDate { get; set; }
        public long? TrialPerDayCapacity { get; set; }
        public long? TrialLineCapacity { get; set; }
        public long? TrialLineAvailableCapacity { get; set; }
        public long? TrialTotalQuantity { get; set; }
        public long? LineInformation_Id { get; set; }
        public string TrialLineNumber { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Added { get; set; }
    }
}