using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectManagement.Models;


namespace ProjectManagement.Infrastructures.Interfaces
{
    interface IHardwareRepository
    {

        #region Get Methods

        List<HwInchargeIssueModel> GetHwInchargeIssueModels(long hwQcInchargeAssignId);
        HwQcInchargeAssignModel GetHwQcInchargeAssignByAssignId(long id);
        List<HwQcAssignCustomMasterModel> GetHwInchargeReceivableProjects();
        List<ProjectMasterModel> GetAllProjects();
        List<ProjectMasterModel> GetAllProjectDistinctName();
        CmnUserModel GetUserInfoByUserId(long userId);
        List<CmnUserModel> GetUsersForHwQcAssign();
        List<HwIssueMasterModel> GetAllHwIssueMaster();
        List<ProjectMasterModel> GetProjectsAssignedToHwQcInchargeForScreening();
        List<ProjectMasterModel> GetProjectsAssignedToHwQcInchargeForRunning();
        List<HwQcAssignCustomMasterModel> GetProjectsAssignedToHwQcForScreening(long hwQcUserId);
        List<HwQcAssignCustomMasterModel> GetProjectsAssignedToHwQcForScreeningForDashBoard(long hwQcUserId);
        List<HwQcAssignCustomMasterModel> GetProjectsAssignedToHwQcForRunningForDashBoard(long hwQcUserId);
        List<HwQcAssignCustomMasterModel> GetProjectsAssignedToHwQcForRunning(long hwQcUserId);
        List<HwQcAssignCustomMasterModel> GetProjectsAssignedToHwQcForFinishedGoods(long hwQcUserId);
        ProjectMasterModel GetProjectInfoByProjectId(long projectId);
        ProjectMasterModel GetProjectInfoByHwQcAssignId(long? hwQcAssignId);
        ProjectMasterModel GetProjectInfoByHwQcInchargeAssignId(long? hwQcInchargeAssignId);
        HwQcInchargeAssignModel GetTestPhaseByHwQcAssignId(long hwQcAssignId);
        HwQcAssignModel GetHwQcInchargeAssignIdForScreening(long projectId);
        HwQcAssignModel GetHwQcInchargeAssignIdForRunning(long projectId);
        List<HwGetQcAssignedByInchargeModel> GetQcAssignedByInchargeAssignIdForScreening(long hwQcInchargeAssignId,
            int testStage);

        List<HwGetQcAssignedByInchargeModel> GetQcAssignedByInchargeAssignIdForRunning(long hwQcInchargeAssignId,
            int testStageRunning);

