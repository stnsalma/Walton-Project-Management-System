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
    
    public partial class CmnIssue
    {
        public long CmnIssueId { get; set; }
        public long ProjectMasterId { get; set; }
        public string HwIssueDescription { get; set; }
        public string HwIssuePriority { get; set; }
        public Nullable<long> HwRaisedBy { get; set; }
        public Nullable<System.DateTime> HwRaisedDate { get; set; }
        public Nullable<long> HwRefferedBy { get; set; }
        public string HwRemarks { get; set; }
        public string HwStatus { get; set; }
        public string SwDescription { get; set; }
        public string SwIssuePriority { get; set; }
        public Nullable<long> SwRaisedBy { get; set; }
        public Nullable<System.DateTime> SwRaisedDate { get; set; }
        public Nullable<long> SwRefferedBy { get; set; }
        public string SwRemarks { get; set; }
        public string SwStatus { get; set; }
        public string PmIssueDescription { get; set; }
        public string PmIssuePriority { get; set; }
        public Nullable<long> PmRaisedBy { get; set; }
        public Nullable<System.DateTime> PmRaisedDate { get; set; }
        public Nullable<long> PmRefferedBy { get; set; }
        public string PmRemarks { get; set; }
        public string PmStatus { get; set; }
        public string CmIssueDescription { get; set; }
        public string CmIssuePriority { get; set; }
        public Nullable<long> CmRaisedBy { get; set; }
        public Nullable<System.DateTime> CmRaisedDate { get; set; }
        public Nullable<long> CmRefferedBy { get; set; }
        public string CmRemarks { get; set; }
        public string CmStatus { get; set; }
        public string CurrentSate { get; set; }
        public bool IsSolved { get; set; }
        public Nullable<long> SolvedBy { get; set; }
        public Nullable<System.DateTime> SolvedDate { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}
