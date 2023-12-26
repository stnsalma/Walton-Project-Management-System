using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.StausObjects
{
    public class HwRunningStatusObject
    {
        //running
        public DateTime? RunningSampleSent { get; set; }
        public DateTime? RunningSampleReceive { get; set; }
        public DateTime? RunningAssign { get; set; }
        public DateTime? RunningSubmit { get; set; }
        public DateTime? RunningVerified { get; set; }
        public DateTime? RunningForward { get; set; }
        public string TestPhase { get; set; }
    }
}