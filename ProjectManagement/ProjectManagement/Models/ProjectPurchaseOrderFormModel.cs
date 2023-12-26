using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models
{
    public class ProjectPurchaseOrderFormModel
    {
        public long ProjectPurchaseOrderFormId { get; set; }
        public long ProjectOrderShipmentId { get; set; }
        [Required]
        public string PurchaseOrderNumber { get; set; }
        [Required]
        public string Receiver { get; set; }
        [Required]
        public long ProjectMasterId { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string CompanyAddress { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string DescriptionHeader { get; set; }
        [Required]
        public string DescriptionBody { get; set; }
        public byte[] Signature { get; set; }
        [Required]
        public long? Quantity { get; set; }
        [Required]
        public string Color { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
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
        public DateTime? PiDate { get; set; }
        public DateTime? ReminderMailFor18Month { get; set; }
        public string IsSpareSubmittedRemark { get; set; }
        public string AfterSalesPmComment { get; set; }
        public string ProcessTeamComment { get; set; }
        public string QcComment { get; set; }
        public string FocStatus { get; set; }
        public bool? IsApprovedByCommercial { get; set; }
        public string InchargeComment { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? OrderDate { get; set; }
        public string ProjectName { get; set; }
        public int? OrderNumber { get; set; }
        public DateTime? MarketClearanceDate { get; set; }
        public string JigsTotalPrice { get; set; }
        public string JigsUnitPrice { get; set; }
        public string RepeatOrderApproved { get; set; }
        public long? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public long? SpareSubmittedBy { get; set; }
        public DateTime? ApproxShipmentDate { get; set; }
    }
}