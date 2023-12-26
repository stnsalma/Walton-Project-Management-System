using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PhMemoryInfoModel
    {
        public long PhMemoryInfoId { get; set; }
        public long ProjectMasterId { get; set; }
        [Required]
        public string Ram { get; set; }
        [Required]
        public string Rom { get; set; }
        [Required]
        public string BrandName { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}