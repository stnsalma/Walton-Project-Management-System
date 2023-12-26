using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;
using ProjectManagement.ViewModels.Production;

namespace ProjectManagement.Infrastructures.Interfaces
{
    public interface IProductionRepository
    {
        List<ProductionTrackerModel> GetProductionTrackerModels();
        void ImportDataFromExcel(string excelFilePath);
        string ImportDataFromExcel2(string filepath);
        List<VmAssemblyPackingProduction> GetProductionProject();
        List<ProjectMasterModel> GetProductionProjectList();
        List<ProjectMasterModel> GetProjectOrders(long proIds);
        ProjectMasterModel GetProjectPoCategory(long proIds);
        string AddedProjectPartialSaves(List<CustomPrdAssemblyAndPackingDetails> results);
        List<CustomPrdAssemblyAndPackingDetails> GetAssemblyAndPackingSavedProject();
        List<CustomPrdAssemblyAndPackingDetails> GetAssemblyAndPackingCompletedProject();
        string UpdateAssemblyAndPackingTables(CustomPrdAssemblyAndPackingDetails assembAndPack);
        string UpdateAssemblyAndPackingTableStatuses(CustomPrdAssemblyAndPackingDetails assembAndPack);
        List<ProductPlanModel> GetGrandChartDatas(List<string> results);
        string InsertProductionRemarks(CustomPrdAssemblyAndPackingDetails productionRemarksData);
        bool GetHolidayDatas(string dateForHoliday);
        List<GovernmentHolidayTableModel> GetHolidayDatasList();
        List<CustomPrdAssemblyAndPackingDetails> GetAssemblyLineDatas(string assemblyStartDate,string assemblyEndDate);
        List<LineInformationModel> SelectLineInfos();
        List<CustomPrdAssemblyAndPackingDetails> GetPackingLineDatas(string packingStartDate, string packingEndDate);
        string AddedChargerPlanData(List<CustomChargerProduction> results);
        List<ChargerProductionViewModel> GetChargerGrandChartDatas(List<string> results);
        bool GetMaterialReceiveForSmt(string materialReceiveStartDateSmt, string materialReceiveEndDateSmt);
        bool GetIqcCompleteForSmt(string iqcCompleteStartDateSmt, string iqcCompleteEndDateSmt);
        bool GetTrialProductionDateForSmt(string trialProductionStartDateSmt, string trialProductionEndDateSmt);
        bool GetMassProductionDateForSmt(string massProductionStartDateSmt, string massProductionEndDateSmt);
        bool GetMaterialReceiveDateHousing(string materialReceiveStartDateHousing, string materialReceiveEndDateHousing);
        bool GetIqcCompleteDateHousing(string iqcCompleteStartDateHousing, string iqcCompleteEndDateHousing);
        bool GetTrialProductionDateHousing(string trialProductionStartDateHousing, string trialProductionEndDateHousing);
        bool GetHousingReliabilityDate(string housingReliabilityStartDateHousing, string housingReliabilityEndtDateHousing);
        bool GetHousingMassProduction(string housingMassProStartDateHousing, string housingMassProEndtDateHousing);
        bool GetMaterialReceiveDateAssembly(string materialReceiveStartDateAssembly, string materialReceiveEndDateAssembly);
        bool GetIqcCompleteDateAssembly(string iqcCompleteStartDateAssembly, string iqcCompleteEndDateAssembly);
        bool GetTrialProductionDateAssembly(string trialProductionStartDateAssembly, string trialProductionEndDateAssembly);
        bool GetRnDConfirmDateAssembly(string rnDConfirmStartDateAssembly, string rnDConfirmEndDateAssembly);
        bool GetAssemblyProduction(string assembStartDateAssembly, string assembEndDateAssembly);
        List<CustomChargerProduction> GetChargerOldHistory(long proIds);
        List<LineInformationModel> SelectLineInfoChargerSmt();
        List<LineInformationModel> SelectLineInfoChargerHousing();
        List<LineInformationModel> SelectLineInfoChargerAssembly();
        List<ChargerSMTLineCapacityDetailsModel> GetAvailableProductionLineForSmt(string massProductionStartDateSmt, string massProductionEndDateSmt);
        List<ChargerHousingLineCapacityDetailsModel> GetAvailableProductionLineForHousing(string housingMassProStartDateHousing, string housingMassProEndtDateHousing);
        List<ChargerAssemblyLineCapacityDetailsModel> GetAvailableProductionLineForAssembly(string assembStartDateAssembly, string assembEndDateAssembly);
        List<LineInformationModel> SelectLineInfoBatterySmt(long proIds);
        List<LineInformationModel> SelectLineInfoBatteryHousing();
        List<LineInformationModel> SelectLineInfoBattery();
        List<LineInformationModel> SelectLineInfoBatteryAssembly(long proIds);
        List<LineInformationModel> SelectLineInfoBatteryPacking(long proIds);
        List<BatteryAssemblyLineCapacityDetailModel> GetAvailableProductionLineForBatteryAssembly(string assembStartDateBAssembly, string assembEndDateBAssembly, long proIds);
        List<BatteryPackingLineCapacityDetailModel> GetAvailableProductionLineForBatteryPacking(string packingMassProductionStartDateBAssembly, string packingMassProductionEndDateBAssembly, long proIds);
        List<BatteryLineCapacityDetailModel> GetAvailableProductionLineForBattery(string batteryMassProductionStartDate, string batteryMassProductionEndDate);
        List<BatteryHousingLineCapacityDetailModel> GetAvailableProductionLineForBHousing(string housingMassProStartDateHousing, string housingMassProEndtDateHousing);
        List<BatterySMTLineCapacityDetailModel> GetAvailableProductionLineForBSmt(string massProductionStartDateBSmt, string massProductionEndDateBSmt, long proIds);
        bool GetMaterialReceiveDateBAssembly(string materialStartDateBAssembly, string materialReceiveEndDateBAssembly);
        bool GetIqcCompleteDateBAssembly(string iqcCompleteStartDateBAssembly, string iqcCompleteEndDateBAssembly);
        bool GetAssemblyBProduction(string assembStartDateBAssembly, string assembEndDateBAssembly);
        bool GetTrialProductionDateBAssembly(string trialProductionStartDateBAssembly, string trialProductionEndDateBAssembly);
        bool GetPackingBProduction(string packingMassProductionStartDateBAssembly, string packingMassProductionEndDateBAssembly);
        bool GetBatteryMassProduction(string batteryMassProductionStartDate, string batteryMassProductionEndDate);
        bool GetTrialProductionDateBattery(string trialProductionStartDateBattery, string trialProductionEndDateBattery);
        bool GetIqcCompleteDateBattery(string iqcCompleteStartDateBattery, string iqcCompleteEndDateBattery);
        bool GetMaterialReceiveDateBattery(string materialReceiveStartDateBattery, string materialReceiveEndDateBattery);
        bool GetHousingMassBProduction(string housingMassProStartDateHousing, string housingMassProEndtDateHousing);
        bool GetTrialBProduction(string trialProductionStartDateHousing, string trialProductionEndDateHousing);
        bool GetIqcCompleteDateBHousing(string iqcCompleteStartDateHousing, string iqcCompleteEndDateHousing);
        bool GetMaterialReceiveDateBHousing(string materialReceiveStartDateHousing, string materialReceiveEndDateHousing);
        bool GetSmtMassBProduction(string massProductionStartDateBSmt, string massProductionEndDateBSmt);
        bool GetMaterialReceiveDateBSmt(string materialReceiveStartDateBSmt, string materialReceiveEndDateBSmt);
        bool GetIqcCompleteDateBSmt(string iqcCompleteStartDateBSmt, string iqcCompleteEndDateBSmt);
        bool GetTrialProductionDateBSmt(string trialProductionStartDateBSmt, string trialProductionEndDateBSmt);
        string SaveBatteryPlanData(List<CustomBatteryProduction> results);
        List<CustomBatteryProduction> GetBatteryOldHistory(long proIds, long planId);
        List<BatteryProductionViewModel> GetBatteryGrandChartDatas(List<string> results);
        List<CustomBatteryProduction> GetPartialProject();
        List<AllTrialInfo> GetSmtTrialLineForEdit();
        string UpdateChdPlanning(CustomBatteryProduction allInfo);
        List<CustomBatteryProduction> GetBatteryOldHistoryEdit(long proIds, long planIds);
        List<BatteryAssemblyLineCapacityDetailModel> GetAvailableProductionLineForBatteryAssembly1(string assembStartDateBAssembly, long proIds);
        List<BatteryProductionViewModel> GetCkdGrandChartData(List<string> results);
        List<CustomBatteryProduction> GetSelectedProjectPlanningHistory(long proIds, string projectName);
        string InActiveAPlan(long proIds, long planId);

