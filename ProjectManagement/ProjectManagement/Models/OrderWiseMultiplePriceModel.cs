using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class OrderWiseMultiplePriceModel
    {
        public long Id { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int? OrderNumber { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? OrderQuantity { get; set; }
        public decimal? Price { get; set; }
        public string Remarks { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? UpdatedBy { get; set; }
    }
}