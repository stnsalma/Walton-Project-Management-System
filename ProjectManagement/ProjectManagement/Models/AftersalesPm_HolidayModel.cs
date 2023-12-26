using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class AftersalesPm_HolidayModel
    {
        public long Id { get; set; }
        public string HolidayName { get; set; }
        public Nullable<System.DateTime> Holiday_SDate { get; set; }
        public Nullable<System.DateTime> Holiday_EDate { get; set; }
        public string Month { get; set; }
        public Nullable<int> MonNum { get; set; }
        public Nullable<int> Year { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}