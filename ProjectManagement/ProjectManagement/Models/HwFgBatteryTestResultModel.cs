using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwFgBatteryTestResultModel
    {
        public long HwFgBatteryTestResultId { get; set; }
        public long HwFgBatteryTestConditionId { get; set; }
        public long CycleNo { get; set; }
        public long ItemNo { get; set; }
        public string TestCondition { get; set; }
        public string ItemName { get; set; }
        public string ChargingCapacity { get; set; }
        public string ChargingTime { get; set; }
        public string DischargingCapacity { get; set; }
        public string DischargingTime { get; set; }
        public Nullable<bool> Result { get; set; }
        public Nullable<long> AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}