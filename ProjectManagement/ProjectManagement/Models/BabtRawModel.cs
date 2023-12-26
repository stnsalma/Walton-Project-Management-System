using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class BabtRawModel
    {
        public long BabtRawId { get; set; }
        [Required(ErrorMessage = "Project or Model is Required")]
        public long? ProjectMasterId { get; set; }
        [Required(ErrorMessage = "TAC is Required")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Should be exactly 8 digit")]
        public string TacNo { get; set; }
        [Required(ErrorMessage = "Request Date is Required")]
        public DateTime? RequestDate { get; set; }
        [Required(ErrorMessage = "Receive Date is Required")]
        public DateTime? ReceiveDate { get; set; }
        public long? TotalImei { get; set; }
        public long? RemainingImei { get; set; }
        public long? RegisterableFrom { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }


        //custom properties

        public string ProjectName { get; set; }
        public string AddedBy { get; set; }
    }
}