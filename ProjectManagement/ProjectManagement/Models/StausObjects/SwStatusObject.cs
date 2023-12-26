using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.StausObjects
{
    public class SwStatusObject
    {
        public long ProjectMasterId { get; set; }
        public DateTime? ProjectManagerAssignToQcInTime { get; set; }
        public DateTime? QcInchargeToQcAssignTime { get; set; }
        public DateTime? QcInchargeToPmProjectSubmitTime { get; set; }
    }
}