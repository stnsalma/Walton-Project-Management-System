using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwQcAssignCustomMasterModel
    {
        public long HwQcAssignId { get; set; }
        [Required]
        public long ProjectMasterId { get; set; } //project assigned to qc
        public string ProjectName { get; set; }
        public string SupplierName { get; set; }
        public string SupplierModelName { get; set; } 

        [Required(ErrorMessage = "required")]
        public long HwQcInchargeAssignId { get; set; }
        [Required]
        public long HwQcInchargeUserId { get; set; } //user id of Incharge 
        public string UserFullName { get; set; }   //project assigned by Incharge name

        public DateTime HwQcInchargeAssignDate { get; set; }
        public long? ReceivedSampleQuantity { get; set; }
        public DateTime? SampleSetReceiveDate { get; set; }
        public DateTime? SampleSetSentDate { get; set; }
        public string ProjectManagerSampleType { get; set; }
        public int? ProjectManagerSampleNo { get; set; }
        public string ProjectManagerAssignComment { get; set; }
        public long? SentSampleQuantity { get; set; }
        public string ReceiveSampleRemark { get; set; }

        public bool IsScreeningTest { get; set; }
        public bool IsRunningTest { get; set; }
        public bool IsFinishedGoodTest { get; set; }
        public string Remark { get; set; }

        [Required]
        public long HwQcUserId { get; set; }
        public DateTime HwQcAssignDate { get; set; }
        public DateTime DeadLineDate { get; set; }
        public string QcDocUploadPath { get; set; }
        public string ImageExtension { get; set; }

        public HttpPostedFileBase HwQcDocUpload { get; set; }

        public DateTime? HwDocUploadDate { get; set; }
        public DateTime? QcSubmissionDate { get; set; }
        public long? VerifiedBy { get; set; }
        public string VerifierName { get; set; }
        public DateTime? VerificationDate { get; set; }
        public string Status { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public string Flag { get; set; }
        public int OrderNuber { get; set; }
    }
}