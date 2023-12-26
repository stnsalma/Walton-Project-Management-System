using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Oracle.ManagedDataAccess.Client;
using ProjectManagement.Controllers;
using ProjectManagement.DAL;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Models;
using ProjectManagement.Models.Common;
using ProjectManagement.Models.StausObjects;
using ProjectManagement.ViewModels.Common;
using ProjectManagement.ViewModels.Hardware;
using System.Data.SqlClient;
using System.Configuration;
using ProjectManagement.ViewModels.Management;
using Remotion.Data.Linq.Clauses.ResultOperators;


namespace ProjectManagement.Infrastructures.Repositories
{
    public class CommonRepository : ICommonRepository
    {
        private readonly CellPhoneProjectEntities _dbEntities;
        private readonly RBSYNERGYEntities _dbrbsyEntities;
        private readonly MRPEntities _mrpEntities;
        private readonly IManagementRepository _managementRepository;

        public CommonRepository()
        {
            _dbEntities = new CellPhoneProjectEntities();
            _dbEntities.Configuration.LazyLoadingEnabled = false;
            _dbrbsyEntities = new RBSYNERGYEntities();
            _mrpEntities = new MRPEntities();
            _managementRepository = new ManagementRepository();
            _dbrbsyEntities.Configuration.LazyLoadingEnabled = false;
        }

        public List<ProjectMasterModel> GetAllProjects()
        {
            var allProjects = _dbEntities.ProjectMasters.Where(i => i.IsActive && i.ProjectStatus != "DISCARDED" && i.ProjectStatus != "REJECTED").Select(i => new ProjectMasterModel
            {
                ProjectMasterId = i.ProjectMasterId,
                ProjectName = i.ProjectName,
                SupplierName = i.SupplierName,
                SupplierModelName = i.SupplierModelName,
                ProjectTypeId = i.ProjectTypeId,
                SourcingType = i.SourcingType,
                NumberOfSample = i.NumberOfSample,
                ApproxProjectFinishDate = i.ApproxProjectFinishDate,
                SupplierTrustLevel = i.SupplierTrustLevel,
                IsScreenTestComplete = i.IsScreenTestComplete,
                IsApproved = i.IsApproved,
                ApproxProjectOrderDate = i.ApproxProjectOrderDate,
                ApproxShipmentDate = i.ApproxShipmentDate,
                ProjectStatus = i.ProjectStatus,
                OrderNuber = i.OrderNuber,
                ApproximatePrice = i.ApproximatePrice,
                InitialApprovalDate = i.InitialApprovalDate,
                SlotType = i.SlotType,
                SimSlotNumber = i.SimSlotNumber,
                CpuName = i.CpuName??i.ProcessorName,
                ChipsetFrequency = i.ChipsetFrequency ?? SqlFunctions.StringConvert(i.ProcessorClock),
                ChipsetName = i.ChipsetName??i.Chipset,
                ChipsetCore = i.ChipsetCore,
                BackCam = i.BackCam,
                FrontCam = i.FrontCam,
                Ram = i.Ram,
                Rom = i.Rom,
                DisplaySize = i.DisplaySize,
                DisplayResulution = i.DisplayResulution,
                DisplaySpeciality = i.DisplaySpeciality,
                Battery = i.Battery,
                BatteryRating = i.BatteryRating,
                FinalPrice = i.FinalPrice??i.ApproximatePrice,
                OsName = i.OsName,
                OsVersion = i.OsVersion,
                AddedDate = i.AddedDate
            }).OrderBy(i => i.ProjectName).ThenBy(i => i.ProjectMasterId).ToList();
            foreach (var project in allProjects)
            {
                project.OrderNumberOrdinal = project.OrderNuber != null
                    ? CommonConversion.AddOrdinal((int)project.OrderNuber) + " Order"
                    : string.Empty;
                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
                {
                    project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
                }
                //total price
                var accessories =
                    _dbEntities.AccessoriesPrices.Where(x => x.ProjectMasterId == project.ProjectMasterId).ToList();
                project.TotalPrice = project.FinalPrice??0;
                foreach (var acc in accessories)
                {
                    project.TotalPrice = project.TotalPrice + Convert.ToDecimal(acc.TotalPrice);
                }
                //market clearance date
                project.MarketClearanceDate =
                    _dbEntities.ProjectPurchaseOrderForms.Where(x => x.ProjectMasterId == project.ProjectMasterId)
                        .Select(x => x.MarketClearanceDate).FirstOrDefault();
            }
            return allProjects;
        }

        public List<ProjectMasterModel> GetAllProjectNames()
        {
            string query = string.Format(@"select projectname from ProjectMasters group by ProjectName order by ProjectName");
            var allProjectNames = _dbEntities.Database.SqlQuery<ProjectMasterModel>(query).ToList();
            return allProjectNames;
        }

        public List<ProjectMasterModel> GetAllProjectModels()
        {
            string query = string.Format(@"select ProjectModel from ProjectMasters group by ProjectModel order by ProjectModel");
            var allProjectNames = _dbEntities.Database.SqlQuery<ProjectMasterModel>(query).ToList();
            return allProjectNames;
        }

        public List<ProjectMasterModel> GetOrderNumbersByProjectName(string projectname)
        {
            string query = string.Format(@"select projectmasterid,OrderNuber from ProjectMasters  where ProjectName = '{0}'", projectname);
            var execute = _dbEntities.Database.SqlQuery<ProjectMasterModel>(query).ToList();
            return execute;
        }

        public List<ProjectMasterModel> GetOrderNumbersByProjectModel(string projectModel)
        {
            string query = string.Format(@"select projectmasterid,OrderNuber from ProjectMasters  where IsActive = 1 and ProjectModel = '{0}'", projectModel);
            var execute = _dbEntities.Database.SqlQuery<ProjectMasterModel>(query).ToList();
            return execute;
        }

        public ProjectMasterModel GetProjectInfoByProjectId(long projectId)
        {
            var query = string.Format("select * from ProjectMasters where ProjectMasterId={0}", projectId);
            var model = _dbEntities.Database.SqlQuery<ProjectMasterModel>(query).FirstOrDefault();
            return model;
        }

        public List<AccessoriesPricesModel> GetAccessoriesPricesByProjectId(long projectId)
        {
            var query = string.Format("select * from AccessoriesPrices where ProjectMasterId={0}", projectId);
            var model = _dbEntities.Database.SqlQuery<AccessoriesPricesModel>(query).ToList();
            return model;
        }

        public List<SupplierKpiPerformanceModel> GetSupplierKpiPerformanceByProjectId(long? projectId)
        {
            var variantName =
                _dbEntities.ProjectOrderQuantityDetails.Where(x => x.ProjectMasterId == projectId)
                    .Select(x => x.ProjectModel)
                    .FirstOrDefault();
            var supplierName =
                _dbrbsyEntities.tblSupplierModelInfoes.Where(x => x.Model == variantName)
                    .Select(x => x.supplierName)
                    .FirstOrDefault();
            var query = string.Format(@"select sm.Model,si.SupplierName,Count(*) TotalReceive,(Select sum(singleIme) 
  from (select Model, 1 singleIme
from WSMS.dbo.ServiceMaster s
where s.Model is not null and s.Model in(
   select ProductModel from RBSYNERGY.dbo.tblProductMaster where DateAdded>=DATEADD(YEAR,-3,GETDATE()))
Group by Model,IME) X
where X.Model=sm.Model) TotalUniqueServiceReceive,  
   (select COUNT(*) 
   from RBSYNERGY.dbo.tblProductMaster pm 
   inner join 
   RBSYNERGY.dbo.tblProductRegistration pr
   on pm.ProductId=pr.ProductModelID
   where pm.ProductModel=sm.Model 
   group by pm.ProductModel) TotalActivation
   from WSMS.dbo.ServiceMaster sm
   inner join [RBSYNERGY].[dbo].[tblSupplierModelInfo] si
   on sm.Model=si.Model
   where si.supplierName ='{0}' and si.Model is not null and Si.Model in(
   select ProductModel from RBSYNERGY.dbo.tblProductMaster where DateAdded>=DATEADD(YEAR,-3,GETDATE())
   )
   Group by sm.Model,si.supplierName", supplierName);
            var supKpi = _dbrbsyEntities.Database.SqlQuery<SupplierKpiPerformanceModel>(query).ToList();
            return supKpi;
        }

        public List<ProjectImage> GetProjectImages(long? projectId)
        {
            var v = _dbEntities.ProjectImages.Where(x => x.ProjectId == projectId).ToList();
            foreach (var img in v)
            {
                var manager = new FileManager();
                img.ImagePath = manager.GetFile(img.ImagePath);
            }
            return v;
        } 

        public string GetJigsPriceByProjectName(string projectName)
        {
            var v = (from po in _dbEntities.ProjectPurchaseOrderForms
                     join pm in _dbEntities.ProjectMasters on po.ProjectMasterId equals pm.ProjectMasterId
                     where pm.ProjectName == projectName && pm.OrderNuber == 1
                     select po.JigsUnitPrice).FirstOrDefault();
            return v;
        }

        public ProjectPurchaseOrderFormModel GetProjectPurchaseOrderByProjectId(long projectId)
        {
            var query = string.Format("select * from ProjectPurchaseOrderForms where ProjectMasterId={0}", projectId);
            var model = _dbEntities.Database.SqlQuery<ProjectPurchaseOrderFormModel>(query).FirstOrDefault();
            return model;
        }

        public string GetProjectAge(string projectName)
        {
            try
            {
                var get1Storder =
                _dbEntities.ProjectMasters.FirstOrDefault(x => x.ProjectName == projectName && x.OrderNuber == 1);
                if (get1Storder == null)
                {
                    var arrProName = projectName.Split(' ');
                    var firstTwoPart = arrProName[0] + " " + arrProName[1];
                    get1Storder = _dbEntities.ProjectMasters.FirstOrDefault(x => x.ProjectName == firstTwoPart && x.OrderNuber == 1);
                }
                var marketClearanceDate =
                    _dbEntities.ProjectPurchaseOrderForms.Where(x => x.ProjectMasterId == get1Storder.ProjectMasterId)
                        .Select(x => x.MarketClearanceDate)
                        .FirstOrDefault();
                if (marketClearanceDate != null)
                {
                    var projectAge = (DateTime.Now - Convert.ToDateTime(marketClearanceDate)).TotalDays;
                    projectAge = Math.Round(projectAge);
                    return Convert.ToString(projectAge + " Days");
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "Market clearance date of 1st order not found";
        }

        public LcApprovalHandsetPriceRangeModel GetPriceRangeByFinalPrice(decimal? finalPrice, string rangeFor)
        {
            var v =
                _dbEntities.LcApprovalHandsetPriceRanges.Where(
                    x => x.StartingRange <= finalPrice && x.FinishingRange > finalPrice && x.RangeFor == rangeFor)
                    .Select(x => new LcApprovalHandsetPriceRangeModel
                    {
                        Id = x.Id,
                        StartingRange = x.StartingRange,
                        FinishingRange = x.FinishingRange,
                        AddedBy = x.AddedBy,
                        AddedDate = x.AddedDate,
                        UpdatedBy = x.UpdatedBy,
                        UpdatedDate = x.UpdatedDate
                    }).FirstOrDefault();
            return v;
        }

        public List<SalesForecastingReport> GetSalesForecastForRelevantModelByPriceRange(decimal startingRange,
            decimal finishingRange, string projectType)
        {
            var query = string.Format(@"select distinct f.* from RBSYNERGY.dbo.SalesForecastingReport f
  inner join CellPhoneProject.dbo.ProjectOrderQuantityDetails d on f.Model=d.ProjectModel
  inner join CellPhoneProject.dbo.ProjectMasters pm on d.ProjectMasterId=pm.ProjectMasterId
  where pm.FinalPrice>={0} and pm.FinalPrice<={1} and pm.ProjectType='{2}' and convert(date,f.AddedDate)=convert(date,(GETDATE()-1))", startingRange, finishingRange, projectType);
            var v = _dbrbsyEntities.Database.SqlQuery<SalesForecastingReport>(query).ToList();
            var forecast = new List<SalesForecastingReport>();
            foreach (var i in v)
            {
                var variant =
                    _dbEntities.ProjectOrderQuantityDetails.Where(x => x.ProjectModel == i.Model).FirstOrDefault();
                if (variant != null && variant.AddedDate > DateTime.Now.AddYears(-2))
                {
                    forecast.Add(i);
                }
            }
            return forecast;
        }

        public MarketPriceModel GetMarketPriceModelByProjectId(long projectId)
        {
            var marketPrice =
                _dbEntities.MarketPrices.Where(i => i.ProjectMasterId == projectId).Select(i => new MarketPriceModel
                {
                    MarketPriceId = i.MarketPriceId,
                    ProjectMasterId = i.ProjectMasterId,
                    FinalPrice = i.FinalPrice,
                    Multiplier = i.Multiplier,
                    Mrp = i.Mrp,
                    IsLocked = i.IsLocked,
                    AddedDate = i.AddedDate
                }).OrderByDescending(i => i.AddedDate).ToList();
            foreach (var marketPriceModel in marketPrice)
            {
                if (marketPriceModel != null)
                {
                    if (marketPriceModel.IsLocked == true)
                    {
                        return marketPriceModel;
                    }
                }
            }
            return marketPrice.FirstOrDefault();
        }

        public List<CommonIssueModel> GetIssues()
        {
            var commonIssueModels = new List<CommonIssueModel>();
            var role = HttpContext.Current.User;
            if (role.IsInRole("PMHEAD"))
            {
                commonIssueModels = GetCommonIssueModels("PMHEAD");

            }
            else if (role.IsInRole("PM"))
            {
                commonIssueModels = GetCommonIssueModels("PM");
            }
            else if (role.IsInRole("QCHEAD"))
            {
                commonIssueModels = GetCommonIssueModels("QCHEAD");
            }
            else if (role.IsInRole("QC"))
            {
                commonIssueModels = GetCommonIssueModels("QC");
            }
            else if (role.IsInRole("CM"))
            {
                commonIssueModels = GetCommonIssueModels("CM");
            }
            else if (role.IsInRole("CMBTRC"))
            {
                commonIssueModels = GetCommonIssueModels("CMBTRC");
            }
            else if (role.IsInRole("MM"))
            {
                commonIssueModels = GetCommonIssueModels("MM");
            }
            else if (role.IsInRole("HWHEAD"))
            {
                commonIssueModels = GetCommonIssueModels("HWHEAD");
            }
            else if (role.IsInRole("HW"))
            {
                commonIssueModels = GetCommonIssueModels("HW");
            }
            else if (role.IsInRole("BRND"))
            {
                commonIssueModels = GetCommonIssueModels("BRND");
            }
            else if (role.IsInRole("MKTHEAD"))
            {
                commonIssueModels = GetCommonIssueModels("MKTHEAD");
            }
            else if (role.IsInRole("SPRHEAD"))
            {
                commonIssueModels = GetCommonIssueModels("SPRHEAD");
            }
            else if (role.IsInRole("SPR"))
            {
                commonIssueModels = GetCommonIssueModels("SPR");
            }
            else if (role.IsInRole("MKT"))
            {
                commonIssueModels = GetCommonIssueModels("MKT");
            }
            else if (role.IsInRole("SA"))
            {
                commonIssueModels = GetCommonIssueModels("SA");
            }


            return commonIssueModels;
        }

        private List<CommonIssueModel> GetCommonIssueModels(string role)
        {
            switch (role)
            {
                case "HWHEAD":
                    role = "HW";
                    break;
                case "QCHEAD":
                    role = "SW";
                    break;
                case "QC":
                    role = "SW";
                    break;
                case "PMHEAD":
                    role = "PM";
                    break;
            }
            List<CommonIssueModel> commonIssueModels;
            if (role == "SA")
            {
                commonIssueModels = (from commonIssue in _dbEntities.CommonIssues
                                     join cmnUser in _dbEntities.CmnUsers on commonIssue.CreatorUserId equals cmnUser.CmnUserId
                                     join projectMaster in _dbEntities.ProjectMasters on commonIssue.ProjectMasterId equals
                                         projectMaster.ProjectMasterId
                                     where !commonIssue.IsSolved
                                     select new CommonIssueModel
                                     {
                                         CommonIssueId = commonIssue.CommonIssueId,
                                         ProjectMasterId = commonIssue.ProjectMasterId,
                                         ProjectName = projectMaster.ProjectName,
                                         IssueTitle = commonIssue.IssueTitle,
                                         Component = commonIssue.Component,
                                         Description = commonIssue.Description,
                                         CreatorUserId = commonIssue.CreatorUserId,
                                         CreatorUserRole = commonIssue.CreatorUserRole,
                                         CreatorName = cmnUser.UserFullName,
                                         ReferenceRemarks = commonIssue.ReferenceRemarks,
                                         ReferenceFlow = commonIssue.ReferenceFlow
                                     }).ToList();
            }
            else
            {
                commonIssueModels = (from commonIssue in _dbEntities.CommonIssues
                                     join cmnUser in _dbEntities.CmnUsers on commonIssue.CreatorUserId equals cmnUser.CmnUserId
                                     join projectMaster in _dbEntities.ProjectMasters on commonIssue.ProjectMasterId equals projectMaster.ProjectMasterId
                                     where commonIssue.CurrentlyWorkingRole == role && !commonIssue.IsSolved
                                     select new CommonIssueModel
                                     {
                                         CommonIssueId = commonIssue.CommonIssueId,
                                         ProjectMasterId = commonIssue.ProjectMasterId,
                                         ProjectName = projectMaster.ProjectName,
                                         IssueTitle = commonIssue.IssueTitle,
                                         Component = commonIssue.Component,
                                         Description = commonIssue.Description,
                                         CreatorUserId = commonIssue.CreatorUserId,
                                         CreatorUserRole = commonIssue.CreatorUserRole,
                                         CreatorName = cmnUser.UserFullName,
                                         ReferenceRemarks = commonIssue.ReferenceRemarks,
                                         ReferenceFlow = commonIssue.ReferenceFlow,
                                         OrderNuber = projectMaster.OrderNuber
                                     }).ToList();
            }
            foreach (var model in commonIssueModels)
            {
                model.ProjectName = model.ProjectName + " (" + CommonConversion.AddOrdinal(model.OrderNuber) + " Order)";

                string[] comments = model.ReferenceRemarks != null ? model.ReferenceRemarks.Split('>') : new string[0];
                string[] flows = model.ReferenceFlow != null ? model.ReferenceFlow.Split('>') : new string[0];
                foreach (var comment in comments)
                {
                    model.FormatedReferenceRemark = model.FormatedReferenceRemark + comment + "\n";
                }
                foreach (var flow in flows)
                {
                    var fullRole = GetRoleFullName(flow);
                    if (!string.IsNullOrWhiteSpace(fullRole))
                        model.FormatedReferenceFlow = model.FormatedReferenceFlow + fullRole + ">>";
                }
                model.FormatedReferenceFlow = model.FormatedReferenceFlow != null ? model.FormatedReferenceFlow.Remove(model.FormatedReferenceFlow.Length - 2) : string.Empty;
                model.RoleFullName = GetRoleFullName(model.CreatorUserRole);
            }



            return commonIssueModels;
        }

        public List<CommonIssueModel> GetIssuesCreatedByUserId(long userid)
        {
            var commonIssueModels = (from commonIssue in _dbEntities.CommonIssues
                                     join cmnUser in _dbEntities.CmnUsers on commonIssue.CreatorUserId equals cmnUser.CmnUserId
                                     join projectMaster in _dbEntities.ProjectMasters on commonIssue.ProjectMasterId equals projectMaster.ProjectMasterId
                                     where !commonIssue.IsSolved && commonIssue.CreatorUserId == userid
                                     select new CommonIssueModel
                                     {
                                         CommonIssueId = commonIssue.CommonIssueId,
                                         ProjectMasterId = commonIssue.ProjectMasterId,
                                         ProjectName = projectMaster.ProjectName,
                                         IssueTitle = commonIssue.IssueTitle,
                                         Component = commonIssue.Component,
                                         Description = commonIssue.Description,
                                         CreatorUserId = commonIssue.CreatorUserId,
                                         CreatorUserRole = commonIssue.CreatorUserRole,
                                         CreatorName = cmnUser.UserFullName,
                                         ReferenceRemarks = commonIssue.ReferenceRemarks,
                                         ReferenceFlow = commonIssue.ReferenceFlow,
                                         OrderNuber = projectMaster.OrderNuber,
                                         CurrentlyWorkingRole = commonIssue.CurrentlyWorkingRole
                                     }).ToList();
            foreach (var model in commonIssueModels)
            {
                model.ProjectName = model.ProjectName + " (" + CommonConversion.AddOrdinal(model.OrderNuber) + " Order)";

                string[] comments = model.ReferenceRemarks != null ? model.ReferenceRemarks.Split('>') : new string[0];
                string[] flows = model.ReferenceFlow != null ? model.ReferenceFlow.Split('>') : new string[0];
                foreach (var comment in comments)
                {
                    model.FormatedReferenceRemark = model.FormatedReferenceRemark + comment + "\n";
                }
                foreach (var flow in flows)
                {
                    var fullRole = GetRoleFullName(flow);
                    if (!string.IsNullOrWhiteSpace(fullRole))
                        model.FormatedReferenceFlow = model.FormatedReferenceFlow + fullRole + ">>";
                }
                model.FormatedReferenceFlow = model.FormatedReferenceFlow != null ? model.FormatedReferenceFlow.Remove(model.FormatedReferenceFlow.Length - 2) : string.Empty;
                model.RoleFullName = GetRoleFullName(model.CurrentlyWorkingRole);
            }
            return commonIssueModels;
        }

        private string GetRoleFullName(string creatorUserRole)
        {
            creatorUserRole = creatorUserRole.Replace(" ", string.Empty);
            if (creatorUserRole == "PMHEAD") return "Project Manager Incharge";
            if (creatorUserRole == "PM") return "Project Manager";
            if (creatorUserRole == "QCHEAD") return "Quality Control Incharge";
            if (creatorUserRole == "SW") return "Quality Control";
            if (creatorUserRole == "CM") return "Commercial";
            if (creatorUserRole == "CMBTRC") return "Commercial (BTRC)";
            if (creatorUserRole == "MM") return "Management";
            if (creatorUserRole == "HWHEAD") return "Hardware R&D Incharge";
            if (creatorUserRole == "HW") return "Hardware R&D";
            if (creatorUserRole == "BRND") return "Branding";
            if (creatorUserRole == "SPRHEAD") return "Spare Commercial Incharge";
            if (creatorUserRole == "SPR") return "Spare Commercial";
            if (creatorUserRole == "MKTHEAD") return "Marketing Incharge";
            if (creatorUserRole == "MKT") return "Marketing";
            return null;
        }

        public long SaveIssue(CommonIssueModel model)
        {
            try
            {
                CmnUser cmnUser = GenereticRepo<CmnUser>.GetById(_dbEntities, model.CreatorUserId);
                if (cmnUser == null || cmnUser.RoleName == null) return 0;
                model.CreatorUserRole = cmnUser.RoleName;
                var detailRole = string.Empty;
                var notificationRole = string.Empty;

                CommonIssue issue = GenericMapper<CommonIssueModel, CommonIssue>.GetDestination(model);
                issue.Added = model.CreatorUserId;
                issue.AddedDate = DateTime.Now;
                switch (issue.CurrentlyWorkingRole)
                {
                    case "MM":
                        detailRole = "Management";
                        notificationRole = "MM";
                        break;
                    case "CM":
                        detailRole = "Commercial";
                        notificationRole = "CM";
                        break;
                    case "SW":
                        detailRole = "Software";
                        notificationRole = "QC";
                        break;
                    case "HW":
                        detailRole = "Hardware";
                        notificationRole = "HW";
                        break;
                    case "PM":
                        detailRole = "Project Manager";
                        notificationRole = "PM";
                        break;
                }

                var viewers = _dbEntities.CmnUsers.Where(i => i.RoleName.Contains(notificationRole) && i.IsActive).ToList();
                if (!viewers.Any()) throw new Exception();
                _dbEntities.CommonIssues.Add(issue);
                var projectMaster = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == issue.ProjectMasterId);
                if (projectMaster == null) throw new Exception();
                var projectName = projectMaster.ProjectName;
                foreach (var viewer in viewers)
                {
                    var notification = new Notification
                    {
                        ProjectMasterId = model.ProjectMasterId,
                        ViewerId = (int?)viewer.CmnUserId,
                        Message =
                            cmnUser.UserFullName + " Created a new Issue about " + projectName + " for " +
                            detailRole,
                        Role = cmnUser.RoleName,
                        Added = DateTime.Now,
                        AddedBy = cmnUser.CmnUserId,
                        IsViewd = false,
                    };
                    _dbEntities.Notifications.Add(notification);
                }
                _dbEntities.SaveChanges();
                string roleDetail = string.Empty;
                var toIdList = new List<string>();
                var usrInfo = "<br/>Issue Created By: " + cmnUser.UserFullName;
                string time = "<br/>Created On: " + DateTime.Now.ToLongDateString();
                switch (issue.CurrentlyWorkingRole)
                {
                    case "MM":
                        roleDetail = "Management";
                        toIdList.Add("MM");
                        break;
                    case "PM":
                        roleDetail = "Project Manager";
                        toIdList.Add("PMHEAD");
                        toIdList.Add("PM");
                        break;
                    case "CM":
                        roleDetail = "Commercial section";
                        toIdList.Add("CM");
                        toIdList.Add("CMBTRC");
                        break;
                    case "HW":
                        roleDetail = "Hardware team";
                        toIdList.Add("HWHEAD");
                        toIdList.Add("HW");
                        break;
                    case "SW":
                        roleDetail = "Sofware team";
                        toIdList.Add("QCHEAD");
                        toIdList.Add("QC");
                        break;
                }
                var body =
                    string.Format(
                        @"This is to inform you that, A new issue has been created in Walton Project Management System for " + roleDetail + "<br/>" + roleDetail + " can check this by accessing Common >> Issues from navigation bar in WPMS.<br/>This issue can be seen by only the team who is responsible for.<br/><br/><b>Project Name: " +
                        projectName + "</b>" + usrInfo + time);
                var mail = new MailSendFromPms();
                var result = mail.SendMail(toIdList,
                    new List<string>(new[] { "MM", "PS" }), "New Issue ( " + projectName + " )", body);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }

        }



        public bool SolveIssue(CommonIssueModel model)
        {

            CommonIssue issue = GenereticRepo<CommonIssue>.GetById(_dbEntities, model.CommonIssueId);
            issue.IsSolved = true;
            issue.Updated = model.Updated;
            issue.UpdatedDate = DateTime.Now;
            issue.SolverUserId = model.SolverUserId;
            issue.SolutionComment = model.SolutionComment;
            try
            {

                GenereticRepo<CommonIssue>.Update(_dbEntities, issue);
                CmnUser cmnUser = GenereticRepo<CmnUser>.GetById(_dbEntities, model.SolverUserId != null ? (long)model.SolverUserId : 0);
                ProjectMaster master = GenereticRepo<ProjectMaster>.GetById(_dbEntities, issue.ProjectMasterId);
                var toIdList = new List<string>();
                var usrInfo = "<br/>Issue Created By: " + (cmnUser != null ? cmnUser.UserFullName : "Undefined");
                string time = "<br/>Created On: " + DateTime.Now.ToLongDateString();
                string solution = "<br/><b>Solution Comment:</b> " + model.SolutionComment;
                switch (issue.CurrentlyWorkingRole)
                {
                    case "MM":
                        toIdList.Add("MM");
                        break;
                    case "PM":
                        toIdList.Add("PMHEAD");
                        toIdList.Add("PM");
                        break;
                    case "CM":
                        toIdList.Add("CM");
                        toIdList.Add("CMBTRC");
                        break;
                    case "HW":
                        toIdList.Add("HWHEAD");
                        toIdList.Add("HW");
                        break;
                    case "SW":
                        toIdList.Add("QCHEAD");
                        toIdList.Add("QC");
                        break;
                }
                var body =
                    string.Format(
                        @"This is to inform you that, An issue has been solved in WPMS.<br/><br/><b>Project Name: " +
                        master.ProjectName + "</b>" + usrInfo + time + solution);
                var mail = new MailSendFromPms();
                var result = mail.SendMail(toIdList,
                    new List<string>(new[] { "MM", "PS" }), "Solved Issue ( " + master.ProjectName + " )", body);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ReferedIssue(CommonIssueModel model)
        {
            long userId;
            long.TryParse(HttpContext.Current.User.Identity.Name, out userId);
            CmnUser cmnUser = GenereticRepo<CmnUser>.GetById(_dbEntities, userId);
            CommonIssue issue = GenereticRepo<CommonIssue>.GetById(_dbEntities, model.CommonIssueId);
            ProjectMaster projectMaster = GenereticRepo<ProjectMaster>.GetById(_dbEntities, issue.ProjectMasterId);
            issue.ReferenceRemarks += issue.CurrentlyWorkingRole + ": " + model.ReferenceRemarks + ">";
            issue.ReferenceFlow = issue.ReferenceFlow == null ? issue.CurrentlyWorkingRole + ">" + model.CurrentlyWorkingRole : issue.ReferenceFlow + ">" + model.CurrentlyWorkingRole;
            issue.CurrentlyWorkingRole = model.CurrentlyWorkingRole;
            issue.NoOfTimesRefered = issue.NoOfTimesRefered == null ? 1 : issue.NoOfTimesRefered + 1;
            issue.Updated = userId;
            issue.UpdatedDate = DateTime.Now;
            try
            {
                GenereticRepo<CommonIssue>.Update(_dbEntities, issue);
                string roleDetail = string.Empty;
                var toIdList = new List<string>();
                var usrInfo = "<br/>Issue Forwarded By: " + cmnUser.UserFullName;
                string time = "<br/>Forwarded On: " + DateTime.Now.ToLongDateString();
                switch (issue.CurrentlyWorkingRole)
                {
                    case "MM":
                        roleDetail = "Management";
                        toIdList.Add("MM");
                        break;
                    case "PM":
                        roleDetail = "Project Manager";
                        toIdList.Add("PMHEAD");
                        toIdList.Add("PM");
                        break;
                    case "CM":
                        roleDetail = "Commercial section";
                        toIdList.Add("CM");
                        toIdList.Add("CMBTRC");
                        break;
                    case "HW":
                        roleDetail = "Hardware team";
                        toIdList.Add("HWHEAD");
                        toIdList.Add("HW");
                        break;
                    case "SW":
                        roleDetail = "Sofware team";
                        toIdList.Add("QCHEAD");
                        toIdList.Add("QC");
                        break;
                }
                var body =
                    string.Format(
                        @"This is to inform you that, A new issue has been forwarded in Walton Project Management System for " + roleDetail + "<br/>" + roleDetail + " can check this by accessing Common >> Issues from navigation bar in WPMS.<br/>This issue can be seen by only the team who is responsible for.<br/><br/><b>Project Name: " +
                        projectMaster.ProjectName + "</b>" + usrInfo + time);
                var mail = new MailSendFromPms();
                var result = mail.SendMail(toIdList,
                    new List<string>(new[] { "MM", "PS" }), "Issue Forwarded ( " + projectMaster.ProjectName + " )", body);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IgnoreIssue(CommonIssueModel model)
        {

            try
            {

                long userId;
                long.TryParse(HttpContext.Current.User.Identity.Name, out userId);
                CommonIssue issue = GenereticRepo<CommonIssue>.GetById(_dbEntities, model.CommonIssueId);
                issue.IgnoreComment = model.IgnoreComment;
                issue.IsIgnored = true;
                issue.IgnoredBy = userId;
                issue.Updated = userId;
                issue.UpdatedDate = DateTime.Now;
                GenereticRepo<CommonIssue>.Update(_dbEntities, issue);


                CmnUser cmnUser = GenereticRepo<CmnUser>.GetById(_dbEntities, userId);
                ProjectMaster master = GenereticRepo<ProjectMaster>.GetById(_dbEntities, issue.ProjectMasterId);
                var toIdList = new List<string>();
                var usrInfo = "<br/>Issue Denied By: " + (cmnUser != null ? cmnUser.UserFullName : "Undefined");
                string time = "<br/>Denied On: " + DateTime.Now.ToLongDateString();
                string ignoreComment = "<br/><b>Deny Comment:</b> " + model.IgnoreComment;
                switch (issue.CurrentlyWorkingRole)
                {
                    case "MM":
                        toIdList.Add("MM");
                        break;
                    case "PM":
                        toIdList.Add("PMHEAD");
                        toIdList.Add("PM");
                        break;
                    case "CM":
                        toIdList.Add("CM");
                        toIdList.Add("CMBTRC");
                        break;
                    case "HW":
                        toIdList.Add("HWHEAD");
                        toIdList.Add("HW");
                        break;
                    case "SW":
                        toIdList.Add("QCHEAD");
                        toIdList.Add("QC");
                        break;
                }
                var body =
                    string.Format(
                        @"This is to inform you that, An issue has been denied in WPMS.<br/><br/><b>Project Name: " +
                        master.ProjectName + "</b>" + usrInfo + time + ignoreComment);
                var mail = new MailSendFromPms();
                var result = mail.SendMail(toIdList,
                    new List<string>(new[] { "MM", "PS" }), "Issue Denied ( " + master.ProjectName + " )", body);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<HardwareIssueCustomModel> GetHardwareIssueModels()
        {
            var vmData = (from hwIssueComment in _dbEntities.HwIssueComments
                          join hwQcAssign in _dbEntities.HwQcAssigns on hwIssueComment.HwQcAssignId equals hwQcAssign.HwQcAssignId

                          join hwQcInchargeAssign in _dbEntities.HwQcInchargeAssigns on hwQcAssign.HwQcInchargeAssignId equals
                              hwQcInchargeAssign.HwQcInchargeAssignId

                          join projectMaster in _dbEntities.ProjectMasters on hwQcInchargeAssign.ProjectMasterId equals
                              projectMaster.ProjectMasterId

                          select new HardwareIssueCustomModel
                          {
                              ProjectMasterId = projectMaster.ProjectMasterId,
                              ProjectName = projectMaster.ProjectName,
                              CommercialComment = hwIssueComment.CommercialComment,
                              HwIssueCommentId = hwIssueComment.HwIssueCommentId,
                              HwQcUserid = hwQcAssign.HwQcUserId,
                              IssueRaiseName =
                                  _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == hwQcAssign.HwQcUserId).UserFullName,
                              IssueComment = hwIssueComment.IssueComment,
                              IssueName = hwIssueComment.IssueName,
                              IssueTypeDetailName = hwIssueComment.IssueTypeDetailName,
                              IssueTypeName = hwIssueComment.IssueTypeName,
                              ProjectType = projectMaster.ProjectType,
                              VerifierComment = hwIssueComment.VerifierComment,
                              //VerifiedName = _dbEntities.CmnUsers.FirstOrDefault(i=>i.CmnUserId == hwIssueComment.)

                          }).ToList();


            return vmData;
        }

        public List<CommonParamModel> GetComponents()
        {
            return new List<CommonParamModel>();
        }

        public SupplierRatingModel GetSupplierRating(long supplierId, long projectMasterId)
        {
            SupplierRatingModel model = (from rating in _dbEntities.SupplierRatings
                                         where rating.ProjectMasterId == projectMasterId && rating.SupplierId == supplierId
                                         select new SupplierRatingModel
                                         {
                                             ProjectMasterId = rating.ProjectMasterId,
                                             Added = rating.Added,
                                             AddedDate = rating.AddedDate,
                                             AfterSalesReturn = rating.AfterSalesReturn,
                                             AfterSalesSupport = rating.AfterSalesSupport,
                                             CustomizationSupport = rating.CustomizationSupport,
                                             Remarks = rating.Remarks,
                                             ShipmentDeliveryPerformance = rating.ShipmentDeliveryPerformance,
                                             SupplierId = rating.SupplierId,
                                             SupplierRatingId = rating.SupplierRatingId,
                                             Updated = rating.Updated,
                                             UpdatedDate = rating.UpdatedDate
                                         }).FirstOrDefault();
            return model;
        }

        public bool SaveSupplierRating(SupplierRatingModel model)
        {
            try
            {
                long userId;
                long.TryParse(HttpContext.Current.User.Identity.Name, out userId);
                model.Added = userId;
                model.AddedDate = DateTime.Now;
                SupplierRating supplier = GenericMapper<SupplierRatingModel, SupplierRating>.GetDestination(model);
                _dbEntities.SupplierRatings.Add(supplier);
                _dbEntities.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SaveOpinion(long partialPostProjectId, string opinionText)
        {
            try
            {
                long userId;
                long.TryParse(HttpContext.Current.User.Identity.Name, out userId);
                if (partialPostProjectId <= 0)
                {
                    throw new Exception();
                }
                var opinion = new Opinion
                {
                    ProjectMasterId = partialPostProjectId,
                    OpinionText = opinionText,
                    AddedBy = userId,
                    AddedDate = DateTime.Now
                };
                _dbEntities.Opinions.Add(opinion);
                _dbEntities.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<OpinionModel> GetOpinionsByProjectId(long projectId)
        {
            List<OpinionModel> opinionModels;
            if (projectId > 0)
            {
                opinionModels = (from o in _dbEntities.Opinions
                                 join user in _dbEntities.CmnUsers on o.AddedBy equals user.CmnUserId
                                 join master in _dbEntities.ProjectMasters on o.ProjectMasterId equals master.ProjectMasterId
                                 where o.ProjectMasterId == projectId //&& o.AddedDate > DateTime.Now.AddDays(-60)
                                 orderby o.AddedDate descending
                                 select new OpinionModel
                                 {
                                     OpinionId = o.OpinionId,
                                     AddedBy = o.AddedBy,
                                     OpinionText = o.OpinionText,
                                     AddedDate = o.AddedDate,
                                     ProjectMasterId = o.ProjectMasterId,
                                     UserFullName = user.UserFullName,
                                     UpdatedBy = o.UpdatedBy,
                                     UpdatedDate = o.UpdatedDate,
                                     WebServerUrl = user.ProfilePictureUrl,
                                     ProjectName = master.ProjectName
                                 }).Take(100).ToList();
            }
            else
            {
                opinionModels = (from o in _dbEntities.Opinions
                                 join user in _dbEntities.CmnUsers on o.AddedBy equals user.CmnUserId
                                 join master in _dbEntities.ProjectMasters on o.ProjectMasterId equals master.ProjectMasterId
                                 orderby o.AddedDate descending
                                 select new OpinionModel
                                 {
                                     OpinionId = o.OpinionId,
                                     AddedBy = o.AddedBy,
                                     OpinionText = o.OpinionText,
                                     AddedDate = o.AddedDate,
                                     ProjectMasterId = o.ProjectMasterId,
                                     UserFullName = user.UserFullName,
                                     UpdatedBy = o.UpdatedBy,
                                     UpdatedDate = o.UpdatedDate,
                                     WebServerUrl = user.ProfilePictureUrl,
                                     ProjectName = master.ProjectName
                                 }).Take(100).ToList();
            }
            var fileManager = new FileManager();
            if (opinionModels.Any())
            {
                foreach (var opinionModel in opinionModels)
                {
                    opinionModel.WebServerUrl = fileManager.GetFile(opinionModel.WebServerUrl);
                    if (string.IsNullOrWhiteSpace(opinionModel.WebServerUrl))
                        opinionModel.WebServerUrl = "../assets/layouts/layout4/img/av.png";
                }

            }
            return opinionModels;
        }

        public FileContentResult GetProfilePicture(long uId)
        {
            string fileName;
            if (uId <= 0)
            {
                fileName = HttpContext.Current.Server.MapPath(@"~/assets/layouts/layout4/img/av.png");
            }
            else
            {
                using (var dbEntities = new CellPhoneProjectEntities())
                {
                    var cmnUser = (from cu in dbEntities.CmnUsers
                                   where cu.CmnUserId == uId
                                   select new CmnUserModel
                                   {
                                       CmnUserId = cu.CmnUserId,
                                       UserName = cu.UserName,
                                       UserFullName = cu.UserFullName,
                                       EmployeeCode = cu.EmployeeCode,
                                       MobileNumber = cu.MobileNumber,
                                       Email = cu.Email,
                                       RoleName = cu.RoleName,
                                       ProfilePictureUrl = cu.ProfilePictureUrl
                                   }).FirstOrDefault();

                    fileName = cmnUser != null
                        ? cmnUser.ProfilePictureUrl
                        : HttpContext.Current.Server.MapPath(@"~/assets/layouts/layout4/img/av.png");
                }
            }
            try
            {
                var fileInfo = new FileInfo(fileName);
                long imageFileLength = fileInfo.Length;
                var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                var br = new BinaryReader(fs);
                byte[] imageData = br.ReadBytes((int)imageFileLength);

                return new FileContentResult(imageData, "image/png");
            }
            catch (Exception)
            {
                fileName = HttpContext.Current.Server.MapPath(@"~/assets/layouts/layout4/img/av.png");
                var fileInfo = new FileInfo(fileName);
                long imageFileLength = fileInfo.Length;
                var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                var br = new BinaryReader(fs);
                byte[] imageData = br.ReadBytes((int)imageFileLength);
                return new FileContentResult(imageData, "image/png");
            }
        }

        public ProjectStatusForHwModel GetProjectStatusForHw(long projectMasterId)
        {
            string query =
                string.Format(
                    @"select * from (select top 1 hqia.ProjectMasterId, hqia.SampleSetSentDate as ScreeningSampleSetSentDate,hqia.SampleSetReceiveDate as ScreeningSampleSetReceiveDate, 
hqa.HwQcAssignDate as ScreeningAssignedDate,hqa.QcSubmissionDate as ScreeningSubmittedDate,hqa.VerificationDate as ScreeningCheckedDate
,hqia.ForwardDate as ScreeningForwardedDate from HwQcInchargeAssigns hqia
left join HwQcAssigns hqa on hqia.HwQcInchargeAssignId=hqa.HwQcInchargeAssignId
 where hqia.IsScreeningTest=1 and hqia.ProjectMasterId={0} order by hqia.SampleSetSentDate desc) a

left join

(select top 1 hqia.ProjectMasterId, hqia.SampleSetSentDate as RunningSampleSetSentDate,hqia.SampleSetReceiveDate as RunningSampleSetReceiveDate, 
hqa.HwQcAssignDate as RunningAssignedDate,hqa.QcSubmissionDate as RunningSubmittedDate,hqa.VerificationDate as RunningCheckedDate
,hqia.ForwardDate as RunningForwardedDate from HwQcInchargeAssigns hqia
left join HwQcAssigns hqa on hqia.HwQcInchargeAssignId=hqa.HwQcInchargeAssignId
 where hqia.IsRunningTest=1 and hqia.ProjectMasterId={0} order by hqia.SampleSetSentDate desc) b

 on a.ProjectMasterId=b.ProjectMasterId

 left join

 (select top 1 hqia.ProjectMasterId, hqia.SampleSetSentDate as FinishedSampleSetSentDate,hqia.SampleSetReceiveDate as FinishedSampleReceivedDate,
  hqa.HwQcAssignDate as FinishedAssignedDate,hqa.QcSubmissionDate as FinishedGoodsSubmittedDate,hqa.VerificationDate as FinishedCheckedDate
 ,hqia.ForwardDate as FinishedForwardedDate from HwQcInchargeAssigns hqia
left join HwQcAssigns hqa on hqia.HwQcInchargeAssignId=hqa.HwQcInchargeAssignId
 where hqia.IsFinishedGoodTest=1 and hqia.ProjectMasterId={0} order by hqia.SampleSetSentDate desc) c

 on a.ProjectMasterId=c.ProjectMasterId", projectMasterId);

            var execute = _dbEntities.Database.SqlQuery<ProjectStatusForHwModel>(query).FirstOrDefault();
            return execute;
        }

        public ProjectDetailStatus GetProjectStatus(long id)
        {
            var status = new ProjectDetailStatus();
            if (id > 0)
            {
                using (var dbEntities = new CellPhoneProjectEntities())
                {
                    var dbProject = dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == id);
                    if (dbProject != null)
                    {
                        string formatedDate = string.Format("{0:ddd, MMM d, yyyy}", dbProject.AddedDate);
                        if (dbProject.ProjectStatus != "NEW" && dbProject.ProjectStatus != "REJECTED" && dbProject.ProjectStatus != "INITIAL")
                        {
                            /*
                             This if condition work for for screening test portion
                             */
                            if (dbProject.ProjectStatus == "PARTIAL" || dbProject.ProjectStatus == "APPROVED" || dbProject.ProjectStatus == "PARTIAL2")
                            {
                                status.CommercialList.Add(new DataObject
                                {
                                    Detail = "Initialized as new project on --" + formatedDate,
                                    ActionDate = dbProject.AddedDate != null ? (DateTime)dbProject.AddedDate : DateTime.MinValue
                                });
                                status.ManagementList.Add(new DataObject
                                {
                                    Detail = "Initially approved for screening test"
                                });
                                status.ManagementList.Add(new DataObject
                                {
                                    Detail = "m"
                                });
                                if (dbProject.GivenSampleToScreening != null)
                                {
                                    var screeningData = dbEntities.HwQcInchargeAssigns.FirstOrDefault(i => i.ProjectMasterId == dbProject.ProjectMasterId && i.IsScreeningTest == true);
                                    if (screeningData != null)
                                    {
                                        if (screeningData.SampleSetSentDate != null)
                                        {
                                            var sendDate = (DateTime)screeningData.SampleSetSentDate;
                                            string screeningDate = string.Format("{0:ddd, MMM d, yyyy}", sendDate);
                                            status.CommercialList.Add(new DataObject
                                            {
                                                Detail = "Forwarded for screening test on --" + screeningDate,
                                                ActionDate = sendDate
                                            });
                                            if (screeningData.TestPhase == "SAMPLESENT")
                                            {
                                                status.HardwareList.Add(new DataObject
                                                {
                                                    Detail = "Pending sample receive (Screening test) for " + Math.Floor((DateTime.Now - (DateTime)screeningData.SampleSetSentDate).TotalDays) + " days"
                                                });
                                                return status;
                                            }
                                            if (screeningData.TestPhase == "NEW")
                                            {
                                                status.HardwareList.Add(new DataObject
                                                {
                                                    Detail = "Sample received for screening test on --" + string.Format("{0:ddd, MMM d, yyyy}", screeningData.SampleSetReceiveDate)
                                                });
                                                var sampleSetReceiveDateVariable = screeningData.SampleSetReceiveDate != null ? (DateTime)screeningData.SampleSetReceiveDate : DateTime.MinValue;
                                                status.HardwareList.Add(new DataObject
                                                {
                                                    Detail = "Screeing test assign pending for " + Math.Floor((DateTime.Now - sampleSetReceiveDateVariable).TotalDays) + " days"
                                                });
                                                return status;
                                            }
                                            if (screeningData.TestPhase == "ASSINED" || screeningData.TestPhase == "QCPASSED" || screeningData.TestPhase == "FINISHED")
                                            {
                                                var hwAssign = dbEntities.HwQcAssigns.FirstOrDefault(i => i.HwQcInchargeAssignId == screeningData.HwQcInchargeAssignId);
                                                if (hwAssign != null)
                                                {
                                                    string dayDifference = hwAssign.UpdatedDate == null ? "undefined" : Math.Floor((DateTime.Now - (DateTime)hwAssign.UpdatedDate).TotalDays).ToString(CultureInfo.InvariantCulture) + " days";
                                                    status.HardwareList.Add(new DataObject
                                                    {
                                                        Detail = "Sample received for screening test on --" + string.Format("{0:ddd, MMM d, yyyy}", screeningData.SampleSetReceiveDate),
                                                        ActionDate = screeningData.SampleSetReceiveDate != null ? (DateTime)screeningData.SampleSetReceiveDate : DateTime.MinValue
                                                    });
                                                    status.HardwareList.Add(new DataObject
                                                    {
                                                        Detail = "Assigned for screening test on--" + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.HwQcAssignDate),
                                                        ActionDate = hwAssign.HwQcAssignDate
                                                    });
                                                    if (hwAssign.Status == "NEW")
                                                    {
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening test is not started yet. Pending for " + Math.Floor((DateTime.Now - hwAssign.HwQcAssignDate).TotalDays) + " days",
                                                            ActionDate = hwAssign.HwQcAssignDate
                                                        });
                                                    }
                                                    else if (hwAssign.Status == "RUNNING")
                                                    {

                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening started on --" + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.UpdatedDate) ?? "undefined",
                                                            ActionDate = hwAssign.HwQcAssignDate
                                                        });
                                                    }
                                                    else if (hwAssign.Status == "QCSUBMITTED")
                                                    {
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening started on --" + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.UpdatedDate) ?? "undefined",
                                                            ActionDate = hwAssign.HwQcAssignDate
                                                        });
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening test completed on " + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.QcSubmissionDate) ?? "undefined",
                                                            ActionDate = hwAssign.HwQcAssignDate
                                                        });
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Sceening test verificatin pending by hardware engineer for " + Math.Floor((DateTime.Now - (DateTime)hwAssign.QcSubmissionDate).TotalDays) + " days",
                                                            ActionDate = hwAssign.HwQcAssignDate
                                                        });
                                                    }
                                                    else if (hwAssign.Status == "QCPASSED")
                                                    {
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening started on --" + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.UpdatedDate) ?? "undefined",
                                                            ActionDate = hwAssign.HwQcAssignDate
                                                        });
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening test completed on " + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.QcSubmissionDate) ?? "undefined",
                                                            ActionDate = hwAssign.HwQcAssignDate
                                                        });
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening test verified by one of hardware engineer on --" + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.VerificationDate) ?? "undefined",
                                                            ActionDate = hwAssign.UpdatedDate ?? DateTime.MinValue
                                                        });
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening test result forward pending for " + Math.Floor((DateTime.Now - (DateTime)hwAssign.VerificationDate).TotalDays) + " days",
                                                            ActionDate = hwAssign.UpdatedDate ?? DateTime.MinValue
                                                        });
                                                    }
                                                    else if (hwAssign.Status == "FORWARDED")
                                                    {
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening started on --" + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.UpdatedDate) ?? "undefined",
                                                            ActionDate = hwAssign.HwQcAssignDate
                                                        });
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening test completed on " + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.QcSubmissionDate) ?? "undefined",
                                                            ActionDate = hwAssign.HwQcAssignDate
                                                        });
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening test verified by one of hardware engineer on --" + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.VerificationDate) ?? "undefined",
                                                            ActionDate = hwAssign.UpdatedDate ?? DateTime.MinValue
                                                        });

                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening test forwarded to commercial on --" + string.Format("{0:ddd, MMM d, yyyy}", screeningData.ForwardDate) ?? "undefined",
                                                            ActionDate = screeningData.ForwardDate ?? DateTime.MinValue
                                                        });
                                                        status.HardwareList.Add(new DataObject { Detail = "screeningFinished" });
                                                    }

                                                }
                                                else
                                                {
                                                    status.HardwareList.Add(new DataObject
                                                    {
                                                        Detail = "Sample received for screening test on --" + string.Format("{0:ddd, MMM d, yyyy}", screeningData.SampleSetReceiveDate),
                                                        ActionDate = screeningData.SampleSetReceiveDate != null ? (DateTime)screeningData.SampleSetReceiveDate : DateTime.MinValue
                                                    });
                                                    status.HardwareList.Add(new DataObject
                                                    {
                                                        Detail = "Assigned for screening test but something goes wrong in the engineer end"
                                                    });
                                                }
                                                //return status;
                                            }

                                        }
                                    }
                                    if (dbProject.IsScreenTestComplete == true && dbProject.SourcingType == "OEM")
                                    {
                                        //status.HardwareList.Add(new DataObject { Detail = "m" });
                                        if (dbProject.ProjectStatus == "PARTIAL2")
                                        {
                                            var pendingDaay = (screeningData != null &&
                                                               screeningData.ForwardDate == null)
                                                ? "undefined"
                                                : Math.Floor(
                                                    (DateTime.Now - (DateTime)screeningData.ForwardDate).TotalDays) +
                                                  " days";
                                            status.CommercialList.Add(new DataObject { Detail = "Screening issues review pending for " + pendingDaay });
                                            return status;
                                        }
                                        if (dbProject.ProjectStatus == "PARTIAL")
                                        {
                                            status.CommercialList.Add(new DataObject
                                            {
                                                Detail = "Screening test result reviewed"
                                            });
                                            status.ManagementList.Add(new DataObject
                                            {
                                                Detail = "Final approval pending"
                                            });
                                            return status;
                                        }
                                        if (dbProject.ProjectStatus == "APPROVED")
                                        {
                                            status.CommercialList.Add(new DataObject
                                            {
                                                Detail = "Screening test result reviewed"
                                            });
                                            status.ManagementList.Add(new DataObject
                                            {
                                                Detail = "Final approval completed"
                                            });
                                            status.ManagementList.Add(new DataObject { Detail = "m" });
                                        }
                                    }
                                    if (dbProject.IsScreenTestComplete != true && dbProject.SourcingType == "ODM" && dbProject.ProjectStatus == "APPROVED")
                                    {
                                        status.ManagementList.Add(new DataObject
                                        {
                                            Detail = "Final approval completed"
                                        });
                                    }
                                    var purchaseOrder = dbEntities.ProjectPurchaseOrderForms.FirstOrDefault(i => i.ProjectMasterId == id);
                                    if (purchaseOrder != null)
                                    {
                                        //-------------------Commercial list creation-------------------------
                                        status.CommercialList.Add(new DataObject { Detail = "Purhase order created on --" + string.Format("{0:ddd, MMM d, yyyy}", purchaseOrder.AddedDate) });
                                        //Commercial all task will load here

                                        List<CommonStatusObject> statusObjects = GetCommercialWorkFlow(id);
                                        if (statusObjects.Any())
                                        {
                                            foreach (var statusObject in statusObjects)
                                            {
                                                status.CommonStatusObjects.Add(statusObject);
                                            }
                                        }
                                        if (dbProject.IsProjectManagerAssigned != true)
                                        {
                                            status.ProjectManagerList.Add(new DataObject { Detail = "Project assign pending to project manager for " + Math.Floor((DateTime.Now - (DateTime)purchaseOrder.AddedDate).TotalDays) + " days" });
                                            return status;
                                        }
                                        if (dbProject.IsProjectManagerAssigned == true)
                                        {
                                            var pmAssign = dbEntities.ProjectPmAssigns.FirstOrDefault(i => i.ProjectMasterId == id);
                                            if (pmAssign != null)
                                            {
                                                status.ProjectManagerList.Add(new DataObject { Detail = "Project assigned to project manager on --" + string.Format("{0:ddd, MMM d, yyyy}", pmAssign.AssignDate) });
                                                var hardwareAssign =
                                                    dbEntities.HwQcInchargeAssigns.FirstOrDefault(
                                                        i => i.ProjectMasterId == id && i.IsRunningTest == true);
                                                var softwareAssign = dbEntities.SwQcInchargeAssigns.Where(i => i.ProjectMasterId == id).OrderByDescending(i => i.ProjectMasterId).FirstOrDefault();
                                                if (hardwareAssign == null && softwareAssign == null)
                                                {
                                                    status.ProjectManagerList.Add(new DataObject { Detail = "Hardware and Software both assign pending for " + Math.Floor((DateTime.Now - pmAssign.AssignDate).TotalDays) + " days" });
                                                    return status;
                                                }
                                                if (hardwareAssign != null)
                                                {
                                                    status.ProjectManagerList.Add(new DataObject { Detail = "Project assigned to hardware for running test on --" + string.Format("{0:ddd, MMM d, yyyy}", hardwareAssign.HwQcInchargeAssignDate) });
                                                    if (hardwareAssign.SampleSetReceiveDate == null)
                                                    {
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail =
                                                                "Running test's sample receive pending for " +
                                                                Math.Floor(
                                                                    (DateTime.Now -
                                                                     (DateTime)hardwareAssign.HwQcInchargeAssignDate)
                                                                        .TotalDays) + " days"
                                                        });
                                                    }
                                                    else
                                                    {
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail =
                                                                "Sample Received for running test on --" +
                                                                string.Format("{0:ddd, MMM d, yyyy}",
                                                                    hardwareAssign.SampleSetReceiveDate)
                                                        });

                                                        //hardware data should be load here
                                                        List<CommonStatusObject> hwList = GetHardwareWorkFlow(hardwareAssign);
                                                        if (hwList.Any())
                                                        {
                                                            foreach (var o in hwList)
                                                            {
                                                                status.CommonStatusObjects.Add(o);
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    status.ProjectManagerList.Add(new DataObject { Detail = "project" + Math.Floor((DateTime.Now - pmAssign.AssignDate).TotalDays) + " days" });
                                                }

                                                if (softwareAssign != null)
                                                {
                                                    status.ProjectManagerList.Add(new DataObject
                                                    {
                                                        Detail =
                                                            "Project assigned to software for test on --" +
                                                            string.Format("{0:ddd, MMM d, yyyy}",
                                                            softwareAssign.AddedDate)
                                                    });
                                                    if (softwareAssign.Status == "NEW")
                                                    {
                                                        status.SoftwareList.Add(new DataObject
                                                        {
                                                            Detail =
                                                                "Software QC engineer assign pending for " +
                                                                Math.Floor((DateTime.Now - (DateTime)softwareAssign.SwQcInchargeAssignTime).TotalDays) +
                                                                " days"
                                                        });
                                                    }
                                                    else
                                                    {
                                                        status.SoftwareList.Add(new DataObject
                                                        {
                                                            Detail =
                                                                "Project assigned to software for test on --" +
                                                                string.Format("{0:ddd, MMM d, yyyy}",
                                                                softwareAssign.QcInchargeToQcAssignTime)
                                                        });
                                                        List<CommonStatusObject> swList = GetSowftwareWorkFlow(softwareAssign);
                                                        if (swList.Any())
                                                        {
                                                            foreach (var o in swList)
                                                            {
                                                                status.CommonStatusObjects.Add(o);
                                                            }
                                                        }
                                                    }

                                                }
                                                else
                                                {
                                                    status.ProjectManagerList.Add(new DataObject
                                                    {
                                                        Detail =
                                                            "Project software testing assign pending for " +
                                                            Math.Floor((DateTime.Now - pmAssign.AssignDate).TotalDays) +
                                                            " days"
                                                    });
                                                }
                                                List<CommonStatusObject> pmList = GetProjectManagerWorkFlow(id);
                                                if (pmList.Any())
                                                {
                                                    foreach (var o in pmList)
                                                    {
                                                        status.CommonStatusObjects.Add(o);
                                                    }
                                                }
                                            }

                                        }
                                        var reversed = status.CommonStatusObjects.OrderBy(i => i.ActionDate).ToList();
                                        status.CommonStatusObjects = reversed;

                                    }
                                    else
                                    {
                                        status.CommercialList.Add(new DataObject { Detail = "Purhase order pending " });
                                    }
                                }
                            }

                        }
                        else switch (dbProject.ProjectStatus)
                            {
                                case "NEW":
                                    status.CommercialList.Add(new DataObject
                                    {
                                        ActionDate =
                                            dbProject.AddedDate != null ? (DateTime)dbProject.AddedDate : new DateTime(),
                                        Detail = "Initialized as new project on --" + formatedDate
                                    });
                                    status.CommercialList.Add(new DataObject
                                    {
                                        ActionDate = DateTime.MinValue,
                                        Detail = "end"
                                    });

                                    status.ManagementList.Add(new DataObject
                                    {
                                        Detail = "Initial approval pending for " + Math.Floor((DateTime.Now - (DateTime)dbProject.AddedDate).TotalDays) + " days"
                                    });
                                    break;
                                case "INITIAL":
                                    status.CommercialList.Add(new DataObject
                                    {
                                        ActionDate =
                                            dbProject.AddedDate != null ? (DateTime)dbProject.AddedDate : new DateTime(),
                                        Detail = "Initialized as new project on --" + formatedDate
                                    });
                                    status.CommercialList.Add(new DataObject
                                    {
                                        ActionDate = DateTime.MinValue,
                                        Detail = "end"
                                    });

                                    status.ManagementList.Add(new DataObject
                                    {
                                        Detail = "Initially approved for screening test"
                                    });
                                    status.ManagementList.Add(new DataObject
                                    {
                                        Detail = "m"
                                    });
                                    break;
                            }
                    }
                }
                return status;
            }
            return null;
        }

        public ProjectDetailStatus GetProjectStatusByModules(long id)
        {
            var status = new ProjectDetailStatus();
            if (id > 0)
            {
                using (var dbEntities = new CellPhoneProjectEntities())
                {
                    var dbProject = dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == id);
                    if (dbProject != null)
                    {
                        string formatedDate = string.Format("{0:ddd, MMM d, yyyy}", dbProject.AddedDate);
                        if (dbProject.ProjectStatus != "NEW" && dbProject.ProjectStatus != "REJECTED" && dbProject.ProjectStatus != "INITIAL")
                        {
                            /*
                             This if condition work for for screening test portion
                             */
                            if (dbProject.ProjectStatus == "PARTIAL" || dbProject.ProjectStatus == "APPROVED" || dbProject.ProjectStatus == "PARTIAL2")
                            {
                                status.CommercialList.Add(new DataObject
                                {
                                    Detail = "Initialized as new project on --" + formatedDate,
                                    ActionDate = dbProject.AddedDate != null ? (DateTime)dbProject.AddedDate : DateTime.MinValue
                                });
                                status.ManagementList.Add(new DataObject
                                {
                                    Detail = "Initially approved for screening test"
                                });
                                status.ManagementList.Add(new DataObject
                                {
                                    Detail = "m"
                                });
                                if (dbProject.GivenSampleToScreening != null)
                                {
                                    var screeningData = dbEntities.HwQcInchargeAssigns.FirstOrDefault(i => i.ProjectMasterId == dbProject.ProjectMasterId && i.IsScreeningTest == true);
                                    if (screeningData != null)
                                    {
                                        if (screeningData.SampleSetSentDate != null)
                                        {
                                            var sendDate = (DateTime)screeningData.SampleSetSentDate;
                                            string screeningDate = string.Format("{0:ddd, MMM d, yyyy}", sendDate);
                                            status.CommercialList.Add(new DataObject
                                            {
                                                Detail = "Forwarded for screening test on --" + screeningDate,
                                                ActionDate = sendDate
                                            });
                                            if (screeningData.TestPhase == "SAMPLESENT")
                                            {
                                                status.HardwareList.Add(new DataObject
                                                {
                                                    Detail = "Pending sample receive (Screening test) for " + Math.Floor((DateTime.Now - (DateTime)screeningData.SampleSetSentDate).TotalDays) + " days"
                                                });
                                                return status;
                                            }
                                            if (screeningData.TestPhase == "NEW")
                                            {
                                                status.HardwareList.Add(new DataObject
                                                {
                                                    Detail = "Sample received for screening test on --" + string.Format("{0:ddd, MMM d, yyyy}", screeningData.SampleSetReceiveDate)
                                                });
                                                var sampleSetReceiveDateVariable = screeningData.SampleSetReceiveDate != null ? (DateTime)screeningData.SampleSetReceiveDate : DateTime.MinValue;
                                                status.HardwareList.Add(new DataObject
                                                {
                                                    Detail = "Screeing test assign pending for " + Math.Floor((DateTime.Now - sampleSetReceiveDateVariable).TotalDays) + " days"
                                                });
                                                return status;
                                            }
                                            if (screeningData.TestPhase == "ASSINED" || screeningData.TestPhase == "QCPASSED" || screeningData.TestPhase == "FINISHED")
                                            {
                                                var hwAssign = dbEntities.HwQcAssigns.FirstOrDefault(i => i.HwQcInchargeAssignId == screeningData.HwQcInchargeAssignId);
                                                if (hwAssign != null)
                                                {
                                                    string dayDifference = hwAssign.UpdatedDate == null ? "undefined" : Math.Floor((DateTime.Now - (DateTime)hwAssign.UpdatedDate).TotalDays).ToString(CultureInfo.InvariantCulture) + " days";
                                                    status.HardwareList.Add(new DataObject
                                                    {
                                                        Detail = "Sample received for screening test on --" + string.Format("{0:ddd, MMM d, yyyy}", screeningData.SampleSetReceiveDate),
                                                        ActionDate = screeningData.SampleSetReceiveDate != null ? (DateTime)screeningData.SampleSetReceiveDate : DateTime.MinValue
                                                    });
                                                    status.HardwareList.Add(new DataObject
                                                    {
                                                        Detail = "Assigned for screening test on--" + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.HwQcAssignDate),
                                                        ActionDate = hwAssign.HwQcAssignDate
                                                    });
                                                    if (hwAssign.Status == "NEW")
                                                    {
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening test is not started yet. Pending for " + Math.Floor((DateTime.Now - hwAssign.HwQcAssignDate).TotalDays) + " days",
                                                            ActionDate = hwAssign.HwQcAssignDate
                                                        });
                                                    }
                                                    else if (hwAssign.Status == "RUNNING")
                                                    {

                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening started on --" + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.UpdatedDate) ?? "undefined",
                                                            ActionDate = hwAssign.HwQcAssignDate
                                                        });
                                                    }
                                                    else if (hwAssign.Status == "QCSUBMITTED")
                                                    {
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening started on --" + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.UpdatedDate) ?? "undefined",
                                                            ActionDate = hwAssign.HwQcAssignDate
                                                        });
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening test completed on " + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.QcSubmissionDate) ?? "undefined",
                                                            ActionDate = hwAssign.HwQcAssignDate
                                                        });
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Sceening test verificatin pending by hardware engineer for " + Math.Floor((DateTime.Now - (DateTime)hwAssign.QcSubmissionDate).TotalDays) + " days",
                                                            ActionDate = hwAssign.HwQcAssignDate
                                                        });
                                                    }
                                                    else if (hwAssign.Status == "QCPASSED")
                                                    {
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening started on --" + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.UpdatedDate) ?? "undefined",
                                                            ActionDate = hwAssign.HwQcAssignDate
                                                        });
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening test completed on " + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.QcSubmissionDate) ?? "undefined",
                                                            ActionDate = hwAssign.HwQcAssignDate
                                                        });
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening test verified by one of hardware engineer on --" + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.VerificationDate) ?? "undefined",
                                                            ActionDate = hwAssign.UpdatedDate ?? DateTime.MinValue
                                                        });
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening test result forward pending for " + Math.Floor((DateTime.Now - (DateTime)hwAssign.VerificationDate).TotalDays) + " days",
                                                            ActionDate = hwAssign.UpdatedDate ?? DateTime.MinValue
                                                        });
                                                    }
                                                    else if (hwAssign.Status == "FORWARDED")
                                                    {
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening started on --" + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.UpdatedDate) ?? "undefined",
                                                            ActionDate = hwAssign.HwQcAssignDate
                                                        });
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening test completed on " + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.QcSubmissionDate) ?? "undefined",
                                                            ActionDate = hwAssign.HwQcAssignDate
                                                        });
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening test verified by one of hardware engineer on --" + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.VerificationDate) ?? "undefined",
                                                            ActionDate = hwAssign.UpdatedDate ?? DateTime.MinValue
                                                        });

                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail = "Screening test forwarded to commercial on --" + string.Format("{0:ddd, MMM d, yyyy}", screeningData.ForwardDate) ?? "undefined",
                                                            ActionDate = screeningData.ForwardDate ?? DateTime.MinValue
                                                        });
                                                        status.HardwareList.Add(new DataObject { Detail = "screeningFinished" });
                                                    }

                                                }
                                                else
                                                {
                                                    status.HardwareList.Add(new DataObject
                                                    {
                                                        Detail = "Sample received for screening test on --" + string.Format("{0:ddd, MMM d, yyyy}", screeningData.SampleSetReceiveDate),
                                                        ActionDate = screeningData.SampleSetReceiveDate != null ? (DateTime)screeningData.SampleSetReceiveDate : DateTime.MinValue
                                                    });
                                                    status.HardwareList.Add(new DataObject
                                                    {
                                                        Detail = "Assigned for screening test but something goes wrong in the engineer end"
                                                    });
                                                }
                                                //return status;
                                            }

                                        }
                                    }
                                    if (dbProject.IsScreenTestComplete == true && dbProject.SourcingType == "OEM")
                                    {
                                        //status.HardwareList.Add(new DataObject { Detail = "m" });
                                        if (dbProject.ProjectStatus == "PARTIAL2")
                                        {
                                            var pendingDaay = (screeningData != null &&
                                                               screeningData.ForwardDate == null)
                                                ? "undefined"
                                                : Math.Floor(
                                                    (DateTime.Now - (DateTime)screeningData.ForwardDate).TotalDays) +
                                                  " days";
                                            status.CommercialList.Add(new DataObject { Detail = "Screening issues review pending for " + pendingDaay });
                                            return status;
                                        }
                                        if (dbProject.ProjectStatus == "PARTIAL")
                                        {
                                            status.CommercialList.Add(new DataObject
                                            {
                                                Detail = "Screening test result reviewed"
                                            });
                                            status.ManagementList.Add(new DataObject
                                            {
                                                Detail = "Final approval pending"
                                            });
                                            return status;
                                        }
                                        if (dbProject.ProjectStatus == "APPROVED")
                                        {
                                            status.CommercialList.Add(new DataObject
                                            {
                                                Detail = "Screening test result reviewed"
                                            });
                                            status.ManagementList.Add(new DataObject
                                            {
                                                Detail = "Final approval completed"
                                            });
                                            status.ManagementList.Add(new DataObject { Detail = "m" });
                                        }
                                    }
                                    if (dbProject.IsScreenTestComplete != true && dbProject.SourcingType == "ODM" && dbProject.ProjectStatus == "APPROVED")
                                    {
                                        status.ManagementList.Add(new DataObject
                                        {
                                            Detail = "Final approval completed"
                                        });
                                    }
                                    var purchaseOrder = dbEntities.ProjectPurchaseOrderForms.FirstOrDefault(i => i.ProjectMasterId == id);
                                    if (purchaseOrder != null)
                                    {
                                        //-------------------Commercial list creation-------------------------
                                        status.CommercialList.Add(new DataObject { Detail = "Purhase order created on --" + string.Format("{0:ddd, MMM d, yyyy}", purchaseOrder.AddedDate) });
                                        //Commercial all task will load here

                                        List<CommonStatusObject> statusObjects = GetCommercialWorkFlow(id);
                                        if (statusObjects.Any())
                                        {
                                            foreach (var statusObject in statusObjects)
                                            {
                                                status.CommonStatusObjects.Add(statusObject);
                                            }
                                        }
                                        if (dbProject.IsProjectManagerAssigned != true)
                                        {
                                            status.ProjectManagerList.Add(new DataObject { Detail = "Project assign pending to project manager for " + Math.Floor((DateTime.Now - (DateTime)purchaseOrder.AddedDate).TotalDays) + " days" });
                                            return status;
                                        }
                                        if (dbProject.IsProjectManagerAssigned == true)
                                        {
                                            var pmAssign = dbEntities.ProjectPmAssigns.FirstOrDefault(i => i.ProjectMasterId == id);
                                            if (pmAssign != null)
                                            {
                                                status.ProjectManagerList.Add(new DataObject { Detail = "Project assigned to project manager on --" + string.Format("{0:ddd, MMM d, yyyy}", pmAssign.AssignDate) });
                                                var hardwareAssign =
                                                    dbEntities.HwQcInchargeAssigns.FirstOrDefault(
                                                        i => i.ProjectMasterId == id && i.IsRunningTest == true);
                                                var softwareAssign = dbEntities.SwQcInchargeAssigns.Where(i => i.ProjectMasterId == id).OrderByDescending(i => i.ProjectMasterId).FirstOrDefault();
                                                if (hardwareAssign == null && softwareAssign == null)
                                                {
                                                    status.ProjectManagerList.Add(new DataObject { Detail = "Hardware and Software both assign pending for " + Math.Floor((DateTime.Now - pmAssign.AssignDate).TotalDays) + " days" });
                                                    return status;
                                                }
                                                if (hardwareAssign != null)
                                                {
                                                    status.ProjectManagerList.Add(new DataObject { Detail = "Project assigned to hardware for running test on --" + string.Format("{0:ddd, MMM d, yyyy}", hardwareAssign.HwQcInchargeAssignDate) });
                                                    if (hardwareAssign.SampleSetReceiveDate == null)
                                                    {
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail =
                                                                "Running test's sample receive pending for " +
                                                                Math.Floor(
                                                                    (DateTime.Now -
                                                                     (DateTime)hardwareAssign.HwQcInchargeAssignDate)
                                                                        .TotalDays) + " days"
                                                        });
                                                    }
                                                    else
                                                    {
                                                        status.HardwareList.Add(new DataObject
                                                        {
                                                            Detail =
                                                                "Sample Received for running test on --" +
                                                                string.Format("{0:ddd, MMM d, yyyy}",
                                                                    hardwareAssign.SampleSetReceiveDate)
                                                        });

                                                        //hardware data should be load here
                                                        List<CommonStatusObject> hwList = GetHardwareWorkFlow(hardwareAssign);
                                                        if (hwList.Any())
                                                        {
                                                            foreach (var o in hwList)
                                                            {
                                                                status.CommonStatusObjects.Add(o);
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    status.ProjectManagerList.Add(new DataObject { Detail = "project" + Math.Floor((DateTime.Now - pmAssign.AssignDate).TotalDays) + " days" });
                                                }

                                                if (softwareAssign != null)
                                                {
                                                    status.ProjectManagerList.Add(new DataObject
                                                    {
                                                        Detail =
                                                            "Project assigned to software for test on --" +
                                                            string.Format("{0:ddd, MMM d, yyyy}",
                                                            softwareAssign.AddedDate)
                                                    });
                                                    if (softwareAssign.Status == "NEW")
                                                    {
                                                        status.SoftwareList.Add(new DataObject
                                                        {
                                                            Detail =
                                                                "Software QC engineer assign pending for " +
                                                                Math.Floor((DateTime.Now - (DateTime)softwareAssign.SwQcInchargeAssignTime).TotalDays) +
                                                                " days"
                                                        });
                                                    }
                                                    else
                                                    {
                                                        status.SoftwareList.Add(new DataObject
                                                        {
                                                            Detail =
                                                                "Project assigned to software for test on --" +
                                                                string.Format("{0:ddd, MMM d, yyyy}",
                                                                softwareAssign.QcInchargeToQcAssignTime)
                                                        });
                                                        List<CommonStatusObject> swList = GetSowftwareWorkFlow(softwareAssign);
                                                        if (swList.Any())
                                                        {
                                                            foreach (var o in swList)
                                                            {
                                                                status.CommonStatusObjects.Add(o);
                                                            }
                                                        }
                                                    }

                                                }
                                                else
                                                {
                                                    status.ProjectManagerList.Add(new DataObject
                                                    {
                                                        Detail =
                                                            "Project software testing assign pending for " +
                                                            Math.Floor((DateTime.Now - pmAssign.AssignDate).TotalDays) +
                                                            " days"
                                                    });
                                                }
                                                List<CommonStatusObject> pmList = GetProjectManagerWorkFlow(id);
                                                if (pmList.Any())
                                                {
                                                    foreach (var o in pmList)
                                                    {
                                                        status.CommonStatusObjects.Add(o);
                                                    }
                                                }
                                            }

                                        }
                                        var reversed = status.CommonStatusObjects.OrderBy(i => i.ActionDate).ToList();
                                        status.CommonStatusObjects = reversed;

                                    }
                                    else
                                    {
                                        status.CommercialList.Add(new DataObject { Detail = "Purhase order pending " });
                                    }
                                }
                            }

                        }
                        else switch (dbProject.ProjectStatus)
                            {
                                case "NEW":
                                    status.CommercialList.Add(new DataObject
                                    {
                                        ActionDate =
                                            dbProject.AddedDate != null ? (DateTime)dbProject.AddedDate : new DateTime(),
                                        Detail = "Initialized as new project on --" + formatedDate
                                    });
                                    status.CommercialList.Add(new DataObject
                                    {
                                        ActionDate = DateTime.MinValue,
                                        Detail = "end"
                                    });

                                    status.ManagementList.Add(new DataObject
                                    {
                                        Detail = "Initial approval pending for " + Math.Floor((DateTime.Now - (DateTime)dbProject.AddedDate).TotalDays) + " days"
                                    });
                                    break;
                                case "INITIAL":
                                    status.CommercialList.Add(new DataObject
                                    {
                                        ActionDate =
                                            dbProject.AddedDate != null ? (DateTime)dbProject.AddedDate : new DateTime(),
                                        Detail = "Initialized as new project on --" + formatedDate
                                    });
                                    status.CommercialList.Add(new DataObject
                                    {
                                        ActionDate = DateTime.MinValue,
                                        Detail = "end"
                                    });

                                    status.ManagementList.Add(new DataObject
                                    {
                                        Detail = "Initially approved for screening test"
                                    });
                                    status.ManagementList.Add(new DataObject
                                    {
                                        Detail = "m"
                                    });
                                    break;
                            }
                    }
                }
                return status;
            }
            return null;
        }



        public CmStatusObject GetCmStatusObject(long id)
        {
            string query = string.Format(@"select top 1 pm.ProjectMasterId,pm.ProjectName,pm.OrderNuber,pm.ProjectStatus,pm.SourcingType,pm.AddedDate as ProjectInitialize, pm.InitialApprovalDate, 
                                           hii.AddedDate as ScreeningIssueReview,hii.UpdatedDate as ScreeningIssueReviewDone,pof.UpdatedDate as PoClosingDate,pof.IsCompleted,
(select top 1 n.added from Notification n where n.message like '%{1}%' and n.ProjectMasterId={0})  as ForwardForFinalApproval,pm.FinalApprovalDate,
                                           pof.PoDate as PurchaseOrder,pm.ScreeningIssueReviewDate from ProjectMasters pm
                                           left join HwInchargeIssues hii on pm.ProjectMasterId=hii.ProjectMasterId
                                           left join Notification n on pm.ProjectMasterId=n.ProjectMasterId
                                           left join ProjectPurchaseOrderForms pof on pm.ProjectMasterId=pof.ProjectMasterId
                                           where pm.ProjectMasterId={0}", id, "approved a project from final approval section");
            var exe = _dbEntities.Database.SqlQuery<CmStatusObject>(query).FirstOrDefault();
            return exe;
        }

        public PmStatusObject GetPmStatusObject(long id)
        {
            string query = string.Format(@"select pma.AssignDate as PmAssignDate,
                                          (select top 1 AddedDate from HwQcInchargeAssigns where ProjectMasterId={0} and IsRunningTest=1) as RunningForwardDate,
                                          (select top 1 AddedDate from HwQcInchargeAssigns where ProjectMasterId={0} and IsFinishedGoodTest=1) as FinishedForwardDate,
                                          (select top 1 AddedDate from SwQcInchargeAssigns where ProjectMasterId={0}) as SwQcForwardDate
                                          from ProjectMasters pm
                                          left join ProjectPmAssigns pma on pm.ProjectMasterId=pma.ProjectMasterId 
                                          where pm.ProjectMasterId={0} and pma.Status like 'ASSIGNED'", id);
            var exe = _dbEntities.Database.SqlQuery<PmStatusObject>(query).FirstOrDefault();
            return exe;
        }

        public HwScreeningStatusObject GetHwScreeningStatusObject(long id)
        {
            string str = string.Format(@"select top 1 hqia.TestPhase,hqia.SampleSetSentDate as ScreeningSampleSent,hqia.SampleSetReceiveDate as ScreeningSampleReceive, hqa.HwQcAssignDate as ScreeningAssign,
                                        hqa.QcSubmissionDate as ScreeningSubmit,hqa.VerificationDate as ScreeningVerified,hqia.ForwardDate as ScreeningForward
                                       from ProjectMasters pm
                                       left join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId
                                       left join HwQcAssigns hqa on hqia.HwQcInchargeAssignId=hqa.HwQcInchargeAssignId
                                       where pm.ProjectMasterId={0} and hqia.IsScreeningTest=1", id);
            var e = _dbEntities.Database.SqlQuery<HwScreeningStatusObject>(str).FirstOrDefault();
            return e;
        }

        public HwRunningStatusObject GetHwRunningStatusObject(long id)
        {
            string str = string.Format(@"select top 1 hqia.TestPhase,hqia.SampleSetSentDate as RunningSampleSent,hqia.SampleSetReceiveDate as RunningSampleReceive, hqa.HwQcAssignDate as RunningAssign,
                                         hqa.QcSubmissionDate as RunningSubmit,hqa.VerificationDate as RunningVerified,hqia.ForwardDate as RunningForward
                                         from ProjectMasters pm
                                         left join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId
                                         left join HwQcAssigns hqa on hqia.HwQcInchargeAssignId=hqa.HwQcInchargeAssignId
                                         where pm.ProjectMasterId={0} and hqia.IsRunningTest=1", id);
            var e = _dbEntities.Database.SqlQuery<HwRunningStatusObject>(str).FirstOrDefault();
            return e;
        }

        public HwFinishedStatusObject GetHwFinishedStatusObject(long id)
        {
            string str = string.Format(@"select top 1 hqia.TestPhase,hqia.SampleSetSentDate as FinishedSampleSent,hqia.SampleSetReceiveDate as FinishedSampleReceive, hqa.HwQcAssignDate as FinishedAssign,
                                         hqa.QcSubmissionDate as FinishedSubmit,hqa.VerificationDate as FinishedVerified,hqia.ForwardDate as FinishedForward
                                         from ProjectMasters pm
                                         left join HwQcInchargeAssigns hqia on pm.ProjectMasterId=hqia.ProjectMasterId
                                         left join HwQcAssigns hqa on hqia.HwQcInchargeAssignId=hqa.HwQcInchargeAssignId
                                         where pm.ProjectMasterId={0} and hqia.IsFinishedGoodTest=1", id);
            var e = _dbEntities.Database.SqlQuery<HwFinishedStatusObject>(str).FirstOrDefault();
            return e;
        }

        public SwStatusObject GetSwStatusObject(long id)
        {
            string query =
                string.Format(@"select top 1 sqia.ProjectManagerAssignToQcInTime, sqia.QcInchargeToQcAssignTime,sqia.QcInchargeToPmProjectSubmitTime 
                                from SwQcInchargeAssigns sqia
                                where sqia.ProjectMasterId={0}", id);
            var exe = _dbEntities.Database.SqlQuery<SwStatusObject>(query).FirstOrDefault();
            return exe;
        }
        public List<ProjectMasterModel> GetPostProductionProjects(long userId = 0)
        {
            var models = _dbEntities.ProjectMasters.Where(i => i.ProjectStatus == "APPROVED").ToList();
            if (models.Any())
            {
                var projectMasterModels = models.Select(i => new ProjectMasterModel
                {
                    ProjectName = i.ProjectName,
                    ProjectMasterId = i.ProjectMasterId
                }).ToList();
                return projectMasterModels;
            }
            return new List<ProjectMasterModel>();
        }

        public List<ProjectMasterModel> GetProjectsByName(string projectName)
        {
            var models = _dbEntities.ProjectMasters.Where(i => i.ProjectName == projectName).Select(i => new ProjectMasterModel
            {
                OrderNuber = i.OrderNuber
            }).ToList();
            return models;
        }

        public bool SavePostProductionIssue(PostProductionIssueModel model)
        {
            try
            {
                string viewers = GetViewerIdDependOnProject(model.ProjectName, model.OrderNumbers);
                if (!string.IsNullOrWhiteSpace(viewers))
                {
                    long userId;
                    long.TryParse(HttpContext.Current.User.Identity.Name, out userId);
                    var postProductionIssue = new PostProductionIssue
                    {
                        IssueName = model.IssueName,
                        Comment = model.Comment,
                        IssueType = model.IssueType,
                        Frequency = model.Frequency,
                        IssueReproducePath = model.IssueReproducePath,
                        ViewerIds = viewers,
                        ProjectName = model.ProjectName,
                        PurchaseOrderNo = string.Join(",", model.OrderNumbers),
                        Added = userId,
                        AddedDate = DateTime.Now
                    };
                    var ordinals = model.OrderNumbers.Select(orderNumber => CommonConversion.AddOrdinal(orderNumber)).ToList();
                    postProductionIssue.PurchaseOrderOrdinals = string.Join(",", ordinals);
                    var fileManager = new FileManager();
                    var fileUrls = model.File.Select(file => fileManager.Upload(0, "postProduction", "postProduction", file)).ToList();
                    postProductionIssue.Upload = string.Join("|", fileUrls);
                    _dbEntities.PostProductionIssues.Add(postProductionIssue);
                    _dbEntities.SaveChanges();
                    CreatePostProductionNotification(userId, postProductionIssue.ProjectName, postProductionIssue.ProjectMasterId, postProductionIssue.PurchaseOrderOrdinals, viewers, model.OrderNumbers);
                }
            }
            catch (Exception)
            {
                return false;
            }


            return true;
        }

        public List<PostProductionIssueModel> GetPostProductionIssues(long userid, long swqcallprojectissueid)
        {
            var issues = new List<PostProductionIssueModel>();
            if (swqcallprojectissueid == 0)
            {
                var postproductionissues =
                _dbEntities.PostProductionIssues.Where(i => i.CurrentStatus != "APPROVED" || i.CurrentStatus != "CANCELED").OrderByDescending(i => i.SwQcAllProjectIssueId).Take(10).ToList();
                issues.AddRange(from postproductionissue in postproductionissues
                                where postproductionissue.ViewerIds != null
                                let str = postproductionissue.ViewerIds.Split(',').ToList()
                                let viewrids = str.Select(long.Parse).ToList()
                                where viewrids.FindIndex(i => i == userid) != -1
                                let firstOrDefault = _dbEntities.CmnUsers.FirstOrDefault(j => j.CmnUserId == postproductionissue.Added)
                                select new PostProductionIssueModel
                                {
                                    SwQcAllProjectIssueId = postproductionissue.SwQcAllProjectIssueId,
                                    ProjectName = postproductionissue.ProjectName,
                                    IssueName = postproductionissue.IssueName,
                                    Comment = postproductionissue.Comment,
                                    IssueReproducePath = postproductionissue.IssueReproducePath ?? "",
                                    PurchaseOrderOrdinals = postproductionissue.PurchaseOrderOrdinals,
                                    AddedByName = firstOrDefault != null ? firstOrDefault.UserFullName : "",
                                    AddedDate = postproductionissue.AddedDate,
                                    Frequency = postproductionissue.Frequency,
                                    IssueType = postproductionissue.IssueType,
                                    PurchaseOrderNo = postproductionissue.PurchaseOrderNo,
                                    Upload = postproductionissue.Upload,
                                    ProfilePictureUrl = firstOrDefault.ProfilePictureUrl
                                });
                var file = new FileManager();
                if (issues.Any())
                {
                    foreach (var model in issues)
                    {
                        var filenamestr = model.Upload.Split('|').ToList();
                        var filenamelist = filenamestr.ToList();
                        if (filenamelist.Any())
                        {
                            foreach (string s in filenamelist)
                            {
                                var files = file.GetFile(s);
                                model.UploadedFileGetUrl.Add(files);
                                string extension = Path.GetExtension(s);
                                model.ExtensionlList.Add(extension);
                            }
                        }
                        model.ProfilePictureUrl = file.GetFile(model.ProfilePictureUrl);
                        if (string.IsNullOrWhiteSpace(model.ProfilePictureUrl))
                            model.ProfilePictureUrl = "../assets/layouts/layout4/img/av.png";
                    }
                }
            }
            else
            {
                var postproductionissues =
                _dbEntities.PostProductionIssues.Where(i => (i.CurrentStatus != "APPROVED" || i.CurrentStatus != "CANCELED") && i.SwQcAllProjectIssueId < swqcallprojectissueid).OrderByDescending(i => i.SwQcAllProjectIssueId).Take(10).ToList();
                issues.AddRange(from postproductionissue in postproductionissues
                                where postproductionissue.ViewerIds != null
                                let str = postproductionissue.ViewerIds.Split(',').ToList()
                                let viewrids = str.Select(long.Parse).ToList()
                                where viewrids.FindIndex(i => i == userid) != -1
                                let firstOrDefault = _dbEntities.CmnUsers.FirstOrDefault(j => j.CmnUserId == postproductionissue.Added)
                                select new PostProductionIssueModel
                                {
                                    SwQcAllProjectIssueId = postproductionissue.SwQcAllProjectIssueId,
                                    ProjectName = postproductionissue.ProjectName,
                                    IssueName = postproductionissue.IssueName,
                                    Comment = postproductionissue.Comment,
                                    IssueReproducePath = postproductionissue.IssueReproducePath ?? "",
                                    PurchaseOrderOrdinals = postproductionissue.PurchaseOrderOrdinals,
                                    AddedByName = firstOrDefault != null ? firstOrDefault.UserFullName : "",
                                    AddedDate = postproductionissue.AddedDate,
                                    Frequency = postproductionissue.Frequency,
                                    IssueType = postproductionissue.IssueType,
                                    PurchaseOrderNo = postproductionissue.PurchaseOrderNo,
                                    Upload = postproductionissue.Upload,
                                    ProfilePictureUrl = firstOrDefault.ProfilePictureUrl
                                });
                var file = new FileManager();
                if (issues.Any())
                {
                    foreach (var model in issues)
                    {
                        var filenamestr = model.Upload.Split('|').ToList();
                        var filenamelist = filenamestr.ToList();
                        foreach (string s in filenamelist)
                        {
                            var files = file.GetFile(s);
                            model.UploadedFileGetUrl.Add(files);
                        }
                        model.ProfilePictureUrl = file.GetFile(model.ProfilePictureUrl);
                        if (string.IsNullOrWhiteSpace(model.ProfilePictureUrl))
                            model.ProfilePictureUrl = "../assets/layouts/layout4/img/av.png";
                    }
                }
            }


            return issues;
        }

        public IssueCommentsDetailModel SaveIssuePostComment(PostCommentModel model)
        {
            Mapper.CreateMap<PostCommentModel, PostComment>();
            var postcomment = Mapper.Map<PostComment>(model);
            _dbEntities.PostComments.Add(postcomment);
            _dbEntities.SaveChanges();
            string query = string.Format(@"select top 1 pc.*,cu.UserFullName as CommenterName,ppi.IssueName,cu.ProfilePictureUrl,
                                           ppi.ProjectName,ppi.PurchaseOrderOrdinals from PostComment pc
                                           inner join PostProductionIssues ppi on pc.SwQcAllProjectIssueId=ppi.SwQcAllProjectIssueId
                                           inner join CmnUsers cu on pc.CommentedBy=cu.CmnUserId
                                           where pc.SwQcAllProjectIssueId={0} order by pc.CommentedDate desc", model.SwQcAllProjectIssueId);
            var latestcomment = _dbEntities.Database.SqlQuery<IssueCommentsDetailModel>(query).FirstOrDefault();
            var fileManager = new FileManager();
            latestcomment.ProfilePictureUrl = fileManager.GetFile(latestcomment.ProfilePictureUrl);
            if (string.IsNullOrWhiteSpace(latestcomment.ProfilePictureUrl))
                latestcomment.ProfilePictureUrl = "../assets/layouts/layout4/img/av.png";
            return latestcomment;
        }



        public List<PostCommentModel> GetPostCommentById(long swqcallprojectissueid)
        {
            string query = string.Format(@"select pi.*,cu.UserFullName as CommenterName,cu.ProfilePictureUrl from PostComment pi
                                           inner join CmnUsers cu on pi.CommentedBy=cu.CmnUserId
                                           where SwQcAllProjectIssueId={0} order by pi.CommentedDate desc", swqcallprojectissueid);
            var exe = _dbEntities.Database.SqlQuery<PostCommentModel>(query).ToList();
            var fileManager = new FileManager();
            if (exe.Any())
            {
                foreach (var model in exe)
                {
                    model.ProfilePictureUrl = fileManager.GetFile(model.ProfilePictureUrl);
                    if (string.IsNullOrWhiteSpace(model.ProfilePictureUrl))
                        model.ProfilePictureUrl = "../assets/layouts/layout4/img/av.png";
                }

            }
            return exe;
        }

        public void UpdateCommentForApproval(long swallqcissueId, long postcommentId, long approve)
        {
            if (approve == 1)
            {
                string query =
                    string.Format(
                        @"update PostComment set IsApproved={0},ApprovedBy={1},ApproveDate='{2}' where PostCommentId={3}", approve, HttpContext.Current.User.Identity.Name, DateTime.Now, postcommentId);
                _dbEntities.Database.ExecuteSqlCommand(query);
            }
            else
            {
                string query =
                    string.Format(
                        @"update PostComment set IsApproved={0},ApprovedBy={1},ApproveDate='{2}' where PostCommentId={3}", approve, HttpContext.Current.User.Identity.Name, DateTime.Now, postcommentId);
                _dbEntities.Database.ExecuteSqlCommand(query);
            }
        }

        public List<PostProductionIssueModel> GetPostProductionIssuesByUser(long userId)
        {
            var issues =
                _dbEntities.PostProductionIssues.Where(i => i.Added == userId && i.CurrentStatus != "APPROVED")
                    .Select(i => new PostProductionIssueModel
                    {
                        ProjectName = i.ProjectName,
                        IssueName = i.IssueName,
                        Comment = i.Comment,
                        PurchaseOrderOrdinals = i.PurchaseOrderOrdinals
                    }).ToList();
            return issues;
        }

        private void CreatePostProductionNotification(long userId, string projectName, long? projectMasterId, string purchaseOrderOrdinals, string viewers, int[] orderNumbers)
        {
            var viewerList = new List<string>(viewers.Split(','));
            var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);
            if (user == null) return;



            if (orderNumbers.Any())
            {
                foreach (var number in orderNumbers)
                {
                    var project =
                        _dbEntities.ProjectMasters.FirstOrDefault(
                            i => i.ProjectName == projectName && i.OrderNuber == number);
                    var ordinal = CommonConversion.AddOrdinal(number);
                    var notificationMessage = "A post production issue has been created by " + user.UserFullName +
                                              ".Project Name: " + projectName + ". For " + ordinal + " Purchase Order.";
                    if (project != null)
                    {
                        foreach (var viewer in viewerList)
                        {

                            int viewerId;
                            int.TryParse(viewer, out viewerId);
                            if (viewerId <= 0) continue;
                            var cmnUser = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == viewerId);
                            if (cmnUser == null) continue;
                            var notification = new Notification
                            {
                                Message = notificationMessage,
                                IsViewd = false,
                                ViewerId = viewerId,
                                Role = cmnUser.RoleName,
                                Added = DateTime.Now,
                                AddedBy = userId,
                                ProjectMasterId = project.ProjectMasterId
                            };
                            _dbEntities.Notifications.Add(notification);
                        }
                    }
                }
                _dbEntities.SaveChanges();
                SendEmailForPostProductionIssue(viewerList, projectName, user.UserFullName);
            }

        }

        private void SendEmailForPostProductionIssue(List<string> viewerList, string projectName, string issueCreatorName)
        {
            List<long> viewerIds = viewerList.ConvertAll(Int64.Parse).ToList();
            var usrInfo = "<br/>Issue Created By: " + issueCreatorName;
            string time = "<br/>Created On: " + DateTime.Now.ToLongDateString();
            var mailSend = new MailSendFromPms(); var body =
                     string.Format(
                         @"This is to inform you that, A new post production issue has been created in Walton Project Management System.<br/><br/><b>Project Name: " +
                         projectName + "</b>" + usrInfo + time);
            mailSend.SendMail(viewerIds, new List<long>(), "Post production issue (" + projectName + ")", body);
        }

        private string GetViewerIdDependOnProject(string projectName, IEnumerable<int> orderNumbers)
        {
            string result = string.Empty;
            var roles = new[] { "MM", "CM", "PS", "PMHEAD", "QCHEAD", "HWHEAD" };
            List<int> oN = orderNumbers.ToList();
            if (orderNumbers.Any())
            {
                var userIdList = _dbEntities.CmnUsers.Where(i => roles.Contains(i.RoleName) && i.IsActive).Select(i => i.CmnUserId).ToList();
                foreach (var o in oN)
                {
                    long projectMasterId = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectName == projectName && i.OrderNuber == o).ProjectMasterId;
                    List<long> pmIds = _dbEntities.ProjectPmAssigns.Where(i => i.ProjectMasterId == projectMasterId).Select(i => i.ProjectManagerUserId).ToList();
                    if (pmIds.Any())
                    {
                        userIdList.AddRange(pmIds);
                    }
                    var qcIds = _dbEntities.SwQcAssigns.Where(i => i.ProjectMasterId == projectMasterId).Select(i => i.SwQcAssignId).ToList();
                    if (qcIds.Any())
                    {
                        userIdList.AddRange(qcIds);
                    }
                    var hwQcIds = _dbEntities.HwQcAssigns.Where(i => i.ProjectMasterId == projectMasterId).Select(i => i.HwQcAssignId).ToList();
                    if (hwQcIds.Any())
                    {
                        userIdList.AddRange(hwQcIds);
                    }
                }
                userIdList = userIdList.Distinct().ToList();
                result = string.Join(",", userIdList);
            }
            return result;
        }
        public List<SwStatusObject> GetSwStatusObjects(long id)
        {
            string query =
               string.Format(@"select  sqia.ProjectManagerAssignToQcInTime, sqia.QcInchargeToQcAssignTime,sqia.QcInchargeToPmProjectSubmitTime 
                                from SwQcInchargeAssigns sqia
                                where sqia.ProjectMasterId={0} order by addeddate", id);
            var exe = _dbEntities.Database.SqlQuery<SwStatusObject>(query).ToList();
            return exe;
        }

        private List<CommonStatusObject> GetProjectManagerWorkFlow(long id)
        {
            var list = new List<CommonStatusObject>();
            using (var dbEntities = new CellPhoneProjectEntities())
            {
                var bootImage = dbEntities.PmBootImageAnimations.FirstOrDefault(i => i.ProjectMasterId == id);
                if (bootImage != null)
                {
                    list.Add(new CommonStatusObject
                    {
                        Detail = "Boot Image Uploaded on --" + string.Format("{0:ddd, MMM d, yyyy}", bootImage.AddedDate),
                        ActionDate = bootImage.AddedDate ?? DateTime.MinValue,
                        IsMarge = 0,
                        MargeTo = string.Empty,
                        UserType = "project manager"
                    });
                }
                var gbDesign = dbEntities.PmGiftBoxes.FirstOrDefault(i => i.ProjectMasterId == id);
                if (gbDesign != null)
                {
                    list.Add(new CommonStatusObject
                    {
                        Detail = "Gift Boxes design Uploaded on --" + string.Format("{0:ddd, MMM d, yyyy}", gbDesign.AddedDate),
                        ActionDate = gbDesign.AddedDate ?? DateTime.MinValue,
                        IsMarge = 0,
                        MargeTo = string.Empty,
                        UserType = "project manager"
                    });
                }

                var labelDesign = dbEntities.PmLabels.FirstOrDefault(i => i.ProjectMasterId == id);
                if (labelDesign != null)
                {
                    list.Add(new CommonStatusObject
                    {
                        Detail = "Label design Uploaded on --" + string.Format("{0:ddd, MMM d, yyyy}", labelDesign.AddedDate),
                        ActionDate = labelDesign.AddedDate ?? DateTime.MinValue,
                        IsMarge = 0,
                        MargeTo = string.Empty,
                        UserType = "project manager"
                    });
                }

                var pmId = dbEntities.PmIDs.FirstOrDefault(i => i.ProjectMasterId == id);
                if (pmId != null)
                {
                    list.Add(new CommonStatusObject
                    {
                        Detail = "ID Uploaded on --" + string.Format("{0:ddd, MMM d, yyyy}", pmId.AddedDate),
                        ActionDate = pmId.AddedDate ?? DateTime.MinValue,
                        IsMarge = 0,
                        MargeTo = string.Empty,
                        UserType = "project manager"
                    });
                }

                var screenProt = dbEntities.PmScreenProtectors.FirstOrDefault(i => i.ProjectMasterId == id);
                if (screenProt != null)
                {
                    list.Add(new CommonStatusObject
                    {
                        Detail = "Screen Protector Uploaded on --" + string.Format("{0:ddd, MMM d, yyyy}", pmId.AddedDate),
                        ActionDate = pmId.AddedDate ?? DateTime.MinValue,
                        IsMarge = 0,
                        MargeTo = string.Empty,
                        UserType = "project manager"
                    });
                }

                var walpaper = dbEntities.PmWalpapers.FirstOrDefault(i => i.ProjectMasterId == id);
                if (walpaper != null)
                {
                    list.Add(new CommonStatusObject
                    {
                        Detail = "Wallpaper Uploaded on --" + string.Format("{0:ddd, MMM d, yyyy}", walpaper.AddedDate),
                        ActionDate = walpaper.AddedDate ?? DateTime.MinValue,
                        IsMarge = 0,
                        MargeTo = string.Empty,
                        UserType = "project manager"
                    });
                }

                var sCustomization = dbEntities.PmSwCustomizationFinals.Where(i => i.ProjectMasterId == id).ToList();
                if (sCustomization.Any())
                {
                    var first = sCustomization.FirstOrDefault();
                    if (first != null)
                        list.Add(new CommonStatusObject
                        {
                            Detail = "Software customization data entry on --" + string.Format("{0:ddd, MMM d, yyyy}", first.AddedDate),
                            ActionDate = first.AddedDate ?? DateTime.MinValue,
                            IsMarge = 0,
                            MargeTo = string.Empty,
                            UserType = "project manager"
                        });
                }

                var accessories = dbEntities.PmPhnAccessories.FirstOrDefault(i => i.ProjectMasterId == id);
                if (accessories != null)
                {
                    list.Add(new CommonStatusObject
                    {
                        Detail = "Accessories data input on --" + string.Format("{0:ddd, MMM d, yyyy}", accessories.AddedDate),
                        ActionDate = accessories.AddedDate ?? DateTime.MinValue,
                        IsMarge = 0,
                        MargeTo = string.Empty,
                        UserType = "project manager"
                    });
                }

                var cam = dbEntities.PmPhnCameras.FirstOrDefault(i => i.ProjectMasterId == id);
                if (cam != null)
                {
                    list.Add(new CommonStatusObject
                    {
                        Detail = "Camera Information input on --" + string.Format("{0:ddd, MMM d, yyyy}", cam.AddedDate),
                        ActionDate = cam.AddedDate ?? DateTime.MinValue,
                        IsMarge = 0,
                        MargeTo = string.Empty,
                        UserType = "project manager"
                    });
                }
            }
            return list;
        }

        private List<CommonStatusObject> GetSowftwareWorkFlow(SwQcInchargeAssign softwareAssign)
        {

            var list = new List<CommonStatusObject>();
            using (var dbEntities = new CellPhoneProjectEntities())
            {
                DateTime projectAssignDate = DateTime.MinValue;
                if (softwareAssign.Status != "NEW")
                {
                    if (softwareAssign.PausedDate != null)
                    {
                        list.Add(new CommonStatusObject
                        {
                            Detail = "Project has been paused by incharge on --" + string.Format(@"{0:ddd, MMM d, yyyy}", softwareAssign.PausedDate),
                            ActionDate = softwareAssign.PausedDate,
                            IsMarge = 0,
                            MargeTo = string.Empty,
                            UserType = "software"
                        });
                    }
                    if (softwareAssign.RestartDate != null)
                    {
                        list.Add(new CommonStatusObject
                        {
                            Detail = "Project has been restarted by incharge on --" + string.Format(@"{0:ddd, MMM d, yyyy}", softwareAssign.PausedDate),
                            ActionDate = softwareAssign.PausedDate,
                            IsMarge = 0,
                            MargeTo = string.Empty,
                            UserType = "software"
                        });
                    }
                }
                if (softwareAssign.Status == "ASSIGNED" || softwareAssign.Status == "QCCOMPLETED")
                {
                    var assigns = dbEntities.SwQcAssigns.Where(i => i.SwQcInchargeAssignId == softwareAssign.SwQcInchargeAssignId && i.Status != "INACTIVE").ToList();
                    if (assigns.All(i => i.Status == "QCCOMPLETED"))
                    {
                        list.Add(new CommonStatusObject
                        {
                            Detail = "Project has been assigned for  on --" + string.Format(@"{0:ddd, MMM d, yyyy}", softwareAssign.QcInchargeToQcAssignTime),
                            ActionDate = softwareAssign.QcInchargeToQcAssignTime,
                            IsMarge = 0,
                            MargeTo = string.Empty,
                            UserType = "software"
                        });
                        list.Add(new CommonStatusObject
                        {
                            Detail = "Software QC completed on --" + string.Format(@"{0:ddd, MMM d, yyyy}", softwareAssign.QcProjectFinisedTime),
                            ActionDate = softwareAssign.QcProjectFinisedTime,
                            IsMarge = 0,
                            MargeTo = string.Empty,
                            UserType = "software"
                        });
                        list.Add(new CommonStatusObject
                        {
                            Detail = "Forward to Project manager pending for " + Math.Floor((DateTime.Now - (DateTime)softwareAssign.QcProjectFinisedTime).TotalDays) + " days",
                            ActionDate = softwareAssign.QcProjectFinisedTime,
                            IsMarge = 0,
                            MargeTo = string.Empty,
                            UserType = "software"
                        });
                    }
                    else
                    {
                        list.Add(new CommonStatusObject
                        {
                            Detail = "Project has been assigned for  on --" + string.Format(@"{0:ddd, MMM d, yyyy}", softwareAssign.QcInchargeToQcAssignTime),
                            ActionDate = softwareAssign.QcInchargeToQcAssignTime,
                            IsMarge = 0,
                            MargeTo = string.Empty,
                            UserType = "software"
                        });
                        var startDate = assigns.OrderByDescending(i => i.AddedDate).FirstOrDefault().AddedDate;

                        list.Add(new CommonStatusObject
                        {
                            Detail = "Software QC is running..",
                            ActionDate = startDate,
                            IsMarge = 0,
                            MargeTo = string.Empty,
                            UserType = "software"
                        });
                    }
                }

                if (softwareAssign.Status == "RECOMMENDED")
                {
                    list.Add(new CommonStatusObject
                    {
                        Detail = "Project has been assigned for  on --" + string.Format(@"{0:ddd, MMM d, yyyy}", softwareAssign.QcInchargeToQcAssignTime),
                        ActionDate = softwareAssign.QcInchargeToQcAssignTime,
                        IsMarge = 0,
                        MargeTo = string.Empty,
                        UserType = "software"
                    });
                    list.Add(new CommonStatusObject
                    {
                        Detail = "Software QC completed on --" + string.Format(@"{0:ddd, MMM d, yyyy}", softwareAssign.QcProjectFinisedTime),
                        ActionDate = softwareAssign.QcProjectFinisedTime,
                        IsMarge = 0,
                        MargeTo = string.Empty,
                        UserType = "software"
                    });
                    list.Add(new CommonStatusObject
                    {
                        Detail = "Software testing report forwarded to project manager on--" + string.Format(@"{0:ddd, MMM d, yyyy}", softwareAssign.QcInchargeToPmProjectSubmitTime),
                        ActionDate = softwareAssign.QcInchargeToPmProjectSubmitTime,
                        IsMarge = 1,
                        MargeTo = "proejctmanager",
                        UserType = "software"
                    });
                }


            }

            return list;
        }

        private List<CommonStatusObject> GetHardwareWorkFlow(HwQcInchargeAssign hwQcInchargeAssign)
        {
            var list = new List<CommonStatusObject>();
            using (var dbEntities = new CellPhoneProjectEntities())
            {
                var hwAssign = dbEntities.HwQcAssigns.FirstOrDefault(i => i.HwQcInchargeAssignId == hwQcInchargeAssign.HwQcInchargeAssignId);
                if (hwAssign != null)
                {
                    string dayDifference = hwAssign.UpdatedDate == null ? "undefined" : Math.Floor((DateTime.Now - (DateTime)hwAssign.UpdatedDate).TotalDays).ToString(CultureInfo.InvariantCulture) + " days";
                    list.Add(new CommonStatusObject
                    {
                        Detail = "Assigned for running test on --" + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.HwQcAssignDate),
                        ActionDate = hwAssign.HwQcAssignDate,
                        IsMarge = 0,
                        MargeTo = string.Empty,
                        UserType = "hardware"
                    });

                    if (hwAssign.Status == "NEW")
                    {
                        list.Add(new CommonStatusObject
                        {
                            Detail = "Running test is not started yet. Pending for " + Math.Floor((DateTime.Now - hwAssign.HwQcAssignDate).TotalDays) + " days",
                            ActionDate = hwAssign.HwQcAssignDate,
                            IsMarge = 0,
                            MargeTo = string.Empty,
                            UserType = "hardware"
                        });
                    }
                    else if (hwAssign.Status == "RUNNING")
                    {
                        list.Add(new CommonStatusObject
                        {
                            Detail = "Running test started on --" + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.UpdatedDate) ?? "undefined",
                            ActionDate = hwAssign.UpdatedDate ?? DateTime.MinValue,
                            IsMarge = 0,
                            MargeTo = string.Empty,
                            UserType = "hardware"
                        });
                    }
                    else if (hwAssign.Status == "QCSUBMITTED")
                    {

                        list.Add(new CommonStatusObject
                        {
                            Detail = "Running started on --" + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.UpdatedDate) ?? "undefined",
                            ActionDate = hwAssign.UpdatedDate ?? DateTime.MinValue,
                            IsMarge = 0,
                            MargeTo = string.Empty,
                            UserType = "hardware"
                        });
                        list.Add(new CommonStatusObject
                        {
                            Detail = "Running test completed on " + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.UpdatedDate) ?? "undefined",
                            ActionDate = hwAssign.UpdatedDate ?? DateTime.MinValue,
                            IsMarge = 0,
                            MargeTo = string.Empty,
                            UserType = "hardware"
                        });

                        list.Add(new CommonStatusObject
                        {
                            Detail = "Running test verificatin pending by hardware engineer for " + dayDifference,
                            ActionDate = hwAssign.UpdatedDate ?? DateTime.MinValue,
                            IsMarge = 0,
                            MargeTo = string.Empty,
                            UserType = "hardware"
                        });
                    }
                    else if (hwAssign.Status == "QCPASSED")
                    {

                        list.Add(new CommonStatusObject
                        {
                            Detail = "Running started on --" + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.UpdatedDate) ?? "undefined",
                            ActionDate = hwAssign.UpdatedDate ?? DateTime.MinValue,
                            IsMarge = 0,
                            MargeTo = string.Empty,
                            UserType = "hardware"
                        });
                        list.Add(new CommonStatusObject
                        {
                            Detail = "Running test completed on " + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.UpdatedDate) ?? "undefined",
                            ActionDate = hwAssign.UpdatedDate ?? DateTime.MinValue,
                            IsMarge = 0,
                            MargeTo = string.Empty,
                            UserType = "hardware"
                        });

                        list.Add(new CommonStatusObject
                        {
                            Detail = "Screening test verified by one of hardware engineer on --" + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.UpdatedDate) ?? "undefined",
                            ActionDate = hwAssign.UpdatedDate ?? DateTime.MinValue,
                            IsMarge = 0,
                            MargeTo = string.Empty,
                            UserType = "hardware"
                        });
                        list.Add(new CommonStatusObject
                        {
                            Detail = "Running test result forward pending for " + dayDifference,
                            ActionDate = hwAssign.UpdatedDate ?? DateTime.MinValue,
                            IsMarge = 0,
                            MargeTo = string.Empty,
                            UserType = "hardware"
                        });
                    }
                    else if (hwAssign.Status == "FORWARDED")
                    {
                        list.Add(new CommonStatusObject
                        {
                            Detail = "Running started on --" + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.UpdatedDate) ?? "undefined",
                            ActionDate = hwAssign.UpdatedDate ?? DateTime.MinValue,
                            IsMarge = 0,
                            MargeTo = string.Empty,
                            UserType = "hardware"
                        });
                        list.Add(new CommonStatusObject
                        {
                            Detail = "Running test completed on " + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.UpdatedDate) ?? "undefined",
                            ActionDate = hwAssign.UpdatedDate ?? DateTime.MinValue,
                            IsMarge = 0,
                            MargeTo = string.Empty,
                            UserType = "hardware"
                        });

                        list.Add(new CommonStatusObject
                        {
                            Detail = "Screening test verified by one of hardware engineer on --" + string.Format("{0:ddd, MMM d, yyyy}", hwAssign.UpdatedDate) ?? "undefined",
                            ActionDate = hwAssign.UpdatedDate ?? DateTime.MinValue,
                            IsMarge = 0,
                            MargeTo = string.Empty,
                            UserType = "hardware"
                        });

                        list.Add(new CommonStatusObject
                        {
                            Detail = "Running test forwarded to project manager on --" + string.Format("{0:ddd, MMM d, yyyy}", hwQcInchargeAssign.ForwardDate) ?? "undefined",
                            ActionDate = hwAssign.UpdatedDate ?? DateTime.MinValue,
                            IsMarge = 1,
                            MargeTo = "projectmanager",
                            UserType = "hardware"
                        });
                        //status.HardwareList.Add(new DataObject { Detail = "screeningFinished" });
                    }

                }
            }

            return list;
        }

        private List<CommonStatusObject> GetCommercialWorkFlow(long id)
        {
            var list = new List<CommonStatusObject>();
            using (var dbEntities = new CellPhoneProjectEntities())
            {
                var ccp = dbEntities.ProjectCriticalControlPoints.FirstOrDefault(i => i.ProjectMasterId == id);
                if (ccp != null)
                {
                    list.Add(new CommonStatusObject
                    {
                        ActionDate = ccp.AddedDate ?? DateTime.MinValue,
                        Detail = "CCP created on --" + string.Format("{0:ddd, MMM d, yyyy}", ccp.AddedDate),
                        IsMarge = 0,
                        MargeTo = string.Empty,
                        UserType = "commercial"
                    });
                }

                var pi = dbEntities.ProjectProformaInvoices.FirstOrDefault(i => i.ProjectMasterId == id);
                if (pi != null)
                {
                    list.Add(new CommonStatusObject
                    {
                        ActionDate = pi.AddedDate ?? DateTime.MinValue,
                        Detail = "PI created on --" + string.Format("{0:ddd, MMM d, yyyy}", pi.AddedDate),
                        IsMarge = 0,
                        MargeTo = string.Empty,
                        UserType = "commercial"
                    });
                }

                var lc = dbEntities.ProjectLcs.FirstOrDefault(i => i.ProjectMasterId == id);
                if (lc != null)
                {
                    list.Add(new CommonStatusObject
                    {
                        ActionDate = lc.AddedDate ?? DateTime.MinValue,
                        Detail = "LC created on --" + string.Format("{0:ddd, MMM d, yyyy}", lc.AddedDate),
                        IsMarge = 0,
                        MargeTo = string.Empty,
                        UserType = "commercial"
                    });
                }
                var shipments = dbEntities.ProjectOrderShipments.Where(i => i.ProjectMasterId == id).ToList();
                if (shipments.Any())
                {
                    int no = 0;
                    foreach (var shipment in shipments)
                    {
                        no = no + 1;
                        list.Add(new CommonStatusObject
                        {
                            ActionDate = shipment.AddedDate ?? DateTime.MinValue,
                            Detail = no + " Shipment created on --" + string.Format("{0:ddd, MMM d, yyyy}", shipment.AddedDate),
                            IsMarge = 0,
                            MargeTo = string.Empty,
                            UserType = "commercial"
                        });
                    }
                }

            }
            return list;
        }

        public List<MajorIssueModel> GetModelNamesHavingIssues()
        {
            List<MajorIssueModel> models =
                (from majorIssue in _dbEntities.MajorIssues
                 group majorIssue by majorIssue.ModelName into m
                 select new MajorIssueModel
                 {
                     ModelName = m.Key
                 }).ToList();
            return models;
        }

        public List<DiagnosticCodeFromOracleModel> GetDiagnosticCodeFromOracleModels()
        {
            List<DiagnosticCodeFromOracleModel> models = (from diag in _dbEntities.DiagnosticCodeFromOracles
                                                          select new DiagnosticCodeFromOracleModel
                                                          {
                                                              DiagnosticCodeId = diag.DiagnosticCodeId,
                                                              IssueId = diag.IssueId,
                                                              DiagonsticCodeName = diag.DiagonsticCodeName
                                                          }).ToList();
            return models;
        }

        public List<HwTestMasterModel> GetHwTestMasterModels(string addedByRole)
        {
            var master = (from m in _dbEntities.HwTestMasters
                          select new HwTestMasterModel
                          {
                              HwTestMasterId = m.HwTestMasterId,
                              HwTestName = m.HwTestName,
                              AddedBy = m.AddedBy,
                              AddedDate = m.AddedDate
                          }).ToList();
            return master;
        }

        public HwTestMasterModel SaveHwTestMaster(HwTestMasterModel model)
        {
            Mapper.CreateMap<HwTestMasterModel, HwTestMaster>();
            var m = Mapper.Map<HwTestMaster>(model);
            _dbEntities.HwTestMasters.Add(m);
            _dbEntities.SaveChanges();
            model.HwTestMasterId = m.HwTestMasterId;
            return model;
        }

        public List<HwTestInchargeAssignModel> GetHwTestInchargeAssignModels()
        {
            var assigned = (from m in _dbEntities.HwTestInchargeAssigns
                            select new HwTestInchargeAssignModel
                                {
                                    HwTestInchargeAssignId = m.HwTestInchargeAssignId,
                                    HwTestMasterId = m.HwTestMasterId,
                                    HwTestName = m.HwTestName,
                                    ProjectMasterId = m.ProjectMasterId,
                                    ProjectName = m.ProjectName,
                                    AddedBy = m.AddedBy,
                                    Remarks = m.Remarks,
                                    AddedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                                    AddedDate = m.AddedDate,
                                    Status = m.Status,
                                    ForwardedBy = m.ForwardedBy,
                                    ForwrdedDate = m.ForwrdedDate
                                }).ToList();
            return assigned;
        }

        public List<HwTestInchargeAssignModel> GetHwEngineerAssignModels()
        {
            var assignPending = (from m in _dbEntities.HwTestInchargeAssigns
                                 select new HwTestInchargeAssignModel
                                 {
                                     HwTestInchargeAssignId = m.HwTestInchargeAssignId,
                                     HwTestMasterId = m.HwTestMasterId,
                                     HwTestName = m.HwTestName,
                                     ProjectMasterId = m.ProjectMasterId,
                                     ProjectName = m.ProjectName,
                                     AddedBy = m.AddedBy,
                                     Remarks = m.Remarks,
                                     AddedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                                     AddedDate = m.AddedDate,
                                     Status = m.Status,
                                     ForwardedBy = m.ForwardedBy,
                                     ForwrdedDate = m.ForwrdedDate,
                                     ForwardRemarks = m.ForwardRemarks,
                                     Serial = m.Serial
                                 }).ToList();
            return assignPending;
        }

        public List<VmHwTestDetail> GetHwTestDetail()
        {
            var vm = new List<VmHwTestDetail>();
            var v = (from x in _dbEntities.HwTestInchargeAssigns
                     join y in _dbEntities.HwEngineerAssigns on x.HwTestInchargeAssignId equals y.HwTestInchargeAssignId
                     into xy
                     from suby in xy.DefaultIfEmpty()
                     select new { x, y = suby }).ToList();
            foreach (var z in v)
            {
                var vmn = new VmHwTestDetail
                {
                    HwTestInchargeAssignModel = new HwTestInchargeAssignModel
                    {
                        HwTestInchargeAssignId = z.x.HwTestInchargeAssignId,
                        ProjectMasterId = z.x.ProjectMasterId,
                        ProjectName = z.x.ProjectName,
                        HwTestMasterId = z.x.HwTestMasterId,
                        HwTestName = z.x.HwTestName,
                        AddedBy = z.x.AddedBy,
                        AddedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == z.x.AddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                        AddedDate = z.x.AddedDate,
                        Remarks = z.x.Remarks,
                        Status = z.x.Status,
                        ForwardedBy = z.x.ForwardedBy,
                        ForwrdedDate = z.x.ForwrdedDate,
                        ForwardRemarks = z.x.ForwardRemarks,
                        Serial = z.x.Serial
                    },
                    HwEngineerAssignModel = new HwEngineerAssignModel
                    {
                        HwEngineerAssignId = z.y != null ? z.y.HwEngineerAssignId : 0,
                        HwEngineerNames = z.y != null ? z.y.HwEngineerNames : "-",
                        HwInchargeRemark = z.y != null ? z.y.HwInchargeRemark : "-",
                        Remark = z.y != null ? z.y.Remark : "-",
                        Result = z.y != null ? z.y.Result : "-",
                        AddedBy = z.y != null ? z.y.AddedBy : 0,
                        AddedByName = z.y != null ? _dbEntities.CmnUsers.Where(x => x.CmnUserId == z.y.AddedBy).Select(x => x.UserFullName).FirstOrDefault() : "-",
                        AddedDate = z.y != null ? z.y.AddedDate : DateTime.MinValue,
                        SubmittedBy = z.y != null ? z.y.SubmittedBy : 0,
                        SubmittedDate = z.y != null ? z.y.SubmittedDate : DateTime.MinValue,
                        UpdatedBy = z.y != null ? z.y.UpdatedBy : 0,
                        UpdatedDate = z.y != null ? z.y.UpdatedDate : DateTime.MinValue,
                        Status = z.y != null ? z.y.Status : "-"
                    }
                };
                vm.Add(vmn);
            }

            return vm;
        }

        public List<CmnUserModel> GetAllEmployee()
        {
            var model = (from m in _dbEntities.CmnUsers
                         where m.IsActive
                         select new CmnUserModel
                         {
                             CmnUserId = m.CmnUserId,
                             UserName = m.UserName,
                             UserFullName = m.UserFullName
                         }).ToList();
            foreach (var m in model)
            {
                m.UserFullName = m.UserFullName + " (" + m.UserName + ")";
            }
            return model;
        }

        public List<ProjectMasterModel> GetOnlyModelName()
        {
            var model = (from m in _dbEntities.ProjectMasters
                         group m by m.ProjectName into p
                         select new ProjectMasterModel
                         {
                             ProjectName = p.Key
                         }).ToList();
            return model;
        }

        public List<MkProjectSpecModel> GetSpec(string specname, string type)
        {
            var model = new List<MkProjectSpecModel>();
            if (type == "brand")
            {
                model = (from v in _dbEntities.MkProjectSpecs
                         where v.Brand.StartsWith(specname)
                         group v by v.Brand into y
                         select new MkProjectSpecModel
                         {
                             Brand = y.Key
                         }).ToList();
            }
            if (type == "model")
            {
                model = (from v in _dbEntities.MkProjectSpecs
                         where v.ModelName.StartsWith(specname)
                         group v by v.ModelName into y
                         select new MkProjectSpecModel
                         {
                             ModelName = y.Key
                         }).ToList();
            }
            return model;
        }

        #region SAMPLE TRACKER

        public void SaveSampleTracker(SampleTrackerModel model)
        {
            Mapper.CreateMap<SampleTrackerModel, SampleTracker>();
            var m = Mapper.Map<SampleTracker>(model);
            _dbEntities.SampleTrackers.Add(m);
            _dbEntities.SaveChanges();
        }

        public List<SampleTrackerModel> GetSampleTrackingByAddedId(long id)
        {
            var model = (from m in _dbEntities.SampleTrackers
                         where m.AddedBy == id //&& m.InventoryReturnedBy==null
                         select new SampleTrackerModel
                         {
                             SampleTrackerId = m.SampleTrackerId,
                             ProjectMasterId = m.ProjectMasterId,
                             Model = m.Model,
                             Role = m.SampleSentToDept,
                             RoleisHead = _dbEntities.CmnRoles.Where(x => x.RoleName == m.SampleSentToDept).Select(x => x.IsHead).FirstOrDefault(),
                             SampleSentToDept = _dbEntities.CmnRoles.Where(x => x.RoleName == m.SampleSentToDept).Select(x => x.RoleDescription).FirstOrDefault(),
                             SampleSentToPersonId = m.SampleSentToPersonId,
                             SampleSentToPersonName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.SampleSentToPersonId).Select(x => x.UserFullName).FirstOrDefault(),
                             SampleCategory = m.SampleCategory,
                             IMEI = m.IMEI,
                             Color = m.Color,
                             Remarks = m.Remarks,
                             AddedBy = m.AddedBy,
                             AddedDate = m.AddedDate,
                             UpdatedBy = m.UpdatedBy,
                             UpdatedDate = m.UpdatedDate,
                             ReceiveDate = m.ReceiveDate,
                             ReceivedBy = m.ReceivedBy,
                             ReceivedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.ReceivedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             ReturnStatus = m.ReturnStatus,
                             ReturnedBy = m.ReturnedBy,
                             ReturnedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.ReturnedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             ReturnDate = m.ReturnDate,
                             Purpose = m.Purpose,
                             SupplierName = m.SupplierName,
                             NumberOfSample = m.NumberOfSample,
                             Others = m.Others,
                             AdditionalInfo = m.AdditionalInfo,
                             ReturnQuantity = m.ReturnQuantity,
                             AddedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             SampleIssueDate = m.SampleIssueDate,
                             SampleIssuePurpose = m.SampleIssuePurpose,
                             SampleIssueQuantity = m.SampleIssueQuantity,
                             SampleIssuedBy = m.SampleIssuedBy,
                             SampleIssuedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.SampleIssuedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             InventoryReturnQuantity = m.InventoryReturnQuantity,
                             InventoryReturnDate = m.InventoryReturnDate,
                             InventoryReturnedBy = m.InventoryReturnedBy,
                             InventoryReturnedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.InventoryReturnedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             InventoryReturnRemarks = m.InventoryReturnRemarks,
                             InventoryReceiveDate = m.InventoryReceiveDate,
                             InventoryReceivedBy = m.InventoryReceivedBy,
                             InventoryReceivedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.InventoryReceivedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             InventoryReceiveRemarks = m.InventoryReceiveRemarks,

                         }).ToList();
            return model;
        }

        public List<SampleTrackerModel> GetSampleTrackingByAddedIdAndSentIdAndDateRange(long id, DateTime fromDate, DateTime toDate)
        {
            toDate = Convert.ToDateTime(Convert.ToString(toDate.ToShortDateString()) + " 23:59:59.9999999");
            var model = (from m in _dbEntities.SampleTrackers
                         where (m.AddedBy == id || m.SampleSentToPersonId == id) && m.AddedDate >= fromDate && m.AddedDate <= toDate
                         select new SampleTrackerModel
                         {
                             SampleTrackerId = m.SampleTrackerId,
                             ProjectMasterId = m.ProjectMasterId,
                             Model = m.Model,
                             //Role = m.SampleSentToDept,
                             RoleisHead = _dbEntities.CmnRoles.Where(x => x.RoleName == m.SampleSentToDept).Select(x => x.IsHead).FirstOrDefault(),
                             SampleSentToPersonId = m.SampleSentToPersonId,
                             SampleSentToDept = _dbEntities.CmnRoles.Where(x => x.RoleName == _dbEntities.CmnUsers.Where(z => z.CmnUserId == m.SampleSentToPersonId).Select(z => z.RoleName).FirstOrDefault()).Select(x => x.RoleDescription).FirstOrDefault(),
                             SampleSentToPersonName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.SampleSentToPersonId).Select(x => x.UserFullName).FirstOrDefault() + "(" + _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.SampleSentToPersonId).Select(x => x.UserName).FirstOrDefault() + ")",
                             SampleCategory = m.SampleCategory,
                             IMEI = m.IMEI,
                             Color = m.Color,
                             Remarks = m.Remarks,
                             AddedBy = m.AddedBy,
                             AddedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy).Select(x => x.UserFullName).FirstOrDefault() + "(" + _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy).Select(x => x.UserName).FirstOrDefault() + ")",
                             AddedByDept = _dbEntities.CmnRoles.Where(x => x.RoleName == _dbEntities.CmnUsers.Where(z => z.CmnUserId == m.AddedBy).Select(z => z.RoleName).FirstOrDefault()).Select(x => x.RoleDescription).FirstOrDefault(),
                             AddedDate = m.AddedDate,
                             UpdatedBy = m.UpdatedBy,
                             UpdatedDate = m.UpdatedDate,
                             ReceiveDate = m.ReceiveDate,
                             ReceivedBy = m.ReceivedBy,
                             ReceivedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.ReceivedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             ReturnStatus = m.ReturnStatus,
                             ReturnedBy = m.ReturnedBy,
                             ReturnedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.ReturnedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             ReturnDate = m.ReturnDate,
                             Purpose = m.Purpose,
                             SupplierName = m.SupplierName,
                             NumberOfSample = m.NumberOfSample,
                             Others = m.Others,
                             AdditionalInfo = m.AdditionalInfo,
                             ReturnQuantity = m.ReturnQuantity,
                             SampleIssueDate = m.SampleIssueDate,
                             SampleIssuePurpose = m.SampleIssuePurpose,
                             SampleIssueQuantity = m.SampleIssueQuantity,
                             SampleIssuedBy = m.SampleIssuedBy,
                             SampleIssuedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.SampleIssuedBy).Select(x => x.UserFullName).FirstOrDefault() + "(" + _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.SampleIssuedBy).Select(x => x.UserName).FirstOrDefault() + ")",
                             SampleIssuedByDept = _dbEntities.CmnRoles.Where(x => x.RoleName == _dbEntities.CmnUsers.Where(z => z.CmnUserId == m.SampleIssuedBy).Select(z => z.RoleName).FirstOrDefault()).Select(x => x.RoleDescription).FirstOrDefault(),
                             InventoryReturnQuantity = m.InventoryReturnQuantity,
                             InventoryReturnDate = m.InventoryReturnDate,
                             InventoryReturnedBy = m.InventoryReturnedBy,
                             InventoryReturnedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.InventoryReturnedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             InventoryReturnRemarks = m.InventoryReturnRemarks,
                             InventoryReceiveDate = m.InventoryReceiveDate,
                             InventoryReceivedBy = m.InventoryReceivedBy,
                             InventoryReceivedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.InventoryReceivedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             InventoryReceiveRemarks = m.InventoryReceiveRemarks,

                         }).ToList();
            return model;
        }

        public List<SampleTrackerModel> GetSampleTrackingByEmployeeId(long id)
        {
            var model = (from m in _dbEntities.SampleTrackers
                         where m.SampleSentToPersonId == id
                         select new SampleTrackerModel
                         {
                             SampleTrackerId = m.SampleTrackerId,
                             ProjectMasterId = m.ProjectMasterId,
                             Model = m.Model,
                             Role = m.SampleSentToDept,
                             RoleisHead = _dbEntities.CmnRoles.Where(x => x.RoleName == m.SampleSentToDept).Select(x => x.IsHead).FirstOrDefault(),
                             SampleSentToDept = _dbEntities.CmnRoles.Where(x => x.RoleName == m.SampleSentToDept).Select(x => x.RoleDescription).FirstOrDefault(),
                             SampleSentToPersonId = m.SampleSentToPersonId,
                             SampleSentToPersonName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.SampleSentToPersonId).Select(x => x.UserFullName).FirstOrDefault(),
                             SampleCategory = m.SampleCategory,
                             IMEI = m.IMEI,
                             Color = m.Color,
                             Remarks = m.Remarks,
                             AddedBy = m.AddedBy,
                             AddedDate = m.AddedDate,
                             UpdatedBy = m.UpdatedBy,
                             UpdatedDate = m.UpdatedDate,
                             ReceiveDate = m.ReceiveDate,
                             ReceivedBy = m.ReceivedBy,
                             ReceivedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.ReceivedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             ReturnStatus = m.ReturnStatus,
                             ReturnedBy = m.ReturnedBy,
                             ReturnedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.ReturnedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             ReturnDate = m.ReturnDate,
                             Purpose = m.Purpose,
                             SupplierName = m.SupplierName,
                             NumberOfSample = m.NumberOfSample,
                             Others = m.Others,
                             AdditionalInfo = m.AdditionalInfo,
                             ReturnQuantity = m.ReturnQuantity,
                             AddedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             SampleIssueDate = m.SampleIssueDate,
                             SampleIssuePurpose = m.SampleIssuePurpose,
                             SampleIssueQuantity = m.SampleIssueQuantity,
                             SampleIssuedBy = m.SampleIssuedBy,
                             SampleIssuedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.SampleIssuedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             InventoryReturnQuantity = m.InventoryReturnQuantity,
                             InventoryReturnDate = m.InventoryReturnDate,
                             InventoryReturnedBy = m.InventoryReturnedBy,
                             InventoryReturnedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.InventoryReturnedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             InventoryReturnRemarks = m.InventoryReturnRemarks,
                             InventoryReceiveDate = m.InventoryReceiveDate,
                             InventoryReceivedBy = m.InventoryReceivedBy,
                             InventoryReceiveRemarks = m.InventoryReceiveRemarks,

                         }).ToList();
            return model;
        }

        public List<SampleTrackerModel> GetSampleTrackingByRole(string role)
        {
            var model = (from m in _dbEntities.SampleTrackers
                         where m.SampleSentToDept == role
                         select new SampleTrackerModel
                         {
                             SampleTrackerId = m.SampleTrackerId,
                             ProjectMasterId = m.ProjectMasterId,
                             Model = m.Model,
                             Role = m.SampleSentToDept,
                             RoleisHead = _dbEntities.CmnRoles.Where(x => x.RoleName == m.SampleSentToDept).Select(x => x.IsHead).FirstOrDefault(),
                             SampleSentToDept = _dbEntities.CmnRoles.Where(x => x.RoleName == m.SampleSentToDept).Select(x => x.RoleDescription).FirstOrDefault(),
                             SampleSentToPersonId = m.SampleSentToPersonId,
                             SampleSentToPersonName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.SampleSentToPersonId).Select(x => x.UserFullName).FirstOrDefault(),
                             SampleCategory = m.SampleCategory,
                             IMEI = m.IMEI,
                             Color = m.Color,
                             Remarks = m.Remarks,
                             AddedBy = m.AddedBy,
                             AddedDate = m.AddedDate,
                             UpdatedBy = m.UpdatedBy,
                             UpdatedDate = m.UpdatedDate,
                             ReceiveDate = m.ReceiveDate,
                             ReceivedBy = m.ReceivedBy,
                             ReceivedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.ReceivedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             ReturnStatus = m.ReturnStatus,
                             ReturnedBy = m.ReturnedBy,
                             ReturnedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.ReturnedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             ReturnDate = m.ReturnDate,
                             Purpose = m.Purpose,
                             SupplierName = m.SupplierName,
                             NumberOfSample = m.NumberOfSample,
                             Others = m.Others,
                             AdditionalInfo = m.AdditionalInfo,
                             ReturnQuantity = m.ReturnQuantity,
                             AddedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             SampleIssueDate = m.SampleIssueDate,
                             SampleIssuePurpose = m.SampleIssuePurpose,
                             SampleIssueQuantity = m.SampleIssueQuantity,
                             SampleIssuedBy = m.SampleIssuedBy,
                             SampleIssuedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.SampleIssuedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             InventoryReturnQuantity = m.InventoryReturnQuantity,
                             InventoryReturnDate = m.InventoryReturnDate,
                             InventoryReturnedBy = m.InventoryReturnedBy,
                             InventoryReturnedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.InventoryReturnedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             InventoryReturnRemarks = m.InventoryReturnRemarks,
                             InventoryReceiveDate = m.InventoryReceiveDate,
                             InventoryReceivedBy = m.InventoryReceivedBy,
                             InventoryReceiveRemarks = m.InventoryReceiveRemarks,
                         }).ToList();
            return model;
        }

        public SampleTrackerModel GetSampleTrackerById(long id)
        {
            var model = (from m in _dbEntities.SampleTrackers
                         where m.SampleTrackerId == id
                         select new SampleTrackerModel
                         {
                             SampleTrackerId = m.SampleTrackerId,
                             ProjectMasterId = m.ProjectMasterId,
                             Model = m.Model,
                             Role = m.SampleSentToDept,
                             RoleisHead = _dbEntities.CmnRoles.Where(x => x.RoleName == m.SampleSentToDept).Select(x => x.IsHead).FirstOrDefault(),
                             SampleSentToDept = _dbEntities.CmnRoles.Where(x => x.RoleName == m.SampleSentToDept).Select(x => x.RoleDescription).FirstOrDefault(),
                             SampleSentToPersonId = m.SampleSentToPersonId,
                             SampleSentToPersonName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.SampleSentToPersonId).Select(x => x.UserFullName).FirstOrDefault(),
                             SampleCategory = m.SampleCategory,
                             IMEI = m.IMEI,
                             Color = m.Color,
                             Remarks = m.Remarks,
                             AddedBy = m.AddedBy,
                             AddedDate = m.AddedDate,
                             UpdatedBy = m.UpdatedBy,
                             UpdatedDate = m.UpdatedDate,
                             ReceiveDate = m.ReceiveDate,
                             ReceivedBy = m.ReceivedBy,
                             ReceivedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.ReceivedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             ReturnStatus = m.ReturnStatus,
                             ReturnedBy = m.ReturnedBy,
                             ReturnedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.ReturnedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             ReturnDate = m.ReturnDate,
                             Purpose = m.Purpose,
                             SupplierName = m.SupplierName,
                             NumberOfSample = m.NumberOfSample,
                             Others = m.Others,
                             AdditionalInfo = m.AdditionalInfo,
                             ReturnQuantity = m.ReturnQuantity,
                             AddedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             SampleIssueDate = m.SampleIssueDate,
                             SampleIssuePurpose = m.SampleIssuePurpose,
                             SampleIssueQuantity = m.SampleIssueQuantity,
                             SampleIssuedBy = m.SampleIssuedBy,
                             SampleIssuedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.SampleIssuedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             InventoryReturnQuantity = m.InventoryReturnQuantity,
                             InventoryReturnDate = m.InventoryReturnDate,
                             InventoryReturnedBy = m.InventoryReturnedBy,
                             InventoryReturnedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.InventoryReturnedBy).Select(x => x.UserFullName).FirstOrDefault(),
                             InventoryReturnRemarks = m.InventoryReturnRemarks,
                             InventoryReceiveDate = m.InventoryReceiveDate,
                             InventoryReceivedBy = m.InventoryReceivedBy,
                             InventoryReceiveRemarks = m.InventoryReceiveRemarks,
                         }).FirstOrDefault();
            return model;
        }

        public SampleTrackerModel UpdateSampleTracker(SampleTrackerModel model)
        {
            if (model.SampleSentToPersonId == null && model.SampleIssuedBy != null)
            {
                model.SampleSentToPersonName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == model.SampleIssuedBy).Select(x => x.UserFullName).FirstOrDefault();
            }
            Mapper.CreateMap<SampleTrackerModel, SampleTracker>();
            var m = Mapper.Map<SampleTracker>(model);
            _dbEntities.SampleTrackers.AddOrUpdate(m);
            _dbEntities.SaveChanges();
            model.ReceivedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == model.ReceivedBy).Select(x => x.UserFullName).FirstOrDefault();
            model.ReturnedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == model.ReturnedBy).Select(x => x.UserFullName).FirstOrDefault();
            model.AddedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == model.AddedBy).Select(x => x.UserFullName).FirstOrDefault();
            model.SampleIssuedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == model.SampleIssuedBy).Select(x => x.UserFullName).FirstOrDefault();
            model.InventoryReturnedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == model.InventoryReturnedBy).Select(x => x.UserFullName).FirstOrDefault();
            model.InventoryReceivedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == model.InventoryReceivedBy).Select(x => x.UserFullName).FirstOrDefault();
            return model;
        }

        public void SaveSampleReturnLog(SampleReturnLogModel model)
        {
            Mapper.CreateMap<SampleReturnLogModel, SampleReturnLog>();
            var m = Mapper.Map<SampleReturnLog>(model);
            _dbEntities.SampleReturnLogs.Add(m);
            _dbEntities.SaveChanges();
        }

        public List<SampleTrackerModel> GetSampleIssueListByIssuerId(long userId)
        {
            var model =
                _dbEntities.SampleTrackers.Where(m => m.SampleIssuedBy == userId).Select(m => new SampleTrackerModel
                {
                    SampleTrackerId = m.SampleTrackerId,
                    ProjectMasterId = m.ProjectMasterId,
                    Model = m.Model,
                    Role = m.SampleSentToDept,
                    RoleisHead = _dbEntities.CmnRoles.Where(x => x.RoleName == m.SampleSentToDept).Select(x => x.IsHead).FirstOrDefault(),
                    SampleSentToDept = _dbEntities.CmnRoles.Where(x => x.RoleName == m.SampleSentToDept).Select(x => x.RoleDescription).FirstOrDefault(),
                    SampleSentToPersonId = m.SampleSentToPersonId,
                    SampleSentToPersonName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.SampleSentToPersonId).Select(x => x.UserFullName).FirstOrDefault(),
                    SampleCategory = m.SampleCategory,
                    IMEI = m.IMEI,
                    Color = m.Color,
                    Remarks = m.Remarks,
                    AddedBy = m.AddedBy,
                    AddedDate = m.AddedDate,
                    UpdatedBy = m.UpdatedBy,
                    UpdatedDate = m.UpdatedDate,
                    ReceiveDate = m.ReceiveDate,
                    ReceivedBy = m.ReceivedBy,
                    ReceivedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.ReceivedBy).Select(x => x.UserFullName).FirstOrDefault(),
                    ReturnStatus = m.ReturnStatus,
                    ReturnedBy = m.ReturnedBy,
                    ReturnedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.ReturnedBy).Select(x => x.UserFullName).FirstOrDefault(),
                    ReturnDate = m.ReturnDate,
                    Purpose = m.Purpose,
                    SupplierName = m.SupplierName,
                    NumberOfSample = m.NumberOfSample,
                    Others = m.Others,
                    AdditionalInfo = m.AdditionalInfo,
                    ReturnQuantity = m.ReturnQuantity,
                    AddedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                    SampleIssueDate = m.SampleIssueDate,
                    SampleIssuePurpose = m.SampleIssuePurpose,
                    SampleIssueQuantity = m.SampleIssueQuantity,
                    SampleIssuedBy = m.SampleIssuedBy,
                    SampleIssuedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.SampleIssuedBy).Select(x => x.UserFullName).FirstOrDefault(),
                    InventoryReturnQuantity = m.InventoryReturnQuantity,
                    InventoryReturnDate = m.InventoryReturnDate,
                    InventoryReturnedBy = m.InventoryReturnedBy,
                    InventoryReturnedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.InventoryReturnedBy).Select(x => x.UserFullName).FirstOrDefault(),
                    InventoryReturnRemarks = m.InventoryReturnRemarks,
                    InventoryReceiveDate = m.InventoryReceiveDate,
                    InventoryReceivedBy = m.InventoryReceivedBy,
                    InventoryReceiveRemarks = m.InventoryReceiveRemarks,
                }).ToList();
            return model;
        }

        public List<SampleTrackerModel> GetAllSampleTrackers()
        {
            var model =
                _dbEntities.SampleTrackers.Select(m => new SampleTrackerModel
                {
                    SampleTrackerId = m.SampleTrackerId,
                    ProjectMasterId = m.ProjectMasterId,
                    Model = m.Model,
                    Role = m.SampleSentToDept,
                    RoleisHead = _dbEntities.CmnRoles.Where(x => x.RoleName == m.SampleSentToDept).Select(x => x.IsHead).FirstOrDefault(),
                    SampleSentToDept = _dbEntities.CmnRoles.Where(x => x.RoleName == m.SampleSentToDept).Select(x => x.RoleDescription).FirstOrDefault(),
                    SampleSentToPersonId = m.SampleSentToPersonId,
                    SampleSentToPersonName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.SampleSentToPersonId).Select(x => x.UserFullName).FirstOrDefault(),
                    SampleCategory = m.SampleCategory,
                    IMEI = m.IMEI,
                    Color = m.Color,
                    Remarks = m.Remarks,
                    AddedBy = m.AddedBy,
                    AddedDate = m.AddedDate,
                    UpdatedBy = m.UpdatedBy,
                    UpdatedDate = m.UpdatedDate,
                    ReceiveDate = m.ReceiveDate,
                    ReceivedBy = m.ReceivedBy,
                    ReceivedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.ReceivedBy).Select(x => x.UserFullName).FirstOrDefault(),
                    ReturnStatus = m.ReturnStatus,
                    ReturnedBy = m.ReturnedBy,
                    ReturnedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.ReturnedBy).Select(x => x.UserFullName).FirstOrDefault(),
                    ReturnDate = m.ReturnDate,
                    Purpose = m.Purpose,
                    SupplierName = m.SupplierName,
                    NumberOfSample = m.NumberOfSample,
                    Others = m.Others,
                    AdditionalInfo = m.AdditionalInfo,
                    ReturnQuantity = m.ReturnQuantity,
                    AddedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                    SampleIssueDate = m.SampleIssueDate,
                    SampleIssuePurpose = m.SampleIssuePurpose,
                    SampleIssueQuantity = m.SampleIssueQuantity,
                    SampleIssuedBy = m.SampleIssuedBy,
                    SampleIssuedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.SampleIssuedBy).Select(x => x.UserFullName).FirstOrDefault(),
                    InventoryReturnQuantity = m.InventoryReturnQuantity,
                    InventoryReturnDate = m.InventoryReturnDate,
                    InventoryReturnedBy = m.InventoryReturnedBy,
                    InventoryReturnedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.InventoryReturnedBy).Select(x => x.UserFullName).FirstOrDefault(),
                    InventoryReturnRemarks = m.InventoryReturnRemarks,
                    InventoryReceiveDate = m.InventoryReceiveDate,
                    InventoryReceivedBy = m.InventoryReceivedBy,
                    InventoryReceiveRemarks = m.InventoryReceiveRemarks,
                }).ToList();
            return model;
        }

        public List<SampleTrackerModel> GetSampleTrackerToReceive()
        {
            var model =
                _dbEntities.SampleTrackers.Where(m => m.InventoryReturnDate != null).Select(m => new SampleTrackerModel
                {
                    SampleTrackerId = m.SampleTrackerId,
                    ProjectMasterId = m.ProjectMasterId,
                    Model = m.Model,
                    Role = m.SampleSentToDept,
                    RoleisHead = _dbEntities.CmnRoles.Where(x => x.RoleName == m.SampleSentToDept).Select(x => x.IsHead).FirstOrDefault(),
                    SampleSentToDept = _dbEntities.CmnRoles.Where(x => x.RoleName == m.SampleSentToDept).Select(x => x.RoleDescription).FirstOrDefault(),
                    SampleSentToPersonId = m.SampleSentToPersonId,
                    SampleSentToPersonName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.SampleSentToPersonId).Select(x => x.UserFullName).FirstOrDefault(),
                    SampleCategory = m.SampleCategory,
                    IMEI = m.IMEI,
                    Color = m.Color,
                    Remarks = m.Remarks,
                    AddedBy = m.AddedBy,
                    AddedDate = m.AddedDate,
                    UpdatedBy = m.UpdatedBy,
                    UpdatedDate = m.UpdatedDate,
                    ReceiveDate = m.ReceiveDate,
                    ReceivedBy = m.ReceivedBy,
                    ReceivedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.ReceivedBy).Select(x => x.UserFullName).FirstOrDefault(),
                    ReturnStatus = m.ReturnStatus,
                    ReturnedBy = m.ReturnedBy,
                    ReturnedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.ReturnedBy).Select(x => x.UserFullName).FirstOrDefault(),
                    ReturnDate = m.ReturnDate,
                    Purpose = m.Purpose,
                    SupplierName = m.SupplierName,
                    NumberOfSample = m.NumberOfSample,
                    Others = m.Others,
                    AdditionalInfo = m.AdditionalInfo,
                    ReturnQuantity = m.ReturnQuantity,
                    AddedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                    SampleIssueDate = m.SampleIssueDate,
                    SampleIssuePurpose = m.SampleIssuePurpose,
                    SampleIssueQuantity = m.SampleIssueQuantity,
                    SampleIssuedBy = m.SampleIssuedBy,
                    SampleIssuedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.SampleIssuedBy).Select(x => x.UserFullName).FirstOrDefault(),
                    InventoryReturnQuantity = m.InventoryReturnQuantity,
                    InventoryReturnDate = m.InventoryReturnDate,
                    InventoryReturnedBy = m.InventoryReturnedBy,
                    InventoryReturnedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.InventoryReturnedBy).Select(x => x.UserFullName).FirstOrDefault(),
                    InventoryReturnRemarks = m.InventoryReturnRemarks,
                    InventoryReceiveDate = m.InventoryReceiveDate,
                    InventoryReceivedBy = m.InventoryReceivedBy,
                    InventoryReceiveRemarks = m.InventoryReceiveRemarks,
                }).ToList();
            return model;
        }
        #endregion


        #region Project Close Penalty

        public List<ProjectClosePenaltyModel> GetRunningPenaltyModels()
        {
            string query = string.Format(@"select ppf.ProjectPurchaseOrderFormId,ppf.ProjectMasterId, pm.ProjectName, ppf.PoDate ,pm.OrderNuber as OrderNumber,
                                           DATEDIFF(MONTH,ppf.PoDate,GETDATE()) as PoCreatedBeforeMonth,
                                           DATEDIFF(DAY, DATEADD(MONTH,7,ppf.PoDate),GETDATE()) as DaysPassedAfterSevenMonth,
                                           DATEDIFF(DAY, DATEADD(MONTH,7,ppf.PoDate),GETDATE())*50 as Penalty 
                                           from ProjectPurchaseOrderForms ppf
                                           inner join ProjectMasters pm on ppf.ProjectMasterId=pm.ProjectMasterId
                                           where ppf.IsCompleted=0 and DATEDIFF(MONTH,ppf.PoDate,GETDATE())>7");

            var model = _dbEntities.Database.SqlQuery<ProjectClosePenaltyModel>(query).ToList();
            return model;
        }

        public List<ProjectClosePenaltyModel> GetClosedPenaltyModels()
        {
            var model = (from v in _dbEntities.ProjectClosePenaltys
                         select new ProjectClosePenaltyModel
                         {
                             ProjectClosePenaltyId = v.ProjectClosePenaltyId,
                             ProjectMasterId = v.ProjectMasterId,
                             ProjectPurchaseOrderFormId = v.ProjectPurchaseOrderFormId,
                             ProjectName = v.ProjectName,
                             OrderNumber = v.OrderNumber,
                             PoDate = v.PoDate,
                             PoCreatedBeforeMonth = v.PoCreatedBeforeMonth,
                             DaysPassedAfterSevenMonth = v.DaysPassedAfterSevenMonth,
                             Penalty = v.Penalty,
                             IsCompletedDate = v.IsCompletedDate
                         }).ToList();
            return model;
        }

        public List<PrPoViewModel> GetPrPoData(long userId)
        {
            var models = new List<PrPoViewModel>();
            var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);
            if (user != null)
            {
                string employeCode = user.EmployeeCode;
                string prPoQuery = "SELECT DISTINCT RHA.ORG_ID CompanyId, HRO.NAME Company, RHA.SEGMENT1 PR_NUM, PRCRE.FULL_NAME||'('||PRCRE.EMPLOYEE_NUM||')' PR_CREATOR_NAME, RHA.CREATION_DATE PR_CREATION_DATE, PHA.SEGMENT1 PO_NUM, POCRE.FULL_NAME||'('||POCRE.EMPLOYEE_NUM||')' PO_Creator, POCRE.EMPLOYEE_NUM CREATOR_ID, PHA.CREATION_DATE PO_CREATION_DATE, PHA.AUTHORIZATION_STATUS PO_STATUS FROM APPS.PO_REQUISITION_HEADERS_ALL RHA, APPS.PO_REQUISITION_LINES_ALL RLA, APPS.HR_ALL_ORGANIZATION_UNITS HRO, APPS.HR_EMPLOYEES PRCRE, APPS.PO_LINE_LOCATIONS_ALL PLLA, APPS.PO_LINES_ALL PLA, APPS.PO_HEADERS_ALL PHA, APPS.HR_EMPLOYEES POCRE WHERE RHA.REQUISITION_HEADER_ID=RLA.REQUISITION_HEADER_ID AND RHA.ORG_ID=RLA.ORG_ID AND RHA.ORG_ID=HRO.ORGANIZATION_ID AND RHA.PREPARER_ID=PRCRE.EMPLOYEE_ID AND RLA.LINE_LOCATION_ID=PLLA.LINE_LOCATION_ID AND RLA.DESTINATION_ORGANIZATION_ID=PLLA.SHIP_TO_ORGANIZATION_ID AND PLLA.PO_LINE_ID=PLA.PO_LINE_ID AND PLA.PO_HEADER_ID=PHA.PO_HEADER_ID AND PLA.ORG_ID=PHA.ORG_ID AND PHA.AGENT_ID=POCRE.EMPLOYEE_ID AND PHA.TYPE_LOOKUP_CODE='STANDARD' AND PHA.AUTHORIZATION_STATUS IN ('IN PROCESS', 'APPROVED') AND RHA.AUTHORIZATION_STATUS IN ('IN PROCESS', 'APPROVED') AND RLA.MODIFIED_BY_AGENT_FLAG IS NULL AND NVL(RLA.CANCEL_FLAG, 'N') NOT IN ('Y') AND NVL(PHA.CANCEL_FLAG,'N') <>'Y' And NVL(PLA.CANCEL_FLAG,'N') <>'Y' AND NVL(PLLA.CANCEL_FLAG,'N') <>'Y' AND POCRE.EMPLOYEE_NUM = '" + employeCode + "' AND RHA.ORG_ID IN (86, 584, 646)";

                var connection = OracleDbConnection.GetOldConnection();
                var command = new OracleCommand(prPoQuery, connection) { CommandType = CommandType.Text };
                try
                {
                    connection.Open();
                    OracleDataReader dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {

                        while (dataReader.Read())
                        {
                            var model = new PrPoViewModel();
                            model.CompanyId = dataReader.IsDBNull(0) ? "" : dataReader.GetInt64(0).ToString(CultureInfo.InvariantCulture);
                            model.Company = dataReader.IsDBNull(1) ? "" : dataReader.GetString(1);
                            model.PrNumber = dataReader.IsDBNull(2) ? "" : dataReader.GetString(2);
                            model.PrCreatorName = dataReader.IsDBNull(3) ? "" : dataReader.GetString(3);
                            model.PrCreationDate = dataReader.IsDBNull(4) ? "" : dataReader.GetDateTime(4).ToString("d", CultureInfo.CreateSpecificCulture("en-NZ"));
                            model.PoNumber = dataReader.IsDBNull(5) ? "" : dataReader.GetString(5);
                            model.PoCreator = dataReader.IsDBNull(6) ? "" : dataReader.GetString(6);
                            model.PoCreatorId = dataReader.IsDBNull(7) ? "" : dataReader.GetString(7);
                            model.PoCreationDate = dataReader.IsDBNull(8) ? "" : dataReader.GetDateTime(8).ToString("d", CultureInfo.CreateSpecificCulture("en-NZ"));
                            model.PoStatus = dataReader.IsDBNull(9) ? "" : dataReader.GetString(9);
                            models.Add(model);
                        }
                    }
                    dataReader.Close();
                    connection.Close();
                    command.Dispose();
                }
                catch (Exception exception)
                {
                    throw new Exception();
                }
            }
            return models;
        }

        #endregion

        #region Gantt

        #endregion

        #region Discussion

        public DiscussionModel SaveDiscussion(DiscussionModel model)
        {
            Mapper.CreateMap<DiscussionModel, Discussion>();
            var m = Mapper.Map<Discussion>(model);
            _dbEntities.Discussions.Add(m);
            _dbEntities.SaveChanges();
            model.DiscussionId = m.DiscussionId;
            return model;
        }

        public List<DiscussionModel> GetDiscussions()
        {
            var model = new List<DiscussionModel>();
            try
            {
                model = (from m in _dbEntities.Discussions
                         orderby m.DiscussionId descending
                         select new DiscussionModel
                         {
                             DiscussionId = m.DiscussionId,
                             Comment = m.Comment,
                             AddedBy = m.AddedBy,
                             AddedDate = m.AddedDate,
                             AddedByName =
                                 _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy)
                                     .Select(x => x.UserFullName)
                                     .FirstOrDefault()
                         }).Take(20).ToList();
                return model;
            }
            catch (Exception e)
            {
                return model;
            }
        }

        public List<DiscussionModel> LoadMoreDiscussions(long id)
        {
            var model = new List<DiscussionModel>();
            try
            {
                model = (from m in _dbEntities.Discussions
                         where m.DiscussionId < id
                         orderby m.DiscussionId descending
                         select new DiscussionModel
                         {
                             DiscussionId = m.DiscussionId,
                             Comment = m.Comment,
                             AddedBy = m.AddedBy,
                             AddedDate = m.AddedDate,
                             AddedByName =
                                 _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy)
                                     .Select(x => x.UserFullName)
                                     .FirstOrDefault()
                         }).Take(20).ToList();
                return model;
            }
            catch (Exception e)
            {
                return model;
            }
        }

        public List<DiscussionFileUploadModel> GetDiscussionFileUploadModels(List<DiscussionModel> model)
        {
            var uploads = new List<DiscussionFileUploadModel>();
            foreach (var u in model)
            {
                var files = (from v in _dbEntities.DiscussionFileUploads
                             where v.DiscussionId == u.DiscussionId
                             select new DiscussionFileUploadModel
                             {
                                 Id = v.Id,
                                 DiscussionId = v.DiscussionId,
                                 FileUploadPath = v.FileUploadPath,
                                 AddedDate = v.AddedDate,
                                 AddedBy = v.AddedBy
                             }).ToList();
                uploads.AddRange(files);
            }
            return uploads;
        }

        public DiscussionFileUploadModel GetFileUploadModelById(long id)
        {
            var files = (from v in _dbEntities.DiscussionFileUploads
                         where v.Id == id
                         select new DiscussionFileUploadModel
                         {
                             Id = v.Id,
                             DiscussionId = v.DiscussionId,
                             FileUploadPath = v.FileUploadPath,
                             AddedDate = v.AddedDate,
                             AddedBy = v.AddedBy
                         }).FirstOrDefault();
            return files;
        }

        public List<DiscussionModel> GetDiscussionByMention(string str)
        {
            var model = (from m in _dbEntities.Discussions
                         orderby m.AddedDate descending
                         where m.Comment.Contains(str)
                         select new DiscussionModel
                         {
                             DiscussionId = m.DiscussionId,
                             Comment = m.Comment,
                             AddedBy = m.AddedBy,
                             AddedDate = m.AddedDate,
                             AddedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy).Select(x => x.UserFullName).FirstOrDefault()
                         }).ToList();
            return model;
        }

        public List<DiscussionModel> GetDiscussionByHashTag(string tag)
        {
            var model = (from m in _dbEntities.Discussions
                         orderby m.AddedDate descending
                         where m.Comment.Contains(tag)
                         select new DiscussionModel
                         {
                             DiscussionId = m.DiscussionId,
                             Comment = m.Comment,
                             AddedBy = m.AddedBy,
                             AddedDate = m.AddedDate,
                             AddedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy).Select(x => x.UserFullName).FirstOrDefault()
                         }).ToList();
            return model;
        }

        public void SaveHashtag(HashtagModel model)
        {
            var duplicate = _dbEntities.Hashtags.FirstOrDefault(x => x.HashtagName == model.HashtagName);
            if (duplicate == null)
            {
                Mapper.CreateMap<HashtagModel, Hashtag>();
                var m = Mapper.Map<Hashtag>(model);
                _dbEntities.Hashtags.Add(m);
                _dbEntities.SaveChanges();
            }
        }

        public List<HashtagModel> GetHashtagByString(string str)
        {
            var model = (from v in _dbEntities.Hashtags
                         where v.HashtagName.Contains(str)
                         select new HashtagModel
                         {
                             HashtagId = v.HashtagId,
                             HashtagName = v.HashtagName,
                             AddedBy = v.AddedBy,
                             AddedDate = v.AddedDate
                         }).ToList();
            return model;
        }

        public string[] GetHashtagByStringToArr(string str)
        {
            var model = (from v in _dbEntities.Hashtags
                         where v.HashtagName.Contains(str)
                         select new HashtagModel
                         {
                             HashtagId = v.HashtagId,
                             HashtagName = v.HashtagName,
                             AddedBy = v.AddedBy,
                             AddedDate = v.AddedDate
                         }).ToList();
            return model.Select(h => h.HashtagName).ToArray();
        }

        public int CommentCount()
        {
            var count = _dbEntities.Discussions.Count();
            return count;
        }

        public List<TopTrendingHashtags> GetTopHashtag()
        {
            var tophashlist = new List<TopTrendingHashtags>();
            var hashes = (from h in _dbEntities.Hashtags
                          select new HashtagModel
                          {
                              HashtagId = h.HashtagId,
                              HashtagName = h.HashtagName
                          }).ToList();
            foreach (var h in hashes)
            {
                var query = string.Format(@"select count(*) Counter,(select HashtagName from Hashtags where HashtagId={0}) Hashtag from Discussions where Comment like '%'+(select HashtagName from Hashtags where HashtagId={0})+'%'", h.HashtagId);
                var exeQuery = _dbEntities.Database.SqlQuery<TopTrendingHashtags>(query).ToList();
                tophashlist.AddRange(exeQuery);
            }
            tophashlist = tophashlist.OrderByDescending(x => x.Counter).Take(10).ToList();
            return tophashlist;
        }

        public void UploadDiscussionFile(DiscussionFileUploadModel model)
        {
            Mapper.CreateMap<DiscussionFileUploadModel, DiscussionFileUpload>();
            var m = Mapper.Map<DiscussionFileUpload>(model);
            _dbEntities.DiscussionFileUploads.Add(m);
            _dbEntities.SaveChanges();
        }

        public DiscussionReplyModel SaveDiscussionReply(DiscussionReplyModel model)
        {
            Mapper.CreateMap<DiscussionReplyModel, DiscussionReply>();
            var m = Mapper.Map<DiscussionReply>(model);
            _dbEntities.DiscussionReplies.Add(m);
            _dbEntities.SaveChanges();
            model.Id = m.Id;
            model.AddedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy).Select(x => x.UserFullName).FirstOrDefault();
            return model;
        }

        public List<DiscussionReplyModel> GetDiscussionReplies(List<DiscussionModel> model)
        {
            var replies = new List<DiscussionReplyModel>();
            foreach (var m in model)
            {
                var reply = (from v in _dbEntities.DiscussionReplies
                             where v.DiscussionId == m.DiscussionId
                             select new DiscussionReplyModel
                             {
                                 Id = v.Id,
                                 DiscussionId = v.DiscussionId,
                                 Reply = v.Reply,
                                 AddedBy = v.AddedBy,
                                 AddedDate = v.AddedDate,
                                 UpdatedBy = v.UpdatedBy,
                                 UpdatedDate = v.UpdatedDate,
                                 AddedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.AddedBy).Select(x => x.UserFullName).FirstOrDefault()
                             }).ToList();
                replies.AddRange(reply);
            }
            return replies;
        }

        public List<DiscussionReplyModel> GetDiscussionReplyByModels(List<DiscussionModel> model)
        {
            var replies = new List<DiscussionReplyModel>();
            foreach (var u in model)
            {
                var reply = (from v in _dbEntities.DiscussionReplies
                             where v.DiscussionId == u.DiscussionId
                             select new DiscussionReplyModel
                             {
                                 Id = v.Id,
                                 DiscussionId = v.DiscussionId,
                                 Reply = v.Reply,
                                 AddedDate = v.AddedDate,
                                 AddedBy = v.AddedBy,
                                 AddedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.AddedBy).Select(x => x.UserFullName).FirstOrDefault()
                             }).ToList();
                replies.AddRange(reply);
            }
            return replies;
        }
        #endregion

        #region Doc Management

        public FolderModel SaveFolderModel(FolderModel model)
        {
            var duplicate = _dbEntities.Folders.FirstOrDefault(x => x.FolderName == model.FolderName && x.ProjectName == model.ProjectName);
            if (duplicate == null)
            {
                Mapper.CreateMap<FolderModel, Folder>();
                var m = Mapper.Map<Folder>(model);
                _dbEntities.Folders.Add(m);
                _dbEntities.SaveChanges();
                model.FolderId = m.FolderId;
            }
            return model;
        }

        public List<FolderModel> GetFolderModelsByProjectAndParent(string projectname, long parentfolder)
        {
            var model = (from v in _dbEntities.Folders
                         where v.ProjectName == projectname && v.Parent == parentfolder
                         select new FolderModel
                         {
                             FolderId = v.FolderId,
                             ProjectName = v.ProjectName,
                             FolderName = v.FolderName,
                             Parent = v.Parent,
                             AddedBy = v.AddedBy,
                             AddedDate = v.AddedDate,
                             UpdatedBy = v.UpdatedBy,
                             UpdatedDate = v.UpdatedDate
                         }).ToList();
            foreach (var v in model)
            {
                v.Size = _dbEntities.DocManagementFileUploads.Where(x => x.FolderId == v.FolderId).Sum(x => x.Size) ?? 0;
            }
            return model;
        }

        public List<FolderModel> BrowseBack(string projectname, long folderid)
        {
            var parentFolderid = _dbEntities.Folders.Where(x => x.FolderId == folderid).Select(x => x.Parent).FirstOrDefault();
            var model = (from v in _dbEntities.Folders
                         where v.Parent == parentFolderid && v.ProjectName == projectname
                         select new FolderModel
                         {
                             FolderId = v.FolderId,
                             ProjectName = v.ProjectName,
                             FolderName = v.FolderName,
                             Parent = v.Parent,
                             AddedBy = v.AddedBy,
                             AddedDate = v.AddedDate,
                             UpdatedBy = v.UpdatedBy,
                             UpdatedDate = v.UpdatedDate
                         }).ToList();
            return model;
        }

        public List<DocManagementFileUploadModel> BrowseBackFiles(string projectname, long folderid)
        {
            var parentFolderid = _dbEntities.Folders.Where(x => x.FolderId == folderid).Select(x => x.Parent).FirstOrDefault();
            var model = (from v in _dbEntities.DocManagementFileUploads
                         where v.ProjectName == projectname && v.FolderId == parentFolderid
                         select new DocManagementFileUploadModel
                         {
                             DocManagerFileId = v.DocManagerFileId,
                             ProjectName = v.ProjectName,
                             FolderId = v.FolderId,
                             DocFilePath = v.DocFilePath,
                             AddedBy = v.AddedBy,
                             AddedDate = v.AddedDate,
                             Size = v.Size
                         }).ToList();
            return model;
        }

        public DocManagementFileUploadModel SaveFileUploadModels(DocManagementFileUploadModel model)
        {
            Mapper.CreateMap<DocManagementFileUploadModel, DocManagementFileUpload>();
            var m = Mapper.Map<DocManagementFileUpload>(model);
            _dbEntities.DocManagementFileUploads.Add(m);
            _dbEntities.SaveChanges();
            model.DocManagerFileId = m.DocManagerFileId;
            //======
            var updatefolder = _dbEntities.Folders.FirstOrDefault(x => x.FolderId == model.FolderId);
            if (updatefolder != null)
            {
                updatefolder.UpdatedBy = model.AddedBy;
                updatefolder.UpdatedDate = model.AddedDate;
                _dbEntities.Folders.AddOrUpdate(updatefolder);
                _dbEntities.SaveChanges();
            } return model;
        }

        public List<DocManagementFileUploadModel> GetFileUploadModels(string projectname, long parentfolder)
        {
            var model = (from v in _dbEntities.DocManagementFileUploads
                         where v.ProjectName == projectname && v.FolderId == parentfolder
                         select new DocManagementFileUploadModel
                         {
                             DocManagerFileId = v.DocManagerFileId,
                             ProjectName = v.ProjectName,
                             FolderId = v.FolderId,
                             DocFilePath = v.DocFilePath,
                             AddedBy = v.AddedBy,
                             AddedDate = v.AddedDate,
                             Size = v.Size
                         }).ToList();
            return model;
        }

        public bool DuplicateFileCheck(string projectname, string filename)
        {
            var model = (from v in _dbEntities.DocManagementFileUploads
                         where v.DocFilePath.Contains(filename) && v.ProjectName == projectname
                         select new DocManagementFileUploadModel
                         {
                             DocManagerFileId = v.DocManagerFileId,
                             ProjectName = v.ProjectName,
                             FolderId = v.FolderId,
                             DocFilePath = v.DocFilePath,
                             AddedBy = v.AddedBy,
                             AddedDate = v.AddedDate
                         }).FirstOrDefault();
            return model != null;
        }

        public DocManagementFileUploadModel GetFileById(long id)
        {
            var model = (from v in _dbEntities.DocManagementFileUploads
                         where v.DocManagerFileId == id
                         select new DocManagementFileUploadModel
                         {
                             DocManagerFileId = v.DocManagerFileId,
                             ProjectName = v.ProjectName,
                             FolderId = v.FolderId,
                             DocFilePath = v.DocFilePath,
                             AddedBy = v.AddedBy,
                             AddedDate = v.AddedDate,
                             Size = v.Size
                         }).FirstOrDefault();
            return model;
        }
        #endregion

        #region Otherwise Cellphone Production Status
        public Produced_UnProducedIMEIViewModel GetProduced_UnProducedIMEIs(string modelname, string order)
        {
            Produced_UnProducedIMEIViewModel final = new Produced_UnProducedIMEIViewModel();
            #region Order Quantity
            var orderqtydetails = _dbEntities.ProjectOrderQuantityDetails;
            var projectmasters = _dbEntities.ProjectMasters;
            var orderqty = from d in orderqtydetails
                           join p in projectmasters
                           on d.ProjectMasterId equals p.ProjectMasterId
                           select new
                           {
                               ProjectModel = d.ProjectModel,
                               Order_No = p.OrderNuber,
                               OrderQuantity = (d.OrderQuantity != null ? d.OrderQuantity : 0),
                           };
            var OrderQty = (from q in orderqty
                            group q by new { q.ProjectModel, Order_Num = q.Order_No }
                                into g
                                select new
                                {
                                    ProjectModel = g.Key.ProjectModel,
                                    Order_No = (int)g.Key.Order_Num,
                                    OrderQuantity = g.Sum(x => (x.OrderQuantity != null ? x.OrderQuantity : 0))
                                }).ToList();
            #endregion
            DateTime datefromcheck = DateTime.Today.AddYears(-4);
            var filteredorderqty = from d in orderqtydetails
                                   join p in projectmasters.Where(x => x.AddedDate >= datefromcheck && x.SourcingType != "OEM")
                           on d.ProjectMasterId equals p.ProjectMasterId
                                   select new
                                   {
                                       ProjectModel = d.ProjectModel,
                                       Order_No = p.OrderNuber,
                                       OrderQuantity = (d.OrderQuantity != null ? d.OrderQuantity : 0),
                                   };
            var xx = (from d in filteredorderqty
                      group d by new { d.ProjectModel, d.Order_No }
                          into g
                          select new ProjectMasterInv
                          {
                              ProjectModel = g.Key.ProjectModel,
                              Order_No = (int)g.Key.Order_No,
                              OrderQuantity = (Int64)g.Sum(x => (x.OrderQuantity != null ? x.OrderQuantity : 0))
                          }).ToList();

            #region RBS Data
            List<rbsBarCodeInv> IMEI_QTY = new List<rbsBarCodeInv>();
            String connectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "";
                query = string.Format(@"Select IMEI_QTY.Model ,Produced, IMEI_QTY.Order_Num,IMEI_QTY.Order_No,LastMonth.LastMonthIMEI
                                        from ((SELECT tblBarCodeInv.Model ,COUNT(tblBarCodeInv.BarCode) AS Produced
                                        ,tblBarCodeInv.Updatedby AS Order_Num
                                        ,CONVERT(int, (SUBSTRING(tblBarCodeInv.Updatedby, PATINDEX('%[0-9]%', tblBarCodeInv.Updatedby), LEN(tblBarCodeInv.Updatedby)))) AS Order_No
                                        FROM RBSYNERGY.dbo.tblBarCodeInv
                                        WHERE tblBarCodeInv.DateAdded >= DATEADD(YEAR, -3, GETDATE()) 
                                        AND tblBarCodeInv.Updatedby not in ( 'Not Tracable', 'Test Order') GROUP BY tblBarCodeInv.Model ,tblBarCodeInv.Updatedby) IMEI_QTY 
                                        Left JOIN 
                                        (Select Model,Count(BarCode) LastMonthIMEI
                                        ,SUBSTRING(tblBarCodeInv.Updatedby, PATINDEX('%[0-9]%', tblBarCodeInv.Updatedby), LEN(tblBarCodeInv.Updatedby)) AS Order_No
                                         from tblBarCodeInv where PrintDate >= DATEADD(MONTH, -1, GETDATE()) 
                                        AND tblBarCodeInv.Updatedby not in ( 'Not Tracable', 'Test Order')
                                        group by Model,Updatedby) LastMonth
                                        ON IMEI_QTY.Model = LastMonth.Model
                                        AND IMEI_QTY.Order_No = LastMonth.Order_No)");

                var command = new SqlCommand(query, connection);
                command.CommandTimeout = 200;

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var obj = new rbsBarCodeInv();
                    obj.ProjectModel = Convert.ToString(reader["Model"]);
                    obj.Produced = (reader["Produced"] != null ? Convert.ToInt64(reader["Produced"]) : 0);
                    obj.Order_Num = Convert.ToString(reader["Order_Num"]);
                    obj.Order_No = reader["Order_No"] != null ? Convert.ToInt32(reader["Order_No"].ToString().Trim()) : 0;
                    obj.LastMonthIMEIProduced = (reader["LastMonthIMEI"] != System.DBNull.Value
                        ? Convert.ToInt64(reader["LastMonthIMEI"])
                        : 0);

                    IMEI_QTY.Add(obj);
                }
                connection.Close();
            }
            #endregion


            var ProjectMaster = (from a in xx
                                 join imq in IMEI_QTY
                                 on new { a.ProjectModel, a.Order_No } equals new { imq.ProjectModel, imq.Order_No }
                                 into gj
                                 from f in gj.DefaultIfEmpty()
                                 select new rbsBarCodeInv
                                    {
                                        ProjectModel = a.ProjectModel,
                                        Order_No = a.Order_No,
                                        OrderQuantity = a.OrderQuantity,
                                        Produced = (f != null ? f.Produced : 0),
                                        UnProduced = a.OrderQuantity - (f != null ? f.Produced : 0),
                                        LastMonthIMEIProduced = (f != null ? f.LastMonthIMEIProduced : 0)

                                    }).ToList();
            var data = from qt in OrderQty
                       join pm in ProjectMaster
                       on new { qt.ProjectModel, qt.Order_No } equals new { pm.ProjectModel, pm.Order_No }
                       into gj
                       from f in gj.DefaultIfEmpty()
                       select new Produced_UnproducedIMEI
                       {
                           ProjectModel = qt.ProjectModel,
                           OrderNumber = qt.Order_No,
                           OrderQuantity = (Int64)qt.OrderQuantity,
                           Difference = (Int64)qt.OrderQuantity - (f != null ? f.OrderQuantity : 0),
                           UnProduced = (f != null ? f.UnProduced : 0),
                           Produced = (f != null ? f.Produced : 0),
                           LastMonthIMEIProduced = (f != null ? f.LastMonthIMEIProduced : 0)
                       };
            data = data.Where(x => x.UnProduced > 0);
            final.Models = (from d in data
                            group d by d.ProjectModel
                                into g
                                select new ProjectMasterInv
                                {
                                    ProjectModel = g.Key,
                                }).ToList();
            final.Orders = (from d in data
                            select new ProjectMasterInv
                           {
                               ProjectModel = d.ProjectModel,
                               Order_No = d.OrderNumber
                           }).ToList();
            if (modelname != "")
                data = data.Where(y => y.ProjectModel.ToUpper() == modelname.ToUpper());
            if (order != "")
                data = data.Where(y => y.OrderNumber == Int32.Parse(order));
            final.ModelName = modelname;
            final.Order = order;
            final.Produced_UnproducedIMEIs = data.ToList();
            return final;
        }

        public List<ProjectMasterInv> GetOrdersfromModel(string modelname)
        {
            var projectmasters = _dbEntities.ProjectMasters.Where(x => x.ProjectModel == modelname);
            var orders = (from d in projectmasters
                          where d.IsActive
                          select new ProjectMasterInv
                          {
                              Order_No = (int)d.OrderNuber
                          }).ToList();

            return orders;
        }
        #endregion

        #region Service Trends
        public ServiceTrendsViewModel GetServiceLog(string modelname)
        {
            var reportModels = new ServiceTrendsViewModel();
            string rbsConnectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            string wsmsConnectionString = ConfigurationManager.ConnectionStrings["WSMSConnectionString"].ConnectionString;



            using (var connection = new SqlConnection(rbsConnectionString))
            {
                string query = string.Format(@"select firstQuery.*, case when secondQuery.ReplacementQuantity is null then 0 else secondQuery.ReplacementQuantity end as ReplacementQuantity, 
                case when secondQuery.ReplacementQuantity is null then 0 else round(cast((cast(secondQuery.ReplacementQuantity*100 as float))/cast(firstQuery.TotalActivated as float) as float), 2) end as ReplacementPercentage from
                (
                select z.Model, MIN(cpd.ReleaseDate) as LaunchDate, cpd.DaysTillToday, z.TotalStock,z.TotalActivated, z.TotalUnactivated from 
                (
	                select x.Model, x.TotalStock, y.TotalActivated, CONVERT(bigint, x.TotalStock)-CONVERT(bigint, y.TotalActivated) as TotalUnactivated from
	                (select Model, count(*) as TotalStock from tblBarCodeInv where Model = '{0}'
	                group by Model) as x
	                left Join

	                (SELECT Model, SUM(Number) AS TotalActivated	
	                FROM            dbo.vw_ActivationbyDays where Model = '{0}'
	                GROUP BY Model) as y
	                on x.Model = y.Model
                ) as z
                left join
                (
                select TOP 1 Model, ReleaseDate, case when ReleaseDate is not null then DATEDIFF(day, ReleaseDate, GETDATE()) else 0 end as DaysTillToday
                from tblCellPhoneDepriciationPrice where Model = '{0}' GROUP BY Model, ReleaseDate
                ORDER BY  ReleaseDate ASC
                ) as cpd
                on z.Model = cpd.Model 
                group by z.Model, z.TotalStock, z.TotalActivated, z.TotalUnactivated, DaysTillToday
                having z.TotalActivated IS NOT NULL
                ) as firstQuery


                left join 
                (
                SELECT Model, count(RequestStatus) as ReplacementQuantity from IMEIReplacementMaster.IMEIReplacementMaster where RequestStatus in ('WareHouse Received','Approved','Recommended','ServiceApproved','WastageReceived', 'WareHouseDelivered') and Model = '{0}'
                group by Model
                ) as secondQuery
                on firstQuery.Model = secondQuery.Model", modelname);
                connection.Open();
                var sqlcommand = new SqlCommand(query, connection);
                var datareader = sqlcommand.ExecuteReader();

                while (datareader.Read())
                {
                    //reportModels.(new ServiceTrendsViewModel
                    //{
                    reportModels.ModelName = Convert.ToString(datareader["Model"]);
                    reportModels.ReleaseDate = Convert.ToString(datareader["LaunchDate"]);
                    reportModels.DayCountfromRelease = Convert.ToString(datareader["DaysTillToday"]);
                    reportModels.Totalhandset = Convert.ToDouble(datareader["TotalStock"]);
                    reportModels.TotalActivated = Convert.ToDouble(datareader["TotalActivated"]);
                    reportModels.UnActivated = Convert.ToDouble(datareader["TotalUnactivated"]);
                    reportModels.Replacement = Convert.ToDouble(datareader["ReplacementQuantity"]);
                    reportModels.ReplacementPercentage = Convert.ToDouble(datareader["ReplacementPercentage"]);
                    //});
                }

                sqlcommand.Dispose();
                connection.Close();
            }

            using (var connection = new SqlConnection(wsmsConnectionString))
            {
                string query = string.Format(@"select Model, sum(case when ServiceType ='StockFaulty' then 1 else 0 end) as StockFaulty,
	            count(ServiceID) as ServicePointEntry 
	            from ServiceMaster
	            where Model = '{0}'
	            group by Model", modelname);
                connection.Open();
                var sqlcommand = new SqlCommand(query, connection);
                var datareader = sqlcommand.ExecuteReader();
                List<StockFaultModel> stockfaultydata = new List<StockFaultModel>();
                while (datareader.Read())
                {
                    stockfaultydata.Add(new StockFaultModel
                    {
                        Model = Convert.ToString(datareader["Model"]),
                        ServicePointEntry = Convert.ToInt64(datareader["ServicePointEntry"]),
                        StockFaulty = Convert.ToInt64(datareader["StockFaulty"])
                    });
                }
                foreach (var row in stockfaultydata)
                {
                    reportModels.ServicePointEntry = row.ServicePointEntry;
                    reportModels.StockFault = row.StockFaulty;
                    //string model = row.Model;

                    //int index = reportModels.FindIndex(i => i.ModelName == model);
                    //if (index != -1)
                    //{
                    //    reportModels[index].ServicePointEntry = row.ServicePointEntry;
                    //    reportModels[index].StockFault = row.StockFaulty;
                    //}
                }



                datareader.Dispose();
                connection.Close();
            }
            //if (reportModels.Any())
            //{
            //    foreach (var iterator in reportModels)
            //    {

            //        iterator.StockFaultPercentage = (iterator.StockFault > 0)
            //            ? Math.Round((iterator.StockFault * 100) / iterator.TotalActivated, 2)
            //            : 0;
            //        //iterator.ReplacementParcentage = (iterator.Replacement > 0)
            //        //    ? (iterator.Replacement*100)/iterator.TotalActivated
            //        //    : 0;
            //        iterator.ServicePointEntryPercentage = (iterator.ServicePointEntry > 0)
            //            ? Math.Round((iterator.ServicePointEntry * 100) / iterator.TotalActivated, 2)
            //            : 0;
            //        iterator.TotalRetern = iterator.ReplacementPercentage + iterator.ServicePointEntryPercentage;
            //        var majorproblems = GetMajorProblems(iterator.ModelName);
            //        iterator.MajorProblems = majorproblems;
            //    }
            //}

            reportModels.StockFaultPercentage = (reportModels.StockFault > 0)
                        ? Math.Round((reportModels.StockFault * 100) / reportModels.TotalActivated, 2)
                        : 0;
            //iterator.ReplacementParcentage = (iterator.Replacement > 0)
            //    ? (iterator.Replacement*100)/iterator.TotalActivated
            //    : 0;
            reportModels.ServicePointEntryPercentage = (reportModels.ServicePointEntry > 0)
                ? Math.Round((reportModels.ServicePointEntry * 100) / reportModels.TotalActivated, 2)
                : 0;
            reportModels.TotalRetern = reportModels.ReplacementPercentage + reportModels.ServicePointEntryPercentage;
            var majorproblems = GetMajorProblems(reportModels.ModelName);
            reportModels.MajorProblems = majorproblems;

            return reportModels;
        }

        public List<MajorProblem> GetMajorProblems(string modelname)
        {
            string wsmsConnectionString = ConfigurationManager.ConnectionStrings["WSMSConnectionString"].ConnectionString;

            var majorProblems = new List<MajorProblem>();
            using (var connection = new SqlConnection(wsmsConnectionString))
            {
                string query = string.Format(@"select count(y.IssueName),
			SUM(CASE WHEN y.IssueName like '%DEAD%'  THEN 1 ELSE 0 END) AS Dead,
	        SUM(CASE WHEN y.IssueName like '%SOFTWARE%' THEN 1 ELSE 0 END) AS Software,
	        SUM(CASE WHEN y.IssueName like '%HANG%' THEN 1 ELSE 0 END) AS Hang,
	        SUM(CASE WHEN y.IssueName like '%WIFI%' THEN 1 ELSE 0 END) AS Wifi,
	        SUM(CASE WHEN y.IssueName like '%CHARGING%' THEN 1 ELSE 0 END) AS Charging,
	        SUM(CASE WHEN y.IssueName like '%BATTERY%' THEN 1 ELSE 0 END) AS Battery,
	        SUM(CASE WHEN y.IssueName like '%NETWORK%' THEN 1 ELSE 0 END) AS Network,
	        SUM(CASE WHEN y.IssueName like '%OVER HEAT%' THEN 1 ELSE 0 END) AS Overheat,
	        SUM(CASE WHEN y.IssueName like 'TOUCH%' OR y.IssueName LIKE '%LCD%' OR y.IssueName LIKE '%DISPLAY%' THEN 1 ELSE 0 END) AS Display,
	        SUM(CASE WHEN y.IssueName like '%TOUCH%' THEN 1 ELSE 0 END) AS Touch,
	        SUM(CASE WHEN y.IssueName like '%AUTO ON/OFF%' THEN 1 ELSE 0 END) AS AutoOnOff,
	        SUM(CASE WHEN y.IssueName like '%CAMERA%' THEN 1 ELSE 0 END) AS Camera,
	        y.Model
            from
            (
	            SELECT sm.ServiceID,sm.Model, i.IssueName from ServiceMaster sm
		            join ServiceIssue si
		            on sm.ServiceID = si.ServiceID
		            join Issues i
		            on si.IssueID = i.IssueID
		            where sm.Model = '{0}'
		            group by sm.ServiceID, i.IssueName, sm.Model
            )y
            group by y.Model", modelname);
                connection.Open();
                var sqlcommand = new SqlCommand(query, connection);
                var datareader = sqlcommand.ExecuteReader();
                while (datareader.Read())
                {
                    majorProblems.Add(new MajorProblem
                    {
                        AutoOnOff = Convert.ToDouble(datareader["AutoOnOff"]),
                        Battery = Convert.ToDouble(datareader["Battery"]),
                        Camera = Convert.ToDouble(datareader["Camera"]),
                        Charging = Convert.ToDouble(datareader["Charging"]),
                        Dead = Convert.ToDouble(datareader["Dead"]),
                        Display = Convert.ToDouble(datareader["Display"]),
                        Hang = Convert.ToDouble(datareader["Hang"]),
                        Network = Convert.ToDouble(datareader["Network"]),
                        Software = Convert.ToDouble(datareader["Software"]),
                        Overheat = Convert.ToDouble(datareader["Overheat"]),
                        Wifi = Convert.ToDouble(datareader["Wifi"]),
                    });
                }

                datareader.Dispose();
                connection.Close();
            }
            return majorProblems;
        }

        public List<PerMonthServiceEntryModel> GetPerMonthServiceEntry(String model)
        {
            Random rnd = new Random();
            List<PerMonthServiceEntryModel> per = new List<PerMonthServiceEntryModel>();
            String query = String.Format(@" select sm.Model,DATEPART(MONTH,sm.ServicePlaceDate) MonthStarted,DATEPART(YEAR,sm.ServicePlaceDate) YearStarted,COUNT(*) Total
             from ServiceMaster sm
             join RBSYNERGY.dbo.tblCellPhonePricing cdp
             on sm.Model=cdp.Model
             where sm.Model='{0}'
             GROUP by sm.Model,DATEPART(MONTH,sm.ServicePlaceDate),DATEPART(YEAR,sm.ServicePlaceDate)
             order By DATEPART(MONTH,sm.ServicePlaceDate),DATEPART(YEAR,sm.ServicePlaceDate)", model);
            string wsmsConnectionString = ConfigurationManager.ConnectionStrings["WSMSConnectionString"].ConnectionString;

            using (var connection = new SqlConnection(wsmsConnectionString))
            {
                connection.Open();
                var sqlcommand = new SqlCommand(query, connection);
                var datareader = sqlcommand.ExecuteReader();
                while (datareader.Read())
                {
                    String MonthStarted = Convert.ToString(datareader["MonthStarted"]);
                    if (MonthStarted.Length == 1)
                    {
                        MonthStarted = "0" + MonthStarted;
                    }
                    String YearStarted = Convert.ToString(datareader["YearStarted"]);
                    int Total = Convert.ToInt32(datareader["Total"]);
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    string dateString = MonthStarted + YearStarted;
                    string format = "MMyyyy";
                    DateTime result = DateTime.ParseExact(dateString, format, provider);
                    var test = result.ToString("MMM yy");
                    var bgcolor = String.Format("#{0:X6}", rnd.Next(0x1000000));
                    per.Add(new PerMonthServiceEntryModel
                    {
                        MonthYear = test,
                        Color = bgcolor.ToString(),
                        ServiceCount = Total
                    });
                }
                datareader.Dispose();
                connection.Close();
            }

            return per;
        }

        public MajorProblem GetMajorProblemsChartData(string modelname)
        {
            var majorproblems = GetMajorProblems(modelname).FirstOrDefault();
            return majorproblems;

        }
        #endregion

        #region Daily Sales Reports
        public DailySalesInvoicesViewModel DailySalesInvoices(string invoicedate)
        {
            DateTime datetocheck = DateTime.Parse(invoicedate);
            var yesterDay = datetocheck.AddDays(-1).ToString("yyyy-MM-dd");

            //var config = new MapperConfiguration(cfg => cfg.CreateMap<vw_DatewiseModelSold, CellPhoneDailySales>()
            //        .ForMember(d => d.InvoiceDate, o => o.MapFrom(s => s.Model != "" ? invoicedate : invoicedate))
            //        .ForMember(d => d.CellPhoneType, o => o.MapFrom(s => s.Model != "" ? "Smart" : "Smart"))
            //        .ForMember(d => d.InvoicePrice, o => o.MapFrom(s => s.Invoice_Price > 0 ? Convert.ToString(s.Invoice_Price) : "0"))
            //        .ForMember(d => d.TotalPrice, o => o.MapFrom(s => s.Total_Price > 0 ? Convert.ToString(s.Total_Price) : "0"))
            //        .ForMember(d => d.Id, o => o.MapFrom(s => s.Model != "" ? GetProductIDfromModelName(s.Model) : ""))
            //        .ForMember(d => d.ServiceToSalesRatio, o => o.MapFrom(s => s.Model != "" ? GetSalesToServiceRatio(s.Model, invoicedate, "vw_DatewiseModelSold") : "0%"))
            //        .ForMember(d => d.RemainingMarketStock, o => o.MapFrom(s => s.Model != "" ? GetRemainingMarketStock(s.Model, invoicedate) : "0")));
            //var mapper = config.CreateMapper();
            //var configfeature = new MapperConfiguration(cfg => cfg.CreateMap<vw_DatewiseFeatureModelSold, CellPhoneDailySales>()
            //        .ForMember(d => d.InvoiceDate, o => o.MapFrom(s => s.Model != "" ? invoicedate : invoicedate))
            //        .ForMember(d => d.CellPhoneType, o => o.MapFrom(s => s.Model != "" ? "Smart" : "Smart"))
            //        .ForMember(d => d.InvoicePrice, o => o.MapFrom(s => s.Invoice_Price > 0 ? Convert.ToString(s.Invoice_Price) : "0"))
            //        .ForMember(d => d.TotalPrice, o => o.MapFrom(s => s.Total_Price > 0 ? Convert.ToString(s.Total_Price) : "0"))
            //        .ForMember(d => d.Id, o => o.MapFrom(s => s.Model != "" ? GetProductIDfromModelName(s.Model) : ""))
            //        .ForMember(d => d.ServiceToSalesRatio, o => o.MapFrom(s => s.Model != "" ? GetSalesToServiceRatio(s.Model, invoicedate, "vw_DatewiseModelSold") : "0%"))
            //        .ForMember(d => d.RemainingMarketStock, o => o.MapFrom(s => s.Model != "" ? GetRemainingMarketStock(s.Model, invoicedate) : "0")));
            //var mapperfeature = configfeature.CreateMapper();
            //var configtab = new MapperConfiguration(cfg => cfg.CreateMap<vw_DatewiseTabletModelSold, CellPhoneDailySales>()
            //        .ForMember(d => d.InvoiceDate, o => o.MapFrom(s => s.Model != "" ? invoicedate : invoicedate))
            //        .ForMember(d => d.CellPhoneType, o => o.MapFrom(s => s.Model != "" ? "Smart" : "Smart"))
            //        .ForMember(d => d.InvoicePrice, o => o.MapFrom(s => s.Invoice_Price > 0 ? Convert.ToString(s.Invoice_Price) : "0"))
            //        .ForMember(d => d.TotalPrice, o => o.MapFrom(s => s.Total_Price > 0 ? Convert.ToString(s.Total_Price) : "0"))
            //        .ForMember(d => d.Id, o => o.MapFrom(s => s.Model != "" ? GetProductIDfromModelName(s.Model) : ""))
            //        .ForMember(d => d.ServiceToSalesRatio, o => o.MapFrom(s => s.Model != "" ? GetSalesToServiceRatio(s.Model, invoicedate, "vw_DatewiseModelSold") : "0%"))
            //        .ForMember(d => d.RemainingMarketStock, o => o.MapFrom(s => s.Model != "" ? GetRemainingMarketStock(s.Model, invoicedate) : "0")));
            //var mappertab = configtab.CreateMapper();


            var dailySalesInvoices = new DailySalesInvoicesViewModel();
            List<CellPhoneDailySales> smartphonesells = new List<CellPhoneDailySales>();
            List<CellPhoneDailySales> featurephonesells = new List<CellPhoneDailySales>();
            List<CellPhoneDailySales> tabsells = new List<CellPhoneDailySales>();
            string rbsConnectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            string query = string.Format(@"select RegistrationDate,sells.Model as Model,Number,Invoice_Price,Total_Price,pm.ProductID as Id, st.RemainingMarketStock as Stock, ratio.ServiceToSalesRatio from
                                         ((select RegistrationDate, Model, Number,Invoice_Price,Total_Price from vw_DatewiseModelSold where RegistrationDate = '{0}') sells
                                         inner join tblProductMaster pm on pm.ProductModel=sells.Model 
                                         inner join (select Model, RemainingMarketStock from DispatchToMarketRemainingStock where DateAdded='{1}') st
                                         on st.Model=sells.Model
                                         left join (SELECT Model,ServiceToSalesRatio from ServiceToSalesRatio where DateAdded='{2}') ratio
                                         on ratio.Model = sells.Model) order by Model desc", invoicedate, yesterDay, yesterDay);
            using (var connection = new SqlConnection(rbsConnectionString))
            {
                connection.Open();
                var sqlcommand = new SqlCommand(query, connection);
                var datareader = sqlcommand.ExecuteReader();
                string tableName = "vw_DatewiseModelSold";
                while (datareader.Read())
                {
                    smartphonesells.Add(new CellPhoneDailySales
                    {
                        RegistrationDate = Convert.ToString(datareader["RegistrationDate"]),
                        InvoiceDate = invoicedate,
                        CellPhoneType = "Smart",
                        Model = Convert.ToString(datareader["Model"]),
                        Number = Convert.ToInt32(datareader["Number"]),
                        InvoicePrice = Convert.ToString(datareader["Invoice_Price"]),
                        TotalPrice = Convert.ToString(datareader["Total_Price"]),
                        Id = Convert.ToString(datareader["Id"]),
                        ServiceToSalesRatio = (datareader["ServiceToSalesRatio"] != System.DBNull.Value ? Convert.ToDecimal(datareader["ServiceToSalesRatio"]).ToString("#.###") : "0") + "%",
                        RemainingMarketStock = Convert.ToString(datareader["Stock"])
                    });
                }
                datareader.Dispose();
                connection.Close();
            }
            //var smartdata = _dbrbsyEntities.vw_DatewiseModelSold.Where(x => x.RegistrationDate == datetocheck).OrderByDescending(y => y.Model).ToList();
            //smartphonesells = mapper.Map<List<CellPhoneDailySales>>(smartdata);

            using (var connection = new SqlConnection(rbsConnectionString))
            {
                connection.Open();
                string fquery = string.Format(@"select RegistrationDate,sells.Model as Model,Number,Invoice_Price,Total_Price,pm.ProductID as Id, st.RemainingMarketStock as Stock, ratio.ServiceToSalesRatio from
                                         ((select RegistrationDate, Model, Number,Invoice_Price,Total_Price from vw_DatewiseFeatureModelSold where RegistrationDate = '{0}') sells
                                         inner join tblProductMaster pm on pm.ProductModel=sells.Model 
                                         inner join (select Model, RemainingMarketStock from DispatchToMarketRemainingStock where DateAdded='{1}') st
                                         on st.Model=sells.Model
                                         left join (SELECT Model,ServiceToSalesRatio from ServiceToSalesRatio where DateAdded='{2}') ratio
                                         on ratio.Model = sells.Model) order by Model desc", invoicedate, yesterDay, yesterDay);

                var sqlcommand = new SqlCommand(fquery, connection);
                var datareader = sqlcommand.ExecuteReader();
                string tableName = "vw_DatewiseFeatureModelSold";
                while (datareader.Read())
                {
                    featurephonesells.Add(new CellPhoneDailySales
                    {
                        RegistrationDate = Convert.ToString(datareader["RegistrationDate"]),
                        InvoiceDate = invoicedate,
                        CellPhoneType = "Feature",
                        Model = Convert.ToString(datareader["Model"]),
                        Number = Convert.ToInt32(datareader["Number"]),
                        InvoicePrice = Convert.ToString(datareader["Invoice_Price"]),
                        TotalPrice = Convert.ToString(datareader["Total_Price"]),
                        Id = Convert.ToString(datareader["Id"]),
                        ServiceToSalesRatio = (datareader["ServiceToSalesRatio"] != System.DBNull.Value ? Convert.ToDecimal(datareader["ServiceToSalesRatio"]).ToString("#.###") : "0") + "%",
                        RemainingMarketStock = Convert.ToString(datareader["Stock"])
                    });
                }
                datareader.Dispose();
                connection.Close();
            }
            //var featuredata = _dbrbsyEntities.vw_DatewiseFeatureModelSold.Where(x => x.RegistrationDate == datetocheck).OrderByDescending(y => y.Model).ToList();
            //featurephonesells = mapperfeature.Map<List<CellPhoneDailySales>>(featuredata);


            using (var connection = new SqlConnection(rbsConnectionString))
            {
                connection.Open();
                string fquery = string.Format(@"select RegistrationDate,sells.Model as Model,Number,Invoice_Price,Total_Price,pm.ProductID as Id, st.RemainingMarketStock as Stock, ratio.ServiceToSalesRatio from
                                         ((select RegistrationDate, Model, Number,Invoice_Price,Total_Price from vw_DatewiseTabletModelSold where RegistrationDate = '{0}') sells
                                         inner join tblProductMaster pm on pm.ProductModel=sells.Model 
                                         inner join (select Model, RemainingMarketStock from DispatchToMarketRemainingStock where DateAdded='{1}') st
                                         on st.Model=sells.Model
                                         left join (SELECT Model,ServiceToSalesRatio from ServiceToSalesRatio where DateAdded='{2}') ratio
                                         on ratio.Model = sells.Model) order by Model desc", invoicedate, yesterDay, yesterDay);
                var sqlcommand = new SqlCommand(fquery, connection);
                var datareader = sqlcommand.ExecuteReader();
                string tableName = "vw_DatewiseTabletModelSold";
                while (datareader.Read())
                {
                    tabsells.Add(new CellPhoneDailySales
                    {
                        RegistrationDate = Convert.ToString(datareader["RegistrationDate"]),
                        InvoiceDate = invoicedate,
                        CellPhoneType = "Tablet",
                        Model = Convert.ToString(datareader["Model"]),
                        Number = Convert.ToInt32(datareader["Number"]),
                        InvoicePrice = Convert.ToString(datareader["Invoice_Price"]),
                        TotalPrice = Convert.ToString(datareader["Total_Price"]),
                        Id = Convert.ToString(datareader["Id"]),
                        ServiceToSalesRatio = (datareader["ServiceToSalesRatio"] != System.DBNull.Value ? Convert.ToDecimal(datareader["ServiceToSalesRatio"]).ToString("#.###") : "0") + "%",
                        RemainingMarketStock = Convert.ToString(datareader["Stock"])
                    });
                }
                datareader.Dispose();
                connection.Close();
            }

            //var tabdata = _dbrbsyEntities.vw_DatewiseTabletModelSold.Where(x => x.RegistrationDate == datetocheck).OrderByDescending(y => y.Model).ToList();
            //tabsells = mappertab.Map<List<CellPhoneDailySales>>(tabdata);

            dailySalesInvoices.SumSmartQunatity = smartphonesells.Sum(x => x.Number);
            dailySalesInvoices.SumSmartPrice = smartphonesells.Sum(i => Convert.ToDecimal(i.TotalPrice)).ToString("#,##0.00");
            dailySalesInvoices.SmartPhoneDailySales = smartphonesells;

            dailySalesInvoices.SumFeatureQunatity = featurephonesells.Sum(x => x.Number);
            dailySalesInvoices.SumFeaturePrice = featurephonesells.Sum(i => Convert.ToDecimal(i.TotalPrice)).ToString("#,##0.00");
            dailySalesInvoices.FeaturePhoneDailySales = featurephonesells;

            dailySalesInvoices.SumTabQunatity = tabsells.Sum(x => x.Number);
            dailySalesInvoices.SumTabPrice = tabsells.Sum(i => Convert.ToDecimal(i.TotalPrice)).ToString("#,##0.00");
            dailySalesInvoices.TabletDailySales = tabsells;

            dailySalesInvoices.GrandTotalQunatity = dailySalesInvoices.SumSmartQunatity + dailySalesInvoices.SumFeatureQunatity + dailySalesInvoices.SumTabQunatity;
            //dailySalesInvoices.GrandTotalPrice = (smartphonesells.Sum(i => Convert.ToDecimal(i.TotalPrice)) + featurephonesells.Sum(i => Convert.ToDecimal(i.TotalPrice)) + tabsells.Sum(i => Convert.ToDecimal(i.TotalPrice))).ToString("#,##0.00");
            dailySalesInvoices.GrandTotalPrice = dailySalesInvoices.SumSmartPrice + dailySalesInvoices.SumFeaturePrice + dailySalesInvoices.SumTabPrice;

            return dailySalesInvoices;
        }

        protected string GetProductIDfromModelName(string Model)
        {
            string rbsConnectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            using (var connection = new SqlConnection(rbsConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT ProductID from tblProductMaster where ProductModel='" + Model + "'", connection))
                {

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 20000;
                    string ProductID = cmd.ExecuteScalar().ToString();
                    return ProductID;
                }
            }
        }

        public string GetSalesToServiceRatio(string model, String presentDate, string tableName)
        {

            string rbsConnectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            var modelName = model;
            var date = Convert.ToDateTime(presentDate);
            var yesterDay = date.AddDays(-1).ToString("yyyy-MM-dd");
            using (var connection = new SqlConnection(rbsConnectionString))
            {
                connection.Open();

                var checkDate = CheckDateinServiceToSalesRatioTable(yesterDay);

                var checkMobileModel = CheckModelExistenceinServiceToSalesRatioTable(model, yesterDay);
                if (checkDate == true && checkMobileModel == true)
                {
                    using (
                        SqlCommand cmd =
                            new SqlCommand(
                                "SELECT ServiceToSalesRatio from ServiceToSalesRatio where Model='" + model +
                                "' and DateAdded='" + yesterDay + "'", connection))
                    {
                        decimal coDecimal;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 20000;

                        string serviceToSalesRatio = cmd.ExecuteScalar().ToString();


                        decimal.TryParse(serviceToSalesRatio, out coDecimal);
                        var formattedToDecimal = Math.Round(coDecimal, 3);
                        var returnResult = formattedToDecimal + "" + '%';
                        return returnResult.ToString();




                    }
                }
                else
                {
                    decimal coDecimal;


                    string serviceToSalesRatio = "0";


                    decimal.TryParse(serviceToSalesRatio, out coDecimal);
                    var formattedToDecimal = Math.Round(coDecimal, 3);
                    var returnResult = formattedToDecimal + "" + '%';
                    return returnResult.ToString();

                }

            }
            return "";
        }

        public Boolean CheckDateinServiceToSalesRatioTable(string addedDate)
        {
            string rbsConnectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            Boolean checkDate = false;
            using (var connection = new SqlConnection(rbsConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Count(*) from  ServiceToSalesRatio where   DateAdded='" + addedDate + "'", connection))
                {

                    //  cmd.CommandType = CommandType.Text;

                    var AddedDate = cmd.ExecuteScalar().ToString();

                    if (AddedDate != "0")
                    {
                        checkDate = true;
                    }

                    return checkDate;
                }
            }



            return checkDate;
        }
        public Boolean CheckModelExistenceinServiceToSalesRatioTable(string Model, string addedDate)
        {
            string rbsConnectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            Boolean checkModel = false;
            using (var connection = new SqlConnection(rbsConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Count(*) from  ServiceToSalesRatio where   DateAdded='" + addedDate + "' and  Model='" + Model + "'", connection))
                {

                    //  cmd.CommandType = CommandType.Text;

                    var checkModelInTable = cmd.ExecuteScalar().ToString();

                    if (checkModelInTable != "0")
                    {
                        checkModel = true;
                    }

                    return checkModel;
                }
            }



            return checkModel;
        }
        public string GetRemainingMarketStock(string model, String presentDate)
        {
            string rbsConnectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            var table = "DispatchToMarketRemainingStock";
            var modelName = model;
            var date = Convert.ToDateTime(presentDate);
            var yesterDay = date.AddDays(-1).ToString("yyyy-MM-dd");
            using (var connection = new SqlConnection(rbsConnectionString))
            {
                connection.Open();

                var checkDate = CheckDateinDispatchToMarketRemainingStock(yesterDay);

                var checkMobileModel = CheckModelExistenceinDispatchToMarketRemainingStock(model, yesterDay);
                if (checkDate == true && checkMobileModel == true)
                {
                    using (
                        SqlCommand cmd =
                            new SqlCommand(
                                "SELECT RemainingMarketStock from DispatchToMarketRemainingStock where Model='" + model +
                                "' and DateAdded='" + yesterDay + "'", connection))
                    {
                        decimal coDecimal;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 20000;

                        string serviceToSalesRatio = cmd.ExecuteScalar().ToString();
                        return serviceToSalesRatio;
                    }
                }
                else
                {
                    string serviceToSalesRatio = "0";
                    return serviceToSalesRatio;

                }

            }
            return "";
        }
        public Boolean CheckDateinDispatchToMarketRemainingStock(string addedDate)
        {
            string rbsConnectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            Boolean checkDate = false;
            using (var connection = new SqlConnection(rbsConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Count(*) from  DispatchToMarketRemainingStock where   DateAdded='" + addedDate + "'", connection))
                {

                    //  cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 10000;

                    var AddedDate = cmd.ExecuteScalar().ToString();

                    if (AddedDate != "0")
                    {
                        checkDate = true;
                    }

                    return checkDate;
                }
            }



            return checkDate;
        }

        public Boolean CheckModelExistenceinDispatchToMarketRemainingStock(string Model, string addedDate)
        {
            string rbsConnectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            Boolean checkModel = false;
            using (var connection = new SqlConnection(rbsConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Count(*) from  DispatchToMarketRemainingStock where   DateAdded='" + addedDate + "' and  Model='" + Model + "'", connection))
                {

                    //  cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 20000;

                    var checkModelInTable = cmd.ExecuteScalar().ToString();

                    if (checkModelInTable != "0")
                    {
                        checkModel = true;
                    }

                    return checkModel;
                }
            }



            return checkModel;
        }

        public List<ModelColorWiseDailySalesViewModel> GetColorWiseActivatedModelNumber(string pid, string invoicedate)
        {
            List<ModelColorWiseDailySalesViewModel> dailySalesInvoices = new List<ModelColorWiseDailySalesViewModel>();

            string rbsConnectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(rbsConnectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("GetColorWiseActivatedModelNumber", con))
                {
                    var productId = pid;
                    var date = Convert.ToDateTime(invoicedate.ToString()).ToString("yyyy-MM-dd");

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    cmd.Parameters.AddWithValue("@RegDate", date);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        dailySalesInvoices.Add(new ModelColorWiseDailySalesViewModel
                        {
                            ModelName = Convert.ToString(reader["Model"]),
                            Color = Convert.ToString(reader["Color"]),
                            Activated = Convert.ToInt32(reader["NumberActivated"])
                        });
                    }
                }
            }
            return dailySalesInvoices;
        }

        public List<ModelWIseDailySalesByDealerTypeViewModel> GetModelWiseADealerType(string pid, string invoicedate)
        {
            string rbsConnectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            List<ModelWIseDailySalesByDealerTypeViewModel> DailySalesDealerTypeWises = new List<ModelWIseDailySalesByDealerTypeViewModel>();
            string tableName = "";
            var tuple = GetProductNameByProductId(pid);
            var modelName = tuple.Item1;
            var type = tuple.Item2;

            switch (type)
            {
                case "Smart":
                    tableName = "vw_DatewiseModelSold";
                    break;
                case "Feature":
                    tableName = "vw_DatewiseFeatureModelSold";
                    break;
                case "Tablet":
                    tableName = "vw_DatewiseTabletModelSold";
                    break;
            }
            using (var connection = new SqlConnection(rbsConnectionString))
            {

                connection.Open();
                string dailywiseModelWiseQuery = "";

                dailywiseModelWiseQuery = string.Format(@"select x.Total - (x.DealerActivation+PlazaActivation) as UnKnownActivation, x.PlazaActivation, x.DealerActivation from
                  ( select 
			            (select count(*) 
            from tblProductMaster pm 
            inner join  tblProductRegistration tpr on pm.ProductID=tpr.ProductModelID 
            where RegistrationDate = '{1}' and pm.ProductModel like '%{0}' )as Total,
			(select sum(TotalNumber) from vw_DatewiseTotalDealerModSold  where Model like '%{0}' and RegistrationDate = '{1}' and DealerType='Dealer'		)as DealerActivation,
			 (select sum(TotalNumber) from vw_DatewiseTotalDealerModSold  where Model like '%{0}' and RegistrationDate = '{1}'	and DealerType='Plaza'		)as PlazaActivation																																			
                        
                )x", modelName, invoicedate);

                var command = new SqlCommand(dailywiseModelWiseQuery, connection);
                command.CommandTimeout = 200;

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var obj = new ModelWIseDailySalesByDealerTypeViewModel
                    {
                        ModelName = modelName,
                        DealerActivationQuantity = Convert.ToString(reader["DealerActivation"]),
                        PlazaActivationQuantity = Convert.ToString(reader["PlazaActivation"]),
                        UnknownActivationQuantity = Convert.ToString(reader["UnKnownActivation"])
                    };

                    DailySalesDealerTypeWises.Add(obj);
                }

                connection.Close();
            }
            return DailySalesDealerTypeWises;
        }
        private Tuple<string, string> GetProductNameByProductId(string productId)
        {
            string rbsConnectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            string modelName = "";
            string type = "";
            using (var connection = new SqlConnection(rbsConnectionString))
            {
                connection.Open();
                string getProductName = "";

                getProductName = string.Format(@"select ProductModel,Category2 from tblProductMaster where ProductId='{0}'", productId);


                var command = new SqlCommand(getProductName, connection);

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {

                    modelName = (string)reader["ProductModel"];
                    type = Convert.ToString(reader["Category2"]);

                }
                connection.Close();
            }


            return new Tuple<string, string>(modelName, type);

        }

        public RemainingMarketStockDealerWiseViewModel GetRemainingStock(string type, string modelname, string invoicedate)
        {
            RemainingMarketStockDealerWiseViewModel vm = new RemainingMarketStockDealerWiseViewModel();
            List<RemainingStocksDetailsModel> RemainingStocks = new List<RemainingStocksDetailsModel>();
            string rbsConnectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            var previousDay = Convert.ToDateTime(invoicedate).AddDays(-1).ToString("yyyy-MM-dd");
            using (var connection = new SqlConnection(rbsConnectionString))
            {
                connection.Open();
                string allPrimoModelQuery = "";
                string tableName = "RemainingMarketDetails";

                allPrimoModelQuery = string.Format(@"SELECT * from RemainingMarketDetails  where Model = '{0}' and AddedDate = '{1}' and RemainingStock <>0", modelname, previousDay);
                var command = new SqlCommand(allPrimoModelQuery, connection);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var stockDetails = new RemainingStocksDetailsModel()
                    {
                        DealerCode = Convert.ToInt64(reader["DealerCode"]).ToString(),
                        ModelName = (string)reader["Model"],
                        RemainingStock = Convert.ToInt64(reader["RemainingStock"]),
                        DealerName = Convert.ToString(reader["DealerName"]),
                        DealerCity = Convert.ToString(reader["DealerCity"]),
                        DealerType = Convert.ToString(reader["DealerType"]) == null ? " " : Convert.ToString(reader["DealerType"]),

                    };
                    RemainingStocks.Add(stockDetails);
                }
                connection.Close();
                vm.TotalRemainingStock = RemainingStocks.Sum(x => x.RemainingStock).ToString();
                vm.RemainingStocksDetails = RemainingStocks;
            }
            return vm;
        }
        public ChartGraphforDailySalesViewModel GetHighChartGraphforDailySales(string id, string invoicedate)
        {
            List<HighChartDataModel> data = new List<HighChartDataModel>();
            ChartGraphforDailySalesViewModel vm = new ChartGraphforDailySalesViewModel();
            String connectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;

            var tuple = GetProductNameByProductId(id);
            var modelName = tuple.Item1;
            var type = tuple.Item2;

            string table = "";

            switch (type)
            {
                case "Smart":
                    table = "vw_DatewiseModelSold";
                    break;
                case "Feature":
                    table = "vw_DatewiseFeatureModelSold";
                    break;
                case "Tablet":
                    table = "vw_DatewiseTabletModelSold";
                    break;
            }
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string getSmartPhoneDailySales = "";

                getSmartPhoneDailySales =
                    string.Format(
                        @"select RegistrationDate,Number,Total_Price from {2} where Model='{0}' and RegistrationDate between '2013-01-01' and '{1}'order by  RegistrationDate asc",
                        modelName, invoicedate, table);


                var command = new SqlCommand(getSmartPhoneDailySales, connection);

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    DateTime date = Convert.ToDateTime(reader["RegistrationDate"]);
                    var dtJsStart = new System.DateTime(1970, 01, 01);
                    var miliseconds = (date - dtJsStart).TotalMilliseconds;

                    var obj = new HighChartDataModel
                    {
                        Date = date.ToString("yyyy-MMM-dd"),
                        DateInJs = (long)miliseconds,
                        quantity = Convert.ToInt32(reader["Number"]),
                        totalPrice = Convert.ToInt64(reader["Total_Price"]),
                        RealDate = date
                    };

                    data.Add(obj);
                }
                connection.Close();
            }
            data = data.OrderBy(x => x.RealDate).ToList();
            vm.ModelName = modelName;
            vm.HighChartData = data;
            return vm;
        }

        public NewMajorMinorIssuesViewModel MajorMinorIssuesViewModel(string modelname, string order)
        {
            NewMajorMinorIssuesViewModel vm = new NewMajorMinorIssuesViewModel();
            string wsmsConnectionString = ConfigurationManager.ConnectionStrings["WSMSConnectionString"].ConnectionString;
            string resultQuery = "";
            List<SpecificModelReportModel> specificmodelreport = new List<SpecificModelReportModel>();
            using (var connection = new SqlConnection(wsmsConnectionString))
            {
                connection.Open();

                if (order == "ALL")
                {
                    resultQuery = String.Format(@"select allCountWithModel.Model, allCountWithModel.TotalReceive, allCountWithModel.NonWarenty, allCountWithModel.Warrenty, allCountWithModel.SparePartsPending, allCountWithModel.WorkPending, allCountWithModel.TotalPending, case when serviceDone.TotalWorkDone is null then 0 else serviceDone.TotalWorkDone end as TotalWorkDone,
                (select  top 1 
                ((10*Tendays+20*Twentydays+40*Fortydays+60*Sixtydays+90*Nintydays+180*OneEightydays+270*Twoseventy+365*ThreeSixtyFive+RestoftheDays)/
                (Tendays+Twentydays+Fortydays+Sixtydays+Nintydays+OneEightydays+Twoseventy+ThreeSixtyFive+RestoftheDays)) 
                    from RBSYNERGY.dbo.ModelWiseServiceQuantityInDays where Model='{0}' order by AddedDate desc)  as AverageReturnTime
                from 
                (
	                select model.ProductModel, serviceInfo.* from 
	                (
		                SELECT Distinct ProductModel
		                FROM [RBSYNERGY].[dbo].[tblProductMaster]
		                where ProductType='Cell Phone'
		                and Inactive=0
		                           
	                ) model


	                left join

	                (
		                select  Model,
		                sum(case when (WarrantyAvailable=0 or WarrantyAvailable=1 or WarrantyAvailable is NULL)  then 1 else 0 end) as TotalReceive,
		                sum(case when WarrantyAvailable=0  then 1 else 0 end) as NonWarenty,
		                sum(case when (WarrantyAvailable=1 or WarrantyAvailable is NULL) then 1 else 0 end) as Warrenty,
		                sum(case when ServiceStatus ='PartsUnAvailable'  then 1 else 0 end) as SparePartsPending,
		                sum(case when ServiceStatus not in ('QCPassed','Delivery','ApprovedReq','Deliverable','Delivered','Terminated','PartsUnAvailable','DecisionPending','Transfered', 'Returned', 'SetReturned')  then 1 else 0 end) as WorkPending,
		                sum(case when (ServiceStatus  not in ('QCPassed','Delivery','ApprovedReq','Deliverable','Delivered','Terminated','DecisionPending', 'Returned', 'SetReturned', 'Replacement', 'ReplacementDelivered'))  then 1 else 0 end) as TotalPending

		                from ServiceMaster ss where  ss.ServicePointID not in (69) and (ss.IME in (select barcode from RBSYNERGY.dbo.tblBarCodeInv tbc where  tbc.Model='{0}')
						or ss.IME in (select barcode2 from RBSYNERGY.dbo.tblBarCodeInv tbc where  tbc.Model='{0}'))
		                group by Model

	                ) serviceInfo
	                on model.ProductModel = serviceInfo.Model WHERE serviceInfo.Model='{0}'
                ) allCountWithModel

                left join
                (
	                select Model,COUNT(*) AS [TotalWorkDone]
	                from ServiceTimeLog STL,ServiceMaster SM
	                where STL.ServiceID=SM.ServiceID  and  SM.ServicePointID not in (69) and QCReleaseStatus ='QCPassed' and (SM.IME in (select barcode from RBSYNERGY.dbo.tblBarCodeInv tbc where  tbc.Model='{0}')
						or SM.IME in (select barcode2 from RBSYNERGY.dbo.tblBarCodeInv tbc where  tbc.Model='{0}'))
	                group by Model
                ) serviceDone
                on allCountWithModel.Model = serviceDone.Model", modelname);
                }
                else
                {
                    resultQuery = String.Format(@"(select allCountWithModel.Model, allCountWithModel.TotalReceive, allCountWithModel.NonWarenty, allCountWithModel.Warrenty, allCountWithModel.SparePartsPending, allCountWithModel.WorkPending, allCountWithModel.TotalPending, 
                case when serviceDone.TotalWorkDone is null then 0 else serviceDone.TotalWorkDone end as TotalWorkDone,(select  top 1 
                ((10*Tendays+20*Twentydays+40*Fortydays+60*Sixtydays+90*Nintydays+180*OneEightydays+270*Twoseventy+365*ThreeSixtyFive+RestoftheDays)/
                (Tendays+Twentydays+Fortydays+Sixtydays+Nintydays+OneEightydays+Twoseventy+ThreeSixtyFive+RestoftheDays))
                 from RBSYNERGY.dbo.ModelWiseServiceQuantityInDaysByOrder where Model='{0}' and Orders='{1}' order by AddedDate desc)  as AverageReturnTime  from 
                (
	                select model.ProductModel, serviceInfo.* from 
	                (
		                SELECT Distinct ProductModel
		                FROM [RBSYNERGY].[dbo].[tblProductMaster]
		                where ProductType='Cell Phone'
		                and Inactive=0
		                           
	                ) model
	                left join
	                (
		                select  ss.Model,
		                sum(case when (WarrantyAvailable=0 or WarrantyAvailable=1 or WarrantyAvailable is NULL)  then 1 else 0 end) as TotalReceive,
		                sum(case when WarrantyAvailable=0  then 1 else 0 end) as NonWarenty,
		                sum(case when (WarrantyAvailable=1 or WarrantyAvailable is NULL) then 1 else 0 end) as Warrenty,
		                sum(case when ServiceStatus ='PartsUnAvailable'  then 1 else 0 end) as SparePartsPending,
		                sum(case when ServiceStatus not in ('QCPassed','Delivery','ApprovedReq','Deliverable','Delivered','Terminated','PartsUnAvailable','DecisionPending','Transfered', 'Returned', 'SetReturned')  then 1 else 0 end) as WorkPending,
		                sum(case when (ServiceStatus  not in ('QCPassed','Delivery','ApprovedReq','Deliverable','Delivered','Terminated','DecisionPending', 'Returned', 'SetReturned', 'Replacement', 'ReplacementDelivered'))  then 1 else 0 end) as TotalPending
		                
                        from ServiceMaster  ss where ss.ServicePointID not in (69) and (ss.IME in (select barcode from RBSYNERGY.dbo.tblBarCodeInv tbc where  tbc.Updatedby='{1}' and tbc.Model='{0}')
						or ss.IME in (select barcode2 from RBSYNERGY.dbo.tblBarCodeInv tbc where  tbc.Updatedby='{1}' and tbc.Model='{0}')
						)
		                group by ss.Model

	                ) serviceInfo
	                on model.ProductModel = serviceInfo.Model WHERE serviceInfo.Model='{0}'  
                ) allCountWithModel

                left join
                (
	                select SM.Model,COUNT(*) AS [TotalWorkDone]
	                from ServiceTimeLog STL,ServiceMaster SM
	                where STL.ServiceID=SM.ServiceID and  SM.ServicePointID not in (69) and QCReleaseStatus ='QCPassed' and 
					(SM.IME  in (select barcode from RBSYNERGY.dbo.tblBarCodeInv tbc where  tbc.Updatedby='{1}' and tbc.Model='{0}')
					or  SM.IME  in (select barcode2 from RBSYNERGY.dbo.tblBarCodeInv tbc where  tbc.Updatedby='{1}' and tbc.Model='{0}')
					)


	                group by SM.Model
                ) serviceDone
                on allCountWithModel.Model = serviceDone.Model)", modelname, order);
                }

                var command = new SqlCommand(resultQuery, connection);
                command.CommandTimeout = 6000;
                SqlDataReader dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {

                    var report = new SpecificModelReportModel();
                    report.ModelName = Convert.ToString(dataReader["Model"]);
                    report.TotalReceive = Convert.ToString(dataReader["TotalReceive"]);
                    report.NonWarranty = Convert.ToString(dataReader["NonWarenty"]);
                    report.Warrenty = Convert.ToString(dataReader["Warrenty"]);
                    report.SparePartsPending = Convert.ToString(dataReader["SparePartsPending"]);
                    report.WorkPending = Convert.ToString(dataReader["WorkPending"]);
                    report.TotalPending = Convert.ToString(dataReader["TotalPending"]);
                    report.TotalWorkDone = Convert.ToString(dataReader["TotalWorkDone"]);
                    report.AverageReturnTime = Convert.ToString(dataReader["AverageReturnTime"]);
                    report.orders = order;
                    report.ReportType = "";
                    specificmodelreport.Add(report);
                }
                connection.Close();
            }
            vm.ModelName = modelname;
            vm.Order = order;
            vm.TotalReceiveCount = specificmodelreport.Sum(i => i.TotalReceive != null ? Convert.ToDouble(i.TotalReceive) : 0);
            vm.NonWarrantyCount = specificmodelreport.Sum(i => i.NonWarranty != null ? Convert.ToDouble(i.NonWarranty) : 0);
            vm.WarrentyCount = specificmodelreport.Sum(i => i.Warrenty != null ? Convert.ToDouble(i.Warrenty) : 0);
            vm.SparePartsPendingCount = specificmodelreport.Sum(i => i.SparePartsPending != null ? Convert.ToDouble(i.SparePartsPending) : 0);
            vm.WorkPendingCount = specificmodelreport.Sum(i => i.WorkPending != null ? Convert.ToDouble(i.WorkPending) : 0);
            vm.TotalPendingCount = specificmodelreport.Sum(i => i.TotalPending != null ? Convert.ToDouble(i.TotalPending) : 0);
            vm.TotalWorkDoneCount = specificmodelreport.Sum(i => i.TotalWorkDone != null ? Convert.ToDouble(i.TotalWorkDone) : 0);
            vm.TotalAverageReturnTimeCount = specificmodelreport.Sum(i => i.AverageReturnTime != null ? Convert.ToDouble(i.AverageReturnTime) : 0);
            return vm;
        }
        public List<Order> GetOrdersOfModel(String model)
        {
            List<Order> uniqueOrder = new List<Order>();
            uniqueOrder.Add(new Order { OrderName = "ALL" });
            string rbsConnectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            using (var connection = new SqlConnection(rbsConnectionString))
            {
                connection.Open();
                string resultQuery =
                    String.Format(@"select  tbc.Updatedby from RBSYNERGY.dbo.tblBarCodeInv tbc where tbc.Model='{0}' group by tbc.Updatedby
                 order by CAST(SUBSTRING(tbc.Updatedby + '0', PATINDEX('%[0-9]%', tbc.Updatedby + '0'), LEN(tbc.Updatedby + '0')) AS INT) ", model);
                var command = new SqlCommand(resultQuery, connection);
                command.CommandTimeout = 6000;
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var item = new Order
                    {
                        OrderName = reader["Updatedby"].ToString()
                    };
                    uniqueOrder.Add(item);
                }
                connection.Close();

            }
            return uniqueOrder;
        }


        public List<PieChartDataForIssueName> GetMajorIssueChartsByMonthWiseServiceQuantity(string model1, string orders1)
        {
            List<PieChartDataForIssueName> data = new List<PieChartDataForIssueName>();
            string wsmsConnectionString = ConfigurationManager.ConnectionStrings["WSMSConnectionString"].ConnectionString;
            List<PieChartDataForIssueName> finaldata = new List<PieChartDataForIssueName>();
            using (var connection = new SqlConnection(wsmsConnectionString))
            {
                connection.Open();
                string getPieChartDataForIssueName = "";
                if (orders1 == "ALL")
                {

                    getPieChartDataForIssueName =
                        string.Format(@"SELECT IssueType,IssueName,COUNT(distinct ServiceID) as TotalMinorProblemQTY
                    FROM (select distinct I.IssueType,SI.IssueID,sm.ServiceID,SM.IME,i.IssueName
                    from WSMS.dbo.ServiceIssue SI,WSMS.dbo.Issues I,WSMS.dbo.ServiceMaster SM 
                    where SI.IssueID=I.IssueID 
                    and SI.ServiceID =SM.ServiceID and SM.ServicePointID not in (69) and
                    SM.Model='{0}' and (SM.IME in (select  barcode from RBSYNERGY.dbo.tblBarCodeInv tbc where   tbc.Model='{0}') 
                    or SM.IME in (select  barcode2 from RBSYNERGY.dbo.tblBarCodeInv tbc where  tbc.Model='{0}') )  group by  I.IssueName,I.IssueType,SM.IME,sm.ServiceID,SI.IssueID
                    ) as dt
                    Group by IssueType,IssueName order by TotalMinorProblemQTY", model1);

                }
                else
                {
                    getPieChartDataForIssueName =
                      string.Format(@"SELECT IssueType,IssueName,COUNT(distinct ServiceID) as TotalMinorProblemQTY
                    FROM (select distinct I.IssueType,SI.IssueID,SM.ServiceID,SM.IME,i.IssueName
                    from WSMS.dbo.ServiceIssue SI,WSMS.dbo.Issues I,WSMS.dbo.ServiceMaster SM 
                    where SI.IssueID=I.IssueID 
                    and SI.ServiceID =SM.ServiceID and SM.ServicePointID not in (69) and
                    SM.Model='{0}' and (SM.IME in (select  barcode from RBSYNERGY.dbo.tblBarCodeInv tbc where  tbc.Updatedby='{1}' and tbc.Model='{0}') 
                    or SM.IME in (select  barcode2 from RBSYNERGY.dbo.tblBarCodeInv tbc where tbc.Updatedby='{1}' and  tbc.Model='{0}') ) 
                        group by  I.IssueName,I.IssueType,SM.IME,sm.ServiceID,SI.IssueID
                    ) as dt
                    Group by IssueType,IssueName order by TotalMinorProblemQTY ", model1, orders1);
                }
                var command = new SqlCommand(getPieChartDataForIssueName, connection);

                command.CommandTimeout = 6000;
                SqlDataReader reader = command.ExecuteReader();
                double total1 = 0;
                while (reader.Read())
                {
                    var obj = new PieChartDataForIssueName();

                    obj.IssueType = reader["IssueType"].ToString();
                    obj.IssueName = reader["IssueName"].ToString();
                    obj.TotalMinorProblemQTY = reader["TotalMinorProblemQTY"].ToString();
                    obj.TotalMinorProblemQTY1 = Convert.ToDouble(reader["TotalMinorProblemQTY"]);
                    total1 = total1 + Convert.ToDouble(obj.TotalMinorProblemQTY);
                    obj.TotalProblem = total1;
                    data.Add(obj);

                }

                finaldata = data.GroupBy(l => new { l.IssueType })
                  .Select(g => new PieChartDataForIssueName
                  {
                      IssueType = g.Key.IssueType,
                      IssueName = string.Join(",", g.Select(i => i.IssueName)),
                      TotalMinorProblemQTY = string.Join(",", g.Select(i => i.TotalMinorProblemQTY)),
                      TotalProblem = total1,
                      TotalMajorProblemQTY = g.Sum(i => i.TotalMinorProblemQTY1)
                  }).OrderByDescending(i => i.TotalMajorProblemQTY).ToList();
                reader.Close();

            }
            return finaldata;
        }
        public List<PieChartDataForSpare> GetSpareChartsByMonthWiseServiceQuantity(string model1, string orders1)
        {
            List<PieChartDataForSpare> data = new List<PieChartDataForSpare>();
            string wsmsConnectionString = ConfigurationManager.ConnectionStrings["WSMSConnectionString"].ConnectionString;
            List<PieChartDataForSpare> finaldata = new List<PieChartDataForSpare>();
            using (var connection = new SqlConnection(wsmsConnectionString))
            {
                connection.Open();
                string getPieChartDataForIssueName = "";

                if (orders1 == "ALL")
                {

                    getPieChartDataForIssueName =
                         string.Format(@"select rd.ItemName, sps.ItemDesc,COUNT(distinct sm.IME) As TotalSpareUsedMinor
                    from Requisition R, RequisitionDetail RD, WSMS.dbo.ServiceMaster SM,wsms.dbo.SpareParts sps 
                    where R.RequisitionID=RD.RequisitionID and rd.ItemCode =sps.ItemCode
                    AND RD.ItemStatus='Used' 
                    AND RD.ItemCode not in ('') AND RD.ItemCode is not null  AND RD.ItemName not in ('') AND RD.ItemName is not null
                    and R.ServiceID =SM.ServiceID and sps.Model='{0}' and SM.ServicePointID not in (69) and sm.Model='{0}'   and (sm.IME in (select  barcode from RBSYNERGY.dbo.tblBarCodeInv tbc where  tbc.Model='{0}')
                    or sm.IME in (select  barcode2 from RBSYNERGY.dbo.tblBarCodeInv tbc where  tbc.Model='{0}'))
                    Group by sps.ItemDesc,rd.ItemName
                    order by sps.ItemDesc
                    ", model1);

                }
                else
                {
                    getPieChartDataForIssueName =
                      string.Format(@"select rd.ItemName, sps.ItemDesc,COUNT(distinct sm.IME) As TotalSpareUsedMinor
                    from Requisition R, RequisitionDetail RD, WSMS.dbo.ServiceMaster SM,wsms.dbo.SpareParts sps 
                    where R.RequisitionID=RD.RequisitionID and rd.ItemCode =sps.ItemCode
                    AND RD.ItemStatus='Used' 
                    AND RD.ItemCode not in ('') AND RD.ItemCode is not null  AND RD.ItemName not in ('') AND RD.ItemName is not null
                    and R.ServiceID =SM.ServiceID and sps.Model='{0}'  and sm.ServicePointID not in (69) and sm.Model='{0}'   and (sm.IME in (select  barcode from RBSYNERGY.dbo.tblBarCodeInv tbc where  tbc.Updatedby='{1}' and tbc.Model='{0}')
                    or sm.IME in (select  barcode from RBSYNERGY.dbo.tblBarCodeInv tbc where  tbc.Updatedby='{1}' and tbc.Model='{0}'))
                    Group by sps.ItemDesc,rd.ItemName
                    order by sps.ItemDesc
	            ", model1, orders1);

                }

                var command = new SqlCommand(getPieChartDataForIssueName, connection);

                command.CommandTimeout = 6000;
                SqlDataReader reader = command.ExecuteReader();

                double total1 = 0;
                while (reader.Read())
                {
                    var obj = new PieChartDataForSpare();

                    obj.ItemDesc = reader["ItemDesc"].ToString();
                    obj.ItemName = reader["ItemName"].ToString();
                    obj.TotalSpareUsedMinor = reader["TotalSpareUsedMinor"].ToString();
                    obj.TotalSpareUsedMinor1 = Convert.ToDouble(reader["TotalSpareUsedMinor"]);
                    total1 = total1 + Convert.ToDouble(obj.TotalSpareUsedMinor);
                    obj.TotalProblem = total1;
                    data.Add(obj);

                }

                finaldata = data.GroupBy(l => new { l.ItemDesc })
                  .Select(g => new PieChartDataForSpare
                  {
                      ItemDesc = g.Key.ItemDesc,
                      ItemName = string.Join(",", g.Select(i => i.ItemName)),
                      TotalSpareUsedMinor = string.Join(",", g.Select(i => i.TotalSpareUsedMinor)),
                      TotalProblem = total1,
                      TotalSpareUsedMajor = g.Sum(i => i.TotalSpareUsedMinor1)
                  }).OrderByDescending(i => i.TotalSpareUsedMajor).ToList();
                reader.Close();
            }
            return finaldata;
        }
        public List<HighChartDataListForTotalReceive> GetTotalReceiveData(string model1, string orders1)
        {
            List<HighChartDataListForTotalReceive> data = new List<HighChartDataListForTotalReceive>();
            string wsmsConnectionString = ConfigurationManager.ConnectionStrings["WSMSConnectionString"].ConnectionString;
            using (var connection = new SqlConnection(wsmsConnectionString))
            {
                connection.Open();
                string getSmartPhoneDailySales = "";

                if (orders1 == "ALL")
                {
                    getSmartPhoneDailySales =
                    string.Format(@"
                Select * from (Select count(sm.ServiceID)as totalServiceQnty,
                CONVERT(varchar(3), DATENAME(MONTH, sm.ServicePlaceDate)) as mon, DATEPART(MONTH,sm.ServicePlaceDate) as monNum,
                DATEPART(YEAR,sm.ServicePlaceDate)as year from ServiceMaster sm 
                where sm.Model = '{0}'  and sm.ServicePointID not in (69)
                and sm.ServicePlaceDate >= (SELECT top 1 dp.ReleaseDate from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice dp where Model = '{0}')
                Group By CONVERT(varchar(3),DATENAME(MONTH, sm.ServicePlaceDate)),DATEPART(MONTH,sm.ServicePlaceDate),DATEPART(YEAR,sm.ServicePlaceDate))C 
                order by   year, monNum", model1);

                }
                else
                {
                    getSmartPhoneDailySales =
                        string.Format(@"Select * from (Select count(sm.ServiceID)as totalServiceQnty,
                    CONVERT(varchar(3), DATENAME(MONTH, sm.ServicePlaceDate)) as mon, DATEPART(MONTH,sm.ServicePlaceDate) as monNum,
                    DATEPART(YEAR,sm.ServicePlaceDate)as year from ServiceMaster sm 
                    where sm.Model = '{0}'  and sm.ServicePointID not in (69)
                    and (SM.IME in (select  barcode from RBSYNERGY.dbo.tblBarCodeInv tbc where tbc.Updatedby='{1}' and tbc.Model='{0}') 
                    or SM.IME in (select  barcode2 from RBSYNERGY.dbo.tblBarCodeInv tbc where tbc.Updatedby='{1}' and tbc.Model='{0}') )
                    and sm.ServicePlaceDate >= (SELECT top 1 dp.ReleaseDate from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice dp where Model = '{0}')
                    Group By CONVERT(varchar(3),DATENAME(MONTH, sm.ServicePlaceDate)),DATEPART(MONTH,sm.ServicePlaceDate),DATEPART(YEAR,sm.ServicePlaceDate))C 
                    order by   year, monNum", model1, orders1);

                }


                var command = new SqlCommand(getSmartPhoneDailySales, connection);
                command.CommandTimeout = 6000;
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var obj = new HighChartDataListForTotalReceive
                    {
                        Month = reader["mon"].ToString(),
                        MonthNum = reader["monNum"].ToString(),
                        ServiceQuantity = Convert.ToInt32(reader["totalServiceQnty"]),
                        Year = Convert.ToInt32(reader["year"])

                    };
                    data.Add(obj);
                }

            }
            return data;
        }
        public List<HighChartDataListByOrderFromLauncingDate> GetOrderFromLauncingDate(string model1, string orders1)
        {
            List<HighChartDataListByOrderFromLauncingDate> data = new List<HighChartDataListByOrderFromLauncingDate>();
            string wsmsConnectionString = ConfigurationManager.ConnectionStrings["WSMSConnectionString"].ConnectionString;
            using (var connection = new SqlConnection(wsmsConnectionString))
            {
                connection.Open();
                string getSmartPhoneDailySales = "";

                if (orders1 == "ALL")
                {

                    getSmartPhoneDailySales =
                        string.Format(
                            @"select * from RBSYNERGY.dbo.ModelWiseServiceQuantityInDays where AddedDate = DATEADD(DD, DATEDIFF(DY, 0, GETDATE()), -1) and Model='{0}'",
                            model1);
                }
                else
                {
                    getSmartPhoneDailySales =
                       string.Format(
                           @"select * from RBSYNERGY.dbo.ModelWiseServiceQuantityInDaysByOrder where AddedDate = DATEADD(DD, DATEDIFF(DY, 0, GETDATE()), -1) and Model='{0}' and Orders='{1}' ",
                           model1, orders1);
                }

                var command = new SqlCommand(getSmartPhoneDailySales, connection);
                command.CommandTimeout = 2000;

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var obj = new HighChartDataListByOrderFromLauncingDate
                    {

                        tendays = Convert.ToInt64(reader["Tendays"]),
                        twentydays = Convert.ToInt64(reader["Twentydays"]),
                        fortydays = Convert.ToInt64(reader["Fortydays"]),
                        sixtydays = Convert.ToInt64(reader["Sixtydays"]),
                        nintydays = Convert.ToInt64(reader["Nintydays"]),
                        oneEightydays = Convert.ToInt64(reader["OneEightydays"]),
                        twoseventy = Convert.ToInt64(reader["Twoseventy"]),
                        threeSixty = Convert.ToInt64(reader["ThreeSixtyFive"]),
                        restoftheDays = Convert.ToInt64(reader["RestoftheDays"]),
                        orders = orders1

                    };
                    data.Add(obj);
                }
                connection.Close();
            }
            return data;
        }
        public void PrintDataListByOrderFromLauncingDate(string model1, string orders1)
        {
            var model = model1;
            var orders = orders1;

            if (model.Contains("(") && model.Contains(")"))
            {
                var input = model;
                model = input.Replace('+', ' ');
            }
            orders = orders.Replace("+", " ");

            var excelFileNames = @"File_" + model + "_" + DateTime.Now.ToString("yyyy-MM-dd hhmmss.mmm") + ".xls";
            String connectionString = ConfigurationManager.ConnectionStrings["WSMSConnectionString"].ConnectionString;


            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string getSmartPhoneDailySales = "";

                if (orders == "ALL")
                {

                    getSmartPhoneDailySales =
                        string.Format(
                            @"select * from RBSYNERGY.dbo.ModelWiseServiceQuantityInDays where AddedDate = DATEADD(DD, DATEDIFF(DY, 0, GETDATE()), -1) and Model='{0}'",
                            model);
                }
                else
                {
                    getSmartPhoneDailySales =
                       string.Format(
                           @"select * from RBSYNERGY.dbo.ModelWiseServiceQuantityInDaysByOrder where AddedDate = DATEADD(DD, DATEDIFF(DY, 0, GETDATE()), -1) and Model='{0}' and Orders='{1}' ",
                           model, orders);
                }


                var command = new SqlCommand(getSmartPhoneDailySales, connection);
                command.CommandTimeout = 2000;


                System.Data.DataSet ds = new System.Data.DataSet("ServiceQuantity");
                System.Data.DataTable dTable = new System.Data.DataTable();

                using (SqlDataReader reader = command.ExecuteReader())
                {

                    if (dTable != null)
                    {
                        dTable.Columns.Add("Model");
                        dTable.Columns.Add("\n");
                        dTable.Columns.Add("Tendays");
                        dTable.Columns.Add("Twentydays");
                        dTable.Columns.Add("Fortydays");
                        dTable.Columns.Add("Sixtydays");
                        dTable.Columns.Add("Nintydays");
                        dTable.Columns.Add("OneEightydays");
                        dTable.Columns.Add("Twoseventy");
                        dTable.Columns.Add("ThreeSixtyFive");
                        dTable.Columns.Add("RestoftheDays");

                        while (reader.Read())
                        {
                            DataRow newRow = dTable.NewRow();
                            newRow["Model"] = Convert.ToString(reader["Model"]);
                            newRow["\n"] = "\n";
                            newRow["Tendays"] = Convert.ToDouble(reader["Tendays"]);
                            newRow["Twentydays"] = Convert.ToDouble(reader["Twentydays"]);
                            newRow["Fortydays"] = Convert.ToDouble(reader["Fortydays"]);
                            newRow["Sixtydays"] = Convert.ToDouble(reader["Sixtydays"]);
                            newRow["Nintydays"] = Convert.ToDouble(reader["Nintydays"]);
                            newRow["OneEightydays"] = Convert.ToDouble(reader["OneEightydays"]);
                            newRow["Twoseventy"] = Convert.ToDouble(reader["Twoseventy"]);
                            newRow["ThreeSixtyFive"] = Convert.ToDouble(reader["ThreeSixtyFive"]);
                            newRow["RestoftheDays"] = Convert.ToDouble(reader["RestoftheDays"]);

                            dTable.Rows.Add(newRow);

                            DataRow newRow1 = dTable.NewRow();
                            newRow1["\n"] = "\n";
                            dTable.Rows.Add(newRow1);

                            DataRow newRow2 = dTable.NewRow();
                            newRow2["\n"] = "\n";
                            dTable.Rows.Add(newRow2);

                            DataRow newRow3 = dTable.NewRow();
                            newRow3["\n"] = "\n";
                            dTable.Rows.Add(newRow3);
                        }
                    }
                }

                ds.Tables.Add(dTable);

                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + excelFileNames);
                HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                StringWriter sw1 = new StringWriter();
                //HtmlTextWriter hw1 = new HtmlTextWriter(sw1);
                //gv1.RenderControl(hw1);
                HttpContext.Current.Response.Output.Write(sw1.ToString());
                HttpContext.Current.Response.End();

                connection.Close();
            }
        }
        #endregion



        #region WSMT BOM Database
        public List<WSMTHandset> GetWSMTModels()
        {
            List<WSMTHandset> handsets = new List<WSMTHandset>();
            List<WSMTHandset> dbhandsets = new List<WSMTHandset>();
            // String connectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            String wsmtconnectionString = ConfigurationManager.ConnectionStrings["WSMTConnectionString"].ConnectionString;
            using (var connection = new SqlConnection(wsmtconnectionString))
            {
                connection.Open();
                string squery = "";
                squery = string.Format(@"Select * from Handsets");

                var command = new SqlCommand(squery, connection);
                command.CommandTimeout = 200;

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var obj = new WSMTHandset
                    {
                        Title = reader["Title"].ToString(),
                        ID = Convert.ToInt64(reader["ID"].ToString())
                    };

                    dbhandsets.Add(obj);
                }
                connection.Close();
            }

            string serverIp = "test";
            string username = "test";
            string password = "test";
            string databaseName = "test";

            string dbConnectionString = string.Format("server={0};uid={1};pwd={2};database={3};", serverIp, username, password, databaseName);
            string query = "SELECT * FROM handsets";
            var conn = new MySql.Data.MySqlClient.MySqlConnection(dbConnectionString);

            try
            {
                conn.Open();
                var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    WSMTHandset model = new WSMTHandset();
                    model.ID = Int64.Parse(reader["id"].ToString());
                    model.Title = reader["title"].ToString();
                    model.BOMPattern = reader["bompattern"].ToString();
                    model.Created = DateTime.Today;
                    //model.Modified = DateTime.Today;
                    //model.Created = reader["created"] != DBNull.Value ? DateTime.Parse(reader["created"].ToString()) : DateTime.Today; 
                    //model.Modified = (reader["modified"] != null ? DateTime.Parse(reader["modified"].ToString()) : DateTime.Today);
                    handsets.Add(model);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                //return handsets;
            }

            handsets = handsets.Where(i => !dbhandsets.Any(e => i.ID == e.ID)).ToList();
            if (handsets.Count > 0)
            {
                using (var connection = new SqlConnection(wsmtconnectionString))
                {
                    connection.Open();
                    string squery = "";
                    foreach (var item in handsets)
                    {
                        squery = string.Format(@"Insert Into Handsets  ([ID]
                                       ,[Title]
                                       ,[BOMPattern]
                                       ,[Created])
                                       VALUES (@ID
                                               , @Title
                                               , @BOMPattern
                                               , @Created)");


                        SqlCommand cmd = new SqlCommand(squery, connection);
                        cmd.Parameters.Add("@ID", SqlDbType.BigInt).Value = item.ID;
                        cmd.Parameters.Add("@Title", SqlDbType.NVarChar, 250).Value = item.Title;
                        cmd.Parameters.Add("@BOMPattern", SqlDbType.NVarChar, 50).Value = item.BOMPattern;
                        cmd.Parameters.Add("@Created", SqlDbType.DateTime).Value = DateTime.Today;


                        cmd.ExecuteNonQuery();

                    }
                    connection.Close();
                }
            }
            dbhandsets.Clear();
            using (var connection = new SqlConnection(wsmtconnectionString))
            {
                connection.Open();
                string squery = "";
                squery = string.Format(@"Select * from Handsets where Model is null");

                var command = new SqlCommand(squery, connection);
                command.CommandTimeout = 200;

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var obj = new WSMTHandset
                    {
                        Title = reader["Title"].ToString(),
                        ID = Convert.ToInt64(reader["ID"].ToString())

                    };

                    dbhandsets.Add(obj);
                }
                connection.Close();
            }

            return dbhandsets;
        }

        public List<RBSYProductModel> GetRBSYProductModels()
        {
            List<RBSYProductModel> result = new List<RBSYProductModel>();
            String connectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string squery = "";
                squery = string.Format(@"Select ProductCode, Category1, Category2, ProductModel, OracleModelName from tblProductMaster where Category1='Cellcom' order by ProductModel");

                var command = new SqlCommand(squery, connection);
                command.CommandTimeout = 200;

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {

                    var obj = new RBSYProductModel
                    {
                        ProductCode = reader["ProductCode"].ToString(),
                        ProductModel = reader["ProductModel"].ToString(),
                        Category1 = reader["Category1"].ToString(),
                        Category2 = reader["Category2"].ToString(),
                        OracleModelName = reader["OracleModelName"].ToString()
                    };

                    result.Add(obj);
                }
                connection.Close();
            }

            return result;
        }

        public ResponseMessage SyncWSMTBoms(WSMTSyncVm vm)
        {
            ResponseMessage resp = new ResponseMessage();
            List<WSMTBom> boms = new List<WSMTBom>();
            List<WSMTAltBom> altboms = new List<WSMTAltBom>();
            if (HasDuplicateBom("BOMs", vm.SelectedHandset) == false)
            {

                string serverIp = "test";
                string username = "test";
                string password = "test";
                string databaseName = "test";

                string dbConnectionString = string.Format("server={0};uid={1};pwd={2};database={3};", serverIp, username, password, databaseName);
                string query = string.Format(@"Select * from boms where handset_id={0}", vm.SelectedHandset);
                var conn = new MySql.Data.MySqlClient.MySqlConnection(dbConnectionString);

                try
                {
                    conn.Open();
                    var cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        OracleItem orcitem = GetOrcaleItem(reader["description"].ToString());
                        WSMTBom bom = new WSMTBom();
                        bom.ID = Int64.Parse(reader["id"].ToString());
                        bom.Handset_Id = Int64.Parse(reader["handset_id"].ToString());
                        bom.Name = reader["name"].ToString();
                        bom.Name_Cn = reader["name_cn"].ToString();
                        bom.Name_Cn = reader["name_cn"].ToString();
                        bom.Manufacturer = reader["manufacturer"].ToString();
                        bom.Manufacturer_PartNumber = reader["manufacturer_partnumber"].ToString();
                        bom.Description = reader["description"].ToString();
                        bom.Reference = reader["reference"].ToString();
                        bom.PartNumber = reader["partnumber"].ToString();
                        bom.Value = reader["value"].ToString();
                        bom.QTY = Int32.Parse(reader["QTY"].ToString());
                        bom.Total_Qty = Int32.Parse(reader["Total_Qty"].ToString());
                        bom.MOQ = Int32.Parse(reader["MOQ"].ToString());
                        bom.MPQ = Int32.Parse(reader["MPQ"].ToString());
                        bom.Number_Of_Reel = Int32.Parse(reader["number_of_reel"].ToString());
                        bom.OrcalePartID = orcitem.ID;
                        bom.OracleItemCode = orcitem.Code;
                        bom.Created = DateTime.Today;

                        boms.Add(bom);
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    //return handsets;
                }

                if (boms.Count > 0)
                {
                    foreach (var item in boms)
                    {
                        string altquery = string.Format(@"Select * from alternateboms where bom_id={0}", item.ID);
                        conn.Open();
                        var cmd1 = new MySql.Data.MySqlClient.MySqlCommand(altquery, conn);
                        var reader1 = cmd1.ExecuteReader();
                        while (reader1.Read())
                        {
                            OracleItem orcitem = GetOrcaleItem(reader1["description"].ToString());
                            WSMTAltBom bom = new WSMTAltBom();
                            bom.ID = Int64.Parse(reader1["id"].ToString());
                            bom.Handset_Id = Int64.Parse(reader1["handset_id"].ToString());
                            bom.BOM_Id = Int64.Parse(reader1["bom_id"].ToString());
                            bom.Manufacturer = reader1["manufacturer"].ToString();
                            bom.Manufacturer_PartNumber = reader1["manufacturer_partnumber"].ToString();
                            bom.Description = reader1["description"].ToString();
                            bom.PartNumber = reader1["partnumber"].ToString();
                            bom.Value = reader1["value"].ToString();
                            bom.OraclePartID = orcitem.ID;
                            bom.OracleItemCode = orcitem.Code;
                            bom.Created = DateTime.Today;

                            altboms.Add(bom);
                        }
                        conn.Close();
                    }

                }

                String wsmtconnectionString = ConfigurationManager.ConnectionStrings["WSMTConnectionString"].ConnectionString;

                if (boms.Count > 0)
                {
                    using (var connection = new SqlConnection(wsmtconnectionString))
                    {

                        #region BOM Sync
                        string squery = "";
                        foreach (var item in boms)
                        {
                            squery = string.Format(@"Insert Into BOMs  ([ID]
                                               ,[PartNumber]
                                               ,[Name]
                                               ,[Name_Cn]
                                               ,[OrcalePartID]
                                               ,[OracleItemCode]
                                               ,[Manufacturer_PartNumber]
                                               ,[Description]
                                               ,[Reference]
                                               ,[QTY]
                                               ,[Total_Qty]
                                               ,[MOQ]
                                               ,[MPQ]
                                               ,[Number_Of_Reel]
                                               ,[Manufacturer]
                                               ,[Value]
                                               ,[Handset_Id]
                                               ,[Created])

                                      Values(@ID
                                               ,@PartNumber
                                               ,@Name
                                               ,@Name_Cn
                                               ,@OrcalePartID
                                               ,@OracleItemCode
                                               ,@Manufacturer_PartNumber
                                               ,@Description
                                               ,@Reference
                                               ,@QTY
                                               ,@Total_Qty
                                               ,@MOQ
                                               ,@MPQ
                                               ,@Number_Of_Reel
                                               ,@Manufacturer
                                               ,@Value
                                               ,@Handset_Id
                                               ,@Created)");


                            SqlCommand cmd = new SqlCommand(squery, connection);
                            cmd.Parameters.Add("@ID", SqlDbType.BigInt).Value = item.ID;
                            cmd.Parameters.Add("@PartNumber", SqlDbType.NVarChar, 250).Value = item.PartNumber;
                            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 150).Value = item.Name;
                            cmd.Parameters.Add("@Name_Cn", SqlDbType.NVarChar, 150).Value = item.Name_Cn;
                            cmd.Parameters.Add("@Manufacturer_PartNumber", SqlDbType.NVarChar, 150).Value = item.Manufacturer_PartNumber;
                            cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 150).Value = item.Description;
                            cmd.Parameters.Add("@Reference", SqlDbType.NVarChar, 150).Value = item.Reference;
                            cmd.Parameters.Add("@OracleItemCode", SqlDbType.NVarChar, 150).Value = item.OracleItemCode;
                            cmd.Parameters.Add("@Manufacturer", SqlDbType.NVarChar, 150).Value = item.Manufacturer;
                            cmd.Parameters.Add("@Value", SqlDbType.NVarChar, 150).Value = item.Value;
                            cmd.Parameters.Add("@OrcalePartID", SqlDbType.BigInt).Value = item.OrcalePartID;
                            cmd.Parameters.Add("@Handset_Id", SqlDbType.BigInt).Value = item.Handset_Id;
                            cmd.Parameters.Add("@QTY", SqlDbType.Int).Value = item.QTY;
                            cmd.Parameters.Add("@Total_Qty", SqlDbType.Int).Value = item.Total_Qty;
                            cmd.Parameters.Add("@MOQ", SqlDbType.Int).Value = item.MOQ;
                            cmd.Parameters.Add("@MPQ", SqlDbType.Int).Value = item.MPQ;
                            cmd.Parameters.Add("@Number_Of_Reel", SqlDbType.Int).Value = item.Number_Of_Reel;
                            cmd.Parameters.Add("@Created", SqlDbType.DateTime).Value = DateTime.Today;

                            try
                            {
                                connection.Open();
                                cmd.ExecuteNonQuery();
                                resp.Message = "BOMs have been updated Succesfully!";
                                connection.Close();
                            }
                            catch (Exception ex)
                            {
                                resp.Success = false;
                                resp.Message = "<strong>BOMs Sync Failed!</strong><br/>" + ex.Message;
                                connection.Close();
                                return resp;
                            }
                        }
                        #endregion

                        #region Alt BOM Sync
                        string aquery = "";
                        foreach (var item in altboms)
                        {
                            aquery = string.Format(@"Insert Into AlternatesBOMs  ([ID]
                                               ,[PartNumber]
                                               ,[BOM_Id]
                                               ,[OraclePartID]
                                               ,[OracleItemCode]
                                               ,[Manufacturer_PartNumber]
                                               ,[Description]
                                               ,[Manufacturer]
                                               ,[Value]
                                               ,[Handset_Id]
                                               ,[Created])

                                      Values(@ID
                                               ,@PartNumber
                                               ,@BOM_Id
                                               ,@OraclePartID
                                               ,@OracleItemCode
                                               ,@Manufacturer_PartNumber
                                               ,@Description
                                               ,@Manufacturer
                                               ,@Value
                                               ,@Handset_Id
                                               ,@Created)");


                            SqlCommand cmd1 = new SqlCommand(aquery, connection);
                            cmd1.Parameters.Add("@ID", SqlDbType.BigInt).Value = item.ID;
                            cmd1.Parameters.Add("@BOM_Id", SqlDbType.BigInt).Value = item.BOM_Id;
                            cmd1.Parameters.Add("@PartNumber", SqlDbType.NVarChar, 250).Value = item.PartNumber;
                            cmd1.Parameters.Add("@Manufacturer_PartNumber", SqlDbType.NVarChar, 150).Value = item.Manufacturer_PartNumber;
                            cmd1.Parameters.Add("@Description", SqlDbType.NVarChar, 150).Value = item.Description;
                            cmd1.Parameters.Add("@OracleItemCode", SqlDbType.NVarChar, 150).Value = item.OracleItemCode;
                            cmd1.Parameters.Add("@Manufacturer", SqlDbType.NVarChar, 150).Value = item.Manufacturer;
                            cmd1.Parameters.Add("@Value", SqlDbType.NVarChar, 150).Value = item.Value;
                            cmd1.Parameters.Add("@OraclePartID", SqlDbType.BigInt).Value = item.OraclePartID;
                            cmd1.Parameters.Add("@Handset_Id", SqlDbType.BigInt).Value = item.Handset_Id;
                            cmd1.Parameters.Add("@Created", SqlDbType.DateTime).Value = DateTime.Today;

                            try
                            {
                                connection.Open();
                                cmd1.ExecuteNonQuery();
                                resp.Message += "<br />Alternative BOMs have been updated Succesfully!";
                                connection.Close();
                            }
                            catch (Exception ex)
                            {
                                resp.Success = false;
                                resp.Message = "<strong>Alternative BOMs Sync Failed!</strong><br/>" + ex.Message;
                                connection.Close();
                                return resp;
                            }
                        }
                        #endregion

                        #region Model Information Update
                        var handsetupdate = string.Format(@"UPDATE [dbo].[HandSets]
                                                       SET [Model] = @Model
                                                          ,[OrderNo] = @OrderNo
                                                          ,[Production_Type] = @Production_Type
                                                          ,[Modified] = @Modified
                                                     WHERE ID=@ID");

                        SqlCommand cmd2 = new SqlCommand(handsetupdate, connection);
                        cmd2.Parameters.Add("@ID", SqlDbType.BigInt).Value = vm.SelectedHandset;
                        cmd2.Parameters.Add("@Model", SqlDbType.NVarChar, 50).Value = vm.RBSYModel;
                        cmd2.Parameters.Add("@OrderNo", SqlDbType.NVarChar, 50).Value = vm.OrderNo;
                        cmd2.Parameters.Add("@Production_Type", SqlDbType.NVarChar, 50).Value = vm.ProductionType;
                        cmd2.Parameters.Add("@Modified", SqlDbType.DateTime).Value = DateTime.Today;


                        try
                        {
                            connection.Open();
                            cmd2.ExecuteNonQuery();
                            resp.Success = true;
                            resp.Message += "<br />Model Information have been updated Succesfully!";
                            connection.Close();
                            var prid = _dbrbsyEntities.tblProductMasters.Where(x => x.ProductModel == vm.RBSYModel).Select(x => x.ProductID).ToString();
                            PushBOMtoOracle(boms, vm.OrderNo, prid, vm.RBSYModel);
                        }
                        catch (Exception ex)
                        {
                            resp.Success = false;
                            resp.Message = "<strong>Model Information Update failed!</strong><br/>" + ex.Message;
                            connection.Close();
                            return resp;

                        }
                        #endregion
                    }
                }
                else
                {
                    resp.Success = false;
                    resp.Message = "<strong>There is no BOM have been found to Sync!</strong>";
                    return resp;
                }
            }
            else
            {
                resp.Success = false;
                resp.Message = "<strong>Sync for This Handset have been done Before!</<strong>";
                return resp;
            }
            return resp;
        }
        private OracleItem GetOrcaleItem(string desc)
        {
            OracleItem item = new OracleItem();
            var connection = OracleDbConnection.GetOldConnection();
            string bomQuery = "SELECT  MSI.CREATION_DATE," + "MSI.ORGANIZATION_ID," + "MSI.INVENTORY_ITEM_ID ," +
                                  "MSI.SEGMENT1 " + "||'.'||" + "MSI.SEGMENT2" + "||'.'||" + "MSI.SEGMENT3 ITEM_CODE," +
                                  "MSI.DESCRIPTION ITEM_NAME," + " MSI.PRIMARY_UOM_CODE UOM," + "MCB.SEGMENT1," +
                                  " MCB.SEGMENT2" +
                                  " FROM MTL_SYSTEM_ITEMS MSI," + " MTL_ITEM_CATEGORIES MIC," + "MTL_CATEGORIES_B MCB" +
                                  " WHERE" +
                                  "  MSI.INVENTORY_ITEM_STATUS_CODE='Active' AND MCB.SEGMENT1 = 'RAW MATERIAL'  AND MSI.ORGANIZATION_ID=646 " +
                                  "AND MSI.DESCRIPTION = '" + desc + "' and  rownum = 1";
            OracleDataReader oracleDataReader = null;
            OracleCommand oracleCommand = new OracleCommand(bomQuery, connection) { CommandType = CommandType.Text };


            try
            {
                connection.Open();
                oracleDataReader = oracleCommand.ExecuteReader();
                if (oracleDataReader.HasRows)
                {
                    while (oracleDataReader.Read())
                    {
                        item.ID = oracleDataReader.IsDBNull(2) ? 0 : oracleDataReader.GetInt64(2);
                        item.Code = oracleDataReader.IsDBNull(3) ? "" : oracleDataReader.GetString(3);
                    }

                }
                else
                {
                    item.ID = 0;
                    item.Code = "";
                }

                oracleDataReader.Close();
                connection.Close();
                oracleCommand.Dispose();

            }
            catch (Exception e)
            {

            }
            return item;

        }

        public bool HasDuplicateBom(string tablename, long handsetid)
        {

            String wsmtconnectionString = ConfigurationManager.ConnectionStrings["WSMTConnectionString"].ConnectionString;
            using (var connection = new SqlConnection(wsmtconnectionString))
            {
                connection.Open();
                string squery = "";
                squery = string.Format(@"Select * from {0} where Handset_Id={1}", tablename, handsetid);

                var command = new SqlCommand(squery, connection);
                command.CommandTimeout = 200;

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    connection.Close();
                    return true;
                }
            }
            return false;
        }
        public List<WSMTHandset> GetWSMTHandsets()
        {
            List<WSMTHandset> handsets = new List<WSMTHandset>();
            String wsmtconnectionString = ConfigurationManager.ConnectionStrings["WSMTConnectionString"].ConnectionString;
            using (var connection = new SqlConnection(wsmtconnectionString))
            {
                connection.Open();
                string squery = "";
                squery = string.Format(@"Select * from Handsets where Model is not null");

                var command = new SqlCommand(squery, connection);
                command.CommandTimeout = 200;

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var obj = new WSMTHandset
                    {
                        RBSYModel = reader["Model"].ToString(),
                        ID = Convert.ToInt64(reader["ID"].ToString())
                    };

                    handsets.Add(obj);
                }
                connection.Close();
            }
            return handsets;
        }

        public BOMReportVm GetWSMTBomReportData(BOMReportVm vm)
        {
            BOMReportVm viewmodel = new BOMReportVm();
            List<WSMTBomVm> boms = new List<WSMTBomVm>();
            String wsmtconnectionString = ConfigurationManager.ConnectionStrings["WSMTConnectionString"].ConnectionString;
            using (var connection = new SqlConnection(wsmtconnectionString))
            {
                connection.Open();
                string squery = "";
                squery = string.Format(@"Select * from BOMs where Handset_Id={0}", vm.Handset_Id);

                var command = new SqlCommand(squery, connection);
                command.CommandTimeout = 200;

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    List<WSMTBomStockDetailsVm> altboms = GetAlternateBOMs(Int64.Parse(reader["ID"].ToString()));
                    var altbomsstock = altboms.Sum(x => x.CurrentStock);
                    var totalstock = GetOrcaleStock(Int64.Parse(reader["OrcalePartID"].ToString())) + altbomsstock;
                    //int totalqty = (int)(Int32.Parse(reader["QTY"].ToString()) * vm.ProductionQty);
                    int totalqty = Int32.Parse(reader["total_qty"].ToString());
                    int reqqty = (totalqty > (int)totalstock ? (totalqty - (int)totalstock) : 0);
                    int moq = Int32.Parse(reader["MOQ"].ToString());
                    int reqorderqty = (moq != 0 ? (reqqty < moq ? moq : (reqqty % moq == 0 ? ((reqqty / moq) * moq) : (((reqqty / moq) + 1) * moq))) : reqqty);
                    reqorderqty = (reqqty > 0 ? reqorderqty : 0);
                    var obj = new WSMTBomVm
                    {
                        ID = Int64.Parse(reader["ID"].ToString()),
                        Handset_Id = Int64.Parse(reader["Handset_Id"].ToString()),
                        Name = reader["Name"].ToString(),
                        Name_Cn = reader["Name_Cn"].ToString(),
                        Manufacturer = reader["Manufacturer"].ToString(),
                        Manufacturer_PartNumber = reader["Manufacturer_PartNumber"].ToString(),
                        Description = reader["Description"].ToString(),
                        Reference = reader["Reference"].ToString(),
                        QTY = Int32.Parse(reader["QTY"].ToString()),
                        TotalQty = totalqty,
                        MOQ = moq,
                        MPQ = Int32.Parse(reader["MPQ"].ToString()),
                        PartNumber = reader["PartNumber"].ToString(),
                        OrcalePartID = Int64.Parse(reader["OrcalePartID"].ToString()),
                        OracleItemCode = reader["OracleItemCode"].ToString(),
                        TotalCurrentStock = totalstock,
                        Requiredqty = reqqty,
                        RequiredOrderqty = reqorderqty,
                        Status = (totalstock > totalqty ? "Stock Available" : "Needed Order Quantity " + reqorderqty.ToString())
                    };

                    boms.Add(obj);
                }
                connection.Close();
            }
            viewmodel.Handset_Id = vm.Handset_Id;
            viewmodel.ModelName = vm.ModelName;
            viewmodel.ProductionQty = vm.ProductionQty;
            viewmodel.Boms = boms;
            return viewmodel;
        }
        public List<WSMTBomStockDetailsVm> GetAlternateBOMs(long bom_id)
        {
            List<WSMTBomStockDetailsVm> altboms = new List<WSMTBomStockDetailsVm>();
            String wsmtconnectionString = ConfigurationManager.ConnectionStrings["WSMTConnectionString"].ConnectionString;
            using (var connection = new SqlConnection(wsmtconnectionString))
            {
                connection.Open();
                string squery = "";
                squery = string.Format(@"Select * from AlternatesBOMs where BOM_Id={0}", bom_id);

                var command = new SqlCommand(squery, connection);
                command.CommandTimeout = 200;

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var obj = new WSMTBomStockDetailsVm
                    {
                        ID = Int64.Parse(reader["ID"].ToString()),
                        Handset_Id = Int64.Parse(reader["Handset_Id"].ToString()),
                        Manufacturer = reader["Manufacturer"].ToString(),
                        Manufacturer_PartNumber = reader["Manufacturer_PartNumber"].ToString(),
                        Description = reader["Description"].ToString(),
                        PartNumber = reader["PartNumber"].ToString(),
                        OrcalePartID = Int64.Parse(reader["OraclePartID"].ToString()),
                        OracleItemCode = reader["OracleItemCode"].ToString(),
                        CurrentStock = GetOrcaleStock(Int64.Parse(reader["OraclePartID"].ToString()))

                    };

                    altboms.Add(obj);
                }
                connection.Close();
            }
            return altboms;
        }
        private int GetOrcaleStock(long itemid)
        {
            int qty = 0;
            var connection = OracleDbConnection.GetOldConnection();
            string bomQuery = "SELECT SUBINVENTORY_CODE,  SUM (MOQ.TRANSACTION_QUANTITY) ON_HAND FROM  APPS.MTL_ONHAND_QUANTITIES MOQ" +
                              " WHERE MOQ.INVENTORY_ITEM_ID=" + itemid + " AND MOQ.ORGANIZATION_ID = 646 GROUP BY SUBINVENTORY_CODE";
            OracleDataReader oracleDataReader = null;
            OracleCommand oracleCommand = new OracleCommand(bomQuery, connection) { CommandType = CommandType.Text };


            try
            {
                connection.Open();
                oracleDataReader = oracleCommand.ExecuteReader();
                if (oracleDataReader.HasRows)
                {
                    while (oracleDataReader.Read())
                    {
                        qty += oracleDataReader.IsDBNull(1) ? 0 : oracleDataReader.GetInt32(1);
                        //item.Code = oracleDataReader.IsDBNull(3) ? "" : oracleDataReader.GetString(3);
                    }

                }
                else
                {
                    qty = 0;
                    //item.Code = "";
                }

                oracleDataReader.Close();
                connection.Close();
                oracleCommand.Dispose();

            }
            catch (Exception e)
            {

            }
            return qty;
        }

        //public 
        private bool PushBOMtoOracle(List<WSMTBom> wsmtboms, string orderno, string productid, string modelname)
        {
            var pushconnection = OracleDbConnection.GetOldConnection();
            bool returnValue = false;
            //Insert Data To Oracle 
            var oracleUploadList = new List<OracleBOMItem>();
            foreach (var item in wsmtboms)
            {
                OracleBOMItem entity = new OracleBOMItem();
                entity.ORDER_DATE = item.Created;
                entity.ORDER_NO = orderno;
                entity.ORGANIZATION_ID = 646;
                entity.ITEM_ID = item.OrcalePartID;
                entity.ITEM_CODE = item.OracleItemCode;
                entity.FG_MODEL_ID = productid;
                entity.FG_MODEL_PROJECT = modelname;
                entity.QUANTITY = item.Total_Qty;
                entity.REMARKS = "";
                entity.CREATED_BY = "SQL DB";
                entity.CREATION_DATE = DateTime.Now;
            }
            try
            {
                pushconnection.Open();
                //OracleTransaction transaction = pushconnection.BeginTransaction();
                // Assign transaction object for a pending local transaction

                string query =
                    @"insert into APPS.XX_MOB_RM_ORDER (ORDER_DATE,ORGANIZATION_ID, FG_MODEL_ID,FG_MODEL_PROJECT, ITEM_ID,ITEM_CODE,QUANTITY, REMARKS,CREATED_BY,CREATION_DATE,ORDER_NO)
                                                values (:ordDate,:orgId, :fgmodelId,:fgmodelproject,:itemId,:itemcCode,:qty,:remarks,:createdBy,:createdDate,:orderNo)";

                using (var command = pushconnection.CreateCommand())
                {
                    //command.Transaction = transaction;

                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    command.BindByName = true;
                    // In order to use ArrayBinding, the ArrayBindCount property
                    // of OracleCommand object must be set to the number of records to be inserted
                    command.ArrayBindCount = oracleUploadList.Count;
                    command.Parameters.Add(":ordDate", OracleDbType.Date,
                        oracleUploadList.Select(c => c.ORDER_DATE).ToArray(), ParameterDirection.ReturnValue);
                    command.Parameters.Add(":orgId", OracleDbType.Int32,
                        oracleUploadList.Select(c => c.ORGANIZATION_ID).ToArray(), ParameterDirection.ReturnValue);
                    command.Parameters.Add(":fgmodelId", OracleDbType.Int32,
                        oracleUploadList.Select(c => c.FG_MODEL_ID).ToArray(), ParameterDirection.ReturnValue);
                    command.Parameters.Add(":fgmodelproject", OracleDbType.Varchar2,
                        oracleUploadList.Select(c => c.FG_MODEL_PROJECT).ToArray(), ParameterDirection.ReturnValue);
                    command.Parameters.Add(":itemId", OracleDbType.Int64,
                        oracleUploadList.Select(c => c.ITEM_ID).ToArray(), ParameterDirection.ReturnValue);
                    command.Parameters.Add(":itemcCode", OracleDbType.Varchar2,
                        oracleUploadList.Select(c => c.ITEM_CODE).ToArray(), ParameterDirection.ReturnValue);
                    command.Parameters.Add(":qty", OracleDbType.Int32,
                        oracleUploadList.Select(c => c.QUANTITY).ToArray(), ParameterDirection.ReturnValue);
                    command.Parameters.Add(":createdBy", OracleDbType.Varchar2,
                        oracleUploadList.Select(c => c.CREATED_BY).ToArray(), ParameterDirection.ReturnValue);
                    command.Parameters.Add(":createdDate", OracleDbType.Date,
                        oracleUploadList.Select(c => c.CREATION_DATE).ToArray(), ParameterDirection.ReturnValue);
                    command.Parameters.Add(":remarks", OracleDbType.Varchar2,
                        oracleUploadList.Select(c => c.REMARKS).ToArray(), ParameterDirection.ReturnValue);
                    command.Parameters.Add(":orderNo", OracleDbType.Varchar2,
                        oracleUploadList.Select(c => c.ORDER_NO).ToArray(), ParameterDirection.ReturnValue);


                    if (oracleUploadList.Count > 0)
                    {
                        int result = command.ExecuteNonQuery();
                        if (result == oracleUploadList.Count)
                        {
                            // transaction.Commit();
                            returnValue = true;
                            pushconnection.Close();
                        }

                        else
                        {
                            // transaction.Rollback();
                            pushconnection.Close();
                        }
                    }
                    else
                    {
                        returnValue = true;
                        pushconnection.Close();
                    }
                }
            }
            catch (OracleException ex)
            {
                //Log error thrown
            }
            finally
            {
                pushconnection.Close();
            }
            return returnValue;
        }
        #endregion


        public List<ProjectPurchaseOrderFormModel> GetProjectPurchaseFormData(string columnName)
        {

            var getDataQueryString =
                string.Format(@"  select ppof.ProjectMasterId,pm.ProjectName,PurchaseOrderNumber,PoCategory,CompanyName,PoDate, pm.OrderNuber as OrderNumber
                                                      from ProjectPurchaseOrderForms ppof
                                                      inner join ProjectMasters pm
                                                      on ppof.ProjectMasterId = pm.ProjectMasterId
                                                      where ppof.IsCompleted = 0 and pm.OrderNuber > 1 and " + columnName + " IS NULL"
                    );
            var t = _dbEntities.Database.SqlQuery<ProjectPurchaseOrderFormModel>(getDataQueryString).ToList();

            return t;

        }

        public bool InsertPurchaseOrderComment(long masterId, string comment)
        {
            var entity = _dbEntities.ProjectPurchaseOrderForms.FirstOrDefault(x => x.ProjectMasterId == masterId);

            if (HttpContext.Current.User.IsInRole("CM") || HttpContext.Current.User.IsInRole("CMHEAD"))
            {
                entity.InchargeComment = comment;
                _dbEntities.SaveChanges();
                return true;
            }
            if (HttpContext.Current.User.IsInRole("ASPM"))
            {
                entity.AfterSalesPmComment = comment;
                _dbEntities.SaveChanges();
                return true;
            }
            if (HttpContext.Current.User.IsInRole("HWHEAD"))
            {

                //_dbEntities.SaveChanges();
                return true;
            }
            if (HttpContext.Current.User.IsInRole("PROC"))
            {
                entity.ProcessTeamComment = comment;
                _dbEntities.SaveChanges();
                return true;
            }
            return false;
        }

        public List<ProjectEventDates> GetAllProjectEventDates()
        {
            var query =
                string.Format(
                    @"SELECT pm.ProjectMasterId, pm.ProjectName,pm.OrderNuber as OrderNumber,ppf.PoDate, pos.RawMaterialShipmentDate as ShipmentTakenDate,
                      pos.WarehouseEntryDate as MaterialsArrivalDate,ppf.MarketClearanceDate, pos.PoWiseShipmentNumber
                      FROM ProjectMasters pm
                      LEFT JOIN ProjectPurchaseOrderForms ppf ON pm.ProjectMasterId=ppf.ProjectMasterId
                      LEFT JOIN ProjectOrderShipments pos ON pm.ProjectMasterId=pos.ProjectMasterId
                      WHERE pm.IsActive=1
                      ORDER BY pm.ProjectMasterId DESC");
            var model = _dbEntities.Database.SqlQuery<ProjectEventDates>(query).ToList();
            foreach (var m in model)
            {
                m.PoOrdinal = CommonConversion.AddOrdinal(m.OrderNumber) + " Purchase Order";
                m.ShipmentNoOrdinal = CommonConversion.AddOrdinal(m.PoWiseShipmentNumber) + " shipment";
            }
            return model;
        }

        public List<ProjectVariantModel> GetProjectVariantModelsByProjectId(long id)
        {
            var list = (from v in _dbEntities.ProjectVariants
                        where v.ProjectId == id
                        select new ProjectVariantModel
                            {
                                Id = v.Id,
                                ProjectId = v.ProjectId,
                                ProjectName = v.ProjectName,
                                ProjectModel = v.ProjectModel,
                                ProjectVariantName = v.ProjectVariantName,
                                VariantByRamRom = v.VariantByRamRom,
                                TotalOrderQuantity = v.TotalOrderQuantity,
                                ProjectVariantQuantity = v.ProjectVariantQuantity,
                                AddedBy = v.AddedBy,
                                AddedDate = v.AddedDate,
                                UpdatedBy = v.UpdatedBy,
                                UpdatedDate = v.UpdatedDate,
                                IsLocked = v.IsLocked,
                                Prefix = v.Prefix,
                                Suffix = v.Suffix
                            }).ToList();
            return list;
        }

        public ProjectVariantModel GetProjectVariantModelById(long id)
        {
            var model = (from v in _dbEntities.ProjectVariants
                         where v.Id == id
                         select new ProjectVariantModel
                         {
                             Id = v.Id,
                             ProjectId = v.ProjectId,
                             ProjectName = v.ProjectName,
                             ProjectModel = v.ProjectModel,
                             ProjectVariantName = v.ProjectVariantName,
                             VariantByRamRom = v.VariantByRamRom,
                             TotalOrderQuantity = v.TotalOrderQuantity,
                             ProjectVariantQuantity = v.ProjectVariantQuantity,
                             AddedBy = v.AddedBy,
                             AddedDate = v.AddedDate,
                             UpdatedBy = v.UpdatedBy,
                             UpdatedDate = v.UpdatedDate,
                             IsLocked = v.IsLocked,
                             Prefix = v.Prefix,
                             Suffix = v.Suffix
                         }).FirstOrDefault();
            return model;
        }

        public ProjectVariantModel SaveUpdateProjectVariant(ProjectVariantModel model)
        {
            Mapper.CreateMap<ProjectVariantModel, ProjectVariant>();
            var variant = Mapper.Map<ProjectVariant>(model);
            _dbEntities.ProjectVariants.AddOrUpdate(variant);
            _dbEntities.SaveChanges();
            model.Id = variant.Id;
            return model;
        }

        public void RemoveProjectVariant(long variantId = 0)
        {
            var model = _dbEntities.ProjectVariants.FirstOrDefault(v => v.Id == variantId);
            _dbEntities.ProjectVariants.Remove(model);
            _dbEntities.SaveChanges();
        }

        public void SaveLockedVariantToOrderQuantityDetailModel(
            ProjectOrderQuantityDetailModel model)
        {
            model.IsActive = true;
            Mapper.CreateMap<ProjectOrderQuantityDetailModel, ProjectOrderQuantityDetail>();
            var detail = Mapper.Map<ProjectOrderQuantityDetail>(model);
            _dbEntities.ProjectOrderQuantityDetails.Add(detail);
            _dbEntities.SaveChanges();
        }

        public List<OrderQuantityDetailsVm> GetOrderQuantityDetailsVms()
        {
            string query =
                string.Format(
                    @"select pm.ProjectMasterId,pm.ProjectName,pm.OrderNuber,pm.ProjectModel,pm.OrderQuantity,
pvc.Id as VariantId,pvc.VariantName,pvc.Quantity as QuantityInCalculator,pvc.AddedBy,pvc.AddedDate,pvc.UpdatedBy,pvc.UpdatedDate,pvc.IsLocked,pvc.UnassignedQuantity
 from ProjectMasters pm
inner join ProjectPurchaseOrderForms ppf on pm.ProjectMasterId=ppf.ProjectMasterId
left join ProjectVariantCalculators pvc on pm.ProjectMasterId=pvc.ProjectId
where pm.AddedDate>'2019-04-01' and pm.OrderQuantity is not NULL and pm.ProjectType='Smart' and ppf.IsCompleted=0
order by pm.ProjectModel,pm.OrderNuber");
            var model = _dbEntities.Database.SqlQuery<OrderQuantityDetailsVm>(query).ToList();
            return model;
        }

        public List<OrderQuantityDetailsVm> GetOrderQuantityDetailsVmsByProjectId(long projectId)
        {
            string query =
                string.Format(
                    @"select pm.ProjectMasterId,pm.ProjectName,pm.OrderNuber,pm.ProjectModel,pm.OrderQuantity,
pvc.Id as VariantId,pvc.VariantName,pvc.Quantity as QuantityInCalculator,pvc.AddedBy,pvc.AddedDate,pvc.UpdatedBy,pvc.UpdatedDate,pvc.IsLocked,pvc.UnassignedQuantity
 from ProjectMasters pm
left join ProjectVariantCalculators pvc on pm.ProjectMasterId=pvc.ProjectId
where pm.ProjectmasterId={0}", projectId);
            var model = _dbEntities.Database.SqlQuery<OrderQuantityDetailsVm>(query).ToList();
            return model;
        }

        public List<ProjectVariantCalculatorModel> GetPreviousOrderVariants(long projectId)
        {
            string query =
                string.Format(
                    @"select pvc.VariantName
 from ProjectMasters pm
inner join ProjectVariantCalculators pvc on pm.ProjectMasterId=pvc.ProjectId
where pm.ProjectName=(select ProjectName from ProjectMasters where ProjectMasterId={0})
group by pvc.VariantName", projectId);
            var model = _dbEntities.Database.SqlQuery<ProjectVariantCalculatorModel>(query).ToList();
            return model;
        }

        public ProjectVariantCalculatorModel SaveProjectVariantCalculator(ProjectVariantCalculatorModel model)
        {
            Mapper.CreateMap<ProjectVariantCalculatorModel, ProjectVariantCalculator>();
            var detail = Mapper.Map<ProjectVariantCalculator>(model);
            _dbEntities.ProjectVariantCalculators.AddOrUpdate(detail);
            _dbEntities.SaveChanges();
            model.Id = detail.Id;
            return model;
        }

        public List<ProjectVariantCalculatorModel> GetVariantCalculatorByProjectId(long projectId)
        {
            var model = (from v in _dbEntities.ProjectVariantCalculators
                         where v.ProjectId == projectId
                         select new ProjectVariantCalculatorModel
                         {
                             Id = v.Id,
                             ProjectId = v.ProjectId,
                             VariantName = v.VariantName,
                             Quantity = v.Quantity,
                             AddedBy = v.AddedBy,
                             AddedDate = v.AddedDate,
                             UpdatedBy = v.UpdatedBy,
                             UpdatedDate = v.UpdatedDate,
                             IsLocked = v.IsLocked,
                             UnassignedQuantity = v.UnassignedQuantity
                         }).ToList();
            return model;
        }

        public ProjectVariantCalculatorModel GetProjectVariantCalculatorById(long id)
        {
            var model = (from v in _dbEntities.ProjectVariantCalculators
                         where v.Id == id
                         select new ProjectVariantCalculatorModel
                         {
                             Id = v.Id,
                             ProjectId = v.ProjectId,
                             VariantName = v.VariantName,
                             Quantity = v.Quantity,
                             AddedBy = v.AddedBy,
                             AddedDate = v.AddedDate,
                             UpdatedBy = v.UpdatedBy,
                             UpdatedDate = v.UpdatedDate
                         }).FirstOrDefault();
            return model;
        }

        public void UpdateProjectModelInProjectMaster(string projectModel, long projectId)
        {
            var query = string.Format(@"UPDATE ProjectMasters SET ProjectModel='{0}' WHERE ProjectMasterId={1}",
                projectModel, projectId);
            _dbEntities.Database.ExecuteSqlCommand(query);
        }

        public void RemoveVariantCalculator(long id)
        {
            var query = string.Format(@"DELETE ProjectVariantCalculators WHERE Id={0} and IsLocked=0", id);
            _dbEntities.Database.ExecuteSqlCommand(query);
        }

        public List<rbsBarCodeInv> SixMonthsUnproducedAverageQty()
        {
            var IMEI_QTY = new List<rbsBarCodeInv>();
            String connectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = string.Format(@"SELECT
OrderQTY.ProjectModel
,OrderQTY.Order_Num
,OrderQTY.OrderQuantity
,ProjectMaster.Produced
,OrderQTY.OrderQuantity-ProjectMaster.Produced as Unproduced
,ProjectMaster.LastMonthIMEI
FROM 
(
select d.ProjectModel, p.OrderNuber Order_Num,
SUM(CASE WHEN d.OrderQuantity IS NOT NULL THEN d.OrderQuantity ELSE 0 END) OrderQuantity
,po.PoDate
from CellPhoneProject.dbo.ProjectOrderQuantityDetails d
join CellPhoneProject.dbo.ProjectMasters p
on p.ProjectMasterId = d.ProjectMasterId
join CellPhoneProject.dbo.ProjectPurchaseOrderForms po
on p.ProjectMasterId=po.ProjectMasterId
where po.PoDate between DATEADD(MONTH,-9,GETDATE()) AND DATEADD(MONTH,-6,GETDATE()) AND po.IsCompleted=1
GROUP BY d.ProjectModel, p.OrderNuber
,po.PoDate
) OrderQTY
LEFT JOIN
(
SELECT xx.ProjectModel, xx.OrderNuber AS Order_No, xx.OrderQuantity, xy.Produced
, xy.LastMonthIMEI FROM
(
SELECT d.ProjectModel, 
SUM(CASE WHEN d.OrderQuantity IS NOT NULL THEN d.OrderQuantity ELSE 0 END) OrderQuantity,
OrderNuber 
from CellPhoneProject.dbo.ProjectOrderQuantityDetails d
join CellPhoneProject.dbo.ProjectMasters
on ProjectMasters.ProjectMasterId = d.ProjectMasterId	
WHERE 
ProjectMasters.AddedDate >= DATEADD(YEAR, -4, GETDATE())
AND ProjectMasters.SourcingType <> 'OEM'
GROUP BY d.ProjectModel, OrderNuber
) xx
LEFT JOIN
(
SELECT
IMEI_QTY.Model
,Produced
,Order_Num,
IMEI_QTY.Order_No,
LastMonth.LastMonthIMEI from
(
SELECT
tblBarCodeInv.Model
,COUNT(tblBarCodeInv.BarCode) AS Produced
,tblBarCodeInv.Updatedby AS Order_Num
,SUBSTRING(tblBarCodeInv.Updatedby, PATINDEX('%[0-9]%', tblBarCodeInv.Updatedby), LEN(tblBarCodeInv.Updatedby)) AS Order_No
FROM RBSYNERGY.dbo.tblBarCodeInv
WHERE tblBarCodeInv.DateAdded >= DATEADD(YEAR, -3, GETDATE()) 
AND tblBarCodeInv.Updatedby not in ( 'Not Tracable', 'Test Order')
GROUP BY tblBarCodeInv.Model
,tblBarCodeInv.Updatedby
) IMEI_QTY
Left JOIN 
(Select Model,Count(BarCode) LastMonthIMEI
,SUBSTRING(tblBarCodeInv.Updatedby, PATINDEX('%[0-9]%', tblBarCodeInv.Updatedby), LEN(tblBarCodeInv.Updatedby)) AS Order_No
from RBSYNERGY.dbo.tblBarCodeInv where PrintDate >= DATEADD(MONTH, -1, GETDATE()) 
AND tblBarCodeInv.Updatedby not in ( 'Not Tracable', 'Test Order')
group by Model,Updatedby) LastMonth
ON IMEI_QTY.Model = LastMonth.Model
AND IMEI_QTY.Order_No = LastMonth.Order_No) xy

ON xx.ProjectModel = xy.Model
AND xx.OrderNuber = xy.Order_No
) ProjectMaster

ON OrderQTY.ProjectModel = ProjectMaster.ProjectModel
AND OrderQTY.Order_Num = ProjectMaster.Order_No
WHERE ProjectMaster.Produced IS NOT NULL 
ORDER BY OrderQTY.ProjectModel");
                var command = new SqlCommand(query, connection);
                command.CommandTimeout = 200;

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var obj = new rbsBarCodeInv();
                    obj.ProjectModel = Convert.ToString(reader["ProjectModel"]);
                    obj.OrderQuantity = (reader["OrderQuantity"] != null ? Convert.ToInt64(reader["OrderQuantity"]) : 0);
                    obj.Produced = (reader["Produced"] != null ? Convert.ToInt64(reader["Produced"]) : 0);
                    obj.UnProduced = (reader["Produced"] != null ? Convert.ToInt64(reader["Unproduced"]) : 0);
                    obj.Order_Num = Convert.ToString(reader["Order_Num"]);
                    obj.Order_No = reader["Order_Num"] != null
                        ? Convert.ToInt32(reader["Order_Num"].ToString().Trim())
                        : 0;
                    obj.LastMonthIMEIProduced = (reader["LastMonthIMEI"] != System.DBNull.Value
                        ? Convert.ToInt64(reader["LastMonthIMEI"])
                        : 0);

                    IMEI_QTY.Add(obj);
                }
                connection.Close();
                foreach (var imei in IMEI_QTY)
                {
                    imei.UnproducedPercentage = imei.UnProduced == 0 ? "0" : Convert.ToString((Math.Round(((decimal)imei.UnProduced / imei.OrderQuantity) * 100)));
                }
                return IMEI_QTY;
            }
        }

        #region PO Feedback
        public List<ProjectPoFeedbackModel> GetPoFeedbackByUserId(long userId)
        {
            var m = _dbEntities.ProjectPoFeedbacks.Where(x => x.AddedBy == userId).Select(x => new ProjectPoFeedbackModel
            {
                Id = x.Id,
                ProjectId = x.ProjectId,
                ProjectModel = x.ProjectModel,
                OrderNumber = x.OrderNumber,
                OnBehalfOf = x.OnBehalfOf,
                FeedBack = x.FeedBack,
                AddedBy = x.AddedBy,
                AddedDate = x.AddedDate,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate,
                AllowReorder = x.AllowReorder,
                FileUploadPath = x.FileUploadPath
            }).ToList();
            return m;
        }

        public ProjectPoFeedbackModel GetPoFeedbackById(long id)
        {
            var m = _dbEntities.ProjectPoFeedbacks.Where(x => x.Id == id).Select(x => new ProjectPoFeedbackModel
            {
                Id = x.Id,
                ProjectId = x.ProjectId,
                ProjectModel = x.ProjectModel,
                OrderNumber = x.OrderNumber,
                OnBehalfOf = x.OnBehalfOf,
                FeedBack = x.FeedBack,
                AddedBy = x.AddedBy,
                AddedDate = x.AddedDate,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate,
                AllowReorder = x.AllowReorder,
                FileUploadPath = x.FileUploadPath
            }).FirstOrDefault();
            return m;
        }

        public List<ProjectPoFeedbackModel> GetPoFeedbackByProjectId(long? projectid)
        {
            var m =
                _dbEntities.ProjectPoFeedbacks.Where(x => x.ProjectId == projectid)
                    .Select(x => new ProjectPoFeedbackModel
                    {
                        Id = x.Id,
                        ProjectId = x.ProjectId,
                        ProjectModel = x.ProjectModel,
                        OrderNumber = x.OrderNumber,
                        OnBehalfOf = x.OnBehalfOf,
                        FeedBack = x.FeedBack,
                        AddedBy = x.AddedBy,
                        AddedDate = x.AddedDate,
                        UpdatedBy = x.UpdatedBy,
                        UpdatedDate = x.UpdatedDate,
                        AllowReorder = x.AllowReorder,
                        FileUploadPath = x.FileUploadPath,
                        SourcingComment = x.SourcingComment,
                        SourcingCommentBy = x.SourcingCommentBy,
                        SourcingCommentDate = x.SourcingCommentDate,
                        SourcingAllowReorder = x.SourcingAllowReorder,
                        AddedByName =
                            _dbEntities.CmnUsers.Where(v => v.CmnUserId == x.AddedBy)
                                .Select(v => v.UserFullName)
                                .FirstOrDefault(),
                        Department = (from u in _dbEntities.CmnUsers
                                      join r in _dbEntities.CmnRoles on u.RoleName equals r.RoleName
                                      where u.CmnUserId == x.AddedBy
                                      select r.RoleDescription).FirstOrDefault(),
                        SourcingCommentByName = _dbEntities.CmnUsers.Where(v => v.CmnUserId == x.SourcingCommentBy)
                            .Select(v => v.UserFullName)
                            .FirstOrDefault(),
                        ManagementComment = x.ManagementComment,
                        ManagementCommentBy = x.ManagementCommentBy,
                        ManagementCommentDate = x.ManagementCommentDate,
                        ManagementDecision = x.ManagementDecision
                    }).ToList();
            return m;
        }

        public List<ProjectPoFeedbackModel> DuplicatePoFeedbackBySamePerson(long? projectid, long? addedby)
        {
            var m = _dbEntities.ProjectPoFeedbacks.Where(x => x.ProjectId == projectid && x.AddedBy == addedby).Select(x => new ProjectPoFeedbackModel
            {
                Id = x.Id,
                ProjectId = x.ProjectId,
                ProjectModel = x.ProjectModel,
                OrderNumber = x.OrderNumber,
                OnBehalfOf = x.OnBehalfOf,
                FeedBack = x.FeedBack,
                AddedBy = x.AddedBy,
                AddedDate = x.AddedDate,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate,
                AllowReorder = x.AllowReorder,
                FileUploadPath = x.FileUploadPath
            }).ToList();
            return m;
        }

        public ProjectPoFeedbackModel SaveUpdatePoFeedBackModel(ProjectPoFeedbackModel model)
        {
            model.ProjectModel =
                _dbEntities.ProjectMasters.Where(x => x.ProjectMasterId == model.ProjectId)
                    .Select(x => x.ProjectModel)
                    .FirstOrDefault();
            model.OrderNumber = Convert.ToString(_dbEntities.ProjectMasters.Where(x => x.ProjectMasterId == model.ProjectId)
                    .Select(x => x.OrderNuber)
                    .FirstOrDefault());
            Mapper.CreateMap<ProjectPoFeedbackModel, ProjectPoFeedback>();
            var v = Mapper.Map<ProjectPoFeedback>(model);
            _dbEntities.ProjectPoFeedbacks.AddOrUpdate(v);
            _dbEntities.SaveChanges();
            model.Id = v.Id;
            //log table data insert
            var log = GenericMapper<ProjectPoFeedback, ProjectPoFeedbackLog>.GetDestination(v);
            _dbEntities.ProjectPoFeedbackLogs.Add(log);
            _dbEntities.SaveChanges();
            return model;
        }

        public List<ProjectPoFeedbackModel> GetAllProjectPoFeedbackModels()
        {
            var m =
                _dbEntities.ProjectPoFeedbacks
                    .Select(x => new ProjectPoFeedbackModel
                    {
                        Id = x.Id,
                        ProjectId = x.ProjectId,
                        ProjectModel = x.ProjectModel,
                        ProjectName = _dbEntities.ProjectMasters.Where(v => v.ProjectMasterId == x.ProjectId).Select(v => v.ProjectName).FirstOrDefault(),
                        OrderNumber = x.OrderNumber,
                        OnBehalfOf = x.OnBehalfOf,
                        FeedBack = x.FeedBack,
                        AddedBy = x.AddedBy,
                        AddedDate = x.AddedDate,
                        UpdatedBy = x.UpdatedBy,
                        UpdatedDate = x.UpdatedDate,
                        AllowReorder = x.AllowReorder,
                        FileUploadPath = x.FileUploadPath,
                        SourcingComment = x.SourcingComment,
                        SourcingCommentBy = x.SourcingCommentBy,
                        SourcingCommentDate = x.SourcingCommentDate,
                        SourcingAllowReorder = x.SourcingAllowReorder,
                        AddedByName =
                            _dbEntities.CmnUsers.Where(v => v.CmnUserId == x.AddedBy)
                                .Select(v => v.UserFullName)
                                .FirstOrDefault(),
                        Department = (from u in _dbEntities.CmnUsers
                                      join r in _dbEntities.CmnRoles on u.RoleName equals r.RoleName
                                      where u.CmnUserId == x.AddedBy
                                      select r.RoleDescription).FirstOrDefault(),
                        SourcingCommentByName = _dbEntities.CmnUsers.Where(v => v.CmnUserId == x.SourcingCommentBy)
              .Select(v => v.UserFullName)
              .FirstOrDefault(),
                        ManagementComment = x.ManagementComment,
                        ManagementCommentBy = x.ManagementCommentBy,
                        ManagementCommentDate = x.ManagementCommentDate,
                        ManagementDecision = x.ManagementDecision
                    }).OrderByDescending(x => x.AddedDate).ToList();
            return m;
        }

        public List<string> GetRoleDescriptions()
        {
            var v = _dbEntities.CmnRoles.Select(x => x.RoleDescription).Distinct().ToList();
            return v;
        }
        #endregion

        #region SMT capacity exceed log

        public List<SmtCapacityExceedLogModel> SmtCapacityExceedLogModels()
        {
            var dateToCompare = DateTime.Now.AddMonths(-4);
            var v = _dbEntities.SmtCapacityExceedLogs.Where(x => x.PoDate > dateToCompare).Select(x => new SmtCapacityExceedLogModel
            {
                Id = x.Id,
                SmtCapacityCrossedForModel = x.SmtCapacityCrossedForModel,
                RunningSmtQuantity = x.RunningSmtQuantity,
                OrderNo = x.OrderNo,
                OrderQuantity = x.OrderQuantity,
                Month = x.Month,
                Year = x.Year,
                AddedDate = x.AddedDate,
                PoDate = x.PoDate,
                ProjectMasterId = x.ProjectMasterId
            }).ToList();
            return v;
        }
        #endregion

        #region Process Cost

        public List<ProjectOrderQuantityDetailModel> GetProjectOrderQuantityDetailModels()
        {
            var model =
                _dbEntities.ProjectOrderQuantityDetails.Where(x => x.IsActive == true)
                    .Select(x => new ProjectOrderQuantityDetailModel
                    {
                        Id = x.Id,
                        ProjectMasterId = x.ProjectMasterId,
                        ProjectModel = x.ProjectModel,
                        OrderQuantity = x.OrderQuantity,
                        AddedDate = x.AddedDate,
                        AddedBy = x.AddedBy,
                        BTRCPush = x.BTRCPush,
                        IsActive = x.IsActive
                    }).ToList();
            return model;
        }

        public void SaveProcessCost(ProcessCostMonthWiseModel model)
        {
            Mapper.CreateMap<ProcessCostMonthWiseModel, ProcessCostMonthWise>();
            var v = Mapper.Map<ProcessCostMonthWise>(model);
            _dbEntities.ProcessCostMonthWises.AddOrUpdate(v);
            _dbEntities.SaveChanges();
        }

        public bool DuplicateProcessCostCheckerByVariantName(string variantName)
        {
            var v = _dbEntities.ProcessCostMonthWises.Any(x => x.VariantName == variantName);
            return v;
        }

        public List<ProcessCostMonthWiseModel> GetProcessCostMonthWiseModels()
        {
            var model = _dbEntities.ProcessCostMonthWises.Select(x => new ProcessCostMonthWiseModel
            {
                Id = x.Id,
                VariantName = x.VariantName,
                ProcessCost = x.ProcessCost,
                Month = x.Month,
                Year = x.Year,
                AddedBy = x.AddedBy,
                AddedDate = x.AddedDate
            }).ToList();
            return model;
        }

        public string MonthNumberToName(int monthno)
        {
            var monthName = "";
            switch (monthno)
            {
                case 1:
                    monthName = "January";
                    break;
                case 2:
                    monthName = "February";
                    break;
                case 3:
                    monthName = "March";
                    break;
                case 4:
                    monthName = "April";
                    break;
                case 5:
                    monthName = "May";
                    break;
                case 6:
                    monthName = "June";
                    break;
                case 7:
                    monthName = "July";
                    break;
                case 8:
                    monthName = "August";
                    break;
                case 9:
                    monthName = "September";
                    break;
                case 10:
                    monthName = "October";
                    break;
                case 11:
                    monthName = "November";
                    break;
                case 12:
                    monthName = "December";
                    break;
            }
            return monthName;
        }

        public Produced_UnProducedIMEIViewModel GetProductionInformation(string modelname, string order)
        {

            Produced_UnProducedIMEIViewModel viewModel = new Produced_UnProducedIMEIViewModel();
            String connectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "";
                query = string.Format(@"SELECT
      OrderQTY.ProjectModel
    , OrderQTY.Order_Num AS OrderNumber
    , CASE WHEN OrderQTY.OrderQuantity IS NULL THEN 0 ELSE OrderQTY.OrderQuantity END AS OrderQuantity
    , CASE WHEN ProjectMaster.Produced IS NULL THEN 0 ELSE ProjectMaster.Produced END AS Produced
    , CASE WHEN ProjectMaster.Unproduced IS NULL THEN 0 ELSE ProjectMaster.Unproduced END AS UnProduced
    , CASE WHEN ProjectMaster.LastMonthIMEI IS NULL THEN 0 ELSE ProjectMaster.LastMonthIMEI END AS LastMonthIMEIProduced
    , OrderQTY.ProjectType
FROM
    (
     SELECT
           d.ProjectModel
         , p.OrderNuber Order_Num
         , Sum(CASE
                   WHEN d.OrderQuantity IS NOT NULL
                   THEN d.OrderQuantity
                   ELSE 0
               END) OrderQuantity
         , p.ProjectType
     FROM
         CellPhoneProject.dbo.ProjectOrderQuantityDetails d JOIN
         CellPhoneProject.dbo.ProjectMasters p ON p.ProjectMasterId = d.ProjectMasterId
     WHERE
          --p.ProjectStatus <> 'REJECTED'
		  p.IsActive = 1
          AND d.IsActive = 1
     GROUP BY
             d.ProjectModel
           , p.OrderNuber
           , p.ProjectType
    ) OrderQTY LEFT JOIN
    (
     SELECT
           xx.ProjectModel
         , xx.OrderNuber AS Order_No
         , xx.OrderQuantity
         , xy.Produced
         , xx.OrderQuantity - xy.Produced AS Unproduced
         , xy.LastMonthIMEI
     FROM
         (
          SELECT
                d.ProjectModel
              , Sum(CASE
                        WHEN d.OrderQuantity IS NOT NULL
                        THEN d.OrderQuantity
                        ELSE 0
                    END) OrderQuantity
              , CellPhoneProject.dbo.ProjectMasters.OrderNuber
          FROM
              CellPhoneProject.dbo.ProjectOrderQuantityDetails d JOIN
              CellPhoneProject.dbo.ProjectMasters ON CellPhoneProject.dbo.ProjectMasters.ProjectMasterId =
                                                     d.ProjectMasterId
          WHERE
               CellPhoneProject.dbo.ProjectMasters.AddedDate >= DateAdd(YEAR, -4, GetDate())
               AND CellPhoneProject.dbo.ProjectMasters.SourcingType <> 'OEM'
               AND ProjectMasters.IsActive=1
          GROUP BY
                  d.ProjectModel
                , CellPhoneProject.dbo.ProjectMasters.OrderNuber
         ) xx LEFT JOIN
         (
          SELECT
                IMEI_QTY.Model
              , IMEI_QTY.Produced
              , IMEI_QTY.Order_Num
              , IMEI_QTY.Order_No
              , LastMonth.LastMonthIMEI
          FROM
              (
               SELECT
                     RBSYNERGY.dbo.tblBarCodeInv.Model
                   , Count(RBSYNERGY.dbo.tblBarCodeInv.BarCode) AS Produced
                   , RBSYNERGY.dbo.tblBarCodeInv.Updatedby AS Order_Num
                   , SubString(RBSYNERGY.dbo.tblBarCodeInv.Updatedby, PatIndex('%[0-9]%',
                     RBSYNERGY.dbo.tblBarCodeInv.Updatedby), Len(RBSYNERGY.dbo.tblBarCodeInv.Updatedby)) AS Order_No
               FROM
                   RBSYNERGY.dbo.tblBarCodeInv
               WHERE
                    RBSYNERGY.dbo.tblBarCodeInv.DateAdded >= DateAdd(YEAR, -4, GetDate())
                    AND RBSYNERGY.dbo.tblBarCodeInv.Updatedby NOT IN ('Not Tracable', 'Test Order')
               GROUP BY
                       RBSYNERGY.dbo.tblBarCodeInv.Model
                     , RBSYNERGY.dbo.tblBarCodeInv.Updatedby
              ) IMEI_QTY LEFT JOIN
              (
               SELECT
                     RBSYNERGY.dbo.tblBarCodeInv.Model
                   , Count(RBSYNERGY.dbo.tblBarCodeInv.BarCode) LastMonthIMEI
                   , SubString(RBSYNERGY.dbo.tblBarCodeInv.Updatedby, PatIndex('%[0-9]%',
                     RBSYNERGY.dbo.tblBarCodeInv.Updatedby), Len(RBSYNERGY.dbo.tblBarCodeInv.Updatedby)) AS Order_No
               FROM
                   RBSYNERGY.dbo.tblBarCodeInv
               WHERE
                    RBSYNERGY.dbo.tblBarCodeInv.PrintDate >= DateAdd(MONTH, -1, GetDate())
                    AND RBSYNERGY.dbo.tblBarCodeInv.Updatedby NOT IN ('Not Tracable', 'Test Order')
               GROUP BY
                       RBSYNERGY.dbo.tblBarCodeInv.Model
                     , RBSYNERGY.dbo.tblBarCodeInv.Updatedby
              ) LastMonth ON IMEI_QTY.Model = LastMonth.Model
                             AND IMEI_QTY.Order_No = LastMonth.Order_No
         ) xy ON xx.ProjectModel = xy.Model
                 AND xx.OrderNuber = xy.Order_No
    ) ProjectMaster ON OrderQTY.ProjectModel = ProjectMaster.ProjectModel
                       AND OrderQTY.Order_Num = ProjectMaster.Order_No");

                var command = new SqlCommand(query, connection);
                command.CommandTimeout = 200;

                SqlDataReader reader = command.ExecuteReader();
                viewModel.Produced_UnproducedIMEIs = new List<Produced_UnproducedIMEI>();
                while (reader.Read())
                {
                    var obj = new Produced_UnproducedIMEI();
                    obj.ProjectModel = Convert.ToString(reader["ProjectModel"]);
                    obj.Produced = (reader["Produced"] != null ? Convert.ToInt64(reader["Produced"]) : 0);
                    obj.OrderQuantity = Convert.ToInt64(reader["OrderQuantity"]);
                    obj.OrderNumber = reader["OrderNumber"] != null ? Convert.ToInt32(reader["OrderNumber"].ToString().Trim()) : 0;
                    obj.LastMonthIMEIProduced = (reader["LastMonthIMEIProduced"] != System.DBNull.Value
                        ? Convert.ToInt64(reader["LastMonthIMEIProduced"])
                        : 0);
                    obj.UnProduced = (reader["UnProduced"] != null ? Convert.ToInt64(reader["UnProduced"]) : 0);

                    viewModel.Produced_UnproducedIMEIs.Add(obj);
                }
                connection.Close();
            }


            viewModel.Models = (from d in viewModel.Produced_UnproducedIMEIs
                                group d by d.ProjectModel
                                    into g
                                    select new ProjectMasterInv
                                    {
                                        ProjectModel = g.Key,
                                    }).ToList();
            viewModel.Orders = (from d in viewModel.Produced_UnproducedIMEIs
                                select new ProjectMasterInv
                                {
                                    ProjectModel = d.ProjectModel,
                                    Order_No = d.OrderNumber
                                }).ToList();
            if (modelname != "")
                viewModel.Produced_UnproducedIMEIs = viewModel.Produced_UnproducedIMEIs.Where(y => y.ProjectModel.ToUpper() == modelname.ToUpper()).ToList();
            if (order != "")
                viewModel.Produced_UnproducedIMEIs = viewModel.Produced_UnproducedIMEIs.Where(y => y.OrderNumber == Int32.Parse(order)).ToList();
            viewModel.ModelName = modelname;
            viewModel.Order = order;
            return viewModel;

        }

        #endregion

        #region FOC Claim
        public List<string> GetBomDescriptionByIdThenProjectModel(long id)
        {
            var projectModel =
                _dbEntities.ProjectOrderQuantityDetails.Where(x => x.Id == id)
                    .Select(x => x.ProjectModel)
                    .FirstOrDefault();
            var bomProductModelId = _mrpEntities.BomProductModels.Where(x => x.Model == projectModel).Select(x => x.Id).FirstOrDefault();
            var descriptions =
                _mrpEntities.BOMs.Where(x => x.BomProductModelId == bomProductModelId)
                    .Select(x => x.Description)
                    .ToList();
            var v = descriptions.GroupBy(x => x);
            descriptions = new List<string>();
            foreach (var x in v)
            {
                var y = x.Key;
                descriptions.Add(y);
            }
            return descriptions;
        }

        public List<string> GetSpareDescriptionByDescription(string description)
        {
            var spareDesc =
                _mrpEntities.BOMs.Where(x => x.Description == description).Select(x => x.SpareDescription).ToList();
            var group = spareDesc.GroupBy(x => x);
            spareDesc = new List<string>();
            foreach (var g in group)
            {
                spareDesc.Add(g.Key);
            }
            return spareDesc;
        }

        public FocClaimModel SaveFocClaimModel(FocClaimModel model)
        {
            model.BomProductModel =
                _dbEntities.ProjectOrderQuantityDetails.Where(x => x.Id == model.OrderQuantityDetailId).Select(x => x.ProjectModel).FirstOrDefault();
            model.BomId =
                _mrpEntities.BOMs.Where(
                    x => x.Description == model.Description && x.SpareDescription == model.SpareDescription)
                    .Select(x => x.Id)
                    .FirstOrDefault();
            model.BomType = _mrpEntities.BOMs.Where(x => x.Id == model.BomId).Select(x => x.BOMType).FirstOrDefault();
            model.BomProductModelId = _mrpEntities.BOMs.Where(x => x.Id == model.BomId).Select(x => x.BomProductModelId).FirstOrDefault();
            model.ProjectMasterId = _dbEntities.ProjectOrderQuantityDetails.Where(x => x.Id == model.OrderQuantityDetailId).Select(x => x.ProjectMasterId).FirstOrDefault();
            model.OrderNo =
                _dbEntities.ProjectMasters.Where(x => x.ProjectMasterId == model.ProjectMasterId)
                    .Select(x => x.OrderNuber)
                    .FirstOrDefault();
            model.OrderQuantity = _dbEntities.ProjectOrderQuantityDetails.Where(x => x.Id == model.OrderQuantityDetailId).Select(x => x.OrderQuantity).FirstOrDefault();
            Mapper.CreateMap<FocClaimModel, FocClaim>();
            var v = Mapper.Map<FocClaim>(model);
            _dbEntities.FocClaims.Add(v);
            _dbEntities.SaveChanges();
            model.Id = v.Id;
            return model;
        }

        public FocClaimModel UpdateFocClaimModel(FocClaimModel model)
        {
            Mapper.CreateMap<FocClaimModel, FocClaim>();
            var v = Mapper.Map<FocClaim>(model);
            _dbEntities.FocClaims.AddOrUpdate(v);
            _dbEntities.SaveChanges();
            model = GetFocClaimById(model.Id);
            return model;
        }

        public List<FocClaimModel> GetFocClaimAddedBy(long claimedBy)
        {
            var model = _dbEntities.FocClaims.Where(x => x.ClaimedBy == claimedBy).Select(x => new FocClaimModel
            {
                Id = x.Id,
                ProjectMasterId = x.ProjectMasterId,
                OrderQuantityDetailId = x.OrderQuantityDetailId,
                OrderNo = x.OrderNo,
                OrderQuantity = x.OrderQuantity,
                BomProductModelId = x.BomProductModelId,
                BomProductModel = x.BomProductModel,
                BomType = x.BomType,
                BomId = x.BomId,
                Description = x.Description,
                SpareDescription = x.SpareDescription,
                ClaimQuantity = x.ClaimQuantity,
                ClaimDate = x.ClaimDate,
                ClaimedBy = x.ClaimedBy,
                ReceiveQuantity = x.ReceiveQuantity,
                ReceivedBy = x.ReceivedBy,
                ReceivedDate = x.ReceivedDate,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate,
                ClaimedByName = _dbEntities.CmnUsers.Where(v => v.CmnUserId == x.ClaimedBy).Select(v => v.UserFullName).FirstOrDefault(),
                ReceivedByName = _dbEntities.CmnUsers.Where(v => v.CmnUserId == x.ReceivedBy).Select(v => v.UserFullName).FirstOrDefault()
            }).ToList();
            return model;
        }

        public List<FocClaimModel> GetAllFocClaims()
        {
            var model = _dbEntities.FocClaims.Select(x => new FocClaimModel
            {
                Id = x.Id,
                ProjectMasterId = x.ProjectMasterId,
                OrderQuantityDetailId = x.OrderQuantityDetailId,
                OrderNo = x.OrderNo,
                OrderQuantity = x.OrderQuantity,
                BomProductModelId = x.BomProductModelId,
                BomProductModel = x.BomProductModel,
                BomType = x.BomType,
                BomId = x.BomId,
                Description = x.Description,
                SpareDescription = x.SpareDescription,
                ClaimQuantity = x.ClaimQuantity,
                ClaimDate = x.ClaimDate,
                ClaimedBy = x.ClaimedBy,
                ReceiveQuantity = x.ReceiveQuantity,
                ReceivedBy = x.ReceivedBy,
                ReceivedDate = x.ReceivedDate,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate,
                ClaimedByName = _dbEntities.CmnUsers.Where(v => v.CmnUserId == x.ClaimedBy).Select(v => v.UserFullName).FirstOrDefault(),
                ReceivedByName = _dbEntities.CmnUsers.Where(v => v.CmnUserId == x.ReceivedBy).Select(v => v.UserFullName).FirstOrDefault()
            }).ToList();
            return model;
        }

        public FocClaimModel GetFocClaimById(long id)
        {
            var model = _dbEntities.FocClaims.Where(x => x.Id == id).Select(x => new FocClaimModel
            {
                Id = x.Id,
                ProjectMasterId = x.ProjectMasterId,
                OrderQuantityDetailId = x.OrderQuantityDetailId,
                OrderNo = x.OrderNo,
                OrderQuantity = x.OrderQuantity,
                BomProductModelId = x.BomProductModelId,
                BomProductModel = x.BomProductModel,
                BomType = x.BomType,
                BomId = x.BomId,
                Description = x.Description,
                SpareDescription = x.SpareDescription,
                ClaimQuantity = x.ClaimQuantity,
                ClaimDate = x.ClaimDate,
                ClaimedBy = x.ClaimedBy,
                ReceiveQuantity = x.ReceiveQuantity,
                ReceivedBy = x.ReceivedBy,
                ReceivedDate = x.ReceivedDate,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate,
                ClaimedByName = _dbEntities.CmnUsers.Where(v => v.CmnUserId == x.ClaimedBy).Select(v => v.UserFullName).FirstOrDefault(),
                ReceivedByName = _dbEntities.CmnUsers.Where(v => v.CmnUserId == x.ReceivedBy).Select(v => v.UserFullName).FirstOrDefault()
            }).FirstOrDefault();
            return model;
        }
        #endregion

        #region create project variant for pm

        public List<ProjectOrderQuantityDetailModel> GetOrderQuantityDetailByProjectId(long id)
        {
            var model =
                _dbEntities.ProjectOrderQuantityDetails.Where(x => x.ProjectMasterId == id && x.IsActive == true)
                    .Select(x => new ProjectOrderQuantityDetailModel
                    {
                        Id = x.Id,
                        ProjectMasterId = x.ProjectMasterId,
                        ProjectModel = x.ProjectModel,
                        OrderQuantity = x.OrderQuantity,
                        AddedDate = x.AddedDate,
                        AddedBy = x.AddedBy,
                        AddedByName = _dbEntities.CmnUsers.Where(v => v.CmnUserId == x.AddedBy).Select(v => v.UserFullName).FirstOrDefault(),
                        UpdatedBy = x.UpdatedBy,
                        UpdatedByName = _dbEntities.CmnUsers.Where(v => v.CmnUserId == x.UpdatedBy).Select(v => v.UserFullName).FirstOrDefault(),
                        UpdatedDate = x.UpdatedDate,
                        BTRCPush = x.BTRCPush,
                        IsActive = x.IsActive,
                        RamVendor = x.RamVendor,
                        RomVendor = x.RomVendor,
                        VariantClosingBy = x.VariantClosingBy,
                        VariantClosingDate = x.VariantClosingDate,
                        VariantClosingByName = _dbEntities.CmnUsers.Where(v => v.CmnUserId == x.VariantClosingBy).Select(v => v.UserFullName).FirstOrDefault()
                    }).ToList();
            return model;
        }

        public ProjectOrderQuantityDetailModel GetOrderQuantityDetailById(long id)
        {
            var model =
                _dbEntities.ProjectOrderQuantityDetails.Where(x => x.Id == id && x.IsActive == true)
                    .Select(x => new ProjectOrderQuantityDetailModel
                    {
                        Id = x.Id,
                        ProjectMasterId = x.ProjectMasterId,
                        ProjectName = _dbEntities.ProjectMasters.Where(v => v.ProjectMasterId == x.ProjectMasterId).Select(v => v.ProjectName).FirstOrDefault(),
                        ProjectModel = x.ProjectModel,
                        OrderQuantity = x.OrderQuantity,
                        AddedDate = x.AddedDate,
                        AddedBy = x.AddedBy,
                        AddedByName = _dbEntities.CmnUsers.Where(v => v.CmnUserId == x.AddedBy).Select(v => v.UserFullName).FirstOrDefault(),
                        UpdatedBy = x.UpdatedBy,
                        UpdatedByName = _dbEntities.CmnUsers.Where(v => v.CmnUserId == x.UpdatedBy).Select(v => v.UserFullName).FirstOrDefault(),
                        UpdatedDate = x.UpdatedDate,
                        BTRCPush = x.BTRCPush,
                        IsActive = x.IsActive,
                        RamVendor = x.RamVendor,
                        RomVendor = x.RomVendor,
                        VariantClosingBy = x.VariantClosingBy,
                        VariantClosingDate = x.VariantClosingDate,
                        VariantClosingByName = _dbEntities.CmnUsers.Where(v => v.CmnUserId == x.VariantClosingBy).Select(v => v.UserFullName).FirstOrDefault()
                    }).FirstOrDefault();
            return model;
        }

        public List<ProjectOrderQuantityDetailModel> GetOrderQuantityDetails()
        {
            var model =
                _dbEntities.ProjectOrderQuantityDetails.Where(x => x.IsActive == true)
                    .Select(x => new ProjectOrderQuantityDetailModel
                    {
                        Id = x.Id,
                        ProjectMasterId = x.ProjectMasterId,
                        ProjectName = _dbEntities.ProjectMasters.Where(v => v.ProjectMasterId == x.ProjectMasterId).Select(v => v.ProjectModel).FirstOrDefault(),
                        ProjectModel = x.ProjectModel,
                        OrderNumber = _dbEntities.ProjectMasters.Where(v => v.ProjectMasterId == x.ProjectMasterId).Select(v => v.OrderNuber).FirstOrDefault(),
                        OrderQuantity = x.OrderQuantity,
                        TotalOrderQuantity = _dbEntities.ProjectPurchaseOrderForms.Where(v => v.ProjectMasterId == x.ProjectMasterId).Select(v => v.Quantity).FirstOrDefault(),
                        AddedDate = x.AddedDate,
                        AddedBy = x.AddedBy,
                        AddedByName = _dbEntities.CmnUsers.Where(v => v.CmnUserId == x.AddedBy).Select(v => v.UserFullName).FirstOrDefault(),
                        UpdatedBy = x.UpdatedBy,
                        UpdatedByName = _dbEntities.CmnUsers.Where(v => v.CmnUserId == x.UpdatedBy).Select(v => v.UserFullName).FirstOrDefault(),
                        UpdatedDate = x.UpdatedDate,
                        BTRCPush = x.BTRCPush,
                        IsActive = x.IsActive,
                        RamVendor = x.RamVendor,
                        RomVendor = x.RomVendor,
                        VariantClosingBy = x.VariantClosingBy,
                        VariantClosingDate = x.VariantClosingDate,
                        VariantClosingByName = _dbEntities.CmnUsers.Where(v => v.CmnUserId == x.VariantClosingBy).Select(v => v.UserFullName).FirstOrDefault(),
                        ClosingRemarks = x.ClosingRemarks
                    }).OrderByDescending(x => x.ProjectMasterId).ToList();
            return model;
        }

        public bool VariantAlreadyExists(string projectModel, long projectId = 0)
        {
            var v =
                _dbEntities.ProjectOrderQuantityDetails.Where(
                    x => x.ProjectMasterId == projectId && x.ProjectModel == projectModel).ToList();
            if (v.Any())
            {
                return true;
            }
            return false;
        }

        public ProjectOrderQuantityDetailModel SaveUpdateProjectVariantInOrderQuantityDetail(ProjectOrderQuantityDetailModel model)
        {
            model.OrderNumber =
                _dbEntities.ProjectMasters.Where(x => x.ProjectMasterId == model.ProjectMasterId)
                    .Select(x => x.OrderNuber)
                    .FirstOrDefault();
            Mapper.CreateMap<ProjectOrderQuantityDetailModel, ProjectOrderQuantityDetail>();
            var v = Mapper.Map<ProjectOrderQuantityDetail>(model);
            _dbEntities.ProjectOrderQuantityDetails.AddOrUpdate(v);
            _dbEntities.SaveChanges();
            model.Id = v.Id;
            model.VariantClosingByName =
                _dbEntities.CmnUsers.Where(x => x.CmnUserId == v.VariantClosingBy)
                    .Select(x => x.UserFullName)
                    .FirstOrDefault();
            return model;
        }
        #endregion

        #region LC Permission Dashboard

        public List<ProjectMaster> GetProjectInfoByProjectModel(string projectModel)
        {
            var info = _dbEntities.ProjectMasters.Where(x => x.ProjectModel == projectModel).ToList();
            return info;
        }

        public List<LCOpeningPermission> GeAllApprovedLcPermissions()
        {
            var lcPer = _dbEntities.LCOpeningPermissions.Where(x => x.IsApproved == true).OrderBy(x => x.OpeningDate).ToList();
            return lcPer;
        }

        public List<LCOpeningPermission> GeAllPipelineLcPermissions()
        {
            var lcPer = _dbEntities.LCOpeningPermissions.Where(x => x.IsApproved == true && x.WarehouseReceiveDate == null).OrderBy(x => x.OpeningDate).ToList();
            return lcPer;
        }

        public List<LCOpeningPermission> GetLcPermissionsByProjectId(long projectId)
        {
            var lcPer = _dbEntities.LCOpeningPermissions.Where(x => x.ProjectMasterId == projectId && x.IsApproved == true).OrderBy(x => x.OpeningDate).ToList();
            return lcPer;
        }

        public List<LCOpeningPermission> GetLcPermissionByProjectModel(string projectModel)
        {
            var lcPer = _dbEntities.LCOpeningPermissions.Where(x => x.Model == projectModel && x.IsApproved == true).OrderBy(x => x.OpeningDate).ToList();
            return lcPer;
        }

        public List<ProjectLcModel> GetProjectLcByProjectName(string projectName)
        {
            var query =
                string.Format(
                    @"SELECT Concat(pm.ProjectName,' (Order ',pm.OrderNuber,')') as ProjectName,OpeningDate,LcValue FROM ProjectLcs lc inner join ProjectMasters pm on lc.ProjectMasterId=pm.ProjectMasterId where lc.lcvalue is not null and lc.openingdate is not null and pm.ProjectName='{0}' order by OpeningDate", projectName);
            var v = _dbEntities.Database.SqlQuery<ProjectLcModel>(query).ToList();
            return v;
        }

        public List<ProjectLcModel> GetMonthWiseTotalLcValue()
        {
            var query =
                string.Format(
                    @"select CONCAT(lc.Year,'-',lc.Month,'-1') StrLcOpeningDate,SUM(lc.LcValue) as LcValue from (SELECT LcValue,DATEPART(MONTH,OpeningDate) AS Month,DATEPART(YEAR,OpeningDate) AS Year FROM ProjectLcs where lcvalue is not null and openingdate is not null )lc group by lc.Month,lc.Year");
            var v = _dbEntities.Database.SqlQuery<ProjectLcModel>(query).ToList();
            return v;
        }

        public List<ProjectLcModel> GetMonthWiseApprovedlLcValue()
        {
            var query =
                string.Format(
                    @"select StrLcOpeningDate, SUM(LcValue) as LcValue from (
select 
CONCAT(FORMAT(ApprovedDate,'yyyy-MM'),'-1') StrLcOpeningDate
,sum(cast(LcAmount as decimal(18,2))) as LcValue 
from 
LCOpeningPermission
where convert(date,ApprovedDate) between DATEADD(MONTH,-12*3,GETDATE()) and GETDATE() and (IsActive=1 OR IsActive IS NULL) 
group by 
FORMAT(ApprovedDate,'yyyy-MM')
union
select 
CONCAT(FORMAT(ApprovedDate,'yyyy-MM'),'-1') StrLcOpeningDate
,SUM(CONVERT(decimal(18,2),case when LcAmount is null then 0 else CONVERT(decimal(18,2),LcAmount) end)) as LcValue 
from 
LcOpeningPermissionOtherProducts
where convert(date,ApprovedDate) between DATEADD(MONTH,-12*3,GETDATE()) and GETDATE() and (IsActive=1 OR IsActive IS NULL)
group by 
FORMAT(ApprovedDate,'yyyy-MM'))A
group by StrLcOpeningDate");
            //var handsetLc = _dbEntities.LCOpeningPermissions.Where(x => x.IsActive != false && x.ApprovedDate!=null).ToList();
            //var otherLc = _dbEntities.LcOpeningPermissionOtherProducts.Where(x => x.IsActive != false && x.ApprovedDate != null).ToList();
            //var lc = new List<ProjectLcModel>();
            //foreach (var item in handsetLc)
            //{
            //    if (item.Currency != "USD")
            //    {
            //        item.LcAmount = CommonConversion.CurrencyConversion(Convert.ToDecimal(item.LcAmount), item.Currency, "USD").ToString("F");
            //        item.Currency = "USD";
            //    }
            //}
            //foreach (var item in otherLc)
            //{
            //    if (item.Currency != "USD")
            //    {
            //        item.LcAmount = CommonConversion.CurrencyConversion(Convert.ToDecimal(item.LcAmount), item.Currency, "USD").ToString("F");
            //        item.Currency = "USD";
            //    }
            //}
            var v = _dbEntities.Database.SqlQuery<ProjectLcModel>(query).ToList();
            return v;
        }

        public List<ProjectLcModel> GetMonthWiseTotalLcValueFromOracle()
        {
            var lcs = new List<ProjectLcModel>();
            OracleConnection con = OracleDbConnection.GetNewConnection();
            OracleCommand cmd = new OracleCommand();
            con.Open();
            cmd.CommandText = string.Format(@"select CONCAT(CONCAT(A.YEAR,'-'),CONCAT(A.MONTH,'-1')) LC_OPENING_DATE,SUM(A.LC_VALUE) LC_VALUE from
(SELECT  DISTINCT XLD.LC_VALUE, to_char(XLD.LC_OPENING_DATE,'YYYY') YEAR,to_char(XLD.LC_OPENING_DATE,'MM') MONTH
FROM APPS.XX_LC_MST MST, APPS.XX_LC_DETAILS XLD, APPS.XX_PURCHASE_ORDER XPO 
WHERE MST.LC_MST_ID=XLD.LC_MST_ID AND MST.ORG_ID=XLD.ORG_ID AND MST.ORG_ID=XPO.ORG_ID(+) 
AND XLD.LC_NUMBER=XPO.LC_NUMBER(+) AND XLD.ORG_ID=223 
AND  TRUNC(XLD.LC_OPENING_DATE) BETWEEN add_months( trunc(sysdate), -12*3 ) AND SYSDATE
 AND XLD.LC_VALUE IS NOT NULL)A 
GROUP BY A.MONTH,A.YEAR ORDER BY A.YEAR,A.MONTH");
            cmd.Connection = con;
            cmd.CommandTimeout = 6000;
            OracleDataReader dr = cmd.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    var lc = new ProjectLcModel
                    {
                        StrLcOpeningDate = (dr["LC_OPENING_DATE"]).ToString(),
                        LcValue = Convert.ToDecimal(dr["LC_VALUE"])

                    };
                    lcs.Add(lc);
                }
            }
            return lcs;
        }

        public LcOpeningPermissionModel GetLcOpeningPermissionById(long id)
        {
            var model = _dbEntities.LCOpeningPermissions.Where(x => x.Id == id).Select(x => new LcOpeningPermissionModel
            {
                Id = x.Id,
                ProjectMasterId = x.ProjectMasterId,
                CompanyName = x.CompanyName,
                OpeningDate = x.OpeningDate,
                SupplierName = x.SupplierName,
                SupplierGrade = x.SupplierGrade,
                Product = x.Product,
                Model = x.Model,
                OrderNo = x.OrderNo,
                PreviousOrderQunatity = x.PreviousOrderQunatity,
                StockQuantity = x.StockQuantity,
                PipelineQuantity = x.PipeLineQuantity,
                OrderQuantity = x.OrderQuantity,
                TotalAmount = x.TotalAmount,
                ApproxDateOfShipment = x.ApproxDateOfShipment,
                AddedBy = x.AddedBy,
                AddedDate = x.AddedDate,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate,
                IsActive = x.IsActive,
                IsApproved = x.IsApproved,
                ApprovedBy = x.ApprovedBy,
                ApprovedDate = x.ApprovedDate,
                ApprovedByRemarks = x.ApprovedByRemarks,
                CheckedBy = x.CheckedBy,
                CheckedDate = x.CheckedDate,
                VerifiedBy = x.VerifiedBy,
                VerifyDate = x.VerifyDate,
                Remarks = x.Remarks,
                TtiPerLine = x.TtiPerLine,
                LcAmount = x.LcAmount,
                UnitPrice = x.UnitPrice,
                OraclePoNo = x.OraclePoNo,
                WarehouseReceiveDate = x.WarehouseReceiveDate,
                ShipmentConfirmDate = x.ShipmentConfirmDate,
                SourcingApprovalBy = x.SourcingApprovalBy,
                SourcingApprovalDate = x.SourcingApprovalDate,
                SourcingRemarks = x.SourcingRemarks,
                CeoApprovalBy = x.CeoApprovalBy,
                CeoApprovalDate = x.CeoApprovalDate,
                CeoRemarks = x.CeoRemarks,
                AccountsApprovalBy = x.AccountsApprovalBy,
                AccountsApprovalDate = x.AccountsApprovalDate,
                AccountsRemarks = x.AccountsRemarks,
                FinanceApprovalBy = x.FinanceApprovalBy,
                FinanceApprovalDate = x.FinanceApprovalDate,
                FinanceRemarks = x.FinanceRemarks,
                AcknowledgeRemarks = x.AcknowledgeRemarks,
                AcknowledgeDate = x.AcknowledgeDate,
                AcknowledgedBy = x.AcknowledgedBy,
                Currency = x.Currency,
                Rate = x.Rate,
                BiRemarks = x.BiRemarks,
                BiApprovalBy = x.BiApprovalBy,
                BiApprovalDate = x.BiApprovalDate
            }).FirstOrDefault();
            return model;
        }

        public SalesForecastingReport GetSalesForecastingReportByVariantName(string model)
        {
            var yesterdayStart = (DateTime.Now.AddDays(-1));
            //var yesterdayEnd = yesterdayStart.AddDays(1).AddTicks(-1);
            var v = _dbrbsyEntities.SalesForecastingReports.FirstOrDefault(x => x.Model == model && x.AddedDate.HasValue && EntityFunctions.TruncateTime(x.AddedDate.Value) == EntityFunctions.TruncateTime(yesterdayStart));
            return v;
        }

        public List<ProjectMaster> GetProjectListByProjectName(string projectName)
        {
            var v = _dbEntities.ProjectMasters.Where(x => x.ProjectName == projectName && x.IsActive).OrderBy(x => x.OrderNuber).ToList();
            return v;
        }

        public List<MasterPoVariantModel> GetMasterPoVariantByProjectName(string projectName)
        {
            var query =
                string.Format(
                    @"select pm.ProjectName
,pm.SourcingType
,po.Quantity as PoQuantity
,Count(i.BarCode) as Produced
,(d.OrderQuantity-Count(i.BarCode)) as UnProduced
,cast(((cast(d.OrderQuantity as decimal(8,2))-Count(i.BarCode))/d.OrderQuantity)*100 as decimal(8,0)) as UnproducedPercentage
,pm.OrderNuber as OrderNumber
,d.projectmodel as VariantName
,d.OrderQuantity as VariantQuantity
,po.PoDate 
from ProjectOrderQuantityDetails d 
inner join projectmasters pm on d.Projectmasterid=pm.projectmasterid 
inner join ProjectPurchaseOrderForms po on d.projectmasterid=po.projectmasterid
left join RBSYNERGY.dbo.tblBarCodeInv i on (i.Model=d.ProjectModel and SubString(i.Updatedby, PatIndex('%[0-9]%',i.Updatedby), Len(i.Updatedby))=pm.OrderNuber and i.Updatedby NOT IN ('Not Tracable', 'Test Order'))
where pm.projectname='{0}' and pm.IsActive=1 
group by pm.ProjectName,pm.SourcingType,po.Quantity,pm.OrderNuber,d.projectmodel,po.PoDate,d.OrderQuantity
order by pm.OrderNuber",
                    projectName);
            var v = _dbEntities.Database.SqlQuery<MasterPoVariantModel>(query).ToList();
            return v;
        }

        public List<MasterPoVariantModel> GetRelevantModelByProjectId(long? id)
        {
            var query = string.Format(@"select pm.ProjectMasterId,pm.ProjectName,po.Quantity as PoQuantity,pm.OrderNuber as OrderNumber,d.projectmodel as VariantName,d.OrderQuantity as VariantQuantity,po.PoDate, pm.FinalPrice from ProjectOrderQuantityDetails d inner join projectmasters pm on d.Projectmasterid=pm.projectmasterid inner join ProjectPurchaseOrderForms po on d.projectmasterid=po.projectmasterid where pm.FinalPrice between (SELECT ((select FinalPrice from ProjectMasters where ProjectMasterId={0})-(SELECT (CASE WHEN (SELECT ProjectType from ProjectMasters WHERE ProjectMasterId={0})='Feature' THEN 1 ELSE 12 END)))) and (SELECT ((select FinalPrice from ProjectMasters where ProjectMasterId={0})+(SELECT (CASE WHEN (SELECT ProjectType from ProjectMasters WHERE ProjectMasterId={0})='Feature' THEN 1 ELSE 12 END)))) and pm.ProjectMasterId not in({0}) AND PM.AddedDate BETWEEN (SELECT DATEADD(year, -1,(SELECT PoDate from ProjectPurchaseOrderForms WHERE ProjectMasterId={0}))) AND (SELECT (SELECT PoDate from ProjectPurchaseOrderForms WHERE ProjectMasterId={0}))  order by pm.OrderNuber", id);
            var v = _dbEntities.Database.SqlQuery<MasterPoVariantModel>(query).ToList();
            var relevants = new List<MasterPoVariantModel>();
            foreach (var i in v)
            {
                var variant =
                    _dbEntities.ProjectOrderQuantityDetails.Where(x => x.ProjectModel == i.ProjectName).FirstOrDefault();
                if (variant != null && variant.AddedDate > DateTime.Now.AddYears(-2))
                {
                    relevants.Add(i);
                }
            }
            return relevants;
        }

        public LcOpeningPermissionOtherProductModel GetLcOpeningPermissionOtherProductById(long id)
        {
            var query = string.Format("select * from LcOpeningPermissionOtherProducts where Id={0}", id);
            var m = _dbEntities.Database.SqlQuery<LcOpeningPermissionOtherProductModel>(query).FirstOrDefault();

            m.AddedByName =
                _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.AddedBy)
                    .Select(x => x.UserFullName)
                    .FirstOrDefault();
            m.ApprovedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.ApprovedBy)
                    .Select(x => x.UserFullName)
                    .FirstOrDefault();
            m.SourcingApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.SourcingApprovalBy)
                    .Select(x => x.UserFullName)
                    .FirstOrDefault();
            m.CeoApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.CeoApprovalBy)
                    .Select(x => x.UserFullName)
                    .FirstOrDefault();
            m.FinanceApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.FinanceApprovalBy)
                    .Select(x => x.UserFullName)
                    .FirstOrDefault();
            m.AccountsApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.AccountsApprovalBy)
                    .Select(x => x.UserFullName)
                    .FirstOrDefault();
            m.AcknowledgedByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == m.AcknowledgedBy)
                        .Select(x => x.UserFullName)
                        .FirstOrDefault();
            return m;
        }

        public LcOpeningPermissionModel SaveLcOpeningPermission(LcOpeningPermissionModel model)
        {
            Mapper.CreateMap<LcOpeningPermissionModel, LCOpeningPermission>();
            var v = Mapper.Map<LCOpeningPermission>(model);
            _dbEntities.LCOpeningPermissions.AddOrUpdate(v);
            _dbEntities.SaveChanges();
            model.SourcingApprovalByName =
                _dbEntities.CmnUsers.Where(x => x.CmnUserId == model.SourcingApprovalBy)
                    .Select(x => x.UserFullName)
                    .FirstOrDefault();
            model.AccountsApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == model.AccountsApprovalBy)
                    .Select(x => x.UserFullName)
                    .FirstOrDefault();
            model.FinanceApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == model.FinanceApprovalBy)
                    .Select(x => x.UserFullName)
                    .FirstOrDefault();
            model.CeoApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == model.CeoApprovalBy)
                    .Select(x => x.UserFullName)
                    .FirstOrDefault();
            model.BiApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == model.BiApprovalBy)
                    .Select(x => x.UserFullName)
                    .FirstOrDefault();
            return model;
        }

        public LcOpeningPermissionOtherProductModel SaveLcOpeningPermissionOtherProduct(LcOpeningPermissionOtherProductModel model)
        {
            Mapper.CreateMap<LcOpeningPermissionOtherProductModel, LcOpeningPermissionOtherProduct>();
            var v = Mapper.Map<LcOpeningPermissionOtherProduct>(model);
            _dbEntities.LcOpeningPermissionOtherProducts.AddOrUpdate(v);
            _dbEntities.SaveChanges();
            model.SourcingApprovalByName =
                _dbEntities.CmnUsers.Where(x => x.CmnUserId == model.SourcingApprovalBy)
                    .Select(x => x.UserFullName)
                    .FirstOrDefault();
            model.AccountsApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == model.AccountsApprovalBy)
                    .Select(x => x.UserFullName)
                    .FirstOrDefault();
            model.FinanceApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == model.FinanceApprovalBy)
                    .Select(x => x.UserFullName)
                    .FirstOrDefault();
            model.CeoApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == model.CeoApprovalBy)
                    .Select(x => x.UserFullName)
                    .FirstOrDefault();
            model.BiApprovalByName = _dbEntities.CmnUsers.Where(x => x.CmnUserId == model.BiApprovalBy)
                    .Select(x => x.UserFullName)
                    .FirstOrDefault();
            return model;
        }

        public List<LcOpeningPermissionFileModel> GetLcOpeningPermissionFilesByLcId(long id)
        {
            var v =
                _dbEntities.LcOpeningPermissionFiles.Where(x => x.LcPermissionId == id && x.FilePath != "failed")
                    .Select(x => new LcOpeningPermissionFileModel
                    {
                        Id = x.Id,
                        LcPermissionId = x.LcPermissionId,
                        FilePath = x.FilePath,
                        Remarks = x.Remarks,
                        AddedBy = x.AddedBy,
                        AddedDate = x.AddedDate
                    }).ToList();
            return v;
        }

        public List<LcOpeningPermissionOtherFileModel> GetLcOpeningPermissionOtherFilesByLcId(long id)
        {
            var v =
                _dbEntities.LcOpeningPermissionOtherFiles.Where(x => x.LcOtherPermissionId == id)
                    .Select(x => new LcOpeningPermissionOtherFileModel
                    {
                        Id = x.Id,
                        LcOtherPermissionId = x.LcOtherPermissionId,
                        FilePath = x.FilePath,
                        Remarks = x.Remarks,
                        AddedBy = x.AddedBy,
                        AddedDate = x.AddedDate
                    }).ToList();
            return v;
        }

        public List<String> GetModelListForRelevantModels()
        {
            var v = _dbEntities.ProjectOrderQuantityDetails.Select(x => x.ProjectModel).ToList();
            v = v.Distinct().ToList();
            //_dbrbsyEntities.tblProductMasters.Where(x => x.ProductType == "Cell Phone" && x.Inactive == false).Select(x=>x.ProductModel)
            //    .ToList();
            return v;
        }

        public List<ProjectOrderQuantityDetail> GetVariantsWithOrderNumber()
        {
            var v = _dbEntities.ProjectOrderQuantityDetails.Where(x => x.IsActive == true).ToList();
            foreach (var item in v)
            {
                var orderNo =
                    _dbEntities.ProjectMasters.Where(x => x.ProjectMasterId == item.ProjectMasterId)
                        .Select(x => x.OrderNuber)
                        .FirstOrDefault();
                item.ProjectModel = item.ProjectModel + " (Order " + orderNo + ")";
            }
            return v;
        }

        public List<ServiceToSalesRatio> GetServiceToSalesRatiosBySplitProjectName(string projectModel)
        {

            var ratios = new List<ServiceToSalesRatio>();
            if (projectModel != null)
            {
                var projectNmaes = projectModel.Split(',');
                foreach (var v in projectNmaes)
                {
                    var ratio = _dbrbsyEntities.ServiceToSalesRatios.Where(x => x.Model == v).OrderByDescending(x => x.ServiceToSalesRatioId).FirstOrDefault();
                    if (ratio != null)
                    {
                        ratios.Add(ratio);
                    }
                }
            }

            return ratios;
        }

        public List<tblActivatedInvoiceValueVsSpareValue> GetTblActivatedInvoiceValueVsSpareValues(string projectModel)
        {
            var values = new List<tblActivatedInvoiceValueVsSpareValue>();
            if (projectModel != null)
            {
                var projectNmaes = projectModel.Split(',');
                foreach (var v in projectNmaes)
                {
                    var value = _dbrbsyEntities.tblActivatedInvoiceValueVsSpareValues.Where(x => x.ModelName == v).OrderByDescending(x => x.Id).FirstOrDefault();
                    if (value != null)
                    {
                        values.Add(value);
                    }
                }
            }
            return values;
        }

        public List<OrderWiseDailyServiceToSalesRatio> GetOrderWiseDailyServiceToSalesRatios(string projectModel)
        {
            var yesterdayStart = DateTime.Now.AddDays(-1);
            var values = new List<OrderWiseDailyServiceToSalesRatio>();
            if (projectModel != null)
            {
                var projectNames = projectModel.Split(',');
                foreach (var v in projectNames)
                {
                    if (v != "All")
                    {
                        var value = _dbrbsyEntities.OrderWiseDailyServiceToSalesRatios.Where(x => x.ModelName == v && EntityFunctions.TruncateTime(x.AddedDate.Value) == EntityFunctions.TruncateTime(yesterdayStart)).OrderByDescending(x => x.Id).ToList();
                        values.AddRange(value);
                    }
                }
            }
            return values;
        }

        public void SaveLcPermissionFiles(LcOpeningPermissionFileModel model)
        {
            Mapper.CreateMap<LcOpeningPermissionFileModel, LcOpeningPermissionFile>();
            var v = Mapper.Map<LcOpeningPermissionFile>(model);
            _dbEntities.LcOpeningPermissionFiles.AddOrUpdate(v);
            _dbEntities.SaveChanges();
        }

        public void DeleteLcFile(long id)
        {
            var v = _dbEntities.LcOpeningPermissionFiles.FirstOrDefault(x => x.Id == id);
            _dbEntities.LcOpeningPermissionFiles.Remove(v);
            _dbEntities.SaveChanges();
        }

        public void SaveLcPermissionOtherFiles(LcOpeningPermissionOtherFileModel model)
        {
            Mapper.CreateMap<LcOpeningPermissionOtherFileModel, LcOpeningPermissionOtherFile>();
            var v = Mapper.Map<LcOpeningPermissionOtherFile>(model);
            _dbEntities.LcOpeningPermissionOtherFiles.AddOrUpdate(v);
            _dbEntities.SaveChanges();
        }

        public LcOpeningPermissionFileModel GetOpeningPermissionFileById(long id)
        {
            var v =
                _dbEntities.LcOpeningPermissionFiles.Where(x => x.Id == id).Select(x => new LcOpeningPermissionFileModel
                {
                    Id = x.Id,
                    LcPermissionId = x.LcPermissionId,
                    FilePath = x.FilePath,
                    Remarks = x.Remarks,
                    AddedBy = x.AddedBy,
                    AddedDate = x.AddedDate
                }).FirstOrDefault();
            return v;
        }

        public LcOpeningPermissionOtherFileModel GetOpeningPermissionOtherFileById(long id)
        {
            var v =
                _dbEntities.LcOpeningPermissionOtherFiles.Where(x => x.Id == id).Select(x => new LcOpeningPermissionOtherFileModel
                {
                    Id = x.Id,
                    LcOtherPermissionId = x.LcOtherPermissionId,
                    FilePath = x.FilePath,
                    Remarks = x.Remarks,
                    AddedBy = x.AddedBy,
                    AddedDate = x.AddedDate
                }).FirstOrDefault();
            return v;
        }

        public List<ProjectOrderPerformanceSum> GetProjectOrderPerformanceSumByModel(string modelName)
        {
            var v = _dbrbsyEntities.ProjectOrderPerformanceSums.Where(x => x.Model == modelName && x.Days == 60).ToList();
            return v;
        }
        #endregion

        #region SWOT analysis

        public List<ProjectMasterModel> GetSwotPendingProjects()
        {
            var v = _dbEntities.ProjectMasters.Where(x => x.ProjectStatus == "SWOTPENDING" && x.IsActive && x.ProjectType == "Smart").Select(x => new ProjectMasterModel
            {
                ProjectMasterId = x.ProjectMasterId,
                ProjectName = x.ProjectName,
                ProjectModel = x.ProjectModel,
                OrderNuber = x.OrderNuber,
                ChipsetName = x.ChipsetName,
                ChipsetFrequency = x.ChipsetFrequency,
                Ram = x.Ram,
                Rom = x.Rom,
                FrontCam = x.FrontCam,
                BackCam = x.BackCam,
                BatteryRating = x.BatteryRating,
                ProjectStatus = x.ProjectStatus,
                Added = x.Added,
                AddedDate = x.AddedDate,
                FinalPrice = x.FinalPrice
            }).ToList();
            return v;
        }

        public List<CommonSpecModel> GetSwotAnalysis(long projectId, long multiplier)
        {
            var commonSpec = new List<CommonSpecModel>();
            var projectInfo = GetProjectInfoByProjectId(projectId);
            var allPrice = _managementRepository.GetAccessoriesPrices(projectId);
            double price = 0;
            foreach (var p in allPrice)
            {
                price = price + Convert.ToDouble(p.Price);
            }
            var v = new CommonSpecModel
            {
                Brand = "Walton",
                Model = projectInfo.ProjectName,
                Chipset = projectInfo.ChipsetName,
                CpuCore = projectInfo.ChipsetCore,
                ClockSpeed = projectInfo.ChipsetFrequency,
                Ram = projectInfo.Ram,
                Rom = projectInfo.Rom,
                FrontCam = projectInfo.FrontCam,
                BackCam = projectInfo.BackCam,
                DisplaySize = projectInfo.DisplaySize.ToString(),
                Resolution = projectInfo.DisplayResulution,
                Battery = projectInfo.BatteryRating,
                DisplayType = projectInfo.DisplaySpeciality,
                MrpPrice = Convert.ToString(price * multiplier),//converted in BDT using multiplier 140
                ReleaseDate = "-"
            };
            commonSpec.Add(v);
            var fromPrice = Convert.ToDouble(v.MrpPrice) - 1001;//in bdt
            var toPrice = Convert.ToDouble(v.MrpPrice) + 1001;//in bdt
            //var oneUsdToBdt = CommonConversion.CurrencyConversion(Decimal.Parse("1"),
            //    "USD", "BDT");
            //oneUsdToBdt = Math.Round(oneUsdToBdt, 2);
            var fProjectsQuery = string.Format("select * from MkProjectSpecs where (Case when Price is null then (cast(upcomingprice as float)*{2}) else price end)  between {0} and {1}",
                fromPrice, toPrice, multiplier);
            _dbEntities.Database.CommandTimeout = 60;//1 minute
            var fProjects = _dbEntities.Database.SqlQuery<MkProjectSpec>(fProjectsQuery).ToList();
            foreach (var fp in fProjects)
            {
                v = new CommonSpecModel
                {
                    Brand = fp.Brand,
                    Model = fp.ModelName,
                    Chipset = fp.Chipset,
                    CpuCore = fp.CPU,
                    ClockSpeed = fp.CPUFrequency,
                    Ram = fp.RAM,
                    Rom = fp.ROM,
                    FrontCam = fp.FrontCamera,
                    BackCam = fp.BackCamera,
                    DisplaySize = fp.DisplaySize,
                    Resolution = fp.Resolution,
                    DisplayType = fp.DisplayType,
                    Battery = fp.BatteryCapacity,
                    MrpPrice = fp.Price,
                    UpcomingPrice = fp.UpcomingPrice == null ? fp.UpcomingPrice : Convert.ToString(Math.Round(Convert.ToDecimal(fp.UpcomingPrice)) * multiplier),//convert to BDT
                    ReleaseDate = fp.ReleaseDate != null ? fp.ReleaseDate.Value.ToShortDateString() : "-"
                };
                commonSpec.Add(v);
            }
            return commonSpec;
        }
        #endregion

        #region Barcode Excel Export from tblBarcodeInv

        public List<string> GetProductModelsFromProductMaster()
        {
            var v =
                _dbrbsyEntities.tblProductMasters.Where(x => x.Category1 == "Cellcom" && !x.Inactive)
                    .Select(x => x.ProductModel)
                    .ToList();
            return v;
        }
        public List<tblBarCodeInv> GeTblBarCodeInvByDateRangeAndProductModel(DateTime startDate, DateTime endDate, string productModel)
        {
            _dbrbsyEntities.Database.CommandTimeout = 30 * 60;//30 min
            var v =
                _dbrbsyEntities.tblBarCodeInvs.Where(x => x.PrintDate >= startDate && x.PrintDate <= endDate && x.Model == productModel).ToList();
            return v;
        }
        #endregion

        #region ColorWiseProjectVariant

        public List<ProjectOrderQuantityDetailModel> GetAllVariantsWithOrderNumber()
        {
            var v = _dbEntities.ProjectOrderQuantityDetails.Where(x => x.IsActive == true).Select(x => new ProjectOrderQuantityDetailModel
            {
                Id = x.Id,
                ProjectModel = x.ProjectModel + " (Order " + (_dbEntities.ProjectMasters.Where(m => m.ProjectMasterId == x.ProjectMasterId).Select(m => m.OrderNuber).FirstOrDefault()) + ")"
            }).ToList();
            return v;
        }

        public List<ColorWiseVariantQuantityModel> GetColorWiseVariantQuantityByVariantId(long id)
        {
            var v =
                _dbEntities.ColorWiseVariantQuantities.Where(x => x.VariantId == id)
                    .Select(x => new ColorWiseVariantQuantityModel
                    {
                        Id = x.Id,
                        VariantId = x.VariantId,
                        ProjectId = x.ProjectId,
                        ProjectName = _dbEntities.ProjectMasters.Where(m => m.ProjectMasterId == x.ProjectId).Select(m => m.ProjectName).FirstOrDefault(),
                        Color = x.Color,
                        Quantity = x.Quantity,
                        Remarks = x.Remarks,
                        AddedBy = x.AddedBy,
                        AddedDate = x.AddedDate,
                        UpdatedBy = x.UpdatedBy,
                        UpdatedDate = x.UpdatedDate
                    }).ToList();
            return v;
        }

        public ColorWiseVariantQuantityModel SaveColorWiseVariantQuantity(ColorWiseVariantQuantityModel m)
        {
            m.ProjectId =
                _dbEntities.ProjectOrderQuantityDetails.Where(x => x.Id == m.VariantId)
                    .Select(x => x.ProjectMasterId)
                    .FirstOrDefault();
            Mapper.CreateMap<ColorWiseVariantQuantityModel, ColorWiseVariantQuantity>();
            var v = Mapper.Map<ColorWiseVariantQuantity>(m);
            _dbEntities.ColorWiseVariantQuantities.Add(v);
            _dbEntities.SaveChanges();
            m.Id = v.Id;
            return m;
        }
        #endregion

        #region Project wise assigned PM
        public List<ProjectPmAssignModel> GetPmAssignModels()
        {
            var query =
                string.Format(
                    "SELECT pm.ProjectName,pm.OrderNuber,pa.AssignDate,cu.UserFullName,(select UserFullName from CmnUsers where CmnUserId=pa.AssignUserId) as AssignedByName FROM ProjectPmAssigns pa inner join ProjectMasters pm on pa.ProjectMasterId=pm.ProjectMasterId inner join CmnUsers cu on pa.ProjectManagerUserId=cu.CmnUserId where pm.IsActive=1");
            var v = _dbEntities.Database.SqlQuery<ProjectPmAssignModel>(query).ToList();
            return v;
        }
        #endregion

        public List<ProjectMasterModel> GetRejectedProjectList()
        {
            var v =
                _dbEntities.ProjectMasters.Where(x => x.ProjectStatus == "REJECTED" && x.PsApprovalBy != null).Select(x => new ProjectMasterModel
                {
                    ProjectMasterId = x.ProjectMasterId,
                    ProjectName = x.ProjectName,
                    OrderQuantity = x.OrderQuantity,
                    OrderNuber = x.OrderNuber,
                    PsApprovalBy = x.PsApprovalBy,
                    PsApprovalByName = _dbEntities.CmnUsers.Where(y => y.CmnUserId == x.PsApprovalBy).Select(y => y.UserFullName).FirstOrDefault(),
                    PsApprovalDate = x.PsApprovalDate,
                    PsRemarks = x.PsRemarks,
                    ManagentComment = x.ManagentComment
                }).ToList();
            return v;
        }

        #region China Qc Inspection Clearance Approval
        public List<ChinaQcInspectionsClearanceModel> GetChinaQcInspectionClearanceDetails()
        {
            String useridentity = HttpContext.Current.User.Identity.Name;
            var users = Convert.ToInt64(useridentity == "" ? "0" : useridentity);

            var userInfos = (from mm in _dbEntities.CmnUsers where mm.CmnUserId == users && mm.IsActive == true select mm).FirstOrDefault();

            var empIds =
                (from mm in _dbEntities.PmChinaQcInspectionsClearanceMailLists
                 where mm.EmployeeCode == userInfos.EmployeeCode && mm.IsActive == true
                 select mm.EmployeeCode);

            var query = new List<ChinaQcInspectionsClearanceModel>();
            if (empIds != null)
            {
                query = _dbEntities.Database.SqlQuery<ChinaQcInspectionsClearanceModel>(@"               
                select pcc.*,cu.UserFullName as Name from
                [CellPhoneProject].[dbo].[PmChinaQcInspectionsClearance] pcc
                inner join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=pcc.Added
                where pcc.ClearanceStatus in ('PENDING') and
                pcc.Id not in (select MainId from [CellPhoneProject].[dbo].PmChinaQcInspectionApprovalLog pcl where pcl.Added={0})", users).ToList();

            }
            return query;
        }

        public List<ChinaQcInspectionsClearanceModel> GetChinaQcInspectionClearanceApprovalDetails()
        {
            String useridentity = HttpContext.Current.User.Identity.Name;
            var users = Convert.ToInt64(useridentity == "" ? "0" : useridentity);
            var userInfos = (from mm in _dbEntities.CmnUsers where mm.CmnUserId == users && mm.IsActive == true select mm).FirstOrDefault();

            var empIds = (from mm in _dbEntities.PmChinaQcInspectionsClearanceMailLists
                          where mm.EmployeeCode == userInfos.EmployeeCode && mm.IsActive == true
                          select mm.EmployeeCode);

            var query = new List<ChinaQcInspectionsClearanceModel>();
            if (empIds != null)
            {
                query = _dbEntities.Database.SqlQuery<ChinaQcInspectionsClearanceModel>(@"
                select pcc.*,pcl.Remarks,pcl.ClearanceStatus as MyStatus,cu.UserFullName as Name from
                [CellPhoneProject].[dbo].[PmChinaQcInspectionsClearance] pcc 
                inner join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=pcc.Added
                inner join [CellPhoneProject].[dbo].PmChinaQcInspectionApprovalLog pcl on pcl.MainId=pcc.Id AND PCC.ProjectMasterId=PCL.ProjectMasterId
                where pcc.ClearanceStatus in ('APPROVED','NOTAPPROVED','PENDING') and pcl.Added={0}  ", users).ToList();
            }
            return query;
        }

        public string SaveChinaShipmentClearance(long ids1, long prIds, string sStatus, string remarks)
        {
            String useridentity = HttpContext.Current.User.Identity.Name;
            var users = Convert.ToInt64(useridentity == "" ? "0" : useridentity);

            // var chinaQc = _dbEntities.PmChinaQcInspectionsClearances.FirstOrDefault(x => x.Id == ids1);
            //chinaQc.ClearanceStatus = sStatus;
            //chinaQc.Updated = users;
            //chinaQc.UpdatedDate = DateTime.Now;
            //_dbEntities.PmChinaQcInspectionsClearances.AddOrUpdate(chinaQc);
            //_dbEntities.SaveChanges();

            var userInfos = (from mm in _dbEntities.CmnUsers where mm.CmnUserId == users && mm.IsActive == true select mm).FirstOrDefault();

            var mailIds =
                _dbEntities.PmChinaQcInspectionsClearanceMailLists.FirstOrDefault(
                    x => x.EmployeeCode == userInfos.EmployeeCode && x.IsActive == true);

            PmChinaQcInspectionApprovalLog model = new PmChinaQcInspectionApprovalLog();
            model.MainId = ids1;
            model.ProjectMasterId = prIds;
            model.MailListId = mailIds.Id;
            model.ClearanceStatus = sStatus;
            model.Remarks = remarks;
            model.Added = users;
            model.AddedDate = DateTime.Now;
            _dbEntities.PmChinaQcInspectionApprovalLogs.Add(model);
            _dbEntities.SaveChanges();

            return "OK";
        }

        public int GetChinaQcInspectionCount(long users)
        {

            #region com
            //            var query = _dbEntities.Database.SqlQuery<ChinaQcInspectionsClearanceModel>(@"
            //                select count(*) as ChinaQcInspectionCount from 
            //                (
            //	                select pcc.*,cu.UserFullName as Name from
            //	                [CellPhoneProject].[dbo].[PmChinaQcInspectionsClearance] pcc
            //	                inner join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=pcc.Added
            //	                where pcc.ClearanceStatus in ('PENDING') and
            //	                pcc.Id not in (select MainId from [CellPhoneProject].[dbo].PmChinaQcInspectionApprovalLog pcl 
            //	                where pcl.Added={0})
            //                )A
            //           ", users).ToList();

            //            return query;
            #endregion

            string query = string.Format(@"
               select count(*) as ChinaQcInspectionCount from 
               (
	                select pcc.*,cu.UserFullName as Name from
	                [CellPhoneProject].[dbo].[PmChinaQcInspectionsClearance] pcc
	                inner join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=pcc.Added
	                where pcc.ClearanceStatus in ('PENDING') and
	                pcc.Id not in (select MainId from [CellPhoneProject].[dbo].PmChinaQcInspectionApprovalLog pcl 
	                where pcl.Added={0})
               )A
            ", users);
            var getScreeningForwardCount = _dbEntities.Database.SqlQuery<int>(query).First();
            return getScreeningForwardCount;
        }

        public List<BomModel> GetBomInfoByItemCode(string itemCode)
        {
            List<BomModel> boms = _mrpEntities.BOMs.Where(i => i.SpareItemCode == itemCode).Select(j => new BomModel
            {
                SpareItemCode = j.SpareItemCode,
                SpareDescription = j.SpareDescription,
                RequiredPerUnit = j.RequiredPerUnit,
                ItemCost = j.ItemCost,
                ItemName = j.ItemName,
                AddedDate = j.AddedDate,
                AssemblyCode = j.AssemblyCode,
                BOMType = j.BOMType,
                AddedBy = j.AddedBy,
                BomProductModelId = j.BomProductModelId,
                Color = j.Color,
                Company = j.Company,
                Description = j.Description,
                Component = j.Component,
                Id = j.Id,
                InventoryItemCode = j.InventoryItemCode,
                InventoryItemId = j.InventoryItemId,
                ItemType = j.ItemType,
                ProductType = j.ProductType,
                Uom = j.Uom
            }).ToList();
            return boms;
        }

        public ResponseModel SaveMaterialWastage(WastageFileUpload wastageFileUpload)
        {
            try
            {
                long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
                if (userId == 0)
                {
                    return new ResponseModel { ResponseCode = 2, ResponseMessage = "Your session has ended. Please logout and login again" };
                }

                wastageFileUpload.MaterialWastageMaster.ProjectOrderQuantityDetailsId = wastageFileUpload.VariantId;
                wastageFileUpload.MaterialWastageMaster.AddedBy = userId;
                wastageFileUpload.MaterialWastageMaster.AddedDate = DateTime.Now;
                wastageFileUpload.MaterialWastageMaster.IsCompleted = false;
                wastageFileUpload.MaterialWastageMaster.IsInchargeApproved = false;
                wastageFileUpload.MaterialWastageMaster.IsCooApproved = false;
                wastageFileUpload.MaterialWastageMaster.IsManagementApproved = false;
                wastageFileUpload.MaterialWastageMaster.IsSpecialApproved = false;


                var materialWastageItems = new List<MaterialWastageItem>();

                foreach (var detail in wastageFileUpload.MaterialWastageDetails)
                {
                    detail.AddedBy = userId;
                    detail.AddedDate = DateTime.Now;
                }

                if (wastageFileUpload.MaterialWastageDetails.Any())
                {
                    foreach (var item in wastageFileUpload.MaterialWastageDetails)
                    {
                        bool ifAny = _dbEntities.MaterialWastageItems.Any(i => i.ItemCode == item.ItemCode);
                        if (ifAny)
                        {
                            var materialWastageItem = new MaterialWastageItem
                            {
                                AddedBy = item.AddedBy,
                                AddedDate = item.AddedDate,
                                AssemblyMaterialFault = item.AssemMaterialFault,
                                AssemblyProcessFault = item.AssemProcessFault,
                                BOMType = item.BOMType,
                                BomUnit = item.BOMUnit,
                                ItemCode = item.ItemCode,
                                ItemDetail = item.ItemName,
                                MonthName = wastageFileUpload.MaterialWastageMaster.MonthName,
                                MonthNumber = wastageFileUpload.MaterialWastageMaster.MonthNumber,
                                RepairMaterialFault = item.RepMaterialFault,
                                RepairProcessFault = item.RepProcessFault,
                                TotalWastageFault = item.TotalFault,
                                WastagePercentage = item.WastagePercentage,
                                YearNumber = wastageFileUpload.MaterialWastageMaster.YearNumber
                            };
                            materialWastageItems.Add(materialWastageItem);

                        }
                        else
                        {
                            if (item.TillNowTotalFault > 0)
                            {
                                int prevQty = item.TillNowTotalFault - item.TotalFault;
                                var materialWastageItem = new MaterialWastageItem
                                {
                                    AddedBy = item.AddedBy,
                                    AddedDate = item.AddedDate,
                                    AssemblyMaterialFault = item.AssemMaterialFault,
                                    AssemblyProcessFault = item.AssemProcessFault,
                                    BOMType = item.BOMType,
                                    BomUnit = item.BOMUnit,
                                    ItemCode = item.ItemCode,
                                    ItemDetail = item.ItemName,
                                    MonthName = wastageFileUpload.MaterialWastageMaster.MonthName,
                                    MonthNumber = wastageFileUpload.MaterialWastageMaster.MonthNumber,
                                    RepairMaterialFault = item.RepMaterialFault,
                                    RepairProcessFault = item.RepProcessFault,
                                    TotalWastageFault = item.TotalFault,
                                    WastagePercentage = item.WastagePercentage,
                                    YearNumber = wastageFileUpload.MaterialWastageMaster.YearNumber

                                };
                                materialWastageItems.Add(materialWastageItem);
                                //int month = DateTime.ParseExact(MonthNameStr, "MMMM", CultureInfo.CurrentCulture ).Month


                                materialWastageItem = new MaterialWastageItem
                                {
                                    AddedBy = item.AddedBy,
                                    AddedDate = item.AddedDate,
                                    AssemblyMaterialFault = item.TillNowAssemMaterialFault - item.AssemMaterialFault,
                                    AssemblyProcessFault = item.TillNowAssemProcessFault - item.AssemProcessFault,
                                    BOMType = item.BOMType,
                                    BomUnit = item.BOMUnit,
                                    ItemCode = item.ItemCode,
                                    ItemDetail = item.ItemName,
                                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(wastageFileUpload.MaterialWastageMaster.MonthNumber - 1),
                                    MonthNumber = wastageFileUpload.MaterialWastageMaster.MonthNumber - 1,
                                    RepairMaterialFault = item.TillNowRepMaterialFault - item.RepMaterialFault,
                                    RepairProcessFault = item.TillNowRepProcessFault - item.RepProcessFault,
                                    TotalWastageFault = item.TillNowTotalFault - item.TotalFault,
                                    WastagePercentage = item.WastagePercentage,
                                    YearNumber = wastageFileUpload.MaterialWastageMaster.YearNumber,
                                };
                                materialWastageItems.Add(materialWastageItem);
                            }
                        }
                    }
                }

                try
                {
                    //_dbEntities.MaterialWastageItems.AddRange(materialWastageItems);
                    //_dbEntities.SaveChanges();


                    wastageFileUpload.MaterialWastageMaster.MaterialWastageDetails =
                        wastageFileUpload.MaterialWastageDetails;
                    wastageFileUpload.MaterialWastageMaster.MaterialWastageItems = materialWastageItems;
                    _dbEntities.MaterialWastageMasters.Add(wastageFileUpload.MaterialWastageMaster);
                    //_dbEntities.MaterialWastageDetails.AddRange(wastageFileUpload.MaterialWastageDetails);
                    _dbEntities.SaveChanges();

                }
                catch (Exception e)
                {
                    return new ResponseModel { ResponseCode = 1, ResponseMessage = e.Message + "-" + e.InnerException.InnerException.Message };
                }

                return new ResponseModel { ResponseCode = 1, ResponseMessage = "Saved Successfully." };
            }
            catch (Exception exception)
            {
                return new ResponseModel { ResponseCode = 1, ResponseMessage = exception.Message };
            }
        }

        public List<MaterialWastageMasterModel> GetMetarialWastageList()
        {
            List<MaterialWastageMasterModel> models = (from materialWastageMaster in _dbEntities.MaterialWastageMasters
                                                       join inchargeUser in _dbEntities.CmnUsers on materialWastageMaster.InchargeId equals
                                                           inchargeUser.CmnUserId into inch
                                                       from inchargeUser in inch.DefaultIfEmpty()
                                                       join cooUser in _dbEntities.CmnUsers on materialWastageMaster.CooId equals cooUser.CmnUserId into coo
                                                       from cooUser in coo.DefaultIfEmpty()
                                                       join specialUser in _dbEntities.CmnUsers on materialWastageMaster.SpecialApproverId equals specialUser.CmnUserId into spec
                                                       from specialUser in spec.DefaultIfEmpty()
                                                       join managementUser in _dbEntities.CmnUsers on materialWastageMaster.ManagementId equals managementUser.CmnUserId into manage
                                                       from managermentUser in manage.DefaultIfEmpty()
                                                       join addedUser in _dbEntities.CmnUsers on materialWastageMaster.AddedBy equals addedUser.CmnUserId into addedU
                                                       from addedUser in addedU.DefaultIfEmpty()
                                                       //join projectOrderQuantityDetail in _dbEntities.ProjectOrderQuantityDetails on materialWastageMaster.ProjectOrderQuantityDetailsId equals projectOrderQuantityDetail.Id
                                                       where materialWastageMaster.IsCompleted == false || materialWastageMaster.IsCompleted == null

                                                       select new MaterialWastageMasterModel
                                                       {
                                                           AddedBy = materialWastageMaster.AddedBy,
                                                           AddedByName = addedUser.UserFullName,
                                                           AddedDate = materialWastageMaster.AddedDate,
                                                           CooApprovername = cooUser.UserFullName,
                                                           Id = materialWastageMaster.Id,
                                                           InchargeApproverName = inchargeUser.UserFullName,
                                                           IsCompleted = materialWastageMaster.IsCompleted,
                                                           IsCooApproved = materialWastageMaster.IsCooApproved,
                                                           IsInchargeApproved = materialWastageMaster.IsInchargeApproved,
                                                           IsManagementApproved = materialWastageMaster.IsManagementApproved,
                                                           IsSpecialApproved = materialWastageMaster.IsSpecialApproved,
                                                           ManagementApproverName = managermentUser.UserFullName,
                                                           MonthName = materialWastageMaster.MonthName,
                                                           MonthNumber = materialWastageMaster.MonthNumber,
                                                           ProjectOrderQuantityDetailsId = materialWastageMaster.ProjectOrderQuantityDetailsId,
                                                           //ProjectVarientName = projectOrderQuantityDetail.ProjectModel,
                                                           ReportName = materialWastageMaster.ReportName,
                                                           SpecialApproverName = specialUser.UserFullName,
                                                           YearNumber = materialWastageMaster.YearNumber
                                                       }
                ).ToList();
            
            foreach (var model in models)
            {
                model.IsDeclinedFromAnyOne =
                    _dbEntities.MaterialWastageRecommendations.Any(
                        i => i.MaterialWastageMasterId == model.Id && i.RecommendationType == "DECLINED");
            }

            return models;
        }

        public List<MaterialWastageMasterModel> GetPendingApprovals(int approvalStage)
        {
            if (approvalStage == 1)
            {
                List<MaterialWastageMasterModel> models = (from materialWastageMaster in _dbEntities.MaterialWastageMasters
                                                           join inchargeUser in _dbEntities.CmnUsers on materialWastageMaster.InchargeId equals
                                                               inchargeUser.CmnUserId into inch
                                                           from inchargeUser in inch.DefaultIfEmpty()
                                                           join cooUser in _dbEntities.CmnUsers on materialWastageMaster.CooId equals cooUser.CmnUserId into coo
                                                           from cooUser in coo.DefaultIfEmpty()
                                                           join specialUser in _dbEntities.CmnUsers on materialWastageMaster.SpecialApproverId equals specialUser.CmnUserId into spec
                                                           from specialUser in spec.DefaultIfEmpty()
                                                           join managementUser in _dbEntities.CmnUsers on materialWastageMaster.ManagementId equals managementUser.CmnUserId into manage
                                                           from managermentUser in manage.DefaultIfEmpty()
                                                           join addedUser in _dbEntities.CmnUsers on materialWastageMaster.AddedBy equals addedUser.CmnUserId into addedU
                                                           from addedUser in addedU.DefaultIfEmpty()
                                                           //join projectOrderQuantityDetail in _dbEntities.ProjectOrderQuantityDetails on materialWastageMaster.ProjectOrderQuantityDetailsId equals projectOrderQuantityDetail.Id
                                                           where materialWastageMaster.IsInchargeApproved == false || materialWastageMaster.IsInchargeApproved == null

                                                           select new MaterialWastageMasterModel
                                                           {
                                                               AddedBy = materialWastageMaster.AddedBy,
                                                               AddedByName = addedUser.UserFullName,
                                                               AddedDate = materialWastageMaster.AddedDate,
                                                               CooApprovername = cooUser.UserFullName,
                                                               Id = materialWastageMaster.Id,
                                                               InchargeApproverName = inchargeUser.UserFullName,
                                                               IsCompleted = materialWastageMaster.IsCompleted,
                                                               IsCooApproved = materialWastageMaster.IsCooApproved,
                                                               IsInchargeApproved = materialWastageMaster.IsInchargeApproved,
                                                               IsManagementApproved = materialWastageMaster.IsManagementApproved,
                                                               IsSpecialApproved = materialWastageMaster.IsSpecialApproved,
                                                               ManagementApproverName = managermentUser.UserFullName,
                                                               MonthName = materialWastageMaster.MonthName,
                                                               MonthNumber = materialWastageMaster.MonthNumber,
                                                               ProjectOrderQuantityDetailsId = materialWastageMaster.ProjectOrderQuantityDetailsId,
                                                               //ProjectVarientName = projectOrderQuantityDetail.ProjectModel,
                                                               ReportName = materialWastageMaster.ReportName,
                                                               SpecialApproverName = specialUser.UserFullName,
                                                               YearNumber = materialWastageMaster.YearNumber
                                                           }
                    ).ToList();

                return models;
            }
            if (approvalStage == 2)
            {
                List<MaterialWastageMasterModel> models = (from materialWastageMaster in _dbEntities.MaterialWastageMasters
                                                           join inchargeUser in _dbEntities.CmnUsers on materialWastageMaster.InchargeId equals
                                                               inchargeUser.CmnUserId into inch
                                                           from inchargeUser in inch.DefaultIfEmpty()
                                                           join cooUser in _dbEntities.CmnUsers on materialWastageMaster.CooId equals cooUser.CmnUserId into coo
                                                           from cooUser in coo.DefaultIfEmpty()
                                                           join specialUser in _dbEntities.CmnUsers on materialWastageMaster.SpecialApproverId equals specialUser.CmnUserId into spec
                                                           from specialUser in spec.DefaultIfEmpty()
                                                           join managementUser in _dbEntities.CmnUsers on materialWastageMaster.ManagementId equals managementUser.CmnUserId into manage
                                                           from managermentUser in manage.DefaultIfEmpty()
                                                           join addedUser in _dbEntities.CmnUsers on materialWastageMaster.AddedBy equals addedUser.CmnUserId into addedU
                                                           from addedUser in addedU.DefaultIfEmpty()
                                                           //join projectOrderQuantityDetail in _dbEntities.ProjectOrderQuantityDetails on materialWastageMaster.ProjectOrderQuantityDetailsId equals projectOrderQuantityDetail.Id
                                                           where materialWastageMaster.IsInchargeApproved == true && (materialWastageMaster.IsDeputyCooApproved == false || materialWastageMaster.IsDeputyCooApproved == null)

                                                           select new MaterialWastageMasterModel
                                                           {
                                                               AddedBy = materialWastageMaster.AddedBy,
                                                               AddedByName = addedUser.UserFullName,
                                                               AddedDate = materialWastageMaster.AddedDate,
                                                               CooApprovername = cooUser.UserFullName,
                                                               Id = materialWastageMaster.Id,
                                                               InchargeApproverName = inchargeUser.UserFullName,
                                                               IsCompleted = materialWastageMaster.IsCompleted,
                                                               IsCooApproved = materialWastageMaster.IsCooApproved,
                                                               IsInchargeApproved = materialWastageMaster.IsInchargeApproved,
                                                               IsManagementApproved = materialWastageMaster.IsManagementApproved,
                                                               IsSpecialApproved = materialWastageMaster.IsSpecialApproved,
                                                               ManagementApproverName = managermentUser.UserFullName,
                                                               MonthName = materialWastageMaster.MonthName,
                                                               MonthNumber = materialWastageMaster.MonthNumber,
                                                               ProjectOrderQuantityDetailsId = materialWastageMaster.ProjectOrderQuantityDetailsId,
                                                               //ProjectVarientName = projectOrderQuantityDetail.ProjectModel,
                                                               ReportName = materialWastageMaster.ReportName,
                                                               SpecialApproverName = specialUser.UserFullName,
                                                               YearNumber = materialWastageMaster.YearNumber
                                                           }
                    ).ToList();

                return models;
            }
            if (approvalStage == 4)
            {
                List<MaterialWastageMasterModel> models = (from materialWastageMaster in _dbEntities.MaterialWastageMasters
                                                           join inchargeUser in _dbEntities.CmnUsers on materialWastageMaster.InchargeId equals
                                                               inchargeUser.CmnUserId into inch
                                                           from inchargeUser in inch.DefaultIfEmpty()
                                                           join cooUser in _dbEntities.CmnUsers on materialWastageMaster.CooId equals cooUser.CmnUserId into coo
                                                           from cooUser in coo.DefaultIfEmpty()
                                                           join specialUser in _dbEntities.CmnUsers on materialWastageMaster.SpecialApproverId equals specialUser.CmnUserId into spec
                                                           from specialUser in spec.DefaultIfEmpty()
                                                           join managementUser in _dbEntities.CmnUsers on materialWastageMaster.ManagementId equals managementUser.CmnUserId into manage
                                                           from managermentUser in manage.DefaultIfEmpty()
                                                           join addedUser in _dbEntities.CmnUsers on materialWastageMaster.AddedBy equals addedUser.CmnUserId into addedU
                                                           from addedUser in addedU.DefaultIfEmpty()
                                                           //join projectOrderQuantityDetail in _dbEntities.ProjectOrderQuantityDetails on materialWastageMaster.ProjectOrderQuantityDetailsId equals projectOrderQuantityDetail.Id
                                                           where materialWastageMaster.IsDeputyCooApproved == true && (materialWastageMaster.IsCooApproved == false || materialWastageMaster.IsCooApproved == null)

                                                           select new MaterialWastageMasterModel
                                                           {
                                                               AddedBy = materialWastageMaster.AddedBy,
                                                               AddedByName = addedUser.UserFullName,
                                                               AddedDate = materialWastageMaster.AddedDate,
                                                               CooApprovername = cooUser.UserFullName,
                                                               Id = materialWastageMaster.Id,
                                                               InchargeApproverName = inchargeUser.UserFullName,
                                                               IsCompleted = materialWastageMaster.IsCompleted,
                                                               IsCooApproved = materialWastageMaster.IsCooApproved,
                                                               IsInchargeApproved = materialWastageMaster.IsInchargeApproved,
                                                               IsManagementApproved = materialWastageMaster.IsManagementApproved,
                                                               IsSpecialApproved = materialWastageMaster.IsSpecialApproved,
                                                               ManagementApproverName = managermentUser.UserFullName,
                                                               MonthName = materialWastageMaster.MonthName,
                                                               MonthNumber = materialWastageMaster.MonthNumber,
                                                               ProjectOrderQuantityDetailsId = materialWastageMaster.ProjectOrderQuantityDetailsId,
                                                               //ProjectVarientName = projectOrderQuantityDetail.ProjectModel,
                                                               ReportName = materialWastageMaster.ReportName,
                                                               SpecialApproverName = specialUser.UserFullName,
                                                               YearNumber = materialWastageMaster.YearNumber
                                                           }
                    ).ToList();

                return models;
            }
            if (approvalStage == 3)
            {
                List<MaterialWastageMasterModel> models = (from materialWastageMaster in _dbEntities.MaterialWastageMasters
                                                           join inchargeUser in _dbEntities.CmnUsers on materialWastageMaster.InchargeId equals
                                                               inchargeUser.CmnUserId into inch
                                                           from inchargeUser in inch.DefaultIfEmpty()
                                                           join cooUser in _dbEntities.CmnUsers on materialWastageMaster.CooId equals cooUser.CmnUserId into coo
                                                           from cooUser in coo.DefaultIfEmpty()
                                                           join specialUser in _dbEntities.CmnUsers on materialWastageMaster.SpecialApproverId equals specialUser.CmnUserId into spec
                                                           from specialUser in spec.DefaultIfEmpty()
                                                           join managementUser in _dbEntities.CmnUsers on materialWastageMaster.ManagementId equals managementUser.CmnUserId into manage
                                                           from managermentUser in manage.DefaultIfEmpty()
                                                           join addedUser in _dbEntities.CmnUsers on materialWastageMaster.AddedBy equals addedUser.CmnUserId into addedU
                                                           from addedUser in addedU.DefaultIfEmpty()
                                                           //join projectOrderQuantityDetail in _dbEntities.ProjectOrderQuantityDetails on materialWastageMaster.ProjectOrderQuantityDetailsId equals projectOrderQuantityDetail.Id
                                                           where materialWastageMaster.IsInchargeApproved == true && materialWastageMaster.IsCooApproved == true && (materialWastageMaster.IsManagementApproved == false || materialWastageMaster.IsManagementApproved == null)

                                                           select new MaterialWastageMasterModel
                                                           {
                                                               AddedBy = materialWastageMaster.AddedBy,
                                                               AddedByName = addedUser.UserFullName,
                                                               AddedDate = materialWastageMaster.AddedDate,
                                                               CooApprovername = cooUser.UserFullName,
                                                               Id = materialWastageMaster.Id,
                                                               InchargeApproverName = inchargeUser.UserFullName,
                                                               IsCompleted = materialWastageMaster.IsCompleted,
                                                               IsCooApproved = materialWastageMaster.IsCooApproved,
                                                               IsInchargeApproved = materialWastageMaster.IsInchargeApproved,
                                                               IsManagementApproved = materialWastageMaster.IsManagementApproved,
                                                               IsSpecialApproved = materialWastageMaster.IsSpecialApproved,
                                                               ManagementApproverName = managermentUser.UserFullName,
                                                               MonthName = materialWastageMaster.MonthName,
                                                               MonthNumber = materialWastageMaster.MonthNumber,
                                                               ProjectOrderQuantityDetailsId = materialWastageMaster.ProjectOrderQuantityDetailsId,
                                                               //ProjectVarientName = projectOrderQuantityDetail.ProjectModel,
                                                               ReportName = materialWastageMaster.ReportName,
                                                               SpecialApproverName = specialUser.UserFullName,
                                                               YearNumber = materialWastageMaster.YearNumber
                                                           }
                    ).ToList();

                return models;
            }
            return new List<MaterialWastageMasterModel>();
        }

        public WastageFileUpload GetMaterialWastageById(long id)
        {
            var model = new WastageFileUpload();
            _dbEntities.Configuration.LazyLoadingEnabled = true;
            MaterialWastageMaster master = _dbEntities.MaterialWastageMasters.FirstOrDefault(i => i.Id == id);
            if (master == null) return model;
            var projectOrderQuantityDetail = _dbEntities.ProjectOrderQuantityDetails.FirstOrDefault(i => i.Id == master.ProjectOrderQuantityDetailsId);
            if (projectOrderQuantityDetail != null) model.ProjectName = projectOrderQuantityDetail.ProjectModel;
            model.MaterialWastageMaster = master;
            model.MaterialWastageDetails = master.MaterialWastageDetails.ToList();
            model.Average1 = Math.Round(master.MaterialWastageDetails.Sum(i => i.ActualAssemblyWastage_TotalLot) / master.MaterialWastageDetails.Count(i => i.ActualAssemblyWastage_TotalLot >= 0),2);
            model.Average2 = Math.Round(master.MaterialWastageDetails.Sum(i => i.ActualRepairWastage_TotalLot) / master.MaterialWastageDetails.Count(i => i.ActualRepairWastage_TotalLot >= 0), 2);
            model.Average3 = Math.Round(master.MaterialWastageDetails.Sum(i => i.ActualWastageOfTotalLot) / master.MaterialWastageDetails.Count(i => i.ActualWastageOfTotalLot >= 0), 2);
            model.Average4 = Math.Round(master.MaterialWastageDetails.Sum(i => i.NetAdjustment) / master.MaterialWastageDetails.Count(i => i.NetAdjustment >= 0), 2);
            return model;
        }

        public ResponseModel RecommendMaterialWastage(long id, bool isRecom, string recomMsg, bool isApproved, string approvedMsg, IPrincipal user, int recommenderType)
        {
            try
            {
                MaterialWastageRecommendation recommendation = null;
                long userId = Convert.ToInt64(user.Identity.Name);
                CmnUser userInfo = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);
                MaterialWastageMaster master = _dbEntities.MaterialWastageMasters.FirstOrDefault(i => i.Id == id);
                if (master != null)
                {
                    recommendation = new MaterialWastageRecommendation
                    {
                        MaterialWastageMasterId = master.Id,
                        AddedBy = userId,
                        AddedDate = DateTime.Now,
                        RecommendedBy = userInfo.UserFullName
                    };
                    if (isRecom)
                    {
                        switch (recommenderType)
                        {
                            case 1:
                                master.IsInchargeApproved = false;
                                master.InchargeId = userId;
                                recommendation.RecommendationType = "DECLINED";
                                recommendation.Remarks = recomMsg;
                                recommendation.UserType = "INCHARGE";
                                break;
                            case 2:
                                master.CooId = userId;
                                master.IsCooApproved = false;
                                recommendation.RecommendationType = "DECLINED";
                                recommendation.Remarks = recomMsg;
                                recommendation.UserType = "COO";
                                break;
                            case 3:
                                master.ManagementId = userId;
                                master.IsManagementApproved = false;
                                recommendation.RecommendationType = "DECLINED";
                                recommendation.Remarks = recomMsg;
                                recommendation.UserType = "MANAGEMENT";
                                break;
                            case 4:
                                master.DeputyCooId = userId;
                                master.IsDeputyCooApproved = false;
                                recommendation.RecommendationType = "DECLINED";
                                recommendation.Remarks = recomMsg;
                                recommendation.UserType = "DCOO";
                                break;
                        }

                    }
                    else if (isApproved)
                    {
                        switch (recommenderType)
                        {
                            case 1:
                                master.IsInchargeApproved = true;
                                master.InchargeId = userId;
                                recommendation.RecommendationType = "APPROVED";
                                recommendation.Remarks = approvedMsg;
                                recommendation.UserType = "INCHARGE";
                                break;
                            case 2:
                                master.CooId = userId;
                                master.IsCooApproved = true;
                                recommendation.RecommendationType = "APPROVED";
                                recommendation.Remarks = approvedMsg;
                                recommendation.UserType = "COO";
                                break;
                            case 3:
                                master.ManagementId = userId;
                                master.IsManagementApproved = true;
                                recommendation.RecommendationType = "APPROVED";
                                recommendation.Remarks = approvedMsg;
                                recommendation.UserType = "MANAGEMENT";
                                break;
                            case 4:
                                master.DeputyCooId = userId;
                                master.IsDeputyCooApproved = true;
                                recommendation.RecommendationType = "APPROVED";
                                recommendation.Remarks = approvedMsg;
                                recommendation.UserType = "DCOO";
                                break;
                        }

                    }
                    else
                    {
                        return new ResponseModel { ResponseCode = 2, ResponseMessage = "One check box must be check. Error!!!" };
                    }


                }
                if (recommendation != null)
                {
                    _dbEntities.MaterialWastageRecommendations.Add(recommendation);
                    _dbEntities.Entry(master).State = EntityState.Modified;
                    _dbEntities.SaveChanges();
                }

                return new ResponseModel { ResponseCode = 1, ResponseMessage = "Successfully Saved." };
            }
            catch (Exception exception)
            {
                return new ResponseModel { ResponseCode = 0, ResponseMessage = exception.Message };
            }
        }

        public List<MaterialWastageRecommendation> GetRecommendationsByMasterId(long id)
        {
            return _dbEntities.MaterialWastageRecommendations.Where(i => i.MaterialWastageMasterId == id).ToList();
        }

        #endregion
        #region Wpms All Projects Details
        public List<WpmsAllProjectDetailsModel> GetAllModels()
        {
            var query = new List<WpmsAllProjectDetailsModel>();

            query = _dbEntities.Database.SqlQuery<WpmsAllProjectDetailsModel>(@"
            select distinct ProjectName from [CellPhoneProject].[dbo].[ProjectMasters] pm order by pm.ProjectName asc").ToList();

            return query;
        }

        public List<WpmsAllProjectDetailsModel> GetProjectOrders(string projectName)
        {
            var query = _dbEntities.Database.SqlQuery<WpmsAllProjectDetailsModel>(@"
            select DISTINCT 'Order '+cast(OrderNuber as varchar(50)) as Orders,ProjectName from CellPhoneProject.dbo.ProjectMasters where ProjectName={0}
            order by ProjectName asc", projectName).ToList();

            return query;
        }

        public List<WpmsAllProjectDetailsModel> GetProjectSpec(string projectName, string orders, string ProStatus, string InitialApproval)
        {
            var query = new List<WpmsAllProjectDetailsModel>();

            if (projectName == "ALL" && orders == "ALL" && ProStatus == "ALL" && InitialApproval == "ALL")
            {

                query = _dbEntities.Database.SqlQuery<WpmsAllProjectDetailsModel>(@"
                 select * from 
                    (

                        select pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,'Order '+ cast(OrderNuber as varchar(50)) as Orders,case when pm.IsActive=1 then 'YES' else 'NO' end as IsActiveStatus,
	                    pm.AddedDate as ProjectCreationDate,pm.SupplierName,pm.ProjectStatus,cu.UserFullName as ProjectManagerName,pdd.MarketClearanceDate,case when pdd.IsActive=1 then 'YES' else 'NO' end as IsActiveOrder,
	                    case when pdd.OrderQuantity is null then 0 else pdd.OrderQuantity end as OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                    pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                    pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                    pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                    pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamBsi,pm.FrontCamSensor,
	                    case when cast(ppo.PoDate as varchar(50)) is null then 'PO Pending' else cast(ppo.PoDate as varchar(50)) end as PoDate, case when pm.ProjectStatus='NEW' and pm.BiApprovalBy is null and pm.InitialApprovalBy is null and pm.PsApprovalBy is null then 'Action Not taken By BI' when pm.ProjectStatus='NEW' and pm.PsApprovalBy is null then 'Action Not taken By Management Coordinator'
	                    when pm.ProjectStatus='NEW' and pm.InitialApprovalBy is null then 'Action Not taken By MD' 
	                    when pm.ProjectStatus='SWOTPENDING' and pm.BiApprovalBy is null and pm.PsApprovalBy is null and pm.InitialApprovalBy is null then 'SWOT analysis Pending'
	                    else '----' end as InitialApprovalPendings

	                    FROM [CellPhoneProject].[dbo].[ProjectMasters] pm
	                    left join CellPhoneProject.dbo.ProjectPmAssigns ppa on ppa.ProjectMasterId=pm.ProjectMasterId and ppa.Status='ASSIGNED'
	                    left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=ppa.ProjectManagerUserId
	                    left join  CellPhoneProject.dbo.ProjectOrderQuantityDetails pdd on pdd.ProjectMasterId=pm.ProjectMasterId and pdd.IsActive=1
	                    left join [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms] ppo on ppo.ProjectMasterId=pm.ProjectMasterId
                    
                        group by pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,pm.OrderNuber,pm.AddedDate,pm.SupplierName,pm.IsActive,
	                    pm.ProjectStatus,cu.UserFullName,pdd.MarketClearanceDate,pdd.IsActive,pdd.OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                    pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.FrontCamBsi,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                    pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                    pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                    pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamSensor,ppo.PoDate,
	                    pm.InitialApprovalBy,pm.PsApprovalBy,pm.BiApprovalBy

                    )A order by A.ProjectMasterId,A.Orders desc
                ", projectName, orders).ToList();
            }
            else if (projectName == "ALL" && orders != "ALL" && ProStatus == "ALL" && InitialApproval == "ALL")
            {
                query = _dbEntities.Database.SqlQuery<WpmsAllProjectDetailsModel>(@"          
                    select * from 
                    (

                        select pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,'Order '+ cast(OrderNuber as varchar(50)) as Orders,case when pm.IsActive=1 then 'YES' else 'NO' end as IsActiveStatus,
	                    pm.AddedDate as ProjectCreationDate,pm.SupplierName,pm.ProjectStatus,cu.UserFullName as ProjectManagerName,pdd.MarketClearanceDate,case when pdd.IsActive=1 then 'YES' else 'NO' end as IsActiveOrder,
	                    case when pdd.OrderQuantity is null then 0 else pdd.OrderQuantity end as OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                    pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                    pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                    pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                    pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamBsi,pm.FrontCamSensor,
	                    case when cast(ppo.PoDate as varchar(50)) is null then 'PO Pending' else cast(ppo.PoDate as varchar(50)) end as PoDate, case when pm.ProjectStatus='NEW' and pm.BiApprovalBy is null and pm.InitialApprovalBy is null and pm.PsApprovalBy is null then 'Action Not taken By BI' when pm.ProjectStatus='NEW' and pm.PsApprovalBy is null then 'Action Not taken By Management Coordinator'
	                    when pm.ProjectStatus='NEW' and pm.InitialApprovalBy is null then 'Action Not taken By MD' 
	                    when pm.ProjectStatus='SWOTPENDING' and pm.BiApprovalBy is null and pm.PsApprovalBy is null and pm.InitialApprovalBy is null then 'SWOT analysis Pending'
	                    else '----' end as InitialApprovalPendings

	                    FROM [CellPhoneProject].[dbo].[ProjectMasters] pm
	                    left join CellPhoneProject.dbo.ProjectPmAssigns ppa on ppa.ProjectMasterId=pm.ProjectMasterId and ppa.Status='ASSIGNED'
	                    left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=ppa.ProjectManagerUserId
	                    left join  CellPhoneProject.dbo.ProjectOrderQuantityDetails pdd on pdd.ProjectMasterId=pm.ProjectMasterId and pdd.IsActive=1
	                    left join [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms] ppo on ppo.ProjectMasterId=pm.ProjectMasterId
                    
                        group by pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,pm.OrderNuber,pm.AddedDate,pm.SupplierName,pm.IsActive,
	                    pm.ProjectStatus,cu.UserFullName,pdd.MarketClearanceDate,pdd.IsActive,pdd.OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                    pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.FrontCamBsi,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                    pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                    pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                    pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamSensor,ppo.PoDate,
	                    pm.InitialApprovalBy,pm.PsApprovalBy,pm.BiApprovalBy

                    )A where A.Orders={1} order by A.ProjectMasterId,A.Orders desc
                ", projectName, orders).ToList();
            }
            else if (projectName == "ALL" && orders == "ALL" && ProStatus != "ALL" && InitialApproval == "ALL")
            {
                query = _dbEntities.Database.SqlQuery<WpmsAllProjectDetailsModel>(@"          
                    select * from 
                    (

                        select pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,'Order '+ cast(OrderNuber as varchar(50)) as Orders,case when pm.IsActive=1 then 'YES' else 'NO' end as IsActiveStatus,
	                    pm.AddedDate as ProjectCreationDate,pm.SupplierName,pm.ProjectStatus,cu.UserFullName as ProjectManagerName,pdd.MarketClearanceDate,case when pdd.IsActive=1 then 'YES' else 'NO' end as IsActiveOrder,
	                    case when pdd.OrderQuantity is null then 0 else pdd.OrderQuantity end as OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                    pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                    pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                    pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                    pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamBsi,pm.FrontCamSensor,
	                    case when cast(ppo.PoDate as varchar(50)) is null then 'PO Pending' else cast(ppo.PoDate as varchar(50)) end as PoDate, case when pm.ProjectStatus='NEW' and pm.BiApprovalBy is null and pm.InitialApprovalBy is null and pm.PsApprovalBy is null then 'Action Not taken By BI' when pm.ProjectStatus='NEW' and pm.PsApprovalBy is null then 'Action Not taken By Management Coordinator'
	                    when pm.ProjectStatus='NEW' and pm.InitialApprovalBy is null then 'Action Not taken By MD' 
	                    when pm.ProjectStatus='SWOTPENDING' and pm.BiApprovalBy is null and pm.PsApprovalBy is null and pm.InitialApprovalBy is null then 'SWOT analysis Pending'
	                    else '----' end as InitialApprovalPendings

	                    FROM [CellPhoneProject].[dbo].[ProjectMasters] pm
	                    left join CellPhoneProject.dbo.ProjectPmAssigns ppa on ppa.ProjectMasterId=pm.ProjectMasterId and ppa.Status='ASSIGNED'
	                    left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=ppa.ProjectManagerUserId
	                    left join  CellPhoneProject.dbo.ProjectOrderQuantityDetails pdd on pdd.ProjectMasterId=pm.ProjectMasterId and pdd.IsActive=1
	                    left join [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms] ppo on ppo.ProjectMasterId=pm.ProjectMasterId
                    
                        group by pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,pm.OrderNuber,pm.AddedDate,pm.SupplierName,pm.IsActive,
	                    pm.ProjectStatus,cu.UserFullName,pdd.MarketClearanceDate,pdd.IsActive,pdd.OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                    pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.FrontCamBsi,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                    pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                    pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                    pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamSensor,ppo.PoDate,
	                    pm.InitialApprovalBy,pm.PsApprovalBy,pm.BiApprovalBy

                    )A where A.ProjectStatus={2} order by A.ProjectMasterId,A.Orders desc
                ", projectName, orders, ProStatus, InitialApproval).ToList();
            }
            else if (projectName == "ALL" && orders == "ALL" && ProStatus == "ALL" && InitialApproval != "ALL")
            {
                query = _dbEntities.Database.SqlQuery<WpmsAllProjectDetailsModel>(@"          
                    select * from 
                    (

                        select pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,'Order '+ cast(OrderNuber as varchar(50)) as Orders,case when pm.IsActive=1 then 'YES' else 'NO' end as IsActiveStatus,
	                    pm.AddedDate as ProjectCreationDate,pm.SupplierName,pm.ProjectStatus,cu.UserFullName as ProjectManagerName,pdd.MarketClearanceDate,case when pdd.IsActive=1 then 'YES' else 'NO' end as IsActiveOrder,
	                    case when pdd.OrderQuantity is null then 0 else pdd.OrderQuantity end as OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                    pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                    pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                    pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                    pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamBsi,pm.FrontCamSensor,
	                    case when cast(ppo.PoDate as varchar(50)) is null then 'PO Pending' else cast(ppo.PoDate as varchar(50)) end as PoDate, case when pm.ProjectStatus='NEW' and pm.BiApprovalBy is null and pm.InitialApprovalBy is null and pm.PsApprovalBy is null then 'Action Not taken By BI' when pm.ProjectStatus='NEW' and pm.PsApprovalBy is null then 'Action Not taken By Management Coordinator'
	                    when pm.ProjectStatus='NEW' and pm.InitialApprovalBy is null then 'Action Not taken By MD' 
	                    when pm.ProjectStatus='SWOTPENDING' and pm.BiApprovalBy is null and pm.PsApprovalBy is null and pm.InitialApprovalBy is null then 'SWOT analysis Pending'
	                    else '----' end as InitialApprovalPendings

	                    FROM [CellPhoneProject].[dbo].[ProjectMasters] pm
	                    left join CellPhoneProject.dbo.ProjectPmAssigns ppa on ppa.ProjectMasterId=pm.ProjectMasterId and ppa.Status='ASSIGNED'
	                    left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=ppa.ProjectManagerUserId
	                    left join  CellPhoneProject.dbo.ProjectOrderQuantityDetails pdd on pdd.ProjectMasterId=pm.ProjectMasterId and pdd.IsActive=1
	                    left join [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms] ppo on ppo.ProjectMasterId=pm.ProjectMasterId
                    
                        group by pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,pm.OrderNuber,pm.AddedDate,pm.SupplierName,pm.IsActive,
	                    pm.ProjectStatus,cu.UserFullName,pdd.MarketClearanceDate,pdd.IsActive,pdd.OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                    pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.FrontCamBsi,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                    pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                    pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                    pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamSensor,ppo.PoDate,
	                    pm.InitialApprovalBy,pm.PsApprovalBy,pm.BiApprovalBy

                    )A where A.InitialApprovalPendings={3} order by A.ProjectMasterId,A.Orders desc
                ", projectName, orders, ProStatus, InitialApproval).ToList();
            }
            else if (projectName != "ALL" && orders == "ALL" && ProStatus == "ALL" && InitialApproval == "ALL")
            {
                query = _dbEntities.Database.SqlQuery<WpmsAllProjectDetailsModel>(@"          
                    select * from 
                    (

                        select pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,'Order '+ cast(OrderNuber as varchar(50)) as Orders,case when pm.IsActive=1 then 'YES' else 'NO' end as IsActiveStatus,
	                    pm.AddedDate as ProjectCreationDate,pm.SupplierName,pm.ProjectStatus,cu.UserFullName as ProjectManagerName,pdd.MarketClearanceDate,case when pdd.IsActive=1 then 'YES' else 'NO' end as IsActiveOrder,
	                    case when pdd.OrderQuantity is null then 0 else pdd.OrderQuantity end as OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                    pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                    pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                    pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                    pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamBsi,pm.FrontCamSensor,
	                    case when cast(ppo.PoDate as varchar(50)) is null then 'PO Pending' else cast(ppo.PoDate as varchar(50)) end as PoDate, case when pm.ProjectStatus='NEW' and pm.BiApprovalBy is null and pm.InitialApprovalBy is null and pm.PsApprovalBy is null then 'Action Not taken By BI' when pm.ProjectStatus='NEW' and pm.PsApprovalBy is null then 'Action Not taken By Management Coordinator'
	                    when pm.ProjectStatus='NEW' and pm.InitialApprovalBy is null then 'Action Not taken By MD' 
	                    when pm.ProjectStatus='SWOTPENDING' and pm.BiApprovalBy is null and pm.PsApprovalBy is null and pm.InitialApprovalBy is null then 'SWOT analysis Pending'
	                    else '----' end as InitialApprovalPendings

	                    FROM [CellPhoneProject].[dbo].[ProjectMasters] pm
	                    left join CellPhoneProject.dbo.ProjectPmAssigns ppa on ppa.ProjectMasterId=pm.ProjectMasterId and ppa.Status='ASSIGNED'
	                    left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=ppa.ProjectManagerUserId
	                    left join  CellPhoneProject.dbo.ProjectOrderQuantityDetails pdd on pdd.ProjectMasterId=pm.ProjectMasterId and pdd.IsActive=1
	                    left join [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms] ppo on ppo.ProjectMasterId=pm.ProjectMasterId
                    
                        group by pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,pm.OrderNuber,pm.AddedDate,pm.SupplierName,pm.IsActive,
	                    pm.ProjectStatus,cu.UserFullName,pdd.MarketClearanceDate,pdd.IsActive,pdd.OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                    pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.FrontCamBsi,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                    pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                    pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                    pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamSensor,ppo.PoDate,
	                    pm.InitialApprovalBy,pm.PsApprovalBy,pm.BiApprovalBy

                    )A where A.ProjectName={0} order by A.ProjectMasterId,A.Orders desc
                ", projectName, orders, ProStatus, InitialApproval).ToList();
            }
            else if (projectName != "ALL" && orders != "ALL" && ProStatus == "ALL" && InitialApproval == "ALL")
            {
                query = _dbEntities.Database.SqlQuery<WpmsAllProjectDetailsModel>(@"
                select * from 
                (

                    select pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,'Order '+ cast(OrderNuber as varchar(50)) as Orders,case when pm.IsActive=1 then 'YES' else 'NO' end as IsActiveStatus,
	                pm.AddedDate as ProjectCreationDate,pm.SupplierName,pm.ProjectStatus,cu.UserFullName as ProjectManagerName,pdd.MarketClearanceDate,case when pdd.IsActive=1 then 'YES' else 'NO' end as IsActiveOrder,
	                case when pdd.OrderQuantity is null then 0 else pdd.OrderQuantity end as OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamBsi,pm.FrontCamSensor,
	                case when cast(ppo.PoDate as varchar(50)) is null then 'PO Pending' else cast(ppo.PoDate as varchar(50)) end as PoDate, case when pm.ProjectStatus='NEW' and pm.BiApprovalBy is null and pm.InitialApprovalBy is null and pm.PsApprovalBy is null then 'Action Not taken By BI' when pm.ProjectStatus='NEW' and pm.PsApprovalBy is null then 'Action Not taken By Management Coordinator'
	                when pm.ProjectStatus='NEW' and pm.InitialApprovalBy is null then 'Action Not taken By MD' 
	                when pm.ProjectStatus='SWOTPENDING' and pm.BiApprovalBy is null and pm.PsApprovalBy is null and pm.InitialApprovalBy is null then 'SWOT analysis Pending'
	                else '----' end as InitialApprovalPendings

	                FROM [CellPhoneProject].[dbo].[ProjectMasters] pm
	                left join CellPhoneProject.dbo.ProjectPmAssigns ppa on ppa.ProjectMasterId=pm.ProjectMasterId and ppa.Status='ASSIGNED'
	                left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=ppa.ProjectManagerUserId
	                left join  CellPhoneProject.dbo.ProjectOrderQuantityDetails pdd on pdd.ProjectMasterId=pm.ProjectMasterId and pdd.IsActive=1
	                left join [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms] ppo on ppo.ProjectMasterId=pm.ProjectMasterId
                    
                    group by pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,pm.OrderNuber,pm.AddedDate,pm.SupplierName,pm.IsActive,
	                pm.ProjectStatus,cu.UserFullName,pdd.MarketClearanceDate,pdd.IsActive,pdd.OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.FrontCamBsi,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamSensor,ppo.PoDate,
	                pm.InitialApprovalBy,pm.PsApprovalBy,pm.BiApprovalBy

                )A where ProjectName={0} and 	Orders={1} 
                order by A.ProjectMasterId,A.Orders desc
                ", projectName, orders, ProStatus, InitialApproval).ToList();
            }
            else if (projectName == "ALL" && orders != "ALL" && ProStatus != "ALL" && InitialApproval == "ALL")
            {
                query = _dbEntities.Database.SqlQuery<WpmsAllProjectDetailsModel>(@"
                
                    select * from 
                    (

                        select pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,'Order '+ cast(OrderNuber as varchar(50)) as Orders,case when pm.IsActive=1 then 'YES' else 'NO' end as IsActiveStatus,
	                    pm.AddedDate as ProjectCreationDate,pm.SupplierName,pm.ProjectStatus,cu.UserFullName as ProjectManagerName,pdd.MarketClearanceDate,case when pdd.IsActive=1 then 'YES' else 'NO' end as IsActiveOrder,
	                    case when pdd.OrderQuantity is null then 0 else pdd.OrderQuantity end as OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                    pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                    pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                    pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                    pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamBsi,pm.FrontCamSensor,
	                    case when cast(ppo.PoDate as varchar(50)) is null then 'PO Pending' else cast(ppo.PoDate as varchar(50)) end as PoDate, case when pm.ProjectStatus='NEW' and pm.BiApprovalBy is null and pm.InitialApprovalBy is null and pm.PsApprovalBy is null then 'Action Not taken By BI' when pm.ProjectStatus='NEW' and pm.PsApprovalBy is null then 'Action Not taken By Management Coordinator'
	                    when pm.ProjectStatus='NEW' and pm.InitialApprovalBy is null then 'Action Not taken By MD' 
	                    when pm.ProjectStatus='SWOTPENDING' and pm.BiApprovalBy is null and pm.PsApprovalBy is null and pm.InitialApprovalBy is null then 'SWOT analysis Pending'
	                    else '----' end as InitialApprovalPendings

	                    FROM [CellPhoneProject].[dbo].[ProjectMasters] pm
	                    left join CellPhoneProject.dbo.ProjectPmAssigns ppa on ppa.ProjectMasterId=pm.ProjectMasterId and ppa.Status='ASSIGNED'
	                    left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=ppa.ProjectManagerUserId
	                    left join  CellPhoneProject.dbo.ProjectOrderQuantityDetails pdd on pdd.ProjectMasterId=pm.ProjectMasterId and pdd.IsActive=1
	                    left join [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms] ppo on ppo.ProjectMasterId=pm.ProjectMasterId
                    
                        group by pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,pm.OrderNuber,pm.AddedDate,pm.SupplierName,pm.IsActive,
	                    pm.ProjectStatus,cu.UserFullName,pdd.MarketClearanceDate,pdd.IsActive,pdd.OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                    pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.FrontCamBsi,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                    pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                    pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                    pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamSensor,ppo.PoDate,
	                    pm.InitialApprovalBy,pm.PsApprovalBy,pm.BiApprovalBy

                    )A where 	Orders={1} and 	ProjectStatus={2} 
                    order by A.ProjectMasterId,A.Orders desc
                ", projectName, orders, ProStatus, InitialApproval).ToList();

            }
            else if (projectName == "ALL" && orders == "ALL" && ProStatus != "ALL" && InitialApproval != "ALL")
            {
                query = _dbEntities.Database.SqlQuery<WpmsAllProjectDetailsModel>(@"

                    select * from 
                    (

                        select pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,'Order '+ cast(OrderNuber as varchar(50)) as Orders,case when pm.IsActive=1 then 'YES' else 'NO' end as IsActiveStatus,
	                    pm.AddedDate as ProjectCreationDate,pm.SupplierName,pm.ProjectStatus,cu.UserFullName as ProjectManagerName,pdd.MarketClearanceDate,case when pdd.IsActive=1 then 'YES' else 'NO' end as IsActiveOrder,
	                    case when pdd.OrderQuantity is null then 0 else pdd.OrderQuantity end as OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                    pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                    pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                    pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                    pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamBsi,pm.FrontCamSensor,
	                    case when cast(ppo.PoDate as varchar(50)) is null then 'PO Pending' else cast(ppo.PoDate as varchar(50)) end as PoDate, case when pm.ProjectStatus='NEW' and pm.BiApprovalBy is null and pm.InitialApprovalBy is null and pm.PsApprovalBy is null then 'Action Not taken By BI' when pm.ProjectStatus='NEW' and pm.PsApprovalBy is null then 'Action Not taken By Management Coordinator'
	                    when pm.ProjectStatus='NEW' and pm.InitialApprovalBy is null then 'Action Not taken By MD' 
	                    when pm.ProjectStatus='SWOTPENDING' and pm.BiApprovalBy is null and pm.PsApprovalBy is null and pm.InitialApprovalBy is null then 'SWOT analysis Pending'
	                    else '----' end as InitialApprovalPendings

	                    FROM [CellPhoneProject].[dbo].[ProjectMasters] pm
	                    left join CellPhoneProject.dbo.ProjectPmAssigns ppa on ppa.ProjectMasterId=pm.ProjectMasterId and ppa.Status='ASSIGNED'
	                    left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=ppa.ProjectManagerUserId
	                    left join  CellPhoneProject.dbo.ProjectOrderQuantityDetails pdd on pdd.ProjectMasterId=pm.ProjectMasterId and pdd.IsActive=1
	                    left join [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms] ppo on ppo.ProjectMasterId=pm.ProjectMasterId
                    
                        group by pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,pm.OrderNuber,pm.AddedDate,pm.SupplierName,pm.IsActive,
	                    pm.ProjectStatus,cu.UserFullName,pdd.MarketClearanceDate,pdd.IsActive,pdd.OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                    pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.FrontCamBsi,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                    pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                    pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                    pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamSensor,ppo.PoDate,
	                    pm.InitialApprovalBy,pm.PsApprovalBy,pm.BiApprovalBy

                    )A where ProjectStatus={2} and 	InitialApprovalPendings={3} 
                    order by A.ProjectMasterId,A.Orders desc
                ", projectName, orders, ProStatus, InitialApproval).ToList();
            }
            else if (projectName != "ALL" && orders == "ALL" && ProStatus != "ALL" && InitialApproval == "ALL")
            {
                query = _dbEntities.Database.SqlQuery<WpmsAllProjectDetailsModel>(@"
                select * from 
                (

                    select pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,'Order '+ cast(OrderNuber as varchar(50)) as Orders,case when pm.IsActive=1 then 'YES' else 'NO' end as IsActiveStatus,
	                pm.AddedDate as ProjectCreationDate,pm.SupplierName,pm.ProjectStatus,cu.UserFullName as ProjectManagerName,pdd.MarketClearanceDate,case when pdd.IsActive=1 then 'YES' else 'NO' end as IsActiveOrder,
	                case when pdd.OrderQuantity is null then 0 else pdd.OrderQuantity end as OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamBsi,pm.FrontCamSensor,
	                case when cast(ppo.PoDate as varchar(50)) is null then 'PO Pending' else cast(ppo.PoDate as varchar(50)) end as PoDate, case when pm.ProjectStatus='NEW' and pm.BiApprovalBy is null and pm.InitialApprovalBy is null and pm.PsApprovalBy is null then 'Action Not taken By BI' when pm.ProjectStatus='NEW' and pm.PsApprovalBy is null then 'Action Not taken By Management Coordinator'
	                when pm.ProjectStatus='NEW' and pm.InitialApprovalBy is null then 'Action Not taken By MD' 
	                when pm.ProjectStatus='SWOTPENDING' and pm.BiApprovalBy is null and pm.PsApprovalBy is null and pm.InitialApprovalBy is null then 'SWOT analysis Pending'
	                else '----' end as InitialApprovalPendings

	                FROM [CellPhoneProject].[dbo].[ProjectMasters] pm
	                left join CellPhoneProject.dbo.ProjectPmAssigns ppa on ppa.ProjectMasterId=pm.ProjectMasterId and ppa.Status='ASSIGNED'
	                left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=ppa.ProjectManagerUserId
	                left join  CellPhoneProject.dbo.ProjectOrderQuantityDetails pdd on pdd.ProjectMasterId=pm.ProjectMasterId and pdd.IsActive=1
	                left join [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms] ppo on ppo.ProjectMasterId=pm.ProjectMasterId
                    
                    group by pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,pm.OrderNuber,pm.AddedDate,pm.SupplierName,pm.IsActive,
	                pm.ProjectStatus,cu.UserFullName,pdd.MarketClearanceDate,pdd.IsActive,pdd.OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.FrontCamBsi,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamSensor,ppo.PoDate,
	                pm.InitialApprovalBy,pm.PsApprovalBy,pm.BiApprovalBy

                )A where ProjectName={0}  and ProjectStatus={2} 
                order by A.ProjectMasterId,A.Orders desc
                ", projectName, orders, ProStatus, InitialApproval).ToList();
            }
            else if (projectName != "ALL" && orders == "ALL" && ProStatus == "ALL" && InitialApproval != "ALL")
            {
                query = _dbEntities.Database.SqlQuery<WpmsAllProjectDetailsModel>(@"
                select * from 
                (

                    select pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,'Order '+ cast(OrderNuber as varchar(50)) as Orders,case when pm.IsActive=1 then 'YES' else 'NO' end as IsActiveStatus,
	                pm.AddedDate as ProjectCreationDate,pm.SupplierName,pm.ProjectStatus,cu.UserFullName as ProjectManagerName,pdd.MarketClearanceDate,case when pdd.IsActive=1 then 'YES' else 'NO' end as IsActiveOrder,
	                case when pdd.OrderQuantity is null then 0 else pdd.OrderQuantity end as OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamBsi,pm.FrontCamSensor,
	                case when cast(ppo.PoDate as varchar(50)) is null then 'PO Pending' else cast(ppo.PoDate as varchar(50)) end as PoDate, case when pm.ProjectStatus='NEW' and pm.BiApprovalBy is null and pm.InitialApprovalBy is null and pm.PsApprovalBy is null then 'Action Not taken By BI' when pm.ProjectStatus='NEW' and pm.PsApprovalBy is null then 'Action Not taken By Management Coordinator'
	                when pm.ProjectStatus='NEW' and pm.InitialApprovalBy is null then 'Action Not taken By MD' 
	                when pm.ProjectStatus='SWOTPENDING' and pm.BiApprovalBy is null and pm.PsApprovalBy is null and pm.InitialApprovalBy is null then 'SWOT analysis Pending'
	                else '----' end as InitialApprovalPendings

	                FROM [CellPhoneProject].[dbo].[ProjectMasters] pm
	                left join CellPhoneProject.dbo.ProjectPmAssigns ppa on ppa.ProjectMasterId=pm.ProjectMasterId and ppa.Status='ASSIGNED'
	                left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=ppa.ProjectManagerUserId
	                left join  CellPhoneProject.dbo.ProjectOrderQuantityDetails pdd on pdd.ProjectMasterId=pm.ProjectMasterId and pdd.IsActive=1
	                left join [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms] ppo on ppo.ProjectMasterId=pm.ProjectMasterId
                    
                    group by pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,pm.OrderNuber,pm.AddedDate,pm.SupplierName,pm.IsActive,
	                pm.ProjectStatus,cu.UserFullName,pdd.MarketClearanceDate,pdd.IsActive,pdd.OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.FrontCamBsi,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamSensor,ppo.PoDate,
	                pm.InitialApprovalBy,pm.PsApprovalBy,pm.BiApprovalBy

                )A where ProjectName={0} and InitialApprovalPendings={3} 
                order by A.ProjectMasterId,A.Orders desc
                ", projectName, orders, ProStatus, InitialApproval).ToList();
            }
            else if (projectName == "ALL" && orders != "ALL" && ProStatus == "ALL" && InitialApproval != "ALL")
            {
                query = _dbEntities.Database.SqlQuery<WpmsAllProjectDetailsModel>(@"
                select * from 
                (

                    select pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,'Order '+ cast(OrderNuber as varchar(50)) as Orders,case when pm.IsActive=1 then 'YES' else 'NO' end as IsActiveStatus,
	                pm.AddedDate as ProjectCreationDate,pm.SupplierName,pm.ProjectStatus,cu.UserFullName as ProjectManagerName,pdd.MarketClearanceDate,case when pdd.IsActive=1 then 'YES' else 'NO' end as IsActiveOrder,
	                case when pdd.OrderQuantity is null then 0 else pdd.OrderQuantity end as OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamBsi,pm.FrontCamSensor,
	                case when cast(ppo.PoDate as varchar(50)) is null then 'PO Pending' else cast(ppo.PoDate as varchar(50)) end as PoDate, case when pm.ProjectStatus='NEW' and pm.BiApprovalBy is null and pm.InitialApprovalBy is null and pm.PsApprovalBy is null then 'Action Not taken By BI' when pm.ProjectStatus='NEW' and pm.PsApprovalBy is null then 'Action Not taken By Management Coordinator'
	                when pm.ProjectStatus='NEW' and pm.InitialApprovalBy is null then 'Action Not taken By MD' 
	                when pm.ProjectStatus='SWOTPENDING' and pm.BiApprovalBy is null and pm.PsApprovalBy is null and pm.InitialApprovalBy is null then 'SWOT analysis Pending'
	                else '----' end as InitialApprovalPendings

	                FROM [CellPhoneProject].[dbo].[ProjectMasters] pm
	                left join CellPhoneProject.dbo.ProjectPmAssigns ppa on ppa.ProjectMasterId=pm.ProjectMasterId and ppa.Status='ASSIGNED'
	                left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=ppa.ProjectManagerUserId
	                left join  CellPhoneProject.dbo.ProjectOrderQuantityDetails pdd on pdd.ProjectMasterId=pm.ProjectMasterId and pdd.IsActive=1
	                left join [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms] ppo on ppo.ProjectMasterId=pm.ProjectMasterId
                    
                    group by pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,pm.OrderNuber,pm.AddedDate,pm.SupplierName,pm.IsActive,
	                pm.ProjectStatus,cu.UserFullName,pdd.MarketClearanceDate,pdd.IsActive,pdd.OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.FrontCamBsi,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamSensor,ppo.PoDate,
	                pm.InitialApprovalBy,pm.PsApprovalBy,pm.BiApprovalBy

                )A where Orders={1} and	InitialApprovalPendings={3} 
                order by A.ProjectMasterId,A.Orders desc
                ", projectName, orders, ProStatus, InitialApproval).ToList();
            }
            //4
            else if (projectName != "ALL" && orders != "ALL" && ProStatus != "ALL" && InitialApproval != "ALL")
            {
                query = _dbEntities.Database.SqlQuery<WpmsAllProjectDetailsModel>(@"
                select * from 
                (

                    select pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,'Order '+ cast(OrderNuber as varchar(50)) as Orders,case when pm.IsActive=1 then 'YES' else 'NO' end as IsActiveStatus,
	                pm.AddedDate as ProjectCreationDate,pm.SupplierName,pm.ProjectStatus,cu.UserFullName as ProjectManagerName,pdd.MarketClearanceDate,case when pdd.IsActive=1 then 'YES' else 'NO' end as IsActiveOrder,
	                case when pdd.OrderQuantity is null then 0 else pdd.OrderQuantity end as OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamBsi,pm.FrontCamSensor,
	                case when cast(ppo.PoDate as varchar(50)) is null then 'PO Pending' else cast(ppo.PoDate as varchar(50)) end as PoDate, case when pm.ProjectStatus='NEW' and pm.BiApprovalBy is null and pm.InitialApprovalBy is null and pm.PsApprovalBy is null then 'Action Not taken By BI' when pm.ProjectStatus='NEW' and pm.PsApprovalBy is null then 'Action Not taken By Management Coordinator'
	                when pm.ProjectStatus='NEW' and pm.InitialApprovalBy is null then 'Action Not taken By MD' 
	                when pm.ProjectStatus='SWOTPENDING' and pm.BiApprovalBy is null and pm.PsApprovalBy is null and pm.InitialApprovalBy is null then 'SWOT analysis Pending'
	                else '----' end as InitialApprovalPendings

	                FROM [CellPhoneProject].[dbo].[ProjectMasters] pm
	                left join CellPhoneProject.dbo.ProjectPmAssigns ppa on ppa.ProjectMasterId=pm.ProjectMasterId and ppa.Status='ASSIGNED'
	                left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=ppa.ProjectManagerUserId
	                left join  CellPhoneProject.dbo.ProjectOrderQuantityDetails pdd on pdd.ProjectMasterId=pm.ProjectMasterId and pdd.IsActive=1
	                left join [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms] ppo on ppo.ProjectMasterId=pm.ProjectMasterId
                    
                    group by pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,pm.OrderNuber,pm.AddedDate,pm.SupplierName,pm.IsActive,
	                pm.ProjectStatus,cu.UserFullName,pdd.MarketClearanceDate,pdd.IsActive,pdd.OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.FrontCamBsi,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamSensor,ppo.PoDate,
	                pm.InitialApprovalBy,pm.PsApprovalBy,pm.BiApprovalBy

                )A where ProjectName={0} and 	Orders={1} and 	ProjectStatus={2} and 	InitialApprovalPendings={3} 
                order by A.ProjectMasterId,A.Orders desc
                ", projectName, orders, ProStatus, InitialApproval).ToList();
            }
            //3
            else if (projectName != "ALL" && orders != "ALL" && ProStatus != "ALL" && InitialApproval == "ALL")
            {
                query = _dbEntities.Database.SqlQuery<WpmsAllProjectDetailsModel>(@"
                select * from 
                (

                    select pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,'Order '+ cast(OrderNuber as varchar(50)) as Orders,case when pm.IsActive=1 then 'YES' else 'NO' end as IsActiveStatus,
	                pm.AddedDate as ProjectCreationDate,pm.SupplierName,pm.ProjectStatus,cu.UserFullName as ProjectManagerName,pdd.MarketClearanceDate,case when pdd.IsActive=1 then 'YES' else 'NO' end as IsActiveOrder,
	                case when pdd.OrderQuantity is null then 0 else pdd.OrderQuantity end as OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamBsi,pm.FrontCamSensor,
	                case when cast(ppo.PoDate as varchar(50)) is null then 'PO Pending' else cast(ppo.PoDate as varchar(50)) end as PoDate, case when pm.ProjectStatus='NEW' and pm.BiApprovalBy is null and pm.InitialApprovalBy is null and pm.PsApprovalBy is null then 'Action Not taken By BI' when pm.ProjectStatus='NEW' and pm.PsApprovalBy is null then 'Action Not taken By Management Coordinator'
	                when pm.ProjectStatus='NEW' and pm.InitialApprovalBy is null then 'Action Not taken By MD' 
	                when pm.ProjectStatus='SWOTPENDING' and pm.BiApprovalBy is null and pm.PsApprovalBy is null and pm.InitialApprovalBy is null then 'SWOT analysis Pending'
	                else '----' end as InitialApprovalPendings

	                FROM [CellPhoneProject].[dbo].[ProjectMasters] pm
	                left join CellPhoneProject.dbo.ProjectPmAssigns ppa on ppa.ProjectMasterId=pm.ProjectMasterId and ppa.Status='ASSIGNED'
	                left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=ppa.ProjectManagerUserId
	                left join  CellPhoneProject.dbo.ProjectOrderQuantityDetails pdd on pdd.ProjectMasterId=pm.ProjectMasterId and pdd.IsActive=1
	                left join [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms] ppo on ppo.ProjectMasterId=pm.ProjectMasterId
                    
                    group by pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,pm.OrderNuber,pm.AddedDate,pm.SupplierName,pm.IsActive,
	                pm.ProjectStatus,cu.UserFullName,pdd.MarketClearanceDate,pdd.IsActive,pdd.OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.FrontCamBsi,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamSensor,ppo.PoDate,
	                pm.InitialApprovalBy,pm.PsApprovalBy,pm.BiApprovalBy

                )A where ProjectName={0} and Orders={1} and 	ProjectStatus={2} 
                order by A.ProjectMasterId,A.Orders desc
                ", projectName, orders, ProStatus, InitialApproval).ToList();
            }
            else if (projectName == "ALL" && orders != "ALL" && ProStatus != "ALL" && InitialApproval != "ALL")
            {
                query = _dbEntities.Database.SqlQuery<WpmsAllProjectDetailsModel>(@"
                select * from 
                (

                    select pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,'Order '+ cast(OrderNuber as varchar(50)) as Orders,case when pm.IsActive=1 then 'YES' else 'NO' end as IsActiveStatus,
	                pm.AddedDate as ProjectCreationDate,pm.SupplierName,pm.ProjectStatus,cu.UserFullName as ProjectManagerName,pdd.MarketClearanceDate,case when pdd.IsActive=1 then 'YES' else 'NO' end as IsActiveOrder,
	                case when pdd.OrderQuantity is null then 0 else pdd.OrderQuantity end as OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamBsi,pm.FrontCamSensor,
	                case when cast(ppo.PoDate as varchar(50)) is null then 'PO Pending' else cast(ppo.PoDate as varchar(50)) end as PoDate, case when pm.ProjectStatus='NEW' and pm.BiApprovalBy is null and pm.InitialApprovalBy is null and pm.PsApprovalBy is null then 'Action Not taken By BI' when pm.ProjectStatus='NEW' and pm.PsApprovalBy is null then 'Action Not taken By Management Coordinator'
	                when pm.ProjectStatus='NEW' and pm.InitialApprovalBy is null then 'Action Not taken By MD' 
	                when pm.ProjectStatus='SWOTPENDING' and pm.BiApprovalBy is null and pm.PsApprovalBy is null and pm.InitialApprovalBy is null then 'SWOT analysis Pending'
	                else '----' end as InitialApprovalPendings

	                FROM [CellPhoneProject].[dbo].[ProjectMasters] pm
	                left join CellPhoneProject.dbo.ProjectPmAssigns ppa on ppa.ProjectMasterId=pm.ProjectMasterId and ppa.Status='ASSIGNED'
	                left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=ppa.ProjectManagerUserId
	                left join  CellPhoneProject.dbo.ProjectOrderQuantityDetails pdd on pdd.ProjectMasterId=pm.ProjectMasterId and pdd.IsActive=1
	                left join [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms] ppo on ppo.ProjectMasterId=pm.ProjectMasterId
                    
                    group by pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,pm.OrderNuber,pm.AddedDate,pm.SupplierName,pm.IsActive,
	                pm.ProjectStatus,cu.UserFullName,pdd.MarketClearanceDate,pdd.IsActive,pdd.OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.FrontCamBsi,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamSensor,ppo.PoDate,
	                pm.InitialApprovalBy,pm.PsApprovalBy,pm.BiApprovalBy

                )A where Orders={1} and 	ProjectStatus={2} and 	InitialApprovalPendings={3} 
                order by A.ProjectMasterId,A.Orders desc
                ", projectName, orders, ProStatus, InitialApproval).ToList();
            }
            else if (projectName != "ALL" && orders == "ALL" && ProStatus != "ALL" && InitialApproval != "ALL")
            {
                query = _dbEntities.Database.SqlQuery<WpmsAllProjectDetailsModel>(@"
                select * from 
                (

                    select pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,'Order '+ cast(OrderNuber as varchar(50)) as Orders,case when pm.IsActive=1 then 'YES' else 'NO' end as IsActiveStatus,
	                pm.AddedDate as ProjectCreationDate,pm.SupplierName,pm.ProjectStatus,cu.UserFullName as ProjectManagerName,pdd.MarketClearanceDate,case when pdd.IsActive=1 then 'YES' else 'NO' end as IsActiveOrder,
	                case when pdd.OrderQuantity is null then 0 else pdd.OrderQuantity end as OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamBsi,pm.FrontCamSensor,
	                case when cast(ppo.PoDate as varchar(50)) is null then 'PO Pending' else cast(ppo.PoDate as varchar(50)) end as PoDate, case when pm.ProjectStatus='NEW' and pm.BiApprovalBy is null and pm.InitialApprovalBy is null and pm.PsApprovalBy is null then 'Action Not taken By BI' when pm.ProjectStatus='NEW' and pm.PsApprovalBy is null then 'Action Not taken By Management Coordinator'
	                when pm.ProjectStatus='NEW' and pm.InitialApprovalBy is null then 'Action Not taken By MD' 
	                when pm.ProjectStatus='SWOTPENDING' and pm.BiApprovalBy is null and pm.PsApprovalBy is null and pm.InitialApprovalBy is null then 'SWOT analysis Pending'
	                else '----' end as InitialApprovalPendings

	                FROM [CellPhoneProject].[dbo].[ProjectMasters] pm
	                left join CellPhoneProject.dbo.ProjectPmAssigns ppa on ppa.ProjectMasterId=pm.ProjectMasterId and ppa.Status='ASSIGNED'
	                left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=ppa.ProjectManagerUserId
	                left join  CellPhoneProject.dbo.ProjectOrderQuantityDetails pdd on pdd.ProjectMasterId=pm.ProjectMasterId and pdd.IsActive=1
	                left join [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms] ppo on ppo.ProjectMasterId=pm.ProjectMasterId
                    
                    group by pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,pm.OrderNuber,pm.AddedDate,pm.SupplierName,pm.IsActive,
	                pm.ProjectStatus,cu.UserFullName,pdd.MarketClearanceDate,pdd.IsActive,pdd.OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.FrontCamBsi,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamSensor,ppo.PoDate,
	                pm.InitialApprovalBy,pm.PsApprovalBy,pm.BiApprovalBy

                )A where ProjectName={0} and ProjectStatus={2} and 	InitialApprovalPendings={3} 
                order by A.ProjectMasterId,A.Orders desc
                ", projectName, orders, ProStatus, InitialApproval).ToList();
            }
            else if (projectName != "ALL" && orders != "ALL" && ProStatus == "ALL" && InitialApproval != "ALL")
            {
                query = _dbEntities.Database.SqlQuery<WpmsAllProjectDetailsModel>(@"

                select * from 
                (

                    select pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,'Order '+ cast(OrderNuber as varchar(50)) as Orders,case when pm.IsActive=1 then 'YES' else 'NO' end as IsActiveStatus,
	                pm.AddedDate as ProjectCreationDate,pm.SupplierName,pm.ProjectStatus,cu.UserFullName as ProjectManagerName,pdd.MarketClearanceDate,case when pdd.IsActive=1 then 'YES' else 'NO' end as IsActiveOrder,
	                case when pdd.OrderQuantity is null then 0 else pdd.OrderQuantity end as OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamBsi,pm.FrontCamSensor,
	                case when cast(ppo.PoDate as varchar(50)) is null then 'PO Pending' else cast(ppo.PoDate as varchar(50)) end as PoDate, case when pm.ProjectStatus='NEW' and pm.BiApprovalBy is null and pm.InitialApprovalBy is null and pm.PsApprovalBy is null then 'Action Not taken By BI' when pm.ProjectStatus='NEW' and pm.PsApprovalBy is null then 'Action Not taken By Management Coordinator'
	                when pm.ProjectStatus='NEW' and pm.InitialApprovalBy is null then 'Action Not taken By MD' 
	                when pm.ProjectStatus='SWOTPENDING' and pm.BiApprovalBy is null and pm.PsApprovalBy is null and pm.InitialApprovalBy is null then 'SWOT analysis Pending'
	                else '----' end as InitialApprovalPendings

	                FROM [CellPhoneProject].[dbo].[ProjectMasters] pm
	                left join CellPhoneProject.dbo.ProjectPmAssigns ppa on ppa.ProjectMasterId=pm.ProjectMasterId and ppa.Status='ASSIGNED'
	                left join CellPhoneProject.dbo.CmnUsers cu on cu.CmnUserId=ppa.ProjectManagerUserId
	                left join  CellPhoneProject.dbo.ProjectOrderQuantityDetails pdd on pdd.ProjectMasterId=pm.ProjectMasterId and pdd.IsActive=1
	                left join [CellPhoneProject].[dbo].[ProjectPurchaseOrderForms] ppo on ppo.ProjectMasterId=pm.ProjectMasterId
                    
                    group by pm.ProjectMasterId,pm.ProjectName,pdd.ProjectModel,pm.OrderNuber,pm.AddedDate,pm.SupplierName,pm.IsActive,
	                pm.ProjectStatus,cu.UserFullName,pdd.MarketClearanceDate,pdd.IsActive,pdd.OrderQuantity,pm.SupplierTrustLevel,pm.ProjectType,pm.OsName,
	                pm.OsVersion,pm.DisplaySize,pm.DisplayName,pm.ProcessorName,pm.ProcessorClock,pm.Chipset,pm.FrontCamera,pm.FrontCam,pm.FrontCamBsi,pm.BackCam,pm.BackCamera,pm.Ram,pm.Rom,
	                pm.Battery,pm.SimSlotNumber,pm.ProjectNameForScreening,pm.PcbaFinalVendor,pm.PcbaVendorName,pm.DisplayResulution,pm.Color,pm.BatteryType,
	                pm.BatteryRating,pm.SecondGen,pm.FlashLight,pm.BatteryCoverLogoType,pm.SpecialSensor,pm.Gps,pm.Otg,pm.HallSensor,pm.Gyroscope,pm.Compass,
	                pm.Lsensor,pm.Psensor,pm.Gsensor,pm.ChipsetCore,pm.ChipsetBit,pm.ChipsetFrequency,pm.ChipsetName,pm.CpuName,pm.FrontCamSensor,ppo.PoDate,
	                pm.InitialApprovalBy,pm.PsApprovalBy,pm.BiApprovalBy

                )A where ProjectName={0} and 	Orders={1} and 	InitialApprovalPendings={3} 
                order by A.ProjectMasterId,A.Orders desc
                ", projectName, orders, ProStatus, InitialApproval).ToList();
            }

            return query;
        }

        public ResponseModel CompleteReport(long id)
        {
            var responseModel = new ResponseModel();
            try
            {
                MaterialWastageMaster materialWastageMaster =
                _dbEntities.MaterialWastageMasters.FirstOrDefault(i => i.Id == id);
                if (materialWastageMaster != null)
                {
                    materialWastageMaster.IsCompleted = true;
                    materialWastageMaster.UpdatedDate = DateTime.Now;
                    materialWastageMaster.UpdatedBy = Convert.ToInt64(HttpContext.Current.User.Identity.Name);
                    materialWastageMaster.CompletedBy = Convert.ToInt64(HttpContext.Current.User.Identity.Name);
                    materialWastageMaster.CompletedDate = DateTime.Now;

                    _dbEntities.Entry(materialWastageMaster).State = EntityState.Modified;
                    _dbEntities.SaveChanges();
                    responseModel.ResponseCode = 1;
                    responseModel.ResponseMessage = "Report Completed Successfully.";
                }
                else
                {
                    responseModel.ResponseCode = 2;
                    responseModel.ResponseMessage = "Data not found to update or complete";
                }


            }
            catch (Exception exception)
            {
                responseModel.ResponseCode = 0;
                responseModel.ResponseMessage = exception.Message;
            }
            return responseModel;
        }

        public List<WpmsAllProjectDetailsModel> GetAllProStatus()
        {
            var query = new List<WpmsAllProjectDetailsModel>();

            query = _dbEntities.Database.SqlQuery<WpmsAllProjectDetailsModel>(@"
            select distinct ProjectStatus from [CellPhoneProject].[dbo].[ProjectMasters] pm order by pm.ProjectStatus asc").ToList();

            return query;
        }


        #endregion
    }

    public class OracleItem
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public int MOQ { get; set; }
        public int MPQ { get; set; }
        public int Number_Of_Reel { get; set; }
    }

    public class OracleBOMItem
    {
        public DateTime ORDER_DATE { get; set; }
        public int ORGANIZATION_ID { get; set; }
        public string FG_MODEL_ID { get; set; }
        public string FG_MODEL_PROJECT { get; set; }
        public long ITEM_ID { get; set; }
        public string ITEM_CODE { get; set; }
        public int QUANTITY { get; set; }
        public string REMARKS { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATION_DATE { get; set; }
        public string ORDER_NO { get; set; }

    }
}