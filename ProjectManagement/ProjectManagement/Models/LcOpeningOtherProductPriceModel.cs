using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class LcOpeningOtherProductPriceModel
    {
        public long Id { get; set; }
        public string Model { get; set; }
        public Nullable<long> Quantity { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<long> AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}