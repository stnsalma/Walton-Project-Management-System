using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class EarphonePoModel
    {
        public long Id { get; set; }
        public string EarphonePoNo { get; set; }
        public string EarphonePoDate { get; set; }
        public string InvoiceNo { get; set; }
        public string CountryOfOrigin { get; set; }
        public string Description { get; set; }
        public string Quantity { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAddress { get; set; }
    }
}