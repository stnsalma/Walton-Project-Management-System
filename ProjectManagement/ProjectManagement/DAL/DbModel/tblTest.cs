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
    
    public partial class tblTest
    {
        public long TestId { get; set; }
        public string TestName { get; set; }
        public Nullable<long> GivenTime { get; set; }
        public Nullable<long> ActualTimeforTask { get; set; }
        public string UnitName { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> AddedBy { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}