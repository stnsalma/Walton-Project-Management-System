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
    
    public partial class ReplacementLog
    {
        public int ReplacementLogID { get; set; }
        public string IMEI_1 { get; set; }
        public string IMEI_2 { get; set; }
        public string Model { get; set; }
        public Nullable<System.DateTime> RegistrationDate { get; set; }
        public Nullable<System.DateTime> DistributionDate { get; set; }
        public Nullable<int> RequestID { get; set; }
        public Nullable<System.DateTime> ReplacementDate { get; set; }
        public string Status { get; set; }
        public string AppDeclinedRemarks { get; set; }
        public string AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}