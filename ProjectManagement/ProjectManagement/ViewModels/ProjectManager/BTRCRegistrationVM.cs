using ProjectManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.DAL.DbModel;

namespace ProjectManagement.ViewModels.ProjectManager
{
    public class BTRCRegistrationVM
    {
        public BTRCRegistrationVM()
        {
            ProjectMaster = new ProjectMasterModel();
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long ProjectMasterId { get; set; }
        public ProjectMasterModel ProjectMaster { get; set; }
    }
}