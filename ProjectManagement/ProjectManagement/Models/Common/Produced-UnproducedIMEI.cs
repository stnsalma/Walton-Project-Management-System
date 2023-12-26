using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.Common
{
    public class Produced_UnproducedIMEI
    {
        public string ProjectModel { get; set; }
        public int OrderNumber { get; set; }
        public long OrderQuantity { get; set; }
        public long Difference { get; set; }
        public long Produced { get; set; }
        public long UnProduced { get; set; }
        public long LastMonthIMEIProduced { get; set; }
    }
}