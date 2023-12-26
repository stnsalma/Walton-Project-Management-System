using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Common
{
    public class VmPostProductionIssue
    {
        public VmPostProductionIssue()
        {
            PostProductionIssueModel = new PostProductionIssueModel();
            PostProductionIssueModels = new List<PostProductionIssueModel>();
        }
        public PostProductionIssueModel PostProductionIssueModel { get; set; }
        public List<PostProductionIssueModel> PostProductionIssueModels { get; set; }
    }
}