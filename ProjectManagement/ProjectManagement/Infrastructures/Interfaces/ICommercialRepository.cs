using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using ProjectManagement.DAL;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Repositories;
using ProjectManagement.Models;
using ProjectManagement.ViewModels.AftersalesPm;
using ProjectManagement.ViewModels.Commercial;
using ProjectManagement.ViewModels.Common;
using Incentive = ProjectManagement.ViewModels.Commercial.Incentive;

namespace ProjectManagement.Infrastructures.Interfaces
{
    public interface ICommercialRepository
    {
        #region Save Methods
        string SaveOpeningProject(ProjectMasterModel projectMasterModel, long userId);
        long SavePrice(ProjectPriceModel model, long userId);
        long SavePhPcbaInfo(PhPcbaInfoModel model, long userId);
        long SavePhAccessory(PhAccessoryModel model, long userId);
        long SavePhCamInfo(PhCamInfoModel model, long userId);
        long SavePhChipsetInfo(PhChipsetInfoModel model, long userId);
        long SavePhHousingInfo(PhHousingInfoModel model, long userId);
        long SavePhMemoryInfo(PhMemoryInfoModel model, long userId);
        long SavePhNetworkFreqAndBand(PhNetworkFreqAndBandModel model, long userId);
        long SavePhSensorAndOther(PhSensorAndOtherModel model, long userId);
        long SavePhOperatingSyModel(PhOperatingSyModel model, long userId);
        long SavePhBatteryInfoModel(PhBatteryInfoModel model, long userId);
        long SavePhColorInfoModel(PhColorInfoModel model, long userId);
        long SavePhTpLcdInfo(PhTpLcdInfoModel model, long userId);
        long SaveCriticalControlPoint(ProjectCriticalControlPointModel model, long userId);
        long SaveProjectProformaInvoice(ProjectProformaInvoiceModel model, long userId);
        long SaveProjectOrder(ProjectOrderModel model, long userId);
        long SaveProjectShipment(ProjectOrderShipmentModel model, long userId, List<ProjectMasterModel> issueList1);
        long SaveProjectBtrcNoc(ProjectBtrcNocModel projectBtrcNocModel, long userId);
        long SaveProjectLc(ProjectLcModel model, long userId);
        #endregion

        #region Update Methods
        long UpdatePhPcbaInfo(PhPcbaInfoModel model, long userId);
        long UpdatePhAccessory(PhAccessoryModel model, long userId);
        long UpdatePhCamInfo(PhCamInfoModel model, long userId);
        long UpdatePhChipsetInfo(PhChipsetInfoModel model, long userId);
        long UpdatePhHousingInfo(PhHousingInfoModel model, long userId);
        long UpdatePhMemoryInfo(PhMemoryInfoModel model, long userId);
        long UpdatePhNetworkFreqAndBand(PhNetworkFreqAndBandModel model, long userId);
        long UpdatePhSensorAndOther(PhSensorAndOtherModel model, long userId);
        long UpdatePhOperatingSyModel(PhOperatingSyModel model, long userId);
        long UpdatePhBatteryInfoModel(PhBatteryInfoModel model, long userId);
        long UpdatePhColorInfoModel(PhColorInfoModel model, long userId);
        long UpdatePhTpLcdInfo(PhTpLcdInfoModel model, long userId);
        long UpdateProjectCriticalControlPointModel(ProjectCriticalControlPointModel model, long userId);
        long UpdateProjectProformaInvoice(ProjectProformaInvoiceModel model, long userId);
        long UpdateProjectOrder(ProjectOrderModel model, long userId);
        long UpdatePrice(ProjectPriceModel model, long userId);
        long UpdateProjectMaster(ProjectMasterModel model, long userId);
        long UpdateProjectShipment(ProjectOrderShipmentModel model, long userId, List<ProjectMasterModel> issueList1);
        long UpdateProjectBtrcNoc(ProjectBtrcNocModel projectBtrcNocModel, bool isFileNull, long userId);
        void UpdatePoNoFromLc(string pono, long projectOrderId, bool lc1, bool lc2);
        long UpdateProjectLc(ProjectLcModel model, long userId);
        #endregion

