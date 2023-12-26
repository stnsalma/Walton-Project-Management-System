using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.ManagementDashboard
{
    public class IssueGraphDrillDownModel
    {
        public IssueGraphDrillDownModel()
        {
            data = new List<Dictionary<string, long>>();
        }
        public string name { get; set; }
        public string id { get; set; }
        //public Dictionary<string,long> data { get; set; }
        public List<Dictionary<string,long>> data { get; set; }
    }
}