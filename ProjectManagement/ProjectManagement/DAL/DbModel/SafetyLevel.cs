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
    
    public partial class SafetyLevel
    {
        public long ID { get; set; }
        public Nullable<double> DailyLevel { get; set; }
        public Nullable<double> MonthlyLevel { get; set; }
        public Nullable<bool> Status { get; set; }
    }
}
