using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.Common
{
    public class HighChartDataModel
    {
        public string Date { get; set; }
        public DateTime RealDate { get; set; }
        public int quantity { get; set; }
        public decimal totalPrice { get; set; }
        public long DateInJs { get; set; }
    }
}