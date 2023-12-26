using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class FocClaimBomDetailModel
    {
        public long FocClaimId { get; set; }
        public long RawMaterialId { get; set; }
        public Nullable<long> ProjectMasterId { get; set; }
        public Nullable<long> ProjectPurchaseOrderFormId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public Nullable<int> Orders { get; set; }
        public string PoCategory { get; set; }
        public Nullable<long> PoQuantity { get; set; }
        public Nullable<int> LotNumber { get; set; }
        public Nullable<long> LotQuantity { get; set; }
        public string BOMType { get; set; }
        public string BOMName { get; set; }
        public string Color { get; set; }
        public string ItemQuantity { get; set; }
        public string BomRemarks { get; set; }
        public string ReceiveQuantity { get; set; }
        public string ReceiveRemarks { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}