        #region Get Methods
        //VmSpecification GetAllProjectSpecification(long projectId);
        List<ProjectMasterModel> GetAllProjects();
        List<ProjectMasterModel> GetAllProjectWithOrderNumber();
        ProjectMasterModel GetProjectMasterModel(long projectId);
        ProjectMasterModel GetProjectMasterModelForPm(long projectId);
        PhAccessoryModel GetPhAccessoryModel(long phAccessoryId = 0, long projectId = 0);
        PhCamInfoModel GetPhCamInfoModel(long camInfoId = 0, long projectId = 0);
        PhChipsetInfoModel GetPhChipsetInfoModel(long phChipsetInfoId = 0, long projectId = 0);
        PhHousingInfoModel GetPhHousingInfoModel(long phHousingInfoId = 0, long projectId = 0);
        PhMemoryInfoModel GetPhMemoryInfoModel(long phMemoryInfoId = 0, long projectId = 0);
        PhNetworkFreqAndBandModel GetPhNetworkFreqAndBandModel(long phNetworkFreqId = 0, long projectId = 0);
        PhPcbaInfoModel GetPhPcbaInfoModel(long phPcbaInfoId = 0, long projectId = 0);
        PhSensorAndOtherModel GetPhSensorAndOtherModel(long phSensorId = 0, long projectId = 0);
        PhTpLcdInfoModel GetPhTpLcdInfoModel(long phTpLcdInfoId = 0, long projectId = 0);
        PhOperatingSyModel GetPhOperatingSyModel(long phOsId = 0, long projectId = 0);
        PhBatteryInfoModel GetPhBatteryInfoModel(long phBatteryInfoId = 0, long projectId = 0);
        PhColorInfoModel GetPhColorInfoModel(long phColorInfoId = 0, long projectId = 0);
        ProjectCriticalControlPointModel GetProjectCriticalControlPointModel(long projectCriticalControlPointId = 0, long projectId = 0);
        ProjectProformaInvoiceModel GetProjectProformaInvoiceModel(long projectProformaInvoiceId = 0, long projectId = 0);
        ProjectOrderModel GetProjectOrderModel(long projectOrderId = 0, long projectId = 0);
        ProjectPriceModel GetProjectPrice(long priceId = 0);
        ProjectOrderShipmentModel GetProjectOrderShipment(long projectOrderShipmentId = 0, long projectId = 0);
        ProjectBtrcNocModel GetProjectBtrcNoc(long projectBtrcNocId = 0, long projectId = 0);
        ProjectLcModel GetProjectLc(long lcId = 0, long projectId = 0);
        List<Brand> GetBrands();
        List<ProjectPurchaseOrderFormModel> GetProjectOrderModels(long projectId = 0);
        List<ProjectLcModel> GetProjectLcModels();
        List<ProjectLcModel> GetProjectLcsByDateRange(DateTime from, DateTime to);
        List<ProjectOrderShipmentModel> GetShipmentModels(long addedById);
        List<ProjectOrderShipmentModel> GetClosedShipmentModels(long addedById);

        List<Accessory> GetAllAccessories();


        #endregion

        #region warehouse
        bool GetWarehouseDetail(int orderNumber, long projectMasterId, string projectName, long projectOrderShipmentId, long projectPurchaseOrderFormId, string purchaseOrderNumber, long quantity, DateTime shipmentDate, DateTime warehouseDate, long warehouseQuantity);
        List<ProjectPurchaseOrderFormModel> GetPurchaseOrders(long proId);
        List<ProjectOrderShipmentModel> GetShipments(long proIds, string purchaseOrderNo);

        List<ProjectPurchaseOrderFormModel> GetShipmentQuantity(long proIds, string purchaseOrderNo,
            string shipmentDate);
        string SaveWarehouseDetails(List<Custom_Warehouse_Details> results);
        //  string GetShipmentTotalQuantity(long projectMasterId, string purchaseOrderNumber, DateTime shipmentDate, long quantity);

        List<Custom_Warehouse_Details> GetShipmentTotalQuantity(long projectMasterId, string purchaseOrderNumber, DateTime shipmentDate, long warehouseQuantity);
        #endregion

