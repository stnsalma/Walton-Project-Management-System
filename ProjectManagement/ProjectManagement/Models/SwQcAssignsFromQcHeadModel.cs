using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwQcAssignsFromQcHeadModel
    {
        public string ProjectAssignedBy { get; set; }
        public long SwQcAssignId { get; set; }
        public long SwQcHeadAssignId { get; set; }
        public long ProjectMasterId { get; set; }
        public long ProjectPmAssignId { get; set; }
        public long SwQcUserId { get; set; }
        public int? OrderNumber { get; set; }
        public string ProjectName { get; set; }
        public string PoCategory { get; set; }
        public string ProjectType { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? SwQcHeadToQcAssignTime { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? SwQcStartTime { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? SwQcEndTime { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? SwQcFinishedTime { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public System.DateTime PmToQcHeadAssignTime { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? ApproxInchargeToQcDeliveryDate { get; set; }
        public string PriorityFromQcHead { get; set; }
        public string Status { get; set; }
        public string SoftwareVersionName { get; set; }
        public int? SoftwareVersionNo { get; set; }
        public int? QcReceivedSampleNo { get; set; }
        public string QcReceivedImeies { get; set; }
        public string SwQcAssignTimeByHead { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public long? Added { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string OrderNumberOrdinal { get; set; }
        public string AssignedPerson { get; set; }
        public string FieldTestFrom { get; set; }
        public long? TestPhaseID { get; set; }
        public string TestPhaseName { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? SwQcHeadToPmSubmitTime { get; set; }
        public string SwQcHeadToPmForwardComment { get; set; }
        public bool? IsFinalPhaseMP { get; set; }
        public string IsFinalPhaseMPs { get; set; }
        public string SwQcHeadToQcAssignComment { get; set; }
        public string FieldTestID { get; set; }
        public string AccessoriesTestType { get; set; }
    }
}