using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Commercial
{
    public class VmCommercialEvent
    {
        public string Month { get; set; }
        public string SupplierName { get; set; }
        public string ProuductType { get; set; }
        public string ModelName { get; set; }
        public string OrderNumber { get; set; }
        public string CkdModelType { get; set; }
        public decimal? OrderQuantity { get; set; }
        public int PoWiseShipmentNumber { get; set; }
        public long ProjectPurchaseOrderFormId { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? InspectionDate { get; set; }
        public DateTime? WareHouseReceiveDate { get; set; }
        
    }
}