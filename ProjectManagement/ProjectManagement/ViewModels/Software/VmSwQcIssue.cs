using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Software
{
    public class VmSwQcIssue
    {
        public  VmSwQcIssue()
        {
            ProjectMasterModels = new List<ProjectMasterModel>();
            CmnUserModels = new List<CmnUserModel>();
        }

        //public long SwQcIssueId { get; set; }

        //public long ProjectMasterId { get; set; }
        //public string QcCategoryName { get; set; }
        //public string QcDescription { get; set; }
     
        //public bool IsIssueChecked { get; set; }
        //public string Comment { get; set; }
        //public HttpPostedFileBase ScreenShots1File { get; set; }

        //public string ScreenShot1FilePath { get; set; }
        //public HttpPostedFileBase ScreenShots2File { get; set; }

        //public string ScreenShot2FilePath { get; set; }
        //public HttpPostedFileBase ScreenShots3File { get; set; }
        //public string ScreenShots3FilePath { get; set; }
        //public HttpPostedFileBase VideoUpload1File { get; set; }
        //public string VideoUpload1FilePath { get; set; }
        //public HttpPostedFileBase VideoUpload2File { get; set; }
        //public string VideoUpload2FilePath { get; set; }
        //public HttpPostedFileBase VideoUpload3File { get; set; }
        //public string VideoUpload3FilePath { get; set; }

        //public List<string> IssueForList { get; set; }

        //public List<SwQcAssignIssueModel> SwQcAssignIssueModelInVm { get; set; }
        public List<ProjectMasterModel> ProjectMasterModels { get; set; }
        public List<CmnUserModel> CmnUserModels { get; set; }


    }
}