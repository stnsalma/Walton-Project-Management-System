using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwEngineerAssignModel
    {
        public long HwEngineerAssignId { get; set; }
        public long? HwTestInchargeAssignId { get; set; }
        public long? ProjectMasterId { get; set; }
        public string HwEngineerIds { get; set; }
        public long? HwTestMasterId { get; set; }
        public string HwEngineerNames { get; set; }
        public string HwInchargeRemark { get; set; }
        public string Remark { get; set; }
        public string Result { get; set; }
        public string AddedByName { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? SubmittedBy { get; set; }
        public string SubmittedByName { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Status { get; set; }
        public string HwTestName { get; set; }
        public string ProjectName { get; set; }
        public int? OrderNumber { get; set; }
    }
}