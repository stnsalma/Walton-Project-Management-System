using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class AftersalesPm_IssueVerificationModel
    {
        public AftersalesPm_IssueVerificationModel()
        {
            FilesDetails=new List<FilesDetail>();
            FilesDetails1=new List<FilesDetail>();
        }
        public List<FilesDetail> FilesDetails { get; set; }
        public List<FilesDetail> FilesDetails1 { get; set; }
        public List<HttpPostedFileBase> UploderDocs { get; set; }
        public string SupportingDocument { get; set; }
        public string DocumentUploadedByQc { get; set; }
        public long Id { get; set; }
        public string ModelName { get; set; }
        public string ProjectName { get; set; }
        public long? ProjectMasterId { get; set; }
        public long? ProjectPmAssignId { get; set; }
        public long? ProjectManagerUserId { get; set; }
        public string SoftwareVersionName { get; set; }
        public int? SoftwareVersionNo { get; set; }
        public string Module { get; set; }
        public string IssueDetails { get; set; }
        public string IssueFrequency { get; set; }
        public string IssueType { get; set; }
        public string TestingPath { get; set; }
        public string ResultFound { get; set; }
        public string ExpectedResult { get; set; }
        public int? NumberOfHSsChecked { get; set; }
        public string HSsIssueRatio { get; set; }
        public int? ComplainPercentage { get; set; }
        public int? NumberOfHSsReturn { get; set; }
        public string IssueSolvingInfo { get; set; }
        public int? NumberOfSample { get; set; }
        public string Status { get; set; }
        public bool? IsActive { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? IssueVerificationId { get; set; }
        public string LogStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public DateTime? SubmitDate { get; set; }
        public DateTime? ValidationDate { get; set; }
        public DateTime? ValidationFailDate { get; set; }
        public DateTime? ReportForwardDate { get; set; }
      //  public DateTime? IssueSolvedDate { get; set; }
        public DateTime? IssueSolvedDate { get; set; }
        public DateTime? IssueFailedDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public DateTime? AcceptationDate { get; set; }
    }
}