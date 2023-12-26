using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ProjectManagement.Models
{
    public class FilesDetailForPostProduction
    {
        public string FilePath { get; set; }
        public string Extention { get; set; }
    }
    public class PostProductionIssueModel
    {
        public PostProductionIssueModel()
        {
            UploadedFileGetUrl = new List<string>();
            ExtensionlList=new List<string>();
            FilesDetails=new List<FilesDetailForPostProduction>();
        }
        public long SwQcAllProjectIssueId { get; set; }
        public long SwQcPostProductAssignId { get; set; }
        public long? ProjectMasterId { get; set; }
        public long? SwQcUserId { get; set; }
        [Required]
        public string IssueName { get; set; }
        [Required]
        public string Comment { get; set; }
        [Required]
        public string IssueType { get; set; }
        [Required]
        public string Frequency { get; set; }
        public string IssueReproducePath { get; set; }
        public string ViewerIds { get; set; }
        public string Upload { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }

        [Required]
        public string ProjectName { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string PurchaseOrderOrdinals { get; set; }
        public bool? IsSolved { get; set; }
        public string SolutionRemarks { get; set; }
        public long? SolvedBy { get; set; }
        public long? ApprovedBy { get; set; }
        public string ApprovalRemarks { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? SolutionDate { get; set; }
        public long? IsIgnored { get; set; }
        public string IgnoreRemarks { get; set; }
        public long? IgnoredBy { get; set; }
        public DateTime? IgnoredDate { get; set; }
        public bool? IsAsItIs { get; set; }
        public string AsItIsRemarks { get; set; }
        public bool? IsCanceled { get; set; }
        public long? CanceledBy { get; set; }
        public DateTime? CanceledDate { get; set; }
        public string CurrentStatus { get; set; }
        public string FinalStatus { get; set; }

        //////ProjectMasters////
        public string OrderNumberOrdinal { get; set; }
        public int OrderNuber { get; set; }
        public string OrderNumber { get; set; }
        [Required]
        public int[] OrderNumbers { get; set; }


        //public string Upload { get; set; }
        public List<HttpPostedFileBase> File { get; set; }
        public string UploadedFile { get; set; }
        public List<string> UploadedFileGetUrl { get; set; }
        public List<string> ExtensionlList { get; set; }

        public List<FilesDetailForPostProduction> FilesDetails { get; set; }

        public string UserFullName { get; set; }
        public int IsRemoved { get; set; }

        public string AddedByName { get; set; }
        public string ProfilePictureUrl { get; set; }

    }
}