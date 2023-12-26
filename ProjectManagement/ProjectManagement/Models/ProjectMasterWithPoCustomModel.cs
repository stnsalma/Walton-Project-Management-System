using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectMasterWithPoCustomModel
    {
        public long ProjectMasterId { get; set; }
        public long ProjectTypeId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectNameForScreening { get; set; }
        public string SupplierName { get; set; }
        public string SupplierModelName { get; set; }
        public string SupplierTrustLevel { get; set; }
        public int? NumberOfSample { get; set; }
        public int? OrderNuber { get; set; }
        public decimal? OrderQuantity { get; set; }
        public decimal? FinalPrice { get; set; }
        public DateTime? PoDate { get; set; }
        public DateTime? ApproxProjectFinishDate { get; set; }
        public DateTime? ApproxShipmentDate { get; set; }
        public DateTime? IsSpareSubmittedDate { get; set; }
        public DateTime? ChainaInspectionDate { get; set; }
    }
}