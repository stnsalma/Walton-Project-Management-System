using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Common
{
    public class BOMReportVm
    {
        public long Handset_Id { get; set; }
        public string ModelName { get; set; }
        public long ProductionQty { get; set; }
        public IList<WSMTBomVm> Boms { get; set; }
        
    }
}