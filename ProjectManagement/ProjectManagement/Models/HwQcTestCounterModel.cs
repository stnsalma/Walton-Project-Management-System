using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwQcTestCounterModel
    {
        public int ScreeningCounter { get; set; }
        public int RunningTestCounter { get; set; }
        public int FinishedGoodsCounter { get; set; }
        public int AfterSalesCounter { get; set; }
    }
}