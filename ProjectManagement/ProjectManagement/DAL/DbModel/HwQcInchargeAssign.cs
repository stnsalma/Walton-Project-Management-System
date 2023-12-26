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
    
    public partial class HwQcInchargeAssign
    {
        public HwQcInchargeAssign()
        {
            this.BatteryTestResultSummarys = new HashSet<BatteryTestResultSummary>();
            this.HwQcAssigns = new HashSet<HwQcAssign>();
        }
    
        public long HwQcInchargeAssignId { get; set; }
        public long ProjectMasterId { get; set; }
        public long HwQcInchargeUserId { get; set; }
        public Nullable<long> HwQcInchargeAssignedBy { get; set; }
        public Nullable<System.DateTime> HwQcInchargeAssignDate { get; set; }
        public Nullable<long> ReceivedSampleQuantity { get; set; }
        public string ReceiveSampleRemark { get; set; }
        public Nullable<long> SentSampleQuantity { get; set; }
        public string SentRemark { get; set; }
        public Nullable<long> ReturnedSampleQuantuty { get; set; }
        public Nullable<bool> IsScreeningTest { get; set; }
        public Nullable<bool> IsRunningTest { get; set; }
        public Nullable<bool> IsFinishedGoodTest { get; set; }
        public string TestPhase { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> SampleSetReceiveDate { get; set; }
        public Nullable<System.DateTime> SampleSetSentDate { get; set; }
        public Nullable<System.DateTime> ForwardDate { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<long> ProjectPmAssignId { get; set; }
        public Nullable<long> ProjectOrderShipmentId { get; set; }
        public Nullable<long> ProjectManagerUserId { get; set; }
        public string ProjectManagerAssignComment { get; set; }
        public string ProjectManagerSampleType { get; set; }
        public Nullable<int> ProjectManagerSampleNo { get; set; }
        public Nullable<System.DateTime> ApproxPmToHwDeliveryDate { get; set; }
        public string PriorityFromPm { get; set; }
        public string Status { get; set; }
    
        public virtual ICollection<BatteryTestResultSummary> BatteryTestResultSummarys { get; set; }
        public virtual ICollection<HwQcAssign> HwQcAssigns { get; set; }
        public virtual ProjectMaster ProjectMaster { get; set; }
    }
}