        long GetHwQcAssignIdForAllTestByProject(long projectId, long hwQcUserId, long hwQcInchargeAssignId);
        HwQcTestCounterModel GetHwQcInchargeTestCounts(long hwQcInchargeUserId);
        HwQcTestCounterModel GetHwQcTestCounts(long hwUserIdWhoLoggedIn);
        List<HwIssueCommentModel> GetIssueCommentsByQcAssignId(long hwQcAssignId);
        List<HwQcAssignCustomMasterModel> GetScreeningTestProjectStatusForInchargeDashboard();
        List<HwQcAssignCustomMasterModel> GetHwScreeningCompleteProjects();
        List<HwQcAssignCustomMasterModel> GetHwRunningCompleteProjects();
        List<HwQcAssignCustomMasterModel> GetHwFinishedCompleteProjects();
        List<HwQcAssignCustomMasterModel> GetRunningTestProjectStatusForInchargeDashboard();
        List<HwQcAssignCustomMasterModel> GetFinishedGoodsTestProjectStatusForInchargeDashboard();
        HwQcAssignCustomMasterModel GetHwQcAssignDetailForVerifyByQcAssignId(long hwQcInchargeAssignId);
        List<HwQcAssignCustomMasterModel> GetHwQcInchargeProjectsForScreeningForward();
        List<HwQcAssignCustomMasterModel> GetHwQcInchargeProjectsForRunningForward();
        List<HwQcAssignCustomMasterModel> GetHwQcInchargeProjectsForFinishedGoodsForward();
        List<HwQcAssignCustomMasterModel> GetQcPassedListByInchargeIdForForward(long hwQcInchargeAssignId);
        string GetQcUploadedDocument(long hwQcInchargeAssignId);
        int GetVerificationPendingCounts();
        int GetScreeningForwardCounter();
        int GetRunningForwardCounter();
        int GetFinishedGoodsForwardCounter();
        List<HwQcAssignCustomMasterModel> GetHwQcScreeningVerificationPending(long whologgedin);
        List<HwQcAssignCustomMasterModel> GetHwQcRunningVerificationPending(long whologgedin);
        List<HwQcAssignCustomMasterModel> GetHwQcFinishedGoodsVerificationPending(long whologgedin);
        HwTestPcbModel GetHwTestPcb(long hwqcassignId);
        HwTestPcbAModel GetHwTestPcbA(long hwqcassignId);
        HwTestCameraInfoModel GetHwTestCameraInfo(long hwqcassignId);
        HwTestTpLcdInfoModel GetHwTestTpLcdInfo(long hwQcInchargeAssignId);
        HwTestSoundInfoModel GetHwTestSoundInfo(long hwQcInchargeAssignId);
        HwTestFPCandSIMSlotInfoModel GetHwTestFpCandSimSlotInfo(long hwQcInchargeAssignId);
        HwTestBatteryInfoModel GetHwTestBatteryInfo(long hwQcInchargeAssignId);
        HwTestChargerInfoModel GetHwTestChargerInfo(long hwQcInchargeAssignId);
        HwTestEarphoneInterfaceInfoModel GetHwTestEarphoneInterfaceInfoModel(long hwQcInchargeAssignId);
        HwTestChargingInfoModel GetHwTestChargingInfoModel(long hwQcInchargeAssignId);
        HwProjectMasterCustomModel GetProjectAndAssignDetailByHwQcInchargeAssignId(long hwQcInchargeAssignId);
        List<HwChipsetModel> GetAllHwChipsetModel();
        List<HwFlashIcModel> GetAllFlashIcModel();
        List<HwRfModel> GetAllRfModel();
        List<HwPmu1IcModel> GetAllPmu1IcModel();
        List<HwFrontCameraIcModel> GetAllHwFrontCameraIcModel();
        List<HwBackCameraIcModel> GetAllHwBackCameraIcModel();

        //finished goods test Get Methods start
        List<HwQcAssignCustomMasterModel> GetProjectsAssignedToHwQcForFinishedGoodsForDashBoard(long hwQcUserId);
        List<ProjectMasterModel> GetProjectsAssignedToHwQcInchargeForFinishedGoods();
        HwQcAssignModel GetHwQcInchargeAssignIdForFinishedGoods(long projectId);

        List<HwGetQcAssignedByInchargeModel> GetQcAssignedByInchargeAssignIdForFinishedGoods(long hwQcInchargeAssignId,
            int testStageRunning);

