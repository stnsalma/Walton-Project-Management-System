using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Infrastructures.Repositories;
using ProjectManagement.Models;
using ProjectManagement.ViewModels.AftersalesPm;


namespace ProjectManagement.Controllers
{
    [Authorize(Roles = "ASPM,ASPMHEAD,ACCNT")]
    public class AfterSalesPmController : Controller
    {
        //private CellPhoneProjectEntities _dbeEntities;
        //private IAfterSalesPmRepository _repository;
        ////
        //// GET: /AfterSalesPm/
        //public ActionResult Index()
        //{
        //    return View();
        //}
        private readonly CellPhoneProjectEntities _dbEntities;

        long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);


        private readonly IProjectManagerRepository _projectManagerRepository;

        private readonly IAfterSalesPmRepository _afterSales;
        private readonly IGeneralIncidentRepository _generalIncidentRepository;
        private readonly ICommonRepository _commonRepository;
        public AfterSalesPmController(CommonRepository commonRepository, AfterSalesPmRepository afterSalesPmRepository, GeneralIncidentRepository generalIncidentRepository)
        {
            this._afterSales = afterSalesPmRepository;
            _generalIncidentRepository = generalIncidentRepository;
            _dbEntities = new CellPhoneProjectEntities();
            _dbEntities.Configuration.LazyLoadingEnabled = false;
            _commonRepository = commonRepository;
            String useridentity = System.Web.HttpContext.Current.User.Identity.Name;
            var users = Convert.ToInt64(useridentity == "" ? "0" : useridentity);
            ViewBag.ChinaQcInspectionCount = _commonRepository.GetChinaQcInspectionCount(users);
        }
        // GET: ProjectManager
        public ActionResult Index()
        {
            long userId = Convert.ToInt64(User.Identity.Name);

            var sd = userId;

            ProjectMasterModel model = _afterSales.GetProjectMasterModel(1);
            var test = model;

            return View();
        }
        public ActionResult AftersalesMonthlyIncentive(string monId, string yearId, string employeeCode)
        {
            var vmAftersales = new VmAftersalesIncentivePolicy();
            long monIds;
            long.TryParse(monId, out monIds);

            long yearIds;
            long.TryParse(yearId, out yearIds);

            vmAftersales.CmnUserModelList = _afterSales.GetAftersalesPmUserList();

            vmAftersales.PmIncentiveBaseModelsList = _afterSales.GetAftersalesPmIncentiveBase();
            List<SelectListItem> itemsIncentive = vmAftersales.PmIncentiveBaseModelsList.Select(model => new SelectListItem { Text = model.IncentiveName + '-' + model.Amount, Value = model.Id.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.IncentivesValue = itemsIncentive;

            vmAftersales.ProjectMasterModelList = _afterSales.GetProjectMasterListForAftersalesPmIncentive(employeeCode);
            List<SelectListItem> items = vmAftersales.ProjectMasterModelList.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.Projects = items;

            return View(vmAftersales);
        }
        [HttpPost]
        public ActionResult AddProject(string incentiveType, string incentiveAmount, long incentiveId, string personEmpCode, string projectName,
           long projectMasterId, string monName, int monId, string yearName,
          long yearId, string remarks, string deductAmount, string d_remarks, string finalAmount)
        {
            using (var dbEntities = new CellPhoneProjectEntities())
            {
                var model = new Custom_Pm_IncentiveModel
                {
                    ProjectId = projectMasterId,
                    ProjectName = projectName,
                    Amount = incentiveAmount.ToString(),
                    EmployeeCode = personEmpCode,
                    Pm_Incentive_Base_Id = incentiveId,
                    MonNum = monId,
                    Month = monName,
                    Year = yearId.ToString(),
                    Remarks = remarks,
                    D_Remarks = d_remarks,
                    DeductionAmount = deductAmount.ToString(),
                    FinalAmount = finalAmount.ToString(),
                    IncentiveTypes = incentiveType

                };
                return PartialView("~/Views/AftersalesPm/Partial/_AftersalesPmIncentiveList.cshtml", model);
            }

        }

        [HttpPost]
        public ActionResult AddProjectForOthers(string incentiveType, string addedAmount, string personEmpCode, string projectName,
           long projectMasterId, string monName, int monId, string yearName,
          long yearId, string remarks, string deductAmount, string d_remarks, string finalAmount)
        {
            using (CellPhoneProjectEntities dEntities = new CellPhoneProjectEntities())
            {
                var model = new Custom_Pm_IncentiveModel
                {
                    ProjectId = projectMasterId,
                    ProjectName = projectName,
                    Amount = addedAmount.ToString(),
                    EmployeeCode = personEmpCode,
                    MonNum = monId,
                    Month = monName,
                    Year = yearId.ToString(),
                    Remarks = remarks,
                    D_Remarks = d_remarks,
                    DeductionAmount = deductAmount.ToString(),
                    FinalAmount = finalAmount.ToString(),
                    IncentiveTypes = incentiveType
                };
                return PartialView("~/Views/AftersalesPm/Partial/_AftersalesPmIncentiveOthersList.cshtml", model);
                //return PartialView("~/Views/ProjectManager/Partial/_PmIncentiveOthersList.cshtml", model);
            }
        }

        [HttpGet]
        public ActionResult CreateFocForAftersalesPm(VmAftersalesPmFoc vmAftersales, string projectType)
        {
            //  var vmAftersales = new VmAftersalesPmFoc();

            vmAftersales.CmnUserModels = _afterSales.GetAftersalesPmUserList();

            vmAftersales.ProjectMasterModels = _afterSales.GetProjectMasterListForAftersalesPmFoc();
            List<SelectListItem> items = vmAftersales.ProjectMasterModels.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.Projects = items;

            vmAftersales.SpareNameModels = _afterSales.GetSpareNameForAftersalesPm(projectType);
            List<SelectListItem> itemsForSpare = vmAftersales.SpareNameModels.Select(model => new SelectListItem { Text = model.SparePartsName, Value = model.SpareId.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.Spares = itemsForSpare;

            vmAftersales.CreateFocForAftersalesPmModels = _afterSales.GetFocForAftersalesPm();

            return View(vmAftersales);
        }

        [HttpPost]
        public JsonResult CreateFocForAftersalesPm(string focDatas)
        {
            var results =
            JsonConvert.DeserializeObject<List<VmAftersalesPmFoc>>(focDatas);
            Console.Write(focDatas);
            var saveMode = "";
            bool isExist = false;

            isExist = _afterSales.CheckDuplicateFoc(results);

            if (isExist)
            {

                saveMode = "Already generated";
                return Json(new { data = saveMode }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                saveMode = _afterSales.SaveFocForAftersalesPm(results);
            }

            return Json(new { data = saveMode }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSpareNameForAftersalesPm(long projectId)
        {
            var modelss = new VmAftersalesPmFoc();
            var proTypes = (from pm in _dbEntities.ProjectMasters
                            where pm.ProjectMasterId == projectId
                            select new
                            {
                                pm.ProjectMasterId,
                                pm.ProjectName,
                                pm.OrderNuber,
                                pm.ProjectType,
                                pm.SupplierName
                            }).ToList();

            foreach (var proType in proTypes)
            {
                modelss.SpareNameModels = _afterSales.GetSpareNameForAftersalesPm(proType.ProjectType);
            }

            List<SelectListItem> items1 = modelss.SpareNameModels.Select(model => new SelectListItem { Text = model.SparePartsName, Value = model.SpareId.ToString(CultureInfo.InvariantCulture) }).ToList();
            var json = JsonConvert.SerializeObject(items1);

            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetSupplierForAftersalesPm(long projectId)
        {
            var modelss = new VmAftersalesPmFoc();

            modelss.ProjectMasterModel = _afterSales.GetSupplierForAftersalesPm(projectId);

            // List<SelectListItem> items1 = modelss.SpareNameModels.Select(model => new SelectListItem { Text = model.SparePartsName, Value = model.SpareId.ToString(CultureInfo.InvariantCulture) }).ToList();
            var json = JsonConvert.SerializeObject(modelss.ProjectMasterModel);

            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult CheckDuplicateFoc(List<VmAftersalesPmFoc> focDatas)
        {
            string focDatas1 = null;
            bool isExist = false;
            if (focDatas != null)
            {
                isExist = _afterSales.CheckDuplicateFoc(focDatas);

            }

            if (isExist)
            {
                focDatas1 = "Already Generated.";

            }

            return Json(new { data = focDatas1 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFocDataForAllIncentive(string monthName, string monNum, string year, string employeeCode)
        {
            var createFocForAftersalesPms = new List<CreateFocForAftersalesPmModel>();
            var pmFocInsForAll = _afterSales.GetFocDataForAllIncentive(monthName, monNum, year, employeeCode);

            var pmFocInsForParticular = _afterSales.GetFocDataForParticularPerson(monthName, monNum, year, employeeCode);


            var asmHead =
             (from asm in _dbEntities.CmnUsers where asm.RoleName == "ASPMHEAD" && asm.IsActive == true select asm).FirstOrDefault();
            var otherAsm =
            (from asm in _dbEntities.CmnUsers where asm.RoleName == "ASPM" && asm.IsActive == true select asm).ToList();

            if (employeeCode == asmHead.EmployeeCode)
            {
                foreach (var pmfoc in pmFocInsForAll)
                {
                    var items = new CreateFocForAftersalesPmModel();
                    items.Id = pmfoc.Id;
                    items.ProjectId = pmfoc.ProjectId;
                    items.ProjectName = pmfoc.ProjectName;
                    items.SpareName = pmfoc.SpareName;
                    items.EmployeeCode = pmfoc.EmployeeCode;
                    items.FocConfirmedDate = pmfoc.FocConfirmedDate;
                    items.InventoryEntryDate = pmfoc.InventoryEntryDate;
                    items.UnitPrice = pmfoc.UnitPrice;
                    items.DeductionAmount = pmfoc.DeductionAmount;

                    if (items.DeductionAmount == null)
                    {
                        items.DeductionAmount = 0;
                    }
                    items.D_Remarks = pmfoc.D_Remarks;
                    items.FinalAmount = pmfoc.FinalAmount;
                    if (items.UnitPrice == null)
                    {
                        items.UnitPrice = 0;
                    }
                    // totalFocIns += Convert.ToDecimal(items.UnitPrice);
                    items.FinalAmountTotal = Convert.ToDecimal(items.UnitPrice);

                    const int fourty = 40;
                    items.SixtyShareIncentive = (items.FinalAmountTotal * fourty) / 100;

                    createFocForAftersalesPms.Add(items);
                }

            }
            else
            {
                foreach (var pmfoc in pmFocInsForParticular)
                {
                    var items = new CreateFocForAftersalesPmModel();
                    items.Id = pmfoc.Id;
                    items.ProjectId = pmfoc.ProjectId;
                    items.ProjectName = pmfoc.ProjectName;
                    items.SpareName = pmfoc.SpareName;
                    items.EmployeeCode = pmfoc.EmployeeCode;
                    items.FocConfirmedDate = pmfoc.FocConfirmedDate;
                    items.InventoryEntryDate = pmfoc.InventoryEntryDate;
                    items.UnitPrice = pmfoc.UnitPrice;
                    items.DeductionAmount = pmfoc.DeductionAmount;

                    if (items.DeductionAmount == null)
                    {
                        items.DeductionAmount = 0;
                    }
                    items.D_Remarks = pmfoc.D_Remarks;
                    items.FinalAmount = pmfoc.FinalAmount;
                    if (items.UnitPrice == null)
                    {
                        items.UnitPrice = 0;
                    }

                    items.FinalAmountTotal = Convert.ToDecimal(items.UnitPrice);

                    const int sixty = 60;
                    items.SixtyShareIncentive = (items.FinalAmountTotal * sixty) / 100;

                    createFocForAftersalesPms.Add(items);
                }

            }
            foreach (var others in otherAsm)
            {
                var items = new CreateFocForAftersalesPmModel();

                var itemTotalIncentive =
                    (from ass in createFocForAftersalesPms
                     where ass.EmployeeCode == others.EmployeeCode
                     select ass.FinalAmountTotal).ToList();

                var itemTotal = itemTotalIncentive.Sum();
                items.TotalShareIncentive1 = itemTotal;


                var itemSixtyShareIncentive =
                    (from ass in createFocForAftersalesPms
                     where ass.EmployeeCode == others.EmployeeCode
                     select ass.SixtyShareIncentive).ToList();

                var itemSixtyShare = itemSixtyShareIncentive.Sum();
                items.SixtyShareIncentive1 = itemSixtyShare;

                items.EmployeeCodes = others.EmployeeCode;

                createFocForAftersalesPms.Add(items);
            }

            var json = JsonConvert.SerializeObject(createFocForAftersalesPms);
            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetFocDataForPmHeadIncentive(string monthName, string monNum, string year, string employeeCode)
        {
            var createFocForAftersalesPms = new List<CreateFocForAftersalesPmModel>();
            var pmFocIns = _afterSales.GetFocDataForPmHeadIncentive(monthName, monNum, year, employeeCode);
            var asmHead =
             (from asm in _dbEntities.CmnUsers where asm.RoleName == "ASPMHEAD" && asm.IsActive == true select asm).FirstOrDefault();

            decimal totalFocIns = 0;

            foreach (var pmfoc in pmFocIns)
            {
                var items = new CreateFocForAftersalesPmModel();
                items.Id = pmfoc.Id;
                items.ProjectId = pmfoc.ProjectId;
                items.ProjectName = pmfoc.ProjectName;
                items.SpareName = pmfoc.SpareName;
                items.EmployeeCode = pmfoc.EmployeeCode;
                items.FocConfirmedDate = pmfoc.FocConfirmedDate;
                items.InventoryEntryDate = pmfoc.InventoryEntryDate;
                items.UnitPrice = pmfoc.UnitPrice;
                items.IncentiveRemarks = pmfoc.IncentiveRemarks;
                items.DeductionAmount = pmfoc.DeductionAmount;
                if (items.DeductionAmount == null)
                {
                    items.DeductionAmount = 0;
                }
                items.D_Remarks = pmfoc.D_Remarks;
                items.FinalAmount = pmfoc.FinalAmount;
                if (items.UnitPrice == null)
                {
                    items.UnitPrice = 0;
                }
                totalFocIns += Convert.ToDecimal(items.UnitPrice);
                items.FinalAmountTotal = Convert.ToDecimal(totalFocIns);

                if (items.EmployeeCode == asmHead.EmployeeCode)
                {
                    items.TotalShareIncentive = items.FinalAmountTotal;
                }
                createFocForAftersalesPms.Add(items);
            }
            var json = JsonConvert.SerializeObject(createFocForAftersalesPms);
            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #region Duplicate Check for Incentive

        [HttpPost]
        public JsonResult AftersalesMonthlyIncentive(string objArr)
        {
            List<Custom_Pm_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Pm_IncentiveModel>>(objArr);
            Console.Write("result :" + results);

            bool isExist = false;
            if (results.Count != 0)
            {
                isExist = _afterSales.GetIncentiveTypeData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message2"] = "Incentive already generated";
                return Json("AftersalesMonthlyIncentive", "AftersalesPm");
            }
            //bool isExist = false;
            //if (results.Count != 0)
            //{
            //    isExist = _afterSales.GetFocDataForAll(results[0].EmployeeCode, results[0].MonNum, results[0].Year);
            //}

            //if (isExist)
            //{
            //    TempData["Message1"] = "All Incentive is already generated";
            //    return Json("AftersalesMonthlyIncentive", "AftersalesPm");
            //}
            var SaveIncentive = "0";

            if (results.Count != 0)
            {
                SaveIncentive = _afterSales.SaveAftersalesPmMonthlyIncentive(results);
            }
            return Json(new { SaveIncentive }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveOthersIncentiveTypesDetails(string othersObjArr)
        {
            var results =
                JsonConvert.DeserializeObject<List<Custom_Pm_IncentiveModel>>(othersObjArr);
            Console.Write(othersObjArr);

            bool isExist = false;
            if (results.Count != 0)
            {
                isExist = _afterSales.GetIncentiveTypeDataForOthers(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message3"] = "Others Incentive already generated";
                return Json("AftersalesMonthlyIncentive", "AftersalesPm");
            }

            var saveOthersIncentiveTypes = "0";

            if (results.Count != 0)
            {
                saveOthersIncentiveTypes = _afterSales.SaveOthersIncentive(results);
            }

            return Json(new { saveOthersIncentiveTypes }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult SaveFocForHeadDetails(string focForHeadObjArr)
        {
            var results =
                JsonConvert.DeserializeObject<List<Custom_Pm_IncentiveModel>>(focForHeadObjArr);
            Console.Write(focForHeadObjArr);

            bool isExist = false;
            if (results.Count != 0)
            {
                isExist = _afterSales.GetFocDataForHead(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message4"] = "FOC Incentive for Head already generated";
                return Json("AftersalesMonthlyIncentive", "AftersalesPm");
            }

            var saveOthersIncentiveTypes = "0";

            if (results.Count != 0)
            {
                saveOthersIncentiveTypes = _afterSales.SaveFocForHeadDetails(results);
            }

            return Json(new { saveOthersIncentiveTypes }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveFocForAllDetails(string focForAllObjArr, string employeeCode)
        {
            var results =
                JsonConvert.DeserializeObject<List<Custom_Pm_IncentiveModel>>(focForAllObjArr);
            Console.Write(focForAllObjArr);

            bool isExist = false;
            if (results.Count != 0)
            {
                isExist = _afterSales.GetFocDataForAll(employeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message5"] = "FOC Incentive for All already generated";
                return Json("AftersalesMonthlyIncentive", "AftersalesPm");
            }

            var saveOthersIncentiveTypes = "0";

            if (results.Count != 0)
            {
                saveOthersIncentiveTypes = _afterSales.SaveFocForAllDetails(results, employeeCode);
            }
            return Json(new { saveOthersIncentiveTypes }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult SaveTotalAftersalesPmIncentive(string EmpCode, string Month, string MonNum, string Year)
        {
            var asmHead =
          (from asm in _dbEntities.CmnUsers where asm.RoleName == "ASPMHEAD" && asm.IsActive == true select asm).FirstOrDefault();

            var otherAsm =
         (from asm in _dbEntities.CmnUsers where asm.RoleName == "ASPM" && asm.IsActive == true select asm).ToList();

            List<Custom_Pm_IncentiveModel> cmList = new List<Custom_Pm_IncentiveModel>();
            var pmIncentiveModels = _afterSales.GetAftersalesPmIncentiveType(EmpCode, MonNum, Year);

            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
                cmList.Add(items);
            }
            var totalIncentiveType = cmList.Sum(i => i.FinalAmount1);
            if (EmpCode == asmHead.EmployeeCode)
            {
                pmIncentiveModels = _afterSales.GetAftersalesPm_FocIncentiveForHead(EmpCode, MonNum, Year);
                foreach (var customPmIncentiveModel in pmIncentiveModels)
                {
                    Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                    items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
                    items.TotalIncentiveForFoc = customPmIncentiveModel.FinalAmount1;
                    cmList.Add(items);
                }

                pmIncentiveModels = _afterSales.GetAftersalesPm_FocIncentiveForHeadFromOthers(EmpCode, MonNum, Year);
                foreach (var customPmIncentiveModel in pmIncentiveModels)
                {
                    foreach (var others in otherAsm)
                    {
                        if (customPmIncentiveModel.EmployeeCode == others.EmployeeCode)
                        {
                            Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                            items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;

                            const int fourty = 40;
                            items.FinalAmount1 = (items.FinalAmount1 * fourty) / 100;
                            items.TotalIncentiveForFoc = items.FinalAmount1;

                            cmList.Add(items);
                        }
                    }
                }
            }

            if (EmpCode != asmHead.EmployeeCode)
            {
                pmIncentiveModels = _afterSales.GetAftersalesPm_FocIncentiveForOthers(EmpCode, MonNum, Year);
                foreach (var customPmIncentiveModel in pmIncentiveModels)
                {
                    Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                    items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;

                    const int sixty = 60;
                    items.FinalAmount1 = (items.FinalAmount1 * sixty) / 100;
                    items.TotalIncentiveForFoc = items.FinalAmount1;

                    cmList.Add(items);
                }
            }
            /////////////////////////////////////
            var totalAmount = cmList.Sum(i => i.FinalAmount1);
            var totalAmountForFoc = cmList.Sum(i => i.TotalIncentiveForFoc);
            bool isExist = false;
            if (totalAmount != null)
            {
                isExist = _afterSales.GetTotalIncentiveDataForDuplicateCheck(EmpCode, Convert.ToInt32(MonNum), Year);
            }

            if (isExist)
            {
                TempData["Message1"] = "All Incentive is already generated";
                return Json("AftersalesMonthlyIncentive", "AftersalesPm");
            }

            var saveTotalIncentive = "0";

            if (totalAmount != null)
            {
                saveTotalIncentive = _afterSales.SaveTotalAftersalesPmIncentive(totalAmount.ToString(), EmpCode, Month, MonNum, Year, totalIncentiveType, totalAmountForFoc);

            }

            return Json(new { data = saveTotalIncentive }, JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region New Incentive Policy
        public ActionResult AftersalesPm_IssueDetails(string MonYear)
        {
            var vmAfersales = new VmAftersalesIncentive();
            if (MonYear != null && MonYear != "")
            {
                ViewBag.GetAftersalesIssueDetails = _afterSales.GetAftersalesIssueDetails(MonYear);

            }
            vmAfersales.MonYear = MonYear;

            return View(vmAfersales);
        }

        public ActionResult AftersalesPm_IssuePercentage(string monName, string years, string ids)
        {
            long genIds;
            long.TryParse(ids, out genIds);
            long yYear;
            long.TryParse(years, out yYear);
            ViewBag.GetAftersalesIssueDetails1 = _afterSales.GetAftersalesIssueDetails1(ids);
            ViewBag.GetAftersaleUsers = _afterSales.GetAftersaleUsers(genIds, monName, yYear);
            return View();
        }

        public JsonResult SaveAftersalesPercentageData(string objIssueArr, string generalIncId)
        {

            List<VmAftersalesIncentive> results = JsonConvert.DeserializeObject<List<VmAftersalesIncentive>>(objIssueArr);
            Console.Write("result :" + results);

            long genIds;
            long.TryParse(generalIncId, out genIds);
            //bool isExist = false;

            //if (results.Count != 0)
            //{
            //    isExist = _afterSales.GetFieldByHeadData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            //}

            //if (isExist)
            //{
            //    TempData["Message2"] = "Generated";
            //    return Json("All_QcMembersMonthlyIncentive", "Software");
            //}
            var saveSwIncentive = "0";
            if (results.Count != 0)
            {
                saveSwIncentive = _afterSales.SaveAftersalesPercentageData(results, genIds);
            }

            return Json(new { data = saveSwIncentive }, JsonRequestBehavior.AllowGet);
        }
        //Month=' + monthName + '&MonNum=' + monthNum + '&Year=' + yearName
        public ActionResult AftersalesPm_Incentive_TopSheet(string Month, string MonNum, string Year)
        {
            int monNum;
            int.TryParse(MonNum, out monNum);

            int yYear;
            int.TryParse(Year, out yYear);

            ViewBag.ShowTeamIncentive = _afterSales.ShowTeamIncentive(monNum, yYear);
            ViewBag.GetPreparedUser = _afterSales.GetPreparedUserName();
            return View();
        }

        public ActionResult AftersalesPm_Incentive_PerPersonSheet(string EmpCode, string Month, string MonNum, string Year)
        {
            List<VmAftersalesIncentive> cmList = new List<VmAftersalesIncentive>();
            // var pmIncentiveModels = _afterSales.GetAftersalesPmIncentiveForPrint(EmpCode, MonNum, Year);
            ViewBag.GetAftersalesPmIncentivePerPerson = _afterSales.GetAftersalesPmIncentivePerPerson(EmpCode, MonNum, Year);

            //foreach (var customPmIncentiveModel in pmIncentiveModels)
            //{
            //    VmAftersalesIncentive items = new VmAftersalesIncentive();
            //    items.GeneralIncidentTitle = customPmIncentiveModel.GeneralIncidentTitle;
            //    items.GeneralIncidentCategories = customPmIncentiveModel.GeneralIncidentCategories;
            //    items.GeneralIncidentDetails = customPmIncentiveModel.GeneralIncidentDetails;
            //    items.IssueRaisedDate = customPmIncentiveModel.IssueRaisedDate;
            //    items.SolutionDate = customPmIncentiveModel.SolutionDate;
            //    items.DaysPassed = customPmIncentiveModel.DaysPassed;
            //    items.EmpName = customPmIncentiveModel.EmpName;
            //    items.EmployeeCode = customPmIncentiveModel.EmployeeCode;
            //    items.Percentage = customPmIncentiveModel.Percentage;
            //    items.TotalAmount = customPmIncentiveModel.TotalAmount;
            //    items.PerPersonAmount = customPmIncentiveModel.PerPersonAmount;
            //    items.IncentiveRemarks = customPmIncentiveModel.IncentiveRemarks;

            //    cmList.Add(items);
            //}

            ViewBag.GetPreparedUser = _afterSales.GetPreparedUserName();
            //ViewBag.GetTotalFinalIncentiveOfPm = _afterSales.GetTotalFinalIncentiveOfAftersalesPm(EmpCode, MonNum, Year);
            ViewBag.GetTotalFinalIncentiveOfPm = _afterSales.GetTotalFinalIncentiveOfPerPm(EmpCode, MonNum, Year);

            return View();
        }

        public ActionResult AftersalesPmHolidays()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetHoliday()
        {
            var getHolidays = _afterSales.GetHoliday();
            var json = JsonConvert.SerializeObject(getHolidays);
            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }
        //SaveHolidayNewData
        [HttpPost]
        public JsonResult SaveHolidayNewData(string Id, string GovernmentHoliday, string HolidayStartDate, string HolidayEndDate)
        {
            var saveData = _afterSales.SaveHolidayNewData(Id, GovernmentHoliday, HolidayStartDate, HolidayEndDate);

            return Json(new { data = saveData }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveHolidayDropData(string Id, string GovernmentHoliday, string HolidayStartDate, string HolidayEndDate)
        {
            var saveData = _afterSales.SaveHolidayDropData(Id, GovernmentHoliday, HolidayStartDate, HolidayEndDate);

            return Json(new { data = saveData }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveHolidayResizeData(string Id, string GovernmentHoliday, string HolidayStartDate, string HolidayEndDate)
        {
            var saveData = _afterSales.SaveHolidayResizeData(Id, GovernmentHoliday, HolidayStartDate, HolidayEndDate);

            return Json(new { data = saveData }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DeleteHolidayData(string Id)
        {
            var deleteEvent = _afterSales.DeleteHolidayData(Id);

            return Json(new { data = deleteEvent }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult AftersalesPmIncentiveSheet()
        {
            var customInc = new Custom_Pm_IncentiveModel();
            customInc.CmnUserModelsList = _afterSales.GetAftersalesPmUserList();
            return View(customInc);
        }
        [HttpGet]
        public ActionResult AftersalesPmIncentiveReportPerPerson(string EmpCode, string Month, string MonNum, string Year)
        {
            List<Custom_Pm_IncentiveModel> cmList = new List<Custom_Pm_IncentiveModel>();
            var pmIncentiveModels = _afterSales.GetAftersalesPmIncentiveForPrint(EmpCode, MonNum, Year);

            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                items.ProjectName = customPmIncentiveModel.ProjectName;
                items.EmployeeCode = customPmIncentiveModel.EmployeeCode;
                items.IncentiveTypes = customPmIncentiveModel.IncentiveTypes;
                items.SpareName = customPmIncentiveModel.SpareName;
                items.PersonNo = customPmIncentiveModel.PersonNo;
                items.Amount1 = customPmIncentiveModel.Amount1;
                items.Remarks = customPmIncentiveModel.Remarks;
                items.DeductionAmount1 = customPmIncentiveModel.DeductionAmount1;
                items.D_Remarks = customPmIncentiveModel.D_Remarks;
                items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
                cmList.Add(items);
            }
            pmIncentiveModels = _afterSales.GetAftersalesPmFocIncentiveForPrint(EmpCode, MonNum, Year);
            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                items.ProjectName = customPmIncentiveModel.ProjectName;
                items.EmployeeCode = customPmIncentiveModel.EmployeeCode;
                items.IncentiveTypes = customPmIncentiveModel.IncentiveTypes;
                items.SpareName = customPmIncentiveModel.SpareName;
                items.PersonNo = customPmIncentiveModel.PersonNo;
                items.Amount1 = customPmIncentiveModel.Amount1;
                items.Remarks = customPmIncentiveModel.Remarks;
                items.DeductionAmount1 = customPmIncentiveModel.DeductionAmount1;
                items.D_Remarks = customPmIncentiveModel.D_Remarks;
                items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
                cmList.Add(items);
            }

            //pmIncentiveModels = _afterSales.GetAftersalesPmFocIncentiveForPrint(EmpCode, MonNum, Year);
            //foreach (var customPmIncentiveModel in pmIncentiveModels)
            //{
            //    Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
            //    items.ProjectName = customPmIncentiveModel.ProjectName;
            //    items.EmployeeCode = customPmIncentiveModel.EmployeeCode;
            //    items.IncentiveTypes = customPmIncentiveModel.IncentiveTypes;
            //    items.SpareName = customPmIncentiveModel.SpareName;
            //    items.PersonNo = customPmIncentiveModel.PersonNo;
            //    items.Amount1 = customPmIncentiveModel.Amount1;
            //    items.Remarks = customPmIncentiveModel.Remarks;
            //    items.DeductionAmount1 = customPmIncentiveModel.DeductionAmount1;
            //    items.D_Remarks = customPmIncentiveModel.D_Remarks;
            //    items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
            //    cmList.Add(items);
            //}

            //ViewBag.PmIncentiveForPrint = cmList;
            ViewBag.GetPreparedUser = _afterSales.GetPreparedUserName();
            ViewBag.GetTotalFinalIncentiveOfPm = _afterSales.GetTotalFinalIncentiveOfAftersalesPm(EmpCode, MonNum, Year);

            return View(cmList);
        }
        [HttpGet]
        public ActionResult AftersalesPmIncentiveReportTopSheet(string Month, string MonNum, string Year)
        {
            List<Custom_Pm_IncentiveModel> customInc = new List<Custom_Pm_IncentiveModel>();
            customInc = _afterSales.AftersalesPmIncentiveForAllPerson(Month, MonNum, Year);
            ViewBag.GetPreparedUser = _afterSales.GetPreparedUserName();
            return View(customInc);
        }
        public ActionResult IncentiveIssueDetails()
        {
            return View();
        }
        #endregion

        #region Aftersales Issue Handling

        public ActionResult AftersalesIssueVerification()
        {
            var fileManager = new FileManager();
            var vmModel = new VmIssueVerification();
            vmModel.ProjectMasterModels = _afterSales.GetModelsForAftersalesIssueVerification();
            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Select One" } };

            foreach (var t in vmModel.ProjectMasterModels)
            {
                selectListItems.Add(new SelectListItem
                {
                    Value = Convert.ToString(t.ProjectMasterId),
                    Text = t.ProjectName
                });
            }
            ViewBag.ProjectName = selectListItems;

            vmModel.AftersalesPmIssueVerificationModels = _afterSales.GetIssueVerificationList();

            if (vmModel.AftersalesPmIssueVerificationModels.Any())
            {
                foreach (AftersalesPm_IssueVerificationModel model in vmModel.AftersalesPmIssueVerificationModels)
                {
                    if (model.SupportingDocument != null)
                    {
                        var urls = model.SupportingDocument.Split('|').ToList();
                        for (int i = 0; i < urls.Count; i++)
                        {
                            FilesDetail detail = new FilesDetail();
                            detail.FilePath = fileManager.GetFile(urls[i]);
                            detail.Extention = fileManager.GetExtension(urls[i]);
                            model.FilesDetails.Add(detail);
                        }
                    }
                    if (model.DocumentUploadedByQc != null)
                    {
                        var urls = model.DocumentUploadedByQc.Split('|').ToList();
                        for (int i = 0; i < urls.Count; i++)
                        {
                            FilesDetail detail1 = new FilesDetail();
                            detail1.FilePath = fileManager.GetFile(urls[i]);
                            detail1.Extention = fileManager.GetExtension(urls[i]);
                            model.FilesDetails1.Add(detail1);
                        }
                    }
                }
            }

            List<SwQcTestPhaseModel> testPhaseList = _afterSales.GetSwQcTestPhasesForPm();
            ViewBag.ddlTestPhasesList = testPhaseList;

            return View(vmModel);
        }
       [HttpPost]
        public ActionResult AftersalesIssueVerification(List<AftersalesPm_IssueVerificationModel> issueList, string ProjectMasterId)
        {
            var manager = new FileManager();
            var Attachment = "";

           long proIds;
           long.TryParse(ProjectMasterId, out proIds);

           var query = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == proIds);

            foreach (var items in issueList)
            {
                if (items.UploderDocs[0] !=null)
                {
                    var res = manager.UploadAnotherDrive(proIds, query.ProjectName, "VerificationIssueByAspm", items.UploderDocs);
                    Console.Write("res  :" + res);
                    items.SupportingDocument = items.SupportingDocument == null ? res : items.SupportingDocument + "|" + res;
                    Attachment = items.SupportingDocument;
                }
            }

            _afterSales.SaveIntoAftersalesIssueVerification(issueList, proIds, query.ProjectName, Attachment);

            return RedirectToAction("AftersalesIssueVerification");
        }
       [HttpPost]
       public JsonResult UpdateIssueConfirmationStatus(String issueIds)
       {
           long ids;
           long.TryParse(issueIds, out ids);

           var saveIncentive = "0";

           if (ids != 0)
           {
               saveIncentive = _afterSales.UpdateIssueConfirmationStatus(ids);
           }
           return Json(new { data = saveIncentive }, JsonRequestBehavior.AllowGet);
       }
       //UpdateActionStatus
       [HttpPost]
       public JsonResult UpdateActionStatus(String issueIds, String selectedValAction)
       {
           long ids;
           long.TryParse(issueIds, out ids);

           var saveData = "0";

           if (ids != 0)
           {
               saveData = _afterSales.UpdateActionStatus(ids, selectedValAction);
           }
           return Json(new { data = saveData }, JsonRequestBehavior.AllowGet);
       }
       [NotificationActionFilter(ReceiverRoles = "QCHEAD,PMHEAD,MM,PS,ASPM,ASPMHEAD")]
       [HttpPost]
       [Authorize(Roles = "PM,PMHEAD,ASPM,ASPMHEAD")]
       public JsonResult ProjectForwardToSoftwareQc(long issueIds, string pmRemarks, string projectMasterId, string projectPmAssignId, string projectManagerUserId, List<string> selectedSampleValue, string sampleNumber, string testPhase, string swVersionNo, string versionName, string sourceVersion, string targetVersion, string ActionType)
       {
           var sWQcInchargeAssignResult = "";
           long userId = Convert.ToInt64(User.Identity.Name);
           long swWcInchargeAssignUserId = _afterSales.GetUserIdByRoleName("QCHEAD");
           long pMasterId, pMAssignId, pmUserId, sampleNo, testPhasefrPm, swVersionNumber;
           long.TryParse(projectMasterId, out pMasterId);
           long.TryParse(projectPmAssignId, out pMAssignId);
           long.TryParse(projectManagerUserId, out pmUserId);
           long.TryParse(sampleNumber, out sampleNo);
           long.TryParse(testPhase, out testPhasefrPm);
           long.TryParse(swVersionNo, out swVersionNumber);
         
           var checkDuplicateAssign = _afterSales.CheckSwQcInchargeDuplicateAssign(pMasterId);

           sWQcInchargeAssignResult = _afterSales.AssignProjectPmToSwQcHead(issueIds, pmRemarks, pMasterId, pMAssignId, pmUserId, selectedSampleValue, sampleNo, userId, swWcInchargeAssignUserId, testPhasefrPm, swVersionNumber, versionName, sourceVersion, targetVersion, ActionType);

           //----------MAIL Start-----------------------
           var project = _afterSales.GetProjectMasterModel(pMasterId);
           var user = _afterSales.GetPmUserInfo(userId);

           var qq1 = _dbEntities.AftersalesPm_IssueVerification.FirstOrDefault(i => i.Id == issueIds);
           if (qq1 != null && qq1.Status == "FullSoftwareChecked")
           {
               MailSendFromPms mailSendFromPms = new MailSendFromPms();
               mailSendFromPms.SendMail(new List<string>(new[] { "QCHEAD" }), new List<string>(new[] { "MM", "ASPM", "ASPMHEAD", "CM", "SA", "PS" }), "Full Software is Forwarded to Software QC for checking purpose",
                   "Project : <b>" + project.ProjectName + "</b> Forwarded for QC from ProjectManager to Software QC.<br/>Forwarded By : " + user.UserFullName
                   + "<br/>Sample Type : " + selectedSampleValue + "<br/>Sample No : " + sampleNo + "<br/>Software Version Name : " + versionName + "<br/>Software Version No. : " + swVersionNo);
               //---------------ends-----------------

           }
           else if (qq1 != null && qq1.Status == "FOTATestResult")
           {
               MailSendFromPms mailSendFromPms = new MailSendFromPms();
               mailSendFromPms.SendMail(new List<string>(new[] { "QCHEAD" }), new List<string>(new[] { "MM", "ASPM", "ASPMHEAD", "CM", "SA", "PS" }), "FOTA Test is Forwarded to Software QC for checking purpose",
                   "Project : <b>" + project.ProjectName + "</b> Forwarded for QC from ProjectManager to Software QC.<br/>Forwarded By : " + user.UserFullName
                   + "<br/>Sample Type : " + selectedSampleValue + "<br/>Sample No : " + sampleNo + "<br/>Software Version Name : " + versionName + "<br/>Software Version No. : " + swVersionNo);
               //---------------ends-----------------

           }

           
           var notificationObject = new NotificationObject
           {
               ProjectId = pMasterId,
               ToUser = "-1",
           };
           notificationObject.Message = "  forwarded a project for software QC ";
           notificationObject.AdditionalMessage = "";
           ViewBag.ControllerVariable = notificationObject;

           return Json(sWQcInchargeAssignResult, JsonRequestBehavior.AllowGet);
       }
       [HttpPost]
       [Authorize(Roles = "PM,PMHEAD,ASPM,ASPMHEAD")]
       public JsonResult FullSoftwareVersionCheckedOrNot(long issueIds)
       {

           var checkFullVersionStatus = _afterSales.FullSoftwareVersionCheckedOrNot(issueIds);

           return Json(checkFullVersionStatus, JsonRequestBehavior.AllowGet);
       }

       [HttpPost]
       public JsonResult SoftwareVersionNameLoadForAll(string swVersionNo, string projectId, string testPhase)
         {
           var _dbEntities = new CellPhoneProjectEntities();
           long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name);
           var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);

           long swVerNo;
           long.TryParse(swVersionNo, out swVerNo);

           long proId;
           long.TryParse(projectId, out proId);

           long testPhases;
           long.TryParse(testPhase, out testPhases);

           var getVersionName = _afterSales.GetAllVersionNameForPm(swVerNo, proId, testPhases);

           var jsondatas = JsonConvert.SerializeObject(getVersionName);

           //if (jsondatas != null)
           //{
           //    jsondatas = jsondatas;
           //}
           //else
           //{
           //    jsondatas = "0";
           //}
           return new JsonResult { Data = jsondatas, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
       }
       [HttpPost]
       public JsonResult SaveSupplierDetails(String IssueVerificationId, String Details)
       {
           var _dbEntities = new CellPhoneProjectEntities();
           var manager = new FileManager();

           var saveIncentive = "";
           long Ids = 0;
           long.TryParse(IssueVerificationId, out Ids);

           var Attachment = "";
           var supportingDocument = "";

           HttpFileCollectionBase files = Request.Files;
           for (int i = 0; i < files.Count; i++)
           {
               HttpPostedFileBase file = files[i];
               if (file != null)
               {
                   //var res = manager.DocManagementUpload(ModelName, "VerificationIssueByQc", file);
                   var res = manager.Upload(Ids, "SuplierFeedback", "SuplierFeedbackByASPM", file);
                   Console.Write("res  :" + res);
                   supportingDocument = supportingDocument == null ? res : supportingDocument + "|" + res;
                   Attachment = supportingDocument;
               }
           }

           if (Ids > 0)
           {
               saveIncentive = _afterSales.SaveSupplierDetails(Ids, Details, Attachment);
           }

           return new JsonResult { Data = saveIncentive, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
       }

        #endregion
    }
}