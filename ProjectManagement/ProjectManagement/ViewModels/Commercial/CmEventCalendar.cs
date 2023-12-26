using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Commercial
{
    public class CmEventCalendar
    {
        public long ProjectMasterId { get; set; }
        public long ProjectTypeId { get; set; }
        public string ProjectName { get; set; }
        public long? SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierModelName { get; set; }
        public int? NumberOfSample { get; set; }

         [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? ApproxProjectFinishDate { get; set; }
        public string SupplierTrustLevel { get; set; }
        public bool? IsScreenTestComplete { get; set; }
        public bool? IsApproved { get; set; }
        public bool IsActive { get; set; }

             [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? ApproxProjectOrderDate { get; set; }

             [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
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
        public int? OrderNuber { get; set; }
        public DateTime? InitialApprovalDate { get; set; }
        public DateTime? FinalApprovalDate { get; set; }
        public string BackCamAutoFocus { get; set; }
        public string FrontCamAutoFocus { get; set; }

        //custom properties
        public string AddedName { get; set; }

        public string PurchaseOrderNumber { get; set; }

        public string OrderNumberOrdinal { get; set; }

        public Nullable<long> TestPhaseID { get; set; }
        public string TestPhaseName { get; set; }

        public long? SwQcInchargeAssignId { get; set; }

        public string PoCategory { get; set; }
        ////////////////////////////////
        public long ProjectOrderShipmentId { get; set; }
        [Required(ErrorMessage = "PO is Required")]
        public long ProjectPurchaseOrderFormId { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime ShipmentApproxDate { get; set; }
        [Required(ErrorMessage = "ShipmentType is Required")]
        public string ShipmentType { get; set; }
        public System.DateTime ShipmentFinalDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Required(ErrorMessage = "Chaina Inspection Date is Required")]
        public DateTime? ChainaInspectionDate { get; set; }
        [Required(ErrorMessage = "Forwarder Date is Required")]
        public DateTime? ForwarderDate { get; set; }
        [Required(ErrorMessage = "Flight Departure Date is Required")]
        public DateTime? FlightDepartureDate { get; set; }
        [Required(ErrorMessage = "Airport Arrival Date is Required")]
        public DateTime? AriportArrivalDate { get; set; }
        public DateTime? BankNocDate { get; set; }
        public DateTime? CnfDate { get; set; }
        public DateTime? CnfPayOrderDate { get; set; }
        public DateTime? AirportReleaseDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? WarehouseEntryDate { get; set; }
        public DateTime? CostingDate { get; set; }
        public DateTime? MarketReleaseDate { get; set; }
      
        public int? PoWiseShipmentNumber { get; set; }

        //Cutom Properties
        public string AddedByName { get; set; }
        public string PoNo { get; set; }
        public DateTime PoDate { get; set; }
        public int? PoCount { get; set; }
        public string PoOrdinal { get; set; }
        public string ShipmentNoOrdinal { get; set; }
        //////////////////////////////////////////
        [Required]
        public string Receiver { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string CompanyAddress { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string DescriptionHeader { get; set; }
        [Required]
        public string DescriptionBody { get; set; }
        public byte[] Signature { get; set; }
        [Required]
        public long? Quantity { get; set; }
        [Required]
        public string Value { get; set; }
        public bool? IsCompleted { get; set; }
        public DateTime? IsSpareConfirmedDate { get; set; }
        public DateTime? IsSpareSubmittedDate { get; set; }
        public DateTime? PiDate { get; set; }
        public int OrderNumber { get; set; }

        public string Title1 { get; set; }
        public DateTime? Title2 { get; set; }
        public DateTime? Start { get; set; }
        public string CmColor { get; set; }

    }
}