using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwFgBatteryTestConditionModel
    {
        public long HwFgBatteryTestConditionId { get; set; }
        public long HwFgBatteryTestMasterId { get; set; }
        public long CycleNo { get; set; }
        public string TestCondition { get; set; }
        public string ChargeCurrent { get; set; }
        public string CutoffCurrent { get; set; }
        public string LimitedVoltage { get; set; }
        public string DischargeCurrent { get; set; }
        public string CutoffVoltage { get; set; }
        public string RestTimeAfter_Charge { get; set; }
        public string RestTimeAfter_Discharge { get; set; }
        public Nullable<long> AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}