using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SoftwareCustomModelForDashboard
    {
        public string AssignedPerson { get; set; }
        public string AssignUserName { get; set; }
        ////ProjectMasterModel////
        
        public string QcStatus { get; set; }
        public string EmployeeCode { get; set; }
        public long ProjectTypeId { get; set; }
        public string ProjectName { get; set; }
        public string SupplierName { get; set; }
        public string SupplierModelName { get; set; }
        public int? NumberOfSample { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime ApproxProjectFinishDate { get; set; }
        public string SupplierTrustLevel { get; set; }
        public bool? IsScreenTestComplete { get; set; }
        public bool? IsApproved { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ApproxProjectOrderDate { get; set; }
        public DateTime? ApproxShipmentDate { get; set; }
        public bool? IsNew { get; set; }
        public bool? IsProjectManagerAssigned { get; set; }
        public string ProjectType { get; set; }
        public bool? IsReorder { get; set; }
        public string OsName { get; set; }
        public string OsVersion { get; set; }
        public decimal? DisplaySize { get; set; }
        public string DisplayName { get; set; }
        public string ProcessorName { get; set; }
        public decimal? ProcessorClock { get; set; }
        public string Chipset { get; set; }
        public string FrontCamera { get; set; }
        public string BackCamera { get; set; }
        public string Ram { get; set; }
        public string Rom { get; set; }
        public string Battery { get; set; }
        public int? SimSlotNumber { get; set; }
        public string SlotType { get; set; }
        public string ProjectStatus { get; set; }
        public string ManagentComment { get; set; }
        ///CmnUserModel/////
        public long CmnUserId { get; set; }
        public string ProjectManagerUserName { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public string QcInchargeUserName { get; set; }
        ////ProjectType////
       
        public string TypeName { get; set; }
        public long TestPhaseID { get; set; }
        public string TestPhaseName { get; set; }
        public string AllAssignedQcsProjectStatus { get; set; }
        public string QcAssignedPerson { get; set; }
        public string QcAssignedPersonID { get; set; }
        public int? OrderNuber { get; set; }
        public string OrderNumberOrdinal { get; set; }
        public long SwQcHeadAssignId { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? PmToQcHeadAssignTime { get; set; }
        public DateTime? SwQcHeadToQcAssignTime { get; set; }
        public string SoftwareVersionName { get; set; }
        public int? SoftwareVersionNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? PausedDate { get; set; }

        public string SourcingType { get; set; }
        public DateTime? SwQcFinishedTime { get; set; }
        public long SwQcAssignId { get; set; }
        public long ProjectMasterId { get; set; }
        public long ProjectPmAssignId { get; set; }
        public long SwQcUserId { get; set; }
        public int? OrderNumber { get; set; }
        public string PoCategory { get; set; }
      
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? SwQcStartTime { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? SwQcEndTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? ApproxInchargeToQcDeliveryDate { get; set; }
        public string PriorityFromQcHead { get; set; }
        public string Status { get; set; }
        public int? QcReceivedSampleNo { get; set; }
        public string QcReceivedImeies { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public long? Added { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string FieldTestFrom { get; set; }
      
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? SwQcHeadToPmSubmitTime { get; set; }
        public string SwQcHeadToPmForwardComment { get; set; }
        public bool? IsFinalPhaseMP { get; set; }
        public string IsFinalPhaseMPs { get; set; }
        public string SwQcHeadToQcAssignComment { get; set; }
        ////
        
        public long ProjectOrderShipmentId { get; set; }
        public long ProjectManagerUserId { get; set; }
        public long SwQcHeadUserId { get; set; }
        
        public string PmToQcHeadAssignComment { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? PmProjectFinishTime { get; set; }

        public string AssignCatagory { get; set; }
        public string PriorityFromPm { get; set; }
        public string ProjectManagerSampleType { get; set; }
        public int ProjectManagerSampleNo { get; set; }
        public string SwQcHeadStatus { get; set; }
    }
}