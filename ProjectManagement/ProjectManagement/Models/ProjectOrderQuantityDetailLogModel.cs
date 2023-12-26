using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectOrderQuantityDetailLogModel
    {
        public long Id { get; set; }
        public Nullable<long> OrderQuantityDetailsId { get; set; }
        public Nullable<long> ProjectMasterId { get; set; }
        public string ProjectModel { get; set; }
        public Nullable<long> OrderQuantity { get; set; }
        public string RamVendor { get; set; }
        public string RomVendor { get; set; }
        public Nullable<decimal> VariantPrice { get; set; }
        public string CurrencyName { get; set; }
        public Nullable<long> AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public bool BTRCPush { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<long> VariantClosingBy { get; set; }
        public Nullable<System.DateTime> VariantClosingDate { get; set; }
        public string ClosingRemarks { get; set; }
        public Nullable<long> DeactivatedBy { get; set; }
        public Nullable<System.DateTime> DeactivationDate { get; set; }
        public Nullable<long> ActivationBy { get; set; }
        public Nullable<System.DateTime> ActivationDate { get; set; }
        public Nullable<long> LogAddedBy { get; set; }
        public Nullable<System.DateTime> LogAddedDate { get; set; }
    }
}