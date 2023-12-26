using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwTestDetailModel
    {
        public long HwTestInchargeAssignId { get; set; }
        public long? HwTestMasterId { get; set; }
        public long? ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public string HwTestName { get; set; }
        public long? InchargeAssignedBy { get; set; }
        public string InchargeAssignedByName { get; set; }
        public DateTime? InchargeAssignedByDate { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public long? ForwardedBy { get; set; }
        public DateTime? ForwrdedDate { get; set; }
        public string ForwardRemarks { get; set; }
        public long? Serial { get; set; }
        public long HwEngineerAssignId { get; set; }
        public string HwEngineerNames { get; set; }
        public string HwInchargeRemark { get; set; }
        public string Remark { get; set; }
        public string Result { get; set; }
        public string AddedByName { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? SubmittedBy { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string HwTestStatus { get; set; }
    }
}