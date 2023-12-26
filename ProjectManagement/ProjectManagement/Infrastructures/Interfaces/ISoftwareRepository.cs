using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;
using ProjectManagement.Models.AssignModels;
using ProjectManagement.ViewModels.Commercial;
using ProjectManagement.ViewModels.Software;

namespace ProjectManagement.Infrastructures.Interfaces
{
    interface ISoftwareRepository
    {
        #region Get Qc Incharge dashboard
        //swqc GetHwQcTestCounts(long hwUserIdWhoLoggedIn);
        SwQcTestCounterModel GetSwQcTestCountsForQcIncharge(long swQcUserId);
        List<SoftwareCustomModelForDashboard> GetNewProjectStatusForInchargeDashboard();
        List<SoftwareCustomModelForDashboard> GetAssignedProjectToQCStatusForInchargeDashboard();
        List<SoftwareCustomModelForDashboard> GetQcCompletedProjectStatusForInchargeDashboard();
        #endregion

        #region Get Qc dashboard
        SwQcTestCounterModel GetSwQcTestCountsForQc(long swQcUserId);
        List<SoftwareCustomModelForDashboard> GetAssignedProjectToQCStatusForQcDashboard(long swQcUserId);
        List<SoftwareCustomModelForDashboard> GetQcCompletedProjectStatusForQcDashboard(long swQcUserId);
        #endregion

        #region Get Methods
        ProjectMasterModel GetProjectMasterModel(long projectId);
        CmnUserModel GetUserInfoByUserId(long userId);
       //CmnUserModel GetRoleNameByUserId(long userId);
        List<ProjectMasterModel> GetProjectList(long projectId);
        List<SwQcAssignModel> GetProjectListForQcInchargeToQcAssign(long projectId);
        List<VmSwInchargeViewModel> GetAllProjectForSendToQcInchargeToPmList();
        List<SwQcTabColorModel> GetAllTabColorAccordingToProject(long projectId);
        List<TestPhaseModel> GetTestPhases(); 
        long GetAssignId(long projectId, long userId);
        List<SwQcStartUpModel> GetStartUps(long projectId, long AssignId, string tabName, string projectType);
        List<SwQcCallSettingModel> GetCallSettings(long projectId, long AssignId, string tabName, string projectType);
        List<SwQcMessageModel> GetMessages(long projectId, long AssignId, string tabName, string projectType);
        List<SwQcToolsCheckModel> GetTools(long projectId, long AssignId, string tabName, string projectType);
        List<SwQcCameraModel> GetCamera(long projectId, long AssignId, string tabName, string projectType);
        List<SwQcDisplayLoopModel> GetDisplayLoop(long projectId, long AssignId, string tabName, string projectType);
        List<SwQcDisplayModel> GetDisplay(long projectId, long AssignId, string tabName, string projectType);
        List<SwQcSettingModel> GetSetting(long projectId, long AssignId, string tabName, string projectType);
        List<SwQcMultimediaModel> GetMultimedia(long projectId, long AssignId, string tabName, string projectType);
        List<SwQcGoogleServiceModel> GetGoogleService(long projectId, long AssignId, string tabName, string projectType);
        List<SwQcStorageCheckModel> GetStorageCheck(long projectId, long AssignId, string tabName, string projectType);
        List<SwQcGameModel> GetGame(long projectId, long AssignId, string tabName, string projectType);
        List<SwQcTestingAppModel> GetTestingApp(long projectId, long AssignId, string tabName, string projectType);
        List<SwQcFileManagerModel> GetFileManager(long projectId, long AssignId, string tabName, string projectType);
        List<SwQcConnectivityModel> GetConnectivity(long projectId, long AssignId, string tabName, string projectType);
        List<SwQcShutDownModel> GetShutDown(long projectId, long AssignId, string tabName, string projectType);
       
        #endregion

        #region Save Method
        bool SaveSwQcStartUp(List<SwQcStartUpModel> swQcStartUpModels);
        bool SaveSwQcCallSetting(List<SwQcCallSettingModel> swQcCallSettingModels);
        bool SaveSwQcMessage(List<SwQcMessageModel> swQcMessagesModels);
        bool SaveSwQcTools(List<SwQcToolsCheckModel> swQcToolsCheckModels);
        bool SaveSwQcCamera(List<SwQcCameraModel> swQcCameraModels);
        bool SaveSwQcDisplayLoop(List<SwQcDisplayLoopModel> swQcDisplayLoopModels);
        bool SaveSwQcDisplay(List<SwQcDisplayModel> swQcDisplayModels);
        bool SaveSwQcSetting(List<SwQcSettingModel> swQcSettingModels);
        bool SaveSwQcMultimedia(List<SwQcMultimediaModel> swQcMultimediaModels);
        bool SaveSwQcGoogleService(List<SwQcGoogleServiceModel> swQcGoogleServiceModels);
        bool SaveSwQcStorageCheck(List<SwQcStorageCheckModel> swQcStorageCheckModels);
        bool SaveSwQcGame(List<SwQcGameModel> swQcGameModels);
        bool SaveSwQcTestingApp(List<SwQcTestingAppModel> swQcTestingAppModels);
        bool SaveSwQcFileManager(List<SwQcFileManagerModel> swQcFileManagerModels);
        bool SaveSwQcConnectivity(List<SwQcConnectivityModel> swQcConnectivityModels);
        bool SaveSwQcShutDown(List<SwQcShutDownModel> swQcShutDownModels);
        #endregion

