using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Management
{
    public class VmFinalApproval
    {
        public VmFinalApproval()
        {
            HwInchargeIssueModels = new List<HwInchargeIssueModel>();
        }
        public long ProjectMasterId { get; set; }
        [Required]
        public string Status { get; set; }
        public string Remarks { get; set; }
        public List<HwInchargeIssueModel> HwInchargeIssueModels { get; set; }
    }
}