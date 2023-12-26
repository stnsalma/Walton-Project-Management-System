using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.ManagementDashboard
{
    public class IssueGraphViewModel
    {
        public IssueGraphViewModel()
        {
            IssueGraphModels = new List<IssueGraphModel>();
            IssueGraphDrillDownModels = new List<IssueGraphDrillDownModel>();
        }
        public List<IssueGraphModel> IssueGraphModels { get; set; }
        public List<IssueGraphDrillDownModel> IssueGraphDrillDownModels { get; set; }
    }
}