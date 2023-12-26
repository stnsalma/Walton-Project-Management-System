using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class LcOpeningApprovalModel
    {
        public long Id { get; set; }
        public long? LcOpeningId { get; set; }
        public long? ApprovedBy { get; set; }
        public string ApprovedByName { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string ApprovalRemarks { get; set; }
        public long? VerifiedBy { get; set; }
        public string VerifiedByName { get; set; }
        public DateTime? VerifyDate { get; set; }
        public string VerificationRemarks { get; set; }
        public long? CheckedBy { get; set; }
        public string CheckedByName { get; set; }
        public DateTime? CheckDate { get; set; }
        public string CheckerRemarks { get; set; }
    }
}