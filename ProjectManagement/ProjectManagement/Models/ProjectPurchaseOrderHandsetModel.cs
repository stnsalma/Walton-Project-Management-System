using System;

namespace ProjectManagement.Models
{
    public class ProjectPurchaseOrderHandsetModel
    {
        public long ProjectPurchaseOrdeHandsetId { get; set; }
        public long ProjectPurchaseOrderFormId { get; set; }
        public long? SerialNo { get; set; }
        public string Model { get; set; }
        public int? OrderQuantity { get; set; }
        public string Color { get; set; }
        public string Value { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}