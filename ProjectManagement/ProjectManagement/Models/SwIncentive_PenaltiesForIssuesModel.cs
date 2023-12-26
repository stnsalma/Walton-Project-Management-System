using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwIncentive_PenaltiesForIssuesModel
    {
        public long Id { get; set; }
        public long? ProjectMasterId { get; set; }
        public string ModelName { get; set; }
        public string Module { get; set; }
        public string IssueDetails { get; set; }
        public string IssueFrequency { get; set; }
        public string IssueType { get; set; }
        public string Status { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ClosingDate { get; set; }
        public DateTime? MonthlyEndDate { get; set; }
        public string EmployeeCode { get; set; }
        public int? AssignedPersons { get; set; }
        public decimal? TotalPenalties { get; set; }
        public decimal? ParticularPersonsPenalties { get; set; }
        public decimal? FinalAmount { get; set; }
        public string Month { get; set; }
        public int? MarketDateDiff { get; set; }
        public int? DateDiffs { get; set; }
        public int? MonNum { get; set; }
        public int? Year { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public Nullable<long> IssueVerificationId { get; set; }
        public Nullable<System.DateTime> AddingDate { get; set; }
        public Nullable<System.DateTime> MonthlyStartDate { get; set; }
    }
}