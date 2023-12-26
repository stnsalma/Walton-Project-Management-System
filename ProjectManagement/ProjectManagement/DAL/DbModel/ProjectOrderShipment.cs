//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProjectManagement.DAL.DbModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProjectOrderShipment
    {
        public long ProjectOrderShipmentId { get; set; }
        public long ProjectMasterId { get; set; }
        public long ProjectPurchaseOrderFormId { get; set; }
        public string ShipmentType { get; set; }
        public System.DateTime ShipmentApproxDate { get; set; }
        public System.DateTime ShipmentFinalDate { get; set; }
        public Nullable<System.DateTime> ChainaInspectionDate { get; set; }
        public Nullable<System.DateTime> ForwarderDate { get; set; }
        public Nullable<System.DateTime> FlightDepartureDate { get; set; }
        public Nullable<System.DateTime> AriportArrivalDate { get; set; }
        public Nullable<System.DateTime> BankNocDate { get; set; }
        public Nullable<System.DateTime> CnfDate { get; set; }
        public Nullable<System.DateTime> CnfPayOrderDate { get; set; }
        public Nullable<System.DateTime> AirportReleaseDate { get; set; }
        public Nullable<System.DateTime> WarehouseEntryDate { get; set; }
        public Nullable<System.DateTime> CostingDate { get; set; }
        public Nullable<System.DateTime> MarketReleaseDate { get; set; }
        public Nullable<bool> IsComplete { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> PoWiseShipmentNumber { get; set; }
        public Nullable<System.DateTime> ProjectManagerClearanceDate { get; set; }
        public Nullable<System.DateTime> RawMaterialShipmentDate { get; set; }
        public Nullable<System.DateTime> IQC_JigsAndFixtureShipmentDate { get; set; }
        public Nullable<System.DateTime> ProductionJigsAndFixtureShipmentDate { get; set; }
        public Nullable<System.DateTime> StencilShipmentDate { get; set; }
        public Nullable<System.DateTime> PalletShipmentDate { get; set; }
        public string RawMaterialShipmentMode { get; set; }
        public Nullable<System.DateTime> BdDate { get; set; }
        public Nullable<System.DateTime> FactoryDate { get; set; }
        public Nullable<System.DateTime> VatDeclerationDate { get; set; }
        public Nullable<decimal> VatDeclerationPrice { get; set; }
        public Nullable<System.DateTime> VesselDate { get; set; }
        public string VesselType { get; set; }
        public string ContatinerType { get; set; }
        public string ShipmentPercentage { get; set; }
        public string IsFinalShipment { get; set; }
        public string ManagementApproval { get; set; }
        public Nullable<System.DateTime> ManagementApprovalDate { get; set; }
        public string ChinaIqcPassHundredPercent { get; set; }
        public Nullable<int> ChinaIqcFail { get; set; }
        public Nullable<long> OrderShipmentQuantity { get; set; }
        public Nullable<long> FocQuantity { get; set; }
    }
}