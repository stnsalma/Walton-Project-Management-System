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
    
    public partial class HwFgUsbTestDetail
    {
        public long HwFgUsbTestDetailId { get; set; }
        public long HwFgUsbCableTestId { get; set; }
        public string TestTopic { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public string StandardValue { get; set; }
        public Nullable<bool> Result { get; set; }
        public string Remarks { get; set; }
        public string QcDocUploadPath { get; set; }
        public Nullable<long> AddedBy { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    
        public virtual HwFgUsbCableTest HwFgUsbCableTest { get; set; }
    }
}