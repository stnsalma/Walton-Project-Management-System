using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwTestInchargeAssignModel
    {
        public long HwTestInchargeAssignId { get; set; }
        public long? HwTestMasterId { get; set; }
        public long? ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public string HwTestName { get; set; }
        public long? AddedBy { get; set; }
        public string AddedByName { get; set; }
        public DateTime? AddedDate { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public long? ForwardedBy { get; set; }
        public DateTime? ForwrdedDate { get; set; }
        public string ForwardRemarks { get; set; }
        public long? Serial { get; set; }
    }
}