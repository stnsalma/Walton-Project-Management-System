using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Commercial
{
    public class VmIncentivePolicy
    {
        public VmIncentivePolicy()
        {
            CmIncentiveModels=new List<CmIncentiveModel>();
            CmIncentiveModel=new CmIncentiveModel();
        }
        public List<CmIncentiveModel> CmIncentiveModels { get; set; }
        public CmIncentiveModel CmIncentiveModel { get; set; }
        #region Incentive Parameter
        public long Id { get; set; }
        public long ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public string ParameterName { get; set; }
        public decimal? Parameter { get; set; }
        public decimal? ParameterValue { get; set; }
        public decimal? OrdersValue { get; set; }
        public bool? IsActive { get; set; }
        public string RoleName { get; set; }
        public string PrimaryParameter_Total_Value1 { get; set; }
        public string PrimaryParameterFromOracle { get; set; }
        public string TotalServiceIMEI { get; set; }
        public string TotalSalesBarcode { get; set; }
        public long ServiceToSalesForFeaturePhone { get; set; }
        #endregion

        #region Incentive
        public string UserId { get; set; }
       // public string UserId1 { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? ThisMonthAmount { get; set; }
        public double? Percentage { get; set; }
        public decimal? Amount { get; set; }
        public decimal? TotalIncentive { get; set; }
        public decimal? FixedIncentive { get; set; }
        public decimal? SalesAmt { get; set; }
        public decimal? AddedAmount { get; set; }
        public string Month { get; set; }
        public int MonNum { get; set; }
        public int Orders { get; set; }
        public int PerLc { get; set; }
        public int SeaShipmentFull { get; set; }
        public int SeaShipmentPartial { get; set; }
        public int AirShipmentFull { get; set; }
        public int AirShipmentPartial { get; set; }
        public long? AmountCarry { get; set; }
        public decimal? AmountDeduction { get; set; }
        public decimal? ImportedSpareValue { get; set; }
        public decimal? GivenHandsetValue { get; set; }
        public string DeductionRemarks { get; set; }
        public string Remarks { get; set; }
        public string Year { get; set; }
        public string DepartmentName { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Added { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long? Updated { get; set; }
        #endregion 


        public long CmnUserId { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }    
        public string EmployeeCode { get; set; }
        public string AssignRoles { get; set; }  
        public string ExtendedRoleName { get; set; }

        public int? Share { get; set; }
        public long? CarryAmount { get; set; }
        public Decimal? TotalDeduction { get; set; }
        public Decimal? TotalDeduction1 { get; set; }
        public Decimal? TotalReward { get; set; }
        public Decimal? PerPersonReward { get; set; }
        public Decimal? PerPersonPenalties { get; set; }
        public Decimal? TotalReward1 { get; set; }
        public Decimal? Reward { get; set; }
        public Decimal? TotalPenalties { get; set; }
        public Decimal? Penalties { get; set; }
        public Decimal? TotalRefund { get; set; }
        public Decimal? TotalRefund1 { get; set; }
        public string PoCategory { get; set; }
        public long? PoQuantity { get; set; }
        public int? LotNumber { get; set; }
        public long? LotQuantity { get; set; }
        public DateTime? ProjectManagerClearanceDate { get; set; }
        public int? NoOfTimeInspection { get; set; }
        public string ChinaIqcPassHundredPercent { get; set; }
        public string OthersType { get; set; }
        public DateTime? EffectiveMonth { get; set; }
        public Decimal? SpecialAmount { get; set; }
        public string SpecialRemarks { get; set; }
    }
}