using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectCriticalControlPointModel
    {
        public long ProjectCriticalControlPointId { get; set; }
        public long ProjectMasterId { get; set; }
        [Required]
        public string Earphone { get; set; }
        [Required]
        public string Charger { get; set; }
        [Required]
        public string Battery { get; set; }
        [Required]
        public string BackCoverMaterial { get; set; }
        [Required]
        public string BackCoverFinishing { get; set; }
        [Required]
        public string LogoPrintType { get; set; }
        [Required]
        public string Flash { get; set; }
        [Required]
        public string OtgCable { get; set; }
        [Required]
        public string FlipCover { get; set; }
        [Required]
        public string ThreeLayerScreenProOnPhone { get; set; }
        [Required]
        public string FreeScreenProOnGb { get; set; }
        [Required]
        public string BackSideScreenPro { get; set; }
        [Required]
        public string BackSideThermalPaper { get; set; }
        [Required]
        public string RawDesignFileOfId { get; set; }
        [Required]
        public string BsiSensor { get; set; }
        [Required]
        public string SarAndCcc { get; set; }
        [Required]
        public string BothSimFourG { get; set; }
        [Required]
        public string UsbCableLength { get; set; }
        [Required]
        public string BackCamera { get; set; }
        [Required]
        public string FronCamera { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}