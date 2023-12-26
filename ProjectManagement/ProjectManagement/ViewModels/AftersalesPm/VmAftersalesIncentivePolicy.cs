using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.AftersalesPm
{
 
    public class VmAftersalesIncentivePolicy
    {
        public VmAftersalesIncentivePolicy()
        {
            CmnUserModel = new CmnUserModel();
            CmnUserModelList = new List<CmnUserModel>();
            ProjectMasterModel = new ProjectMasterModel();
            ProjectMasterModelList = new List<ProjectMasterModel>();
            PmIncentiveBaseModelsList=new List<Pm_Incentive_BaseModel>();
            PmIncentiveBaseModels=new Pm_Incentive_BaseModel();
            PmIncentiveModels=new Pm_IncentiveModel();
            PmIncentiveModelsList=new List<Pm_IncentiveModel>();
        }

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
        public string ProjectType { get; set; }
        public int? OrderNuber { get; set; }

        public CmnUserModel CmnUserModel { get; set; }
        public List<CmnUserModel> CmnUserModelList { get; set; }
        public ProjectMasterModel ProjectMasterModel { get; set; }
        public List<ProjectMasterModel> ProjectMasterModelList { get; set; }
        public List<Pm_Incentive_BaseModel> PmIncentiveBaseModelsList { get; set; }
        public Pm_Incentive_BaseModel PmIncentiveBaseModels { get; set; }
        public Pm_IncentiveModel PmIncentiveModels { get; set; }
        public List<Pm_IncentiveModel> PmIncentiveModelsList { get; set; } 
    }
}