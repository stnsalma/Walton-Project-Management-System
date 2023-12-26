using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SpareOrderModel
    {
        public long SpareOrderId { get; set; }
        public long? SpareId { get; set; }
        public string ProjectName { get; set; }
        public string OrderNumber { get; set; }
        public string SparePartsName { get; set; }
        public string HandsetQuantity { get; set; }
        public string Quantity { get; set; }
        public string ProposedImportRatio { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public string PiNumber { get; set; }
        public long? ProjectMasterId { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Remarks { get; set; }


        public DateTime? IsSpareConfirmedDate { get; set; }
        public DateTime? IsSpareSubmittedDate { get; set; }
        public DateTime? PiDate { get; set; }
    }
}