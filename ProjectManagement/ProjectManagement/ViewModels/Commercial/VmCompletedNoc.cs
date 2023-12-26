using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Commercial
{
    public class VmCompletedNoc
    {
        public long ProjectBtrcNocId { get; set; }
        public long ProjectMasterId { get; set; }
        public long PurchaseOrderId { get; set; }
        public long BtrcRawId { get; set; }
        public bool? IsNocComplete { get; set; }
        public string NocNo { get; set; }
        public long? RemainingQuantity { get; set; }
        public long? AllocateableFrom { get; set; }



        public string ProjectName { get; set; }
        public long PurchaseOrderQuantity { get; set; }
    }
}