using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Helpers;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Infrastructures.Repositories;
using ProjectManagement.Models;
using ProjectManagement.Models.ManagementDashboard;
using ProjectManagement.ViewModels.Management;
using ProjectManagement.Models.StausObjects;

namespace ProjectManagement.Controllers
{
    [Authorize(Roles = "MM,SA,PS,PM,PMHEAD,ASPM,ASPMHEAD,QCHEAD,CEO,AUDHEAD,BIHEAD,CMHEAD")]
    public class ManagementController : Controller
    {
        private readonly IManagementRepository _repository;

        private readonly IHardwareRepository _hwRepository;
        private readonly ICommercialRepository _commercialRepository;
        private readonly ICommonRepository _commonRepository;



        public ManagementController(ManagementRepository repository, CommercialRepository commercialRepository, CommonRepository commonRepository)
        {
            _repository = repository;
            _hwRepository = new HardwareRepository();
            _commercialRepository = commercialRepository;
            _commonRepository = commonRepository;
            String useridentity = System.Web.HttpContext.Current.User.Identity.Name;
            var users = Convert.ToInt64(useridentity == "" ? "0" : useridentity);
            ViewBag.ChinaQcInspectionCount = _commonRepository.GetChinaQcInspectionCount(users);
        }


        public ActionResult CompletedProjecstList()
        {
            var userId = HttpContext.User.Identity.Name;
            long uId;
            long.TryParse(userId, out uId);
            ViewBag.userInfo = _hwRepository.GetUserInfoByUserId(uId);
            List<ProjectMasterWithPoCustomModel> masterModels = _repository.GetCompletedProjectMasterModelList();
            return View(masterModels);
        }

        [Authorize(Roles = "MM,SA,PS,PM,PMHEAD,ASPM,ASPMHEAD,QCHEAD,AUDHEAD,BIHEAD,CEO")]
        public ActionResult Index(string projectName)
        {
            long projectId = 0;
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _hwRepository.GetUserInfoByUserId(userId);
            var masterModels = _hwRepository.GetAllProjects();
            var items = new List<SelectListItem> { new SelectListItem { Value = "-1", Text = "Top 5 Projects" }, new SelectListItem { Value = "0", Text = "Latest 5 Projects" } };
            items.AddRange(masterModels.Select(masterModel => new SelectListItem
            {
                Value = masterModel.ProjectMasterId.ToString(CultureInfo.InvariantCulture),
                Text = masterModel.ProjectName
            }));
            ViewBag.ProjectMaster = items;
            //List<WorkProgressData> progresses = _repository.ProjectMonthlyWorkprogress(projectId);
            //ViewBag.work = JsonConvert.SerializeObject(progresses);
            var dashBoardCounter = new DashBoardCounter();
            if (User.IsInRole("MM"))
            {
                ViewBag.MmDashboardConter = dashBoardCounter.GetDashBoardCounter("Management", "MM", userId);
            }
            if (User.IsInRole("CEO"))
            {
                ViewBag.MmDashboardConter = dashBoardCounter.GetDashBoardCounter("Management", "CEO", userId);
            }
            if (User.IsInRole("PS"))
            {
                ViewBag.MmDashboardConter = dashBoardCounter.GetDashBoardCounter("Management", "PS", userId);
            }
            if (User.IsInRole("BIHEAD"))
            {
                ViewBag.MmDashboardConter = dashBoardCounter.GetDashBoardCounter("Management", "BIHEAD", userId);
            }
            ViewBag.Project = projectName;
            //---------------------------------------
            ViewBag.ProjectNames = _commonRepository.GetAllProjectNames();
            ViewBag.Projects = _commonRepository.GetAllProjects();
            //------Unproduced Quantity----
            var unproducedAverage = new List<SixMonthsUnproducedAverageQtyModel>();
            var smartCounter = 0;
            var totalUnproducedSmart = 0;
            var totalSmartOrderQuantity = 0;
            var featureCounter = 0;
            var totalUnproducedFeature = 0;
            var totalFeatureOrderQuantity = 0;
            var model = _commonRepository.SixMonthsUnproducedAverageQty();
            for (var i = 0; i < model.Count; i++)
            {
                var partsCurrent = model[i].ProjectModel.Split(' ');
                if (partsCurrent[0].ToLower() == "primo")
                {
                    smartCounter = smartCounter + 1;
                    totalUnproducedSmart = totalUnproducedSmart + Convert.ToInt32(model[i].UnProduced);
                    totalSmartOrderQuantity = totalSmartOrderQuantity + Convert.ToInt32(model[i].OrderQuantity);
                }
                if (partsCurrent[0].ToLower() == "olvio" || partsCurrent[0].ToLower() == "axino")
                {
                    featureCounter = featureCounter + 1;
                    totalUnproducedFeature = totalUnproducedFeature + Convert.ToInt32(model[i].UnProduced);
                    totalFeatureOrderQuantity = totalFeatureOrderQuantity + Convert.ToInt32(model[i].OrderQuantity);
                }
            }
            var smartAverage = smartCounter == 0 ? 0 : totalUnproducedSmart / smartCounter;
            var smartPercentage = (totalUnproducedSmart == 0 || totalSmartOrderQuantity==0) ? 0 : ((decimal)totalUnproducedSmart / totalSmartOrderQuantity) * 100;
            var featureAverage = featureCounter == 0 ? 0 : totalUnproducedFeature / featureCounter;
            var featurePercentage = (totalFeatureOrderQuantity==0 || totalUnproducedFeature==0)?0: ((decimal)totalUnproducedFeature / totalFeatureOrderQuantity) * 100;
            unproducedAverage.Add(new SixMonthsUnproducedAverageQtyModel
            {
                ProjectType = "Smart",
                AverageValue = Convert.ToString(smartAverage),
                Percentage = Convert.ToString(Math.Round(smartPercentage))
            });
            unproducedAverage.Add(new SixMonthsUnproducedAverageQtyModel
            {
                ProjectType = "Feature",
                AverageValue = Convert.ToString(featureAverage),
                Percentage = Convert.ToString(Math.Round(featurePercentage))
            });
            ViewBag.Unproduced = unproducedAverage;
            //======================
            //=====SMT capacity exceed log====
            ViewBag.SmtExceedLog = _commonRepository.SmtCapacityExceedLogModels();
            //=============
            return View();
        }

