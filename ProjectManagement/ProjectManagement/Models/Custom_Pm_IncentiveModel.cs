using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace ProjectManagement.Models
{
    public class Custom_Pm_IncentiveModel
    {
        public Custom_Pm_IncentiveModel()
        {
            CmnUserModelsList = new List<CmnUserModel>();
            CmnUserModel=new CmnUserModel();
        }
        public long Id { get; set; }
        public string IncentiveTypeForAccessories { get; set; }
        public string EmployeeCode { get; set; }
        public string MultiProjectName { get; set; }
        public string MultiProjectIds { get; set; }
        public string ProjectModel { get; set; }
        public int? PersonNo { get; set; }
        public long Pm_Incentive_Base_Id { get; set; }
        public long ProjectId { get; set; }
        public string Amount { get; set; }
        public decimal? Amount1 { get; set; }
        public string DeductionAmount { get; set; }
        public decimal? DeductionAmount1 { get; set; }
        public decimal? DeductByIncharge { get; set; }
        public decimal? SystemDeduction { get; set; }
        public string FinalAmount { get; set; }
        public decimal FinalAmount1 { get; set; }
        public long? FinalAmount2 { get; set; }
        public decimal? Penalties { get; set; }
        public decimal? Reward { get; set; }
        public decimal? TotalPenalties { get; set; }
        public decimal TotalIncentive { get; set; }
        public string TotalIncentive1 { get; set; }
        public string D_Remarks { get; set; }
        public string Remarks { get; set; }
        public string Month { get; set; }
        public int MonNum { get; set; }
        public string Year { get; set; }
        public long Year1 { get; set; }
        public string DepartmentName { get; set; }
        public string ProjectName { get; set; }
        public DateTime? AddedDate { get; set; }
        public long Added { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long Updated { get; set; }
        public string PoCategory { get; set; }
        public string SourcingType { get; set; }
        public DateTime? PoDate { get; set; }
        public DateTime? WarehouseEntryDate { get; set; }
        public DateTime? ExtendedWarehouseDate { get; set; }
        public DateTime? VesselDate { get; set; }
        public DateTime? RawMaterialAddedDate { get; set; }
        public int? OrderNumber { get; set; }
        public int? DaysPassed { get; set; }
        public long? ProjectManagerUserId { get; set; }
        public string ProjectType { get; set; }

        public string IncentiveTypes { get; set; }
        public string Others { get; set; }
        public DateTime? ApproxShipmentDate { get; set; }
        public DateTime? LSD { get; set; }
        public DateTime? ChainaInspectionDate { get; set; }
        public DateTime? ShipmentTaken { get; set; }
        public int? NoOfdays { get; set; }
        public int? EarlierOrLateShipment { get; set; }
        public string OrderNumberOrdinal { get; set; }
        public string UserFullName { get; set; }
        public List<CmnUserModel> CmnUserModelsList { get; set; }
        public CmnUserModel CmnUserModel { get; set; }
        public DateTime? ProjectManagerClearanceDate { get; set; }
        public string ClosingMonth { get; set; }  
        public DateTime? ClosingDate { get; set; }
        public string ClosingAmount { get; set; }
        public string ClosingType { get; set; }
        public DateTime? FlightDepartureDate { get; set; }
        public string OthersIncentiveName { get; set; }

        //Aftersales Pm//
        public string IncentiveRemarks { get; set; }
        public long? AsmUserId { get; set; }
        public long? RewardAmount { get; set; }
        public decimal? DeductedAmount { get; set; }
        public string UnitPrice { get; set; }
        public string FinalAmountForHead { get; set; }
        public string FinalAmountForOthers { get; set; }
        public string SpareName { get; set; }
        public string Orders { get; set; }

        public decimal TotalIncentiveForIncentiveType { get; set; }
        public decimal TotalIncentiveForFoc { get; set; }

        public long? PoReward { get; set; }
        public string IncsTypes { get; set; }
        public string ShipmentType { get; set; }
        public long? DeductPoint { get; set; }
        public long? PoQuantity { get; set; }
        public long? RewardPoint { get; set; }
        public int? PoVsLSDDiff { get; set; }
        public int? LsdVsVesselDiffForDeduct { get; set; }
        public int? LsdVsVesselDiffForReward { get; set; }
        public long? PerDayDeduction { get; set; }
        public long? SmartBase { get; set; }
        public long? FeatureBase { get; set; }
        public long? OrderQuantity { get; set; }
        public long? TotalSalesOut { get; set; }
        public long? ExistedPercentage { get; set; }
        public Decimal? TeamIncentive { get; set; }
        public Decimal? InchargePecentage { get; set; }

        //new//
        public string IncentiveType { get; set; }
        public long? ProjectMasterId { get; set; }
        public DateTime? EffectiveMonth { get; set; }
        public string AccessoriesType { get; set; }
        public Nullable<decimal> DeductAmount { get; set; }
        public string DeductRemarks { get; set; }
        public string ProjectIds { get; set; }
        
        public DateTime? ProjectClosingDate { get; set; }
        public DateTime? MarketClearanceDate { get; set; }

       
    }
}