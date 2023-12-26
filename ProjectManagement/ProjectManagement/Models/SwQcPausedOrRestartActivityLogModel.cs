using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwQcPausedOrRestartActivityLogModel
    {
        public long SwQcInchargeActivityId { get; set; }
        public Nullable<long> ProjectMasterId { get; set; }
        public Nullable<long> SwQcInchargeAssignId { get; set; }
        public Nullable<long> SwQcInchargeUserId { get; set; }
        public string Status { get; set; }
        public string PausedReason { get; set; }
        public Nullable<System.DateTime> PausedDate { get; set; }
        public Nullable<System.DateTime> RestartDate { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
    }
}