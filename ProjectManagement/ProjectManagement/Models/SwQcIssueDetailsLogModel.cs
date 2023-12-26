using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwQcIssueDetailsLogModel
    {
        public long SwQcIssueLogId { get; set; }
        public long SwQcIssueId { get; set; }
        public long SwQcAssignId { get; set; }
        public long SwQcHeadAssignId { get; set; }
        public long ProjectMasterId { get; set; }
        public int? OrderNumber { get; set; }
        public string ProjectName { get; set; }
        public DateTime? WaltonQcComDate { get; set; }
        public string WaltonQcComment { get; set; }
        public DateTime? SupplierComDate { get; set; }
        public string SupplierComment { get; set; }
        public DateTime? WaltonPmComDate { get; set; }
        public string WaltonPmComment { get; set; }
        public string SupplierFeedbackForAppend { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long IssueSerial { get; set; }
    }
}