        #region Update Method
        bool UpdateSwQcStartUp(List<SwQcStartUpModel> swQcStartUpModels);
        bool UpdateSwQcCallSetting(List<SwQcCallSettingModel> swQcCallSettingModels);
        bool UpdateSwQcMessage(List<SwQcMessageModel> swQcMessagesModels);
        bool UpdateSwQcTools(List<SwQcToolsCheckModel> swQcToolsCheckModels);
        bool UpdateSwQcCamera(List<SwQcCameraModel> swQcCameraModels);
        bool UpdateSwQcDisplayLoop(List<SwQcDisplayLoopModel> swQcDisplayLoopModels);
        bool UpdateSwQcDisplay(List<SwQcDisplayModel> swQcDisplayModels);
        bool UpdateSwQcSetting(List<SwQcSettingModel> swQcSettingModels);
        bool UpdateSwQcMultimedia(List<SwQcMultimediaModel> swQcMultimediaModels);
        bool UpdateSwQcGoogleService(List<SwQcGoogleServiceModel> swQcGoogleServiceModels);
        bool UpdateSwQcStorageCheck(List<SwQcStorageCheckModel> swQcStorageCheckModels);
        bool UpdateSwQcGame(List<SwQcGameModel> swQcGameModels);
        bool UpdateSwQcTestingApp(List<SwQcTestingAppModel> swQcTestingAppModels);
        bool UpdateSwQcFileManager(List<SwQcFileManagerModel> swQcFileManagerModels);
        bool UpdateSwQcConnectivity(List<SwQcConnectivityModel> swQcConnectivityModels);
        bool UpdateSwQcShutDown(List<SwQcShutDownModel> swQcShutDownModels);
        #endregion

        #region Get Methods For Details
        List<SwQcStartUpModel> GetStartUpsForDetails(long projectId, long swqcInchargeId);
        List<SwQcCallSettingModel> GetCallSettingForDetails(long projectId, long swqcInchargeId);
        List<SwQcMessageModel> GetMessageForDetails(long projectId, long swqcInchargeId);
        List<SwQcToolsCheckModel> GetToolsForDetails(long projectId, long swqcInchargeId);
        List<SwQcCameraModel> GetCameraForDetails(long projectId, long swqcInchargeId);
        List<SwQcDisplayLoopModel> GetDisplayLoopForDetails(long projectId, long swqcInchargeId);
        List<SwQcDisplayModel> GetDisplayForDetails(long projectId, long swqcInchargeId);
        List<SwQcSettingModel> GetSettingForDetails(long projectId, long swqcInchargeId);
        List<SwQcMultimediaModel> GetMultimediaForDetails(long projectId, long swqcInchargeId);
        List<SwQcGoogleServiceModel> GetGoogleServiceForDetails(long projectId, long swqcInchargeId);
        List<SwQcStorageCheckModel> GetStorageCheckForDetails(long projectId, long swqcInchargeId);
        List<SwQcGameModel> GetGameForDetails(long projectId, long swqcInchargeId);
        List<SwQcTestingAppModel> GetTestingAppForDetails(long projectId, long swqcInchargeId);
        List<SwQcFileManagerModel> GetFileManageForDetails(long projectId, long swqcInchargeId);
        List<SwQcConnectivityModel> GetConnectivityForDetails(long projectId, long swqcInchargeId);
        List<SwQcShutDownModel> GetShutDownForDetails(long projectId, long swqcInchargeId);
        List<SwQcProjectWiseIssueViewModel> GetProjectWiseIssueViewModelsForDetails(long projectId, long swqcInchargeId);
        #endregion

        //#region Get Methods of Details for All
        //List<SwQcStartUpModel> AllGetStartUpsForDetails(long projectId);
        //List<SwQcCallSettingModel> AllGetCallSettingForDetails(long projectId);
        //List<SwQcMessageModel> AllGetMessageForDetails(long projectId);
        //List<SwQcToolsCheckModel> AllGetToolsForDetails(long projectId);
        //List<SwQcCameraModel> AllGetCameraForDetails(long projectId);
        //List<SwQcDisplayLoopModel> AllGetDisplayLoopForDetails(long projectId);
        //List<SwQcDisplayModel> AllGetDisplayForDetails(long projectId);
        //List<SwQcSettingModel> AllGetSettingForDetails(long projectId);
        //List<SwQcMultimediaModel> AllGetMultimediaForDetails(long projectId);
        //List<SwQcGoogleServiceModel> AllGetGoogleServiceForDetails(long projectId);
        //List<SwQcStorageCheckModel> AllGetStorageCheckForDetails(long projectId);
        //List<SwQcGameModel> AllGetGameForDetails(long projectId);
        //List<SwQcTestingAppModel> AllGetTestingAppForDetails(long projectId);
        //List<SwQcFileManagerModel> AllGetFileManageForDetails(long projectId);
        //List<SwQcConnectivityModel> AllGetConnectivityForDetails(long projectId);
        //List<SwQcShutDownModel> AllGetShutDownForDetails(long projectId);
        //List<SwQcProjectWiseIssueViewModel> AllGetProjectWiseIssueViewModelsForDetails(long projectId);
        //#endregion

        #region Re-Assign QcIncharge to QC 
        string SaveQcInchargeToQcReAssignProject(string testPhaseId, long pMasterId, string projectName, long pSwQcInId, string multiple1, string ApproxInchargeToQcDeliveryDate, string SwInchargeAssignToQcComment, long pPrPmAssignId, string pmDate, string softwareName, string softwareNo);

        #endregion

        #region AssignMutipleQc
        List<PmQcAssignModel> GetPmQcAssignModels();
        List<PmQcAssignModel> GetPmToQcHeadAssignModels();
        List<CmnUserModel> GetActiveQc();
        List<CmnUserModel> GetActiveHw();
        string SaveAssignMuliplePerson(string projectMasterId, string swQcInchargeAssignId, string projectPmAssignId, string swInchargeAssignToQcComment, string[] multiple, string approxInchargeToQcDeliveryDate, string accessoriesTestType);
        #endregion

