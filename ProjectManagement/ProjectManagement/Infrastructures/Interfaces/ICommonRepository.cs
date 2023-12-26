using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ProjectManagement.Controllers;
using ProjectManagement.DAL;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;
using ProjectManagement.Models.Common;
using ProjectManagement.Models.StausObjects;
using ProjectManagement.ViewModels.Common;
using ProjectManagement.ViewModels.Hardware;
using ProjectManagement.ViewModels.Management;
using ProjectManagement.ViewModels.ProjectManager;

namespace ProjectManagement.Infrastructures.Interfaces
{
    public interface ICommonRepository
    {
        #region IssuesMethods
        
        #endregion

        List<ProjectMasterModel> GetAllProjects();
        List<ProjectMasterModel> GetAllProjectNames();
        List<ProjectMasterModel> GetAllProjectModels();
        List<ProjectMasterModel> GetOrderNumbersByProjectName(string projectname);
        List<ProjectMasterModel> GetOrderNumbersByProjectModel(string projectModel);
        ProjectMasterModel GetProjectInfoByProjectId(long projectId);
        List<AccessoriesPricesModel> GetAccessoriesPricesByProjectId(long projectId);
        List<SupplierKpiPerformanceModel> GetSupplierKpiPerformanceByProjectId(long? projectId);
        List<ProjectImage> GetProjectImages(long? projectId);
        string GetJigsPriceByProjectName(string projectName);
        ProjectPurchaseOrderFormModel GetProjectPurchaseOrderByProjectId(long projectId);
        string GetProjectAge(string projectName);
        LcApprovalHandsetPriceRangeModel GetPriceRangeByFinalPrice(decimal? finalPrice, string rangeFor);
        List<SalesForecastingReport> GetSalesForecastForRelevantModelByPriceRange(decimal startingRange,
            decimal finishingRange, string projectType);
        MarketPriceModel GetMarketPriceModelByProjectId(long projectId);
        List<CommonIssueModel> GetIssues();
        List<CommonIssueModel> GetIssuesCreatedByUserId(long userid);
        long SaveIssue(CommonIssueModel model);
        
        bool SolveIssue(CommonIssueModel model);
        bool ReferedIssue(CommonIssueModel model);
        bool IgnoreIssue(CommonIssueModel model);

        List<HardwareIssueCustomModel> GetHardwareIssueModels();
        List<CommonParamModel> GetComponents();
        SupplierRatingModel GetSupplierRating(long supplierId, long projectMasterId);
        bool SaveSupplierRating(SupplierRatingModel model);
        bool SaveOpinion(long partialPostProjectId, string opinionText);
        List<OpinionModel> GetOpinionsByProjectId(long projectId);
        FileContentResult GetProfilePicture(long uId);
        ProjectStatusForHwModel GetProjectStatusForHw(long projectMasterId);
        ProjectDetailStatus GetProjectStatus(long id);
        ProjectDetailStatus GetProjectStatusByModules(long id);
        
        CmStatusObject GetCmStatusObject(long id);
        PmStatusObject GetPmStatusObject(long id);
        
