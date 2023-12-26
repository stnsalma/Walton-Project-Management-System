using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Commercial
{
    public class VmWarehouseEntry
    {
        public List<string> MonthList { get; set; }
        public List<TypeSummary> ProductTypeList { get; set; }
        public List<double> MonthlyTotalQty { get; set; }
        public List<VmCommercialEvent> CommercialEvents { get; set; }
    }

    public class TypeSummary
    {
        public string MonthName { get; set; }
        public string TypeName { get; set; }
        public long ProjectPurchaseOrderFormId { get; set; }
        public double TotalQty { get; set; }
    }
}