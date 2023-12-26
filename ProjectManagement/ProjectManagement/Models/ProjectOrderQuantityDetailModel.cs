using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectOrderQuantityDetailModel
    {
        public long Id { get; set; }
        public long? ProjectMasterId { get; set; }
        public long? ProjectPurchaseOrderFormId { get; set; }
        public string ProjectModel { get; set; }
        public string ProjectName { get; set; }
        public long? OrderQuantity { get; set; }
        public long? TotalOrderQuantity { get; set; }
        public long? AddedBy { get; set; }
        public string AddedByName { get; set; }
        public DateTime? AddedDate { get; set; }
        public bool BTRCPush { get; set; }
        public int? OrderNumber { get; set; }
        public bool? IsActive { get; set; }
        public long? UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string RamVendor { get; set; }
        public string RomVendor { get; set; }
        public long? VariantClosingBy { get; set; }
        public string VariantClosingByName { get; set; }
        public DateTime? VariantClosingDate { get; set; }
        public string ClosingRemarks { get; set; }
    }
}