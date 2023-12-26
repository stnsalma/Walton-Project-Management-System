using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models
{
    public class PhSensorAndOtherModel
    {
        public long PhSensorAndOthersInfoId { get; set; }
        public long ProjectMasterId { get; set; }
        [Required]
        public bool Gsensor { get; set; }
        [Required]
        public bool Psensor { get; set; }
        [Required]
        public bool Lsensor { get; set; }
        [Required]
        public bool Compass { get; set; }
        [Required]
        public bool Gyroscope { get; set; }
        [Required]
        public bool HallSensor { get; set; }
        [Required]
        public bool Otg { get; set; }
        [Required]
        public bool Gps { get; set; }
        public string SpecialSensor { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}