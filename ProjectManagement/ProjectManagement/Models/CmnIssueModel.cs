using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class CmnIssueModel
    {
        public long CmnIssueId { get; set; }
        public long ProjectMasterId { get; set; }
        public string HwIssueDescription { get; set; }
        public string HwIssuePriority { get; set; }
        public long? HwRaisedBy { get; set; }
        public DateTime? HwRaisedDate { get; set; }
        public long? HwRefferedBy { get; set; }
        public string HwRemarks { get; set; }
        public string HwStatus { get; set; }
        public string SwDescription { get; set; }
        public string SwIssuePriority { get; set; }
        public long? SwRaisedBy { get; set; }
        public DateTime? SwRaisedDate { get; set; }
        public long? SwRefferedBy { get; set; }
        public string SwRemarks { get; set; }
        public string SwStatus { get; set; }
        public string PmIssueDescription { get; set; }
        public string PmIssuePriority { get; set; }
        public long? PmRaisedBy { get; set; }
        public DateTime? PmRaisedDate { get; set; }
        public long? PmRefferedBy { get; set; }
        public string PmRemarks { get; set; }
        public string PmStatus { get; set; }
        public string CmIssueDescription { get; set; }
        public string CmIssuePriority { get; set; }
        public long? CmRaisedBy { get; set; }
        public DateTime? CmRaisedDate { get; set; }
        public long? CmRefferedBy { get; set; }
        public string CmRemarks { get; set; }
        public string CmStatus { get; set; }
        public string CurrentSate { get; set; }
        public bool IsSolved { get; set; }
        public long? SolvedBy { get; set; }
        public DateTime? SolvedDate { get; set; }
    }
}