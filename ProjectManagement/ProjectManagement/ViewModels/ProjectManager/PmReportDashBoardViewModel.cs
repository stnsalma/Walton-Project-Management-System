using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.ProjectManager
{
    public class PmReportDashBoardViewModel
    {
        public PmReportDashBoardViewModel()
        {
            ProjectMasterModels = new List<ProjectMasterModel>();
            ProjectPmAssignModels = new List<ProjectPmAssignModel>();
            CmnUserModels = new List<CmnUserModel>();
            CmnUserModel= new CmnUserModel();
            PmBootImageAnimationModels = new List<PmBootImageAnimationModel>();
            PmGiftBoxModels=new List<PmGiftBoxModel>();
            PmLabelsModels=new List<PmLabelsModel>();
            PmIdModels=new List<PmIdModel>();
            PmScreenProtectorModels=new List<PmScreenProtectorModel>();
            PmWalpaperModels=new List<PmWalpaperModel>();
            PmSwCustomizationFinalModels=new List<PmSwCustomizationFinalModel>();
            PmPhnAccessoriesModels=new List<PmPhnAccessoriesModel>();
            PmPhnCameraModels=new List<PmPhnCameraModel>();
        }
        public List<ProjectMasterModel> ProjectMasterModels { get; set; }
        public List<ProjectPmAssignModel> ProjectPmAssignModels { get; set; }
        public List<CmnUserModel> CmnUserModels { get; set; }
        public CmnUserModel CmnUserModel { get; set; }
        public List<PmBootImageAnimationModel> PmBootImageAnimationModels { get; set; }
        public List<PmGiftBoxModel> PmGiftBoxModels { get; set; }
        public List<PmLabelsModel> PmLabelsModels { get; set; }
        public List<PmIdModel> PmIdModels { get; set; }
        public List<PmScreenProtectorModel> PmScreenProtectorModels { get; set; }
        public List<PmWalpaperModel> PmWalpaperModels { get; set; }
        public List<PmSwCustomizationFinalModel> PmSwCustomizationFinalModels { get; set; }
        public List<PmPhnAccessoriesModel> PmPhnAccessoriesModels { get; set; }
        public List<PmPhnCameraModel> PmPhnCameraModels { get; set; }
        /// <summary>
        /// /////////////////prjectmanager
        /// </summary>
        public long ProjectMasterId { get; set; }
        public long ProjectTypeId { get; set; }
        public string ProjectName { get; set; }
        public long? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierModelName { get; set; }
        public int? NumberOfSample { get; set; }
        public DateTime? ApproxProjectFinishDate { get; set; }
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
        public string RevisedStatus { get; set; }
        public string ManagentComment { get; set; }
        public decimal? ApproximatePrice { get; set; }
        public long? GivenSampleToScreening { get; set; }
        public decimal? FinalPrice { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? Date { get; set; }
        public string ProjectNameForScreening { get; set; }
        public string SourcingType { get; set; }
        public string ScreeningCommentFromCommercial { get; set; }
        public string PcbaVendorName { get; set; }
        public string PcbaFinalVendor { get; set; }

        public string DisplayResulution { get; set; }
        public string DisplaySpeciality { get; set; }
        public string TpVendor { get; set; }
        public string TpFinalVendor { get; set; }
        public string LcdVendor { get; set; }
        public string LcdFinalVendor { get; set; }
        public string HousingVendorName { get; set; }
        public string HousingFinalVendorName { get; set; }
        public string BackCam { get; set; }
        public string BackCamSensor { get; set; }
        public string BackCamBsi { get; set; }
        public string FrontCam { get; set; }
        public string FrontCamSensor { get; set; }
        public string FrontCamBsi { get; set; }
        public string CpuName { get; set; }
        public string ChipsetName { get; set; }
        public string ChipsetFrequency { get; set; }
        public int? ChipsetBit { get; set; }
        public string ChipsetCore { get; set; }
        public string MemoryBrandName { get; set; }
        public bool? Gsensor { get; set; }
        public bool? Psensor { get; set; }
        public bool? Lsensor { get; set; }
        public bool? Compass { get; set; }
        public bool? Gyroscope { get; set; }
        public bool? HallSensor { get; set; }
        public bool? Otg { get; set; }
        public bool? Gps { get; set; }
        public string SpecialSensor { get; set; }
        public decimal? EarphoneConfirmPrice { get; set; }
        public string EarphoneSupplierName { get; set; }
        public string ChargerRating { get; set; }
        public string ChargerSupplierName { get; set; }
        public bool? ThreeLayerScreenProtector { get; set; }
        public string BatteryCoverFinishingType { get; set; }
        public string BatteryCoverLogoType { get; set; }
        public bool? OtgCable { get; set; }
        public bool? FlashLight { get; set; }
        public string SecondGen { get; set; }
        public string ThirdGen { get; set; }
        public string FourthGenFdd { get; set; }
        public string FourthGenTdd { get; set; }
        public string Cdma { get; set; }
        public string BatteryRating { get; set; }
        public string BatteryType { get; set; }
        public string BatterySupplierName { get; set; }
        public string BateeryPossibleSupplierNames { get; set; }
        public decimal? OrderQuantity { get; set; }
        public string Color { get; set; }
        public Nullable<int> OrderNuber { get; set; }


        //custom properties
        public string AddedName { get; set; }

        public string PurchaseOrderNumber { get; set; }

        public string OrderNumberOrdinal { get; set; }

        ///////////////pmassigns

        public long ProjectPmAssignId { get; set; }
        public string PONumber { get; set; }
        public DateTime AssignDate { get; set; }
        public long AssignUserId { get; set; }
        public long ProjectManagerUserId { get; set; }
        public string ProjectHeadRemarks { get; set; }
        public string UserFullName { get; set; }
        public string Status { get; set; }
        public string ProjectHeadInactiveRemarks { get; set; }
        public DateTime? InactiveDate { get; set; }
        public DateTime? ApproxPmInchargeToPmFinishDate { get; set; }

        //////////////cmusers
        public long CmnUserId { get; set; }
             
        public string UserName { get; set; }
          
        public string EmployeeCode { get; set; }
        public string QcAssignedPersonID { get; set; }
        public string RoleName { get; set; }
        public long? AssignBy { get; set; }
       
        public DateTime? AssignStartDate { get; set; }
        public DateTime? AssignEndDate { get; set; }
        public string AssignRoles { get; set; }
      
        public string ExtendedRoleName { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Designation { get; set; }


        ////////////purchase order


        public long ProjectPurchaseOrderFormId { get; set; }
      
      
      
        public string Receiver { get; set; }
     
     
       
        public string CompanyName { get; set; }
      
        public string CompanyAddress { get; set; }
      
        public string Subject { get; set; }
      
        public string DescriptionHeader { get; set; }
      
        public string DescriptionBody { get; set; }
        public byte[] Signature { get; set; }
       
        public long? Quantity { get; set; }
      
      
        public string Value { get; set; }
      
        public DateTime? PoDate { get; set; }
        public bool? IsCompleted { get; set; }



        /////////////
        public string AllAssignedPmProjectStatus { get; set; }
        public string QcAssignedPerson { get; set; }
        public string AssignedPerson { get; set; }

        public string AssignDate1 { get; set; }

        public string InactiveDate1 { get; set; }

        /////////
        public long? SwQcInchargeAssignId { get; set; }
        public long? TestPhaseID { get; set; }
        public string TestPhaseName { get; set; }
    }
}