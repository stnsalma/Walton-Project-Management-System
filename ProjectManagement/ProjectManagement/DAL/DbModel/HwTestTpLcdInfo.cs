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
    
    public partial class HwTestTpLcdInfo
    {
        public long HwTestTpLcdInfoId { get; set; }
        public Nullable<long> HwQcAssignId { get; set; }
        public Nullable<long> HwQcInchargeAssignId { get; set; }
        public string Recommendation { get; set; }
        public Nullable<bool> Technology { get; set; }
        public string LCD_Size { get; set; }
        public string LCD_MaterialInfo { get; set; }
        public string LCD_Resolution { get; set; }
        public Nullable<bool> LCD_BackSteelProtection { get; set; }
        public string LCD_Brigntness { get; set; }
        public string LCD_DriverIcInfo { get; set; }
        public string TP_Point { get; set; }
        public string TP_MaterialType { get; set; }
        public string TP_DriverInfo_Vendor { get; set; }
        public string TP_DriverInfo_IcTouch { get; set; }
        public string TP_DriverInfo_Capability { get; set; }
        public string TP_ProtectionGlassThickness { get; set; }
        public string TP_ProtectionGlassInfo { get; set; }
        public Nullable<bool> TP_DriverAtPCBA { get; set; }
        public Nullable<bool> FPC_TpAndLcdConnection { get; set; }
        public string Comment { get; set; }
        public string QcDocUploadPath { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    
        public virtual HwQcAssign HwQcAssign { get; set; }
    }
}
