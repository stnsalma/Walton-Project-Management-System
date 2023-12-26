using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.StausObjects
{
    public class HwScreeningStatusObject
    {
        //screening
        public DateTime? ScreeningSampleSent { get; set; }
        public DateTime? ScreeningSampleReceive { get; set; }
        public DateTime? ScreeningAssign { get; set; }
        public DateTime? ScreeningSubmit { get; set; }
        public DateTime? ScreeningVerified { get; set; }
        public DateTime? ScreeningForward { get; set; }
        public string TestPhase { get; set; }
    }
}