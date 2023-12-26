using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;
using ProjectManagement.ViewModels.Software;

namespace ProjectManagement.Infrastructures.Helper
{
    public class ReportPortal
    {
        private readonly CellPhoneProjectEntities _dbEntities;

        public ReportPortal()
        {
            _dbEntities = new CellPhoneProjectEntities();
            _dbEntities.Configuration.LazyLoadingEnabled = false;
        }
        public IEnumerable GetPurchaseOrderForm(long id)
        {
            var form =
                _dbEntities.ProjectPurchaseOrderForms.Where(i => i.ProjectPurchaseOrderFormId == id)
                    .Select(orderForm => new ProjectPurchaseOrderFormModel
                    {
                        ProjectMasterId = orderForm.ProjectMasterId,
                        CompanyName = orderForm.CompanyName,
                        ProjectName =
                            _dbEntities.ProjectMasters.FirstOrDefault(
                                i => i.ProjectMasterId == orderForm.ProjectMasterId).ProjectName,
                        Added = orderForm.Added,
                        AddedDate = orderForm.AddedDate,
                        Color = orderForm.Color,
                        CompanyAddress = orderForm.CompanyAddress,
                        DescriptionBody = orderForm.DescriptionBody,
                        DescriptionHeader = orderForm.DescriptionHeader,
                        IsCompleted = orderForm.IsCompleted,
                        PoDate = orderForm.PoDate,
                        ProjectPurchaseOrderFormId = orderForm.ProjectPurchaseOrderFormId,
                        PurchaseOrderNumber = orderForm.PurchaseOrderNumber,
                        Quantity = orderForm.Quantity,
                        Receiver = orderForm.Receiver,
                        Signature = orderForm.Signature,
                        Subject = orderForm.Subject,
                        Updated = orderForm.Updated,
                        UpdatedDate = orderForm.UpdatedDate,
                        Value = orderForm.Value
                    }).ToList();
            return form;
        }

        public IEnumerable GetPurchaseOrderConditions(long id)
        {
            var conditions =
                _dbEntities.ProjectPurchaseOrderConditions.Where(i => i.ProjectPurchaseOrderFormId == id).ToList();
            return conditions;
        }

        public IEnumerable GetFieldTestProject(long id, long qcFieldId, long swQcInId)
        {
           
            var models = new List<SwFieldTestReportView>();

            var dbIssues1 = from ft in _dbEntities.SwFieldTests
                            join pm in _dbEntities.ProjectMasters on ft.ProjectMasterId equals pm.ProjectMasterId
                            join pma in _dbEntities.ProjectPmAssigns on pm.ProjectMasterId equals pma.ProjectMasterId
                            //join swa in _dbEntities.SwQcInchargeAssigns on ft.SwQcInchargeAssignId equals swa.SwQcInchargeAssignId
                            join cmn in _dbEntities.CmnUsers on pma.ProjectManagerUserId equals cmn.CmnUserId

                            where
                                pm.ProjectStatus == "APPROVED" && pm.ProjectMasterId == id && ft.SwQcInchargeAssignId == swQcInId
                                && ft.SwFieldTestId == qcFieldId
                            select new SwFieldTestReportView
                            {
                                ProjectMasterId = id,
                                CmnUserId = pma.ProjectManagerUserId,
                                SwFieldTestId = qcFieldId,
                                HwQcInchargeUserId = swQcInId,
                                // SwQcInchargeUserId = userId,
                                IssueOf = ft.IssueOf,
                                ComparedWith = ft.ComparedWith,
                                ProjectType = pm.ProjectType,
                                ProjectName = pm.ProjectName,
                                OsVersion = pm.OsVersion,
                                OsName = pm.OsName,
                                SupplierModelName = pm.SupplierModelName,
                                SupplierName = pm.SupplierName,
                                UserFullName = cmn.UserFullName,
                                UserName = cmn.UserName,
                                ProjectManagerUserId = cmn.CmnUserId,
                                FieldTestAssignCommentFromIncharge = ft.FieldTestAssignCommentFromIncharge,
                                AddedDate = ft.AddedDate

                            };

            models = dbIssues1.ToList();

            return models;
        }

        public IEnumerable GetSwFieldTestAssignsList(long id, long qcFieldId)
        {

            var models = new List<SwFieldTestReportView>();

            var dbIssues1 = from ft in _dbEntities.SwFieldTests
                            join sfa in _dbEntities.SwFieldTestAssigns on ft.SwFieldTestId equals sfa.SwFieldTestId

                            join cmn in _dbEntities.CmnUsers on sfa.SwQcUserId equals cmn.CmnUserId

                          
                                where ft.SwFieldTestId == qcFieldId && sfa.ProjectMasterId==id

                            select new SwFieldTestReportView
                            {
                                ProjectMasterId = id,
                                SwFieldTestId = qcFieldId,
                                UserFullName=cmn.UserFullName,
                                EmployeeCode = cmn.EmployeeCode,
                                SwQcInchargeAssignId=ft.SwQcInchargeAssignId,
                                AddedDate = ft.AddedDate

                            };

            models = dbIssues1.ToList();

            return models;
        }

        public IEnumerable GetSwFieldTestDetailsList(long id, long qcFieldId)
        {

            var models = new List<SwFieldTestReportView>();

            var dbIssues1 = from sfd in _dbEntities.SwFieldTestDetails
                            join ft in _dbEntities.SwFieldTests on sfd.SwFieldTestId equals ft.SwFieldTestId

                            where sfd.ProjectMasterId == id && ft.SwFieldTestId == qcFieldId
                         
                            select new SwFieldTestReportView
                            {
                                ProjectMasterId = id,
                                SwFieldTestDetailId=sfd.SwFieldTestDetailId,
                                SwFieldTestId = qcFieldId,
                                TestDate=sfd.TestDate,
                                Location=sfd.Location,
                                Severity=sfd.Severity,
                                Description = sfd.Description,
                                Condition_Op_TT_dbm = sfd.Condition_Op_TT_dbm,
                                Condition_Op_TT_Bar = sfd.Condition_Op_TT_Bar,
                                Condition_Op_RB_dbm = sfd.Condition_Op_RB_dbm,
                                Condition_Op_RB_Bar = sfd.Condition_Op_RB_Bar,
                                Condition_Op_BL_dbm = sfd.Condition_Op_BL_dbm,
                                Condition_Op_BL_Bar = sfd.Condition_Op_BL_Bar,
                                Condition_Op_AT_dbm = sfd.Condition_Op_AT_dbm,
                                Condition_Op_AT_Bar = sfd.Condition_Op_AT_Bar,
                                Ref_Op_TT_dbm = sfd.Ref_Op_TT_dbm,
                                Ref_Op_TT_Bar = sfd.Ref_Op_TT_Bar,
                                Ref_Op_RB_dbm = sfd.Ref_Op_RB_dbm,
                                Ref_Op_RB_Bar = sfd.Ref_Op_RB_Bar,
                                Ref_Op_BL_dbm = sfd.Ref_Op_BL_dbm,
                                Ref_Op_BL_Bar = sfd.Ref_Op_BL_Bar,
                                Ref_Op_AT_dbm = sfd.Ref_Op_AT_dbm,
                                Ref_Op_AT_Bar = sfd.Ref_Op_AT_Bar,
                                Remarks = sfd.Remarks
                            };

            models = dbIssues1.ToList();

            return models;
        }
    }
}