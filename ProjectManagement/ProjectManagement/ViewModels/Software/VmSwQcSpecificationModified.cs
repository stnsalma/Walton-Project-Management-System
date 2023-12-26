using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Software
{
    public class VmSwQcSpecificationModified
    {
        public VmSwQcSpecificationModified()
        {
            ProjectMasterModel = new ProjectMasterModel();
            ProjectMasterModelsList = new List<ProjectMasterModel>();
            SwQcStartUpModels = new List<SwQcStartUpModel>();
            SwQcCallSettingModels = new List<SwQcCallSettingModel>();
            SwQcMessageModels= new List<SwQcMessageModel>();
            SwQcToolsCheckModels = new List<SwQcToolsCheckModel>();
            SwQcCameraModels= new List<SwQcCameraModel>();
            SwQcDisplayLoopModels=new List<SwQcDisplayLoopModel>();
            SwQcDisplayModels=new List<SwQcDisplayModel>();
            SwQcSettingModels= new List<SwQcSettingModel>();
            SwQcMultimediaModels = new List<SwQcMultimediaModel>();
            SwQcGoogleServiceModels = new List<SwQcGoogleServiceModel>();
            SwQcStorageCheckModels = new List<SwQcStorageCheckModel>();
            SwQcGameModels = new List<SwQcGameModel>();
            SwQcTestingAppModels = new List<SwQcTestingAppModel>();
            SwQcFileManagerModels = new List<SwQcFileManagerModel>();
            SwQcConnectivityModels = new List<SwQcConnectivityModel>();
            SwQcShutDownModels = new List<SwQcShutDownModel>();
            SwQcAssignModels = new List<SwQcAssignModel>();
            CmnUsers= new List<CmnUser>();
            SwQcInchargeAssignModels = new List<SwQcInchargeAssignModel>();
            CmnUserModels = new List<CmnUserModel>();
            SwQcTestCounterModel = new SwQcTestCounterModel();
            SwFieldTestReportViews= new List<SwFieldTestReportView>();
            SwQcTabColorModels= new List<SwQcTabColorModel>();
            SwQcProjectWiseIssueViewModels= new List<SwQcProjectWiseIssueViewModel>();

            SwQcStartUpModelss=new SwQcStartUpModel();
            HwQcInchargeAssignModels = new List<HwQcInchargeAssignModel>();
            HwQcAssignModels = new List<HwQcAssignModel>();
            TestPhaseModels = new List<TestPhaseModel>();
            SwQcInactiveOrAssignLogModels = new List<SwQcInactiveOrAssignLogModel>();
            SwQcPausedOrRestartActivityLogModels = new List<SwQcPausedOrRestartActivityLogModel>();
            SwList=new List<SoftwareCustomModelForDashboard>();
            AllProjectIssuesForSwQcModels=new List<PostProductionIssueModel>();

            SwQcHeadAssignsFromPmModel=new SwQcHeadAssignsFromPmModel();
            SwQcHeadAssignsFromPmModels=new List<SwQcHeadAssignsFromPmModel>();
            SwQcAssignsFromQcHeadModel=new SwQcAssignsFromQcHeadModel();
            SwQcAssignsFromQcHeadModels = new List<SwQcAssignsFromQcHeadModel>();
            ProjectDetailsForSwQcModels=new List<ProjectDetailsForSwQcModel>();
            ProjectDetailsForSwQcModel=new ProjectDetailsForSwQcModel();
            SwQcIssueDetailModel = new SwQcIssueDetailModel();
            SwQcIssueDetailModels=new List<SwQcIssueDetailModel>();
            SwQcIssueCategoryModels=new List<SwQcIssueCategoryModel>();
            SwQcIssueCategoryModel=new SwQcIssueCategoryModel();
            SwQcPersonalUseFindingsIssueDetail=new SwQcPersonalUseFindingsIssueDetail();
            SwQcPersonalUseFindingsIssueDetailModels=new List<SwQcPersonalUseFindingsIssueDetailModel>();
            SwQcNewInnovationModel=new SwQcNewInnovationModel();
            SwQcNewInnovationModels=new List<SwQcNewInnovationModel>();
            //ddlReferenceListForModal = new List<SwQcTestPhaseModel>();
            SwQcTestPhaseModel=new SwQcTestPhaseModel();
            SwQcTestPhaseModels=new List<SwQcTestPhaseModel>();

            SwQcIssueStaus = new SwQcIssueDetailModel();
            SwQcIssueStauses = new List<SwQcIssueDetailModel>();

            SwQcGlassProtectorTests = new List<SwQcGlassProtectorTestModel>();
            SwQcGlassProtectorTest = new SwQcGlassProtectorTestModel();
        }

        public List<SwQcGlassProtectorTestModel> SwQcGlassProtectorTests { get; set; }
        public SwQcGlassProtectorTestModel SwQcGlassProtectorTest { get; set; }
        public HttpPostedFileBase FileId { get; set; }
        public string ProjectType { get; set; }
        public string ProjectsDetails { get; set; }
        public string AllOrLatest { get; set; }
        public string AccessoriesCategories { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CurrDate { get; set; }
        public List<SwQcIssueDetailModel> SwQcIssueStauses { get; set; }
        public SwQcIssueDetailModel SwQcIssueStaus { get; set; }
        public string CombinedProjectId2 { get; set; }//WaltonQcStatus
        public string CombinedTestPhaseIds { get; set; }
        public string WaltonQcStatus { get; set; }
        public string CombinedProjectId { get; set; }
        public string CombinedProjectIds { get; set; }
        public long SoftwareVersionNumber { get; set; }
        public int OrderNumber { get; set; }
        public string Tabname { get; set; }
        public string projectType { get; set; }
        public string projectId { get; set; }
        public bool IsEdit { get; set; }
        public long AssignId { get; set; }
        public long ProjectMasterId  { get; set; }
        public ProjectMasterModel ProjectMasterModel { get; set; }
        public List<ProjectMasterModel> ProjectMasterModelsList { get; set; }
        public List<SwQcStartUpModel> SwQcStartUpModels { get; set; }
        public SwQcStartUpModel SwQcStartUpModelss { get; set; }
        public List<SwQcCallSettingModel> SwQcCallSettingModels { get; set; }
        public List<SwQcMessageModel> SwQcMessageModels { get; set; }
        public List<SwQcToolsCheckModel> SwQcToolsCheckModels { get; set; }
        public List<SwQcCameraModel> SwQcCameraModels { get; set; }
        public List<SwQcDisplayLoopModel> SwQcDisplayLoopModels { get; set; }
        public List<SwQcDisplayModel> SwQcDisplayModels { get; set; }
        public List<SwQcSettingModel> SwQcSettingModels { get; set; }
        public List<SwQcMultimediaModel> SwQcMultimediaModels { get; set; }
        public List<SwQcGoogleServiceModel> SwQcGoogleServiceModels { get; set; }
        public List<SwQcStorageCheckModel> SwQcStorageCheckModels { get; set; }
        public List<SwQcGameModel> SwQcGameModels { get; set; }
        public List<SwQcTestingAppModel> SwQcTestingAppModels { get; set; }
        public List<SwQcFileManagerModel> SwQcFileManagerModels { get; set; }
        public List<SwQcConnectivityModel> SwQcConnectivityModels { get; set; }
        public List<SwQcShutDownModel> SwQcShutDownModels { get; set; }

        public List<SwQcAssignModel> SwQcAssignModels { get; set; }
        public List<CmnUser> CmnUsers { get; set; }
        public List<SwQcInchargeAssignModel> SwQcInchargeAssignModels { get; set; }

        public List<CmnUserModel> CmnUserModels { get; set; }
        public SwQcTestCounterModel SwQcTestCounterModel { get; set; }
        public List<SwFieldTestReportView> SwFieldTestReportViews { get; set; }

        public List<SwQcTabColorModel> SwQcTabColorModels { get; set; }
        public List<SwQcProjectWiseIssueViewModel> SwQcProjectWiseIssueViewModels { get; set; }
        public List<HwQcInchargeAssignModel> HwQcInchargeAssignModels { get; set; }
        public List<HwQcAssignModel> HwQcAssignModels { get; set; }

        public List<TestPhaseModel> TestPhaseModels { get; set; }

        public List<SwQcInactiveOrAssignLogModel> SwQcInactiveOrAssignLogModels { get; set; }
        public List<SwQcPausedOrRestartActivityLogModel> SwQcPausedOrRestartActivityLogModels { get; set; }

        public List<SoftwareCustomModelForDashboard> SwList { get; set; }
        public List<PostProductionIssueModel> AllProjectIssuesForSwQcModels { get; set; }
        public string ProjectName { get; set; }
        public int? OrderNuber { get; set; }
        public string OrderNumberOrdinal { get; set; }

        //new//

        public SwQcAssignsFromQcHeadModel SwQcAssignsFromQcHeadModel { get; set; }
        public List<SwQcAssignsFromQcHeadModel> SwQcAssignsFromQcHeadModels { get; set; }
        public SwQcHeadAssignsFromPmModel SwQcHeadAssignsFromPmModel { get; set; }
        public List<SwQcHeadAssignsFromPmModel> SwQcHeadAssignsFromPmModels { get; set; }
        public ProjectDetailsForSwQcModel ProjectDetailsForSwQcModel { get; set; }
        public List<ProjectDetailsForSwQcModel> ProjectDetailsForSwQcModels { get; set; }
        public SwQcIssueDetailModel SwQcIssueDetailModel { get; set; }
        public List<SwQcIssueDetailModel> SwQcIssueDetailModels { get; set; }
        public List<SwQcIssueCategoryModel> SwQcIssueCategoryModels { get; set; }
        public SwQcIssueCategoryModel SwQcIssueCategoryModel { get; set; }
        public List<SwQcPersonalUseFindingsIssueDetailModel> SwQcPersonalUseFindingsIssueDetailModels { get; set; }
        public SwQcPersonalUseFindingsIssueDetail SwQcPersonalUseFindingsIssueDetail { get; set; }
        public SwQcNewInnovationModel SwQcNewInnovationModel { get; set; }
        public List<SwQcNewInnovationModel> SwQcNewInnovationModels { get; set; }

        public List<SelectListItem> ddlReferenceListForModal { get; set; }

        public List<SwQcTestPhaseModel> SwQcTestPhaseModels { get; set; }
        public SwQcTestPhaseModel SwQcTestPhaseModel { get; set; }

        //Field test

        public long Id { get; set; }
        public string OperatorName { get; set; }
        public string Operator { get; set; }
        public string FrequencyBand { get; set; }
        public string TestName { get; set; }
        public string TestCategory { get; set; }
        public string TestDuration { get; set; }
        public string TestFocus1 { get; set; }
        public string TestFocus2 { get; set; }
        public string TestFocus3 { get; set; }
        public string NumberOfCalls { get; set; }
        public string Location { get; set; }
        public string SpeedLimit { get; set; }
        public string TRssiBars { get; set; }
        public string TCallDrop { get; set; }
        public string TNoiseInterference { get; set; }
        public string TLongMute { get; set; }
        public string BRssiBars { get; set; }
        public string BCallDrop { get; set; }
        public string BNoiseInterference { get; set; }
        public string BLongMute { get; set; }
        public string Pass { get; set; }
        public string Fail { get; set; }
        public string TestPhaseName { get; set; }
        public string AssignedPerson { get; set; }
        public string SoftwareVersionName { get; set; }
        public long? TestPhaseID { get; set; }
        public int? SoftwareVersionNo { get; set; }
        public long? SwQcAssignId { get; set; }
        public long? SwQcHeadAssignId { get; set; }
        public string BenchmarkPhone { get; set; }
        public string Route { get; set; }
        public string Region { get; set; }
        public string FieldTestResult { get; set; }
        public string Remarks { get; set; }
        public long FieldTestId { get; set; }
        public long ProjectPmAssignId { get; set; }
        public string Issue { get; set; }
        public string ExpectedOutcome { get; set; }
        public string IssueType { get; set; }
      
        public string FieldTestFrom { get; set; }
        public string Attachment { get; set; }
        public DateTime? EntryDate { get; set; }

    } 
}