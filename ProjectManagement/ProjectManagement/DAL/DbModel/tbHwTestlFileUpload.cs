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
    
    public partial class tbHwTestlFileUpload
    {
        public long Id { get; set; }
        public Nullable<long> TestId { get; set; }
        public string TestName { get; set; }
        public Nullable<long> ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public string AddedBy { get; set; }
    }
}