        HwFgBatteryTestMasterModel GetHwFgBatteryTestMasterModel(long hwQcInchargeAssignId);
        BatteryTestResultSummaryModel GetBatteryTestResultSummaryModel(long hwQcInchargeAssignId);
        HwFgBatteryTestConditionModel GetHwFgBatteryTestConditionModel(long hwFgBatteryTestMasterId);
        List<HwFgBatteryTestConditionModel> GetHwFgBatteryTestConditionModelList(long hwFgBatteryTestMasterId);
        List<HwFgBatteryTestResultModel> GetHwFgBatteryTestResultModelList(long hwFgBatteryTestConditionId);
        HwFgChargerTestModel GetHwFgChargerTestModel(long hwQcInchargeAssignId);
        HwFgUsbCableTestModel GetHwFgUsbCableTestModel(long? hwQcInchargeAssignId);
        List<HwFgUsbTestDetailModel> GetHwFgUsbTestDetailModelList(long hwFgUsbCableTestId);
        CmnUserModel GetUserInfoByHwQcInchargeAssignedBy(long hwQcInchargeAssignedBy);
        HwTestUSBCableInfoModel GetHwTestUSBCableInfo(long hwQcInchargeAssignId);
        HwTestHousingInfoModel GetHwTestHousingInfoModel(long hwQcInchargeAssignId);
        HwTestCrossMatchInfoModel GetHwTestCrossMatchInfoModel(long hwQcInchargeAssignId);
        HwTestOverallResultModel GetHwTestOverallResultModel(long hwQcInchargeAssignId);
        HwQcAssignCustomMasterModel GetReportInitialInfo(long hwQcInchargeAssignId);
        List<CmnUserModel> GetHwTestedBy(long hwQcInchargeAssignId);
        CmnUserModel GetHwTestCheckedBy(long hwQcInchargeAssignId);
        List<ProjectMasterModel> GetProjectListByItemNameForChipset(string icNoSize, long hwQcInchargeAssignId);
        List<ProjectMasterModel> GetProjectListByItemNameForFlashIc(string icNoSize, long hwQcInchargeAssignId);
        List<ProjectMasterModel> GetProjectListByItemNameForPmu1Ic(string icNoSize, long hwQcInchargeAssignId);
        List<ProjectMasterModel> GetProjectListByItemNameForRfIc(string icNoSize, long hwQcInchargeAssignId);
        List<ProjectMasterModel> GetProjectListByItemNameForBackCamera(string icNoSize, long hwQcInchargeAssignId);
        List<ProjectMasterModel> GetProjectListByItemNameForFrontCamera(string icNoSize, long hwQcInchargeAssignId);
        HwBatteryTestCustomModel GetHwBatteryTestCustomModel(long hwQcInchargeAssignId);
        List<HwFgBatteryTestResultModel> GetHwFgBatteryTestResultByHwQcInchargeAssignId(long hwQcInchargeAssignId);
        BatteryTestResultSummaryModel GetBatteryTestResultSummaryModelByHwQcInchargeId(long hwQcInchargeAssignId);
        CmnUserModel GetProjectManagerInfoByProjectid(long projectId);
        List<HwQcAssignCustomMasterModel> GetAllDocs(long hwQcInchargeAssignId);
        List<HwItemComponentModel> GetHwItemComponentModels();
        List<HwIcComponentNumberModel> GetHwIcComponentNumberModels(long hwItemComponentId);
        List<GetHwItemizationModel> GetHwItemizationModels(long hwqcinchargeassignId);
        GetHwItemizationModel GetLatestHwItemizationModel();
        HwFieldTestMasterModel GetHwFieldTestMasterModel(long hwQcinchargeAssignId);
        List<HwFieldTestModel> GetAllHwFieldTestModelByFieldTestMasterId(long fieldTestMasterId);
        List<CmnUserModel> GetHwEnginnersForAssign();
        List<HwEngineerAssignModel> GetHwEngineerAssignModels(long assignId);
        List<HwTestFileUploadModel> GetHwTestFileUploadModels(long hwinchargeassignId);
        List<HwTestFileUploadModel> GetFileByHwEngAssignId(long id);
        List<HwTestAdditionalInfoModel> GetHwTestAdditionalInfoModels(long hwinchargeassignId);
        HwTestFileUploadModel GetHwTestFileUploadModel(long fileuploadId);

        #endregion


        #region Set Methods
        long SaveHwQcAssign(HwQcAssignModel model, long[] assignIds);
        long SaveHwIssueComment(HwIssueCommentModel model);
        void SavePcbMaterial(HwTestPcbModel model);
        void SavePcbaComponentInfo(HwTestPcbAModel model);
        void SaveHwTestCameraInfo(HwTestCameraInfoModel model);
        void SaveHwTestTpLcdInfo(HwTestTpLcdInfoModel model);
        void SaveHwTestSoundInfo(HwTestSoundInfoModel model);
        void SaveHwTestFPCandSIMSlotInfo(HwTestFPCandSIMSlotInfoModel model);
        void SaveHwTestBatteryInfo(HwTestBatteryInfoModel model);
        void SaveHwTestChargerInfo(HwTestChargerInfoModel model);
        void SaveHwTestEarphoneInterfaceInfo(HwTestEarphoneInterfaceInfoModel model);
        void SaveHwFgBatteryTestMaster(HwFgBatteryTestMasterModel model);
        void SaveBatteryTestResultSummary(BatteryTestResultSummaryModel model);
        void SaveHwFgBatteryTestCondition(HwFgBatteryTestConditionModel model);
        void SaveHwFgBatteryTestResult(HwFgBatteryTestResultModel model);
        void SaveHwFgChargerTest(HwFgChargerTestModel model);
        void SaveHwFgChargerDetailTest(HwFgChargerDetailModel model);
        List<HwFgChargerDetailModel> GetHwFgChargerDetailModel(long hwFgChargerTestId);
        void SaveHwFgUsbCableTest(HwFgUsbCableTestModel model);
        void SaveHwFgUsbCableDetail(HwFgUsbTestDetailModel model);
        void SaveHwInchargeIssues(HwInchargeIssueModel model);
        void SaveHwItemizationModel(HwItemizationModel model);
        void SaveItemComponentModel(HwItemComponentModel model);
        void SaveIcComponentNumberModel(HwIcComponentNumberModel model);
        void SaveHwFieldTestMasterModel(HwFieldTestMasterModel model);
        void SaveHwFieldTest(HwFieldTestModel model);
        void SaveHwTestUSBCableInfo(HwTestUSBCableInfoModel model);
        void SaveHwTestChargingInfo(HwTestChargingInfoModel model);
        void SaveHwTestHousingInfo(HwTestHousingInfoModel model);
        void SaveHwTestCrossMatchInfo(HwTestCrossMatchInfoModel model);
        void SaveHwTestOverallResult(HwTestOverallResultModel model);