        int IsAnyNullOrEmpty(object myObject)
        {
            var i = 0;
            if (myObject != null)
            {
                foreach (PropertyInfo pi in myObject.GetType().GetProperties())
                {

                    var value = pi.GetValue(myObject);
                    if (value == null)
                    {
                        i++;
                    }

                }
            }
            return i;
        }

        public ActionResult HwAndSwSummaryForManagement(string projectName, long projectMasterId = 0)
        {
            ViewBag.ProjectName = projectName;
            ViewBag.HwFgTests = _repository.GetProjectForHwFinishedTestByProjectName(projectName);
            ViewBag.HwScreeningTests = _repository.GetProjectForHwScreeningTestByProjectName(projectName);
            ViewBag.HwRunningTests = _repository.GetProjectForHwRunningTestByProjectName(projectName);
            ViewBag.SwQcInchargeAssignModels =
                      _repository.GetSwQcInchargeAssignByProjectName(projectName) ?? new List<SwQcInchargeAssignModel>();
            return View();
        }

        public JsonResult MakeAllprojectGanttChart()
        {
            var cmstat = _repository.GetAllCmStatusObject();
            var stat = new List<OverallProjectStatusModel>();
            foreach (var cmStatusObject in cmstat)
            {
                var cmMmPendingActionCount = IsAnyNullOrEmpty(cmStatusObject);
                var hwscrstat = _commonRepository.GetHwScreeningStatusObject(cmStatusObject.ProjectMasterId);
                var hwrunstat = _commonRepository.GetHwRunningStatusObject(cmStatusObject.ProjectMasterId);
                var hwfinstat = _commonRepository.GetHwFinishedStatusObject(cmStatusObject.ProjectMasterId);
                var pmstat = _commonRepository.GetPmStatusObject(cmStatusObject.ProjectMasterId);
                var swqcstat = _commonRepository.GetSwStatusObject(cmStatusObject.ProjectMasterId);

                DateTime? lastactiondate = _repository.GetLastActionDate(cmStatusObject.ProjectMasterId);

                if (cmStatusObject.ApproxProjectFinishDate == null)
                {
                    cmMmPendingActionCount = cmMmPendingActionCount - 1;
                }
                if (cmStatusObject.SourcingType == null)
                {
                    cmMmPendingActionCount = cmMmPendingActionCount - 1;
                }
                if (cmStatusObject.PoClosingDate == null)
                {
                    cmMmPendingActionCount = cmMmPendingActionCount - 1;
                }
                var hwscrPendingActionCount = 0;
                hwscrPendingActionCount = hwscrstat != null ? IsAnyNullOrEmpty(hwscrstat) : 6;
                var hwRunPendingActionCount = 0;
                hwRunPendingActionCount = hwrunstat != null ? IsAnyNullOrEmpty(hwrunstat) : 6;
                var hwfinPendingActionCount = 0;
                hwfinPendingActionCount = hwfinstat != null ? IsAnyNullOrEmpty(hwfinstat) : 6;
                var pmPendingActionCount = 0;
                pmPendingActionCount = pmstat != null ? IsAnyNullOrEmpty(pmstat) : 4;
                var swPendingActionCOunt = 0;
                swPendingActionCOunt = swqcstat != null ? IsAnyNullOrEmpty(swqcstat) : 3;


                if (cmStatusObject.ApproxProjectFinishDate == null || cmStatusObject.ApproxProjectFinishDate < cmStatusObject.ProjectInitialize)
                {
                    cmStatusObject.ApproxProjectFinishDate = Convert.ToDateTime(cmStatusObject.ProjectInitialize).AddDays(1).AddMonths(3).AddDays(-1);
                }
                var overallPendingActionCount = 0;
                if ((cmStatusObject.SourcingType == "OEM" || cmStatusObject.SourcingType == null) && cmStatusObject.OrderNuber == 1)
                {
                    overallPendingActionCount = cmMmPendingActionCount + hwscrPendingActionCount + hwRunPendingActionCount + hwfinPendingActionCount +
                                                    pmPendingActionCount + swPendingActionCOunt;

                    stat.Add(new OverallProjectStatusModel()
                    {
                        ProjectMasterId = cmStatusObject.ProjectMasterId,
                        ProjectName = cmStatusObject.ProjectName + "( Order " + cmStatusObject.OrderNuber + ")",
                        ActionCount = Math.Round(((34 - Convert.ToDouble(overallPendingActionCount)) / 34) * 100, 2),
                        StartDate = cmStatusObject.PurchaseOrder,
                        EndDate = cmStatusObject.ApproxProjectFinishDate,
                        LastActionDate = lastactiondate,
                        PoClosingDate = cmStatusObject.PoClosingDate,
                        IsCompleted = cmStatusObject.IsCompleted 
                    });
                }
                if (cmStatusObject.OrderNuber > 1)
                {
                    overallPendingActionCount = (cmMmPendingActionCount - 2) + hwRunPendingActionCount + hwfinPendingActionCount +
                                                    pmPendingActionCount + swPendingActionCOunt;
                    stat.Add(new OverallProjectStatusModel()
                    {
                        ProjectMasterId = cmStatusObject.ProjectMasterId,
                        ProjectName = cmStatusObject.ProjectName + "( Order " + cmStatusObject.OrderNuber + ")",
                        ActionCount = Math.Round(((26 - Convert.ToDouble(overallPendingActionCount)) / 26) * 100, 2),
                        StartDate = cmStatusObject.ProjectInitialize,
                        EndDate = cmStatusObject.ApproxProjectFinishDate,
                        LastActionDate = lastactiondate,
                        PoClosingDate = cmStatusObject.PoClosingDate,
                        IsCompleted = cmStatusObject.IsCompleted
                    });
                }
                if (cmStatusObject.SourcingType == "ODM" && cmStatusObject.OrderNuber == 1)
                {
                    overallPendingActionCount = (cmMmPendingActionCount - 1) + hwRunPendingActionCount + hwfinPendingActionCount +
                                                     pmPendingActionCount + swPendingActionCOunt;
                    stat.Add(new OverallProjectStatusModel()
                    {
                        ProjectMasterId = cmStatusObject.ProjectMasterId,
                        ProjectName = cmStatusObject.ProjectName + "( Order " + cmStatusObject.OrderNuber + ")",
                        ActionCount = Math.Round(((27 - Convert.ToDouble(overallPendingActionCount)) / 27) * 100, 2),
                        StartDate = cmStatusObject.ProjectInitialize,
                        EndDate = cmStatusObject.ApproxProjectFinishDate,
                        LastActionDate = lastactiondate,
                        PoClosingDate = cmStatusObject.PoClosingDate,
                        IsCompleted = cmStatusObject.IsCompleted
                    });
                }
            }
            //---------------------------------------
            var json = JsonConvert.SerializeObject(stat);
            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult GetDataByModule(long id)
        {
            var combinedstat = new CombinedStatusObject();
            combinedstat.CmStatusObject = _commonRepository.GetCmStatusObject(id);
            combinedstat.PmStatusObject = _commonRepository.GetPmStatusObject(id);
            combinedstat.HwScreeningStatusObject = _commonRepository.GetHwScreeningStatusObject(id);
            combinedstat.HwRunningStatusObject = _commonRepository.GetHwRunningStatusObject(id);
            combinedstat.HwFinishedStatusObject = _commonRepository.GetHwFinishedStatusObject(id);
            combinedstat.SwStatusObject = _commonRepository.GetSwStatusObject(id);
            combinedstat.SwStatusObjects = _commonRepository.GetSwStatusObjects(id);

            string jsonStr = JsonConvert.SerializeObject(combinedstat);
            return new JsonResult { Data = jsonStr, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        

        #region Dashboard Ajax Functions
        [HttpPost]
        public JsonResult GetWorkProgress(long projectId)
        {
            List<WorkProgressData> progresses = _repository.ProjectMonthlyWorkprogress(projectId);
            var json = JsonConvert.SerializeObject(progresses);
            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult GetRecentNofificationAsFeed()
        {
            var feeds = _repository.GetRecentNotifications();
            string jsonData = JsonConvert.SerializeObject(feeds);
            return new JsonResult { Data = jsonData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetProjectWiseRecentNofificationAsFeed(long projectId)
        {
            var feeds = _repository.GetProjectWiseRecentNotifications(projectId);
            string jsonData = JsonConvert.SerializeObject(feeds);
            return new JsonResult { Data = jsonData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public JsonResult GetProjectsByIssueCount()
        {
            string issuesJsonString = _repository.GetProjectsCountByIssueOccured();
            return new JsonResult { Data = issuesJsonString, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public JsonResult GetProjectsByCommentCount()
        {
            string commentJsonString = _repository.GetProjectsCountByCommentOccured();
            return new JsonResult { Data = commentJsonString, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult GetIssuePieData(string projectName, string status)
        {
            List<PieSlideDataModel> issueModels = _repository.GetIssuesForManagerPieSlide(projectName, status);
            string jsonData = JsonConvert.SerializeObject(issueModels);
            return new JsonResult { Data = jsonData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion


        #region Final Approved
        [Authorize(Roles = "MM,SA,PM,PMHEAD,QCHEAD")]
        [HttpGet]
        public ActionResult RunningProjecstList()
        {

            var userId = HttpContext.User.Identity.Name;
            long uId;
            long.TryParse(userId, out uId);
            ViewBag.userInfo = _hwRepository.GetUserInfoByUserId(uId);
            List<ProjectMasterWithPoCustomModel> masterModels = _repository.GetRunningProjectMasterModelList();
            foreach (var m in masterModels)
            {
                m.OrderQuantity = Convert.ToInt64(m.OrderQuantity);
            }
            return View(masterModels);
        }
        #endregion


        #region SampleSetApproval Page

        [Authorize(Roles = "MM,SA,PS,BIHEAD,CEO,CMHEAD")]
        [HttpGet]
        public ActionResult SampleSetApprovalDecision()
        {

            var userId = HttpContext.User.Identity.Name;
            long uId;
            long.TryParse(userId, out uId);
            ViewBag.userInfo = _hwRepository.GetUserInfoByUserId(uId);
            List<ProjectMasterModel> masterModels = _repository.GetInitialApprovalPendingProjectList();
            if (User.IsInRole("BIHEAD"))
            {
                masterModels = masterModels.Where(x => x.BiApprovalBy == null).ToList();
            }
            if (User.IsInRole("PS"))
            {
                masterModels = masterModels.Where(x => x.BiApprovalBy != null && x.PsApprovalBy==null).ToList();
            }
            if (User.IsInRole("CEO"))
            {
                masterModels = masterModels.Where(x => x.CeoApprovalBy == null && x.PsApprovalBy != null && x.BiApprovalBy != null).ToList();
            }
            if (User.IsInRole("MM"))
            {
                masterModels = masterModels.Where(x => x.CeoApprovalBy != null && x.PsApprovalBy != null && x.BiApprovalBy != null && x.InitialApprovalBy==null).ToList();
            }
            ViewBag.InitialApprovedProjects = _commonRepository.GetAllProjects();
            return View(masterModels);
        }
        [HttpPost]
        // [ValidateAntiForgeryToken]
        [NotificationActionFilter(ReceiverRoles = "HWHEAD,CM,MM,QCHEAD,CMBTRC,QC,HW,PS,CEO")]
        public JsonResult SampleSetRejection(long projectId, String comment)
        {
            try
            {
                String updatedMessage = _repository.SetProjectMaster(projectId, comment);//Reject project
                var notificationObject = new NotificationObject
                {

                    ProjectId = projectId,
                    ToUser = "-1",
                };
                notificationObject.Message = "  rejected sample set for screening test ";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                return Json(new { Status = "success" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Status = "failure" }, JsonRequestBehavior.AllowGet);
            }
        }

        [NotificationActionFilter(ReceiverRoles = "HWHEAD,CM,MM,QCHEAD,CMBTRC,QC,HW,PS,CEO")]
        [HttpPost]
        public JsonResult SampleSetApproval(long projectId, String comment)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            try
            {
                _repository.SetSampleSetApproval(projectId, comment);//Approve project

                var notificationObject = new NotificationObject
                {

                    ProjectId = projectId,
                    ToUser = "-1",
                };
                notificationObject.Message = "  has approved sample set for screening test ";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;

                return Json(new { Status = "success" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception exception)
            {

                return Json(new { Status = exception.Message }, JsonRequestBehavior.AllowGet);
            }


        }
        #endregion


        #region ProjectAlive
        [Authorize(Roles = "MM,SA")]
        [HttpGet]
        public ActionResult ProjectAlive()
        {

            List<ProjectMasterModel> models = _repository.GetProjectAlive();

            // models = models.Where(x => x.ProjectStatus == "REJECTED").ToList();
            return View(models);
        }
        [HttpPost]
        public JsonResult ProjectAlive(String comment, String concerns)
        {
            if (concerns == "Commercial")
            {
                //Send Project To Commercial
            }
            else if (concerns == "Hardware")
            {
                //Send Project To Screeningtest
            }

            return Json(new { Status = "failure" }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region HwCmProjectFinalApproval
        [Authorize(Roles = "MM,SA")]
        [HttpGet]
        public ActionResult Approval()
        {
            List<HwCmProjectFinalApprovalViewModel> viewModel = _repository.GetHwCmProjectFinalApprovalViewModel();

            return View(viewModel);
        }

        [HttpPost]
        [NotificationActionFilter(ReceiverRoles = "HWHEAD,CM,MM,QCHEAD,CMBTRC,QC,HW,PS")]
        public JsonResult HwCmProjectFinalApproval(string status, string projectId, string comment)
        {
            //{"status":1,"projectId":"1","comment":"SSSSSSSSSS"}

            try
            {
                long pId = 0;
                long.TryParse(projectId, out pId);
                var notificationObject = new NotificationObject
                {

                    ProjectId = pId,
                    ToUser = "-1",
                };
                if (status == "1")
                {
                    _repository.SetHwCmProjectFinalApproval(pId, comment, "APPROVED");

                    notificationObject.Message = "  approved a project from final approval section.";
                    notificationObject.AdditionalMessage = "";
                    ViewBag.ControllerVariable = notificationObject;

                    return Json(new { Status = "success" }, JsonRequestBehavior.AllowGet);

                }
                _repository.SetHwCmProjectFinalApproval(pId, comment, "REJECTED");
                notificationObject.Message = "  rejected a project from final approval section";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                return Json(new { Status = "declined" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return Json(new { Status = exception.Message }, JsonRequestBehavior.AllowGet);
            }


        }
        #endregion


        [HttpGet]
        public ActionResult FinalDecision(long id = 0)
        {
            var issues = new VmFinalApproval();
            if (id > 0)
            {
                issues.ProjectMasterId = id;
                issues.HwInchargeIssueModels = _commercialRepository.GetScreeningIssues(id);
                return PartialView(issues);
            }

            return PartialView(new VmFinalApproval());
        }
        [NotificationActionFilter(ReceiverRoles = "HWHEAD,CM,MM,QCHEAD,CMBTRC,QC,HW,PS")]
        [HttpPost]
        public ActionResult FinalDecision(VmFinalApproval model)
        {
            if (ModelState.IsValid)
            {
                long result = _repository.SaveFinalDecision(model);
                var notificationObject = new NotificationObject();
                if (result > 0)
                {
                    notificationObject.ProjectId = model.ProjectMasterId;
                    notificationObject.ToUser = "-1";
                    notificationObject.Message = "  approved a project from final approval section.";
                    notificationObject.AdditionalMessage = "";
                    ViewBag.ControllerVariable = notificationObject;
                    return new JsonResult { Data = "ok", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                notificationObject.ProjectId = model.ProjectMasterId;
                notificationObject.ToUser = "-1";
                notificationObject.Message = "  rejected a project from final approval section";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                return new JsonResult { Data = "err", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return new JsonResult { Data = "err", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public ActionResult MarketPriceCalculator()
        {
            var models = new List<MarketPriceModel>();
            models = _repository.GetMarketPriceModels();
            return View(models);
        }
        [HttpPost]
        public JsonResult SaveMarketPrice(int type, long projectId, decimal price, decimal mul, decimal marketPrice)
        {
            string result = _repository.SaveMarketPrice(type, projectId, price, mul, marketPrice);
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public JsonResult GetPrice(string pName)
        {
            string result = _repository.GetLockedPrice(pName);
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult SparePartsWorkProgress()
        {
            var model = _repository.SpareOrderStatus();
            return View(model);
        }

        public ActionResult ProjectSpec(long id)
        {
            ProjectMasterModel model = _commonRepository.GetProjectInfoByProjectId(id);
            MarketPriceModel marketPriceModel = _commonRepository.GetMarketPriceModelByProjectId(id);
            return View(model);
        }

        public ActionResult GetAllPriceTogether(long projectId)
        {
            List<AccessoriesPricesModel> result = _repository.GetAccessoriesPrices(projectId);
            return View(result);
        }

        public ActionResult ProjectVariantLists()
        {
            var model = _commonRepository.GetOrderQuantityDetailsVms();
            foreach (var m in model)
            {
                var rowSpan = 0;
                //foreach (var v in model)
                //{
                //    if (m.OrderNuber == v.OrderNuber && m.ProjectModel == v.ProjectModel)
                //    {
                //        rowSpan = rowSpan + 1;
                //    }
                //}
                if (m.ProjectModel == "Primo HM5")
                {
                    var a = "hello world";
                }
                rowSpan = model.Count(i => i.OrderNuber == m.OrderNuber && i.ProjectModel == m.ProjectModel);
                m.RowSpan = rowSpan;
            }

            var distinctList = model.Select(i =>  new VarientViewModel
                {
                    ModelName = i.ProjectModel,
                    OrderNo = (int) i.OrderNuber
                }).ToList();


            distinctList = distinctList.DistinctBy(x => new {x.ModelName, x.OrderNo}).ToList();
            ViewBag.DistinctList = distinctList;
            return View(model);
        }

        public ActionResult AddVariant(long projectId = 0)
        {
            var model = _commonRepository.GetOrderQuantityDetailsVmsByProjectId(projectId);
            ViewBag.ProjectId = projectId;
            return View(model);
        }

        public JsonResult GetVariantCalculator(long projectId = 0)
        {
            var model = _commonRepository.GetVariantCalculatorByProjectId(projectId);
            return Json(model);
        }

        public JsonResult SaveVariant(string variantName,bool isLocked, long quantity=0,long variantId=0, long projectId = 0)
        {
            var model = new ProjectVariantCalculatorModel();
            var userId = HttpContext.User.Identity.Name;
            long uId;
            long.TryParse(userId, out uId);
            if (variantId == 0)
            {
                model.ProjectId = projectId;
                model.VariantName = variantName;
                model.Quantity = quantity;
                model.AddedBy = uId;
                model.AddedDate = DateTime.Now;
                model.IsLocked = false;
            }
            else
            {
                model = _commonRepository.GetProjectVariantCalculatorById(variantId);
                model.Quantity = quantity;
                model.UpdatedBy = uId;
                model.UpdatedDate = DateTime.Now;
                model.IsLocked = isLocked;
            }
            var json=_commonRepository.SaveProjectVariantCalculator(model);
            return Json(json);
        }

        public JsonResult RemoveVariantCalculator(long variantId = 0)
        {
            _commonRepository.RemoveVariantCalculator(variantId);
            return Json(true);
        }

        public JsonResult GetPreviousOrderVariants(long projectId = 0)
        {
            return Json(_commonRepository.GetPreviousOrderVariants(projectId));
        }


        #region Repeat order approval
        public ActionResult RepeatOrderApproval()
        {
            List<ProjectPurchaseOrderFormModel> formModels = _commercialRepository.GetUnclosedPoList() ?? new List<ProjectPurchaseOrderFormModel>();
            var model = formModels.Where(x => x.OrderNumber > 1 && x.RepeatOrderApproved == null && x.IsCompleted==false).ToList();
            ViewBag.ApporvedOrders = formModels.Where(x => x.OrderNumber > 1 && x.RepeatOrderApproved == "APPROVED" && x.IsCompleted == false).ToList();
            return View(model);
        }

        public JsonResult ApproveRepeatOrder(long orderId = 0)
        {
            try
            {
                _repository.ApproveRepeatOrder(orderId);
                //===Mail===
                List<ProjectPurchaseOrderFormModel> formModels = _commercialRepository.GetUnclosedPoList() ?? new List<ProjectPurchaseOrderFormModel>();
                var model = formModels.FirstOrDefault(x=>x.ProjectPurchaseOrderFormId==orderId);
                if (model != null)
                {
                    var body =
                        string.Format(
                            @"This is to inform you that, A repeat order has been approved in Walton Project Management System By Management.<br/><br/><b>Project Name: " +
                            model.ProjectName + "</b>");
                    var mail = new MailSendFromPms();
                    mail.SendMail(new List<string>(new[] { "CM" }),
                        new List<string>(new[] { "" }), "Repeat order approved for( " + model.ProjectName + " )", body);
                }
                return Json("APPROVED");
            }
            catch (Exception e)
            {
                return Json(e.ToString());
            }
        }

        #endregion

        #region PO Feedback permission

        public ActionResult NegativePoFeedbackDecision()
        {
            var v = _repository.GetNegativeSourcingPoFeedbacks();
            return View(v);
        }

        public JsonResult SaveManagementDecision(string manCom, string manDec, long id = 0)
        {
            var v = _repository.SaveManagementDecision(manCom, manDec, id);
            return Json(v);
        }
        #endregion
    }
}