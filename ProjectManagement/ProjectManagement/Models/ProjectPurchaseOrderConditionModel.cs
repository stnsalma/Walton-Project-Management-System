using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectPurchaseOrderConditionModel
    {
        public long ProjectPurchaseOrderConditionId { get; set; }
        public long? ProjectPurchaseOrderFormId { get; set; }
        public int? SerialNo { get; set; }
        public string Statement { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}