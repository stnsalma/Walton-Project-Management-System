using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PhChipsetInfoModel
    {
        public long PhChipsetInfoId { get; set; }
        public long ProjectMasterId { get; set; }
        [Required]
        public string ChipsetName { get; set; }
        [Required]
        public string ChipsetFrequency { get; set; }
        [Required]
        public int Bit { get; set; }
        [Required]
        public string Core { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}