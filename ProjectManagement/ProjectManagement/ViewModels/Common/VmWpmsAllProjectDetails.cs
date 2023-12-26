using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Common
{
    public class VmWpmsAllProjectDetails
    {
        public VmWpmsAllProjectDetails()
        {
            WpmsAllProjectDetailsModel1=new WpmsAllProjectDetailsModel();
            WpmsAllProjectDetailsModels1=new List<WpmsAllProjectDetailsModel>();
            WpmsAllProjectDetailsModel2 = new WpmsAllProjectDetailsModel();
            WpmsAllProjectDetailsModels2 = new List<WpmsAllProjectDetailsModel>();
            WpmsAllProjectDetailsModel3 = new WpmsAllProjectDetailsModel();
            WpmsAllProjectDetailsModels3 = new List<WpmsAllProjectDetailsModel>();
            WpmsAllProjectDetailsModel4 = new WpmsAllProjectDetailsModel();
            WpmsAllProjectDetailsModels4 = new List<WpmsAllProjectDetailsModel>();
            WpmsAllProjectDetailsModel5 = new WpmsAllProjectDetailsModel();
            WpmsAllProjectDetailsModels5 = new List<WpmsAllProjectDetailsModel>();
        }

        public List<WpmsAllProjectDetailsModel> WpmsAllProjectDetailsModels1 { get; set; }
        public WpmsAllProjectDetailsModel WpmsAllProjectDetailsModel1 { get; set; }
        public List<WpmsAllProjectDetailsModel> WpmsAllProjectDetailsModels2 { get; set; }
        public WpmsAllProjectDetailsModel WpmsAllProjectDetailsModel2 { get; set; }
        public List<WpmsAllProjectDetailsModel> WpmsAllProjectDetailsModels3 { get; set; }
        public WpmsAllProjectDetailsModel WpmsAllProjectDetailsModel3 { get; set; }
        public List<WpmsAllProjectDetailsModel> WpmsAllProjectDetailsModels4 { get; set; }
        public WpmsAllProjectDetailsModel WpmsAllProjectDetailsModel4 { get; set; }
        public List<WpmsAllProjectDetailsModel> WpmsAllProjectDetailsModels5 { get; set; }
        public WpmsAllProjectDetailsModel WpmsAllProjectDetailsModel5 { get; set; }
        public string ProjectName { get; set; }
        public long ProjectMasterId { get; set; }
        public string ProjectStatus { get; set; }
        public string Orders { get; set; }
        public string InitialApprovalPendings { get; set; }
        public string InitialApproval { get; set; }
        public string ProStatus { get; set; }

      
    }
}