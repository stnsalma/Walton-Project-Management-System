using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.ProjectManager
{
    public class NewProjectsViewModel
    {

        public List<ProjectMasterModel> ProjectMasters { get; set; }
        public List<PmCmnUserModel> CmnUsers { get; set; }

        public ProjectPurchaseOrderForm ProjectPurchaseOrderForm { get; set; }


    }
}