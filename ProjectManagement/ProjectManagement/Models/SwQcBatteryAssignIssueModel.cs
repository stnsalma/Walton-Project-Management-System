using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwQcBatteryAssignIssueModel
    {
       
        public long SwQcBatteryAssignIssuesId { get; set; }
        public long SwQcAssignId { get; set; }
        public long ProjectMasterId { get; set; }
        public bool IsIssueChecked { get; set; }
        public string ModuleName { get; set; }
        public string CheckingOption { get; set; }
        public string Decreased { get; set; }
        public string Charging { get; set; }
        public string Time { get; set; }
        public string Voltage { get; set; }
        public string Issues { get; set; }
        public string IssueComment { get; set; }
        public HttpPostedFileBase ScreenShots1File { get; set; }

        public string ScreenShot1FilePath { get; set; }
        public HttpPostedFileBase ScreenShots2File { get; set; }

        public string ScreenShot2FilePath { get; set; }
        public HttpPostedFileBase ScreenShots3File { get; set; }
        public string ScreenShots3FilePath { get; set; }
        public HttpPostedFileBase VideoUpload1File { get; set; }
        public string VideoUpload1FilePath { get; set; }
        public HttpPostedFileBase VideoUpload2File { get; set; }
        public string VideoUpload2FilePath { get; set; }
        public HttpPostedFileBase VideoUpload3File { get; set; }
        public string VideoUpload3FilePath { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        public List<ProjectMasterModel> ProjectMasterModelsList { get; set; }

        public string ScreenShotGetUrl1 { get; set; }
        public string ScreenShotGetUrl2 { get; set; }
        public string ScreenShotGetUrl3 { get; set; }
        public string VideoUploadGetUrl1 { get; set; }
        public string VideoUploadGetUrl2 { get; set; }
        public string VideoUploadGetUrl3 { get; set; }
    }
}