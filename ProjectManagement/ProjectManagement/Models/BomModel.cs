using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class BomModel
    {
        public long Id { get; set; }
        public long? BomProductModelId { get; set; }
        public string InventoryItemId { get; set; }
        public string InventoryItemCode { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public string Component { get; set; }
        public decimal? RequiredPerUnit { get; set; }
        public string SpareItemCode { get; set; }
        public string SpareDescription { get; set; }
        public string ItemType { get; set; }
        public string AssemblyCode { get; set; }
        public string Company { get; set; }
        public string Uom { get; set; }
        public string ProductType { get; set; }
        public string Color { get; set; }
        public string BOMType { get; set; }
        public DateTime? AddedDate { get; set; }
        public string AddedBy { get; set; }
        public decimal? ItemCost { get; set; }
        //Order quantity of project is the bom quantity here
        public string BomQuantity { get; set; }
    }
}