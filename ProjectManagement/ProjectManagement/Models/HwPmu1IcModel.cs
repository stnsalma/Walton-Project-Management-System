using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManagement.Models
{
    public class HwPmu1IcModel
    {
        public long Pmu_1_Id { get; set; }
        public string Pmu_1_Vendor { get; set; }
        [Required(ErrorMessage = "required")]
        [Remote("")]
        public string IcNoSize { get; set; }
        public string PinType { get; set; }
        public int? PinNumber { get; set; }
        public string NewItemNo { get; set; }
        public string ItemCode { get; set; }
        public string Remarks { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}