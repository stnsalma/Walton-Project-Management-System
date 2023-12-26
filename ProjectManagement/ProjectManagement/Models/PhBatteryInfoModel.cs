using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PhBatteryInfoModel
    {
        [Required]
        public long PhBatteryInfoId { get; set; }
        [Required]
        public long ProjectMasterId { get; set; }
        [Required]
        public string BatteryRating { get; set; }
        [Required]
        public string BatteryType { get; set; }
        public string BatterySupplierName { get; set; }
        [Required]
        public string SupplierNames { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}