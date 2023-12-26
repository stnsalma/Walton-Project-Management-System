using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.Common
{
    public class HighChartDataListByOrderFromLauncingDate
    {
        public long tendays { get; set; }
        public long twentydays { get; set; }
        public long fortydays { get; set; }
        public long sixtydays { get; set; }
        public long nintydays { get; set; }
        public long oneEightydays { get; set; }
        public long twoseventy { get; set; }
        public long threeSixty { get; set; }
        public long restoftheDays { get; set; }
        public string orders { get; set; }
    }
}