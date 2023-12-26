using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Ajax.Utilities;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Infrastructures.Repositories;
using ProjectManagement.Models;
using ProjectManagement.ViewModels.Hardware;
using System.Web.Script.Serialization;


namespace ProjectManagement.Controllers
{
    [Authorize]
    public class HardwareController : Controller
    {
        private CellPhoneProjectEntities db = new CellPhoneProjectEntities();
        private IHardwareRepository _repository;
        private readonly CommonRepository _commonRepository;
        private readonly ProjectManagerRepository _projectManagerRepository;

        public HardwareController(HardwareRepository repository, CommonRepository commonRepository)
        {
            _repository = repository;
            _commonRepository = new CommonRepository();
            _projectManagerRepository=new ProjectManagerRepository();
            ViewBag.VerificationPendingCount = _repository.GetVerificationPendingCounts();
            ViewBag.ScreeningForwardCount = _repository.GetScreeningForwardCounter();
            ViewBag.RunningForwardCount = _repository.GetRunningForwardCounter();
            ViewBag.FinishedGoodsCount = _repository.GetFinishedGoodsForwardCounter();
            _commonRepository = commonRepository;
            String useridentity = System.Web.HttpContext.Current.User.Identity.Name;
            var users = Convert.ToInt64(useridentity == "" ? "0" : useridentity);
            ViewBag.ChinaQcInspectionCount = _commonRepository.GetChinaQcInspectionCount(users);
        }
        // GET: Hardware
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "HWHEAD,SA")]
        public ActionResult HwReceivableProjects()
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var userInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.GetReceivableProjects = _repository.GetHwInchargeReceivableProjects();
            foreach (var i in ViewBag.GetReceivableProjects)
            {
                i.ProjectName = i.ProjectName + " (" + CommonConversion.AddOrdinal((int)i.OrderNuber) + " order)";
            }
            return View();
        }

        [NotificationActionFilter(ReceiverRoles = "CM,PM,PMHEAD,MM,PS")]
        [HttpPost]
        public ActionResult HwReceivableProjects(VmHardwareDetailTest vmaVmHardwareDetailTest)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var userInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            _repository.UpdateHwQcIncharge(vmaVmHardwareDetailTest.HwQcAssignCustomMasterModel.HwQcInchargeAssignId,
                vmaVmHardwareDetailTest.HwQcAssignCustomMasterModel.ReceivedSampleQuantity,
                vmaVmHardwareDetailTest.HwQcAssignCustomMasterModel.ReceiveSampleRemark);

            //-------------------MAIL starts----------------------------

            vmaVmHardwareDetailTest.ProjectMasterModel =
                _repository.GetProjectInfoByHwQcInchargeAssignId(vmaVmHardwareDetailTest.HwQcAssignCustomMasterModel.HwQcInchargeAssignId);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<string>(new[] { "CM", "PM" }),
                        new List<string>(new[] { "PMHEAD", "MM", "SA", "PS" }), "Sample Set Received By Hardware for " + vmaVmHardwareDetailTest.HwQcAssignCustomMasterModel.Flag + " (" + vmaVmHardwareDetailTest.ProjectMasterModel.ProjectName + ")",
                        "This is to inform you that, Sample set received by Hardware Dept. from "
                        + (vmaVmHardwareDetailTest.HwQcAssignCustomMasterModel.Flag = (vmaVmHardwareDetailTest.HwQcAssignCustomMasterModel.Flag == "Screening Test") ? "Commercial Dept." : "Project Manager Dept.")
                        + "<br/><br/>Project Name: <b>" + vmaVmHardwareDetailTest.ProjectMasterModel.ProjectName + "</b><br/>Received By : " + userInfo.UserFullName +
                        "<br/>Order Number: " + vmaVmHardwareDetailTest.ProjectMasterModel.OrderNuber +
                        "<br/>Sample Sent :" + vmaVmHardwareDetailTest.HwQcAssignCustomMasterModel.SentSampleQuantity +
                        "<br/>Sample Quantity Received : " + vmaVmHardwareDetailTest.HwQcAssignCustomMasterModel.ReceivedSampleQuantity + "<br/>Sample Type : " + vmaVmHardwareDetailTest.HwQcAssignCustomMasterModel.ProjectManagerSampleType
                        + "<br/>Sample Sent Date : " + vmaVmHardwareDetailTest.HwQcAssignCustomMasterModel.SampleSetSentDate);
            //----------end------------------------

            var notificationObject = new NotificationObject
            {

                ProjectId = vmaVmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                ToUser = "-1",
            };
            notificationObject.Message = " has received sample set for a Project.<br/> ";
            notificationObject.AdditionalMessage = "Sample Quantity : " + vmaVmHardwareDetailTest.HwQcAssignCustomMasterModel.ReceivedSampleQuantity + ", Sample Sent Date : " +
                vmaVmHardwareDetailTest.HwQcAssignCustomMasterModel.SampleSetSentDate + " Sample Type : " + vmaVmHardwareDetailTest.HwQcAssignCustomMasterModel.ProjectManagerSampleType;
            ViewBag.ControllerVariable = notificationObject;
            TempData["message"] = "Project Received";
            return RedirectToAction("HwReceivableProjects");
        }

        //Incharge dashboard
        [Authorize(Roles = "HWHEAD,SA")]
        public ActionResult HwQcInchargeDashboard(VmHardwareTest model)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            //ViewBag.VerificationPendingCount = _repository.GetVerificationPendingCounts();

            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.GetScreeningTestProjectStatus = _repository.GetScreeningTestProjectStatusForInchargeDashboard();
            foreach (var i in ViewBag.GetScreeningTestProjectStatus)
            {
                i.ProjectName = i.ProjectName + " (" + CommonConversion.AddOrdinal((int)i.OrderNuber) + " order)";
            }

            ViewBag.GetRunningTestProjectStatus = _repository.GetRunningTestProjectStatusForInchargeDashboard();
            foreach (var i in ViewBag.GetRunningTestProjectStatus)
            {
                i.ProjectName = i.ProjectName + " (" + CommonConversion.AddOrdinal((int)i.OrderNuber) + " order)";
            }

            ViewBag.GetFinishedGoodsTestProjectStatus = _repository.GetFinishedGoodsTestProjectStatusForInchargeDashboard();
            foreach (var i in ViewBag.GetFinishedGoodsTestProjectStatus)
            {
                i.ProjectName = i.ProjectName + " (" + CommonConversion.AddOrdinal((int)i.OrderNuber) + " order)";
            }
            //model.HwQcTestCounterModel = _repository.GetHwQcInchargeTestCounts(userId);
            DashBoardCounter dashBoardCounter = new DashBoardCounter();
            ViewBag.HwInchargeCounter = dashBoardCounter.GetDashBoardCounter("Hardware", "HWHEAD", userId);
            return View(model);
        }

        public JsonResult GetCounter(string url)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            if (url == "verification")
            {
                int x = _repository.GetVerificationPendingCounts();
                return Json(x, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        //view finished projects
        public ActionResult HwFinishedProjects(VmHardwareTest model)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.GetScreeningTestCompleteProjects = _repository.GetHwScreeningCompleteProjects();
            foreach (var i in ViewBag.GetScreeningTestCompleteProjects)
            {
                i.ProjectName = i.ProjectName + " (" + CommonConversion.AddOrdinal((int)i.OrderNuber) + " order)";
            }
            ViewBag.GetRunningTestCompleteProjects = _repository.GetHwRunningCompleteProjects();
            foreach (var i in ViewBag.GetRunningTestCompleteProjects)
            {
                i.ProjectName = i.ProjectName + " (" + CommonConversion.AddOrdinal((int)i.OrderNuber) + " order)";
            }
            ViewBag.GetFinishedGoodTestCompleteProjects = _repository.GetHwFinishedCompleteProjects();
            foreach (var i in ViewBag.GetFinishedGoodTestCompleteProjects)
            {
                i.ProjectName = i.ProjectName + " (" + CommonConversion.AddOrdinal((int)i.OrderNuber) + " order)";
            }
            return View();
        }

        // QC  assign by Incharge for screening
        [Authorize(Roles = "HWHEAD,SA")]
        public ActionResult HwScreeningTestQcAssign(long projectId = 0)
        {
            VmHardwareTest vmhardwaretest = new VmHardwareTest();
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            vmhardwaretest.HwQcInchargeAssignModel.IsScreeningTest = true;
            ViewBag.ProjectMaster = _repository.GetProjectsAssignedToHwQcInchargeForScreening(); //the value will be the user ID of the Qc Incharge who will log in 
            foreach (var i in ViewBag.ProjectMaster)
            {
                i.ProjectName = i.ProjectName + " (" + CommonConversion.AddOrdinal((int)i.OrderNuber) + " order)";
            }
            var qcEngineers = _repository.GetUsersForHwQcAssign();
            //ViewBag.CmnUser

            vmhardwaretest.ProjectMasterModel = _repository.GetProjectInfoByProjectId(projectId) ?? new ProjectMasterModel();
            vmhardwaretest.HwQcAssignModel = _repository.GetHwQcInchargeAssignIdForScreening(projectId) ?? new HwQcAssignModel();
            vmhardwaretest.HwGetQcAssignedByInchargeModel = _repository.GetQcAssignedByInchargeAssignIdForScreening(vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId, 1);
            //ViewBag.ProjectMaster = _repository.GetProjectsAssignedToHwQcInchargeForScreening(11); //the value will be the user ID of the Qc Incharge who will log in
            List<CmnUserModel> cmnUserModels = qcEngineers.Where(cmnUserModel => !vmhardwaretest.HwGetQcAssignedByInchargeModel.Exists(i => i.CmnUserId.Equals(cmnUserModel.CmnUserId))).ToList();
            ViewBag.CmnUser = cmnUserModels;
            return View(vmhardwaretest);
        }

        //QC forward by Qc Incharge
        [NotificationActionFilter(ReceiverRoles = "CM,MM,PS")]
        [Authorize(Roles = "HWHEAD,SA")]
        public ActionResult HwQcInchargeScreeningForward(VmHardwareTest vmhardwaretest, string remark, long projectId = 0, long hwqcassignid = 0, long hwqcinchargeassignid = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var userInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            var manager = new FileManager();
            vmhardwaretest.HwInchargeIssueModel.ProjectMasterId = projectId;
            //vmhardwaretest.HwQcInchargeAssignModel.Remark = remark;
            if (hwqcinchargeassignid == 0)
            {
                ViewBag.ProjectMaster = _repository.GetHwQcInchargeProjectsForScreeningForward(); // user id of qc here
                foreach (var i in ViewBag.ProjectMaster)
                {
                    i.ProjectName = i.ProjectName + " (" + CommonConversion.AddOrdinal((int)i.OrderNuber) + " order)";
                }
                vmhardwaretest.ProjectMasterModel = _repository.GetProjectInfoByProjectId(projectId) ??
                                                    new ProjectMasterModel();
                vmhardwaretest.HwQcAssignModel =
                    _repository.GetHwQcInchargeAssignIdForScreening(projectId) ?? new HwQcAssignModel();
                ViewBag.QcPassedQcAssigns =
                    _repository.GetQcPassedListByInchargeIdForForward(
                        vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId);
                vmhardwaretest.HwInchargeIssueModel.HwQcInchargeAssignId = vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId;
                vmhardwaretest.HwInchargeIssueModels =
                    _repository.GetHwInchargeIssueModels(vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId);
                vmhardwaretest.HwQcAssignModel = _repository.GetHwQcInchargeAssignIdForScreening(projectId) ?? new HwQcAssignModel();


                vmhardwaretest.HwQcAssignModel.QcDocUploadPath = manager.GetFile(vmhardwaretest.HwQcAssignModel.QcDocUploadPath);

                vmhardwaretest.HwQcAssignModel.ImageExtension = manager.GetExtension(vmhardwaretest.HwQcAssignModel.QcDocUploadPath);
                return View(vmhardwaretest);
            }


            vmhardwaretest.HwQcAssignModel = _repository.GetHwQcInchargeAssignIdForScreening(projectId) ?? new HwQcAssignModel();
            _repository.UpdateHwQcAssignStatusForQC(vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId, "FORWARDED");
            // vmhardwaretest.HwQcAssignModel.HwQcAssignId = _repository.GetHwQcAssignIdForAllTestByProject(projectId, userId, vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId); //I will need it
            _repository.UpdateHwQcInchargeProjectStatus(vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId, remark, "FINISHED");
            _repository.UpdateProjectMasterScreenTestCompleteStatus(projectId);

            //-------------------MAIL starts----------------------------
            vmhardwaretest.HwQcAssignCustomMasterModel = _repository.GetHwQcAssignDetailForVerifyByQcAssignId(hwqcinchargeassignid);
            vmhardwaretest.ProjectMasterModel =
                _repository.GetProjectInfoByHwQcInchargeAssignId(hwqcinchargeassignid);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<string>(new[] { "MM" }),
                        new List<string>(new[] { "HWHEAD", "CM", "SA", "PS" }), "Screening Test Forwarded For Final Approval" + " (" + vmhardwaretest.ProjectMasterModel.ProjectName + ")",
                        "This is to inform you that, Screning Test has been completed by Hardware Dept.<br/><br/>Project Name: <b>" + vmhardwaretest.ProjectMasterModel.ProjectName + "</b><br/>Forwarded By : " + userInfo.UserFullName
                        + "<br/>Test Done By : " + vmhardwaretest.HwQcAssignCustomMasterModel.UserFullName);
            //----------end------------------------

            var notificationObject = new NotificationObject
            {

                ProjectId = projectId,
                ToUser = "-1",
            };
            notificationObject.Message = " has forwarded Completed Screening test ";
            notificationObject.AdditionalMessage = "";
            ViewBag.ControllerVariable = notificationObject;
            return RedirectToAction("HwQcInchargeScreeningForward");


        }

        [NotificationActionFilter(ReceiverRoles = "PM,MM,PS")]
        [Authorize(Roles = "HWHEAD,SA")]
        public ActionResult HwQcInchargeRunningForward(VmHardwareTest vmhardwaretest, string remark, long projectId = 0, long hwqcassignid = 0, long hwqcinchargeassignid = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var userInfo = _repository.GetUserInfoByUserId(userId);
            var manager = new FileManager();
            if (hwqcinchargeassignid == 0)
            {
                ViewBag.ProjectMaster = _repository.GetHwQcInchargeProjectsForRunningForward(); // user id of qc here
                foreach (var i in ViewBag.ProjectMaster)
                {
                    i.ProjectName = i.ProjectName + " (" + CommonConversion.AddOrdinal((int)i.OrderNuber) + " order)";
                }
                vmhardwaretest.ProjectMasterModel = _repository.GetProjectInfoByProjectId(projectId) ??
                                                    new ProjectMasterModel();
                vmhardwaretest.HwQcAssignModel =
                    _repository.GetHwQcInchargeAssignIdForRunning(projectId) ?? new HwQcAssignModel();
                ViewBag.QcPassedQcAssigns =
                    _repository.GetQcPassedListByInchargeIdForForward(
                        vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId);

                vmhardwaretest.HwQcAssignModel = _repository.GetHwQcInchargeAssignIdForRunning(projectId) ?? new HwQcAssignModel();


                vmhardwaretest.HwQcAssignModel.QcDocUploadPath = manager.GetFile(vmhardwaretest.HwQcAssignModel.QcDocUploadPath);

                vmhardwaretest.HwQcAssignModel.ImageExtension = manager.GetExtension(vmhardwaretest.HwQcAssignModel.QcDocUploadPath);
                return View(vmhardwaretest);
            }
            _repository.UpdateHwQcAssignStatusForQC(hwqcinchargeassignid, "FORWARDED");
            vmhardwaretest.HwQcAssignModel = _repository.GetHwQcInchargeAssignIdForRunning(projectId) ?? new HwQcAssignModel();
            // vmhardwaretest.HwQcAssignModel.HwQcAssignId = _repository.GetHwQcAssignIdForAllTestByProject(projectId, userId, vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId); //I will need it
            _repository.UpdateHwQcInchargeProjectStatus(vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId, remark, "FINISHED");
            //_repository.UpdateProjectMasterScreenTestCompleteStatus(projectId);

            //-------------------MAIL starts----------------------------
            vmhardwaretest.HwQcAssignCustomMasterModel = _repository.GetHwQcAssignDetailForVerifyByQcAssignId(hwqcinchargeassignid);
            vmhardwaretest.ProjectMasterModel =
                _repository.GetProjectInfoByHwQcInchargeAssignId(hwqcinchargeassignid);
            var projectmanagerInfo = _repository.GetProjectManagerInfoByProjectid(projectId);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<long>(new[] { projectmanagerInfo.CmnUserId }),
                        new List<string>(new[] { "HWHEAD", "CM", "PMHEAD", "MM", "SA", "PS" }), "Completed Running Test Forwarded" + " (" + vmhardwaretest.ProjectMasterModel.ProjectName + ")",
                        "This is to inform you that, Running Test completed by Hardware Dept.<br/><br/>Project Name: <b>" + vmhardwaretest.ProjectMasterModel.ProjectName + "</b><br/>Forwarded By : " + userInfo.UserFullName
                        + "<br/>Project Manager : " + projectmanagerInfo.UserFullName + "<br/>Test Done By : " + vmhardwaretest.HwQcAssignCustomMasterModel.UserFullName);
            //----------end------------------------

            var notificationObject = new NotificationObject
            {

                ProjectId = projectId,
                ToUser = "-1",
            };
            notificationObject.Message = " has forwarded Completed Running test ";
            notificationObject.AdditionalMessage = "";
            ViewBag.ControllerVariable = notificationObject;
            return RedirectToAction("HwQcInchargeRunningForward");

        }

        [NotificationActionFilter(ReceiverRoles = "PM,MM,PS")]
        [Authorize(Roles = "HWHEAD,SA")]
        public ActionResult HwQcInchargeFinishedGoodsForward(VmHardwareTest vmhardwaretest, string remark, long projectId = 0, long hwqcassignid = 0, long hwqcinchargeassignid = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var userInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);

            var manager = new FileManager();
            if (hwqcinchargeassignid == 0)
            {
                ViewBag.ProjectMaster = _repository.GetHwQcInchargeProjectsForFinishedGoodsForward(); // user id of qc here
                foreach (var i in ViewBag.ProjectMaster)
                {
                    i.ProjectName = i.ProjectName + " (" + CommonConversion.AddOrdinal((int)i.OrderNuber) + " order)";
                }
                vmhardwaretest.ProjectMasterModel = _repository.GetProjectInfoByProjectId(projectId) ??
                                                    new ProjectMasterModel();
                vmhardwaretest.HwQcAssignModel =
                    _repository.GetHwQcInchargeAssignIdForFinishedGoods(projectId) ?? new HwQcAssignModel();
                ViewBag.QcPassedQcAssigns =
                    _repository.GetQcPassedListByInchargeIdForForward(
                        vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId);

                vmhardwaretest.HwQcAssignModel = _repository.GetHwQcInchargeAssignIdForFinishedGoods(projectId) ?? new HwQcAssignModel();


                vmhardwaretest.HwQcAssignModel.QcDocUploadPath = manager.GetFile(vmhardwaretest.HwQcAssignModel.QcDocUploadPath);

                vmhardwaretest.HwQcAssignModel.ImageExtension = manager.GetExtension(vmhardwaretest.HwQcAssignModel.QcDocUploadPath);
                return View(vmhardwaretest);
            }
            _repository.UpdateHwQcAssignStatusForQC(hwqcinchargeassignid, "FORWARDED");
            //_repository.UpdateHwQcAssignStatusForQConForwardProject(vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId, userId, "FORWARDED");//Problem>> single forward status is used for collecting hwqcassginID in many place 

            vmhardwaretest.HwQcAssignModel = _repository.GetHwQcInchargeAssignIdForFinishedGoods(projectId) ?? new HwQcAssignModel();
            // vmhardwaretest.HwQcAssignModel.HwQcAssignId = _repository.GetHwQcAssignIdForAllTestByProject(projectId, userId, vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId); //I will need it
            _repository.UpdateHwQcInchargeProjectStatus(vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId, remark, "FINISHED");
            //_repository.UpdateProjectMasterScreenTestCompleteStatus(projectId);

            //-------------------MAIL starts----------------------------
            vmhardwaretest.HwQcAssignCustomMasterModel = _repository.GetHwQcAssignDetailForVerifyByQcAssignId(hwqcinchargeassignid);
            vmhardwaretest.ProjectMasterModel =
                _repository.GetProjectInfoByHwQcInchargeAssignId(hwqcinchargeassignid);
            var projectmanagerInfo = _repository.GetProjectManagerInfoByProjectid(projectId);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<long>(new[] { projectmanagerInfo.CmnUserId }),
                        new List<string>(new[] { "HWHEAD", "CM", "PMHEAD", "MM", "SA", "PS" }), "Completed Finished Goods Test Forwarded" + " (" + vmhardwaretest.ProjectMasterModel.ProjectName + ")",
                        "This is to inform you that, Finished Goods Test completed by Hardware Dept.<br/><br/>Project Name: <b>" + vmhardwaretest.ProjectMasterModel.ProjectName + "</b><br/>Forwarded By : " + userInfo.UserFullName
                        + "<br/>Project Manager : " + projectmanagerInfo.UserFullName + "<br/>Test Done By : " + vmhardwaretest.HwQcAssignCustomMasterModel.UserFullName);
            //----------end------------------------

            var notificationObject = new NotificationObject
            {

                ProjectId = projectId,
                ToUser = "-1",
            };
            notificationObject.Message = " has forwarded Completed Finished Goods test ";
            notificationObject.AdditionalMessage = "";
            ViewBag.ControllerVariable = notificationObject;
            return RedirectToAction("HwQcInchargeFinishedGoodsForward");
        }

        [HttpPost]
        public ActionResult HwInchargeIssues(VmHardwareTest vmHardwareTest)
        {
            _repository.SaveHwInchargeIssues(vmHardwareTest.HwInchargeIssueModel);
            return RedirectToAction("HwQcInchargeScreeningForward", new { projectId = vmHardwareTest.HwInchargeIssueModel.ProjectMasterId });
        }

        // QC  assign by Incharge for Running
        [Authorize(Roles = "HWHEAD,SA")]
        [HttpGet]
        public ActionResult HwRunningTestQcAssign(long projectId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            VmHardwareTest vmhardwaretest = new VmHardwareTest();
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            vmhardwaretest.HwQcInchargeAssignModel.IsRunningTest = true;
            ViewBag.ProjectMaster = _repository.GetProjectsAssignedToHwQcInchargeForRunning(); //the value will be the user ID of the Qc Incharge who will log in 
            foreach (var i in ViewBag.ProjectMaster)
            {
                i.ProjectName = i.ProjectName + " (" + CommonConversion.AddOrdinal((int)i.OrderNuber) + ")";
            }
            var qcEngineers = _repository.GetUsersForHwQcAssign();
            //if (projectId <= 0)
            //{
            //    return View(vmhardwaretest);
            //}
            vmhardwaretest.ProjectMasterModel = _repository.GetProjectInfoByProjectId(projectId) ?? new ProjectMasterModel();
            vmhardwaretest.HwQcAssignModel = _repository.GetHwQcInchargeAssignIdForRunning(projectId) ?? new HwQcAssignModel();
            vmhardwaretest.HwGetQcAssignedByInchargeModel = _repository.GetQcAssignedByInchargeAssignIdForRunning(vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId, 1);
            List<CmnUserModel> cmnUserModels = qcEngineers.Where(cmnUserModel => !vmhardwaretest.HwGetQcAssignedByInchargeModel.Exists(i => i.CmnUserId.Equals(cmnUserModel.CmnUserId))).ToList();
            ViewBag.CmnUser = cmnUserModels;
            return View(vmhardwaretest);
        }


        [NotificationActionFilter(ReceiverRoles = "HW,HWHEAD,MM,PS")]
        [HttpPost]
        public ActionResult PostHwQcAssign(VmHardwareTest vmhardwaretest)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.CmnUser = _repository.GetUsersForHwQcAssign();
            if (ModelState.IsValidField("HwQcUserId,HwQcInchargeAssignId"))
            {
                if (string.IsNullOrWhiteSpace(vmhardwaretest.HwQcAssignUserIds))
                {
                    TempData["message"] = "ERROR !!! There is no selected engineer to assign. Please select one";
                    if (vmhardwaretest.HwQcInchargeAssignModel.IsScreeningTest == true)
                    {
                        return RedirectToAction("HwScreeningTestQcAssign", new { projectId = vmhardwaretest.ProjectMasterModel.ProjectMasterId });
                    }
                    if (vmhardwaretest.HwQcInchargeAssignModel.IsRunningTest == true)
                    {
                        return RedirectToAction("HwRunningTestQcAssign", new { projectId = vmhardwaretest.ProjectMasterModel.ProjectMasterId });
                    }
                    if (vmhardwaretest.HwQcInchargeAssignModel.IsFinishedGoodTest == true)
                    {
                        return RedirectToAction("HwFinishedGoodsTestQcAssign", new { projectId = vmhardwaretest.ProjectMasterModel.ProjectMasterId });
                    }
                }
                else
                {
                    long[] assignIds = vmhardwaretest.HwQcAssignUserIds.Split(',').Select(Int64.Parse).ToArray();
                    if (assignIds.Any())
                    {
                        if (vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId != 0)
                        {
                            //--------------------------------Code for checking duplicate qc engineer assigned------------------------



                            //var isDuplicate = _repository.CheckDuplicateHwQcAssign(vmhardwaretest.HwQcAssignModel.HwQcUserId, vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId);
                            //if (isDuplicate == 1)
                            //{
                            //    if (vmhardwaretest.HwQcInchargeAssignModel.IsScreeningTest == true)
                            //    {
                            //        vmhardwaretest.ProjectMasterModel = _repository.GetProjectInfoByProjectId(vmhardwaretest.ProjectMasterModel.ProjectMasterId) ?? new ProjectMasterModel();
                            //        vmhardwaretest.HwQcAssignModel = _repository.GetHwQcInchargeAssignIdForScreening(vmhardwaretest.ProjectMasterModel.ProjectMasterId) ?? new HwQcAssignModel();
                            //        vmhardwaretest.HwGetQcAssignedByInchargeModel = _repository.GetQcAssignedByInchargeAssignIdForScreening(vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId, 1);
                            //        ViewBag.ProjectMaster = _repository.GetProjectsAssignedToHwQcInchargeForScreening(); //the value will be the user ID of the Qc Incharge who will log in
                            //        ViewBag.duplicatemessage = "This Engineer has already been assigned for this project";
                            //        return View("HwScreeningTestQcAssign", vmhardwaretest);
                            //    }

                            //    if (vmhardwaretest.HwQcInchargeAssignModel.IsRunningTest == true)
                            //    {
                            //        vmhardwaretest.ProjectMasterModel = _repository.GetProjectInfoByProjectId(vmhardwaretest.ProjectMasterModel.ProjectMasterId) ?? new ProjectMasterModel();
                            //        vmhardwaretest.HwQcAssignModel = _repository.GetHwQcInchargeAssignIdForRunning(vmhardwaretest.ProjectMasterModel.ProjectMasterId) ?? new HwQcAssignModel();
                            //        vmhardwaretest.HwGetQcAssignedByInchargeModel = _repository.GetQcAssignedByInchargeAssignIdForRunning(vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId, 1);
                            //        ViewBag.ProjectMaster = _repository.GetProjectsAssignedToHwQcInchargeForRunning(); //the value will be the user ID of the Qc Incharge who will log in 
                            //        ViewBag.duplicatemessage = "This Engineer has already been assigned for this project";
                            //        return View("HwRunningTestQcAssign", vmhardwaretest);
                            //    }
                            //    if (vmhardwaretest.HwQcInchargeAssignModel.IsFinishedGoodTest == true)
                            //    {
                            //        vmhardwaretest.ProjectMasterModel = _repository.GetProjectInfoByProjectId(vmhardwaretest.ProjectMasterModel.ProjectMasterId) ?? new ProjectMasterModel();
                            //        vmhardwaretest.HwQcAssignModel = _repository.GetHwQcInchargeAssignIdForFinishedGoods(vmhardwaretest.ProjectMasterModel.ProjectMasterId) ?? new HwQcAssignModel();
                            //        vmhardwaretest.HwGetQcAssignedByInchargeModel = _repository.GetQcAssignedByInchargeAssignIdForFinishedGoods(vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId, 1);
                            //        ViewBag.ProjectMaster = _repository.GetProjectsAssignedToHwQcInchargeForFinishedGoods(); //the value will be the user ID of the Qc Incharge who will log in 
                            //        ViewBag.duplicatemessage = "This Engineer has already been assigned for this project";
                            //        return View("HwFinishedGoodsTestQcAssign", vmhardwaretest);
                            //    }
                            //}

                            _repository.UpdateHwQcInchargeProjectStatus(vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId, "", "ASSIGNED");//have to correct it later===========================
                            vmhardwaretest.HwQcAssignModel.ProjectMasterId = vmhardwaretest.ProjectMasterModel.ProjectMasterId;
                            _repository.SaveHwQcAssign(vmhardwaretest.HwQcAssignModel, assignIds);
                            TempData["message"] = "Engineer assigned successfully";
                            //-------------------MAIL starts----------------------------
                            var assignedUser = _repository.GetUserInfoByUserId(vmhardwaretest.HwQcAssignModel.HwQcUserId);
                            vmhardwaretest.ProjectMasterModel =
                                _repository.GetProjectInfoByHwQcInchargeAssignId(
                                    vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId);
                            MailSendFromPms mailSendFromPms = new MailSendFromPms();
                            if (vmhardwaretest.HwQcInchargeAssignModel.IsScreeningTest == true)
                            {
                                mailSendFromPms.SendMail(assignIds.ToList(),
                                    new List<string>(new[] { "MM", "SA", "PS", "HWHEAD" }), "Project Assigned For Hardware Screening Test" + " (" + vmhardwaretest.ProjectMasterModel.ProjectName + ")",
                                    "This is to inform you that, You have been assigned for hardware Screening test for the project: <b>" + vmhardwaretest.ProjectMasterModel.ProjectName + "</b>.<br/><br/>Project Assigned to: <b>" + assignedUser.UserFullName + "</b>");
                            }
                            if (vmhardwaretest.HwQcInchargeAssignModel.IsRunningTest == true)
                            {
                                mailSendFromPms.SendMail(assignIds.ToList(),
                                    new List<string>(new[] { "MM", "SA", "PS", "HWHEAD" }), "Project Assigned For Hardware Running Test" + " (" + vmhardwaretest.ProjectMasterModel.ProjectName + ")",
                                    "This is to inform you that, You have been assigned for hardware running test for the project: <b>" + vmhardwaretest.ProjectMasterModel.ProjectName + "</b>.<br/><br/>Project Assigned to: <b>" + assignedUser.UserFullName + "</b>");
                            }
                            if (vmhardwaretest.HwQcInchargeAssignModel.IsFinishedGoodTest == true)
                            {
                                mailSendFromPms.SendMail(assignIds.ToList(),
                                    new List<string>(new[] { "MM", "SA", "PS", "HWHEAD" }), "Project Assigned For Hardware Finished Goods Test" + " (" + vmhardwaretest.ProjectMasterModel.ProjectName + ")",
                                    "This is to inform you that, You have been assigned for hardware finished goods test for the project: <b>" + vmhardwaretest.ProjectMasterModel.ProjectName + "</b>.<br/><br/>Project Assigned to: <b>" + assignedUser.UserFullName + "</b>");
                            }

                            //---------------------- ENDS --------------------------
                            var notificationObject = new NotificationObject
                            {

                                ProjectId = vmhardwaretest.ProjectMasterModel.ProjectMasterId,
                                ToUser = vmhardwaretest.HwQcAssignUserIds,
                            };
                            if (vmhardwaretest.HwQcInchargeAssignModel.IsScreeningTest == true)
                            {
                                notificationObject.Message = "assigned for screening test";
                                notificationObject.AdditionalMessage = "";

                                ViewBag.ProjectMaster = _repository.GetProjectsAssignedToHwQcInchargeForScreening(); //the value will be the user ID of the Qc Incharge who will log in

                                foreach (var i in ViewBag.ProjectMaster)
                                {
                                    i.ProjectName = i.ProjectName + " (" + CommonConversion.AddOrdinal((int)i.OrderNuber) + ")";
                                }


                                ViewBag.ControllerVariable = notificationObject;
                                return RedirectToAction("HwScreeningTestQcAssign", new { projectId = vmhardwaretest.ProjectMasterModel.ProjectMasterId });
                            }
                            if (vmhardwaretest.HwQcInchargeAssignModel.IsRunningTest == true)
                            {
                                notificationObject.Message = "assigned for running test";
                                notificationObject.AdditionalMessage = "";
                                ViewBag.ProjectMaster = _repository.GetProjectsAssignedToHwQcInchargeForRunning(); //the value will be the user ID of the Qc Incharge who will log in 
                                foreach (var i in ViewBag.ProjectMaster)
                                {
                                    i.ProjectName = i.ProjectName + " (" + CommonConversion.AddOrdinal((int)i.OrderNuber) + ")";
                                }

                                ViewBag.ControllerVariable = notificationObject;
                                return RedirectToAction("HwRunningTestQcAssign", new { projectId = vmhardwaretest.ProjectMasterModel.ProjectMasterId });
                            }
                            if (vmhardwaretest.HwQcInchargeAssignModel.IsFinishedGoodTest == true)
                            {
                                notificationObject.Message = "assigned for finished goods test";
                                notificationObject.AdditionalMessage = "";
                                ViewBag.ProjectMaster = _repository.GetProjectsAssignedToHwQcInchargeForFinishedGoods(); //the value will be the user ID of the Qc Incharge who will log in
                                foreach (var i in ViewBag.ProjectMaster)
                                {
                                    i.ProjectName = i.ProjectName + " (" + CommonConversion.AddOrdinal((int)i.OrderNuber) + ")";
                                }

                                ViewBag.ControllerVariable = notificationObject;
                                return RedirectToAction("HwFinishedGoodsTestQcAssign", new { projectId = vmhardwaretest.ProjectMasterModel.ProjectMasterId });
                            }
                        }
                    }
                }

            }
            TempData["message"] = "Data is not valied !!!";
            if (vmhardwaretest.HwQcInchargeAssignModel.IsScreeningTest == true)
            {
                return RedirectToAction("HwScreeningTestQcAssign", new { projectId = vmhardwaretest.ProjectMasterModel.ProjectMasterId });
            }
            if (vmhardwaretest.HwQcInchargeAssignModel.IsRunningTest == true)
            {
                return RedirectToAction("HwRunningTestQcAssign", new { projectId = vmhardwaretest.ProjectMasterModel.ProjectMasterId });
            }
            if (vmhardwaretest.HwQcInchargeAssignModel.IsFinishedGoodTest == true)
            {
                return RedirectToAction("HwFinishedGoodsTestQcAssign", new { projectId = vmhardwaretest.ProjectMasterModel.ProjectMasterId });
            }
            return View(vmhardwaretest);
        }




        //screening test by QC
        [Authorize(Roles = "HWHEAD,HW,SA")]
        public ActionResult HwQcDashboard(VmHardwareTest vmhardwaretest)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            //ViewBag.VerificationPendingCount = _repository.GetVerificationPendingCounts();
            ViewBag.ScreeningTests = _repository.GetProjectsAssignedToHwQcForScreeningForDashBoard(userId);
            foreach (var i in ViewBag.ScreeningTests)
            {
                i.ProjectName = i.ProjectName + " (" + CommonConversion.AddOrdinal((int)i.OrderNuber) + " order)";
            }
            ViewBag.RunningTests = _repository.GetProjectsAssignedToHwQcForRunningForDashBoard(userId);
            foreach (var i in ViewBag.RunningTests)
            {
                i.ProjectName = i.ProjectName + " (" + CommonConversion.AddOrdinal((int)i.OrderNuber) + " order)";
            }
            ViewBag.FinishedGoodsTest = _repository.GetProjectsAssignedToHwQcForFinishedGoodsForDashBoard(userId);
            foreach (var i in ViewBag.FinishedGoodsTest)
            {
                i.ProjectName = i.ProjectName + " (" + CommonConversion.AddOrdinal((int)i.OrderNuber) + " order)";
            }
            //vmhardwaretest.HwQcTestCounterModel = _repository.GetHwQcTestCounts(userId);
            DashBoardCounter dashBoardCounter = new DashBoardCounter();
            ViewBag.HwQcTestCounter = dashBoardCounter.GetDashBoardCounter("HardWare", "HW", userId);
            return View(vmhardwaretest);
        }

        [Authorize(Roles = "HWHEAD,HW,SA")]
        public ActionResult HardwareQcScreeningTest(VmHardwareTest vmhardwaretest, long projectId = 0)
        {
            var manager = new FileManager();
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            vmhardwaretest.HwQcInchargeAssignModel.IsScreeningTest = true;
            ViewBag.ProjectMaster = _repository.GetProjectsAssignedToHwQcForScreening(userId);// user id of qc here
            foreach (var i in ViewBag.ProjectMaster)
            {
                i.ProjectName = i.ProjectName + " (" + CommonConversion.AddOrdinal((int)i.OrderNuber) + " order)";
            }
            ViewBag.HwIssueMaster = _repository.GetAllHwIssueMaster();
            ViewBag.HwIssueType = db.HwIssueTypes.ToList();
            ViewBag.HwIssueTypeDetail = db.HwIssueTypeDetails.ToList();
            ViewBag.HwAllIssueCommentsByQcAssignId = _repository.GetIssueCommentsByQcAssignId(vmhardwaretest.HwQcAssignModel.HwQcAssignId);

            //var vmhardwaretest = new VmHardwareTest();
            if (projectId <= 0)
            {
                return View(vmhardwaretest);
            }


            vmhardwaretest.ProjectMasterModel = _repository.GetProjectInfoByProjectId(projectId) ?? new ProjectMasterModel();

            vmhardwaretest.HwQcAssignModel = _repository.GetHwQcInchargeAssignIdForScreening(projectId) ?? new HwQcAssignModel();
            vmhardwaretest.HwQcAssignModel.HwQcAssignId = _repository.GetHwQcAssignIdForAllTestByProject(projectId, userId, vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId);

            vmhardwaretest.HwQcAssignModel.QcDocUploadPath = _repository.GetQcUploadedDocument(vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId);

            vmhardwaretest.HwQcAssignModel.QcDocUploadPath = manager.GetFile(vmhardwaretest.HwQcAssignModel.QcDocUploadPath);

            vmhardwaretest.HwQcAssignModel.ImageExtension = manager.GetExtension(vmhardwaretest.HwQcAssignModel.QcDocUploadPath);

            ViewBag.HwAllIssueCommentsByQcAssignId =
               _repository.GetIssueCommentsByQcAssignId(vmhardwaretest.HwQcAssignModel.HwQcAssignId);
            vmhardwaretest.HwQcInchargeAssignModel.IsScreeningTest = true;
            return View(vmhardwaretest);
        }

        [HttpPost]
        public ActionResult PostDocUploadByHwQc(VmHardwareTest vmhardwaretest)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var fileManager = new FileManager();
            var moduleDirectory = "";
            var userDirectory = "HW";

            ViewBag.ProjectMaster = _repository.GetProjectsAssignedToHwQcForScreening(userId);
            ViewBag.HwIssueMaster = _repository.GetAllHwIssueMaster();
            ViewBag.HwIssueType = db.HwIssueTypes.ToList();
            ViewBag.HwIssueTypeDetail = db.HwIssueTypeDetails.ToList();
            ViewBag.HwAllIssueCommentsByQcAssignId =
            _repository.GetIssueCommentsByQcAssignId(vmhardwaretest.HwQcAssignModel.HwQcAssignId);


            if (vmhardwaretest.HwQcInchargeAssignModel.IsScreeningTest == true)
            {
                moduleDirectory = "HWScreeningUpload";
                vmhardwaretest.HwQcAssignModel.QcDocUploadPath =
                fileManager.Upload(vmhardwaretest.ProjectMasterModel.ProjectMasterId, userDirectory, moduleDirectory,
                    vmhardwaretest.HwQcAssignCustomMasterModel.HwQcDocUpload);
                _repository.UpdateHwQcDocUploadPath(vmhardwaretest.HwQcAssignModel.QcDocUploadPath,
                    vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId);
                return RedirectToAction("HardwareQcScreeningTest", new { projectId = vmhardwaretest.ProjectMasterModel.ProjectMasterId });
            }
            else if (vmhardwaretest.HwQcInchargeAssignModel.IsRunningTest == true)
            {
                moduleDirectory = "HWRunningUpload";
                vmhardwaretest.HwQcAssignModel.QcDocUploadPath =
                fileManager.Upload(vmhardwaretest.ProjectMasterModel.ProjectMasterId, userDirectory, moduleDirectory,
                    vmhardwaretest.HwQcAssignCustomMasterModel.HwQcDocUpload);
                _repository.UpdateHwQcDocUploadPath(vmhardwaretest.HwQcAssignModel.QcDocUploadPath,
                    vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId);
                return RedirectToAction("HwQcRunningTest", new { projectId = vmhardwaretest.ProjectMasterModel.ProjectMasterId });
            }
            else if (vmhardwaretest.HwQcInchargeAssignModel.IsFinishedGoodTest == true)
            {
                moduleDirectory = "HWFinishedGoodsUpload";
                vmhardwaretest.HwQcAssignModel.QcDocUploadPath =
                fileManager.Upload(vmhardwaretest.ProjectMasterModel.ProjectMasterId, userDirectory, moduleDirectory,
                    vmhardwaretest.HwQcAssignCustomMasterModel.HwQcDocUpload);
                _repository.UpdateHwQcDocUploadPath(vmhardwaretest.HwQcAssignModel.QcDocUploadPath,
                    vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId);
                return RedirectToAction("HwQcFinishedGoodsTest", new { projectId = vmhardwaretest.ProjectMasterModel.ProjectMasterId });
            }

            return View();
        }

        [HttpPost]
        public ActionResult _HwIssueComment(VmHardwareTest vmhardwaretest)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            vmhardwaretest.HwIssueCommentModel.HwQcAssignId = vmhardwaretest.HwQcAssignModel.HwQcAssignId;
            vmhardwaretest.HwIssueCommentModel.ProjectMasterId = vmhardwaretest.ProjectMasterModel.ProjectMasterId;
            _repository.SaveHwIssueComment(vmhardwaretest.HwIssueCommentModel);
            ViewBag.ProjectMaster = _repository.GetProjectsAssignedToHwQcForScreening(userId);
            ViewBag.HwIssueMaster = _repository.GetAllHwIssueMaster();
            ViewBag.HwIssueType = db.HwIssueTypes.ToList();
            ViewBag.HwIssueTypeDetail = db.HwIssueTypeDetails.ToList();
            ViewBag.HwAllIssueCommentsByQcAssignId =
                _repository.GetIssueCommentsByQcAssignId(vmhardwaretest.HwQcAssignModel.HwQcAssignId);
            if (vmhardwaretest.HwQcInchargeAssignModel.IsScreeningTest == true)
            {
                return RedirectToAction("HardwareQcScreeningTest", new { projectId = vmhardwaretest.HwIssueCommentModel.ProjectMasterId });
            }
            else if (vmhardwaretest.HwQcInchargeAssignModel.IsRunningTest == true)
            {
                return RedirectToAction("HwQcRunningTest", new { projectId = vmhardwaretest.HwIssueCommentModel.ProjectMasterId });
            }
            else if (vmhardwaretest.HwQcInchargeAssignModel.IsFinishedGoodTest == true)
            {
                return RedirectToAction("HwQcFinishedGoodsTest", new { projectId = vmhardwaretest.HwIssueCommentModel.ProjectMasterId });
            }
            return View();
        }

        public ActionResult HwForwardToSubmitForVerification(VmHardwareTest vmHardwareTest, long hwqcassignId = 0)
        {
            vmHardwareTest.HwQcInchargeAssignModel = _repository.GetTestPhaseByHwQcAssignId(hwqcassignId);
            if (vmHardwareTest.HwQcInchargeAssignModel.IsScreeningTest == true)
            {
                return RedirectToAction("HardwareQcScreeningTest", new { projectId = vmHardwareTest.HwQcInchargeAssignModel.ProjectMasterId });
            }
            else if (vmHardwareTest.HwQcInchargeAssignModel.IsRunningTest == true)
            {
                return RedirectToAction("HwQcRunningTest", new { projectId = vmHardwareTest.HwQcInchargeAssignModel.ProjectMasterId });
            }
            else if (vmHardwareTest.HwQcInchargeAssignModel.IsFinishedGoodTest == true)
            {
                return RedirectToAction("HwQcFinishedGoodsTest", new { projectId = vmHardwareTest.HwQcInchargeAssignModel.ProjectMasterId });
            }
            return RedirectToAction("HwQcDashboard");
        }

        [NotificationActionFilter(ReceiverRoles = "HW,HWHEAD,MM,PS")]
        public ActionResult SubmitForVerification(VmHardwareTest vmhardwaretest, int isScreening = 0, int isRunning = 0, int isFinishedGoods = 0, long hwqcassignId = 0, long projectId = 0, long hwqcinchargeassignId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            vmhardwaretest.CmnUserModel = _repository.GetUserInfoByUserId(userId);
            _repository.UpdateHwQcAssignStatusForQC(hwqcinchargeassignId, "QCSUBMITTED");
            TempData["message"] = "Submit for verification successful";
            ViewBag.HwIssueMaster = _repository.GetAllHwIssueMaster();
            ViewBag.HwIssueType = db.HwIssueTypes.ToList();
            ViewBag.HwIssueTypeDetail = db.HwIssueTypeDetails.ToList();
            ViewBag.HwAllIssueCommentsByQcAssignId =
                _repository.GetIssueCommentsByQcAssignId(hwqcassignId);

            //-----------MAIL--------------
            vmhardwaretest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(hwqcinchargeassignId);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            if (isScreening == 1)
            {
                mailSendFromPms.SendMail(new List<string>(new[] { "HW", "HWHEAD" }), new List<string>(new[] { "MM", "SA", "PS" }),
                    "Screening Test Submitted for Verification" + " (" + vmhardwaretest.ProjectMasterModel.ProjectName + ")",
                    "This is to inform you that, Project: <b>" + vmhardwaretest.ProjectMasterModel.ProjectName + "</b> submitted for verification.<br/>Submitted By : " + vmhardwaretest.CmnUserModel.UserFullName);
            }
            if (isRunning == 1)
            {
                mailSendFromPms.SendMail(new List<string>(new[] { "HW", "HWHEAD" }), new List<string>(new[] { "MM", "SA", "PS" }),
                    "Running Test Submitted for Verification" + " (" + vmhardwaretest.ProjectMasterModel.ProjectName + ")",
                    "This is to inform you that, Project: <b>" + vmhardwaretest.ProjectMasterModel.ProjectName + "</b> submitted for verification.<br/>Submitted By : " + vmhardwaretest.CmnUserModel.UserFullName);
            }
            if (isFinishedGoods == 1)
            {
                mailSendFromPms.SendMail(new List<string>(new[] { "HW", "HWHEAD" }), new List<string>(new[] { "MM", "SA", "PS" }),
                    "Finished Goods Test Submitted for Verification" + " (" + vmhardwaretest.ProjectMasterModel.ProjectName + ")",
                    "This is to inform you that, Project: <b>" + vmhardwaretest.ProjectMasterModel.ProjectName + "</b> submitted for verification.<br/>Submitted By : " + vmhardwaretest.CmnUserModel.UserFullName);
            }

            //----------------------------

            var notificationObject = new NotificationObject
            {

                ProjectId = projectId,
                ToUser = "-1",
            };

            if (isScreening == 1)
            {
                notificationObject.Message = "  submitted screening test for verification ";
                notificationObject.AdditionalMessage = "";
                ViewBag.ProjectMaster = _repository.GetProjectsAssignedToHwQcForScreening(userId);
                ViewBag.ControllerVariable = notificationObject;
                return RedirectToAction("HardwareQcScreeningTest");
            }
            if (isRunning == 1)
            {
                notificationObject.Message = "  submitted Running test for verification ";
                notificationObject.AdditionalMessage = "";
                ViewBag.ProjectMaster = _repository.GetProjectsAssignedToHwQcForRunning(userId);
                ViewBag.ControllerVariable = notificationObject;
                return RedirectToAction("HwQcRunningTest");
            }
            if (isFinishedGoods == 1)
            {
                notificationObject.Message = "  submitted Finished goods test for verification ";
                notificationObject.AdditionalMessage = "";
                ViewBag.ProjectMaster = _repository.GetProjectsAssignedToHwQcForRunning(userId);
                ViewBag.ControllerVariable = notificationObject;
                return RedirectToAction("HwQcFinishedGoodsTest");
            }
            return View();

        }

        public ActionResult HwQcScreeningTestDisplayByProject()
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.ProjectMaster = _repository.GetProjectsAssignedToHwQcForScreening(userId);
            return View();
        }

        //Running Test by QC
        [Authorize(Roles = "HWHEAD,HW,SA")]
        public ActionResult HwQcRunningTest(VmHardwareTest vmhardwaretest, long projectId = 0)
        {
            var manager = new FileManager();
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            vmhardwaretest.HwQcInchargeAssignModel.IsRunningTest = true;
            ViewBag.ProjectMaster = _repository.GetProjectsAssignedToHwQcForRunning(userId);// user id of qc here
            foreach (var i in ViewBag.ProjectMaster)
            {
                i.ProjectName = i.ProjectName + " (" + CommonConversion.AddOrdinal((int)i.OrderNuber) + " order)";
            }
            ViewBag.HwIssueMaster = _repository.GetAllHwIssueMaster();
            ViewBag.HwIssueType = db.HwIssueTypes.ToList();
            ViewBag.HwIssueTypeDetail = db.HwIssueTypeDetails.ToList();
            ViewBag.HwAllIssueCommentsByQcAssignId =
               _repository.GetIssueCommentsByQcAssignId(vmhardwaretest.HwQcAssignModel.HwQcAssignId);
            if (projectId <= 0)
            {
                return View(vmhardwaretest);
            }
            vmhardwaretest.ProjectMasterModel = _repository.GetProjectInfoByProjectId(projectId) ?? new ProjectMasterModel();
            vmhardwaretest.HwQcAssignModel = _repository.GetHwQcInchargeAssignIdForRunning(projectId) ?? new HwQcAssignModel();
            vmhardwaretest.HwQcAssignModel.HwQcAssignId = _repository.GetHwQcAssignIdForAllTestByProject(projectId, userId, vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId);

            vmhardwaretest.HwQcAssignModel.QcDocUploadPath = _repository.GetQcUploadedDocument(vmhardwaretest.HwQcAssignModel.HwQcAssignId);

            vmhardwaretest.HwQcAssignModel.QcDocUploadPath = manager.GetFile(vmhardwaretest.HwQcAssignModel.QcDocUploadPath);

            vmhardwaretest.HwQcAssignModel.ImageExtension = manager.GetExtension(vmhardwaretest.HwQcAssignModel.QcDocUploadPath);

            vmhardwaretest.HwQcInchargeAssignModel.IsRunningTest = true;
            ViewBag.HwAllIssueCommentsByQcAssignId =
               _repository.GetIssueCommentsByQcAssignId(vmhardwaretest.HwQcAssignModel.HwQcAssignId);
            return View(vmhardwaretest);
        }

        //Finished goods test by QC
        [Authorize(Roles = "HWHEAD,HW,SA")]
        public ActionResult HwQcFinishedGoodsTest(VmHardwareTest vmhardwaretest, long projectId = 0)
        {
            var manager = new FileManager();
            long userId = Convert.ToInt64(User.Identity.Name);
            vmhardwaretest.HwQcInchargeAssignModel.IsFinishedGoodTest = true;
            ViewBag.ProjectMaster = _repository.GetProjectsAssignedToHwQcForFinishedGoods(userId);// user id of qc here
            foreach (var i in ViewBag.ProjectMaster)
            {
                i.ProjectName = i.ProjectName + " (" + CommonConversion.AddOrdinal((int)i.OrderNuber) + " order)";
            }
            ViewBag.HwIssueMaster = _repository.GetAllHwIssueMaster();
            ViewBag.HwIssueType = db.HwIssueTypes.ToList();
            ViewBag.HwIssueTypeDetail = db.HwIssueTypeDetails.ToList();
            ViewBag.HwAllIssueCommentsByQcAssignId =
               _repository.GetIssueCommentsByQcAssignId(vmhardwaretest.HwQcAssignModel.HwQcAssignId);
            if (projectId <= 0)
            {
                return View(vmhardwaretest);
            }
            vmhardwaretest.ProjectMasterModel = _repository.GetProjectInfoByProjectId(projectId) ?? new ProjectMasterModel();
            vmhardwaretest.HwQcAssignModel = _repository.GetHwQcInchargeAssignIdForFinishedGoods(projectId) ?? new HwQcAssignModel();
            vmhardwaretest.HwQcAssignModel.HwQcAssignId = _repository.GetHwQcAssignIdForAllTestByProject(projectId, userId, vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId);

            vmhardwaretest.HwQcAssignModel.QcDocUploadPath = _repository.GetQcUploadedDocument(vmhardwaretest.HwQcAssignModel.HwQcAssignId);

            vmhardwaretest.HwQcAssignModel.QcDocUploadPath = manager.GetFile(vmhardwaretest.HwQcAssignModel.QcDocUploadPath);

            vmhardwaretest.HwQcAssignModel.ImageExtension = manager.GetExtension(vmhardwaretest.HwQcAssignModel.QcDocUploadPath);

            vmhardwaretest.HwQcInchargeAssignModel.IsFinishedGoodTest = true;
            ViewBag.HwAllIssueCommentsByQcAssignId =
               _repository.GetIssueCommentsByQcAssignId(vmhardwaretest.HwQcAssignModel.HwQcAssignId);
            return View(vmhardwaretest);
        }


        [NotificationActionFilter(ReceiverRoles = "HW,HWHEAD,MM,PS")]
        //Qc Verification
        [Authorize(Roles = "HWHEAD,HW,SA")]
        public ActionResult HwQcVerification(VmHardwareTest vmhardwaretest, string flag, long hwqcassignId = 0, long hwqcinchargeassignId = 0, string status = "")
        {
            ViewBag.HwQcInchargeAssignId = hwqcinchargeassignId;
            long userId = Convert.ToInt64(User.Identity.Name);
            var userInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.flag = flag;
            var manager = new FileManager();
            if (status == "")
            {
                vmhardwaretest.HwQcAssignModel.QcDocUploadPath = _repository.GetQcUploadedDocument(hwqcinchargeassignId);

                vmhardwaretest.HwQcAssignModel.QcDocUploadPath = manager.GetFile(vmhardwaretest.HwQcAssignModel.QcDocUploadPath);

                vmhardwaretest.HwQcAssignModel.ImageExtension = manager.GetExtension(vmhardwaretest.HwQcAssignModel.QcDocUploadPath);
                vmhardwaretest.HwQcAssignCustomMasterModel = _repository.GetHwQcAssignDetailForVerifyByQcAssignId(hwqcinchargeassignId);
                ViewBag.HwAllIssueCommentsByQcAssignId = _repository.GetIssueCommentsByQcAssignId(hwqcassignId);
                return View(vmhardwaretest);
            }

            vmhardwaretest.CmnUserModel = _repository.GetUserInfoByUserId(userId);
            _repository.UpdateQcAssignStatByVerifier(hwqcinchargeassignId, userId, status, vmhardwaretest.CmnUserModel.UserFullName);
            //vmhardwaretest.HwQcAssignCustomMasterModel = _repository.GetHwQcAssignDetailForVerifyByQcAssignId(hwqcassignId);
            _repository.UpdateHwInchargeTestPhaseAfterAllQcDone(hwqcinchargeassignId, status);
            vmhardwaretest.HwQcAssignCustomMasterModel = _repository.GetHwQcAssignDetailForVerifyByQcAssignId(hwqcinchargeassignId);
            ViewBag.HwAllIssueCommentsByQcAssignId = _repository.GetIssueCommentsByQcAssignId(hwqcassignId);
            //return RedirectToAction("HwQcVerification");
            var notificationObject = new NotificationObject
            {
                ProjectId = vmhardwaretest.HwQcAssignCustomMasterModel.ProjectMasterId,
                ToUser = "-1",
            };
            if (status == "RUNNING")
            {
                //-------------------MAIL starts----------------------------
                vmhardwaretest.ProjectMasterModel =
                    _repository.GetProjectInfoByHwQcInchargeAssignId(hwqcinchargeassignId);
                MailSendFromPms mailSendFromPms = new MailSendFromPms();
                mailSendFromPms.SendMail(new List<string>(new[] { "HWHEAD" }),
                            new List<string>(new[] { "HW", "MM", "SA", "PS" }), "Project reverted for correction" + " (" + vmhardwaretest.ProjectMasterModel.ProjectName + ")",
                            "This is to inform you that, Project Name: <b>" + vmhardwaretest.ProjectMasterModel.ProjectName + "</b><br/>Reverted By : " + userInfo.UserFullName
                            + "<br/>Project Submitted By : " + vmhardwaretest.HwQcAssignCustomMasterModel.UserFullName);
                //----------end------------------------
                notificationObject.Message = " has reverted a project for correction";
                notificationObject.AdditionalMessage = "project submitted by " + vmhardwaretest.HwQcAssignCustomMasterModel.UserFullName;
                ViewBag.ControllerVariable = notificationObject;
            }
            else
            {
                //-------------------MAIL starts----------------------------
                vmhardwaretest.ProjectMasterModel =
                    _repository.GetProjectInfoByHwQcInchargeAssignId(hwqcinchargeassignId);
                MailSendFromPms mailSendFromPms = new MailSendFromPms();
                mailSendFromPms.SendMail(new List<string>(new[] { "HWHEAD" }),
                            new List<string>(new[] { "HW", "MM", "SA", "PS" }), "Hardware Test Verification Completed" + " (" + vmhardwaretest.ProjectMasterModel.ProjectName + ")",
                            "This is to inform you that, Project Name: <b>" + vmhardwaretest.ProjectMasterModel.ProjectName + "</b><br/>Verification Completed By : " + userInfo.UserFullName
                            + "<br/>Project Submitted By : " + vmhardwaretest.HwQcAssignCustomMasterModel.UserFullName);
                //----------end------------------------
                notificationObject.Message = " has completed verification";
                notificationObject.AdditionalMessage = "project submitted by " + vmhardwaretest.HwQcAssignCustomMasterModel.UserFullName;
                ViewBag.ControllerVariable = notificationObject;
            }

            _repository.NotificationForProjectsReadyToForward(vmhardwaretest.HwQcAssignCustomMasterModel.HwQcInchargeAssignId, userId);//Custom notification
            return RedirectToAction("HwQcVerificationPending");

        }

        public ActionResult UpdateHwIssueComment(VmHardwareTest vmhardwaretest)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            _repository.UpdateIssueCommentByQcVerifier(vmhardwaretest.HwIssueCommentModel.VerifierComment,
                vmhardwaretest.HwIssueCommentModel.IssueStatus, vmhardwaretest.HwIssueCommentModel.HwIssueCommentId, userId);
            vmhardwaretest.HwQcAssignCustomMasterModel = _repository.GetHwQcAssignDetailForVerifyByQcAssignId(vmhardwaretest.HwQcAssignModel.HwQcAssignId);
            ViewBag.HwAllIssueCommentsByQcAssignId = _repository.GetIssueCommentsByQcAssignId(vmhardwaretest.HwQcAssignModel.HwQcAssignId);
            return RedirectToAction("HwQcVerification", new { hwqcassignId = vmhardwaretest.HwQcAssignModel.HwQcAssignId });
        }

        [Authorize(Roles = "HWHEAD,HW,SA")]
        public ActionResult HwQcVerificationPending(VmHardwareTest vmhardwaretest)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.GetScreeningTestProjectStatus = _repository.GetHwQcScreeningVerificationPending(userId);
            ViewBag.GetRunningTestProjectStatus = _repository.GetHwQcRunningVerificationPending(userId);
            ViewBag.GetFinishedGoodsTestProjectStatus = _repository.GetHwQcFinishedGoodsVerificationPending(userId);
            return View();
        }



        //All test here
        //Screening
        public ActionResult HwTestNavigation(string projectName, long hwQcAssignId = 0, long hwQcInchargeAssignId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(hwQcInchargeAssignId);
            ViewBag.ProjectDetailForModal = _repository.GetProjectAndAssignDetailByHwQcInchargeAssignId(hwQcInchargeAssignId);
            ViewBag.HwQcAssignId = hwQcAssignId;
            ViewBag.HwQcInchargeAssignId = hwQcInchargeAssignId;
            ViewBag.ProjectName = projectName;
            return View();
        }
        public ActionResult HardwareTest()
        {
            ViewBag.ProjectMaster = _repository.GetAllProjects();
            return View();
        }

        public ActionResult HwTestPCBMaterial(long hwQcAssignId = 0, long hwQcInchargeAssignId = 0, long flag = 0)
        {
            var manager = new FileManager();
            ViewBag.HwQcAssignId = hwQcAssignId;
            ViewBag.HwQcInchargeAssignId = hwQcInchargeAssignId;
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(hwQcInchargeAssignId);
            var vmHardwareDetailTest = new VmHardwareDetailTest();
            vmHardwareDetailTest.HwTestPcbModel = _repository.GetHwTestPcb(hwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(hwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestPcbModel != null)
            {
                vmHardwareDetailTest.HwTestPcbModel.QcDocUploadPath = manager.GetFile(vmHardwareDetailTest.HwTestPcbModel.QcDocUploadPath);
                vmHardwareDetailTest.HwTestPcbModel.ImageExtension = manager.GetExtension(vmHardwareDetailTest.HwTestPcbModel.QcDocUploadPath);
                ViewBag.src = vmHardwareDetailTest.HwTestPcbModel.QcDocUploadPath;
            }

            ViewBag.FgFlag = flag;
            return View(vmHardwareDetailTest);
        }

        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public ActionResult HwTestPCBMaterial(VmHardwareDetailTest vmHardwareDetailTest)
        {
            var manager = new FileManager();
            var moduleDirectory = "HwTestPCBMaterial";
            var userDirectory = "HW";
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            string message = null;
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(vmHardwareDetailTest.HwTestPcbModel.HwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestPcbModel.HwQcInchargeAssignId);
            if (ViewBag.HwTestCustomModel.HwTestPcbId == null)
            {
                _repository.SavePcbMaterial(vmHardwareDetailTest.HwTestPcbModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Saved PCB Material info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                message = "data saved";
            }
            else
            {
                _repository.UpdateHwTestPcbMaterial(vmHardwareDetailTest.HwTestPcbModel.HwQcInchargeAssignId, vmHardwareDetailTest.HwTestPcbModel.Thickness, vmHardwareDetailTest.HwTestPcbModel.Materials, vmHardwareDetailTest.HwTestPcbModel.Recommendation, vmHardwareDetailTest.HwTestPcbModel.Comment, userId);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Updated PCB Material info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                message = "data updated";
            }
            vmHardwareDetailTest.ProjectMasterModel =
                _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestPcbModel.HwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestPcbModel.HwQcDocUpload != null)
            {
                vmHardwareDetailTest.HwTestPcbModel.QcDocUploadPath =
            manager.Upload(vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId, userDirectory, moduleDirectory,
                    vmHardwareDetailTest.HwTestPcbModel.HwQcDocUpload);
                _repository.UpdateHwTestPcbDocUploadPath(vmHardwareDetailTest.HwTestPcbModel.QcDocUploadPath, vmHardwareDetailTest.HwTestPcbModel.HwQcInchargeAssignId);
            }
            TempData["message"] = message;
            return RedirectToAction("HwTestPCBMaterial", new
            {
                hwQcAssignId = vmHardwareDetailTest.HwTestPcbModel.HwQcAssignId,
                hwQcInchargeAssignId = vmHardwareDetailTest.HwTestPcbModel.HwQcInchargeAssignId,
                flag = vmHardwareDetailTest.HwQcAssignCustomMasterModel.Flag
            });
        }


        public ActionResult HwTestPcbAComponentInfo(long hwQcAssignId = 0, long hwQcInchargeAssignId = 0, long flag = 0)
        {
            var manager = new FileManager();
            ViewBag.HwQcAssignId = hwQcAssignId;
            ViewBag.HwQcInchargeAssignId = hwQcInchargeAssignId;
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(hwQcInchargeAssignId);
            var vmHardwareDetailTest = new VmHardwareDetailTest();
            ViewBag.HwChipsetModel = _repository.GetAllHwChipsetModel();
            ViewBag.HwFlashIcModel = _repository.GetAllFlashIcModel();
            ViewBag.HwRfModel = _repository.GetAllRfModel();
            ViewBag.HwPmu1IcModel = _repository.GetAllPmu1IcModel();
            vmHardwareDetailTest.HwTestPcbAModel = _repository.GetHwTestPcbA(hwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(hwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestPcbAModel != null)
            {
                vmHardwareDetailTest.HwTestPcbAModel.QcDocUploadPath = manager.GetFile(vmHardwareDetailTest.HwTestPcbAModel.QcDocUploadPath);
                vmHardwareDetailTest.HwTestPcbAModel.ImageExtension = manager.GetExtension(vmHardwareDetailTest.HwTestPcbAModel.QcDocUploadPath);
                ViewBag.src = vmHardwareDetailTest.HwTestPcbAModel.QcDocUploadPath;
            }
            ViewBag.FgFlag = flag;
            return View(vmHardwareDetailTest);
        }

        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public ActionResult HwTestPcbAComponentInfo(VmHardwareDetailTest vmHardwareDetailTest)
        {
            var manager = new FileManager();
            var moduleDirectory = "HwTestPcbAComponentInfo";
            var userDirectory = "HW";
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            string message = null;
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(vmHardwareDetailTest.HwTestPcbAModel.HwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestPcbAModel.HwQcInchargeAssignId);
            if (ViewBag.HwTestCustomModel.HwTestPcbAId == null)
            {
                vmHardwareDetailTest.HwTestPcbAModel.Added = userId;
                vmHardwareDetailTest.HwTestPcbAModel.AddedDate = DateTime.Now;
                _repository.SavePcbaComponentInfo(vmHardwareDetailTest.HwTestPcbAModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Saved PCBA Component info";
                notificationObject.AdditionalMessage = "Chipset:" + vmHardwareDetailTest.HwTestPcbAModel.IcNoSize + "," + "Chipset Vendor:" + vmHardwareDetailTest.HwTestPcbAModel.Chipset_Vendor
                                                       + "," + "Core:" + vmHardwareDetailTest.HwTestPcbAModel.Chipset_Core + "," + "Clock Speed:" + vmHardwareDetailTest.HwTestPcbAModel.Chipset_Speed
                                                       + "," + "RAM:" + vmHardwareDetailTest.HwTestPcbAModel.FlashIC_RAM + "," + "ROM:" + vmHardwareDetailTest.HwTestPcbAModel.FlashIC_ROM;
                ViewBag.ControllerVariable = notificationObject;
                message = "data saved";
            }
            else
            {
                vmHardwareDetailTest.HwTestPcbAModel.Updated = userId;
                vmHardwareDetailTest.HwTestPcbAModel.UpdatedDate = DateTime.Now;
                vmHardwareDetailTest.HwTestPcbAModel.HwTestPcbAId = ViewBag.HwTestCustomModel.HwTestPcbAId;
                _repository.UpdateHwTestPcbA(vmHardwareDetailTest.HwTestPcbAModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Updated PCBA Component info";
                notificationObject.AdditionalMessage = "Chipset:" + vmHardwareDetailTest.HwTestPcbAModel.IcNoSize + "," + "Chipset Vendor:" + vmHardwareDetailTest.HwTestPcbAModel.Chipset_Vendor
                                                       + "," + "Core:" + vmHardwareDetailTest.HwTestPcbAModel.Chipset_Core + "," + "Clock Speed:" + vmHardwareDetailTest.HwTestPcbAModel.Chipset_Speed
                                                       + "," + "RAM:" + vmHardwareDetailTest.HwTestPcbAModel.FlashIC_RAM + "," + "ROM:" + vmHardwareDetailTest.HwTestPcbAModel.FlashIC_ROM;
                ViewBag.ControllerVariable = notificationObject;
                message = "data updated";
            }
            vmHardwareDetailTest.ProjectMasterModel =
                _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestPcbAModel.HwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestPcbAModel.HwQcDocUpload != null)
            {
                vmHardwareDetailTest.HwTestPcbAModel.QcDocUploadPath =
            manager.Upload(vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId, userDirectory, moduleDirectory,
                    vmHardwareDetailTest.HwTestPcbAModel.HwQcDocUpload);
                _repository.UpdateHwTestPcbADocUploadPath(vmHardwareDetailTest.HwTestPcbAModel.QcDocUploadPath, vmHardwareDetailTest.HwTestPcbAModel.HwQcInchargeAssignId);
            }
            TempData["message"] = message;
            return RedirectToAction("HwTestPcbAComponentInfo", new
            {
                hwQcAssignId = vmHardwareDetailTest.HwTestPcbAModel.HwQcAssignId,
                hwQcInchargeAssignId = vmHardwareDetailTest.HwTestPcbAModel.HwQcInchargeAssignId,
                flag = vmHardwareDetailTest.HwQcAssignCustomMasterModel.Flag
            });
        }

        public ActionResult HwTestCameraInfo(long hwQcAssignId = 0, long hwQcInchargeAssignId = 0, long flag = 0)
        {
            var manager = new FileManager();
            ViewBag.HwQcAssignId = hwQcAssignId;
            ViewBag.HwQcInchargeAssignId = hwQcInchargeAssignId;
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(hwQcInchargeAssignId);
            ViewBag.HwFrontCameraIcModel = _repository.GetAllHwFrontCameraIcModel();
            ViewBag.HwBackCameraIcModel = _repository.GetAllHwBackCameraIcModel();
            var vmHardwareDetailTest = new VmHardwareDetailTest();
            vmHardwareDetailTest.HwTestCameraInfoModel = _repository.GetHwTestCameraInfo(hwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(hwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestCameraInfoModel != null)
            {
                vmHardwareDetailTest.HwTestCameraInfoModel.QcDocUploadPath = manager.GetFile(vmHardwareDetailTest.HwTestCameraInfoModel.QcDocUploadPath);
                vmHardwareDetailTest.HwTestCameraInfoModel.ImageExtension = manager.GetExtension(vmHardwareDetailTest.HwTestCameraInfoModel.QcDocUploadPath);
                ViewBag.src = vmHardwareDetailTest.HwTestCameraInfoModel.QcDocUploadPath;
            }
            ViewBag.FgFlag = flag;
            return View(vmHardwareDetailTest);
        }

        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public ActionResult HwTestCameraInfo(VmHardwareDetailTest vmHardwareDetailTest)
        {
            var manager = new FileManager();
            var moduleDirectory = "HwTestCameraInfo";
            var userDirectory = "HW";
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            string message = null;
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(vmHardwareDetailTest.HwTestCameraInfoModel.HwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestCameraInfoModel.HwQcInchargeAssignId);
            if (ViewBag.HwTestCustomModel.HwTestCameraInfoId == null)
            {
                vmHardwareDetailTest.HwTestCameraInfoModel.Added = userId;
                vmHardwareDetailTest.HwTestCameraInfoModel.AddedDate = DateTime.Now;
                _repository.SaveHwTestCameraInfo(vmHardwareDetailTest.HwTestCameraInfoModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Saved Camera info";
                notificationObject.AdditionalMessage = "Front Camera: " + vmHardwareDetailTest.HwTestCameraInfoModel.FrontCamera_MPSW + "," + "Back Camera: " + vmHardwareDetailTest.HwTestCameraInfoModel.BackCamera_MPSW;
                ViewBag.ControllerVariable = notificationObject;
                message = "data saved";
            }
            else
            {
                vmHardwareDetailTest.HwTestCameraInfoModel.Updated = userId;
                vmHardwareDetailTest.HwTestCameraInfoModel.UpdatedDate = DateTime.Now;
                vmHardwareDetailTest.HwTestCameraInfoModel.HwTestCameraInfoId =
                    vmHardwareDetailTest.HwTestCustomModel.HwTestCameraInfoId;
                _repository.UpdateHwTestCameraInfo(vmHardwareDetailTest.HwTestCameraInfoModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Updated Camera info";
                notificationObject.AdditionalMessage = "Front Camera: " + vmHardwareDetailTest.HwTestCameraInfoModel.FrontCamera_MPSW + "," + "Back Camera: " + vmHardwareDetailTest.HwTestCameraInfoModel.BackCamera_MPSW;
                ViewBag.ControllerVariable = notificationObject;
                message = "data updated";
            }
            vmHardwareDetailTest.ProjectMasterModel =
                _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestCameraInfoModel.HwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestCameraInfoModel.HwQcDocUpload != null)
            {
                vmHardwareDetailTest.HwTestCameraInfoModel.QcDocUploadPath =
            manager.Upload(vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId, userDirectory, moduleDirectory,
                    vmHardwareDetailTest.HwTestCameraInfoModel.HwQcDocUpload);
                _repository.UpdateHwTestCameraInfoDocUploadPath(vmHardwareDetailTest.HwTestCameraInfoModel.QcDocUploadPath, vmHardwareDetailTest.HwTestCameraInfoModel.HwQcInchargeAssignId);
            }
            TempData["message"] = message;
            return RedirectToAction("HwTestCameraInfo", new
            {
                hwQcAssignId = vmHardwareDetailTest.HwTestCameraInfoModel.HwQcAssignId,
                hwQcInchargeAssignId = vmHardwareDetailTest.HwTestCameraInfoModel.HwQcInchargeAssignId,
                flag = vmHardwareDetailTest.HwQcAssignCustomMasterModel.Flag
            });
        }

        public ActionResult HwTestTpLcdInfo(long hwQcAssignId = 0, long hwQcInchargeAssignId = 0, long flag = 0)
        {
            var manager = new FileManager();
            ViewBag.HwQcAssignId = hwQcAssignId;
            ViewBag.HwQcInchargeAssignId = hwQcInchargeAssignId;
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(hwQcInchargeAssignId);
            var vmHardwareDetailTest = new VmHardwareDetailTest();
            vmHardwareDetailTest.HwTestTpLcdInfoModel = _repository.GetHwTestTpLcdInfo(hwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(hwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestTpLcdInfoModel != null)
            {
                vmHardwareDetailTest.HwTestTpLcdInfoModel.QcDocUploadPath = manager.GetFile(vmHardwareDetailTest.HwTestTpLcdInfoModel.QcDocUploadPath);
                vmHardwareDetailTest.HwTestTpLcdInfoModel.ImageExtension = manager.GetExtension(vmHardwareDetailTest.HwTestTpLcdInfoModel.QcDocUploadPath);
                ViewBag.src = vmHardwareDetailTest.HwTestTpLcdInfoModel.QcDocUploadPath;
            }
            ViewBag.FgFlag = flag;
            return View(vmHardwareDetailTest);
        }

        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public ActionResult HwTestTpLcdInfo(VmHardwareDetailTest vmHardwareDetailTest)
        {
            var manager = new FileManager();
            var moduleDirectory = "HwTestTpLcdInfo";
            var userDirectory = "HW";
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            string message = null;
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(vmHardwareDetailTest.HwTestTpLcdInfoModel.HwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestTpLcdInfoModel.HwQcInchargeAssignId);
            if (ViewBag.HwTestCustomModel.HwTestTpLcdInfoId == null)
            {
                vmHardwareDetailTest.HwTestTpLcdInfoModel.Added = userId;
                vmHardwareDetailTest.HwTestTpLcdInfoModel.AddedDate = DateTime.Now;
                _repository.SaveHwTestTpLcdInfo(vmHardwareDetailTest.HwTestTpLcdInfoModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Saved TP LCD info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                message = "data saved";
            }
            else
            {
                vmHardwareDetailTest.HwTestTpLcdInfoModel.Updated = userId;
                vmHardwareDetailTest.HwTestTpLcdInfoModel.UpdatedDate = DateTime.Now;
                vmHardwareDetailTest.HwTestTpLcdInfoModel.HwTestTpLcdInfoId =
                    vmHardwareDetailTest.HwTestCustomModel.HwTestTpLcdInfoId;
                _repository.UpdateHwTestTpLcdInfo(vmHardwareDetailTest.HwTestTpLcdInfoModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Updated TP LCD info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                message = "data updated";
            }
            vmHardwareDetailTest.ProjectMasterModel =
                _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestTpLcdInfoModel.HwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestTpLcdInfoModel.HwQcDocUpload != null)
            {
                vmHardwareDetailTest.HwTestTpLcdInfoModel.QcDocUploadPath =
            manager.Upload(vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId, userDirectory, moduleDirectory,
                    vmHardwareDetailTest.HwTestTpLcdInfoModel.HwQcDocUpload);
                _repository.UpdateHwTestTpLcdInfoDocUploadPath(vmHardwareDetailTest.HwTestTpLcdInfoModel.QcDocUploadPath, vmHardwareDetailTest.HwTestTpLcdInfoModel.HwQcInchargeAssignId);
            }
            TempData["message"] = message;
            return RedirectToAction("HwTestTpLcdInfo", new
            {
                hwQcAssignId = vmHardwareDetailTest.HwTestTpLcdInfoModel.HwQcAssignId,
                hwQcInchargeAssignId = vmHardwareDetailTest.HwTestTpLcdInfoModel.HwQcInchargeAssignId,
                flag = vmHardwareDetailTest.HwQcAssignCustomMasterModel.Flag
            });
        }

        public ActionResult HwTestSoundInfo(long hwQcAssignId = 0, long hwQcInchargeAssignId = 0, long flag = 0)
        {
            var manager = new FileManager();
            ViewBag.HwQcAssignId = hwQcAssignId;
            ViewBag.HwQcInchargeAssignId = hwQcInchargeAssignId;
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(hwQcInchargeAssignId);
            var vmHardwareDetailTest = new VmHardwareDetailTest();
            vmHardwareDetailTest.HwTestSoundInfoModel = _repository.GetHwTestSoundInfo(hwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(hwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestSoundInfoModel != null)
            {
                vmHardwareDetailTest.HwTestSoundInfoModel.QcDocUploadPath = manager.GetFile(vmHardwareDetailTest.HwTestSoundInfoModel.QcDocUploadPath);
                vmHardwareDetailTest.HwTestSoundInfoModel.ImageExtension = manager.GetExtension(vmHardwareDetailTest.HwTestSoundInfoModel.QcDocUploadPath);
                ViewBag.src = vmHardwareDetailTest.HwTestSoundInfoModel.QcDocUploadPath;
            }
            ViewBag.FgFlag = flag;
            return View(vmHardwareDetailTest);
        }

        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public ActionResult HwTestSoundInfo(VmHardwareDetailTest vmHardwareDetailTest)
        {
            var manager = new FileManager();
            var moduleDirectory = "HwTestSoundInfo";
            var userDirectory = "HW";
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            string message = null;
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(vmHardwareDetailTest.HwTestSoundInfoModel.HwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestSoundInfoModel.HwQcInchargeAssignId);
            if (ViewBag.HwTestCustomModel.HwTestSoundInfoId == null)
            {
                vmHardwareDetailTest.HwTestSoundInfoModel.Added = userId;
                vmHardwareDetailTest.HwTestSoundInfoModel.AddedDate = DateTime.Now;
                _repository.SaveHwTestSoundInfo(vmHardwareDetailTest.HwTestSoundInfoModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Saved Sound info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                message = "data saved";
            }
            else
            {
                vmHardwareDetailTest.HwTestSoundInfoModel.Updated = userId;
                vmHardwareDetailTest.HwTestSoundInfoModel.UpdatedDate = DateTime.Now;
                vmHardwareDetailTest.HwTestSoundInfoModel.HwTestSoundInfoId =
                    vmHardwareDetailTest.HwTestCustomModel.HwTestSoundInfoId;
                _repository.UpdateHwTestSoundInfo(vmHardwareDetailTest.HwTestSoundInfoModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Updated Sound info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                message = "data updated";
            }
            vmHardwareDetailTest.ProjectMasterModel =
                _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestSoundInfoModel.HwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestSoundInfoModel.HwQcDocUpload != null)
            {
                vmHardwareDetailTest.HwTestSoundInfoModel.QcDocUploadPath =
            manager.Upload(vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId, userDirectory, moduleDirectory,
                    vmHardwareDetailTest.HwTestSoundInfoModel.HwQcDocUpload);
                _repository.UpdateHwTestSoundInfoDocUploadPath(vmHardwareDetailTest.HwTestSoundInfoModel.QcDocUploadPath, vmHardwareDetailTest.HwTestSoundInfoModel.HwQcInchargeAssignId);
            }
            TempData["message"] = message;
            return RedirectToAction("HwTestSoundInfo", new
            {
                hwQcAssignId = vmHardwareDetailTest.HwTestSoundInfoModel.HwQcAssignId,
                hwQcInchargeAssignId = vmHardwareDetailTest.HwTestSoundInfoModel.HwQcInchargeAssignId,
                flag = vmHardwareDetailTest.HwQcAssignCustomMasterModel.Flag
            });
        }

        public ActionResult HwTestFPCandSIMSlotInfo(long hwQcAssignId = 0, long hwQcInchargeAssignId = 0, long flag = 0)
        {
            var manager = new FileManager();
            ViewBag.HwQcAssignId = hwQcAssignId;
            ViewBag.HwQcInchargeAssignId = hwQcInchargeAssignId;
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(hwQcInchargeAssignId);
            var vmHardwareDetailTest = new VmHardwareDetailTest();
            vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel = _repository.GetHwTestFpCandSimSlotInfo(hwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(hwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel != null)
            {
                vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel.QcDocUploadPath = manager.GetFile(vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel.QcDocUploadPath);
                vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel.ImageExtension = manager.GetExtension(vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel.QcDocUploadPath);
                ViewBag.src = vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel.QcDocUploadPath;
            }
            ViewBag.FgFlag = flag;
            return View(vmHardwareDetailTest);
        }


        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public ActionResult HwTestFPCandSIMSlotInfo(VmHardwareDetailTest vmHardwareDetailTest)
        {
            var manager = new FileManager();
            var moduleDirectory = "HwTestSoundInfo";
            var userDirectory = "HW";
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            string message = null;
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel.HwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel.HwQcInchargeAssignId);
            if (ViewBag.HwTestCustomModel.HwTestFpcConnectionAndSIMSlotInfoId == null)
            {
                vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel.Added = userId;
                vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel.AddedDate = DateTime.Now;
                _repository.SaveHwTestFPCandSIMSlotInfo(vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Saved FPC and SIM info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                message = "data saved";
            }
            else
            {
                vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel.Updated = userId;
                vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel.UpdatedDate = DateTime.Now;
                vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel.HwTestFpcConnectionAndSIMSlotInfoId =
                    vmHardwareDetailTest.HwTestCustomModel.HwTestFpcConnectionAndSIMSlotInfoId;
                _repository.UpdateHwTestFPCandSIMSlotInfo(vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Updated FPC and SIM info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                message = "data updated";
            }
            vmHardwareDetailTest.ProjectMasterModel =
                _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel.HwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel.HwQcDocUpload != null)
            {
                vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel.QcDocUploadPath =
            manager.Upload(vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId, userDirectory, moduleDirectory,
                    vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel.HwQcDocUpload);
                _repository.UpdateHwTestFPCandSIMSlotInfoDocUploadPath(vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel.QcDocUploadPath, vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel.HwQcInchargeAssignId);
            }
            TempData["message"] = message;
            return RedirectToAction("HwTestFPCandSIMSlotInfo", new
            {
                hwQcAssignId = vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel.HwQcAssignId,
                hwQcInchargeAssignId = vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel.HwQcInchargeAssignId,
                flag = vmHardwareDetailTest.HwQcAssignCustomMasterModel.Flag
            });
        }

        public ActionResult HwTestBatteryInfo(long hwQcAssignId = 0, long hwQcInchargeAssignId = 0, long flag = 0)
        {
            var manager = new FileManager();
            ViewBag.HwQcAssignId = hwQcAssignId;
            ViewBag.HwQcInchargeAssignId = hwQcInchargeAssignId;
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(hwQcInchargeAssignId);
            var vmHardwareDetailTest = new VmHardwareDetailTest();
            vmHardwareDetailTest.HwTestBatteryInfoModel = _repository.GetHwTestBatteryInfo(hwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(hwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestBatteryInfoModel != null)
            {
                vmHardwareDetailTest.HwTestBatteryInfoModel.QcDocUploadPath = manager.GetFile(vmHardwareDetailTest.HwTestBatteryInfoModel.QcDocUploadPath);
                vmHardwareDetailTest.HwTestBatteryInfoModel.ImageExtension = manager.GetExtension(vmHardwareDetailTest.HwTestBatteryInfoModel.QcDocUploadPath);
                ViewBag.src = vmHardwareDetailTest.HwTestBatteryInfoModel.QcDocUploadPath;
            }
            ViewBag.FgFlag = flag;
            return View(vmHardwareDetailTest);
        }

        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public ActionResult HwTestBatteryInfo(VmHardwareDetailTest vmHardwareDetailTest)
        {
            var manager = new FileManager();
            var moduleDirectory = "HwTestBatteryInfo";
            var userDirectory = "HW";
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            string message = null;
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(vmHardwareDetailTest.HwTestBatteryInfoModel.HwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestBatteryInfoModel.HwQcInchargeAssignId);
            if (ViewBag.HwTestCustomModel.HwTestBatteryInfoId == null)
            {
                vmHardwareDetailTest.HwTestBatteryInfoModel.Added = userId;
                vmHardwareDetailTest.HwTestBatteryInfoModel.AddedDate = DateTime.Now;
                _repository.SaveHwTestBatteryInfo(vmHardwareDetailTest.HwTestBatteryInfoModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Saved Battery info";
                notificationObject.AdditionalMessage = "Battery Capacity: " + vmHardwareDetailTest.HwTestBatteryInfoModel.Battery_Capacity;
                ViewBag.ControllerVariable = notificationObject;
                message = "data saved";
            }
            else
            {
                vmHardwareDetailTest.HwTestBatteryInfoModel.Updated = userId;
                vmHardwareDetailTest.HwTestBatteryInfoModel.UpdatedDate = DateTime.Now;
                vmHardwareDetailTest.HwTestBatteryInfoModel.HwTestBatteryInfoId =
                    vmHardwareDetailTest.HwTestCustomModel.HwTestBatteryInfoId;
                _repository.UpdateHwTestBatteryInfo(vmHardwareDetailTest.HwTestBatteryInfoModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Updated Battery info";
                notificationObject.AdditionalMessage = "Battery Capacity: " + vmHardwareDetailTest.HwTestBatteryInfoModel.Battery_Capacity;
                ViewBag.ControllerVariable = notificationObject;
                message = "data updated";
            }
            vmHardwareDetailTest.ProjectMasterModel =
                _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestBatteryInfoModel.HwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestBatteryInfoModel.HwQcDocUpload != null)
            {
                vmHardwareDetailTest.HwTestBatteryInfoModel.QcDocUploadPath =
            manager.Upload(vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId, userDirectory, moduleDirectory,
                    vmHardwareDetailTest.HwTestBatteryInfoModel.HwQcDocUpload);
                _repository.UpdateHwTestBatteryInfoDocUploadPath(vmHardwareDetailTest.HwTestBatteryInfoModel.QcDocUploadPath, vmHardwareDetailTest.HwTestBatteryInfoModel.HwQcInchargeAssignId);
            }
            TempData["message"] = message;
            return RedirectToAction("HwTestBatteryInfo", new
            {
                hwQcAssignId = vmHardwareDetailTest.HwTestBatteryInfoModel.HwQcAssignId,
                hwQcInchargeAssignId = vmHardwareDetailTest.HwTestBatteryInfoModel.HwQcInchargeAssignId,
                flag = vmHardwareDetailTest.HwQcAssignCustomMasterModel.Flag
            });
        }

        public ActionResult HwTestChargerInfo(long hwQcAssignId = 0, long hwQcInchargeAssignId = 0, long flag = 0)
        {
            var manager = new FileManager();
            ViewBag.HwQcAssignId = hwQcAssignId;
            ViewBag.HwQcInchargeAssignId = hwQcInchargeAssignId;
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(hwQcInchargeAssignId);
            var vmHardwareDetailTest = new VmHardwareDetailTest();
            vmHardwareDetailTest.HwTestChargerInfoModel = _repository.GetHwTestChargerInfo(hwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(hwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestChargerInfoModel != null)
            {
                vmHardwareDetailTest.HwTestChargerInfoModel.QcDocUploadPath = manager.GetFile(vmHardwareDetailTest.HwTestChargerInfoModel.QcDocUploadPath);
                vmHardwareDetailTest.HwTestChargerInfoModel.ImageExtension = manager.GetExtension(vmHardwareDetailTest.HwTestChargerInfoModel.QcDocUploadPath);
                ViewBag.src = vmHardwareDetailTest.HwTestChargerInfoModel.QcDocUploadPath;
            }
            ViewBag.FgFlag = flag;
            return View(vmHardwareDetailTest);
        }

        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public ActionResult HwTestChargerInfo(VmHardwareDetailTest vmHardwareDetailTest)
        {
            var manager = new FileManager();
            var moduleDirectory = "HwTestChargerInfo";
            var userDirectory = "HW";
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            string message = null;
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(vmHardwareDetailTest.HwTestChargerInfoModel.HwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestChargerInfoModel.HwQcInchargeAssignId);
            if (ViewBag.HwTestCustomModel.HwTestChargerInfoId == null)
            {
                vmHardwareDetailTest.HwTestChargerInfoModel.Added = userId;
                vmHardwareDetailTest.HwTestChargerInfoModel.AddedDate = DateTime.Now;
                _repository.SaveHwTestChargerInfo(vmHardwareDetailTest.HwTestChargerInfoModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Saved Charger info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                message = "data saved";
            }
            else
            {
                vmHardwareDetailTest.HwTestChargerInfoModel.Updated = userId;
                vmHardwareDetailTest.HwTestChargerInfoModel.UpdatedDate = DateTime.Now;
                vmHardwareDetailTest.HwTestChargerInfoModel.HwTestChargerInfoId =
                    vmHardwareDetailTest.HwTestCustomModel.HwTestChargerInfoId;
                _repository.UpdateHwTestChargerInfo(vmHardwareDetailTest.HwTestChargerInfoModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Updated Charger info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                message = "data updated";
            }
            vmHardwareDetailTest.ProjectMasterModel =
                _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestChargerInfoModel.HwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestChargerInfoModel.HwQcDocUpload != null)
            {
                vmHardwareDetailTest.HwTestChargerInfoModel.QcDocUploadPath =
            manager.Upload(vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId, userDirectory, moduleDirectory,
                    vmHardwareDetailTest.HwTestChargerInfoModel.HwQcDocUpload);
                _repository.UpdateHwTestChargerInfoDocUploadPath(vmHardwareDetailTest.HwTestChargerInfoModel.QcDocUploadPath, vmHardwareDetailTest.HwTestChargerInfoModel.HwQcInchargeAssignId);
            }
            TempData["message"] = message;
            return RedirectToAction("HwTestChargerInfo", new
            {
                hwQcAssignId = vmHardwareDetailTest.HwTestChargerInfoModel.HwQcAssignId,
                hwQcInchargeAssignId = vmHardwareDetailTest.HwTestChargerInfoModel.HwQcInchargeAssignId,
                flag = vmHardwareDetailTest.HwQcAssignCustomMasterModel.Flag
            });
        }

        public ActionResult HwTestUsbCableInfo(long hwQcAssignId = 0, long hwQcInchargeAssignId = 0, long flag = 0)
        {
            var manager = new FileManager();
            ViewBag.HwQcAssignId = hwQcAssignId;
            ViewBag.HwQcInchargeAssignId = hwQcInchargeAssignId;
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(hwQcInchargeAssignId);
            VmHardwareDetailTest vmHardwareDetailTest = new VmHardwareDetailTest();
            vmHardwareDetailTest.HwTestUSBCableInfoModel = _repository.GetHwTestUSBCableInfo(hwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(hwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestUSBCableInfoModel != null)
            {
                vmHardwareDetailTest.HwTestUSBCableInfoModel.QcDocUploadPath = manager.GetFile(vmHardwareDetailTest.HwTestUSBCableInfoModel.QcDocUploadPath);
                vmHardwareDetailTest.HwTestUSBCableInfoModel.ImageExtension = manager.GetExtension(vmHardwareDetailTest.HwTestUSBCableInfoModel.QcDocUploadPath);
                ViewBag.src = vmHardwareDetailTest.HwTestUSBCableInfoModel.QcDocUploadPath;
            }
            ViewBag.FgFlag = flag;
            return View(vmHardwareDetailTest);
        }

        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public ActionResult HwTestUsbCableInfo(VmHardwareDetailTest vmHardwareDetailTest)
        {
            var manager = new FileManager();
            var moduleDirectory = "HwTestUsbCableInfo";
            var userDirectory = "HW";
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            string message = null;
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(vmHardwareDetailTest.HwTestUSBCableInfoModel.HwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestUSBCableInfoModel.HwQcInchargeAssignId);
            if (ViewBag.HwTestCustomModel.HwTestUSBCableInfoId == null)
            {
                vmHardwareDetailTest.HwTestUSBCableInfoModel.Added = userId;
                vmHardwareDetailTest.HwTestUSBCableInfoModel.AddedDate = DateTime.Now;
                _repository.SaveHwTestUSBCableInfo(vmHardwareDetailTest.HwTestUSBCableInfoModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Saved USB Cable info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                message = "data saved";
            }
            else
            {
                vmHardwareDetailTest.HwTestUSBCableInfoModel.Updated = userId;
                vmHardwareDetailTest.HwTestUSBCableInfoModel.UpdatedDate = DateTime.Now;
                vmHardwareDetailTest.HwTestUSBCableInfoModel.HwTestUSBCableInfoId =
                    vmHardwareDetailTest.HwTestCustomModel.HwTestUSBCableInfoId;
                _repository.UpdateHwTestUSBCableInfo(vmHardwareDetailTest.HwTestUSBCableInfoModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Updated USB Cable info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                message = "data updated";
            }
            vmHardwareDetailTest.ProjectMasterModel =
                _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestUSBCableInfoModel.HwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestUSBCableInfoModel.HwQcDocUpload != null)
            {
                vmHardwareDetailTest.HwTestUSBCableInfoModel.QcDocUploadPath =
            manager.Upload(vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId, userDirectory, moduleDirectory,
                    vmHardwareDetailTest.HwTestUSBCableInfoModel.HwQcDocUpload);
                _repository.UpdateHwTestUSBCableInfoDocUploadPath(vmHardwareDetailTest.HwTestUSBCableInfoModel.QcDocUploadPath, vmHardwareDetailTest.HwTestUSBCableInfoModel.HwQcInchargeAssignId);
            }
            TempData["message"] = message;
            return RedirectToAction("HwTestUsbCableInfo", new
            {
                hwQcAssignId = vmHardwareDetailTest.HwTestUSBCableInfoModel.HwQcAssignId,
                hwQcInchargeAssignId = vmHardwareDetailTest.HwTestUSBCableInfoModel.HwQcInchargeAssignId,
                flag = vmHardwareDetailTest.HwQcAssignCustomMasterModel.Flag
            });
        }

        public ActionResult HwTestEarphoneInterfaceInfo(long hwQcAssignId = 0, long hwQcInchargeAssignId = 0, long flag = 0)
        {
            var manager = new FileManager();
            ViewBag.HwQcAssignId = hwQcAssignId;
            ViewBag.HwQcInchargeAssignId = hwQcInchargeAssignId;
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(hwQcInchargeAssignId);
            VmHardwareDetailTest vmHardwareDetailTest = new VmHardwareDetailTest();
            vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel =
                _repository.GetHwTestEarphoneInterfaceInfoModel(hwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(hwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel != null)
            {
                vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel.QcDocUploadPath = manager.GetFile(vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel.QcDocUploadPath);
                vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel.ImageExtension = manager.GetExtension(vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel.QcDocUploadPath);
                ViewBag.src = vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel.QcDocUploadPath;
            }
            ViewBag.FgFlag = flag;
            return View(vmHardwareDetailTest);
        }

        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public ActionResult HwTestEarphoneInterfaceInfo(VmHardwareDetailTest vmHardwareDetailTest)
        {
            var manager = new FileManager();
            var moduleDirectory = "HwTestEarphoneInterfaceInfo";
            var userDirectory = "HW";
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            string message = null;
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel.HwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel.HwQcInchargeAssignId);
            if (ViewBag.HwTestCustomModel.HwTestEarphoneInterfaceInfoId == null)
            {
                vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel.Added = userId;
                vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel.AddedDate = DateTime.Now;
                _repository.SaveHwTestEarphoneInterfaceInfo(vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Saved Earphone Interface info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                message = "data saved";
            }
            else
            {
                vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel.Updated = userId;
                vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel.UpdatedDate = DateTime.Now;
                vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel.HwTestEarphoneInterfaceInfoId =
                    vmHardwareDetailTest.HwTestCustomModel.HwTestEarphoneInterfaceInfoId;
                _repository.UpdateHwTestEarphoneInterfaceInfo(vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Updated Earphone Interface info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                message = "data updated";
            }
            vmHardwareDetailTest.ProjectMasterModel =
                _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel.HwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel.HwQcDocUpload != null)
            {
                vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel.QcDocUploadPath =
            manager.Upload(vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId, userDirectory, moduleDirectory,
                    vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel.HwQcDocUpload);
                _repository.UpdateHwTestEarphoneInterfaceInfoDocUploadPath(vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel.QcDocUploadPath, vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel.HwQcInchargeAssignId);
            }
            TempData["message"] = message;
            return RedirectToAction("HwTestEarphoneInterfaceInfo", new
            {
                hwQcAssignId = vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel.HwQcAssignId,
                hwQcInchargeAssignId = vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel.HwQcInchargeAssignId,
                flag = vmHardwareDetailTest.HwQcAssignCustomMasterModel.Flag
            });
        }

        public ActionResult HwTestChargingInfo(long hwQcAssignId = 0, long hwQcInchargeAssignId = 0, long flag = 0)
        {
            var manager = new FileManager();
            ViewBag.HwQcAssignId = hwQcAssignId;
            ViewBag.HwQcInchargeAssignId = hwQcInchargeAssignId;
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(hwQcInchargeAssignId);
            VmHardwareDetailTest vmHardwareDetailTest = new VmHardwareDetailTest();
            vmHardwareDetailTest.HwTestChargingInfoModel = _repository.GetHwTestChargingInfoModel(hwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(hwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestChargingInfoModel != null)
            {
                vmHardwareDetailTest.HwTestChargingInfoModel.QcDocUploadPath = manager.GetFile(vmHardwareDetailTest.HwTestChargingInfoModel.QcDocUploadPath);
                vmHardwareDetailTest.HwTestChargingInfoModel.ImageExtension = manager.GetExtension(vmHardwareDetailTest.HwTestChargingInfoModel.QcDocUploadPath);
                ViewBag.src = vmHardwareDetailTest.HwTestChargingInfoModel.QcDocUploadPath;
            }
            ViewBag.FgFlag = flag;
            return View(vmHardwareDetailTest);
        }

        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public ActionResult HwTestChargingInfo(VmHardwareDetailTest vmHardwareDetailTest)
        {
            var manager = new FileManager();
            var moduleDirectory = "HwTestChargingInfo";
            var userDirectory = "HW";
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            string message = null;
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(vmHardwareDetailTest.HwTestChargingInfoModel.HwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestChargingInfoModel.HwQcInchargeAssignId);
            if (ViewBag.HwTestCustomModel.HwTestChargingInfoId == null)
            {
                vmHardwareDetailTest.HwTestChargingInfoModel.Added = userId;
                vmHardwareDetailTest.HwTestChargingInfoModel.AddedDate = DateTime.Now;
                _repository.SaveHwTestChargingInfo(vmHardwareDetailTest.HwTestChargingInfoModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Saved Charging info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                message = "Data saved";
            }
            else
            {
                vmHardwareDetailTest.HwTestChargingInfoModel.Updated = userId;
                vmHardwareDetailTest.HwTestChargingInfoModel.UpdatedDate = DateTime.Now;
                vmHardwareDetailTest.HwTestChargingInfoModel.HwTestChargingInfoId =
                     vmHardwareDetailTest.HwTestCustomModel.HwTestChargingInfoId;
                _repository.UpdateHwTestChargingInfo(vmHardwareDetailTest.HwTestChargingInfoModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Updated Charging info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                message = "Data updated";
            }
            vmHardwareDetailTest.ProjectMasterModel =
                _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestChargingInfoModel.HwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestChargingInfoModel.HwQcDocUpload != null)
            {
                vmHardwareDetailTest.HwTestChargingInfoModel.QcDocUploadPath =
            manager.Upload(vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId, userDirectory, moduleDirectory,
                    vmHardwareDetailTest.HwTestChargingInfoModel.HwQcDocUpload);
                _repository.UpdateHwTestChargingInfoDocUploadPath(vmHardwareDetailTest.HwTestChargingInfoModel.QcDocUploadPath, vmHardwareDetailTest.HwTestChargingInfoModel.HwQcInchargeAssignId);
            }
            TempData["message"] = message;
            return RedirectToAction("HwTestChargingInfo", new
            {
                hwQcAssignId = vmHardwareDetailTest.HwTestChargingInfoModel.HwQcAssignId,
                hwQcInchargeAssignId = vmHardwareDetailTest.HwTestChargingInfoModel.HwQcInchargeAssignId,
                flag = vmHardwareDetailTest.HwQcAssignCustomMasterModel.Flag
            });
        }

        public ActionResult HwTestHousingInfo(long hwQcAssignId = 0, long hwQcInchargeAssignId = 0, long flag = 0)
        {
            var manager = new FileManager();
            ViewBag.HwQcAssignId = hwQcAssignId;
            ViewBag.HwQcInchargeAssignId = hwQcInchargeAssignId;
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(hwQcInchargeAssignId);
            VmHardwareDetailTest vmHardwareDetailTest = new VmHardwareDetailTest();
            vmHardwareDetailTest.HwTestHousingInfoModel = _repository.GetHwTestHousingInfoModel(hwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(hwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestHousingInfoModel != null)
            {
                vmHardwareDetailTest.HwTestHousingInfoModel.QcDocUploadPath = manager.GetFile(vmHardwareDetailTest.HwTestHousingInfoModel.QcDocUploadPath);
                vmHardwareDetailTest.HwTestHousingInfoModel.ImageExtension = manager.GetExtension(vmHardwareDetailTest.HwTestHousingInfoModel.QcDocUploadPath);
                ViewBag.src = vmHardwareDetailTest.HwTestHousingInfoModel.QcDocUploadPath;
            }
            ViewBag.FgFlag = flag;
            return View(vmHardwareDetailTest);
        }


        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public ActionResult HwTestHousingInfo(VmHardwareDetailTest vmHardwareDetailTest)
        {
            var manager = new FileManager();
            var moduleDirectory = "HwTestHousingInfo";
            var userDirectory = "HW";
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            string message = null;
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(vmHardwareDetailTest.HwTestHousingInfoModel.HwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestHousingInfoModel.HwQcInchargeAssignId);
            if (ViewBag.HwTestCustomModel.HwTestHousingInfoId == null)
            {
                vmHardwareDetailTest.HwTestHousingInfoModel.Added = userId;
                vmHardwareDetailTest.HwTestHousingInfoModel.AddedDate = DateTime.Now;
                _repository.SaveHwTestHousingInfo(vmHardwareDetailTest.HwTestHousingInfoModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Saved Housing info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                message = "Data saved";
            }
            else
            {
                vmHardwareDetailTest.HwTestHousingInfoModel.Updated = userId;
                vmHardwareDetailTest.HwTestHousingInfoModel.UpdatedDate = DateTime.Now;
                vmHardwareDetailTest.HwTestHousingInfoModel.HwTestHousingInfoId =
                     vmHardwareDetailTest.HwTestCustomModel.HwTestHousingInfoId;
                _repository.UpdateHwTestHousingInfo(vmHardwareDetailTest.HwTestHousingInfoModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Updated Housing info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                message = "Data updated";
            }
            vmHardwareDetailTest.ProjectMasterModel =
                _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestHousingInfoModel.HwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwTestHousingInfoModel.HwQcDocUpload != null)
            {
                vmHardwareDetailTest.HwTestHousingInfoModel.QcDocUploadPath =
            manager.Upload(vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId, userDirectory, moduleDirectory,
                    vmHardwareDetailTest.HwTestHousingInfoModel.HwQcDocUpload);
                _repository.UpdateHwTestHousingInfoDocUploadPath(vmHardwareDetailTest.HwTestHousingInfoModel.QcDocUploadPath, vmHardwareDetailTest.HwTestHousingInfoModel.HwQcInchargeAssignId);
            }
            TempData["message"] = message;
            return RedirectToAction("HwTestHousingInfo", new
            {
                hwQcAssignId = vmHardwareDetailTest.HwTestHousingInfoModel.HwQcAssignId,
                hwQcInchargeAssignId = vmHardwareDetailTest.HwTestHousingInfoModel.HwQcInchargeAssignId,
                flag = vmHardwareDetailTest.HwQcAssignCustomMasterModel.Flag
            });
        }

        public ActionResult HwTestCrossMatchInfo(long hwQcAssignId = 0, long hwQcInchargeAssignId = 0, long flag = 0)
        {
            ViewBag.HwQcAssignId = hwQcAssignId;
            ViewBag.HwQcInchargeAssignId = hwQcInchargeAssignId;
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(hwQcInchargeAssignId);
            VmHardwareDetailTest vmHardwareDetailTest = new VmHardwareDetailTest();
            vmHardwareDetailTest.HwTestCrossMatchInfoModel = _repository.GetHwTestCrossMatchInfoModel(hwQcInchargeAssignId);
            ViewBag.FgFlag = flag;
            return View(vmHardwareDetailTest);
        }


        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public ActionResult HwTestCrossMatchInfo(VmHardwareDetailTest vmHardwareDetailTest)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            string message = null;
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(vmHardwareDetailTest.HwTestCrossMatchInfoModel.HwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestCrossMatchInfoModel.HwQcInchargeAssignId);
            if (ViewBag.HwTestCustomModel.HwTestCrossMatchInfoId == null)
            {
                vmHardwareDetailTest.HwTestCrossMatchInfoModel.Added = userId;
                vmHardwareDetailTest.HwTestCrossMatchInfoModel.AddedDate = DateTime.Now;
                _repository.SaveHwTestCrossMatchInfo(vmHardwareDetailTest.HwTestCrossMatchInfoModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Saved Cross Match info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                message = "Data saved";
            }
            else
            {
                vmHardwareDetailTest.HwTestCrossMatchInfoModel.Updated = userId;
                vmHardwareDetailTest.HwTestCrossMatchInfoModel.UpdatedDate = DateTime.Now;
                vmHardwareDetailTest.HwTestCrossMatchInfoModel.HwTestCrossMAtchInfoId =
                     vmHardwareDetailTest.HwTestCustomModel.HwTestCrossMatchInfoId;
                _repository.UpdateHwTestCrossMatchInfo(vmHardwareDetailTest.HwTestCrossMatchInfoModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Updated Cross Match info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                message = "Data updated";
            }
            TempData["message"] = message;
            return RedirectToAction("HwTestCrossMatchInfo", new
            {
                hwQcAssignId = vmHardwareDetailTest.HwTestCrossMatchInfoModel.HwQcAssignId,
                hwQcInchargeAssignId = vmHardwareDetailTest.HwTestCrossMatchInfoModel.HwQcInchargeAssignId,
                flag = vmHardwareDetailTest.HwQcAssignCustomMasterModel.Flag
            });
        }

        public ActionResult HwTestOverall(long hwQcAssignId = 0, long hwQcInchargeAssignId = 0, long flag = 0)
        {
            ViewBag.HwQcAssignId = hwQcAssignId;
            ViewBag.HwQcInchargeAssignId = hwQcInchargeAssignId;
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(hwQcInchargeAssignId);
            ViewBag.ProjectDetailForModal = _repository.GetProjectAndAssignDetailByHwQcInchargeAssignId(hwQcInchargeAssignId);
            VmHardwareDetailTest vmHardwareDetailTest = new VmHardwareDetailTest();
            vmHardwareDetailTest.HwTestOverallResultModel = _repository.GetHwTestOverallResultModel(hwQcInchargeAssignId);
            ViewBag.FgFlag = flag;
            return View(vmHardwareDetailTest);
        }


        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public ActionResult HwTestOverall(VmHardwareDetailTest vmHardwareDetailTest)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            string message = null;
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(vmHardwareDetailTest.HwTestOverallResultModel.HwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwTestOverallResultModel.HwQcInchargeAssignId);
            if (ViewBag.HwTestCustomModel.HwTestOverallResultId == null)
            {
                vmHardwareDetailTest.HwTestOverallResultModel.Added = userId;
                vmHardwareDetailTest.HwTestOverallResultModel.AddedDate = DateTime.Now;
                _repository.SaveHwTestOverallResult(vmHardwareDetailTest.HwTestOverallResultModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Saved Overall info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                message = "Data saved";
            }
            else
            {
                vmHardwareDetailTest.HwTestOverallResultModel.Updated = userId;
                vmHardwareDetailTest.HwTestOverallResultModel.UpdatedDate = DateTime.Now;
                vmHardwareDetailTest.HwTestOverallResultModel.HwTestOverallResultId =
                     vmHardwareDetailTest.HwTestCustomModel.HwTestOverallResultId;
                _repository.UpdateHwTestOverallResult(vmHardwareDetailTest.HwTestOverallResultModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Updated Overall info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                message = "Data Updated";
            }
            TempData["message"] = message;
            return RedirectToAction("HwTestOverall", new
            {
                hwQcAssignId = vmHardwareDetailTest.HwTestOverallResultModel.HwQcAssignId,
                hwQcInchargeAssignId = vmHardwareDetailTest.HwTestOverallResultModel.HwQcInchargeAssignId,
                flag = vmHardwareDetailTest.HwQcAssignCustomMasterModel.Flag
            });
        }

        public ActionResult GetAllDoc(string projectName, long hwQcInchargeAssignId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            var v = new VmHardwareDetailTest();
            v.ProjectMasterModel =
                _repository.GetProjectInfoByHwQcInchargeAssignId(hwQcInchargeAssignId);
            v.HwQcAssignCustomMasterModel.HwQcInchargeAssignId = hwQcInchargeAssignId;
            v.HwQcAssignCustomMasterModel.ProjectName = projectName;//+ " (" + CommonConversion.AddOrdinal(v.ProjectMasterModel.OrderNuber) + " order)";
            ViewBag.Files = _repository.GetAllDocs(hwQcInchargeAssignId);
            return View(v);
        }


        [HttpPost]
        public FileResult GetAllDoc(List<string> files, VmHardwareDetailTest vmHardwareDetailTest)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);

            var archive = Server.MapPath("~/archive/UploadedDocs.zip");
            var temp = Server.MapPath("~/temp");

            // clear any existing archive
            if (System.IO.File.Exists(archive))
            {
                System.IO.File.Delete(archive);
            }
            // empty the temp folder
            Directory.EnumerateFiles(temp).ToList().ForEach(f => System.IO.File.Delete(f));

            // copy the selected files to the temp folder
            if (files != null)
            {
                var count = files.Count;
                for (int i = 0; i < count; i++)
                {
                    if (System.IO.File.Exists(files[i]))
                    {
                        System.IO.File.Copy(files[i], Path.Combine(temp, Path.GetFileName(files[i])));
                    }
                }
            }
            // create a new archive
            ZipFile.CreateFromDirectory(temp, archive);

            return File(archive, "application/zip", DateTime.Now.ToString("dd-MM-yyyy hh:mm ss") + " - HW - " + vmHardwareDetailTest.HwQcAssignCustomMasterModel.ProjectName + ".zip");
        }

        [Authorize(Roles = "HWHEAD,HW,MM,PMHEAD,PM,SA,PS")]
        public ActionResult HwDisplayScreeningTest(VmHardwareDetailTest vmHardwareDetailTest, long projectId = 0, long hwQcAssignId = 0, long hwQcInchargeAssignId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            vmHardwareDetailTest.HwQcAssignCustomMasterModel.HwQcInchargeAssignId = hwQcInchargeAssignId;
            ViewBag.HwProjectMasterCustomInfo = _repository.GetProjectAndAssignDetailByHwQcInchargeAssignId(hwQcInchargeAssignId);
            ViewBag.HwProjectMasterCustomInfo.ProjectName = ViewBag.HwProjectMasterCustomInfo.ProjectName + " (" + CommonConversion.AddOrdinal(ViewBag.HwProjectMasterCustomInfo.OrderNuber) + " order)";
            //var vmHardwareDetailTest = new VmHardwareDetailTest();
            vmHardwareDetailTest.HwTestPcbModel = _repository.GetHwTestPcb(hwQcInchargeAssignId);
            vmHardwareDetailTest.HwTestPcbAModel = _repository.GetHwTestPcbA(hwQcInchargeAssignId);
            vmHardwareDetailTest.HwTestCameraInfoModel = _repository.GetHwTestCameraInfo(hwQcInchargeAssignId);
            vmHardwareDetailTest.HwTestTpLcdInfoModel = _repository.GetHwTestTpLcdInfo(hwQcInchargeAssignId);
            vmHardwareDetailTest.HwTestSoundInfoModel = _repository.GetHwTestSoundInfo(hwQcInchargeAssignId);
            vmHardwareDetailTest.HwTestFPCandSIMSlotInfoModel = _repository.GetHwTestFpCandSimSlotInfo(hwQcInchargeAssignId);
            vmHardwareDetailTest.HwTestChargerInfoModel = _repository.GetHwTestChargerInfo(hwQcInchargeAssignId);
            vmHardwareDetailTest.HwTestUSBCableInfoModel = _repository.GetHwTestUSBCableInfo(hwQcInchargeAssignId);
            vmHardwareDetailTest.HwTestEarphoneInterfaceInfoModel =
                _repository.GetHwTestEarphoneInterfaceInfoModel(hwQcInchargeAssignId);
            vmHardwareDetailTest.HwTestChargingInfoModel = _repository.GetHwTestChargingInfoModel(hwQcInchargeAssignId);
            vmHardwareDetailTest.HwTestHousingInfoModel = _repository.GetHwTestHousingInfoModel(hwQcInchargeAssignId);
            vmHardwareDetailTest.HwTestCrossMatchInfoModel = _repository.GetHwTestCrossMatchInfoModel(hwQcInchargeAssignId);
            vmHardwareDetailTest.HwTestOverallResultModel = _repository.GetHwTestOverallResultModel(hwQcInchargeAssignId);
            vmHardwareDetailTest.HwTestBatteryInfoModel = _repository.GetHwTestBatteryInfo(hwQcInchargeAssignId);
            //ViewBag.ProjectMaster = _repository.GetProjectsAssignedToHwQcForScreening(userId);
            //if (projectId <= 0)
            //{
            //    return View(vmhardwaretest);
            //}
            //vmhardwaretest.ProjectMasterModel = _repository.GetProjectInfoByProjectId(projectId) ?? new ProjectMasterModel();
            return View(vmHardwareDetailTest);
        }

        public ActionResult HwDisplayFgReport(long hwQcInchargeAssignId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            @ViewBag.hwqcinchargeassignId = hwQcInchargeAssignId;
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.FgReportInitialInfo = _repository.GetReportInitialInfo(hwQcInchargeAssignId);
            ViewBag.HwTestedBy = _repository.GetHwTestedBy(hwQcInchargeAssignId);
            ViewBag.HwTestCheckedBy = _repository.GetHwTestCheckedBy(hwQcInchargeAssignId);
            return View();
        }


        //Finished goods test
        [Authorize(Roles = "HWHEAD,SA,PS")]
        [HttpGet]
        public ActionResult HwFinishedGoodsTestQcAssign(long projectId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            VmHardwareTest vmhardwaretest = new VmHardwareTest();
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            vmhardwaretest.HwQcInchargeAssignModel.IsFinishedGoodTest = true;
            ViewBag.ProjectMaster = _repository.GetProjectsAssignedToHwQcInchargeForFinishedGoods(); //the value will be the user ID of the Qc Incharge who will log in
            foreach (var i in ViewBag.ProjectMaster)
            {
                i.ProjectName = i.ProjectName + " (" + CommonConversion.AddOrdinal((int)i.OrderNuber) + ")";
            }
            var qcEngineers = _repository.GetUsersForHwQcAssign();

            vmhardwaretest.ProjectMasterModel = _repository.GetProjectInfoByProjectId(projectId) ?? new ProjectMasterModel();
            vmhardwaretest.HwQcAssignModel = _repository.GetHwQcInchargeAssignIdForFinishedGoods(projectId) ?? new HwQcAssignModel();
            vmhardwaretest.HwGetQcAssignedByInchargeModel = _repository.GetQcAssignedByInchargeAssignIdForFinishedGoods(vmhardwaretest.HwQcAssignModel.HwQcInchargeAssignId, 1);
            List<CmnUserModel> cmnUserModels = qcEngineers.Where(cmnUserModel => !vmhardwaretest.HwGetQcAssignedByInchargeModel.Exists(i => i.CmnUserId.Equals(cmnUserModel.CmnUserId))).ToList();
            ViewBag.CmnUser = cmnUserModels;
            return View(vmhardwaretest);
        }

        public ActionResult HwFgTestNavigation(string projectName, long hwQcAssignId = 0, long hwQcInchargeAssignId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(hwQcInchargeAssignId);
            ViewBag.HwQcAssignId = hwQcAssignId;
            ViewBag.HwQcInchargeAssignId = hwQcInchargeAssignId;
            ViewBag.ProjectName = projectName;
            ViewBag.HwQcInchargeAssign = _repository.GetHwQcInchargeAssignByAssignId(hwQcInchargeAssignId);
            return View();
        }


        public ActionResult HwFgBatteryTest(long hwQcAssignId = 0, long hwQcInchargeAssignId = 0, long flag = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            VmHardwareDetailTest vmHardwareDetailTest = new VmHardwareDetailTest();
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(hwQcInchargeAssignId);
            vmHardwareDetailTest.HwFgBatteryTestMasterModel = _repository.GetHwFgBatteryTestMasterModel(hwQcInchargeAssignId);
            vmHardwareDetailTest.BatteryTestResultSummaryModel = _repository.GetBatteryTestResultSummaryModel(hwQcInchargeAssignId);

            if (vmHardwareDetailTest.HwFgBatteryTestMasterModel != null)
            {
                ViewBag.hwFgBatteryTestMasterId = vmHardwareDetailTest.HwFgBatteryTestMasterModel.HwFgBatteryTestMasterId;
                ViewBag.HwFgBatteryTestConditionList =
                    _repository.GetHwFgBatteryTestConditionModelList(
                        vmHardwareDetailTest.HwFgBatteryTestMasterModel.HwFgBatteryTestMasterId);
                //vmHardwareDetailTest.HwFgBatteryTestConditionModel =
                //_repository.GetHwFgBatteryTestConditionModel(
                //    vmHardwareDetailTest.HwFgBatteryTestMasterModel.HwFgBatteryTestMasterId);
            }
            if (vmHardwareDetailTest.HwFgBatteryTestConditionModel != null)
            {
                ViewBag.HwFgBatteryTestConditionId =
                    vmHardwareDetailTest.HwFgBatteryTestConditionModel.HwFgBatteryTestConditionId;
            }
            ViewBag.HwQcAssignId = hwQcAssignId;
            ViewBag.HwQcInchargeAssignId = hwQcInchargeAssignId;
            ViewBag.FgFlag = flag;
            return View(vmHardwareDetailTest);
        }


        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public ActionResult HwFgBatteryTest(VmHardwareDetailTest vmHardwareDetailTest)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.HwTestCustomModel = _repository.CheckDuplicateHwTest(vmHardwareDetailTest.HwFgBatteryTestMasterModel.HwQcInchargeAssignId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwFgBatteryTestMasterModel.HwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwFgBatteryTestMasterModel.HwFgBatteryTestMasterId == 0)
            {
                vmHardwareDetailTest.HwFgBatteryTestMasterModel.AddedBy = userId;//????????CORRECTION:-AddedBy should be long 
                vmHardwareDetailTest.HwFgBatteryTestMasterModel.AddedDate = DateTime.Now;
                _repository.SaveHwFgBatteryTestMaster(vmHardwareDetailTest.HwFgBatteryTestMasterModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Saved Battery test info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                TempData["message"] = "Initial battery info saved";
            }
            else
            {
                vmHardwareDetailTest.HwFgBatteryTestMasterModel.UpdatedBy = userId;//????????CORRECTION:-AddedBy should be long 
                vmHardwareDetailTest.HwFgBatteryTestMasterModel.UpdatedDate = DateTime.Now;
                _repository.UpdateHwFgBatteryTestMaster(vmHardwareDetailTest.HwFgBatteryTestMasterModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Updated Battery test info";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                TempData["message"] = "Initial battery info updated";
            }



            return RedirectToAction("HwFgBatteryTest", new
            {
                hwQcAssignId = vmHardwareDetailTest.HwFgBatteryTestMasterModel.HwQcAssignId,
                hwQcInchargeAssignId = vmHardwareDetailTest.HwFgBatteryTestMasterModel.HwQcInchargeAssignId,
                flag = vmHardwareDetailTest.HwQcAssignCustomMasterModel.Flag
            });
        }


        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        public ActionResult BatteryTestResultSummary(VmHardwareDetailTest vmHardwareDetailTest)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.BatteryTestResultSummaryModel.HwQcInchargeAssignId);
            if (vmHardwareDetailTest.BatteryTestResultSummaryModel.BatteryTestResultSummaryId == 0)
            {
                vmHardwareDetailTest.BatteryTestResultSummaryModel.AddedBy = userId;
                vmHardwareDetailTest.BatteryTestResultSummaryModel.AddedDate = DateTime.Now;
                _repository.SaveBatteryTestResultSummary(vmHardwareDetailTest.BatteryTestResultSummaryModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Saved Battery test result";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                TempData["message"] = "Battery test result summary info saved";
            }
            else
            {
                vmHardwareDetailTest.BatteryTestResultSummaryModel.UpdatedBy = userId;
                vmHardwareDetailTest.BatteryTestResultSummaryModel.UpdatedDate = DateTime.Now;
                _repository.UpdateBatteryTestResultSummary(vmHardwareDetailTest.BatteryTestResultSummaryModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Updated Battery test result";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                TempData["message"] = "Battery test result summary info updated";
            }
            return RedirectToAction("HwFgBatteryTest", new
            {
                hwQcAssignId = vmHardwareDetailTest.HwFgBatteryTestMasterModel.HwQcAssignId,
                hwQcInchargeAssignId = vmHardwareDetailTest.BatteryTestResultSummaryModel.HwQcInchargeAssignId,
                flag = vmHardwareDetailTest.HwQcAssignCustomMasterModel.Flag
            });
        }

        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public ActionResult PostHwFgTestCondition(VmHardwareDetailTest vmHardwareDetailTest)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwFgBatteryTestMasterModel.HwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwFgBatteryTestConditionModel.HwFgBatteryTestConditionId == 0)
            {
                vmHardwareDetailTest.HwFgBatteryTestConditionModel.AddedBy = userId;
                vmHardwareDetailTest.HwFgBatteryTestConditionModel.AddedDate = DateTime.Now;
                _repository.SaveHwFgBatteryTestCondition(vmHardwareDetailTest.HwFgBatteryTestConditionModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Saved Battery test condition";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                TempData["message"] = "Battery test condition saved";
            }
            else
            {
                vmHardwareDetailTest.HwFgBatteryTestConditionModel.UpdatedBy = userId;
                vmHardwareDetailTest.HwFgBatteryTestConditionModel.UpdatedDate = DateTime.Now;
                _repository.UpdateHwFgBatteryTestCondition(vmHardwareDetailTest.HwFgBatteryTestConditionModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Updated Battery test condition";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                TempData["message"] = "Battery test condition updated";
            }
            return RedirectToAction("HwFgBatteryTest", new
            {
                hwQcAssignId = vmHardwareDetailTest.HwFgBatteryTestMasterModel.HwQcAssignId,
                hwQcInchargeAssignId = vmHardwareDetailTest.HwFgBatteryTestMasterModel.HwQcInchargeAssignId,
                flag = vmHardwareDetailTest.HwQcAssignCustomMasterModel.Flag
            });
        }

        public ActionResult DeleteHwFgTestCondition(long hwfgbatterytestconditionId = 0, long hwQcAssignId = 0)
        {
            _repository.DeleteHwFgBatteryTestCondition(hwfgbatterytestconditionId);
            return RedirectToAction("HwFgBatteryTest", new { hwQcAssignId });
        }

        public ActionResult HwFgBatteryTestResults(string testCondition, long hwfgbatterytestconditionId = 0, long hwQcInchargeAssignId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.HwFgTestConditionId = hwfgbatterytestconditionId;
            ViewBag.TestCondition = testCondition;
            ViewBag.hwQcInchargeAssignId = hwQcInchargeAssignId;
            //VmHardwareDetailTest vmHardwareDetailTest=new VmHardwareDetailTest();
            ViewBag.HwFgBatteryTestResultModel =
                _repository.GetHwFgBatteryTestResultModelList(hwfgbatterytestconditionId);
            return View();
        }


        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public ActionResult HwFgBatteryTestResults(VmHardwareDetailTest vmHardwareDetailTest)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwFgBatteryTestMasterModel.HwQcInchargeAssignId);
            vmHardwareDetailTest.HwFgBatteryTestResultModel.AddedBy = userId;
            vmHardwareDetailTest.HwFgBatteryTestResultModel.AddedDate = DateTime.Now;
            _repository.SaveHwFgBatteryTestResult(vmHardwareDetailTest.HwFgBatteryTestResultModel);
            var notificationObject = new NotificationObject
            {
                ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                ToUser = "-1",
            };
            notificationObject.Message = " has Saved Battery test(cycle) result";
            notificationObject.AdditionalMessage = "Cycle no : " + vmHardwareDetailTest.HwFgBatteryTestResultModel.CycleNo + ","
                                                   + "Item NO :  " + vmHardwareDetailTest.HwFgBatteryTestResultModel.ItemNo + ","
                                                   + "Item Name : " + vmHardwareDetailTest.HwFgBatteryTestResultModel.ItemName;
            ViewBag.ControllerVariable = notificationObject;
            TempData["message"] = "Battery test result saved successfully";
            return RedirectToAction("HwFgBatteryTestResults", new { hwfgbatterytestconditionId = vmHardwareDetailTest.HwFgBatteryTestResultModel.HwFgBatteryTestConditionId, testCondition = vmHardwareDetailTest.HwFgBatteryTestConditionModel.TestCondition, hwQcInchargeAssignId = vmHardwareDetailTest.HwFgBatteryTestMasterModel.HwQcInchargeAssignId });
        }


        public ActionResult HwFgChargerTest(long hwQcAssignId = 0, long hwQcInchargeAssignId = 0, long flag = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.HwQcAssignId = hwQcAssignId;
            ViewBag.HwQcInchargeAssignId = hwQcInchargeAssignId;
            VmHardwareDetailTest vmHardwareDetailTest = new VmHardwareDetailTest();
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(hwQcInchargeAssignId);
            vmHardwareDetailTest.HwFgChargerTestModel = _repository.GetHwFgChargerTestModel(hwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwFgChargerTestModel != null)
            {
                ViewBag.HwFgChargerTestId = vmHardwareDetailTest.HwFgChargerTestModel.HwFgChargerTestId;
                ViewBag.HwFgChargerDetail =
                    _repository.GetHwFgChargerDetailModel(vmHardwareDetailTest.HwFgChargerTestModel.HwFgChargerTestId);
            }
            ViewBag.FgFlag = flag;
            return View(vmHardwareDetailTest);
        }


        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public ActionResult HwFgChargerTest(VmHardwareDetailTest vmHardwareDetailTest)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwFgChargerTestModel.HwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwFgChargerTestModel.HwFgChargerTestId == 0)
            {
                vmHardwareDetailTest.HwFgChargerTestModel.AddedBy = userId;
                vmHardwareDetailTest.HwFgChargerTestModel.AddedDate = DateTime.Now;
                _repository.SaveHwFgChargerTest(vmHardwareDetailTest.HwFgChargerTestModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Saved Charger test info";
                notificationObject.AdditionalMessage = "Input Spec-" + vmHardwareDetailTest.HwFgChargerTestModel.InputSpec + "," + "Output Spec-" + vmHardwareDetailTest.HwFgChargerTestModel.OutputSpec;
                ViewBag.ControllerVariable = notificationObject;
                TempData["message"] = "Initial charger test info saved successfully";
            }
            else
            {
                vmHardwareDetailTest.HwFgChargerTestModel.UpdatedBy = userId;
                vmHardwareDetailTest.HwFgChargerTestModel.UpdatedDate = DateTime.Now;
                _repository.UpdateHwFgChargerTest(vmHardwareDetailTest.HwFgChargerTestModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Updated Charger test info";
                notificationObject.AdditionalMessage = "Input Spec-" + vmHardwareDetailTest.HwFgChargerTestModel.InputSpec + "," + "Output Spec-" + vmHardwareDetailTest.HwFgChargerTestModel.OutputSpec;
                ViewBag.ControllerVariable = notificationObject;
                TempData["message"] = "Initial charger test info updated successfully";
            }
            return RedirectToAction("HwFgChargerTest", new
            {
                hwQcAssignId = vmHardwareDetailTest.HwFgChargerTestModel.HwQcAssignId,
                hwQcInchargeAssignId = vmHardwareDetailTest.HwFgChargerTestModel.HwQcInchargeAssignId,
                flag = vmHardwareDetailTest.HwQcAssignCustomMasterModel.Flag
            });
        }


        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public ActionResult PostHwFgChargerDetailTest(VmHardwareDetailTest vmHardwareDetailTest)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            vmHardwareDetailTest.HwFgChargerDetailModel.AddedBy = userId;
            vmHardwareDetailTest.HwFgChargerDetailModel.AddedDate = DateTime.Now;
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwFgChargerTestModel.HwQcInchargeAssignId);
            _repository.SaveHwFgChargerDetailTest(vmHardwareDetailTest.HwFgChargerDetailModel);
            var notificationObject = new NotificationObject
            {
                ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                ToUser = "-1",
            };
            notificationObject.Message = " has Saved Charger detail test";
            notificationObject.AdditionalMessage = "";
            ViewBag.ControllerVariable = notificationObject;

            TempData["message"] = "Charger detail test info saved successfully";
            return RedirectToAction("HwFgChargerTest", new { hwQcAssignId = vmHardwareDetailTest.HwFgChargerTestModel.HwQcAssignId, hwQcInchargeAssignId = vmHardwareDetailTest.HwFgChargerTestModel.HwQcInchargeAssignId });
        }

        public ActionResult HwFgUsbCableTest(long hwQcAssignId = 0, long hwQcInchargeAssignId = 0, long flag = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.HwQcAssignId = hwQcAssignId;
            ViewBag.HwQcInchargeAssignId = hwQcInchargeAssignId;
            ViewBag.AssignedByName = _repository.GetUserInfoByHwQcInchargeAssignedBy(hwQcAssignId);
            VmHardwareDetailTest vmHardwareDetailTest = new VmHardwareDetailTest();
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(hwQcInchargeAssignId);
            vmHardwareDetailTest.HwFgUsbCableTestModel = _repository.GetHwFgUsbCableTestModel(hwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwFgUsbCableTestModel != null)
            {
                ViewBag.GetHwFgUsbTestDetailModelList =
                    _repository.GetHwFgUsbTestDetailModelList(
                        vmHardwareDetailTest.HwFgUsbCableTestModel.HwFgUsbCableTestId);
            }
            ViewBag.FgFlag = flag;
            return View(vmHardwareDetailTest);
        }

        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public ActionResult HwFgUsbCableTest(VmHardwareDetailTest vmHardwareDetailTest)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwFgUsbCableTestModel.HwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwFgUsbCableTestModel.HwFgUsbCableTestId == 0)
            {
                vmHardwareDetailTest.HwFgUsbCableTestModel.AddedBy = userId;
                vmHardwareDetailTest.HwFgUsbCableTestModel.AddedDate = DateTime.Now;
                _repository.SaveHwFgUsbCableTest(vmHardwareDetailTest.HwFgUsbCableTestModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Saved USB Cable test";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                TempData["message"] = "USB cable test topics saved successfully";
            }
            else
            {
                vmHardwareDetailTest.HwFgUsbCableTestModel.UpdatedBy = userId;
                vmHardwareDetailTest.HwFgUsbCableTestModel.UpdatedDate = DateTime.Now;
                _repository.UpdateHwFgUsbCableTest(vmHardwareDetailTest.HwFgUsbCableTestModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Updated USB Cable test";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                TempData["message"] = "USB cable test topics updated successfully";
            }

            return RedirectToAction("HwFgUsbCableTest", new { hwQcAssignId = vmHardwareDetailTest.HwFgUsbCableTestModel.HwQcAssignId, hwQcInchargeAssignId = vmHardwareDetailTest.HwFgUsbCableTestModel.HwQcInchargeAssignId });
        }

        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public ActionResult PostHwFgUsbCableDetail(VmHardwareDetailTest vmHardwareDetailTest)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(vmHardwareDetailTest.HwFgUsbCableTestModel.HwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwFgUsbCableTestModel.HwFgUsbCableTestId != 0)
            {
                vmHardwareDetailTest.HwFgUsbCableTestModel.AddedBy = userId;
                vmHardwareDetailTest.HwFgUsbCableTestModel.AddedDate = DateTime.Now;
                vmHardwareDetailTest.HwFgUsbTestDetailModel.AddedBy = userId;
                vmHardwareDetailTest.HwFgUsbTestDetailModel.AddedDate = DateTime.Now;
                vmHardwareDetailTest.HwFgUsbTestDetailModel.HwFgUsbCableTestId =
                    vmHardwareDetailTest.HwFgUsbCableTestModel.HwFgUsbCableTestId;
                _repository.SaveHwFgUsbCableDetail(vmHardwareDetailTest.HwFgUsbTestDetailModel);
                _repository.SaveHwFgUsbCableTest(vmHardwareDetailTest.HwFgUsbCableTestModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Saved USB Cable Detail test";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                TempData["message"] = "USB cable test topics saved successfully";
            }
            else
            {
                vmHardwareDetailTest.HwFgUsbCableTestModel.AddedBy = userId;
                vmHardwareDetailTest.HwFgUsbCableTestModel.AddedDate = DateTime.Now;
                vmHardwareDetailTest.HwFgUsbTestDetailModel.AddedBy = userId;
                vmHardwareDetailTest.HwFgUsbTestDetailModel.AddedDate = DateTime.Now;
                _repository.SaveHwFgUsbCableTest(vmHardwareDetailTest.HwFgUsbCableTestModel);
                vmHardwareDetailTest.HwFgUsbCableTestModel = _repository.GetHwFgUsbCableTestModel(vmHardwareDetailTest.HwFgUsbCableTestModel.HwQcInchargeAssignId);
                vmHardwareDetailTest.HwFgUsbTestDetailModel.HwFgUsbCableTestId =
                    vmHardwareDetailTest.HwFgUsbCableTestModel.HwFgUsbCableTestId;
                _repository.SaveHwFgUsbCableDetail(vmHardwareDetailTest.HwFgUsbTestDetailModel);
                _repository.SaveHwFgUsbCableTest(vmHardwareDetailTest.HwFgUsbCableTestModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };
                notificationObject.Message = " has Updated USB Cable Detail test";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
                TempData["message"] = "USB cable test topics saved successfully";
            }
            return RedirectToAction("HwFgUsbCableTest", new { hwQcAssignId = vmHardwareDetailTest.HwFgUsbCableTestModel.HwQcAssignId, hwQcInchargeAssignId = vmHardwareDetailTest.HwFgUsbCableTestModel.HwQcInchargeAssignId });
        }
        //Get or Load Project Basic info
        //[HttpPost]
        //public JsonResult GetProjectBasics(string projectmasterid)
        //{

        //    ProjectMasterModel projectinfo = null;

        //    var model = new VmHardwareTest();
        //    ViewBag.ProjectMasterModel = _repository.GetAllProjects();

        //    return Json(projectinfo);
        //}



        //[ChildActionOnly]
        //public ActionResult _HwItemization(long projectmasterid)
        //{
        //    //HwCmQcStartProjectViewModel itemization = null;
        //    var model = new HwCmQcStartProjectViewModel();
        //    model.HwItemizationItemName = _repository.GetHwItemaizationItemNameList();
        //    model.HwItemizations = _repository.GetAllHwItemizations(projectmasterid);
        //    return PartialView(model);
        //}

        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        public JsonResult PostChipsetIc(string chipsetVendor, string icNoSize, string chipsetCore, string chipsetSpeed, string pinType, string pinNumber, string newitemno, string itemcode, string remarks, string hwqcassignId)
        {
            var chipset = new HwChipsetModel();
            long userId = Convert.ToInt64(User.Identity.Name);
            int pinNo, hwQcAssignId;
            int.TryParse(pinNumber, out pinNo);
            int.TryParse(hwqcassignId, out hwQcAssignId);
            VmHardwareDetailTest vmHardwareDetailTest = new VmHardwareDetailTest();
            var duplicateCheck = db.HwChipsets.FirstOrDefault(model => model.IcNoSize.Equals(icNoSize, StringComparison.CurrentCultureIgnoreCase));
            //chipset = data != null ? _repository.UpdateHwChipset(data.ChipsetId, chipsetVendor, icNoSize, chipsetCore, chipsetSpeed, pinType, pinNo, remarks, userId) : _repository.SaveHwChipsetIc(chipsetVendor, icNoSize, chipsetCore, chipsetSpeed, pinType, pinNo, remarks, userId);
            chipset = duplicateCheck != null ? null : _repository.SaveHwChipsetIc(chipsetVendor, icNoSize, chipsetCore, chipsetSpeed, pinType, pinNo, newitemno, itemcode, remarks, userId);

            ViewBag.HwChipsetModel = _repository.GetAllHwChipsetModel();
            var json = new JavaScriptSerializer().Serialize(chipset);
            //---------------------------------------------------

            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcAssignId(hwQcAssignId);
            var notificationObject = new NotificationObject
            {
                ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                ToUser = "-1",
            };
            if (duplicateCheck == null)
            {
                notificationObject.Message = " has added a new chipset : " + icNoSize + " in database for the";
                notificationObject.AdditionalMessage = "Vendor: " + chipsetVendor + ", Core: " + chipsetCore + ", Clock Speed: " + chipsetSpeed;
                ViewBag.ControllerVariable = notificationObject;
            }
            //--------------------------------------------------
            return Json(json, JsonRequestBehavior.AllowGet);
        }


        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        public JsonResult PostFlashIc(string flashIcBall, string flashIcRam, string flashIcRom, string flashIcTechnology, string flashIcVendor, string icNoSize, string pinNumber, string pinType, string remarks, string hwqcassignId)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            int pinNo, hwQcAssignId;
            int.TryParse(pinNumber, out pinNo);
            int.TryParse(hwqcassignId, out hwQcAssignId);
            VmHardwareDetailTest vmHardwareDetailTest = new VmHardwareDetailTest();
            var data = db.HwFlashIcs.FirstOrDefault(model => model.IcNoSize.Equals(icNoSize, StringComparison.CurrentCultureIgnoreCase));
            //var flashIc= data != null?_repository.UpdateHwFlashIcModel(data.FlashIcId,flashIcBall,flashIcRam,flashIcRom,flashIcTechnology,flashIcVendor,icNoSize,pinNo,pinType,remarks,userId) :  _repository.SaveHwFlashIcModel(flashIcBall, flashIcRam, flashIcRom, flashIcTechnology, flashIcVendor, icNoSize, pinNo, pinType, remarks, userId);
            var flashIc = data != null ? null : _repository.SaveHwFlashIcModel(flashIcBall, flashIcRam, flashIcRom, flashIcTechnology, flashIcVendor, icNoSize, pinNo, pinType, remarks, userId);
            var json = new JavaScriptSerializer().Serialize(flashIc);
            //---------------------------------------------------
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcAssignId(hwQcAssignId);
            var notificationObject = new NotificationObject
            {
                ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                ToUser = "-1",
            };
            if (data == null)
            {
                notificationObject.Message = " has added a new Flash IC : " + icNoSize + " in database for the";
                notificationObject.AdditionalMessage = "Vendor: " + flashIcVendor + ", RAM: " + flashIcRam + ", ROM: " + flashIcRom;
                ViewBag.ControllerVariable = notificationObject;
            }
            //--------------------------------------------------
            return Json(json, JsonRequestBehavior.AllowGet);
        }


        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        public JsonResult PostRfIc(string icNoSize, string rfVendor, string pinNumber, string pinType, string remarks, string hwqcassignId)
        {
            var rfIC = new HwRfModel();
            long userId = Convert.ToInt64(User.Identity.Name);
            int pinNo, hwQcAssignId;
            int.TryParse(pinNumber, out pinNo);
            int.TryParse(hwqcassignId, out hwQcAssignId);
            VmHardwareDetailTest vmHardwareDetailTest = new VmHardwareDetailTest();
            var data = db.HwRfs.FirstOrDefault(model => model.IcNoSize.Equals(icNoSize, StringComparison.CurrentCultureIgnoreCase));
            rfIC = data != null ? null : _repository.SaveHwRfModel(icNoSize, rfVendor, pinNo, pinType, remarks, userId);
            var json = new JavaScriptSerializer().Serialize(rfIC);
            //---------------------------------------------------
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcAssignId(hwQcAssignId);
            var notificationObject = new NotificationObject
            {
                ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                ToUser = "-1",
            };
            if (data == null)
            {
                notificationObject.Message = " has added a new RF IC : " + icNoSize + " in database for the";
                notificationObject.AdditionalMessage = "Vendor: " + rfVendor;
                ViewBag.ControllerVariable = notificationObject;
            }
            //--------------------------------------------------
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        public JsonResult PostPmu1Ic(string icNoSize, string Pmu_1_Vendor, string pinNumber, string pinType, string newitemno, string itemcode, string remarks, string hwqcassignId)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            int pinNo, hwQcAssignId;
            int.TryParse(pinNumber, out pinNo);
            int.TryParse(hwqcassignId, out hwQcAssignId);
            VmHardwareDetailTest vmHardwareDetailTest = new VmHardwareDetailTest();
            var data = db.HwPmu1s.FirstOrDefault(model => model.IcNoSize.Equals(icNoSize, StringComparison.CurrentCultureIgnoreCase));
            var pmu1IC = data != null ? null : _repository.SaveHwPmu1IcModel(icNoSize, Pmu_1_Vendor, pinNo, pinType, newitemno, itemcode, remarks, userId);
            var json = new JavaScriptSerializer().Serialize(pmu1IC);
            //---------------------------------------------------
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcAssignId(hwQcAssignId);
            var notificationObject = new NotificationObject
            {
                ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                ToUser = "-1",
            };
            if (data == null)
            {
                notificationObject.Message = " has added a new PMU1 IC : " + icNoSize + " in database for the";
                notificationObject.AdditionalMessage = "Vendor: " + Pmu_1_Vendor;
                ViewBag.ControllerVariable = notificationObject;
            }

            //--------------------------------------------------
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        public JsonResult PostHwFrontCameraIc(string icNoSize, string vendor, string pinNumber, string pinType, string remarks, string hwqcassignId)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            int pinNo, hwQcAssignId;
            int.TryParse(pinNumber, out pinNo);
            int.TryParse(hwqcassignId, out hwQcAssignId);
            VmHardwareDetailTest vmHardwareDetailTest = new VmHardwareDetailTest();
            var data = db.HwFrontCameraIcs.FirstOrDefault(model => model.IcNoSize.Equals(icNoSize, StringComparison.CurrentCultureIgnoreCase));
            var frontCameraIc = data != null ? null : _repository.SaveFrontCameraIcModel(icNoSize, vendor, pinNo, pinType, remarks, userId);
            var json = new JavaScriptSerializer().Serialize(frontCameraIc);
            //---------------------------------------------------

            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcAssignId(hwQcAssignId);
            var notificationObject = new NotificationObject
            {
                ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                ToUser = "-1",
            };
            if (data == null)
            {
                notificationObject.Message = " has added a new Front Camera IC : " + icNoSize + " in database for the";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
            }

            //--------------------------------------------------
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        public JsonResult PostHwBackCameraIc(string icNoSize, string vendor, string pinNumber, string pinType, string remarks, string hwqcassignId)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            int pinNo, hwQcAssignId;
            int.TryParse(pinNumber, out pinNo);
            int.TryParse(hwqcassignId, out hwQcAssignId);
            VmHardwareDetailTest vmHardwareDetailTest = new VmHardwareDetailTest();
            var data = db.HwBackCameraIcs.FirstOrDefault(model => model.IcNoSize.Equals(icNoSize, StringComparison.CurrentCultureIgnoreCase));
            var backCameraIc = data != null ? null : _repository.SaveBackCameraIcModel(icNoSize, vendor, pinNo, pinType, remarks, userId);
            var json = new JavaScriptSerializer().Serialize(backCameraIc);
            //---------------------------------------------------

            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcAssignId(hwQcAssignId);
            var notificationObject = new NotificationObject
            {
                ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                ToUser = "-1",
            };
            if (data == null)
            {
                notificationObject.Message = " has added a new Back Camera IC : " + icNoSize + " in database for the";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
            }

            //--------------------------------------------------
            return Json(json, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ChipsetDuplicationCheckFor(HwChipsetModel hwChipsetModel)
        {

            var data =
                db.HwChipsets.FirstOrDefault(model => model.IcNoSize.Equals(hwChipsetModel.IcNoSize, StringComparison.CurrentCultureIgnoreCase));
            if (data != null)
            {
                return Json("already exists", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult HwItemization(long hwQcInchargeAssignId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            var vmHardwareDetailTest = new VmHardwareDetailTest();

            vmHardwareDetailTest.GetHwItemizationModels = _repository.GetHwItemizationModels(hwQcInchargeAssignId);

            //vmHardwareDetailTest.HwTestPcbAModel = _repository.GetHwTestPcbA(hwQcInchargeAssignId);
            //vmHardwareDetailTest.HwTestCameraInfoModel = _repository.GetHwTestCameraInfo(hwQcInchargeAssignId);
            //vmHardwareDetailTest.HwTestTpLcdInfoModel = _repository.GetHwTestTpLcdInfo(hwQcInchargeAssignId);
            //vmHardwareDetailTest.HwTestSoundInfoModel = _repository.GetHwTestSoundInfo(hwQcInchargeAssignId);
            //vmHardwareDetailTest.HwTestHousingInfoModel = _repository.GetHwTestHousingInfoModel(hwQcInchargeAssignId);
            //vmHardwareDetailTest.HwTestBatteryInfoModel = _repository.GetHwTestBatteryInfo(hwQcInchargeAssignId);
            //ViewBag.ProForChipset = vmHardwareDetailTest.HwTestPcbAModel == null ? (dynamic)null : _repository.GetProjectListByItemNameForChipset(vmHardwareDetailTest.HwTestPcbAModel.IcNoSize, hwQcInchargeAssignId);
            //ViewBag.ProForFlashIc = vmHardwareDetailTest.HwTestPcbAModel == null ? (dynamic)null : _repository.GetProjectListByItemNameForFlashIc(vmHardwareDetailTest.HwTestPcbAModel.Flash_IcNoSize, hwQcInchargeAssignId);
            //ViewBag.ProForPmu1Ic = vmHardwareDetailTest.HwTestPcbAModel == null ? (dynamic)null : _repository.GetProjectListByItemNameForPmu1Ic(vmHardwareDetailTest.HwTestPcbAModel.PMU1IC, hwQcInchargeAssignId);
            //ViewBag.ProForRfIc = vmHardwareDetailTest.HwTestPcbAModel == null ? (dynamic)null : _repository.GetProjectListByItemNameForRfIc(vmHardwareDetailTest.HwTestPcbAModel.RFIC, hwQcInchargeAssignId);
            //ViewBag.ProForBackCamera = vmHardwareDetailTest.HwTestCameraInfoModel == null ? (dynamic)null : _repository.GetProjectListByItemNameForBackCamera(vmHardwareDetailTest.HwTestCameraInfoModel.BackCamera_IcNoSize, hwQcInchargeAssignId);
            //ViewBag.ProForFrontCamera = vmHardwareDetailTest.HwTestCameraInfoModel == null ? (dynamic)null : _repository.GetProjectListByItemNameForFrontCamera(vmHardwareDetailTest.HwTestCameraInfoModel.FrontCamera_IcNoSize, hwQcInchargeAssignId);
            return View(vmHardwareDetailTest);
        }

        public ActionResult HwItemizationForm(long hwQcInchargeAssignId = 0, long HwQcAssignId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            VmHardwareDetailTest vmHardwareDetailTest = new VmHardwareDetailTest();
            vmHardwareDetailTest.ProjectMasterModel =
                _repository.GetProjectInfoByHwQcInchargeAssignId(hwQcInchargeAssignId);
            vmHardwareDetailTest.HwItemizationModel.ProjectMasterId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId;
            vmHardwareDetailTest.HwItemizationModel.HwQcInchargeAssignId = hwQcInchargeAssignId;
            vmHardwareDetailTest.HwItemizationModel.HwQcAssignId = HwQcAssignId;
            vmHardwareDetailTest.HwItemComponentModels = _repository.GetHwItemComponentModels();
            vmHardwareDetailTest.GetHwItemizationModels = _repository.GetHwItemizationModels(hwQcInchargeAssignId);
            ViewBag.AllProjects = _repository.GetAllProjectDistinctName();
            return View(vmHardwareDetailTest);
        }

        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public JsonResult HwItemizationForm(VmHardwareDetailTest vmHardwareDetailTest)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            vmHardwareDetailTest.HwItemizationModel.AddedBy = userId;
            vmHardwareDetailTest.HwItemizationModel.AddedDate = DateTime.Now;
            if (vmHardwareDetailTest.HwItemizationModel.ItemComponentId == 0) return Json(null, JsonRequestBehavior.AllowGet);
            if (vmHardwareDetailTest.HwItemizationModel.YesNot == null) return Json(null, JsonRequestBehavior.AllowGet);
            _repository.SaveHwItemizationModel(vmHardwareDetailTest.HwItemizationModel);
            var notificationObject = new NotificationObject
            {
                ProjectId = vmHardwareDetailTest.HwItemizationModel.ProjectMasterId,
                ToUser = "-1",
            };

            notificationObject.Message = " has saved itemization ";
            notificationObject.AdditionalMessage = "";
            ViewBag.ControllerVariable = notificationObject;

            var item = _repository.GetLatestHwItemizationModel();
            return Json(item, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetHwIcComponentNumberModels(long hwItemComponentId)
        {
            List<HwIcComponentNumberModel> models = _repository.GetHwIcComponentNumberModels(hwItemComponentId);
            return new JsonResult { Data = models, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult PostItemComponentName(string itemComponentName)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            HwItemComponentModel model = new HwItemComponentModel();
            model.ItemComponentName = itemComponentName;
            model.AddedBy = userId;
            model.AddedDate = DateTime.Now;
            _repository.SaveItemComponentModel(model);
            var i = db.HwItemComponents.OrderByDescending(item => item.AddedDate).FirstOrDefault();//refactor it later
            return Json(i, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PostIcComponentNumber(long itemComponentId, string icComponentNumber)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            HwIcComponentNumberModel model = new HwIcComponentNumberModel();
            model.ItemComponentId = itemComponentId;
            model.IcComponentNumber = icComponentNumber;
            model.AddedBy = userId;
            model.AddedDate = DateTime.Now;
            _repository.SaveIcComponentNumberModel(model);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult HwBatteryTestReport(long hwQcInchargeAssignId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            VmHardwareDetailTest vmHardwareDetailTest = new VmHardwareDetailTest();
            vmHardwareDetailTest.HwBatteryTestCustomModel = _repository.GetHwBatteryTestCustomModel(hwQcInchargeAssignId);
            ViewBag.HwFgBatteryTestResultModel =
                _repository.GetHwFgBatteryTestResultByHwQcInchargeAssignId(hwQcInchargeAssignId);
            vmHardwareDetailTest.BatteryTestResultSummaryModel =
                _repository.GetBatteryTestResultSummaryModelByHwQcInchargeId(hwQcInchargeAssignId);
            vmHardwareDetailTest.HwQcAssignCustomMasterModel = _repository.GetHwQcAssignDetailForVerifyByQcAssignId(hwQcInchargeAssignId);
            return View(vmHardwareDetailTest);
        }

        public ActionResult HwChargerTestReport(long hwQcInchargeAssignId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            VmHardwareDetailTest vmHardwareDetailTest = new VmHardwareDetailTest();
            ViewBag.FgReportInitialInfo =
                _repository.GetReportInitialInfo(hwQcInchargeAssignId);
            vmHardwareDetailTest.HwFgChargerTestModel = _repository.GetHwFgChargerTestModel(hwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwFgChargerTestModel != null)
            {
                ViewBag.HwFgChargerTestId = vmHardwareDetailTest.HwFgChargerTestModel.HwFgChargerTestId;
                ViewBag.HwFgChargerDetail =
                    _repository.GetHwFgChargerDetailModel(vmHardwareDetailTest.HwFgChargerTestModel.HwFgChargerTestId);
            }
            return View(vmHardwareDetailTest);
        }

        public ActionResult HwUsbCableTestReport(long hwQcInchargeAssignId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.FgReportInitialInfo = _repository.GetReportInitialInfo(hwQcInchargeAssignId);
            VmHardwareDetailTest vmHardwareDetailTest = new VmHardwareDetailTest();
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(hwQcInchargeAssignId);
            vmHardwareDetailTest.HwFgUsbCableTestModel = _repository.GetHwFgUsbCableTestModel(hwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwFgUsbCableTestModel != null)
            {
                ViewBag.GetHwFgUsbTestDetailModelList =
                    _repository.GetHwFgUsbTestDetailModelList(
                        vmHardwareDetailTest.HwFgUsbCableTestModel.HwFgUsbCableTestId);
            }
            return View(vmHardwareDetailTest);
        }

        public ActionResult HwFieldTest(long hwQcInchargeAssignId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.HwQcInchargeAssignId = hwQcInchargeAssignId;
            VmHardwareDetailTest vmHardwareDetailTest = new VmHardwareDetailTest();
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(hwQcInchargeAssignId);
            vmHardwareDetailTest.HwFieldTestMasterModel = _repository.GetHwFieldTestMasterModel(hwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwFieldTestMasterModel != null)
            {
                vmHardwareDetailTest.HwFieldTestModels =
                _repository.GetAllHwFieldTestModelByFieldTestMasterId(vmHardwareDetailTest.HwFieldTestMasterModel.FieldTestMasterId);
            }
            return View(vmHardwareDetailTest);
        }

        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,MM,PS")]
        [HttpPost]
        public ActionResult HwFieldTest(VmHardwareDetailTest vmHardwareDetailTest)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            if (vmHardwareDetailTest.HwFieldTestMasterModel.FieldTestMasterId == 0)
            {
                vmHardwareDetailTest.HwFieldTestMasterModel.ProjectMasterId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId;
                vmHardwareDetailTest.HwFieldTestMasterModel.Model = vmHardwareDetailTest.ProjectMasterModel.ProjectName;
                vmHardwareDetailTest.HwFieldTestMasterModel.AddedBy = userId;
                vmHardwareDetailTest.HwFieldTestMasterModel.AddedDate = DateTime.Now;
                _repository.SaveHwFieldTestMasterModel(vmHardwareDetailTest.HwFieldTestMasterModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };

                notificationObject.Message = " has saved field test report. ";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;

            }
            else
            {
                vmHardwareDetailTest.HwFieldTestMasterModel.ProjectMasterId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId;
                vmHardwareDetailTest.HwFieldTestMasterModel.Model = vmHardwareDetailTest.ProjectMasterModel.ProjectName;
                vmHardwareDetailTest.HwFieldTestMasterModel.UpdatedBy = userId;
                vmHardwareDetailTest.HwFieldTestMasterModel.UpdatedDate = DateTime.Now;
                _repository.UpdateHwFieldTestMaster(vmHardwareDetailTest.HwFieldTestMasterModel);
                var notificationObject = new NotificationObject
                {
                    ProjectId = vmHardwareDetailTest.ProjectMasterModel.ProjectMasterId,
                    ToUser = "-1",
                };

                notificationObject.Message = " has updated field test report. ";
                notificationObject.AdditionalMessage = "";
                ViewBag.ControllerVariable = notificationObject;
            }
            return RedirectToAction("HwFieldTest", new { hwQcInchargeAssignId = vmHardwareDetailTest.HwFieldTestMasterModel.HwQcInchargeAssignId });
        }

        [HttpPost]
        public JsonResult PostHwFieldTestJsonResult(VmHardwareDetailTest vmHardwareDetailTest)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            vmHardwareDetailTest.HwFieldTestModel.FieldTestMasterId =
                vmHardwareDetailTest.HwFieldTestMasterModel.FieldTestMasterId;
            vmHardwareDetailTest.HwFieldTestModel.AddedBy = userId;
            vmHardwareDetailTest.HwFieldTestModel.AddedDate = DateTime.Now;
            _repository.SaveHwFieldTest(vmHardwareDetailTest.HwFieldTestModel);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult HwFieldTestReport(long hwQcInchargeAssignId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            VmHardwareDetailTest vmHardwareDetailTest = new VmHardwareDetailTest();
            vmHardwareDetailTest.ProjectMasterModel = _repository.GetProjectInfoByHwQcInchargeAssignId(hwQcInchargeAssignId);
            vmHardwareDetailTest.HwFieldTestMasterModel = _repository.GetHwFieldTestMasterModel(hwQcInchargeAssignId);
            if (vmHardwareDetailTest.HwFieldTestMasterModel != null)
            {
                vmHardwareDetailTest.HwFieldTestModels =
                _repository.GetAllHwFieldTestModelByFieldTestMasterId(vmHardwareDetailTest.HwFieldTestMasterModel.FieldTestMasterId);
            }
            return View(vmHardwareDetailTest);
        }

        [Authorize(Roles = "HWHEAD,SA")]
        public ActionResult HwTestEngineerAssign()
        {
            ViewBag.CmnUser = _repository.GetHwEnginnersForAssign();
            ViewBag.AssignedTests = _commonRepository.GetHwEngineerAssignModels();
            var vm = _commonRepository.GetHwTestDetail();
            return View(vm);
        }

        public JsonResult HwEngineerAssignSubmit(string[] engineerIds, string remarks, long hwinchargeassignId = 0, long projectId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            List<long> mailtoList=new List<long>();
            //ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            var strEngineerIds = "";
            var engineerNames = "";
            for (int i = 0; i < engineerIds.Length; i++)
            {
                mailtoList.Add(Convert.ToInt64(engineerIds[i]));
                long engineerId = Convert.ToInt64(engineerIds[i]);
                var userDetail = _repository.GetUserInfoByUserId(engineerId);
                if (engineerIds.Length == 1)
                {
                    strEngineerIds = engineerIds[0];
                    engineerNames = userDetail.UserFullName;
                }
                if (i == 0 && engineerIds.Length > 1)
                {
                    strEngineerIds = engineerIds[0] + ",";
                    engineerNames = userDetail.UserFullName + ",";
                }
                if (i > 0 && i < engineerIds.Length - 1)
                {
                    strEngineerIds = strEngineerIds + engineerIds[i] + ",";
                    engineerNames = engineerNames + userDetail.UserFullName + ",";
                }
                if (i != 0 && i == engineerIds.Length - 1)
                {
                    strEngineerIds = strEngineerIds + engineerIds[i];
                    engineerNames = engineerNames + userDetail.UserFullName;
                }
            }
            var engineerAssign = new HwEngineerAssignModel
            {
                HwTestInchargeAssignId = hwinchargeassignId,
                ProjectMasterId = projectId,
                HwEngineerIds = strEngineerIds,
                HwEngineerNames = engineerNames,
                HwInchargeRemark = remarks,
                AddedBy = userId,
                AddedDate = DateTime.Now,
                Status = "NEW"
            };
            var json = _repository.SaveHwEngineerAssign(engineerAssign);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(mailtoList,
                new List<string>(new[] { "HWHEAD" }), "HW Engineer Assigned for "+json.HwTestName+" test, Model:"+json.ProjectName, "This is to inform you that, you've been assigned for '" + json.HwTestName + " test for project "+json.ProjectName);
            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult HwTest()
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.Tests = _repository.GetHwEngineerAssignModels(userId);
            return View();
        }

        [HttpPost]
        public ActionResult HwTest(HwTestFileUploadModel model)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var manager = new FileManager();
            var moduleDirectory = "HwTest";
            var userDirectory = "HW";
            HttpFileCollectionBase hpf = Request.Files;
            for (int i = 0; i < hpf.Count; i++)
            {
                HttpPostedFileBase file = hpf[i];
                if (file != null && file.ContentLength > 0)
                {
                    //string folderPath = Server.MapPath("~/ServerFolderPath");
                    //Directory.CreateDirectory(folderPath);

                    //string savedFileName = Server.MapPath("~/ServerFolderPath/" + file.FileName);
                    //file.SaveAs(savedFileName);
                    //return Content("File Uploaded Successfully");
                    model.FileUploadPath = manager.IncidentUpload(userDirectory, moduleDirectory, file);
                    model.AddedBy = userId;
                    model.AddedDate = DateTime.Now;
                    _repository.SaveHwTestFileUploadModel(model);
                }
                else
                {
                    return Content("Invalid File");
                }
            }
            return RedirectToAction("HwTest");
        }

        public JsonResult HwTestSubmit(List<HwTestAdditionalInfoModel> additionalInfo,string result,string remark, string projectname, long hwengineerassignId = 0, long hwinchargeassignId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            if (additionalInfo != null)
            {
                foreach (var v in additionalInfo)
                {
                    var model = new HwTestAdditionalInfoModel
                    {
                        HwTestInchargeAssignId = hwinchargeassignId,
                        HwEngineerAssignId = hwengineerassignId,
                        FieldName = v.FieldName,
                        FieldValue = v.FieldValue,
                        AddedBy = userId,
                        AddedDate = DateTime.Now
                    };
                    _repository.SaveHwAdditionalInfo(model);
                }
            }
            
            var json = _repository.SubmitHwTest(hwengineerassignId, hwinchargeassignId, result, remark, userId);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<string>(new[] { "HWHEAD" }),
                new List<string>(new[] { "" }), json.HwTestName + " test submitted by " + json.SubmittedByName + " for project " + projectname, "This is to inform you that, " + json.HwTestName + " test submitted by " + json.SubmittedByName + " for project " + projectname+".");
            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet }; 
        }

        public JsonResult PostHwTest(HttpFileCollectionBase filesBase, string remark, long hwengineerassignId = 0, long hwinchargeassignId = 0, long projectId = 0)
        {
            var manager = new FileManager();
            var moduleDirectory = "HwTest";
            var userDirectory = "HW";
            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                var model = new HwTestFileUploadModel
                {
                    HwEngineerAssignId = hwengineerassignId,
                    HwTestInchargeAssignId = hwinchargeassignId,
                    ProjectMasterId = projectId,
                    FileUploadPath = manager.IncidentUpload(userDirectory, moduleDirectory, file)
                };
            }
            //for (int i = 0; i < fileUpload.Length; i++)
            //{
            //    var model = new HwTestFileUploadModel
            //    {
            //        HwEngineerAssignId = hwengineerassignId,
            //        HwTestInchargeAssignId = hwinchargeassignId,
            //        ProjectMasterId = projectId,
            //        FileUploadPath = manager.IncidentUpload(userDirectory,moduleDirectory,fileUpload[i])
            //    };
            //}
            return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetUploadedFiles(long hwinchargeassignId = 0)
        {
            var json = _repository.GetHwTestFileUploadModels(hwinchargeassignId);
            foreach (var v in json)
            {
                v.FileUploadPath = Path.GetFileNameWithoutExtension(v.FileUploadPath);
            }
            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetFileByHwEngAssignId(long id = 0)
        {
            var json = _repository.GetFileByHwEngAssignId(id);
            foreach (var v in json)
            {
                v.FileUploadPath = Path.GetFileNameWithoutExtension(v.FileUploadPath);
            }
            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetAdditionalInfos(long hwinchargeassignId = 0)
        {
            var json = _repository.GetHwTestAdditionalInfoModels(hwinchargeassignId);
            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult DownloadHwTestFile(long fileuploadId=0)
        {
            var manager = new FileManager();
            var file = _repository.GetHwTestFileUploadModel(fileuploadId);
            var path = manager.GetFile(file.FileUploadPath);
            var apppath = HttpContext.Server.MapPath(path);//HttpRuntime.AppDomainAppPath;
            string fileName = Path.GetFileName(path);
            var extension = Path.GetExtension(fileName);
            if (!string.IsNullOrEmpty(extension))
            {
                string ext = extension.Remove(0, 1);
                return File(apppath, "application/" + ext, fileName);
            }
            return Content("No files found");
        }

        public JsonResult ForwardHwTest(string remarks, long hwinchargeassignId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            _repository.UpdateHwTestInchargeAssign(remarks,hwinchargeassignId,userId);
            return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #region HW SELF TEST

        public ActionResult HwSelfTest()
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.CmnUser = _repository.GetHwEnginnersForAssign();
            ViewBag.Projects = _commonRepository.GetAllProjects();
            ViewBag.hwTestMasters = _commonRepository.GetHwTestMasterModels(ViewBag.UserInfo.RoleName);
            var model = _repository.GetHwSelfTests(userId);
            return View(model);
        }

        public JsonResult SaveHwSelfTest(string engIds, string hwTestName, string remarks, string projectName, long hwTestMasterId = 0, long projectId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var moduleDirectory = "HwSelfTest";
            var userDirectory = "HW";
            var manager = new FileManager();

            //=========================================
            List<long> mailtoList = new List<long>();
            var strEngineerIds = "";
            var engineerNames = "";
            string[] engineerIds=engIds.Split(',');
            for (int i = 0; i < engineerIds.Length; i++)
            {
                mailtoList.Add(Convert.ToInt64(engineerIds[i]));
                long engineerId = Convert.ToInt64(engineerIds[i]);
                var userDetail = _repository.GetUserInfoByUserId(engineerId);
                if (engineerIds.Length == 1)
                {
                    strEngineerIds = engineerIds[0];
                    engineerNames = userDetail.UserFullName;
                }
                if (i == 0 && engineerIds.Length > 1)
                {
                    strEngineerIds = engineerIds[0] + ",";
                    engineerNames = userDetail.UserFullName + ",";
                }
                if (i > 0 && i < engineerIds.Length - 1)
                {
                    strEngineerIds = strEngineerIds + engineerIds[i] + ",";
                    engineerNames = engineerNames + userDetail.UserFullName + ",";
                }
                if (i != 0 && i == engineerIds.Length - 1)
                {
                    strEngineerIds = strEngineerIds + engineerIds[i];
                    engineerNames = engineerNames + userDetail.UserFullName;
                }
            }
            var engineerAssign = new HwEngineerAssignModel
            {
                ProjectMasterId = projectId,
                ProjectName = projectName,
                HwTestMasterId = hwTestMasterId,
                HwTestName = hwTestName,
                HwEngineerIds = strEngineerIds,
                HwEngineerNames = engineerNames,
                HwInchargeRemark = remarks,
                AddedBy = userId,
                AddedDate = DateTime.Now,
                Status = "ASSIGNED FOR SELF TEST"
            };
            var data = _repository.SaveEngineerAssignModelForSelfTest(engineerAssign);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(mailtoList,
                new List<string>(new[] { "HWHEAD" }), "HW Engineer Assigned for " + data.HwTestName + " test (Self Test), Model:" + data.ProjectName, "This is to inform you that, you've been assigned for '" + data.HwTestName + " test' (Self Test) for project " + data.ProjectName);
            //-----------------------------------------
            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                var fileupload = new HwTestFileUploadModel
                {
                    HwEngineerAssignId = data.HwEngineerAssignId,
                    ProjectMasterId = projectId,
                    AddedBy = userId,
                    AddedDate = DateTime.Now,
                    FileUploadPath = manager.IncidentUpload(userDirectory, moduleDirectory, file)
                };
                _repository.SaveHwTestFileUploadModel(fileupload);
            }
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion
    }
}