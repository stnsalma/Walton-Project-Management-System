using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwQcTabColorModel
    {
        public long SwQcTabColorId { get; set; }
        public Nullable<long> ProjectMasterId { get; set; }
        public Nullable<long> SwQcInchargeAssignId { get; set; }
        public Nullable<long> SwQcUserId { get; set; }
        public Nullable<long> SwQcAssignId { get; set; }
        public string QcCategoryName { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
    }
}