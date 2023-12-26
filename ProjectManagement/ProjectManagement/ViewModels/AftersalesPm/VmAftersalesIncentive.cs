using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.AftersalesPm
{
    public class VmAftersalesIncentive
    {
        public long Id { get; set; }
        public string ProjectName { get; set; }
        public long? ProjectMasterId { get; set; }
        public int? OrderNumber { get; set; }
        public string ProjectType { get; set; }
        public string IssueType { get; set; }
        public string IssueDetails { get; set; }
        public DateTime? IssueRaisedDate { get; set; }
        public DateTime? SolutionDate { get; set; }
        public int? DaysPassed { get; set; }
        public decimal? Penalties { get; set; }
        public decimal? Reward { get; set; }
        public decimal? TotalAmount { get; set; }
        public string Remarks { get; set; }
        public string Month { get; set; }
        public int? MonNum { get; set; }
        public int? Year { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Added { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? Updated { get; set; }
        //
        public long? IssuesIncentiveId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmpName { get; set; }
        public int? Percentage { get; set; }
        public int? GeneralCount { get; set; }
        public decimal? PerPersonAmount { get; set; }
        public string IncentiveRemarks { get; set; }
        public string MonthAndYear { get; set; }
        //
        public decimal Incentive { get; set; }
        public decimal Incentive1 { get; set; }
        public string GeneralIncidentTitle { get; set; }
        public string GeneralIncidentCategories { get; set; }
        public string GeneralIncidentDetails { get; set; }
        public string Issues { get; set; }
        public string Status { get; set; }
        public long GeneralIncidentId { get; set; }
        public long? AssignedTo { get; set; }
        public DateTime? RaiseDate { get; set; }
        public string AssignedRemarks { get; set; }
        public string AssignedPerson { get; set; }
        public string Solution { get; set; }
        public string SolutionGivenBy { get; set; }

        public string MonYear { get; set; }
        public string UserFullName { get; set; }
    }
}