using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models.Common;

namespace ProjectManagement.ViewModels.Common
{
    public class DailySalesInvoicesViewModel
    {
        public long SumSmartQunatity { get; set; }
        public string SumSmartPrice { get; set; }
        public IList<CellPhoneDailySales> SmartPhoneDailySales { get; set; }
        public long SumFeatureQunatity { get; set; }
        public string SumFeaturePrice { get; set; }
        public IList<CellPhoneDailySales> FeaturePhoneDailySales { get; set; }
        public long SumTabQunatity { get; set; }
        public string SumTabPrice { get; set; }
        public IList<CellPhoneDailySales> TabletDailySales { get; set; }
        public long GrandTotalQunatity { get; set; }
        public string GrandTotalPrice { get; set; }
        public string InvoiceDate { get; set; }

    }
}