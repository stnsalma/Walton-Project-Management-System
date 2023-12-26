using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.ProjectManager
{
    public class AssignProjectsViewModel
    {
        public AssignProjectsViewModel()
        {
            ProjectMasterModel = new ProjectMasterModel();
            ProjectMasterModelp=new ProjectMasterModel();
            ProjectMasterModels=new List<ProjectMasterModel>();
            ProjectMasterModelsForBomName = new List<ProjectMasterModel>();
            FilesDetails = new List<FilesDetail>();
        }
        public List<FilesDetail> FilesDetails { get; set; }
        public ProjectMasterModel ProjectMasterModel { get; set; }
        public ProjectMasterModel ProjectMasterModelp { get; set; }
        public List<ProjectMasterModel> ProjectMasterModels { get; set; }
        public List<ProjectMasterModel> ProjectMasterModelsForBomName { get; set; }
        public ProjectPmAssignModel ProjectPmAssignModel { get; set; }
        public PmCmnUserModel PmCmnUserModel { get; set; }
        public List<CmnUserModel> CmnUserModels { get; set; }

        ////
        public long RawMaterialId { get; set; }
        public long? ProjectMasterId { get; set; }
        public long? ProjectPurchaseOrderFormId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public int? Orders { get; set; }
        public string PoCategory { get; set; }
        public long? PoQuantity { get; set; }
        public long? TotalQuantity { get; set; }
        public DateTime? ProjectManagerClearanceDate { get; set; }
        public string ChinaIqcPassHundredPercent { get; set; }
        public int? NoOfTimeInspection { get; set; }
        public string SourcingApproval { get; set; }
        public string ManagementApproval { get; set; }
        public DateTime? ManagementApprovalDate { get; set; }
        public string SupportingDocument { get; set; }
        public string BOMType { get; set; }
        public string BOMName { get; set; }
        public string Remarks { get; set; }
        public string Color { get; set; }
        public string Attachment { get; set; }
        public string InspectionRemarks { get; set; }
        public string ItemQuantity { get; set; }
        public int? LotNumber { get; set; }
        public long? LotQuantity { get; set; }
        public string BomRemarks { get; set; }
        public string PmToQcHeadAssignComment { get; set; }
        public string ProjectManagerSampleType { get; set; }
        public string SoftwareVersionName { get; set; }
        public int? NewIssue { get; set; }
        public string Status { get; set; }
        public int? ProjectManagerSampleNo { get; set; }
        public int? SoftwareVersionNo { get; set; }
        public DateTime? PmToQcHeadAssignTime { get; set; }
        public DateTime? SwQcFinishedTime { get; set; }
        public int? FeedbackDuration { get; set; }
        public long FocClaimId { get; set; }
        public string ReceiveQuantity { get; set; }
        public string ReceiveRemarks { get; set; }

        public string MajorDelayReason { get; set; }
        public string HardwareSampleReceive { get; set; }
        public string InspectionMajorFailItems { get; set; }
        public string OrderColorRatioWithQty { get; set; }
        public DateTime? InspectionStartingDate { get; set; }
    }
}