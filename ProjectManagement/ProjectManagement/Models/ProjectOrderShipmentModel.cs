using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectOrderShipmentModel
    {
        public long ProjectOrderShipmentId { get; set; }
        public long ProjectMasterId { get; set; }
        [Required(ErrorMessage = "PO is Required")]
        public long ProjectPurchaseOrderFormId { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime ShipmentApproxDate { get; set; }
        [Required(ErrorMessage = "ShipmentType is Required")]
        public string ShipmentType { get; set; }
        public System.DateTime ShipmentFinalDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        //[Required(ErrorMessage = "China Inspection Date is Required")]
        public DateTime? ChainaInspectionDate { get; set; }
        //[Required(ErrorMessage = "Forwarder Date is Required")]
        public DateTime? ForwarderDate { get; set; }
        //[Required(ErrorMessage = "Flight Departure Date is Required")]
        public DateTime? FlightDepartureDate { get; set; }
        //[Required(ErrorMessage = "Airport Arrival Date is Required")]
        public DateTime? AriportArrivalDate { get; set; }
        //[Required(ErrorMessage = "Bank NOC Date is Required")]
        public DateTime? BankNocDate { get; set; }
        //[Required(ErrorMessage = "CNF Date is Required")]
        public DateTime? CnfDate { get; set; }
        //[Required(ErrorMessage = "CNF Pay Order Date is Required")]
        public DateTime? CnfPayOrderDate { get; set; }
        //[Required(ErrorMessage = "Airport Release Date is Required")]
        public DateTime? AirportReleaseDate { get; set; }
        //[Required(ErrorMessage = "Warehouse Entry Date is Required")]
        public DateTime? WarehouseEntryDate { get; set; }
        //[Required(ErrorMessage = "Costing Date is Required")]
        public DateTime? CostingDate { get; set; }
        //[Required(ErrorMessage = "Market Release Date is Required")]
        public DateTime? MarketReleaseDate { get; set; }
        public bool? IsComplete { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? PoWiseShipmentNumber { get; set; }
      
        public DateTime? ProjectManagerClearanceDate { get; set; }
        public DateTime? RawMaterialShipmentDate { get; set; }
        public DateTime? IQC_JigsAndFixtureShipmentDate { get; set; }
        public DateTime? ProductionJigsAndFixtureShipmentDate { get; set; }
        public DateTime? StencilShipmentDate { get; set; }
        public DateTime? PalletShipmentDate { get; set; }
        public string RawMaterialShipmentMode { get; set; }
        public DateTime? BdDate { get; set; }
        public DateTime? FactoryDate { get; set; }
        public DateTime? VatDeclerationDate { get; set; }
        public decimal? VatDeclerationPrice { get; set; }
        public DateTime? VesselDate { get; set; }
        public string VesselType { get; set; }
        public string ContatinerType { get; set; }

        //Cutom Properties
        public string AddedByName { get; set; }
        public string ProjectModel { get; set; }
        public string ProjectName { get; set; }
        public string PoNo { get; set; }
        public DateTime PoDate { get; set; }
        public DateTime? PoDate1 { get; set; }
        public int? PoCount { get; set; }
        public string PoOrdinal { get; set; }
        public string ShipmentNoOrdinal { get; set; }
        public string ShipmentPercentage { get; set; }
        public string IsFinalShipment { get; set; }
        public int? ChinaIqcFail { get; set; }
        public string ChinaIqcPassHundredPercent { get; set; }
        public string ManagementApproval { get; set; }
        public DateTime? ManagementApprovalDate { get; set; }
        public long? FinishGoodProjectMasterId { get; set; }
        public long? FocQuantity { get; set; }
        public long? OrderShipmentQuantity { get; set; }
        public string FinishGoodModel { get; set; }
        public int? ApproxFinishGoodManufactureQty { get; set; }

        public string FinishGoodCheck { get; set; }

        public long? ProjectOrderShipmentId1 { get; set; }
        public int? FinishGoodModelOrderNumber { get; set; }

    }
}