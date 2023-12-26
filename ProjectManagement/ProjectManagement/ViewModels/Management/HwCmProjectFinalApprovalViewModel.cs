using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Management
{
    public class HwCmProjectFinalApprovalViewModel
    {
        #region ProjectMaster
        public long ProjectMasterId { get; set; }
        public long ProjectTypeId { get; set; }
        public string ProjectName { get; set; }
        public string SupplierName { get; set; }
        public string SupplierModelName { get; set; }
        public Nullable<int> NumberOfSample { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public System.DateTime? ApproxProjectFinishDate { get; set; }
        public string SupplierTrustLevel { get; set; }
        public Nullable<bool> IsScreenTestComplete { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> ApproxProjectOrderDate { get; set; }
        public Nullable<System.DateTime> ApproxShipmentDate { get; set; }
        public Nullable<bool> IsNew { get; set; }
        public Nullable<bool> IsProjectManagerAssigned { get; set; }
        public string ProjectType { get; set; }
        public Nullable<bool> IsReorder { get; set; }
        public string OsName { get; set; }
        public string OsVersion { get; set; }
        public Nullable<decimal> DisplaySize { get; set; }
        public string DisplayName { get; set; }
        public string ProcessorName { get; set; }
        public Nullable<decimal> ProcessorClock { get; set; }
        public string Chipset { get; set; }
        public string FrontCamera { get; set; }
        public string BackCamera { get; set; }
        public string Ram { get; set; }
        public string Rom { get; set; }
        public string Battery { get; set; }
        public string ProjectStatus { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        #endregion
        #region HwQcInchargeAssigns
        public long HwQcInchargeAssignId { get; set; }
        //public long ProjectMasterId { get; set; }
        public long HwQcInchargeUserId { get; set; }
        public Nullable<long> HwQcInchargeAssignedBy { get; set; }
        public Nullable<System.DateTime> HwQcInchargeAssignDate { get; set; }
        public Nullable<bool> IsScreeningTest { get; set; }
        public Nullable<bool> IsRunningTest { get; set; }
        public Nullable<bool> IsFinishedGoodTest { get; set; }
        public string TestPhase { get; set; }
        public string Remark { get; set; }
        //public Nullable<long> Added { get; set; }
        //public Nullable<System.DateTime> AddedDate { get; set; }
        //public Nullable<long> Updated { get; set; }
        //public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<long> ProjectPmAssignId { get; set; }
        public Nullable<long> ProjectOrderShipmentId { get; set; }
        public Nullable<long> ProjectManagerUserId { get; set; }
        public string ProjectManagerAssignComment { get; set; }
        public string ProjectManagerSampleType { get; set; }
        public Nullable<int> ProjectManagerSampleNo { get; set; }
        public Nullable<System.DateTime> ApproxPmToHwDeliveryDate { get; set; }
        public string PriorityFromPm { get; set; }
        public string Status { get; set; }
        #endregion
        #region HwQcAssigns
        public long HwQcAssignId { get; set; }
        // public Nullable<long> ProjectMasterId { get; set; }
        public Nullable<long> HwAllTestId { get; set; }
        // public long HwQcInchargeAssignId { get; set; }
        public long HwQcUserId { get; set; }
        public System.DateTime HwQcAssignDate { get; set; }
        public string QcDocUploadPath { get; set; }
        public Nullable<System.DateTime> HwDocUploadDate { get; set; }
        public Nullable<long> VerifiedBy { get; set; }
        public string VerifierName { get; set; }
        public Nullable<System.DateTime> VerificationDate { get; set; }
        // public string Status { get; set; }
        //  public Nullable<long> Added { get; set; }
        //  public Nullable<System.DateTime> AddedDate { get; set; }
        //  public Nullable<long> Updated { get; set; }
        //  public Nullable<System.DateTime> UpdatedDate { get; set; }
        #endregion
        #region HwIssueComments
        public long HwIssueCommentId { get; set; }
        // public long HwQcAssignId { get; set; }
        // public long ProjectMasterId { get; set; }
        public string IssueName { get; set; }
        public string IssueTypeName { get; set; }
        public string IssueTypeDetailName { get; set; }
        public string IssueComment { get; set; }
        public System.DateTime IssueCommetDate { get; set; }
        public string CommercialComment { get; set; }
        public System.DateTime? CommercialCommentDate { get; set; }
        public string VerifierComment { get; set; }
        public string IssueStatus { get; set; }
        //public Nullable<long> Added { get; set; }
        //public Nullable<System.DateTime> AddedDate { get; set; }
        //public Nullable<long> Updated { get; set; }
        //public Nullable<System.DateTime> UpdatedDate { get; set; }
        #endregion
    }
}