using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class NinetyFiveProductionRewardModel
    {
        public long? ProjectMasterID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectModel { get; set; }
        public string EmployeeCode { get; set; }
        public string UserFullName { get; set; }
        public string SourcingType { get; set; }
        public string ProjectType { get; set; }
        public string Orders { get; set; }
        public string WpmsOrders { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}")]
        public DateTime? WarehouseEntryDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}")]
        public DateTime? ExtendedWarehouseDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}")]
        public DateTime? PoDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}")]
        public DateTime? LSD { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}")]
        public DateTime? VesselDate { get; set; }
        public long? ProjectManagerUserId { get; set; }
        public long? OrderQuantity { get; set; }
        public long? TotalProductionQuantity { get; set; }
        public long? TotalSalesOut { get; set; }
        public long? DaysDiff { get; set; }
        public long? EffectiveDays { get; set; }
        public long? RewardPercentage { get; set; }
        public long? ExistedPercentage { get; set; }
        public long? TotalTblBarcodeIMEI { get; set; }
        public long? RewardAmount { get; set; }
        public long? DeductAmount { get; set; }
        public string Month { get; set; }
        public int MonNum { get; set; }
        public string Year { get; set; }
        public string IsFinalShipment { get; set; }
        public string ShipmentType { get; set; }
        public long Year1 { get; set; }
        public long? DeductPoint { get; set; }
        public long? AmountDeduct { get; set; }
        public long? DeductedAmount { get; set; }
        public long? RewardPoint { get; set; }
        public long? AmountReward { get; set; }

        public int? PoVsLSDDiff { get; set; }
        public int? LsdVsVesselDiffForDeduct { get; set; }
        public int? LsdVsVesselDiffForReward { get; set; }
        public long? PerDayDeduction { get; set; }
        public long? DaysDiffForReward { get; set; }
        public long? DaysDiffForDeduct { get; set; }
        public long? SmartBase { get; set; }
        public long? FeatureBase { get; set; }
        public long? PoReward { get; set; }
        public long? Penalties { get; set; }
        public string IsRefund { get; set; }
        public long? RefundAmount { get; set; }
        public Decimal? RefundAmount1 { get; set; }
        public int? RefundPercentage { get; set; }
        public DateTime? EffectiveMonth { get; set; }
        public string ProjectSourchingType { get; set; }

    }
}