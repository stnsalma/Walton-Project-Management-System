using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ProjectManagement.Models.AssignModels;

namespace ProjectManagement.ViewModels.Management
{
    public class AssignMuliplePersonViewModel
    {
        public List<PmQcAssignModel> PmQcAssignModels { get; set; }

        #region DropdownRemoteValidationForADropDownList
        [Required(ErrorMessage = "First name is required")]
        public String ddlAssignUserId { get; set; }
        public List<String> ddlAssignUsersList { get; set; }
        #endregion
    }
}