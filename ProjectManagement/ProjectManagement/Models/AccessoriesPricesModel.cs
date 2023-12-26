using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class AccessoriesPricesModel
    {
        public long Id { get; set; }
        public long ProjectMasterId { get; set; }
        public string AccessoryName { get; set; }
        public string Price { get; set; }
        public string Vendor { get; set; }
        public string Currency { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Type { get; set; }
        public string Duty { get; set; }
        public string TotalPrice { get; set; }

        //custom 
        public decimal USDValue { get; set; }
    }
}