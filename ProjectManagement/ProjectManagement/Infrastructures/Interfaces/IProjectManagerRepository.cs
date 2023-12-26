using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using ProjectManagement.DAL;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;
using ProjectManagement.Models.AssignModels;
using ProjectManagement.ViewModels.ProjectManager;
using System.Web.Mvc;
using ProjectManagement.ViewModels.Global;


namespace ProjectManagement.Infrastructures.Interfaces
{
    public interface IProjectManagerRepository
    {

        #region get methods
        ProjectMasterModel GetProjectMasterModel(long porjectId);
        List<ProjectMasterModel> GetProjectMasterList();

        List<PmCmnUserModel> GetPmCmnUsers();

        List<CmnUserModel> GetPmCmnUsersForAssign();

        ProjectPmAssignModel GetPmAssignInfo( long currentMasterId);
        PmViewHwTestHybridModel GetPmViewHwTestHybridModelForScreening(long projectMasterId);
        List<PmViewHwTestHybridModel> GetPmViewHwTestHybridModelForRunning(long projectMasterId);
        List<PmViewHwTestHybridModel> GetPmViewHwTestHybridModelForFinished(long projectMasterId);

        PmCmnUserModel GetPmUserInfo(long pmUserId);

        List<ProjectMasterModel> GetProjectMasterModelsByProjectManager(long pmUserId);


        List<ProjectMasterModel> GetAssignedProjectMasterInfo(long projectMasterId);

        PmBootImageAnimationModel GetPmBootImageAnimationModel(long pmBootAnimationId, long userId);

        PmGiftBoxModel GetPmGiftBoxModel(long projectMasterId,long userId);

        PmLabelsModel GetPmLabelsModel(long projectMasterId,long userId);

        PmScreenProtectorModel GetPmScreenProtectorModel(long projectMasterId,long userId);

        PmServiceDocumentsModel GetPmServiceDocumentsModel(long projectMasterId);

        PmSwCustomizationModel GetPmSwCustomizationModel(long projectMasterId);

        PmIdModel GetPmIdModel(long projectMasterId,long userId);

        PmWalpaperModel GetPmWalpaperModel(long projectMasterId,long userId);
        PmPhnAccessoriesModel GetPmPhnAccessoriesModel(long projectMasterId,long userId);

        List<PmSwCustomizationInitialModel> GetPmSwCustomizationInitialModels(long projectMasterId);
        List<PmSwCustomizationFinalModel> GetPmSwCustomizationFinalModels(long projectMasterId,long userId);
        List<SpareAnalysisReportMonitorModel> GetAnalysisReportMonitorModels();

#endregion 

        #region update method
       
        long UpdateBootImageAnimationInfo(PmBootImageAnimationModel pmBootImageAnimationModel);
        long UpdateGBinfo(PmGiftBoxModel pmGiftBoxModel);
        long UpdateLabelInfo(PmLabelsModel pmLabelsModel);
        long UpdateIdInfo(PmIdModel pmIdModel);
        long UpdateScreenProtectorInfo(PmScreenProtectorModel pmScreenProtectorModel);
        long UpdateServiceDocInfo(PmServiceDocumentsModel pmServiceDocumentsModel);
        long UpdateSwCustomizationInfo(PmSwCustomizationModel pmSwCustomizationModel);
        long UpdateWalPaperInfo(PmWalpaperModel pmWalpaperModel);

        #endregion

        #region Save Method
        long SaveWalPaperInfo(PmWalpaperModel pmWalpaperModel);
        long SaveSwCustomizationInfo(VmSoftwareCustomization model);
        long SaveServiceDocInfo(PmServiceDocumentsModel pmServiceDocumentsModel);
        long SaveScreenProtectorInfo(PmScreenProtectorModel pmScreenProtectorModel);
        long SaveIdInfo(PmIdModel pmIdModel);
        long SaveLabelInfo(PmLabelsModel pmLabelsModel);
        long SaveGbInfo(PmGiftBoxModel pmGiftBoxModel);
        long SaveBootImageAnimationInfo(PmBootImageAnimationModel pmBootImageAnimationModel);

