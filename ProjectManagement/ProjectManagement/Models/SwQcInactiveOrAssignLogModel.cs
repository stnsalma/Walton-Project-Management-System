using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwQcInactiveOrAssignLogModel
    {
        public long SwQcInActiveOrAssignId { get; set; }
        public long? SwQcHeadAssignId { get; set; }
        public long? ProjectMasterId { get; set; }
        public long? ProjectPmAssignId { get; set; }
        public long? SwQcUserId { get; set; }
        public string Status { get; set; }
        public bool? IsInActive { get; set; }
        public string InActiveReasonComment { get; set; }
        public bool? IsAssign { get; set; }
        public string AssignComment { get; set; }
        public DateTime? ActivityDate { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
    }
}