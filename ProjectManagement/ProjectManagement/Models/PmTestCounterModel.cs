using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PmTestCounterModel
    {
        public int NewProjectCounter { get; set; }
        public int ProjectForwaredToSwCounter { get; set; }
        public int ProjectForwaredToHwCounter { get; set; }
        public int RequestImeiRangeCounter { get; set; }
    }
}