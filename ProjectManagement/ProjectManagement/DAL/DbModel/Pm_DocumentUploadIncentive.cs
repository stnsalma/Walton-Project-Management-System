//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProjectManagement.DAL.DbModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Pm_DocumentUploadIncentive
    {
        public long PmDocUploadInsId { get; set; }
        public string IncentiveType { get; set; }
        public Nullable<long> ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public Nullable<int> Orders { get; set; }
        public string MultiProjectName { get; set; }
        public string MultiProjectIds { get; set; }
        public Nullable<System.DateTime> EffectiveMonth { get; set; }
        public string AccessoriesType { get; set; }
        public string Remarks { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> DeductAmount { get; set; }
        public string DeductRemarks { get; set; }
        public Nullable<decimal> FinalAmount { get; set; }
        public string Month { get; set; }
        public Nullable<int> MonNum { get; set; }
        public Nullable<long> Year { get; set; }
        public string EmployeeCode { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}
