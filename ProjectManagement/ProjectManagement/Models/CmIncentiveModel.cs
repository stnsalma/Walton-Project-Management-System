using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class CmIncentiveModel
    {
        public string OthersType { get; set; }
        public string IncentiveTypes { get; set; }
        public long? ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public int? Orders { get; set; }
        public string PoCategory { get; set; }
        public long? PoQuantity { get; set; }
        public int? LotNumber { get; set; }
        public long? LotQuantity { get; set; }
        public DateTime? ProjectManagerClearanceDate { get; set; }
        public DateTime? EffectiveMonth { get; set; }
        public string ChinaIqcPassHundredPercent { get; set; }
        public int? NoOfTimeInspection { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AddedAmount { get; set; }
        public decimal? TotalDeduction { get; set; }
        public string DeductionRemarks { get; set; }
        public decimal? FinalAmount { get; set; }
        public string Remarks { get; set; }
        public int? MonNum { get; set; }
        public long? Year { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}