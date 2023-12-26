using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.Common
{
    public class CellPhoneDailySales
    {
        public string RegistrationDate { get; set; }
        public string InvoiceDate { get; set; }
        public string Model { get; set; }
        public int Number { get; set; }
        public string InvoicePrice { get; set; }
        public string TotalPrice { get; set; }
        public string Id { get; set; }
        public string CellPhoneType { get; set; }
        public string ServiceToSalesRatio { get; set; }
        public string RemainingMarketStock { get; set; }
    }
}