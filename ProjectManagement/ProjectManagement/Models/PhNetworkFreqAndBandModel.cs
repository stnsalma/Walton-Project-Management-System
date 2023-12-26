using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models
{
    public class PhNetworkFreqAndBandModel
    {
        public long PhNetworkFreqAndBandsId { get; set; }
        public long ProjectMasterId { get; set; }
        [Required]
        public string SecondGen { get; set; }
        [Required]
        public string ThirdGen { get; set; }
        [Required]
        public string FourthGenFdd { get; set; }
        [Required]
        public string FourthGenTdd { get; set; }
        [Required]
        public string Cdma { get; set; }public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}