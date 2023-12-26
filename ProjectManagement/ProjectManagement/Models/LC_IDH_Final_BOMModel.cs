using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class LC_IDH_Final_BOMModel
    {
        public long Id { get; set; }
        public Nullable<long> VariantId { get; set; }
        public string variantName { get; set; }
        public string MaterialCoding { get; set; }
        public string MaterialName { get; set; }
        public string InventoryCode { get; set; }
        public string Specification { get; set; }
        public string Vendor { get; set; }
        public Nullable<int> TotalQuantity { get; set; }
        public Nullable<int> PerUnitQuantity { get; set; }
        public string UnitOfMeasurement { get; set; }
        public Nullable<decimal> ExtraOrderPerUnitQuantity { get; set; }
        public Nullable<int> ExtraOrderQuantity { get; set; }
        public Nullable<decimal> PerUnitQuantityConsideringWastage { get; set; }
        public Nullable<int> TotalQuantityConsideringWastage { get; set; }
        public string UsedIn { get; set; }
        public string Remarks { get; set; }
        public Nullable<long> AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<long> ProjectMasterId { get; set; }
        //custom
        public HttpPostedFileBase BomFile { get; set; }
        public Nullable<int> RemainingQuantity { get; set; }
    }
}