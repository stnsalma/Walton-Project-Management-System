using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Models
{
    public class ProjectBabtModel
    {
        public ProjectBabtModel()
        {
            ProjectMasterModel = new ProjectMasterModel();
        }
        public long ProjectBabtId { get; set; }
        public long ProjectMasterId { get; set; }
        public long? PmAssignId { get; set; }
        public long? ProjectPurchaseOrderFormId { get; set; }
        public DateTime? PmImeiRangeRequestDate { get; set; }
        [Required(ErrorMessage = "TAC is Required")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Should be exactly 8 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only Number")]
        public string TacNo { get; set; }
        [Required(ErrorMessage = "TAC Request date is Required")]
        public DateTime? TacRequestDate { get; set; }
        [Required(ErrorMessage = "TAC Receive date is Required")]
        public DateTime? TacReceiveDate { get; set; }
        [Required(ErrorMessage = "Range Starts From is Required")]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "Should be exactly 7 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only Number")]
        public string ImeiRangeFrom { get; set; }
        [Required(ErrorMessage = "Range End point is Required")]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "Should be exactly 7 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only Number")]
        public string ImeiRangeTo { get; set; }
        public DateTime? RangeToPmDate { get; set; }
        public DateTime? RangeToSupplierDate { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }

        //User defined properties
        public string ProjectName { get; set; }
        public string ProjectManagerName { get; set; }
        public DateTime? PoDate { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public ProjectMasterModel ProjectMasterModel { get; set; }

        public long? RequestedImeiQuantity { get; set; }
        public long? GivenQuantityFromCommercial { get; set; }
        public long? PurchaseOrderQuantity { get; set; }
        public long? BabtRawId { get; set; }
        public long? RemainingRawImei { get; set; }
        public string OrderNo { get; set; }
    }
}