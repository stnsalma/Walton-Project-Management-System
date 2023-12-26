using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SpareClaimModel
    {

        public SpareClaimModel() 
        {
            ProjectMasterModels=new List<ProjectMasterModel>();
            ProjectMasterModel=new ProjectMasterModel();
        }

        public List<ProjectMasterModel> ProjectMasterModels { get; set; }
        public ProjectMasterModel ProjectMasterModel { get; set; }
        public long Id { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string PoCategory { get; set; }
        public int? OrderNumber { get; set; }
        public DateTime? SpareClaimDate { get; set; }
        public DateTime? WarehouseReceiveDate { get; set; }
        public long? Quantity { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? MonthRange { get; set; }
        public int? MonNum { get; set; }
        public int? YearName { get; set; }
        public string Month { get; set; }
        public int? SpareClaimIncentive { get; set; }
        public int TotalSpareClaim { get; set; }
        
        public decimal? ThisMonthAmount { get; set; }
        public decimal? TotalIncentive { get; set; }
        public decimal? AddedAmount { get; set; }
        public decimal? AmountDeduction { get; set; }
        public string DeductionRemarks { get; set; }
        public string RoleName { get; set; }
        public string DepartmentName { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }
        public string EmployeeCode { get; set; }
        public string UserId { get; set; }
    }
}