        #region Qc Incharge to PM
       // string SoftWareQcInchargeToPm(long pMasterId, string swQcInchargeComment, long userId, long pSwQcInId);
        List<SwQcAssignsFromQcHeadModel> GetCompletedProjectForQcHeadToPmSubmit();
        #endregion

        #region QC to Incharge Final Project Submit
        string SoftwareQcToQcInchargeProjectSubmit(long sPMasterId, long sQcUserId, string proStatus, long sQcInchargeAssignId);

        #endregion

        #region Filed test
        //void SaveFieldTest(string[] ddlUsers, List<SwFieldTestDetailModel> details, string comparedWith, string issueOf, string comment);
        string SaveFieldTest(long pMasterId, long phwQcInuId, string[] ddlUsers, List<SwFieldTestDetailModel> details, string comparedWith, string issueOf, string comment);
        //List<ProjectMasterModel> GetProjectListForFieldTest(long projectId);
        List<ProjectMasterModel> GetProjectListForFieldTest();
        List<SwQcInchargeAssignModel> GetSwQcInchargeAssignModelsForFieldTest();
        #endregion

        #region Field test report

        List<ProjectMasterModel> GetFieldTestCompletedProjectList();
        List<SwFieldTestReportView> GetFieldTestProjectLisForPrintReport(long projectId,long userId);
        #endregion

        #region Add new Issues && Get Issues
        List<SwQcProjectWiseIssueViewModel> GetSwQcProjectWiseIssueViewModelss(long pMasterId, long pSwQcInId, long pSwQcAssignId);
        List<string> GetIssueModules(string type);
        string SaveProjectWiseIssues(List<SwQcProjectWiseIssueViewModel> issueList, long pMasterId, long pSwQcInId, long pSwQcAssignId);
        #endregion

        #region Paused or Restart any project

        //List<CmnUserModel> GetAssignQc(long pMasterId);
        string SaveSwPauseOrRestartAssignedProject(long pMasterId, string projectHeadRemarks, long pSwQcInId, string projectName, long pmHeadAssignId);
        string SaveSwRestartAssignedProject(long pMasterId, long pSwQcInId, string projectName, long pmHeadAssignId);

        #endregion

        #region Report Dashboard

        //List<ProjectMaster> GetProjectListForReportOfQcIncharge();
        List<SoftwareCustomModelForDashboard> GetAllProjectListDetailsForInchargeReport(string startValue, string endValue, string emplyCode);
       // List<SoftwareCustomModelForDashboard> GetAllProjectListDetailsOfQcPersonForInchargeReport(string startValue, string endValue, string emplyCode);

        /// ////////////showing Report Data Details after click on TestPhaase///////////

        List<SwQcStartUpModel> GetStartUpsForDetailsReport(long projectId, long swqcInchargeAsngId, string emplyCode);
        List<SwQcCallSettingModel> GetCallSettingForDetailsReport(long projectId, long swqcInchargeAsngId, string emplyCode);
        List<SwQcMessageModel> GetMessageForDetailsReport(long projectId, long swqcInchargeAsngId, string emplyCode);
        List<SwQcToolsCheckModel> GetToolsForDetailsReport(long projectId, long swqcInchargeAsngId, string emplyCode);
        List<SwQcCameraModel> GetCameraForDetailsReport(long projectId, long swqcInchargeAsngId, string emplyCode);
        List<SwQcDisplayLoopModel> GetDisplayLoopForDetailsReport(long projectId, long swqcInchargeAsngId, string emplyCode);
        List<SwQcDisplayModel> GetDisplayForDetailsReport(long projectId, long swqcInchargeAsngId, string emplyCode);
        List<SwQcSettingModel> GetSettingForDetailsReport(long projectId, long swqcInchargeAsngId, string emplyCode);
        List<SwQcMultimediaModel> GetMultimediaForDetailsReport(long projectId, long swqcInchargeAsngId, string emplyCode);
        List<SwQcGoogleServiceModel> GetGoogleServiceForDetailsReport(long projectId, long swqcInchargeAsngId, string emplyCode);
        List<SwQcStorageCheckModel> GetStorageCheckForDetailsReport(long projectId, long swqcInchargeAsngId, string emplyCode);
        List<SwQcGameModel> GetGameForDetailsReport(long projectId, long swqcInchargeAsngId, string emplyCode);
        List<SwQcTestingAppModel> GetTestingAppForDetailsReport(long projectId, long swqcInchargeAsngId, string emplyCode);
        List<SwQcFileManagerModel> GetFileManageForDetailsReport(long projectId, long swqcInchargeAsngId, string emplyCode);
        List<SwQcConnectivityModel> GetConnectivityForDetailsReport(long projectId, long swqcInchargeAsngId, string emplyCode);
        List<SwQcShutDownModel> GetShutDownForDetailsReport(long projectId, long swqcInchargeAsngId, string emplyCode);
        List<SwQcProjectWiseIssueViewModel> GetProjectWiseIssueViewModelsForDetailsReport(long projectId, long swqcInchargeAsngId, string emplyCode);
        #endregion

        #region Qc Report Dashboard
        List<SoftwareCustomModelForDashboard> GetAllProjectListDetailsForQc(string startValue, string endValue,
            long userId);
        #endregion

