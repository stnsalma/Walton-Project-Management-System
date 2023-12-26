using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;
using ProjectManagement.ViewModels.AftersalesPm;

namespace ProjectManagement.Infrastructures.Interfaces
{
    interface IAfterSalesPmRepository
    {
        ProjectMasterModel GetProjectMasterModel(long i);
        List<CmnUserModel> GetAftersalesPmUserList();
        List<Pm_Incentive_BaseModel> GetAftersalesPmIncentiveBase();
        List<ProjectMasterModel> GetProjectMasterListForAftersalesPmIncentive(string employeeCode);
        List<ProjectMasterModel> GetProjectMasterListForAftersalesPmFoc();
        List<SpareNameModel> GetSpareNameForAftersalesPm(string projectType);
        ProjectMasterModel GetSupplierForAftersalesPm(long projectId);
      //  string SaveFocForAftersalesPm(VmAftersalesPmFoc model);
        string SaveFocForAftersalesPm(List<VmAftersalesPmFoc> results);
        List<CreateFocForAftersalesPmModel> GetFocForAftersalesPm();
        bool CheckDuplicateFoc(List<VmAftersalesPmFoc> focDatas);
        List<CreateFocForAftersalesPmModel> GetFocDataForAllIncentive(string monthName, string monNum, string year, string employeeCode);
        List<CreateFocForAftersalesPmModel> GetFocDataForPmHeadIncentive(string monthName, string monNum, string year, string employeeCode);
        string SaveAftersalesPmMonthlyIncentive(List<Custom_Pm_IncentiveModel> results);
        string SaveOthersIncentive(List<Custom_Pm_IncentiveModel> results);
        string SaveFocForHeadDetails(List<Custom_Pm_IncentiveModel> results);

        string SaveFocForAllDetails(List<Custom_Pm_IncentiveModel> results, string employeeCode);
        bool GetIncentiveTypeData(string employeeCode, int monNum, string year);
        bool GetFocDataForHead(string employeeCode, int monNum, string year);
        bool GetIncentiveTypeDataForOthers(string employeeCode, int monNum, string year);
        bool GetFocDataForAll(string employeeCode, int monNum, string year);
        List<Custom_Pm_IncentiveModel> GetAftersalesPmIncentiveType(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetAftersalesPm_FocIncentiveForHead(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetAftersalesPm_FocIncentiveForHeadFromOthers(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetAftersalesPm_FocIncentiveForOthers(string empCode, string monNum, string year);
        string SaveTotalAftersalesPmIncentive(string totalAmount, string empCode, string month, string monNum, string year, decimal totalIncentiveType, decimal totalAmountForFoc);
        bool GetTotalIncentiveDataForDuplicateCheck(string empCode, int monNum, string year);
        List<CreateFocForAftersalesPmModel> GetFocDataForParticularPerson(string monthName, string monNum, string year, string employeeCode);

        List<Custom_Pm_IncentiveModel> GetAftersalesPmIncentiveForPrint(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetAftersalesPmFocIncentiveForPrint(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> GetPreparedUserName();
        List<Custom_Pm_IncentiveModel> GetTotalFinalIncentiveOfAftersalesPm(string empCode, string monNum, string year);
        List<Custom_Pm_IncentiveModel> AftersalesPmIncentiveForAllPerson(string month, string monNum, string year);

        #region New Incentive Policy
        List<VmAftersalesIncentive> GetAftersalesIssueDetails(string monYear);
        List<VmAftersalesIncentive> GetAftersalesIssueDetails1(string ids);
        List<VmAftersalesIncentive> GetAftersaleUsers(long genIds,string month, long year);
        string SaveAftersalesPercentageData(List<VmAftersalesIncentive> results, long genIds);
        List<VmAftersalesIncentive> ShowTeamIncentive(int monNum, int yYear);
        List<VmAftersalesIncentive> GetAftersalesPmIncentivePerPerson(string empCode, string monNum, string year);
        List<VmAftersalesIncentive> GetTotalFinalIncentiveOfPerPm(string empCode, string monNum, string year);
        List<GovernmentHolidayTableModel> GetHoliday();
        string SaveHolidayNewData(string id, string governmentHoliday, string holidayStartDate, string holidayEndDate);
        string SaveHolidayDropData(string id, string governmentHoliday, string holidayStartDate, string holidayEndDate);
        string SaveHolidayResizeData(string id, string governmentHoliday, string holidayStartDate, string holidayEndDate);
        string DeleteHolidayData(string id);
        #endregion

        #region Aftersales Issue Handling
        List<ProjectMasterModel> GetModelsForAftersalesIssueVerification();
        string SaveIntoAftersalesIssueVerification(List<AftersalesPm_IssueVerificationModel> issueList, long projectMasterId,string projectName, string attachment);
        List<AftersalesPm_IssueVerificationModel> GetIssueVerificationList();
        string UpdateIssueConfirmationStatus(long ids);
        List<SwQcTestPhaseModel> GetSwQcTestPhasesForPm();
        SwQcInchargeAssignModel CheckSwQcInchargeDuplicateAssign(long projectMasterId);
        string AssignProjectPmToSwQcHead(long issueIds, string pmRemarks, long pMasterId, long pMAssignId, long pmUserId, List<string> selectedSampleValue, long sampleNo, long userId, long swWcInchargeAssignUserId, long testPhasefrPm, long swVersionNumber, string versionName, string sourceVersion, string targetVersion, string ActionType);
        SwQcHeadAssignsFromPm GetAllVersionNameForPm(long swVerNo, long proId, long testPhases);
        long GetUserIdByRoleName(string roleName);
        PmCmnUserModel GetPmUserInfo(long pmUserId);
        bool FullSoftwareVersionCheckedOrNot(long issueIds);
        string UpdateActionStatus(long ids, string selectedValAction);
        string SaveSupplierDetails(long ids, string details, string attachment);
        #endregion

    }
}