        ProjectLc CloseLc(long id);
        ProjectOrderShipment CloseShipment(long id);
        void DeleteShipment(long id);
        ProjectBabtModel GetProjectBabtModel(long id);
        List<ProjectBabtModel> GetAllBabt();
        bool UpdateBabtWithTac(ProjectBabtModel model);
        List<ProjectBtrcNocModel> GetBtrcNocRequestList();
        List<ProjectBrtcNocModel> GetProjectsForBtrcNoc();
        bool SaveBtrcNocs(VmBtrcNoc model);
        long SaveProjectPurchaseOrderFormModel(VmProjectPurchaseOrder model);
        bool SaveProjectPurchaseOrderHandsetModel(List<ProjectPurchaseOrderHandsetModel> models);
        bool SaveProjectPurchaseOrderConditionModel(List<ProjectPurchaseOrderConditionModel> models);
        List<ProjectPurchaseOrderConditionModel> GetPredefinedPurhcaseOrderConditions();
        List<ProjectPurchaseOrderFormModel> GetUnclosedPoList(long projectId = 0);
        List<ProjectPurchaseOrderFormModel> GetAllPoList();
        ProjectPurchaseOrderFormModel GetPurchaseOrderById(long id);
        ProjectPurchaseOrderForm GetPurchaseOrderByIdAsNoTracking(long id);
        List<ProjectPurchaseOrderConditionModel> GetPurchaseOrderConditionsByOrder(long orderId);
        void SaveProjectPurchaseOrderConditionLogs(long orderId, long logAddedBy);
        bool UpdateProjectPurchaseOrderFormModel(ProjectPurchaseOrderFormModel projectPurchaseOrderFormModel, DateTime? appxProjectFinishDate);
        void SaveProjectPurchaseOrderFormLog(ProjectPurchaseOrderForm model, long logAddedBy);
        bool UpdateProjectPurchaseOrderConditionModel(long formId, List<ProjectPurchaseOrderConditionModel> projectPurchaseOrderConditionModels);
        bool SaveSupplier(SupplierModel model);
        SupplierModel GetSupplier(long id);
        List<SupplierModel> GeTAllSuppliers();
        bool UpdateSupplier(SupplierModel model, long userId);
        List<ProjectMasterModel> GetAllCreatedProjects();
        long SaveBabt(BabtRawModel model);
        BabtRawModel GetBabt(long id);
        List<BabtRawModel> GetBabts();
        List<VmCompletedNoc> GetCompletedNocs();
        VmImeiRange GetCustomImeiRange(long projectId, long orderId, long quantity);
        long UpdateBabt(BabtRawModel model);
        bool SaveImeiRange(VmImeiRange model);
        List<ProjectMasterModel> GetProjectBySupplierId(long supplierId = 0);
        long ScreeningRequest(long projectId, long quantity, string sampleType, string remarks);
        List<string> GetCpuCores();
        string CheckProjectName(string projectName);
        ProjectMasterModel GetProjectModel(long? projectId);
        decimal UpdateProject(ProjectMasterModel model, long userId);
        long SaveProject(ProjectMasterModel model, long userId);
        long GetProjectId(string projectName);
        List<ProjectMasterModel> GetProjectsForPurchaseOrder();
        List<HwInchargeIssueModel> GetScreeningIssues(long id);
        int SaveScreeningIssues(VmScreeningIssues model);
        List<ProjectMasterModel> GetAllProjectsForStatus();
        void ScreeningIssueNotification(long projectMasterId, long userId);
        SelectListItem ClosePurchaseOrder(DateTime marketClearanceDate, long proOrdersId, long proIds, string BdIqcResult);
        List<BtrcRawListModel> GetBtrcRowModels();

        List<VmIncentivePolicy> GetCmUserList1(long monIds, long yearIds);
        List<VmIncentivePolicy> GetCmUserList2(long monIds, long yearIds);
        List<VmIncentivePolicy> GetCmUserList3(long monIds, long yearIds);
        List<VmIncentivePolicy> GetUserInfoSpare(long monIds, long yearIds);