        #region Qc person Delete or Newly assign person
        //repository.DeleteOrNewAssignQcByQcIncharge(pMasterId, pSwQcInId, pPrPmAssignId, multideleteValue, multiReassignValue, approxInchargeToQcDeliveryDate, swInchargeDeleteQcComment, swQcInchargeReassignToQcComment);
        List<CmnUserModel> GetDeletedPersonNameList(long projectId, long swqcInchargeIds, long projectPmAssignIds);
        string DeleteOrNewAssignQcByQcIncharge(long pMasterId, long pSwQcInId, long pPrPmAssignId, string multideleteValue, string multiReassignValue, string approxInchargeToQcDeliveryDate, string swInchargeDeleteQcComment, string swQcInchargeReassignToQcComment);
        //string DeleteQcByQcIncharge(long pMasterId, long pSwQcInId, long pPrPmAssignId, string multideleteValue1, string  approxInchargeToQcDeliveryDate, string swInchargeDeleteQcComment);

        //string NewlyAssignQcByQcIncharge(long pMasterId, long pSwQcInId, long pPrPmAssignId, string multiReassignValue,
        //   string approxInchargeToQcDeliveryDate, string swQcInchargeReassignToQcComment);

        #endregion      

        #region Zip All Files
        List<SwQcAllFilesModel> GetAllFilesModel(long projectId, long swqcInchargeId);

        #endregion

        #region Post production
        List<SwQcPostProductionAssignModel> GetProjectListForPostProductionAssign();
        List<SwQcPostProductionAssignModel> GetProjectListForPostProductionIssueList(long userId);
        List<PostProductionIssueModel> GetProjectListForPostProductionIssueListAfterSearch(long pMasterId, long userId);
        List<PostProductionIssueModel> GetProjectListForPostProductionIssueListAfterSearchForMK(string pro_name, long userId);

        List<PostProductionIssueModel> GetProjectListForPostProductionIssueListAfterSearchForAll(long pMasterId,
            long userId);
        string DeleteOrNewAssignQcByQcInchargeForPostProduction(long pMasterId,
            string swInchargeDeleteQcComment, string swQcInchargeAssignToQcComment, long sampleNumber,
            string multideleteValue, string multiAssignValue);
        //List<AllProjectIssuesForSwQcModel> GetAllProjectIssuesForSwQcModelsForPostProduction(long pMasterId, long pSwQcInId, long pSwQcAssignId);
        List<CmnUserModel> GetDeletedPersonNameListForPostProduction(long projectId);
        List<ProjectMasterModel> GetProjectMasterModelForPostProduction();

        string SaveAllProjectIssuesForSwQcModels(List<PostProductionIssueModel> issueList, long pMasterId, long swQcPostPro, string allUsersList);
        string SaveAllProjectIssuesForMKTModels(List<PostProductionIssueModel> issueList, long pMasterId, string allUsersList);
        List<ProjectMasterModel> GetProjectOrderNumberList(string projectName);

        #endregion

        #region comment

        //List<SwQcBatteryAssignIssueModel> GetSwQcBatteryForDetails(long projectId);
        //List<SwQcBatteryAssignIssueModel> GetSwQcBatteryForList(long projectId);
        /// /////////others////////////
        //SwQcInchargeAssignModel GetInchargeAssignModel(long currentMasterId1);

        #endregion

        //#region
        //#endregion

        #region SwQc New
        List<ProjectMasterModel> GetProjectListForSwQc(long userId);
        List<SwQcAssignsFromQcHeadModel> GetProjectListForQcHeadToQcAssign(long userId);
        List<SwQcHeadAssignsFromPmModel> GetSwQcHeadAssignModelsFromPmForIssue();
        List<ProjectDetailsForSwQcModel> GetProjectDetailsForSwQc(long userId, long pMasterId, long pSwQcInId);
        string SaveSwQcProjectIssueDetails(List<SwQcIssueDetailModel> issueList, List<SwQcIssueDetailModel> issueList1, long pMasterId, bool issuesChk, bool filesChk, long pSwQcInId, long pSwQcAssignId);
        List<SwQcIssueDetailModel> GetSwQcIssueDetails(long pMasterId, long pSwQcInId, long pSwQcAssignId);

        string SaveSwQcSubmittedProjectToQcHead(long pMasterId, long pSwQcInId, long pSwQcAssignId, long userId);
        List<ProjectMasterModel> GetProjectListForSwQcHead();
        List<SwQcIssueCategoryModel> GetIssueCategory();
        List<SwQcIssueDetailModel> GetSwQcIssueDetailsForSupplier(string projectName, string moduleName, int projectOrders, int softVersionNo,string testPhases);
        List<SwQcAssignsFromQcHeadModel> GetSwQcsAssignsInfo(string projectName, int projectOrders, int softVersionNo, string testPhases);
        string UpdateSwQcIssueDetailModelForApprove(SwQcIssueDetailModel results);
        string UpdateSwQcIssueDetailModelForDecline(SwQcIssueDetailModel results);
        string UpdateNewInnovationModelForApprove(SwQcNewInnovationModel results);
        string UpdateNewInnovationModelForDecline(SwQcNewInnovationModel results);

        string UpdatePersonalFindingsForApprove(SwQcPersonalUseFindingsIssueDetailModel results);
        string UpdatePersonalFindingsForDecline(SwQcPersonalUseFindingsIssueDetailModel results);

        string UpdateSwQcIssueDetailModelForSupplier(SwQcIssueDetailModel supplierUpdate);
        List<ProjectMasterModel> GetProjectListForFieldTestNew();
        List<PmQcAssignModel> GetProjectDetailsForQcFieldTest(long pMasterId, long softwareVerNumber, long proOrder);
        string SaveAssignForFieldTestFromQcHead(string ProjectName, string projectMasterId, string projectPmAssignId, string swInchargeAssignToQcComment, string[] multiple, string singleOne, string approxInchargeToQcDeliveryDate, string SoftwareVersionNo);
        List<SwQcHeadAssignsFromPmModel> GetProjectListForMPVersionSwQc(long userId);

