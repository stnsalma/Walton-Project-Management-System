using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models.Common;

namespace ProjectManagement.ViewModels.Common
{
    public class RemainingMarketStockDealerWiseViewModel
    {

        public IList<RemainingStocksDetailsModel> RemainingStocksDetails { get; set; }
        public string TotalRemainingStock { get; set; }
    }
}