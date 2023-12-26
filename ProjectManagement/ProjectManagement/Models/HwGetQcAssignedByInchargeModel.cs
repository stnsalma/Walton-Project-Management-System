using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwGetQcAssignedByInchargeModel
    {
        public long CmnUserId { get; set; }
        public string UserFullName { get; set; }
        public string Email { get; set; }
        public string TestName { get; set; }
        public DateTime HwQcAssignDate { get; set; }
        public string Status { get; set; }
        public DateTime? DeadLineDate { get; set; }
    }
}