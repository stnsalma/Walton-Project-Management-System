using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class GeneralIncidentDashboardModel
    {
        public int TotalIncidents { get; set; }
        public int SolutionDone { get; set; }
        public int SolutionPending { get; set; }
        public int DisclosePending { get; set; }
        public int Disclosed { get; set; }
    }
}