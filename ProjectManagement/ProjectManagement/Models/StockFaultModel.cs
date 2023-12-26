using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class StockFaultModel
    {
        public string Model { get; set; }
        public long ServicePointEntry { get; set; }
        public long  StockFaulty { get; set; }
    }
}