        #region VENDOR AUTOCOMPLETE
        List<ProjectMasterModel> GetVendorList(string vendor, string type);
        #endregion

        bool UpdateBulkProject(BulkUpdateModel model);
        VmWarehouseEntry GetCommercialWarehouseEvent();
        VmWarehouseEntry GetCommercialWarehouseEventList(DateTime? fromDate, DateTime? toDate, string searchString);
        List<CreateFocForAftersalesPmModel> GetAftersalesPmFoc();
        string UpdateFocForAftersalesPm(VmAftersalesPmFoc focUpdate);
        List<VmPendingTac> GetPendingTacList();
        ProjectMasterModel GetProjectByName(string projectName);

        #region Incentive Upto August 2019
        List<VmIncentivePolicy> GetVmIncentivePolicy();
        List<VmIncentivePolicy> GetIncentiveOrders(long monIds, long yearIds);
        List<VmIncentivePolicy> GetIncentiveLcs(long monIds, long yearIds);
        List<VmIncentivePolicy> GetPrimarySales(long monIds, long yearIds);
        List<VmIncentivePolicy> GetFeaturePhoneService(long monIds, long yearIds);
        List<VmIncentivePolicy> GetFeaturePhoneSales(long monIds, long yearIds);
        List<VmIncentivePolicy> GetSmartPhoneService(long monIds, long yearIds);
        List<VmIncentivePolicy> GetSmartPhoneSales(long monIds, long yearIds);
        List<VmIncentivePolicy> GetCmUserList(long monIds, long yearIds);

        List<VmIncentivePolicy> GetIncentiveReport(string monId, long yearIds);
        List<VmIncentivePolicy> GetIncentiveReport1(string monId, long yearIds);
        List<VmIncentivePolicy> GetPreparedUserName();
        bool GetCheckDate(string monId, string yearId);
        List<VmIncentivePolicy> GetIncentiveSeaShipmentFulls(long monIds, long yearIds);
        //List<VmIncentivePolicy> CmIncentiveForAllPerson(string month, string monNum, string year);
        List<VmIncentivePolicy> GetIncentiveSeaShipmentPartials(long monIds, long yearIds);
        List<VmIncentivePolicy> GetIncentiveAirShipmentFulls(long monIds, long yearIds);
        List<VmIncentivePolicy> GetIncentiveAirShipmentPartials(long monIds, long yearIds);

        string GetSaveIncentive21(string month, string monNum, string year, string totalAmount21, List<Incentive> results21, List<Incentive> results31);
        string GetSaveIncentive(string month, string monNum, string year, string totalAmount, List<Incentive> results, List<CmIncentiveModel> results2);
        List<NinetyFiveProductionRewardModel> CmPenaltiesCkdSkd(string monNum, string year);
        List<NinetyFiveProductionRewardModel> CmPenaltiesRepeatOrder(string monNum, string year);
        #endregion

        #region Spare & Incentive  Upto August 2019
        List<ProjectMasterModel> GetSpareProjectList();
        List<ProjectMasterModel> GetProjectWiseOrderForSpare(long proId);
        ProjectOrderShipmentModel GetWarehouseReceiveDate(long proId);
        bool CheckSpareDataAlreadySaved(List<SpareClaimModel> results);
        string SaveSpareClaimDatas(List<SpareClaimModel> results);
        List<SpareClaimModel> GetPreviousSpareDatas(long proId);
        bool CheckSpareIncentiveData(long monIds, long yearIds);
        List<SpareClaimModel> GetNewSpareComplain();
        string SaveSpareApprovedData(long proIds, string spareClaimDate, string warehouseDate, long quantity, string remarks, string status);
        string SaveSpareDeclinedData(long proIds, string spareClaimDate, string warehouseDate, long quantity, string remarks, string status);
        List<SpareClaimModel> GetTotalSpareClaim(string monId, string yearId);
        List<SpareClaimModel> GetUserInfoForSpareClaims(long monIds, long yearIds);
        string SaveMonthlyIncentiveForSpareClaims(List<SpareClaimModel> results);
        string SaveCmPenaltiesCkdSkd(NinetyFiveProductionRewardModel refundSave);
        bool GetCmRefundData(NinetyFiveProductionRewardModel refundSave);
        string SaveCmPenaltiesRepeatOrder(NinetyFiveProductionRewardModel refundSave);
        List<Cm_OthersIncentiveModel> GetOthersIncentive(long monIds, long yearIds);
        #endregion

