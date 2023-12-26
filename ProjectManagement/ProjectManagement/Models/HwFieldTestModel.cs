using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwFieldTestModel
    {
        public long FieldTestId { get; set; }
        public long FieldTestMasterId { get; set; }
        public string OperatorName { get; set; }
        public string Location { get; set; }
        public string SpeedLimit { get; set; }
        public string TestedRssiBars { get; set; }
        public string TestedCallDrop { get; set; }
        public string TestedShortMute { get; set; }
        public string TestedLongMute { get; set; }
        public string BechmarkRssiBars { get; set; }
        public string BenchmarkCallDrop { get; set; }
        public string BenchmarkShortMute { get; set; }
        public string BenchMarkLongMute { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}