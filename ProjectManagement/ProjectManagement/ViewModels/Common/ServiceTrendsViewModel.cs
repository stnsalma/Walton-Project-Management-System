using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models.Common;

namespace ProjectManagement.ViewModels.Common
{
    public class ServiceTrendsViewModel
    {
        public string ModelName { get; set; }
        public string ReleaseDate { get; set; }
        public string DayCountfromRelease { get; set; }
        public double Totalhandset { get; set; }
        public double TotalActivated { get; set; }
        public double UnActivated { get; set; }
        public double StockFault { get; set; }
        public double StockFaultPercentage { get; set; }
        public double Replacement { get; set; }
        public double ReplacementPercentage { get; set; }
        public double ServicePointEntry { get; set; }
        public double ServicePointEntryPercentage { get; set; }
        public double TotalRetern { get; set; }
        public double TotalReternPercentage { get; set; }
        public IList<MajorProblem> MajorProblems { get; set; }
       
    }
}