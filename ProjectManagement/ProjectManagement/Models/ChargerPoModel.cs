using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ChargerPoModel
    {
        public long Id { get; set; }
        public string ChargerPoNo { get; set; }
        public string OrderNo { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAddress { get; set; }
        public string ChargerPoDate { get; set; }
        public string InvoiceNo { get; set; }
        public string SlNo { get; set; }
        public string Description { get; set; }
        public string Quantity { get; set; }
        public string VoltageRating { get; set; }
        public string CurrentRating { get; set; }
        public string PortType { get; set; }
        public string ChargerType { get; set; }
        public string Remarks { get; set; }
        public string CountryOfOrigin { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}