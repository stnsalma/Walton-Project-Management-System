using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.StausObjects
{
    public class HwFinishedStatusObject
    {
        //finish
        public DateTime? FinishedSampleSent { get; set; }
        public DateTime? FinishedSampleReceive { get; set; }
        public DateTime? FinishedAssign { get; set; }
        public DateTime? FinishedSubmit { get; set; }
        public DateTime? FinishedVerified { get; set; }
        public DateTime? FinishedForward { get; set; }
        public string TestPhase { get; set; }
    }
}