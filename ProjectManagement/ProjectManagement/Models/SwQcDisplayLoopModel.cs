using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class FilesDetailForDisplayLoop
    {
        public string FilePath { get; set; }
        public string Extention { get; set; }
    }
    public class SwQcDisplayLoopModel
    {
        public SwQcDisplayLoopModel()
        {
            ScreenShotGetUrl1=new List<string>();
            FilesDetails = new List<FilesDetailForDisplayLoop>();
        }
        public string EmployeeCode { get; set; }
        public string UserFullName { get; set; }
        public long SwQcDisplayLoopId { get; set; }
        public long SwQcIssueId { get; set; }
        public string SwQcDescription { get; set; }
        public long ProjectMasterId { get; set; }
        public string ProjectType { get; set; }
        public long SwQcAssignId { get; set; }
        public bool IsIssueChecked { get; set; }
        public string Result { get; set; }
        public string IssueComment { get; set; }
        public string QcCategoryName { get; set; }
        public string IssueType { get; set; }
        public string Frequency { get; set; }
        public string IssueReproducePath { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}")]
        public DateTime? StartTime { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}")]
        public DateTime? EndTime { get; set; }
        public bool IsSmart { get; set; }
        public bool IsFeature { get; set; }
        public bool IsWalpad { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string UploadedFile { get; set; }
        public List<string> ScreenShotGetUrl1 { get; set; }
        public HttpPostedFileBase File1 { get; set; }
        public List<FilesDetailForDisplayLoop> FilesDetails { get; set; }
        public long SwQcInchargeAssignId { get; set; }
        public long SwQcUserId { get; set; }
    }
}