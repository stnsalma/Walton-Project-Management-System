using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PenaltiesForAllProjectModel
    {
        public long Id { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime? EffectiveMonDate { get; set; }
        public string ProjectName { get; set; }
        public string ProblemName { get; set; }
        public string ProblemSubCategory { get; set; }
        public decimal? SubCategoryQty { get; set; }
        public decimal? Activated { get; set; }
        public decimal? Percentage { get; set; }
        public decimal? TotalIssuePercentage { get; set; }
        public string Month { get; set; }
        public int? MonNum { get; set; }
        public int? Year { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
    }
}