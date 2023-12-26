using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;
using ProjectManagement.Models.AssignModels;

namespace ProjectManagement.ViewModels.Software
{
    public class VmSwInchargeViewModel
    {

        public VmSwInchargeViewModel()
        {
            ProjectMasterModel = new ProjectMasterModel();
            ProjectMasterModelsList = new List<ProjectMasterModel>();
            SwQcStartUpModels = new List<SwQcStartUpModel>();
            SwQcCallSettingModels = new List<SwQcCallSettingModel>();
            SwQcMessageModels= new List<SwQcMessageModel>();
            SwQcToolsCheckModels = new List<SwQcToolsCheckModel>();
            SwQcCameraModels= new List<SwQcCameraModel>();
            SwQcDisplayLoopModels=new List<SwQcDisplayLoopModel>();
            SwQcDisplayModels=new List<SwQcDisplayModel>();
            SwQcSettingModels= new List<SwQcSettingModel>();
            SwQcMultimediaModels = new List<SwQcMultimediaModel>();
            SwQcGoogleServiceModels = new List<SwQcGoogleServiceModel>();
            SwQcStorageCheckModels = new List<SwQcStorageCheckModel>();
            SwQcGameModels = new List<SwQcGameModel>();
            SwQcTestingAppModels = new List<SwQcTestingAppModel>();
            SwQcFileManagerModels = new List<SwQcFileManagerModel>();
            SwQcConnectivityModels = new List<SwQcConnectivityModel>();
            SwQcShutDownModels = new List<SwQcShutDownModel>();
            SwQcProjectWiseIssueViewModels = new List<SwQcProjectWiseIssueViewModel>();
            SwQcBatteryAssignIssueModelsList = new List<SwQcBatteryAssignIssueModel>();
            SwCustomModelForDashboards =new List<SoftwareCustomModelForDashboard>();

            ddlTestPhasesList= new List<TestPhaseModel>();
        }
        public List<SoftwareCustomModelForDashboard> SwCustomModelForDashboards { get; set; }
        public List<TestPhaseModel> ddlTestPhasesList { get; set; }

        public List<PmQcAssignModel> PmQcAssignModels { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public String ddlAssignUserId { get; set; }
        public List<CmnUserModel> ddlAssignUsersList { get; set; }
        public string Tabname { get; set; }
        public ProjectMasterModel ProjectMasterModel { get; set; }
        public List<ProjectMasterModel> ProjectMasterModelsList { get; set; }
        public List<SwQcStartUpModel> SwQcStartUpModels { get; set; }
        public List<SwQcCallSettingModel> SwQcCallSettingModels { get; set; }
        public List<SwQcMessageModel> SwQcMessageModels { get; set; }
        public List<SwQcToolsCheckModel> SwQcToolsCheckModels { get; set; }
        public List<SwQcCameraModel> SwQcCameraModels { get; set; }
        public List<SwQcDisplayLoopModel> SwQcDisplayLoopModels { get; set; }
        public List<SwQcDisplayModel> SwQcDisplayModels { get; set; }
        public List<SwQcSettingModel> SwQcSettingModels { get; set; }
        public List<SwQcMultimediaModel> SwQcMultimediaModels { get; set; }
        public List<SwQcGoogleServiceModel> SwQcGoogleServiceModels { get; set; }
        public List<SwQcStorageCheckModel> SwQcStorageCheckModels { get; set; }
        public List<SwQcGameModel> SwQcGameModels { get; set; }
        public List<SwQcTestingAppModel> SwQcTestingAppModels { get; set; }
        public List<SwQcFileManagerModel> SwQcFileManagerModels { get; set; }
        public List<SwQcConnectivityModel> SwQcConnectivityModels { get; set; }
        public List<SwQcShutDownModel> SwQcShutDownModels { get; set; }
        public List<SwQcProjectWiseIssueViewModel> SwQcProjectWiseIssueViewModels { get; set; } 
        public List<SwQcBatteryAssignIssueModel> SwQcBatteryAssignIssueModelsList { get; set; }
 
       /// <summary>
        /// ////SwQcInchargeAssigns////
       /// </summary>
        public bool IsEdit { get; set; }
        public long SwQcInchargeAssignId { get; set; }
        public long ProjectPmAssignId { get; set; }
        public long SwProjectMasterId { get; set; }
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
        public string ProjectManagerImeies { get; set; }
        public System.DateTime ApproxPmToQcDeliveryDate { get; set; }
        public string PriorityFromPm { get; set; }
    

        public virtual ProjectPmAssign ProjectPmAssign { get; set; }


        ////////// /// ////ProjectMasters//////
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
       
        /////SwQcAssigns////

        public long SwQcAssignId { get; set; }
        public Nullable<long> SwQcAssignSwQcInchargeAssignId { get; set; }
        public Nullable<long> SwQcAssignProjectPmAssignId { get; set; }
        public long SwQcAssignProjectMasterId { get; set; }
        public Nullable<long> SwQcUserId { get; set; }
        public Nullable<System.DateTime> SwQcAssignTime { get; set; }
        public Nullable<System.DateTime> SwQcReceiveTime { get; set; }
        public Nullable<System.DateTime> SwQcEndTime { get; set; }
        public Nullable<int> QcReceivedSampleNo { get; set; }
        public string QcReceivedImeies { get; set; }
        public Nullable<System.DateTime> ReceiveDate { get; set; }
        public string SwInchargeAssignToQcComment { get; set; }
        public string SwQcAssignComment { get; set; }
        public string SwQcEndComment { get; set; }
        public Nullable<System.DateTime> ApproxInchargeToQcDeliveryDate { get; set; }
        public string PriorityFromIncharge { get; set; }
        public Nullable<long> SwQcAssignAdded { get; set; }
        public Nullable<System.DateTime> SwQcAssignAddedDate { get; set; }
        public Nullable<long> SwQcAssignUpdated { get; set; }
        public Nullable<System.DateTime> SwQcAssignUpdatedDate { get; set; }
        public List<SwQcAssignModel> SwQcAssignModels { get; set; }


        public int? OrderNuber { get; set; }
        public string OrderNumberOrdinal { get; set; }
        public string TestPhaseName { get; set; }
        public long TestPhaseID { get; set; }
        public string QcStatus { get; set; }
    }
}