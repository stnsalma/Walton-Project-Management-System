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
    
    public partial class HwTestSoundInfo
    {
        public long HwTestSoundInfoId { get; set; }
        public Nullable<long> HwQcAssignId { get; set; }
        public Nullable<long> HwQcInchargeAssignId { get; set; }
        public string Recommendation { get; set; }
        public string Speaker_Size { get; set; }
        public string Receiver_Size { get; set; }
        public string Speaker_TwoInOne_Size { get; set; }
        public string MIC { get; set; }
        public string Comment { get; set; }
        public string QcDocUploadPath { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    
        public virtual HwQcAssign HwQcAssign { get; set; }
    }
}