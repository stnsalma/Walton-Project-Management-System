using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwQcHeadStatusChangeActivityModel
    {
        public long ActivityId { get; set; }
        public long? ProjectMasterId { get; set; }
        public long? ProjectPmAssignId { get; set; }
        public long? SwQcHeadAssignId { get; set; }
        public string Status { get; set; }
        public bool? IsPaused { get; set; }
        public string PausedReason { get; set; }
        public DateTime? ActivityDate { get; set; }
        public bool? IsReStart { get; set; }
        public DateTime? RestartDate { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
    }
}