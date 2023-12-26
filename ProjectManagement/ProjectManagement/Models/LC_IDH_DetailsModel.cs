using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class LC_IDH_DetailsModel
    {
        public long Id { get; set; }
        public Nullable<long> VariantId { get; set; }
        public Nullable<long> LcIdhFinalBomId { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<long> OrderQuantity { get; set; }
        public Nullable<decimal> TotalValue { get; set; }
        public Nullable<int> OrderSerial { get; set; }
        public Nullable<long> AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }

        //custom
        public string MaterialCoding { get; set; }
        public string MaterialName { get; set; }
        public string InventoryCode { get; set; }
        public string Specification { get; set; }
        public string Vendor { get; set; }
        public int? TotalQuantity { get; set; }
        public int? TotalQuantityConsideringWastage { get; set; }
    }
}