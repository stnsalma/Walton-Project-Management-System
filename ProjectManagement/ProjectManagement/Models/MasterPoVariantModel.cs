using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class MasterPoVariantModel
    {
        public string ProjectName { get; set; }
        public string SourcingType { get; set; }
        public long? PoQuantity { get; set; }
        public int? Produced { get; set; }
        public long? UnProduced { get; set; }
        public decimal? UnproducedPercentage { get; set; }
        public DateTime? PoDate { get; set; }
        public int? OrderNumber { get; set; }
        public string VariantName { get; set; }
        public long? VariantQuantity { get; set; }
    }
}