        long SaveAccessoriesInfo(PmPhnAccessoriesModel pmPhnAccessoriesModel);

        long SaveCameraInfo(PmPhnCameraModel pmPhnCameraModel);
#endregion

        #region others method
        string AssignProjectToProjectManager(long pMasterId, long pManagerId, string projectHeadRemarks, string purchaseOrderNumber //, string pmAproDate
            );
        void InsertDataInBabt(long pMasterId, long pManagerId);

        List<ProjectMasterModel> GetAssignedProjectList();
        SwQcInchargeAssignModel CheckSwQcInchargeDuplicateAssign(long projectMasterId);

        string AssignProjectPmToSwQcHead(string pmRemarks, long pMasterId, long pMAssignId, long pmUserId, string selectedSampleValue, long sampleNo, long userId, long swWcInchargeAssignUserId, long testPhasefrPm, long swVersionNumber, string versionName);

        #endregion

        List<SwQcTestPhaseModel> GetSwQcTestPhasesForPm();
        string AssignProjectToHardWare(string pmRemarks, long pMasterId, long pMAssignId, long pmUserId, string selectedSampleValue, long sampleNo, long userId, string runningTestValue, string finisTestValue,string poNumber);
        List<SwQcInchargeAssignModel> GetSwQcInchargeAssign(long projectId);
        List<SwQcHeadAssignsFromPmModel> GetSwQcHeadAssignInfoForPm(long projectId);
        //Tuple<List<PmSwCustomizationFinalModel>, bool> GetSoftwareCustomizationDataList(long projectId);
        Tuple<List<PmSwCustomizationFinalModel>, bool> GetSoftwareCustomizationDataList(long projectId);
        string SaveBtrcDocFiles(IEnumerable<VmPmToBtrcNocRequest> attachments);
        List<HwQcInchargeAssignModel> GetHwQcInchargeAssignInfo(long projectId);
        PmAllFilesModel GetAllFilesModel(long projectId);
        long UpdateAccessories(PmPhnAccessoriesModel pmWalpaperModel);
        long UpdateCameraInfo(PmPhnCameraModel pmWalpaperModel);
        string CheckDuplicateAssignToHardware(long pMasterId, string runningTestValue, string finisTestValue);


        long GetUserIdByRoleName(string roleName);
        List<FileShowModel> GetFilesServerPaths(long nocId);
        ProjectBtrcNocModel GetProjectBtrcNoc(long projectId=0, long orderId=0, string imei = null);
        List<ProjectBtrcNocModel> GetBtrcNocByProjectId(long projectId);
        bool UpdateSoftwareCustomization(VmSoftwareCustomization model);
        PmPhnCameraModel GetCameraModel(long projectId, long userId);
        long UpdatePmOtaInfo(PmOtaUpdateModel otaUpdateModel);
        void SubmitSpareAnalysisReport(long id, long userId);
        void ReceiveSpareAnalysisReport(long id, long userId);
        long SavePmOtaUpdateInfo(PmOtaUpdateModel otaUpdateModel);
        HwTestInchargeAssignModel SaveHwTestInchargeAssign(HwTestInchargeAssignModel model);
        List<ProjectMasterModel> GetNewProjectsList();
        List<HwQcAssignCustomMasterModel> GetProjectForHwFgTestByProjectId(long projectMasterId);
        List<HwQcAssignCustomMasterModel> GetProjectForHwScreeningTestByProjectId(long projectMasterId);
        List<HwQcAssignCustomMasterModel> GetProjectForHwRunningTestByProjectId(long projectMasterId);

        #region PM Delete or Newly assigned by PM Incharge
        string PmReassignFromPmIncharge(long pMasterId, string approxPmInchargeToPmFinishDate,
                   string pmInchargeDeleteQcComment, string projectHeadRemarks, string multideleteValue, string multiReassignValue, string multideleteID, string poNumbers);
        #endregion

        #region PM DashBoard

        PmTestCounterModel GetPmTestCounts(long userId);

        #endregion

        #region PMHEAD Report DashBoard

