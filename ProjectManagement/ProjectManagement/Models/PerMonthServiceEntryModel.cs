using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PerMonthServiceEntryModel
    {
        public string MonthYear { get; set; }
        public int ServiceCount { get; set; }
        public string Color { get; set; }
    }
}