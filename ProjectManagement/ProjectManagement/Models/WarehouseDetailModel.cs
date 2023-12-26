using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class WarehouseDetailModel
    {
        public long Id { get; set; }
        public Nullable<long> ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public Nullable<int> OrderNumber { get; set; }
        public Nullable<long> ProjectOrderShipmentId { get; set; }
        public Nullable<System.DateTime> ShipmentDate { get; set; }
        public Nullable<long> ProjectPurchaseOrderFormId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public Nullable<long> Quantity { get; set; }
        public Nullable<long> WarehouseQuantity { get; set; }
        public Nullable<System.DateTime> WarehouseDate { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}