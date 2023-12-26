using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.Common
{
    public class ProjectMasterInv
    {
        public string ProjectModel { get; set; }
        public int Order_No { get; set; }
        public long OrderQuantity { get; set; }
    }
}