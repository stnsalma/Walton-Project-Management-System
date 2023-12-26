using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.ProjectManager
{
    public class VmNocReq
    {
        public long ProjectMasterId { get; set; }
        public List<ProjectBtrcNocModel> ProjectBtrcNocModels { get; set; }
    }
}