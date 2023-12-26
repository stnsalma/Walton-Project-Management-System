using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class IncentiveModel
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> ThisMonthAmount { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> TotalIncentive { get; set; }
        public Nullable<decimal> FixedIncentive { get; set; }
        public Nullable<double> Percentage { get; set; }
        public string Month { get; set; }
        public Nullable<int> MonNum { get; set; }
        public Nullable<long> AmountCarry { get; set; }
        public Nullable<decimal> AmountDeduction { get; set; }
        public string DeductionRemarks { get; set; }
        public Nullable<decimal> AddedAmount { get; set; }
        public string Remarks { get; set; }
        public Nullable<long> Year { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        //QcHwad and Deputy
        public Decimal TeamIncentive { get; set; }
        public int SwPercentage { get; set; }
        public Decimal SwIncentive { get; set; }
    }
}