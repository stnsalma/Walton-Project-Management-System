using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.Common
{
    public class PieChartDataForSpare
    {
        public string ItemName { get; set; }
        public string ItemDesc { get; set; }
        public string TotalSpareUsedMinor { get; set; }
        public double TotalSpareUsedMinor1 { get; set; }
        public double TotalSpareUsedMajor { get; set; }
        public double TotalProblem { get; set; }
    }
}