using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;
using ProjectManagement.Models.AssignModels;

namespace ProjectManagement.ViewModels.Software
{
    public class AssignForPostProductionMuliplePersonViewModel
    {
        public AssignForPostProductionMuliplePersonViewModel() 
        {
            ProjectMasterModelsList=new List<ProjectMasterModel>();
        }
        public string CombinedProjectId { get; set; }
        public string CombinedProjectId1 { get; set; }
        public string ProjectName { get; set; }
        public List<SwQcPostProductionAssignModel> swQcPostProductionAssignModels { get; set; }
        public List<SwQcPostProductionAssignModel> swQcPostProductionAssignModels1 { get; set; }
        public List<PostProductionIssueModel> AllProjectIssuesForSwQcModels { get; set; }
        public List<SoftwareCustomModelForDashboard> SoftwareCustomModelDashboards { get; set; }
        public List<ProjectMasterModel> ProjectMasterModelsList { get; set; }
        public List<ProjectMasterModel> DdlOrderNumber { get; set; }

        #region DropdownRemoteValidationForADropDownList
        [Required(ErrorMessage = "First name is required")]
        public String ddlAssignUserId { get; set; }
        public List<CmnUserModel> ddlAssignUsersList { get; set; }
        //public long TestPahseID { get; set; }
        public List<TestPhaseModel> ddlTestPhasesList { get; set; }
        public string OrderNumber { get; set; }
        public List<OrderNumberModel> DdlOrderNumberModels { get; set; } 
        #endregion
    }

    
}