using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.ProjectManager
{
    public class VmSoftwareCustomization
    {
        public VmSoftwareCustomization()
        {
            PmSwCustomizationFinalModels = new List<PmSwCustomizationFinalModel>();
            Others = new List<PmSwCustomizationFinalModel>();
            //ProjectPmAssignModel=new ProjectPmAssignModel();
           PmSwCustomizationInitialModels=new PmSwCustomizationInitialModel();
        }

        public long ProjectId { get; set; }
        public bool IsUpdateable { get; set; }
        public List<PmSwCustomizationFinalModel> PmSwCustomizationFinalModels { get; set; }
        public List<PmSwCustomizationFinalModel> Others { get; set; }
        //public ProjectPmAssignModel ProjectPmAssignModel { get; set; }
        public PmSwCustomizationInitialModel PmSwCustomizationInitialModels { get; set; }
    }
}