using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ProjectManagement.Infrastructures.Helper;

namespace ProjectManagement.Models
{
    public class ProjectModel
    {
        public long ProjectMasterId { get; set; }
        [Required(ErrorMessage = "Project type is Requirde")]
        public long ProjectTypeId { get; set; }
        //[Remote("CheckProjectName", "Commercial")]
        [RequiredIf("SourcingType", "ODM", ErrorMessage = "Project Name is required")]
        public string ProjectName { get; set; }
        [Required(ErrorMessage = "Supplier Name is Requirde")]
        public long? SupplierId { get; set; }
        public string SupplierName { get; set; }
        [Required(ErrorMessage = "Supplier model name is Requirde")]
        public string SupplierModelName { get; set; }
        public int? NumberOfSample { get; set; }

        [Required(ErrorMessage = "Approximate finish date is Required")]
        public DateTime? ApproxProjectFinishDate { get; set; }
        [Required(ErrorMessage = "Supplier trust level is Requirde")]
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
        [Required(ErrorMessage = "Display size is Requirde")]
        public decimal? DisplaySize { get; set; }
        public string DisplayName { get; set; }
        public string ProcessorName { get; set; }
        public decimal? ProcessorClock { get; set; }
        public string Chipset { get; set; }
        public string FrontCamera { get; set; }
        public string BackCamera { get; set; }
        [Required(ErrorMessage = "RAM is Requirde")]
        public string Ram { get; set; }
        [Required(ErrorMessage = "ROM is Requirde")]
        public string Rom { get; set; }
        public string Battery { get; set; }
        public int? SimSlotNumber { get; set; }
        public string SlotType { get; set; }
        public string ProjectStatus { get; set; }
        public string RevisedStatus { get; set; }
        public string ManagentComment { get; set; }
        [Required(ErrorMessage = "Approx. Price is Requirde")]
        public decimal? ApproximatePrice { get; set; }
        public long? GivenSampleToScreening { get; set; }
        [Required(ErrorMessage = "Final Price is Requirde")]
        public decimal? FinalPrice { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? Date { get; set; }

        [Required(ErrorMessage = "Screening Name is Requirde")]
        public string ProjectNameForScreening { get; set; }
        [Required(ErrorMessage = "Sourcing type is Requirde")]
        public string SourcingType { get; set; }
        public string ScreeningCommentFromCommercial { get; set; }
        public string PcbaVendorName { get; set; }
        public string PcbaFinalVendor { get; set; }

        public string DisplayResulution { get; set; }
        [Required(ErrorMessage = "Display speciality is Requirde")]
        public string DisplaySpeciality { get; set; }
        public string TpVendor { get; set; }
        public string TpFinalVendor { get; set; }
        public string LcdVendor { get; set; }
        public string LcdFinalVendor { get; set; }
        public string HousingVendorName { get; set; }
        public string HousingFinalVendorName { get; set; }
        [Required(ErrorMessage = "Back Cam is Requirde")]
        public string BackCam { get; set; }
        public string BackCamSensor { get; set; }
        public string BackCamBsi { get; set; }
        [Required(ErrorMessage = "Front Cam is Requirde")]
        public string FrontCam { get; set; }
        public string FrontCamSensor { get; set; }
        public string FrontCamBsi { get; set; }
        [Required(ErrorMessage = "CPU name is Requirde")]
        public string CpuName { get; set; }
        [Required(ErrorMessage = "Chipset is Requirde")]
        public string ChipsetName { get; set; }
        [Required(ErrorMessage = "Chipset Freq. is Requirde")]
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
        [Required(ErrorMessage = "Battery is Requirde")]
        public string BatteryRating { get; set; }
        public string BatteryType { get; set; }
        public string BatterySupplierName { get; set; }
        public string BateeryPossibleSupplierNames { get; set; }
        public decimal? OrderQuantity { get; set; }
        public string Color { get; set; }
        public int? OrderNuber { get; set; }
        public string CameraVendor { get; set; }
        public string RamVendor { get; set; }
        public string RomVendor { get; set; }


        //custom properties
        public string AddedName { get; set; }

        public string PurchaseOrderNumber { get; set; }
    }
}