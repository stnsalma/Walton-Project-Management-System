using System.Collections.Generic;
using ProjectManagement.Models;


namespace ProjectManagement.ViewModels.Hardware
{
    public class VmHardwareTest
    {
        public VmHardwareTest()
        {
            ProjectMasterModel = new ProjectMasterModel();
            HwQcAssignModel = new HwQcAssignModel();
            HwQcInchargeAssignModel=new HwQcInchargeAssignModel();
            HwAllTestModel=new HwAllTestModel();
            CmnUserModel = new CmnUserModel();
            HwQcTestCounterModel = new HwQcTestCounterModel();
            HwQcAssignCustomMasterModel= new HwQcAssignCustomMasterModel();
            HwGetQcAssignedByInchargeModel = new List<HwGetQcAssignedByInchargeModel>();
            HwIssueMasterModel=new HwIssueMasterModel();
            HwIssueTypeModel=new HwIssueTypeModel();
            HwIssueTypeDetailModel=new HwIssueTypeDetailModel();
            HwIssueCommentModel=new HwIssueCommentModel();
            HwInchargeIssueModels=new List<HwInchargeIssueModel>();
            HwInchargeIssueModel=new HwInchargeIssueModel();
        }

        public ProjectMasterModel ProjectMasterModel { get; set; }
        public HwQcAssignModel HwQcAssignModel { get; set; }
        public HwQcInchargeAssignModel HwQcInchargeAssignModel { get; set; }
        public HwAllTestModel HwAllTestModel { get; set; }
        public CmnUserModel CmnUserModel { get; set; }
        public HwQcTestCounterModel HwQcTestCounterModel { get; set; }

        public HwQcAssignCustomMasterModel HwQcAssignCustomMasterModel {get;set;}

        public List<HwGetQcAssignedByInchargeModel> HwGetQcAssignedByInchargeModel { get; set; }

        public HwIssueMasterModel HwIssueMasterModel { get; set; }

        public HwIssueTypeModel HwIssueTypeModel { get; set; }
        public HwIssueTypeDetailModel HwIssueTypeDetailModel { get; set; }

        public HwIssueCommentModel HwIssueCommentModel { get; set; }

        public List<HwInchargeIssueModel> HwInchargeIssueModels { get; set; }
        public HwInchargeIssueModel HwInchargeIssueModel { get; set; }

        public string HwQcAssignUserIds { get; set; }
    }
}