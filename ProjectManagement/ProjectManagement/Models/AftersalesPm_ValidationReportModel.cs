using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class AftersalesPm_ValidationReportModel
    {
        public long Id { get; set; }
        public long? IssueVerificationId { get; set; }
        public string ModelName { get; set; }
        public string SoftwareVersionName { get; set; }
        public int? SoftwareVersionNo { get; set; }
        public string IssueDetails { get; set; }
        public string IssueOrRequirement { get; set; }
        public int? NumberOfMpHsCheck { get; set; }
        public int? DateDiff { get; set; }
        public int? AssignCount { get; set; }
        public string FoundInGoldenHs { get; set; }
        public string FoundInMpHs { get; set; }
        public string ValidationResult { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        //
        public string LogStatus { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }
         [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }
        public string SupportingDocument { get; set; }
        public List<HttpPostedFileBase> UpFiles { get; set; }
    }
}