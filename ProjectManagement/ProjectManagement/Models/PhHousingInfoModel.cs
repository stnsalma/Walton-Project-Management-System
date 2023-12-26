using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PhHousingInfoModel
    {
        public long PhHousingInfoId { get; set; }
        public long ProjectMasterId { get; set; }
        [Required]
        public string VendorName { get; set; }
        public string FinalVendorName { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}