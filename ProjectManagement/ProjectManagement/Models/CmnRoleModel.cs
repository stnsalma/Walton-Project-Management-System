using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class CmnRoleModel
    {
        public long CmnRoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public bool? IsHead { get; set; }
    }
}