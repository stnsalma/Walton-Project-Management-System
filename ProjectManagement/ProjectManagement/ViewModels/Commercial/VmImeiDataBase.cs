using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Commercial
{
    public class VmImeiDataBase
    {

        public VmImeiDataBase()
        {
            ProjcetBabts = new List<ProjectBabtModel>();
            TacList = new List<string>();
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<ProjectBabtModel> ProjcetBabts { get; set; }

        public List<string> TacList { get; set; }
    }
}