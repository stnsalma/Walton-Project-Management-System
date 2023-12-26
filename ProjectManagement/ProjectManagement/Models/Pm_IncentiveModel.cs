using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
  
    public class Pm_IncentiveModel
    {
        public Pm_IncentiveModel()
        {
            CustomPmIncentiveModels=new List<Custom_Pm_IncentiveModel>();
            FilesDetails = new List<FilesDetail>();
        }
        public List<FilesDetail> FilesDetails { get; set; }
        public long Id { get; set; }
        public string IsDocumentUploaded { get; set; }
        public string EmployeeCode { get; set; }
        public long Pm_Incentive_Base_Id { get; set; }
        public long ProjectId { get; set; }
        public string ProjectIds { get; set; }
        public string IncentiveType { get; set; }
        public Nullable<int> PersonNo { get; set; }
        public decimal Amount { get; set; }
        public decimal DeductionAmount { get; set; }
        public decimal? DeductAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string D_Remarks { get; set; }
        public string Remarks { get; set; }
        public string CurrencyType { get; set; }
        public string Month { get; set; }
        public int MonNum { get; set; }
        public long Year { get; set; }
        public string DepartmentName { get; set; }
        public string ProjectName { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public List<Custom_Pm_IncentiveModel> CustomPmIncentiveModels { get; set; }
        public string OthersType { get; set; }

        #region new incentive policy 2020-08-09
        public long? ProjectMasterId { get; set; }
        public int? Orders { get; set; }
        public List<HttpPostedFileBase> UploderDocs { get; set; }
        public string SupportingDocument { get; set; }
        public DateTime? EffectiveMonth { get; set; }
        public string AccessoriesType { get; set; }
        public string MultiProjectName { get; set; }
        public string MultiProjectIds { get; set; }
        public DateTime? ProjectClosingDate { get; set; }
        public DateTime? MarketClearanceDate { get; set; }

        public string PoCategory { get; set; }
        public long? PoQuantity { get; set; }
        public DateTime? ProjectManagerClearanceDate { get; set; }
        public DateTime? RawMaterialAddedDate { get; set; }
        public int? DaysPassed { get; set; }
        public string IncentiveName { get; set; }
        public string ProjectType { get; set; }
        public DateTime? LSD { get; set; }
        public int? DaysBeforeLsd { get; set; }
        public int? DaysAfterLsd { get; set; }
        public decimal? Reward { get; set; }
        public decimal? RealPenalties { get; set; }
        public decimal? Penalties { get; set; }
        public string PmName { get; set; }

        #endregion
    }
}