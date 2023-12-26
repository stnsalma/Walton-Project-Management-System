using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class IncentiveShareModel
    {
        public long Id { get; set; }
        public string EmployeeCode { get; set; }
        public Nullable<int> Share { get; set; }
        public Nullable<long> CarryAmount { get; set; }
    }
}