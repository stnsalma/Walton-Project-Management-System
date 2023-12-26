using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Spare
{
    public class SpareOrderListObject
    {
        public long SpareId { get; set; }
        public string IsChecked { get; set; }
        public string SpareName { get; set; }
        public string Quantity { get; set; }
        public string ProposedImportRatio { get; set; }
        public string Remarks { get; set; }
    }
}