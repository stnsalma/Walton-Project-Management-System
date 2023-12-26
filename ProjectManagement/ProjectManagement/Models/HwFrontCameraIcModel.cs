using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManagement.Models
{
    public class HwFrontCameraIcModel
    {
        public long FrontCameraIcId { get; set; }
        public string FrontCameraVendor { get; set; }
        [Remote("")]
        [Required(ErrorMessage = "required")]
        public string IcNoSize { get; set; }
        public string PinType { get; set; }
        public int? PinNumber { get; set; }
        public string Remarks { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}