        List<ProjectDetailsForSwQcModel> GetProjectDetailsForSwQcPersonalFindings(long userId, long pMasterId, long pSwQcInId);
        string SaveSwQcPersonalUseFindingsIssueDetails(List<SwQcPersonalUseFindingsIssueDetailModel> issueList, List<SwQcPersonalUseFindingsIssueDetailModel> issueList1, long pMasterId, bool issuesChk, bool filesChk, long pSwQcInId, long pPmAssignId);
        List<SwQcPersonalUseFindingsIssueDetailModel> GetSwQcPersonalFindingIssueDetails(long pMasterId, long pSwQcInId, long pPmAssignId);
        List<string> GetAllRoles();
        string SaveSwQcNewInnovation(List<SwQcNewInnovationModel> issueList);
        List<SwQcNewInnovationModel> GetSwQcNewInnovation();
        List<SwQcIssueDetailModel> GetSwQcIssueDetailsForQcHeadToPmForward(string projectId, string swqcInchargeId, string pmAssignId, DateTime swQcHeadToQcAssignTime, string testPhaseId, string SoftwareVersionNo);
        List<SwQcIssueDetailModel> GetSwQcCtsMonkeyOrCameraAutomationData(string projectId, string swqcInchargeId, string pmAssignId, DateTime swQcHeadToQcAssignTime, string testPhaseId,string SoftwareVersionNo);
        List<SwQcPersonalUseFindingsIssueDetailModel> GetPersonalUseFindingsForQcHead(string projectId, string swqcInchargeId, string pmAssignId, DateTime swQcHeadToQcAssignTime, string testPhaseId);
        List<SwQcNewInnovationModel> GetNewInnovationForQcHead();

        #endregion

        string SoftWareQcInchargeToPm(long pMasterId, long pmUserIdCon, string testPhaseId, long userId, long pSwQcInId, string projectName, string proComment, DateTime pmDate, string softwareName, string softwareNo, bool isFinals, DateTime swQcHeadToQcAssignTime);
        List<SwQcAssignsFromQcHeadModel> GetAssignedProjectDetailsForQcFieldTest(long pMasterId, long softwareVerNumber, long proOrder, string projectName);
        List<SoftwareCustomModelForDashboard> GetAllFieldTestListForInchargeReport(string startValue, string endValue, string emplyCode);
        bool GetSupplierFeedbackData(SwQcIssueDetailModel supplierUpdate);
        string AllApproveForChaina(List<SwQcIssueDetailModel> results);
        string SwQcIssueDelete(SwQcIssueDetailModel supplierUpdate);
        List<SwQcHeadAssignsFromPmModel> GetProjectVersionName(string projectId, int swVersionNo, long testPhaseIds);
        List<SwQcIssueDetailModel> GetSwQcIssueDetailsForReport(long projectId, string projectName, long swqcInchargeAsngId, string emplyCode, DateTime swQcHeadToQcAssignTime, long testPhaseId);
        List<SwQcIssueDetailModel> GetSwQcCtsMonkeyOrCameraAutomationDataForReport(long projectId, string projectName, long swqcInchargeAsngId, string emplyCode, DateTime swQcHeadToQcAssignTime, long testPhaseId);
        List<SwQcPersonalUseFindingsIssueDetailModel> GetPersonalUseFindingsForQcHeadForReport(long projectId, long swqcInchargeAsngId);
        List<SoftwareCustomModelForDashboard> GetAllProjectPersonStatus(string emplyCode);
        List<SwQcTestPhaseModel> GetSwQcTestPhase();
        string EditSwQcIssueDetails(long proId, long swIssueId, string issueScenario, string expectedOutcome, string result, string refernceModule, string issueReproducePath, string attachment, string issueType, string filesUrl, string frequency);
        List<SwQcTestPhaseModel> GetTestPhasesForQcHeadIssue();
        List<ProjectMasterModel> GetProjectListForQcHeadIssue();
        string SaveIssueDetailsForQcHead(List<SwQcIssueDetailModel> issueList, List<SwQcIssueDetailModel> issueList1, bool issuesChk, bool filesChk, long pMasterId, long pSwQcInId, long pSwQcAssignId, string projectName, long testPhaseIds, int softwareVersionNames);
        List<SwQcIssueDetailModel> GetSwQcIssuesForHead(string proName, string testPhaseNameId, int softVersionNo);
        string DeleteQcInnovation(SwQcNewInnovationModel supplierUpdate);
        List<CmnUserModel> GetInnoVationAssignedBy();
        string EditInnovationDetails(long newInnoIds, string projectName, string assignedBy1, string assignedBy2, string description, string workType, DateTime effectiveDate);
        //List<SwQcAllIncentiveListModel> GetAllIncentiveList();
        bool UpdateDbByExcel(string projectName, long softVersion, HttpPostedFileBase excelFile, string testPhaseIds);
        List<SwQcAssignsFromQcHeadModel> GetCompletedFieldTestProjectForQcHeadToPmSubmit();
        List<SwQcIssueDetailModel> GetIssueStatus();
        List<SwQcIssueDetailModel> GetTotalProjectsIssue(string projectId, string waltonQcStatus);
        List<SwQcAllIncentiveListModel> GetAllIncentiveList(string projectType);
        List<ProjectTypeModel> GetProjectType();
        string UpdateAllIncentiveList(List<SwQcAllIncentiveListModel> swQcAllIncentiveListModels);
        List<CmnUserModel> GetQcUserList();
        List<SwQcIssueDetailModel> GetIssueDetailsForIncentive(string months, string years, string roles, string persons);
        List<SwQcNewInnovationModel> GetNewInnovationForIncentive(string months, string years, string roles, string persons);
        List<SwQcPersonalUseFindingsIssueDetailModel> GetPersonalUseIncentive(string months, string years, string roles, string persons);
        List<SwQcIssueDetailModel> GetCtsDataForIncentive(string months, string years, string roles, string persons);
        List<SwQcIssueDetailModel> GetFieldAssignByHeadDataForIncentive(string months, string years, string roles, string persons);
        List<ProjectMasterModel> GetAllProjectName();

