using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;
using ProjectManagement.Models.AssignModels;

namespace ProjectManagement.ViewModels.Software
{
    public class AssignMuliplePersonViewModel
    {
        public AssignMuliplePersonViewModel()
        {

            ProjectMasterModel = new ProjectMasterModel();
            ProjectMasterModelsList = new List<ProjectMasterModel>();
            PmQcAssignModels=new List<PmQcAssignModel>();
            PmQcAssignModel=new PmQcAssignModel();
            SwQcAssignsFromQcHeadModel = new SwQcAssignsFromQcHeadModel();
            SwQcAssignsFromQcHeadModels = new List<SwQcAssignsFromQcHeadModel>();

            PmQcAssignModels1 = new List<PmQcAssignModel>();
            PmQcAssignModel1= new PmQcAssignModel();
        }
        public string ProjectName { get; set; }
        public string CombinedProjectId { get; set; }
        public string CombinedProjectIds { get; set; }
        public ProjectMasterModel ProjectMasterModel { get; set; }
        public List<ProjectMasterModel> ProjectMasterModelsList { get; set; }
        public List<PmQcAssignModel> PmQcAssignModels { get; set; }
        public PmQcAssignModel PmQcAssignModel { get; set; }
        public List<PmQcAssignModel> PmQcAssignModels1 { get; set; }
        public PmQcAssignModel PmQcAssignModel1 { get; set; }
        public List<SoftwareCustomModelForDashboard> SoftwareCustomModelDashboards { get; set; }
        public SwQcAssignsFromQcHeadModel SwQcAssignsFromQcHeadModel { get; set; }
        public List<SwQcAssignsFromQcHeadModel> SwQcAssignsFromQcHeadModels { get; set; }

        #region DropdownRemoteValidationForADropDownList
        [Required(ErrorMessage = "First name is required")]
        public String ddlAssignUserId { get; set; }
        public List<CmnUserModel> ddlAssignUsersList { get; set; }
        //public long TestPahseID { get; set; }
        public List<TestPhaseModel> ddlTestPhasesList { get; set; }
        #endregion
    }

    
}