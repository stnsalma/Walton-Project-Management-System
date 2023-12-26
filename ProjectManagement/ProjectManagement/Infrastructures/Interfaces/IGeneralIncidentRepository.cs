using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Infrastructures.Repositories;
using ProjectManagement.Models;

namespace ProjectManagement.Infrastructures.Interfaces
{
    interface IGeneralIncidentRepository
    {
        #region GET

        List<GeneralIncidentCategoryModel> GetGeneralIncidentCategories();
        List<CmnRoleModel> GetAllRoleModels();
        List<GeneralIncidentCategoryModel> GetaGeneralIncidentCategoryModels();
        List<CmnUserModel> GetUserModels(long userId);
        List<GeneralIncidentModel> GetGeneralIncidentByAddedBy(long addedby);
        List<GeneralIncidentModel> GetIncidentsForAssign();
        List<GeneralIncidentModel> GetAssignedIncidentForMe(long id);
        List<GeneralIncidentAssignModel> GetGeneralIncidentAssignModels(long incidentId);
        string GetRoleDescriptionByRoleName(string role);
        GeneralIncidentModel GetGeneralIncidentByIncidentId(long incidentId);
        List<GeneralIncidentLogModel> GetGeneralIncidentLogModels(long incidentId);
        List<GeneralIncidentModel> GetGeneralIncidentForDisclose();
        GeneralIncidentSolutionModel GetIncidentSolutionByIncidentId(long incidentId);
        GeneralIncidentDashboardModel GetGeneralIncidentDashboardCounter();
        GeneralIncidentDashboardModel GetGeneralIncidentDashboardCounterByReferredRole(string referredRole);
        List<GeneralIncidentRepository.ModelNamesModel> GetModelses();
        List<GeneralIncidentModel> GetGeneralIncidentModelsByRole(string referredRole);
        List<GeneralIncidentRepository.WsmsIssuesModel> GetWsmsIssuesModels();
        List<GeneralIncidentModel> GetSolutionPendingIncidents();
        List<GeneralIncidentModel> GetIncidentSolvedByMe(long userId);
        #endregion

        #region SET

        void SaveGeneralIncidentCategory(GeneralIncidentModel model);
        void SaveGeneralIncidentSolutionModel(GeneralIncidentSolutionModel model);
        void ForwardIncident(string remark, string forwardrole, long userId = 0, long incidentid = 0);
        void ReassignIncident(string remark, string reassignrole, long userId = 0, long incidentid = 0);
        void SaveGeneralIncidentAssignModel(GeneralIncidentAssignModel model);
        List<GeneralIncidentModel> GetDisclosedIncidents();

        #endregion

        #region UPDATE

        void DiscloseIncident(string remark, long incidentid, long disclosedBy);
        void UpdateGeneralIncidentModel(string remark, long incidentId,long userId);

        #endregion
    }
}