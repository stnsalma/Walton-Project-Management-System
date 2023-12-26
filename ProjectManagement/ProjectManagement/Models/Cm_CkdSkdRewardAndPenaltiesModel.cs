using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class Cm_CkdSkdRewardAndPenaltiesModel
    {
        public long Id { get; set; }
        public Nullable<long> ProjectMasterID { get; set; }
        public string ProjectModel { get; set; }
        public string Orders { get; set; }
        public string ProjectType { get; set; }
        public string ShipmentType { get; set; }
        public Nullable<System.DateTime> PoDate { get; set; }
        public Nullable<System.DateTime> WarehouseEntryDate { get; set; }
        public Nullable<int> DaysDiff { get; set; }
        public Nullable<int> EffectiveDays { get; set; }
        public Nullable<int> DeductPoint { get; set; }
        public Nullable<int> DaysDiffForDeduct { get; set; }
        public Nullable<decimal> AmountDeduct { get; set; }
        public Nullable<int> RewardPoint { get; set; }
        public Nullable<int> DaysDiffForReward { get; set; }
        public Nullable<decimal> AmountReward { get; set; }
        public string IsFinalShipment { get; set; }
        public string IncentiveType { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string SourcingType { get; set; }
        public Nullable<System.DateTime> ExtendedWarehouseDate { get; set; }
        public Nullable<long> OrderQuantity { get; set; }
        public Nullable<long> TotalProductionQuantity { get; set; }
        public Nullable<int> RewardPercentage { get; set; }
        public Nullable<int> ExistedPercentage { get; set; }
        public Nullable<long> TotalTblBarcodeIMEI { get; set; }
        public Nullable<long> TotalSalesOut { get; set; }
    }
}