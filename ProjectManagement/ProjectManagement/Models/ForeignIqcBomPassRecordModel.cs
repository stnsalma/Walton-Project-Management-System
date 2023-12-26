using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ForeignIqcBomPassRecordModel
    {
        public long Id { get; set; }
        public long? VariantId { get; set; }
        public long? ProjectId { get; set; }
        public long? BomId { get; set; }
        public string Description { get; set; }
        public string SpareDescription { get; set; }
        public string BOMType { get; set; }
        public string BomQuantity { get; set; }
        public string BomPassedQuantity { get; set; }
        public string BomFailQuantity { get; set; }
        public string Remarks { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? ForeignIqcId { get; set; }
    }
}