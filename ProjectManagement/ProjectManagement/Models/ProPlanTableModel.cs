using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProPlanTableModel
    {
        public bool? IsActive { get; set; }
        public long PlanId { get; set; }
        public long? ProjectId { get; set; }
        public bool? IsCharger { get; set; }
        public bool? IsCkd { get; set; }
        public DateTime? AddadDate { get; set; }
    }
}