using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwQcAssignModel
    {
        public long SwQcAssignId { get; set; }
        public long? SwQcInchargeAssignId { get; set; }
        public long ProjectPmAssignId { get; set; }
        public long ProjectMasterId { get; set; }
        public long SwQcUserId { get; set; }
        public System.DateTime SwQcAssignTime { get; set; }
        public Nullable<System.DateTime> SwQcReceiveTime { get; set; }
        public Nullable<System.DateTime> SwQcEndTime { get; set; }
        public int QcReceivedSampleNo { get; set; }
        public string QcReceivedImeies { get; set; }
        public Nullable<System.DateTime> ReceiveDate { get; set; }
        public string SwInchargeAssignToQcComment { get; set; }
        public string SwQcAssignComment { get; set; }
        public string SwQcEndComment { get; set; }
        public Nullable<System.DateTime> ApproxInchargeToQcDeliveryDate { get; set; }
        public string PriorityFromIncharge { get; set; }
        public string InactiveReasonComment { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> QcInchargeToQcReAssignTime { get; set; }
        public Nullable<System.DateTime> QcReAssignProjectFinisedTime { get; set; }
        public int? OrderNuber { get; set; }
        public string OrderNumberOrdinal { get; set; }
    }
}