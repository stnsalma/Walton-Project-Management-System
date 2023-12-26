using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.Common
{
    public class SpecificModelReportModel
    {
        public string ModelName { get; set; }
        public string TotalReceive { get; set; }
        public string NonWarranty { get; set; }
        public string Warrenty { get; set; }
        public string SparePartsPending { get; set; }
        public string WorkPending { get; set; }
        public string TotalPending { get; set; }
        public string TotalWorkDone { get; set; }
        public double Ratio { get; set; }
        public string ReportType { get; set; }
        public string orders { get; set; }
        public string AverageReturnTime { get; set; }
    }
}