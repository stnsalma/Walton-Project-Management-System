using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Web;
using Antlr.Runtime.Misc;
using AutoMapper;
using ProjectManagement.DAL;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Models;
using ProjectManagement.ViewModels.AftersalesPm;
using ProjectManagement.ViewModels.ProjectManager;
using SignalRDemo.DAL;

namespace ProjectManagement.Infrastructures.Repositories
{
    public class AfterSalesPmRepository : IAfterSalesPmRepository
    {
        private readonly CellPhoneProjectEntities _dbeEntities;

        public AfterSalesPmRepository()
        {
            _dbeEntities = new CellPhoneProjectEntities();
            _dbeEntities.Configuration.LazyLoadingEnabled = false;
        }

        public ProjectMasterModel GetProjectMasterModel(long projectid)
        {
            var projectRepository = new Repository<ProjectMaster>(_dbeEntities);
            ProjectMaster projectMaster = projectRepository.Get(projectid);

            var config = new MapperConfiguration(cfg => cfg.CreateMap<ProjectMaster, ProjectMasterModel>());
            var mapper = config.CreateMapper();
            var projectMasterModel = mapper.Map<ProjectMasterModel>(projectMaster);
            return projectMasterModel;

        }

        public List<CmnUserModel> GetAftersalesPmUserList()
        {
            var models = new List<CmnUserModel>();
            var userLists = (from cmnUsers in _dbeEntities.CmnUsers
                             where (cmnUsers.RoleName == "ASPMHEAD" || cmnUsers.RoleName == "ASPM") && cmnUsers.IsActive == true
                             select new CmnUserModel
                             {
                                 UserFullName = cmnUsers.UserFullName,
                                 UserName = cmnUsers.UserName,
                                 EmployeeCode = cmnUsers.EmployeeCode,
                                 RoleName = cmnUsers.RoleName
                             }).ToList();

            models = userLists;
            return models;
        }

        public List<Pm_Incentive_BaseModel> GetAftersalesPmIncentiveBase()
        {
            var models = new List<Pm_Incentive_BaseModel>();

            var insBase = (from pmBase in _dbeEntities.Pm_Incentive_Base
                           where pmBase.ActiveRole == 2
                           select new Pm_Incentive_BaseModel
                           {
                               IncentiveName = pmBase.IncentiveName,
                               Amount = pmBase.Amount,
                               Id = pmBase.Id
                           }).ToList();
            models = insBase;

            return models;
        }

        public List<ProjectMasterModel> GetProjectMasterListForAftersalesPmIncentive(string employeeCode)
        {
            var nocList = (from pm in _dbeEntities.ProjectMasters
                           select new ProjectMasterModel
                           {
                               ProjectMasterId = pm.ProjectMasterId,
                               ProjectName = pm.ProjectName,
                               OrderNuber = pm.OrderNuber,
                           }).ToList();

            //foreach (var project in nocList)
            //{
            //    project.OrderNumberOrdinal = project.OrderNuber != null
            //        ? CommonConversion.AddOrdinal((int)project.OrderNuber) + " Order"
            //        : string.Empty;
            //    if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
            //    {
            //        project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
            //    }
            //}
            return nocList;
        }

        public List<ProjectMasterModel> GetProjectMasterListForAftersalesPmFoc()
        {
            var nocList = (from pm in _dbeEntities.ProjectMasters
                           select new ProjectMasterModel
                           {
                               ProjectMasterId = pm.ProjectMasterId,
                               ProjectName = pm.ProjectName,
                               OrderNuber = pm.OrderNuber,
                               ProjectType = pm.ProjectType
                           }).ToList();

            foreach (var project in nocList)
            {
                project.OrderNumberOrdinal = project.OrderNuber != null
                    ? CommonConversion.AddOrdinal((int)project.OrderNuber) + " Order"
                    : string.Empty;
                if (!string.IsNullOrWhiteSpace(project.OrderNumberOrdinal))
                {
                    project.ProjectName = project.ProjectName + " (" + project.OrderNumberOrdinal + ")";
                }
            }
            return nocList;
        }

        public List<SpareNameModel> GetSpareNameForAftersalesPm(string projectType)
        {
            List<SpareNameModel> nocList = new ListStack<SpareNameModel>();

            if (projectType == "Smart")
            {
                nocList = (from sp in _dbeEntities.SpareNames
                           where sp.SpareType == "smart"
                           select new SpareNameModel
                           {
                               SparePartsName = sp.SparePartsName,
                               SpareType = sp.SpareType,
                               SpareId = sp.SpareId
                           }).ToList();
            }
            else if (projectType == "Feature")
            {
                nocList = (from sp in _dbeEntities.SpareNames
                           where sp.SpareType == "feature"
                           select new SpareNameModel
                           {
                               SparePartsName = sp.SparePartsName,
                               SpareType = sp.SpareType,
                               SpareId = sp.SpareId
                           }).ToList();
            }
            return nocList;
        }

        public ProjectMasterModel GetSupplierForAftersalesPm(long projectId)
        {
            var nocList = (from sp in _dbeEntities.ProjectMasters
                           where sp.ProjectMasterId == projectId
                           select new ProjectMasterModel
                           {
                               SupplierName = sp.SupplierName
                           }).FirstOrDefault();

            return nocList;
        }

        public string SaveFocForAftersalesPm(List<VmAftersalesPmFoc> results)
        {
            String userIdentity =
        System.Web.HttpContext.Current.User.Identity.Name; ;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);


            foreach (var result in results)
            {
                var proDetails =
               (from pm in _dbeEntities.ProjectMasters where pm.ProjectMasterId == result.ProjectId select pm)
                   .FirstOrDefault();
                var userSDetails = (from cm in _dbeEntities.CmnUsers where cm.EmployeeCode == result.EmployeeCode select cm)
                        .FirstOrDefault();
                var poDetails = (from cm in _dbeEntities.ProjectPurchaseOrderForms where cm.ProjectMasterId == result.ProjectId select cm)
                        .FirstOrDefault();

                long spareIds;
                long.TryParse(result.SparePartsName, out spareIds);

                var spars = (from cm in _dbeEntities.SpareNames where cm.SpareId == spareIds select cm)
                        .FirstOrDefault();

                var createModel = new CreateFocForAftersalesPm();
                createModel.ProjectId = result.ProjectId;
                createModel.ProjectName = proDetails.ProjectName;
                createModel.SpareName = spars.SparePartsName;
                createModel.OrderNumber = proDetails.OrderNuber;

                if (poDetails != null)
                {
                    createModel.PoDate = poDetails.PoDate;
                }

                createModel.PoCategory = proDetails.SourcingType;
                createModel.EmployeeCode = result.EmployeeCode;
                createModel.AsmUserId = userSDetails.CmnUserId;
                createModel.Supplier = result.SupplierName;
                createModel.Remarks = result.Remarks;
                createModel.FocConfirmedDate = result.FocConfirmedDate;
                createModel.Quantity = result.Quantity;
                createModel.Added = userId;
                createModel.AddedDate = DateTime.Now;

                _dbeEntities.CreateFocForAftersalesPms.Add(createModel);
                _dbeEntities.SaveChanges();

            }
            return "OK";
        }

        //public string SaveFocForAftersalesPm(VmAftersalesPmFoc model)
        //{
        //    String userIdentity =
        //  System.Web.HttpContext.Current.User.Identity.Name; ;
        //    long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

        //    var proDetails =
        //        (from pm in _dbeEntities.ProjectMasters where pm.ProjectMasterId == model.ProjectMasterId select pm)
        //            .FirstOrDefault();
        //    var userSDetails = (from cm in _dbeEntities.CmnUsers where cm.EmployeeCode == model.EmployeeCode select cm)
        //            .FirstOrDefault();
        //    var poDetails = (from cm in _dbeEntities.ProjectPurchaseOrderForms where cm.ProjectMasterId == model.ProjectMasterId select cm)
        //            .FirstOrDefault();

        //    long spareIds;
        //    long.TryParse(model.SparePartsName, out spareIds);

        //    var spars = (from cm in _dbeEntities.SpareNames where cm.SpareId == spareIds select cm)
        //            .FirstOrDefault();

        //    var createModel = new CreateFocForAftersalesPm();
        //    createModel.ProjectId = model.ProjectMasterId;
        //    createModel.ProjectName = proDetails.ProjectName;
        //    createModel.SpareName = spars.SparePartsName;
        //    createModel.OrderNumber = proDetails.OrderNuber;

        //    if (poDetails !=null)
        //    {
        //        createModel.PoDate = poDetails.PoDate;
        //    }

        //    createModel.PoCategory = proDetails.SourcingType;
        //    createModel.EmployeeCode = model.EmployeeCode;
        //    createModel.AsmUserId = userSDetails.CmnUserId;
        //    createModel.Supplier = model.SupplierName;
        //    createModel.Remarks = model.Remarks;
        //    createModel.FocConfirmedDate = model.FocConfirmedDate;
        //    createModel.Quantity = model.Quantity;
        //    createModel.Added = userId;
        //    createModel.AddedDate = DateTime.Now;

        //    _dbeEntities.CreateFocForAftersalesPms.Add(createModel);
        //    _dbeEntities.SaveChanges();

        //    return "OK";
        //}

