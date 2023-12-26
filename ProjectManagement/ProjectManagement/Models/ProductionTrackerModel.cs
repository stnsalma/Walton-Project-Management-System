using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProductionTrackerModel
    {
        public long ProductionId { get; set; }
        public string ModelName { get; set; }
        public string IMEI1 { get; set; }
        public string IMEI2 { get; set; }
        public string Color { get; set; }
        public string OrderNo { get; set; }
        public bool? IsTrial { get; set; }
        public DateTime? ProductionDate { get; set; }
        public string SoftwareVersion { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public HttpPostedFileBase FilePath { get; set; }
    }
}