        List<CmnUserModel> GetActivePmList();
        CmnUserModel GetUserInfoByUserId(long userId);

        List<PmReportDashBoardViewModel> GetAllProjectListDetailsForInchargeReport(string startValue, string endValue,
            string emplyCode);

        /////////////Details of Pm Work History////
        List<PmBootImageAnimationModel> GetPmBootImageAnimationModelsDetails(long projectId, string poNumber, string emplyCode);
        List<PmGiftBoxModel> GetPmGiftBoxModelsDetails(long projectId, string poNumber, string emplyCode);
        List<PmLabelsModel> GetPmLabelsModelsDetails(long projectId, string poNumber, string emplyCode);
        List<PmIdModel> GetPmIdModelsDetails(long projectId, string poNumber, string emplyCode);
        List<PmScreenProtectorModel> GetPmScreenProtectorModelsDetails(long projectId, string poNumber, string emplyCode);
        List<PmWalpaperModel> GetPmWalpaperModelsDetails(long projectId, string poNumber, string emplyCode);
        List<PmSwCustomizationFinalModel> GetPmSwCustomizationFinalModelsDetails(long projectId, string poNumber, string emplyCode);
        List<PmPhnAccessoriesModel> GetPmPhnAccessoriesModelsDetails(long projectId, string poNumber, string emplyCode);
        List<PmPhnCameraModel> GetPmPhnCameraModelsDetails(long projectId, string poNumber, string emplyCode);

        /////////////Details of Pm Work History////


        #endregion

        #region Pm Own Report DashBoard
        List<PmReportDashBoardViewModel> GetAllProjectListDetailsForPMReport(string startValue, string endValue, long userId);
        #endregion

        #region Incentive sept 2019 new update
        List<VmPmIncentivePolicy> GetUserPerameterList();
        List<Pm_Incentive_BaseModel> GetPmIncentiveBase();
        List<CmnUserModel> GetPmUserList();
        List<ProjectMasterModel> GetProjectMasterListForPmIncentive(string employeeCode);
       // bool SavePmMonthlyIncentive(VmPmIncentivePolicy model);
        string SavePmMonthlyIncentive(List<Custom_Pm_IncentiveModel> results);
        List<Pm_Po_IncentiveModel> GetPmPoIncentiveForSKD(string employeeCode);
       
