using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManagement.Models
{
    public class HwChipsetModel
    {
        public long ChipsetId { get; set; }
        public string ChipsetVendor { get; set; }

        [Required(ErrorMessage = "Required")]
        public string ChipsetCore { get; set; }

        [Required(ErrorMessage = "Required")]
        public string ChipsetSpeed { get; set; }

        [Required(ErrorMessage = "Required")]
        [Remote("ChipsetDuplicationCheckFor", "Hardware")]
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