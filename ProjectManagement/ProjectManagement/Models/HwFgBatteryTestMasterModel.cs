using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwFgBatteryTestMasterModel
    {
        public long HwFgBatteryTestMasterId { get; set; }
        public long? HwQcInchargeAssignId { get; set; }
        public long HwQcAssignId { get; set; }
        public long ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public string BatteryCapacity { get; set; }
        public string BatteryCellVoltage { get; set; }
        public string MaxChargeVoltage { get; set; }
        public string SampleNumber { get; set; }
        public long? SampleQuantity_Battery { get; set; }
        public long? SampleQuantity_Cell { get; set; }
        public string SampleSupllier { get; set; }
        public string TestEnvironment_Temperature { get; set; }
        public string TestEnvironment_Humidity { get; set; }
        public string TestItem { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public DateTime? TestDate { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}