        #region Save Incentive
        string SaveSwIncentive_Issue(List<Custom_Sw_IncentiveModel> results);
        string SaveSwIncentive_ExtraWork(List<Custom_Sw_IncentiveModel> results);
        string SaveSwIncentive_PersonalUse(List<Custom_Sw_IncentiveModel> results);
        string SaveSwIncentive_Cts(List<Custom_Sw_IncentiveModel> results);
        string SaveSwIncentive_FieldByHead(List<Custom_Sw_IncentiveModel> results);
        string SaveSwIncentive_Others(List<Custom_Sw_IncentiveModel> results);
        string SaveSwIncentive_Penalties(List<Custom_Sw_IncentiveModel> results);
        string SaveSwIncentive_Incentive(List<Custom_Sw_IncentiveModel> results);
        string SaveSwIncentive_BrandIssue(List<Custom_Sw_IncentiveModel> results);
        string SaveSwIncentive_BrandCost(List<Custom_Sw_IncentiveModel> results);
        string SaveQcAllMemberRewardsAndPenalties(int monNum, string year);
        //bool
        bool GetAll_QcMembersMonthlyIncentiveData(string employeeCode, int monNum, string year);
        bool GetExtraWorkData(string employeeCode, int monNum, string year);//GetPersonalUseData
        bool GetPersonalUseData(string employeeCode, int monNum, string year);
        bool GetCtsIncentiveData(string employeeCode, int monNum, string year);
        bool GetFieldByHeadData(string employeeCode, int monNum, string year);
        bool GetOthersData(string employeeCode, int monNum, string year);
        bool GetPenaltiesData(string employeeCode, int monNum, string year);
        bool GetIncentiveData(string employeeCode, int monNum, string year);
        bool GetBrandIssuesData(string employeeCode, int monNum, string year);
        bool GetBrandCostData(string employeeCode, int monNum, string year);
        List<IncentiveModel> GetHeadDeputyIncentiveList(string empCode, int mons, string year, string roles);

        #endregion

        #region Incentive Sheet
        List<CmnUserModel> GetSwUserList();
        List<Custom_Sw_IncentiveModel> All_QcMembersIncentiveReportTopSheet(string month, string monNum, string year);
        List<Custom_Sw_IncentiveModel> GetPreparedUserName();
        List<Custom_Sw_IncentiveModel> GetSwIncentive_IssueForPrint(string empCode, string monNum, string year);
        List<Custom_Sw_IncentiveModel> GetTotalFinalIncentiveOfSw(string empCode, string monNum, string year);
        List<Custom_Sw_IncentiveModel> GetSwIncentive_ExtraWorkForPrint(string empCode, string monNum, string year);
        List<Custom_Sw_IncentiveModel> GetSwIncentive_PersonalUseForPrint(string empCode, string monNum, string year);
        List<Custom_Sw_IncentiveModel> GetSwIncentive_CtsForPrint(string empCode, string monNum, string year);
        List<Custom_Sw_IncentiveModel> GetSwIncentive_FieldByHeadForPrint(string empCode, string monNum, string year);
        List<Custom_Sw_IncentiveModel> GetSwIncentive_OthersForPrint(string empCode, string monNum, string year);
        List<Custom_Sw_IncentiveModel> GetSwIncentive_PenaltiesForPrint(string empCode, string monNum, string year);
        List<Custom_Sw_IncentiveModel> GetSwIncentive_PenaltiesForIssuesForPrint(string empCode, string monNum, string year);
        List<Custom_Sw_IncentiveModel> GetSwIncentive_BrandIssuesForPrint(string empCode, string monNum, string year);
        List<Custom_Sw_IncentiveModel> GetSwIncentive_BrandCostForPrint(string empCode, string monNum, string year);
        #endregion

        #region Penalties
        List<ProjectMasterModel> GetAllProjectNamesForPenalties();
        List<SwIncentive_PenaltiesForIssuesModel> GetPenaltiesForIssues(string months, string years, string roles, string persons);
        bool GetPenaltiesDataForIssues(string employeeCode, int monNum, int year);
        string SaveSwIncentive_AutoPenaltiesForIssues(List<SwIncentive_PenaltiesForIssuesModel> results);
        #endregion

