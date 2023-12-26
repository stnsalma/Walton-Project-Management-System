using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Infrastructures.Repositories;
using ProjectManagement.Models;
using ProjectManagement.ViewModels.ProjectCommercial;

namespace ProjectManagement.Controllers
{
    public class ProjectCommercialController : Controller
    {
        private readonly IProjectCommercialRepository _projectCommercialRepository;
        public ProjectCommercialController(ProjectCommercialRepository projectCommercialRepository)
        {
            _projectCommercialRepository = projectCommercialRepository;
            new ProjectManagerRepository();
        }

        // GET: ProjectCommercial
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TacRequest(long projectId = 0)
        {
            long userId = 0;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            List<VmTacRequest> vmTacRequests = _projectCommercialRepository.GetProjectsForTac(userId);
            return View(vmTacRequests);
        }


        [NotificationActionFilter(ReceiverRoles = "CM,MM,PMHEAD,PM,CMBTRC,ACCNT")]
        [HttpPost]
        public JsonResult SaveBabtInfo(string masterId, string assignedId, string orderId, string reqQuantity, string babtId)
        {
            long mstId, assiId, odId, qty, bbtId;
            long.TryParse(masterId, out mstId);
            long.TryParse(assignedId, out assiId);
            long.TryParse(orderId, out odId);
            long.TryParse(reqQuantity, out qty);
            long.TryParse(babtId, out bbtId);
            var notificationObject = new NotificationObject
            {

                ProjectId = mstId,
                ToUser = "-1",
            };
            notificationObject.Message = " has requested for TAC ";
            notificationObject.AdditionalMessage = "";
            ViewBag.ControllerVariable = notificationObject;
            var result = _projectCommercialRepository.SaveBabtInfo(mstId, assiId, odId, bbtId, qty);
            if (result == "y")
            {
                TempData["message"] = "TAC Request Send Successfully.";
                TempData["messageType"] = 1;
            }
            else if (result == "n")
            {
                TempData["message"] = "Error occured!!! Please Contact with Administrator.";
                TempData["messageType"] = 2;
            }
            
            return new JsonResult{Data = TempData["messageType"], JsonRequestBehavior = JsonRequestBehavior.AllowGet};
        }

        [HttpPost]
        public JsonResult SaveSendingSupplierDate(string projectMasterId, string sendingDate)
        {
            long pmId;
            long.TryParse(projectMasterId, out pmId);
            var result = _projectCommercialRepository.SaveSendingSupplierDate(pmId, sendingDate);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region CommercialKpi
       
        public ActionResult CommercialKpi(string persons, string MonNum, string Year)
        {
            var vmComKpi = new VmCommercialKpi();
            vmComKpi.UserFullName = persons;

            long monIds;
            long.TryParse(MonNum, out monIds);

            long yearIds;
            long.TryParse(Year, out yearIds);


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
            //
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

            vmComKpi.CmnUserModels = _projectCommercialRepository.GetCommercialUsers();
            List<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "SELECT PERSON" },
                new SelectListItem { Value = "ALL", Text = "ALL" }
            };
            items.AddRange(vmComKpi.CmnUserModels.Select(model => new SelectListItem { Text = model.UserFullName, Value = model.EmployeeCode.ToString(CultureInfo.InvariantCulture) }).ToList());
            ViewBag.GetCommercialUsers = items;

            var fileManager = new FileManager();

