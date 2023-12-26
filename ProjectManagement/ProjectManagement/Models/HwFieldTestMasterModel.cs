using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwFieldTestMasterModel
    {
        public long FieldTestMasterId { get; set; }
        public long? ProjectMasterId { get; set; }
        public long HwQcInchargeAssignId { get; set; }
        public string Model { get; set; }
        public string BenchMarkPhone { get; set; }
        public string Route { get; set; }
        public string Region { get; set; }
        public string FrequencyBand { get; set; }
        public string Operator { get; set; }
        public string TestName { get; set; }
        public string TestCategory { get; set; }
        public string TestDuration { get; set; }
        public string TestFocus { get; set; }
        public string NumberOfCalls { get; set; }
        public DateTime? TestDate { get; set; }
        public string FieldTestResult { get; set; }
        public string Remark { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}