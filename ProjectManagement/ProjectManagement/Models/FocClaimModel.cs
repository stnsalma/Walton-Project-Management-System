using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class FocClaimModel
    {
        public long Id { get; set; }
        public long? BomProductModelId { get; set; }
        public string BomProductModel { get; set; }
        public string BomType { get; set; }
        public long? BomId { get; set; }
        public string Description { get; set; }
        public string SpareDescription { get; set; }
        public string ClaimQuantity { get; set; }
        public string ClaimedByName { get; set; }
        public long? ClaimedBy { get; set; }
        public DateTime? ClaimDate { get; set; }
        public string StrClaimDate { get; set; }
        public string ReceiveQuantity { get; set; }
        public long? ReceivedBy { get; set; }
        public string ReceivedByName { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string StrReceivedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? ProjectMasterId { get; set; }
        public long? OrderQuantityDetailId { get; set; }
        public int? OrderNo { get; set; }
        public long? OrderQuantity { get; set; }
    }
}