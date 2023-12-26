using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProductionPlanRemarkModel
    {
        public long Id { get; set; }
        public bool? IsCkd { get; set; }
        public bool? IsCharger { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> ProductionDate { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}