        List<Pm_Po_IncentiveModel> GetPmPoIncentiveForPerOrder(string employeeCode);
        string SavePoMonthlyIncentive(List<Custom_Pm_IncentiveModel> results);
        //List<Pm_Shipment_IncentiveModel> GetPmShipmentIncentive(string employeeCode); //Old policy upto Sept 2019 shipment related
        // string SaveShipmentIncentive(List<Custom_Pm_IncentiveModel> results);  //Old policy upto Sept 2019 shipment related
        //List<Custom_Pm_IncentiveModel> GetPmShipIncentive(string empCode, string monNum, string year);//Old policy upto Sept 2019 shipment related
        // List<Pm_Po_IncentiveModel> GetPmPoIncentiveForCBU(string employeeCode);//Old policy upto Sept 2019 shipment related
        // List<Custom_Pm_IncentiveModel> GetPmShipIncentiveForPrint(string empCode, string monNum, string year);//Old policy upto Sept 2019 shipment related
        string SaveOthersIncentive(List<Custom_Pm_IncentiveModel> results);
        List<Pm_Incentive_BaseModel> GetPmInventoryAndMarketIssues();
        List<ProjectMasterModel> GetAllProjectsForPmIncentive();
        string SaveMarketIssueIncentive(List<Custom_Pm_IncentiveModel> results);
        List<Custom_Pm_IncentiveModel> GetPmIncentive(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPmPoIncentive(string empCode, string monNum, string year);

        string SaveTotalIncentive(string totalAmount, string totalPenalties, string empCode, string month, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPmIncentiveForPrint(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPmPoIncentiveForPrint(string empCode, string monNum, string year);
       
        List<Custom_Pm_IncentiveModel> GetPreparedUserName();
        List<Custom_Pm_IncentiveModel> GetTotalFinalIncentiveOfPm(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> PmIncentiveForAllPerson(string Month, string MonNum, string Year);
        bool GetIncentiveTypeData(string employeeCode, int monNum, string year);
        bool GetPoIncentiveData(string employeeCode, int monNum, string year);
        bool GetDocIncentiveData(string employeeCode, int monNum, string year);
        //bool GetShipmentIncentiveData(string employeeCode, int monNum, string year); //Old policy upto Sept 2019
        bool GetTotalIncentiveData(string empCode, int monNum, string year);
        List<ProjectMasterModel> GetAllProjectsForOthers();
        List<Custom_Pm_IncentiveModel> GetAllIncentiveDataOfFourMonths(string empCode, string month1, string month2, string year);
        List<Custom_Pm_IncentiveModel> GetTotalIncentiveForMonthRange(string empCode, string monthNum1, string monthNum2, string yearName);
        //new policy 2019 from sept
        List<NinetyFiveProductionRewardModel> GetProductionReward(string employeeCode, string monthName, string monNum, string year);
        string SaveProductionRewardData(List<NinetyFiveProductionRewardModel> results);
        List<NinetyFiveProductionRewardModel> GetSoldOutRewardData(string employeeCode, string monthName, string monNum, string year);
        string SaveSalesOutRewardData(List<NinetyFiveProductionRewardModel> results);
        List<NinetyFiveProductionRewardModel> GetPmAndQcLsdToVesselData(string employeeCode, string monthName, string monNum, string year);
        string SaveVesselRewardOrPenaltiesData(List<NinetyFiveProductionRewardModel> results);
        bool ProductionRewardDataCheck(string employeeCode, int monNum, string year);
        bool SalesOutRewardDataCheck(string employeeCode, int monNum, string year);
        bool VesselRewardOrPenaltiesDataCheck(string employeeCode, int monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPmHeadPercentage(string employeeCode, string monthName, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPmProductionRewardIncentive(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPmProductionDeductIncentive(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPmSalesOutRewardIncentive(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPmSalesOutDeductIncentive(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPmLsdToVesselRewardIncentive(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPmLsdToVesselPenaltiesIncentive(string empCode, string monNum, string year);
        List<NinetyFiveProductionRewardModel> GetPerPoRewardSumForPmHead(string employeeCode, string monthName, string monNum, string year);
        List<NinetyFiveProductionRewardModel> GetPoDetailsPageForPmHead(string empCode, string month, string monNum, string year);
        string SavePoDetailsPageForPmHead(List<NinetyFiveProductionRewardModel> results);
        bool PoDetailsPageForPmHeadDataCheck(string employeeCode, int monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPmHead_PerPoIncentive(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPmHead_TeamPercentInc(string empCode, string monNum, string year);
        List<NinetyFiveProductionRewardModel> GetPenaltiesSumForPmHead(string employeeCode, string monthName, string monNum, string year);
        List<NinetyFiveProductionRewardModel> GetRatioWisePenaltiesForPmHead(string empCode, string month, string monNum, string year);
        bool PenaltiesDetailsPageForPmHeadCheck(string employeeCode, int monNum, string year);
        string SavePenaltiesDetailsPageForPmHead(List<NinetyFiveProductionRewardModel> results);
        List<Custom_Pm_IncentiveModel> GetSumOfVesselPenaltiesIncentive(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetProductionIncentiveForPrint(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetSalesOutIncentiveForPrint(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetVesselRewardOrPenaltiesForPrint(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPmHeadPerPoIncentiveForPrint(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPmHeadVesselPenaltiesForPrint(string empCode, string monNum, string year);

        //new incentive addition
        List<Pm_IncentiveModel> PmAccessoriesProjectList();
        string SaveAccessoriesProject(List<Pm_IncentiveModel> issueList, string attachment, List<string> projectMasterId, List<string> projectName);
        List<Pm_IncentiveModel> PmFollowUpFocMaterial();
        string SaveFollowUpFocMaterial(List<Pm_IncentiveModel> issueList, string attachment);
        List<Pm_IncentiveModel> PmPoFeedbackAndInfoUpdate();
        string SavePoFeedbackAndInfoUpdate(List<Pm_IncentiveModel> issueList, string attachment);
        List<Pm_IncentiveModel> PmSupplierPenaltiesList();
        string SaveSupplierPenalties(List<Pm_IncentiveModel> issueList, string attachment, List<string> projectMasterId, List<string> projectName);
        List<Pm_IncentiveModel> PmGuidelinesList();
        string SavePmGuidelines(List<Pm_IncentiveModel> issueList, long proId, string attachment);
        List<Pm_IncentiveModel> PmProjectMarketingSpecList();
        string SavePmProjectMarketingSpec(List<Pm_IncentiveModel> issueList, long proId, string attachment);
        List<Pm_IncentiveModel> PmPolicyUpdateList();
        string SavePolicyUpdate(List<Pm_IncentiveModel> issueList, string attachment);
        List<Pm_IncentiveModel> PmSampleHandsetManagementList();
        string SaveSampleHandset(List<Pm_IncentiveModel> issueList, string attachment);

        List<Pm_IncentiveModel> GetAccessoriesProjectIncentive(string employeeCode,string monNum,string year);
        List<Pm_IncentiveModel> GetFollowUpFocMaterialIncentive(string employeeCode, string monNum, string year);
        List<Pm_IncentiveModel> GetPoFeedbackIncentive(string employeeCode, string monNum, string year);
        List<Pm_IncentiveModel> GetSupplierPenaltiesIncentive(string employeeCode, string monNum, string year);
        List<Pm_IncentiveModel> GetPmGuidelinesIncentive(string employeeCode, string monNum, string year);
        List<Pm_IncentiveModel> GetProjectMarketingIncentive(string employeeCode, string monNum, string year);
        List<Pm_IncentiveModel> GetPolicyUpdateIncentive(string employeeCode, string monNum, string year);
        List<Pm_IncentiveModel> GetSampleHandsetIncentive(string employeeCode, string monNum, string year);
        string SaveAllDocumentDetails(List<Custom_Pm_IncentiveModel> results);
        List<Custom_Pm_IncentiveModel> GetPmDocIncentive(string empCode, string monNum, string year);
        List<Pm_IncentiveModel> GetProClosingIncentive(string employeeCode, string monNum, string year);
        bool GetProClosingIncentiveData(string employeeCode, int monNum, long year);
        bool CheckTeamIncentivePercentage(string employeeCode, int monNum, string year);
        string SaveProjectClosingDetails(List<Pm_IncentiveModel> results);
        List<Custom_Pm_IncentiveModel> GetPmProClosingIncentive(string empCode, string monNum, string year);
        List<Pm_IncentiveModel> GetRawMaterialDelayUploadIncentive(string employeeCode, string monNum, string year);
        List<Pm_IncentiveModel> GetShipClearenceVsLsdIncentive(string employeeCode, string monNum, string year);
        bool GetRawUploadIncentiveData(string employeeCode, int monNum, long year);
        string SaveRawUploadDelayDetails(List<Pm_IncentiveModel> results);
        string SaveHeadTeamIncentivePercentage(List<Custom_Pm_IncentiveModel> results);
        bool GetShipmentVsLsdIncentiveData(string employeeCode, int monNum, long year);
        string SaveShipmentClearanceVsLsdDetails(List<Pm_IncentiveModel> results);
        List<Custom_Pm_IncentiveModel> GetPmRawUploadIncentive(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPmShipmentClearanceVsLsdIncentive(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPm_DocumentUploadIncentiveForPrint(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPm_ProjectClosingIncentiveForPrint(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPm_RawMaterialUpDelayPenaltiesForPrint(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPm_ShipmentClearanceVsLsdForPrint(string empCode, string monNum, string year);

        #endregion
        void SaveOrderQuantityWithColorModel(PmOrderQuantityWithColorModel model);
        List<PmOrderQuantityWithColorModel> GetOrderQuantityWithColorModel(long addedby);
        List<PmOrderQuantityWithColorModel> GetOrderWiseTotalCounts(string projectName);
        List<PmOrderQuantityWithColorModel> GetOrderWiseCountsByProject(string projectName);
        List<tblIMEIRecordModel> GetWareHouseQuantity(string projectName);
        List<tblIMEIRecordModel> GetServiceCenterQuantity(string projectName);
        List<PmOrderQuantityWithColorModel> GetColorsList(string color);
        List<SampleTrackerModel> SampleListByProjectName(string project);
        List<SampleTrackerModel> DeptWiseSampleStatus(string roledesc);
        List<SampleTrackerModel> PersonWiseSampleStatus(long id);
        string SaveAccessoriesDetails(List<Custom_Pm_IncentiveModel> results);
        string SavePmPiClosing(Vm_PiClosing model);
        List<Pm_PiClosingModel> GetPreviousPiClosingData(); 
        List<Pm_PiClosingModel> GetPmPiIncentive(string employeeCode,string monthName,string monNum,string year);
        string SavePiDetails(List<Custom_Pm_IncentiveModel> results);
        bool GetPiData(string employeeCode, int monNum, string year);
        bool GetAccessoriesSavedData(string employeeCode, int monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPmAccessoriesFinalIncentive(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPmPiFinalIncentive(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPmAccessoriesIncentiveForPrint(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPmPiIncentiveForPrint(string empCode, string monNum, string year);

        #region New QcAssign Phase
        //string GetProjectMasterModelForPm(long projectId);

        #endregion

        #region PM AknowledgeMent
        //List<PMAcknowledgementModel> GetAllPMAcknowledgeList();
        ProjectAcknowledgementViewModel GetAllProjectByPlanId(long planid);
        bool UpdatePMAcknowledge(ProjectAcknowledgementViewModel vmodel);
        #endregion

        List<SwQcIssueDetailModel> GetSwQcIssueDetailsForPm(string projectId, string swqcInchargeId, string pmAssignId, string testPhaseId, DateTime pmAssignDate);
        List<SwQcIssueDetailModel> GetSwQcCtsMonkeyOrCameraAutomationDataForPm(string projectId, string swqcInchargeId, string pmAssignId, string testPhaseId, DateTime pmAssignDate);
        List<SwQcPersonalUseFindingsIssueDetailModel> GetPersonalUseFindingsForPm(string projectId, string swqcInchargeId, string pmAssignId, string testPhaseId, DateTime pmAssignDate);
        List<ProjectMasterModel> GetProjectListForSwQcHead();
        List<SwQcIssueCategoryModel> GetIssueCategory();
        List<SwQcIssueDetailModel> GetSwQcIssueDetailsForSupplier(string projectName, string moduleName, int projectOrders, int softVersionNo, string testPhases);
        string UpdateSwQcIssueDetailModelForSupplier(SwQcIssueDetailModel supplierUpdate);
        List<SwQcAssignsFromQcHeadModel> GetSwQcsAssignsInfo(string projectName, int projectOrders, int softVersionNo, string testPhases);
        List<SwQcAssignsFromQcHeadModel> GetSwQcHeadToQcAssignInfo(long projectId);
        bool GetSupplierFeedbackData(SwQcIssueDetailModel supplierUpdate);
        List<SwQcHeadAssignsFromPmModel> GetProjectVersionName(string projectId, int swVersionNo, long testPhaseIds);
        SwQcHeadAssignsFromPm GetAllVersionNameForPm(long swVerNo, long proId, long testPhases);
        SwQcHeadAssignsFromPm GetVersionNameForPm(long swVerNo, long proId);
        bool UpdateDbByExcel(string projectName, long softVersion, HttpPostedFileBase excelFile, string testPhaseIds);
        string AssignFieldAccessoriesPmToSwQcHead(string pmRemarks, long pMasterId, long pMAssignId, long pmUserId, long sampleNo, long userId, long swWcInchargeAssignUserId, long testPhasefrPm, long swVersionNumber, string versionName, string accessoriesTest);
        List<SwQcHeadAssignsFromPmModel> GetSwQcAccessoriesAssign(long projectId);
        List<SwQcHeadAssignsFromPmModel> GetSwQcFieldAssignBy(long projectId);
        List<tblBarCodeInv> GetLatestIMEIs(DateTime sdate, DateTime edate, string modelname = "");
        List<SelectListItem> GetModelsFromBarCodeInv(DateTime sdate, DateTime edate);
        ProjectMasterModel GetProjectMasterInfo(long pmid);
        ClientSideResponse SaveBTRCData(DateTime sdate, DateTime edate, List<SelectListItem> models);
        ClientSideResponse SaveBTRCModelInformation(BTRCRegistrationVM vmdata);
        List<SelectListItem> GetModelsFromPMS();
        List<BTRCModel> GetBTRCModels();
        List<BTRCIMEIExportLog> GetBTRCExportLog();
        BTRCModel GetBTRCModel(string projectmodel);
        List<SwQcTestPhaseModel> GetSwQcTestPhaseForSupp();
        List<SwQcTestPhaseModel> GetSwQcTestPhaseForSuppDemo();


        List<ProjectMasterModel> GetAllProjects();
        ProjectMasterModel GetProjectDetails(long projectMasterId);
        List<ProjectMasterModel> AllBOMType();

        List<ProjectMasterModel> GetBomName(long proIds, string bomsTypes, string projectNames);
        string SaveRawMaterialInspection(List<ProjectMasterModel> issueList, List<ProjectMasterModel> issueList1, long proId, string focChk1, string attachment);
        List<AssignProjectsViewModel> GetRawMaterialInspectionList();
        List<AssignProjectsViewModel> GetBomDetails(long rawMaterialId);
        List<AssignProjectsViewModel> GetQcDelayReport(string projectName, string projectType, string startDate, string endDate, string EmployeeCode);
        CmnUserModel GetRoleName(long userId);
        List<ProjectMasterModel> GetProjectMasterModelsByAspm();
        AssignProjectsViewModel GetRawDetails(long proId);
        string UpdateRawMaterialInspection(long proId, string attachment);
        string SaveNewFoc(long rawMatIds, string bomsTypes, string bomName, string bomQuantity, string color, string bomRemarks);
        
        #region Finish Good
        List<FinishGoodVariantModel> GetShipmentDetailsForFinishGood();
        List<FinishGoodVariantModel> GetFinishGoodDetails(long proShipOrder);
        #endregion
        List<PmQcAssignModel> GetPmToQcHeadAssignModels(long userId);
        List<CmnUserModel> GetActiveQc();
        string UpdateInactiveAssignedProjectToQc(long proId, long swQcHeadIds);
        string UpdateQcheadToQcAssignedProjectForInactive(long proId, long swQcHeadIds);
        string AssignOsRequirementToSwQcHead(string pmRemarks, long pMasterId, long pMAssignId, long pmUserId, long userId, long swWcInchargeAssignUserId);
        List<SwQcHeadAssignsFromPmModel> GetOsAssignInfoForPm(long projectId);
        List<PmQcAssignModel> GetQcHeadToQcAssignModels(long userId);

        #region China Qc Inspection Clearance
        List<ChinaQcInspectionsClearanceModel> GetProjectListForChinaQc();
        List<ChinaQcInspectionsClearanceModel> GetProjectOrders(string projectName);
        ChinaQcInspectionsClearanceModel GetProjectOrderQuantity(string projectMasterId);
        List<ChinaQcInspectionsClearanceModel> GetChinaInspectionDetails(string projectMasterId);
        string SaveChinaQcInspectionClearanceDetails(List<ChinaQcInspectionsClearanceModel> issueList);
        List<ChinaQcInspectionsClearanceModel> GetChinaInspectionProjectDetails(string projectMasterId);
        List<ChinaQcInspectionsClearanceModel> GetChinaApprovalLog(string ids);
        ChinaQcInspectionsClearanceModel GetChinaApprovalStatus(string ids);
        string SaveShipmentDeniedData(long id, long projectMasterId,string Remarks);
        string SaveChinaShipmentClearance(long proIds);
        #endregion
      
    }
}
