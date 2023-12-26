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
    
    public partial class MaterialWastageDetail
    {
        public long Id { get; set; }
        public long MaterialWastageMasterId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public double BOMUnit { get; set; }
        public double WastagePercentage { get; set; }
        public int RecQtyWOWastage { get; set; }
        public int RecQtyWWastage { get; set; }
        public int TotalLot { get; set; }
        public int WastageWOBom { get; set; }
        public int WastageWBom { get; set; }
        public int TotalWastage { get; set; }
        public int AssemMaterialFault { get; set; }
        public int AssemProcessFault { get; set; }
        public int RepMaterialFault { get; set; }
        public int RepProcessFault { get; set; }
        public int TotalFault { get; set; }
        public int TotalMaterialFaultApproved { get; set; }
        public int TotalProcessFaultApproved { get; set; }
        public int TotalFaultApproved { get; set; }
        public int TillNowAssemMaterialFault { get; set; }
        public int TillNowAssemProcessFault { get; set; }
        public int TillNowRepMaterialFault { get; set; }
        public int TillNowRepProcessFault { get; set; }
        public int TillNowTotalFault { get; set; }
        public double ActualAssemblyWastage_TotalLot { get; set; }
        public double ActualRepairWastage_TotalLot { get; set; }
        public double ActualWastageOfTotalLot { get; set; }
        public double NetAdjustment { get; set; }
        public int ImportedQtyWithWastage { get; set; }
        public int WastageQtyInBOM { get; set; }
        public int NeedToDeclare { get; set; }
        public int AlreadySined { get; set; }
        public int NeedSign { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }
        public int CrossCheck { get; set; }
        public Nullable<System.DateTime> FOCTakenDate { get; set; }
        public int FOCQty { get; set; }
        public string Remarks { get; set; }
        public string BOMType { get; set; }
        public System.DateTime AddedDate { get; set; }
        public long AddedBy { get; set; }
        public Nullable<long> BOMTypeId { get; set; }
    
        public virtual MaterialWastageMaster MaterialWastageMaster { get; set; }
    }
}
