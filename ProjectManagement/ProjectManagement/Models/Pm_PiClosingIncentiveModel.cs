using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class Pm_PiClosingIncentiveModel
    {
        public long Id { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int? OrderNumber { get; set; }
        public DateTime? PoDate { get; set; }
        public string PoCategory { get; set; }
        public string EmployeeCode { get; set; }
        public long? ProjectManagerUserId { get; set; }
        public decimal? Amount { get; set; }
        public string Remarks { get; set; }
        public decimal? DeductionAmount { get; set; }
        public string D_Remarks { get; set; }
        public decimal? FinalAmount { get; set; }
        public string Month { get; set; }
        public int? MonNum { get; set; }
        public long? Year { get; set; }
        public string DepartmentName { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Added { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? Updated { get; set; }
        public string ClosingType { get; set; }
        public DateTime? ClosingDate { get; set; }
    }
}