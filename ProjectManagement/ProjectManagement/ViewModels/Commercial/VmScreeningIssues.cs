
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Commercial
{
    public class VmScreeningIssues
    {
        public VmScreeningIssues()
        {
            HwInchargeIssueModels = new List<HwInchargeIssueModel>();
        }
        public long ProjectMasterId { get; set; }
        public List<HwInchargeIssueModel> HwInchargeIssueModels { get; set; }
    }
}