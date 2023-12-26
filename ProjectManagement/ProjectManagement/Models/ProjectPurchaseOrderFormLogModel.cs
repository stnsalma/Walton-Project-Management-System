using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectPurchaseOrderFormLogModel
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
        public long? Quantity { get; set; }
        public string Color { get; set; }
        public string Value { get; set; }
        public DateTime? PoDate { get; set; }
        public bool? IsCompleted { get; set; }
        public DateTime? IsCompletedDate { get; set; }
        public string PoCategory { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? IsSpareConfirmedDate { get; set; }
        public DateTime? IsSpareSubmittedDate { get; set; }
        public long? SpareSubmittedBy { get; set; }
        public string IsSpareSubmittedRemark { get; set; }
        public DateTime? PiDate { get; set; }
        public DateTime? ReminderMailFor18Month { get; set; }
        public string AfterSalesPmComment { get; set; }
        public string ProcessTeamComment { get; set; }
        public string QcComment { get; set; }
        public string FocStatus { get; set; }
        public bool? IsApprovedByCommercial { get; set; }
        public string InchargeComment { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? MarketClearanceDate { get; set; }
        public string BdIqcResult { get; set; }
        public string JigsTotalPrice { get; set; }
        public string JigsUnitPrice { get; set; }
        public string RepeatOrderApproved { get; set; }
        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? LogAddedDate { get; set; }
        public long? LogAddedBy { get; set; }
    }
}