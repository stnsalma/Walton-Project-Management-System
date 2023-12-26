using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.StausObjects
{
    public class PmStatusObject
    {
        public long ProjectMasterId { get; set; }
        public DateTime? PmAssignDate { get; set; }
        public DateTime? RunningForwardDate { get; set; }
        public DateTime? FinishedForwardDate { get; set; }
        public DateTime? SwQcForwardDate { get; set; }
    }
}