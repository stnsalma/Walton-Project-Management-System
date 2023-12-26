using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectMasterModel
    {
        public ProjectMasterModel()
        {
            AccessoriesPrices = new List<AccessoriesPricesModel>();
            ProjectImageModels = new List<ProjectImageModel>();
        }
        public List<HttpPostedFileBase> FileId { get; set; }
        public int IsRemoved { get; set; }
        public long ProjectMasterId { get; set; }
        public long? ProjectPurchaseOrderFormId { get; set; }
        public long ProjectTypeId { get; set; }
        [Required(ErrorMessage = "ProjectName is Required")]
        public string ProjectName { get; set; }
        public string ProjectModel { get; set; }
        public long? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierModelName { get; set; }
        public int? NumberOfSample { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ApproxProjectFinishDate { get; set; }
        public string SupplierTrustLevel { get; set; }
        public bool? IsScreenTestComplete { get; set; }
        public bool? IsApproved { get; set; }
        public bool IsActive { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ApproxProjectOrderDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ApproxShipmentDate { get; set; }
        public DateTime? LSD { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime? ChainaInspectionDate { get; set; }
        public DateTime? ShipmentTaken { get; set; }
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
        public decimal? PriceBDT { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedName { get; set; }
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
        public long? OrderQuantities { get; set; }
        public string Color { get; set; }
        public int? OrderNuber { get; set; }
        public DateTime? InitialApprovalDate { get; set; }
        public long? InitialApprovalBy { get; set; }
        public DateTime? FinalApprovalDate { get; set; }
        public long? FinalApprovalBy { get; set; }
        public string BackCamAutoFocus { get; set; }
        public string FrontCamAutoFocus { get; set; }
        public string CameraVendor { get; set; }
        public string RamVendor { get; set; }
        public string RomVendor { get; set; }
        public string AddedName { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string OrderNumberOrdinal { get; set; }
        public long? TestPhaseID { get; set; }
        public string TestPhaseName { get; set; }
        public long? SwQcInchargeAssignId { get; set; }
        public string PoCategory { get; set; }
        public string ProjectActualName { get; set; }
        public DateTime? PoDate { get; set; }
        public string SoftwareVersionName { get; set; }
        public int? SoftwareVersionNo { get; set; }
        public long SwQcAssignId { get; set; }
        public long SwQcHeadAssignId { get; set; }
        public DateTime? ApproxInchargeToQcDeliveryDate { get; set; }
        public string SwQcHeadStatus { get; set; }
        public long SwQcUserId { get; set; }
        public string Status { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? SwQcHeadToQcAssignTime { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? PmToQcHeadAssignTime { get; set; }
        public string ProjectNames { get; set; }
        public List<AccessoriesPricesModel> AccessoriesPrices { get; set; }
        public List<ProjectImageModel> ProjectImageModels { get; set; }
        //penalties
        public DateTime? PenaltiesDate { get; set; }
        public Decimal SubCategoryQuantity { get; set; }
        public Decimal Activated { get; set; }
        public Decimal IssuePercentage { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public int MonNum { get; set; }
        public string ProblemName { get; set; }
        public string SubCategory { get; set; }
        public Decimal TotalIssuePercentage { get; set; }
        public Decimal TotalSum { get; set; }
        public DateTime EffectiveMonDate { get; set; }
        public string ProjectManagerAssigned { get; set; }
        public string ProjectClosingStatus { get; set; }
        public DateTime? ScreeningIssueReviewDate { get; set; }
        public DateTime? ProjectClosingDate { get; set; }
        public string ProjectClosedBy { get; set; }

        public string ApprovalReviewRemarks { get; set; }

        public string Motherboard { get; set; }
        public string MotherboardModel { get; set; }
        public string MarketingName { get; set; }
        public string ApplicationRef { get; set; }
        public string BatteryCapacityTested { get; set; }
        public string ChargerAdapterType { get; set; }
        public string NFC { get; set; }
        public string BlueTooth { get; set; }
        public string WLAN { get; set; }
        public string DataSpeed { get; set; }
        public string SARValue { get; set; }
        public string CameraResulution { get; set; }
        public string RadioInterface { get; set; }
        public string FourthGen { get; set; }
        public string MarketingPeriod { get; set; }
        public string SerialNo { get; set; }
        public string ShipmentMode { get; set; }
        public decimal TotalPrice { get; set; }
        public long ModelID { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? AssignDate { get; set; }
        public string UserFullName { get; set; }
        public long? ProjectManagerUserId { get; set; }

        //
        public long RawMaterialId { get; set; }
        public int? Orders { get; set; }
        public long? PoQuantity { get; set; }
        public long? TotalQuantity { get; set; }
        public DateTime? ProjectManagerClearanceDate { get; set; }
        public string ChinaIqcPassHundredPercent { get; set; }
        public int? NoOfTimeInspection { get; set; }
        public string ManagementApproval { get; set; }
        public DateTime? ManagementApprovalDate { get; set; }
        public string SupportingDocument { get; set; }
        public string BOMType { get; set; }
        public string BOMName { get; set; }
        public string Remarks { get; set; }
        public string ItemQuantity { get; set; }
        public int? LotNumber { get; set; }
        public long? LotQuantity { get; set; }
        public string InspectionRemarks { get; set; }
        public string BomRemarks { get; set; }
        public string SourcingApproval { get; set; }
        public string LastOrderPmName { get; set; }
        public DateTime? LastAssignDate { get; set; }

        public string MajorDelayReason { get; set; }
        public string HardwareSampleReceive { get; set; }
        public string InspectionMajorFailItems { get; set; }
        public string OrderColorRatioWithQty { get; set; }
        public DateTime? InspectionStartingDate { get; set; }
        public string FinishGoodModel { get; set; }
        public int ApproxFinishGoodManufactureQty { get; set; }
        public bool? IsFinallyClosed { get; set; }
        public decimal? AnotherPrice { get; set; }
        public string PricingRemarks { get; set; }
        public DateTime? SwotAnalysisDate { get; set; }
        public long? SwotAnalysisBy { get; set; }
        public string SwotOpportunityRemarks { get; set; }
        public string BrandName { get; set; }
        public long? BrandId { get; set; }
        public long? DeactivatedBy { get; set; }
        public DateTime? DeactivationDate { get; set; }
        public long? ActivationBy { get; set; }
        public DateTime? ActivationDate { get; set; }
        public string ActivationDeactivationRemarks { get; set; }
        public DateTime? PsApprovalDate { get; set; }
        public long? PsApprovalBy { get; set; }
        public string PsApprovalByName { get; set; }
        public long? CeoApprovalBy { get; set; }
        public string CeoApprovalByName { get; set; }
        public DateTime? CeoApprovalDate { get; set; }
        public string PsRemarks { get; set; }
        public string CeoRemarks { get; set; }
        public bool? ChecklistEditPermission { get; set; }
        public long? BiApprovalBy { get; set; }
        public DateTime? BiApprovalDate { get; set; }
        public string BiRemarks { get; set; }
        public string InitialApprovalRemarks { get; set; }
        public string AddOrUpdateRemarks { get; set; }
        public DateTime? MarketClearanceDate { get; set; }
    }
}