        #region Incentive From September 2019 com
        List<VmIncentivePolicy> GetVmIncentivePolicyNew();
        List<VmIncentivePolicy> GetIncentiveOrdersNew(long monIds, long yearIds);
        List<VmIncentivePolicy> GetIncentiveLcsNew(long monIds, long yearIds);
        List<VmIncentivePolicy> GetPrimarySalesNew(long monIds, long yearIds);
        List<VmIncentivePolicy> GetFeaturePhoneServiceNew(long monIds, long yearIds);
        List<VmIncentivePolicy> GetFeaturePhoneSalesNew(long monIds, long yearIds);
        List<VmIncentivePolicy> GetSmartPhoneServiceNew(long monIds, long yearIds);
        List<VmIncentivePolicy> GetSmartPhoneSalesNew(long monIds, long yearIds);
        List<VmIncentivePolicy> GetCmUserListNew(long monIds, long yearIds);

        List<VmIncentivePolicy> GetIncentiveReportNew(string monId, long yearIds);
        List<VmIncentivePolicy> GetIncentiveReport1New(string monId, long yearIds);
        List<VmIncentivePolicy> GetPreparedUserNameNew();
        bool GetCheckDateNew(string monId, string yearId);
        List<VmIncentivePolicy> GetIncentiveSeaShipmentFullsNew(long monIds, long yearIds);
        //List<VmIncentivePolicy> CmIncentiveForAllPerson(string month, string monNum, string year);
        List<VmIncentivePolicy> GetIncentiveSeaShipmentPartialsNew(long monIds, long yearIds);
        List<VmIncentivePolicy> GetIncentiveAirShipmentFullsNew(long monIds, long yearIds);
        List<VmIncentivePolicy> GetIncentiveAirShipmentPartialsNew(long monIds, long yearIds);

        string GetSaveIncentive21New(string month, string monNum, string year, string totalAmount21, List<Incentive> results21);
        string GetSaveIncentiveNew(string month, string monNum, string year, string totalAmount, List<Incentive> results);
        string SaveCmPenaltiesAndRewardData(string month, string monNum, string year);
        string SaveCmOthersIncentive(List<Cm_OthersIncentiveModel> results);

        #endregion

        #region Spare & Incentive From September 2019
        List<ProjectMasterModel> GetSpareProjectListNew();
        List<ProjectMasterModel> GetProjectWiseOrderForSpareNew(long proId);
        ProjectOrderShipmentModel GetWarehouseReceiveDateNew(long proId);
        bool CheckSpareDataAlreadySavedNew(List<SpareClaimModel> results);
        string SaveSpareClaimDatasNew(List<SpareClaimModel> results);
        List<SpareClaimModel> GetPreviousSpareDatasNew(long proId);
        bool CheckSpareIncentiveDataNew(long monIds, long yearIds);
        List<SpareClaimModel> GetNewSpareComplainNew();
        string SaveSpareApprovedDataNew(long proIds, string spareClaimDate, string warehouseDate, long quantity, string remarks, string status);
        string SaveSpareDeclinedDataNew(long proIds, string spareClaimDate, string warehouseDate, long quantity, string remarks, string status);
        List<SpareClaimModel> GetTotalSpareClaimNew(string monId, string yearId);
        List<SpareClaimModel> GetUserInfoForSpareClaimsNew(long monIds, long yearIds);
        string SaveMonthlyIncentiveForSpareClaimsNew(List<SpareClaimModel> results);
        long WarehouseEntryQuantityThisMonth(DateTime poDate, string projectName);
        void SaveOrUpdateJigsAndFixtures(List<JigsAndFixtureModel> model);
        List<JigsAndFixtureModel> GetJigsAndFixtureModelsByProjectId(long projectId);
        ChargerPoModel SaveUpdteChargerPoModel(ChargerPoModel model);
        ChargerPoModel GetChargerPoModelById(long id);
        List<ChargerPoModel> GetAllChargerPoModels();
        EarphonePoModel SaveUpdateEarphonePoModel(EarphonePoModel model);
        List<EarphonePoModel> GetAllEarphonePoModels();
        EarphonePoModel GetEarphonePoModelById(long id);

