using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.StausObjects
{
    public class CmStatusObject
    {
        public long ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public int? OrderNuber { get; set; }
        public string ProjectStatus { get; set; }
        public string SourcingType { get; set; }
        public DateTime? ProjectInitialize { get; set; }
        public DateTime? InitialApprovalDate { get; set; }
        public DateTime? ScreeningSampleSent { get; set; }
        public DateTime? ScreeningIssueReview { get; set; }
        public DateTime? ScreeningIssueReviewDone { get; set; }
        public DateTime? ForwardForFinalApproval { get; set; }
        public DateTime? FinalApprovalDate { get; set; }
        public DateTime? PurchaseOrder { get; set; }
        public DateTime? ApproxProjectFinishDate { get; set; }
        public DateTime? ScreeningIssueReviewDate { get; set; }
        public DateTime? PoClosingDate { get; set; }
        public bool? IsCompleted { get; set; }
    }
}