using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class Custom_Sw_IncentiveModel
    {

        public Custom_Sw_IncentiveModel()
        {
            CmnUserModelsList = new List<CmnUserModel>();
            CmnUserModel = new CmnUserModel();
            ProjectMasterModels=new List<ProjectMasterModel>();
            ProjectMasterModel=new ProjectMasterModel();
            ProjectMastersForPenalties=new List<ProjectMasterModel>();
            ProjectMastersForPenalty=new ProjectMasterModel();
        }

        public List<ProjectMasterModel> ProjectMasterModels { get; set; }
        public ProjectMasterModel ProjectMasterModel { get; set; }

        public List<ProjectMasterModel> ProjectMastersForPenalties { get; set; }
        public ProjectMasterModel ProjectMastersForPenalty { get; set; }
        public int? Percentage { get; set; }
        public List<CmnUserModel> CmnUserModelsList { get; set; }
        public CmnUserModel CmnUserModel { get; set; }
        public decimal? FinalAmount1 { get; set; }
        public string EmployeeCode { get; set; }
        public int? PersonNo { get; set; }
        public long? ProjectId { get; set; }
        public long? SwQcHeadAssignId { get; set; }
        public int? TestPhaseId { get; set; }
        public int? SoftwareVersionNumber { get; set; }
        public string IncentiveClaimArea { get; set; }
        public string TestPhaseName { get; set; }
        public string SoftwareVersionName { get; set; }
        public string Amount { get; set; }
        public string MonNum1 { get; set; }
        public int? Critical { get; set; }
        public int? Major { get; set; }
        public int? Minor { get; set; }
        public Decimal? BaseAmount { get; set; }
        public Decimal? IssueAmount { get; set; }
        public Decimal? TotalAmount { get; set; }
      
        public decimal? AddedAmount { get; set; }
        public string AddAmountRemarks { get; set; }
        public decimal? Deduction { get; set; }
        public Decimal? FinalIssueAmount { get; set; }
        public string DeductionRemarks { get; set; }
        public decimal? FinalAmount { get; set; }

        public decimal? ThisMonthAmount { get; set; }
        public decimal? TotalIncentive { get; set; }
        public string TotalIncentive1 { get; set; }

        public string Month { get; set; }
        public int MonNum { get; set; }
        public string Year { get; set; }
        public long Year1 { get; set; }
        public int Year2 { get; set; }
        public string DepartmentName { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public string IncentiveTypes { get; set; }
        public string Others { get; set; }
        public string OrderNumberOrdinal { get; set; }
        public string UserFullName { get; set; }
        public string OthersIncentiveName { get; set; }
        public string IncentiveRemarks { get; set; }
        public string FinalAmountForHead { get; set; }
        public string FinalAmountForOthers { get; set; }
        //
        public string PenaltiesReason { get; set; }
        public string AssignedEmployees { get; set; }
        public int AssignedPersons { get; set; }
        public Decimal? ParticularPersonIncentive { get; set; }
        public Decimal? TotalPenalties { get; set; }
        public Decimal? ParticularPersonsPenalties { get; set; }
        public string PenaltiesRemarks { get; set; }
        public Decimal? TotalAmountForBrand { get; set; }
        public int BrandIssueAmountPercentage { get; set; }
        public Decimal? BrandFinalAmount { get; set; }
        public string BrandRemarks { get; set; }

        public Decimal? BrandCost { get; set; }
        public int? BrandCostPercentage { get; set; }//BrandCostPerPersonIncentive
        public Decimal? BrandCostPerPersonIncentive { get; set; }
        public Decimal? BrandCostAddedAmount { get; set; }
        public string BrandCostAddedRemarks { get; set; }
        public Decimal? BrandCostDeduction { get; set; }
        public string BrandCostDeductionRemarks { get; set; }
        public Decimal? BrandCostFinalAmount { get; set; }

        public Decimal? FinalTotalAmount { get; set; }
        public Decimal? FinalAddedAmount { get; set; }
        public string FinalAddedRemarks { get; set; }
        public Decimal? FinalDeduction { get; set; }
        public string FinalDeductionRemarks { get; set; }
        public Decimal? FinalIncentive { get; set; }
        
        //New Innovation
        public long NewInnovationId { get; set; }
        public string AssignedBy { get; set; }
        public string Description { get; set; }
        public string WorkType { get; set; }
        public DateTime EffectiveDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? EffectiveMonth { get; set; }
        public string Persons { get; set; }
       //Personal use
        public long SwQcPrUseFindId { get; set; }
        //
        public string OthersType { get; set; }
        public string RoleName { get; set; }
        //

        public Decimal? BrandIssuebrandFinalAmount { get; set; }
        public string BrandIssuebrandRemarks { get; set; }
        public string Types { get; set; }

        public Decimal? TotalIssuePercentage { get; set; }
        public Decimal? PreviousDeductedAmount { get; set; }
        public Decimal? PerPersonPenalties { get; set; }
        public string PenaltiesPercentage { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public long? TotalDeduction { get; set; }
        public long? TotalReward { get; set; }
        public Decimal? PerPersonReward { get; set; }
        public Decimal? Reward { get; set; }
        public Decimal? Penalties { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? DateDiffWithHoliday { get; set; }
        public decimal? Timeline { get; set; }
        public Decimal? HundredPercentIssueAmount { get; set; }
        public string ClaimingField { get; set; }
    }
}