using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.Common
{
    public class PieChartDataForIssueName
    {
        public string IssueName { get; set; }
        public string IssueType { get; set; }
        public string TotalMinorProblemQTY { get; set; }
        public double TotalMinorProblemQTY1 { get; set; }
        public double TotalMajorProblemQTY { get; set; }
        public double TotalProblem { get; set; }
    }
}