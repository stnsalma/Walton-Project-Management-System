using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    //public class FilesDetailForSwQcPersonalFinding
    //{
    //    public string FilePath { get; set; }
    //    public string Extention { get; set; }
    //}
    public class SwQcPersonalUseFindingsIssueDetailModel
    {
        public SwQcPersonalUseFindingsIssueDetailModel()
        {
            UploadedFileGetUrl1 = new List<string>();
           // FilesDetailForSwQcPersonal = new List<FilesDetailForSwQcPersonalFinding>();
            FilesDetails=new List<FilesDetail>();
        }
       
        //incentive
        public int Months { get; set; }
        public int Years { get; set; }
        public string Persons { get; set; }
        public int Critical { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public Decimal BaseAmount { get; set; }
        public Decimal IssueAmount { get; set; }
        public Decimal TotalAmount { get; set; }
        public Decimal ParticularPersonIncentive { get; set; }
        public string IncentiveClaim { get; set; }
        public long Deduction { get; set; }
        public string DeductionRemarks { get; set; }
        public int Percentage { get; set; }
        public long AddedAmount { get; set; }
        public string AddAmountRemarks { get; set; }

        //end incentive
        public long SwQcPrUseFindId { get; set; }
        public string FileOrIssue { get; set; }
        public long SwQcIssueId { get; set; }
        public long SwQcHeadAssignId { get; set; }
        public long SwQcAssignId { get; set; }
        public long ProjectPmAssignId { get; set; }
        public long ProjectMasterId { get; set; }
        public int? OrderNumber { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public string IssueScenario { get; set; }
        public string ExpectedOutcome { get; set; }
        public string IssueDetails { get; set; }
        public string RefernceModule { get; set; }
        public string Frequency { get; set; }
        public string IssueReproducePath { get; set; }
        public string Attachment { get; set; }
        public string IssueType { get; set; }
        public string Result { get; set; }
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
        public List<FilesDetail> FilesDetails { get; set; }
     //   public List<FilesDetailForSwQcPersonalFinding> FilesDetailForSwQcPersonal { get; set; }
        public string UserFullName { get; set; }
        public int IsRemoved { get; set; }
        public string TestPhaseName { get; set; }
        public bool? IsApprovedForIncentive { get; set; }
        public string IsApprovedForIncentives { get; set; }
    }
}