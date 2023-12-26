using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class FinishGoodVariantModel
    {
        public long Id { get; set; }
        public long? ProjectMasterId { get; set; }
        public long? ProjectOrderShipmentId { get; set; }
        public long? FinishGoodProjectMasterId { get; set; }
        public string FinishGoodModel { get; set; }
        public int? FinishGoodModelOrderNumber { get; set; }
        public int? ApproxFinishGoodManufactureQty { get; set; }
        public long? ProjectPurchaseOrderFormId { get; set; }
        public DateTime? WarehouseEntryDate { get; set; }
        public string FinishGoodCheck { get; set; }
        public string AddedByName { get; set; }
        public string ProjectName { get; set; }
        public DateTime? PoDate1 { get; set; }
        public int? PoCount { get; set; }
        public int? PoWiseShipmentNumber { get; set; }
        public string PoOrdinal { get; set; }
        public string ShipmentNoOrdinal { get; set; }
        public long? Added { get; set; }
        public long? OrderQuantity { get; set; }
        public DateTime? AddedDate { get; set; }
        public string PoNo { get; set; }
        public string Remarks { get; set; }
    }
}