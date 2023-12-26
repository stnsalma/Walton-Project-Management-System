using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwInchargeIssueModel
    {
        public long HwInchargeIssuesId { get; set; }
        public long? ProjectMasterId { get; set; }
        public long? HwQcInchargeAssignId { get; set; }
        public string HwIssue { get; set; }
        public string HwIssueDetail { get; set; }
        public string CommercialDecision { get; set; }
        public string Remarks { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}