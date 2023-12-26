using System;

namespace ProjectManagement.Models
{
    public class PhColorInfoModel
    {
        public long PhColorInfoId { get; set; }
        public long ProjectMasterId { get; set; }
        public decimal Quantity { get; set; }
        public string Color { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}