using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ColorWiseVariantQuantityModel
    {
        public long Id { get; set; }
        public long? VariantId { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Color { get; set; }
        public long? Quantity { get; set; }
        public string Remarks { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}