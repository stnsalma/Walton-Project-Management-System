using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class BatteryTestResultSummaryModel
    {
        public long BatteryTestResultSummaryId { get; set; }
        public long? HwQcInchargeAssignId { get; set; }
        public string BatteryInternalResistance { get; set; }
        public string CellInternalResistance { get; set; }
        public string Battery1TestValue { get; set; }
        public string Battery2TestValue { get; set; }
        public string Cell1TestValue { get; set; }
        public string Cell2TestValue { get; set; }
        public string NtcRemarks { get; set; }
        public string AnnouncedCapacity { get; set; }
        public string PhysicalCondition { get; set; }
        public string Cadex30CycleTest1C { get; set; }
        public string Remarks { get; set; }
        public Nullable<long> AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string Battery1Result { get; set; }
        public string Battery2Result { get; set; }
        public string Cell1Result { get; set; }
        public string Cell2Result { get; set; }
        public string Battery1Ntc { get; set; }
        public string Battery2Ntc { get; set; }
        public string Cell1Ntc { get; set; }
        public string Cell2Ntc { get; set; }
    }
}