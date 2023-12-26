using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectOrderModel
    {
        public long ProjectOrderId { get; set; }
        public long ProjectMasterId { get; set; }
        public string PoNo { get; set; }
        public DateTime PoDate { get; set; }
        public long OrderQuantity { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}