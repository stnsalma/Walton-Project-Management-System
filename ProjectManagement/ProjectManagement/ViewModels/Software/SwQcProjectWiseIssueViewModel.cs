using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ProjectManagement.ViewModels.Software
{
    public class FilesDetailForSwQcProjectWise
    {
        public string FilePath { get; set; }
        public string Extention { get; set; }
    }
    public class SwQcProjectWiseIssueViewModel
    {
        public SwQcProjectWiseIssueViewModel()
        {
            UploadedFileGetUrl1 = new List<string>();
            FilesDetails = new List<FilesDetailForSwQcProjectWise>();
        }
        public string EmployeeCode { get; set; }
        public long? SwQcProjectWiseIssueId { get; set; }
        public long? SwQcInchargeAssignId { get; set; }
        public long? ProjectMasterId { get; set; }
        public long? SwQcAssignId { get; set; }
        public string IssueName { get; set; }
        public string Result { get; set; }
        public string Comment { get; set; }
        public string RefernceModule { get; set; }
        public Nullable<long> RefBy { get; set; }
        public string IssueType { get; set; }
        public string Frequency { get; set; }
        public string IssueReproducePath { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}")]
        public DateTime? StartTime { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}")]
        public DateTime? EndTime { get; set; }
        public int IsRemoved { get; set; }
        public string PhoneType { get; set; }
        public Nullable<bool> IsSmart { get; set; }
        public Nullable<bool> IsFeature { get; set; }
        public Nullable<bool> IsWalpad { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UploadedFile { get; set; }
        public List<string> UploadedFileGetUrl1 { get; set; }
        public List<HttpPostedFileBase> File { get; set; }
        public List<FilesDetailForSwQcProjectWise> FilesDetails { get; set; }
        public string UserFullName { get; set; }
    }
}