        #region Mobile Production Plan New
        List<GovernmentHolidayTableModel> GetHoliday();
        string SaveHolidayDropData(string Id,string governmentHoliday, string holidayStartDate,string holidayEndDate);
        string DeleteHolidayData(string id);
        #endregion

        #region Capacity Planning
        List<Pro_Type_Model> GetProductionType();
        string SaveShift(List<Pro_Shift_Model> issueList, int mon, string monName, int years, string productionType);
        List<Pro_Shift_Model> GetShiftSavedData(int mons, string year, string productionType);
        List<Pro_Shift_Model> GetDailyShiftData(int mons, string year, string productionType);
        List<Pro_Shift_Model> GetLine(int mons, string year, string productionType);
        List<Pro_Shift_Model> GetShift(int mons, string year, string productionType, string phoneType);
        string SaveCapacityData(List<Pro_CapacityPlanning_Model> results);
        List<Pro_CapacityPlanning_Model> GetCapacity(int mons, string year, string productionType, string categories);
        List<Pro_CapacityPlanning_Model> GetTeam(int mons, string year, string productionType, string phoneType, string categories);
        List<Pro_CapacityPlanning_Model> GetPercentage(int mons, string year, string productionType, string phoneType,string categories);
        List<Pro_CapacityPlanning_Model> GetQuantityRange(int mons, string year, string productionType, string phoneType,string categories);
        List<Pro_CapacityPlanning_Model> GetAll(int mons, string year, string productionType, string phoneType, string categories);
        string SaveTeam(List<Pro_Shift_Model> issueList1, string productionType);
        List<Pro_Shift_Model> GetTeamForUpdate(string productionType);
        string UpdateTeam(long ids);
        List<string> GetAllTeam(string productionType);
        List<string> GetAllCategory(string productionType11, string phoneType);
        string SaveProduct(List<Pro_Shift_Model> issueList1, string productionType);
        string EditTeam(string id, string team);
        string SaveLine(List<Pro_Shift_Model> issueList1, string productionType);
        List<Pro_Shift_Model> GetLineForUpdate(string productionType);
        string EditLine(string id, string line, string lineType, string productionDaysPerMonth, string shiftPerDay, string hoursPerShift);
        string InActiveLine(long ids);
        List<Pro_Shift_Model> GetProductForUpdate(string productionType);
        string InActiveProduct(long ids);
        List<string> GetAllLine(string productionType11);
        string InActiveShift(long ids);
        List<Pro_Shift_Model> GetProductName(string productionType);
        List<Pro_Shift_Model> GetCategoryName(string productionType, string proPhoneName);
        List<Pro_CapacityPlanning_Model> GetAll1(int mons, string year, string productionType, string phoneType, string categories);
        string UpdateDailyPlan(long ids, DateTime effectiveDate, string line, string shift1, string shift2, string shift3, string productionType, string monNum, string month, string year);
        string SaveDailyPlan(List<Pro_Shift_Model> results);
        #endregion

