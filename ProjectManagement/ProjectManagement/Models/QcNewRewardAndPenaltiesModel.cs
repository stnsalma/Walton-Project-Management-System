using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class QcNewRewardAndPenaltiesModel
    {
        public long Id { get; set; }
        public long? ProjectMasterID { get; set; }
        public string ProjectModel { get; set; }
        public string Orders { get; set; }
        public string SourcingType { get; set; }
        public string ShipmentType { get; set; }
        public int? TeamMember { get; set; }
        public long? RewardAmount { get; set; }
        public long? TeamReward { get; set; }
        public long? PerPersonReward { get; set; }
        public long? DeputyAmount { get; set; }
        public long? HeadAmount { get; set; }
        public long? DeductedAmount { get; set; }
        public long? TeamPenalties { get; set; }
        public long? PerPersonPenalties { get; set; }
        public long? DeputyPenalties { get; set; }
        public long? HeadPenalties { get; set; }
        public DateTime? WarehouseEntryDate { get; set; }
        public DateTime? ExtendedWarehouseDate { get; set; }
        public DateTime? LSD { get; set; }
        public DateTime? VesselDate { get; set; }
        public int? LsdVsVesselDiffForDeduct { get; set; }
        public int? LsdVsVesselDiffForReward { get; set; }
        public int? EffectiveDays { get; set; }
        public long? OrderQuantity { get; set; }
        public long? TotalSalesOut { get; set; }
        public long? TotalProductionQuantity { get; set; }
        public int? ExistedPercentage { get; set; }
        public int? MonNum { get; set; }
        public long? Year { get; set; }
        public string IncentiveType { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //new
        public long? RawMaterialId { get; set; }
        //public long? ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public string PoCategory { get; set; }
        public DateTime? ProjectManagerClearanceDate { get; set; }
        public int? DaysBeforeLsd { get; set; }
        public int? DaysAfterLsd { get; set; }
        public decimal? Reward { get; set; }
        public decimal? RealPenalties { get; set; }
        public decimal? FinalAmount { get; set; }
        public decimal? Penalties { get; set; }
        public decimal? DeputyReward { get; set; }
        public decimal? HeadReward { get; set; }
        public string RoleName { get; set; }
        public string EmployeeCode { get; set; }
        public string Month { get; set; }

    }
}