        List<ProjectMasterModel> SaveGetAllModelsHistory(string model, string releaseDate1, DateTime startDate, DateTime endDate, string fNames, int mons, string years);
        List<ProjectMasterModel> GetAllModelsHistory(string model, int mons, string year);
        List<ProjectMasterModel> GetDataFromPenaltiesTable(string monNum1, string year);
        List<SwQcAssignsFromQcHeadModel> GetAccessoriesModel(long userId);
        string SaveAccessoriesTest(List<SwQcEarphoneTestModel> issueList, List<SwQcEarphoneTestModel> issueList1, long swQcHeadAssignId, long swQcAssignId, string projectType, long proId, long testId);
        List<SwQcEarphoneTestModel> GetSavedAccessoriesDataEarphone(string swQcheadId, string swQcAssignId);
        string SaveAccessoriesIssueDelete(SwQcEarphoneTestModel supplierUpdate);
        string SaveEditAccessoriesData(long accessIds, string headphoneModel1, string musicPlayerPlayback, string videoPlayerPlayback, string voiceCall, string voiceCallController, string fmPlayback, string fmController, string controller, string remarks, string musicBase, string youtubePlayback, string youtubeController, string volumeController, string highEndDevice, string midRangeDevice, string lowerMidRangeDevice, string lowRangeDevice);
        string SaveAccessoriesSubmittedProjectToQcHead(long swQcHeadIds, long swQcAssignIds);
        List<SwQcEarphoneTestModel> GetEarphoneDataForDetails(string projectId, string swqcInchargeId);
        List<SwQcHeadAssignsFromPmModel> GetAccessoriesModelForExcel();
        List<SwQcEarphoneTestModel> GetAccessListForExportEarphone(string projectName, string allOrLatest);
        string SaveBatteryTest(List<SwQcBatteryTestModel> issueList3, long swQcHeadAssignId, long swQcAssignId, string projectType, long proId, long testId, string accessoriesTestType);
        List<SwQcBatteryTestModel> GetSavedAccessoriesDataBattery(string swQcheadId, string swQcAssignId);
        List<SwQcGlassProtectorTestModel> GetSavedGlassProtectorAndChargerData(string swQcheadId, string swQcAssignId);
        string SaveBatteryIssueDelete(SwQcBatteryTestModel supplierUpdate);
        string SaveEditBatteryData(long batteryIds, string checkPoints1, string batterymAh, string hundredToNighty, string nightyToEighty, string eightyToSeventy, string seventyToSixty, string sixtyToFifty, string fiftyToFourty, string fourtyToThirty, string thirtyToTwenty, string twentyToTen, string tenToZero, string averageFullDischarge);
        List<SwQcBatteryTestModel> GetSavedBatteryDataForDetails(string projectId, string swqcInchargeId);
        List<SwQcBatteryTestModel> GetAccessListForExportBattery(string projectName, string allOrLatest);
        List<SwQcAssignsFromQcHeadModel> GetFieldTestModel(long userId);
        List<SwQcFieldTestStaticDataModel> GetFieldTestDetailsData(string swQcheadId, string proName, string swQcAssignId);