        HwScreeningStatusObject GetHwScreeningStatusObject(long id);
        HwRunningStatusObject GetHwRunningStatusObject(long id);
        HwFinishedStatusObject GetHwFinishedStatusObject(long id);
        SwStatusObject GetSwStatusObject(long id);
        List<ProjectMasterModel> GetPostProductionProjects(long userId = 0);
        List<ProjectMasterModel> GetProjectsByName(string projectName);
        bool SavePostProductionIssue(PostProductionIssueModel model);
        List<PostProductionIssueModel> GetPostProductionIssues(long userid, long swqcallprojectissueid);
        IssueCommentsDetailModel SaveIssuePostComment(PostCommentModel model);
        List<PostCommentModel> GetPostCommentById(long swqcallprojectissueid);
        void UpdateCommentForApproval(long swallqcissueId, long postcommentId, long approve);
        List<PostProductionIssueModel> GetPostProductionIssuesByUser(long userId);
        List<SwStatusObject> GetSwStatusObjects(long id);
        List<MajorIssueModel> GetModelNamesHavingIssues();
        List<DiagnosticCodeFromOracleModel> GetDiagnosticCodeFromOracleModels();
        List<HwTestMasterModel> GetHwTestMasterModels(string addedByRole);
        HwTestMasterModel SaveHwTestMaster(HwTestMasterModel model);
        List<HwTestInchargeAssignModel> GetHwTestInchargeAssignModels();
        List<HwTestInchargeAssignModel> GetHwEngineerAssignModels();
        List<VmHwTestDetail> GetHwTestDetail();
        List<CmnUserModel> GetAllEmployee();
        List<ProjectMasterModel> GetOnlyModelName();
        void SaveSampleTracker(SampleTrackerModel model);
        List<SampleTrackerModel> GetSampleTrackingByAddedId(long id);
        List<SampleTrackerModel> GetSampleTrackingByAddedIdAndSentIdAndDateRange(long id, DateTime fromDate, DateTime toDate);
        List<SampleTrackerModel> GetSampleTrackingByEmployeeId(long id);
        List<SampleTrackerModel> GetSampleTrackingByRole(string role);
        SampleTrackerModel GetSampleTrackerById(long id);
        SampleTrackerModel UpdateSampleTracker(SampleTrackerModel model);
        void SaveSampleReturnLog(SampleReturnLogModel model);
        List<ProjectClosePenaltyModel> GetRunningPenaltyModels();
        List<ProjectClosePenaltyModel> GetClosedPenaltyModels();
        List<PrPoViewModel> GetPrPoData(long userId);
        DiscussionModel SaveDiscussion(DiscussionModel model);
        List<DiscussionModel> GetDiscussions();
        List<DiscussionModel> LoadMoreDiscussions(long id);
        List<DiscussionFileUploadModel> GetDiscussionFileUploadModels(List<DiscussionModel> model);
        DiscussionFileUploadModel GetFileUploadModelById(long id);
        List<DiscussionModel> GetDiscussionByMention(string str);
        List<DiscussionModel> GetDiscussionByHashTag(string str);
        void SaveHashtag(HashtagModel model);
        List<HashtagModel> GetHashtagByString(string str);
        string[] GetHashtagByStringToArr(string str);
        int CommentCount();
        List<TopTrendingHashtags> GetTopHashtag();
        void UploadDiscussionFile(DiscussionFileUploadModel model);
        DiscussionReplyModel SaveDiscussionReply(DiscussionReplyModel model);
        List<DiscussionReplyModel> GetDiscussionReplies(List<DiscussionModel> model);
        List<DiscussionReplyModel> GetDiscussionReplyByModels(List<DiscussionModel> model);
        FolderModel SaveFolderModel(FolderModel model);
        List<FolderModel> GetFolderModelsByProjectAndParent(string projectname, long parentfolder);
        List<FolderModel> BrowseBack(string projectname, long folderid);
        List<DocManagementFileUploadModel> BrowseBackFiles(string projectname, long folderid);
        DocManagementFileUploadModel SaveFileUploadModels(DocManagementFileUploadModel model);
        List<DocManagementFileUploadModel> GetFileUploadModels(string projectname, long parentfolder);
        bool DuplicateFileCheck(string projectname, string filename);
        DocManagementFileUploadModel GetFileById(long id);
        List<MkProjectSpecModel> GetSpec(string specname, string type);
        Produced_UnProducedIMEIViewModel GetProduced_UnProducedIMEIs(string modelname, string order);
        List<ProjectMasterInv> GetOrdersfromModel(string modelname);
        ServiceTrendsViewModel GetServiceLog(string modelname);
        List<PerMonthServiceEntryModel> GetPerMonthServiceEntry(String model);
        MajorProblem GetMajorProblemsChartData(string modelname);
        DailySalesInvoicesViewModel DailySalesInvoices(string invoicedate);
        List<ModelColorWiseDailySalesViewModel> GetColorWiseActivatedModelNumber(string pid, string invoicedate);
        List<ModelWIseDailySalesByDealerTypeViewModel> GetModelWiseADealerType(string pid, string invoicedate);
        RemainingMarketStockDealerWiseViewModel GetRemainingStock(string type, string modelname, string invoicedate);
        ChartGraphforDailySalesViewModel GetHighChartGraphforDailySales(string id, string invoicedate);

