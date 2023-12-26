using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Net;
using System.ServiceModel.Description;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Http.ModelBinding;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using Microsoft.ReportingServices.Rendering.ImageRenderer;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using ProjectManagement.DAL;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Models;
using ProjectManagement.Models.AssignModels;
using ProjectManagement.Models.ManagementDashboard;
using ProjectManagement.ViewModels.Management;
using ProjectManagement.Models.StausObjects;
using System.Configuration;
using System.Data.SqlClient;

namespace ProjectManagement.Infrastructures.Repositories
{
    public class ManagementRepository : IManagementRepository
    {
        private readonly CellPhoneProjectEntities _dbContext;

        public ManagementRepository()
        {
            _dbContext = new CellPhoneProjectEntities();
            _dbContext.Configuration.LazyLoadingEnabled = false;
        }
        public ProjectMasterModel GetProjectMasterModel(long id)
        {
            ProjectMaster master = GenereticRepo<ProjectMaster>.GetById(_dbContext, id);
            ProjectMasterModel masterModel = GenericMapper<ProjectMaster, ProjectMasterModel>.GetDestination(master);
            return masterModel;
        }

        public List<ProjectMasterModel> GetInitialApprovalPendingProjectList()
        {
            List<ProjectMaster> masters = GenereticRepo<ProjectMaster>.GetList(_dbContext, x => x.ProjectStatus == "NEW");

            List<ProjectMasterModel> masterModels = GenericMapper<ProjectMaster, ProjectMasterModel>.GetDestinationList(masters);

            foreach (var model in masterModels)
            {
                model.PsApprovalByName =
                    _dbContext.CmnUsers.Where(x => x.CmnUserId == model.PsApprovalBy)
                        .Select(x => x.UserFullName)
                        .FirstOrDefault();
                model.CeoApprovalByName =
                    _dbContext.CmnUsers.Where(x => x.CmnUserId == model.CeoApprovalBy)
                        .Select(x => x.UserFullName)
                        .FirstOrDefault();
                //Accessories part
                var accessories = _dbContext.AccessoriesPrices.Where(i => i.ProjectMasterId == model.ProjectMasterId);
                if (accessories.Any())
                {
                    decimal price = 0;
                    foreach (var accessoriesPrice in accessories)
                    {
                        if (accessoriesPrice.Currency != "USD")
                            price += CommonConversion.CurrencyConversion(Decimal.Parse(accessoriesPrice.Price),
                                accessoriesPrice.Currency, "USD");
                        else
                        {
                            price += Decimal.Parse(accessoriesPrice.Price);
                        }
                    }
                    model.TotalPrice = price;
                }
                //project image part
                List<ProjectImage> projectImages = _dbContext.ProjectImages.Where(x => x.ProjectId == model.ProjectMasterId).ToList();
                var config = new MapperConfiguration(c => c.CreateMap<ProjectImage, ProjectImageModel>());
                var map = config.CreateMapper();
                var proimg = map.Map<List<ProjectImageModel>>(projectImages);
                foreach (var img in proimg)
                {
                    var manager = new FileManager();
                    img.ImagePath = manager.GetFile(img.ImagePath);
                }
                model.ProjectImageModels = proimg;
            }
            return masterModels;
        }

        public List<ProjectMasterWithPoCustomModel> GetRunningProjectMasterModelList()
        {
            //List<ProjectMaster> masters =  //GenereticRepo<ProjectMaster>.GetList(_dbContext, x => x.ProjectStatus == "APPROVED");
            var masters = (from master in _dbContext.ProjectMasters
                           join orderForm in _dbContext.ProjectPurchaseOrderForms on master.ProjectMasterId equals
                               orderForm.ProjectMasterId
                           where master.ProjectStatus == "APPROVED" && orderForm.IsCompleted == false && master.IsActive
                           select new{master,orderForm}).ToList();
            var masterModels=new List<ProjectMasterWithPoCustomModel>();
            foreach (var m in masters)
            {
                var master = new ProjectMasterWithPoCustomModel
                {
                    ProjectMasterId = m.master.ProjectMasterId,
                    ProjectTypeId = m.master.ProjectTypeId,
                    ProjectName = m.master.ProjectName,
                    ProjectNameForScreening = m.master.ProjectNameForScreening,
                    SupplierModelName = m.master.SupplierModelName,
                    SupplierTrustLevel = m.master.SupplierTrustLevel,
                    SupplierName = m.master.SupplierName,
                    NumberOfSample = m.master.NumberOfSample,
                    OrderNuber = m.master.OrderNuber,
                    OrderQuantity = m.orderForm.Quantity,
                    PoDate = m.orderForm.PoDate,
                    FinalPrice = m.master.FinalPrice,
                    ApproxProjectFinishDate = m.master.ApproxProjectFinishDate,
                    ChainaInspectionDate = _dbContext.ProjectOrderShipments.Where(x => x.ProjectMasterId == m.master.ProjectMasterId).OrderByDescending(x=>x.ChainaInspectionDate).Select(x => x.ChainaInspectionDate).FirstOrDefault()
                };
                masterModels.Add(master);
            }
            return masterModels;
        }

        public List<ProjectMasterWithPoCustomModel> GetCompletedProjectMasterModelList()
        {
            //List<ProjectMaster> masters =  //GenereticRepo<ProjectMaster>.GetList(_dbContext, x => x.ProjectStatus == "APPROVED");
            var masters = (from master in _dbContext.ProjectMasters
                           join orderForm in _dbContext.ProjectPurchaseOrderForms on master.ProjectMasterId equals
                               orderForm.ProjectMasterId
                           where master.ProjectStatus == "APPROVED" && orderForm.IsCompleted == true
                           select new { master, orderForm }).ToList();
            var masterModels = new List<ProjectMasterWithPoCustomModel>();
            foreach (var m in masters)
            {
                var master = new ProjectMasterWithPoCustomModel
                {
                    ProjectMasterId = m.master.ProjectMasterId,
                    ProjectTypeId = m.master.ProjectTypeId,
                    ProjectName = m.master.ProjectName,
                    ProjectNameForScreening = m.master.ProjectNameForScreening,
                    SupplierModelName = m.master.SupplierModelName,
                    SupplierTrustLevel = m.master.SupplierTrustLevel,
                    SupplierName = m.master.SupplierName,
                    NumberOfSample = m.master.NumberOfSample,
                    OrderNuber = m.master.OrderNuber,
                    OrderQuantity = m.orderForm.Quantity,
                    PoDate = m.orderForm.PoDate,
                    FinalPrice = m.master.FinalPrice,
                    ApproxProjectFinishDate = m.master.ApproxProjectFinishDate
                };
                masterModels.Add(master);
            }
            return masterModels;
        }