        public List<CreateFocForAftersalesPmModel> GetFocForAftersalesPm()
        {
            var nocList = _dbeEntities.Database.SqlQuery<CreateFocForAftersalesPmModel>(@"select top 100 * from  [CellPhoneProject].[dbo].[CreateFocForAftersalesPm]
                order by Id desc").Take(100).ToList();

            return nocList;
        }

        public bool CheckDuplicateFoc(List<VmAftersalesPmFoc> focDatas)
        {
            List<VmAftersalesPmFoc> getIncentiveReports = null;

            foreach (var focDatass in focDatas)
            {
                if (focDatass.EmployeeCode != null)
                {
                    long spareIds;
                    long.TryParse(focDatass.SparePartsName, out spareIds);

                    var spars = (from cm in _dbeEntities.SpareNames where cm.SpareId == spareIds select cm)
                            .FirstOrDefault();


                    string tempDate = String.Format("{0:yyyy-MM-dd}", focDatass.FocConfirmedDate);

                    string getIncentiveReportQuery = string.Format(@"select * from CellPhoneProject.dbo.CreateFocForAftersalesPm where 
                ProjectId='" + focDatass.ProjectId + "' and EmployeeCode='" + focDatass.EmployeeCode + "' and SpareName='" + spars.SparePartsName + "' and Supplier='" + focDatass.SupplierName + "' and FocConfirmedDate='" + tempDate + "' and Quantity='" + focDatass.Quantity + "'");
                    getIncentiveReports =
                       _dbeEntities.Database.SqlQuery<VmAftersalesPmFoc>(getIncentiveReportQuery).ToList();

                }
            }

            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public List<CreateFocForAftersalesPmModel> GetFocDataForAllIncentive(string monthName, string monNum, string year, string employeeCode)
        {
            var asmHead =
                (from asm in _dbeEntities.CmnUsers where asm.RoleName == "ASPMHEAD" && asm.IsActive == true select asm).FirstOrDefault();

            string query =
               string.Format(@"select * from CellPhoneProject.dbo.CreateFocForAftersalesPm where EmployeeCode !='{3}' and  DATEPART(year, InventoryEntryDate)='{2}'
                and  DATEPART(MONTH, InventoryEntryDate)='{1}'", monthName, monNum, year, asmHead.EmployeeCode);

            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<CreateFocForAftersalesPmModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<CreateFocForAftersalesPmModel> GetFocDataForPmHeadIncentive(string monthName, string monNum, string year, string employeeCode)
        {
            List<CreateFocForAftersalesPmModel> getPmPoIncentiveModel = new List<CreateFocForAftersalesPmModel>();

            var asmHead =
               (from asm in _dbeEntities.CmnUsers where asm.RoleName == "ASPMHEAD" && asm.IsActive == true select asm).FirstOrDefault();


            if (employeeCode == asmHead.EmployeeCode)
            {
                string query =
            string.Format(@"select * from CellPhoneProject.dbo.CreateFocForAftersalesPm where EmployeeCode='{3}' and  DATEPART(year, InventoryEntryDate)='{2}'
                and  DATEPART(MONTH, InventoryEntryDate)='{1}' order by EmployeeCode desc", monthName, monNum, year, employeeCode);

                getPmPoIncentiveModel =
                   _dbeEntities.Database.SqlQuery<CreateFocForAftersalesPmModel>(query).ToList();
            }
            return getPmPoIncentiveModel;
        }

        public string SaveAftersalesPmMonthlyIncentive(List<Custom_Pm_IncentiveModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var insResult in results)
            {
                var asmId = (from asm in _dbeEntities.CmnUsers where asm.EmployeeCode == insResult.EmployeeCode select asm).FirstOrDefault();
                var insType = (from asm in _dbeEntities.Pm_Incentive_Base where asm.Id == insResult.Pm_Incentive_Base_Id select asm).FirstOrDefault();

                var model = new AftersalesPm_Incentive();

                model.AsmUserId = asmId.CmnUserId;
                model.EmployeeCode = insResult.EmployeeCode;
                model.Pm_Incentive_Base_Id = Convert.ToInt64(insResult.Pm_Incentive_Base_Id);
                model.IncentiveTypes = insType.IncentiveName;
                model.ProjectId = Convert.ToInt64(insResult.ProjectId);
                model.ProjectName = insResult.ProjectName;
                model.Amount = Convert.ToDecimal(insResult.Amount);
                model.Remarks = insResult.Remarks;
                model.DeductionAmount = Convert.ToDecimal(insResult.DeductionAmount);
                model.D_Remarks = insResult.D_Remarks;
                model.FinalAmount = Convert.ToDecimal(insResult.FinalAmount);
                model.Month = insResult.Month;
                model.MonNum = Convert.ToInt32(insResult.MonNum);
                model.Year = Convert.ToInt64(insResult.Year);
                model.DepartmentName = "AftersalesPM";
                model.Added = userId;
                model.AddedDate = DateTime.Now;

                _dbeEntities.AftersalesPm_Incentive.AddOrUpdate(model);
            }
            _dbeEntities.SaveChanges();

            return "ok";
        }

        public string SaveOthersIncentive(List<Custom_Pm_IncentiveModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var inResults in results)
            {
                var asmId = (from asm in _dbeEntities.CmnUsers where asm.EmployeeCode == inResults.EmployeeCode select asm).FirstOrDefault();

                var model = new AftersalesPm_Incentive();
                model.AsmUserId = asmId.CmnUserId;
                model.EmployeeCode = inResults.EmployeeCode;
                model.OthersIncentiveName = inResults.OthersIncentiveName;
                model.ProjectId = inResults.ProjectId;
                model.ProjectName = inResults.ProjectName;
                model.Month = inResults.Month;
                model.MonNum = inResults.MonNum;
                model.Year = !string.IsNullOrWhiteSpace(inResults.Year) ? Convert.ToInt64(inResults.Year) : 0;
                model.FinalAmount = !string.IsNullOrWhiteSpace(inResults.FinalAmount) ? Convert.ToDecimal(inResults.FinalAmount) : 0;
                model.Amount = !string.IsNullOrWhiteSpace(inResults.Amount) ? Convert.ToDecimal(inResults.Amount) : 0;
                model.Remarks = inResults.Remarks;
                model.DeductionAmount = !string.IsNullOrWhiteSpace(inResults.DeductionAmount) ? Convert.ToDecimal(inResults.DeductionAmount) : 0;
                model.D_Remarks = inResults.D_Remarks;
                model.DepartmentName = "AftersalesPM";
                model.Added = userId;
                model.AddedDate = DateTime.Now;
                _dbeEntities.AftersalesPm_Incentive.AddOrUpdate(model);
            }
            _dbeEntities.SaveChanges();

            return "ok";
        }

        public string SaveFocForHeadDetails(List<Custom_Pm_IncentiveModel> results)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var inResults in results)
            {
                var asmId = (from asm in _dbeEntities.CmnUsers where asm.EmployeeCode == inResults.EmployeeCode select asm).FirstOrDefault();

                var model = new AftersalesPm_FocIncentive();
                model.CreateFocId = inResults.Id;
                model.AsmUserId = asmId.CmnUserId;
                model.EmployeeCode = inResults.EmployeeCode;
                model.ProjectId = inResults.ProjectId;
                model.ProjectName = inResults.ProjectName;
                model.SpareName = inResults.SpareName;
                model.Amount = !string.IsNullOrWhiteSpace(inResults.Amount) ? Convert.ToDecimal(inResults.Amount) : 0;
                model.IncentiveRemarks = inResults.IncentiveRemarks;

                if (inResults.EmployeeCode == "16271")
                {
                    model.FinalAmountForHead = !string.IsNullOrWhiteSpace(inResults.FinalAmountForHead) ? Convert.ToDecimal(inResults.FinalAmountForHead) : 0;
                    // model.FinalAmountForOthers = !string.IsNullOrWhiteSpace(inResults.FinalAmountForOthers) ? Convert.ToDecimal(inResults.FinalAmountForOthers) : 0;              
                }
                //else 
                //{
                //    model.FinalAmountForOthers = !string.IsNullOrWhiteSpace(inResults.FinalAmountForOthers) ? Convert.ToDecimal(inResults.FinalAmountForOthers) : 0;

                //}
                model.DeductionAmount = !string.IsNullOrWhiteSpace(inResults.DeductionAmount) ? Convert.ToDecimal(inResults.DeductionAmount) : 0;
                model.D_Remarks = inResults.D_Remarks;

                model.Month = inResults.Month;
                model.MonNum = inResults.MonNum;
                model.Year = !string.IsNullOrWhiteSpace(inResults.Year) ? Convert.ToInt64(inResults.Year) : 0;
                model.DepartmentName = "AftersalesPM";
                model.Added = userId;
                model.AddedDate = DateTime.Now;
                _dbeEntities.AftersalesPm_FocIncentive.AddOrUpdate(model);
            }
            _dbeEntities.SaveChanges();
            return "ok";
        }

        public string SaveFocForAllDetails(List<Custom_Pm_IncentiveModel> results, string employeeCode)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            foreach (var inResults in results)
            {
                var asmId = (from asm in _dbeEntities.CmnUsers where asm.EmployeeCode == inResults.EmployeeCode select asm).FirstOrDefault();

                var model = new AftersalesPm_FocIncentive();
                model.CreateFocId = inResults.Id;
                model.AsmUserId = asmId.CmnUserId;
                model.EmployeeCode = inResults.EmployeeCode;
                model.ProjectId = inResults.ProjectId;
                model.ProjectName = inResults.ProjectName;
                model.SpareName = inResults.SpareName;
                model.Amount = !string.IsNullOrWhiteSpace(inResults.Amount) ? Convert.ToDecimal(inResults.Amount) : 0;
                model.IncentiveRemarks = inResults.IncentiveRemarks;

                if (employeeCode == "16271")
                {
                    model.FinalAmountForHead = !string.IsNullOrWhiteSpace(inResults.FinalAmountForOthers) ? Convert.ToDecimal(inResults.FinalAmountForOthers) : 0;

                }
                else
                {
                    model.FinalAmountForOthers = !string.IsNullOrWhiteSpace(inResults.FinalAmountForOthers) ? Convert.ToDecimal(inResults.FinalAmountForOthers) : 0;

                }
                model.DeductionAmount = !string.IsNullOrWhiteSpace(inResults.DeductionAmount) ? Convert.ToDecimal(inResults.DeductionAmount) : 0;
                model.D_Remarks = inResults.D_Remarks;

                model.Month = inResults.Month;
                model.MonNum = inResults.MonNum;
                model.Year = !string.IsNullOrWhiteSpace(inResults.Year) ? Convert.ToInt64(inResults.Year) : 0;
                model.DepartmentName = "AftersalesPM";
                model.Added = userId;
                model.AddedDate = DateTime.Now;
                _dbeEntities.AftersalesPm_FocIncentive.AddOrUpdate(model);
            }
            _dbeEntities.SaveChanges();
            return "ok";
        }
        public bool GetIncentiveTypeData(string employeeCode, int monNum, string year)
        {
            int MonNum = Convert.ToInt32(monNum);
            List<Custom_Pm_IncentiveModel> getIncentiveReports = null;
            if (MonNum > 0 && year != null)
            {
                string getIncentiveReportQuery = string.Format(@"
             select MonNum,CONVERT(varchar(10),Year),EmployeeCode from [CellPhoneProject].[dbo].AftersalesPm_Incentive where EmployeeCode='{2}' and Year='{1}' and  MonNum='{0}' ", MonNum, year, employeeCode);
                getIncentiveReports =
                   _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(getIncentiveReportQuery).ToList();

            }

            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }
        public bool GetIncentiveTypeDataForOthers(string employeeCode, int monNum, string year)
        {
            int MonNum = Convert.ToInt32(monNum);
            List<Custom_Pm_IncentiveModel> getIncentiveReports = null;
            if (MonNum > 0 && year != null)
            {
                string getIncentiveReportQuery = string.Format(@"
             select MonNum,CONVERT(varchar(10),Year),EmployeeCode from [CellPhoneProject].[dbo].AftersalesPm_Incentive where EmployeeCode='{2}' and Year='{1}' and  MonNum='{0}' and OthersIncentiveName is not null ", MonNum, year, employeeCode);
                getIncentiveReports =
                   _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(getIncentiveReportQuery).ToList();

            }

            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }
        public bool GetFocDataForHead(string employeeCode, int monNum, string year)
        {
            int MonNum = Convert.ToInt32(monNum);
            List<Custom_Pm_IncentiveModel> getIncentiveReports = null;
            if (MonNum > 0 && year != null)
            {
                string getIncentiveReportQuery = string.Format(@"
             select MonNum,CONVERT(varchar(10),Year),EmployeeCode from [CellPhoneProject].[dbo].AftersalesPm_FocIncentive where EmployeeCode='{2}' and Year='{1}' and  MonNum='{0}' ", MonNum, year, employeeCode);
                getIncentiveReports =
                   _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(getIncentiveReportQuery).ToList();

            }

            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public bool GetFocDataForAll(string employeeCode, int monNum, string year)
        {
            int MonNum = Convert.ToInt32(monNum);
            List<Custom_Pm_IncentiveModel> getIncentiveReports = null;

            //       var asmHead =
            //(from asm in _dbeEntities.CmnUsers where asm.RoleName == "ASPMHEAD" && asm.IsActive == true select asm).FirstOrDefault();

            if (MonNum > 0 && year != null && employeeCode == "16271")
            {
                string getIncentiveReportQuery = string.Format(@"
             select MonNum,CONVERT(varchar(10),Year),EmployeeCode from [CellPhoneProject].[dbo].AftersalesPm_FocIncentive where EmployeeCode not in ('16271') and FinalAmountForHead is not null and Year='{1}' and  MonNum='{0}' ", MonNum, year, employeeCode);
                getIncentiveReports =
                   _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(getIncentiveReportQuery).ToList();

            }
            else if (MonNum > 0 && year != null && employeeCode != "16271")
            {
                string getIncentiveReportQuery = string.Format(@"
             select MonNum,CONVERT(varchar(10),Year),EmployeeCode from [CellPhoneProject].[dbo].AftersalesPm_FocIncentive where FinalAmountForOthers is not null and EmployeeCode='{2}' and Year='{1}' and  MonNum='{0}' ", MonNum, year, employeeCode);
                getIncentiveReports =
                   _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(getIncentiveReportQuery).ToList();
            }

            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public List<Custom_Pm_IncentiveModel> GetAftersalesPmIncentiveType(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
                       string.Format(@"select case when sum(FinalAmount) is null then '0' else  sum(FinalAmount) end as FinalAmount1 from [CellPhoneProject].[dbo].[AftersalesPm_Incentive] pmi
                    where pmi.EmployeeCode='{0}' and pmi.Year='{2}' and pmi.MonNum='{1}' ", empCode, monNums, years);

            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();

            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetAftersalesPm_FocIncentiveForHead(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
                       string.Format(@"select case when sum(FinalAmountForHead) is null then '0' else  sum(FinalAmountForHead) end as FinalAmount1 from [CellPhoneProject].[dbo].[AftersalesPm_FocIncentive] pmi
                    where pmi.EmployeeCode='{0}' and pmi.Year='{2}' and pmi.MonNum='{1}' ", empCode, monNums, years);

            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();

            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetAftersalesPm_FocIncentiveForHeadFromOthers(string empCode, string monNum, string year)
        {
            var asmHead =
        (from asm in _dbeEntities.CmnUsers where asm.RoleName == "ASPMHEAD" && asm.IsActive == true select asm).FirstOrDefault();

            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
                       string.Format(@"select EmployeeCode,case when sum(FinalAmountForHead) is null then '0' else  sum(FinalAmountForHead) end as FinalAmount1  from [CellPhoneProject].[dbo].[AftersalesPm_FocIncentive] pmi
                    where pmi.EmployeeCode !='{3}' and FinalAmountForHead is not null and pmi.Year='{2}' and pmi.MonNum='{1}' group by EmployeeCode ", empCode, monNums, years, asmHead.EmployeeCode);

            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();

            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetAftersalesPm_FocIncentiveForOthers(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
                       string.Format(@"select case when sum(FinalAmountForOthers) is null then '0' else  sum(FinalAmountForOthers) end as FinalAmount1 from [CellPhoneProject].[dbo].[AftersalesPm_FocIncentive] pmi
                    where pmi.EmployeeCode='{0}' and FinalAmountForOthers is not null and pmi.Year='{2}' and pmi.MonNum='{1}' ", empCode, monNums, years);

            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();

            return getPmPoIncentiveModel;
        }

        public string SaveTotalAftersalesPmIncentive(string totalAmount, string empCode, string month, string monNum, string year, decimal totalIncentiveType, decimal totalAmountForFoc)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);
            var model = new Incentive();
            model.UserId = empCode;
            model.TotalAmount = Convert.ToDecimal(totalAmount);
            model.ThisMonthAmount = totalIncentiveType;
            model.Amount = totalAmountForFoc;
            model.Month = month;
            model.MonNum = Convert.ToInt32(monNum);
            model.Year = Convert.ToInt64(year);

            model.TotalIncentive = Convert.ToDecimal(totalAmount);
            model.DepartmentName = "AftersalesPM";
            model.Added = userId;
            model.AddedDate = DateTime.Now;

            _dbeEntities.Incentives.AddOrUpdate(model);

            _dbeEntities.SaveChanges();
            return "ok";
        }

        public bool GetTotalIncentiveDataForDuplicateCheck(string empCode, int monNum, string year)
        {
            int MonNum = Convert.ToInt32(monNum);
            List<Custom_Pm_IncentiveModel> getIncentiveReports = null;
            if (MonNum > 0 && year != null)
            {
                string getIncentiveReportQuery = string.Format(@"
             select MonNum,CONVERT(varchar(10),Year),UserId from [CellPhoneProject].[dbo].[Incentive] where UserId='{2}' and Year='{1}' and  MonNum='{0}' ", MonNum, year, empCode);
                getIncentiveReports =
                   _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(getIncentiveReportQuery).ToList();


            }

            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }

        public List<CreateFocForAftersalesPmModel> GetFocDataForParticularPerson(string monthName, string monNum, string year, string employeeCode)
        {
            var asmHead =
                (from asm in _dbeEntities.CmnUsers where asm.RoleName == "ASPMHEAD" && asm.IsActive == true select asm).FirstOrDefault();

            string query =
               string.Format(@"select * from CellPhoneProject.dbo.CreateFocForAftersalesPm where EmployeeCode !='{3}' and EmployeeCode='{4}'  and  DATEPART(year, InventoryEntryDate)='{2}'
                and  DATEPART(MONTH, InventoryEntryDate)='{1}'", monthName, monNum, year, asmHead.EmployeeCode, employeeCode);

            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<CreateFocForAftersalesPmModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetAftersalesPmIncentiveForPrint(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
                       string.Format(@"SELECT EmployeeCode,SpareName='-------',case when IncentiveTypes is null then OthersIncentiveName else IncentiveTypes end as IncentiveTypes,
                    ProjectName,Amount as Amount1,Remarks,DeductionAmount as DeductionAmount1,D_Remarks,FinalAmount as FinalAmount1
                    FROM [CellPhoneProject].[dbo].[AftersalesPm_Incentive] pmi where pmi.EmployeeCode='{0}' and pmi.Year='{2}' and pmi.MonNum='{1}' ", empCode, monNums, years);

            var getPmPoIncentiveModel =
            _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();

            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetAftersalesPmFocIncentiveForPrint(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);
            var asmHead =
        (from asm in _dbeEntities.CmnUsers where asm.RoleName == "ASPMHEAD" && asm.IsActive == true select asm).FirstOrDefault();
            string query;
            if (empCode == asmHead.EmployeeCode)
            {
                query =
                    string.Format(@"select EmployeeCode,SpareName,IncentiveTypes='FOC FOR HEAD',
                        ProjectName,Amount as Amount1,IncentiveRemarks as Remarks,DeductionAmount as DeductionAmount1,D_Remarks,
                        FinalAmountForHead as FinalAmount1 FROM [CellPhoneProject].[dbo].[AftersalesPm_FocIncentive] pmi 
                        where FinalAmountForHead is not null and pmi.EmployeeCode='{0}' and pmi.Year='{2}' and pmi.MonNum='{1}' ",
                        empCode, monNums, years);
            }
            else
            {
                query =
                  string.Format(@"select EmployeeCode,SpareName,IncentiveTypes='FOC',
                        ProjectName,Amount as Amount1,IncentiveRemarks as Remarks,DeductionAmount as DeductionAmount1,D_Remarks,
                        FinalAmountForOthers as FinalAmount1 FROM [CellPhoneProject].[dbo].[AftersalesPm_FocIncentive] pmi
                        where FinalAmountForOthers is not null and pmi.EmployeeCode='{0}' and pmi.Year='{2}' and pmi.MonNum='{1}' ", empCode, monNums, years);

            }
            var getPmPoIncentiveModel =
            _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();

            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> GetPreparedUserName()
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            string getIncentiveReportQuery = string.Format(@"select UserFullName,EmployeeCode  FROM [CellPhoneProject].[dbo].CmnUsers where CmnUserId={0}", userId);
            var getIncentiveReports =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(getIncentiveReportQuery).ToList();
            return getIncentiveReports;
        }

        public List<Custom_Pm_IncentiveModel> GetTotalFinalIncentiveOfAftersalesPm(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
             string.Format(@"select TotalIncentive as FinalAmount1 from CellPhoneProject.dbo.Incentive
                where UserId='{0}' and Year='{2}' and MonNum='{1}' ", empCode, monNums, years);


            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<Custom_Pm_IncentiveModel> AftersalesPmIncentiveForAllPerson(string month, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
             string.Format(@"select cmu.UserFullName,cmu.EmployeeCode,inc.Month,inc.Year as Year1,inc.TotalIncentive from CellPhoneProject.dbo.Incentive inc
                join CellPhoneProject.dbo.CmnUsers cmu on inc.UserId=cmu.EmployeeCode
                 where inc.DepartmentName='AftersalesPM' and inc.MonNum='{0}' and inc.Year='{1}' ", monNums, years);

            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<Custom_Pm_IncentiveModel>(query).ToList();
            return getPmPoIncentiveModel;
        }
        #region New Incentive Policy
        public List<VmAftersalesIncentive> GetAftersalesIssueDetails(string monYear)
        {
            var getPmPoIncentiveModel = new List<VmAftersalesIncentive>();
            if (monYear != null && monYear != "")
            {
                var currentDate1 = monYear.Split(',');
                var currentMonth = currentDate1[0].Trim();
                var currentYear = currentDate1[1].Trim();
                int currentYear1 = Convert.ToInt32(currentYear);
                int currentMonNum1 = DateTime.ParseExact(currentMonth, "MMMM", CultureInfo.CurrentCulture).Month;

                //                string query =
                //                string.Format(@"
                //            select distinct C.GeneralIncidentId,cast(Incentive as decimal(18,2)) as Incentive,cast(Incentive1 as decimal(18,2)) as Incentive1,cast(DaysPassed as int) as DaysPassed,ModelName as ProjectName,
                //            C.GeneralIncidentTitle,C.GeneralIncidentCategories,C.GeneralIncidentDetails,
                //            Issues,Status,cast(AssignedTo as bigint) as AssignedTo,RaiseDate,AssignRemarks as AssignedRemarks,AssignedPerson,C.EmployeeCode,Solution,SolutionGivenBy,SolutionDate,
                //            case when aii.GeneralIncidentId is not null then 1 else 0 end as GeneralCount
                //             from
                //            (
                //	            select case when Incentive1<0 then 0 else Incentive1 end as Incentive,* from
                //	            (
                //		            select case
                //		            when A.GeneralIncidentCategories='Hardware' and DaysPassed<=14 then 8000+(100 * (14-DaysPassed))
                //		            when A.GeneralIncidentCategories='Hardware' and DaysPassed>14 then 8000-((DaysPassed-14)*100)
                //		            when A.GeneralIncidentCategories='Software' and DaysPassed<=14 then 8000+(100*(14-DaysPassed))
                //		            when A.GeneralIncidentCategories='Software' and (DaysPassed>14 and DaysPassed<=30) then 8000
                //		            when A.GeneralIncidentCategories='Software' and DaysPassed>30 then 8000-((DaysPassed-30)*100)
                //		            when A.GeneralIncidentCategories='Improvement Software' then 2500
                //
                //		            end as Incentive1,* from
                //		            (
                //			            select gi.GeneralIncidentId,DATEDIFF(day, ga.AssignDate, gss.AddedDate)+1 AS DaysPassed,gi.ModelName,gi.GeneralIncidentTitle,gi.GeneralIncidentCategories,
                //			            gi.GeneralIncidentDetails,gi.Issues,gi.Status,ga.AssignedTo,ga.AssignDate as RaiseDate,ga.AssignRemarks,
                //			            cu.UserFullName as AssignedPerson,cu.EmployeeCode,gss.Solution,gss.AddedByName as SolutionGivenBy,gss.AddedDate as SolutionDate
                //			            from [CellPhoneProject].[dbo].[GeneralIncidents] gi
                //
                //			            left join [CellPhoneProject].[dbo].[GeneralIncidentAssigns] ga on ga.GeneralIncidentId=gi.GeneralIncidentId
                //			            left join [CellPhoneProject].[dbo].CmnUsers cu on cu.CmnUserId=ga.AssignedTo
                //			            left join [CellPhoneProject].[dbo].[GeneralIncidentSolutions] gss on gss.GeneralIncidentId=gi.GeneralIncidentId
                //
                //			            where gi.Status in ('SUBMITTED','DISCLOSE') and RoleName in ('ASPM','ASPMHEAD','CPSDHEAD') and MONTH(gss.AddedDate)='{0}' and YEAR(gss.AddedDate)='{1}'
                //		            )A
                //	            )B
                //            )C 
                //            left join [CellPhoneProject].[dbo].AftersalesPm_Issue_Percentage aii on aii.GeneralIncidentId=C.GeneralIncidentId", currentMonNum1, currentYear1);
                string query =
               string.Format(@"select (DaysPassed-Holiday) as DaysPassed,CONVERT(DATETIME, RaiseDate, 102) as RaiseDate,CONVERT(DATETIME, SolutionDate, 102) as SolutionDate,GeneralIncidentId,Incentive,Incentive1,ProjectName,GeneralIncidentTitle,GeneralIncidentCategories,
GeneralIncidentDetails,Issues,Status,AssignedTo,AssignedRemarks,AssignedPerson,EmployeeCode,Solution,SolutionGivenBy,GeneralCount

 from
(select distinct 
case when aff.Holiday_SDate between C.RaiseDate and C.SolutionDate then  
(select count (*)  from CellPhoneProject.dbo.AftersalesPm_Holiday aff1 where  aff1.Holiday_SDate between C.RaiseDate and C.SolutionDate)
else 0 end as Holiday,  
RaiseDate,SolutionDate,
C.GeneralIncidentId,cast(Incentive as decimal(18,2)) as Incentive,cast(Incentive1 as decimal(18,2)) as Incentive1,cast(DaysPassed as int) as DaysPassed,ModelName as ProjectName,
C.GeneralIncidentTitle,C.GeneralIncidentCategories,C.GeneralIncidentDetails,
Issues,Status,cast(AssignedTo as bigint) as AssignedTo,AssignRemarks as AssignedRemarks,AssignedPerson,C.EmployeeCode,Solution,SolutionGivenBy,
case when aii.GeneralIncidentId is not null then 1 else 0 end as GeneralCount			
             
			from
            (
	            select case when Incentive1<0 then 0 else Incentive1 end as Incentive,* from
	            (
		            select case
		            when A.GeneralIncidentCategories='Hardware' and DaysPassed<=14 then 8000+(100 * (14-DaysPassed))
		            when A.GeneralIncidentCategories='Hardware' and DaysPassed>14 then 8000-((DaysPassed-14)*100)
		            when A.GeneralIncidentCategories='Software' and DaysPassed<=14 then 8000+(100*(14-DaysPassed))
		            when A.GeneralIncidentCategories='Software' and (DaysPassed>14 and DaysPassed<=30) then 8000
		            when A.GeneralIncidentCategories='Software' and DaysPassed>30 then 8000-((DaysPassed-30)*100)
		            when A.GeneralIncidentCategories='Improvement Software' then 2500
		            end as Incentive1,*				
					
					from
		            (
			            select gi.GeneralIncidentId,DATEDIFF(day, ga.AssignDate, gss.AddedDate)+1 AS DaysPassed,gi.ModelName,gi.GeneralIncidentTitle,gi.GeneralIncidentCategories,
			            gi.GeneralIncidentDetails,gi.Issues,gi.Status,ga.AssignedTo,convert(varchar, ga.AssignDate, 23) as RaiseDate,ga.AssignRemarks,
			            cu.UserFullName as AssignedPerson,cu.EmployeeCode,gss.Solution,gss.AddedByName as SolutionGivenBy,
						convert(varchar, gss.AddedDate, 23) as SolutionDate

			            from [CellPhoneProject].[dbo].[GeneralIncidents] gi
			            left join [CellPhoneProject].[dbo].[GeneralIncidentAssigns] ga on ga.GeneralIncidentId=gi.GeneralIncidentId
			            left join [CellPhoneProject].[dbo].CmnUsers cu on cu.CmnUserId=ga.AssignedTo
			            left join [CellPhoneProject].[dbo].[GeneralIncidentSolutions] gss on gss.GeneralIncidentId=gi.GeneralIncidentId
						
			            where gi.Status in ('SUBMITTED','DISCLOSE') and RoleName in ('ASPM','ASPMHEAD','CPSDHEAD') and MONTH(gss.AddedDate)='{0}' and YEAR(gss.AddedDate)='{1}'
		            )A 
	            )B
            )C 
            left join [CellPhoneProject].[dbo].AftersalesPm_Issue_Percentage aii on aii.GeneralIncidentId=C.GeneralIncidentId
			left join CellPhoneProject.dbo.AftersalesPm_Holiday aff on aff.Holiday_SDate between C.RaiseDate and C.SolutionDate
		)D", currentMonNum1, currentYear1);

                getPmPoIncentiveModel =
                   _dbeEntities.Database.SqlQuery<VmAftersalesIncentive>(query).ToList();

            }
            return getPmPoIncentiveModel;
        }

        public List<VmAftersalesIncentive> GetAftersalesIssueDetails1(string ids)
        {
            var getPmPoIncentiveModel = new List<VmAftersalesIncentive>();
            if (ids != null)
            {
                long id1 = 0;
                long.TryParse(ids, out id1);

                //                string query = string.Format(@"
                //            select GeneralIncidentId,cast(Incentive as decimal(18,2)) as Incentive,cast(Incentive1 as decimal(18,2)) as Incentive1,cast(DaysPassed as int) as DaysPassed,ModelName as ProjectName,
                //            GeneralIncidentTitle,GeneralIncidentCategories,GeneralIncidentDetails,
                //            Issues,Status,cast(AssignedTo as bigint) as AssignedTo,RaiseDate,AssignRemarks,AssignedPerson,EmployeeCode,Solution,SolutionGivenBy,SolutionDate
                //             from
                //            (
                //	            select case when Incentive1<0 then 0 else Incentive1 end as Incentive,* from
                //	            (
                //		            select case
                //		            when A.GeneralIncidentCategories='Hardware' and DaysPassed<=14 then 8000+(100 * (14-DaysPassed))
                //		            when A.GeneralIncidentCategories='Hardware' and DaysPassed>14 then 8000-((DaysPassed-14)*100)
                //		            when A.GeneralIncidentCategories='Software' and DaysPassed<=14 then 8000+(100*(14-DaysPassed))
                //		            when A.GeneralIncidentCategories='Software' and (DaysPassed>14 and DaysPassed<=30) then 8000
                //		            when A.GeneralIncidentCategories='Software' and DaysPassed>30 then 8000-((DaysPassed-30)*100)
                //		            when A.GeneralIncidentCategories='Improvement Software' then 2500
                //
                //		            end as Incentive1,* from
                //		            (
                //			            select gi.GeneralIncidentId,DATEDIFF(day, ga.AssignDate, gss.AddedDate) AS DaysPassed,gi.ModelName,gi.GeneralIncidentTitle,gi.GeneralIncidentCategories,
                //			            gi.GeneralIncidentDetails,gi.Issues,gi.Status,ga.AssignedTo,ga.AssignDate as RaiseDate,ga.AssignRemarks,
                //			            cu.UserFullName as AssignedPerson,cu.EmployeeCode,gss.Solution,gss.AddedByName as SolutionGivenBy,gss.AddedDate as SolutionDate
                //			            from [CellPhoneProject].[dbo].[GeneralIncidents] gi
                //
                //			            left join [CellPhoneProject].[dbo].[GeneralIncidentAssigns] ga on ga.GeneralIncidentId=gi.GeneralIncidentId
                //			            left join [CellPhoneProject].[dbo].CmnUsers cu on cu.CmnUserId=ga.AssignedTo
                //			            left join [CellPhoneProject].[dbo].[GeneralIncidentSolutions] gss on gss.GeneralIncidentId=gi.GeneralIncidentId
                //
                //			            where gi.Status in ('SUBMITTED','DISCLOSE') and RoleName in ('ASPM','ASPMHEAD','CPSDHEAD') and gi.GeneralIncidentId='{0}'
                //		            )A
                //	            )B
                //            )C", id1);

                string query = string.Format(@"select (DaysPassed-Holiday) as DaysPassed, GeneralIncidentId,Incentive,Incentive1,ProjectName,GeneralIncidentTitle,GeneralIncidentCategories,GeneralIncidentDetails,
Issues,Status,AssignedTo,AssignRemarks,AssignedPerson,EmployeeCode,Solution,SolutionGivenBy,CONVERT(DATETIME, RaiseDate, 102) as RaiseDate,CONVERT(DATETIME, SolutionDate, 102) as SolutionDate from 
(select distinct
case when aff.Holiday_SDate between C.RaiseDate and C.SolutionDate then  
(select count (*)  from CellPhoneProject.dbo.AftersalesPm_Holiday aff1 where  aff1.Holiday_SDate between C.RaiseDate and C.SolutionDate)
else 0 end as Holiday,  
GeneralIncidentId,cast(Incentive as decimal(18,2)) as Incentive,cast(Incentive1 as decimal(18,2)) as Incentive1,cast(DaysPassed as int) as DaysPassed,ModelName as ProjectName,
GeneralIncidentTitle,GeneralIncidentCategories,GeneralIncidentDetails,
Issues,Status,cast(AssignedTo as bigint) as AssignedTo,RaiseDate,AssignRemarks,AssignedPerson,EmployeeCode,Solution,SolutionGivenBy,SolutionDate
             from
            (
	            select case when Incentive1<0 then 0 else Incentive1 end as Incentive,* from
	            (
		            select case
		            when A.GeneralIncidentCategories='Hardware' and DaysPassed<=14 then 8000+(100 * (14-DaysPassed))
		            when A.GeneralIncidentCategories='Hardware' and DaysPassed>14 then 8000-((DaysPassed-14)*100)
		            when A.GeneralIncidentCategories='Software' and DaysPassed<=14 then 8000+(100*(14-DaysPassed))
		            when A.GeneralIncidentCategories='Software' and (DaysPassed>14 and DaysPassed<=30) then 8000
		            when A.GeneralIncidentCategories='Software' and DaysPassed>30 then 8000-((DaysPassed-30)*100)
		            when A.GeneralIncidentCategories='Improvement Software' then 2500

		            end as Incentive1,* from
		            (
			            select gi.GeneralIncidentId,DATEDIFF(day, ga.AssignDate, gss.AddedDate)+1 AS DaysPassed,gi.ModelName,gi.GeneralIncidentTitle,gi.GeneralIncidentCategories,
			            gi.GeneralIncidentDetails,gi.Issues,gi.Status,ga.AssignedTo,
						convert(varchar, ga.AssignDate, 23) as RaiseDate,ga.AssignRemarks,
			            cu.UserFullName as AssignedPerson,cu.EmployeeCode,gss.Solution,gss.AddedByName as SolutionGivenBy,
						convert(varchar, gss.AddedDate, 23) as SolutionDate					
			            from [CellPhoneProject].[dbo].[GeneralIncidents] gi

			            left join [CellPhoneProject].[dbo].[GeneralIncidentAssigns] ga on ga.GeneralIncidentId=gi.GeneralIncidentId
			            left join [CellPhoneProject].[dbo].CmnUsers cu on cu.CmnUserId=ga.AssignedTo
			            left join [CellPhoneProject].[dbo].[GeneralIncidentSolutions] gss on gss.GeneralIncidentId=gi.GeneralIncidentId

			            where gi.Status in ('SUBMITTED','DISCLOSE') and RoleName in ('ASPM','ASPMHEAD','CPSDHEAD') and gi.GeneralIncidentId='{0}'
		            )A
	            )B
            )C
			left join CellPhoneProject.dbo.AftersalesPm_Holiday aff on aff.Holiday_SDate between C.RaiseDate and C.SolutionDate

		)D", id1);
                getPmPoIncentiveModel =
                   _dbeEntities.Database.SqlQuery<VmAftersalesIncentive>(query).ToList();

            }
            return getPmPoIncentiveModel;
        }

        public List<VmAftersalesIncentive> GetAftersaleUsers(long genIds, string month, long year)
        {
            // select distinct cu.UserFullName,cu.EmployeeCode,cu.RoleName,ap.GeneralIncidentId,ap.IssuesIncentiveId,ap.EmpName,ap.TotalAmount,ap.Percentage,ap.PerPersonAmount,ap.IncentiveRemarks from [CellPhoneProject].[dbo].CmnUsers cu
            //left join [CellPhoneProject].[dbo].AftersalesPm_Issue_Percentage ap 
            //on cu.EmployeeCode=ap.EmployeeCode and ap.GeneralIncidentId=20040 and ap.MonNum=3 and ap.Year=2020
            // where RoleName in ('ASPM','ASPMHEAD','CPSDHEAD') AND IsActive=1 

            int monNum = DateTime.ParseExact(month, "MMMM", CultureInfo.CurrentCulture).Month;

            string users = String.Format(@"select distinct cu.UserFullName,cu.EmployeeCode,cu.RoleName,
            case when ap.GeneralIncidentId is null then 0 else ap.GeneralIncidentId end as GeneralIncidentId,
            case when ap.IssuesIncentiveId is null then 0 else ap.IssuesIncentiveId end as IssuesIncentiveId,ap.EmpName,
            case when ap.TotalAmount is null then 0 else ap.TotalAmount end as TotalAmount,
            case when ap.Percentage is null then 0 else ap.Percentage end as Percentage,
            case when ap.PerPersonAmount is null then 0 else ap.PerPersonAmount end as PerPersonAmount,
            ap.IncentiveRemarks from [CellPhoneProject].[dbo].CmnUsers cu
            left join [CellPhoneProject].[dbo].AftersalesPm_Issue_Percentage ap 
            on cu.EmployeeCode=ap.EmployeeCode and ap.GeneralIncidentId='{0}' and ap.MonNum='{1}' and ap.Year='{2}'
            where RoleName in ('ASPM','ASPMHEAD','CPSDHEAD') AND IsActive=1  ", genIds, monNum, year);

            var usersReturn = _dbeEntities.Database.SqlQuery<VmAftersalesIncentive>(users).ToList();

            return usersReturn;
        }
        public bool CheckIssueIncentiveData(int monNum, long year, long genIds)
        {
            int MonNum = Convert.ToInt32(monNum);
            List<VmAftersalesIncentive> getIncentiveReports = null;
            if (MonNum > 0 && year != null)
            {
                string getIncentiveReportQuery = string.Format(@"select GeneralIncidentId from [CellPhoneProject].[dbo].[AftersalesPm_Issue_Incentive] where GeneralIncidentId='{0}' ", genIds);
                getIncentiveReports = _dbeEntities.Database.SqlQuery<VmAftersalesIncentive>(getIncentiveReportQuery).ToList();

            }
            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }
        public string SaveAftersalesPercentageData(List<VmAftersalesIncentive> results, long genIds)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            string query = string.Format(@"
            select GeneralIncidentId,cast(Incentive as decimal(18,2)) as Incentive,cast(Incentive1 as decimal(18,2)) as Incentive1,cast(DaysPassed as int) as DaysPassed,ModelName as ProjectName,
            GeneralIncidentTitle,GeneralIncidentCategories,GeneralIncidentDetails,
            Issues,Status,cast(AssignedTo as bigint) as AssignedTo,RaiseDate,AssignRemarks as AssignedRemarks,AssignedPerson,EmployeeCode,Solution,SolutionGivenBy,SolutionDate
             from
            (
	            select case when Incentive1<0 then 0 else Incentive1 end as Incentive,* from
	            (
		            select case
		            when A.GeneralIncidentCategories='Hardware' and DaysPassed<=14 then 8000+(100 * (14-DaysPassed))
		            when A.GeneralIncidentCategories='Hardware' and DaysPassed>14 then 8000-((DaysPassed-14)*100)
		            when A.GeneralIncidentCategories='Software' and DaysPassed<=14 then 8000+(100*(14-DaysPassed))
		            when A.GeneralIncidentCategories='Software' and (DaysPassed>14 and DaysPassed<=30) then 8000
		            when A.GeneralIncidentCategories='Software' and DaysPassed>30 then 8000-((DaysPassed-30)*100)
		            when A.GeneralIncidentCategories='Improvement Software' then 2500

		            end as Incentive1,* from
		            (
			            select gi.GeneralIncidentId,DATEDIFF(day, ga.AssignDate, gss.AddedDate) AS DaysPassed,gi.ModelName,gi.GeneralIncidentTitle,gi.GeneralIncidentCategories,
			            gi.GeneralIncidentDetails,gi.Issues,gi.Status,ga.AssignedTo,ga.AssignDate as RaiseDate,ga.AssignRemarks,
			            cu.UserFullName as AssignedPerson,cu.EmployeeCode,gss.Solution,gss.AddedByName as SolutionGivenBy,gss.AddedDate as SolutionDate
			            from [CellPhoneProject].[dbo].[GeneralIncidents] gi

			            left join [CellPhoneProject].[dbo].[GeneralIncidentAssigns] ga on ga.GeneralIncidentId=gi.GeneralIncidentId
			            left join [CellPhoneProject].[dbo].CmnUsers cu on cu.CmnUserId=ga.AssignedTo
			            left join [CellPhoneProject].[dbo].[GeneralIncidentSolutions] gss on gss.GeneralIncidentId=gi.GeneralIncidentId

			            where gi.Status in ('SUBMITTED','DISCLOSE') and RoleName in ('ASPM','ASPMHEAD','CPSDHEAD') and cu.IsActive=1 and gi.GeneralIncidentId='{0}'
		            )A
	            )B
            )C", genIds);

            long cc = 0;

            var qq =
                  _dbeEntities.Database.SqlQuery<VmAftersalesIncentive>(query).ToList();

            int monNum = DateTime.ParseExact(results[0].Month, "MMMM", CultureInfo.CurrentCulture).Month;

            var yYears = Convert.ToInt64(results[0].Year);


            bool isCheck = CheckIssueIncentiveData(monNum, yYears, genIds);

            if (isCheck == false)
            {
                var models = new AftersalesPm_Issue_Incentive();
                models.GeneralIncidentId = qq[0].GeneralIncidentId;
                models.GeneralIncidentTitle = qq[0].GeneralIncidentTitle;
                models.GeneralIncidentCategories = qq[0].GeneralIncidentCategories;
                models.GeneralIncidentDetails = qq[0].GeneralIncidentDetails;
                models.ProjectName = qq[0].ProjectName;
                models.Status = qq[0].Status;
                models.IssueDetails = qq[0].IssueDetails;
                models.AssignedTo = qq[0].AssignedTo;
                models.IssueRaisedDate = qq[0].RaiseDate;
                models.AssignedRemarks = qq[0].AssignedRemarks;
                models.AssignedPerson = qq[0].AssignedPerson;
                models.EmployeeCode = qq[0].EmployeeCode;
                models.Solution = qq[0].Solution;
                models.SolutionDate = qq[0].SolutionDate;
                models.DaysPassed = qq[0].DaysPassed;
                models.Incentive = qq[0].Incentive;
                models.TotalAmount = qq[0].Incentive;
                models.Remarks = qq[0].Remarks;
                models.Month = results[0].Month;
                models.MonNum = monNum;
                models.Year = results[0].Year;
                models.AddedDate = DateTime.Now;
                models.Added = userId;

                _dbeEntities.AftersalesPm_Issue_Incentive.Add(models);
                _dbeEntities.SaveChanges();
                cc = models.Id;

                var kk = cc;

                var qq2 =
                           (from ss in _dbeEntities.AftersalesPm_Issue_Incentive where ss.Id == kk select ss)
                               .FirstOrDefault();
                if (kk != 0)
                {
                    foreach (var mod in results)
                    {
                        var model2 = new AftersalesPm_Issue_Percentage();
                        model2.IssuesIncentiveId = qq2.Id;
                        model2.GeneralIncidentId = genIds;
                        model2.GeneralIncidentTitle = qq2.GeneralIncidentTitle;
                        model2.TotalAmount = qq[0].Incentive;

                        model2.EmpName = mod.UserFullName;
                        model2.EmployeeCode = mod.EmployeeCode;
                        if (mod.Percentage == null)
                        {
                            model2.Percentage = 0;
                        }
                        else
                        {
                            model2.Percentage = mod.Percentage;
                        }
                        if (mod.PerPersonAmount == null)
                        {
                            model2.PerPersonAmount = 0;
                        }
                        else
                        {
                            model2.PerPersonAmount = mod.PerPersonAmount;
                        }

                        model2.IncentiveRemarks = mod.IncentiveRemarks;

                        model2.Month = qq2.Month;
                        model2.MonNum = qq2.MonNum;
                        model2.Year = qq2.Year;

                        model2.Added = userId;
                        model2.AddedDate = DateTime.Now;

                        _dbeEntities.AftersalesPm_Issue_Percentage.Add(model2);
                        _dbeEntities.SaveChanges();
                    }
                }
            }
            else if (isCheck == true)
            {
                var queries1 = (from ss in _dbeEntities.AftersalesPm_Issue_Incentive where ss.GeneralIncidentId == genIds select ss)
                               .FirstOrDefault();

                if (queries1 != null)
                {
                    var queries2 = (from ss1 in _dbeEntities.AftersalesPm_Issue_Percentage where ss1.IssuesIncentiveId == queries1.Id select ss1)
                              .ToList();

                    foreach (var qq2 in queries2)
                    {
                        foreach (var res in results)
                        {
                            if (qq2.EmployeeCode == res.EmployeeCode && qq2.MonNum == monNum && qq2.Year == res.Year)
                            {
                                qq2.TotalAmount = res.PerPersonAmount;
                                qq2.PerPersonAmount = res.PerPersonAmount;
                                qq2.Percentage = res.Percentage;
                                qq2.IncentiveRemarks = res.IncentiveRemarks;

                                qq2.Updated = userId;
                                qq2.UpdatedDate = DateTime.Now;

                                _dbeEntities.AftersalesPm_Issue_Percentage.AddOrUpdate(qq2);
                                _dbeEntities.SaveChanges();
                            }
                        }
                    }
                }
            }

            return "OK";
        }

        public List<VmAftersalesIncentive> ShowTeamIncentive(int monNum, int yYear)
        {
            string query = String.Format(@"select EmpName,EmployeeCode,sum(PerPersonAmount) as Incentive,Month,cast(Year as int) as Year,MonNum 
              from [CellPhoneProject].[dbo].[AftersalesPm_Issue_Percentage] where MonNum='{0}' and Year='{1}'
              group by EmpName,EmployeeCode,Month,Year,MonNum", monNum, yYear);

            var queryList = _dbeEntities.Database.SqlQuery<VmAftersalesIncentive>(query).ToList();

            return queryList;
        }

        public List<VmAftersalesIncentive> GetAftersalesPmIncentivePerPerson(string empCode, string monNum, string year)
        {
            int mon;
            int.TryParse(monNum, out mon);

            long years;
            long.TryParse(year, out years);

            string query = String.Format(@"select aii.GeneralIncidentTitle,aii.GeneralIncidentCategories,aii.GeneralIncidentDetails,aii.IssueRaisedDate,aii.SolutionDate,aii.DaysPassed,
            aip.EmpName,aip.EmployeeCode,aip.Percentage,aip.TotalAmount,aip.PerPersonAmount,aip.IncentiveRemarks
            from [CellPhoneProject].[dbo].[AftersalesPm_Issue_Incentive] aii left join
            [CellPhoneProject].[dbo].[AftersalesPm_Issue_Percentage] aip on aii.Id=aip.IssuesIncentiveId
            where aip.MonNum='{0}' and aip.Year='{1}' and aip.EmployeeCode='{2}'", mon, years, empCode);

            var queryList = _dbeEntities.Database.SqlQuery<VmAftersalesIncentive>(query).ToList();

            return queryList;
        }

        public List<VmAftersalesIncentive> GetTotalFinalIncentiveOfPerPm(string empCode, string monNum, string year)
        {
            long years = 0;
            long.TryParse(year, out years);
            int monNums = 0;
            int.TryParse(monNum, out monNums);

            string query =
             string.Format(@"select sum(PerPersonAmount) as PerPersonAmount FROM [CellPhoneProject].[dbo].[AftersalesPm_Issue_Percentage] aip
             where aip.MonNum='{1}' and aip.Year='{2}' and aip.EmployeeCode='{0}' ", empCode, monNums, years);

            var getPmPoIncentiveModel =
                _dbeEntities.Database.SqlQuery<VmAftersalesIncentive>(query).ToList();
            return getPmPoIncentiveModel;
        }

        public List<GovernmentHolidayTableModel> GetHoliday()
        {
            var query = _dbeEntities.Database.SqlQuery<GovernmentHolidayTableModel>(@"
             select * from [CellPhoneProject].[dbo].[AftersalesPm_Holiday]").ToList();
            return query;
        }
        public bool GetExistedHolidayData(DateTime sDate)
        {
            List<AftersalesPm_HolidayModel> getIncentiveReports = null;
            var dDate = Convert.ToDateTime(sDate);

            if (dDate != null)
            {
                string getIncentiveReportQuery = string.Format(@"select top 1 Holiday_SDate from [CellPhoneProject].[dbo].[AftersalesPm_Holiday]
                where Holiday_SDate ='" + dDate + "'  ");
                getIncentiveReports =
                   _dbeEntities.Database.SqlQuery<AftersalesPm_HolidayModel>(getIncentiveReportQuery).ToList();

            }
            if (getIncentiveReports != null && getIncentiveReports.Count != 0)
            {
                return true;
            }
            return false;
        }
        public string SaveHolidayNewData(string id, string governmentHoliday, string holidayStartDate, string holidayEndDate)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            DateTime start = DateTime.Parse(holidayStartDate);
            DateTime end = DateTime.Parse(holidayEndDate);

            bool isSaveCheck = false;
            if (holidayStartDate != holidayEndDate)
            {
                for (DateTime counter = start; counter <= end; counter = counter.AddDays(1))
                {

                    isSaveCheck = GetExistedHolidayData(counter);

                    if (isSaveCheck == false)
                    {
                        var models = new AftersalesPm_Holiday();
                        models.HolidayName = governmentHoliday;
                        models.Holiday_SDate = Convert.ToDateTime(counter);
                        // models.Holiday_EDate = Convert.ToDateTime(holidayEndDate);               
                        models.Added = userId;
                        models.AddedDate = DateTime.Now;

                        _dbeEntities.AftersalesPm_Holiday.Add(models);
                        _dbeEntities.SaveChanges();
                    }
                    else if (isSaveCheck == true)
                    {
                        long ids;
                        long.TryParse(id, out ids);

                        counter = Convert.ToDateTime(counter);
                        var model = _dbeEntities.AftersalesPm_Holiday.FirstOrDefault(x => x.Holiday_SDate == counter);
                        model.HolidayName = governmentHoliday;
                        model.Holiday_SDate = counter;
                        //model.Holiday_EDate = Convert.ToDateTime(holidayEndDate);
                        model.Updated = userId;
                        model.UpdatedDate = DateTime.Now;

                        _dbeEntities.SaveChanges();
                    }
                }
            }
            else if (holidayStartDate == holidayEndDate)
            {
                if (isSaveCheck == false)
                {
                    var models = new AftersalesPm_Holiday();
                    models.HolidayName = governmentHoliday;
                    models.Holiday_SDate = Convert.ToDateTime(holidayStartDate);
                    // models.Holiday_EDate = Convert.ToDateTime(holidayEndDate);               
                    models.Added = userId;
                    models.AddedDate = DateTime.Now;

                    _dbeEntities.AftersalesPm_Holiday.Add(models);
                    _dbeEntities.SaveChanges();
                }
                else if (isSaveCheck == true)
                {
                    long ids;
                    long.TryParse(id, out ids);

                    var holiday_S = Convert.ToDateTime(holidayStartDate);
                    var model = _dbeEntities.AftersalesPm_Holiday.FirstOrDefault(x => x.Holiday_SDate == holiday_S);
                    model.HolidayName = governmentHoliday;
                    model.Holiday_SDate = holiday_S;
                    //model.Holiday_EDate = Convert.ToDateTime(holidayEndDate);
                    model.Updated = userId;
                    model.UpdatedDate = DateTime.Now;

                    _dbeEntities.SaveChanges();
                }
            }

            return "ok";
        }

        public string SaveHolidayDropData(string id, string governmentHoliday, string holidayStartDate, string holidayEndDate)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            DateTime start = DateTime.Parse(holidayStartDate);
            DateTime end = DateTime.Parse(holidayEndDate);

            bool isSaveCheck = false;
            long ids1;
            long.TryParse(id, out ids1);

            if (ids1 != 0)
            {

                var dropModel = _dbeEntities.AftersalesPm_Holiday.FirstOrDefault(x => x.Id == ids1);
                //var deleteEvents = (from c in _dbeEntities.AftersalesPm_Holiday
                //                    where c.Id == ids
                //                    select c).FirstOrDefault();

                _dbeEntities.AftersalesPm_Holiday.Remove(dropModel);
                _dbeEntities.SaveChanges();

                if (holidayStartDate != holidayEndDate)
                {
                    for (DateTime counter = start; counter <= end; counter = counter.AddDays(1))
                    {

                        isSaveCheck = GetExistedHolidayData(counter);

                        if (isSaveCheck == false)
                        {
                            var models = new AftersalesPm_Holiday();
                            models.HolidayName = governmentHoliday;
                            models.Holiday_SDate = Convert.ToDateTime(counter);
                            // models.Holiday_EDate = Convert.ToDateTime(holidayEndDate);               
                            models.Added = userId;
                            models.AddedDate = DateTime.Now;

                            _dbeEntities.AftersalesPm_Holiday.Add(models);
                            _dbeEntities.SaveChanges();
                        }
                        else if (isSaveCheck == true)
                        {
                            long ids;
                            long.TryParse(id, out ids);

                            counter = Convert.ToDateTime(counter);
                            var model = _dbeEntities.AftersalesPm_Holiday.FirstOrDefault(x => x.Holiday_SDate == counter);
                            model.HolidayName = governmentHoliday;
                            model.Holiday_SDate = counter;
                            //model.Holiday_EDate = Convert.ToDateTime(holidayEndDate);
                            model.Updated = userId;
                            model.UpdatedDate = DateTime.Now;

                            _dbeEntities.SaveChanges();
                        }
                    }
                }
                else if (holidayStartDate == holidayEndDate)
                {
                    if (isSaveCheck == false)
                    {
                        var models = new AftersalesPm_Holiday();
                        models.HolidayName = governmentHoliday;
                        models.Holiday_SDate = Convert.ToDateTime(holidayStartDate);
                        // models.Holiday_EDate = Convert.ToDateTime(holidayEndDate);               
                        models.Added = userId;
                        models.AddedDate = DateTime.Now;

                        _dbeEntities.AftersalesPm_Holiday.Add(models);
                        _dbeEntities.SaveChanges();
                    }
                    else if (isSaveCheck == true)
                    {
                        long ids;
                        long.TryParse(id, out ids);

                        var holiday_S = Convert.ToDateTime(holidayStartDate);
                        var model = _dbeEntities.AftersalesPm_Holiday.FirstOrDefault(x => x.Holiday_SDate == holiday_S);
                        model.HolidayName = governmentHoliday;
                        model.Holiday_SDate = holiday_S;
                        //model.Holiday_EDate = Convert.ToDateTime(holidayEndDate);
                        model.Updated = userId;
                        model.UpdatedDate = DateTime.Now;

                        _dbeEntities.SaveChanges();
                    }
                }

            }


            return "ok";
        }
        public string SaveHolidayResizeData(string id, string governmentHoliday, string holidayStartDate, string holidayEndDate)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            DateTime start = DateTime.Parse(holidayStartDate);
            DateTime end = DateTime.Parse(holidayEndDate);

            //for (DateTime counter = start; counter <= end; counter = counter.AddDays(1))
            //{
            //    //calculatedDates.Add(counter);
            //}
            bool isSaveCheck = false;
            if (holidayStartDate != holidayEndDate)
            {
                for (DateTime counter = start; counter <= end; counter = counter.AddDays(1))
                {

                    isSaveCheck = GetExistedHolidayData(counter);

                    if (isSaveCheck == false)
                    {
                        var models = new AftersalesPm_Holiday();
                        models.HolidayName = governmentHoliday;
                        models.Holiday_SDate = Convert.ToDateTime(counter);
                        // models.Holiday_EDate = Convert.ToDateTime(holidayEndDate);               
                        models.Added = userId;
                        models.AddedDate = DateTime.Now;

                        _dbeEntities.AftersalesPm_Holiday.Add(models);
                        _dbeEntities.SaveChanges();
                    }
                    else if (isSaveCheck == true)
                    {
                        long ids;
                        long.TryParse(id, out ids);

                        counter = Convert.ToDateTime(counter);
                        var model = _dbeEntities.AftersalesPm_Holiday.FirstOrDefault(x => x.Holiday_SDate == counter);
                        model.HolidayName = governmentHoliday;
                        model.Holiday_SDate = counter;
                        //model.Holiday_EDate = Convert.ToDateTime(holidayEndDate);
                        model.Updated = userId;
                        model.UpdatedDate = DateTime.Now;

                        _dbeEntities.SaveChanges();
                    }
                }
            }
            else if (holidayStartDate == holidayEndDate)
            {
                if (isSaveCheck == false)
                {
                    var models = new AftersalesPm_Holiday();
                    models.HolidayName = governmentHoliday;
                    models.Holiday_SDate = Convert.ToDateTime(holidayStartDate);
                    // models.Holiday_EDate = Convert.ToDateTime(holidayEndDate);               
                    models.Added = userId;
                    models.AddedDate = DateTime.Now;

                    _dbeEntities.AftersalesPm_Holiday.Add(models);
                    _dbeEntities.SaveChanges();
                }
                else if (isSaveCheck == true)
                {
                    long ids;
                    long.TryParse(id, out ids);

                    var holiday_S = Convert.ToDateTime(holidayStartDate);
                    var model = _dbeEntities.AftersalesPm_Holiday.FirstOrDefault(x => x.Holiday_SDate == holiday_S);
                    model.HolidayName = governmentHoliday;
                    model.Holiday_SDate = holiday_S;
                    //model.Holiday_EDate = Convert.ToDateTime(holidayEndDate);
                    model.Updated = userId;
                    model.UpdatedDate = DateTime.Now;

                    _dbeEntities.SaveChanges();
                }
            }

            return "ok";
        }

        public string DeleteHolidayData(string id)
        {
            long ids;
            long.TryParse(id, out ids);
            var deleteEvents = (from c in _dbeEntities.AftersalesPm_Holiday
                                where c.Id == ids
                                select c).FirstOrDefault();

            _dbeEntities.AftersalesPm_Holiday.Remove(deleteEvents);
            _dbeEntities.SaveChanges();
            return "OK";
        }


        #endregion
        #region Aftersales Issue Handling
        public List<AftersalesPm_IssueVerificationModel> GetIssueVerificationList()
        {
            var query = _dbeEntities.Database.SqlQuery<AftersalesPm_IssueVerificationModel>(@"SELECT afi.[Id],afi.ProjectMasterId,afi.[ModelName],afi.[SoftwareVersionName],afi.[SoftwareVersionNo],afi.[Module],afi.[IssueDetails],afi.[IssueFrequency],afi.[IssueType]
            ,afi.[TestingPath],afi.[ResultFound],afi.[ExpectedResult],afi.[NumberOfHSsChecked],afi.[HSsIssueRatio],afi.[ComplainPercentage],afi.[NumberOfHSsReturn]
            ,afi.[IssueSolvingInfo],afi.[NumberOfSample],afi.[Status],afi.[IsActive],afi.[SupportingDocument],afi.DocumentUploadedByQc,afi.[Added],afi.[AddedDate]  
            FROM [CellPhoneProject].[dbo].[AftersalesPm_IssueVerification] afi  where afi.IsActive=1
            order by Id desc").ToList();



            foreach (var aff in query)
            {
                var query2 =
                    _dbeEntities.ProjectPmAssigns.FirstOrDefault(
                        i => i.ProjectMasterId == aff.ProjectMasterId && i.Status != "INACTIVE");
                if (query2 != null)
                {
                    aff.ProjectPmAssignId = query2.ProjectPmAssignId;
                    aff.ProjectManagerUserId = query2.ProjectManagerUserId;
                }
            }

            //foreach (var afPm in query)
            //{
            //    var query2 = (from aa in _dbeEntities.AftersalesPm_IssueVerificationStatusLog
            //                  where aa.IssueVerificationId == afPm.Id
            //                  select aa).ToList();

            //    foreach (var afPmL in query2)
            //    {
            //        if (afPmL.LogStatus == "SUBMITTED")
            //        {
            //            afPm.SubmitDate = afPmL.StartDate;
            //        }
            //        if (afPmL.LogStatus == "VALIDATED")
            //        {
            //            afPm.ValidationDate = afPmL.StartDate;
            //        }
            //        if (afPmL.LogStatus == "NOTVALIDATED")
            //        {
            //            afPm.ValidationFailDate = afPmL.StartDate;
            //        }
            //        if (afPmL.LogStatus == "CONFIRMED")
            //        {
            //            afPm.ConfirmationDate = afPmL.StartDate;
            //        }

            //        if (afPmL.LogStatus == "FINISHED")
            //        {
            //            afPm.ConfirmationDate = afPmL.StartDate;
            //        }
            //    }

            //}

            return query;
        }

        public string UpdateIssueConfirmationStatus(long ids)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var returnVal = "OK";

            try
            {
                var query = _dbeEntities.AftersalesPm_IssueVerification.FirstOrDefault(i => i.Id == ids);
                if (query != null)
                {
                    query.Status = "CONFIRMED";
                    query.Updated = userId;
                    query.UpdatedDate = DateTime.Now;
                }
                _dbeEntities.AftersalesPm_IssueVerification.AddOrUpdate(query);
                _dbeEntities.SaveChanges();

                var qq2 = new AftersalesPm_IssueVerificationStatusLog();
                qq2.IssueVerificationId = query.Id;
                qq2.LogStatus = "CONFIRMED";
                qq2.StartDate = DateTime.Now;
                qq2.Added = userId;
                qq2.AddedDate = DateTime.Now;

                _dbeEntities.AftersalesPm_IssueVerificationStatusLog.Add(qq2);
                _dbeEntities.SaveChanges();

                #region mail

                if (ids > 0)
                {
                    List<long> ids3 = new List<long>();
                    List<long> ids4 = new List<long>();
                    var wpmsUsers = _dbeEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);
                    var wpmsUsers2 = _dbeEntities.CmnUsers.FirstOrDefault(i => i.RoleName == "QCHEAD" && i.IsActive == true);
                    var wpmsUsers3 = _dbeEntities.CmnUsers.FirstOrDefault(i => i.RoleName == "ASPMHEAD" && i.IsActive == true);

                    ids3.Add(wpmsUsers2.CmnUserId);
                    ids4.Add(userId);
                    ids4.Add(wpmsUsers3.CmnUserId);

                    string body = string.Empty;

                    body += "This is to inform you that Aftersales Issues status (" + query.Status + ") has been changed by  <b>" + wpmsUsers.UserFullName
                        + " </b> for Verification purpose. Issue details are given below : ";

                    body += "<br/><br/> <b>Modele Name :</b>" + query.ModelName + "<br/><b>Issue Details :</b>" + query.IssueDetails +
                        "<br/><b>Expected Result :</b>" + query.ExpectedResult + "<br/><br/><br/>";

                    var mailSendFromPms = new MailSendFromPms();
                    mailSendFromPms.SendMail(ids3, ids4, "Aftersales Issues status (" + query.Status + ") has been changed for Verification purpose.", body);

                }
                #endregion
            }
            catch (Exception exception)
            {
                returnVal = exception.Message;
            }
            return returnVal;
        }

        public List<SwQcTestPhaseModel> GetSwQcTestPhasesForPm()
        {
            //List<SwQcTestPhase> list = _dbeEntities.SwQcTestPhases.ToList();

            //List<SwQcTestPhaseModel> models = GenericMapper<SwQcTestPhase, SwQcTestPhaseModel>.GetDestinationList(list);
            //var vmSwQc = new AssignedProjectListViewModel();
            //vmSwQc.SwQcTestPhaseModels = models;

            var models = _dbeEntities.Database.SqlQuery<SwQcTestPhaseModel>(@"SELECT [TestPhaseID],[TestPhaseName],[TestPhaseIsActive]
            FROM [CellPhoneProject].[dbo].[SwQcTestPhase] where [TestPhaseIsActive]=1 and [TestPhaseName] not in ('Accessories Test','Field (Network Test)')").ToList();

            return models;
        }

        public SwQcInchargeAssignModel CheckSwQcInchargeDuplicateAssign(long projectMasterId)
        {
            string query = string.Format(@"select top 1 Status from SwQcInchargeAssigns where ProjectMasterId={0} order by AddedDate desc", projectMasterId);
            var exe = _dbeEntities.Database.SqlQuery<SwQcInchargeAssignModel>(query).FirstOrDefault();
            return exe;
        }

        public string AssignProjectPmToSwQcHead(long issueIds, string pmRemarks, long pMasterId, long pMAssignId, long pmUserId,
            List<string> selectedSampleValue, long sampleNo, long userId, long swWcInchargeAssignUserId, long testPhasefrPm,
            long swVersionNumber, string versionName, string sourceVersion, string targetVersion, string ActionType)
        {
            var roleName = _dbeEntities.Database.SqlQuery<CmnUserModel>(@"select RoleName from [CellPhoneProject].[dbo].[CmnUsers]
                where CmnUserId={0} ", userId).FirstOrDefault();
            var query = (from pm in _dbeEntities.ProjectMasters where pm.ProjectMasterId == pMasterId select pm).FirstOrDefault();

            SwQcHeadAssignsFromPm swQcInchargeAssign = new SwQcHeadAssignsFromPm();
            swQcInchargeAssign.ProjectMasterId = pMasterId;
            swQcInchargeAssign.ProjectName = query.ProjectName;
            swQcInchargeAssign.ProjectType = query.ProjectType;
            swQcInchargeAssign.OrderNumber = query.OrderNuber;
            swQcInchargeAssign.ProjectPmAssignId = pMAssignId;
            swQcInchargeAssign.ProjectOrderShipmentId = 1;
            swQcInchargeAssign.ProjectManagerUserId = userId;
            swQcInchargeAssign.ProjectManagerSampleNo = Convert.ToInt32(sampleNo);
            var kk = "";
            foreach (var ss in selectedSampleValue)
            {
                kk += ss + ',';

            }
            var kk1 = kk.TrimStart(',');
            swQcInchargeAssign.ProjectManagerSampleType = kk1.TrimEnd(',');
            swQcInchargeAssign.SoftwareVersionName = versionName;
            swQcInchargeAssign.SoftwareVersionNo = Convert.ToInt32(swVersionNumber);
            swQcInchargeAssign.PriorityFromPm = "HIGH";
            if (roleName.RoleName == "ASPMHEAD" || roleName.RoleName == "ASPM")
            {
                swQcInchargeAssign.AssignCatagory = roleName.RoleName;
            }
            else
            {
                swQcInchargeAssign.AssignCatagory = "DONOTKNOW";
            }
            swQcInchargeAssign.Status = "NEW";
            swQcInchargeAssign.PmToQcHeadAssignTime = DateTime.Now;
            swQcInchargeAssign.SwQcHeadUserId = swWcInchargeAssignUserId;
            swQcInchargeAssign.PmToQcHeadAssignComment = pmRemarks;
            swQcInchargeAssign.TestPhaseID = testPhasefrPm;
            swQcInchargeAssign.IsFinalPhaseMP = false;
            swQcInchargeAssign.SourceVersion = sourceVersion;
            swQcInchargeAssign.TargetVersion = targetVersion;
            swQcInchargeAssign.Added = userId;
            swQcInchargeAssign.AddedDate = DateTime.Now;

            _dbeEntities.SwQcHeadAssignsFromPms.AddOrUpdate(swQcInchargeAssign);
            _dbeEntities.SaveChanges();

            var swQcheadAssignIds = swQcInchargeAssign.SwQcHeadAssignId;

            if (swVersionNumber != 0)
            {
                if (testPhasefrPm == 5)
                {
                    List<SwQcIssueDetail> swQcHead = (from swQcHeads in _dbeEntities.SwQcIssueDetails
                                                      where
                                                          swQcHeads.SoftwareVersionNo == swVersionNumber && swQcHeads.ProjectName == query.ProjectName &&
                                                          swQcHeads.TestPhaseID == testPhasefrPm && swQcHeads.Demo == "Demo"
                                                      select swQcHeads).ToList();

                    foreach (var swQcIssueDetail in swQcHead)
                    {
                        swQcIssueDetail.SoftwareVersionName = versionName;

                        _dbeEntities.SwQcIssueDetails.AddOrUpdate(swQcIssueDetail);
                        _dbeEntities.SaveChanges();
                    }
                }
                else
                {
                    List<SwQcIssueDetail> swQcHead = (from swQcHeads in _dbeEntities.SwQcIssueDetails
                                                      where
                                                          swQcHeads.SoftwareVersionNo == swVersionNumber && swQcHeads.ProjectName == query.ProjectName &&
                                                          swQcHeads.TestPhaseID != 5 && swQcHeads.TestPhaseID != 10
                                                      select swQcHeads).ToList();

                    foreach (var swQcIssueDetail in swQcHead)
                    {
                        swQcIssueDetail.SoftwareVersionName = versionName;

                        _dbeEntities.SwQcIssueDetails.AddOrUpdate(swQcIssueDetail);
                        _dbeEntities.SaveChanges();
                    }
                }
            }

            var qq1 = _dbeEntities.AftersalesPm_IssueVerification.FirstOrDefault(i => i.Id == issueIds);
            if (qq1 != null && (qq1.Status == "CONFIRMED" || qq1.Status == "ForwardedForFullSwCheck" || qq1.Status == "ForwardedForFOTATest"))
            {
                //qq1.SwQcHeadAssignId = swQcheadAssignIds;
                if (ActionType.Trim() == "Full Sw Check")
                {
                    qq1.Status = "ForwardedForFullSwCheck";
                }
                if (ActionType.Trim() == "FOTA Test")
                {
                    qq1.Status = "ForwardedForFOTATest";
                }
                qq1.UpdatedDate = DateTime.Now;
                qq1.Updated = userId;
                _dbeEntities.AftersalesPm_IssueVerification.AddOrUpdate(qq1);
                _dbeEntities.SaveChanges();

                var qq2 = new AftersalesPm_IssueVerificationStatusLog();
                qq2.SwQcHeadAssignId = swQcheadAssignIds;
                qq2.IssueVerificationId = issueIds;
                //  qq2.LogStatus = "FullSoftwareChecked";
                if (ActionType.Trim() == "Full Sw Check")
                {
                    qq2.LogStatus = "ForwardedForFullSwCheck";
                }
                if (ActionType.Trim() == "FOTA Test")
                {
                    qq2.LogStatus = "ForwardedForFOTATest";
                }
                qq2.StartDate = DateTime.Now;
                qq2.Added = userId;
                qq2.AddedDate = DateTime.Now;
                _dbeEntities.AftersalesPm_IssueVerificationStatusLog.Add(qq2);
                _dbeEntities.SaveChanges();
            }

            //else if (qq1 != null && qq1.Status == "FullSoftwareChecked")
            //{
            //     var qq21 =
            //            _dbeEntities.AftersalesPm_IssueVerificationStatusLog.FirstOrDefault(
            //                i => i.IssueVerificationId == issueIds && i.LogStatus == "FullSoftwareChecked");

            //    var query22 =
            //        _dbeEntities.SwQcHeadAssignsFromPms.FirstOrDefault(i => i.SwQcHeadAssignId == qq21.SwQcHeadAssignId);

            //    if (query22.Status == "RECOMMENDED")
            //    {

            //        qq1.Status = "FOTATestResult";

            //        if (qq21 != null)
            //        {
            //            qq21.EndDate = query22.SwQcHeadToPmSubmitTime;
            //            qq21.Updated = userId;
            //            qq21.UpdatedDate = DateTime.Now;
            //            _dbeEntities.AftersalesPm_IssueVerificationStatusLog.AddOrUpdate(qq21);
            //            _dbeEntities.SaveChanges();
            //        }
            //    }
            //    else
            //    {
            //        qq1.Status = "Full Software is not checked";
            //    }
            //    qq1.UpdatedDate = DateTime.Now;
            //    qq1.Updated = userId;
            //    _dbeEntities.AftersalesPm_IssueVerification.AddOrUpdate(qq1);
            //    _dbeEntities.SaveChanges();

            //    var qq2 = new AftersalesPm_IssueVerificationStatusLog();
            //    qq2.SwQcHeadAssignId = swQcheadAssignIds;
            //    qq2.IssueVerificationId = issueIds;
            //    if (query22.Status == "RECOMMENDED")
            //    {
            //        qq2.LogStatus = "FOTATestResult";
            //    }
            //    else
            //    {
            //        qq2.LogStatus = "Full Software is not checked";
            //    }
            //    qq2.StartDate = DateTime.Now;
            //    qq2.Added = userId;
            //    qq2.AddedDate = DateTime.Now;
            //    _dbeEntities.AftersalesPm_IssueVerificationStatusLog.Add(qq2);
            //    _dbeEntities.SaveChanges();
            //}

            return "ok";
        }

        public string SaveIntoAftersalesIssueVerification(List<AftersalesPm_IssueVerificationModel> issueList, long projectMasterId, string projectName, string attachment)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            string returnValue = "ok";
            long idss = 0;
            try
            {
                var model = new AftersalesPm_IssueVerification();
                model.ProjectMasterId = projectMasterId;
                model.ModelName = projectName;
                model.SoftwareVersionName = issueList[0].SoftwareVersionName;
                model.SoftwareVersionNo = issueList[0].SoftwareVersionNo;
                model.Module = issueList[0].Module;
                model.IssueDetails = issueList[0].IssueDetails;
                model.IssueFrequency = issueList[0].IssueFrequency;
                model.IssueType = issueList[0].IssueType;
                model.TestingPath = issueList[0].TestingPath;
                model.ResultFound = issueList[0].ResultFound;
                model.ExpectedResult = issueList[0].ExpectedResult;
                model.NumberOfHSsChecked = issueList[0].NumberOfHSsChecked;
                model.HSsIssueRatio = issueList[0].HSsIssueRatio;
                model.IssueSolvingInfo = issueList[0].IssueSolvingInfo;
                model.ComplainPercentage = issueList[0].ComplainPercentage;
                model.NumberOfHSsReturn = issueList[0].NumberOfHSsReturn;
                model.NumberOfSample = issueList[0].NumberOfSample;
                model.Status = "SUBMITTED";
                model.SupportingDocument = attachment;
                model.IsActive = true;
                model.Added = userId;
                model.AddedDate = DateTime.Now;
                _dbeEntities.AftersalesPm_IssueVerification.Add(model);
                _dbeEntities.SaveChanges();

                idss = model.Id;

                if (idss > 0)
                {
                    var logModel = new AftersalesPm_IssueVerificationStatusLog();
                    logModel.IssueVerificationId = idss;
                    logModel.StartDate = DateTime.Now;
                    logModel.LogStatus = "SUBMITTED";
                    logModel.Added = userId;
                    logModel.AddedDate = DateTime.Now;
                    _dbeEntities.AftersalesPm_IssueVerificationStatusLog.Add(logModel);
                    _dbeEntities.SaveChanges();
                }

                _dbeEntities.SaveChanges();

                #region mail

                if (idss > 0)
                {
                    List<long> ids3 = new List<long>();
                    List<long> ids4 = new List<long>();
                    var wpmsUsers = _dbeEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);
                    var wpmsUsers2 = _dbeEntities.CmnUsers.FirstOrDefault(i => i.RoleName == "QCHEAD" && i.IsActive == true);
                    var wpmsUsers3 = _dbeEntities.CmnUsers.FirstOrDefault(i => i.RoleName == "ASPMHEAD" && i.IsActive == true);

                    ids3.Add(wpmsUsers2.CmnUserId);
                    ids4.Add(userId);
                    ids4.Add(wpmsUsers3.CmnUserId);

                    string body = string.Empty;

                    body += "This is to inform you that a new Aftersales Issue has been raised by  <b>" + wpmsUsers.UserFullName
                        + " </b> for Verification purpose. Issue details are given below : ";

                    body += "<br/><br/> <b>Modele Name :</b>" + projectName + "<br/><b>Issue Details :</b>" + issueList[0].IssueDetails +
                        "<br/><b>Expected Result :</b>" + issueList[0].ExpectedResult + "<br/><br/><br/>";

                    var mailSendFromPms = new MailSendFromPms();
                    mailSendFromPms.SendMail(ids3, ids4, "An Aftersales Issue has been raised for Verification.", body);

                }
                #endregion
            }
            catch (Exception exception)
            {
                returnValue = exception.Message;
            }

            return returnValue;
        }
        public List<ProjectMasterModel> GetModelsForAftersalesIssueVerification()
        {
            var query =
            _dbeEntities.Database.SqlQuery<ProjectMasterModel>(
                @"select distinct ProjectName,ProjectMasterId from CellPhoneProject.dbo.ProjectMasters pm where IsActive=1 and
                ProjectType in ('Smart','Feature') 
                and ProjectName not in ('5 inch 2700 battery','HIT 5 Inch FWVGA 3G','HIT 5.2 inch FWVGA 4G','Tinno 5.5inch FHD','V SCOPE 5 inch FWVGA 3G','V3961','VSUN 5.5 FHD')
                and ProjectMasterId in (select top 1 ProjectMasterId from CellPhoneProject.dbo.ProjectMasters where ProjectName =pm.ProjectName and IsActive=1 order by ProjectMasterId desc) 
                order by ProjectName asc").ToList();

            return query;
        }

        public SwQcHeadAssignsFromPm GetAllVersionNameForPm(long swVerNo, long proId, long testPhases)
        {
            var proNames = (from pm in _dbeEntities.ProjectMasters where pm.ProjectMasterId == proId select pm.ProjectName).FirstOrDefault();

            var getSwQcInchargeTestCounts = new SwQcHeadAssignsFromPm();
            if (testPhases != 5)
            {
                getSwQcInchargeTestCounts = _dbeEntities.Database.SqlQuery<SwQcHeadAssignsFromPm>(@"select * from SwQcHeadAssignsFromPm
                where ProjectName={1} and SoftwareVersionNo={0} and TestPhaseID !=5 and  TestPhaseID !=10", swVerNo, proNames, testPhases).FirstOrDefault();
            }
            else if (testPhases == 5)
            {
                getSwQcInchargeTestCounts = _dbeEntities.Database.SqlQuery<SwQcHeadAssignsFromPm>(@"select * from SwQcHeadAssignsFromPm
                where ProjectName={1} and SoftwareVersionNo={0} and TestPhaseID =5", swVerNo, proNames, testPhases).FirstOrDefault();
            }
            return getSwQcInchargeTestCounts;
        }

        public long GetUserIdByRoleName(string roleName)
        {
            long userId = 0;
            if (roleName != null)
            {
                userId = _dbeEntities.CmnUsers.FirstOrDefault(i => i.RoleName == roleName && i.IsActive == true).CmnUserId;
            }
            return userId;
        }

        public PmCmnUserModel GetPmUserInfo(long pmUserId)
        {
            var pmUserInfo = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == pmUserId && x.IsActive == true).Select(j => new PmCmnUserModel
            {
                CmnUserId = j.CmnUserId,
                UserFullName = j.UserFullName,
                UserName = j.UserName
            }).FirstOrDefault();
            if (pmUserInfo == null) return new PmCmnUserModel
            {
                CmnUserId = 0,
                UserFullName = "",
                UserName = ""
            };
            return pmUserInfo;
        }

        public bool FullSoftwareVersionCheckedOrNot(long issueIds)
        {
            SwQcHeadAssignsFromPmModel getIncentiveReports = null;

            var returnVal = false;

            string getIncentiveReportQuery = string.Format(@"select sw.Status as SwQcHeadAssignStatus  from CellPhoneProject.dbo.AftersalesPm_IssueVerification aiv 
            inner join  CellPhoneProject.dbo.AftersalesPm_IssueVerificationStatusLog al on aiv.Id=al.IssueVerificationId
            inner join  CellPhoneProject.dbo.SwQcHeadAssignsFromPm sw on al.SwQcHeadAssignId=sw.SwQcHeadAssignId
            where aiv.Id={0} and aiv.Status='FullSoftwareChecked'", issueIds);
            getIncentiveReports =
               _dbeEntities.Database.SqlQuery<SwQcHeadAssignsFromPmModel>(getIncentiveReportQuery).FirstOrDefault();

            if (getIncentiveReports != null && getIncentiveReports.SwQcHeadAssignStatus == "RECOMMENDED")
            {
                returnVal = true;
            }
            else
            {
                returnVal = false;
            }
            return returnVal;
        }
        public string FullSoftwareOrFotaCheckedOrNot(long issueIds)
        {
            var returnVal = "";

            var getReport = _dbeEntities.Database.SqlQuery<SwQcHeadAssignsFromPmModel>(@"select sum(ForwardedForFullSwCheck) as ForwardedForFullSwCheck,sum(FullSoftwareConfirmed) as FullSoftwareConfirmed,
                sum(ForwardedForFOTATest) as ForwardedForFOTATest,sum(FOTATestConfirmed) as FOTATestConfirmed
                from
                (
	                select lg.LogStatus as SwQcHeadAssignStatus,
	                case when lg.LogStatus='ForwardedForFullSwCheck' then count(lg.LogStatus) else 0 end as ForwardedForFullSwCheck,
	                case when lg.LogStatus='FullSoftwareConfirmed' then count(lg.LogStatus)  else 0  end as FullSoftwareConfirmed,
	                case when lg.LogStatus='ForwardedForFOTATest' then count(lg.LogStatus)  else 0  end as ForwardedForFOTATest,
	                case when lg.LogStatus='FOTATestConfirmed' then count(lg.LogStatus)  else 0  end as FOTATestConfirmed

	                from CellPhoneProject.dbo.AftersalesPm_IssueVerificationStatusLog lg
	                where lg.IssueVerificationId={0} and LogStatus in ('ForwardedForFOTATest','ForwardedForFullSwCheck','FullSoftwareConfirmed','FOTATestConfirmed') 
	                group by LogStatus
                )A", issueIds).FirstOrDefault();

            if (getReport.ForwardedForFullSwCheck != getReport.FullSoftwareConfirmed)
            {
                returnVal = "Please Complete/Done Full Sw Check";
            }
            else if (getReport.ForwardedForFOTATest != getReport.FOTATestConfirmed)
            {
                returnVal = "Please Complete/Done FOTA Test";
            }
            return returnVal;
        }
        public string UpdateActionStatus(long ids, string selectedValAction)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var returnVal = "OK";
            var query = _dbeEntities.AftersalesPm_IssueVerification.FirstOrDefault(i => i.Id == ids);
            try
            {
             
                if (query != null)
                {
                    if (selectedValAction == "Full Sw Check")
                    {
                        query.Status = "FullSoftwareConfirmed";
                        query.Updated = userId;
                        query.UpdatedDate = DateTime.Now;
                        _dbeEntities.AftersalesPm_IssueVerification.AddOrUpdate(query);
                        _dbeEntities.SaveChanges();
                    }
                    else if (selectedValAction == "FOTA Test")
                    {
                        query.Status = "FOTATestConfirmed";
                        query.Updated = userId;
                        query.UpdatedDate = DateTime.Now;
                        _dbeEntities.AftersalesPm_IssueVerification.AddOrUpdate(query);
                        _dbeEntities.SaveChanges();
                    }
                    else if (selectedValAction == "ForwardToSupplier")
                    {
                        query.Status = "ForwardToSupplier";
                        query.Updated = userId;
                        query.UpdatedDate = DateTime.Now;
                        _dbeEntities.AftersalesPm_IssueVerification.AddOrUpdate(query);
                        _dbeEntities.SaveChanges();
                    }
                    else if (selectedValAction == "CLOSED")
                    {
                        var sdd = FullSoftwareOrFotaCheckedOrNot(ids);

                        if (sdd == "Please Complete/Done Full Sw Check")
                        {
                            returnVal = sdd;
                        }
                        else if (sdd == "Please Complete/Done FOTA Test")
                        {
                            returnVal = sdd;
                        }
                        else
                        {
                            query.Status = "CLOSED";
                            query.Updated = userId;
                            query.UpdatedDate = DateTime.Now;
                            _dbeEntities.AftersalesPm_IssueVerification.AddOrUpdate(query);
                            _dbeEntities.SaveChanges();
                        }
                    }
                }
                var qq2 = new AftersalesPm_IssueVerificationStatusLog();
                qq2.IssueVerificationId = query.Id;
                if (selectedValAction == "Full Sw Check")
                {
                    qq2.LogStatus = "FullSoftwareConfirmed";
                    qq2.StartDate = DateTime.Now;
                    qq2.Added = userId;
                    qq2.AddedDate = DateTime.Now;

                    _dbeEntities.AftersalesPm_IssueVerificationStatusLog.Add(qq2);
                    _dbeEntities.SaveChanges();
                }
                else if (selectedValAction == "FOTA Test")
                {
                    qq2.LogStatus = "FOTATestConfirmed";
                    qq2.StartDate = DateTime.Now;
                    qq2.Added = userId;
                    qq2.AddedDate = DateTime.Now;

                    _dbeEntities.AftersalesPm_IssueVerificationStatusLog.Add(qq2);
                    _dbeEntities.SaveChanges();
                }
                else if (selectedValAction == "ForwardToSupplier")
                {
                    qq2.LogStatus = "ForwardToSupplier";
                    qq2.StartDate = DateTime.Now;
                    qq2.Added = userId;
                    qq2.AddedDate = DateTime.Now;

                    _dbeEntities.AftersalesPm_IssueVerificationStatusLog.Add(qq2);
                    _dbeEntities.SaveChanges();
                }
                else if (selectedValAction == "CLOSED")
                {
                    var sdd = FullSoftwareOrFotaCheckedOrNot(ids);
                    if (sdd == "Please Complete/Done Full Sw Check")
                    {
                        returnVal = sdd;
                    }
                    else if (sdd == "Please Complete/Done FOTA Test")
                    {
                        returnVal = sdd;
                    }
                    else
                    {
                        qq2.LogStatus = "CLOSED";
                        qq2.StartDate = DateTime.Now;
                        qq2.Added = userId;
                        qq2.AddedDate = DateTime.Now;

                        _dbeEntities.AftersalesPm_IssueVerificationStatusLog.Add(qq2);
                        _dbeEntities.SaveChanges();
                    }
                }
                //qq2.LogStatus = selectedValAction;
                #region mail

                if (ids > 0)
                {
                    List<long> ids3 = new List<long>();
                    List<long> ids4 = new List<long>();
                    var wpmsUsers = _dbeEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);
                    var wpmsUsers2 = _dbeEntities.CmnUsers.FirstOrDefault(i => i.RoleName == "QCHEAD" && i.IsActive == true);
                    var wpmsUsers3 = _dbeEntities.CmnUsers.FirstOrDefault(i => i.RoleName == "ASPMHEAD" && i.IsActive == true);

                    ids3.Add(wpmsUsers2.CmnUserId);
                    ids4.Add(userId);
                    ids4.Add(wpmsUsers3.CmnUserId);

                    string body = string.Empty;

                    body += "This is to inform you that Aftersales Issues status (" + query.Status + ") has been changed by  <b>" + wpmsUsers.UserFullName
                        + " </b> for Verification purpose. Issue details are given below : ";

                    body += "<br/><br/> <b>Modele Name :</b>" + query.ModelName + "<br/><b>Issue Details :</b>" + query.IssueDetails +
                        "<br/><b>Expected Result :</b>" + query.ExpectedResult + "<br/><br/><br/>";

                    var mailSendFromPms = new MailSendFromPms();
                    mailSendFromPms.SendMail(ids3, ids4, "Aftersales Issues status (" + query.Status + ") has been changed for Verification purpose.", body);

                }
                #endregion
            }
            catch (Exception exception)
            {
                returnVal = exception.Message;
            }

           

            return returnVal;
        }

        public string SaveSupplierDetails(long ids, string details, string attachment)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var returnVal = "OK";
            try
            {
                string Dollar = attachment.ToString();
                string dolreplace = "";
                if (Dollar != "")
                {
                    dolreplace = Dollar.Substring(1, Dollar.Length - 1);
                }
                else
                {
                    dolreplace = "";
                }

                var aftersalesPm = new AftersalesPm_SupplierFeedBack();
                aftersalesPm.IssueVerificationId = ids;
                aftersalesPm.Details = details;
                aftersalesPm.Attachment = dolreplace;
                aftersalesPm.Added = userId;
                aftersalesPm.AddedDate = DateTime.Now;
                _dbeEntities.AftersalesPm_SupplierFeedBack.Add(aftersalesPm);
                _dbeEntities.SaveChanges();

                var qq1 = _dbeEntities.AftersalesPm_IssueVerification.FirstOrDefault(i => i.Id == ids);
                qq1.Status = "SUBMITTED";
                qq1.Updated = userId;
                qq1.UpdatedDate = DateTime.Now;
                _dbeEntities.AftersalesPm_IssueVerification.AddOrUpdate(qq1);
                _dbeEntities.SaveChanges();


                var qq2 = new AftersalesPm_IssueVerificationStatusLog();
                qq2.IssueVerificationId = ids;
                qq2.LogStatus = "SUBMITTED";
                qq2.StartDate = DateTime.Now;
                qq2.Added = userId;
                qq2.AddedDate = DateTime.Now;
                _dbeEntities.AftersalesPm_IssueVerificationStatusLog.Add(qq2);
                _dbeEntities.SaveChanges();

                //
                #region mail

                if (ids > 0)
                {
                    List<long> ids3 = new List<long>();
                    List<long> ids4 = new List<long>();
                    var wpmsUsers = _dbeEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);
                    var wpmsUsers2 = _dbeEntities.CmnUsers.FirstOrDefault(i => i.RoleName == "QCHEAD" && i.IsActive == true);
                    var wpmsUsers3 = _dbeEntities.CmnUsers.FirstOrDefault(i => i.RoleName == "ASPMHEAD" && i.IsActive == true);

                    ids3.Add(wpmsUsers2.CmnUserId);
                    ids4.Add(userId);
                    ids4.Add(wpmsUsers3.CmnUserId);

                    string body = string.Empty;

                    body += "This is to inform you that Aftersales Issues status (" + qq1.Status + ") has been changed by  <b>" + wpmsUsers.UserFullName
                        + " </b> for Verification purpose. Issue details are given below : ";

                    body += "<br/><br/> <b>Modele Name :</b>" + qq1.ModelName + "<br/><b>Issue Details :</b>" + qq1.IssueDetails +
                        "<br/><b>Expected Result :</b>" + qq1.ExpectedResult + "<br/><br/><br/>";

                    var mailSendFromPms = new MailSendFromPms();
                    mailSendFromPms.SendMail(ids3, ids4, "Aftersales Issues status (" + qq1.Status + ") has been changed for Verification purpose.", body);

                }
                #endregion
            }
            catch (Exception exception)
            {
                returnVal = exception.Message;
            }
            return returnVal;
        }

        #endregion
    }
}