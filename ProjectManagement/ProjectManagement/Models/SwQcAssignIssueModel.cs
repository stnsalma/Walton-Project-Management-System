using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwQcAssignIssueModel
    {
        public long SwQcAssignIssuesId { get; set; }
        public long SwQcIssueId { get; set; }
        public long ProjectMasterId { get; set; }
        public Nullable<long> SwQcAssignId { get; set; }
        public bool IsIssueChecked { get; set; }
        public string IssueComment { get; set; }
        public string QcCategoryName { get; set; }
        public string OccurenceRate { get; set; }
        public string ReproducePath { get; set; }
       
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
        public string ExpectedOutcome { get; set; }
        public string Priority { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        
    }
}