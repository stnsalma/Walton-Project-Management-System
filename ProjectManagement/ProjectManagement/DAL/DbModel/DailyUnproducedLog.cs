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
    
    public partial class DailyUnproducedLog
    {
        public long Id { get; set; }
        public Nullable<System.DateTime> LogDate { get; set; }
        public string ProjectModel { get; set; }
        public string OrderNumber { get; set; }
        public string OrderQuantity { get; set; }
        public string Produced { get; set; }
        public string Unproduced { get; set; }
        public string LastMonthIMEI { get; set; }
    }
}