            if (persons != null && monIds != 0 && yearIds != 0)
            {
             
                ViewBag.GetCmKpi = _projectCommercialRepository.GetCmKpi(persons, monIds, yearIds);

               // if (persons != "ALL" && monIds != 0 && yearIds != 0)
               // {
                    ViewBag.GetCommercialUsersDetailsCM = _projectCommercialRepository.GetCommercialUsersDetailsCM(persons);
                    //ViewBag.GetCommercialUsersDetailsCMHEAD = _projectCommercialRepository.GetCommercialUsersDetailsCMHEAD();

                    var dd = ViewBag.GetCommercialUsersDetailsCM;
                    ViewBag.GetCommercialUsersDetailsInformation = _projectCommercialRepository.GetCommercialUsersDetailsInformation(persons);
                   // UserPhotos(persons);


                    foreach (var model in ViewBag.GetCommercialUsersDetailsCM)
                    {
                        if (model.ProfilePictureUrl != null)
                        {
                            var urls = model.ProfilePictureUrl;

                            FilesDetail detail = new FilesDetail();
                            detail.FilePath = fileManager.GetFile(urls);
                            detail.Extention = fileManager.GetExtension(urls);
                            model.FilesDetails.Add(detail);
                        }
                        
                    }

                    //user load under head
                    ViewBag.GetCmUsersUnderHead = _projectCommercialRepository.GetCmUsersUnderHead(persons, monIds, yearIds);

                    foreach (var model in ViewBag.GetCmUsersUnderHead)
                    {
                        if (model.ProfilePictureUrl != null)
                        {
                            var urls = model.ProfilePictureUrl;

                            FilesDetail detail = new FilesDetail();
                            detail.FilePath = fileManager.GetFile(urls);
                            detail.Extention = fileManager.GetExtension(urls);
                            model.FilesDetails.Add(detail);
                        }

                        foreach (var mod2 in ViewBag.GetCmKpi)
                        {
                            if (mod2.KpiName == "Total :")
                            {
                                model.TotalAverageScorePercent = mod2.TotalAverageScorePercent;
                                
                            }
                        }
                    }
               // }

                vmComKpi.TeamKpiPercentageListModels1= _projectCommercialRepository.CommercialKpiSingleBarChart(persons, MonNum, Year);
              
            }
            vmComKpi.Month = MonNum;
            vmComKpi.Year = Year;
            return View(vmComKpi);
        }

