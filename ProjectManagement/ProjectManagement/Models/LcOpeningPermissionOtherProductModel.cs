using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.ViewModels.Common;

namespace ProjectManagement.Models
{
    public class LcOpeningPermissionOtherProductModel
    {
        public long Id { get; set; }
        public string ProductType { get; set; }
        public string Product { get; set; }
        public string CompanyName { get; set; }
        public Nullable<System.DateTime> OpeningDate { get; set; }
        public string SupplierName { get; set; }
        public string SupplierGrade { get; set; }
        public string Model { get; set; }
        public string OrderNo { get; set; }
        public Nullable<long> PreviousOrderQunatity { get; set; }
        public Nullable<long> StockQuantity { get; set; }
        public Nullable<long> PipeLineQuantity { get; set; }
        public Nullable<long> OrderQuantity { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<System.DateTime> ApproxDateOfShipment { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<long> AddedBy { get; set; }
        public string AddedByName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public Nullable<long> ApprovedBy { get; set; }
        public string ApprovedByName { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public string ApprovedByRemarks { get; set; }
        public Nullable<bool> IsRejected { get; set; }
        public Nullable<long> CheckedBy { get; set; }
        public Nullable<System.DateTime> CheckedDate { get; set; }
        public Nullable<long> VerifiedBy { get; set; }
        public Nullable<System.DateTime> VerifyDate { get; set; }
        public string Remarks { get; set; }
        public string TtiPerLine { get; set; }
        public string LcAmount { get; set; }
        public string Currency { get; set; }
        public string UnitPrice { get; set; }
        public string OraclePoNo { get; set; }
        public Nullable<System.DateTime> WarehouseReceiveDate { get; set; }
        public Nullable<System.DateTime> ShipmentConfirmDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<long> SourcingApprovalBy { get; set; }
        public string SourcingApprovalByName { get; set; }
        public Nullable<System.DateTime> SourcingApprovalDate { get; set; }
        public string SourcingRemarks { get; set; }
        public Nullable<long> CeoApprovalBy { get; set; }
        public string CeoApprovalByName { get; set; }
        public Nullable<System.DateTime> CeoApprovalDate { get; set; }
        public string CeoRemarks { get; set; }
        public Nullable<long> AccountsApprovalBy { get; set; }
        public string AccountsApprovalByName { get; set; }
        public Nullable<System.DateTime> AccountsApprovalDate { get; set; }
        public string AccountsRemarks { get; set; }
        public Nullable<long> FinanceApprovalBy { get; set; }
        public string FinanceApprovalByName { get; set; }
        public Nullable<System.DateTime> FinanceApprovalDate { get; set; }
        public string FinanceRemarks { get; set; }
        public string RelevantWaltonProjects { get; set; }
        public string ProductProfile { get; set; }
        public Nullable<long> AcknowledgedBy { get; set; }
        public string AcknowledgedByName { get; set; }
        public Nullable<System.DateTime> AcknowledgeDate { get; set; }
        public string AcknowledgeRemarks { get; set; }
        public string OtherProductLcForTheProject { get; set; }
        public Nullable<decimal> FreeOfCostClaimValue { get; set; }
        public string BdtLcValue { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<long> BiApprovalBy { get; set; }
        public string BiApprovalByName { get; set; }
        public Nullable<System.DateTime> BiApprovalDate { get; set; }
        public string BiRemarks { get; set; }
        public Nullable<System.DateTime> TtDate { get; set; }
        public string TtNumber { get; set; }
        public string TtValue { get; set; }
    }
}