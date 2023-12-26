using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Commercial
{
    public class VmCommercialKpi
    {
        public VmCommercialKpi()
        {
            CmnUserModel=new CmnUserModel();
            CmnUserModels=new List<CmnUserModel>();
            TeamKpiPercentageListModel=new TeamKpiPercentageListModel();
            TeamKpiPercentageListModels=new List<TeamKpiPercentageListModel>();
        }
        public List<CmnUserModel> CmnUserModels { get; set; }
        public CmnUserModel CmnUserModel { get; set; }
        public TeamKpiPercentageListModel TeamKpiPercentageListModel { get; set; }
        public List<TeamKpiPercentageListModel> TeamKpiPercentageListModels { get; set; }
        public string UserFullName { get; set; }
        public string Month { get; set; }
        public int MonNum { get; set; }
        public string Year { get; set; }
    }
}