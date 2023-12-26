using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class GetHwItemizationModel
    {
        public long HwItemizationId { get; set; }
        public long ProjectMasterId { get; set; }
        public long HwQcInchargeAssignId { get; set; }
        public long HwQcAssignId { get; set; }
        public string ItemComponentName { get; set; }
        public string YesNot { get; set; }
        public string IcComponentNumber { get; set; }
        public string IcComponent_Vendor { get; set; }
        public string Compatibility { get; set; }
        public string Type { get; set; }
        public string ExistingItem { get; set; }
        public string SupplierCode { get; set; }
        public string Remarks { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}