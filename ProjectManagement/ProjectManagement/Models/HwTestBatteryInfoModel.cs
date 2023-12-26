using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwTestBatteryInfoModel
    {
        public long? HwTestBatteryInfoId { get; set; }
        public long? HwQcAssignId { get; set; }
        public long? HwQcInchargeAssignId { get; set; }
        public string Recommendation { get; set; }
        public string Battery_Type { get; set; }
        public string Battery_CellVoltage { get; set; }
        public string Battery_Capacity { get; set; }
        public string FiveC_TestInLab { get; set; }
        public string FiveC_TestResult { get; set; }
        public string FiveC_TestResult_Capacity { get; set; }
        public string Battery_Impedance { get; set; }
        public string Comment { get; set; }
        public HttpPostedFileBase HwQcDocUpload { get; set; }
        public string QcDocUploadPath { get; set; }
        public string ImageExtension { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}