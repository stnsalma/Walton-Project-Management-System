using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class Pro_CapacityPlanning_Model
    {
        public int IsRemoved { get; set; }
        public long Id { get; set; }
        public string Team { get; set; }
        public string ProductionType { get; set; }
        public string ProdutionType { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public string CategoryName { get; set; }
        public int? Percentage { get; set; }
        public string Percentage2 { get; set; }
        public string QuantityRange { get; set; }
        public decimal? TotalCapacity { get; set; }
        public string TotalCapacity2 { get; set; }
        public string TotalCapacities { get; set; }
        public string TotalCap1 { get; set; }
        public string TotalCap2 { get; set; }
        public string TotalCap3 { get; set; }
        public string TotalCap4 { get; set; }
        public string TotalCap5 { get; set; }
        public string TotalCap6 { get; set; }
        public string TotalCap7 { get; set; }
        public string TotalCap8 { get; set; }
        public string TotalCap9 { get; set; }
        public string TotalCap10 { get; set; }
        public string PhoneType { get; set; }
        public string Month { get; set; }
        public int? MonNum { get; set; }
        public int? Year { get; set; }
        public string Product { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string AllShift { get; set; }

        //list//
        public List<string> Category1 { get; set; }
        public List<string> Percentage1 { get; set; }
        public List<string> QuantityRange1 { get; set; }
        public List<string> TotalCapacity1 { get; set; }
        public List<string> AllShift1 { get; set; }
    }
}