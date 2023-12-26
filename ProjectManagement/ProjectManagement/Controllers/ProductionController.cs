using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Infrastructures.Repositories;
using ProjectManagement.Models;
using ProjectManagement.ViewModels.Production;

namespace ProjectManagement.Controllers
{
    [Authorize(Roles = "PRD,MM,PM,PS,PMHEAD,CMHEAD,SA")]
    public class ProductionController : Controller
    {
        private IProductionRepository _repository;

        public ProductionController(ProductionRepository repository)
        {
            _repository = repository;
        }
        public ActionResult Index()
        {
            //ViewBag.ProductionTracker = _repository.GetProductionTrackerModels();
            return View();
        }
        [HttpPost]
        public ActionResult Index(ProductionTrackerModel model)
        {
            var fileManager = new FileManager();
            var filepath = fileManager.ImportExcelData("PRD", "ExcelImport", model.FilePath);
            if (filepath != "failed" && !string.IsNullOrWhiteSpace(filepath))
            {
                var e = _repository.ImportDataFromExcel2(filepath);
                if (e != "success") return Content(e);
            }
            else
            {
                return Content("upload failed");
            }

            //System.IO.File.Delete(filepath);
            return RedirectToAction("Index");
        }
        [HttpGet]

        #region old Plan
        public ActionResult ProductionPlan(string proId)
        {
            long proIds;
            long.TryParse(proId, out proIds);
            var vmAssembly = new VmAssemblyPackingProduction();


            // vmAssembly.ProjectMasterModelsList = _repository.GetProductionProjectList();
            List<SelectListItem> items = vmAssembly.ProjectMasterModelsList.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.Projects = items;

            vmAssembly.ProjectMasterModelsList = _repository.GetProjectOrders(proIds);
            List<SelectListItem> items1 = vmAssembly.ProjectMasterModelsList.Select(model => new SelectListItem { Text = model.OrderNuber.ToString(), Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.ProjectOrders = items1;


            /////////////////////below list///////
            List<VmAssemblyPackingProduction> productionEvent = _repository.GetProductionProject();
            ViewBag.ProductionEvents = productionEvent;

            /////////////////////Get saved Project////
            List<CustomPrdAssemblyAndPackingDetails> getPackingProduction = _repository.GetAssemblyAndPackingSavedProject();
            ViewBag.GetPackingProductions = getPackingProduction;

            /////////////////////Get Completed Project////
            List<CustomPrdAssemblyAndPackingDetails> getPackingCompletedPlan = _repository.GetAssemblyAndPackingCompletedProject();
            ViewBag.GetPackingCompletedPlans = getPackingCompletedPlan;


            return View(vmAssembly);
        }
        [HttpPost]
        public JsonResult AddedProjectPartialSave(string objArr)
        {

            List<CustomPrdAssemblyAndPackingDetails> results = JsonConvert.DeserializeObject<List<CustomPrdAssemblyAndPackingDetails>>(objArr);
            Console.Write("result :" + results);

            var saveIncentive = "0";

            if (results.Count != 0)
            {
                saveIncentive = _repository.AddedProjectPartialSaves(results);
            }


            return Json(new { SaveIncentive = saveIncentive }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProjectOrder(string proId)
        {
            long proIds;
            long.TryParse(proId, out proIds);

            var orderNumberList = _repository.GetProjectOrders(proIds);
            List<SelectListItem> itemss =
                orderNumberList.Select(
                    model =>
                        new SelectListItem
                        {
                            Text = model.OrderNuber.ToString(),
                            Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture)
                        }).ToList();

            var json = JsonConvert.SerializeObject(itemss);

            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        public JsonResult GetProjectPoCategorys(string proId)
        {
            long proIds;
            long.TryParse(proId, out proIds);

            ProjectMasterModel projectPoCategory = _repository.GetProjectPoCategory(proIds);

            var json = JsonConvert.SerializeObject(projectPoCategory);

            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        [HttpPost]
        public ActionResult AddProject(string projectName,
           long projectMasterId, string orderNum, string poCategory, string materialReceiveDate, string iqcCompleteDate, string trialProductionDate, string softwareConfirmationDate,
            string rnDClearanceDate, string assemblyLine, string assemblyProductionDate, string assemblyQuantity, string assemblyPerDayCapacity, string assemblyProductionEndDate, string packingLine,
            string packingStartDate, string packingQty, string packingPerDayCapacity, string packingProductionEndDate)
        {

            var model = new CustomPrdAssemblyAndPackingDetails
            {
                ProjectId = projectMasterId,
                ProjectName = projectName,
                PoCategory = poCategory,
                OrderNumber = orderNum,
                MaterialReceiveDate = Convert.ToDateTime(materialReceiveDate),
                IqcCompleteDate = Convert.ToDateTime(iqcCompleteDate),
                TrialProductionDate = Convert.ToDateTime(trialProductionDate),
                SoftwareConfirmationDate = Convert.ToDateTime(softwareConfirmationDate),
                RnDClearanceDate = Convert.ToDateTime(rnDClearanceDate),
                AssemblyLineInformation = assemblyLine,
                AssemblyProductionStartDate = Convert.ToDateTime(assemblyProductionDate),
                AssemblyQuantity = assemblyQuantity,
                AssemblyPerDayCapacity = assemblyPerDayCapacity,
                AssemblyProductionEndDate = Convert.ToDateTime(assemblyProductionEndDate),
                PackingLineInformation = packingLine,
                PackingProductionStartDate = Convert.ToDateTime(packingStartDate),
                PackingQuantity = packingQty,
                PackingPerDayCapacity = packingPerDayCapacity,
                PackingProductionEndDate = Convert.ToDateTime(packingProductionEndDate)
            };
            return PartialView("~/Views/Production/Partial/_PrdAssemblyAndPackingDetails.cshtml", model);
        }

        [HttpPost]
        public ActionResult UpdateAssemblyAndPackingTable(CustomPrdAssemblyAndPackingDetails assembAndPack)
        {

            var assembAndPack1 = _repository.UpdateAssemblyAndPackingTables(assembAndPack);

            return View(assembAndPack);
        }
        [HttpPost]
        public ActionResult InsertProductionRemark(CustomPrdAssemblyAndPackingDetails productionRemarksData)
        {

            var productionRemarksData1 = _repository.InsertProductionRemarks(productionRemarksData);

            return View(productionRemarksData);
        }
        [HttpPost]
        public ActionResult UpdateAssemblyAndPackingTableStatus(CustomPrdAssemblyAndPackingDetails assembAndPack)
        {

            var assembAndPack1 = _repository.UpdateAssemblyAndPackingTableStatuses(assembAndPack);

            return View(assembAndPack);
        }

        [HttpGet]
        public JsonResult GetGrandChartData(string dateArr2)
        {
            var results = JsonConvert.DeserializeObject<List<string>>(dateArr2);


            var assembAndPack1 = _repository.GetGrandChartDatas(results);
            // var json = JsonConvert.SerializeObject(assembAndPack1);

            //return Json(new { data = assembAndPack1 }, JsonRequestBehavior.AllowGet);
            return Json(new { data = assembAndPack1 }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetHolidayData(string dateForHoliday)
        {

            // GovernmentHolidayTableModel getHoliday=null;
            // if (dateForHoliday !="")
            // {
            //     getHoliday = _repository.GetHolidayDatas(dateForHoliday);

            // }

            //var json = JsonConvert.SerializeObject(getHoliday);

            bool getHoliday = false;
            if (dateForHoliday != "")
            {
                getHoliday = _repository.GetHolidayDatas(dateForHoliday);

            }

            var json = JsonConvert.SerializeObject(getHoliday);

            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetHolidayDataList()
        {
            //  List<GovernmentHolidayTableModel> getHoliday = null;

            List<GovernmentHolidayTableModel> getHoliday = _repository.GetHolidayDatasList();

            var json = JsonConvert.SerializeObject(getHoliday);

            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAssemblyLineData(string assemblyStartDate, string assemblyEndDate)
        {

            List<CustomPrdAssemblyAndPackingDetails> getAssembly = null;
            if (assemblyStartDate != null && assemblyEndDate != null)
            {
                getAssembly = _repository.GetAssemblyLineDatas(assemblyStartDate, assemblyEndDate);

            }

            var json = JsonConvert.SerializeObject(getAssembly);

            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetPackingLineData(string packingStartDate, string packingEndDate)
        {


            List<CustomPrdAssemblyAndPackingDetails> getPacking = null;
            if (packingStartDate != null && packingEndDate != null)
            {
                getPacking = _repository.GetPackingLineDatas(packingStartDate, packingEndDate);

            }

            var json = JsonConvert.SerializeObject(getPacking);

            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SelectLineInfo()
        {

            List<LineInformationModel> getHoliday = _repository.SelectLineInfos();

            var json = JsonConvert.SerializeObject(getHoliday);

            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }

        #region Charger Production
        public ActionResult ChargerPlan(string projectId, string projectName)
        {
            long proIds;
            long.TryParse(projectId, out proIds);

            var vnChargerProduction = new VmChargerProduction();
            vnChargerProduction.ProjectMasterModels = _repository.GetProductionProjectList();
            List<SelectListItem> items = vnChargerProduction.ProjectMasterModels.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.Projects = items;
            //ViewBag.SelectedVal = projectId;
            //ViewBag.SelectedTxt = projectName;

            return View(vnChargerProduction);
        }

        [HttpPost]
        public JsonResult ChargerPlanAllData(string objArr)
        {
            List<CustomChargerProduction> results = JsonConvert.DeserializeObject<List<CustomChargerProduction>>(objArr);
            Console.Write("result :" + results);

            var saveCharger = "0";
            string body = string.Empty;
            if (results.Count != 0)
            {
                saveCharger = _repository.AddedChargerPlanData(results);
                //body = "This is to inform you that a project has been assigned by  <b>" + user.UserFullName + " </b> for testing of Software QC issues.<br/><br/><br/> <br/>" + "Project : <b>" + proName.ProjectName + "</b> <br/>Assigned to : " + assignedQc + "<br/>Sample Type - " + swQcInchargeproInfo.ProjectManagerSampleType + "<br/>Sample Quantity - " + swQcInchargeproInfo.ProjectManagerSampleNo + "<br/>Test Phase Name - " + tPhaseName.TestPhaseName;
                //var mailSendFromPms = new MailSendFromPms();
                //mailSendFromPms.SendMail(ids, new List<string>(new[] { "MM", "SA", "PS" }), "Software QC has been assigned for a New Project(" + proName.ProjectName + ")", body);

                //body = "This is to inform you that a project has been assigned by  <b>" + user.UserFullName + " </b> for testing of Software QC issues.<br/><br/><br/> <br/>" + "Project : <b>" + proName.ProjectName + "</b> <br/>Assigned to : " + assignedQc + "<br/>Sample Type - " + swQcInchargeproInfo.ProjectManagerSampleType + "<br/>Sample Quantity - " + swQcInchargeproInfo.ProjectManagerSampleNo + "<br/>Test Phase Name - " + tPhaseName.TestPhaseName;
                //var mailSendFromPms = new MailSendFromPms();
                //mailSendFromPms.SendMail( new List<string>(new[] {"PRD" }), "Software QC has been assigned for a New Project(" + proName.ProjectName + ")", body);
            }

            return Json(new { SaveCharger = saveCharger }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetChargerGrandChartData(string dateArr2)
        {
            var results = JsonConvert.DeserializeObject<List<string>>(dateArr2);
            var chargerPro = _repository.GetChargerGrandChartDatas(results);
            // var json = JsonConvert.SerializeObject(chargerPro);

            return Json(new { data = chargerPro }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetMaterialReceiveForSmt(string materialReceiveStartDateSmt, string materialReceiveEndDateSmt)
        {
            //  var isExist = _repository.GetMaterialReceiveForSmt(materialReceiveStartDateSmt, materialReceiveEndDateSmt);
            var isExist = false;
            if (materialReceiveStartDateSmt != "" && materialReceiveEndDateSmt != "")
            {
                isExist = _repository.GetMaterialReceiveForSmt(materialReceiveStartDateSmt, materialReceiveEndDateSmt);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetIqcCompleteForSmt(string iqcCompleteStartDateSmt, string iqcCompleteEndDateSmt)
        {
            // var isExist = _repository.GetIqcCompleteForSmt(iqcCompleteStartDateSmt, iqcCompleteEndDateSmt);
            var isExist = false;
            if (iqcCompleteStartDateSmt != "" && iqcCompleteEndDateSmt != "")
            {
                isExist = _repository.GetIqcCompleteForSmt(iqcCompleteStartDateSmt, iqcCompleteEndDateSmt);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetTrialProductionDateForSmt(string trialProductionStartDateSmt, string trialProductionEndDateSmt)
        {
            //  var isExist = _repository.GetTrialProductionDateForSmt(trialProductionStartDateSmt, trialProductionEndDateSmt);
            var isExist = false;
            if (trialProductionStartDateSmt != "" && trialProductionEndDateSmt != "")
            {
                isExist = _repository.GetTrialProductionDateForSmt(trialProductionStartDateSmt, trialProductionEndDateSmt);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        public JsonResult GetMassProductionDateForSmt(string massProductionStartDateSmt, string massProductionEndDateSmt)
        {
            //  var isExist = _repository.GetMassProductionDateForSmt(massProductionStartDateSmt, massProductionEndDateSmt);
            var isExist = false;
            if (massProductionStartDateSmt != "" && massProductionEndDateSmt != "")
            {
                isExist = _repository.GetMassProductionDateForSmt(massProductionStartDateSmt, massProductionEndDateSmt);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        public JsonResult GetMaterialReceiveDateHousing(string materialReceiveStartDateHousing, string materialReceiveEndDateHousing)
        {
            // var isExist = _repository.GetMaterialReceiveDateHousing(materialReceiveStartDateHousing, materialReceiveEndDateHousing);
            var isExist = false;
            if (materialReceiveStartDateHousing != "" && materialReceiveEndDateHousing != "")
            {
                isExist = _repository.GetMaterialReceiveDateHousing(materialReceiveStartDateHousing, materialReceiveEndDateHousing);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        public JsonResult GetIqcCompleteDateHousing(string iqcCompleteStartDateHousing, string iqcCompleteEndDateHousing)
        {
            // var isExist = _repository.GetIqcCompleteDateHousing(iqcCompleteStartDateHousing, iqcCompleteEndDateHousing);
            var isExist = false;
            if (iqcCompleteStartDateHousing != "" && iqcCompleteEndDateHousing != "")
            {
                isExist = _repository.GetIqcCompleteDateHousing(iqcCompleteStartDateHousing, iqcCompleteEndDateHousing);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        public JsonResult GetTrialProductionDateHousing(string trialProductionStartDateHousing, string trialProductionEndDateHousing)
        {
            var isExist = false;
            if (trialProductionStartDateHousing != "" && trialProductionEndDateHousing != "")
            {
                isExist = _repository.GetTrialProductionDateHousing(trialProductionStartDateHousing, trialProductionEndDateHousing);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetHousingReliabilityDate(string housingReliabilityStartDateHousing, string housingReliabilityEndtDateHousing)
        {
            //  var isExist = _repository.GetHousingReliabilityDate(housingReliabilityStartDateHousing, housingReliabilityEndtDateHousing);
            var isExist = false;
            if (housingReliabilityStartDateHousing != "" && housingReliabilityEndtDateHousing != "")
            {
                isExist = _repository.GetHousingReliabilityDate(housingReliabilityStartDateHousing, housingReliabilityEndtDateHousing);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetHousingMassProduction(string housingMassProStartDateHousing, string housingMassProEndtDateHousing)
        {
            //  var isExist = _repository.GetHousingMassProduction(housingMassProStartDateHousing, housingMassProEndtDateHousing);
            var isExist = false;
            if (housingMassProStartDateHousing != "" && housingMassProEndtDateHousing != "")
            {
                isExist = _repository.GetHousingMassProduction(housingMassProStartDateHousing, housingMassProEndtDateHousing);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetMaterialReceiveDateAssembly(string materialReceiveStartDateAssembly, string materialReceiveEndDateAssembly)
        {
            var isExist = false;
            if (materialReceiveStartDateAssembly != "" && materialReceiveEndDateAssembly != null)
            {
                isExist = _repository.GetMaterialReceiveDateAssembly(materialReceiveStartDateAssembly, materialReceiveEndDateAssembly);
            }

            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetIqcCompleteDateAssembly(string iqcCompleteStartDateAssembly, string iqcCompleteEndDateAssembly)
        {
            var isExist = false;
            if (iqcCompleteStartDateAssembly != "" && iqcCompleteEndDateAssembly != "")
            {
                isExist = _repository.GetIqcCompleteDateAssembly(iqcCompleteStartDateAssembly, iqcCompleteEndDateAssembly);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetTrialProductionDateAssembly(string trialProductionStartDateAssembly, string trialProductionEndDateAssembly)
        {
            // var isExist = _repository.GetTrialProductionDateAssembly(trialProductionStartDateAssembly, trialProductionEndDateAssembly);
            var isExist = false;
            if (trialProductionStartDateAssembly != "" && trialProductionEndDateAssembly != "")
            {
                isExist = _repository.GetTrialProductionDateAssembly(trialProductionStartDateAssembly, trialProductionEndDateAssembly);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetRnDConfirmDateAssembly(string rnDConfirmStartDateAssembly, string rnDConfirmEndDateAssembly)
        {
            //  var isExist = _repository.GetRnDConfirmDateAssembly(rnDConfirmStartDateAssembly, rnDConfirmEndDateAssembly);
            var isExist = false;
            if (rnDConfirmStartDateAssembly != "" && rnDConfirmEndDateAssembly != "")
            {
                isExist = _repository.GetRnDConfirmDateAssembly(rnDConfirmStartDateAssembly, rnDConfirmEndDateAssembly);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAssemblyProduction(string assembStartDateAssembly, string assembEndDateAssembly)
        {
            var isExist = _repository.GetAssemblyProduction(assembStartDateAssembly, assembEndDateAssembly);

            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        public JsonResult GetChargerOldHistory(string projectId, string projectName)
        {
            long proIds;
            long.TryParse(projectId, out proIds);

            var chargerHistory = _repository.GetChargerOldHistory(proIds);


            return Json(new { data = chargerHistory }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SelectLineInfoChargerSmt()
        {

            List<LineInformationModel> getCharger = _repository.SelectLineInfoChargerSmt();

            var json = JsonConvert.SerializeObject(getCharger);



            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult SelectLineInfoChargerHousing()
        {

            List<LineInformationModel> getCharger = _repository.SelectLineInfoChargerHousing();

            var json = JsonConvert.SerializeObject(getCharger);

            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SelectLineInfoChargerAssembly()
        {

            List<LineInformationModel> getCharger = _repository.SelectLineInfoChargerAssembly();

            var json = JsonConvert.SerializeObject(getCharger);

            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAvailableProductionLineForSmt(string massProductionStartDateSmt, string massProductionEndDateSmt)
        {
            var json = "";
            if (massProductionStartDateSmt != null && massProductionEndDateSmt != null)
            {
                List<ChargerSMTLineCapacityDetailsModel> getCharger = _repository.GetAvailableProductionLineForSmt(massProductionStartDateSmt, massProductionEndDateSmt);

                json = JsonConvert.SerializeObject(getCharger);
            }
            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAvailableProductionLineForHousing(string housingMassProStartDateHousing, string housingMassProEndtDateHousing)
        {
            var json = "";
            if (housingMassProStartDateHousing != null && housingMassProEndtDateHousing != null)
            {
                List<ChargerHousingLineCapacityDetailsModel> getCharger = _repository.GetAvailableProductionLineForHousing(housingMassProStartDateHousing, housingMassProEndtDateHousing);

                json = JsonConvert.SerializeObject(getCharger);
            }
            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetAvailableProductionLineForAssembly(string assembStartDateAssembly, string assembEndDateAssembly)
        {
            var json = "";
            if (assembStartDateAssembly != null && assembEndDateAssembly != null)
            {
                List<ChargerAssemblyLineCapacityDetailsModel> getCharger = _repository.GetAvailableProductionLineForAssembly(assembStartDateAssembly, assembEndDateAssembly);

                json = JsonConvert.SerializeObject(getCharger);
            }
            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Mobile Production Plan
        public ActionResult BatteryPlan(string projectId, string projectName)
        {
            long proIds;
            long.TryParse(projectId, out proIds);

            var vmBattery = new VmBatteryProduction();
            vmBattery.ProjectMasterModels = _repository.GetProductionProjectList();
            List<SelectListItem> items = vmBattery.ProjectMasterModels.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.Projects = items;

            //  vmBattery.CustomBatteryProductions = _repository.GetSelectedProjectPlanningHistory(proIds, projectName);

            //vmBattery.ProjectMasterModels = _repository.GetProductionProjectList();
            //List<SelectListItem> items1 = vmBattery.ProjectMasterModels.Select(model => new SelectListItem { Text = model.SourcingType, Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture) }).ToList();
            //ViewBag.Projects1 = items1;

            /////////////////////Get Partial Project////
            List<CustomBatteryProduction> getPartialPro = _repository.GetPartialProject();
            ViewBag.GetPartialProject = getPartialPro;

            return View(vmBattery);
        }
        [HttpPost]
        public JsonResult GetBatteryOldHistory(string projectId, string projectName, string planIds)
        {
            long proIds;
            long.TryParse(projectId, out proIds);

            long planId;
            long.TryParse(planIds, out planId);

            var batteryHistory = _repository.GetBatteryOldHistory(proIds, planId);


            return Json(new { data = batteryHistory }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult MobileNewPlan(string projectId, string projectName, string planIds)
        {
            long proIds;
            long.TryParse(projectId, out proIds);
            long planId;
            long.TryParse(projectId, out planId);
            var vmBattery = new VmBatteryProduction();
            List<CustomBatteryProduction> getPartialPro = _repository.GetPartialProject();
            ViewBag.GetPartialProject = getPartialPro;

            vmBattery.CustomBatteryProductions = _repository.GetBatteryOldHistory(proIds, planId);

            return View(vmBattery);


            //long proIds;
            //long.TryParse(projectId, out proIds);

            //List<CustomBatteryProduction> cmList = new List<CustomBatteryProduction>();
            //var pmIncentiveModels = _repository.GetBatteryOldHistoryEdit(proIds);

            //foreach (var customPmIncentiveModel in pmIncentiveModels)
            //{
            //    CustomBatteryProduction items = new CustomBatteryProduction();

            //    items.AsmProjectId = customPmIncentiveModel.AsmProjectId;
            //    items.AsmPlanId = customPmIncentiveModel.AsmPlanId;
            //    items.SmtPlanId = customPmIncentiveModel.SmtPlanId;
            //    items.BbPlanId = customPmIncentiveModel.BbPlanId;
            //    items.BhPlanId = customPmIncentiveModel.BhPlanId;
            //    items.SmtId = customPmIncentiveModel.SmtId;
            //    items.BhId = customPmIncentiveModel.BhId;
            //    items.BbId = customPmIncentiveModel.BbId;
            //    items.AsmId = customPmIncentiveModel.AsmId;
            //    items.AsmProjectName = customPmIncentiveModel.AsmProjectName;

            //    items.MaterialReceiveStartDateBSmt = customPmIncentiveModel.MaterialReceiveStartDateBSmt;
            //    items.MaterialReceiveEndDateBSmt = customPmIncentiveModel.MaterialReceiveEndDateBSmt;
            //    items.MaterialReceiveStartDateBSmt = customPmIncentiveModel.MaterialReceiveStartDateBSmt;
            //    items.MaterialReceiveEndDateBSmt = customPmIncentiveModel.MaterialReceiveEndDateBSmt;
            //    items.IqcCompleteStartDateBSmt = customPmIncentiveModel.IqcCompleteStartDateBSmt;
            //    items.IqcCompleteEndDateBSmt = customPmIncentiveModel.IqcCompleteEndDateBSmt;
            //    items.TrialProductionStartDateBSmt = customPmIncentiveModel.TrialProductionStartDateBSmt;
            //    items.SmtTrialLine = customPmIncentiveModel.SmtTrialLine;
            //    items.TrialProductionEndDateBSmt = customPmIncentiveModel.TrialProductionEndDateBSmt;
            //    items.SmtMassProductionStartDateBSmt = customPmIncentiveModel.SmtMassProductionStartDateBSmt;
            //    items.TotalQuantityBSmt = customPmIncentiveModel.TotalQuantityBSmt;
            //    items.SmtAllLineNumber = customPmIncentiveModel.SmtAllLineNumber;
            //    items.SmtAllLineCapacity = customPmIncentiveModel.SmtAllLineCapacity;
            //    items.BatterySmtPerDayCapacity = customPmIncentiveModel.BatterySmtPerDayCapacity;
            //    items.SmtMassProductionEndDateBSmt = customPmIncentiveModel.SmtMassProductionEndDateBSmt;

            //    items.MaterialReceiveStartDateBHousing = customPmIncentiveModel.MaterialReceiveStartDateBHousing;
            //    items.MaterialReceiveEndDateBHousing = customPmIncentiveModel.MaterialReceiveEndDateBHousing;
            //    items.IqcCompleteStartDateBHousing = customPmIncentiveModel.IqcCompleteStartDateBHousing;
            //    items.IqcCompleteEndDateBHousing = customPmIncentiveModel.IqcCompleteEndDateBHousing;
            //    items.TrialProductionStartDateBHousing = customPmIncentiveModel.TrialProductionStartDateBHousing;
            //    items.TrialProductionEndDateBHousing = customPmIncentiveModel.TrialProductionEndDateBHousing;
            //    items.HousingReliabilityTestStartDateBHousing = customPmIncentiveModel.HousingReliabilityTestStartDateBHousing;
            //    items.HousingReliabilityTestEndDateBHousing = customPmIncentiveModel.HousingReliabilityTestEndDateBHousing;
            //    items.HousingMassProductionStartDateBHousing = customPmIncentiveModel.HousingMassProductionStartDateBHousing;
            //    items.HousingMassProductionEndDateBHousing = customPmIncentiveModel.HousingMassProductionEndDateBHousing;
            //    items.TotalQuantity = customPmIncentiveModel.TotalQuantity;
            //    items.HousingAllLineNumber = customPmIncentiveModel.HousingAllLineNumber;
            //    items.BatteryHousingPerDayCapacity = customPmIncentiveModel.BatteryHousingPerDayCapacity;
            //    items.HousingAllLineCapacity = customPmIncentiveModel.HousingAllLineCapacity;
            //    items.HousingTrialLine = customPmIncentiveModel.HousingTrialLine;

            //    items.MaterialReceiveStartDateBattery = customPmIncentiveModel.MaterialReceiveStartDateBattery;
            //    items.MaterialReceiveEndDateBattery = customPmIncentiveModel.MaterialReceiveEndDateBattery;
            //    items.IqcCompleteStartDateBattery = customPmIncentiveModel.IqcCompleteStartDateBattery;
            //    items.IqcCompleteEndDateBattery = customPmIncentiveModel.IqcCompleteEndDateBattery;
            //    items.TrialProductionStartDateBattery = customPmIncentiveModel.TrialProductionStartDateBattery;
            //    items.TrialProductionEndDateBattery = customPmIncentiveModel.TrialProductionEndDateBattery;
            //    items.BatteryReliabilityTestStartDate = customPmIncentiveModel.BatteryReliabilityTestStartDate;
            //    items.BatteryReliabilityTestEndDate = customPmIncentiveModel.BatteryReliabilityTestEndDate;
            //    items.BatteryMassProductionStartDate = customPmIncentiveModel.BatteryMassProductionStartDate;
            //    items.BatteryMassProductionEndDate = customPmIncentiveModel.BatteryMassProductionEndDate;
            //    items.TotalQuantityBattery = customPmIncentiveModel.TotalQuantityBattery;
            //    items.BatteryAllLineNumber = customPmIncentiveModel.BatteryAllLineNumber;
            //    items.BatteryPerDayCapacity = customPmIncentiveModel.BatteryPerDayCapacity;
            //    items.BatteryAllLineCapacity = customPmIncentiveModel.BatteryAllLineCapacity;
            //    items.BatteryTrialLine = customPmIncentiveModel.BatteryTrialLine;

            //    items.MaterialReceiveStartDateBAssembly = customPmIncentiveModel.MaterialReceiveStartDateBAssembly;
            //    items.MaterialReceiveEndDateBAssembly = customPmIncentiveModel.MaterialReceiveEndDateBAssembly;
            //    items.IqcCompleteStartDateBAssembly = customPmIncentiveModel.IqcCompleteStartDateBAssembly;
            //    items.IqcCompleteEndDateBAssembly = customPmIncentiveModel.IqcCompleteEndDateBAssembly;
            //    items.TrialProductionStartDateBAssembly = customPmIncentiveModel.TrialProductionStartDateBAssembly;
            //    items.AssemblyTrialLine = customPmIncentiveModel.AssemblyTrialLine;
            //    items.TrialProductionEndDateBAssembly = customPmIncentiveModel.TrialProductionEndDateBAssembly;
            //    items.SoftwareConfirmationStartDateBAssembly = customPmIncentiveModel.SoftwareConfirmationStartDateBAssembly;
            //    items.SoftwareConfirmationEndDateBAssembly = customPmIncentiveModel.SoftwareConfirmationEndDateBAssembly;
            //    items.RandDConfirmationStartDateBAssembly = customPmIncentiveModel.RandDConfirmationStartDateBAssembly;
            //    items.RandDConfirmationEndDateBAssembly = customPmIncentiveModel.RandDConfirmationEndDateBAssembly;
            //    items.AssemblyMassProductionStartDateBAssembly = customPmIncentiveModel.AssemblyMassProductionStartDateBAssembly;
            //    items.AssemblyMassProductionEndDateBAssembly = customPmIncentiveModel.AssemblyMassProductionEndDateBAssembly;
            //    items.TotalQuantityBAssembly = customPmIncentiveModel.TotalQuantityBAssembly;
            //    items.AssemblyAllLineNumber = customPmIncentiveModel.AssemblyAllLineNumber;
            //    items.BatteryAssemblyPerDayCapacity = customPmIncentiveModel.BatteryAssemblyPerDayCapacity;
            //    items.AssemblyAllLineCapacity = customPmIncentiveModel.AssemblyAllLineCapacity;
            //    items.PackingMassProductionStartDateBAssembly = customPmIncentiveModel.PackingMassProductionStartDateBAssembly;
            //    items.PackingMassProductionEndDateBAssembly = customPmIncentiveModel.PackingMassProductionEndDateBAssembly;
            //    items.TotalQuantityBPacking = customPmIncentiveModel.TotalQuantityBPacking;
            //    items.PackingAllLineNumber = customPmIncentiveModel.PackingAllLineNumber;
            //    items.BatteryPackingPerDayCapacity = customPmIncentiveModel.BatteryPackingPerDayCapacity;
            //    items.PackingAllLineCapacity = customPmIncentiveModel.PackingAllLineCapacity;

            //    cmList.Add(items);
            //}
            //return View(cmList);
        }

        [HttpGet]
        public JsonResult GetSelectedProjectPlanningHistory(string projectId, string projectName)
        {
            long proIds;
            long.TryParse(projectId, out proIds);
            //  var results = JsonConvert.DeserializeObject<List<string>>(dateArr2);
            var chargerPro = _repository.GetSelectedProjectPlanningHistory(proIds, projectName);
            // var json = JsonConvert.SerializeObject(chargerPro);

            return Json(new { data = chargerPro }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SelectLineInfoBatterySmt(string projectId)
        {
            long proIds;
            long.TryParse(projectId, out proIds);
            List<LineInformationModel> getBattery = null;

            if (proIds != 0)
            {
                getBattery = _repository.SelectLineInfoBatterySmt(proIds);
            }

            var json = JsonConvert.SerializeObject(getBattery);
            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SelectLineInfoBatteryHousing()
        {

            List<LineInformationModel> getBattery = _repository.SelectLineInfoBatteryHousing();
            var json = JsonConvert.SerializeObject(getBattery);
            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SelectLineInfoBattery()
        {
            List<LineInformationModel> getBattery = null;
            getBattery = _repository.SelectLineInfoBattery();

            var json = JsonConvert.SerializeObject(getBattery);
            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SelectLineInfoBatteryAssembly(string projectId)
        {
            long proIds;
            long.TryParse(projectId, out proIds);
            List<LineInformationModel> getBattery = null;

            if (proIds != 0)
            {
                getBattery = _repository.SelectLineInfoBatteryAssembly(proIds);
            }

            var json = JsonConvert.SerializeObject(getBattery);
            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SelectLineInfoBatteryPacking(string projectId)
        {
            long proIds;
            long.TryParse(projectId, out proIds);
            List<LineInformationModel> getBattery = null;

            if (proIds != 0)
            {
                getBattery = _repository.SelectLineInfoBatteryPacking(proIds);
            }

            var json = JsonConvert.SerializeObject(getBattery);
            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetAvailableProductionLineForBatteryAssembly(string assembStartDateBAssembly, string assembEndDateBAssembly, string projectId)
        {
            long proIds;
            long.TryParse(projectId, out proIds);
            var json = "";
            if (assembStartDateBAssembly != null && assembEndDateBAssembly != null)
            {
                List<BatteryAssemblyLineCapacityDetailModel> getCharger = _repository.GetAvailableProductionLineForBatteryAssembly(assembStartDateBAssembly, assembEndDateBAssembly, proIds);

                json = JsonConvert.SerializeObject(getCharger);
            }
            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetAvailableProductionLineForBatteryAssembly1(string assembStartDateBAssembly, string projectId)
        {
            long proIds;
            long.TryParse(projectId, out proIds);
            var json = "";
            if (assembStartDateBAssembly != null)
            {
                List<BatteryAssemblyLineCapacityDetailModel> getCharger = _repository.GetAvailableProductionLineForBatteryAssembly1(assembStartDateBAssembly, proIds);

                json = JsonConvert.SerializeObject(getCharger);
            }
            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetAvailableProductionLineForBatteryPacking(string packingMassProductionStartDateBAssembly, string packingMassProductionEndDateBAssembly, string projectId)
        {
            long proIds;
            long.TryParse(projectId, out proIds);
            var json = "";
            if (packingMassProductionStartDateBAssembly != null && packingMassProductionEndDateBAssembly != null)
            {
                List<BatteryPackingLineCapacityDetailModel> getCharger = _repository.GetAvailableProductionLineForBatteryPacking(packingMassProductionStartDateBAssembly, packingMassProductionEndDateBAssembly, proIds);

                json = JsonConvert.SerializeObject(getCharger);
            }
            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetAvailableProductionLineForBattery(string batteryMassProductionStartDate, string batteryMassProductionEndDate)
        {
            var json = "";
            if (batteryMassProductionStartDate != null && batteryMassProductionEndDate != null)
            {
                List<BatteryLineCapacityDetailModel> getCharger = _repository.GetAvailableProductionLineForBattery(batteryMassProductionStartDate, batteryMassProductionEndDate);

                json = JsonConvert.SerializeObject(getCharger);
            }
            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetAvailableProductionLineForBHousing(string housingMassProStartDateHousing, string housingMassProEndtDateHousing)
        {
            var json = "";
            if (housingMassProStartDateHousing != null && housingMassProEndtDateHousing != null)
            {
                List<BatteryHousingLineCapacityDetailModel> getCharger = _repository.GetAvailableProductionLineForBHousing(housingMassProStartDateHousing, housingMassProEndtDateHousing);

                json = JsonConvert.SerializeObject(getCharger);
            }
            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetAvailableProductionLineForBSmt(string massProductionStartDateBSmt, string massProductionEndDateBSmt, string projectId)
        {
            long proIds;
            long.TryParse(projectId, out proIds);
            var json = "";
            if (massProductionStartDateBSmt != null && massProductionEndDateBSmt != null)
            {
                List<BatterySMTLineCapacityDetailModel> getCharger = _repository.GetAvailableProductionLineForBSmt(massProductionStartDateBSmt, massProductionEndDateBSmt, proIds);

                json = JsonConvert.SerializeObject(getCharger);
            }
            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetMaterialReceiveDateBAssembly(string materialStartDateBAssembly, string materialReceiveEndDateBAssembly)
        {
            var isExist = false;
            if (materialStartDateBAssembly != "" && materialReceiveEndDateBAssembly != null)
            {
                isExist = _repository.GetMaterialReceiveDateBAssembly(materialStartDateBAssembly, materialReceiveEndDateBAssembly);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetIqcCompleteDateBAssembly(string iqcCompleteStartDateBAssembly, string iqcCompleteEndDateBAssembly)
        {
            var isExist = false;
            if (iqcCompleteStartDateBAssembly != "" && iqcCompleteEndDateBAssembly != "")
            {
                isExist = _repository.GetIqcCompleteDateBAssembly(iqcCompleteStartDateBAssembly, iqcCompleteEndDateBAssembly);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetTrialProductionDateBAssembly(string trialProductionStartDateBAssembly, string trialProductionEndDateBAssembly)
        {
            var isExist = false;
            if (trialProductionStartDateBAssembly != "" && trialProductionEndDateBAssembly != "")
            {
                isExist = _repository.GetTrialProductionDateBAssembly(trialProductionStartDateBAssembly, trialProductionEndDateBAssembly);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetAssemblyBProduction(string assembStartDateBAssembly, string assembEndDateBAssembly)
        {
            var isExist = false;
            if (assembStartDateBAssembly != "" && assembEndDateBAssembly != "")
            {
                isExist = _repository.GetAssemblyBProduction(assembStartDateBAssembly, assembEndDateBAssembly);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetPackingBProduction(string packingMassProductionStartDateBAssembly, string packingMassProductionEndDateBAssembly)
        {
            var isExist = false;
            if (packingMassProductionStartDateBAssembly != "" && packingMassProductionEndDateBAssembly != "")
            {
                isExist = _repository.GetPackingBProduction(packingMassProductionStartDateBAssembly, packingMassProductionEndDateBAssembly);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetBatteryMassProduction(string batteryMassProductionStartDate, string batteryMassProductionEndDate)
        {
            var isExist = false;
            if (batteryMassProductionStartDate != "" && batteryMassProductionEndDate != "")
            {
                isExist = _repository.GetBatteryMassProduction(batteryMassProductionStartDate, batteryMassProductionEndDate);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetTrialProductionDateBattery(string trialProductionStartDateBattery, string trialProductionEndDateBattery)
        {
            var isExist = false;
            if (trialProductionStartDateBattery != "" && trialProductionEndDateBattery != "")
            {
                isExist = _repository.GetTrialProductionDateBattery(trialProductionStartDateBattery, trialProductionEndDateBattery);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetIqcCompleteDateBattery(string iqcCompleteStartDateBattery, string iqcCompleteEndDateBattery)
        {
            var isExist = false;
            if (iqcCompleteStartDateBattery != "" && iqcCompleteEndDateBattery != "")
            {
                isExist = _repository.GetIqcCompleteDateBattery(iqcCompleteStartDateBattery, iqcCompleteEndDateBattery);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetMaterialReceiveDateBattery(string materialReceiveStartDateBattery, string materialReceiveEndDateBattery)
        {
            var isExist = false;
            if (materialReceiveStartDateBattery != "" && materialReceiveEndDateBattery != "")
            {
                isExist = _repository.GetMaterialReceiveDateBattery(materialReceiveStartDateBattery, materialReceiveEndDateBattery);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetHousingMassBProduction(string housingMassProStartDateHousing, string housingMassProEndtDateHousing)
        {
            var isExist = false;
            if (housingMassProStartDateHousing != "" && housingMassProEndtDateHousing != "")
            {
                isExist = _repository.GetHousingMassBProduction(housingMassProStartDateHousing, housingMassProEndtDateHousing);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetTrialBProduction(string trialProductionStartDateHousing, string trialProductionEndDateHousing)
        {
            var isExist = false;
            if (trialProductionStartDateHousing != "" && trialProductionEndDateHousing != "")
            {
                isExist = _repository.GetTrialBProduction(trialProductionStartDateHousing, trialProductionEndDateHousing);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetIqcCompleteDateBHousing(string iqcCompleteStartDateHousing, string iqcCompleteEndDateHousing)
        {
            var isExist = false;
            if (iqcCompleteStartDateHousing != "" && iqcCompleteEndDateHousing != "")
            {
                isExist = _repository.GetIqcCompleteDateBHousing(iqcCompleteStartDateHousing, iqcCompleteEndDateHousing);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetMaterialReceiveDateBHousing(string materialReceiveStartDateHousing, string materialReceiveEndDateHousing)
        {
            var isExist = false;
            if (materialReceiveStartDateHousing != "" && materialReceiveEndDateHousing != "")
            {
                isExist = _repository.GetMaterialReceiveDateBHousing(materialReceiveStartDateHousing, materialReceiveEndDateHousing);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetSmtMassBProduction(string massProductionStartDateBSmt, string massProductionEndDateBSmt)
        {
            var isExist = false;
            if (massProductionStartDateBSmt != "" && massProductionEndDateBSmt != "")
            {
                isExist = _repository.GetSmtMassBProduction(massProductionStartDateBSmt, massProductionEndDateBSmt);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetMaterialReceiveDateBSmt(string materialReceiveStartDateBSmt, string materialReceiveEndDateBSmt)
        {
            var isExist = false;
            if (materialReceiveStartDateBSmt != "" && materialReceiveEndDateBSmt != "")
            {
                isExist = _repository.GetMaterialReceiveDateBSmt(materialReceiveStartDateBSmt, materialReceiveEndDateBSmt);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetIqcCompleteDateBSmt(string iqcCompleteStartDateBSmt, string iqcCompleteEndDateBSmt)
        {
            var isExist = false;
            if (iqcCompleteStartDateBSmt != "" && iqcCompleteEndDateBSmt != "")
            {
                isExist = _repository.GetIqcCompleteDateBSmt(iqcCompleteStartDateBSmt, iqcCompleteEndDateBSmt);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetTrialProductionDateBSmt(string trialProductionStartDateBSmt, string trialProductionEndDateBSmt)
        {
            var isExist = false;
            if (trialProductionStartDateBSmt != "" && trialProductionEndDateBSmt != "")
            {
                isExist = _repository.GetTrialProductionDateBSmt(trialProductionStartDateBSmt, trialProductionEndDateBSmt);
            }
            if (isExist)
            {
                var saveSpare1 = "YES";
                return Json(new { SaveSpareData = saveSpare1 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { SaveSpareData = false }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult BatteryPlanAllData(string objArr)
        {
            var results = JsonConvert.DeserializeObject<List<CustomBatteryProduction>>(objArr);
            Console.Write("result :" + results);

            var saveBattery = "0";

            if (results.Count != 0)
            {
                saveBattery = _repository.SaveBatteryPlanData(results);
            }

            return Json(new { SaveCharger = saveBattery }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetBatteryGrandChartData(string dateArr2)
        {
            var results = JsonConvert.DeserializeObject<List<string>>(dateArr2);
            var chargerPro = _repository.GetBatteryGrandChartDatas(results);
            // var json = JsonConvert.SerializeObject(chargerPro);

            return Json(new { data = chargerPro }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetCkdGrandChartData(string dateArr2)
        {
            var results = JsonConvert.DeserializeObject<List<string>>(dateArr2);
            var chargerPro = _repository.GetCkdGrandChartData(results);
            // var json = JsonConvert.SerializeObject(chargerPro);

            return Json(new { data = chargerPro }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProductionReport(string projectId, string projectName)
        {
            long proIds;
            long.TryParse(projectId, out proIds);

            var vmBattery = new VmBatteryProduction();

            /////////////////////Get Partial Project////
            List<CustomBatteryProduction> getPartialPro = _repository.GetPartialProject();
            ViewBag.GetPartialProject = getPartialPro;

            return View(vmBattery);
        }
        #endregion

        #region Mobile Plan Edit
        [HttpGet]
        public JsonResult GetSmtTrialLineForEdit()
        {
            var results = _repository.GetSmtTrialLineForEdit();

            return Json(new { data = results }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateChdPlanning(CustomBatteryProduction allInfo)
        {

            var allInfo1 = _repository.UpdateChdPlanning(allInfo);

            // return View(allInfo1);
            return Json(new { data = allInfo1 }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult CkdEdit(string projectId, string projectName, string planId)
        {
            long proIds;
            long.TryParse(projectId, out proIds);

            long planIds;
            long.TryParse(planId, out planIds);


            List<CustomBatteryProduction> cmList = new List<CustomBatteryProduction>();
            var pmIncentiveModels = _repository.GetBatteryOldHistoryEdit(proIds, planIds);

            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                CustomBatteryProduction items = new CustomBatteryProduction();

                items.AsmProjectId = customPmIncentiveModel.AsmProjectId;
                items.AsmPlanId = customPmIncentiveModel.AsmPlanId;
                items.SmtPlanId = customPmIncentiveModel.SmtPlanId;
                items.BbPlanId = customPmIncentiveModel.BbPlanId;
                items.BhPlanId = customPmIncentiveModel.BhPlanId;
                items.SmtId = customPmIncentiveModel.SmtId;
                items.BhId = customPmIncentiveModel.BhId;
                items.BbId = customPmIncentiveModel.BbId;
                items.AsmId = customPmIncentiveModel.AsmId;
                items.AsmProjectName = customPmIncentiveModel.AsmProjectName;

                items.MaterialReceiveStartDateBSmt = customPmIncentiveModel.MaterialReceiveStartDateBSmt;
                items.MaterialReceiveEndDateBSmt = customPmIncentiveModel.MaterialReceiveEndDateBSmt;
                items.MaterialReceiveStartDateBSmt = customPmIncentiveModel.MaterialReceiveStartDateBSmt;
                items.MaterialReceiveEndDateBSmt = customPmIncentiveModel.MaterialReceiveEndDateBSmt;
                items.IqcCompleteStartDateBSmt = customPmIncentiveModel.IqcCompleteStartDateBSmt;
                items.IqcCompleteEndDateBSmt = customPmIncentiveModel.IqcCompleteEndDateBSmt;
                items.TrialProductionStartDateBSmt = customPmIncentiveModel.TrialProductionStartDateBSmt;
                items.SmtTrialLine = customPmIncentiveModel.SmtTrialLine;
                items.TrialProductionEndDateBSmt = customPmIncentiveModel.TrialProductionEndDateBSmt;
                items.SmtMassProductionStartDateBSmt = customPmIncentiveModel.SmtMassProductionStartDateBSmt;
                items.TotalQuantityBSmt = customPmIncentiveModel.TotalQuantityBSmt;
                items.SmtAllLineNumber = customPmIncentiveModel.SmtAllLineNumber;
                items.SmtAllLineCapacity = customPmIncentiveModel.SmtAllLineCapacity;
                items.BatterySmtPerDayCapacity = customPmIncentiveModel.BatterySmtPerDayCapacity;
                items.SmtMassProductionEndDateBSmt = customPmIncentiveModel.SmtMassProductionEndDateBSmt;

                items.MaterialReceiveStartDateBHousing = customPmIncentiveModel.MaterialReceiveStartDateBHousing;
                items.MaterialReceiveEndDateBHousing = customPmIncentiveModel.MaterialReceiveEndDateBHousing;
                items.IqcCompleteStartDateBHousing = customPmIncentiveModel.IqcCompleteStartDateBHousing;
                items.IqcCompleteEndDateBHousing = customPmIncentiveModel.IqcCompleteEndDateBHousing;
                items.TrialProductionStartDateBHousing = customPmIncentiveModel.TrialProductionStartDateBHousing;
                items.TrialProductionEndDateBHousing = customPmIncentiveModel.TrialProductionEndDateBHousing;
                items.HousingReliabilityTestStartDateBHousing = customPmIncentiveModel.HousingReliabilityTestStartDateBHousing;
                items.HousingReliabilityTestEndDateBHousing = customPmIncentiveModel.HousingReliabilityTestEndDateBHousing;
                items.HousingMassProductionStartDateBHousing = customPmIncentiveModel.HousingMassProductionStartDateBHousing;
                items.HousingMassProductionEndDateBHousing = customPmIncentiveModel.HousingMassProductionEndDateBHousing;
                items.TotalQuantity = customPmIncentiveModel.TotalQuantity;
                items.HousingAllLineNumber = customPmIncentiveModel.HousingAllLineNumber;
                items.BatteryHousingPerDayCapacity = customPmIncentiveModel.BatteryHousingPerDayCapacity;
                items.HousingAllLineCapacity = customPmIncentiveModel.HousingAllLineCapacity;
                items.HousingTrialLine = customPmIncentiveModel.HousingTrialLine;

                items.MaterialReceiveStartDateBattery = customPmIncentiveModel.MaterialReceiveStartDateBattery;
                items.MaterialReceiveEndDateBattery = customPmIncentiveModel.MaterialReceiveEndDateBattery;
                items.IqcCompleteStartDateBattery = customPmIncentiveModel.IqcCompleteStartDateBattery;
                items.IqcCompleteEndDateBattery = customPmIncentiveModel.IqcCompleteEndDateBattery;
                items.TrialProductionStartDateBattery = customPmIncentiveModel.TrialProductionStartDateBattery;
                items.TrialProductionEndDateBattery = customPmIncentiveModel.TrialProductionEndDateBattery;
                items.BatteryReliabilityTestStartDate = customPmIncentiveModel.BatteryReliabilityTestStartDate;
                items.BatteryReliabilityTestEndDate = customPmIncentiveModel.BatteryReliabilityTestEndDate;
                items.BatteryMassProductionStartDate = customPmIncentiveModel.BatteryMassProductionStartDate;
                items.BatteryMassProductionEndDate = customPmIncentiveModel.BatteryMassProductionEndDate;
                items.TotalQuantityBattery = customPmIncentiveModel.TotalQuantityBattery;
                items.BatteryAllLineNumber = customPmIncentiveModel.BatteryAllLineNumber;
                items.BatteryPerDayCapacity = customPmIncentiveModel.BatteryPerDayCapacity;
                items.BatteryAllLineCapacity = customPmIncentiveModel.BatteryAllLineCapacity;
                items.BatteryTrialLine = customPmIncentiveModel.BatteryTrialLine;

                items.MaterialReceiveStartDateBAssembly = customPmIncentiveModel.MaterialReceiveStartDateBAssembly;
                items.MaterialReceiveEndDateBAssembly = customPmIncentiveModel.MaterialReceiveEndDateBAssembly;
                items.IqcCompleteStartDateBAssembly = customPmIncentiveModel.IqcCompleteStartDateBAssembly;
                items.IqcCompleteEndDateBAssembly = customPmIncentiveModel.IqcCompleteEndDateBAssembly;
                items.TrialProductionStartDateBAssembly = customPmIncentiveModel.TrialProductionStartDateBAssembly;
                items.AssemblyTrialLine = customPmIncentiveModel.AssemblyTrialLine;
                items.TrialProductionEndDateBAssembly = customPmIncentiveModel.TrialProductionEndDateBAssembly;
                items.SoftwareConfirmationStartDateBAssembly = customPmIncentiveModel.SoftwareConfirmationStartDateBAssembly;
                items.SoftwareConfirmationEndDateBAssembly = customPmIncentiveModel.SoftwareConfirmationEndDateBAssembly;
                items.RandDConfirmationStartDateBAssembly = customPmIncentiveModel.RandDConfirmationStartDateBAssembly;
                items.RandDConfirmationEndDateBAssembly = customPmIncentiveModel.RandDConfirmationEndDateBAssembly;
                items.AssemblyMassProductionStartDateBAssembly = customPmIncentiveModel.AssemblyMassProductionStartDateBAssembly;
                items.AssemblyMassProductionEndDateBAssembly = customPmIncentiveModel.AssemblyMassProductionEndDateBAssembly;
                items.TotalQuantityBAssembly = customPmIncentiveModel.TotalQuantityBAssembly;
                items.AssemblyAllLineNumber = customPmIncentiveModel.AssemblyAllLineNumber;
                items.BatteryAssemblyPerDayCapacity = customPmIncentiveModel.BatteryAssemblyPerDayCapacity;
                items.AssemblyAllLineCapacity = customPmIncentiveModel.AssemblyAllLineCapacity;
                items.PackingMassProductionStartDateBAssembly = customPmIncentiveModel.PackingMassProductionStartDateBAssembly;
                items.PackingMassProductionEndDateBAssembly = customPmIncentiveModel.PackingMassProductionEndDateBAssembly;
                items.TotalQuantityBPacking = customPmIncentiveModel.TotalQuantityBPacking;
                items.PackingAllLineNumber = customPmIncentiveModel.PackingAllLineNumber;
                items.BatteryPackingPerDayCapacity = customPmIncentiveModel.BatteryPackingPerDayCapacity;
                items.PackingAllLineCapacity = customPmIncentiveModel.PackingAllLineCapacity;

                cmList.Add(items);
            }
            return View(cmList);
        }

        [HttpPost]
        public JsonResult InActiveAPlan(string projectId, string projectName, string planIds)
        {
            long proIds;
            long.TryParse(projectId, out proIds);

            long planId;
            long.TryParse(planIds, out planId);
            var getPacking = "";
            if (projectId != null && planIds != null)
            {
                getPacking = _repository.InActiveAPlan(proIds, planId);

            }

            //   var json = JsonConvert.SerializeObject(getPacking);

            return Json(new { data = getPacking }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #endregion

        #region New Production Plan

        public ActionResult FactoryHoliday()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetHoliday()
        {
            var getHolidays = _repository.GetHoliday();
            var json = JsonConvert.SerializeObject(getHolidays);
            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveHolidayDropData(string Id, string GovernmentHoliday, string HolidayStartDate, string HolidayEndDate)
        {
            var saveData = _repository.SaveHolidayDropData(Id, GovernmentHoliday, HolidayStartDate, HolidayEndDate);

            return Json(new { data = saveData }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteHolidayData(string Id)
        {
            var deleteEvent = _repository.DeleteHolidayData(Id);

            return Json(new { data = deleteEvent }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Capacity Planning
        public ActionResult Shift(string monNum1, string year, string productionType, string phoneType)
        {
            int mons;
            int.TryParse(monNum1, out mons);

            var vmCapacity = new VmCapacityPlanning();
            //Month//
            List<SelectListItem> selectListItemsMonth = new List<SelectListItem>();
            selectListItemsMonth.Add(new SelectListItem() { Text = "SELECT MONTH", Value = "0" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "January", Value = "1" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "February", Value = "2" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "March", Value = "3" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "April", Value = "4" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "May", Value = "5" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "June", Value = "6" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "July", Value = "7" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "August", Value = "8" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "September", Value = "9" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "October", Value = "10" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "November", Value = "11" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "December", Value = "12" });

            ViewBag.ddlMonths = selectListItemsMonth;
            //Year//
            List<SelectListItem> selectListItemsYear = new List<SelectListItem>();
            selectListItemsYear.Add(new SelectListItem() { Text = "SELECT YEAR", Value = "0" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2019", Value = "2019" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2020", Value = "2020" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2021", Value = "2021" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2022", Value = "2022" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2023", Value = "2023" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2024", Value = "2024" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2025", Value = "2025" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2026", Value = "2026" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2027", Value = "2027" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2028", Value = "2028" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2029", Value = "2029" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2030", Value = "2030" });

            ViewBag.ddlYears = selectListItemsYear;
            vmCapacity.MonNum1 = monNum1;
            vmCapacity.Year1 = year;
            vmCapacity.ProductionType = productionType;

            vmCapacity.ProTypeModels = _repository.GetProductionType();
            List<SelectListItem> items = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT UNIT" } };
            items.AddRange(vmCapacity.ProTypeModels.Select(model => new SelectListItem { Text = model.ProductionType, Value = model.ProductionType.ToString(CultureInfo.InvariantCulture) }).ToList());
            ViewBag.GetProductionType = items;

            ViewBag.GetShiftSavedData = _repository.GetShiftSavedData(mons, year, productionType);

            return View(vmCapacity);
        }
        [HttpPost]
        public ActionResult Shift(List<Pro_Shift_Model> issueList, string monNum1, string year, string monName, string productionType)
        {

            issueList = issueList.Where(x => x.IsRemoved == 0).ToList();

            long userId = Convert.ToInt64(User.Identity.Name);

            int mon;
            int.TryParse(monNum1, out mon);

            int years;
            int.TryParse(year, out years);

            _repository.SaveShift(issueList, mon, monName, years, productionType);

            // MonNum1= monNum1 +"&year="+ year + "&productionType="+ productionType 

            return RedirectToAction("Shift", new { MonNum1 = monNum1, year = year, productionType = productionType });
        }
        public ActionResult DailyPlan(string monNum1, string year, string productionType, string phoneType)
        {
            int mons;
            int.TryParse(monNum1, out mons);

            var vmCapacity = new VmCapacityPlanning();
            //Month//
            List<SelectListItem> selectListItemsMonth = new List<SelectListItem>();
            selectListItemsMonth.Add(new SelectListItem() { Text = "SELECT MONTH", Value = "0" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "January", Value = "1" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "February", Value = "2" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "March", Value = "3" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "April", Value = "4" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "May", Value = "5" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "June", Value = "6" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "July", Value = "7" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "August", Value = "8" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "September", Value = "9" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "October", Value = "10" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "November", Value = "11" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "December", Value = "12" });

            ViewBag.ddlMonths = selectListItemsMonth;
            //Year//
            List<SelectListItem> selectListItemsYear = new List<SelectListItem>();
            selectListItemsYear.Add(new SelectListItem() { Text = "SELECT YEAR", Value = "0" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2019", Value = "2019" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2020", Value = "2020" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2021", Value = "2021" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2022", Value = "2022" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2023", Value = "2023" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2024", Value = "2024" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2025", Value = "2025" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2026", Value = "2026" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2027", Value = "2027" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2028", Value = "2028" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2029", Value = "2029" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2030", Value = "2030" });

            ViewBag.ddlYears = selectListItemsYear;
            vmCapacity.MonNum1 = monNum1;
            vmCapacity.Year1 = year;
            vmCapacity.ProductionType = productionType;

            vmCapacity.ProTypeModels = _repository.GetProductionType();
            List<SelectListItem> items = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT UNIT" } };
            items.AddRange(vmCapacity.ProTypeModels.Select(model => new SelectListItem { Text = model.ProductionType, Value = model.ProductionType.ToString(CultureInfo.InvariantCulture) }).ToList());
            ViewBag.GetProductionType = items;

            if (mons != 0 && year != null)
            {
                ViewBag.GetDailyShiftData = _repository.GetDailyShiftData(mons, year, productionType);
                ViewBag.GetLine = _repository.GetLine(mons, year, productionType);
                ViewBag.GetShiftSavedData = _repository.GetShiftSavedData(mons, year, productionType);

                ViewBag.DailySaved = _repository.DailySaved(mons, year, productionType);
                var dd = _repository.DailySaved(mons, year, productionType);
                if (ViewBag.DailySaved.Count == 0)
                {
                   
                    ViewBag.GetDailyShiftData1 = _repository.GetDailyShiftData1(mons, year, productionType);
                   // ViewBag.ChangedDailyPlanData = _repository.ChangedDailyPlanData(mons, year, productionType);
                }
                
            }

            return View(vmCapacity);
        }

        public ActionResult CapacityPlanning(string monNum1, string year, string productionType, string phoneType, string categories)
        {
            int mons;
            int.TryParse(monNum1, out mons);

            var vmCapacity = new VmCapacityPlanning();
            //Month//
            List<SelectListItem> selectListItemsMonth = new List<SelectListItem>();
            selectListItemsMonth.Add(new SelectListItem() { Text = "SELECT MONTH", Value = "0" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "January", Value = "1" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "February", Value = "2" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "March", Value = "3" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "April", Value = "4" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "May", Value = "5" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "June", Value = "6" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "July", Value = "7" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "August", Value = "8" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "September", Value = "9" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "October", Value = "10" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "November", Value = "11" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "December", Value = "12" });

            ViewBag.ddlMonths = selectListItemsMonth;
            //Year//
            List<SelectListItem> selectListItemsYear = new List<SelectListItem>();
            selectListItemsYear.Add(new SelectListItem() { Text = "SELECT YEAR", Value = "0" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2019", Value = "2019" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2020", Value = "2020" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2021", Value = "2021" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2022", Value = "2022" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2023", Value = "2023" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2024", Value = "2024" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2025", Value = "2025" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2026", Value = "2026" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2027", Value = "2027" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2028", Value = "2028" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2029", Value = "2029" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2030", Value = "2030" });

            ViewBag.ddlYears = selectListItemsYear;
            //
            //Year//
            List<SelectListItem> selectListItemsPhoneType = new List<SelectListItem>();
            selectListItemsPhoneType.Add(new SelectListItem() { Text = "SELECT", Value = "0" });
            selectListItemsPhoneType.Add(new SelectListItem() { Text = "Smart", Value = "Smart" });
            selectListItemsPhoneType.Add(new SelectListItem() { Text = "Feature", Value = "Feature" });
            selectListItemsPhoneType.Add(new SelectListItem() { Text = "Charger", Value = "Charger" });
            ViewBag.ddlPhoneType = selectListItemsPhoneType;

            //
            vmCapacity.MonNum1 = monNum1;
            vmCapacity.Year1 = year;
            vmCapacity.ProductionType = productionType;
            vmCapacity.ProductName = phoneType;
            vmCapacity.CategoryName = categories;

            vmCapacity.ProTypeModels = _repository.GetProductionType();
            List<SelectListItem> items = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT UNIT" } };
            items.AddRange(vmCapacity.ProTypeModels.Select(model => new SelectListItem { Text = model.ProductionType, Value = model.ProductionType.ToString(CultureInfo.InvariantCulture) }).ToList());
            ViewBag.GetProductionType = items;

            vmCapacity.ProShiftModels = _repository.GetProductName(productionType);
            List<SelectListItem> itemsForProduct = vmCapacity.ProShiftModels.Select(model => new SelectListItem { Text = model.ProductName, Value = model.ProductName.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.Products = itemsForProduct;

            vmCapacity.ProShiftModels1 = _repository.GetCategoryName(productionType, phoneType);
            List<SelectListItem> itemsForProduct1 = vmCapacity.ProShiftModels1.Select(model => new SelectListItem { Text = model.CategoryName, Value = model.CategoryName.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.Categories = itemsForProduct1;

            if (mons != 0 && year != null)
            {
                ViewBag.GetShift = _repository.GetShift(mons, year, productionType, phoneType);
                // ViewBag.GetTeam = _repository.GetTeam(mons, year, productionType, phoneType);
                ViewBag.GetPercentage = _repository.GetPercentage(mons, year, productionType, phoneType, categories);
                ViewBag.GetQuantityRange = _repository.GetQuantityRange(mons, year, productionType, phoneType, categories);
                // ViewBag.GetCapacity = _repository.GetCapacity(mons, year, productionType, categories);
                ViewBag.GetAll1 = _repository.GetAll1(mons, year, productionType, phoneType, categories);

            }
            return View(vmCapacity);
        }
        public JsonResult GetProductName(string productionType)
        {
            var vmCapacity = new VmCapacityPlanning();

            if (productionType != null)
            {
                vmCapacity.ProShiftModels = _repository.GetProductName(productionType);

            }

            List<SelectListItem> items1 = vmCapacity.ProShiftModels.Select(model => new SelectListItem { Text = model.ProductName, Value = model.ProductName.ToString(CultureInfo.InvariantCulture) }).ToList();
            var json = JsonConvert.SerializeObject(items1);

            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetCategoryName(string productionType, string proPhoneName)
        {
            var vmCapacity = new VmCapacityPlanning();

            if (productionType != null && proPhoneName != null)
            {
                vmCapacity.ProShiftModels1 = _repository.GetCategoryName(productionType, proPhoneName);

            }

            List<SelectListItem> items1 = vmCapacity.ProShiftModels1.Select(model => new SelectListItem { Text = model.CategoryName, Value = model.CategoryName.ToString(CultureInfo.InvariantCulture) }).ToList();
            var json = JsonConvert.SerializeObject(items1);

            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public ActionResult CapacityPlanning(string arrMain)
        {
            var results = JsonConvert.DeserializeObject<List<Pro_CapacityPlanning_Model>>(arrMain);

            var saveResult = "0";
            if (results.Count != 0)
            {
                saveResult = _repository.SaveCapacityData(results);
            }

            return Json(new { data = saveResult }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateCapacityPlanning(string monNum1, string year, string productionType, string phoneType, string categories)
        {
            int mons;
            int.TryParse(monNum1, out mons);

            var vmCapacity = new VmCapacityPlanning();
            if (mons != 0 && year != null)
            {
                ViewBag.GetShift = _repository.GetShift(mons, year, productionType, phoneType);
                ViewBag.GetTeam = _repository.GetTeam(mons, year, productionType, phoneType, categories);
                ViewBag.GetPercentage = _repository.GetPercentage(mons, year, productionType, phoneType, categories);
                ViewBag.GetQuantityRange = _repository.GetQuantityRange(mons, year, productionType, phoneType, categories);
                ViewBag.GetCapacity = _repository.GetCapacity(mons, year, productionType, phoneType);
                ViewBag.GetAll = _repository.GetAll(mons, year, productionType, phoneType, categories);

            }
            return View(vmCapacity);
        }
        [HttpPost]
        public ActionResult UpdateCapacityPlanning(string arrMain)
        {
            var results = JsonConvert.DeserializeObject<List<Pro_CapacityPlanning_Model>>(arrMain);

            var saveResult = "0";
            if (results.Count != 0)
            {
                saveResult = _repository.SaveCapacityData(results);
            }

            return Json(new { data = saveResult }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateTeam(string productionType)
        {

            var vmCapacity = new VmCapacityPlanning();
            vmCapacity.ProductionType = productionType;

            vmCapacity.ProTypeModels = _repository.GetProductionType();
            List<SelectListItem> items = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT UNIT" } };
            items.AddRange(vmCapacity.ProTypeModels.Select(model => new SelectListItem { Text = model.ProductionType, Value = model.ProductionType.ToString(CultureInfo.InvariantCulture) }).ToList());
            ViewBag.GetProductionType = items;

            if (productionType != null)
            {

                ViewBag.GetTeamForUpdate = _repository.GetTeamForUpdate(productionType);

            }
            return View(vmCapacity);
        }
        [HttpPost]
        public ActionResult CreateTeam(List<Pro_Shift_Model> issueList1, string productionType)
        {

            issueList1 = issueList1.Where(x => x.IsRemoved == 0).ToList();

            long userId = Convert.ToInt64(User.Identity.Name);
            if (ModelState.IsValid)
            {
                _repository.SaveTeam(issueList1, productionType);
            }

            return RedirectToAction("CreateTeam", new { productionType = productionType });
        }

        public JsonResult InActiveTeam(string inactiveObj)
        {
            long ids;
            long.TryParse(inactiveObj, out ids);

            var SaveInactive = "0";

            if (ids != 0)
            {
                SaveInactive = _repository.UpdateTeam(ids);
            }

            return Json(new { data = SaveInactive }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult EditTeam(String Id, String Team)
        {

            var editTeam = _repository.EditTeam(Id, Team);

            return new JsonResult { Data = editTeam, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult CreateLine(string productionType)
        {
            var vmCapacity = new VmCapacityPlanning();
            vmCapacity.ProductionType = productionType;

            vmCapacity.ProTypeModels = _repository.GetProductionType();
            List<SelectListItem> items = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT UNIT" } };
            items.AddRange(vmCapacity.ProTypeModels.Select(model => new SelectListItem { Text = model.ProductionType, Value = model.ProductionType.ToString(CultureInfo.InvariantCulture) }).ToList());
            ViewBag.GetProductionType = items;

            if (productionType != null)
            {

                ViewBag.GetLineForUpdate = _repository.GetLineForUpdate(productionType);

            }
            return View(vmCapacity);
        }
        [HttpPost]
        public ActionResult CreateLine(List<Pro_Shift_Model> issueList1, string productionType)
        {

            issueList1 = issueList1.Where(x => x.IsRemoved == 0).ToList();

            long userId = Convert.ToInt64(User.Identity.Name);
            if (productionType != null)
            {
                _repository.SaveLine(issueList1, productionType);
            }

            return RedirectToAction("CreateLine", new { productionType = productionType });
        }
        [HttpPost]
        public JsonResult EditLine(String Id, String Line, String LineType, String ProductionDaysPerMonth, String ShiftPerDay, String HoursPerShift)
        {

            var editTeam = _repository.EditLine(Id, Line, LineType, ProductionDaysPerMonth, ShiftPerDay, HoursPerShift);

            return new JsonResult { Data = editTeam, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult InActiveLine(string inactiveObj)
        {
            long ids;
            long.TryParse(inactiveObj, out ids);

            var SaveInactive = "0";

            if (ids != 0)
            {
                SaveInactive = _repository.InActiveLine(ids);
            }

            return Json(new { data = SaveInactive }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InActiveShift(string inactiveObj)
        {
            long ids;
            long.TryParse(inactiveObj, out ids);

            var SaveInactive = "0";

            if (ids != 0)
            {
                SaveInactive = _repository.InActiveShift(ids);
            }

            return Json(new { data = SaveInactive }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllTeam(string productionType11)
        {
            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT" } };

            List<String> moduleList = _repository.GetAllTeam(productionType11);
            foreach (var module in moduleList)
            {
                selectListItems.Add(new SelectListItem
                {
                    Value = module,
                    Text = module
                });
            }
            return Json(new { list = selectListItems }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllLine(string productionType11)
        {
            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT" } };

            List<String> moduleList = _repository.GetAllLine(productionType11);
            foreach (var module in moduleList)
            {
                selectListItems.Add(new SelectListItem
                {
                    Value = module,
                    Text = module
                });
            }
            return Json(new { list = selectListItems }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllCategory(string productionType11, string PhoneType)
        {
            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT" } };

            List<String> moduleList = _repository.GetAllCategory(productionType11, PhoneType);
            foreach (var module in moduleList)
            {
                selectListItems.Add(new SelectListItem
                {
                    Value = module,
                    Text = module
                });
            }
            return Json(new { list = selectListItems }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateProduct(string productionType)
        {
            var vmCapacity = new VmCapacityPlanning();
            vmCapacity.ProductionType = productionType;

            vmCapacity.ProTypeModels = _repository.GetProductionType();
            List<SelectListItem> items = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT UNIT" } };
            items.AddRange(vmCapacity.ProTypeModels.Select(model => new SelectListItem { Text = model.ProductionType, Value = model.ProductionType.ToString(CultureInfo.InvariantCulture) }).ToList());
            ViewBag.GetProductionType = items;

            if (productionType != null)
            {

                ViewBag.GetProductForUpdate = _repository.GetProductForUpdate(productionType);

            }
            return View(vmCapacity);
        }
        [HttpPost]
        public ActionResult CreateProduct(List<Pro_Shift_Model> issueList1, string productionType)
        {
            issueList1 = issueList1.Where(x => x.IsRemoved == 0).ToList();

            long userId = Convert.ToInt64(User.Identity.Name);

            if (ModelState.IsValid)
            {
                _repository.SaveProduct(issueList1, productionType);
            }
            return RedirectToAction("CreateProduct", new { productionType = productionType });
        }
        public JsonResult InActiveProduct(string inactiveObj)
        {
            long ids;
            long.TryParse(inactiveObj, out ids);

            var SaveInactive = "0";

            if (ids != 0)
            {
                SaveInactive = _repository.InActiveProduct(ids);
            }

            return Json(new { data = SaveInactive }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateDailyPlan(String Id,DateTime EffectiveDate, String Line, String Shift_1, String Shift_2, String Shift_3, String ProductionType,
            String MonNum, String Month, String Year)
        {
            long ids;
            long.TryParse(Id, out ids);

            var editPlan = _repository.UpdateDailyPlan(ids,EffectiveDate, Line, Shift_1, Shift_2, Shift_3, ProductionType,
                MonNum, Month, Year);

            return new JsonResult { Data = editPlan, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public ActionResult SaveDailyPlan(string arrMain)
        {
            var results = JsonConvert.DeserializeObject<List<Pro_Shift_Model>>(arrMain);

            var saveResult = "0";
            if (results.Count != 0)
            {
                saveResult = _repository.SaveDailyPlan(results);
            }

            return Json(new { data = saveResult }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Capacity Report
        public ActionResult Report_CapacityPlanning(string monNum1, string year, string productionType)
        {
            int mons;
            int.TryParse(monNum1, out mons);

            var vmCapacity = new VmCapacityPlanning();
            //Month//
            List<SelectListItem> selectListItemsMonth = new List<SelectListItem>();
            selectListItemsMonth.Add(new SelectListItem() { Text = "SELECT MONTH", Value = "0" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "January", Value = "1" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "February", Value = "2" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "March", Value = "3" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "April", Value = "4" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "May", Value = "5" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "June", Value = "6" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "July", Value = "7" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "August", Value = "8" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "September", Value = "9" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "October", Value = "10" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "November", Value = "11" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "December", Value = "12" });

            ViewBag.ddlMonths = selectListItemsMonth;
            //Year//
            List<SelectListItem> selectListItemsYear = new List<SelectListItem>();
            selectListItemsYear.Add(new SelectListItem() { Text = "SELECT YEAR", Value = "0" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2019", Value = "2019" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2020", Value = "2020" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2021", Value = "2021" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2022", Value = "2022" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2023", Value = "2023" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2024", Value = "2024" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2025", Value = "2025" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2026", Value = "2026" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2027", Value = "2027" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2028", Value = "2028" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2029", Value = "2029" });
            selectListItemsYear.Add(new SelectListItem() { Text = "2030", Value = "2030" });

            ViewBag.ddlYears = selectListItemsYear;
            //

            vmCapacity.MonNum1 = monNum1;
            vmCapacity.Year1 = year;
            vmCapacity.ProductionType = productionType;


            vmCapacity.ProTypeModels = _repository.GetProductionType();
            List<SelectListItem> items = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT UNIT" } };
            items.AddRange(vmCapacity.ProTypeModels.Select(model => new SelectListItem { Text = model.ProductionType, Value = model.ProductionType.ToString(CultureInfo.InvariantCulture) }).ToList());
            ViewBag.GetProductionType = items;

            vmCapacity.ProShiftModels = _repository.GetProductName(productionType);
            List<SelectListItem> itemsForProduct = vmCapacity.ProShiftModels.Select(model => new SelectListItem { Text = model.ProductName, Value = model.ProductName.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.Products = itemsForProduct;

            if (mons != 0 && year != null)
            {
                ViewBag.ProductNameForReport = _repository.ProductNameForReport(mons, year, productionType);
                ViewBag.TeamNameForReport = _repository.TeamNameForReport(mons, year, productionType);
                ViewBag.CategoryNameForReport = _repository.CategoryNameForReport(mons, year, productionType);
                ViewBag.GetPercentage1 = _repository.GetPercentage1(mons, year, productionType);
                ViewBag.GetQuantityRange1 = _repository.GetQuantityRange1(mons, year, productionType);
                ViewBag.GetTotalCapacities1 = _repository.GetTotalCapacities1(mons, year, productionType);
            }
            return View(vmCapacity);
        }
        #endregion

        #region Project Categorize

        public ActionResult ProjectCategorize()
        {
            var vmCapacity = new VmCapacityPlanning();

            ViewBag.GetProjectForCategorization = _repository.GetProjectForCategorization();
            ViewBag.GetCompletedCategorization = _repository.GetCompletedCategorization();

            //List<SelectListItem> items =new List<SelectListItem>();
            //foreach (var mdd in ViewBag.GetProjectForCategorization)
            //{
            //    vmCapacity.ProShiftModels1 = _repository.GetAssemblyCategory(mdd.ProjectType);
            //    items = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT ASSEMBLY" } };
            //    items.AddRange(vmCapacity.ProShiftModels1.Select(model => new SelectListItem { Text = model.CategoryName, Value = model.ProductionType.ToString(CultureInfo.InvariantCulture) }).ToList());
            //    ViewBag.GetAssemblyCategory = items;
            //}



            return View(vmCapacity);
        }
        public JsonResult GetAssemblyCategory(string AssemblyCategory)
        {
            var vmCapacity = new VmCapacityPlanning();
            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Select Assembly" } };

            vmCapacity.ProShiftModels1 = _repository.GetAssemblyCategory(AssemblyCategory);
            foreach (var module in vmCapacity.ProShiftModels1)
            {
                selectListItems.Add(new SelectListItem
                {
                    Value = module.ProductionType,
                    Text = module.AssemblyCategory
                });
            }
            return Json(new { list = selectListItems }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSmtCategory(string SmtCategory)
        {
            var vmCapacity = new VmCapacityPlanning();
            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Select Smt" } };

            vmCapacity.ProShiftModels1 = _repository.GetSmtCategory(SmtCategory);
            foreach (var module in vmCapacity.ProShiftModels1)
            {
                selectListItems.Add(new SelectListItem
                {
                    Value = module.ProductionType,
                    Text = module.SmtCategory
                });
            }
            return Json(new { list = selectListItems }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetHousingCategory(string HousingCategory)
        {
            var vmCapacity = new VmCapacityPlanning();
            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Select Housing" } };

            vmCapacity.ProShiftModels1 = _repository.GetHousingCategory(HousingCategory);
            foreach (var module in vmCapacity.ProShiftModels1)
            {
                selectListItems.Add(new SelectListItem
                {
                    Value = module.ProductionType,
                    Text = module.HousingCategory
                });
            }
            return Json(new { list = selectListItems }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveCategorizeData(string ProjectName, string ProductFamily, string AssemblyCategory, string SmtCategory, string HousingCategory)
        {

            var saveData = "0";

            if (AssemblyCategory != "" || SmtCategory != "" || HousingCategory != "")
            {
                saveData = _repository.SaveCategorizeData(ProjectName, ProductFamily, AssemblyCategory, SmtCategory, HousingCategory);
            }

            return Json(new { data = saveData }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CompleteCategorizeData(string ProjectName, string ProductFamily)
        {

            var saveData = "0";

            if (ProjectName != "" && ProductFamily != "")
            {
                saveData = _repository.CompleteCategorizeData(ProjectName, ProductFamily);
            }

            return Json(new { data = saveData }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateCategorizeData(string CatIds, string AssemblyCategory1, string SmtCategory1, string HousingCategory1)
        {
            long ids;
            long.TryParse(CatIds, out ids);

            var saveData = "0";

            if (AssemblyCategory1 != "" || SmtCategory1 != "" || HousingCategory1 != "")
            {
                saveData = _repository.UpdateCategorizeData(ids, AssemblyCategory1, SmtCategory1, HousingCategory1);
            }

            return Json(new { data = saveData }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Shift and Capacity Forward

        public ActionResult ForwardShiftAndCapacity()
        {
            var vm = new VmCapacityPlanning();

            vm.ProTypeModels = _repository.GetProductionType();
            List<SelectListItem> items = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT UNIT" } };
            items.AddRange(vm.ProTypeModels.Select(model => new SelectListItem { Text = model.ProductionType, Value = model.ProductionType.ToString(CultureInfo.InvariantCulture) }).ToList());
            ViewBag.GetProductionType = items;

            return View(vm);
        }

       [HttpPost]
        public JsonResult ForwardShift(string unitValues, string currentDate, string forwardedDate, string shiftForward)
        {
            var saveIncentive = "0";

            if (unitValues!="" && currentDate != "" && forwardedDate !="" & shiftForward !="")
            {
                saveIncentive = _repository.ForwardShift(unitValues,currentDate, forwardedDate, shiftForward);
            }

            return Json(new { data = saveIncentive }, JsonRequestBehavior.AllowGet);
        }
       [HttpPost]
       public JsonResult ForwardCapacity(string unitValues, string currentDate, string forwardedDate, string capForward)
       {
           var saveIncentive = "0";

           if (unitValues != "" && currentDate != "" && forwardedDate != "" & capForward != "")
           {
               saveIncentive = _repository.ForwardCapacity(unitValues,currentDate, forwardedDate, capForward);
           }

           return Json(new { data = saveIncentive }, JsonRequestBehavior.AllowGet);
       }
        #endregion
    }
}