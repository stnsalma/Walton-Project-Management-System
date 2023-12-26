using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.ProjectCommercial
{
    public class VmCommercialKpi
    {
        public VmCommercialKpi()
        {
            CmnUserModel=new CmnUserModel();
            CmnUserModels=new List<CmnUserModel>();
            CmnUserModels2=new List<CmnUserModel>();
            TeamKpiPercentageListModel=new TeamKpiPercentageListModel();
            TeamKpiPercentageListModels=new List<TeamKpiPercentageListModel>();
            TeamKpiPercentageListModels1=new List<TeamKpiPercentageListModel>();
            TeamKpiRoleTables=new List<TeamKpiRoleTable>();
            TeamKpiRoleTable=new TeamKpiRoleTable();
            //FilesDetails = new List<FilesDetail>();
        }
        //public List<FilesDetail> FilesDetails { get; set; }
        //public string FilesDetail { get; set; }
        //public string Upload { get; set; }

        public List<TeamKpiRoleTable> TeamKpiRoleTables { get; set; }
        public TeamKpiRoleTable TeamKpiRoleTable { get; set; }
        public List<CmnUserModel> CmnUserModels { get; set; }
        public List<CmnUserModel> CmnUserModels2 { get; set; }
        public CmnUserModel CmnUserModel { get; set; }
        public TeamKpiPercentageListModel TeamKpiPercentageListModel { get; set; }
        public List<TeamKpiPercentageListModel> TeamKpiPercentageListModels { get; set; }
        public List<TeamKpiPercentageListModel> TeamKpiPercentageListModels1 { get; set; }
        public string UserFullName { get; set; }
        public string EmployeeCode { get; set; }
        public string Month { get; set; }
        public int MonNum { get; set; }
        public string Year { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string kpiRoles { get; set; }
        public string kpiRolePerson { get; set; }
        public string kpiRolePersonName { get; set; }
    }
}