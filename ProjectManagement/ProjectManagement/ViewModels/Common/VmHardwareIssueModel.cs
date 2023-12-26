using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Common
{
    public class VmHardwareIssueModel
    {
        public long ProjectMasterId { get; set; }
        public List<HardwareIssueCustomModel> HardwareIssueCustomModels { get; set; }

    }
}