        public string SetProjectMaster(long projectMasterId, string managementComment)
        {
            long userId = Convert.ToInt64(HttpContext.Current.User.Identity.Name == "" ? "0" : HttpContext.Current.User.Identity.Name);
            ProjectMaster master = GenereticRepo<ProjectMaster>.GetById(_dbContext, projectMasterId);
            //Do some update
            if (HttpContext.Current.User.IsInRole("PS"))//Adnan vai's rejection
            {
                //master.ProjectStatus = "NEW";
                //master.Updated = userId;
                //master.UpdatedDate = DateTime.Now;
                master.PsApprovalBy = userId;
                master.PsApprovalDate = DateTime.Now;
                master.PsRemarks = managementComment;
                GenereticRepo<ProjectMaster>.Update(_dbContext, master);//update method
                //save into tracker table for log
                ProjectMasterTracker tracker = GenericMapper<ProjectMaster, ProjectMasterTracker>.GetDestination(master);
                tracker.TrackerAddedDate = DateTime.Now;
                tracker.TrackerAddedBy = userId;
                _dbContext.ProjectMasterTrackers.Add(tracker);
                _dbContext.SaveChanges();
                //mail
                var msg =
                        @"This is to inform you that, Project "+master.ProjectName+"(order "+master.OrderNuber+") has been REJECTED in Walton Project Management System by <b>" + "Adnan Afzal.";
                var body = string.Format(msg);
                var mail = new MailSendFromPms();
                mail.SendMail(new List<string>(new[] { "MM" }),
                    new List<string>(new[] {"CM", "CMHEAD", "SPRHEAD", "PMHEAD", "QCHEAD", "HWHEAD", "PS","CEO","BIHEAD"}), "PROJECT REJECTED ( " + master.ProjectName + " )", body);
            }
            if (HttpContext.Current.User.IsInRole("BIHEAD"))//Reza vai's rejection
            {
                //master.ProjectStatus = "NEW";
                //master.Updated = userId;
                //master.UpdatedDate = DateTime.Now;
                master.BiApprovalBy = userId;
                master.BiApprovalDate = DateTime.Now;
                master.BiRemarks = managementComment;
                GenereticRepo<ProjectMaster>.Update(_dbContext, master);//update method
                //save into tracker table for log
                ProjectMasterTracker tracker = GenericMapper<ProjectMaster, ProjectMasterTracker>.GetDestination(master);
                tracker.TrackerAddedDate = DateTime.Now;
                tracker.TrackerAddedBy = userId;
                _dbContext.ProjectMasterTrackers.Add(tracker);
                _dbContext.SaveChanges();
                //mail
                var msg =
                        @"This is to inform you that, Project " + master.ProjectName + "(order " + master.OrderNuber + ") has been REJECTED in Walton Project Management System by <b>" + "Md. Rezaul Hasan.";
                var body = string.Format(msg);
                var mail = new MailSendFromPms();
                mail.SendMail(new List<string>(new[] { "MM" }),
                    new List<string>(new[] { "CM", "CMHEAD", "SPRHEAD", "PMHEAD", "QCHEAD", "HWHEAD", "PS", "CEO", "BIHEAD" }), "PROJECT REJECTED ( " + master.ProjectName + " )", body);
            }
            if (HttpContext.Current.User.IsInRole("MM"))
            {
                master.ProjectStatus = "REJECTED";
                //master.Updated = userId;
                //master.UpdatedDate = DateTime.Now;
                master.ManagentComment = managementComment;
                GenereticRepo<ProjectMaster>.Update(_dbContext, master);
                //save into tracker table for log
                ProjectMasterTracker tracker = GenericMapper<ProjectMaster, ProjectMasterTracker>.GetDestination(master);
                tracker.TrackerAddedDate = DateTime.Now;
                tracker.TrackerAddedBy = userId;
                _dbContext.ProjectMasterTrackers.Add(tracker);
                _dbContext.SaveChanges();
                //mail
                var msg =
                        @"This is to inform you that, Project " + master.ProjectName + "(order " + master.OrderNuber + ") has been REJECTED in WPMS by Management";
                var body = string.Format(msg);
                var mail = new MailSendFromPms();
                mail.SendMail(new List<string>(new[] { "MM" }),
                    new List<string>(new[] { "CM", "CMHEAD", "SPRHEAD", "PMHEAD", "QCHEAD", "HWHEAD", "PS","CEO","BIHEAD" }), "PROJECT REJECTED ( " + master.ProjectName + " )", body);
            }
            return "success";
        }

        public List<PmQcAssignModel> GetPmQcAssignModels()
        {
            String query = String.Format(@"select pm.ProjectMasterId,pm.ProjectName,
pt.TypeName ,
ppa.AssignUserId,
(select UserFullName from CmnUsers cu where Cu.CmnUserId=ppa.AssignUserId) 
AssignUserName,ppa.AssignDate,ppa.ProjectManagerUserId,
(select UserFullName from CmnUsers cu where Cu.CmnUserId=ppa.ProjectManagerUserId) 
ProjectManagerUserName
from ProjectMasters pm
inner join ProjectTypes pt
on pm.ProjectTypeId=pt.ProjectTypeId
inner join ProjectPmAssigns ppa
on pm.ProjectMasterId=ppa.ProjectMasterId
");
            List<PmQcAssignModel> models = GenereticRepo<PmQcAssignModel>.GetList(_dbContext, query);
            return models;
        }

        public void SetSampleSetApproval(long projectMasterId,string remarks)
        {
            long userId = Convert.ToInt64(string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? "0" : HttpContext.Current.User.Identity.Name);
            ProjectMaster master = GenereticRepo<ProjectMaster>.GetById(_dbContext, projectMasterId);

            var user = _dbContext.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);
            if (user != null && user.RoleName == "MM")//Ovee sir's approval
            {
                master.ProjectStatus = master.SourcingType == "OEM" ? "INITIAL" : "APPROVED";
                master.InitialApprovalBy = userId;
                master.InitialApprovalDate = DateTime.Now;
                master.InitialApprovalRemarks = remarks;
                if (master.SourcingType != "OEM") master.FinalApprovalDate = DateTime.Now;
                GenereticRepo<ProjectMaster>.Update(_dbContext, master);
                string usrInfo;
                string time;
                string msg = string.Empty;
                
                if (master.SourcingType == "OEM")
                {
                    msg =
                        @"This is to inform you that, A new project has been initially approved for screening test in Walton Project Management System by our Management.<br/><br/><b>Project Name:";
                    usrInfo = "<br/>Project Initially Approved By: " + user.UserFullName;
                    time = "<br/>Initial approval date : " + DateTime.Now.ToLongDateString();
                }
                else
                {
                    msg =
                        @"This is to inform you that, A new project has been finally approved in Walton Project Management System by our Management.<br/><br/><b>Project Name:";
                    usrInfo =  "<br/>Project finally Approved By: " + user.UserFullName;
                    time = "<br/>Final approval date : " + DateTime.Now.ToLongDateString();
                }


                var body = string.Format(msg + master.ProjectName + "</b>" + usrInfo + time);
                var mail = new MailSendFromPms();
                mail.SendMail(new List<string>(new[] { "MM" }),
                    new List<string>(new[] { "CM", "CMHEAD", "SPRHEAD", "PMHEAD", "QCHEAD", "HWHEAD", "SA", "PS", "BIHEAD","CEO" }), "NEW PROJECT( " + master.ProjectName + " )", body);
            }
            if (user != null && user.RoleName == "PS" && master.PsApprovalBy==null)//Adnan vai's Approval
            {
                master.PsApprovalBy = userId;
                master.PsApprovalDate = DateTime.Now;
                master.PsRemarks = remarks;
                GenereticRepo<ProjectMaster>.Update(_dbContext, master);
                var msg =
                        @"This is to inform you that, A new project has been approved in Walton Project Management System by <b>" + user.UserFullName + "</b>.<br/><br/><b>Project Name:";
                var time = "<br/>Approval date : " + DateTime.Now.ToLongDateString();
                var body = string.Format(msg + master.ProjectName + "</b>" + time);
                var mail = new MailSendFromPms();
                mail.SendMail(new List<string>(new[] { "MM" }),
                    new List<string>(new[] { "CM", "CMHEAD", "SPRHEAD", "PMHEAD", "QCHEAD", "HWHEAD", "SA", "PS", "BIHEAD","CEO" }), "NEW PROJECT( " + master.ProjectName + " )", body);
            }
            if (user != null && user.RoleName == "BIHEAD" && master.BiApprovalBy == null)//Reza vai's Approval
            {
                master.BiApprovalBy = userId;
                master.BiApprovalDate = DateTime.Now;
                master.BiRemarks = remarks;
                GenereticRepo<ProjectMaster>.Update(_dbContext, master);
                var msg =
                        @"This is to inform you that, A new project has been approved in Walton Project Management System by <b>" + user.UserFullName + "</b>.<br/><br/><b>Project Name:";
                var time = "<br/>Approval date : " + DateTime.Now.ToLongDateString();
                var body = string.Format(msg + master.ProjectName + "</b>" + time);
                var mail = new MailSendFromPms();
                mail.SendMail(new List<string>(new[] { "MM" }),
                    new List<string>(new[] { "CM", "CMHEAD", "SPRHEAD", "PMHEAD", "QCHEAD", "HWHEAD", "SA", "PS", "BIHEAD","CEO" }), "NEW PROJECT( " + master.ProjectName + " )", body);
            }
            if (user != null && user.RoleName == "CEO" && master.CeoApprovalBy==null)//Shiplu sir's Approval
            {
                master.CeoApprovalBy = userId;
                master.CeoApprovalDate = DateTime.Now;
                GenereticRepo<ProjectMaster>.Update(_dbContext, master);
                var msg =
                        @"This is to inform you that, A new project has been approved in Walton Project Management System by <b>" + user.UserFullName + "</b>.<br/><br/><b>Project Name:";
                var time = "<br/>Approval date : " + DateTime.Now.ToLongDateString();
                var body = string.Format(msg + master.ProjectName + "</b>" + time);
                var mail = new MailSendFromPms();
                mail.SendMail(new List<string>(new[] { "MM" }),
                    new List<string>(new[] { "CM", "CMHEAD", "SPRHEAD", "PMHEAD", "QCHEAD", "HWHEAD", "SA", "PS", "BIHEAD","CEO" }), "NEW PROJECT( " + master.ProjectName + " )", body);
            }
        }

