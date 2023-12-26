using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.ViewModels.Commercial
{
    public class VmImeiRange
    {
        public long ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public long PurchaseOrderFormId { get; set; }
        public long? PurchaseOrderQuantiy { get; set; }
        public long BabtRawId { get; set; }
        public long? TotalNocImeiQuantity { get; set; }
        public long? RemainingQuantity { get; set; }
        
        public long? AllocatedFrom { get; set; }
        [Required]
        public string SampleStartImei { get; set; }
        public string SampleEndImei { get; set; }
        [Required]
        public string TacNo { get; set; }
        public string NocNo { get; set; }
        public long RequestedQuantity { get; set; }
        public long GivenQuantity { get; set; }
    }
}