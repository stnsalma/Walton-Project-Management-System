using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwQcFieldTestStaticDataModel
    {
        public long Id { get; set; }
        public string OperatorName { get; set; }
        public string Operator { get; set; }
        public string FrequencyBand { get; set; }
        public string TestName { get; set; }
        public string TestCategory { get; set; }
        public string TestDuration { get; set; }
        public string TestFocus1 { get; set; }
        public string TestFocus2 { get; set; }
        public string TestFocus3 { get; set; }
        public string NumberOfCalls { get; set; }
        public string Location { get; set; }
        public string SpeedLimit { get; set; }
        public string TRssiBars { get; set; }
        public string TCallDrop { get; set; }
        public string TNoiseInterference { get; set; }
        public string TLongMute { get; set; }
        public string BRssiBars { get; set; }
        public string BCallDrop { get; set; }
        public string BNoiseInterference { get; set; }
        public string BLongMute { get; set; }
        public string Pass { get; set; }
        public string Fail { get; set; }
        public string TestPhaseName { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string AssignedPerson { get; set; }
        public string ProjectName { get; set; }
        public string SoftwareVersionName { get; set; }

        public long? TestPhaseID { get; set; }
        public int? SoftwareVersionNo { get; set; }
        public long? ProjectMasterId { get; set; }
        public long? SwQcAssignId { get; set; }
        public long? SwQcHeadAssignId { get; set; }
        public string BenchmarkPhone { get; set; }
        public string Route { get; set; }
        public string Region { get; set; }
        public string FieldTestResult { get; set; }
        public string Remarks { get; set; }
       
    }
}