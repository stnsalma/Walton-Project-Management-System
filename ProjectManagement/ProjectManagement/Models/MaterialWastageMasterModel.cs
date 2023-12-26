using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.DAL.DbModel;

namespace ProjectManagement.Models
{
    public class MaterialWastageMasterModel
    {
        public long Id { get; set; }
        public string ReportName { get; set; }
        public string MonthName { get; set; }
        public int YearNumber { get; set; }
        public System.DateTime AddedDate { get; set; }
        public long AddedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public long ProjectOrderQuantityDetailsId { get; set; }
        public int MonthNumber { get; set; }
        public bool? IsInchargeApproved { get; set; }
        public bool? IsCooApproved { get; set; }
        public bool? IsSpecialApproved { get; set; }
        public bool? IsManagementApproved { get; set; }
        public bool? IsCompleted { get; set; }

        public string InchargeApproverName { get; set; }
        public string CooApprovername { get; set; }
        public string SpecialApproverName { get; set; }
        public string ManagementApproverName { get; set; }
        public string AddedByName { get; set; }
        public string ProjectVarientName { get; set; }

        public bool IsDeclinedFromAnyOne { get; set; }
    }
}