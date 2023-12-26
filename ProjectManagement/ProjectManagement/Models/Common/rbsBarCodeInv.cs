using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.Common
{
    public class rbsBarCodeInv
    {
        public string ProjectModel { get; set; }
        public long Produced { get; set; }
        public long OrderQuantity { get; set; }
        public long UnProduced { get; set; }
        public string Order_Num { get; set; }
        public int Order_No { get; set; }
        public long LastMonthIMEIProduced { get; set; }
        public string UnproducedPercentage { get; set; }
    }
}