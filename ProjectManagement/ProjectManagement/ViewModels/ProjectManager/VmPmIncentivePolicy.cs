using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.ProjectManager
{
    public class VmPmIncentivePolicy
    {

        public VmPmIncentivePolicy() 
        {
            PmIncentiveBaseModels=new Pm_Incentive_BaseModel();
            PmIncentiveModels=new Pm_IncentiveModel();
            CmnUserModel = new CmnUserModel();
            ProjectMasterModel=new ProjectMasterModel();
            PmPoIncentiveModel=new Pm_Po_IncentiveModel();

            PmIncentiveBaseModelsList=new List<Pm_Incentive_BaseModel>();
            PmIncentiveModelsList=new List<Pm_IncentiveModel>();
            CmnUserModelsList=new List<CmnUserModel>();
            ProjectMasterModelsList=new List<ProjectMasterModel>();
            PmPoIncentiveModels=new List<Pm_Po_IncentiveModel>();
            PmAccessoriesModel=new Pm_AccessoriesModel();
            PmAccessoriesModels=new List<Pm_AccessoriesModel>();
            SwQcOthersIncentiveModelTotal=new SwQcOthersIncentiveModel();
        }
        public string ParameterName { get; set; }
        public Nullable<decimal> Parameter { get; set; }
        public Nullable<decimal> ParameterValue { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string RoleName { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<long> Updated { get; set; }

        public long CmnUserId { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }
        public string EmployeeCode { get; set; }
        public string AssignRoles { get; set; }
        public string ExtendedRoleName { get; set; }
        public decimal Amount { get; set; }
        public string IncentiveName { get; set; }

        ////ProjectMasterModel////

        public long ProjectMasterId { get; set; }
        public long ProjectTypeId { get; set; }
        public string ProjectName { get; set; }
        public string MultiProjectName { get; set; }
        public string MultiProjectIds { get; set; }
        public string ProjectType { get; set; }
        public int? OrderNuber { get; set; }

        //////ProjectPmAssignModel/////
        public long ProjectPmAssignId { get; set; }
      
        public string PONumber { get; set; }
        public DateTime AssignDate { get; set; }
        public long AssignUserId { get; set; }
        public long ProjectManagerUserId { get; set; }
        public string Status { get; set; }

        //////ModelList && Model////

        public List<Pm_Incentive_BaseModel> PmIncentiveBaseModelsList { get; set; }
        public List<Pm_IncentiveModel> PmIncentiveModelsList { get; set; }
        public List<CmnUserModel> CmnUserModelsList { get; set; }
        public List<ProjectMasterModel> ProjectMasterModelsList { get; set; }
       // public List<ProjectPmAssignModel> ProjectPmAssignModelsList { get; set; }
        public List<Pm_Po_IncentiveModel> PmPoIncentiveModels { get; set; }

        public CmnUserModel CmnUserModel { get; set; }
        public Pm_Incentive_BaseModel PmIncentiveBaseModels { get; set; }
        public Pm_IncentiveModel PmIncentiveModels { get; set; }
        public ProjectMasterModel ProjectMasterModel { get; set; }
       // public ProjectPmAssignModel ProjectPmAssignModel { get; set; }
        public Pm_Po_IncentiveModel PmPoIncentiveModel { get; set; }
        public List<Pm_AccessoriesModel> PmAccessoriesModels { get; set; }
        public Pm_AccessoriesModel PmAccessoriesModel { get; set; }
        public SwQcOthersIncentiveModel SwQcOthersIncentiveModelTotal { get; set; }

        #region new incentive policy 2020_08_09
        public DateTime? EffectiveMonth { get; set; }
        public string Remarks { get; set; }
        #endregion
    }
}