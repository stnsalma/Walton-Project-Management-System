using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ChargerHousingModel
    {
        public long Id { get; set; }
        public Nullable<long> ProjectId { get; set; }
        public string ProjectName { get; set; }
        public Nullable<int> OrderNumber { get; set; }
        public string PoCategory { get; set; }
        public Nullable<System.DateTime> MaterialReceiveStartDate { get; set; }
        public Nullable<System.DateTime> MaterialReceiveEndDate { get; set; }
        public Nullable<System.DateTime> IqcCompleteStartDate { get; set; }
        public Nullable<System.DateTime> IqcCompleteEndDate { get; set; }
        public Nullable<System.DateTime> TrialProductionStartDate { get; set; }
        public Nullable<System.DateTime> TrialProductionEndDate { get; set; }
        public Nullable<System.DateTime> HousingReliabilityTestStartDate { get; set; }
        public Nullable<System.DateTime> HousingReliabilityTestEndDate { get; set; }
        public Nullable<System.DateTime> HousingMassProductionStartDate { get; set; }
        public Nullable<System.DateTime> HousingMassProductionEndDate { get; set; }
        public long? TotalQuantity { get; set; }
        public string Status { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}