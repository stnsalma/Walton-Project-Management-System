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
    
    public partial class IncentiveShare
    {
        public long Id { get; set; }
        public string EmployeeCode { get; set; }
        public Nullable<int> Share { get; set; }
        public Nullable<long> CarryAmount { get; set; }
        public Nullable<long> Category { get; set; }
    }
}