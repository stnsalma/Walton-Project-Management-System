using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class AftersalesPm_IncentiveModel
    {
        public long Id { get; set; }
        public long? AsmUserId { get; set; }
        public string EmployeeCode { get; set; }
        public long? Pm_Incentive_Base_Id { get; set; }
        public string OthersIncentiveName { get; set; }
        public string IncentiveTypes { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public decimal? Amount { get; set; }
        public decimal? DeductionAmount { get; set; }
        public decimal? FinalAmount { get; set; }
        public string D_Remarks { get; set; }
        public string Remarks { get; set; }
        public string Month { get; set; }
        public int? MonNum { get; set; }
        public long? Year { get; set; }
        public string DepartmentName { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Added { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? Updated { get; set; }
    }
}