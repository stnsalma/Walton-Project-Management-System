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
    
    public partial class IMEIReplacementMaster
    {
        public int RequestID { get; set; }
        public string IMEI_1 { get; set; }
        public string IMEI_2 { get; set; }
        public string Model { get; set; }
        public string DealerCode { get; set; }
        public string RequestType { get; set; }
        public Nullable<System.DateTime> RegistrationDate { get; set; }
        public Nullable<System.DateTime> DistributionDate { get; set; }
        public Nullable<System.DateTime> RequestDate { get; set; }
        public string Issues { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string OperatorName { get; set; }
        public string RequestStatus { get; set; }
        public string IsSeen { get; set; }
        public string IssueDetails { get; set; }
        public string ReplaceIMEI_1 { get; set; }
        public string ReplaceIMEI_2 { get; set; }
        public string ReplaceModel { get; set; }
        public string ActionTakenDetails { get; set; }
        public string ActionTakenBy { get; set; }
        public string AppliedSwVersion { get; set; }
        public string PartsRequired { get; set; }
        public string PlazaCode { get; set; }
        public Nullable<decimal> DuePrice { get; set; }
        public string InvoiceNo { get; set; }
        public string BatchNo { get; set; }
        public string Accessories { get; set; }
        public string WastageRemarks { get; set; }
        public Nullable<System.DateTime> ReworkDeliveredDate { get; set; }
        public Nullable<System.DateTime> ReworkReceiveDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<long> ServicePointId { get; set; }
        public string ServicePointName { get; set; }
    }
}