        NewMajorMinorIssuesViewModel MajorMinorIssuesViewModel(string modelname, string order);
        List<Order> GetOrdersOfModel(String model);
        List<PieChartDataForIssueName> GetMajorIssueChartsByMonthWiseServiceQuantity(string model1, string orders1);
        List<PieChartDataForSpare> GetSpareChartsByMonthWiseServiceQuantity(string model1, string orders1);
        List<HighChartDataListForTotalReceive> GetTotalReceiveData(string model1, string orders1);
        List<HighChartDataListByOrderFromLauncingDate> GetOrderFromLauncingDate(string model1, string orders1);
        List<WSMTHandset> GetWSMTModels();
        List<RBSYProductModel> GetRBSYProductModels();
        ResponseMessage SyncWSMTBoms(WSMTSyncVm vm);
        List<WSMTHandset> GetWSMTHandsets();
        BOMReportVm GetWSMTBomReportData(BOMReportVm vm);
        List<WSMTBomStockDetailsVm> GetAlternateBOMs(long bom_id);
        List<ProjectPurchaseOrderFormModel> GetProjectPurchaseFormData(string columnName);
        bool InsertPurchaseOrderComment(long masterId, string comment);
        List<ProjectEventDates> GetAllProjectEventDates();
        List<ProjectVariantModel> GetProjectVariantModelsByProjectId(long id);
        ProjectVariantModel GetProjectVariantModelById(long id);
        ProjectVariantModel SaveUpdateProjectVariant(ProjectVariantModel model);
        void RemoveProjectVariant(long variantId = 0);

        void SaveLockedVariantToOrderQuantityDetailModel(
            ProjectOrderQuantityDetailModel model);

        List<OrderQuantityDetailsVm> GetOrderQuantityDetailsVms();
        List<OrderQuantityDetailsVm> GetOrderQuantityDetailsVmsByProjectId(long projectId);
        List<ProjectVariantCalculatorModel> GetPreviousOrderVariants(long projectId);
        ProjectVariantCalculatorModel SaveProjectVariantCalculator(ProjectVariantCalculatorModel model);
        List<ProjectVariantCalculatorModel> GetVariantCalculatorByProjectId(long projectId);
        ProjectVariantCalculatorModel GetProjectVariantCalculatorById(long id);
        void UpdateProjectModelInProjectMaster(string projectModel, long projectId);
        void RemoveVariantCalculator(long id);
        List<rbsBarCodeInv> SixMonthsUnproducedAverageQty();
        List<ProjectPoFeedbackModel> GetPoFeedbackByUserId(long userId);
        ProjectPoFeedbackModel SaveUpdatePoFeedBackModel(ProjectPoFeedbackModel model);
        ProjectPoFeedbackModel GetPoFeedbackById(long id);
        List<ProjectPoFeedbackModel> GetPoFeedbackByProjectId(long? projectid);
        List<ProjectPoFeedbackModel> DuplicatePoFeedbackBySamePerson(long? projectid, long? addedby);
        List<ProjectPoFeedbackModel> GetAllProjectPoFeedbackModels();
        List<string> GetRoleDescriptions();
        List<SmtCapacityExceedLogModel> SmtCapacityExceedLogModels();
        List<ProjectOrderQuantityDetailModel> GetProjectOrderQuantityDetailModels();
        void SaveProcessCost(ProcessCostMonthWiseModel model);
        bool DuplicateProcessCostCheckerByVariantName(string variantName);
        List<ProcessCostMonthWiseModel> GetProcessCostMonthWiseModels();
        string MonthNumberToName(int monthno);
        Produced_UnProducedIMEIViewModel GetProductionInformation(string modelname, string order);
        List<string> GetBomDescriptionByIdThenProjectModel(long id);
        List<string> GetSpareDescriptionByDescription(string description);
        FocClaimModel SaveFocClaimModel(FocClaimModel model);
        FocClaimModel UpdateFocClaimModel(FocClaimModel model);
        List<FocClaimModel> GetFocClaimAddedBy(long claimedBy);
        List<FocClaimModel> GetAllFocClaims();
        FocClaimModel GetFocClaimById(long id);
        List<ProjectOrderQuantityDetailModel> GetOrderQuantityDetailByProjectId(long id);
        ProjectOrderQuantityDetailModel GetOrderQuantityDetailById(long id);
        List<ProjectOrderQuantityDetailModel> GetOrderQuantityDetails();
        bool VariantAlreadyExists(string projectModel, long projectId = 0);
        ProjectOrderQuantityDetailModel SaveUpdateProjectVariantInOrderQuantityDetail(ProjectOrderQuantityDetailModel model);
        List<ProjectMaster> GetProjectInfoByProjectModel(string projectModel);
        List<LCOpeningPermission> GeAllApprovedLcPermissions();
        List<LCOpeningPermission> GeAllPipelineLcPermissions();
        List<LCOpeningPermission> GetLcPermissionsByProjectId(long projectId);
        List<LCOpeningPermission> GetLcPermissionByProjectModel(string projectModel);
        List<ProjectLcModel> GetProjectLcByProjectName(string projectName);
        List<ProjectLcModel> GetMonthWiseTotalLcValue();
        List<ProjectLcModel> GetMonthWiseApprovedlLcValue();
        List<ProjectLcModel> GetMonthWiseTotalLcValueFromOracle();
        LcOpeningPermissionModel GetLcOpeningPermissionById(long id);
        SalesForecastingReport GetSalesForecastingReportByVariantName(string model);
        List<ProjectMaster> GetProjectListByProjectName(string projectName);
        List<MasterPoVariantModel> GetMasterPoVariantByProjectName(string projectName);
        List<MasterPoVariantModel> GetRelevantModelByProjectId(long? id);
        LcOpeningPermissionOtherProductModel GetLcOpeningPermissionOtherProductById(long id);
        LcOpeningPermissionModel SaveLcOpeningPermission(LcOpeningPermissionModel model);

