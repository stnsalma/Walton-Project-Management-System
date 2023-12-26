using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.ManagementDashboard
{
    public class IssueGraphModel
    {
        public long id { get; set; }
        public string name { get; set; }
        public long y { get; set; }
        public string drilldown { get; set; }
    }
}