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
    
    public partial class VariantWiseProductionStartDate
    {
        public long Id { get; set; }
        public Nullable<long> ProjectId { get; set; }
        public Nullable<long> PoId { get; set; }
        public Nullable<long> VariantId { get; set; }
        public Nullable<int> OrderNumber { get; set; }
        public string ProjectName { get; set; }
        public string VariantName { get; set; }
        public Nullable<System.DateTime> ProductionStartDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
    }
}
