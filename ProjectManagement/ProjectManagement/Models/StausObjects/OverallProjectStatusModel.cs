using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.StausObjects
{
    public class OverallProjectStatusModel
    {
        public long ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public bool? IsCompleted { get; set; }
        public double ActionCount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? LastActionDate { get; set; }
        
        //milestones
        public DateTime? InitialApprovalDate { get; set; }
        public DateTime? FinalApprovalDate { get; set; }
        public DateTime? PurchaseOrderDate { get; set; }
        public DateTime? PoClosingDate { get; set; }
    }
}