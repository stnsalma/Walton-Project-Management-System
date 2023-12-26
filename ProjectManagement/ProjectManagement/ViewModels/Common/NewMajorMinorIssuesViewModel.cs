using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models.Common;

namespace ProjectManagement.ViewModels.Common
{
    public class NewMajorMinorIssuesViewModel
    {
        public string ModelName { get; set; }
        public string Order { get; set; }
        public double TotalReceiveCount { get; set; }
        public double NonWarrantyCount { get; set; }
        public double WarrentyCount { get; set; }
        public double SparePartsPendingCount { get; set; }
        public double WorkPendingCount { get; set; }
        public double TotalPendingCount { get; set; }
        public double TotalWorkDoneCount { get; set; }
        public double TotalAverageReturnTimeCount { get; set; }
        public IList<SpecificModelReportModel> SpecificModelReports { get; set; }
    }
}