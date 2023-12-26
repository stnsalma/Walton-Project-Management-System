using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PmOrderQuantityWithColorModel
    {
        public long PmOrderQuantityWithColorId { get; set; }
        public long ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public int? OrderNumber { get; set; }
        public string Color { get; set; }
        public long PmOrderQuantity { get; set; }
        public string ConcernPmComment { get; set; }
        public long? AddedBy { get; set; }
        public string AddedByName { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? InventoryReceivedQuantity { get; set; }
        public long? InventoryReceivedAddedBy { get; set; }
        public string InventoryReceivedAddedName { get; set; }
        public DateTime? InventoryReceivedDate { get; set; }
        public long? CompleteProductionQuantity { get; set; }
        public long? CompletedQuantityAddedBy { get; set; }
        public string CompletedQuantityAddedName { get; set; }
        public DateTime? CompletedQuantityAddedDate { get; set; }
        public long? WarehouseReceivedQuantity { get; set; }
        public long? WarehouseQuantityAddedBy { get; set; }
        public string WarehouseQuantityAddedByName { get; set; }
        public DateTime? WarehouseQuantityAddedDate { get; set; }
        public long? ServiceCenterQuantity { get; set; }
        public long? ServiceCenterQuantityAddedBy { get; set; }
        public string ServiceCenterQuantityAddedByName { get; set; }
        public DateTime? ServiceCenterQuantityAddedDate { get; set; }
        public string ProductionTeamComment { get; set; }
    }
}