        #region Capacity Report
       
        List<Pro_Shift_Model> ProductNameForReport(int mons, string year, string productionType);
        List<Pro_Shift_Model> TeamNameForReport(int mons, string year, string productionType);
        List<Pro_Shift_Model> CategoryNameForReport(int mons, string year, string productionType);
        List<Pro_CapacityPlanning_Model> GetPercentage1(int mons, string year, string productionType);
        List<Pro_CapacityPlanning_Model> GetQuantityRange1(int mons, string year, string productionType);
        List<Pro_CapacityPlanning_Model> GetTotalCapacities1(int mons, string year, string productionType);
        #endregion

        #region Project Categorize
        List<Pro_Shift_Model> GetProjectForCategorization();
        List<Pro_Shift_Model> GetAssemblyCategory(string projectType);
        List<Pro_Shift_Model> GetSmtCategory(string smtCategory);
        List<Pro_Shift_Model> GetHousingCategory(string housingCategory);
        string SaveCategorizeData(string projectName, string productFamily, string assemblyCategory, string smtCategory, string housingCategory);
        string CompleteCategorizeData(string projectName, string productFamily);
        List<Pro_Shift_Model> GetCompletedCategorization();
        string UpdateCategorizeData(long ids, string assemblyCategory1, string smtCategory1, string housingCategory1);
        List<Pro_Shift_Model> ChangedDailyPlanData(int mons, string year, string productionType);
        List<Pro_Shift_Model> GetDailyShiftData1(int mons, string year, string productionType);
        List<Pro_Shift_Model> DailySaved(int mons, string year, string productionType);
        string ForwardShift(string unitValues, string currentDate, string forwardedDate, string shiftForward);
        string ForwardCapacity(string unitValues, string currentDate, string forwardedDate, string capForward);
        #endregion

       
    }
}
