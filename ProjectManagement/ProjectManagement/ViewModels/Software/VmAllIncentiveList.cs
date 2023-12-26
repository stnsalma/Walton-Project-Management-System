using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Software
{
    public class VmAllIncentiveList
    {
        public VmAllIncentiveList()
        {
            SwQcAllIncentiveListModels=new List<SwQcAllIncentiveListModel>();
            SwQcAllIncentiveListModel=new SwQcAllIncentiveListModel();
            CmnUserModel = new CmnUserModel();
            CmnUserModels = new List<CmnUserModel>();
            SwQcIssueDetailModels1=new List<SwQcIssueDetailModel>();
            SwQcIssueDetailModel1=new SwQcIssueDetailModel();

            SwQcIssueDetailModels2 = new List<SwQcIssueDetailModel>();
            SwQcIssueDetailModel2 = new SwQcIssueDetailModel();

            SwQcNewInnovationModels=new List<SwQcNewInnovationModel>();
            SwQcNewInnovationModel=new SwQcNewInnovationModel();
            SwQcPersonalUseFindingsIssueDetailModels=new List<SwQcPersonalUseFindingsIssueDetailModel>();
            SwQcPersonalUseFindingsIssueDetailModel=new SwQcPersonalUseFindingsIssueDetailModel();

            SwQcIssueDetailModelsPenalties = new List<SwQcIssueDetailModel>();
            SwQcIssueDetailModelsPenalty = new SwQcIssueDetailModel();

            SwQcIssueDetailModels3 = new List<SwQcIssueDetailModel>();
            SwQcIssueDetailModel3 = new SwQcIssueDetailModel();
            SwQcOthersIncentiveModel=new SwQcOthersIncentiveModel();
            SwQcOthersIncentiveModels=new List<SwQcOthersIncentiveModel>();

            SwQcOthersIncentiveModel2 = new SwQcOthersIncentiveModel();
            SwQcOthersIncentiveModels2 = new List<SwQcOthersIncentiveModel>();

            SwQcOthersIncentiveModelBrandIssue = new SwQcOthersIncentiveModel();
            SwQcOthersIncentiveModelBrandIssues = new List<SwQcOthersIncentiveModel>();

            SwQcOthersIncentiveModelBrandCost = new SwQcOthersIncentiveModel();
            SwQcOthersIncentiveModelBrandCosts = new List<SwQcOthersIncentiveModel>();

            SwQcOthersIncentiveModelTotal = new SwQcOthersIncentiveModel();
            SwQcOthersIncentiveModelTotals = new List<SwQcOthersIncentiveModel>();

            ProjectMasterModels=new List<ProjectMasterModel>();
            ProjectMasterModel=new ProjectMasterModel();
            QcHeadIncentive = new IncentiveModel();
            QcHeadIncentiveList = new List<IncentiveModel>();
            ProjectMastersForPenalties = new List<ProjectMasterModel>();
            ProjectMastersForPenalty = new ProjectMasterModel();

            ProjectMastersForPenaltiesAll = new List<ProjectMasterModel>();
            ProjectMastersForPenaltyAll = new ProjectMasterModel();
            SwQcIssueDetailModelsOthers=new List<SwQcIssueDetailModel>();
            SwQcIssueDetailModelsOther=new SwQcIssueDetailModel();
            SwIncentive_PenaltiesForIssuesModels=new List<SwIncentive_PenaltiesForIssuesModel>();
            SwIncentive_PenaltiesForIssuesModel=new SwIncentive_PenaltiesForIssuesModel();
        }
        public List<ProjectMasterModel> ProjectMastersForPenalties { get; set; }
        public ProjectMasterModel ProjectMastersForPenalty { get; set; }
        public List<ProjectMasterModel> ProjectMastersForPenaltiesAll { get; set; }
        public ProjectMasterModel ProjectMastersForPenaltyAll { get; set; }
        public List<IncentiveModel> QcHeadIncentiveList { get; set; }
        public IncentiveModel QcHeadIncentive { get; set; }
        public List<SwQcIssueDetailModel> SwQcIssueDetailModels1 { get; set; }
        public SwQcIssueDetailModel SwQcIssueDetailModel1 { get; set; }
        public List<SwIncentive_PenaltiesForIssuesModel> SwIncentive_PenaltiesForIssuesModels { get; set; }
        public SwIncentive_PenaltiesForIssuesModel SwIncentive_PenaltiesForIssuesModel { get; set; }
        public List<SwQcIssueDetailModel> SwQcIssueDetailModels2 { get; set; }
        public SwQcIssueDetailModel SwQcIssueDetailModel2 { get; set; }
        public List<SwQcAllIncentiveListModel> SwQcAllIncentiveListModels { get; set; }
        public SwQcAllIncentiveListModel SwQcAllIncentiveListModel { get; set; }
        public List<SwQcNewInnovationModel> SwQcNewInnovationModels { get; set; }   
        public SwQcNewInnovationModel SwQcNewInnovationModel { get; set; }
        public List<SwQcPersonalUseFindingsIssueDetailModel> SwQcPersonalUseFindingsIssueDetailModels { get; set; }
        public SwQcPersonalUseFindingsIssueDetailModel SwQcPersonalUseFindingsIssueDetailModel { get; set; }
        public List<SwQcIssueDetailModel> SwQcIssueDetailModels3 { get; set; }
        public SwQcIssueDetailModel SwQcIssueDetailModel3 { get; set; }
        public List<SwQcIssueDetailModel> SwQcIssueDetailModelsPenalties { get; set; }
        public SwQcIssueDetailModel SwQcIssueDetailModelsPenalty { get; set; }
        public List<SwQcIssueDetailModel> SwQcIssueDetailModelsOthers { get; set; }
        public SwQcIssueDetailModel SwQcIssueDetailModelsOther { get; set; }
        public List<SwQcOthersIncentiveModel> SwQcOthersIncentiveModels { get; set; }
        public SwQcOthersIncentiveModel SwQcOthersIncentiveModel { get; set; }
        public List<SwQcOthersIncentiveModel> SwQcOthersIncentiveModels2 { get; set; }
        public SwQcOthersIncentiveModel SwQcOthersIncentiveModel2 { get; set; }
        public List<SwQcOthersIncentiveModel> SwQcOthersIncentiveModelBrandIssues { get; set; }
        public SwQcOthersIncentiveModel SwQcOthersIncentiveModelBrandIssue { get; set; }
        public List<SwQcOthersIncentiveModel> SwQcOthersIncentiveModelBrandCosts { get; set; }
        public SwQcOthersIncentiveModel SwQcOthersIncentiveModelBrandCost { get; set; }
        public List<SwQcOthersIncentiveModel> SwQcOthersIncentiveModelTotals { get; set; }
        public SwQcOthersIncentiveModel SwQcOthersIncentiveModelTotal { get; set; }
        public CmnUserModel CmnUserModel { get; set; }
        public List<CmnUserModel> CmnUserModels { get; set; }
        public List<ProjectMasterModel> ProjectMasterModels { get; set; }
        public ProjectMasterModel ProjectMasterModel { get; set; }
        //public long? ProjectMasterID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectModel { get; set; }
        public string SourcingType { get; set; }
        public string ProjectType { get; set; }
        public string EmployeeCode { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string RoleName { get; set; }
        public string PersonName { get; set; }
        public long ProjectMasterId { get; set; }
        public int MonNum { get; set; }
        public int TestPhaseId { get; set; }
        public string MonNum1 { get; set; }
        public string UserFullName { get; set; }
        public string Orders { get; set; }
        public DateTime? PoDate { get; set; }
        public DateTime? LSD { get; set; }
        public DateTime? ProjectManagerClearanceDate { get; set; }
        public DateTime? VesselDate { get; set; }
        public DateTime? WarehouseEntryDate { get; set; }
        public DateTime? ExtendedWarehouseDate { get; set; }
        public int? PoVsLSDDiff { get; set; }
        public int? DaysBeforeLsd { get; set; }
        public int? DaysAfterLsd { get; set; }
        public int? LsdVsVesselDiffForDeduct { get; set; }
        public int? LsdVsVesselDiffForReward { get; set; }
        public long? EffectiveDays { get; set; }
        public long? ExistedPercentage { get; set; }
        public long? RewardPercentage { get; set; }
        public long? OrderQuantity { get; set; }
        public long? TotalProductionQuantity { get; set; }
        public long? DeductPoint { get; set; }
        public long? DeductedAmount { get; set; }
        public long? RewardPoint { get; set; }
        public long? RewardAmount { get; set; }
        public decimal? Reward { get; set; }
        public decimal? RealPenalties { get; set; }
        public long? TotalSalesOut { get; set; }
        public long? TeamAmount { get; set; }
        public int? TeamMember { get; set; }
        public decimal? TeamReward { get; set; }
        public decimal? TeamPenalties { get; set; }
        public decimal? DeputyReward { get; set; }
        public decimal? DeputyPenalties { get; set; }
        public decimal? HeadReward { get; set; }
        public decimal? HeadPenalties { get; set; }
        public long? DeputyAmount { get; set; }
        public long? QcheadAmount { get; set; }
        public long? PenaltiesTeamAmount { get; set; }
        public long? PenaltiesDeputyAmount { get; set; }
        public long? PenaltiesQcheadAmount { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string PoCategory { get; set; }
      
    }
}