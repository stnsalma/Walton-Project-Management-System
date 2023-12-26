using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.ProjectManager
{
    public class AssignedProjectListViewModel {
        public AssignedProjectListViewModel()
        {
           IndividualProjectViewModel = new IndividualProjectViewModel();
            ProjectPmAssignModel = new ProjectPmAssignModel();
            ProjectMasterModel = new ProjectMasterModel();
            VmSoftwareCustomization = new VmSoftwareCustomization();
            PmSwCustomizationInitialModels = new PmSwCustomizationInitialModel();
            PmViewHwTestHybridModel=new PmViewHwTestHybridModel();
            TestPhaseModels = new List<TestPhaseModel>();
            CmnUserModel = new CmnUserModel();

        }

        public CmnUserModel CmnUserModel { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public List<ProjectMasterModel> ProjectMasterModels { get; set; }
        public string ProjectName { get; set; }
        public long ProjectMasterId { get; set; }
        public IndividualProjectViewModel IndividualProjectViewModel { get; set; }
        public ProjectPmAssignModel ProjectPmAssignModel { get; set; }
        public List<SwQcInchargeAssignModel> SwQcInchargeAssignModels { get; set; }
        public ProjectMasterModel ProjectMasterModel { get; set; }
        public VmSoftwareCustomization VmSoftwareCustomization { get; set; }
        public List<HwQcInchargeAssignModel> HwQcInchargeAssignModels { get; set; }
        public PmViewHwTestHybridModel PmViewHwTestHybridModel { get; set; }
        public List<ProjectPurchaseOrderFormModel> ProjectPurchaseOrderFormModels { get; set; }
        public PmSwCustomizationInitialModel PmSwCustomizationInitialModels { get; set; }
        public List<TestPhaseModel> TestPhaseModels { get; set; }

        #region newPmAssign

        public List<SwQcTestPhaseModel> SwQcTestPhaseModels { get; set; }
        #endregion
    }
}