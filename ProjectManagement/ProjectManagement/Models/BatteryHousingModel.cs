using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class BatteryHousingModel
    {
        public bool? IsActive { get; set; }
        public long Id { get; set; }
        public long PlanId { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int? OrderNumber { get; set; }
        public string PoCategory { get; set; }
        public DateTime? MaterialReceiveStartDateBHousing { get; set; }
        public DateTime? MaterialReceiveEndDateBHousing { get; set; }
        public DateTime? IqcCompleteStartDateBHousing { get; set; }
        public DateTime? IqcCompleteEndDateBHousing { get; set; }
        public DateTime? TrialProductionStartDateBHousing { get; set; }
        public DateTime? TrialProductionEndDateBHousing { get; set; }
        public DateTime? HousingReliabilityTestStartDateBHousing { get; set; }
        public DateTime? HousingReliabilityTestEndDateBHousing { get; set; }
        public DateTime? HousingMassProductionStartDateBHousing { get; set; }
        public long? TotalQuantity { get; set; }
        public DateTime? HousingMassProductionEndDateBHousing { get; set; }
        public string StatusBHousing { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}