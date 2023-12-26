using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.DAL.DbModel;

namespace ProjectManagement.Models
{
    public class SwQcInchargeAssignModel
    {
        public long SwQcInchargeAssignId { get; set; }
        public long ProjectPmAssignId { get; set; }
        public long ProjectMasterId { get; set; }
        public long ProjectOrderShipmentId { get; set; }
        public long ProjectManagerUserId { get; set; }
        public long SwQcInchargeUserId { get; set; }
        public System.DateTime ProjectManagerAssignToQcInTime { get; set; }
        public Nullable<System.DateTime> SwQcInchargeAssignTime { get; set; }
        public Nullable<System.DateTime> SwQcInchargeEndTime { get; set; }
        public string ProjectManagerAssignComment { get; set; }
        public string SwQcInchargeAssignComment { get; set; }
        public string SwQcInchargeEndComment { get; set; }
        public string AssignCatagory { get; set; }
        public int ProjectManagerSampleNo { get; set; }
        public Nullable<long> TestPhaseIDFromPm { get; set; }
        public string ProjectManagerSampleType { get; set; }
        public string ProjectManagerImeies { get; set; }
        public System.DateTime ApproxPmToQcDeliveryDate { get; set; }
        public string PriorityFromPm { get; set; }
        public string Status { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<System.DateTime> PausedDate { get; set; }
        public Nullable<long> PausedDone { get; set; }
        public Nullable<System.DateTime> RestartDate { get; set; }
        public Nullable<long> RestartDone { get; set; }
        public string PasuedReason { get; set; }
        public virtual ProjectPmAssign ProjectPmAssign { get; set; }
        public Nullable<long> TestPhaseID { get; set; }
        public string TestPhaseName { get; set; }
        public Nullable<System.DateTime> ApproxInchargeToQcDeliveryDate { get; set; }
        public Nullable<System.DateTime> QcInchargeToQcAssignTime { get; set; }
        public Nullable<System.DateTime> QcProjectFinisedTime { get; set; }
        public Nullable<System.DateTime> QcInchargeToPmProjectSubmitTime { get; set; }
        public Nullable<System.DateTime> QcInchargeToQcReAssignTime { get; set; }
        public Nullable<System.DateTime> QcReAssignProjectFinisedTime { get; set; }
        public Nullable<System.DateTime> QcInchargeToPmReAssignProjectSubmitTime { get; set; }

        public string ProjectName { get; set; }
        public int? OrderNuber { get; set; }
        public string OrderNumberOrdinal { get; set; }
    }
}