        #endregion

        List<VmIncentivePolicy> GetRewardAndPenalties(long monIds, long yearIds);
        List<VmIncentivePolicy> GetChinaIqcIncentive(long monIds, long yearIds);
        List<NinetyFiveProductionRewardModel> CmPenaltiesAndRewardCkdSkd(string MonNum, string Year);
        List<NinetyFiveProductionRewardModel> CmPenaltiesAndRewardRepeatOrder(string monNum, string year);
        List<NinetyFiveProductionRewardModel> CmRewardNinetyFiveProduction(string monNum, string year);
        List<NinetyFiveProductionRewardModel> CmRewardNinetyFiveSalesOut(string monNum, string year);

        #region LC Permission
        LcOpeningPermissionModel AddToLcPermission(LcOpeningPermissionModel lcPermissionModel);
        List<LcOpeningPermissionModel> GetLcPermissionList();
        List<LcOpeningPermissionOtherProductModel> GetLcPermissionOtherProductList();
        List<LcOpeningPermissionOtherProductModel> GetTtPendingLc();
        LcOpeningPermissionModel GetLcPermissionDetailsById(long id);
        LcOpeningPermissionModel UpdateApprovalStatus(long id, string checkedValue);
        LcOpeningPermissionModel UpdateLcOpeningPermissionModel(LcOpeningPermissionModel m);
        LcOpeningPermissionOtherProductModel GetLcOpeningPermissionOtherProductById(long id);
        long SaveLcOpeningOtherProduct(LcOpeningPermissionOtherProductModel m);
        LcOpeningPermissionModel GetLcOpeningPermissionByProjectId(long projectId);
        void SaveLcOpeningPermissionLog(LcOpeningPermissionModel m);
        List<LcOpeningPermissionModel> GetHandsetLcApprovalsByDateRange(DateTime fromDate, DateTime toDate);

        List<LcOpeningPermissionOtherProductModel> GetLcOpeningPermissionOtherProdWithinDateRange(DateTime from,
            DateTime to);
        #endregion

        void SaveOrderWiseMultiplePrice(OrderWiseMultiplePriceModel orderWiseMultiplePrice);
        OrderWiseMultiplePriceModel UpdateOrderWiseMultiplePrice(OrderWiseMultiplePriceModel orderWiseMultiplePrice);
        List<OrderWiseMultiplePriceModel> GetallOrderWiseMultiplePrice();
        OrderWiseMultiplePriceModel GetOrderWiseMultiplePriceById(long id);

        VmImeiDataBase GetProjectBabtList(VmImeiDataBase model);
        List<ProjectMasterModel> GetAllProductModel();
        List<ProjectOrderShipmentModel> GetFinishGoodDetails(long proShipOrder);
        List<LC_IDH_Final_BOM> GetLcIdhFinalBomsByVariantId(long id);
        List<LC_IDH_Final_BOMModel> GetLcIdhFinalBomModelByVariantId(long id);
        LC_IDH_Final_BOM GetIDHFinalBomInfoBySpareId(long id);
        List<LC_IDH_DetailsModel> GetPrevIdhDetailsByVariantId(long variantId);
        void SaveIdhBom(LC_IDH_Final_BOMModel model);
        //long SaveIdhLcMaster(LC_IDH_Masters model);
        void SaveIdhLcDetails(LC_IDH_Details model);
        int? GetLastOrderSerialInIdhDetails(long variantId);
        List<ModelListForIMEIDownload> GetModelList(DateTime fromDate, DateTime todate);
        List<ModelListForIMEIDownload> GetModelWiseReportData(DateTime fromDate, DateTime todate, string modelname);
        List<FobPriceUpdateLog> GetFobPriceUpdateLogByProjectId(long projectId);
    }
}
