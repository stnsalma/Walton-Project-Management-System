using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.Common
{
    public class RemainingStocksDetailsModel
    {
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
        public string ModelName { get; set; }
        public long RemainingStock { get; set; }
        public string DealerCity { get; set; }
        public string DealerType { get; set; }
    }
}