using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HardwareIssueCustomModel
    {
        public long ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public long HwQcUserid { get; set; }
        public string IssueRaiseName { get; set; }
        public long VerifyBy { get; set; }
        public string VerifiedName { get; set; }
        public string IssueName { get; set; }
        public string IssueTypeName { get; set; }
        public string IssueTypeDetailName { get; set; }
        public string IssueComment { get; set; }
        public string VerifierComment { get; set; }
        public string CommercialComment { get; set; }
        public long HwIssueCommentId { get; set; }
    }
}