        HwChipsetModel SaveHwChipsetIc(string chipsetVendor, string icNoSize, string chipsetCore, string chipsetSpeed,
            string pinType,
            int pinNumber, string newitemno, string itemcode, string remarks, long userId);

        HwFlashIcModel SaveHwFlashIcModel(string flashIcBall, string flashIcRam, string flashIcRom,
            string flashIcTechnology, string flashIcVendor, string icNoSize, int pinNumber, string pinType,
            string remarks, long userId);

        HwRfModel SaveHwRfModel(string icNoSize, string rfVendor, int pinNumber, string pinType, string remarks, long userId);
        void NotificationForProjectsReadyToForward(long hwQcInchargeAssignId, long userId);

        HwPmu1IcModel SaveHwPmu1IcModel(string icNoSize, string Pmu_1_Vendor, int pinNumber, string pinType,
            string newitemno,string itemcode,string remarks, long userId);

        HwFrontCameraIcModel SaveFrontCameraIcModel(string icNoSize, string vendor, int pinNumber, string pinType,
            string remarks, long userId);

        HwBackCameraIcModel SaveBackCameraIcModel(string icNoSize, string vendor, int pinNumber, string pinType,
            string remarks, long userId);

        HwEngineerAssignModel SaveHwEngineerAssign(HwEngineerAssignModel model);
        void SaveHwTestFileUploadModel(HwTestFileUploadModel model);
        HwEngineerAssignModel SubmitHwTest(long hwengineerassignId, long hwinchargeassignId, string result, string remarks, long userId);
        void SaveHwAdditionalInfo(HwTestAdditionalInfoModel model);

        #endregion

        #region Update Methods

        void UpdateHwQcIncharge(long hwQcInchargeAssignId, long? receivedSampleQuantity, string receiveSampleRemark);
        HwQcInchargeAssignModel UpdateHwQcInchargeProjectStatus(long hwQcInchargeAssignId,string remark, string status);

        HwQcAssignModel UpdateHwQcDocUploadPath(string hwQcDocUploadPath, long hwQcInchargeAssignId);
        HwTestPcbModel UpdateHwTestPcbDocUploadPath(string hwQcDocUploadPath, long? hwQcInchargeAssignId);
        HwTestPcbAModel UpdateHwTestPcbADocUploadPath(string hwQcDocUploadPath, long? hwQcInchargeAssignId);
        HwTestCameraInfoModel UpdateHwTestCameraInfoDocUploadPath(string hwQcDocUploadPath, long? hwQcAssignId);
        HwTestTpLcdInfoModel UpdateHwTestTpLcdInfoDocUploadPath(string hwQcDocUploadPath, long? hwQcInchargeAssignId);
        HwTestSoundInfoModel UpdateHwTestSoundInfoDocUploadPath(string hwQcDocUploadPath, long? hwQcInchargeAssignId);

        HwTestFPCandSIMSlotInfoModel UpdateHwTestFPCandSIMSlotInfoDocUploadPath(string hwQcDocUploadPath,
            long? hwQcInchargeAssignId);

        HwTestBatteryInfoModel UpdateHwTestBatteryInfoDocUploadPath(string hwQcDocUploadPath, long? hwQcInchargeAssignId);
        HwTestChargerInfoModel UpdateHwTestChargerInfoDocUploadPath(string hwQcDocUploadPath, long? hwQcInchargeAssignId);
        HwTestUSBCableInfoModel UpdateHwTestUSBCableInfoDocUploadPath(string hwQcDocUploadPath, long? hwQcAssignId);

        HwTestEarphoneInterfaceInfoModel UpdateHwTestEarphoneInterfaceInfoDocUploadPath(string hwQcDocUploadPath,
            long? hwQcInchargeAssignId);

