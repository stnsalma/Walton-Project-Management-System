using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwQcAssignModel
    {
        public long HwQcAssignId { get; set; }
        [Required(ErrorMessage = "required")]

        public long ProjectMasterId { get; set; }
        public long HwAllTestId { get; set; }

        [Required(ErrorMessage = "required")]
        public long HwQcInchargeAssignId { get; set; }

        [Required]
        public long HwQcUserId { get; set; }
        public DateTime HwQcAssignDate { get; set; }
        public string QcDocUploadPath { get; set; }
        public HttpPostedFileBase QcDocUpload { get; set; }
        public string ImageExtension { get; set; }
        public DateTime? HwDocUploadDate { get; set; }
        public DateTime? QcSubmissionDate { get; set; }
        public long? VerifiedBy { get; set; }
        public string VerifierName { get; set; }
        public DateTime? VerificationDate { get; set; }
        public string Status { get; set; }
        [Required]
        public DateTime? DeadLineDate { get; set; }
    }
}