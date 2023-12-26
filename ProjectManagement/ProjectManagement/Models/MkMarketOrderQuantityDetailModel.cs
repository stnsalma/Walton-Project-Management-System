using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class MkMarketOrderQuantityDetailModel
    {
        public long Id { get; set; }
        public long? MkProjectSpecId { get; set; }
        public int? OrderNumber { get; set; }
        public string PoName { get; set; }
        public long? NOC_Quantity { get; set; }
        public long? OrderQuantity { get; set; }
        public long? FOB_Price { get; set; }
        public string FOB_PriceCurrencyType { get; set; }
        public long? BTRC_NOC_Price { get; set; }
        public string BTRC_NOC_PriceCurrencyType { get; set; }
        public long? CustomsAssessmentPrice { get; set; }
        public string CustAssPriceCurrencyType { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ModelName { get; set; }
    }
}