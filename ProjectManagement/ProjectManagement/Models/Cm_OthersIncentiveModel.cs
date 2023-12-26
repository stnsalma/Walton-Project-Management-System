using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class Cm_OthersIncentiveModel
    {
        public long Id { get; set; }
        public string OthersType { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Remarks { get; set; }
        public Nullable<decimal> DeductAmount { get; set; }
        public Nullable<decimal> FinalAmount { get; set; }
        public string DeductRemarks { get; set; }
        public Nullable<System.DateTime> EffectiveMonth { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}