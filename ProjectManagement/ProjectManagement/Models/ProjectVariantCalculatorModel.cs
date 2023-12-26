using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectVariantCalculatorModel
    {
        public long Id { get; set; }
        public long? ProjectId { get; set; }
        public string VariantName { get; set; }
        public long? Quantity { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? UnassignedQuantity { get; set; }
        public bool? IsLocked { get; set; }
    }
}