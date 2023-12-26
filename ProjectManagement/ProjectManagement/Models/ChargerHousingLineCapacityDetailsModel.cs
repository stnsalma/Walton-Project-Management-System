using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ChargerHousingLineCapacityDetailsModel
    {
        public long Id { get; set; }
        public long? ChargerHousing_Id { get; set; }
        public DateTime? WorkingDate { get; set; }
        public long? PerDayCapacity { get; set; }
        public long? LineCapacity { get; set; }
        public long? TotalQuantity { get; set; }
        public long? LineAvailableCapacity { get; set; }
        public long? LineInformation_Id { get; set; }
        public string LineNumber { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Added { get; set; }
    }
}