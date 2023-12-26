using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models.Common;

namespace ProjectManagement.ViewModels.Common
{
    public class ChartGraphforDailySalesViewModel
    {
        public string ModelName { get; set; }
        public IList<HighChartDataModel> HighChartData { get; set; }
    }
}