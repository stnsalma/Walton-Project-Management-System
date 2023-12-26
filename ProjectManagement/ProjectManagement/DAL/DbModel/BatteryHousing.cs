//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProjectManagement.DAL.DbModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class BatteryHousing
    {
        public long Id { get; set; }
        public long PlanId { get; set; }
        public Nullable<long> ProjectId { get; set; }
        public string ProjectName { get; set; }
        public Nullable<int> OrderNumber { get; set; }
        public string PoCategory { get; set; }
        public Nullable<System.DateTime> MaterialReceiveStartDateBHousing { get; set; }
        public Nullable<System.DateTime> MaterialReceiveEndDateBHousing { get; set; }
        public Nullable<System.DateTime> IqcCompleteStartDateBHousing { get; set; }
        public Nullable<System.DateTime> IqcCompleteEndDateBHousing { get; set; }
        public Nullable<System.DateTime> TrialProductionStartDateBHousing { get; set; }
        public Nullable<System.DateTime> TrialProductionEndDateBHousing { get; set; }
        public Nullable<System.DateTime> HousingReliabilityTestStartDateBHousing { get; set; }
        public Nullable<System.DateTime> HousingReliabilityTestEndDateBHousing { get; set; }
        public Nullable<System.DateTime> HousingMassProductionStartDateBHousing { get; set; }
        public Nullable<long> TotalQuantity { get; set; }
        public Nullable<System.DateTime> HousingMassProductionEndDateBHousing { get; set; }
        public string StatusBHousing { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}
