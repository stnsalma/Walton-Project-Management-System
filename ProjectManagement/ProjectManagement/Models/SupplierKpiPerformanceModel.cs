using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SupplierKpiPerformanceModel
    {
        public string Model { get; set; }
        public string SupplierName { get; set; }
        public int? TotalReceive { get; set; }
        public int? TotalUniqueServiceReceive { get; set; }
        public int? TotalActivation { get; set; }
    }
}