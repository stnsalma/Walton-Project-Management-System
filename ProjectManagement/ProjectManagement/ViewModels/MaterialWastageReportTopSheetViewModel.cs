using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels
{
    public class MaterialWastageReportTopSheetViewModel
    {
        public MaterialWastageReportTopSheetViewModel()
        {
            Particulars = new List<WastageParticular>();
        }
        public string MonthName { get; set; }
        public string CompanyName { get; set; }
        public string UnitName { get; set; }
        public string Address { get; set; }
        public string Adderes2 { get; set; }
        public List<WastageParticular> Particulars { get; set; }
        public List<string> CreatorList { get; set; }
        public List<string> InChargeList { get; set; }
        public List<string> DeputyCooList { get; set; }
        public List<string> CooList { get; set; }
        public List<string> ApprovalList { get; set; }
    }

    public class WastageParticular
    {
        public string Particular { get; set; }
        public double PriceValue { get; set; }
    }
}