        public List<HwCmProjectFinalApprovalViewModel> GetHwCmProjectFinalApprovalViewModel()
        {
            //            String query = String.Format(@"select * 
            //from ProjectMasters pm
            //inner join HwQcInchargeAssigns hqia
            //on pm.ProjectMasterId=hqia.ProjectMasterId
            //--inner join HwQcAssigns a
            //--on hqia.HwQcInchargeAssignId=a.HwQcInchargeAssignId
            //--inner join HwIssueComments c
            //--on a.HwQcAssignId=c.HwQcAssignId
            //where pm.ProjectStatus='PARTIAL' 
            //and hqia.TestPhase='Finished'");
            //            List<HwCmProjectFinalApprovalViewModel> model =
            //                GenereticRepo<HwCmProjectFinalApprovalViewModel>.GetList(_dbContext, query);

            List<HwCmProjectFinalApprovalViewModel> model = (
                            from master in _dbContext.ProjectMasters
                            join assign in _dbContext.HwQcInchargeAssigns on master.ProjectMasterId equals assign.ProjectMasterId
                            join hwQcAssign in _dbContext.HwQcAssigns on assign.HwQcInchargeAssignId equals hwQcAssign.HwQcInchargeAssignId
                            where master.ProjectStatus == "PARTIAL" && assign.IsScreeningTest == true && assign.TestPhase == "FINISHED"
                            select new HwCmProjectFinalApprovalViewModel
                            {
                                ProjectMasterId = master.ProjectMasterId,
                                HwQcInchargeAssignId = hwQcAssign.HwQcInchargeAssignId,
                                //HwQcAssignId = hwQcAssign.HwQcAssignId,
                                ProjectName = master.ProjectName,
                                SupplierName = master.SupplierName,
                                ProjectType = master.ProjectType,
                                OsName = master.OsName,
                                Remark = assign.Remark,
                                DisplayName = master.DisplayName,
                                ProcessorName = master.ProcessorName,
                                Chipset = master.Chipset,
                                ApproxProjectFinishDate = master.ApproxProjectFinishDate
                            }
                        ).Distinct().ToList();


            return model;
        }

        public void SetHwCmProjectFinalApproval(long projectMasterId, string comment, string approved)
        {

            long userId = Convert.ToInt64(HttpContext.Current.User.Identity.Name == "" ? "0" : HttpContext.Current.User.Identity.Name);
            ProjectMaster master = GenereticRepo<ProjectMaster>.GetById(_dbContext, projectMasterId);
            master.ProjectStatus = approved;
            master.FinalApprovalBy = userId;
            master.FinalApprovalDate = DateTime.Now;

            GenereticRepo<ProjectMaster>.Update(_dbContext, master);
            string apprvStr = approved == "APPROVED"
                ? "This is to inform you that, a project finally has been approved as a walton brand model by our Management after reviewing the screening test report. Let's get started to make this new project more powerful and successful."
                : "This is to inform you that, a new project has been rejected test by our Management after reviewing a screening test report. Hope we will complete a project with the best of best in future.";
            var user = _dbContext.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);
            var usrInfo = user != null ? "<br/>Action taken by : " + user.UserFullName : "";
            string time = "<br/>Action taken date : " + DateTime.Now.ToLongDateString();
            var body =
                string.Format(
                    @"{0}<br/><br/><b>Project Name: " +
                    master.ProjectName + "</b>" + usrInfo + time, apprvStr);
            var mail = new MailSendFromPms();
            var result = mail.SendMail(new List<string>(new[] { "MM" }),
                new List<string>(new[] { "CM", "PMHEAD", "QCHEAD", "HWHEAD", "SA", "PS" }), "NEW PROJECT( " + master.ProjectName + " )", body);
        }

        public List<ProjectMasterModel> GetProjectAlive()
        {
            List<ProjectMaster> masters = GenereticRepo<ProjectMaster>.GetList(_dbContext, x => x.ProjectStatus == "REJECTED");

            List<ProjectMasterModel> masterModels =
                GenericMapper<ProjectMaster, ProjectMasterModel>.GetDestinationList(masters);
            return masterModels;
        }