        LcOpeningPermissionOtherProductModel SaveLcOpeningPermissionOtherProduct(
            LcOpeningPermissionOtherProductModel model);
        List<LcOpeningPermissionFileModel> GetLcOpeningPermissionFilesByLcId(long id);
        List<LcOpeningPermissionOtherFileModel> GetLcOpeningPermissionOtherFilesByLcId(long id);
        List<String> GetModelListForRelevantModels();
        List<ProjectOrderQuantityDetail> GetVariantsWithOrderNumber();
        List<ServiceToSalesRatio> GetServiceToSalesRatiosBySplitProjectName(string projectModel);
        List<tblActivatedInvoiceValueVsSpareValue> GetTblActivatedInvoiceValueVsSpareValues(string projectModel);
        List<OrderWiseDailyServiceToSalesRatio> GetOrderWiseDailyServiceToSalesRatios(string projectModel);
        void SaveLcPermissionFiles(LcOpeningPermissionFileModel model);
        void DeleteLcFile(long id);
        void SaveLcPermissionOtherFiles(LcOpeningPermissionOtherFileModel model);
        LcOpeningPermissionFileModel GetOpeningPermissionFileById(long id);
        List<ProjectOrderPerformanceSum> GetProjectOrderPerformanceSumByModel(string modelName);
        LcOpeningPermissionOtherFileModel GetOpeningPermissionOtherFileById(long id);
        List<SampleTrackerModel> GetSampleIssueListByIssuerId(long userId);
        List<SampleTrackerModel> GetAllSampleTrackers();
        List<SampleTrackerModel> GetSampleTrackerToReceive();
        List<ProjectMasterModel> GetSwotPendingProjects();
        List<CommonSpecModel> GetSwotAnalysis(long projectId, long multiplier);
        List<tblBarCodeInv> GeTblBarCodeInvByDateRangeAndProductModel(DateTime startDate, DateTime endDate, string productModel);
        List<ProjectOrderQuantityDetailModel> GetAllVariantsWithOrderNumber();
        List<ColorWiseVariantQuantityModel> GetColorWiseVariantQuantityByVariantId(long id);
        ColorWiseVariantQuantityModel SaveColorWiseVariantQuantity(ColorWiseVariantQuantityModel m);
        List<ProjectPmAssignModel> GetPmAssignModels();
        List<ProjectMasterModel> GetRejectedProjectList();
        List<string> GetProductModelsFromProductMaster();

        #region China Qc Inspection Clearance Approval
        List<ChinaQcInspectionsClearanceModel> GetChinaQcInspectionClearanceDetails();
        List<ChinaQcInspectionsClearanceModel> GetChinaQcInspectionClearanceApprovalDetails();
        string SaveChinaShipmentClearance(long ids1, long prIds, string sStatus, string remarks);
        int GetChinaQcInspectionCount(long users);
        #endregion

        List<BomModel> GetBomInfoByItemCode(string itemCode);
        ResponseModel SaveMaterialWastage(WastageFileUpload wastageFileUpload);
        List<MaterialWastageMasterModel> GetMetarialWastageList();
        List<MaterialWastageMasterModel> GetPendingApprovals(int approvalStage);
        WastageFileUpload GetMaterialWastageById(long id);
        ResponseModel RecommendMaterialWastage(long id, bool isRecom, string recomMsg, bool isApproved, string approvedMsg, IPrincipal user, int recommenderType);
        List<MaterialWastageRecommendation> GetRecommendationsByMasterId(long id);
        #region Wpms All Projects Details
        List<WpmsAllProjectDetailsModel> GetAllModels();
        List<WpmsAllProjectDetailsModel> GetProjectOrders(string projectName);
        List<WpmsAllProjectDetailsModel> GetProjectSpec(string projectName, string orders, string ProStatus, string InitialApproval);
        #endregion

        ResponseModel CompleteReport(long id);
        List<WpmsAllProjectDetailsModel> GetAllProStatus();
    }
}
