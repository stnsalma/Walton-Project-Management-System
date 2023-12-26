using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SpareOrderByMultipleModelModel
    {
        public long SpareOrderByMultipleModelId { get; set; }
        public string ModelNames { get; set; }
        public string SpareName { get; set; }
        public string MaterialCode { get; set; }
        public long? OrderQuantity { get; set; }
        public double? Price { get; set; }
        public double? Amount { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}