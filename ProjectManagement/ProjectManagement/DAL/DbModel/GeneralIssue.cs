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
    
    public partial class GeneralIssue
    {
        public long GeneralIssueId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreatorRole { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ReferenceComment { get; set; }
        public string ReferenceFlow { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<System.DateTime> SolveDate { get; set; }
        public Nullable<System.DateTime> DenyDate { get; set; }
    }
}
