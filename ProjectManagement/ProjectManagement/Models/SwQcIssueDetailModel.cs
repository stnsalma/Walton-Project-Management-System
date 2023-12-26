using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ProjectManagement.ViewModels.Software;

namespace ProjectManagement.Models
{
    public class SwQcIssueDetailModel
    {
        public SwQcIssueDetailModel()
        {
            UploadedFileGetUrl1 = new List<string>();
            FilesDetails = new List<FilesDetail>();
        }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? EffectiveMonth { get; set; }
        public int Year2 { get; set; }
        public int LastSoftwareVersionNo { get; set; }
        //incentive
        public string Status { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Month { get; set; }
        public int Months { get; set; }
        public int Years { get; set; }
        public string AssignPerson { get; set; }//
        public int AssignedPerson { get; set; }//
        public int AssignedPersons { get; set; }
        public int Critical { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int AllIssues { get; set; }
        public Decimal? BaseAmount { get; set; }
        public Decimal? IssueAmount { get; set; }
        public Decimal? TotalAmount { get; set; }
        public Decimal? TotalIssuePercentage { get; set; }
        public Decimal? PreviousDeductedAmount { get; set; }
        public Decimal? PerPersonPenalties { get; set; }
        public Decimal ParticularPersonIncentive { get; set; }
        public Decimal TotalPenalties { get; set; }
        public Decimal? FinalAmount { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string EmployeeCode { get; set; }
        public long Deduction { get; set; }
        public string DeductionRemarks { get; set; }
        public string PenaltiesPercentage { get; set; }
        public int Percentage { get; set; }
        public long AddedAmount { get; set; }
        public string AddAmountRemarks { get; set; }
        public string FileOrIssue { get; set; }
        public string OthersType { get; set; }
        public int? NewIssue { get; set; }
        public string SoftwareVersionNo1 { get; set; }
        public long SwQcIssueId { get; set; }
        public long SwQcHeadAssignId { get; set; }
        public long SwQcAssignId { get; set; }
        public long ProjectPmAssignId { get; set; }
        public long ProjectMasterId { get; set; }
        public string projectId { get; set; }
        public int? OrderNumber { get; set; }
        public string ProjectName { get; set; }
        public string ProjectNames { get; set; }
        public string ProjectType { get; set; }
        public string IssueScenario { get; set; }
        public string ExpectedOutcome { get; set; }
        public string IssueDetails { get; set; }
        public string RefernceModule { get; set; }
        public string Frequency { get; set; }
        public string IssueReproducePath { get; set; }
        public string Attachment { get; set; }
        public string IssueType { get; set; }
        public string Demo { get; set; }
        public string ExtendedRoleName { get; set; }
        public string Result { get; set; }
        public string SwQcHeadToPmSubmitDate { get; set; }
        public string IsFinalPhaseMPs { get; set; }
        public long? TestPhaseID { get; set; }
        public string SoftwareVersionName { get; set; }
        public int? SoftwareVersionNo { get; set; }
        public string FilesUrl { get; set; }
        public string FilesDetail { get; set; }
        public string Upload { get; set; }
        public bool? IsFile { get; set; }
        public bool? IsIssue { get; set; }
        public DateTime? WaltonQcComDate { get; set; }
        public string WaltonQcComment { get; set; }
        public string FixedVersion { get; set; }
        public DateTime? SupplierComDate { get; set; }
        public string SupplierComment { get; set; }
        public DateTime? WaltonPmComDate { get; set; }
        public string WaltonPmComment { get; set; }
        public bool? IsSmart { get; set; }
        public bool? IsFeature { get; set; }
        public bool? IsWalpad { get; set; }
        public bool? IsTab { get; set; }
        public bool? IsApprovedForChina { get; set; }
        public string IsApprovedForChinas { get; set; }
        public string FieldTestFrom { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //new
        public string RefernceModules1 { get; set; }
        public List<string> RefernceModules { get; set; }
        public string UploadedFile { get; set; }
        public List<string> UploadedFileGetUrl1 { get; set; }
        public List<HttpPostedFileBase> File { get; set; }
       
        public string UserFullName { get; set; }
        public int IsRemoved { get; set; }
        public string TestPhaseName { get; set; }
        public List<FilesDetail> FilesDetails { get; set; }
        public DateTime? SwQcHeadToQcAssignTime { get; set; }
        public DateTime? SwQcFinishedTime { get; set; }
        public DateTime? SwQcHeadToPmSubmitTime { get; set; }
        public bool? IsApprovedForIncentive { get; set; }
        public string IsApprovedForIncentives { get; set; }
        public DateTime? PmToQcHeadAssignTime { get; set; }
        public DateTime? PmToQcHeadAssignTime1 { get; set; }
        public DateTime? DeadLineFromIncharge { get; set; }
        public string RoleName { get; set; }
        public string WaltonQcStatus { get; set; }
        public string WaltonQcStatusSelect { get; set; }
        public string SupplierStatus { get; set; }
        public string SupplierStatusSelect { get; set; }
        public string WaltonPmStatus { get; set; }
        public string SupplierFeedbackForAppend { get; set; }
        public long IssueSerial { get; set; }
        public DateTime? QcHeadToPmSubmit { get; set; }
        public int? DateRangeCount { get; set; }
        public int? DateDiff { get; set; }
        public decimal? Timeline { get; set; }
        public int? AssignHours { get; set; }
        public int? AssignMinutes { get; set; }
        public bool? IsManagementApproved { get; set; }
        public DateTime? ManagementApproveDate { get; set; }
    }
}