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
    
    public partial class SwRequirementSpecificationDetailsLog
    {
        public long Id { get; set; }
        public long DetailsId { get; set; }
        public Nullable<long> ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public string Specification { get; set; }
        public string SubSpecification { get; set; }
        public string FiveToSevenK { get; set; }
        public string SevenToTenK { get; set; }
        public string TenToFourteenK { get; set; }
        public string FourteenKPlus { get; set; }
        public string Remarks { get; set; }
        public string RemarksForSpecification { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<long> AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
    }
}
