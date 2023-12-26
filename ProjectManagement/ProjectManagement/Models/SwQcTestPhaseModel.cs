using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwQcTestPhaseModel
    {
        public long TestPhaseID { get; set; }
        public string TestPhaseName { get; set; }
        public bool? TestPhaseIsActive { get; set; }
        public bool? ModuleIsActive { get; set; }
    }
}