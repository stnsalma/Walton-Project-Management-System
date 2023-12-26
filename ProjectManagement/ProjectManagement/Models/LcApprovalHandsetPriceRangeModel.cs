using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class LcApprovalHandsetPriceRangeModel
    {
        public long Id { get; set; }
        public decimal StartingRange { get; set; }
        public decimal FinishingRange { get; set; }
        public Nullable<long> AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string RangeFor { get; set; }
    }
}