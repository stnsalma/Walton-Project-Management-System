using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PmProjectCompletedTaskLogModel
    {
        public long PmTabId { get; set; }
        public Nullable<long> ProjectMasterId { get; set; }
        public Nullable<long> ProjectPmAssignId { get; set; }
        public Nullable<long> ProjectManagerUserId { get; set; }
        public Nullable<long> AssignUserId { get; set; }
        public string PmCategoryName { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
    }
}