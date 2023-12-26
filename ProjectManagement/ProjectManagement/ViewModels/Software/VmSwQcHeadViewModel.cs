using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Software
{
    public class VmSwQcHeadViewModel
    {
        public VmSwQcHeadViewModel()
        {
            ProjectMasterModel = new ProjectMasterModel();
            ProjectMasterModels = new List<ProjectMasterModel>();
            SwQcAssignsFromQcHeadModel = new SwQcAssignsFromQcHeadModel();
            SwQcAssignsFromQcHeadModels = new List<SwQcAssignsFromQcHeadModel>();
            SwQcAssignsFromQcHeadModel1 = new SwQcAssignsFromQcHeadModel();
            SwQcAssignsFromQcHeadModels1 = new List<SwQcAssignsFromQcHeadModel>();
            SwQcHeadAssignsFromPmModel = new SwQcHeadAssignsFromPmModel();
            SwQcHeadAssignsFromPmModels = new List<SwQcHeadAssignsFromPmModel>();
            SwQcIssueDetailModel=new SwQcIssueDetailModel();
            SwQcIssueDetailModels=new List<SwQcIssueDetailModel>();
            SwQcIssueDetailModel1 = new SwQcIssueDetailModel();
            SwQcIssueDetailModels1 = new List<SwQcIssueDetailModel>();
            SwQcPersonalUseFindingsIssueDetailModel=new SwQcPersonalUseFindingsIssueDetailModel();
            SwQcPersonalUseFindingsIssueDetailModels=new List<SwQcPersonalUseFindingsIssueDetailModel>();
            SwQcNewInnovationModel=new SwQcNewInnovationModel();
            SwQcNewInnovationModels=new List<SwQcNewInnovationModel>();
            ddlAssignUsersList=new List<CmnUserModel>();
        }
        public List<CmnUserModel> ddlAssignUsersList { get; set; }
        public ProjectMasterModel ProjectMasterModel { get; set; }
        public List<ProjectMasterModel> ProjectMasterModels { get; set; }
        public SwQcAssignsFromQcHeadModel SwQcAssignsFromQcHeadModel { get; set; }
        public List<SwQcAssignsFromQcHeadModel> SwQcAssignsFromQcHeadModels { get; set; }
        public SwQcAssignsFromQcHeadModel SwQcAssignsFromQcHeadModel1 { get; set; }
        public List<SwQcAssignsFromQcHeadModel> SwQcAssignsFromQcHeadModels1 { get; set; }
        public List<SwQcHeadAssignsFromPmModel> SwQcHeadAssignsFromPmModels { get; set; }
        public SwQcHeadAssignsFromPmModel SwQcHeadAssignsFromPmModel { get; set; }
        public SwQcIssueDetailModel SwQcIssueDetailModel { get; set; }
        public List<SwQcIssueDetailModel> SwQcIssueDetailModels { get; set; }
        public SwQcIssueDetailModel SwQcIssueDetailModel1 { get; set; }
        public List<SwQcIssueDetailModel> SwQcIssueDetailModels1 { get; set; }
        public SwQcPersonalUseFindingsIssueDetailModel SwQcPersonalUseFindingsIssueDetailModel { get; set; }
        public List<SwQcPersonalUseFindingsIssueDetailModel> SwQcPersonalUseFindingsIssueDetailModels { get; set; }
        public SwQcNewInnovationModel SwQcNewInnovationModel { get; set; }
        public List<SwQcNewInnovationModel> SwQcNewInnovationModels { get; set; }
        ////////// /// ////ProjectMasters//////

        
        public bool IsEdit { get; set; }
        public long ProjectMasterId { get; set; }
        public long ProjectTypeId { get; set; }
        public string ProjectName { get; set; }
        public string SupplierName { get; set; }
        public string SupplierModelName { get; set; }
        public int? NumberOfSample { get; set; }
        public System.DateTime? ApproxProjectFinishDate { get; set; }
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

        /// ////ProjectPmAssigns///

        public System.DateTime AssignDate { get; set; }
        public long AssignUserId { get; set; }
        public string ProjectHeadRemarks { get; set; }

        /// ////CmnUsers///
        public long CmnUserId { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }

        //SwQcAssignsFromQcHeadModel//
        public long SwQcAssignId { get; set; }
        public long? SwQcHeadAssignId { get; set; }
        public long? ProjectPmAssignId { get; set; }
        public long SwQcUserId { get; set; }
        public int? OrderNumber { get; set; }
        public string PoCategory { get; set; }

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

        //SwQcHeadAssignsFromPmModel//

        public long ProjectOrderShipmentId { get; set; }
        public long ProjectManagerUserId { get; set; }
        public long SwQcHeadUserId { get; set; }
        public string PmToQcHeadAssignComment { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? PmProjectFinishTime { get; set; }
        public string SwQcHeadToQcAssignComment { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? SwQcHeadToPmSubmitTime { get; set; }
        public string SwQcHeadToPmForwardComment { get; set; }
        public string AssignCatagory { get; set; }
        public string PriorityFromPm { get; set; }
        public string ProjectManagerSampleType { get; set; }
        public int? ProjectManagerSampleNo { get; set; }
        public bool? IsFinalPhaseMP { get; set; }
        public string IsFinalPhaseMPs { get; set; }
        public int? OrderNuber { get; set; }
        public string SwQcHeadStatus { get; set; }
        //new//
        public string ProjectAssignedBy { get; set; }
       
    }
}