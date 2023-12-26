using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class BatteryModel
    {
        public bool? IsActive { get; set; }
        public long Id { get; set; }
        public long PlanId { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int? OrderNumber { get; set; }
        public string PoCategory { get; set; }
        public DateTime? MaterialReceiveStartDateBattery { get; set; }
        public DateTime? MaterialReceiveEndDateBattery { get; set; }
        public DateTime? IqcCompleteStartDateBattery { get; set; }
        public DateTime? IqcCompleteEndDateBattery { get; set; }
        public DateTime? TrialProductionStartDateBattery { get; set; }
        public DateTime? TrialProductionEndDateBattery { get; set; }
        public DateTime? BatteryReliabilityTestStartDate { get; set; }
        public DateTime? BatteryReliabilityTestEndDate { get; set; }
        public DateTime? BatteryMassProductionStartDate { get; set; }
        public long? TotalQuantityBattery { get; set; }
        public DateTime? BatteryMassProductionEndDate { get; set; }
        public DateTime? BatteryAgingTestStartDate { get; set; }
        public DateTime? BatteryAgingTestEndDate { get; set; }
        public string StatusBattery { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}