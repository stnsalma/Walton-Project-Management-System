using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models
{
    public class PhTpLcdInfoModel
    {
        public long PhTpLcdInfoId { get; set; }
        [Required]
        public long ProjectMasterId { get; set; }
        [Required]
        public string DisplaySize { get; set; }
        [Required]
        public string DisplayResulution { get; set; }
        [Required]
        public string DisplaySpeciality { get; set; }
        [Required]
        public string TpVendor { get; set; }

        public string TpFinalVendor { get; set; }
        [Required]
        public string LcdVendor { get; set; }
        public string LcdFinalVendor { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}