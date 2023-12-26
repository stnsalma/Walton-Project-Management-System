using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwQcOthersIncentiveModel
    {
        public SwQcOthersIncentiveModel()
        {
            Custom_Sw_IncentiveModels = new List<Custom_Sw_IncentiveModel>();
            Custom_Sw_IncentiveModel=new Custom_Sw_IncentiveModel();
        }

        public List<Custom_Sw_IncentiveModel> Custom_Sw_IncentiveModels { get; set; }
        public Custom_Sw_IncentiveModel Custom_Sw_IncentiveModel { get; set; }
        public string OthersType { get; set; }
        public int Months { get; set; }
        public int Years { get; set; }
        public Decimal TotalAmount { get; set; }
      
        public string EmployeeCode { get; set; }
        public long Deduction { get; set; }
        public string DeductionRemarks { get; set; }
        public long AddedAmount { get; set; }
        public string AddAmountRemarks { get; set; }
        public string PenaltiesReason { get; set; }
        public int AssignedPersons { get; set; }
        public Decimal ParticularPersonIncentive { get; set; }
        public Decimal TotalPenalties { get; set; }
        public Decimal ParticularPersonsPenalties { get; set; }
        public string PenaltiesRemarks { get; set; }
        public Decimal TotalAmountForBrand { get; set; }
        public int BrandIssueAmountPercentage { get; set; }
        public Decimal BrandFinalAmount { get; set; }
        public string BrandRemarks { get; set; }


        public Decimal BrandCost { get; set; }
        public Decimal BrandCostPercentage { get; set; }
        public Decimal BrandCostPerPersonIncentive { get; set; }
        public Decimal BrandCostAddedAmount { get; set; }
        public string BrandCostAddedRemarks { get; set; }
        public Decimal BrandCostDeduction { get; set; }
        public string BrandCostDeductionRemarks { get; set; }

      
        //finals
        public Decimal FinalTotalAmount { get; set; }
        public Decimal? FinalIssueAmount { get; set; }
        public Decimal? HundredPercentIssueAmount { get; set; }
        public Decimal FinalAddedAmount { get; set; }
        public string FinalAddedRemarks { get; set; }
        public Decimal FinalDeduction { get; set; }
        public string FinalDeductionRemarks { get; set; }
        public Decimal FinalIncentive { get; set; }

        //others
        public int? TestPhaseId { get; set; }
        public string IncentiveClaimArea { get; set; }
        public int? Critical { get; set; }
        public int? Major { get; set; }
        public int? Minor { get; set; }
        public decimal? BaseAmount { get; set; }
        public decimal? IssueAmount { get; set; }
         [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EffectiveMonth { get; set; }

    }
}