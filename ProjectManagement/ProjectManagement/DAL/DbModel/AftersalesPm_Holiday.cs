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
    
    public partial class AftersalesPm_Holiday
    {
        public long Id { get; set; }
        public string HolidayName { get; set; }
        public Nullable<System.DateTime> Holiday_SDate { get; set; }
        public Nullable<System.DateTime> Holiday_EDate { get; set; }
        public string Month { get; set; }
        public Nullable<int> MonNum { get; set; }
        public Nullable<int> Year { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}
