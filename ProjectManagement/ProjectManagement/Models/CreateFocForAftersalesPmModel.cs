using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class CreateFocForAftersalesPmModel
    {
        public long Id { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string SpareName { get; set; }
        public int? OrderNumber { get; set; }
        public DateTime? PoDate { get; set; }
        public string PoCategory { get; set; }
        public string EmployeeCode { get; set; }
        public long? AsmUserId { get; set; }
        public string Supplier { get; set; }
        public string Remarks { get; set; }
        public DateTime? FocConfirmedDate { get; set; }
        public DateTime? InventoryEntryDate { get; set; }
        public decimal? UnitPrice { get; set; }
        public long? Quantity { get; set; }
        public long? ShipmentQuantity { get; set; }
        public string IncentiveRemarks { get; set; }
        public decimal? DeductionAmount { get; set; }
        public decimal? FinalAmount { get; set; }
        public string D_Remarks { get; set; }
        public string Month { get; set; }
        public int? MonNum { get; set; }
        public long? Year { get; set; }
        public string DepartmentName { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Added { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? Updated { get; set; }
        public decimal? FinalAmountTotal { get; set; }
        public decimal? FourtyShareIncentive { get; set; }
        public decimal? SixtyShareIncentive { get; set; }
        public decimal? TotalShareIncentive { get; set; }

        public decimal? SixtyShareIncentive1 { get; set; }
        public decimal? TotalShareIncentive1 { get; set; }
        public decimal? SixtyShareIncentive2 { get; set; }
        public decimal? TotalShareIncentive2 { get; set; }
        public string EmployeeCodes { get; set; }

        public string MonthNames { get; set; }
        public int? MonthNos { get; set; }
        public int? Years { get; set; }
    }
}