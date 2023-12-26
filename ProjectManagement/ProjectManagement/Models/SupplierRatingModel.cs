using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models
{
    public class SupplierRatingModel
    {
        public long SupplierRatingId { get; set; }
        [Required(ErrorMessage = "Supplier is Required")]
        public long? SupplierId { get; set; }
        [Required(ErrorMessage = "Project/ Model is Required")]
        public long? ProjectMasterId { get; set; }
        [Required(ErrorMessage = "Required")]
        public decimal? ShipmentDeliveryPerformance { get; set; }
        [Required(ErrorMessage = "Required")]
        public decimal? AfterSalesSupport { get; set; }
        [Required(ErrorMessage = "Required")]
        public decimal? AfterSalesReturn { get; set; }
        [Required(ErrorMessage = "Required")]
        public decimal? CustomizationSupport { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Remarks { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}