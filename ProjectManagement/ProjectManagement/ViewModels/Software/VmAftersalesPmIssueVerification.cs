using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Software
{
    public class VmAftersalesPmIssueVerification
    {
        public VmAftersalesPmIssueVerification()
        {
          
            AftersalesPmIssueVerificationModels=new List<AftersalesPm_IssueVerificationModel>();
            AftersalesPmIssueVerificationModel=new AftersalesPm_IssueVerificationModel();
            AftersalesPmIssueVerificationStatusLogModel=new AftersalesPm_IssueVerificationStatusLogModel();
            AftersalesPmIssueVerificationStatusLogModels=new List<AftersalesPm_IssueVerificationStatusLogModel>();
            AftersalesPm_ValidationReportModels = new List<AftersalesPm_ValidationReportModel>();
            AftersalesPm_ValidationReportModel = new AftersalesPm_ValidationReportModel();
            AftersalesPm_SupplierFeedBackModel = new AftersalesPm_SupplierFeedBackModel();
            AftersalesPm_SupplierFeedBackModels = new List<AftersalesPm_SupplierFeedBackModel>();
        }
        public List<AftersalesPm_IssueVerificationModel> AftersalesPmIssueVerificationModels=new List<AftersalesPm_IssueVerificationModel>(); 
        public AftersalesPm_IssueVerificationModel AftersalesPmIssueVerificationModel=new AftersalesPm_IssueVerificationModel();
        public AftersalesPm_IssueVerificationStatusLogModel AftersalesPmIssueVerificationStatusLogModel=new AftersalesPm_IssueVerificationStatusLogModel();
        public List<AftersalesPm_IssueVerificationStatusLogModel> AftersalesPmIssueVerificationStatusLogModels = new List<AftersalesPm_IssueVerificationStatusLogModel>();
        public List<AftersalesPm_ValidationReportModel> AftersalesPm_ValidationReportModels { get; set; }
        public AftersalesPm_ValidationReportModel AftersalesPm_ValidationReportModel { get; set; }
        public List<AftersalesPm_SupplierFeedBackModel> AftersalesPm_SupplierFeedBackModels { get; set; }
        public AftersalesPm_SupplierFeedBackModel AftersalesPm_SupplierFeedBackModel { get; set; }
    }
}