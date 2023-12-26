using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SpareAnalysisReportMonitorModel
    {
        public long SpareAnalysisId { get; set; }
        public string ModelName { get; set; }
        public long? WarningFor { get; set; }
        public bool? IsReportSubmitted { get; set; }
        public DateTime? ReportSubmitDate { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? SubmittedBy { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public long? ReceivedBy { get; set; }
    }
}