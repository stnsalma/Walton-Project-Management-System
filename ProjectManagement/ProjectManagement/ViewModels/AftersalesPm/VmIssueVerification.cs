using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.AftersalesPm
{
    public class VmIssueVerification
    {
        public VmIssueVerification()
        {
            ProjectMasterModels=new List<ProjectMasterModel>();
            ProjectMasterModel=new ProjectMasterModel();
            AftersalesPmIssueVerificationModels=new List<AftersalesPm_IssueVerificationModel>();
            AftersalesPmIssueVerificationModel=new AftersalesPm_IssueVerificationModel();
            AftersalesPmIssueVerificationStatusLogModel=new AftersalesPm_IssueVerificationStatusLogModel();
            AftersalesPmIssueVerificationStatusLogModels=new List<AftersalesPm_IssueVerificationStatusLogModel>();
        }
        public long ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public List<ProjectMasterModel> ProjectMasterModels=new List<ProjectMasterModel>();
        public ProjectMasterModel ProjectMasterModel = new ProjectMasterModel();
        public List<AftersalesPm_IssueVerificationModel> AftersalesPmIssueVerificationModels=new List<AftersalesPm_IssueVerificationModel>(); 
        public AftersalesPm_IssueVerificationModel AftersalesPmIssueVerificationModel=new AftersalesPm_IssueVerificationModel();
        public AftersalesPm_IssueVerificationStatusLogModel AftersalesPmIssueVerificationStatusLogModel=new AftersalesPm_IssueVerificationStatusLogModel();
        public List<AftersalesPm_IssueVerificationStatusLogModel> AftersalesPmIssueVerificationStatusLogModels = new List<AftersalesPm_IssueVerificationStatusLogModel>();
    }
}