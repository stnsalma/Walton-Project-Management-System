using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ReturnImeiLogModel
    {
        public long ReturnImeiId { get; set; }
        public string IMEI { get; set; }
        public string IMEI2 { get; set; }
        public string Model { get; set; }
        public string DistributorName { get; set; }
        public string DistributionDate { get; set; }
        public string DealerCode { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
    }
}