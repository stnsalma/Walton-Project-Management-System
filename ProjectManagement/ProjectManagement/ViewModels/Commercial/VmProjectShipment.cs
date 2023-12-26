using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Commercial
{
    public class VmProjectShipment
    {
        public VmProjectShipment()
        {
            ProjectMasterModel = new ProjectMasterModel();
            ProjectMasterModels=new List<ProjectMasterModel>();
            ProjectOrderShipmentModel = new ProjectOrderShipmentModel();
        }
        public ProjectMasterModel ProjectMasterModel { get; set; }
        public List<ProjectMasterModel> ProjectMasterModels { get; set; }
        public ProjectOrderShipmentModel ProjectOrderShipmentModel { get; set; }
    }
}