      //  string SaveOrUpdateFieldTestData(List<SwQcFieldTestDetailModel> issueList, string operatorStatus, long swQcHeadAssignId, long swQcAssignId, string projectType, long proId, long testId, string projectName, string attachment1, string benchmarkPhone1, string route1, string region1, string fieldTestResult1, string remarks1, string location1, string speedLimit1, string rssiBars1, string bRssiBars1, string airtelMt, string airtelMtTCallDrop, string airtelMtTNoiseInterference, string airtelMtTLongMute, string airtelMtBCallDrop, string airtelMtBNoiseInterference, string airtelMtBLongMute, string airtelMo, string airtelMoTCallDrop, string airtelMoTNoiseInterference, string airtelMoTLongMute, string airtelMoBCallDrop, string airtelMoBNoiseInterference, string airtelMoBLongMute, string teletalkMt, string teletalkMtTCallDrop, string teletalkMtTNoiseInterference, string teletalkMtTLongMute, string teletalkMtBCallDrop, string teletalkMtBNoiseInterference, string teletalkMtBLongMute, string teletalkMo, string teletalkMoTCallDrop, string teletalkMoTNoiseInterference, string teletalkMoTLongMute, string teletalkMoBCallDrop, string teletalkMoBNoiseInterference, string teletalkMoBLongMute, string robiMt, string robiMtTCallDrop, string robiMtTNoiseInterference, string robiMtTLongMute, string robiMtBCallDrop, string robiMtBNoiseInterference, string robiMtBLongMute, string robiMo, string robiMoTCallDrop, string robiMoTNoiseInterference, string robiMoTLongMute, string robiMoBCallDrop, string robiMoBNoiseInterference, string robiMoBLongMute, string banglalinkMt, string banglalinkMtTCallDrop, string banglalinkMtTNoiseInterference, string banglalinkMtTLongMute, string banglalinkMtBCallDrop, string banglalinkMtBNoiseInterference, string banglalinkMtBLongMute, string banglalinkMo, string banglalinkMoTCallDrop, string banglalinkMoTNoiseInterference, string banglalinkMoTLongMute, string banglalinkMoBCallDrop, string banglalinkMoBNoiseInterference, string banglalinkMoBLongMute, string grameenphoneMt, string grameenphoneMtTCallDrop, string grameenphoneMtTNoiseInterference, string grameenphoneMtTLongMute, string grameenphoneMtBCallDrop, string grameenphoneMtBNoiseInterference, string grameenphoneMtBLongMute, string grameenphoneMo, string grameenphoneMoTCallDrop, string grameenphoneMoTNoiseInterference, string grameenphoneMoTLongMute, string grameenphoneMoBCallDrop, string grameenphoneMoBNoiseInterference, string grameenphoneMoBLongMute);
        List<SwQcFieldTestDetailModel> GetFieldTestDetailsSavedData(string swQcheadId, string swQcAssignId);
        string UpdateFieldOperatorData(long fieldTestIds,string TRSSIbars,string BRSSIbars,  string callDrop, string noiseInterference, string longMute, string bCallDrop, string bNoiseInterference, string bLongMute);
        string UpdateFieldRouteData(long fieldTestIds, string benchmarkPhone, string route, string region, string FrequencyBand, string fieldTestResult, string remarks);
        string UpdateFieldIssueData(long fieldTestIds, string issue, string expectedOutcome, string issueType);
        string DeleteFieldIssueData(long fieldTestIds);
        string FieldTestFinalSubmit(long swQcheadIds, long swQcAssignIds);
        string SaveOrUpdateFieldTestData(List<SwQcFieldTestDetailModel> issueList, List<SwQcFieldTestDetailModel> issueList1, string SoftwareVersionName1,string FrequencyBand33, string operatorStatus, long swQcHeadAssignId, long swQcAssignId, string projectType, long proId, long testId, string projectName, string attachment1, string benchmarkPhone1, string route1, string region1, string fieldTestResult1, string remarks1, string location1, string speedLimit1, string rssiBars1, string bRssiBars1);
        string SaveGlassProtectorAndChargerTest(List<SwQcBatteryTestModel> issueList4, long swQcHeadAssignId, long swQcAssignId, string projectType, long proId, long testId, string accessoriesTestType);
        List<SwQcAssignsFromQcHeadModel> GetFieldTestModelForPrint(long userId);
        List<SwQcAssignsFromQcHeadModel> GetProjectDetailsForFieldDetails(string ProjectsDetails);
        List<SwQcFieldTestDetailModel> GetDataForFieldTestPrint(long swQcheadIds, string projectName);
        List<SwQcIssueDetailModel> GetPenaltiesDataForIncentive(string months, string years, string roles, string persons);
        string SaveSwIncentive_AutoPenalties(List<Custom_Sw_IncentiveModel> results);
        List<SwQcIssueDetailModel> GetPenaltiesDataForHeadIncentive(string monNum, string year, string roles, string empCode);
        string SaveSwIncentive_OthersAuto(List<Custom_Sw_IncentiveModel> results);
        List<Custom_Sw_IncentiveModel> GetOthersDetails();
        List<SwQcIssueDetailModel> GetOthersDataForIncentive(string months, string years, string roles, string persons);
        List<ProjectMasterModel> GetDataFromPenaltiesTablePerProject(string monNum1, string year, string projectName);
        List<SwQcTestPhaseModel> GetSwQcTestPhaseForSupp();
        string ForwardSwQcIssues(long SwQcIssueIds, string ProjectName, long IssueSerials, string issueScenario, int softwareVersionNos, long testPhaseIDs, string waltonQcStatus, string waltonQcComment, string supplierComment);
        List<SwQcTestPhaseModel> GetSwQcTestPhaseForSuppDemo();
        List<VmAllIncentiveList> GetPoDateWisePenalties(string monthNum, string yearName);
        List<VmAllIncentiveList> GetNinetyFiveProductionReward(string monthNum, string yearName);
        List<VmAllIncentiveList> GetPmClearanceVsLsdForReport(string monNum1, string year);
        List<VmAllIncentiveList> GetNinetyFiveSalesOutReward(string monthNum, string yearName);
        List<QcNewRewardAndPenaltiesModel> GetPoDateWisePenaltiesAccountant(string monNum1, string year);
        List<QcNewRewardAndPenaltiesModel> GetNinetyFiveProductionRewardAccountant(string monNum1, string year);
        List<QcNewRewardAndPenaltiesModel> GetNinetyFiveSalesOutRewardAccountant(string monNum1, string year);
        List<QcNewRewardAndPenaltiesModel> GetTotalRewardAndPenalties(string monNum1, string year);
        List<VmIncentivePolicy> GetRewardAndPenaltiesQc(string months, string years);
        List<VmIncentivePolicy> GetRewardAndPenaltiesDeputyAndHead(string monNum, string year, string roles);
        string ForwardFirstVersionIssueToSecondVersion(List<SwQcIssueDetailModel> results);
        List<SwQcIssueDetailModel> QcRecommendedProjectDetails1(string startDate, string endDate);
        SwQcHeadAssignsFromPmModel GetExcelsNames(string projectId, int swVersionNo, long testPhaseIds);
        List<SwQcGlassProtectorTestModel> GetAccessListForExportGlassProtectorAndCharger(string projectName, string allOrLatest, string AccessoriesCategories);
        
        #region Qc Work Progress
        List<SwQcHeadAssignsFromPmModel> GetRunningProjectForSwQcWork();
        List<SwQcHeadAssignsFromPmModel> GetRunningProjectCountForSw();
        List<SwQcHeadAssignsFromPmModel> GetRunningProjectForFtQcWork();
        List<SwQcHeadAssignsFromPmModel> GetRunningProjectCountForFt();
        List<SwQcHeadAssignsFromPmModel> GetNewProjectForSw();
        List<SwQcHeadAssignsFromPmModel> GetNewProjectForSwCount();
        List<SwQcHeadAssignsFromPmModel> GetNewProjectForFt();
        List<SwQcHeadAssignsFromPmModel> GetNewProjectForFtCount();
        List<SwQcHeadAssignsFromPmModel> GetAgentProgress();
        #endregion

        List<PmQcAssignModel> OsRequirementAnalysisData(long userId);
        string UpdateOsRequirementAnalysis(long proId, long swQcAssignIds, long swQcHeadUserIds, string attachment);
        List<PmQcAssignModel> OsRequirementAnalysisDoneData(long userId);
        #region Aftersales Issue Handling
        List<AftersalesPm_IssueVerificationModel> GetAftersalesIssuesForVerification();
        string UpdateIssueVerificationStatus(long ids);
        string SaveDataIntoValidationReportTable(long ids, string modelName, string softwareVersionName, int softVersionNo, string issueDetails, string issueOrRequirement, int noOfMpHsCheck, string foundInGoldenHs, string foundInMpHs, string validationResult, string remarks,string Attachment);
        List<AftersalesPm_ValidationReportModel> GetLogHistory(long issueIdss);
        List<AftersalesPm_ValidationReportModel> ValidationAndRootCauseAnalysisReport(long issueIdss);
        List<AftersalesPm_SupplierFeedBackModel> GetSupplierFeedBackHistory(long issueIdss);
        string SaveQcFeedback(long ids, string remarks, string qcStatus);
        #endregion

      
    }
}
