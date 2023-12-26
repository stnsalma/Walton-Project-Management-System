using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectManagement.Models
{
    public class HwFlashIcModel
    {
        public long FlashIcId { get; set; }
        public string FlashIdVendor { get; set; }
        [Remote("")]
        public string IcNoSize { get; set; }
        public string PinType { get; set; }
        public int? PinNumber { get; set; }
        public string FlashIcTechnology { get; set; }
        public string FlashIcRam { get; set; }
        public string FlashIcRom { get; set; }
        public string FlashIcBall { get; set; }
        public string Remarks { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}