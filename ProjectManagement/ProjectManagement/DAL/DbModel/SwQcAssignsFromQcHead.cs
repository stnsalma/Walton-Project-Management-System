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
    
    public partial class SwQcAssignsFromQcHead
    {
        public long SwQcAssignId { get; set; }
        public long SwQcHeadAssignId { get; set; }
        public long ProjectMasterId { get; set; }
        public long ProjectPmAssignId { get; set; }
        public long SwQcUserId { get; set; }
        public Nullable<int> OrderNumber { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public Nullable<System.DateTime> SwQcHeadToQcAssignTime { get; set; }
        public Nullable<System.DateTime> SwQcStartTime { get; set; }
        public Nullable<System.DateTime> SwQcEndTime { get; set; }
        public System.DateTime PmToQcHeadAssignTime { get; set; }
        public Nullable<System.DateTime> ApproxInchargeToQcDeliveryDate { get; set; }
        public string PriorityFromQcHead { get; set; }
        public string Status { get; set; }
        public string SoftwareVersionName { get; set; }
        public Nullable<int> SoftwareVersionNo { get; set; }
        public Nullable<int> QcReceivedSampleNo { get; set; }
        public string QcReceivedImeies { get; set; }
        public Nullable<System.DateTime> ReceiveDate { get; set; }
        public string FieldTestFrom { get; set; }
        public string FieldTestID { get; set; }
        public string AccessoriesTestType { get; set; }
        public Nullable<long> TestPhaseID { get; set; }
        public Nullable<bool> IsFinalPhaseMP { get; set; }
        public string SwQcHeadToQcAssignComment { get; set; }
        public Nullable<System.DateTime> SwQcHeadToPmSubmitTime { get; set; }
        public string SwQcHeadToPmForwardComment { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}