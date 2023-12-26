using System;

namespace ProjectManagement.ViewModels.ProjectCommercial
{
    public class VmTacRequest
    {
        public long ProjectMasterId { get; set; }
        public long? ProjectBabtId { get; set; }
        public string ProjectName { get; set; }
        public long AssignedId { get; set; }
        public long ProjectPurchaseFormOrderId { get; set; }
        public string PurchaseOrderNo { get; set; }
        public long? PurchaseOrderQuantity { get; set; }
        public string TacNo { get; set; }
        public DateTime? TacRequestDate { get; set; }
        public string ImeiRangeFrom { get; set; }
        public string ImeiRangeTo { get; set; }
        public DateTime? ToSupplierDate { get; set; }
        public long? RequestedImeiQuantity { get; set; }

        /// <summary>
        /// /custom property for project name
        /// </summary>
        public int? OrderNuber { get; set; }
        public string OrderNumberOrdinal { get; set; }
    }
}