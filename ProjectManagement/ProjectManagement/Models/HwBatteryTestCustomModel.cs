using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwBatteryTestCustomModel
    {
        public long HwFgBatteryTestMasterId { get; set; }
        public long ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public string BatteryCapacity { get; set; }
        public string BatteryCellVoltage { get; set; }
        public string MaxChargeVoltage { get; set; }
        public int? NumberOfSample { get; set; }
        public long? SampleQuantity_Battery { get; set; }
        public long? SampleQuantity_Cell { get; set; }
        public string BatterySupplierName { get; set; }
        public DateTime? TestDate { get; set; }
        public string TestEnvironment_Temperature { get; set; }
        public string TestEnvironment_Humidity { get; set; }
        public string TestItem { get; set; }

    }
}