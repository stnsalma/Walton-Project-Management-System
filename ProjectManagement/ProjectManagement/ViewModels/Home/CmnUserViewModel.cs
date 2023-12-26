using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Home
{
    public class CmnUserViewModel
    {
        public CmnUserModel CmnUserModel { get; set; }
        public CmnRoleModel CmnRoleModel { get; set; }
    }
}