        public ActionResult CommercialKpiDetails(string persons, string MonNum, string Year, string kpiName)
        {
            var vmComKpi = new VmCommercialKpi();
            vmComKpi.UserFullName = persons;

            long monIds;
            long.TryParse(MonNum, out monIds);

            long yearIds;
            long.TryParse(Year, out yearIds);

            if (persons != null && monIds != 0 && yearIds != 0)
            {
                ViewBag.CommercialKpiDetails = _projectCommercialRepository.CommercialKpiDetails(persons, monIds, yearIds, kpiName);
                ViewBag.CommercialIqcKpiDetails = _projectCommercialRepository.CommercialIqcKpiDetails(persons, monIds, yearIds, kpiName);
            }
            vmComKpi.Month = MonNum;
            vmComKpi.Year = Year;

            return View(vmComKpi);
        }
        public ActionResult CommercialIqcKpiDetails(string persons, string MonNum, string Year, string kpiName)
        {
            var vmComKpi = new VmCommercialKpi();
            vmComKpi.UserFullName = persons;

            long monIds;
            long.TryParse(MonNum, out monIds);

            long yearIds;
            long.TryParse(Year, out yearIds);

            if (persons != null && monIds != 0 && yearIds != 0)
            {
                ViewBag.CommercialKpiDetails = _projectCommercialRepository.CommercialIqcKpiDetails(persons, monIds, yearIds, kpiName);
            }
            vmComKpi.Month = MonNum;
            vmComKpi.Year = Year;

            return View(vmComKpi);
        }
        //public ActionResult CommercialKpiChart(string persons, string MonNum, string Year)
        public ActionResult CommercialKpiChart()
        {
            var vmComKpi = new VmCommercialKpi();

            vmComKpi.CmnUserModels = _projectCommercialRepository.GetCommercialUsers();
            List<SelectListItem> items = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "SELECT PERSON" },
                new SelectListItem { Value = "ALL", Text = "ALL" }
            };
            items.AddRange(vmComKpi.CmnUserModels.Select(model => new SelectListItem { Text = model.UserFullName, Value = model.EmployeeCode.ToString(CultureInfo.InvariantCulture) }).ToList());
            ViewBag.GetCommercialUsers = items;
           
            return View(vmComKpi);
        }
        //obj.persons = userTypeValue;
         //   obj.MonNum = monValue;
         //   obj.Year = yearsValue;

        //single bar
        [HttpPost]
        public JsonResult CommercialKpiSingleBarChart(string persons, string MonNum, string Year)
        {
            var editTeam = _projectCommercialRepository.CommercialKpiSingleBarChart(persons, MonNum, Year);

            return new JsonResult { Data = editTeam, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        //multiple line chart
        [HttpPost]
        public JsonResult CommercialKpiLineChart(String userValue, String sDate, String endDate)
        {
            var editTeam = _projectCommercialRepository.CommercialKpiLineChart(userValue, sDate, endDate);

            return new JsonResult { Data = editTeam, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult YearlyKpiForm(string proRoleName, string startValue, string endValue, string kpiRoles, string kpiRolePerson, string kpiRolePersonName)
        {
            var vmCk= new VmCommercialKpi();

            vmCk.TeamKpiRoleTables = _projectCommercialRepository.GetKpiRoleName();
            List<SelectListItem> items = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT ROLE" } };
            items.AddRange(vmCk.TeamKpiRoleTables.Select(model => new SelectListItem { Text = model.KpiRoleName, Value = model.KpiRoleName.ToString(CultureInfo.InvariantCulture) }).ToList());
            ViewBag.GetKpiRoleName = items;

            vmCk.CmnUserModels = _projectCommercialRepository.GetRolePerson(kpiRoles);
            List<SelectListItem> itemsForProduct = vmCk.CmnUserModels.Select(model => new SelectListItem { Text = model.UserFullName, Value = model.EmployeeCode.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.GetRolePerson = itemsForProduct;

            if (startValue !=null && endValue !=null && kpiRoles!=null && kpiRolePerson !=null)
            {
                ViewBag.GetCmYearlyKpi = _projectCommercialRepository.GetCmYearlyKpi(startValue, endValue, kpiRoles, kpiRolePerson);

                ViewBag.GetCmYearlyOthersKpi = _projectCommercialRepository.GetCmYearlyOthersKpi(startValue, endValue, kpiRoles, kpiRolePerson);

            }
            

            vmCk.kpiRoles = kpiRoles;
            vmCk.kpiRolePerson = kpiRolePerson;
            vmCk.kpiRolePersonName = kpiRolePersonName;

            return View(vmCk);
        }

        [HttpPost]
        public JsonResult YearlyKpiForm(string objArr)
        {

            List<TeamKpiPercentageListModel> results = JsonConvert.DeserializeObject<List<TeamKpiPercentageListModel>>(objArr);
            Console.Write("result :" + results);

            bool isExist = false;
            //if (results.Count != 0)
            //{
            //    isExist = _projectCommercialRepository.GetSavedKpiData(results[0].EmployeeCode, results[0].KpiName);

            //}

            //if (isExist)
            //{
            //    TempData["Message2"] = "Incentive already generated";
            //    return Json("YearlyKpiForm", "ProjectCommercial");
            //}

            var SaveIncentive = "0";

            if (results.Count != 0)
            {
                SaveIncentive = _projectCommercialRepository.SaveKpiValueBData(results);
            }


            return Json(new { SaveIncentive }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRolePerson(string proRoleName)
        {
            var vmCapacity = new VmCommercialKpi();

            if (proRoleName != null)
            {
                vmCapacity.CmnUserModels = _projectCommercialRepository.GetRolePerson(proRoleName);
            }

            List<SelectListItem> items1 = vmCapacity.CmnUserModels.Select(model => new SelectListItem { Text = model.UserFullName, Value = model.EmployeeCode.ToString(CultureInfo.InvariantCulture) }).ToList();
            var json = JsonConvert.SerializeObject(items1);

            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult YearlyKpiPrint(string startValue, string endValue, string kpiRoles, string kpiRolePerson)
        {
            return View();
        }
       // YearlyKpiPrint

        //public FileContentResult UserPhotos(string persons1)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        String userId = User.Identity.Name;
        //        string fileName;
        //        string uId="";
        //        //long.TryParse(userId, out uId);
        //        if (persons1 != null) uId = persons1;
        //        FileContentResult result = _projectCommercialRepository.GetProfilePicture(uId);
        //        return result;
        //    }
        //    else
        //    {
        //        string fileName = HttpContext.Server.MapPath(@"~/assets/layouts/layout4/img/av.png");
        //        var fileInfo = new FileInfo(fileName);
        //        long imageFileLength = fileInfo.Length;
        //        var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        //        var br = new BinaryReader(fs);
        //        byte[] imageData = br.ReadBytes((int)imageFileLength);
        //        return File(imageData, "image/png");
        //    }
        //}
        #endregion
    }

}