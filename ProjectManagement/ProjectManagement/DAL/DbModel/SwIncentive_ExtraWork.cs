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
    
    public partial class SwIncentive_ExtraWork
    {
        public long InsExWorkId { get; set; }
        public Nullable<long> NewInnovationId { get; set; }
        public string EmployeeCode { get; set; }
        public string ProjectName { get; set; }
        public string AssignedBy { get; set; }
        public string Description { get; set; }
        public string WorkType { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public string Persons { get; set; }
        public Nullable<decimal> BaseAmount { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> AddedAmount { get; set; }
        public string AddAmountRemarks { get; set; }
        public Nullable<decimal> Deduction { get; set; }
        public string DeductionRemarks { get; set; }
        public Nullable<decimal> FinalAmount { get; set; }
        public string Month { get; set; }
        public Nullable<int> MonNum { get; set; }
        public Nullable<int> Year { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}
