using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Commercial
{
    public class Custom_Warehouse_Details
    {
        public long ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public int OrderNumber { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public long Quantity { get; set; }
        public long WarehouseQuantity { get; set; }
        public DateTime WarehouseDate { get; set; }
        public long ProjectPurchaseOrderFormId { get; set; }
        public long ProjectOrderShipmentId { get; set; }
        public DateTime ShipmentDate { get; set; }
    }
}