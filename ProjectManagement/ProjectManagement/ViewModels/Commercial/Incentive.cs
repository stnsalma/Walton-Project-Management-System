using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Commercial
{
    public class Incentive
    {
        public string Role { get; set; }
        public string UserName { get; set; }
        public string EmployeeCode { get; set; }
        public string Id { get; set; }
        public string Percentage { get; set; }
        public string FinalAmount { get; set; }
        public long? Incentives { get; set; }
        public Nullable<decimal> FixedIncentive { get; set; }
        public Nullable<decimal> AddedAmount { get; set; }
        public Nullable<decimal> AmountDeduction { get; set; }
        public string DeductionRemarks { get; set; }
        public string Remarks { get; set; }
       
        public long CarryOver { get; set; }
        public long ThisMonthAmount { get; set; }
        public Decimal? TotalReward { get; set; }
        public Decimal? Reward { get; set; }
        public Decimal? TotalPenalties { get; set; }
        public Decimal? Penalties { get; set; }
        public decimal? ImportedSpareValue { get; set; }
        public decimal? GivenHandsetValue { get; set; }
        public decimal? SpecialAmount { get; set; }
        public string SpecialRemarks { get; set; }
    }
}