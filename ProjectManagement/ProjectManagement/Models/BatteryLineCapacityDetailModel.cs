using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class BatteryLineCapacityDetailModel
    {
        public bool? IsActive { get; set; }
        public long Id { get; set; }
        public Nullable<long> PlanId { get; set; }
        public long? Battery_Id { get; set; }
        public DateTime? WorkingDate { get; set; }
        public long? PerDayCapacity { get; set; }
        public long? LineCapacity { get; set; }
        public long? LineAvailableCapacity { get; set; }
        public long? Production { get; set; }
        public long? TotalQuantityBattery { get; set; }
        public long? LineInformation_Id { get; set; }
        public string LineNumber { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Added { get; set; }
        public string ProjectType { get; set; }
    }
}