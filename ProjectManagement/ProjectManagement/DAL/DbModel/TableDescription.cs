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
    
    public partial class TableDescription
    {
        public long Id { get; set; }
        public string SoftwareQc { get; set; }
        public string HardwareQc { get; set; }
        public string Commercial { get; set; }
        public string ProjectManager { get; set; }
        public string Production { get; set; }
        public string AftersalesPM { get; set; }
    }
}