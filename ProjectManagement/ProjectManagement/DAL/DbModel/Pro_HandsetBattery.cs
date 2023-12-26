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
    
    public partial class Pro_HandsetBattery
    {
        public long HandsetBatteryId { get; set; }
        public long PlanId { get; set; }
        public Nullable<long> ProjectMasterID { get; set; }
        public string ProjectName { get; set; }
        public Nullable<int> OrderNumber { get; set; }
        public string PoCategory { get; set; }
        public Nullable<System.DateTime> MaterialReceive_SDate_Auto { get; set; }
        public Nullable<System.DateTime> MaterialReceive_EDate_Auto { get; set; }
        public Nullable<System.DateTime> MaterialReceive_SDate_Manual { get; set; }
        public Nullable<System.DateTime> MaterialReceive_EDate_Manual { get; set; }
        public Nullable<System.DateTime> Iqc_SDate_Auto { get; set; }
        public Nullable<System.DateTime> Iqc_EDate_Auto { get; set; }
        public Nullable<System.DateTime> Iqc_SDate_Manual { get; set; }
        public Nullable<System.DateTime> Iqc_EDate_Manual { get; set; }
        public Nullable<System.DateTime> Trial_SDate_Auto { get; set; }
        public Nullable<System.DateTime> Trial_EDate_Auto { get; set; }
        public Nullable<System.DateTime> Trial_SDate_Manual { get; set; }
        public Nullable<System.DateTime> Trial_EDate_Manual { get; set; }
        public Nullable<System.DateTime> ReliabilityTest_SDate_Auto { get; set; }
        public Nullable<System.DateTime> ReliabilityTest_EDate_Auto { get; set; }
        public Nullable<System.DateTime> ReliabilityTest_SDate_Manual { get; set; }
        public Nullable<System.DateTime> ReliabilityTest_EDate_Manual { get; set; }
        public Nullable<System.DateTime> MassProduction_SDate_Auto { get; set; }
        public Nullable<System.DateTime> MassProduction_EDate_Auto { get; set; }
        public Nullable<System.DateTime> MassProduction_SDate_Manual { get; set; }
        public Nullable<System.DateTime> MassProduction_EDate_Manual { get; set; }
        public Nullable<System.DateTime> AgingTest_SDate_Auto { get; set; }
        public Nullable<System.DateTime> AgingTest_EDate_Auto { get; set; }
        public Nullable<System.DateTime> AgingTest_SDate_Manual { get; set; }
        public Nullable<System.DateTime> AgingTest_EDate_Manual { get; set; }
        public Nullable<System.DateTime> Packing_SDate_Auto { get; set; }
        public Nullable<System.DateTime> Packing_EDate_Auto { get; set; }
        public Nullable<System.DateTime> Packing_SDate_Manual { get; set; }
        public Nullable<System.DateTime> Packing_EDate_Manual { get; set; }
        public Nullable<long> TotalOrderQuantity { get; set; }
        public string HandsetBatteryStatus { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string MaterialRules { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}
