using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models
{
    public class PhAccessoryModel
    {
        public long PhAccessoriesId { get; set; }
        public long ProjectMasterId { get; set; }
        [Required]
        public decimal EarphoneConfirmPrice { get; set; }
        [Required]
        public string EarphoneSupplierName { get; set; }
        [Required]
        public string ChargerRating { get; set; }
        [Required]
        public string ChargerSupplierName { get; set; }
        [Required]
        public bool ThreeLayerScreenProtector { get; set; }
        [Required]
        public string BatteryCoverFinishingType { get; set; }
        [Required]
        public string BatteryCoverLogoType { get; set; }
        [Required]
        public bool OtgCable { get; set; }
        [Required]
        public bool FlashLight { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}