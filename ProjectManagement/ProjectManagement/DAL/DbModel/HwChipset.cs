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
    
    public partial class HwChipset
    {
        public long ChipsetId { get; set; }
        public string ChipsetVendor { get; set; }
        public string ChipsetCore { get; set; }
        public string ChipsetSpeed { get; set; }
        public string IcNoSize { get; set; }
        public string PinType { get; set; }
        public Nullable<int> PinNumber { get; set; }
        public string NewItemNo { get; set; }
        public string ItemCode { get; set; }
        public string Remarks { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}
