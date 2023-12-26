using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwQcIssueModel
    {
        public long SwQcIssueId { get; set; }
        public string QcCategoryName { get; set; }
        public string QcDescription { get; set; }
        public string ProjectType { get; set; }
        public bool IsSmart { get; set; }
        public bool IsFeature { get; set; }
        public bool IsWalpad { get; set; }
        public long SwQcAssignId { get; set; }
       
    }
}