using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;
using ProjectManagement.ViewModels.ProjectCommercial;

namespace ProjectManagement.Infrastructures.Interfaces
{
    public  interface IProjectCommercialRepository
    {
        ProjectBabtModel GetProjectBabtInfo(long projectId);
        long  GetProjectPurchaseOrderFormId(long masterId);
        string SaveBabtInfo(long projectMasterId, long assignedId, long purchaseOrderFormId, long babtId, long quantity);
        string SaveSendingSupplierDate(long pmId,string sendingDate);
        List<VmTacRequest> GetProjectsForTac(long userId);

        #region CommercialKpi
        List<CmnUserModel> GetCmUsersUnderHead(string persons, long monIds, long yearIds);
        List<CmnUserModel> GetCommercialUsers();
        List<TeamKpiPercentageListModel> GetCmKpi(string persons, long monIds, long yearIds);
        List<TeamKpiPercentageListModel> CommercialKpiDetails(string persons, long monIds, long yearIds, string kpiName);
        List<TeamKpiPercentageListModel> CommercialIqcKpiDetails(string persons, long monIds, long yearIds, string kpiName);
        List<TeamKpiPercentageListModel> CommercialKpiLineChart(string persons, string sDate, string endDate);
        List<CmnUserModel> GetCommercialUsersDetailsCM(string persons);
        List<CmnUserModel> GetCommercialUsersDetailsCMHEAD();
        List<CmnUserModel> GetCommercialUsersDetailsInformation(string persons);
        List<TeamKpiPercentageListModel> CommercialKpiSingleBarChart(string persons, string monNum, string year);
       // FileContentResult GetProfilePicture(string uId);
        List<TeamKpiRoleTable> GetKpiRoleName();
        List<CmnUserModel> GetRolePerson(string proRoleName);
        List<TeamKpiPercentageListModel> GetCmYearlyKpi(string startValue, string endValue, string kpiRoles, string kpiRolePerson);
        List<TeamKpiPercentageListModel> GetCmYearlyOthersKpi(string startValue, string endValue, string kpiRoles, string kpiRolePerson);
        string SaveKpiValueBData(List<TeamKpiPercentageListModel> results);
        bool GetSavedKpiData(string employeeCode, string KpiName);

        #endregion
       
    }
}
