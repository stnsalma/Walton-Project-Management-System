using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class MkProjectSpecModel
    {
        public long Id { get; set; }
        public string Brand { get; set; }
        public string ModelName { get; set; }
        public string SimSlotNumber { get; set; }
        public string SimSlotType { get; set; }
        public string DisplaySize { get; set; }
        public string Resolution { get; set; }
        public string DisplayType { get; set; }
        public string OperatingSystem { get; set; }
        public string Chipset { get; set; }
        public string CPU { get; set; }
        public string CPUFrequency { get; set; }
        public string GPU { get; set; }
        public string FrontCamera { get; set; }
        public string BackCamera { get; set; }
        public string RAM { get; set; }
        public string ROM { get; set; }
        public string Price { get; set; }
        public long? AddedBy { get; set; }
        public string AddedByName { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string OsVersion { get; set; }
        public string BatteryCapacity { get; set; }
        public string BatteryType { get; set; }
        public string DealerPrice { get; set; }
        public string InvoicePrice { get; set; }
        public string DealerComission { get; set; }
        public string RetailerComission { get; set; }
        public string CommercialImportPrice { get; set; }
        public string ColorAvailable { get; set; }
        public string Gift { get; set; }
        public bool HeadPhone { get; set; }
        public bool DataCable { get; set; }
        public bool Charger { get; set; }
        public bool ScreenProtector { get; set; }
        public bool PhoneCase { get; set; }
        public string ExpandableStorage { get; set; }
        public string Network { get; set; }
        public string UpcomingPrice { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string ProductType { get; set; }
        public string ExtraFeatures { get; set; }
        public bool Torch { get; set; }
        public bool FmRadio { get; set; }
    }
}