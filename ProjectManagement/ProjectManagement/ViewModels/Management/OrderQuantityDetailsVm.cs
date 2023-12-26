using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Management
{
    public class OrderQuantityDetailsVm
    {
        public long ProjectMasterId { get; set; }
        public long? VariantId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectModel { get; set; }
        public string VariantName { get; set; }
        public int? OrderNuber { get; set; }
        public decimal? OrderQuantity { get; set; }
        public long? QuantityInCalculator { get; set; }
        public bool? IsLocked { get; set; }
        public bool IsActive { get; set; }
        public DateTime? PoDate { get; set; }
        public int RowSpan { get; set; }
    }
}