        public List<WorkProgressData> ProjectMonthlyWorkprogress(long projectId)
        {
            var query = string.Empty;
            if (projectId == 0)
            {
                query = string.Format(@"select top 5 x.ProjectMasterId, x.ProjectName,
  sum(case when x.MonthNo = 1 then 1 else 0 end) Jan,
   sum(case when x.MonthNo = 2 then 1 else 0 end) Feb,
    sum(case when x.MonthNo = 3 then 1 else 0 end) Mar,
	sum(case when x.MonthNo = 4 then 1 else 0 end) Apr,
	sum(case when x.MonthNo = 5 then 1 else 0 end) May,
	sum(case when x.MonthNo = 6 then 1 else 0 end) Jun,
	sum(case when x.MonthNo = 7 then 1 else 0 end) Jul,
	sum(case when x.MonthNo = 8 then 1 else 0 end) Aug,
	sum(case when x.MonthNo = 9 then 1 else 0 end) Sep,
	sum(case when x.MonthNo = 10 then 1 else 0 end) Oct,
	sum(case when x.MonthNo = 11 then 1 else 0 end) Nov,
	sum(case when x.MonthNo = 12 then 1 else 0 end) Dec from 
  (
	  select n.ProjectMasterId,pm.AddedDate,pm.ProjectName, n.Message, n.AddedBy, Month(n.Added) MonthNo
	  from Notification n
	  join ProjectMasters pm
	  on n.ProjectMasterId = pm.ProjectMasterId
	   where n.AddedBy is not Null and YEAR(n.Added) = '2017'
	  group by n.ProjectMasterId, pm.ProjectName, pm.AddedDate, n.AddedBy, Month(n.Added), n.Message
  ) x
  group by x.ProjectMasterId,x.ProjectName, x.AddedDate
  order by x.AddedDate desc");

            }
            else if (projectId < 0)
            {
                query = string.Format(@"select top 5 x.ProjectMasterId, x.ProjectName,
  sum(case when x.MonthNo = 1 then 1 else 0 end) Jan,
   sum(case when x.MonthNo = 2 then 1 else 0 end) Feb,
    sum(case when x.MonthNo = 3 then 1 else 0 end) Mar,
	sum(case when x.MonthNo = 4 then 1 else 0 end) Apr,
	sum(case when x.MonthNo = 5 then 1 else 0 end) May,
	sum(case when x.MonthNo = 6 then 1 else 0 end) Jun,
	sum(case when x.MonthNo = 7 then 1 else 0 end) Jul,
	sum(case when x.MonthNo = 8 then 1 else 0 end) Aug,
	sum(case when x.MonthNo = 9 then 1 else 0 end) Sep,
	sum(case when x.MonthNo = 10 then 1 else 0 end) Oct,
	sum(case when x.MonthNo = 11 then 1 else 0 end) Nov,
	sum(case when x.MonthNo = 12 then 1 else 0 end) Dec from 
  (
	  select n.ProjectMasterId,pm.AddedDate,pm.ProjectName, n.Message, n.AddedBy, Month(n.Added) MonthNo
	  from Notification n
	  join ProjectMasters pm
	  on n.ProjectMasterId = pm.ProjectMasterId
	   where n.AddedBy is not Null and YEAR(n.Added) = '2017'
	  group by n.ProjectMasterId, pm.ProjectName, pm.AddedDate, n.AddedBy, Month(n.Added), n.Message
  ) x
  group by x.ProjectMasterId,x.ProjectName, x.AddedDate
  order by count(x.ProjectMasterId) desc");
            }
            else
            {
                query =
                    string.Format(
                        @"select x.ProjectMasterId, x.ProjectName,
  sum(case when x.MonthNo = 1 then 1 else 0 end) Jan,
   sum(case when x.MonthNo = 2 then 1 else 0 end) Feb,
    sum(case when x.MonthNo = 3 then 1 else 0 end) Mar,
	sum(case when x.MonthNo = 4 then 1 else 0 end) Apr,
	sum(case when x.MonthNo = 5 then 1 else 0 end) May,
	sum(case when x.MonthNo = 6 then 1 else 0 end) Jun,
	sum(case when x.MonthNo = 7 then 1 else 0 end) Jul,
	sum(case when x.MonthNo = 8 then 1 else 0 end) Aug,
	sum(case when x.MonthNo = 9 then 1 else 0 end) Sep,
	sum(case when x.MonthNo = 10 then 1 else 0 end) Oct,
	sum(case when x.MonthNo = 11 then 1 else 0 end) Nov,
	sum(case when x.MonthNo = 12 then 1 else 0 end) Dec from 
  (
	  select n.ProjectMasterId,pm.AddedDate,pm.ProjectName, n.Message, n.AddedBy, Month(n.Added) MonthNo
	  from Notification n
	  join ProjectMasters pm
	  on n.ProjectMasterId = pm.ProjectMasterId
	   where n.AddedBy is not Null and YEAR(n.Added) = '2017' and pm.ProjectMasterId = {0}
	  group by n.ProjectMasterId, pm.ProjectName, pm.AddedDate, n.AddedBy, Month(n.Added), n.Message
  ) x
  group by x.ProjectMasterId,x.ProjectName, x.AddedDate", projectId);
            }

            var list = _dbContext.Database.SqlQuery<YearlyWorkProgress>(query).ToList();
            var workProgress = new List<WorkProgressData>();
            foreach (var progress in list)
            {
                var data = new WorkProgressData { name = progress.ProjectName };
                data.data.Add(progress.Jan);
                data.data.Add(progress.Feb);
                data.data.Add(progress.Mar);
                data.data.Add(progress.Apr);
                data.data.Add(progress.May);
                data.data.Add(progress.Jun);
                data.data.Add(progress.Jul);
                data.data.Add(progress.Aug);
                data.data.Add(progress.Sep);
                data.data.Add(progress.Oct);
                data.data.Add(progress.Nov);
                data.data.Add(progress.Dec);
                workProgress.Add(data);
            }
            return workProgress;
        }

        public List<NotificationModel> GetRecentNotifications()
        {
            string query =
                string.Format(@"SELECT ProjectMasterId, Message, AdditionalMessage, CAST(CONVERT(VARCHAR(20), Added, 100) AS datetime2) AS Added FROM Notification
                    WHERE AddedBy IS NOT NULL AND Message NOT LIKE 'You%'
					and Added between DATEADD(MONTH, -1, GETDATE()) and GETDATE()
                    GROUP BY ProjectMasterId, Message, AdditionalMessage, CAST(CONVERT(VARCHAR(20), Added, 100) AS datetime2)
                    ORDER BY CAST(CONVERT(VARCHAR(20), Added, 100) AS datetime2) DESC");
            var notificationModels = _dbContext.Database.SqlQuery<NotificationModel>(query).ToList();
            if (notificationModels.Any())
            {
                foreach (var model in notificationModels)
                {
                    var span = (TimeSpan)(DateTime.Now - model.Added);
                    model.NotificationTime = CommonConversion.ToPrettyFormat(span);
                }
            }
            return notificationModels;
        }

        public List<NotificationModel> GetProjectWiseRecentNotifications(long projectId)
        {
            string query =
                string.Format(@"SELECT ProjectMasterId, Message, AdditionalMessage, CAST(CONVERT(VARCHAR(20), Added, 100) AS datetime2) AS Added FROM Notification
                    WHERE ProjectMasterId={0} AND AddedBy IS NOT NULL AND Message NOT LIKE 'You%'
                    GROUP BY ProjectMasterId, Message, AdditionalMessage, CAST(CONVERT(VARCHAR(20), Added, 100) AS datetime2)
                    ORDER BY CAST(CONVERT(VARCHAR(20), Added, 100) AS datetime2) DESC", projectId);
            var notificationModels = _dbContext.Database.SqlQuery<NotificationModel>(query).ToList();
            if (notificationModels.Any())
            {
                foreach (var model in notificationModels)
                {
                    var span = (TimeSpan)(DateTime.Now - model.Added);
                    model.NotificationTime = CommonConversion.ToPrettyFormat(span);
                }
            }
            return notificationModels;
        }

        public string GetProjectsCountByIssueOccured()
        {
            var issueGraphViewModel = new IssueGraphViewModel();
            List<IssueGraphModel> feeds = (from ci in _dbContext.CommonIssues

                                           group ci.ProjectMasterId by ci.ProjectMasterId
                                               into temp
                                               join pm in _dbContext.ProjectMasters on temp.Key equals pm.ProjectMasterId
                                               orderby temp.Count() descending
                                               select new IssueGraphModel { name = pm.ProjectName, y = temp.Count(), drilldown = pm.ProjectName }).Take(
                    10).ToList();
            if (!feeds.Any())
            {
                var model = new IssueGraphModel { name = "No Data Found", y = 100, drilldown = "No Data Found" };
                feeds.Add(model);
            }
            issueGraphViewModel.IssueGraphModels = feeds;
            if (feeds.Any())
            {
                foreach (var feed in feeds)
                {
                    IssueGraphModel feed1 = feed;
                    var issueData = (from master in _dbContext.ProjectMasters
                                     where master.ProjectName == feed1.name
                                     join issue in _dbContext.CommonIssues on master.ProjectMasterId equals issue.ProjectMasterId
                                     select new
                                     {
                                         refered = issue.ReferenceFlow,
                                         isSoleved = issue.IsSolved
                                     }).ToList();
                    var drillDown = new IssueGraphDrillDownModel();
                    drillDown.name = drillDown.id = feed1.name;
                    if (issueData.Any())
                    {
                        long solved = 0, unsolved = 0, forwarde = 0;

                        foreach (var d in issueData)
                        {

                            if (d.isSoleved) solved = solved + 1;
                            if (!d.isSoleved && !string.IsNullOrWhiteSpace(d.refered)) forwarde = forwarde + 1;
                            if (!d.isSoleved) unsolved = unsolved + 1;
                        }
                        var solevedDictionary = new Dictionary<string, long> { { "Solved", solved } };
                        drillDown.data.Add(solevedDictionary);
                        solevedDictionary = new Dictionary<string, long> { { "Unsolved", unsolved } };
                        drillDown.data.Add(solevedDictionary);
                        solevedDictionary = new Dictionary<string, long> { { "Forwarded", forwarde } };
                        drillDown.data.Add(solevedDictionary);
                    }
                    issueGraphViewModel.IssueGraphDrillDownModels.Add(drillDown);
                }
            }
            string jsonData = JsonConvert.SerializeObject(issueGraphViewModel);
            return jsonData;
        }

        public string GetProjectsCountByCommentOccured()
        {
            //var issueGraphViewModel = new IssueGraphViewModel();
            List<IssueGraphModel> feeds = (from opinion in _dbContext.Opinions

                                           group opinion.ProjectMasterId by opinion.ProjectMasterId
                                               into temp
                                               join pm in _dbContext.ProjectMasters on temp.Key equals pm.ProjectMasterId
                                               orderby temp.Count() descending
                                               select new IssueGraphModel { name = pm.ProjectName, y = temp.Count(), drilldown = pm.ProjectName, id = pm.ProjectMasterId }).Take(
                    10).ToList();
            if (!feeds.Any())
            {
                var model = new IssueGraphModel { name = "No Data Found", y = 100, drilldown = "No Data Found" };
                feeds.Add(model);
            }
            //issueGraphViewModel.IssueGraphModels = feeds;
            //if (feeds.Any())
            //{
            //    foreach (var feed in feeds)
            //    {
            //        IssueGraphModel feed1 = feed;
            //        var issueData = (from master in _dbContext.ProjectMasters
            //                         where master.ProjectName == feed1.name
            //                         join issue in _dbContext.Opinions on master.ProjectMasterId equals issue.ProjectMasterId
            //                         select new
            //                         {
            //                             refered = issue.ReferenceFlow,
            //                             isSoleved = issue.IsSolved
            //                         }).ToList();
            //        var drillDown = new IssueGraphDrillDownModel();
            //        drillDown.name = drillDown.id = feed1.name;
            //        if (issueData.Any())
            //        {
            //            long solved = 0, unsolved = 0, forwarde = 0;

            //            foreach (var d in issueData)
            //            {

            //                if (d.isSoleved) solved = solved + 1;
            //                else if (!d.isSoleved && !string.IsNullOrWhiteSpace(d.refered)) forwarde = forwarde + 1;
            //                else if (!d.isSoleved) unsolved = unsolved + 1;
            //            }
            //            var solevedDictionary = new Dictionary<string, long> { { "Solved", solved } };
            //            drillDown.data.Add(solevedDictionary);
            //            solevedDictionary = new Dictionary<string, long> { { "Unsolved", unsolved } };
            //            drillDown.data.Add(solevedDictionary);
            //            solevedDictionary = new Dictionary<string, long> { { "Forwarded", forwarde } };
            //            drillDown.data.Add(solevedDictionary);
            //        }
            //        issueGraphViewModel.IssueGraphDrillDownModels.Add(drillDown);
            //    }
            //}
            string jsonData = JsonConvert.SerializeObject(feeds);
            return jsonData;
        }

        public List<PieSlideDataModel> GetIssuesForManagerPieSlide(string projectName, string status)
        {
            var list = new List<PieSlideDataModel>();
            try
            {

                var projectMaster = _dbContext.ProjectMasters.FirstOrDefault(i => i.ProjectName == projectName);
                if (projectMaster != null)
                {


                    long projectId = projectMaster.ProjectMasterId;
                    if (status == "Solved")
                    {
                        list =
                            _dbContext.CommonIssues.Where(i => i.ProjectMasterId == projectId && i.IsSolved)
                                .Select(j => new PieSlideDataModel
                                {
                                    Id = j.CommonIssueId,
                                    Description = j.Description,
                                    FlagStatus = "Complete",
                                    WorkingRole = j.CurrentlyWorkingRole,
                                    AddedBy = j.Added ?? 0,
                                    Component = j.Component,
                                    AddedDate = j.AddedDate ?? new DateTime()
                                })
                                .ToList();
                    }
                    else if (status == "Unsolved")
                    {
                        list = _dbContext.CommonIssues.Where(i => i.ProjectMasterId == projectId && !i.IsSolved)
                                .Select(j => new PieSlideDataModel
                                {
                                    Id = j.CommonIssueId,
                                    Description = j.Description,
                                    FlagStatus = "Pending",
                                    WorkingRole = j.CurrentlyWorkingRole,
                                    AddedBy = j.Added ?? 0,
                                    Component = j.Component,
                                    AddedDate = j.AddedDate ?? new DateTime()
                                })
                                .ToList();
                    }
                    else if (status == "Forwarded")
                    {
                        list = _dbContext.CommonIssues.Where(i => i.ProjectMasterId == projectId && !i.IsSolved && i.ReferenceFlow != null)
                                .Select(j => new PieSlideDataModel
                                {
                                    Id = j.CommonIssueId,
                                    Description = j.Description,
                                    FlagStatus = "Forwarded",
                                    WorkingRole = j.CurrentlyWorkingRole,
                                    AddedBy = j.Added ?? 0,
                                    Component = j.Component,
                                    AddedDate = j.AddedDate ?? new DateTime()
                                })
                                .ToList();
                    }
                }

            }
            catch (Exception)
            {

                return new List<PieSlideDataModel>();
            }
            if (list.Any())
            {
                foreach (var issueModel in list)
                {
                    var manager = new FileManager();
                    var user = _dbContext.CmnUsers.FirstOrDefault(i => i.CmnUserId == issueModel.AddedBy);
                    if (user != null)
                    {
                        issueModel.CreatorName = user.UserFullName;
                        issueModel.ProfilePicture = manager.GetFile(user.ProfilePictureUrl);
                        if (string.IsNullOrWhiteSpace(issueModel.ProfilePicture))
                            issueModel.ProfilePicture = "../assets/layouts/layout4/img/av.png";
                    }
                    switch (issueModel.WorkingRole)
                    {
                        case "CM":
                            issueModel.FlagStatus = issueModel.FlagStatus + "(Commercial)";
                            break;
                        case "SW":
                            issueModel.FlagStatus = issueModel.FlagStatus + "(Software)";
                            break;
                        case "HW":
                            issueModel.FlagStatus = issueModel.FlagStatus + "(Hardware)";
                            break;
                        case "MM":
                            issueModel.FlagStatus = issueModel.FlagStatus + "(Management)";
                            break;
                        case "PM":
                            issueModel.FlagStatus = issueModel.FlagStatus + "(Project Manager)";
                            break;
                    }
                }
            }
            return list;
        }

        public long SaveFinalDecision(VmFinalApproval model)
        {
            try
            {
                long userId = Convert.ToInt64(HttpContext.Current.User.Identity.Name == "" ? "0" : HttpContext.Current.User.Identity.Name);
                ProjectMaster master = _dbContext.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == model.ProjectMasterId);
                if (master != null)
                {
                    string apprvStr = string.Empty;
                    switch (model.Status)
                    {
                        case "Approve":
                            master.ProjectStatus = "APPROVED";
                            master.ManagentComment = model.Remarks;
                            master.FinalApprovalDate = DateTime.Now;
                            apprvStr = "This is to inform you that, a project finally has been approved as a walton brand model by our Management after reviewing the screening test report. Let's get started to make this new project more powerful and successful.";
                            break;
                        case "Reject":
                            master.ProjectStatus = "REJECTED";
                            master.ManagentComment = model.Remarks;
                            apprvStr = "This is to inform you that, a new project has been rejected test by our Management after reviewing a screening test report. Hope we will complete a project with the best of best in future.";
                            break;
                        case "Review":
                            master.ProjectStatus = "PARTIAL2";
                            master.ApprovalReviewRemarks = model.Remarks;
                            var issues = _dbContext.HwInchargeIssues.Where(i => i.ProjectMasterId == model.ProjectMasterId && i.CommercialDecision != "Solvable").ToList();
                            if (issues.Any())
                            {
                                foreach (var issue in issues)
                                {
                                    var obj = new HwInchargeIssue
                                    {
                                        ProjectMasterId = issue.ProjectMasterId,
                                        HwQcInchargeAssignId = issue.HwQcInchargeAssignId,
                                        HwIssue = issue.HwIssue,
                                        HwIssueDetail = issue.HwIssueDetail,
                                        CommercialDecision = issue.CommercialDecision,
                                        Remarks = issue.Remarks,
                                        AddedDate = issue.AddedDate,
                                        AddedBy = issue.AddedBy,
                                        UpdatedDate = issue.UpdatedDate,
                                        UpdatedBy = issue.UpdatedBy

                                    };
                                    _dbContext.HwInchargeIssues.Add(obj);
                                    issue.IsReviewd = true;
                                    _dbContext.Entry(issue).State = EntityState.Modified;
                                }
                            }
                            apprvStr = "This is to inform you that, a new project has been forwarded to commercial for review the screening test issues again by management.";
                            break;
                    }
                    master.Updated = userId;
                    master.UpdatedDate = DateTime.Now;

                    _dbContext.Entry(master).State = EntityState.Modified;

                    _dbContext.SaveChanges();

                    var user = _dbContext.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);
                    var usrInfo = user != null ? "<br/>Action taken by : " + user.UserFullName : "";
                    string time = "<br/>Action taken date : " + DateTime.Now.ToLongDateString();
                    var body =
                        string.Format(
                            @"{0}<br/><br/><b>Project Name: " +
                            master.ProjectName + "</b>" + usrInfo + time, apprvStr);
                    var mail = new MailSendFromPms();
                    var result = mail.SendMail(new List<string>(new[] { "MM" }),
                        new List<string>(new[] { "CM", "PMHEAD", "QCHEAD", "HWHEAD", "SA", "PS" }), "NEW PROJECT( " + master.ProjectName + " )", body);
                }
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public List<MarketPriceModel> GetMarketPriceModels()
        {
            List<string> RBSProductModels = new List<string>();
            String connectionString = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                string resultQuery =
                    String.Format(@"select productmaster.ProductModel, productmaster.Inactive, productmaster.ProductType, productmaster.DateAdded from RBSYNERGY.dbo.tblProductMaster productmaster where productmaster.Inactive=1 and productmaster.ProductType='Cell Phone' and productmaster.DateAdded>= '2018-01-01'");
                
                var command = new SqlCommand(resultQuery, connection);

                command.CommandTimeout = 6000;
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    RBSProductModels.Add(reader["ProductModel"].ToString());
                }
                connection.Close();
            }

            string query = string.Format(@"select a.ProjectMasterId, a.ProjectName, a.ProjectModel, a.FinalPrice as ProjectMasterPrice, b.MarketPriceId, b.FinalPrice, b.Multiplier, b.Mrp, b.IsLocked, b.AddedBy, b.AddedDate, b.UpdatedBy, b.UpdateDate,a.JigsUnitPrice,a.HandsetProcessCost from
            (
	            SELECT T.ProjectMasterId, T.ProjectName, T.ProjectModel, T.OrderNuber, T.FinalPrice, T.RecentOrder, T.JigsUnitPrice,T.HandsetProcessCost
	            FROM (
	                SELECT ProjectMasterId, ProjectName,ProjectModel, OrderNuber, FinalPrice, row_number() over (partition by ProjectName order by OrderNuber desc) as RecentOrder,
					(
					  select top 1 JigsUnitPrice from ProjectPurchaseOrderForms ppf
					  inner join ProjectMasters p on ppf.ProjectMasterId=p.ProjectMasterId
					  where p.ProjectName=pm.ProjectName and p.OrderNuber=1
					) as JigsUnitPrice,
					(select top 1 ProcessCost from ProcessCostMonthWise where VariantName=pm.ProjectModel) as HandsetProcessCost 
					from ProjectMasters pm where ProjectStatus = 'APPROVED' and Isactive=1
	            ) T
	            WHERE T.RecentOrder = 1
            )a
            left join 
            (
	            SELECT x.MarketPriceId, x.ProjectMasterId, x.FinalPrice, x.Multiplier, x.Mrp, x.IsLocked,x.AddedBy, x.AddedDate, x.UpdatedBy, x.UpdateDate, x.Row
	            FROM (
	                SELECT MarketPriceId, ProjectMasterId, FinalPrice, Multiplier, Mrp, IsLocked, AddedBy, AddedDate, UpdatedBy, UpdateDate, row_number() over (partition by ProjectMasterId order by MarketPriceId desc) as Row from MarketPrices
	            ) x
	            WHERE x.Row = 1
            )b
            on a.ProjectMasterId = b.ProjectMasterId");
            var execute = _dbContext.Database.SqlQuery<MarketPriceModel>(query).ToList();

            var accessories = _dbContext.AccessoriesPrices.ToList();
            foreach (var accessory in accessories)
            {
                accessory.Price = accessory.TotalPrice ?? accessory.Price;
            }
            var config = new MapperConfiguration(c => c.CreateMap<AccessoriesPrice, AccessoriesPricesModel>()
                .ForMember(d => d.USDValue, o => o.MapFrom(m => (m.Currency != "USD" ? CommonConversion.CurrencyConversion(Decimal.Parse(m.Price), m.Currency, "USD") : Decimal.Parse(m.Price)))));
            var map = config.CreateMapper();

            List<AccessoriesPricesModel> retdata = new List<AccessoriesPricesModel>();
            retdata.AddRange(map.Map<List<AccessoriesPricesModel>>(accessories));
            var groupedaccessoryprices = (from vm in retdata
                                          group vm by vm.ProjectMasterId
                                              into gr
                                              select new AccessoriesPricesModel
                                              {
                                                  ProjectMasterId = gr.Key,
                                                  USDValue = gr.Sum(x => x.USDValue)
                                              }).ToList();

            var final = (from pn in execute
                         join rb in RBSProductModels
                         on pn.ProjectModel equals rb
                         join ac in groupedaccessoryprices
                         on pn.ProjectMasterId equals ac.ProjectMasterId
                         into gr
                         from ac in gr.DefaultIfEmpty()
                         select new MarketPriceModel { 
                         MarketPriceId=pn.MarketPriceId,
                         ProjectName=pn.ProjectName,
                         ProjectModel=pn.ProjectModel,
                         ProjectMasterId = pn.ProjectMasterId,
                         FinalPrice = pn.FinalPrice,
                         //TotalPrice = (pn.FinalPrice ?? pn.ProjectMasterPrice) + (ac != null ? ac.USDValue : 0),
                         TotalPrice = (pn.ProjectMasterPrice ?? 0) + (ac != null ? ac.USDValue : 0) + (pn.JigsUnitPrice == null ? 0 : decimal.Parse(pn.JigsUnitPrice)) + (pn.HandsetProcessCost == null ? 0 : CommonConversion.CurrencyConversion(Decimal.Parse(pn.HandsetProcessCost), "BDT", "USD")),
                         ProjectMasterPrice=pn.ProjectMasterPrice,
                         Multiplier = pn.Multiplier,
                         Mrp = pn.Mrp,
                         IsLocked = pn.IsLocked,
                         AddedBy = pn.AddedBy,
                         UpdatedBy = pn.UpdatedBy
                         }).ToList();

            return final;
        }

        public string SaveMarketPrice(int type, long projectId, decimal price, decimal mul, decimal marketPrice)
        {
            try
            {
                long userId;
                long.TryParse(HttpContext.Current.User.Identity.Name, out userId);
                var marketpricemodel = new MarketPriceModel
                {
                    ProjectMasterId = projectId,
                    FinalPrice = price,
                    Multiplier = mul,
                    Mrp = marketPrice,
                    IsLocked = type == 1,
                    AddedDate = DateTime.Now,
                    AddedBy = userId
                };
                Mapper.CreateMap<MarketPriceModel, MarketPrice>();
                var marketprice = Mapper.Map<MarketPrice>(marketpricemodel);
                _dbContext.MarketPrices.Add(marketprice);
                _dbContext.SaveChanges();
                return "ok";
            }
            catch (Exception)
            {
                return "err";
            }

        }

        public string GetLockedPrice(string pName)
        {
            string html = string.Empty;
            var prices = (from projectMaster in _dbContext.ProjectMasters
                          where projectMaster.ProjectName == pName
                          join marketPrice in _dbContext.MarketPrices on projectMaster.ProjectMasterId equals
                              marketPrice.ProjectMasterId into g
                          from marketPrice1 in g.DefaultIfEmpty()
                          where marketPrice1.IsLocked == true
                          select new
                          {
                              marketPrice1.AddedDate,
                              marketPrice1.FinalPrice,
                              projectMaster.OrderNuber
                          }).ToList();
            if (prices.Any())
            {
                html = prices.Aggregate(@"<table border='1'><th>ON.</th><th>L.Date</th><th>L.Price</th>", (current, price) => current + "<tr><td>" + CommonConversion.AddOrdinal(price.OrderNuber) + "</td><td>" + string.Format("{0:d/M/yyyy}", price.AddedDate) + "</td><td>$" + price.FinalPrice + "</td>");
                html = html + "</tr></table>";
            }
            return html;
        }

        //Gnatt chart
        public List<CmStatusObject> GetAllCmStatusObject()
        {
            string query = string.Format(@"select distinct  pm.ProjectMasterId,pm.ProjectName,pm.OrderNuber,pm.ProjectStatus,pm.SourcingType,pm.AddedDate as ProjectInitialize, pm.InitialApprovalDate, 
                                           hii.AddedDate as ScreeningIssueReview,hii.UpdatedDate as ScreeningIssueReviewDone,pof.UpdatedDate as PoClosingDate,pof.IsCompleted,
										   (select top 1 n.added from Notification n  where n.projectmasterid=pm.projectmasterid and n.message like '%{0}%' )  as ForwardForFinalApproval,
										   pm.FinalApprovalDate,pm.ApproxProjectFinishDate,
                                           pof.PoDate as PurchaseOrder from ProjectMasters pm
                                           left join HwInchargeIssues hii on pm.ProjectMasterId=hii.ProjectMasterId
                                           left join Notification n on pm.ProjectMasterId=n.ProjectMasterId
                                           left join ProjectPurchaseOrderForms pof on pm.ProjectMasterId=pof.ProjectMasterId where pm.ProjectStatus = 'APPROVED' and pof.PoDate is not null and pm.IsActive=1", "approved a project from final approval section");
            _dbContext.Database.CommandTimeout=500;
            var exe = _dbContext.Database.SqlQuery<CmStatusObject>(query).ToList();

            return exe;
        }

        public DateTime? GetLastActionDate(long projectid)
        {
            string query = string.Format(
                @"select top 1 Added as LastActionDate from Notification where ProjectMasterId={0} order by Added desc", projectid);
            
                var x= _dbContext.Database.SqlQuery<OverallProjectStatusModel>(query).FirstOrDefault();
            DateTime? exe = x != null ? Convert.ToDateTime(x.LastActionDate) : DateTime.Now;
            return exe;
        }

        public List<HwQcAssignCustomMasterModel> GetProjectForHwScreeningTestByProjectName(string projectName)
        {
            string query =
                string.Format(@"select distinct  hqa.HwQcInchargeAssignId,pm.ProjectName,pm.SupplierModelName,pm.OrderNuber,
                                STUFF((select ','+cu.UserFullName from CmnUsers cu inner join HwQcAssigns hqa on cu.CmnUserId=hqa.HwQcUserId  
                                where hqa.HwQcInchargeAssignId = hqia.HwQcInchargeAssignId for xml path(''),type ).value('.','nvarchar(max)'),1,1,'') as UserFullName,
                                hqa.VerifierName,hqia.Remark from HwQcInchargeAssigns hqia
                                inner join HwQcAssigns hqa  on hqia.HwQcInchargeAssignId = hqa.HwQcInchargeAssignId
                                inner join ProjectMasters pm on hqia.ProjectMasterId = pm.ProjectMasterId
                                inner join CmnUsers cu on hqa.HwQcUserId=cu.CmnUserId 
                                where hqia.IsScreeningTest=1 and pm.ProjectName='{0}' and hqia.TestPhase='FINISHED'", projectName);
            var exe = _dbContext.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return exe;
        }

        public List<HwQcAssignCustomMasterModel> GetProjectForHwRunningTestByProjectName(string projectName)
        {
            string query =
                string.Format(@"select distinct  hqa.HwQcInchargeAssignId,pm.ProjectName,pm.SupplierModelName,pm.OrderNuber,
                                STUFF((select ','+cu.UserFullName from CmnUsers cu inner join HwQcAssigns hqa on cu.CmnUserId=hqa.HwQcUserId  
                                where hqa.HwQcInchargeAssignId = hqia.HwQcInchargeAssignId for xml path(''),type ).value('.','nvarchar(max)'),1,1,'') as UserFullName,
                                hqa.VerifierName,hqia.Remark from HwQcInchargeAssigns hqia
                                inner join HwQcAssigns hqa  on hqia.HwQcInchargeAssignId = hqa.HwQcInchargeAssignId
                                inner join ProjectMasters pm on hqia.ProjectMasterId = pm.ProjectMasterId
                                inner join CmnUsers cu on hqa.HwQcUserId=cu.CmnUserId 
                                where hqia.IsRunningTest=1 and pm.ProjectName='{0}' and hqia.TestPhase='FINISHED'", projectName);
            var exe = _dbContext.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return exe;
        }

        public List<HwQcAssignCustomMasterModel> GetProjectForHwFinishedTestByProjectName(string projectName)
        {
            string query =
                string.Format(@"select distinct  hqa.HwQcInchargeAssignId,pm.ProjectName,pm.SupplierModelName,pm.OrderNuber,
                                STUFF((select ','+cu.UserFullName from CmnUsers cu inner join HwQcAssigns hqa on cu.CmnUserId=hqa.HwQcUserId  
                                where hqa.HwQcInchargeAssignId = hqia.HwQcInchargeAssignId for xml path(''),type ).value('.','nvarchar(max)'),1,1,'') as UserFullName,
                                hqa.VerifierName,hqia.Remark from HwQcInchargeAssigns hqia
                                inner join HwQcAssigns hqa  on hqia.HwQcInchargeAssignId = hqa.HwQcInchargeAssignId
                                inner join ProjectMasters pm on hqia.ProjectMasterId = pm.ProjectMasterId
                                inner join CmnUsers cu on hqa.HwQcUserId=cu.CmnUserId 
                                where hqia.IsFinishedGoodTest=1 and pm.ProjectName='{0}' and hqia.TestPhase='FINISHED'", projectName);
            var exe = _dbContext.Database.SqlQuery<HwQcAssignCustomMasterModel>(query).ToList();
            return exe;
        }

        public List<SwQcInchargeAssignModel> GetSwQcInchargeAssignByProjectName(string projectName)
        {
            var getSwQcInchargeAssign = new List<SwQcInchargeAssign>();
            var projectid =
                _dbContext.ProjectMasters.Where(x => x.ProjectName == projectName)
                    .Select(x => x.ProjectMasterId)
                    .ToList();
            foreach (var i in projectid)
            {
                getSwQcInchargeAssign = _dbContext.SwQcInchargeAssigns.Where(x => x.ProjectMasterId == i && x.Status == "RECOMMENDED").ToList();
                if (getSwQcInchargeAssign != null && getSwQcInchargeAssign.Count > 0)
                {

                    Mapper.Initialize(cfg => cfg.CreateMap<SwQcInchargeAssign, SwQcInchargeAssignModel>());
                    List<SwQcInchargeAssignModel> inchargeAssign = Mapper.Map<List<SwQcInchargeAssign>, List<SwQcInchargeAssignModel>>(getSwQcInchargeAssign);
                    var orderNumber = _dbContext.ProjectMasters.Where(x => x.ProjectMasterId == i)
                    .Select(x => x.OrderNuber)
                    .FirstOrDefault();
                    foreach (SwQcInchargeAssignModel t in inchargeAssign)
                    {
                        t.OrderNuber = orderNumber;
                    }
                    return inchargeAssign;
                }
            }



            return new List<SwQcInchargeAssignModel>();
        }

        public List<ProjectMasterWithPoCustomModel> SpareOrderStatus()
        {
            DateTime date = Convert.ToDateTime("2017-08-10 00:00:00.0000000");
            var spareOrderStatus =_dbContext.ProjectPurchaseOrderForms
                .Join(_dbContext.ProjectMasters,
                ppf=>ppf.ProjectMasterId,
                pm=>pm.ProjectMasterId,
                (ppf,pm)=>new{PPF=ppf,PM=pm})
                .Where(ppfAndPm => ppfAndPm.PM.AddedDate > date)
                .Select(x => new ProjectMasterWithPoCustomModel
                {
                   ProjectMasterId = x.PM.ProjectMasterId,
                   ProjectName = x.PM.ProjectName,
                   OrderNuber = x.PM.OrderNuber,
                   PoDate = x.PPF.PoDate,
                   IsSpareSubmittedDate = x.PPF.IsSpareSubmittedDate
                }).OrderByDescending(x=>x.PoDate).ToList();
            return spareOrderStatus;
        }
        public List<AccessoriesPricesModel> GetAccessoriesPrices(long projectId)
        {
            var project = _dbContext.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == projectId);
            var accessories = _dbContext.AccessoriesPrices.Where(x => x.ProjectMasterId == projectId).ToList();
            foreach (var accessory in accessories)
            {
                accessory.Price = accessory.TotalPrice ?? accessory.Price;
            }
            var order = (from po in _dbContext.ProjectPurchaseOrderForms
                join pm in _dbContext.ProjectMasters on po.ProjectMasterId equals pm.ProjectMasterId
                where pm.ProjectName == project.ProjectName && pm.OrderNuber == 1
                select new {po, pm}).FirstOrDefault();
            var handsetProcessCost = (from p in _dbContext.ProcessCostMonthWises
                join pm in _dbContext.ProjectMasters on p.VariantName equals pm.ProjectModel
                where
                    p.VariantName ==
                    (_dbContext.ProjectMasters.Where(x => x.ProjectMasterId == projectId)
                        .Select(x => x.ProjectModel)
                        .FirstOrDefault())
                select new {p, pm}).FirstOrDefault();
            var config = new MapperConfiguration(c => c.CreateMap<AccessoriesPrice, AccessoriesPricesModel>()
                .ForMember(d => d.USDValue, o => o.MapFrom(m => (m.Currency != "USD" ? CommonConversion.CurrencyConversion(Decimal.Parse(m.Price), m.Currency, "USD") : Decimal.Parse(m.Price)))));
            var map = config.CreateMapper();
          
            List<AccessoriesPricesModel> retdata = new List<AccessoriesPricesModel>();
            var handset = new AccessoriesPricesModel
            {
                AccessoryName = "Handset",
                Price = project.FinalPrice.ToString(),
                Currency = "USD",
                USDValue = (decimal)project.FinalPrice
            };
            if (order != null && order.po.JigsUnitPrice!=null)
            {
                var jigs = new AccessoriesPricesModel
                {
                    AccessoryName = "Jigs",
                    Price = order.po.JigsUnitPrice,
                    Currency = "USD",
                    USDValue = Convert.ToDecimal(order.po.JigsUnitPrice)
                };
                retdata.Add(jigs);
            }
            //===Handset Process Cost====
            if (handsetProcessCost != null && handsetProcessCost.p != null)
            {
                var proCost = new AccessoriesPricesModel
                {
                    AccessoryName = "Handset Process Cost",
                    Price = handsetProcessCost.p.ProcessCost,
                    Currency = "BDT",
                    USDValue = CommonConversion.CurrencyConversion(Decimal.Parse(handsetProcessCost.p.ProcessCost), "BDT", "USD")
                };
                retdata.Add(proCost);
            }
            retdata.Add(handset);
            retdata.AddRange(map.Map<List<AccessoriesPricesModel>>(accessories));
            return retdata;
        }

        public void ApproveRepeatOrder(long orderId)
        {
            long userId = Convert.ToInt64(HttpContext.Current.User.Identity.Name == "" ? "0" : HttpContext.Current.User.Identity.Name);
            var model = _dbContext.ProjectPurchaseOrderForms.FirstOrDefault(x => x.ProjectPurchaseOrderFormId == orderId);
            if (model != null)
            {
                model.RepeatOrderApproved = "APPROVED";
                model.ApprovedBy = userId;
                model.ApprovedDate = DateTime.Now;
            }
            _dbContext.ProjectPurchaseOrderForms.AddOrUpdate(model);
            _dbContext.SaveChanges();
        }

        public List<ProjectPoFeedbackModel> GetNegativeSourcingPoFeedbacks()
        {
            var model =
                _dbContext.ProjectPoFeedbacks.Where(x => x.SourcingAllowReorder == "No")
                    .Select(x => new ProjectPoFeedbackModel
                    {
                        Id = x.Id,
                        ProjectId = x.ProjectId,
                        ProjectModel = x.ProjectModel,
                        OrderNumber = x.OrderNumber,
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
                            _dbContext.CmnUsers.Where(v => v.CmnUserId == x.AddedBy)
                                .Select(v => v.UserFullName)
                                .FirstOrDefault(),
                        Department = (from u in _dbContext.CmnUsers
                                      join r in _dbContext.CmnRoles on u.RoleName equals r.RoleName
                                      where u.CmnUserId == x.AddedBy
                                      select r.RoleDescription).FirstOrDefault(),
                        SourcingCommentByName = _dbContext.CmnUsers.Where(v => v.CmnUserId == x.SourcingCommentBy)
                            .Select(v => v.UserFullName)
                            .FirstOrDefault(),
                            ManagementComment = x.ManagementComment,
                            ManagementCommentBy = x.ManagementCommentBy,
                            ManagementCommentDate = x.ManagementCommentDate,
                            ManagementDecision = x.ManagementDecision
                    }).ToList();
            return model;
        }

        public string SaveManagementDecision(string manCom, string manDec, long id)
        {
            long userId = Convert.ToInt64(HttpContext.Current.User.Identity.Name == "" ? "0" : HttpContext.Current.User.Identity.Name);
            try
            {
                var model = _dbContext.ProjectPoFeedbacks.FirstOrDefault(x => x.Id == id);
                if (model != null)
                {
                    model.ManagementComment = manCom;
                    model.ManagementDecision = manDec;
                    model.ManagementCommentDate = DateTime.Now;
                    model.ManagementCommentBy = userId;
                    _dbContext.ProjectPoFeedbacks.AddOrUpdate(model);
                    _dbContext.SaveChanges();
                    //log
                    ProjectPoFeedbackLog log =
                        GenericMapper<ProjectPoFeedback, ProjectPoFeedbackLog>.GetDestination(model);
                    _dbContext.ProjectPoFeedbackLogs.Add(log);
                    _dbContext.SaveChanges();
                    return "Success";
                }
            }
            catch (Exception ex)
            { 
                return ex.ToString();
            }
            return "No action";
        }
    }

    public class LockedPrice
    {
        public DateTime? AddedDate { get; set; }
        public decimal? FinalPrice { get; set; }
    }


}