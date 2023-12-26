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
    
    public partial class ProjectPurchaseOrderFormLog
    {
        public long Id { get; set; }
        public long ProjectPurchaseOrderFormId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string Receiver { get; set; }
        public long ProjectMasterId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string Subject { get; set; }
        public string DescriptionHeader { get; set; }
        public string DescriptionBody { get; set; }
        public byte[] Signature { get; set; }
        public Nullable<long> Quantity { get; set; }
        public string Color { get; set; }
        public string Value { get; set; }
        public Nullable<System.DateTime> PoDate { get; set; }
        public Nullable<bool> IsCompleted { get; set; }
        public Nullable<System.DateTime> IsCompletedDate { get; set; }
        public string PoCategory { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<System.DateTime> IsSpareConfirmedDate { get; set; }
        public Nullable<System.DateTime> IsSpareSubmittedDate { get; set; }
        public Nullable<long> SpareSubmittedBy { get; set; }
        public string IsSpareSubmittedRemark { get; set; }
        public Nullable<System.DateTime> PiDate { get; set; }
        public Nullable<System.DateTime> ReminderMailFor18Month { get; set; }
        public string AfterSalesPmComment { get; set; }
        public string ProcessTeamComment { get; set; }
        public string QcComment { get; set; }
        public string FocStatus { get; set; }
        public Nullable<bool> IsApprovedByCommercial { get; set; }
        public string InchargeComment { get; set; }
        public Nullable<System.DateTime> OrderDate { get; set; }
        public Nullable<System.DateTime> MarketClearanceDate { get; set; }
        public string BdIqcResult { get; set; }
        public string JigsTotalPrice { get; set; }
        public string JigsUnitPrice { get; set; }
        public string RepeatOrderApproved { get; set; }
        public Nullable<long> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public Nullable<System.DateTime> LogAddedDate { get; set; }
        public Nullable<long> LogAddedBy { get; set; }
    }
}
