using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models
{
    public class HwIssueCommentModel
    {
        public long HwIssueCommentId { get; set; }
        [Required]
        public long HwQcAssignId { get; set; }
        [Required]
        public long ProjectMasterId { get; set; }
        [Required]
        public string IssueName { get; set; }
        [Required]
        public string IssueTypeName { get; set; }
        [Required]
        public string IssueTypeDetailName { get; set; }
        [Required]
        public string IssueComment { get; set; }
        [Required]
        public System.DateTime IssueCommetDate { get; set; }
        public string IssueStatus { get; set; }
        public string VerifierComment { get; set; }
        public long VerifiedBy { get; set; }

        public string HwQcInchargeComment { get; set; }
        public DateTime? HwQcInchargeCommentDate { get; set; }
        public string CommercialComment { get; set; }
        public DateTime? CommercialCommentDate { get; set; }
    }
}