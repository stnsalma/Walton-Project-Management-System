using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class WpmsAllProjectDetailsModel
    {
        public long ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectModel { get; set; }
        public string SupplierName { get; set; }
        public string Orders { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ProjectCreationDate { get; set; }
        public string IsActiveStatus { get; set; }
        public string IsActiveOrder { get; set; }
        public string ProjectManagerName { get; set; }
        public string ProjectStatus { get; set; }
        public long? OrderQuantity { get; set; }
        public DateTime? MarketClearanceDate { get; set; }
        //
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ApproxShipmentDate { get; set; }

        public string PoDate { get; set; }
        public string SupplierTrustLevel { get; set; }
        public string ProjectType { get; set; }
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
        public decimal? ApproximatePrice { get; set; }
        public long? GivenSampleToScreening { get; set; }
        public decimal? FinalPrice { get; set; }
        public decimal? PriceBDT { get; set; }
        public string SourcingType { get; set; }
        public string DisplayResulution { get; set; }
        public string DisplaySpeciality { get; set; }

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
        public bool? OtgCable { get; set; }
        public bool? FlashLight { get; set; }
        public string SecondGen { get; set; }
        public string ThirdGen { get; set; }
        public string FourthGenFdd { get; set; }
        public string FourthGenTdd { get; set; }
        public string Cdma { get; set; }
        public string BatteryRating { get; set; }
        public string BatteryType { get; set; }
        public string Color { get; set; }

        public string BackCamAutoFocus { get; set; }
        public string FrontCamAutoFocus { get; set; }
        public string CameraVendor { get; set; }
        public string RamVendor { get; set; }
        public string RomVendor { get; set; }
        public string Motherboard { get; set; }
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
        public string ProjectNameForScreening { get; set; }
        public string ShipmentMode { get; set; }
        public string PcbaFinalVendor { get; set; }
        public string PcbaVendorName { get; set; }
        public string BatteryCoverLogoType { get; set; }
        public decimal TotalPrice { get; set; }
        public string InitialApprovalPendings { get; set; }
        public string InitialApproval { get; set; }
        public string ProStatus { get; set; }


    }
}