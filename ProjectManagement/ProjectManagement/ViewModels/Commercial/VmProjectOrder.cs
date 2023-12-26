using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Commercial
{
    public class VmProjectOrder
    {
        public VmProjectOrder()
        {
            ProjectMasterModel = new ProjectMasterModel();
            ProjectOrderModel = new ProjectOrderModel();
        }

        public HttpPostedFileBase FileBase { get; set; }
        public ProjectMasterModel ProjectMasterModel { get; set; }
        public ProjectOrderModel ProjectOrderModel { get; set; }
    }
}