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
    
    public partial class OrderWiseDailyServiceToSalesRatio
    {
        public long Id { get; set; }
        public string ModelName { get; set; }
        public Nullable<int> OrderNo { get; set; }
        public Nullable<int> SalesQuantity { get; set; }
        public Nullable<int> ServiceQuantity { get; set; }
        public Nullable<decimal> ServiceToSalesRatio { get; set; }
        public Nullable<int> MonthsPassed { get; set; }
        public Nullable<int> DaysPassed { get; set; }
        public Nullable<System.DateTime> ReleaseDate { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
    }
}