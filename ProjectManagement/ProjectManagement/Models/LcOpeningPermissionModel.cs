using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class LcOpeningPermissionModel
    {
        //public long ProjectLcId { get; set; }
        public long Id { get; set; }
        public long? ProjectMasterId { get; set; }
        public long? ProjectOrderId { get; set; }
        //public string LcNo { get; set; }
        public DateTime? OpeningDate { get; set; }
        public string StrOpeningDate { get; set; }
        public string CompanyName { get; set; }
        public string SupplierName { get; set; }
        public string SupplierGrade { get; set; }
        
        public string Model { get; set; }
        public string OrderNo { get; set; }

        public string Product { get; set; }
        public long? PreviousOrderQunatity { get; set; }
        public long? StockQuantity { get; set; }
        public long? PipelineQuantity { get; set; }
        public long? OrderQuantity { get; set; }
        public DateTime? ApproxDateOfShipment { get; set; }
        public string StrApproxDateOfShipment { get; set; }
        //public List<long?> LcAmount { get; set; }
        public decimal? TotalAmount { get; set; }

        public long? AddedBy { get; set; }
        public string AddedByName { get; set; }
        public DateTime? AddedDate { get; set; }
        public bool? IsActive { get; set; }

        public bool? IsApproved { get; set; }
        public string TtiPerLine { get; set; }
        public string LcAmount { get; set; }
        public string Currency { get; set; }
        public string UnitPrice { get; set; }
        public string OraclePoNo { get; set; }
        public DateTime? WarehouseReceiveDate { get; set; }
        public string StrWarehouseReceiveDate { get; set; }
        public DateTime? ShipmentConfirmDate { get; set; }
        public string StrShipmentConfirmDate { get; set; }
        public long? ApprovedBy { get; set; }
        public string ApprovedByName { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public long? CheckedBy { get; set; }
        public DateTime? CheckedDate { get; set; }
        public long? VerifiedBy { get; set; }
        public DateTime? VerifyDate { get; set; }
        public string Remarks { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public string ApprovedByRemarks { get; set; }
        public long? SourcingApprovalBy { get; set; }
        public string SourcingApprovalByName { get; set; }
        public DateTime? SourcingApprovalDate { get; set; }
        public string SourcingRemarks { get; set; }
        public long? CeoApprovalBy { get; set; }
        public string CeoApprovalByName { get; set; }
        public DateTime? CeoApprovalDate { get; set; }
        public string CeoRemarks { get; set; }
        public long? AccountsApprovalBy { get; set; }
        public string AccountsApprovalByName { get; set; }
        public DateTime? AccountsApprovalDate { get; set; }
        public string AccountsRemarks { get; set; }
        public long? FinanceApprovalBy { get; set; }
        public string FinanceApprovalByName { get; set; }
        public DateTime? FinanceApprovalDate { get; set; }
        public string FinanceRemarks { get; set; }
        public long? AcknowledgedBy { get; set; }
        public DateTime? AcknowledgeDate { get; set; }
        public string AcknowledgeRemarks { get; set; }
        public string AcknowledgedByName { get; set; }
        public bool? IsRejected { get; set; }
        public string BdtLcValue { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<long> BiApprovalBy { get; set; }
        public string BiApprovalByName { get; set; }
        public Nullable<System.DateTime> BiApprovalDate { get; set; }
        public string BiRemarks { get; set; }
    }
}