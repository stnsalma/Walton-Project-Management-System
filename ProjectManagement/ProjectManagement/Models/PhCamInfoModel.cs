using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PhCamInfoModel
    {
        public long PhCamInfoId { get; set; }
        public long ProjectMasterId { get; set; }
        [Required]
        public string BackCam { get; set; }
        [Required]
        public string BackCamSensor { get; set; }
        [Required]
        public string BackCamBsi { get; set; }
        [Required]
        public string FrontCam { get; set; }
        [Required]
        public string FrontCamSensor { get; set; }
        [Required]
        public string FrontCamBsi { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}