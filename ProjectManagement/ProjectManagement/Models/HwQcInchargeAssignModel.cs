using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models
{
    public class HwQcInchargeAssignModel
    {
        [Required]
        public long HwQcInchargeAssignId { get; set; }
        [Required]
        public long ProjectMasterId { get; set; }
        [Required]
        public long HwQcInchargeUserId { get; set; }
        [Required]
        public long? HwQcInchargeAssignedBy { get; set; }
        public DateTime? HwQcInchargeAssignDate { get; set; }
        public long? ReceivedSampleQuantity { get; set; }
        public string ReceiveSampleRemark { get; set; }
        public long? SentSampleQuantity { get; set; }
        public string SentRemark { get; set; }
        public long? ReturnedSampleQuantuty { get; set; }
        public bool? IsScreeningTest { get; set; }
        public bool? IsRunningTest { get; set; }
        public bool? IsFinishedGoodTest { get; set; }
        public string TestPhase { get; set; }
        public DateTime? SampleSetReceiveDate { get; set; }
        public DateTime? SampleSetSentDate { get; set; }
        public DateTime? ForwardDate { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? ProjectPmAssignId { get; set; }
        public long? ProjectOrderShipmentId { get; set; }
        public long? ProjectManagerUserId { get; set; }
        public string ProjectManagerAssignComment { get; set; }
        public string ProjectManagerSampleType { get; set; }
        public int? ProjectManagerSampleNo { get; set; }
        public DateTime? ApproxPmToHwDeliveryDate { get; set; }
        public string PriorityFromPm { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }
}