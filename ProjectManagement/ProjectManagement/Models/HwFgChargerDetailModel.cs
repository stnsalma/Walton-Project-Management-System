using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwFgChargerDetailModel
    {
        public long HwFgChargerDetailId { get; set; }
        public long HwFgChargerTestId { get; set; }
        public string FloatingVoltage { get; set; }
        public string SetLoadOutPutVoltage_Ac180v { get; set; }
        public string SetLoadOutPutVoltage_Ac220v { get; set; }
        public string SetLoadOutPutVoltage_Ac240v { get; set; }
        public string SetLoadOutPutCurrent_Ac180v { get; set; }
        public string SetLoadOutPutCurrent_Ac220v { get; set; }
        public string SetLoadOutPutCurrent_Ac240v { get; set; }
        public string CurrentLeakage { get; set; }
        public string ShortProtection { get; set; }
        public string LoadRipple { get; set; }
        public Nullable<long> AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}