        HwTestChargingInfoModel UpdateHwTestChargingInfoDocUploadPath(string hwQcDocUploadPath, long? hwQcInchargeAssignId);
        HwTestHousingInfoModel UpdateHwTestHousingInfoDocUploadPath(string hwQcDocUploadPath, long? hwQcInchargeAssignId);
        HwTestCrossMatchInfoModel UpdateHwTestCrossMatchInfoDocUploadPath(string hwQcDocUploadPath, long? hwQcAssignId);
        HwTestOverallResultModel UpdateHwTestOverallResultDocUploadPath(string hwQcDocUploadPath, long? hwQcAssignId);
        long UpdateHwQcAssignStatusForQC(long hwQcInchargeAssignId,string status);
        void UpdateHwQcAssignStatusForQConForwardProject(long hwQcInchargeAssignId,long userId, string status);
        long UpdateIssueCommentByQcVerifier(string verifierComment, string issueStatus, long hwIssueCommentId,long verifiedBy);
        long UpdateQcAssignStatByVerifier(long hwQcInchargeAssignId, long verifiedBy, string status,string verifierName);

        void UpdateHwInchargeTestPhaseAfterAllQcDone(long hwQcInchargeAssignId,  string status);

        void UpdateProjectMasterScreenTestCompleteStatus(long projectId);

        void UpdateHwTestPcbMaterial(long? hwqcassignId, string thickness, string materials, string recommendation,
            string comment, long? updated);

        void UpdateHwTestPcbA(HwTestPcbAModel model);
        void UpdateHwTestCameraInfo(HwTestCameraInfoModel model);
        void UpdateHwTestTpLcdInfo(HwTestTpLcdInfoModel model);
        void UpdateHwTestSoundInfo(HwTestSoundInfoModel model);
        void UpdateHwTestFPCandSIMSlotInfo(HwTestFPCandSIMSlotInfoModel model);
        void UpdateHwTestBatteryInfo(HwTestBatteryInfoModel model);
        void UpdateHwTestChargerInfo(HwTestChargerInfoModel model);
        void UpdateHwTestUSBCableInfo(HwTestUSBCableInfoModel model);
        void UpdateHwFgBatteryTestMaster(HwFgBatteryTestMasterModel model);
        void UpdateBatteryTestResultSummary(BatteryTestResultSummaryModel model);
        void UpdateHwFgBatteryTestCondition(HwFgBatteryTestConditionModel model);
        void UpdateHwFgChargerTest(HwFgChargerTestModel model);
        void UpdateHwFgUsbCableTest(HwFgUsbCableTestModel model);
        void UpdateHwTestEarphoneInterfaceInfo(HwTestEarphoneInterfaceInfoModel model);
        void UpdateHwTestChargingInfo(HwTestChargingInfoModel model);
        void UpdateHwTestHousingInfo(HwTestHousingInfoModel model);
        void UpdateHwTestCrossMatchInfo(HwTestCrossMatchInfoModel model);
        void UpdateHwTestOverallResult(HwTestOverallResultModel model);
        void UpdateHwFieldTestMaster(HwFieldTestMasterModel model);

        HwChipsetModel UpdateHwChipset(long chipsetId, string chipsetVendor, string icNoSize, string chipsetCore,
            string chipsetSpeed, string pinType, int pinNumber, string remarks, long userId);

        HwFlashIcModel UpdateHwFlashIcModel(long flashIcId, string flashIcBall, string flashIcRam, string flashIcRom,
            string flashIcTechnology, string flashIcVendor, string icNoSize, int pinNumber, string pinType,
            string remarks, long userId);

        void UpdateHwTestInchargeAssign(string remarks, long hwinchargeassignId = 0, long userId = 0);

        #endregion

        #region DUPLICATE
        int CheckDuplicateHwQcAssign(long hwQcUserId, long hwQcInchargeAssignId);
        HwTestCustomModel CheckDuplicateHwTest(long? hwqcassignId);
        #endregion

        #region DELETE

        void DeleteHwFgBatteryTestCondition(long hwFgTestConditionId);

        #endregion

        HwSelfTestModel SaveHwSelfTestModel(HwSelfTestModel model);
        List<HwEngineerAssignModel> GetHwSelfTests(long addedby);
        HwEngineerAssignModel SaveEngineerAssignModelForSelfTest(HwEngineerAssignModel model);
    }
}
