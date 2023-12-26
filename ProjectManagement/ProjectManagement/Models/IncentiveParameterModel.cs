using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class IncentiveParameterModel
    {
        public long Id { get; set; }
        public string ParameterName { get; set; }
        public Nullable<decimal> Parameter { get; set; }
        public Nullable<decimal> ParameterValue { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string RoleName { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<long> Updated { get; set; }
    }
}