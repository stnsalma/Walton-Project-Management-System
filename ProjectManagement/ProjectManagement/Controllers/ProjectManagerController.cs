using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Infrastructures.Repositories;
using ProjectManagement.Models;
using ProjectManagement.Models.AssignModels;
using ProjectManagement.ViewModels.ProjectManager;
using System.IO.Compression;

using System.IO;
using ProjectManagement.ViewModels.Software;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace ProjectManagement.Controllers
{
    [Authorize(Roles = "PMHEAD,PM,SA,CM,MM,CPSD,ASPM,ASPMHEAD,CPSDHEAD,ACCNT,INV,INVHEAD")]
    public class ProjectManagerController : Controller
    {
        long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);

        private readonly IProjectManagerRepository _projectManagerRepository;
        private readonly CommercialRepository _commercialRepository;
        private readonly HardwareRepository _hardwareRepository;
        private readonly CommonRepository _commonRepository;
        private readonly SoftwareRepository _repository;
        private readonly IGeneralIncidentRepository _generalIncidentRepository;

        static String _connectionStringCellphone = ConfigurationManager.ConnectionStrings["CellPhoneForExcel"].ConnectionString;
        public ProjectManagerController(ProjectManagerRepository projectManagerRepository, GeneralIncidentRepository generalIncidentRepository)
        {
            this._projectManagerRepository = projectManagerRepository;
            _commercialRepository = new CommercialRepository();
            _hardwareRepository = new HardwareRepository();
            _commonRepository = new CommonRepository();
            _generalIncidentRepository = generalIncidentRepository;
        }
        // GET: ProjectManager
        public ActionResult Index()
        {
            long userId = Convert.ToInt64(User.Identity.Name);

            var sd = userId;

            ProjectMasterModel model = _projectManagerRepository.GetProjectMasterModel(1);
            var test = model;

            return View();
        }

        [Authorize(Roles = "PMHEAD")]
        public ActionResult HeadOfProject()
        {
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);

            var counter = new DashBoardCounter();
            ViewBag.Count = counter.GetDashBoardCounter("ProjectManager", "PMHEAD", userId);
            return View();

        }

        [Authorize(Roles = "PM")]
        public ActionResult ProjectManagerOfProjectDashboard()
        {

            PmTestCounterModel model = new PmTestCounterModel();

            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);

            var counter = new DashBoardCounter();
            ViewBag.Count = counter.GetDashBoardCounter("ProjectManager", "PM", userId);

            model = _projectManagerRepository.GetPmTestCounts(userId);

            return View(model);

        }

        [Authorize(Roles = "PMHEAD")]
        public ActionResult ListOfNewProject()
        {
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            var projectList = _projectManagerRepository.GetNewProjectsList();
            foreach (var i in projectList)
            {
                if (i.OrderNuber != null)
                {
                    i.OrderNumberOrdinal = CommonConversion.AddOrdinal((int)i.OrderNuber);
                }

            }
            var pmUser = _projectManagerRepository.GetPmCmnUsers();


            var newProjectAssign = new NewProjectsViewModel { ProjectMasters = projectList, CmnUsers = pmUser };
            return View(newProjectAssign);
        }

        [HttpPost]
        [Authorize(Roles = "PMHEAD")]
        [NotificationActionFilter(ReceiverRoles = "PM,PMHEAD,CM,CMBTRC,MM,QCHEAD,HWHEAD", MessageHeader = "Project Assign")]
        public JsonResult ProjectAssign(string projectMsterId, string selectedProjectManagerId, string projectHeadRemarks, string poNumber
            //, string pmAproDate
            )
        {
            long pMasterId, pManagerId;//, purchaseOrderNumber;
            long.TryParse(projectMsterId, out pMasterId);
            long.TryParse(selectedProjectManagerId, out pManagerId);
            // long.TryParse(poNumber, out purchaseOrderNumber);

            var assignResult = _projectManagerRepository.AssignProjectToProjectManager(pMasterId, pManagerId, projectHeadRemarks, poNumber  //, pmAproDate
                );
            _projectManagerRepository.InsertDataInBabt(pMasterId, pManagerId);
            //----------MAIL Start-----------------------
            var project = _projectManagerRepository.GetProjectMasterModel(pMasterId);
            var userAssigned = _projectManagerRepository.GetPmUserInfo(pManagerId);
            var assignedBy = _projectManagerRepository.GetPmUserInfo(userId);

            MailSendFromPms mailSendFromPms = new MailSendFromPms();

            mailSendFromPms.SendMail(new List<long>(new[] { pManagerId }), new List<string>(new[] { "MM", "CM", "PMHEAD", "HWHEAD", "QCHEAD", "SA", "PS" }), "Project Manager has been assigned for a New Project ",
            "This is to inform you that, <b>" + userAssigned.UserFullName + " </b> has been assigned by <b>" + assignedBy.UserFullName + "</b> for a new Project.<br/><br/><br/><br/>"
                       + "Project : <b>" + project.ProjectName + "</b> <br/>Sample Quantity - "
                      + project.NumberOfSample + "<br/>PO number - " + poNumber);
            //---------------ends-----------------

            var notificationClass = new NotificationObject
            {
                ToUser = pManagerId.ToString(CultureInfo.InvariantCulture),
                Message = "Assigned a project",
                ProjectId = pMasterId,
                AdditionalMessage = ""
            };
            ViewBag.ControllerVariable = notificationClass;
            return Json(assignResult, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "PMHEAD")]
        public ActionResult ListofAssignedProject()
        {
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);

            // var listOfAssignedProjects = _projectManagerRepository.GetAssignedProjectList();
            ViewBag.GetAssignedProjectList = _projectManagerRepository.GetAssignedProjectList();
            //var assignedProjectViewModelList = (from listOfAssignedProject in listOfAssignedProjects
            //                                    let currentMasterId = listOfAssignedProject.ProjectMasterId
            //                                    let pmAssignInfo = _projectManagerRepository.GetPmAssignInfo(currentMasterId)
            //                                    let pmUserId = pmAssignInfo.ProjectManagerUserId
            //                                    let pmUserInfo = _projectManagerRepository.GetPmUserInfo(pmUserId)
            //                                    let newAssign = _projectManagerRepository.GetPmCmnUsersForAssign()

            //                                    select new AssignProjectsViewModel
            //                                    {
            //                                        ProjectMasterModel = listOfAssignedProject,
            //                                        ProjectPmAssignModel = pmAssignInfo,
            //                                        PmCmnUserModel = pmUserInfo,
            //                                        CmnUserModels = newAssign

            //                                    }).ToList();

            ViewBag.GetPmCmnUsers = _projectManagerRepository.GetPmCmnUsers();
            return View();
        }
        [Authorize(Roles = "PM,PMHEAD")]
        public ActionResult IndividualProjectManagerDashBoard(long projectId = 0)
        {
            string name = "";
            var manager = new FileManager();
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);

            var listOfNewProjectOfPm = _projectManagerRepository.GetProjectMasterModelsByProjectManager(userId);

            var assigendProjectPmInfo = new AssignedProjectListViewModel { ProjectMasterModels = listOfNewProjectOfPm };
            var user = _projectManagerRepository.GetPmUserInfo(userId);

            assigendProjectPmInfo.UserName = user.UserFullName;
            assigendProjectPmInfo.UserId = user.CmnUserId;


            if (projectId > 0)
            {




                assigendProjectPmInfo.ProjectPmAssignModel = _projectManagerRepository.GetPmAssignInfo(projectId) ??
                                                             new ProjectPmAssignModel();
                assigendProjectPmInfo.IndividualProjectViewModel.PmBootImageAnimationModel = _projectManagerRepository.GetPmBootImageAnimationModel(projectId, userId) ?? new PmBootImageAnimationModel();
                assigendProjectPmInfo.IndividualProjectViewModel.PmBootImageAnimationModel.ImageUpload1 = manager.GetFile(assigendProjectPmInfo.IndividualProjectViewModel.PmBootImageAnimationModel.ImageUpload1);

                assigendProjectPmInfo.IndividualProjectViewModel.PmBootImageAnimationModel.VideoUpload1 = manager.GetFile(assigendProjectPmInfo.IndividualProjectViewModel.PmBootImageAnimationModel.VideoUpload1);

                assigendProjectPmInfo.IndividualProjectViewModel.PmBootImageAnimationModel.ImageExtension =
                       manager.GetExtension(
                           assigendProjectPmInfo.IndividualProjectViewModel.PmBootImageAnimationModel.ImageUpload1);
                assigendProjectPmInfo.IndividualProjectViewModel.PmBootImageAnimationModel.VideoExtension = manager.GetExtension(
                        assigendProjectPmInfo.IndividualProjectViewModel.PmBootImageAnimationModel.VideoUpload1);


                assigendProjectPmInfo.IndividualProjectViewModel.PmGiftBoxModel = _projectManagerRepository.GetPmGiftBoxModel(projectId, userId) ?? new PmGiftBoxModel();

                assigendProjectPmInfo.IndividualProjectViewModel.PmGiftBoxModel.PmGbImageUploadPath =
                    manager.GetFile(assigendProjectPmInfo.IndividualProjectViewModel.PmGiftBoxModel.PmGbImageUploadPath);
                assigendProjectPmInfo.IndividualProjectViewModel.PmGiftBoxModel.GBExtension = manager.GetExtension(
                       assigendProjectPmInfo.IndividualProjectViewModel.PmGiftBoxModel.PmGbImageUploadPath);


                assigendProjectPmInfo.IndividualProjectViewModel.PmIdModel =
                    _projectManagerRepository.GetPmIdModel(projectId, userId) ?? new PmIdModel();

                assigendProjectPmInfo.IndividualProjectViewModel.PmIdModel.PmFinishingImageUploadPath =
                  manager.GetFile(assigendProjectPmInfo.IndividualProjectViewModel.PmIdModel.PmFinishingImageUploadPath);
                assigendProjectPmInfo.IndividualProjectViewModel.PmIdModel.PmIdDesignImageUploadPath =
                  manager.GetFile(assigendProjectPmInfo.IndividualProjectViewModel.PmIdModel.PmIdDesignImageUploadPath);
                assigendProjectPmInfo.IndividualProjectViewModel.PmIdModel.PmLogoTypeImageUploadPath =
                  manager.GetFile(assigendProjectPmInfo.IndividualProjectViewModel.PmIdModel.PmLogoTypeImageUploadPath);
                assigendProjectPmInfo.IndividualProjectViewModel.PmIdModel.PmModelPrintImageUploadPath =
                  manager.GetFile(assigendProjectPmInfo.IndividualProjectViewModel.PmIdModel.PmModelPrintImageUploadPath);


                assigendProjectPmInfo.IndividualProjectViewModel.PmIdModel.FinishingDesignExtension = manager.GetExtension(
                       assigendProjectPmInfo.IndividualProjectViewModel.PmIdModel.PmFinishingImageUploadPath); assigendProjectPmInfo.IndividualProjectViewModel.PmIdModel.LogoDesignExtension = manager.GetExtension(
                       assigendProjectPmInfo.IndividualProjectViewModel.PmIdModel.PmLogoTypeImageUploadPath); assigendProjectPmInfo.IndividualProjectViewModel.PmIdModel.ModelPrintDesignExtension = manager.GetExtension(
                       assigendProjectPmInfo.IndividualProjectViewModel.PmIdModel.PmModelPrintImageUploadPath); assigendProjectPmInfo.IndividualProjectViewModel.PmIdModel.IdDesignExtension = manager.GetExtension(
                       assigendProjectPmInfo.IndividualProjectViewModel.PmIdModel.PmIdDesignImageUploadPath);

                assigendProjectPmInfo.IndividualProjectViewModel.PmLabelsModel =
                    _projectManagerRepository.GetPmLabelsModel(projectId, userId) ?? new PmLabelsModel();

                assigendProjectPmInfo.IndividualProjectViewModel.PmLabelsModel.PmLabelImageUploadPath =
                   manager.GetFile(assigendProjectPmInfo.IndividualProjectViewModel.PmLabelsModel.PmLabelImageUploadPath);
                assigendProjectPmInfo.IndividualProjectViewModel.PmLabelsModel.LabelImageExtension = manager.GetExtension(
                       assigendProjectPmInfo.IndividualProjectViewModel.PmLabelsModel.PmLabelImageUploadPath);

                assigendProjectPmInfo.IndividualProjectViewModel.PmScreenProtectorModel =
                    _projectManagerRepository.GetPmScreenProtectorModel(projectId, userId) ?? new PmScreenProtectorModel();

                assigendProjectPmInfo.IndividualProjectViewModel.PmScreenProtectorModel.PmScreenProtectorImageUploadPath =
                   manager.GetFile(assigendProjectPmInfo.IndividualProjectViewModel.PmScreenProtectorModel.PmScreenProtectorImageUploadPath);
                assigendProjectPmInfo.IndividualProjectViewModel.PmScreenProtectorModel.SpExtension = manager.GetExtension(
                       assigendProjectPmInfo.IndividualProjectViewModel.PmScreenProtectorModel.PmScreenProtectorImageUploadPath);

                assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel =
                    _projectManagerRepository.GetPmWalpaperModel(projectId, userId) ?? new PmWalpaperModel();

                assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload1 =
                    manager.GetFile(assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload1);
                assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.W1Extension = manager.GetExtension(
                     assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload1);

                assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload2 =
                    manager.GetFile(assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload2);

                assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.W2Extension = manager.GetExtension(
                 assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload2);

                assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload3 =
                    manager.GetFile(assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload3);

                assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.W3Extension = manager.GetExtension(
                 assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload3);


                assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload4 =
                    manager.GetFile(assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload4);


                assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.W4Extension = manager.GetExtension(
                 assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload4);

                assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload5 =
                    manager.GetFile(assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload5);

                assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.W5Extension = manager.GetExtension(
                 assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload5);


                assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload6 =
                    manager.GetFile(assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload6);

                assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.W6Extension = manager.GetExtension(
                 assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload6);

                assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload7 =
                    manager.GetFile(assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload7);

                assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.W7Extension = manager.GetExtension(
                 assigendProjectPmInfo.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload7);
                //assigendProjectPmInfo.IndividualProjectViewModel.PmSwCustomizationModel =
                //    _projectManagerRepository.GetPmSwCustomizationModel(projectId) ?? new PmSwCustomizationModel();

                //assigendProjectPmInfo.IndividualProjectViewModel.PmSwCustomizationModel.PmSwCustomizationUploadPath =
                //  manager.GetFile(assigendProjectPmInfo.IndividualProjectViewModel.PmSwCustomizationModel.PmSwCustomizationUploadPath);

                //assigendProjectPmInfo.IndividualProjectViewModel.PmSwCustomizationModel.PmSwCustomizationUploadPath2 =
                //  manager.GetFile(assigendProjectPmInfo.IndividualProjectViewModel.PmSwCustomizationModel.PmSwCustomizationUploadPath2);

                //assigendProjectPmInfo.IndividualProjectViewModel.PmSwCustomizationModel.SwExtension = manager.GetExtension(
                //       assigendProjectPmInfo.IndividualProjectViewModel.PmScreenProtectorModel.PmScreenProtectorImageUploadPath);

                //assigendProjectPmInfo.IndividualProjectViewModel.PmSwCustomizationModel.SwExtension2 = manager.GetExtension(
                //       assigendProjectPmInfo.IndividualProjectViewModel.PmScreenProtectorModel.PmScreenProtectorImageUploadPath);

                assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel =
                    _projectManagerRepository.GetPmPhnAccessoriesModel(projectId, userId) ?? new PmPhnAccessoriesModel();

                assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesEarphone =
                    manager.GetFile(
                        assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesEarphone);

                assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesEarphoneExtension = manager.GetExtension(
                 assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesEarphone);


                assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesUSBCable =
                    manager.GetFile(
                        assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesUSBCable);

                assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesUSBCableExtension = manager.GetExtension(
              assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesUSBCable);


                assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesCharger =
                    manager.GetFile(
                        assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesCharger);

                assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesChargerExtension = manager.GetExtension(
               assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesCharger);




                assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesOTGCable =
                    manager.GetFile(
                        assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesOTGCable);


                assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesOTGCableExtension = manager.GetExtension(
              assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesOTGCable);





                assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesBackCover =
                    manager.GetFile(
                        assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesBackCover);


                assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesBackCoverExtension = manager.GetExtension(
            assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesBackCover);





                assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesFlipCover =
                    manager.GetFile(
                        assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesFlipCover);
                assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesFlipCoverExtension = manager.GetExtension(
          assigendProjectPmInfo.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesFlipCover);




                assigendProjectPmInfo.IndividualProjectViewModel.ProjectMasterModel =
                    _commercialRepository.GetProjectMasterModelForPm(projectId) ?? new ProjectMasterModel();

                assigendProjectPmInfo.IndividualProjectViewModel.PmSwCustomizationInitialModels =
                    _projectManagerRepository.GetPmSwCustomizationInitialModels(projectId);

                assigendProjectPmInfo.IndividualProjectViewModel.PmSwCustomizationFinalModels =
                    _projectManagerRepository.GetPmSwCustomizationFinalModels(projectId, userId);

                assigendProjectPmInfo.IndividualProjectViewModel.PmPhnCameraModel =
                    _projectManagerRepository.GetCameraModel(projectId, userId);
            }

            // assigendProjectPmInfo.ProjectName = name;
            return View(assigendProjectPmInfo);
        }

        [HttpGet]
        [Authorize(Roles = "PM,PMHEAD")]
        public ActionResult SoftwareCustomizationActionResult(long projectId)
        {
            var model = new VmSoftwareCustomization
            {
                ProjectId = projectId
            };
            var tuple = _projectManagerRepository.GetSoftwareCustomizationDataList(projectId);
            model.PmSwCustomizationFinalModels = tuple.Item1.OrderBy(i => i.PmSwCustomizationFinalMenu).ToList();
            model.IsUpdateable = tuple.Item2;
            //model.Others = model.PmSwCustomizationFinalModels.Where(i => i.PmSwCustomizationFinalMenu == "Others").ToList();
            //List<PmSwCustomizationFinalModel> customizationFinalModels = _projectManagerRepository.GetSoftwareCustomizationDataList(projectId);
            return PartialView("~/Views/ProjectManager/Partial/_SoftwareCutomization.cshtml", model);
        }

        [HttpPost]
        [Authorize(Roles = "PM,PMHEAD")]
        public ActionResult SoftwareCustomizationActionResult(VmSoftwareCustomization model, long projectId = 0)
        {
            bool isSaved = false;

            if (model.PmSwCustomizationFinalModels.Any() && model.IsUpdateable == false)
            {
                _projectManagerRepository.SaveSwCustomizationInfo(model);
            }
            else if (model.PmSwCustomizationFinalModels.Any() && model.IsUpdateable)
            {
                isSaved = _projectManagerRepository.UpdateSoftwareCustomization(model);
            }
            TempData["Message"] = "Software Customization Added Successfully";
            TempData["tabId"] = 7;
            return RedirectToAction("IndividualProjectManagerDashBoard", new { projectId = model.PmSwCustomizationFinalModels[0].ProjectMasterId });
        }
        [HttpPost]
        [Authorize(Roles = "PM")]
        public ActionResult AddOtherSoftwareCustomization(long projectId)
        {
            var model = new PmSwCustomizationFinalModel()
            {
                PmSwCustomizationFinalMenu = "Others",
                PmSwCustomizationFinalPath = "",
                PmSwCustomizationFinalSettings = "",
                ProjectMasterId = projectId
            };
            return PartialView("~/Views/ProjectManager/Partial/_AddSoftwareCustomizationOther.cshtml", model);
        }

        [NotificationActionFilter(ReceiverRoles = "PM,CM,PMHEAD,MM,HWHEAD")]
        [HttpPost]
        [Authorize(Roles = "PM,PMHEAD")]

        public ActionResult PmUploadFile(AssignedProjectListViewModel model, int tabIdentifier)
        {
            long result;
            TempData["tabId"] = tabIdentifier;
            var userId = Convert.ToInt64(User.Identity.Name);
            var userInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            if (tabIdentifier == (decimal)GlobalIdentifier.ProjectManagerTabIdentifier.BootAnimation)
            {
                if (model.IndividualProjectViewModel.PmBootImageAnimationModel.PmBootImageAnimationId > 0)
                {

                    result = _projectManagerRepository.UpdateBootImageAnimationInfo(model.IndividualProjectViewModel.PmBootImageAnimationModel);

                    if (result > 0)
                    {
                        //====notification====
                        var notificationObject = new NotificationObject
                        {
                            ProjectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId,
                            ToUser = "-1",
                        };
                        notificationObject.Message = " has uploaded boot image & animation ";
                        notificationObject.AdditionalMessage = "";
                        ViewBag.ControllerVariable = notificationObject;
                        //==================
                        TempData["Message"] = "Boot Image and Animation Saved Successfully";
                        TempData["messageType"] = "s";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });

                    }
                    else
                    {
                        TempData["Message"] = "Boot Image and Animation save failed";
                        TempData["messageType"] = "e";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                }
                else
                {
                    model.IndividualProjectViewModel.PmBootImageAnimationModel.ProjectMasterId =
                        model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId;

                    var manager = new FileManager();

                    const string userFileDirectory = "PM";

                    const string moduleDirectory = "BootAnimation";
                    model.IndividualProjectViewModel.PmBootImageAnimationModel.ProjectAssignId =
                        model.ProjectPmAssignModel.ProjectPmAssignId;
                    model.IndividualProjectViewModel.PmBootImageAnimationModel.Added = userId;
                    model.IndividualProjectViewModel.PmBootImageAnimationModel.AddedDate = DateTime.Now;
                    model.IndividualProjectViewModel.PmBootImageAnimationModel.Updated = userId;
                    model.IndividualProjectViewModel.PmBootImageAnimationModel.UpdatedDate = DateTime.Now;
                    model.IndividualProjectViewModel.PmBootImageAnimationModel.AssignUserId =
                        model.ProjectPmAssignModel.AssignUserId;
                    model.IndividualProjectViewModel.PmBootImageAnimationModel.Remarks =
                        model.IndividualProjectViewModel.PmBootImageAnimationModel.Remarks;
                    model.IndividualProjectViewModel.PmBootImageAnimationModel.ImageUpload1 = manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId, userFileDirectory, moduleDirectory, model.IndividualProjectViewModel.PmBootImageAnimationModel.ImageUploadFile);
                    model.IndividualProjectViewModel.PmBootImageAnimationModel.VideoUpload1 =
                        manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId,
                            userFileDirectory, moduleDirectory,
                            model.IndividualProjectViewModel.PmBootImageAnimationModel.VideoUploadFile);

                    result = _projectManagerRepository.SaveBootImageAnimationInfo(model.IndividualProjectViewModel.PmBootImageAnimationModel);

                    if (result > 0)
                    {
                        //====notification====
                        var notificationObject = new NotificationObject
                        {
                            ProjectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId,
                            ToUser = "-1",
                        };
                        notificationObject.Message = " has uploaded boot image & animation ";
                        notificationObject.AdditionalMessage = "";
                        ViewBag.ControllerVariable = notificationObject;
                        //==================
                        TempData["Message"] = "Boot Image and Animation Save Successfully";
                        TempData["messageType"] = "s";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });

                    }
                    else
                    {
                        TempData["Message"] = "Boot Image and Animation save failed";
                        TempData["messageType"] = "e";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                }

            }
            else if (tabIdentifier == (decimal)GlobalIdentifier.ProjectManagerTabIdentifier.GiftBox)
            {
                if (model.IndividualProjectViewModel.PmGiftBoxModel.PmGiftBoxId > 0)
                {

                    result = _projectManagerRepository.UpdateGBinfo(model.IndividualProjectViewModel.PmGiftBoxModel);
                    if (result > 0)
                    {
                        //====notification====
                        var notificationObject = new NotificationObject
                        {
                            ProjectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId,
                            ToUser = "-1",
                        };
                        notificationObject.Message = " has uploaded GB design. ";
                        notificationObject.AdditionalMessage = "";
                        ViewBag.ControllerVariable = notificationObject;
                        //==================
                        TempData["Message"] = "GB design has Saved Successfully";
                        TempData["messageType"] = "s";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                    else
                    {
                        TempData["Message"] = "GB Design upload failed";
                        TempData["messageType"] = "e";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                }
                else
                {
                    model.IndividualProjectViewModel.PmGiftBoxModel.ProjectMasterId =
                     model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId;

                    var manager = new FileManager();

                    const string userFileDirectory = "PM";

                    const string moduleDirectory = "GBDesign";
                    model.IndividualProjectViewModel.PmGiftBoxModel.ProjectAssignId =
                        model.ProjectPmAssignModel.ProjectPmAssignId;

                    model.IndividualProjectViewModel.PmGiftBoxModel.Added = userId;
                    model.IndividualProjectViewModel.PmGiftBoxModel.AddedDate = DateTime.Now;
                    model.IndividualProjectViewModel.PmGiftBoxModel.Updated = userId;
                    model.IndividualProjectViewModel.PmGiftBoxModel.UpdatedDate = DateTime.Now;
                    model.IndividualProjectViewModel.PmGiftBoxModel.AssignUserId =
                      model.ProjectPmAssignModel.AssignUserId;
                    model.IndividualProjectViewModel.PmGiftBoxModel.Remarks =
                        model.IndividualProjectViewModel.PmGiftBoxModel.Remarks;
                    model.IndividualProjectViewModel.PmGiftBoxModel.PmGbImageUploadPath = manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId, userFileDirectory, moduleDirectory, model.IndividualProjectViewModel.PmGiftBoxModel.GbDesignUploadFile);

                    result = _projectManagerRepository.SaveGbInfo(model.IndividualProjectViewModel.PmGiftBoxModel);

                    if (result > 0)
                    {
                        //====notification====
                        var notificationObject = new NotificationObject
                        {
                            ProjectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId,
                            ToUser = "-1",
                        };
                        notificationObject.Message = " has uploaded GB design. ";
                        notificationObject.AdditionalMessage = "";
                        ViewBag.ControllerVariable = notificationObject;
                        //==================
                        TempData["Message"] = "GB design has Saved Successfully";
                        TempData["messageType"] = "s";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                    else
                    {
                        TempData["Message"] = "GB Design upload failed";
                        TempData["messageType"] = "e";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                }
            }
            else if (tabIdentifier == (decimal)GlobalIdentifier.ProjectManagerTabIdentifier.Label)
            {
                if (model.IndividualProjectViewModel.PmLabelsModel.PmLabelId > 0)
                {

                    result = _projectManagerRepository.UpdateLabelInfo(model.IndividualProjectViewModel.PmLabelsModel);

                    if (result > 0)
                    {
                        //====notification====
                        var notificationObject = new NotificationObject
                        {
                            ProjectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId,
                            ToUser = "-1",
                        };
                        notificationObject.Message = " has uploaded Label design. ";
                        notificationObject.AdditionalMessage = "";
                        ViewBag.ControllerVariable = notificationObject;
                        //==================
                        TempData["Message"] = "Label Design Save Successfully";
                        TempData["messageType"] = "s";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                    else
                    {
                        TempData["Message"] = "Label Design upload failed";
                        TempData["messageType"] = "e";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                }
                else
                {
                    model.IndividualProjectViewModel.PmLabelsModel.ProjectMasterId =
                      model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId;

                    model.IndividualProjectViewModel.PmLabelsModel.AssignUserId =
                     model.ProjectPmAssignModel.AssignUserId;
                    model.IndividualProjectViewModel.PmLabelsModel.Remarks =
                        model.IndividualProjectViewModel.PmLabelsModel.Remarks;

                    model.IndividualProjectViewModel.PmLabelsModel.Added = userId;
                    model.IndividualProjectViewModel.PmLabelsModel.AddedDate = DateTime.Now;
                    model.IndividualProjectViewModel.PmLabelsModel.Updated = userId;
                    model.IndividualProjectViewModel.PmLabelsModel.UpdatedDate = DateTime.Now;

                    var manager = new FileManager();

                    const string userFileDirectory = "PM";

                    const string moduleDirectory = "Label";


                    model.IndividualProjectViewModel.PmLabelsModel.ProjectAssignId =
                        model.ProjectPmAssignModel.ProjectPmAssignId;
                    model.IndividualProjectViewModel.PmLabelsModel.PmLabelImageUploadPath = manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId, userFileDirectory, moduleDirectory, model.IndividualProjectViewModel.PmLabelsModel.LabelImageUploadFile);

                    result = _projectManagerRepository.SaveLabelInfo(model.IndividualProjectViewModel.PmLabelsModel);

                    if (result > 0)
                    {
                        //====notification====
                        var notificationObject = new NotificationObject
                        {
                            ProjectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId,
                            ToUser = "-1",
                        };
                        notificationObject.Message = " has uploaded Label design. ";
                        notificationObject.AdditionalMessage = "";
                        ViewBag.ControllerVariable = notificationObject;
                        //==================
                        TempData["Message"] = "Label Design Save Successfully";
                        TempData["messageType"] = "s";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                    else
                    {
                        TempData["Message"] = "Label Design upload failed";
                        TempData["messageType"] = "e";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }

                }
            }
            else if (tabIdentifier == (decimal)GlobalIdentifier.ProjectManagerTabIdentifier.IdModel)
            {
                if (model.IndividualProjectViewModel.PmIdModel.PmIDId > 0)
                {

                    result = _projectManagerRepository.UpdateIdInfo(model.IndividualProjectViewModel.PmIdModel);

                    if (result > 0)
                    {
                        //====notification====
                        var notificationObject = new NotificationObject
                        {
                            ProjectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId,
                            ToUser = "-1",
                        };
                        notificationObject.Message = " has uploaded ID design. ";
                        notificationObject.AdditionalMessage = "";
                        ViewBag.ControllerVariable = notificationObject;
                        //==================
                        TempData["Message"] = "ID Design Save Successfully";
                        TempData["messageType"] = "s";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                    else
                    {
                        TempData["Message"] = "ID design uplaod failed";
                        TempData["messageType"] = "e";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                }
                else
                {
                    model.IndividualProjectViewModel.PmIdModel.ProjectMasterId =
                      model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId;

                    model.IndividualProjectViewModel.PmIdModel.AssignUserId =
                    model.ProjectPmAssignModel.AssignUserId;
                    model.IndividualProjectViewModel.PmIdModel.Remarks =
                        model.IndividualProjectViewModel.PmIdModel.Remarks;

                    model.IndividualProjectViewModel.PmIdModel.Added = userId;
                    model.IndividualProjectViewModel.PmIdModel.AddedDate = DateTime.Now;
                    model.IndividualProjectViewModel.PmIdModel.Updated = userId;
                    model.IndividualProjectViewModel.PmIdModel.UpdatedDate = DateTime.Now;


                    var manager = new FileManager();
                    const string userFileDirectory = "PM";
                    const string moduleDirectory = "ID";

                    model.IndividualProjectViewModel.PmIdModel.ProjectAssignId =
                        model.ProjectPmAssignModel.ProjectPmAssignId;

                    model.IndividualProjectViewModel.PmIdModel.PmFinishingImageUploadPath = manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId, userFileDirectory, moduleDirectory, model.IndividualProjectViewModel.PmIdModel.PmFinishImageUpload);

                    model.IndividualProjectViewModel.PmIdModel.PmLogoTypeImageUploadPath = manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId, userFileDirectory, moduleDirectory, model.IndividualProjectViewModel.PmIdModel.PmLogoTypeImageUpload);

                    model.IndividualProjectViewModel.PmIdModel.PmModelPrintImageUploadPath = manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId, userFileDirectory, moduleDirectory, model.IndividualProjectViewModel.PmIdModel.PmModelPrintImageUpload);


                    result = _projectManagerRepository.SaveIdInfo(model.IndividualProjectViewModel.PmIdModel);

                    if (result > 0)
                    {
                        //====notification====
                        var notificationObject = new NotificationObject
                        {
                            ProjectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId,
                            ToUser = "-1",
                        };
                        notificationObject.Message = " has uploaded ID design. ";
                        notificationObject.AdditionalMessage = "";
                        ViewBag.ControllerVariable = notificationObject;
                        //==================
                        TempData["Message"] = "ID Design Save Successfully";
                        TempData["messageType"] = "s";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                    else
                    {
                        TempData["Message"] = "ID design uplaod failed";
                        TempData["messageType"] = "e";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                }
            }
            else if (tabIdentifier == (decimal)GlobalIdentifier.ProjectManagerTabIdentifier.Sprotector)
            {
                if (model.IndividualProjectViewModel.PmScreenProtectorModel.PmScreenProtectorId > 0)
                {

                    result = _projectManagerRepository.UpdateScreenProtectorInfo(model.IndividualProjectViewModel.PmScreenProtectorModel);

                    if (result > 0)
                    {
                        //====notification====
                        var notificationObject = new NotificationObject
                        {
                            ProjectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId,
                            ToUser = "-1",
                        };
                        notificationObject.Message = " has uploaded Screen Protector. ";
                        notificationObject.AdditionalMessage = "";
                        ViewBag.ControllerVariable = notificationObject;
                        //==================
                        TempData["Message"] = "Screen Protector Save Successfully";
                        TempData["messageType"] = "s";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                    else
                    {
                        TempData["Message"] = "Screen Protector upload failed";
                        TempData["messageType"] = "e";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                }
                else
                {
                    model.IndividualProjectViewModel.PmScreenProtectorModel.ProjectMasterId =
                      model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId;

                    model.IndividualProjectViewModel.PmScreenProtectorModel.AssignUserId =
                  model.ProjectPmAssignModel.AssignUserId;
                    model.IndividualProjectViewModel.PmScreenProtectorModel.Remarks =
                        model.IndividualProjectViewModel.PmScreenProtectorModel.Remarks;

                    model.IndividualProjectViewModel.PmScreenProtectorModel.Added = userId;
                    model.IndividualProjectViewModel.PmScreenProtectorModel.AddedDate = DateTime.Now;
                    model.IndividualProjectViewModel.PmScreenProtectorModel.Updated = userId;
                    model.IndividualProjectViewModel.PmScreenProtectorModel.UpdatedDate = DateTime.Now;

                    var manager = new FileManager();

                    const string userFileDirectory = "PM";

                    const string moduleDirectory = "SCRNProtector";


                    model.IndividualProjectViewModel.PmScreenProtectorModel.ProjectAssignId =
                        model.ProjectPmAssignModel.ProjectPmAssignId;

                    model.IndividualProjectViewModel.PmScreenProtectorModel.PmScreenProtectorImageUploadPath = manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId, userFileDirectory, moduleDirectory, model.IndividualProjectViewModel.PmScreenProtectorModel.PmScreenProtectorImageUpload);

                    result = _projectManagerRepository.SaveScreenProtectorInfo(model.IndividualProjectViewModel.PmScreenProtectorModel);

                    if (result > 0)
                    {
                        //====notification====
                        var notificationObject = new NotificationObject
                        {
                            ProjectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId,
                            ToUser = "-1",
                        };
                        notificationObject.Message = " has uploaded Screen Protector. ";
                        notificationObject.AdditionalMessage = "";
                        ViewBag.ControllerVariable = notificationObject;
                        //==================
                        TempData["Message"] = "Screen Protector Save Successfully";
                        TempData["messageType"] = "s";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                    else
                    {
                        TempData["Message"] = "Screen Protector upload failed";
                        TempData["messageType"] = "e";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                }
            }
            else if (tabIdentifier == (decimal)GlobalIdentifier.ProjectManagerTabIdentifier.ServiceDoc)
            {
                if (model.IndividualProjectViewModel.PmServiceDocumentsModel.PmServiceDocumentId > 0)
                {
                    //this upload method is incomplete. may be it will not use
                    result = _projectManagerRepository.UpdateServiceDocInfo(model.IndividualProjectViewModel.PmServiceDocumentsModel);

                    if (result > 0)
                    {
                        TempData["Message"] = "Boot Image and Animation Save Successfully";
                        TempData["messageType"] = "s";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                    else
                    {
                        TempData["Message"] = "Boot Image and Animation Not Save Successfully";
                        TempData["messageType"] = "e";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                }
                else
                {
                    model.IndividualProjectViewModel.PmServiceDocumentsModel.ProjectMasterId =
                      model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId;

                    result = _projectManagerRepository.SaveServiceDocInfo(model.IndividualProjectViewModel.PmServiceDocumentsModel);

                    if (result > 0)
                    {
                        TempData["Message"] = "Boot Image and Animation Save Successfully";
                        TempData["messageType"] = "s";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                    else
                    {
                        TempData["Message"] = "Boot Image and Animation Not Save Successfully";
                        TempData["messageType"] = "e";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                }
            }
            //else if (tabIdentifier == (decimal)GlobalIdentifier.ProjectManagerTabIdentifier.SwCustomization)
            //{
            //    if (model.IndividualProjectViewModel.PmSwCustomizationModel.PmSwCustomizationId > 0)
            //    {

            //        result = _projectManagerRepository.UpdateSwCustomizationInfo(model.IndividualProjectViewModel.PmSwCustomizationModel);
            //    }
            //    else
            //    {
            //        model.IndividualProjectViewModel.PmSwCustomizationModel.ProjectMasterId =
            //          model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId;


            //        FileManager manager = new FileManager();

            //        var UserFileDirectory = "PM";

            //        var moduleDirectory = "SWCustomization";


            //        model.IndividualProjectViewModel.PmSwCustomizationModel.ProjectAssignId =
            //            model.ProjectPmAssignModel.ProjectPmAssignId;


            //        model.IndividualProjectViewModel.PmSwCustomizationModel.Added = userId;
            //        model.IndividualProjectViewModel.PmSwCustomizationModel.AddedDate = DateTime.Now;
            //        model.IndividualProjectViewModel.PmSwCustomizationModel.Updated = userId;
            //        model.IndividualProjectViewModel.PmSwCustomizationModel.UpdatedDate = DateTime.Now;

            //        model.IndividualProjectViewModel.PmSwCustomizationModel.PmSwCustomizationUploadPath = manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId, UserFileDirectory, moduleDirectory, model.IndividualProjectViewModel.PmSwCustomizationModel.PmSwCustomizationUploadFile1); model.IndividualProjectViewModel.PmSwCustomizationModel.PmSwCustomizationUploadPath2 = manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId, UserFileDirectory, moduleDirectory, model.IndividualProjectViewModel.PmSwCustomizationModel.PmSwCustomizationUploadFile2);
            //result = 1;//_projectManagerRepository.SaveSwCustomizationInfo(model.IndividualProjectViewModel.PmSwCustomizationModel);
            //        result = _projectManagerRepository.SaveSwCustomizationInfo(model.IndividualProjectViewModel.PmSwCustomizationModel);

            //        if (result > 0)
            //        {

            //            return RedirectToAction("IndividualProjectManagerDashBoard",
            //                new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
            //        }
            //    }
            //}
            else if (tabIdentifier == (decimal)GlobalIdentifier.ProjectManagerTabIdentifier.Walpaper)
            {
                if (model.IndividualProjectViewModel.PmWalpaperModel.PmWalpaperId > 0)
                {

                    result = _projectManagerRepository.UpdateWalPaperInfo(model.IndividualProjectViewModel.PmWalpaperModel);

                    if (result > 0)
                    {
                        //====notification====
                        var notificationObject = new NotificationObject
                        {
                            ProjectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId,
                            ToUser = "-1",
                        };
                        notificationObject.Message = " has uploaded Wallpaper. ";
                        notificationObject.AdditionalMessage = "";
                        ViewBag.ControllerVariable = notificationObject;
                        //==================
                        TempData["Message"] = "Wallpaper Saved Successfully";
                        TempData["messageType"] = "s";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                    else
                    {
                        TempData["Message"] = "Wallpaper Save Failed";
                        TempData["messageType"] = "e";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                }
                else
                {
                    model.IndividualProjectViewModel.PmWalpaperModel.ProjectMasterId =
                      model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId;

                    model.IndividualProjectViewModel.PmWalpaperModel.AssignUserId =
                 model.ProjectPmAssignModel.AssignUserId;
                    model.IndividualProjectViewModel.PmWalpaperModel.Remarks =
                        model.IndividualProjectViewModel.PmWalpaperModel.Remarks;

                    model.IndividualProjectViewModel.PmWalpaperModel.Added = userId;
                    model.IndividualProjectViewModel.PmWalpaperModel.AddedDate = DateTime.Now;
                    model.IndividualProjectViewModel.PmWalpaperModel.Updated = userId;
                    model.IndividualProjectViewModel.PmWalpaperModel.UpdatedDate = DateTime.Now;


                    var manager = new FileManager();

                    const string userFileDirectory = "PM";

                    const string moduleDirectory = "WallPaper";


                    model.IndividualProjectViewModel.PmWalpaperModel.ProjectAssignId =
                        model.ProjectPmAssignModel.ProjectPmAssignId;

                    model.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload1 = manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId, userFileDirectory, moduleDirectory, model.IndividualProjectViewModel.PmWalpaperModel.WalpaperFile1);

                    model.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload2 = manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId, userFileDirectory, moduleDirectory, model.IndividualProjectViewModel.PmWalpaperModel.WalpaperFile2);

                    model.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload3 = manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId, userFileDirectory, moduleDirectory, model.IndividualProjectViewModel.PmWalpaperModel.WalpaperFile3);


                    model.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload4 = manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId, userFileDirectory, moduleDirectory, model.IndividualProjectViewModel.PmWalpaperModel.WalpaperFile4);


                    model.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload5 = manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId, userFileDirectory, moduleDirectory, model.IndividualProjectViewModel.PmWalpaperModel.WalpaperFile5);


                    model.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload6 = manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId, userFileDirectory, moduleDirectory, model.IndividualProjectViewModel.PmWalpaperModel.WalpaperFile6);

                    model.IndividualProjectViewModel.PmWalpaperModel.WalpaperUpload7 = manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId, userFileDirectory, moduleDirectory, model.IndividualProjectViewModel.PmWalpaperModel.WalpaperFile7);

                    result = _projectManagerRepository.SaveWalPaperInfo(model.IndividualProjectViewModel.PmWalpaperModel);

                    if (result > 0)
                    {
                        //====notification====
                        var notificationObject = new NotificationObject
                        {
                            ProjectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId,
                            ToUser = "-1",
                        };
                        notificationObject.Message = " has uploaded Wallpaper. ";
                        notificationObject.AdditionalMessage = "";
                        ViewBag.ControllerVariable = notificationObject;
                        //==================
                        TempData["Message"] = "Wallpaper Saved Successfully";
                        TempData["messageType"] = "s";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                    else
                    {
                        TempData["Message"] = "Wallpaper Save Failed";
                        TempData["messageType"] = "e";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                }
            }
            else if (tabIdentifier == (decimal)GlobalIdentifier.ProjectManagerTabIdentifier.Accessories)
            {
                if (model.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesID > 0)
                {

                    result = _projectManagerRepository.UpdateAccessories(model.IndividualProjectViewModel.PmPhnAccessoriesModel);

                    if (result > 0)
                    {
                        //====notification====
                        var notificationObject = new NotificationObject
                        {
                            ProjectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId,
                            ToUser = "-1",
                        };
                        notificationObject.Message = " has uploaded Accessories image. ";
                        notificationObject.AdditionalMessage = "";
                        ViewBag.ControllerVariable = notificationObject;
                        //==================
                        TempData["Message"] = "Accessories Image Saved Successfully";
                        TempData["messageType"] = "s";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                    else
                    {
                        TempData["Message"] = "Accessories Image Upload Failed";
                        TempData["messageType"] = "e";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                }
                else
                {
                    model.IndividualProjectViewModel.PmPhnAccessoriesModel.ProjectMasterId =
                      model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId;


                    var manager = new FileManager();

                    const string userFileDirectory = "PM";

                    const string moduleDirectory = "Accessories";


                    model.IndividualProjectViewModel.PmPhnAccessoriesModel.ProjectAssignId =
                        model.ProjectPmAssignModel.ProjectPmAssignId;

                    model.IndividualProjectViewModel.PmPhnAccessoriesModel.AssignUserId =
                model.ProjectPmAssignModel.AssignUserId;
                    model.IndividualProjectViewModel.PmPhnAccessoriesModel.Remarks =
                        model.IndividualProjectViewModel.PmPhnAccessoriesModel.Remarks;

                    model.IndividualProjectViewModel.PmPhnAccessoriesModel.Added = userId;
                    model.IndividualProjectViewModel.PmPhnAccessoriesModel.AddedDate = DateTime.Now;
                    model.IndividualProjectViewModel.PmPhnAccessoriesModel.Updated = userId;
                    model.IndividualProjectViewModel.PmPhnAccessoriesModel.UpdatedDate = DateTime.Now;

                    model.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesOTGCable = manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId, userFileDirectory, moduleDirectory, model.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesOTGCableFile);

                    model.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesUSBCable = manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId, userFileDirectory, moduleDirectory, model.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesUSBCableFile);

                    model.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesEarphone = manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId, userFileDirectory, moduleDirectory, model.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesEarphoneFile);


                    model.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesCharger = manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId, userFileDirectory, moduleDirectory, model.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesChargerFile);


                    model.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesFlipCover = manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId, userFileDirectory, moduleDirectory, model.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesFlipCoverFile);

                    model.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesBackCover = manager.Upload(model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId, userFileDirectory, moduleDirectory, model.IndividualProjectViewModel.PmPhnAccessoriesModel.PmPhnAccessoriesBackCoverFile);

                    result = _projectManagerRepository.SaveAccessoriesInfo(model.IndividualProjectViewModel.PmPhnAccessoriesModel);

                    if (result > 0)
                    {
                        //====notification====
                        var notificationObject = new NotificationObject
                        {
                            ProjectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId,
                            ToUser = "-1",
                        };
                        notificationObject.Message = " has uploaded Accessories image. ";
                        notificationObject.AdditionalMessage = "";
                        ViewBag.ControllerVariable = notificationObject;
                        //==================
                        TempData["Message"] = "Accessories Image Saved Successfully";
                        TempData["messageType"] = "s";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                    else
                    {
                        TempData["Message"] = "Accessories Image Upload Failed";
                        TempData["messageType"] = "e";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                }
            }

            else if (tabIdentifier == (decimal)GlobalIdentifier.ProjectManagerTabIdentifier.Camera)
            {
                if (model.IndividualProjectViewModel.PmPhnCameraModel.PmPhnCameraID > 0)
                {

                    result = _projectManagerRepository.UpdateCameraInfo(model.IndividualProjectViewModel.PmPhnCameraModel);

                    if (result > 0)
                    {
                        //====notification====
                        var notificationObject = new NotificationObject
                        {
                            ProjectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId,
                            ToUser = "-1",
                        };
                        notificationObject.Message = " has uploaded Camera image. ";
                        notificationObject.AdditionalMessage = "";
                        ViewBag.ControllerVariable = notificationObject;
                        //==================
                        TempData["Message"] = "Camera Image Save Successfully";
                        TempData["messageType"] = "s";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                    else
                    {
                        TempData["Message"] = "Camera Image Upload Failed";
                        TempData["messageType"] = "e";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                }
                else
                {
                    model.IndividualProjectViewModel.PmPhnCameraModel.ProjectMasterId =
                      model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId;

                    model.IndividualProjectViewModel.PmPhnCameraModel.ProjectAssignId =
                        model.ProjectPmAssignModel.ProjectPmAssignId;

                    model.IndividualProjectViewModel.PmPhnCameraModel.AssignUserId =
          model.ProjectPmAssignModel.AssignUserId;
                    model.IndividualProjectViewModel.PmPhnCameraModel.Remarks =
                        model.IndividualProjectViewModel.PmPhnCameraModel.Remarks;

                    model.IndividualProjectViewModel.PmPhnCameraModel.Added = userId;
                    model.IndividualProjectViewModel.PmPhnCameraModel.AddedDate = DateTime.Now;
                    model.IndividualProjectViewModel.PmPhnCameraModel.Updated = userId;
                    model.IndividualProjectViewModel.PmPhnCameraModel.UpdatedDate = DateTime.Now;

                    result = _projectManagerRepository.SaveCameraInfo(model.IndividualProjectViewModel.PmPhnCameraModel);

                    if (result > 0)
                    {
                        //====notification====
                        var notificationObject = new NotificationObject
                        {
                            ProjectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId,
                            ToUser = "-1",
                        };
                        notificationObject.Message = " has uploaded Camera image. ";
                        notificationObject.AdditionalMessage = "";
                        ViewBag.ControllerVariable = notificationObject;
                        //==================
                        TempData["Message"] = "Camera Image Save Successfully";
                        TempData["messageType"] = "s";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                    else
                    {
                        TempData["Message"] = "Camera Image Upload Failed";
                        TempData["messageType"] = "e";
                        return RedirectToAction("IndividualProjectManagerDashBoard",
                            new { projectId = model.IndividualProjectViewModel.ProjectMasterModel.ProjectMasterId });
                    }
                }
            }

            return RedirectToAction("IndividualProjectManagerDashBoard");
        }

        [Authorize(Roles = "PM,PMHEAD,ASPM,ASPMHEAD")]
        public ActionResult ProjectForwardToSoftWareView(long projectId = 0)
        {
            var assigendProjectPmInfo = new AssignedProjectListViewModel();

            long userId = Convert.ToInt64(User.Identity.Name);

            assigendProjectPmInfo.CmnUserModel = _projectManagerRepository.GetRoleName(userId);

            var listOfNewProjectOfPm = new List<ProjectMasterModel>();

            if (assigendProjectPmInfo.CmnUserModel.RoleName == "ASPMHEAD" || assigendProjectPmInfo.CmnUserModel.RoleName == "ASPM")
            {
                listOfNewProjectOfPm = _projectManagerRepository.GetProjectMasterModelsByAspm();
                assigendProjectPmInfo.ProjectMasterModels = listOfNewProjectOfPm;
            }
            else
            {
                listOfNewProjectOfPm = _projectManagerRepository.GetProjectMasterModelsByProjectManager(userId);
                assigendProjectPmInfo.ProjectMasterModels = listOfNewProjectOfPm;
            }

            List<SwQcTestPhaseModel> testPhaseList = _projectManagerRepository.GetSwQcTestPhasesForPm();
            ViewBag.ddlTestPhasesList = testPhaseList;

            var user = _projectManagerRepository.GetPmUserInfo(userId);
            assigendProjectPmInfo.UserName = user.UserFullName;
            assigendProjectPmInfo.UserId = user.CmnUserId;


            if (projectId > 0)
            {
                assigendProjectPmInfo.IndividualProjectViewModel.ProjectMasterModel = _commercialRepository.GetProjectMasterModelForPm(projectId) ?? new ProjectMasterModel();
                assigendProjectPmInfo.ProjectPmAssignModel = _projectManagerRepository.GetPmAssignInfo(projectId) ??
                                                             new ProjectPmAssignModel();

                ViewBag.SwQcHeadAssignInfoForPm =
                     _projectManagerRepository.GetSwQcHeadAssignInfoForPm(projectId) ?? new List<SwQcHeadAssignsFromPmModel>();

                ViewBag.OsAssignInfoForPm =
                   _projectManagerRepository.GetOsAssignInfoForPm(projectId) ?? new List<SwQcHeadAssignsFromPmModel>();

                var fileManager = new FileManager();
                if (ViewBag.OsAssignInfoForPm != null)
                {
                    foreach (var model in ViewBag.OsAssignInfoForPm)
                    {
                        if (model.SupportingDocument != null)
                        {
                            var urls = model.SupportingDocument;

                            FilesDetail detail = new FilesDetail();
                            detail.FilePath = fileManager.GetFile(urls);
                            detail.Extention = fileManager.GetExtension(urls);
                            model.FilesDetails.Add(detail);
                        }
                    }
                }
                //
                ViewBag.SwQcAssignsFromQcHeadInfo =
                  _projectManagerRepository.GetSwQcHeadToQcAssignInfo(projectId) ?? new List<SwQcAssignsFromQcHeadModel>();

                ViewBag.SwQcAccessoriesAssign =
              _projectManagerRepository.GetSwQcAccessoriesAssign(projectId) ?? new List<SwQcHeadAssignsFromPmModel>();
                ViewBag.SwQcFieldAssignByPm =
             _projectManagerRepository.GetSwQcFieldAssignBy(projectId) ?? new List<SwQcHeadAssignsFromPmModel>();

            }
            return View(assigendProjectPmInfo);
        }

        [NotificationActionFilter(ReceiverRoles = "QCHEAD,PMHEAD,MM,PS,ASPM,ASPMHEAD")]
        [HttpPost]
        [Authorize(Roles = "PM,PMHEAD,ASPM,ASPMHEAD")]
        public JsonResult ProjectForwardToSoftwareQc(string pmRemarks, string projectMasterId, string projectPmAssignId, string projectManagerUserId, string selectedSampleValue, string sampleNumber, string testPhase, string swVersionNo, string versionName)
        {
            var sWQcInchargeAssignResult = "";
            long userId = Convert.ToInt64(User.Identity.Name);
            long swWcInchargeAssignUserId = _projectManagerRepository.GetUserIdByRoleName("QCHEAD");
            long pMasterId, pMAssignId, pmUserId, sampleNo, testPhasefrPm, swVersionNumber;
            long.TryParse(projectMasterId, out pMasterId);
            long.TryParse(projectPmAssignId, out pMAssignId);
            long.TryParse(projectManagerUserId, out pmUserId);
            long.TryParse(sampleNumber, out sampleNo);
            long.TryParse(testPhase, out testPhasefrPm);
            long.TryParse(swVersionNo, out swVersionNumber);

            var checkDuplicateAssign = _projectManagerRepository.CheckSwQcInchargeDuplicateAssign(pMasterId);

            sWQcInchargeAssignResult = _projectManagerRepository.AssignProjectPmToSwQcHead(pmRemarks, pMasterId, pMAssignId, pmUserId, selectedSampleValue, sampleNo, userId, swWcInchargeAssignUserId, testPhasefrPm, swVersionNumber, versionName);

            //----------MAIL Start-----------------------
            var project = _projectManagerRepository.GetProjectMasterModel(pMasterId);
            var user = _projectManagerRepository.GetPmUserInfo(userId);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<string>(new[] { "QCHEAD" }), new List<string>(new[] { "MM", "PMHEAD", "CM", "SA", "PS" }), "New Project Forwarded for Software QC",
                "Project : <b>" + project.ProjectName + "</b> Forwarded for QC from ProjectManager to Software QC.<br/>Forwarded By : " + user.UserFullName
                + "<br/>Sample Type : " + selectedSampleValue + "<br/>Sample No : " + sampleNo + "<br/>Software Version Name : " + versionName + "<br/>Software Version No. : " + swVersionNo);
            //---------------ends-----------------

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
        //Os Requirement Analysis
        [HttpPost]
        [Authorize(Roles = "PM,PMHEAD,ASPM,ASPMHEAD")]
        public JsonResult SaveOsRequirementAnalysisData(string pmRemarks, string projectMasterId, string projectPmAssignId, string projectManagerUserId)
        {
            var sWQcInchargeAssignResult = "";
            long userId = Convert.ToInt64(User.Identity.Name);
            long swWcInchargeAssignUserId = _projectManagerRepository.GetUserIdByRoleName("QCHEAD");
            long pMasterId, pMAssignId, pmUserId;
            long.TryParse(projectMasterId, out pMasterId);
            long.TryParse(projectPmAssignId, out pMAssignId);
            long.TryParse(projectManagerUserId, out pmUserId);
           
            //var checkDuplicateAssign = _projectManagerRepository.CheckSwQcInchargeDuplicateAssign(pMasterId);

            sWQcInchargeAssignResult = _projectManagerRepository.AssignOsRequirementToSwQcHead(pmRemarks, pMasterId, pMAssignId, pmUserId, userId, swWcInchargeAssignUserId);

            return Json(sWQcInchargeAssignResult, JsonRequestBehavior.AllowGet);
        }

        [NotificationActionFilter(ReceiverRoles = "QCHEAD,PMHEAD,MM,PS,ASPM,ASPMHEAD")]
        [HttpPost]
        [Authorize(Roles = "PM,PMHEAD,ASPM,ASPMHEAD")]
        public JsonResult FieldOrAccessoriesForwardToSoftwareQc(string pmRemarks, string projectMasterId, string projectPmAssignId, string projectManagerUserId, string sampleNumber, string testPhase, string swVersionNo, string versionName, string accessoriesTest)
        {
            var sWQcInchargeAssignResult = "";
            long userId = Convert.ToInt64(User.Identity.Name);
            long swWcInchargeAssignUserId = _projectManagerRepository.GetUserIdByRoleName("QCHEAD");
            long pMasterId, pMAssignId, pmUserId, sampleNo, testPhasefrPm, swVersionNumber;
            long.TryParse(projectMasterId, out pMasterId);
            long.TryParse(projectPmAssignId, out pMAssignId);
            long.TryParse(projectManagerUserId, out pmUserId);
            long.TryParse(sampleNumber, out sampleNo);
            long.TryParse(testPhase, out testPhasefrPm);
            long.TryParse(swVersionNo, out swVersionNumber);


            sWQcInchargeAssignResult = _projectManagerRepository.AssignFieldAccessoriesPmToSwQcHead(pmRemarks, pMasterId, pMAssignId, pmUserId, sampleNo, userId, swWcInchargeAssignUserId, testPhasefrPm, swVersionNumber, versionName, accessoriesTest);

            //----------MAIL Start-----------------------
            var project = _projectManagerRepository.GetProjectMasterModel(pMasterId);
            var user = _projectManagerRepository.GetPmUserInfo(userId);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<string>(new[] { "QCHEAD" }), new List<string>(new[] { "MM", "PMHEAD", "CM", "SA", "PS" }), "New Project Forwarded for Software QC",
                "Project : <b>" + project.ProjectName + "</b> Forwarded for QC from ProjectManager to Software QC.<br/>Forwarded By : " + user.UserFullName
                + "<br/>Sample No : " + sampleNo + "<br/>Software Version Name : " + versionName + "<br/>Software Version No. : " + swVersionNo);
            //---------------ends-----------------

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

        [Authorize(Roles = "PM,PMHEAD")]
        public ActionResult ProjectForwardToHardWareView(long projectId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var listOfNewProjectOfPm = _projectManagerRepository.GetProjectMasterModelsByProjectManager(userId);
            var assigendProjectPmInfo = new AssignedProjectListViewModel();
            assigendProjectPmInfo.ProjectMasterModels = listOfNewProjectOfPm;
            var user = _projectManagerRepository.GetPmUserInfo(userId);
            assigendProjectPmInfo.UserName = user.UserFullName;
            assigendProjectPmInfo.UserId = user.CmnUserId;


            if (projectId > 0)
            {

                assigendProjectPmInfo.IndividualProjectViewModel.ProjectMasterModel = _commercialRepository.GetProjectMasterModelForPm(projectId) ?? new ProjectMasterModel();
                assigendProjectPmInfo.ProjectPmAssignModel = _projectManagerRepository.GetPmAssignInfo(projectId) ??
                                                             new ProjectPmAssignModel();
                assigendProjectPmInfo.PmViewHwTestHybridModel =
                    _projectManagerRepository.GetPmViewHwTestHybridModelForScreening(projectId);
                ViewBag.PmViewHwTestHybridModelForRunning =
                    _projectManagerRepository.GetPmViewHwTestHybridModelForRunning(projectId);
                ViewBag.PmViewHwTestHybridModelForFinished =
                    _projectManagerRepository.GetPmViewHwTestHybridModelForFinished(projectId);
                assigendProjectPmInfo.HwQcInchargeAssignModels =
                    _projectManagerRepository.GetHwQcInchargeAssignInfo(projectId);
                assigendProjectPmInfo.ProjectPurchaseOrderFormModels =
                    _commercialRepository.GetUnclosedPoList(projectId);
            }
            return View(assigendProjectPmInfo);
        }

        [NotificationActionFilter(ReceiverRoles = "HWHEAD,PMHEAD,MM,PS")]
        [HttpPost]
        [Authorize(Roles = "PM,PMHEAD")]
        public JsonResult ProjectForwardToHardWare(string pmRemarks, string projectMasterId, string projectPmAssignId, string projectManagerUserId, string selectedSampleValue, string sampleNumber, string runningTestValue, string finisTestValue, string selectedPoNumber)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            long pMasterId, pMAssignId, pmUserId, sampleNo, poNumber;
            long.TryParse(projectMasterId, out pMasterId);
            long.TryParse(projectPmAssignId, out pMAssignId);
            long.TryParse(projectManagerUserId, out pmUserId);
            long.TryParse(sampleNumber, out sampleNo);
            //long.TryParse(selectedPoNumber, out poNumber);
            var hWQcInchargeAssignResult = "";
            var testInProgress = _projectManagerRepository.CheckDuplicateAssignToHardware(pMasterId, runningTestValue,
                finisTestValue);

            //----------mail-------------
            var project = _projectManagerRepository.GetProjectMasterModel(pMasterId);
            var user = _projectManagerRepository.GetPmUserInfo(userId);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            //--------end---------------
            if (testInProgress == "0")
            {
                hWQcInchargeAssignResult = _projectManagerRepository.AssignProjectToHardWare(pmRemarks, pMasterId, pMAssignId, pmUserId, selectedSampleValue, sampleNo, userId, runningTestValue, finisTestValue, selectedPoNumber);

                if (runningTestValue == "1")
                {
                    //----------MAIL Start-----------------------
                    mailSendFromPms.SendMail(new List<string>(new[] { "HWHEAD" }), new List<string>(new[] { "MM", "PMHEAD", "CM", "SA", "PS" }), "New Project Forwarded for Running Test",
                        "Project : <b>" + project.ProjectName + "</b> Forwarded for Running Test from ProjectManager to Hardware.<br/>Forwarded By : " + user.UserFullName
                        + "<br/>Sample Type : " + selectedSampleValue + "<br/>Sample Quantity : " + sampleNo);
                    //---------------ends-----------------
                    var notificationObject = new NotificationObject
                    {

                        ProjectId = pMasterId,
                        ToUser = "-1",
                    };
                    notificationObject.Message = "  forwarded a project for Running test ";
                    notificationObject.AdditionalMessage = "";
                    ViewBag.ControllerVariable = notificationObject;
                }
                if (finisTestValue == "1")
                {
                    //----------MAIL Start-----------------------
                    mailSendFromPms.SendMail(new List<string>(new[] { "HWHEAD" }), new List<string>(new[] { "MM", "PMHEAD", "CM", "SA" }), "New Project Forwarded for Finished goods Test",
                        "Project : <b>" + project.ProjectName + "</b> Forwarded for Finished goods Test from ProjectManager to Hardware.<br/>Forwarded By : " + user.UserFullName
                        + "<br/>Sample Type : " + selectedSampleValue + "<br/>Sample Quantity : " + sampleNo);
                    //---------------ends-----------------
                    var notificationObject = new NotificationObject
                    {

                        ProjectId = pMasterId,
                        ToUser = "-1",
                        Message = "  forwarded a project for Finished goods test ",
                        AdditionalMessage = "",
                    };
                    ViewBag.ControllerVariable = notificationObject;
                }
            }
            else
            {
                hWQcInchargeAssignResult = testInProgress;
            }
            return Json(hWQcInchargeAssignResult, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "PM")]
        public ActionResult PmSwCustomizationCategoryCreateView()
        {
            return View();
        }


        [HttpGet]
        [Authorize(Roles = "PM")]
        public ActionResult PmtoBtrcNocRequest(long projectId = 0, long orderId = 0, string imei = null)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var listOfNewProjectOfPm = _projectManagerRepository.GetProjectMasterModelsByProjectManager(userId);
            var pmTobtrcNocRequest = new VmPmToBtrcNocRequest();
            pmTobtrcNocRequest.ProjectMasterModel = listOfNewProjectOfPm;
            //var user = _projectManagerRepository.GetPmUserInfo(userId);
            if (projectId > 0 && orderId > 0)
            {
                pmTobtrcNocRequest.ProjectMasterId = projectId;
                pmTobtrcNocRequest.ProjectBtrcNocModel = _projectManagerRepository.GetProjectBtrcNoc(projectId, orderId, imei);
                pmTobtrcNocRequest.FilesWebServerPaths = _projectManagerRepository.GetFilesServerPaths(pmTobtrcNocRequest.ProjectBtrcNocModel.ProjectBrtcNocId);
                var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select One" } };
                var pos = _commercialRepository.GetProjectOrderModels(projectId);
                selectListItems.AddRange(pos.Select(p => new SelectListItem { Value = p.ProjectPurchaseOrderFormId.ToString(CultureInfo.InvariantCulture), Text = p.PurchaseOrderNumber + " -- " + p.PoDate }));
                ViewBag.ProjectOrders = selectListItems;
            }
            else if (projectId > 0)
            {
                pmTobtrcNocRequest.ProjectMasterId = projectId;
                pmTobtrcNocRequest.ProjectBtrcNocModel = new ProjectBtrcNocModel();
                pmTobtrcNocRequest.FilesWebServerPaths = new List<FileShowModel>();
                var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select One" } };
                var pos = _commercialRepository.GetProjectOrderModels(projectId);
                selectListItems.AddRange(pos.Select(p => new SelectListItem { Value = p.ProjectPurchaseOrderFormId.ToString(CultureInfo.InvariantCulture), Text = p.PurchaseOrderNumber + " -- " + p.PoDate }));
                ViewBag.ProjectOrders = selectListItems;
            }
            else if (projectId == 0)
            {
                return RedirectToAction("NocRequest");
            }
            return View(pmTobtrcNocRequest);
        }

        [HttpPost]//For generating multiple file attachment
        public ActionResult GetFile(long projectId, string imei, long nocId, long poId)
        {
            var btrcToNoc = new VmPmToBtrcNocRequest();
            btrcToNoc.ProjectMasterId = projectId;
            btrcToNoc.ProjectBtrcNocModel.FinalSampleImei = imei;
            btrcToNoc.ProjectBtrcNocModel.ProjectBrtcNocId = nocId;
            btrcToNoc.ProjectBtrcNocModel.ProjectPurchaseOrderFormId = poId;
            return PartialView("~/Views/ProjectManager/Partial/_File.cshtml", btrcToNoc);
        }

        [HttpPost]
        public ActionResult AttachFiles(IEnumerable<VmPmToBtrcNocRequest> attachments)
        {
            var repositoryFiles = _projectManagerRepository.SaveBtrcDocFiles(attachments);
            var dataArray = repositoryFiles.Split('-');
            long masterId;
            long orderId;
            long.TryParse(dataArray[0], out masterId);
            long.TryParse(dataArray[1], out orderId);
            if (masterId > 0)
            {
                return RedirectToAction("PmtoBtrcNocRequest", new { projectId = masterId, orderId = orderId, imei = dataArray[2] });
            }
            return RedirectToAction("NocRequest");
        }

        //noc new codes
        [HttpGet]
        public ActionResult NocRequest(long projectId = 0)
        {
            var model = new VmNocReq { ProjectMasterId = projectId };
            ViewBag.Projects = _commercialRepository.GetAllProjects();
            model.ProjectBtrcNocModels = _projectManagerRepository.GetBtrcNocByProjectId(projectId);
            return View(model);
        }


        [HttpGet]
        public ActionResult GetOtaUpdateRequest()
        {

            long userId =
                Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == " "
                    ? ""
                    : System.Web.HttpContext.Current.User.Identity.Name);

            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);


            var listOfAssignedProjects = _projectManagerRepository.GetProjectMasterModelsByProjectManager(userId);

            var pmOtaUpdateModel = new PmOtaUpdateModel();


            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select One" } };
            ViewBag.Projects = listOfAssignedProjects;

            return View(pmOtaUpdateModel);
        }

        [HttpPost]

        public ActionResult SaveOtaUpdateRequest(PmOtaUpdateModel otaUpdateModel)
        {

            //var mailSend = new MailSendFromPms();
            //mailSend.MailSendByPms("mrahman.cse32@waltonbd.com", "u.salma@waltonbd.com", "Apnara Sobai Pagol",
            //    "Sob Koita Pagol");
            // var smsSendByBl = new SmsSendByBanglalinkFromPms();
            //smsSendByBl.SmsSendByBanglalink("01835099322", "dsfsdfdf");

            if (ModelState.IsValidField("PmOtaUpdateModel"))
            {
                long result = 0;
                long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
                otaUpdateModel.ProjectManagerUserId = userId;
                otaUpdateModel.AddedBy = Convert.ToString(userId);
                otaUpdateModel.AddedDate = DateTime.Now;
                otaUpdateModel.UpdatedBy = Convert.ToString(userId);
                otaUpdateModel.UpdatedDate = DateTime.Now;

                if (otaUpdateModel.PmOtaUpdateId > 0)
                {
                    result = _projectManagerRepository.UpdatePmOtaInfo(otaUpdateModel);

                    if (result > 0)
                    {
                        return RedirectToAction("GetOtaUpdateRequest");

                    }



                }
                else
                {
                    result = _projectManagerRepository.SavePmOtaUpdateInfo(otaUpdateModel);

                    if (result > 0)
                    {

                        return RedirectToAction("GetOtaUpdateRequest");
                    }


                }


            }

            return RedirectToAction("GetOtaUpdateRequest");
        }


        [HttpGet]
        public ActionResult ProjectManagerDashBoard()
        {

            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);

            var counter = new DashBoardCounter();
            ViewBag.Count = counter.GetDashBoardCounter("ProjectManager", "PM", userId);
            return View();
        }

        [HttpGet]

        public ActionResult RunningProjectOfAProjectManager()
        {
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            var runningProjectList = _projectManagerRepository.GetProjectMasterModelsByProjectManager(userId);



            var newProjectAssign = new NewProjectsViewModel { ProjectMasters = runningProjectList };

            return View(newProjectAssign);


        }

        public ActionResult PmHeadHwAndSwSummary(string projectName, long projectMasterId = 0)
        {
            ViewBag.ProjectName = projectName;
            ViewBag.HwFgTests = _projectManagerRepository.GetProjectForHwFgTestByProjectId(projectMasterId);
            ViewBag.HwScreeningTests = _projectManagerRepository.GetProjectForHwScreeningTestByProjectId(projectMasterId);
            ViewBag.HwRunningTests = _projectManagerRepository.GetProjectForHwRunningTestByProjectId(projectMasterId);
            ViewBag.SwQcInchargeAssignModels =
                      _projectManagerRepository.GetSwQcInchargeAssign(projectMasterId) ?? new List<SwQcInchargeAssignModel>();
            return View();
        }


        public FileResult Download(string ImageName)
        {

            return File("" + ImageName, System.Net.Mime.MediaTypeNames.Application.Octet);


        }


        public ActionResult GetAllPmFiles(string projectName, long projectMasterId = 0)
        {
            PmAllFilesModel pmAllFilesModel = new PmAllFilesModel();
            pmAllFilesModel = _projectManagerRepository.GetAllFilesModel(projectMasterId);
            return View(pmAllFilesModel);
        }

        [HttpPost]
        public FileResult GetAllPmFiles(List<string> files, PmAllFilesModel pmAllFilesModel)
        {
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

            return File(archive, "application/zip", DateTime.Now.ToString("dd-MM-yyyy hh:mm ss") + " - PM - " + pmAllFilesModel.ProjectName + ".zip");
        }

        /////////PmReassignFromPmIncharge///////////
        #region PM Delete or Newly assigned by PM Incharge
        [Authorize(Roles = "PMHEAD,SA")]

        [NotificationActionFilter(ReceiverRoles = "PMHEAD,CM,PM,MM,PS")]
        [HttpPost]

        public JsonResult PmReassignFromPmIncharge(string projectMasterId, string approxPmInchargeToPmFinishDate, string pmInchargeDeleteQcComment, string projectHeadRemarks, string multideleteValue, string multiReassignValue, string multideleteID, string poNumber)
        {
            var _dbEntities = new CellPhoneProjectEntities();
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name);
            var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);

            if (userId > 0)
            {
                long pMasterId;
                long.TryParse(projectMasterId, out pMasterId);

                // long poNumbers;
                //long.TryParse(poNumber, out poNumbers);

                //----------MAIL Start-----------------------           
                var proName = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == pMasterId);
                //---------------ends-----------------

                var notificationObject1 = new NotificationObject();
                var notificationObject2 = new NotificationObject();

                if (multideleteID.ToString().Trim() != "null")
                {
                    var multideleteValue1 = multideleteID.Split(',');

                    notificationObject1 = new NotificationObject
                    {
                        Message = "inactivated from the assigned project",
                        AdditionalMessage = pmInchargeDeleteQcComment,
                        ProjectId = pMasterId,
                    };

                    List<long> ids = new List<long>();
                    string body = string.Empty;
                    string deletedQc = string.Empty;

                    foreach (var mId1 in multideleteValue1)
                    {
                        notificationObject1.ToUser = notificationObject1.ToUser + mId1 + ",";
                        long qcIDs;
                        long.TryParse(mId1, out qcIDs);
                        ids.Add(qcIDs);

                        var deletedUserName = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == qcIDs);
                        deletedQc = deletedQc + deletedUserName.UserFullName;
                    }

                    body = "This is to inform you that, <b>" + deletedQc + " </b> has been inactivated by <b>" + user.UserFullName + "</b> from an assigned Project.<br/><br/><br/> <br/>"
                        + "Project : <b>" + proName.ProjectName + "</b> <br/>Sample Quantity - "
                        + proName.NumberOfSample + "<br/>PO number - " + poNumber;

                    var mailSendFromPms = new MailSendFromPms();
                    mailSendFromPms.SendMail(ids, new List<string>(new[] { "MM", "SA", "PS" }), "Project Manager has been inactivated from an assigned project", body);

                }

                if (multiReassignValue.ToString().Trim() != "null")
                {
                    var multiReassignValue1 = multiReassignValue.Split(',');
                    notificationObject2 = new NotificationObject
                    {
                        Message = " assigned for a Project",
                        ProjectId = pMasterId,
                    };

                    List<long> ids = new List<long>();
                    string body = string.Empty;
                    string assignedQc = string.Empty;

                    foreach (var mIdr1 in multiReassignValue1)
                    {
                        notificationObject2.ToUser = notificationObject2.ToUser + mIdr1 + ",";

                        long qcIDs;
                        long.TryParse(mIdr1, out qcIDs);
                        ids.Add(qcIDs);

                        var assignedUserName = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == qcIDs);
                        assignedQc = assignedQc + assignedUserName.UserFullName;
                    }

                    body = "This is to inform you that, <b>" + assignedQc + " </b> has been assigned by <b>" + user.UserFullName + "</b> for a new Project.<br/><br/><br/> <br/>"
                        + "Project : <b>" + proName.ProjectName + "</b> <br/>Sample Quantity - "
                        + proName.NumberOfSample + "<br/>PO number - " + poNumber;

                    var mailSendFromPms = new MailSendFromPms();
                    mailSendFromPms.SendMail(ids, new List<string>(new[] { "MM", "SA", "PS" }), "Project Manager has been assigned for a new Project", body);

                }
                var pmQCDeleteOrNewAssignByQcIncharge = _projectManagerRepository.PmReassignFromPmIncharge(pMasterId, approxPmInchargeToPmFinishDate,
                    pmInchargeDeleteQcComment, projectHeadRemarks, multideleteValue, multiReassignValue, multideleteID, poNumber);

                ViewBag.ControllerVariable = notificationObject1;
                ViewBag.ControllerVariable = notificationObject2;

                return Json(pmQCDeleteOrNewAssignByQcIncharge, JsonRequestBehavior.AllowGet);

            }

            return Json(new { result = "Redirect", url = Url.Action("ProjectManager", "ListofAssignedProject") });
        }

        #endregion
        /////////PmReassignFromPmIncharge///////////

        #region PMHEAD Report Dashboard
        [Authorize(Roles = "PMHEAD,SA")]

        public ActionResult PmReportDashBoard(PmReportDashBoardViewModel model, string startValue = "", string endValue = "", string emplyCode = "", long projectId = 0, long swqcInchargeAsngId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _projectManagerRepository.GetUserInfoByUserId(userId);

            ViewBag.GetActivePmList = _projectManagerRepository.GetActivePmList();
            //ViewBag.GetProjectsForReport = _repository.GetProjectListForReportOfQcIncharge();
            ViewBag.GetAllProjectListDetailsForInchargeReport = _projectManagerRepository.GetAllProjectListDetailsForInchargeReport(startValue, endValue, emplyCode);

            return View(model);
        }


        public ActionResult DetailsOfPmWorkList(long projectId, string poNumber, string emplyCode)
        {

            var fileManager = new FileManager();
            PmReportDashBoardViewModel pmReportDashBoard = new PmReportDashBoardViewModel();
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name);
            ViewBag.UserInfo = _projectManagerRepository.GetUserInfoByUserId(userId);
            if (projectId > 0)
            {
                pmReportDashBoard.PmBootImageAnimationModels = _projectManagerRepository.GetPmBootImageAnimationModelsDetails(projectId, poNumber, emplyCode);
                if (pmReportDashBoard.PmBootImageAnimationModels.Any())
                {


                    foreach (PmBootImageAnimationModel model in pmReportDashBoard.PmBootImageAnimationModels)
                    {

                        if (model.ImageUpload1 != null)
                        {
                            string urls = model.ImageUpload1;
                            model.ImageUpload1 = fileManager.GetFile(urls);
                            model.ImageExtension = fileManager.GetExtension(urls);

                        }

                        if (model.VideoUpload1 != null)
                        {
                            string urls = model.VideoUpload1;
                            model.VideoUpload1 = fileManager.GetFile(urls);
                            model.VideoExtension = fileManager.GetExtension(urls);

                        }

                    }
                }
                pmReportDashBoard.PmGiftBoxModels = _projectManagerRepository.GetPmGiftBoxModelsDetails(projectId, poNumber, emplyCode);
                if (pmReportDashBoard.PmGiftBoxModels.Any())
                {


                    foreach (PmGiftBoxModel model in pmReportDashBoard.PmGiftBoxModels)
                    {

                        if (model.PmGbImageUploadPath != null)
                        {
                            string urls = model.PmGbImageUploadPath;
                            model.PmGbImageUploadPath = fileManager.GetFile(urls);
                            model.GBExtension = fileManager.GetExtension(urls);

                        }

                    }
                }

                pmReportDashBoard.PmLabelsModels = _projectManagerRepository.GetPmLabelsModelsDetails(projectId, poNumber, emplyCode);
                if (pmReportDashBoard.PmLabelsModels.Any())
                {


                    foreach (PmLabelsModel model in pmReportDashBoard.PmLabelsModels)
                    {

                        if (model.PmLabelImageUploadPath != null)
                        {
                            string urls = model.PmLabelImageUploadPath;
                            model.PmLabelImageUploadPath = fileManager.GetFile(urls);
                            model.LabelImageExtension = fileManager.GetExtension(urls);

                        }

                    }
                }

                pmReportDashBoard.PmIdModels = _projectManagerRepository.GetPmIdModelsDetails(projectId, poNumber, emplyCode);
                if (pmReportDashBoard.PmIdModels.Any())
                {


                    foreach (PmIdModel model in pmReportDashBoard.PmIdModels)
                    {

                        if (model.PmFinishingImageUploadPath != null)
                        {
                            string urls = model.PmFinishingImageUploadPath;
                            model.PmFinishingImageUploadPath = fileManager.GetFile(urls);
                            model.FinishingDesignExtension = fileManager.GetExtension(urls);

                        }
                        if (model.PmLogoTypeImageUploadPath != null)
                        {
                            string urls = model.PmLogoTypeImageUploadPath;
                            model.PmLogoTypeImageUploadPath = fileManager.GetFile(urls);
                            model.LogoDesignExtension = fileManager.GetExtension(urls);

                        }
                        if (model.PmModelPrintImageUploadPath != null)
                        {
                            string urls = model.PmModelPrintImageUploadPath;
                            model.PmModelPrintImageUploadPath = fileManager.GetFile(urls);
                            model.ModelPrintDesignExtension = fileManager.GetExtension(urls);

                        }

                    }
                }

                pmReportDashBoard.PmScreenProtectorModels = _projectManagerRepository.GetPmScreenProtectorModelsDetails(projectId, poNumber, emplyCode);
                if (pmReportDashBoard.PmScreenProtectorModels.Any())
                {


                    foreach (PmScreenProtectorModel model in pmReportDashBoard.PmScreenProtectorModels)
                    {

                        if (model.PmScreenProtectorImageUploadPath != null)
                        {
                            string urls = model.PmScreenProtectorImageUploadPath;
                            model.PmScreenProtectorImageUploadPath = fileManager.GetFile(urls);
                            model.SpExtension = fileManager.GetExtension(urls);

                        }

                    }
                }
                pmReportDashBoard.PmWalpaperModels = _projectManagerRepository.GetPmWalpaperModelsDetails(projectId, poNumber, emplyCode);
                if (pmReportDashBoard.PmWalpaperModels.Any())
                {


                    foreach (PmWalpaperModel model in pmReportDashBoard.PmWalpaperModels)
                    {

                        if (model.WalpaperUpload1 != null)
                        {
                            string urls = model.WalpaperUpload1;
                            model.WalpaperUpload1 = fileManager.GetFile(urls);
                            model.W1Extension = fileManager.GetExtension(urls);

                        }
                        if (model.WalpaperUpload2 != null)
                        {
                            string urls = model.WalpaperUpload2;
                            model.WalpaperUpload2 = fileManager.GetFile(urls);
                            model.W2Extension = fileManager.GetExtension(urls);

                        }
                        if (model.WalpaperUpload3 != null)
                        {
                            string urls = model.WalpaperUpload3;
                            model.WalpaperUpload3 = fileManager.GetFile(urls);
                            model.W3Extension = fileManager.GetExtension(urls);

                        }
                        if (model.WalpaperUpload4 != null)
                        {
                            string urls = model.WalpaperUpload4;
                            model.WalpaperUpload4 = fileManager.GetFile(urls);
                            model.W4Extension = fileManager.GetExtension(urls);

                        }
                        if (model.WalpaperUpload5 != null)
                        {
                            string urls = model.WalpaperUpload5;
                            model.WalpaperUpload5 = fileManager.GetFile(urls);
                            model.W5Extension = fileManager.GetExtension(urls);

                        }
                        if (model.WalpaperUpload6 != null)
                        {
                            string urls = model.WalpaperUpload6;
                            model.WalpaperUpload6 = fileManager.GetFile(urls);
                            model.W6Extension = fileManager.GetExtension(urls);

                        }
                        if (model.WalpaperUpload7 != null)
                        {
                            string urls = model.WalpaperUpload7;
                            model.WalpaperUpload7 = fileManager.GetFile(urls);
                            model.W7Extension = fileManager.GetExtension(urls);

                        }
                    }
                }

                pmReportDashBoard.PmSwCustomizationFinalModels = _projectManagerRepository.GetPmSwCustomizationFinalModelsDetails(projectId, poNumber, emplyCode);

                pmReportDashBoard.PmPhnAccessoriesModels = _projectManagerRepository.GetPmPhnAccessoriesModelsDetails(projectId, poNumber, emplyCode);
                if (pmReportDashBoard.PmPhnAccessoriesModels.Any())
                {


                    foreach (PmPhnAccessoriesModel model in pmReportDashBoard.PmPhnAccessoriesModels)
                    {

                        if (model.PmPhnAccessoriesEarphone != null)
                        {
                            string urls = model.PmPhnAccessoriesEarphone;
                            model.PmPhnAccessoriesEarphone = fileManager.GetFile(urls);
                            model.PmPhnAccessoriesEarphoneExtension = fileManager.GetExtension(urls);

                        }
                        if (model.PmPhnAccessoriesUSBCable != null)
                        {
                            string urls = model.PmPhnAccessoriesUSBCable;
                            model.PmPhnAccessoriesUSBCable = fileManager.GetFile(urls);
                            model.PmPhnAccessoriesUSBCableExtension = fileManager.GetExtension(urls);

                        }
                        if (model.PmPhnAccessoriesCharger != null)
                        {
                            string urls = model.PmPhnAccessoriesCharger;
                            model.PmPhnAccessoriesCharger = fileManager.GetFile(urls);
                            model.PmPhnAccessoriesChargerExtension = fileManager.GetExtension(urls);

                        }
                        if (model.PmPhnAccessoriesOTGCable != null)
                        {
                            string urls = model.PmPhnAccessoriesOTGCable;
                            model.PmPhnAccessoriesOTGCable = fileManager.GetFile(urls);
                            model.PmPhnAccessoriesOTGCableExtension = fileManager.GetExtension(urls);

                        }
                        if (model.PmPhnAccessoriesBackCover != null)
                        {
                            string urls = model.PmPhnAccessoriesBackCover;
                            model.PmPhnAccessoriesBackCover = fileManager.GetFile(urls);
                            model.PmPhnAccessoriesBackCoverExtension = fileManager.GetExtension(urls);

                        }

                        if (model.PmPhnAccessoriesFlipCover != null)
                        {
                            string urls = model.PmPhnAccessoriesFlipCover;
                            model.PmPhnAccessoriesFlipCover = fileManager.GetFile(urls);
                            model.PmPhnAccessoriesFlipCoverExtension = fileManager.GetExtension(urls);

                        }
                    }
                }

                pmReportDashBoard.PmPhnCameraModels = _projectManagerRepository.GetPmPhnCameraModelsDetails(projectId, poNumber, emplyCode);

            }
            return View(pmReportDashBoard);
        }


        //public ActionResult DetailsOfSwQcTestCase(long projectId, long swqcInchargeAsngId, string emplyCode)
        //{
        //    var fileManager = new FileManager();
        //    var vmSwInchargemodel = new VmSwInchargeViewModel();
        //    long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name);
        //    ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
        //    if (projectId > 0)
        //    {
        //        vmSwInchargemodel.ProjectMasterModel = _commercialRepository.GetProjectMasterModel(projectId);
        //        vmSwInchargemodel.SwQcStartUpModels = _repository.GetStartUpsForDetailsReport(projectId, swqcInchargeAsngId, emplyCode);
        //        if (vmSwInchargemodel.SwQcStartUpModels.Any())
        //        {


        //            foreach (SwQcStartUpModel model in vmSwInchargemodel.SwQcStartUpModels)
        //            {
        //                if (model.UploadedFile != null)
        //                {
        //                    var urls = model.UploadedFile.Split('|').ToList();
        //                    for (int i = 0; i < urls.Count; i++)
        //                    {
        //                        FilesDetail detail = new FilesDetail();
        //                        detail.FilePath = fileManager.GetFile(urls[i]);
        //                        detail.Extention = fileManager.GetExtension(urls[i]);

        //                        model.FilesDetails.Add(detail);

        //                    }

        //                }

        //            }
        //        }
        //        vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcStartUpModels.Any() &&
        //                                   vmSwInchargemodel.SwQcStartUpModels[0].SwQcStartUpId > 0;
        //        ///////////////SwQcCallSettingModel/////
        //        vmSwInchargemodel.SwQcCallSettingModels = _repository.GetCallSettingForDetailsReport(projectId, swqcInchargeAsngId, emplyCode);
        //        if (vmSwInchargemodel.SwQcCallSettingModels.Any())
        //        {

        //            foreach (SwQcCallSettingModel model in vmSwInchargemodel.SwQcCallSettingModels)
        //            {

        //                if (model.UploadedFile != null)
        //                {
        //                    var urls = model.UploadedFile.Split('|').ToList();
        //                    for (int i = 0; i < urls.Count; i++)
        //                    {
        //                        FilesDetailForCall detail = new FilesDetailForCall();
        //                        detail.FilePath = fileManager.GetFile(urls[i]);
        //                        detail.Extention = fileManager.GetExtension(urls[i]);

        //                        model.FilesDetails.Add(detail);

        //                    }

        //                }

        //            }
        //        }
        //        vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcCallSettingModels.Any() &&
        //                                   vmSwInchargemodel.SwQcCallSettingModels[0].SwQcCallSettingId > 0;

        //        ///////////////SwQcMessageModel/////
        //        vmSwInchargemodel.SwQcMessageModels = _repository.GetMessageForDetailsReport(projectId, swqcInchargeAsngId, emplyCode);
        //        if (vmSwInchargemodel.SwQcMessageModels.Any())
        //        {


        //            foreach (SwQcMessageModel model in vmSwInchargemodel.SwQcMessageModels)
        //            {

        //                if (model.UploadedFile != null)
        //                {
        //                    var urls = model.UploadedFile.Split('|').ToList();
        //                    for (int i = 0; i < urls.Count; i++)
        //                    {
        //                        FilesDetailForMessage detail = new FilesDetailForMessage();
        //                        detail.FilePath = fileManager.GetFile(urls[i]);
        //                        detail.Extention = fileManager.GetExtension(urls[i]);

        //                        model.FilesDetails.Add(detail);

        //                    }

        //                }

        //            }
        //        }
        //        vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcMessageModels.Any() &&
        //                                   vmSwInchargemodel.SwQcMessageModels[0].SwQcMassageId > 0;

        //        /////////////SwQcToolsCheckModel/////
        //        vmSwInchargemodel.SwQcToolsCheckModels = _repository.GetToolsForDetailsReport(projectId, swqcInchargeAsngId, emplyCode);
        //        if (vmSwInchargemodel.SwQcToolsCheckModels.Any())
        //        {
        //            foreach (SwQcToolsCheckModel model in vmSwInchargemodel.SwQcToolsCheckModels)
        //            {
        //                if (model.UploadedFile != null)
        //                {
        //                    var urls = model.UploadedFile.Split('|').ToList();
        //                    for (int i = 0; i < urls.Count; i++)
        //                    {
        //                        FilesDetailForTools detail = new FilesDetailForTools();
        //                        detail.FilePath = fileManager.GetFile(urls[i]);
        //                        detail.Extention = fileManager.GetExtension(urls[i]);

        //                        model.FilesDetails.Add(detail);

        //                    }

        //                }

        //            }
        //        }
        //        vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcToolsCheckModels.Any() &&
        //                                   vmSwInchargemodel.SwQcToolsCheckModels[0].SwQcToolsCheckId > 0;

        //        /////////////SwQcCameraModel/////
        //        vmSwInchargemodel.SwQcCameraModels = _repository.GetCameraForDetailsReport(projectId, swqcInchargeAsngId, emplyCode);
        //        if (vmSwInchargemodel.SwQcCameraModels.Any())
        //        {
        //            foreach (SwQcCameraModel model in vmSwInchargemodel.SwQcCameraModels)
        //            {
        //                if (model.UploadedFile != null)
        //                {
        //                    var urls = model.UploadedFile.Split('|').ToList();
        //                    for (int i = 0; i < urls.Count; i++)
        //                    {
        //                        FilesDetailForCamera detail = new FilesDetailForCamera();
        //                        detail.FilePath = fileManager.GetFile(urls[i]);
        //                        detail.Extention = fileManager.GetExtension(urls[i]);

        //                        model.FilesDetails.Add(detail);

        //                    }

        //                }

        //            }
        //        }
        //        vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcCameraModels.Any() &&
        //                                   vmSwInchargemodel.SwQcCameraModels[0].SwQcCameraId > 0;
        //        /////////////SwQcDisplayLoopModel/////
        //        vmSwInchargemodel.SwQcDisplayLoopModels = _repository.GetDisplayLoopForDetailsReport(projectId, swqcInchargeAsngId, emplyCode);
        //        if (vmSwInchargemodel.SwQcDisplayLoopModels.Any())
        //        {
        //            foreach (SwQcDisplayLoopModel model in vmSwInchargemodel.SwQcDisplayLoopModels)
        //            {
        //                if (model.UploadedFile != null)
        //                {
        //                    var urls = model.UploadedFile.Split('|').ToList();
        //                    for (int i = 0; i < urls.Count; i++)
        //                    {
        //                        FilesDetailForDisplayLoop detail = new FilesDetailForDisplayLoop();
        //                        detail.FilePath = fileManager.GetFile(urls[i]);
        //                        detail.Extention = fileManager.GetExtension(urls[i]);

        //                        model.FilesDetails.Add(detail);

        //                    }

        //                }

        //            }
        //        }
        //        vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcDisplayLoopModels.Any() &&
        //                                   vmSwInchargemodel.SwQcDisplayLoopModels[0].SwQcDisplayLoopId > 0;

        //        /////////////SwQcDisplayModel/////
        //        vmSwInchargemodel.SwQcDisplayModels = _repository.GetDisplayForDetailsReport(projectId, swqcInchargeAsngId, emplyCode);
        //        if (vmSwInchargemodel.SwQcDisplayModels.Any())
        //        {
        //            foreach (SwQcDisplayModel model in vmSwInchargemodel.SwQcDisplayModels)
        //            {
        //                if (model.UploadedFile != null)
        //                {
        //                    var urls = model.UploadedFile.Split('|').ToList();
        //                    for (int i = 0; i < urls.Count; i++)
        //                    {
        //                        FilesDetailForDisplay detail = new FilesDetailForDisplay();
        //                        detail.FilePath = fileManager.GetFile(urls[i]);
        //                        detail.Extention = fileManager.GetExtension(urls[i]);

        //                        model.FilesDetails.Add(detail);

        //                    }

        //                }

        //            }
        //        }
        //        vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcDisplayModels.Any() &&
        //                                   vmSwInchargemodel.SwQcDisplayModels[0].SwQcDisplayId > 0;
        //        /////////////SwQcSettingModel/////
        //        vmSwInchargemodel.SwQcSettingModels = _repository.GetSettingForDetailsReport(projectId, swqcInchargeAsngId, emplyCode);
        //        if (vmSwInchargemodel.SwQcSettingModels.Any())
        //        {
        //            foreach (SwQcSettingModel model in vmSwInchargemodel.SwQcSettingModels)
        //            {
        //                if (model.UploadedFile != null)
        //                {
        //                    var urls = model.UploadedFile.Split('|').ToList();
        //                    for (int i = 0; i < urls.Count; i++)
        //                    {
        //                        FilesDetailForSetting detail = new FilesDetailForSetting();
        //                        detail.FilePath = fileManager.GetFile(urls[i]);
        //                        detail.Extention = fileManager.GetExtension(urls[i]);

        //                        model.FilesDetails.Add(detail);

        //                    }

        //                }

        //            }
        //        }
        //        vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcSettingModels.Any() &&
        //                                   vmSwInchargemodel.SwQcSettingModels[0].SwQcSettingsId > 0;
        //        /////////////SwQcMultimediaModel/////
        //        vmSwInchargemodel.SwQcMultimediaModels = _repository.GetMultimediaForDetailsReport(projectId, swqcInchargeAsngId, emplyCode);
        //        if (vmSwInchargemodel.SwQcMultimediaModels.Any())
        //        {
        //            foreach (SwQcMultimediaModel model in vmSwInchargemodel.SwQcMultimediaModels)
        //            {
        //                if (model.UploadedFile != null)
        //                {
        //                    var urls = model.UploadedFile.Split('|').ToList();
        //                    for (int i = 0; i < urls.Count; i++)
        //                    {
        //                        FilesDetailForMultimedia detail = new FilesDetailForMultimedia();
        //                        detail.FilePath = fileManager.GetFile(urls[i]);
        //                        detail.Extention = fileManager.GetExtension(urls[i]);

        //                        model.FilesDetails.Add(detail);

        //                    }

        //                }

        //            }
        //        }
        //        vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcMultimediaModels.Any() &&
        //                                   vmSwInchargemodel.SwQcMultimediaModels[0].SwQcMultimediaId > 0;
        //        ///////////////SwQcGoogleServiceModel/////
        //        vmSwInchargemodel.SwQcGoogleServiceModels = _repository.GetGoogleServiceForDetailsReport(projectId, swqcInchargeAsngId, emplyCode);
        //        if (vmSwInchargemodel.SwQcGoogleServiceModels.Any())
        //        {
        //            foreach (SwQcGoogleServiceModel model in vmSwInchargemodel.SwQcGoogleServiceModels)
        //            {
        //                if (model.UploadedFile != null)
        //                {
        //                    var urls = model.UploadedFile.Split('|').ToList();
        //                    for (int i = 0; i < urls.Count; i++)
        //                    {
        //                        FilesDetailForGoogleService detail = new FilesDetailForGoogleService();
        //                        detail.FilePath = fileManager.GetFile(urls[i]);
        //                        detail.Extention = fileManager.GetExtension(urls[i]);

        //                        model.FilesDetails.Add(detail);

        //                    }

        //                }

        //            }
        //        }
        //        vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcGoogleServiceModels.Any() &&
        //                                   vmSwInchargemodel.SwQcGoogleServiceModels[0].SwQcGoogleServicesId > 0;

        //        ///////////////SwQcStorageCheckModel/////
        //        vmSwInchargemodel.SwQcStorageCheckModels = _repository.GetStorageCheckForDetailsReport(projectId, swqcInchargeAsngId, emplyCode);
        //        if (vmSwInchargemodel.SwQcStorageCheckModels.Any())
        //        {
        //            foreach (SwQcStorageCheckModel model in vmSwInchargemodel.SwQcStorageCheckModels)
        //            {
        //                if (model.UploadedFile != null)
        //                {
        //                    var urls = model.UploadedFile.Split('|').ToList();
        //                    for (int i = 0; i < urls.Count; i++)
        //                    {
        //                        FilesDetailForStorage detail = new FilesDetailForStorage();
        //                        detail.FilePath = fileManager.GetFile(urls[i]);
        //                        detail.Extention = fileManager.GetExtension(urls[i]);

        //                        model.FilesDetails.Add(detail);

        //                    }

        //                }

        //            }
        //        }
        //        vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcStorageCheckModels.Any() &&
        //                                   vmSwInchargemodel.SwQcStorageCheckModels[0].SwQcStorageCheckId > 0;
        //        ///////////////SwQcGameModel/////
        //        vmSwInchargemodel.SwQcGameModels = _repository.GetGameForDetailsReport(projectId, swqcInchargeAsngId, emplyCode);
        //        if (vmSwInchargemodel.SwQcGameModels.Any())
        //        {
        //            foreach (SwQcGameModel model in vmSwInchargemodel.SwQcGameModels)
        //            {
        //                if (model.UploadedFile != null)
        //                {
        //                    var urls = model.UploadedFile.Split('|').ToList();
        //                    for (int i = 0; i < urls.Count; i++)
        //                    {
        //                        FilesDetailForGame detail = new FilesDetailForGame();
        //                        detail.FilePath = fileManager.GetFile(urls[i]);
        //                        detail.Extention = fileManager.GetExtension(urls[i]);

        //                        model.FilesDetails.Add(detail);

        //                    }

        //                }

        //            }
        //        }
        //        vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcGameModels.Any() &&
        //                                   vmSwInchargemodel.SwQcGameModels[0].SwQcGameId > 0;

        //        ///////////////SwQcTestingAppModel/////
        //        vmSwInchargemodel.SwQcTestingAppModels = _repository.GetTestingAppForDetailsReport(projectId, swqcInchargeAsngId, emplyCode);
        //        if (vmSwInchargemodel.SwQcTestingAppModels.Any())
        //        {
        //            foreach (SwQcTestingAppModel model in vmSwInchargemodel.SwQcTestingAppModels)
        //            {
        //                if (model.UploadedFile != null)
        //                {
        //                    var urls = model.UploadedFile.Split('|').ToList();
        //                    for (int i = 0; i < urls.Count; i++)
        //                    {
        //                        FilesDetailForTesting detail = new FilesDetailForTesting();
        //                        detail.FilePath = fileManager.GetFile(urls[i]);
        //                        detail.Extention = fileManager.GetExtension(urls[i]);

        //                        model.FilesDetails.Add(detail);

        //                    }

        //                }

        //            }
        //        }
        //        vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcTestingAppModels.Any() &&
        //                                   vmSwInchargemodel.SwQcTestingAppModels[0].SwQcTestingAppId > 0;

        //        ///////////////SwQcFileManagerModel/////
        //        vmSwInchargemodel.SwQcFileManagerModels = _repository.GetFileManageForDetailsReport(projectId, swqcInchargeAsngId, emplyCode);
        //        if (vmSwInchargemodel.SwQcFileManagerModels.Any())
        //        {
        //            foreach (SwQcFileManagerModel model in vmSwInchargemodel.SwQcFileManagerModels)
        //            {
        //                if (model.UploadedFile != null)
        //                {
        //                    var urls = model.UploadedFile.Split('|').ToList();
        //                    for (int i = 0; i < urls.Count; i++)
        //                    {
        //                        FilesDetailForFileManager detail = new FilesDetailForFileManager();
        //                        detail.FilePath = fileManager.GetFile(urls[i]);
        //                        detail.Extention = fileManager.GetExtension(urls[i]);

        //                        model.FilesDetails.Add(detail);

        //                    }

        //                }

        //            }
        //        }
        //        vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcFileManagerModels.Any() &&
        //                                   vmSwInchargemodel.SwQcFileManagerModels[0].SwQcFileManagerId > 0;

        //        ///////////////SwQcConnectivityModel/////
        //        vmSwInchargemodel.SwQcConnectivityModels = _repository.GetConnectivityForDetailsReport(projectId, swqcInchargeAsngId, emplyCode);
        //        if (vmSwInchargemodel.SwQcConnectivityModels.Any())
        //        {
        //            foreach (SwQcConnectivityModel model in vmSwInchargemodel.SwQcConnectivityModels)
        //            {
        //                if (model.UploadedFile != null)
        //                {
        //                    var urls = model.UploadedFile.Split('|').ToList();
        //                    for (int i = 0; i < urls.Count; i++)
        //                    {
        //                        FilesDetailForConnectivity detail = new FilesDetailForConnectivity();
        //                        detail.FilePath = fileManager.GetFile(urls[i]);
        //                        detail.Extention = fileManager.GetExtension(urls[i]);

        //                        model.FilesDetails.Add(detail);

        //                    }

        //                }

        //            }
        //        }
        //        vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcConnectivityModels.Any() &&
        //                                   vmSwInchargemodel.SwQcConnectivityModels[0].SwQcConnectivityId > 0;
        //        ///////////////SwQcShutDownModel/////
        //        vmSwInchargemodel.SwQcShutDownModels = _repository.GetShutDownForDetailsReport(projectId, swqcInchargeAsngId, emplyCode);
        //        if (vmSwInchargemodel.SwQcShutDownModels.Any())
        //        {
        //            foreach (SwQcShutDownModel model in vmSwInchargemodel.SwQcShutDownModels)
        //            {
        //                if (model.UploadedFile != null)
        //                {
        //                    var urls = model.UploadedFile.Split('|').ToList();
        //                    for (int i = 0; i < urls.Count; i++)
        //                    {
        //                        FilesDetailForShutDown detail = new FilesDetailForShutDown();
        //                        detail.FilePath = fileManager.GetFile(urls[i]);
        //                        detail.Extention = fileManager.GetExtension(urls[i]);

        //                        model.FilesDetails.Add(detail);

        //                    }

        //                }

        //            }
        //        }
        //        vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcShutDownModels.Any() &&
        //                                   vmSwInchargemodel.SwQcShutDownModels[0].SwQcShutDownId > 0;

        //        ///////////////SwQcProjectWiseIssueViewModel/////
        //        vmSwInchargemodel.SwQcProjectWiseIssueViewModels = _repository.GetProjectWiseIssueViewModelsForDetailsReport(projectId, swqcInchargeAsngId, emplyCode);
        //        if (vmSwInchargemodel.SwQcProjectWiseIssueViewModels.Any())
        //        {
        //            foreach (SwQcProjectWiseIssueViewModel model in vmSwInchargemodel.SwQcProjectWiseIssueViewModels)
        //            {
        //                if (model.UploadedFile != null)
        //                {
        //                    var urls = model.UploadedFile.Split('|').ToList();
        //                    for (int i = 0; i < urls.Count; i++)
        //                    {
        //                        FilesDetailForSwQcProjectWise detail = new FilesDetailForSwQcProjectWise();
        //                        detail.FilePath = fileManager.GetFile(urls[i]);
        //                        detail.Extention = fileManager.GetExtension(urls[i]);

        //                        model.FilesDetails.Add(detail);

        //                    }

        //                }

        //            }
        //        }
        //        vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcProjectWiseIssueViewModels.Any() &&
        //                                   vmSwInchargemodel.SwQcProjectWiseIssueViewModels[0].SwQcProjectWiseIssueId > 0;

        //    }
        //    return View(vmSwInchargemodel);
        //}


        #endregion

        #region Pm Own Report DashBoard
        [Authorize(Roles = "PM,SA")]

        public ActionResult PmOwnReportDashBoard(PmReportDashBoardViewModel model, string startValue = "", string endValue = "", long projectId = 0, long swqcInchargeAsngId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _projectManagerRepository.GetUserInfoByUserId(userId);

            ViewBag.GetAllProjectListDetailsForPMReport = _projectManagerRepository.GetAllProjectListDetailsForPMReport(startValue, endValue, userId);

            return View(model);
        }

        #endregion

        [Authorize(Roles = "PM,PMHEAD,CPSD,CPSDHEAD,MM,ASPM,ASPMHEAD")]
        public ActionResult SpareConsumptionReportMonitor()
        {
            List<SpareAnalysisReportMonitorModel> model = _projectManagerRepository.GetAnalysisReportMonitorModels();
            return View(model);
        }

        public JsonResult SpareReportConfirmation(string flag, long id = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            if (flag == "receive")
            {
                _projectManagerRepository.ReceiveSpareAnalysisReport(id, userId);
            }
            if (flag == "submit")
            {
                _projectManagerRepository.SubmitSpareAnalysisReport(id, userId);
            }
            return Json(new
            {
                redirectUrl = Url.Action("SpareConsumptionReportMonitor", "ProjectManager"),
                isRedirect = true
            });
        }

        #region Incentive Sept 2019

        [HttpGet]
        public ActionResult PmMonthlyIncentive(string monId, string yearId, string employeeCode)
        {
            var vmPmIncentive = new VmPmIncentivePolicy();
            long monIds;
            long.TryParse(monId, out monIds);

            long yearIds;
            long.TryParse(yearId, out yearIds);

            var isExist = _projectManagerRepository.GetIncentiveTypeData(employeeCode, Convert.ToInt32(monId), yearId);

            if (isExist)
            {
                TempData["Message2"] = "Incentive already generated";
                return Json("PmMonthlyIncentive", "ProjectManager");
            }
            var isExist1 = _projectManagerRepository.GetPoIncentiveData(employeeCode, Convert.ToInt32(monId), yearId);

            if (isExist1)
            {
                TempData["Message2"] = "Incentive already generated";
                return Json("PmMonthlyIncentive", "ProjectManager");
            }
            //old policy upto sept 2019 related with shipment
            //var isExist2 = _projectManagerRepository.GetShipmentIncentiveData(employeeCode, Convert.ToInt32(monId), yearId);

            //if (isExist2)
            //{
            //    TempData["Message2"] = "Incentive already generated";
            //    return Json("PmMonthlyIncentive", "ProjectManager");
            //}

            var isExist3 = _projectManagerRepository.GetTotalIncentiveData(employeeCode, Convert.ToInt32(monId), yearId);

            if (isExist3)
            {
                TempData["Message2"] = "Incentive already generated";
                return Json("PmMonthlyIncentive", "ProjectManager");
            }

            ViewBag.GetUserPerameterList = _projectManagerRepository.GetUserPerameterList();
            vmPmIncentive.CmnUserModelsList = _projectManagerRepository.GetPmUserList();

            vmPmIncentive.PmIncentiveBaseModelsList = _projectManagerRepository.GetPmIncentiveBase();
            List<SelectListItem> itemsIncentive = vmPmIncentive.PmIncentiveBaseModelsList.Select(model => new SelectListItem { Text = model.IncentiveName + '-' + model.Amount, Value = model.Id.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.IncentivesValue = itemsIncentive;

            vmPmIncentive.PmIncentiveBaseModelsList = _projectManagerRepository.GetPmInventoryAndMarketIssues();
            List<SelectListItem> itemsIncentive1 = vmPmIncentive.PmIncentiveBaseModelsList.Select(model => new SelectListItem { Text = model.IncentiveName + '-' + model.Amount, Value = model.Id.ToString(CultureInfo.InvariantCulture) }).ToList();
            itemsIncentive1.Add(new SelectListItem { Text = "Others", Value = "Others" });
            ViewBag.IncentivesValue1 = itemsIncentive1;

            vmPmIncentive.ProjectMasterModelsList = _projectManagerRepository.GetProjectMasterListForPmIncentive(employeeCode);
            List<SelectListItem> items = vmPmIncentive.ProjectMasterModelsList.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.Projects = items;

            vmPmIncentive.ProjectMasterModelsList = _projectManagerRepository.GetAllProjectsForPmIncentive();
            List<SelectListItem> itemsOfProjects = vmPmIncentive.ProjectMasterModelsList.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.AllProjects = itemsOfProjects;

            vmPmIncentive.ProjectMasterModelsList = _projectManagerRepository.GetAllProjectsForOthers();
            List<SelectListItem> itemsOfProjectsForOthers =
                vmPmIncentive.ProjectMasterModelsList.Select(
                    model =>
                        new SelectListItem
                        {
                            Text = model.ProjectName,
                            Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture)
                        }).ToList();
            ViewBag.AllProjects1 = itemsOfProjectsForOthers;

            return View(vmPmIncentive);
        }

        [HttpPost]
        [NotificationActionFilter(ReceiverRoles = "PM,MM,PS", MessageHeader = "Incentive")]
        public JsonResult PmMonthlyIncentive(string objArr)
        {

            List<Custom_Pm_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Pm_IncentiveModel>>(objArr);
            Console.Write("result :" + results);

            bool isExist = false;
            if (results.Count != 0)
            {
                isExist = _projectManagerRepository.GetIncentiveTypeData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message2"] = "Incentive already generated";
                return Json("PmMonthlyIncentive", "ProjectManager");
            }

            var notificationObject = new NotificationObject
            {
                ProjectId = 1
            };
            ViewBag.ControllerVariable = notificationObject;

            var SaveIncentive = "0";

            if (results.Count != 0)
            {
                SaveIncentive = _projectManagerRepository.SavePmMonthlyIncentive(results);
            }


            return Json(new { SaveIncentive }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveHeadTeamIncentivePercentage(string inChrgObjArr)
        {
            List<Custom_Pm_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Pm_IncentiveModel>>(inChrgObjArr);
            Console.Write("result :" + results);

            bool isExist = false;
            if (results.Count != 0)
            {
                isExist = _projectManagerRepository.CheckTeamIncentivePercentage(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message71"] = "Incentive already generated";
                return Json("PmMonthlyIncentive", "ProjectManager");
            }
            var saveProductIncentive = "0";

            if (results.Count != 0)
            {
                saveProductIncentive = _projectManagerRepository.SaveHeadTeamIncentivePercentage(results);

            }

            return Json(new { saveProductIncentive }, JsonRequestBehavior.AllowGet);
        }
 
        [HttpPost]
        public JsonResult GetPoOrderDetails(string poObjArr)
        {
            List<Custom_Pm_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Pm_IncentiveModel>>(poObjArr);
            Console.Write("result :" + results);
            bool isExist = false;

            if (results.Count != 0)
            {
                isExist = _projectManagerRepository.GetPoIncentiveData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);
            }
            if (isExist)
            {
                TempData["Message2"] = "Incentive already generated";
                return Json("PmMonthlyIncentive", "ProjectManager");
            }
            var savePoIncentive = "0";

            if (results.Count != 0)
            {
                savePoIncentive = _projectManagerRepository.SavePoMonthlyIncentive(results);
            }

            return Json(new { savePoIncentive }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveAllDocumentDetails(string docObjArr)
        {
            List<Custom_Pm_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Pm_IncentiveModel>>(docObjArr);
            Console.Write("result :" + results);
            bool isExist = false;

            if (results.Count != 0)
            {
                //isExist = _projectManagerRepository.GetPoIncentiveData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);
                isExist = _projectManagerRepository.GetDocIncentiveData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);
            }
            if (isExist)
            {
                TempData["Message2"] = "Incentive already generated";
                return Json("PmMonthlyIncentive", "ProjectManager");
            }
            var savedocIncentive = "0";

            if (results.Count != 0)
            {
                savedocIncentive = _projectManagerRepository.SaveAllDocumentDetails(results);
            }

            return Json(new { savedocIncentive }, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public JsonResult SaveProjectClosingDetails(string proClsObjArr)
        {
            List<Pm_IncentiveModel> results = JsonConvert.DeserializeObject<List<Pm_IncentiveModel>>(proClsObjArr);
            Console.Write("result :" + results);
            bool isExist = false;

            if (results.Count != 0)
            {
                isExist = _projectManagerRepository.GetProClosingIncentiveData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);
            }
            if (isExist)
            {
                TempData["Message2"] = "Incentive already generated";
                return Json("PmMonthlyIncentive", "ProjectManager");
            }
            var savedocIncentive = "0";

            if (results.Count != 0)
            {
                savedocIncentive = _projectManagerRepository.SaveProjectClosingDetails(results);
            }

            return Json(new { savedocIncentive }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveRawUploadDelayDetails(string rawObjArr)
        {
            List<Pm_IncentiveModel> results = JsonConvert.DeserializeObject<List<Pm_IncentiveModel>>(rawObjArr);
            Console.Write("result :" + results);
            bool isExist = false;

            if (results.Count != 0)
            {
                isExist = _projectManagerRepository.GetRawUploadIncentiveData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);
            }
            if (isExist)
            {
                TempData["Message2"] = "Incentive already generated";
                return Json("PmMonthlyIncentive", "ProjectManager");
            }
            var savedocIncentive = "0";

            if (results.Count != 0)
            {
                savedocIncentive = _projectManagerRepository.SaveRawUploadDelayDetails(results);
            }

            return Json(new { savedocIncentive }, JsonRequestBehavior.AllowGet);
        }
       
        [HttpPost]
        public JsonResult SaveShipmentClearanceVsLsdDetails(string shipClrObjArr)
        {
            List<Pm_IncentiveModel> results = JsonConvert.DeserializeObject<List<Pm_IncentiveModel>>(shipClrObjArr);
            Console.Write("result :" + results);
            bool isExist = false;

            if (results.Count != 0)
            {
                isExist = _projectManagerRepository.GetShipmentVsLsdIncentiveData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);
            }
            if (isExist)
            {
                TempData["Message2"] = "Incentive already generated";
                return Json("PmMonthlyIncentive", "ProjectManager");
            }
            var savedocIncentive = "0";

            if (results.Count != 0)
            {
                savedocIncentive = _projectManagerRepository.SaveShipmentClearanceVsLsdDetails(results);
            }

            return Json(new { savedocIncentive }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveOthersIncentiveTypesDetails(string othersObjArr)
        {
            var results =
                JsonConvert.DeserializeObject<List<Custom_Pm_IncentiveModel>>(othersObjArr);
            Console.Write(othersObjArr);
            bool isExist = false;

            //if (results.Count != 0)
            //{
            //    isExist = _projectManagerRepository.GetIncentiveTypeData(results[0].EmployeeCode, results[0].MonNum,
            //        results[0].Year);
            //}

            //if (isExist)
            //{
            //    TempData["Message6"] = "Incentive already generated";
            //    return Json("PmMonthlyIncentive", "ProjectManager");
            //}

            var saveOthersIncentiveTypes = "0";

            if (results.Count != 0)
            {
                saveOthersIncentiveTypes = _projectManagerRepository.SaveOthersIncentive(results);
            }

            return null;
        }
        [HttpPost]
        public JsonResult SaveAccessoriesDetails(string accessObjArr)
        {
            var results =
                JsonConvert.DeserializeObject<List<Custom_Pm_IncentiveModel>>(accessObjArr);
            Console.Write(accessObjArr);

            bool isExist = false;

            if (results.Count != 0)
            {
                isExist = _projectManagerRepository.GetAccessoriesSavedData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message89"] = "Incentive already generated";
                return Json("PmMonthlyIncentive", "ProjectManager");
            }

            var saveAccessoriesIncentiveTypes = "0";

            if (results.Count != 0)
            {
                saveAccessoriesIncentiveTypes = _projectManagerRepository.SaveAccessoriesDetails(results);
            }

            return null;
        }

        [HttpPost]
        public JsonResult SaveMarketIssue(string marketIssueObjArr)
        {
            List<Custom_Pm_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Pm_IncentiveModel>>(marketIssueObjArr);
            Console.Write("result :" + results);

            bool isExist = false;
            if (results.Count != 0)
            {
                isExist = _projectManagerRepository.GetIncentiveTypeData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message2"] = "Incentive already generated";
                return Json("PmMonthlyIncentive", "ProjectManager");
            }
            var saveMarketIssueIncentive = "0";

            if (results.Count != 0)
            {
                saveMarketIssueIncentive = _projectManagerRepository.SaveMarketIssueIncentive(results);

            }

            return Json(new { saveMarketIssueIncentive }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SavePiDetails(string piObjArr)
        {
            List<Custom_Pm_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Pm_IncentiveModel>>(piObjArr);
            Console.Write("result :" + results);

            bool isExist = false;
            if (results.Count != 0)
            {
                isExist = _projectManagerRepository.GetPiData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message88"] = "Incentive already generated";
                return Json("PmMonthlyIncentive", "ProjectManager");
            }
            var savePiIncentive = "0";

            if (results.Count != 0)
            {
                savePiIncentive = _projectManagerRepository.SavePiDetails(results);

            }

            return Json(new { savePiIncentive }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveProductionRewardData(string productObjArr)
        {
            List<NinetyFiveProductionRewardModel> results = JsonConvert.DeserializeObject<List<NinetyFiveProductionRewardModel>>(productObjArr);
            Console.Write("result :" + results);

            bool isExist = false;
            if (results.Count != 0)
            {
                isExist = _projectManagerRepository.ProductionRewardDataCheck(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message82"] = "Incentive already generated";
                return Json("PmMonthlyIncentive", "ProjectManager");
            }
            var saveProductIncentive = "0";

            if (results.Count != 0)
            {
                saveProductIncentive = _projectManagerRepository.SaveProductionRewardData(results);

            }

            return Json(new { saveProductIncentive }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveSalesOutRewardData(string tblSalesOutObjArr)
        {
            List<NinetyFiveProductionRewardModel> results = JsonConvert.DeserializeObject<List<NinetyFiveProductionRewardModel>>(tblSalesOutObjArr);
            Console.Write("result :" + results);

            bool isExist = false;
            if (results.Count != 0)
            {
                isExist = _projectManagerRepository.SalesOutRewardDataCheck(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message83"] = "Incentive already generated";
                return Json("PmMonthlyIncentive", "ProjectManager");
            }
            var saveProductIncentive = "0";

            if (results.Count != 0)
            {
                saveProductIncentive = _projectManagerRepository.SaveSalesOutRewardData(results);

            }

            return Json(new { saveProductIncentive }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveVesselRewardOrPenaltiesData(string tblVesselObjArr)
        {
            List<NinetyFiveProductionRewardModel> results = JsonConvert.DeserializeObject<List<NinetyFiveProductionRewardModel>>(tblVesselObjArr);
            Console.Write("result :" + results);

            bool isExist = false;
            if (results.Count != 0)
            {
                isExist = _projectManagerRepository.VesselRewardOrPenaltiesDataCheck(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message84"] = "Incentive already generated";
                return Json("PmMonthlyIncentive", "ProjectManager");
            }
            var saveProductIncentive = "0";

            if (results.Count != 0)
            {
                saveProductIncentive = _projectManagerRepository.SaveVesselRewardOrPenaltiesData(results);

            }

            return Json(new { saveProductIncentive }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SavePoDetailsPageForPmHead(string poRewardObjArr)
        {
            List<NinetyFiveProductionRewardModel> results = JsonConvert.DeserializeObject<List<NinetyFiveProductionRewardModel>>(poRewardObjArr);
            Console.Write("result :" + results);

            bool isExist = false;
            if (results.Count != 0)
            {
                isExist = _projectManagerRepository.PoDetailsPageForPmHeadDataCheck(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message85"] = "Incentive already generated";
                return Json("PmMonthlyIncentive", "ProjectManager");
            }
            var saveProductIncentive = "0";

            if (results.Count != 0)
            {
                saveProductIncentive = _projectManagerRepository.SavePoDetailsPageForPmHead(results);

            }

            return Json(new { saveProductIncentive }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SavePenaltiesDetailsPageForPmHead(string poPenaltiesObjArr)
        {
            List<NinetyFiveProductionRewardModel> results = JsonConvert.DeserializeObject<List<NinetyFiveProductionRewardModel>>(poPenaltiesObjArr);
            Console.Write("result :" + results);

            bool isExist = false;
            if (results.Count != 0)
            {
                isExist = _projectManagerRepository.PenaltiesDetailsPageForPmHeadCheck(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message87"] = "Incentive already generated";
                return Json("PmMonthlyIncentive", "ProjectManager");
            }
            var saveProductIncentive = "0";

            if (results.Count != 0)
            {
                saveProductIncentive = _projectManagerRepository.SavePenaltiesDetailsPageForPmHead(results);

            }

            return Json(new { saveProductIncentive }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProject(string employeeCode)
        {
            var projectMasterModelsList = _projectManagerRepository.GetProjectMasterListForPmIncentive(employeeCode);
            List<SelectListItem> items = projectMasterModelsList.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture) }).ToList();
            var json = JsonConvert.SerializeObject(items);

            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //old policy Upto Sept 2019 shipment related
        //[HttpPost]
        //public JsonResult GetShipmentDetailsForSave(string shipObjArr)
        //{
        //    List<Custom_Pm_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Pm_IncentiveModel>>(shipObjArr);
        //    Console.Write("result :" + results);

        //    bool isExist = false;

        //    if (results.Count != 0)
        //    {
        //        isExist = _projectManagerRepository.GetShipmentIncentiveData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

        //    }

        //    if (isExist)
        //    {
        //        TempData["Message2"] = "Incentive already generated";
        //        return Json("PmMonthlyIncentive", "ProjectManager");
        //    }
        //    var saveShipIncentive = "0";
        //    if (results.Count != 0)
        //    {
        //        saveShipIncentive = _projectManagerRepository.SaveShipmentIncentive(results);
        //    }

        //    return Json(new { saveShipIncentive }, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetShipmentRelatedProject(string employeeCode)
        //{
        //    List<Pm_Shipment_Incentive> itemsPmShipmentIncentive = new List<Pm_Shipment_Incentive>();
        //    var PmShipmentIncentiveModels = _projectManagerRepository.GetPmShipmentIncentive(employeeCode);

        //    foreach (var pmShipmentIncentiveModel in PmShipmentIncentiveModels)
        //    {
        //        Pm_Shipment_Incentive items = new Pm_Shipment_Incentive();

        //        items.ProjectType = pmShipmentIncentiveModel.ProjectType;
        //        items.ProjectId = pmShipmentIncentiveModel.ProjectId;
        //        items.ProjectName = pmShipmentIncentiveModel.ProjectName;
        //        items.ApproxShipmentDate = pmShipmentIncentiveModel.ApproxShipmentDate;
        //        items.ChainaInspectionDate = Convert.ToDateTime(pmShipmentIncentiveModel.FlightDepartureDate).Date;
        //        items.D_Remarks = pmShipmentIncentiveModel.D_Remarks;
        //        items.Remarks = pmShipmentIncentiveModel.Remarks;
        //        items.OrderNumber = pmShipmentIncentiveModel.OrderNumber;
        //        items.EmployeeCode = pmShipmentIncentiveModel.EmployeeCode;
        //        items.ProjectManagerUserId = pmShipmentIncentiveModel.ProjectManagerUserId;
        //        items.NoOfdays = pmShipmentIncentiveModel.NoOfdays;
        //        items.Amount = pmShipmentIncentiveModel.Amount;
        //        //  items.DeductionAmount = pmShipmentIncentiveModel.DeductionAmount;

        //        if (Convert.ToDouble(items.NoOfdays) > 0)
        //        {

        //            //if (items.NoOfdays >= 15)
        //            //{
        //            //    double de_Am = (50.00 * 15.00) / (100.00 * 15.00);
        //            //    items.DeductionAmount = Convert.ToDecimal(de_Am) * items.Amount;
        //            //    items.FinalAmount = items.Amount - items.DeductionAmount ;
        //            //}
        //            //else if (items.NoOfdays <= 15 && items.NoOfdays>0)
        //            //{
        //            //    double de_Am = Convert.ToDouble((50.00 * items.NoOfdays) / (100.00 * 15.00));
        //            //    items.DeductionAmount = Convert.ToDecimal(de_Am) * items.Amount;
        //            //    items.FinalAmount = items.Amount - items.DeductionAmount;
        //            //}
        //            if (items.NoOfdays >= 15)
        //            {
        //                double de_Am = (50.00 * 15.00) / (100.00 * 15.00);
        //                items.DeductionAmount = Convert.ToDecimal(de_Am) * items.Amount;
        //                items.Amount = 0;
        //                items.FinalAmount = items.Amount - items.DeductionAmount;
        //            }
        //            else if (items.NoOfdays <= 15 && items.NoOfdays > 0)
        //            {
        //                double de_Am = Convert.ToDouble((50.00 * items.NoOfdays) / (100.00 * 15.00));
        //                items.DeductionAmount = Convert.ToDecimal(de_Am) * items.Amount;
        //                items.Amount = 0;
        //                items.FinalAmount = items.Amount - items.DeductionAmount;
        //            }
        //        }
        //        else if (Convert.ToDouble(items.NoOfdays) < -15)
        //        {
        //            items.DeductionAmount = 0;
        //            items.FinalAmount = items.NoOfdays * 150;
        //        }

        //        itemsPmShipmentIncentive.Add(items);
        //    }

        //    ViewBag.PmShipmentIncentive = itemsPmShipmentIncentive;

        //    var json = JsonConvert.SerializeObject(itemsPmShipmentIncentive);

        //    return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}

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
                return PartialView("~/Views/ProjectManager/Partial/_PmIncentiveList.cshtml", model);
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

                return PartialView("~/Views/ProjectManager/Partial/_PmIncentiveOthersList.cshtml", model);
            }
        }
        [HttpPost]
        public ActionResult AddProjectFoAccessories(string incentiveTypeAccessories, string accessoriesAmount, string personEmpCode, string projectName,
           long projectMasterId, string monName, int monId, string yearName,
          long yearId, string remarks, string deductAmount, string d_remarks, string finalAmount)
        {
            using (CellPhoneProjectEntities dEntities = new CellPhoneProjectEntities())
            {
                var model = new Custom_Pm_IncentiveModel
                {
                    ProjectId = projectMasterId,
                    ProjectName = projectName,
                    IncentiveTypeForAccessories = incentiveTypeAccessories,
                    Amount = accessoriesAmount.ToString(),
                    EmployeeCode = personEmpCode,
                    MonNum = monId,
                    Month = monName,
                    Year = yearId.ToString(),
                    Remarks = remarks,
                    D_Remarks = d_remarks,
                    DeductionAmount = deductAmount.ToString(),
                    FinalAmount = finalAmount.ToString()
                };

                return PartialView("~/Views/ProjectManager/Partial/_PmIncentiveForAccessories.cshtml", model);
            }
        }

        [HttpPost]
        public ActionResult AddProjectForMarketIssue(string incentiveType, string incentiveAmount, long incentiveId, string personEmpCode, string multiProjectName,
           string[] multiprojectIds, string monName, int monId, string yearName,
          long yearId, string remarks, string deductAmount, string d_remarks, string finalAmount, string perPersonNo)
        {
            using (var dbEntities = new CellPhoneProjectEntities())
            {
                var mIdss = string.Empty;
                foreach (var mIds in multiprojectIds)
                {
                    mIdss = mIdss + mIds + ",";
                }


                var model = new Custom_Pm_IncentiveModel();

                model = new Custom_Pm_IncentiveModel
               {
                   MultiProjectIds = mIdss.Trim(','),
                   MultiProjectName = multiProjectName.Trim(','),
                   PersonNo = Convert.ToInt32(perPersonNo),
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

                return PartialView("~/Views/ProjectManager/Partial/_PmIncentiveMarketIssue.cshtml", model);
            }

        }
        public JsonResult GetPoProject(string employeeCode)
        {
            List<Pm_Po_Incentive> itemsPmPoIncentive = new List<Pm_Po_Incentive>();
            var PmPoIncentiveModels = _projectManagerRepository.GetPmPoIncentiveForSKD(employeeCode);
            //itemsPmPoIncentive = PmPoIncentiveModels.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectId.ToString(CultureInfo.InvariantCulture) }).ToList();
            foreach (var pmPoIncentiveModel in PmPoIncentiveModels)
            {
                Pm_Po_Incentive items = new Pm_Po_Incentive();
                items.ProjectType = pmPoIncentiveModel.ProjectType;
                items.ProjectId = pmPoIncentiveModel.ProjectId;
                items.ProjectName = pmPoIncentiveModel.ProjectName;
                items.PoCategory = pmPoIncentiveModel.PoCategory;
                items.PoDate = Convert.ToDateTime(pmPoIncentiveModel.PoDate).Date;
                items.Amount = pmPoIncentiveModel.Amount;
                items.DeductionAmount = pmPoIncentiveModel.DeductionAmount;
                items.D_Remarks = pmPoIncentiveModel.D_Remarks;
                items.Remarks = pmPoIncentiveModel.Remarks;
                items.FinalAmount = pmPoIncentiveModel.FinalAmount;
                items.OrderNumber = pmPoIncentiveModel.OrderNumber;
                items.EmployeeCode = pmPoIncentiveModel.EmployeeCode;
                items.ProjectManagerUserId = pmPoIncentiveModel.ProjectManagerUserId;
                itemsPmPoIncentive.Add(items);
            }
            //old policy upto sept 2019
            //PmPoIncentiveModels = _projectManagerRepository.GetPmPoIncentiveForCBU(employeeCode);
            //foreach (var pmPoIncentiveModel in PmPoIncentiveModels)
            //{
            //    Pm_Po_Incentive items = new Pm_Po_Incentive();
            //    items.ProjectType = pmPoIncentiveModel.ProjectType;
            //    items.ProjectId = pmPoIncentiveModel.ProjectId;
            //    items.ProjectName = pmPoIncentiveModel.ProjectName;
            //    items.PoCategory = pmPoIncentiveModel.PoCategory;
            //    items.PoDate = Convert.ToDateTime(pmPoIncentiveModel.PoDate).Date;
            //    items.Amount = pmPoIncentiveModel.Amount;
            //    items.DeductionAmount = pmPoIncentiveModel.DeductionAmount;
            //    items.D_Remarks = pmPoIncentiveModel.D_Remarks;
            //    items.Remarks = pmPoIncentiveModel.Remarks;
            //    items.FinalAmount = pmPoIncentiveModel.FinalAmount;
            //    items.OrderNumber = pmPoIncentiveModel.OrderNumber;
            //    items.EmployeeCode = pmPoIncentiveModel.EmployeeCode;
            //    items.ProjectManagerUserId = pmPoIncentiveModel.ProjectManagerUserId;
            //    itemsPmPoIncentive.Add(items);
            //}
            PmPoIncentiveModels = _projectManagerRepository.GetPmPoIncentiveForPerOrder(employeeCode);
            foreach (var pmPoIncentiveModel in PmPoIncentiveModels)
            {
                Pm_Po_Incentive items = new Pm_Po_Incentive();
                items.ProjectType = pmPoIncentiveModel.ProjectType;
                items.ProjectId = pmPoIncentiveModel.ProjectId;
                items.ProjectName = pmPoIncentiveModel.ProjectName;
                items.PoCategory = pmPoIncentiveModel.PoCategory;
                items.PoDate = Convert.ToDateTime(pmPoIncentiveModel.PoDate).Date;
                items.Amount = pmPoIncentiveModel.Amount;
                items.DeductionAmount = pmPoIncentiveModel.DeductionAmount;
                items.D_Remarks = pmPoIncentiveModel.D_Remarks;
                items.Remarks = pmPoIncentiveModel.Remarks;
                items.FinalAmount = pmPoIncentiveModel.FinalAmount;
                items.OrderNumber = pmPoIncentiveModel.OrderNumber;
                items.EmployeeCode = pmPoIncentiveModel.EmployeeCode;
                items.ProjectManagerUserId = pmPoIncentiveModel.ProjectManagerUserId;
                itemsPmPoIncentive.Add(items);
            }
            ViewBag.PmPoIncentive = itemsPmPoIncentive;

            var json = JsonConvert.SerializeObject(itemsPmPoIncentive);

            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetDocumentUploadIncentive(string employeeCode,string monthName,string monNum,string year)
        {
            List<Pm_IncentiveModel> itemsPmDocIncentive = new List<Pm_IncentiveModel>();
            var PmDocIncentiveModels = _projectManagerRepository.GetAccessoriesProjectIncentive(employeeCode,monNum,year);
            foreach (var pmPoIncentiveModel in PmDocIncentiveModels)
            {
                Pm_IncentiveModel items = new Pm_IncentiveModel();
                items.ProjectIds = pmPoIncentiveModel.ProjectIds;
                items.IncentiveType = pmPoIncentiveModel.IncentiveType;
                items.MultiProjectName = pmPoIncentiveModel.MultiProjectName;
                items.Orders = pmPoIncentiveModel.Orders;
                items.EmployeeCode = pmPoIncentiveModel.EmployeeCode;
                items.EffectiveMonth = Convert.ToDateTime(pmPoIncentiveModel.EffectiveMonth).Date;
                items.AccessoriesType = pmPoIncentiveModel.AccessoriesType;
                items.CurrencyType = pmPoIncentiveModel.CurrencyType;
                items.Remarks = pmPoIncentiveModel.Remarks;
                items.IsDocumentUploaded = pmPoIncentiveModel.IsDocumentUploaded;
                items.Amount = pmPoIncentiveModel.Amount;
                items.DeductAmount = pmPoIncentiveModel.DeductAmount;
                items.FinalAmount = pmPoIncentiveModel.FinalAmount;
                itemsPmDocIncentive.Add(items);
            }
            PmDocIncentiveModels = _projectManagerRepository.GetFollowUpFocMaterialIncentive(employeeCode, monNum, year);
            foreach (var pmPoIncentiveModel in PmDocIncentiveModels)
            {
                Pm_IncentiveModel items = new Pm_IncentiveModel();
                items.ProjectIds = pmPoIncentiveModel.ProjectIds;
                items.IncentiveType = pmPoIncentiveModel.IncentiveType;
                items.MultiProjectName = pmPoIncentiveModel.MultiProjectName;
                items.Orders = pmPoIncentiveModel.Orders;
                items.EmployeeCode = pmPoIncentiveModel.EmployeeCode;
                items.EffectiveMonth = Convert.ToDateTime(pmPoIncentiveModel.EffectiveMonth).Date;
                items.AccessoriesType = pmPoIncentiveModel.AccessoriesType;
                items.CurrencyType = pmPoIncentiveModel.CurrencyType;
                items.Remarks = pmPoIncentiveModel.Remarks;
                items.IsDocumentUploaded = pmPoIncentiveModel.IsDocumentUploaded;
                items.Amount = pmPoIncentiveModel.Amount;
                items.DeductAmount = pmPoIncentiveModel.DeductAmount;
                items.FinalAmount = pmPoIncentiveModel.FinalAmount;
                itemsPmDocIncentive.Add(items);
            }
            PmDocIncentiveModels = _projectManagerRepository.GetPoFeedbackIncentive(employeeCode, monNum, year);
            foreach (var pmPoIncentiveModel in PmDocIncentiveModels)
            {
                Pm_IncentiveModel items = new Pm_IncentiveModel();
                items.ProjectIds = pmPoIncentiveModel.ProjectIds;
                items.IncentiveType = pmPoIncentiveModel.IncentiveType;
                items.MultiProjectName = pmPoIncentiveModel.MultiProjectName;
                items.Orders = pmPoIncentiveModel.Orders;
                items.EmployeeCode = pmPoIncentiveModel.EmployeeCode;
                items.EffectiveMonth = Convert.ToDateTime(pmPoIncentiveModel.EffectiveMonth).Date;
                items.AccessoriesType = pmPoIncentiveModel.AccessoriesType;
                items.CurrencyType = pmPoIncentiveModel.CurrencyType;
                items.Remarks = pmPoIncentiveModel.Remarks;
                items.IsDocumentUploaded = pmPoIncentiveModel.IsDocumentUploaded;
                items.Amount = pmPoIncentiveModel.Amount;
                items.DeductAmount = pmPoIncentiveModel.DeductAmount;
                items.FinalAmount = pmPoIncentiveModel.FinalAmount;
                itemsPmDocIncentive.Add(items);
            }
            PmDocIncentiveModels = _projectManagerRepository.GetSupplierPenaltiesIncentive(employeeCode, monNum, year);
            foreach (var pmPoIncentiveModel in PmDocIncentiveModels)
            {
                Pm_IncentiveModel items = new Pm_IncentiveModel();
                items.ProjectIds = pmPoIncentiveModel.ProjectIds;
                items.IncentiveType = pmPoIncentiveModel.IncentiveType;
                items.MultiProjectName = pmPoIncentiveModel.MultiProjectName;
                items.Orders = pmPoIncentiveModel.Orders;
                items.EmployeeCode = pmPoIncentiveModel.EmployeeCode;
                items.EffectiveMonth = Convert.ToDateTime(pmPoIncentiveModel.EffectiveMonth).Date;
                items.AccessoriesType = pmPoIncentiveModel.AccessoriesType;
                items.CurrencyType = pmPoIncentiveModel.CurrencyType;
                items.Remarks = pmPoIncentiveModel.Remarks;
                items.IsDocumentUploaded = pmPoIncentiveModel.IsDocumentUploaded;
                items.Amount = pmPoIncentiveModel.Amount;
                items.DeductAmount = pmPoIncentiveModel.DeductAmount;
                items.FinalAmount = pmPoIncentiveModel.FinalAmount;
                itemsPmDocIncentive.Add(items);
            }
            PmDocIncentiveModels = _projectManagerRepository.GetPmGuidelinesIncentive(employeeCode, monNum, year);
            foreach (var pmPoIncentiveModel in PmDocIncentiveModels)
            {
                Pm_IncentiveModel items = new Pm_IncentiveModel();
                items.ProjectIds = pmPoIncentiveModel.ProjectIds;
                items.IncentiveType = pmPoIncentiveModel.IncentiveType;
                items.MultiProjectName = pmPoIncentiveModel.MultiProjectName;
                items.Orders = pmPoIncentiveModel.Orders;
                items.EmployeeCode = pmPoIncentiveModel.EmployeeCode;
                items.EffectiveMonth = Convert.ToDateTime(pmPoIncentiveModel.EffectiveMonth).Date;
                items.AccessoriesType = pmPoIncentiveModel.AccessoriesType;
                items.CurrencyType = pmPoIncentiveModel.CurrencyType;
                items.Remarks = pmPoIncentiveModel.Remarks;
                items.IsDocumentUploaded = pmPoIncentiveModel.IsDocumentUploaded;
                items.Amount = pmPoIncentiveModel.Amount;
                items.DeductAmount = pmPoIncentiveModel.DeductAmount;
                items.FinalAmount = pmPoIncentiveModel.FinalAmount;
                itemsPmDocIncentive.Add(items);
            }
            PmDocIncentiveModels = _projectManagerRepository.GetProjectMarketingIncentive(employeeCode, monNum, year);
            foreach (var pmPoIncentiveModel in PmDocIncentiveModels)
            {
                Pm_IncentiveModel items = new Pm_IncentiveModel();
                items.ProjectIds = pmPoIncentiveModel.ProjectIds;
                items.IncentiveType = pmPoIncentiveModel.IncentiveType;
                items.MultiProjectName = pmPoIncentiveModel.MultiProjectName;
                items.Orders = pmPoIncentiveModel.Orders;
                items.EmployeeCode = pmPoIncentiveModel.EmployeeCode;
                items.EffectiveMonth = Convert.ToDateTime(pmPoIncentiveModel.EffectiveMonth).Date;
                items.AccessoriesType = pmPoIncentiveModel.AccessoriesType;
                items.CurrencyType = pmPoIncentiveModel.CurrencyType;
                items.Remarks = pmPoIncentiveModel.Remarks;
                items.IsDocumentUploaded = pmPoIncentiveModel.IsDocumentUploaded;
                items.Amount = pmPoIncentiveModel.Amount;
                items.DeductAmount = pmPoIncentiveModel.DeductAmount;
                items.FinalAmount = pmPoIncentiveModel.FinalAmount;
                itemsPmDocIncentive.Add(items);
            }
            PmDocIncentiveModels = _projectManagerRepository.GetPolicyUpdateIncentive(employeeCode, monNum, year);
            foreach (var pmPoIncentiveModel in PmDocIncentiveModels)
            {
                Pm_IncentiveModel items = new Pm_IncentiveModel();
                items.ProjectIds = pmPoIncentiveModel.ProjectIds;
                items.IncentiveType = pmPoIncentiveModel.IncentiveType;
                items.MultiProjectName = pmPoIncentiveModel.MultiProjectName;
                items.Orders = pmPoIncentiveModel.Orders;
                items.EmployeeCode = pmPoIncentiveModel.EmployeeCode;
                items.EffectiveMonth = Convert.ToDateTime(pmPoIncentiveModel.EffectiveMonth).Date;
                items.AccessoriesType = pmPoIncentiveModel.AccessoriesType;
                items.CurrencyType = pmPoIncentiveModel.CurrencyType;
                items.Remarks = pmPoIncentiveModel.Remarks;
                items.IsDocumentUploaded = pmPoIncentiveModel.IsDocumentUploaded;
                items.Amount = pmPoIncentiveModel.Amount;
                items.DeductAmount = pmPoIncentiveModel.DeductAmount;
                items.FinalAmount = pmPoIncentiveModel.FinalAmount;
                itemsPmDocIncentive.Add(items);
            }
            PmDocIncentiveModels = _projectManagerRepository.GetSampleHandsetIncentive(employeeCode, monNum, year);
            foreach (var pmPoIncentiveModel in PmDocIncentiveModels)
            {
                Pm_IncentiveModel items = new Pm_IncentiveModel();
                items.ProjectIds = pmPoIncentiveModel.ProjectIds;
                items.IncentiveType = pmPoIncentiveModel.IncentiveType;
                items.MultiProjectName = pmPoIncentiveModel.MultiProjectName;
                items.Orders = pmPoIncentiveModel.Orders;
                items.EmployeeCode = pmPoIncentiveModel.EmployeeCode;
                items.EffectiveMonth = Convert.ToDateTime(pmPoIncentiveModel.EffectiveMonth).Date;
                items.AccessoriesType = pmPoIncentiveModel.AccessoriesType;
                items.CurrencyType = pmPoIncentiveModel.CurrencyType;
                items.Remarks = pmPoIncentiveModel.Remarks;
                items.IsDocumentUploaded = pmPoIncentiveModel.IsDocumentUploaded;
                items.Amount = pmPoIncentiveModel.Amount;
                items.DeductAmount = pmPoIncentiveModel.DeductAmount;
                items.FinalAmount = pmPoIncentiveModel.FinalAmount;
                itemsPmDocIncentive.Add(items);
            }
            //
            ViewBag.PmDocUploadIncentive = itemsPmDocIncentive;

            var json = JsonConvert.SerializeObject(itemsPmDocIncentive);

            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetProjectClosingIncentive(string employeeCode, string monthName, string monNum, string year)
        {
            List<Pm_IncentiveModel> itemsPmDocIncentive = new List<Pm_IncentiveModel>();
            var PmDocIncentiveModels = _projectManagerRepository.GetProClosingIncentive(employeeCode, monNum, year);
            foreach (var pmPoIncentiveModel in PmDocIncentiveModels)
            {
                Pm_IncentiveModel items = new Pm_IncentiveModel();
                items.ProjectMasterId = pmPoIncentiveModel.ProjectMasterId;
                items.ProjectName = pmPoIncentiveModel.ProjectName;
                items.Orders = pmPoIncentiveModel.Orders;
                items.EmployeeCode = pmPoIncentiveModel.EmployeeCode;
                items.ProjectClosingDate = Convert.ToDateTime(pmPoIncentiveModel.ProjectClosingDate).Date;
                items.MarketClearanceDate = Convert.ToDateTime(pmPoIncentiveModel.MarketClearanceDate).Date;
                items.Amount = pmPoIncentiveModel.Amount;
                items.FinalAmount = pmPoIncentiveModel.FinalAmount;
                itemsPmDocIncentive.Add(items);
            }
            //
            ViewBag.PmDocUploadIncentive = itemsPmDocIncentive;

            var json = JsonConvert.SerializeObject(itemsPmDocIncentive);

            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetRawMaterialDelayUploadIncentive(string employeeCode, string monthName, string monNum, string year)
        {
            List<Pm_IncentiveModel> itemsPmDocIncentive = new List<Pm_IncentiveModel>();
            var PmDocIncentiveModels = _projectManagerRepository.GetRawMaterialDelayUploadIncentive(employeeCode, monNum, year);
            foreach (var pmPoIncentiveModel in PmDocIncentiveModels)
            {
                Pm_IncentiveModel items = new Pm_IncentiveModel();
                items.ProjectMasterId = pmPoIncentiveModel.ProjectMasterId;
                items.ProjectName = pmPoIncentiveModel.ProjectName;
                items.Orders = pmPoIncentiveModel.Orders;
                items.PoCategory = pmPoIncentiveModel.PoCategory;
                items.PoQuantity = pmPoIncentiveModel.PoQuantity;
                items.EmployeeCode = pmPoIncentiveModel.EmployeeCode;
                items.ProjectManagerClearanceDate = Convert.ToDateTime(pmPoIncentiveModel.ProjectManagerClearanceDate).Date;
                items.RawMaterialAddedDate = Convert.ToDateTime(pmPoIncentiveModel.RawMaterialAddedDate).Date;
                items.DaysPassed = pmPoIncentiveModel.DaysPassed;
                items.Amount = pmPoIncentiveModel.Amount;
                items.IncentiveName = pmPoIncentiveModel.IncentiveName;
                items.FinalAmount = pmPoIncentiveModel.FinalAmount;
                itemsPmDocIncentive.Add(items);
            }
            //
            ViewBag.PmDocUploadIncentive = itemsPmDocIncentive;

            var json = JsonConvert.SerializeObject(itemsPmDocIncentive);
            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetShipClearenceVsLsdIncentive(string employeeCode, string monthName, string monNum, string year)
        {
            List<Pm_IncentiveModel> itemsPmDocIncentive = new List<Pm_IncentiveModel>();
            var PmDocIncentiveModels = _projectManagerRepository.GetShipClearenceVsLsdIncentive(employeeCode, monNum, year);
            foreach (var pmPoIncentiveModel in PmDocIncentiveModels)
            {
                Pm_IncentiveModel items = new Pm_IncentiveModel();
                items.ProjectMasterId = pmPoIncentiveModel.ProjectMasterId;
                items.ProjectName = pmPoIncentiveModel.ProjectName;
                items.Orders = pmPoIncentiveModel.Orders;
                items.ProjectType = pmPoIncentiveModel.ProjectType;
                items.PoCategory = pmPoIncentiveModel.PoCategory;
                items.EmployeeCode = pmPoIncentiveModel.EmployeeCode;
                items.ProjectManagerClearanceDate = Convert.ToDateTime(pmPoIncentiveModel.ProjectManagerClearanceDate).Date;
                items.LSD = Convert.ToDateTime(pmPoIncentiveModel.LSD).Date;
                items.DaysBeforeLsd = pmPoIncentiveModel.DaysBeforeLsd;
                items.DaysAfterLsd = pmPoIncentiveModel.DaysAfterLsd;
                items.Reward = pmPoIncentiveModel.Reward;
                items.RealPenalties = pmPoIncentiveModel.RealPenalties;
                items.Penalties = pmPoIncentiveModel.Penalties;
                items.Amount = pmPoIncentiveModel.FinalAmount;
                items.FinalAmount = pmPoIncentiveModel.FinalAmount;
                itemsPmDocIncentive.Add(items);
            }
            //
            ViewBag.PmDocUploadIncentive = itemsPmDocIncentive;

            var json = JsonConvert.SerializeObject(itemsPmDocIncentive);
            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult GetPiRelatedProject(string employeeCode, string monthName, string monNum, string year)
        {
            List<Pm_PiClosingModel> itemPiClosings = new List<Pm_PiClosingModel>();
            var pmPiIncentives = _projectManagerRepository.GetPmPiIncentive(employeeCode, monthName, monNum, year);


            foreach (var pmPoIncentiveModel in pmPiIncentives)
            {
                Pm_PiClosingModel items = new Pm_PiClosingModel();
                // items.ProjectType = pmPoIncentiveModel.ProjectType;
                items.ProjectId = pmPoIncentiveModel.ProjectId;
                items.ProjectName = pmPoIncentiveModel.ProjectName;
                items.PoCategory = pmPoIncentiveModel.PoCategory;
                items.PoDate = pmPoIncentiveModel.PoDate;
                items.ClosingAmount = pmPoIncentiveModel.ClosingAmount;
                items.ClosingDate = pmPoIncentiveModel.ClosingDate;
                items.MonthName = pmPoIncentiveModel.MonthName;
                items.MonthNo = pmPoIncentiveModel.MonthNo;
                items.Year = pmPoIncentiveModel.Year;
                items.DeductionAmount = pmPoIncentiveModel.DeductionAmount;
                items.FinalAmount = pmPoIncentiveModel.FinalAmount;
                items.ClosingType = pmPoIncentiveModel.ClosingType;
                items.OrderNumber = pmPoIncentiveModel.OrderNumber;
                items.EmployeeCode = pmPoIncentiveModel.EmployeeCode;
                items.ProjectManagerUserId = pmPoIncentiveModel.ProjectManagerUserId;
                itemPiClosings.Add(items);
            }
            //pmPiIncentives = _projectManagerRepository.GetPmPoIncentiveForCBU(employeeCode);

            ViewBag.PmPiIncentive = itemPiClosings;

            var json = JsonConvert.SerializeObject(itemPiClosings);

            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult GetProductionReward(string employeeCode, string monthName, string monNum, string year)
        {
            List<NinetyFiveProductionRewardModel> itemPiClosings = new List<NinetyFiveProductionRewardModel>();
            var pmPiIncentives = _projectManagerRepository.GetProductionReward(employeeCode, monthName, monNum, year);
            //ProjectModel
            foreach (var pmPoIncentiveModel in pmPiIncentives)
            {
                NinetyFiveProductionRewardModel items = new NinetyFiveProductionRewardModel();
                items.ProjectMasterID = pmPoIncentiveModel.ProjectMasterID;
                if (pmPoIncentiveModel.ProjectName != null)
                {
                    items.ProjectModel = pmPoIncentiveModel.ProjectName;
                }
                else
                {
                    items.ProjectModel = pmPoIncentiveModel.ProjectModel;
                }
                //items.ProjectName = pmPoIncentiveModel.ProjectName;
                // items.ProjectModel = pmPoIncentiveModel.ProjectName;
                items.EmployeeCode = pmPoIncentiveModel.EmployeeCode;
                items.UserFullName = pmPoIncentiveModel.UserFullName;
                items.SourcingType = pmPoIncentiveModel.SourcingType;
                items.WpmsOrders = pmPoIncentiveModel.WpmsOrders;
                items.WarehouseEntryDate = pmPoIncentiveModel.WarehouseEntryDate;
                items.ExtendedWarehouseDate = pmPoIncentiveModel.ExtendedWarehouseDate;
                items.ProjectManagerUserId = pmPoIncentiveModel.ProjectManagerUserId;
                items.OrderQuantity = pmPoIncentiveModel.OrderQuantity;
                items.TotalProductionQuantity = pmPoIncentiveModel.TotalProductionQuantity;
                items.EffectiveDays = pmPoIncentiveModel.EffectiveDays;
                items.RewardPercentage = pmPoIncentiveModel.RewardPercentage;
                items.ExistedPercentage = pmPoIncentiveModel.ExistedPercentage;
                items.RewardAmount = pmPoIncentiveModel.RewardAmount;

                itemPiClosings.Add(items);
            }
            //ViewBag.PmProductionReward = itemPiClosings;

            var json = JsonConvert.SerializeObject(itemPiClosings);

            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult GetSoldOutRewardData(string employeeCode, string monthName, string monNum, string year)
        {
            List<NinetyFiveProductionRewardModel> itemPiClosings = new List<NinetyFiveProductionRewardModel>();
            var pmPiIncentives = _projectManagerRepository.GetSoldOutRewardData(employeeCode, monthName, monNum, year);

            foreach (var pmPoIncentiveModel in pmPiIncentives)
            {
                NinetyFiveProductionRewardModel items = new NinetyFiveProductionRewardModel();
                items.ProjectMasterID = pmPoIncentiveModel.ProjectMasterID;
                items.ProjectName = pmPoIncentiveModel.ProjectModel;
                items.ProjectModel = pmPoIncentiveModel.ProjectModel;
                items.EmployeeCode = pmPoIncentiveModel.EmployeeCode;
                items.UserFullName = pmPoIncentiveModel.UserFullName;
                items.Orders = pmPoIncentiveModel.Orders;
                items.WarehouseEntryDate = pmPoIncentiveModel.WarehouseEntryDate;
                items.ExtendedWarehouseDate = pmPoIncentiveModel.ExtendedWarehouseDate;
                items.ProjectManagerUserId = pmPoIncentiveModel.ProjectManagerUserId;
                items.OrderQuantity = pmPoIncentiveModel.OrderQuantity;
                items.TotalTblBarcodeIMEI = pmPoIncentiveModel.TotalTblBarcodeIMEI;
                items.TotalSalesOut = pmPoIncentiveModel.TotalSalesOut;
                items.EffectiveDays = pmPoIncentiveModel.EffectiveDays;
                items.RewardPercentage = pmPoIncentiveModel.RewardPercentage;
                items.ExistedPercentage = pmPoIncentiveModel.ExistedPercentage;
                items.RewardAmount = pmPoIncentiveModel.RewardAmount;

                itemPiClosings.Add(items);
            }

            var json = JsonConvert.SerializeObject(itemPiClosings);
            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult GetPmAndQcLsdToVesselData(string employeeCode, string monthName, string monNum, string year)
        {
            List<NinetyFiveProductionRewardModel> itemPiClosings = new List<NinetyFiveProductionRewardModel>();
            var pmPiIncentives = _projectManagerRepository.GetPmAndQcLsdToVesselData(employeeCode, monthName, monNum, year);

            foreach (var pmPoIncentiveModel in pmPiIncentives)
            {
                NinetyFiveProductionRewardModel items = new NinetyFiveProductionRewardModel();
                items.ProjectMasterID = pmPoIncentiveModel.ProjectMasterID;
                items.ProjectName = pmPoIncentiveModel.ProjectName;
                items.EmployeeCode = pmPoIncentiveModel.EmployeeCode;
                items.UserFullName = pmPoIncentiveModel.UserFullName;
                items.ProjectType = pmPoIncentiveModel.ProjectType;
                items.ProjectManagerUserId = pmPoIncentiveModel.ProjectManagerUserId;
                items.Orders = pmPoIncentiveModel.Orders;
                items.PoDate = pmPoIncentiveModel.PoDate;
                items.LSD = pmPoIncentiveModel.LSD;
                items.PoVsLSDDiff = pmPoIncentiveModel.PoVsLSDDiff;
                items.VesselDate = pmPoIncentiveModel.VesselDate;
                items.LsdVsVesselDiffForDeduct = pmPoIncentiveModel.LsdVsVesselDiffForDeduct;
                items.DeductPoint = pmPoIncentiveModel.DeductPoint;
                items.DeductedAmount = pmPoIncentiveModel.DeductedAmount;
                items.LsdVsVesselDiffForReward = pmPoIncentiveModel.LsdVsVesselDiffForReward;
                items.RewardPoint = pmPoIncentiveModel.RewardPoint;
                items.RewardAmount = pmPoIncentiveModel.RewardAmount;

                itemPiClosings.Add(items);
            }

            var json = JsonConvert.SerializeObject(itemPiClosings);
            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult GetPerPoRewardSumForPmHead(string employeeCode, string monthName, string monNum, string year)
        {
            List<NinetyFiveProductionRewardModel> itemPiClosings = new List<NinetyFiveProductionRewardModel>();
            var pmPiIncentives = _projectManagerRepository.GetPerPoRewardSumForPmHead(employeeCode, monthName, monNum, year);

            foreach (var pmPoIncentiveModel in pmPiIncentives)
            {
                NinetyFiveProductionRewardModel items = new NinetyFiveProductionRewardModel();
                items.PoReward = pmPoIncentiveModel.PoReward;
                items.EmployeeCode = employeeCode;
                items.Month = monthName;
                items.Year = year;
                itemPiClosings.Add(items);
            }

            var json = JsonConvert.SerializeObject(itemPiClosings);

            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult GetPmHeadPercentage(string employeeCode, string monthName, string monNum, string year)
        {
            List<Custom_Pm_IncentiveModel> itemPiClosings = new List<Custom_Pm_IncentiveModel>();
            var pmPiIncentives = _projectManagerRepository.GetPmHeadPercentage(employeeCode, monthName, monNum, year);

            foreach (var pmPoIncentiveModel in pmPiIncentives)
            {
                Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                items.TeamIncentive = pmPoIncentiveModel.TeamIncentive;
                items.InchargePecentage = pmPoIncentiveModel.InchargePecentage;
                items.Month = monthName;
                items.Year = year;
                itemPiClosings.Add(items);
            }

            var json = JsonConvert.SerializeObject(itemPiClosings);

            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult GetPenaltiesSumForPmHead(string employeeCode, string monthName, string monNum, string year)
        {
            List<NinetyFiveProductionRewardModel> itemPiClosings = new List<NinetyFiveProductionRewardModel>();
            var pmPiIncentives = _projectManagerRepository.GetPenaltiesSumForPmHead(employeeCode, monthName, monNum, year);

            foreach (var pmPoIncentiveModel in pmPiIncentives)
            {
                NinetyFiveProductionRewardModel items = new NinetyFiveProductionRewardModel();
                items.Penalties = pmPoIncentiveModel.Penalties;
                items.EmployeeCode = employeeCode;
                items.Month = monthName;
                items.Year = year;
                itemPiClosings.Add(items);
            }

            var json = JsonConvert.SerializeObject(itemPiClosings);

            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult PoDetailsPageForPmHead(string EmpCode, string Month, string MonNum, string Year)
        {
            var poDetailsPageForPmHead = _projectManagerRepository.GetPoDetailsPageForPmHead(EmpCode, Month, MonNum, Year);
            ViewBag.GetPoDetailsPageForPmHead = poDetailsPageForPmHead;

            return View();
        }
        public ActionResult RatioWisePenaltiesForPmHead(string EmpCode, string Month, string MonNum, string Year)
        {
            var poDetailsPageForPmHead = _projectManagerRepository.GetRatioWisePenaltiesForPmHead(EmpCode, Month, MonNum, Year);
            ViewBag.GetRatioWisePenaltiesForPmHead = poDetailsPageForPmHead;

            return View();
        }
        //end sept update 2019
        [HttpPost]
        public JsonResult SaveTotalIncentive(string EmpCode, string Month, string MonNum, string Year)
        {
            List<Custom_Pm_IncentiveModel> cmList = new List<Custom_Pm_IncentiveModel>();
            var pmIncentiveModels = _projectManagerRepository.GetPmIncentive(EmpCode, MonNum, Year);

            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
                cmList.Add(items);
            }
            pmIncentiveModels = _projectManagerRepository.GetPmPoIncentive(EmpCode, MonNum, Year);
            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
                cmList.Add(items);
            }
            pmIncentiveModels = _projectManagerRepository.GetPmDocIncentive(EmpCode, MonNum, Year);
            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
                cmList.Add(items);
            }
            pmIncentiveModels = _projectManagerRepository.GetPmProClosingIncentive(EmpCode, MonNum, Year);
            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
                cmList.Add(items);
            }
            pmIncentiveModels = _projectManagerRepository.GetPmRawUploadIncentive(EmpCode, MonNum, Year);
            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
                cmList.Add(items);
            }
            pmIncentiveModels = _projectManagerRepository.GetPmShipmentClearanceVsLsdIncentive(EmpCode, MonNum, Year);
            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
                cmList.Add(items);
            }
            #region com
            //old policy upto sept 2019 shipment related
            //pmIncentiveModels = _projectManagerRepository.GetPmShipIncentive(EmpCode, MonNum, Year);
            //foreach (var customPmIncentiveModel in pmIncentiveModels)
            //{
            //    Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
            //    items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
            //    cmList.Add(items);
            //}
            /////Accessories & Pi closing/////

            //pmIncentiveModels = _projectManagerRepository.GetPmAccessoriesFinalIncentive(EmpCode, MonNum, Year);
            //foreach (var customPmIncentiveModel in pmIncentiveModels)
            //{
            //    Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
            //    items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
            //    cmList.Add(items);
            //}

            //pmIncentiveModels = _projectManagerRepository.GetPmPiFinalIncentive(EmpCode, MonNum, Year);
            //foreach (var customPmIncentiveModel in pmIncentiveModels)
            //{
            //    Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
            //    items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
            //    cmList.Add(items);
            //}
            //2019 from sept new policy
            //pmIncentiveModels = _projectManagerRepository.GetPmSalesOutRewardIncentive(EmpCode, MonNum, Year);
            //foreach (var customPmIncentiveModel in pmIncentiveModels)
            //{
            //    Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
            //    items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
            //    cmList.Add(items);
            //}
            //pmIncentiveModels = _projectManagerRepository.GetPmSalesOutDeductIncentive(EmpCode, MonNum, Year);
            //foreach (var customPmIncentiveModel in pmIncentiveModels)
            //{
            //    Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
            //    items.Penalties = customPmIncentiveModel.Penalties;
            //    cmList.Add(items);
            //}
            //pmIncentiveModels = _projectManagerRepository.GetPmLsdToVesselRewardIncentive(EmpCode, MonNum, Year);
            //foreach (var customPmIncentiveModel in pmIncentiveModels)
            //{
            //    Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
            //    items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
            //    cmList.Add(items);
            //}
            //pmIncentiveModels = _projectManagerRepository.GetPmLsdToVesselPenaltiesIncentive(EmpCode, MonNum, Year);
            //foreach (var customPmIncentiveModel in pmIncentiveModels)
            //{
            //    Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
            //    items.Penalties = customPmIncentiveModel.Penalties;
            //    cmList.Add(items);
            //}
            //pm incharge penalties
            //pmIncentiveModels = _projectManagerRepository.GetSumOfVesselPenaltiesIncentive(EmpCode, MonNum, Year);
            //foreach (var customPmIncentiveModel in pmIncentiveModels)
            //{
            //    Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
            //    items.Penalties = customPmIncentiveModel.Penalties;
            //    cmList.Add(items);
            //}
            #endregion
            pmIncentiveModels = _projectManagerRepository.GetPmProductionRewardIncentive(EmpCode, MonNum, Year);
            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
                cmList.Add(items);
            }
            pmIncentiveModels = _projectManagerRepository.GetPmProductionDeductIncentive(EmpCode, MonNum, Year);
            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                items.Penalties = customPmIncentiveModel.Penalties;
                cmList.Add(items);
            }
        
            pmIncentiveModels = _projectManagerRepository.GetPmHead_PerPoIncentive(EmpCode, MonNum, Year);
            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
                cmList.Add(items);
            }
            //
            pmIncentiveModels = _projectManagerRepository.GetPmHead_TeamPercentInc(EmpCode, MonNum, Year);
            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
                cmList.Add(items);
            }
            //
            /////////////////////////////////////
            var totalAmount = cmList.Sum(i => i.FinalAmount1);
            var totalPenalties = cmList.Sum(i => i.Penalties);

            bool isExist = false;
            if (totalAmount != null)
            {
                isExist = _projectManagerRepository.GetTotalIncentiveData(EmpCode, Convert.ToInt32(MonNum), Year);
            }

            if (isExist)
            {
                TempData["Message2"] = "Incentive already generated";
                return Json("PmMonthlyIncentive", "ProjectManager");
            }

            var saveTotalIncentive = "0";

            if (totalAmount != null)
            {
                saveTotalIncentive = _projectManagerRepository.SaveTotalIncentive(totalAmount.ToString(), totalPenalties.ToString(), EmpCode, Month, MonNum, Year);

            }
            return Json(new { saveTotalIncentive }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PmIncentiveReportPerPerson(string EmpCode, string Month, string MonNum, string Year)
        {
            List<Custom_Pm_IncentiveModel> cmList = new List<Custom_Pm_IncentiveModel>();
            var pmIncentiveModels = _projectManagerRepository.GetPmIncentiveForPrint(EmpCode, MonNum, Year);

            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                items.ProjectName = customPmIncentiveModel.ProjectName;
                items.ShipmentType = customPmIncentiveModel.ShipmentType;
                items.ProjectType = customPmIncentiveModel.ProjectType;
                items.RewardPoint = customPmIncentiveModel.RewardPoint;
                items.RewardAmount = customPmIncentiveModel.RewardAmount;
                items.PoCategory = customPmIncentiveModel.PoCategory;
                items.PoDate = customPmIncentiveModel.PoDate;
                items.LSD = customPmIncentiveModel.LSD;
                items.PoVsLSDDiff = customPmIncentiveModel.PoVsLSDDiff;
                items.VesselDate = customPmIncentiveModel.VesselDate;
                items.LsdVsVesselDiffForDeduct = customPmIncentiveModel.LsdVsVesselDiffForDeduct;
                items.DeductPoint = customPmIncentiveModel.DeductPoint;
                items.FeatureBase = customPmIncentiveModel.FeatureBase;
                items.SmartBase = customPmIncentiveModel.SmartBase;
                items.PoReward = customPmIncentiveModel.PoReward;
                items.PerDayDeduction = customPmIncentiveModel.PerDayDeduction;
                items.WarehouseEntryDate = customPmIncentiveModel.WarehouseEntryDate;
                items.ExtendedWarehouseDate = customPmIncentiveModel.ExtendedWarehouseDate;
                items.OrderQuantity = customPmIncentiveModel.OrderQuantity;
                items.TotalSalesOut = customPmIncentiveModel.TotalSalesOut;
                items.ExistedPercentage = customPmIncentiveModel.ExistedPercentage;
                //items.ShipmentTaken = customPmIncentiveModel.ShipmentTaken;
                //items.EarlierOrLateShipment = customPmIncentiveModel.EarlierOrLateShipment;
                items.Orders = Convert.ToString(customPmIncentiveModel.OrderNumber) + " Order";
                items.Others = customPmIncentiveModel.Others;
                items.PersonNo = customPmIncentiveModel.PersonNo;
                items.Amount1 = customPmIncentiveModel.Amount1;
                items.Remarks = customPmIncentiveModel.Remarks;
                items.DeductionAmount1 = customPmIncentiveModel.DeductionAmount1;
                items.D_Remarks = customPmIncentiveModel.D_Remarks;
                items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
                items.IncsTypes = customPmIncentiveModel.IncsTypes;
                items.AccessoriesType = customPmIncentiveModel.AccessoriesType;
                items.ProjectClosingDate = customPmIncentiveModel.ProjectClosingDate;
                items.MarketClearanceDate = customPmIncentiveModel.MarketClearanceDate;
                items.ProjectManagerClearanceDate = customPmIncentiveModel.ProjectManagerClearanceDate;
                items.RawMaterialAddedDate = customPmIncentiveModel.RawMaterialAddedDate;
                items.DaysPassed = customPmIncentiveModel.DaysPassed;
                //items.Reward = items.Reward;
                //items.Penalties = items.Penalties;
                cmList.Add(items);
            }
            pmIncentiveModels = _projectManagerRepository.GetPmPoIncentiveForPrint(EmpCode, MonNum, Year);
            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                items.ProjectName = customPmIncentiveModel.ProjectName;
                items.ShipmentType = customPmIncentiveModel.ShipmentType;
                items.ProjectType = customPmIncentiveModel.ProjectType;
                items.RewardPoint = customPmIncentiveModel.RewardPoint;
                items.RewardAmount = customPmIncentiveModel.RewardAmount;
                items.PoCategory = customPmIncentiveModel.PoCategory;
                items.PoDate = customPmIncentiveModel.PoDate;
                items.LSD = customPmIncentiveModel.LSD;
                items.PoVsLSDDiff = customPmIncentiveModel.PoVsLSDDiff;
                items.VesselDate = customPmIncentiveModel.VesselDate;
                items.LsdVsVesselDiffForDeduct = customPmIncentiveModel.LsdVsVesselDiffForDeduct;
                items.DeductPoint = customPmIncentiveModel.DeductPoint;
                items.FeatureBase = customPmIncentiveModel.FeatureBase;
                items.SmartBase = customPmIncentiveModel.SmartBase;
                items.PoReward = customPmIncentiveModel.PoReward;
                items.PerDayDeduction = customPmIncentiveModel.PerDayDeduction;
                items.WarehouseEntryDate = customPmIncentiveModel.WarehouseEntryDate;
                items.ExtendedWarehouseDate = customPmIncentiveModel.ExtendedWarehouseDate;
                items.OrderQuantity = customPmIncentiveModel.OrderQuantity;
                items.TotalSalesOut = customPmIncentiveModel.TotalSalesOut;
                items.ExistedPercentage = customPmIncentiveModel.ExistedPercentage;
                //items.ShipmentTaken = customPmIncentiveModel.ShipmentTaken;
                //items.EarlierOrLateShipment = customPmIncentiveModel.EarlierOrLateShipment;
                items.Orders = Convert.ToString(customPmIncentiveModel.OrderNumber) + " Order";
                items.Others = customPmIncentiveModel.Others;
                items.PersonNo = customPmIncentiveModel.PersonNo;
                items.Amount1 = customPmIncentiveModel.Amount1;
                items.Remarks = customPmIncentiveModel.Remarks;
                items.DeductionAmount1 = customPmIncentiveModel.DeductionAmount1;
                items.D_Remarks = customPmIncentiveModel.D_Remarks;
                items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
                items.IncsTypes = customPmIncentiveModel.IncsTypes;
                items.AccessoriesType = customPmIncentiveModel.AccessoriesType;
                items.ProjectClosingDate = customPmIncentiveModel.ProjectClosingDate;
                items.MarketClearanceDate = customPmIncentiveModel.MarketClearanceDate;
                items.ProjectManagerClearanceDate = customPmIncentiveModel.ProjectManagerClearanceDate;
                items.RawMaterialAddedDate = customPmIncentiveModel.RawMaterialAddedDate;
                items.DaysPassed = customPmIncentiveModel.DaysPassed;
                //items.Reward = items.Reward;
                //items.Penalties = items.Penalties;
                cmList.Add(items);
            }
            pmIncentiveModels = _projectManagerRepository.GetPm_DocumentUploadIncentiveForPrint(EmpCode, MonNum, Year);
            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                items.ProjectName = customPmIncentiveModel.ProjectName;
                items.ShipmentType = customPmIncentiveModel.ShipmentType;
                items.ProjectType = customPmIncentiveModel.ProjectType;
                items.RewardPoint = customPmIncentiveModel.RewardPoint;
                items.RewardAmount = customPmIncentiveModel.RewardAmount;
                items.PoCategory = customPmIncentiveModel.PoCategory;
                items.PoDate = customPmIncentiveModel.PoDate;
                items.LSD = customPmIncentiveModel.LSD;
                items.PoVsLSDDiff = customPmIncentiveModel.PoVsLSDDiff;
                items.VesselDate = customPmIncentiveModel.VesselDate;
                items.LsdVsVesselDiffForDeduct = customPmIncentiveModel.LsdVsVesselDiffForDeduct;
                items.DeductPoint = customPmIncentiveModel.DeductPoint;
                items.FeatureBase = customPmIncentiveModel.FeatureBase;
                items.SmartBase = customPmIncentiveModel.SmartBase;
                items.PoReward = customPmIncentiveModel.PoReward;
                items.PerDayDeduction = customPmIncentiveModel.PerDayDeduction;
                items.WarehouseEntryDate = customPmIncentiveModel.WarehouseEntryDate;
                items.ExtendedWarehouseDate = customPmIncentiveModel.ExtendedWarehouseDate;
                items.OrderQuantity = customPmIncentiveModel.OrderQuantity;
                items.TotalSalesOut = customPmIncentiveModel.TotalSalesOut;
                items.ExistedPercentage = customPmIncentiveModel.ExistedPercentage;
                //items.ShipmentTaken = customPmIncentiveModel.ShipmentTaken;
                //items.EarlierOrLateShipment = customPmIncentiveModel.EarlierOrLateShipment;
                items.Orders = customPmIncentiveModel.Orders;
                items.Others = customPmIncentiveModel.Others;
                items.PersonNo = customPmIncentiveModel.PersonNo;
                items.Amount1 = customPmIncentiveModel.Amount1;
                items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
                items.Remarks = customPmIncentiveModel.Remarks;
                items.DeductionAmount1 = customPmIncentiveModel.DeductionAmount1;
                items.D_Remarks = customPmIncentiveModel.D_Remarks;
               
                items.IncsTypes = customPmIncentiveModel.IncsTypes;
                items.AccessoriesType = customPmIncentiveModel.AccessoriesType;
                items.ProjectClosingDate = customPmIncentiveModel.ProjectClosingDate;
                items.MarketClearanceDate = customPmIncentiveModel.MarketClearanceDate;
                items.ProjectManagerClearanceDate = customPmIncentiveModel.ProjectManagerClearanceDate;
                items.RawMaterialAddedDate = customPmIncentiveModel.RawMaterialAddedDate;
                items.DaysPassed = customPmIncentiveModel.DaysPassed;
                //items.Reward = items.Reward;
                //items.Penalties = items.Penalties;
                cmList.Add(items);
            }
            pmIncentiveModels = _projectManagerRepository.GetPm_ProjectClosingIncentiveForPrint(EmpCode, MonNum, Year);
            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                items.ProjectName = customPmIncentiveModel.ProjectName;
                items.ShipmentType = customPmIncentiveModel.ShipmentType;
                items.ProjectType = customPmIncentiveModel.ProjectType;
                items.RewardPoint = customPmIncentiveModel.RewardPoint;
                items.RewardAmount = customPmIncentiveModel.RewardAmount;
                items.PoCategory = customPmIncentiveModel.PoCategory;
                items.PoDate = customPmIncentiveModel.PoDate;
                items.LSD = customPmIncentiveModel.LSD;
                items.PoVsLSDDiff = customPmIncentiveModel.PoVsLSDDiff;
                items.VesselDate = customPmIncentiveModel.VesselDate;
                items.LsdVsVesselDiffForDeduct = customPmIncentiveModel.LsdVsVesselDiffForDeduct;
                items.DeductPoint = customPmIncentiveModel.DeductPoint;
                items.FeatureBase = customPmIncentiveModel.FeatureBase;
                items.SmartBase = customPmIncentiveModel.SmartBase;
                items.PoReward = customPmIncentiveModel.PoReward;
                items.PerDayDeduction = customPmIncentiveModel.PerDayDeduction;
                items.WarehouseEntryDate = customPmIncentiveModel.WarehouseEntryDate;
                items.ExtendedWarehouseDate = customPmIncentiveModel.ExtendedWarehouseDate;
                items.OrderQuantity = customPmIncentiveModel.OrderQuantity;
                items.TotalSalesOut = customPmIncentiveModel.TotalSalesOut;
                items.ExistedPercentage = customPmIncentiveModel.ExistedPercentage;
                //items.ShipmentTaken = customPmIncentiveModel.ShipmentTaken;
                //items.EarlierOrLateShipment = customPmIncentiveModel.EarlierOrLateShipment;
                items.Orders = customPmIncentiveModel.Orders;
                items.Others = customPmIncentiveModel.Others;
                items.PersonNo = customPmIncentiveModel.PersonNo;
                items.Amount1 = customPmIncentiveModel.Amount1;
                items.Remarks = customPmIncentiveModel.Remarks;
                items.DeductionAmount1 = customPmIncentiveModel.DeductionAmount1;
                items.D_Remarks = customPmIncentiveModel.D_Remarks;
                items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
                items.IncsTypes = customPmIncentiveModel.IncsTypes;
                items.AccessoriesType = customPmIncentiveModel.AccessoriesType;
                items.ProjectClosingDate = customPmIncentiveModel.ProjectClosingDate;
                items.MarketClearanceDate = customPmIncentiveModel.MarketClearanceDate;
                items.ProjectManagerClearanceDate = customPmIncentiveModel.ProjectManagerClearanceDate;
                items.RawMaterialAddedDate = customPmIncentiveModel.RawMaterialAddedDate;
                items.DaysPassed = customPmIncentiveModel.DaysPassed;
                //items.Reward = items.Reward;
                //items.Penalties = items.Penalties;
                cmList.Add(items);
            }
            pmIncentiveModels = _projectManagerRepository.GetPm_RawMaterialUpDelayPenaltiesForPrint(EmpCode, MonNum, Year);
            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                items.ProjectName = customPmIncentiveModel.ProjectName;
                items.ShipmentType = customPmIncentiveModel.ShipmentType;
                items.ProjectType = customPmIncentiveModel.ProjectType;
                items.RewardPoint = customPmIncentiveModel.RewardPoint;
                items.RewardAmount = customPmIncentiveModel.RewardAmount;
                items.PoCategory = customPmIncentiveModel.PoCategory;
                items.PoDate = customPmIncentiveModel.PoDate;
                items.LSD = customPmIncentiveModel.LSD;
                items.PoVsLSDDiff = customPmIncentiveModel.PoVsLSDDiff;
                items.VesselDate = customPmIncentiveModel.VesselDate;
                items.LsdVsVesselDiffForDeduct = customPmIncentiveModel.LsdVsVesselDiffForDeduct;
                items.DeductPoint = customPmIncentiveModel.DeductPoint;
                items.FeatureBase = customPmIncentiveModel.FeatureBase;
                items.SmartBase = customPmIncentiveModel.SmartBase;
                items.PoReward = customPmIncentiveModel.PoReward;
                items.PerDayDeduction = customPmIncentiveModel.PerDayDeduction;
                items.WarehouseEntryDate = customPmIncentiveModel.WarehouseEntryDate;
                items.ExtendedWarehouseDate = customPmIncentiveModel.ExtendedWarehouseDate;
                items.OrderQuantity = customPmIncentiveModel.PoQuantity;
                items.TotalSalesOut = customPmIncentiveModel.TotalSalesOut;
                items.ExistedPercentage = customPmIncentiveModel.ExistedPercentage;
                //items.ShipmentTaken = customPmIncentiveModel.ShipmentTaken;
                //items.EarlierOrLateShipment = customPmIncentiveModel.EarlierOrLateShipment;
                items.Orders = customPmIncentiveModel.Orders;
                items.Others = customPmIncentiveModel.Others;
                items.PersonNo = customPmIncentiveModel.PersonNo;
                items.Amount1 = customPmIncentiveModel.Amount1;
                items.Remarks = customPmIncentiveModel.Remarks;
                items.DeductionAmount1 = customPmIncentiveModel.DeductionAmount1;
                items.D_Remarks = customPmIncentiveModel.D_Remarks;
                items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
                items.IncsTypes = customPmIncentiveModel.IncsTypes;
                items.AccessoriesType = customPmIncentiveModel.AccessoriesType;
                items.ProjectClosingDate = customPmIncentiveModel.ProjectClosingDate;
                items.MarketClearanceDate = customPmIncentiveModel.MarketClearanceDate;
                items.ProjectManagerClearanceDate = customPmIncentiveModel.ProjectManagerClearanceDate;
                items.RawMaterialAddedDate = customPmIncentiveModel.RawMaterialAddedDate;
                items.DaysPassed = customPmIncentiveModel.DaysPassed;
               // items.Reward = items.Reward;
                //items.Penalties = items.Penalties;
                cmList.Add(items);
            }
            pmIncentiveModels = _projectManagerRepository.GetPm_ShipmentClearanceVsLsdForPrint(EmpCode, MonNum, Year);
            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                items.ProjectName = customPmIncentiveModel.ProjectName;
                items.ShipmentType = customPmIncentiveModel.ShipmentType;
                items.ProjectType = customPmIncentiveModel.ProjectType;
                items.RewardPoint = customPmIncentiveModel.RewardPoint;
                items.RewardAmount = Convert.ToInt64(customPmIncentiveModel.Reward);
                items.PoCategory = customPmIncentiveModel.PoCategory;
                items.PoDate = customPmIncentiveModel.PoDate;
                items.LSD = customPmIncentiveModel.LSD;
                items.PoVsLSDDiff = customPmIncentiveModel.PoVsLSDDiff;
                items.VesselDate = customPmIncentiveModel.VesselDate;
                items.LsdVsVesselDiffForDeduct = customPmIncentiveModel.LsdVsVesselDiffForDeduct;
                items.DeductPoint = Convert.ToInt64(customPmIncentiveModel.Penalties);
                items.FeatureBase = customPmIncentiveModel.FeatureBase;
                items.SmartBase = customPmIncentiveModel.SmartBase;
                items.PoReward = customPmIncentiveModel.PoReward;
                items.PerDayDeduction = customPmIncentiveModel.PerDayDeduction;
                items.WarehouseEntryDate = customPmIncentiveModel.WarehouseEntryDate;
                items.ExtendedWarehouseDate = customPmIncentiveModel.ExtendedWarehouseDate;
                items.OrderQuantity = customPmIncentiveModel.PoQuantity;
                items.TotalSalesOut = customPmIncentiveModel.TotalSalesOut;
                items.ExistedPercentage = customPmIncentiveModel.ExistedPercentage;
                //items.ShipmentTaken = customPmIncentiveModel.ShipmentTaken;
                //items.EarlierOrLateShipment = customPmIncentiveModel.EarlierOrLateShipment;
                items.Orders = customPmIncentiveModel.Orders;
                items.Others = customPmIncentiveModel.Others;
                items.PersonNo = customPmIncentiveModel.PersonNo;
                items.Amount1 = customPmIncentiveModel.Amount1;
                items.Remarks = customPmIncentiveModel.Remarks;
                items.DeductionAmount1 = customPmIncentiveModel.Penalties;
                items.D_Remarks = customPmIncentiveModel.D_Remarks;
                items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
                items.IncsTypes = customPmIncentiveModel.IncsTypes;
                items.AccessoriesType = customPmIncentiveModel.AccessoriesType;
                items.ProjectClosingDate = customPmIncentiveModel.ProjectClosingDate;
                items.MarketClearanceDate = customPmIncentiveModel.MarketClearanceDate;
                items.ProjectManagerClearanceDate = customPmIncentiveModel.ProjectManagerClearanceDate;
                items.RawMaterialAddedDate = customPmIncentiveModel.RawMaterialAddedDate;
                items.DaysPassed = customPmIncentiveModel.DaysPassed;
               // items.Reward = items.Reward;
                //items.Penalties = items.Penalties;

                cmList.Add(items);
            }
            #region com
            //pmIncentiveModels = _projectManagerRepository.GetPmShipIncentiveForPrint(EmpCode, MonNum, Year);
            //foreach (var customPmIncentiveModel in pmIncentiveModels)
            //{
            //    Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
            //    items.ProjectName = customPmIncentiveModel.ProjectName;
            //    items.PoCategory = customPmIncentiveModel.PoCategory;
            //    items.PoDate = customPmIncentiveModel.PoDate;
            //    items.LSD = customPmIncentiveModel.LSD;
            //    items.ShipmentTaken = customPmIncentiveModel.ShipmentTaken;
            //    items.EarlierOrLateShipment = customPmIncentiveModel.EarlierOrLateShipment;
            //    items.OrderNumber = customPmIncentiveModel.OrderNumber;
            //    items.Others = customPmIncentiveModel.Others;
            //    items.PersonNo = customPmIncentiveModel.PersonNo;
            //    items.Amount1 = customPmIncentiveModel.Amount1;
            //    items.Remarks = customPmIncentiveModel.Remarks;
            //    items.DeductionAmount1 = customPmIncentiveModel.DeductionAmount1;
            //    items.D_Remarks = customPmIncentiveModel.D_Remarks;
            //    items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
            //    cmList.Add(items);
            //}
            //pmIncentiveModels = _projectManagerRepository.GetPmAccessoriesIncentiveForPrint(EmpCode, MonNum, Year);
            //foreach (var customPmIncentiveModel in pmIncentiveModels)
            //{
            //    Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
            //    items.ProjectName = customPmIncentiveModel.ProjectName;
            //    items.ShipmentType = customPmIncentiveModel.ShipmentType;
            //    items.ProjectType = customPmIncentiveModel.ProjectType;
            //    items.RewardPoint = customPmIncentiveModel.RewardPoint;
            //    items.RewardAmount = customPmIncentiveModel.RewardAmount;
            //    items.PoCategory = customPmIncentiveModel.PoCategory;
            //    items.PoDate = customPmIncentiveModel.PoDate;
            //    items.LSD = customPmIncentiveModel.LSD;
            //    items.PoVsLSDDiff = customPmIncentiveModel.PoVsLSDDiff;
            //    items.VesselDate = customPmIncentiveModel.VesselDate;
            //    items.LsdVsVesselDiffForDeduct = customPmIncentiveModel.LsdVsVesselDiffForDeduct;
            //    items.DeductPoint = customPmIncentiveModel.DeductPoint;
            //    items.FeatureBase = customPmIncentiveModel.FeatureBase;
            //    items.SmartBase = customPmIncentiveModel.SmartBase;
            //    items.PoReward = customPmIncentiveModel.PoReward;
            //    items.PerDayDeduction = customPmIncentiveModel.PerDayDeduction;
            //    items.WarehouseEntryDate = customPmIncentiveModel.WarehouseEntryDate;
            //    items.ExtendedWarehouseDate = customPmIncentiveModel.ExtendedWarehouseDate;
            //    items.OrderQuantity = customPmIncentiveModel.OrderQuantity;
            //    items.TotalSalesOut = customPmIncentiveModel.TotalSalesOut;
            //    items.ExistedPercentage = customPmIncentiveModel.ExistedPercentage;
            //    //items.ShipmentTaken = customPmIncentiveModel.ShipmentTaken;
            //    //items.EarlierOrLateShipment = customPmIncentiveModel.EarlierOrLateShipment;
            //    items.Orders = Convert.ToString(customPmIncentiveModel.OrderNumber) + " Order";
            //    items.Others = customPmIncentiveModel.Others;
            //    items.PersonNo = customPmIncentiveModel.PersonNo;
            //    items.Amount1 = customPmIncentiveModel.Amount1;
            //    items.Remarks = customPmIncentiveModel.Remarks;
            //    items.DeductionAmount1 = customPmIncentiveModel.DeductionAmount1;
            //    items.D_Remarks = customPmIncentiveModel.D_Remarks;
            //    items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
            //    items.IncsTypes = customPmIncentiveModel.IncsTypes;
            //    cmList.Add(items);
            //}

            //pmIncentiveModels = _projectManagerRepository.GetPmPiIncentiveForPrint(EmpCode, MonNum, Year);
            //foreach (var customPmIncentiveModel in pmIncentiveModels)
            //{
            //    Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
            //    items.ProjectName = customPmIncentiveModel.ProjectName;
            //    items.ShipmentType = customPmIncentiveModel.ShipmentType;
            //    items.ProjectType = customPmIncentiveModel.ProjectType;
            //    items.RewardPoint = customPmIncentiveModel.RewardPoint;
            //    items.RewardAmount = customPmIncentiveModel.RewardAmount;
            //    items.PoCategory = customPmIncentiveModel.PoCategory;
            //    items.PoDate = customPmIncentiveModel.PoDate;
            //    items.LSD = customPmIncentiveModel.LSD;
            //    items.PoVsLSDDiff = customPmIncentiveModel.PoVsLSDDiff;
            //    items.VesselDate = customPmIncentiveModel.VesselDate;
            //    items.LsdVsVesselDiffForDeduct = customPmIncentiveModel.LsdVsVesselDiffForDeduct;
            //    items.DeductPoint = customPmIncentiveModel.DeductPoint;
            //    items.FeatureBase = customPmIncentiveModel.FeatureBase;
            //    items.SmartBase = customPmIncentiveModel.SmartBase;
            //    items.PoReward = customPmIncentiveModel.PoReward;
            //    items.PerDayDeduction = customPmIncentiveModel.PerDayDeduction;
            //    items.WarehouseEntryDate = customPmIncentiveModel.WarehouseEntryDate;
            //    items.ExtendedWarehouseDate = customPmIncentiveModel.ExtendedWarehouseDate;
            //    items.OrderQuantity = customPmIncentiveModel.OrderQuantity;
            //    items.TotalSalesOut = customPmIncentiveModel.TotalSalesOut;
            //    items.ExistedPercentage = customPmIncentiveModel.ExistedPercentage;
            //    //items.ShipmentTaken = customPmIncentiveModel.ShipmentTaken;
            //    //items.EarlierOrLateShipment = customPmIncentiveModel.EarlierOrLateShipment;
            //    items.Orders = Convert.ToString(customPmIncentiveModel.OrderNumber) + " Order";
            //    items.Others = customPmIncentiveModel.Others;
            //    items.PersonNo = customPmIncentiveModel.PersonNo;
            //    items.Amount1 = customPmIncentiveModel.Amount1;
            //    items.Remarks = customPmIncentiveModel.Remarks;
            //    items.DeductionAmount1 = customPmIncentiveModel.DeductionAmount1;
            //    items.D_Remarks = customPmIncentiveModel.D_Remarks;
            //    items.FinalAmount1 = customPmIncentiveModel.FinalAmount1;
            //    items.IncsTypes = customPmIncentiveModel.IncsTypes;
            //    cmList.Add(items);
            //}

            //pmIncentiveModels = _projectManagerRepository.GetSalesOutIncentiveForPrint(EmpCode, MonNum, Year);
            //foreach (var customPmIncentiveModel in pmIncentiveModels)
            //{
            //    Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
            //    items.ProjectName = customPmIncentiveModel.ProjectName;
            //    items.ShipmentType = customPmIncentiveModel.ShipmentType;
            //    items.ProjectType = customPmIncentiveModel.ProjectType;
            //    items.RewardPoint = customPmIncentiveModel.RewardPoint;
            //    items.RewardAmount = customPmIncentiveModel.RewardAmount;
            //    items.WarehouseEntryDate = customPmIncentiveModel.WarehouseEntryDate;
            //    items.ExtendedWarehouseDate = customPmIncentiveModel.ExtendedWarehouseDate;
            //    items.OrderQuantity = customPmIncentiveModel.OrderQuantity;
            //    items.TotalSalesOut = customPmIncentiveModel.TotalSalesOut;
            //    items.ExistedPercentage = customPmIncentiveModel.ExistedPercentage;
            //    items.PoCategory = customPmIncentiveModel.PoCategory;
            //    items.PoDate = customPmIncentiveModel.PoDate;
            //    items.LSD = customPmIncentiveModel.LSD;
            //    items.PoVsLSDDiff = customPmIncentiveModel.PoVsLSDDiff;
            //    items.VesselDate = customPmIncentiveModel.VesselDate;
            //    items.LsdVsVesselDiffForDeduct = customPmIncentiveModel.LsdVsVesselDiffForDeduct;
            //    items.DeductPoint = customPmIncentiveModel.DeductPoint;
            //    items.FeatureBase = customPmIncentiveModel.FeatureBase;
            //    items.SmartBase = customPmIncentiveModel.SmartBase;
            //    items.PoReward = customPmIncentiveModel.PoReward;
            //    items.PerDayDeduction = customPmIncentiveModel.PerDayDeduction;

            //    //items.ShipmentTaken = customPmIncentiveModel.ShipmentTaken;
            //    //items.EarlierOrLateShipment = customPmIncentiveModel.EarlierOrLateShipment;
            //    items.Orders = Convert.ToString(customPmIncentiveModel.Orders);
            //    items.Others = customPmIncentiveModel.Others;
            //    items.PersonNo = customPmIncentiveModel.PersonNo;
            //    items.Amount1 = Convert.ToDecimal(customPmIncentiveModel.RewardAmount);
            //    items.Remarks = customPmIncentiveModel.Remarks;
            //    items.DeductionAmount1 = customPmIncentiveModel.DeductionAmount1;
            //    items.D_Remarks = customPmIncentiveModel.D_Remarks;
            //    items.FinalAmount1 = Convert.ToDecimal(customPmIncentiveModel.FinalAmount2);
            //    items.IncsTypes = customPmIncentiveModel.IncsTypes;
            //    cmList.Add(items);
            //}
            //pmIncentiveModels = _projectManagerRepository.GetVesselRewardOrPenaltiesForPrint(EmpCode, MonNum, Year);
            //foreach (var customPmIncentiveModel in pmIncentiveModels)
            //{
            //    Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
            //    items.ProjectName = customPmIncentiveModel.ProjectName;
            //    items.ProjectType = customPmIncentiveModel.ProjectType;
            //    items.ShipmentType = customPmIncentiveModel.ShipmentType;
            //    items.RewardPoint = customPmIncentiveModel.RewardPoint;
            //    items.RewardAmount = customPmIncentiveModel.RewardAmount;
            //    items.PoCategory = customPmIncentiveModel.PoCategory;
            //    items.PoDate = customPmIncentiveModel.PoDate;
            //    items.LSD = customPmIncentiveModel.LSD;
            //    items.PoVsLSDDiff = customPmIncentiveModel.PoVsLSDDiff;
            //    items.VesselDate = customPmIncentiveModel.VesselDate;
            //    items.LsdVsVesselDiffForDeduct = customPmIncentiveModel.LsdVsVesselDiffForDeduct;
            //    items.DeductPoint = customPmIncentiveModel.DeductPoint;
            //    items.FeatureBase = customPmIncentiveModel.FeatureBase;
            //    items.SmartBase = customPmIncentiveModel.SmartBase;
            //    items.PoReward = customPmIncentiveModel.PoReward;
            //    items.PerDayDeduction = customPmIncentiveModel.PerDayDeduction;
            //    items.WarehouseEntryDate = customPmIncentiveModel.WarehouseEntryDate;
            //    items.ExtendedWarehouseDate = customPmIncentiveModel.ExtendedWarehouseDate;
            //    items.OrderQuantity = customPmIncentiveModel.OrderQuantity;
            //    items.TotalSalesOut = customPmIncentiveModel.TotalSalesOut;
            //    items.ExistedPercentage = customPmIncentiveModel.ExistedPercentage;
            //    //items.ShipmentTaken = customPmIncentiveModel.ShipmentTaken;
            //    //items.EarlierOrLateShipment = customPmIncentiveModel.EarlierOrLateShipment;
            //    items.Orders = Convert.ToString(customPmIncentiveModel.OrderNumber);
            //    items.Others = customPmIncentiveModel.Others;
            //    items.PersonNo = customPmIncentiveModel.PersonNo;
            //    items.Amount1 = Convert.ToDecimal(customPmIncentiveModel.RewardAmount);
            //    items.Remarks = customPmIncentiveModel.Remarks;
            //    items.DeductionAmount1 = customPmIncentiveModel.DeductedAmount;
            //    items.D_Remarks = customPmIncentiveModel.D_Remarks;
            //    if (items.RewardAmount != 0)
            //    {
            //        items.FinalAmount1 = Convert.ToDecimal(items.RewardAmount);
            //    }
            //    else if (items.DeductionAmount1 != 0)
            //    {
            //        items.FinalAmount1 = -Convert.ToDecimal(items.DeductionAmount1);
            //    }

            //    items.IncsTypes = customPmIncentiveModel.IncsTypes;
            //    cmList.Add(items);
            //}

            //pmIncentiveModels = _projectManagerRepository.GetPmHeadPerPoIncentiveForPrint(EmpCode, MonNum, Year);
            //foreach (var customPmIncentiveModel in pmIncentiveModels)
            //{
            //    Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
            //    items.ProjectName = customPmIncentiveModel.ProjectName;
            //    items.ShipmentType = customPmIncentiveModel.ShipmentType;
            //    items.ProjectType = customPmIncentiveModel.ProjectType;
            //    items.RewardPoint = customPmIncentiveModel.PoReward;
            //    items.RewardAmount = customPmIncentiveModel.RewardAmount;
            //    items.PoCategory = customPmIncentiveModel.PoCategory;
            //    items.PoDate = customPmIncentiveModel.PoDate;
            //    items.LSD = customPmIncentiveModel.LSD;
            //    items.PoVsLSDDiff = customPmIncentiveModel.PoVsLSDDiff;
            //    items.VesselDate = customPmIncentiveModel.VesselDate;
            //    items.LsdVsVesselDiffForDeduct = customPmIncentiveModel.LsdVsVesselDiffForDeduct;
            //    items.DeductPoint = customPmIncentiveModel.DeductPoint;
            //    items.FeatureBase = customPmIncentiveModel.FeatureBase;
            //    items.SmartBase = customPmIncentiveModel.SmartBase;
            //    items.PoReward = customPmIncentiveModel.PoReward;
            //    items.PerDayDeduction = customPmIncentiveModel.PerDayDeduction;
            //    items.WarehouseEntryDate = customPmIncentiveModel.WarehouseEntryDate;
            //    items.ExtendedWarehouseDate = customPmIncentiveModel.ExtendedWarehouseDate;
            //    items.OrderQuantity = customPmIncentiveModel.OrderQuantity;
            //    items.TotalSalesOut = customPmIncentiveModel.TotalSalesOut;
            //    items.ExistedPercentage = customPmIncentiveModel.ExistedPercentage;
            //    //items.ShipmentTaken = customPmIncentiveModel.ShipmentTaken;
            //    //items.EarlierOrLateShipment = customPmIncentiveModel.EarlierOrLateShipment;
            //    items.Orders = customPmIncentiveModel.Orders;
            //    items.Others = customPmIncentiveModel.Others;
            //    items.PersonNo = customPmIncentiveModel.PersonNo;
            //    items.Amount1 = Convert.ToDecimal(customPmIncentiveModel.PoReward);
            //    items.Remarks = customPmIncentiveModel.Remarks;
            //    items.DeductionAmount1 = customPmIncentiveModel.DeductionAmount1;
            //    items.D_Remarks = customPmIncentiveModel.D_Remarks;
            //    items.FinalAmount1 = Convert.ToDecimal(customPmIncentiveModel.PoReward);
            //    items.IncsTypes = customPmIncentiveModel.IncsTypes;
            //    cmList.Add(items);
            //}
            //pmIncentiveModels = _projectManagerRepository.GetPmHeadVesselPenaltiesForPrint(EmpCode, MonNum, Year);
            //foreach (var customPmIncentiveModel in pmIncentiveModels)
            //{
            //    Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
            //    items.ProjectName = customPmIncentiveModel.ProjectName;
            //    items.ShipmentType = customPmIncentiveModel.ShipmentType;
            //    items.ProjectType = customPmIncentiveModel.ProjectType;
            //    items.RewardPoint = customPmIncentiveModel.RewardPoint;
            //    items.RewardAmount = customPmIncentiveModel.RewardAmount;
            //    items.PoCategory = customPmIncentiveModel.PoCategory;
            //    items.ProjectType = customPmIncentiveModel.ProjectType;
            //    items.PoDate = customPmIncentiveModel.PoDate;
            //    items.LSD = customPmIncentiveModel.LSD;
            //    items.PoVsLSDDiff = customPmIncentiveModel.PoVsLSDDiff;
            //    items.VesselDate = customPmIncentiveModel.VesselDate;
            //    items.LsdVsVesselDiffForDeduct = customPmIncentiveModel.LsdVsVesselDiffForDeduct;
            //    items.DeductPoint = customPmIncentiveModel.DeductPoint;
            //    items.FeatureBase = customPmIncentiveModel.FeatureBase;
            //    items.SmartBase = customPmIncentiveModel.SmartBase;
            //    items.PoReward = customPmIncentiveModel.PoReward;
            //    items.PerDayDeduction = customPmIncentiveModel.PerDayDeduction;
            //    items.WarehouseEntryDate = customPmIncentiveModel.WarehouseEntryDate;
            //    items.ExtendedWarehouseDate = customPmIncentiveModel.ExtendedWarehouseDate;
            //    items.OrderQuantity = customPmIncentiveModel.OrderQuantity;
            //    items.TotalSalesOut = customPmIncentiveModel.TotalSalesOut;
            //    items.ExistedPercentage = customPmIncentiveModel.ExistedPercentage;
            //    //items.Penalties = customPmIncentiveModel.TotalPenalties;

            //    //items.ShipmentTaken = customPmIncentiveModel.ShipmentTaken;
            //    //items.EarlierOrLateShipment = customPmIncentiveModel.EarlierOrLateShipment;
            //    items.Orders = customPmIncentiveModel.Orders;
            //    items.Others = customPmIncentiveModel.Others;
            //    items.PersonNo = customPmIncentiveModel.PersonNo;
            //    items.Amount1 = customPmIncentiveModel.Amount1;
            //    items.Remarks = customPmIncentiveModel.Remarks;
            //    items.DeductionAmount1 = customPmIncentiveModel.Penalties;
            //    items.D_Remarks = customPmIncentiveModel.D_Remarks;
            //    items.FinalAmount1 = -Convert.ToDecimal(customPmIncentiveModel.Penalties);
            //    items.IncsTypes = customPmIncentiveModel.IncsTypes;
            //    cmList.Add(items);
            //}
            #endregion

            pmIncentiveModels = _projectManagerRepository.GetProductionIncentiveForPrint(EmpCode, MonNum, Year);
            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Pm_IncentiveModel items = new Custom_Pm_IncentiveModel();
                items.ProjectName = customPmIncentiveModel.ProjectName;
                items.ShipmentType = customPmIncentiveModel.ShipmentType;
                items.ProjectType = customPmIncentiveModel.ProjectType;
                items.RewardPoint = customPmIncentiveModel.RewardPoint;
                items.RewardAmount = customPmIncentiveModel.RewardAmount;
                items.PoCategory = customPmIncentiveModel.SourcingType;
                items.PoDate = customPmIncentiveModel.PoDate;
                items.LSD = customPmIncentiveModel.LSD;
                items.PoVsLSDDiff = customPmIncentiveModel.PoVsLSDDiff;
                items.VesselDate = customPmIncentiveModel.VesselDate;
                items.LsdVsVesselDiffForDeduct = customPmIncentiveModel.LsdVsVesselDiffForDeduct;
                items.DeductPoint = customPmIncentiveModel.DeductPoint;
                items.FeatureBase = customPmIncentiveModel.FeatureBase;
                items.SmartBase = customPmIncentiveModel.SmartBase;
                items.PoReward = customPmIncentiveModel.PoReward;
                items.PerDayDeduction = customPmIncentiveModel.PerDayDeduction;
                items.WarehouseEntryDate = customPmIncentiveModel.WarehouseEntryDate;
                items.ExtendedWarehouseDate = customPmIncentiveModel.ExtendedWarehouseDate;
                items.OrderQuantity = customPmIncentiveModel.OrderQuantity;
                items.TotalSalesOut = customPmIncentiveModel.TotalSalesOut;
                items.ExistedPercentage = customPmIncentiveModel.ExistedPercentage;
                //items.ShipmentTaken = customPmIncentiveModel.ShipmentTaken;
                //items.EarlierOrLateShipment = customPmIncentiveModel.EarlierOrLateShipment;
                items.Orders = Convert.ToString(customPmIncentiveModel.OrderNumber);
                items.Others = customPmIncentiveModel.Others;
                items.PersonNo = customPmIncentiveModel.PersonNo;
                items.Amount1 = Convert.ToDecimal(customPmIncentiveModel.RewardAmount);
                items.Remarks = customPmIncentiveModel.Remarks;
                items.DeductionAmount1 = customPmIncentiveModel.DeductionAmount1;
                items.D_Remarks = customPmIncentiveModel.D_Remarks;
                items.FinalAmount1 = Convert.ToDecimal(customPmIncentiveModel.FinalAmount2);
                items.IncsTypes = customPmIncentiveModel.IncsTypes;
                items.AccessoriesType = customPmIncentiveModel.AccessoriesType;
                items.ProjectClosingDate = customPmIncentiveModel.ProjectClosingDate;
                items.MarketClearanceDate = customPmIncentiveModel.MarketClearanceDate;
                items.ProjectManagerClearanceDate = customPmIncentiveModel.ProjectManagerClearanceDate;
                items.RawMaterialAddedDate = customPmIncentiveModel.RawMaterialAddedDate;
                items.DaysPassed = customPmIncentiveModel.DaysPassed;
                //items.Reward = items.Reward;
                //items.Penalties = items.Penalties;
                cmList.Add(items);
            }
           
            ViewBag.PmIncentiveForPrint = cmList;
            ViewBag.GetPreparedUser = _projectManagerRepository.GetPreparedUserName();
            ViewBag.GetTotalFinalIncentiveOfPm = _projectManagerRepository.GetTotalFinalIncentiveOfPm(EmpCode, MonNum, Year);

            return View(cmList);
        }

        [HttpGet]
        public ActionResult PmIncentiveSheet()
        {
            var customInc = new Custom_Pm_IncentiveModel();
            customInc.CmnUserModelsList = _projectManagerRepository.GetPmUserList();
            return View(customInc);
        }

        [HttpGet]
        [Authorize(Roles = "PMHEAD,ACCNT")]
        public ActionResult PmIncentiveReportTopSheet(string Month, string MonNum, string Year)
        {
            List<Custom_Pm_IncentiveModel> customInc = new List<Custom_Pm_IncentiveModel>();
            customInc = _projectManagerRepository.PmIncentiveForAllPerson(Month, MonNum, Year);
            ViewBag.GetPreparedUser = _projectManagerRepository.GetPreparedUserName();
            return View(customInc);
        }
        [HttpGet]
        [Authorize(Roles = "PMHEAD")]
        public ActionResult PmIncentiveReportPreviousFourMonths(string EmpCode, string MonthNum1, string MonthNum2, string YearName)
        {

            var pmIns = _projectManagerRepository.GetAllIncentiveDataOfFourMonths(EmpCode, MonthNum1, MonthNum2, YearName);
            var cmListedVal1 = pmIns.OrderBy(x => x.MonNum).ToList();
            ViewBag.PmMonthWiseIncentive = cmListedVal1;

            var pmTotalIns = _projectManagerRepository.GetTotalIncentiveForMonthRange(EmpCode, MonthNum1, MonthNum2, YearName);
            var cmListedVal2 = pmTotalIns.OrderBy(x => x.MonNum).ToList();
            ViewBag.PmTotalIncentive = cmListedVal2;

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "PM,PMHEAD")]
        public ActionResult Pm_PiClosingSheet()
        {
            var fileManager = new FileManager();
            ViewBag.Projects = _projectManagerRepository.GetAllProjectsForOthers();
            ViewBag.PreviousPIProjects = _projectManagerRepository.GetPreviousPiClosingData();

            var vmPiClose = new Vm_PiClosing();
            vmPiClose.PmPiClosingModels = _projectManagerRepository.GetPreviousPiClosingData();

            foreach (Pm_PiClosingModel model in vmPiClose.PmPiClosingModels)
            {
                // if (model.UploadedFile != null)
                //  {
                var urls = model.UploadedFile;
                // for (int i = 0; i < urls.Count; i++)
                // {
                FilesDetail detail = new FilesDetail();
                detail.FilePath = fileManager.GetFile(urls);
                detail.Extention = fileManager.GetExtension(urls);

                model.FilesDetails.Add(detail);

                // }

                //  }

            }

            return View(vmPiClose);
        }

        [HttpPost]
        public ActionResult Pm_PiClosingSheet(Vm_PiClosing model)
        {
            var manager = new FileManager();

            //var id = Convert.ToString(model.ProjectMasterModel.ProjectMasterId);
            var moduleDirectory = "Pm_PiClosingSheet";
            var userDirectory = "PM";
            long userId = Convert.ToInt64(User.Identity.Name);
            //model.UploadedFile = manager.IncidentUpload(userDirectory, moduleDirectory,
            //    model.UploadedFilePath);
            model.UploadedFile = manager.Upload(model.ProjectMasterId, userDirectory, moduleDirectory,
               model.UploadedFilePath);
            model.Added = userId;
            model.AddedDate = DateTime.Now;

            var savePmPiClosing = "0";
            if (model.ProjectMasterModel.ProjectMasterId != 0)
            {
                savePmPiClosing = _projectManagerRepository.SavePmPiClosing(model);

            }

            return RedirectToAction("Pm_PiClosingSheet");
        }
        [HttpGet]
        public JsonResult GetPreviousPiClosingData()
        {

            List<Pm_PiClosingModel> getPiData = _projectManagerRepository.GetPreviousPiClosingData();

            var json = JsonConvert.SerializeObject(getPiData);

            return Json(new { data = getPiData }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult AccessoriesProject()
        {
            var vmIns = new VmPmIncentivePolicy();

            ViewBag.Projects = _projectManagerRepository.GetAllProjects();

            //
            //ViewBag.PmAccessoriesProjectList = _projectManagerRepository.GetRawMaterialInspectionList();
            ViewBag.PmAccessoriesProjectList = _projectManagerRepository.PmAccessoriesProjectList();

            var fileManager = new FileManager();
            if (ViewBag.PmAccessoriesProjectList != null)
            {
                foreach (var model in ViewBag.PmAccessoriesProjectList)
                {
                    if (model.SupportingDocument != null)
                    {
                        var urls = model.SupportingDocument;

                        FilesDetail detail = new FilesDetail();
                        detail.FilePath = fileManager.GetFile(urls);
                        detail.Extention = fileManager.GetExtension(urls);
                        model.FilesDetails.Add(detail);
                    }
                }
            }

            return View(vmIns);
        }
        [HttpPost]
        //public ActionResult AccessoriesProject(List<Pm_IncentiveModel> issueList, String ProjectMasterId )
        public ActionResult AccessoriesProject(List<Pm_IncentiveModel> issueList, List<String> ProjectMasterId, List<String> ProjectName)
        {
            var manager = new FileManager();
            
            var Attachment = "";
            
            foreach (var items in issueList)
            {
                if (items.UploderDocs != null)
                {
                    var res = manager.UploadAnotherDrive(0,"Pm_AccessoriesProject", "AccessoriesProject", items.UploderDocs);
                    Console.Write("res  :" + res);
                    items.SupportingDocument = items.SupportingDocument == null ? res : items.SupportingDocument + "|" + res;
                    Attachment = items.SupportingDocument;
                }
            }

            _projectManagerRepository.SaveAccessoriesProject(issueList, Attachment, ProjectMasterId, ProjectName);

            return RedirectToAction("AccessoriesProject");

        }

        [HttpGet]
        public ActionResult FollowUpFocMaterial()
        {
            var vmIns = new VmPmIncentivePolicy();

            ViewBag.PmFollowUpFocMaterial = _projectManagerRepository.PmFollowUpFocMaterial();

            var fileManager = new FileManager();
            if (ViewBag.PmFollowUpFocMaterial != null)
            {
                foreach (var model in ViewBag.PmFollowUpFocMaterial)
                {
                    if (model.SupportingDocument != null)
                    {
                        var urls = model.SupportingDocument;

                        FilesDetail detail = new FilesDetail();
                        detail.FilePath = fileManager.GetFile(urls);
                        detail.Extention = fileManager.GetExtension(urls);
                        model.FilesDetails.Add(detail);
                    }
                }
            }

            return View(vmIns);
        }
        [HttpPost]
        public ActionResult FollowUpFocMaterial(List<Pm_IncentiveModel> issueList)
        {
            var manager = new FileManager();

            var Attachment = "";
            //long proId = 0;
            //long.TryParse(ProjectMasterId, out proId);

            foreach (var items in issueList)
            {
                if (items.UploderDocs != null)
                {
                    var res = manager.UploadAnotherDrive(0, "Pm_FollowUpFocMaterial", "FollowUpFocMaterial", items.UploderDocs);
                    Console.Write("res  :" + res);
                    items.SupportingDocument = items.SupportingDocument == null ? res : items.SupportingDocument + "|" + res;
                    Attachment = items.SupportingDocument;
                }
            }

            _projectManagerRepository.SaveFollowUpFocMaterial(issueList, Attachment);

            return RedirectToAction("FollowUpFocMaterial");

        }

        [HttpGet]
        public ActionResult PoFeedbackAndInfoUpdate()
        {
            var vmIns = new VmPmIncentivePolicy();

            ViewBag.PmPoFeedbackAndInfoUpdate = _projectManagerRepository.PmPoFeedbackAndInfoUpdate();

            var fileManager = new FileManager();
            if (ViewBag.PmPoFeedbackAndInfoUpdate != null)
            {
                foreach (var model in ViewBag.PmPoFeedbackAndInfoUpdate)
                {
                    if (model.SupportingDocument != null)
                    {
                        var urls = model.SupportingDocument;

                        FilesDetail detail = new FilesDetail();
                        detail.FilePath = fileManager.GetFile(urls);
                        detail.Extention = fileManager.GetExtension(urls);
                        model.FilesDetails.Add(detail);
                    }
                }
            }

            return View(vmIns);
        }
        [HttpPost]
        public ActionResult PoFeedbackAndInfoUpdate(List<Pm_IncentiveModel> issueList)
        {
            var manager = new FileManager();

            var Attachment = "";
            //long proId = 0;
            //long.TryParse(ProjectMasterId, out proId);

            foreach (var items in issueList)
            {
                if (items.UploderDocs != null)
                {
                    var res = manager.UploadAnotherDrive(0, "Pm_PoFeedback", "PoFeedback", items.UploderDocs);
                    Console.Write("res  :" + res);
                    items.SupportingDocument = items.SupportingDocument == null ? res : items.SupportingDocument + "|" + res;
                    Attachment = items.SupportingDocument;
                }
            }

            _projectManagerRepository.SavePoFeedbackAndInfoUpdate(issueList, Attachment);

            return RedirectToAction("PoFeedbackAndInfoUpdate");

        }

        [HttpGet]
        public ActionResult SupplierPenalties()
        {
            var vmIns = new VmPmIncentivePolicy();

            ViewBag.Projects = _projectManagerRepository.GetAllProjects();

            ViewBag.PmSupplierPenaltiesList = _projectManagerRepository.PmSupplierPenaltiesList();

            var fileManager = new FileManager();
            if (ViewBag.PmSupplierPenaltiesList != null)
            {
                foreach (var model in ViewBag.PmSupplierPenaltiesList)
                {
                    if (model.SupportingDocument != null)
                    {
                        var urls = model.SupportingDocument;

                        FilesDetail detail = new FilesDetail();
                        detail.FilePath = fileManager.GetFile(urls);
                        detail.Extention = fileManager.GetExtension(urls);
                        model.FilesDetails.Add(detail);
                    }
                }
            }

            return View(vmIns);
        }
        [HttpPost]
        public ActionResult SupplierPenalties(List<Pm_IncentiveModel> issueList, List<String> ProjectMasterId, List<String> ProjectName)
        {
            var manager = new FileManager();

            var Attachment = "";
            //long proId = 0;
            //long.TryParse(ProjectMasterId, out proId);

            foreach (var items in issueList)
            {
                if (items.UploderDocs != null)
                {
                    var res = manager.UploadAnotherDrive(0, "Pm_SupplierPenalties", "SupplierPenalties", items.UploderDocs);
                    Console.Write("res  :" + res);
                    items.SupportingDocument = items.SupportingDocument == null ? res : items.SupportingDocument + "|" + res;
                    Attachment = items.SupportingDocument;
                }
            }
            _projectManagerRepository.SaveSupplierPenalties(issueList, Attachment, ProjectMasterId, ProjectName);

            return RedirectToAction("SupplierPenalties");

        }
        [HttpGet]
        public ActionResult PmGuidelines()
        {
            var vmIns = new VmPmIncentivePolicy();

            ViewBag.Projects = _projectManagerRepository.GetAllProjects();

            ViewBag.PmGuidelinesList = _projectManagerRepository.PmGuidelinesList();

            var fileManager = new FileManager();
            if (ViewBag.PmGuidelinesList != null)
            {
                foreach (var model in ViewBag.PmGuidelinesList)
                {
                    if (model.SupportingDocument != null)
                    {
                        var urls = model.SupportingDocument;

                        FilesDetail detail = new FilesDetail();
                        detail.FilePath = fileManager.GetFile(urls);
                        detail.Extention = fileManager.GetExtension(urls);
                        model.FilesDetails.Add(detail);
                    }
                }
            }

            return View(vmIns);
        }
        [HttpPost]
        public ActionResult PmGuidelines(List<Pm_IncentiveModel> issueList, String ProjectMasterId)
        {
            var manager = new FileManager();

            var Attachment = "";
            long proId = 0;
            long.TryParse(ProjectMasterId, out proId);

            foreach (var items in issueList)
            {
                if (items.UploderDocs != null)
                {
                    var res = manager.UploadAnotherDrive(proId, "Pm_Guidelines", "PmGuidelines", items.UploderDocs);
                    Console.Write("res  :" + res);
                    items.SupportingDocument = items.SupportingDocument == null ? res : items.SupportingDocument + "|" + res;
                    Attachment = items.SupportingDocument;
                }
            }

            _projectManagerRepository.SavePmGuidelines(issueList, proId, Attachment);

            return RedirectToAction("PmGuidelines");

        }
        [HttpGet]
        public ActionResult ProjectMarketingSpec()
        {
            var vmIns = new VmPmIncentivePolicy();

            ViewBag.Projects = _projectManagerRepository.GetAllProjects();

            ViewBag.PmProjectMarketingSpecList = _projectManagerRepository.PmProjectMarketingSpecList();

            var fileManager = new FileManager();
            if (ViewBag.PmProjectMarketingSpecList != null)
            {
                foreach (var model in ViewBag.PmProjectMarketingSpecList)
                {
                    if (model.SupportingDocument != null)
                    {
                        var urls = model.SupportingDocument;

                        FilesDetail detail = new FilesDetail();
                        detail.FilePath = fileManager.GetFile(urls);
                        detail.Extention = fileManager.GetExtension(urls);
                        model.FilesDetails.Add(detail);
                    }
                }
            }

            return View(vmIns);
        }
        [HttpPost]
        public ActionResult ProjectMarketingSpec(List<Pm_IncentiveModel> issueList, String ProjectMasterId)
        {
            var manager = new FileManager();

            var Attachment = "";
            long proId = 0;
            long.TryParse(ProjectMasterId, out proId);

            foreach (var items in issueList)
            {
                if (items.UploderDocs != null)
                {
                    var res = manager.UploadAnotherDrive(proId, "Pm_ProjectMarketingSpec", "ProjectMarketingSpec", items.UploderDocs);
                    Console.Write("res  :" + res);
                    items.SupportingDocument = items.SupportingDocument == null ? res : items.SupportingDocument + "|" + res;
                    Attachment = items.SupportingDocument;
                }
            }

            _projectManagerRepository.SavePmProjectMarketingSpec(issueList, proId, Attachment);

            return RedirectToAction("ProjectMarketingSpec");

        }

        [HttpGet]
        public ActionResult PolicyUpdate()
        {
            var vmIns = new VmPmIncentivePolicy();

            ViewBag.PmPolicyUpdateList = _projectManagerRepository.PmPolicyUpdateList();

            var fileManager = new FileManager();
            if (ViewBag.PmPolicyUpdateList != null)
            {
                foreach (var model in ViewBag.PmPolicyUpdateList)
                {
                    if (model.SupportingDocument != null)
                    {
                        var urls = model.SupportingDocument;

                        FilesDetail detail = new FilesDetail();
                        detail.FilePath = fileManager.GetFile(urls);
                        detail.Extention = fileManager.GetExtension(urls);
                        model.FilesDetails.Add(detail);
                    }
                }
            }

            return View(vmIns);
        }
        [HttpPost]
        public ActionResult PolicyUpdate(List<Pm_IncentiveModel> issueList)
        {
            var manager = new FileManager();

            var Attachment = "";
            //long proId = 0;
            //long.TryParse(ProjectMasterId, out proId);

            foreach (var items in issueList)
            {
                if (items.UploderDocs != null)
                {
                    var res = manager.UploadAnotherDrive(0, "Pm_PolicyUpdate", "PolicyUpdate", items.UploderDocs);
                    Console.Write("res  :" + res);
                    items.SupportingDocument = items.SupportingDocument == null ? res : items.SupportingDocument + "|" + res;
                    Attachment = items.SupportingDocument;
                }
            }

            _projectManagerRepository.SavePolicyUpdate(issueList, Attachment);

            return RedirectToAction("PolicyUpdate");

        }
        [HttpGet]
        public ActionResult SampleHandsetManagement()
        {
            var vmIns = new VmPmIncentivePolicy();

            ViewBag.PmSampleHandsetManagementList = _projectManagerRepository.PmSampleHandsetManagementList();

            var fileManager = new FileManager();
            if (ViewBag.PmSampleHandsetManagementList != null)
            {
                foreach (var model in ViewBag.PmSampleHandsetManagementList)
                {
                    if (model.SupportingDocument != null)
                    {
                        var urls = model.SupportingDocument;

                        FilesDetail detail = new FilesDetail();
                        detail.FilePath = fileManager.GetFile(urls);
                        detail.Extention = fileManager.GetExtension(urls);
                        model.FilesDetails.Add(detail);
                    }
                }
            }

            return View(vmIns);
        }
        [HttpPost]
        public ActionResult SampleHandsetManagement(List<Pm_IncentiveModel> issueList)
        {
            var manager = new FileManager();

            var Attachment = "";
            //long proId = 0;
            //long.TryParse(ProjectMasterId, out proId);

            foreach (var items in issueList)
            {
                if (items.UploderDocs != null)
                {
                    var res = manager.UploadAnotherDrive(0, "Pm_SampleHandset", "SampleHandset", items.UploderDocs);
                    Console.Write("res  :" + res);
                    items.SupportingDocument = items.SupportingDocument == null ? res : items.SupportingDocument + "|" + res;
                    Attachment = items.SupportingDocument;
                }
            }

            _projectManagerRepository.SaveSampleHandset(issueList, Attachment);

            return RedirectToAction("SampleHandsetManagement");

        }
        #endregion

        #region Independent HW test Asign

        public ActionResult HwTestAssign()
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            ViewBag.Projects = _projectManagerRepository.GetProjectMasterModelsByProjectManager(userId);
            ViewBag.hwTestMasers = _commonRepository.GetHwTestMasterModels(ViewBag.UserInfo.RoleName);
            ViewBag.AssignedTests = _commonRepository.GetHwTestInchargeAssignModels();
            var vm = _commonRepository.GetHwTestDetail();
            return View(vm);
        }

        public JsonResult HwTestInchargeAssign(string hwTestName, string remarks, string projectName, long hwTestMasterId = 0, long projectId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var moduleDirectory = "HwTest";
            var userDirectory = "PM";
            var manager = new FileManager();
            var assign = new HwTestInchargeAssignModel
            {
                HwTestMasterId = hwTestMasterId,
                ProjectMasterId = projectId,
                ProjectName = projectName,
                HwTestName = hwTestName,
                Remarks = remarks,
                AddedBy = userId,
                AddedDate = DateTime.Now,
                Status = "NEW"
            };
            var json = _projectManagerRepository.SaveHwTestInchargeAssign(assign);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<string>(new[] { "HWHEAD" }),
                new List<string>(new[] { "" }), json.HwTestName + " test assigned by " + json.AddedByName + " for project " + json.ProjectName, "This is to inform you that, " + json.HwTestName + " test assigned by " + json.AddedByName + " for project " + json.ProjectName + ".");
            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                var fileupload = new HwTestFileUploadModel
                {
                    HwTestInchargeAssignId = json.HwTestInchargeAssignId,
                    ProjectMasterId = projectId,
                    AddedBy = userId,
                    AddedDate = DateTime.Now,
                    FileUploadPath = manager.IncidentUpload(userDirectory, moduleDirectory, file)
                };
                _hardwareRepository.SaveHwTestFileUploadModel(fileupload);
            }
            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion

        #region PM ORDER QUANTITY WITH COLOR RATIO REPORT

        public ActionResult PmOrderQuantityWithColorRatio()
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.OrderList = _projectManagerRepository.GetOrderQuantityWithColorModel(userId);
            ViewBag.Projects = _commonRepository.GetAllProjects();
            return View();
        }

        public JsonResult GetColors(string color)
        {
            var json = _projectManagerRepository.GetColorsList(color);
            return Json(json);
        }

        [HttpPost]
        public ActionResult PmOrderQuantityWithColorRatio(PmOrderQuantityWithColorModel model)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            model.AddedBy = userId;
            model.AddedDate = DateTime.Now;
            _projectManagerRepository.SaveOrderQuantityWithColorModel(model);
            return RedirectToAction("PmOrderQuantityWithColorRatio");
        }

        public ActionResult PmOrderQuantityWithColorRatioReport()
        {
            ViewBag.ProjectName = _hardwareRepository.GetAllProjectDistinctName();
            return View();
        }

        public JsonResult GetOrderQuantityReport(string projectname, string model)
        {
            var total = _projectManagerRepository.GetOrderWiseTotalCounts(projectname);
            var totalbreakdown = _projectManagerRepository.GetOrderWiseCountsByProject(projectname);
            var warquantity = _projectManagerRepository.GetWareHouseQuantity(model);
            var servquantity = _projectManagerRepository.GetServiceCenterQuantity(model);
            var json = new List<object> { total, totalbreakdown, warquantity, servquantity };
            return Json(json);
        }
        #endregion

        #region sample status
        public ActionResult ProjectWiseSampleStatus()
        {
            ViewBag.Models = _commonRepository.GetOnlyModelName();
            return View();
        }

        public JsonResult GetSampleByProjectName(string project)
        {
            var json = _projectManagerRepository.SampleListByProjectName(project);
            return Json(json);
        }

        public ActionResult DeptAndPersonWiseSampleStat()
        {
            ViewBag.Dept = _generalIncidentRepository.GetAllRoleModels();
            ViewBag.Person = _commonRepository.GetAllEmployee();
            return View();
        }

        public JsonResult SampleSentToDept(string roledesc)
        {
            var json = _projectManagerRepository.DeptWiseSampleStatus(roledesc);
            return Json(json);
        }

        public JsonResult SampleSentToPerson(long id)
        {
            var json = _projectManagerRepository.PersonWiseSampleStatus(id);
            return Json(json);
        }
        #endregion

        public ActionResult ProjectAcknowledgedList()
        {
            // var projectList = _projectManagerRepository.GetAllPMAcknowledgeList();
            // var pmProjects = new PMAcknowledgementViewModel { PMAcknowledgements = projectList};
            // return View(pmProjects);

            return View();
        }
        public ActionResult ProjectAcknowledge(long planid)
        {
            var projectList = _projectManagerRepository.GetAllProjectByPlanId(planid);
            return View(projectList);
        }

        [HttpPost]
        public ActionResult SavePMAcknowledge(ProjectAcknowledgementViewModel viewmodel)
        {
            var result = _projectManagerRepository.UpdatePMAcknowledge(viewmodel);
            if (result)
                return RedirectToAction("ProjectAcknowledgedList");
            else
                return RedirectToAction("ProjectAcknowledge", new { PlanId = viewmodel.PlanId });
        }

        #region New For SwQc

        public ActionResult DetailsOfSwQcsAllWorkForPm(string projectId, string swqcInchargeId, string pmAssignId, string testPhaseId, DateTime pmAssignDate, string projectName)
        {
            var fileManager = new FileManager();
            var vmSwInchargemodel = new VmSwQcHeadViewModel();
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name);

            if (projectId != null)
            {
                vmSwInchargemodel.SwQcIssueDetailModels = _projectManagerRepository.GetSwQcIssueDetailsForPm(projectId, swqcInchargeId, pmAssignId, testPhaseId, pmAssignDate);
                if (vmSwInchargemodel.SwQcIssueDetailModels.Any())
                {
                    foreach (SwQcIssueDetailModel model in vmSwInchargemodel.SwQcIssueDetailModels)
                    {
                        if (model.Upload != null)
                        {
                            var urls = model.Upload.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                FilesDetail detail = new FilesDetail();
                                detail.FilePath = fileManager.GetFile(urls[i]);
                                detail.Extention = fileManager.GetExtension(urls[i]);
                                model.FilesDetails.Add(detail);
                            }
                        }
                    }
                }
                vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcIssueDetailModels.Any() &&
                                           vmSwInchargemodel.SwQcIssueDetailModels[0].SwQcIssueId > 0;

                vmSwInchargemodel.SwQcIssueDetailModels1 = _projectManagerRepository.GetSwQcCtsMonkeyOrCameraAutomationDataForPm(projectId, swqcInchargeId, pmAssignId, testPhaseId, pmAssignDate);
                if (vmSwInchargemodel.SwQcIssueDetailModels1.Any())
                {
                    foreach (SwQcIssueDetailModel model in vmSwInchargemodel.SwQcIssueDetailModels1)
                    {
                        if (model.Upload != null)
                        {
                            var urls = model.Upload.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                FilesDetail detail = new FilesDetail();
                                detail.FilePath = fileManager.GetFile(urls[i]);
                                detail.Extention = fileManager.GetExtension(urls[i]);
                                model.FilesDetails.Add(detail);
                            }
                        }
                    }
                }
                vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcIssueDetailModels1.Any() &&
                                           vmSwInchargemodel.SwQcIssueDetailModels1[0].SwQcIssueId > 0;


                vmSwInchargemodel.SwQcPersonalUseFindingsIssueDetailModels = _projectManagerRepository.GetPersonalUseFindingsForPm(projectId, swqcInchargeId, pmAssignId, testPhaseId, pmAssignDate);
                if (vmSwInchargemodel.SwQcPersonalUseFindingsIssueDetailModels.Any())
                {
                    foreach (SwQcPersonalUseFindingsIssueDetailModel model in vmSwInchargemodel.SwQcPersonalUseFindingsIssueDetailModels)
                    {
                        if (model.Upload != null)
                        {
                            var urls = model.Upload.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                FilesDetail detail = new FilesDetail();
                                detail.FilePath = fileManager.GetFile(urls[i]);
                                detail.Extention = fileManager.GetExtension(urls[i]);
                                model.FilesDetails.Add(detail);
                            }
                        }
                    }
                }
                vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcPersonalUseFindingsIssueDetailModels.Any() &&
                                           vmSwInchargemodel.SwQcPersonalUseFindingsIssueDetailModels[0].SwQcIssueId > 0;

            }

            return View(vmSwInchargemodel);
        }

        public ActionResult SwQcsIssueDetailsSendToChainaFromPm(string projectId, string swVersionNo, string projectOrder, string moduleName, string testPhases)
        {
            var vmSwQcSpecification = new VmSwQcSpecificationModified();
            long userId = Convert.ToInt64(User.Identity.Name);

            int softVersionNo;
            int.TryParse(swVersionNo, out softVersionNo);

            int projectOrders;
            int.TryParse(projectOrder, out projectOrders);

            vmSwQcSpecification.ProjectMasterModelsList = _projectManagerRepository.GetProjectListForSwQcHead();

            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Select One" } };

            var query = (from master in vmSwQcSpecification.ProjectMasterModelsList
                         // where master.SwQcHeadStatus != "NEW" || master.SwQcHeadStatus != "INACTIVE"
                         select new
                         {
                             master.ProjectName,

                         }).ToList();

            foreach (var t in query)
            {
                selectListItems.Add(new SelectListItem
                {
                    Value = t.ProjectName,
                    Text = t.ProjectName
                });
            }
            ViewBag.CombinedIds = selectListItems;
            //Test phase//
            var selectListItemsTestPhase = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Select Test Phase" } };
            List<SwQcTestPhaseModel> listTestPhase = _projectManagerRepository.GetSwQcTestPhaseForSupp() ??
                                                     new List<SwQcTestPhaseModel>();
            selectListItemsTestPhase.AddRange(listTestPhase.Select(p => new SelectListItem { Value = p.TestPhaseID.ToString(), Text = p.TestPhaseName }));
            ViewBag.CombinedIdsForTestPhase = selectListItemsTestPhase;
            ////ModuleList///
            vmSwQcSpecification.SwQcIssueCategoryModels = _projectManagerRepository.GetIssueCategory();

            var selectListItemsForModule = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Select Category" } };

            var queryModule = (from master in vmSwQcSpecification.SwQcIssueCategoryModels
                               select master).ToList();

            foreach (var t in queryModule)
            {
                String selectedValue = Convert.ToString(t.SwQcIssueCategorytId);

                selectListItemsForModule.Add(new SelectListItem
                {
                    Value = t.QcCategoryName,
                    Text = t.QcCategoryName
                });
            }
            ViewBag.CombinedIdsForModule = selectListItemsForModule;

            ///////////////For load issues list/////////

            string obj1 = null;
            obj1 = projectId;
            if (obj1 != null)
            {
                if (obj1 != "")
                {

                    String pro_id1 = null;
                    String swQcIncharge_id1 = null;
                    String swQcAssign_id1 = null;
                    String projectName = null;

                    var match1 = obj1.Split(',');

                    for (var i = 0; i < match1.Length; i++)
                    {
                        Console.Write("<br /> Element " + i + " of the array is: " + match1[i]);
                        projectName = match1[0];
                    }

                    vmSwQcSpecification.SwQcAssignsFromQcHeadModels = _projectManagerRepository.GetSwQcsAssignsInfo(projectName, projectOrders, softVersionNo, testPhases);

                    vmSwQcSpecification.SwQcIssueDetailModels = _projectManagerRepository.GetSwQcIssueDetailsForSupplier(projectName, moduleName, projectOrders, softVersionNo, testPhases);

                }
            }
            vmSwQcSpecification.CombinedProjectId = projectId;
            vmSwQcSpecification.CombinedProjectIds = moduleName;
            vmSwQcSpecification.OrderNumber = projectOrders;
            vmSwQcSpecification.CombinedTestPhaseIds = testPhases;
            vmSwQcSpecification.SoftwareVersionNumber = softVersionNo;

            return View(vmSwQcSpecification);
        }

        [HttpPost]
        public ActionResult UpdateSwQcIssueDetailModelForSupplier(SwQcIssueDetailModel supplierUpdate)
        {
            //string supplierUpdate1 = null;
            //bool isExist = false;
            //if (supplierUpdate != null)
            //{
            //    isExist = _projectManagerRepository.GetSupplierFeedbackData(supplierUpdate);

            //}

            //if (isExist)
            //{

            //    supplierUpdate1 = "Already Generated.";
            //}
            //else
            //{
            //    supplierUpdate1 = _projectManagerRepository.UpdateSwQcIssueDetailModelForSupplier(supplierUpdate);
            //}


            //return Json(new { data = supplierUpdate1 }, JsonRequestBehavior.AllowGet);
            string supplierUpdate1 = null;
            bool isExist = false;
            if (supplierUpdate != null)
            {
                isExist = _projectManagerRepository.GetSupplierFeedbackData(supplierUpdate);

            }

            supplierUpdate1 = _projectManagerRepository.UpdateSwQcIssueDetailModelForSupplier(supplierUpdate);

            if (isExist)
            {
                supplierUpdate1 = "Already Supplier Feeadback Generated.";

            }

            return Json(new { data = supplierUpdate1 }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region excel

        public static DataTable GetFirstRowForPmExcel(string projectId, string softVersionName, long testIds, int swVersionNo)
        {
            DataTable totalhistry = new DataTable();
            var cn = new SqlConnection(_connectionStringCellphone);

            cn.Open();

            //totalhistry.Columns.Add("Model");
            totalhistry.Columns.Add("Issue SL.");
            totalhistry.Columns.Add("Issue Scenario");
            totalhistry.Columns.Add("Expected Outcome");
            totalhistry.Columns.Add("Frequency");
            totalhistry.Columns.Add("Reproduce Path");
            totalhistry.Columns.Add("Attachment");
            totalhistry.Columns.Add("Issue Type");

            totalhistry.Columns.Add("Wal. QC. Com. Date");
            totalhistry.Columns.Add("Walton QC Comment");
            totalhistry.Columns.Add("Walton QC Status");

            totalhistry.Columns.Add("Fixed Version");
            totalhistry.Columns.Add("Sup. Com. Date");
            totalhistry.Columns.Add("Supplier Status");
            totalhistry.Columns.Add("Supplier Comment");

            totalhistry.Columns.Add("Wal. PM. Com. Date");
            totalhistry.Columns.Add("Walton PM Comment");
            totalhistry.Columns.Add("Demo");
            String sql = "";

            if (projectId.Trim() == softVersionName.Trim())
            {
                //                sql = String.Format(@"select distinct SwQcIssueId,IssueSerial,SoftwareVersionNo,SoftwareVersionName,IssueScenario,ExpectedOutcome,Frequency,IssueReproducePath,Attachment,IssueType,WaltonQcComDate,WaltonQcComment,WaltonQcStatus,
                //                FixedVersion,SupplierComDate,SupplierStatus,SupplierComment,WaltonPmComDate,WaltonPmComment
                //                from CellPhoneProject.dbo.SwQcIssueDetails sii where ProjectName='{0}' and (SoftwareVersionName is not null and SoftwareVersionName != '')
                //                and IsApprovedForChina=1 and SwQcIssueId in (select  top 1  SwQcIssueId from CellPhoneProject.dbo.SwQcIssueDetails where IssueSerial=sii.IssueSerial and ProjectName='{0}' 
                //                and IsApprovedForChina=1 and (SoftwareVersionName is not null and SoftwareVersionName != '')  and 
                //                SoftwareVersionNo in (select  top 1  SoftwareVersionNo from CellPhoneProject.dbo.SwQcIssueDetails where IssueSerial=sii.IssueSerial and ProjectName='{0}' 
                //                and IsApprovedForChina=1 and (SoftwareVersionName is not null and SoftwareVersionName != '')  order by SoftwareVersionNo desc)
                //                order by SwQcIssueId desc)
                //                order by IssueSerial asc", projectId.Trim(), softVersionName);
//                sql = String.Format(@"select distinct SwQcIssueId,IssueSerial,
//                (select top 1 'Fixed in Demo_SW_V_'+cast(si1.SoftwareVersionNo as varchar(10))+'_Demo' from CellPhoneProject.dbo.SwQcIssueDetails si1 where si1.ProjectName='{0}' and si1.TestPhaseID=5 and si1.WaltonQcStatus='FIXED' and si1.IssueSerial=sii.IssueSerial
//                 order by si1.SwQcIssueId asc) as Demo2,
//                SoftwareVersionNo,SoftwareVersionName,IssueScenario,ExpectedOutcome,Frequency,IssueReproducePath,Attachment,IssueType,WaltonQcComDate,WaltonQcComment,WaltonQcStatus,
//                FixedVersion,SupplierComDate,SupplierStatus,SupplierComment,WaltonPmComDate,WaltonPmComment
//                from CellPhoneProject.dbo.SwQcIssueDetails sii where ProjectName='{0}' and (SoftwareVersionName is not null and SoftwareVersionName != '')
//                and IsApprovedForChina=1 and SwQcIssueId in (select  top 1  SwQcIssueId from CellPhoneProject.dbo.SwQcIssueDetails where IssueSerial=sii.IssueSerial and ProjectName='{0}' 
//                and IsApprovedForChina=1 and (SoftwareVersionName is not null and SoftwareVersionName != '')  and 
//                SoftwareVersionNo in (select  top 1  SoftwareVersionNo from CellPhoneProject.dbo.SwQcIssueDetails where IssueSerial=sii.IssueSerial and ProjectName='{0}' 
//                and IsApprovedForChina=1 and (SoftwareVersionName is not null and SoftwareVersionName != '')  order by SoftwareVersionNo desc)
//                order by SwQcIssueId desc)
//                order by IssueSerial asc", projectId.Trim(), softVersionName);
                sql = String.Format(@"select distinct SwQcIssueId,IssueSerial,
                (select top 1 'Fixed in Demo_SW_V_'+cast(si1.SoftwareVersionNo as varchar(10))+'_Demo' from CellPhoneProject.dbo.SwQcIssueDetails si1 where si1.ProjectName='{0}' and si1.TestPhaseID=5 and si1.WaltonQcStatus='FIXED' and si1.IssueSerial=sii.IssueSerial
                 order by si1.SwQcIssueId asc) as Demo2,
                SoftwareVersionNo,SoftwareVersionName,IssueScenario,ExpectedOutcome,Frequency,IssueReproducePath,Attachment,IssueType,WaltonQcComDate,WaltonQcComment,WaltonQcStatus,
                FixedVersion,SupplierComDate,SupplierStatus,SupplierComment,WaltonPmComDate,WaltonPmComment
                from CellPhoneProject.dbo.SwQcIssueDetails sii where ProjectName='{0}' and (SoftwareVersionName is not null and SoftwareVersionName != '')
                and IsApprovedForChina=1 and SwQcIssueId in (select  top 1  SwQcIssueId from CellPhoneProject.dbo.SwQcIssueDetails where IssueSerial=sii.IssueSerial and ProjectName='{0}' 
                and IsApprovedForChina=1 and (SoftwareVersionName is not null and SoftwareVersionName != '')  and 
                SoftwareVersionNo in (select  top 1  SoftwareVersionNo from CellPhoneProject.dbo.SwQcIssueDetails where IssueSerial=sii.IssueSerial and ProjectName='{0}' 
                and IsApprovedForChina=1 and (SoftwareVersionName is not null and SoftwareVersionName != '')  order by SoftwareVersionNo desc)
                order by SwQcIssueId desc)
                order by IssueSerial asc", projectId.Trim(), softVersionName);
            }
            else
            {
                // sql = String.Format(@" select * from CellPhoneProject.dbo.SwQcIssueDetails where ProjectName='{0}' and IsApprovedForChina=1  and SoftwareVersionName='{1}' order by SoftwareVersionNo,IssueSerial asc", projectId.Trim(), softVersionName.Trim());

                if (testIds == 5)
                {
                    sql = String.Format(@" select (select top 1 'Fixed in Demo_SW_V_'+cast(si1.SoftwareVersionNo as varchar(10))+'_Demo' from
                    CellPhoneProject.dbo.SwQcIssueDetails si1 where si1.ProjectName='{0}' 
                    and si1.TestPhaseID=5 and si1.WaltonQcStatus='FIXED' and si1.IssueSerial=sii.IssueSerial
                     order by si1.SwQcIssueId asc) as Demo2, * from CellPhoneProject.dbo.SwQcIssueDetails sii where ProjectName='{0}' and IsApprovedForChina=1  and SoftwareVersionName='{1}' and SoftwareVersionNo='{2}' and TestPhaseID =5 order by SoftwareVersionNo,IssueSerial asc", projectId.Trim(), softVersionName.Trim().Trim(), swVersionNo);

                }
                if (testIds != 5)
                {
                    sql = String.Format(@" select (select top 1 'Fixed in Demo_SW_V_'+cast(si1.SoftwareVersionNo as varchar(10))+'_Demo' from
                    CellPhoneProject.dbo.SwQcIssueDetails si1 where si1.ProjectName='{0}' 
                    and si1.TestPhaseID=5 and si1.WaltonQcStatus='FIXED' and si1.IssueSerial=sii.IssueSerial
                     order by si1.SwQcIssueId asc) as Demo2, * from CellPhoneProject.dbo.SwQcIssueDetails sii where ProjectName='{0}' and IsApprovedForChina=1  and SoftwareVersionName='{1}' and SoftwareVersionNo='{2}' and TestPhaseID !=5 order by SoftwareVersionNo,IssueSerial asc", projectId.Trim(), softVersionName.Trim().Trim(), swVersionNo);

                }
            }

            SqlCommand cmd = new SqlCommand(sql, cn);

            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    DataRow newRow = totalhistry.NewRow();
                    // totalhistry.Rows.Add((String)rdr["ProjectName"], (String)rdr["SoftwareVersionName"], (String)rdr["IssueScenario"]);
                    newRow["Issue SL."] = rdr["IssueSerial"].ToString();
                    newRow["Issue Scenario"] = rdr["IssueScenario"].ToString();
                    newRow["Expected Outcome"] = rdr["ExpectedOutcome"].ToString();
                    newRow["Frequency"] = rdr["Frequency"].ToString();
                    newRow["Reproduce Path"] = rdr["IssueReproducePath"].ToString();
                    newRow["Attachment"] = rdr["Attachment"].ToString();
                    newRow["Issue Type"] = rdr["IssueType"].ToString();

                    newRow["Wal. QC. Com. Date"] = rdr["WaltonQcComDate"].ToString();
                    newRow["Walton QC Comment"] = rdr["WaltonQcComment"].ToString();
                    newRow["Walton QC Status"] = rdr["WaltonQcStatus"].ToString();
                    newRow["Fixed Version"] = rdr["FixedVersion"].ToString();
                    newRow["Sup. Com. Date"] = rdr["SupplierComDate"].ToString();
                    newRow["Supplier Status"] = rdr["SupplierStatus"].ToString();
                    newRow["Supplier Comment"] = rdr["SupplierComment"].ToString();
                    newRow["Wal. PM. Com. Date"] = rdr["WaltonPmComDate"].ToString();
                    newRow["Walton PM Comment"] = rdr["WaltonPmComment"].ToString();
                    newRow["Demo"] = rdr["Demo2"].ToString();
                    totalhistry.Rows.Add(newRow);
                }
            }
            return totalhistry;
        }
        public void GetExcelForPm(DataTable ds, string projectId, int swVersionNo, long testPhaseIds)
        {
            var vms = new VmSwQcSpecificationModified();
            vms.SwQcHeadAssignsFromPmModels = _projectManagerRepository.GetProjectVersionName(projectId, swVersionNo, testPhaseIds);

            //Creae an Excel application instance
            Excel.Application excelApp = new Excel.Application();

            Excel.Workbook excelWorkBook = excelApp.Workbooks.Add("");

            foreach (var table in vms.SwQcHeadAssignsFromPmModels)
            {
                //Add a new worksheet to workbook with the Datatable name
                Excel.Worksheet excelWorkSheet = excelWorkBook.Sheets.Add();

                var swVersionsName = table.SoftVersionName;
                long testIds = Convert.ToInt64(table.TestPhaseID);

                excelWorkSheet.Name = table.SoftwareVersionName;

                //Excel design//

                //freeze row//

                excelWorkSheet.Range["A2", "P2"].Application.ActiveWindow.SplitRow = 2;
                //excelWorkSheet.Range["A2", "P2"].Rows.Application.ActiveWindow.FreezePanes = true;

                //Adjust all row
                excelWorkSheet.Rows.AutoFit();

                //Column width and Hight//
                excelWorkSheet.Range["A2", "P2"].Rows.RowHeight = 40;

                excelWorkSheet.Columns[1].ColumnWidth = 10;
                excelWorkSheet.Columns[2].ColumnWidth = 30;
                excelWorkSheet.Columns[3].ColumnWidth = 30;

                excelWorkSheet.Columns[5].ColumnWidth = 25;
                excelWorkSheet.Columns[6].ColumnWidth = 25;
                excelWorkSheet.Columns[10].ColumnWidth = 20;
                excelWorkSheet.Columns[11].ColumnWidth = 20;
                excelWorkSheet.Columns[14].ColumnWidth = 20;

                excelWorkSheet.Columns[4].ColumnWidth = 15;
                excelWorkSheet.Columns[7].ColumnWidth = 15;
                excelWorkSheet.Columns[8].ColumnWidth = 20;
                excelWorkSheet.Columns[9].ColumnWidth = 20;
                excelWorkSheet.Columns[12].ColumnWidth = 20;
                excelWorkSheet.Columns[13].ColumnWidth = 20;
                excelWorkSheet.Columns[15].ColumnWidth = 20;
                excelWorkSheet.Columns[16].ColumnWidth = 20;
                excelWorkSheet.Columns[17].ColumnWidth = 20;
                /////////

                //wrap text//
                excelWorkSheet.get_Range("A2", "C2").Style.WrapText = true;

                //freeze column//

                excelWorkSheet.Range["A1", "C1"].Application.ActiveWindow.SplitColumn = 3;
                excelWorkSheet.Range["A1", "C1"].EntireColumn.Application.ActiveWindow.FreezePanes = true;

                //Adjust all column
                excelWorkSheet.Columns.AutoFit();

                //For Header Name//
                excelWorkSheet.Cells[1, 2] = projectId;
                excelWorkSheet.get_Range("A1", "J1").Font.Bold = true;
                excelWorkSheet.get_Range("A1", "J1").Font.Name = "Calibri";
                excelWorkSheet.get_Range("A1", "J1").Font.Size = 32;
                excelWorkSheet.get_Range("A1", "J1").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                excelWorkSheet.get_Range("A1", "J1").HorizontalAlignment =
       Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                //excelWorkSheet.get_Range("A1", "I1").Columns.AutoFit();



                //For Issue List Color Group 1//
                excelWorkSheet.get_Range("A2", "G2").Font.Bold = true;
                excelWorkSheet.get_Range("A2", "G2").Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.MediumPurple);
                excelWorkSheet.get_Range("A2", "G2").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                excelWorkSheet.get_Range("A2", "G2").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                // excelWorkSheet.get_Range("A2", "F2").Columns.AutoFit();
                excelWorkSheet.get_Range("A2", "G2").Application.StandardFont = "Calibri";
                excelWorkSheet.get_Range("A2", "G2").Application.StandardFontSize = 11;
                //end Issue List Color Group 1 

                //For Issue List Color Group 2//
                excelWorkSheet.get_Range("H2", "K2").Font.Bold = true;
                excelWorkSheet.get_Range("H2", "K2").Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Green);
                excelWorkSheet.get_Range("H2", "K2").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                excelWorkSheet.get_Range("H2", "K2").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                //  excelWorkSheet.get_Range("G2", "J2").Columns.AutoFit();
                excelWorkSheet.get_Range("H2", "K2").Application.StandardFont = "Calibri";
                excelWorkSheet.get_Range("H2", "K2").Application.StandardFontSize = 11;
                //end Issue List Color Group 2

                //For Issue List Color Group 3//
                excelWorkSheet.get_Range("L2", "N2").Font.Bold = true;
                excelWorkSheet.get_Range("L2", "N2").Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Tomato);
                excelWorkSheet.get_Range("L2", "N2").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                excelWorkSheet.get_Range("L2", "N2").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                // excelWorkSheet.get_Range("K2", "M2").Columns.AutoFit();
                excelWorkSheet.get_Range("L2", "N2").Application.StandardFont = "Calibri";
                excelWorkSheet.get_Range("L2", "N2").Application.StandardFontSize = 11;
                //end Issue List Color Group 3

                //For Issue List Color Group 3//
                excelWorkSheet.get_Range("O2", "P2").Font.Bold = true;
                excelWorkSheet.get_Range("O2", "P2").Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DodgerBlue);
                excelWorkSheet.get_Range("O2", "P2").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                excelWorkSheet.get_Range("O2", "P2").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                //  excelWorkSheet.get_Range("N2", "O2").Columns.AutoFit();
                excelWorkSheet.get_Range("O2", "P2").Application.StandardFont = "Calibri";
                excelWorkSheet.get_Range("O2", "P2").Application.StandardFontSize = 11;
                //end Issue List Color Group 3

                excelWorkSheet.get_Range("Q2", "Q2").Font.Bold = true;
                excelWorkSheet.get_Range("Q2", "Q2").Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Red);
                excelWorkSheet.get_Range("Q2", "Q2").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                excelWorkSheet.get_Range("Q2", "Q2").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                //  excelWorkSheet.get_Range("N2", "O2").Columns.AutoFit();
                excelWorkSheet.get_Range("Q2", "Q2").Application.StandardFont = "Calibri";
                excelWorkSheet.get_Range("Q2", "Q2").Application.StandardFontSize = 11;

                DataTable dt = new DataTable();
                using (dt = GetFirstRowForPmExcel(projectId, swVersionsName, testIds, swVersionNo))
                {
                    var qcF = "";
                    var supF = "";
                    for (int i = 1; i < dt.Columns.Count + 1; i++)
                    {
                        //excelWorkSheet.Cells[1, i] = dt.Columns[i - 1].ColumnName;
                        excelWorkSheet.Cells[2, i] = dt.Columns[i - 1].ColumnName;
                    }

                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        for (int k = 0; k < dt.Columns.Count; k++)
                        {
                            //excelWorkSheet.Cells[j + 2, k + 1] = dt.Rows[j].ItemArray[k].ToString();
                            excelWorkSheet.Cells[j + 3, k + 1] = dt.Rows[j].ItemArray[k].ToString();

                            qcF = excelWorkSheet.Cells[2, 8].Value2;
                            supF = excelWorkSheet.Cells[2, 12].Value2;

                            if (dt.Rows[j].ItemArray[k].ToString() == "CRITICAL" || dt.Rows[j].ItemArray[k].ToString() == "NOT FIXED")  // CHECKING CONDITION WITH THE DATATABLE
                            {
                                // USE TO COLOR THE ROW AND THE COLUMN RANGE 
                                excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(255, 0, 0);
                                excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
                            }
                            else if (dt.Rows[j].ItemArray[k].ToString() == "MAJOR")
                            {
                                excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(142, 169, 219);
                                excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
                            }
                            else if (dt.Rows[j].ItemArray[k].ToString() == "MINOR")
                            {
                                excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(0, 176, 240);
                                excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
                            }
                            else if (dt.Rows[j].ItemArray[k].ToString() == "IMPROVED")
                            {
                                excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(255, 255, 0);
                                excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
                            }
                            else if (dt.Rows[j].ItemArray[k].ToString() == "OPTIMIZED")
                            {
                                excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(255, 242, 204);
                                excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
                            }
                            //|| dt.Rows[j].ItemArray[k].ToString() == "FIXED" || dt.Rows[j].ItemArray[k].ToString() == "SUPPLIER CAN'T FIXED"
                            else if (dt.Rows[j].ItemArray[k].ToString() == "FIXED")
                            {
                                excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(0, 176, 80);
                                excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
                                //for (int i = 1; i < dt.Columns.Count + 1; i++)
                                //{
                                //    //excelWorkSheet.Cells[2, i] = dt.Columns[i - 1].ColumnName;
                                //    if (dt.Columns[i - 1].ColumnName == "Supplier Status")
                                //    {
                                //        excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(0, 176, 240);
                                //        excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
                                //    }
                                //    if (dt.Columns[i - 1].ColumnName == "Walton QC Status")
                                //    {
                                //        excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(0, 176, 80);
                                //        excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
                                //    }

                                //}
                            }
                            //else if (dt.Rows[j].ItemArray[k].ToString() == "FIXED" && supF == "Supplier Status")
                            //{
                            //    excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(0, 176, 240);
                            //    excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
                            //}
                            //else if (dt.Rows[j].ItemArray[k].ToString() == "FIXED")
                            //{
                            //    excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(0, 176, 80);
                            //    excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
                            //}

                            else if (dt.Rows[j].ItemArray[k].ToString() == "SUPPLIER CAN'T FIXED")
                            {
                                excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(0, 176, 240);
                                excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
                                //for (int i = 1; i < dt.Columns.Count + 1; i++)
                                //{
                                //    //excelWorkSheet.Cells[2, i] = dt.Columns[i - 1].ColumnName;
                                //    if (dt.Columns[i - 1].ColumnName == "Supplier Status")
                                //    {
                                //        excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(255, 0, 0);
                                //        excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
                                //    }
                                //    if (dt.Columns[i - 1].ColumnName == "Walton QC Status")
                                //    {
                                //        excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(0, 176, 240);
                                //        excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
                                //    }

                                //}
                            }


                        }
                    }

                }

            }

            string dd = projectId + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xlsx";
            string files2 = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            excelWorkBook.SaveAs(files2 + "\\" + dd);
            excelWorkBook.Close();
            excelApp.Quit();

            try
            {
                string XlsPath = files2 + "\\" + dd;
                FileInfo fileDet = new System.IO.FileInfo(XlsPath);
                Response.Clear();
                Response.Charset = "UTF-8";
                Response.ContentEncoding = Encoding.UTF8;
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(fileDet.Name));
                Response.AddHeader("Content-Length", fileDet.Length.ToString());
                Response.ContentType = "application/ms-excel";
                Response.WriteFile(fileDet.FullName);
                Response.End();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

     
        #region com
        //foreach (var aa in vms.SwQcHeadAssignsFromPmModels)
        //{
        //    modName = aa.SoftwareVersionName;

        //    WebGrid grid = new WebGrid(source: allCust, canPage: false, canSort: false);

        //    string gridData = grid.GetHtml(
        //        columns: grid.Columns(
        //                grid.Column("ProjectName", "Project Name"),
        //                grid.Column("SoftwareVersionName", "SoftwareVersionName"),
        //                grid.Column("SoftwareVersionNo", "SoftwareVersionNo")

        //                )
        //            ).ToString();

        //    Response.ClearContent();
        //    Response.AddHeader("content-disposition", "attachment; filename=CustomerInfo.xls");

        //    Response.ContentType = "application/excel";
        //    Response.Write(gridData);
        //    Response.End();

        //}
        #endregion
        #endregion

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

            var getVersionName = _projectManagerRepository.GetAllVersionNameForPm(swVerNo, proId, testPhases);

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
        public JsonResult SoftwareVersionNameLoad(string swVersionNo, string projectId)
        {
            var _dbEntities = new CellPhoneProjectEntities();
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name);
            var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);

            long swVerNo;
            long.TryParse(swVersionNo, out swVerNo);

            long proId;
            long.TryParse(projectId, out proId);

            var getVersionName = _projectManagerRepository.GetVersionNameForPm(swVerNo, proId);

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

        #region Excel Supplier Feedback
        //Added By Fahad

        [HttpGet]
        public ActionResult ExcelUploaderSupplierFeedback()
        {
            var swQcIssueExcelModel = new SwQcIssueExcelModel();
            var vmSwQcSpecification = new VmSwQcSpecificationModified();
            long userId = Convert.ToInt64(User.Identity.Name);

            var modelList = new List<ProjectMasterModel>();

            modelList = _projectManagerRepository.GetProjectListForSwQcHead();

            var selectListItems = new List<SelectListItem>();
            //{ new SelectListItem { Value = "0", Text = "Select One" } }

            var query = (from master in modelList
                         // where master.SwQcHeadStatus != "NEW" || master.SwQcHeadStatus != "INACTIVE"
                         select new
                         {
                             master.ProjectName,

                         }).ToList();

            foreach (var t in query)
            {
                selectListItems.Add(new SelectListItem
                {
                    Value = t.ProjectName,
                    Text = t.ProjectName
                });
            }
            ViewBag.ProjectId = selectListItems;
            //Test phase//
            var selectListItemsTestPhase = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Select Test Phase" } };
            List<SwQcTestPhaseModel> listTestPhase = _projectManagerRepository.GetSwQcTestPhaseForSuppDemo() ??
                                                     new List<SwQcTestPhaseModel>();
            selectListItemsTestPhase.AddRange(listTestPhase.Select(p => new SelectListItem { Value = p.TestPhaseID.ToString(), Text = p.TestPhaseName }));
            ViewBag.CombinedIdsForTestPhase = selectListItemsTestPhase;
            return View();
        }

        [HttpPost]
        public ActionResult ExcelUploaderSupplierFeedback(SwQcIssueExcelModel swQcIssueExcelModel)
        {

            long userId = Convert.ToInt64(User.Identity.Name);

            var modelList = new List<ProjectMasterModel>();

            modelList = _projectManagerRepository.GetProjectListForSwQcHead();

            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Select One" } };

            var query = (from master in modelList
                         // where master.SwQcHeadStatus != "NEW" || master.SwQcHeadStatus != "INACTIVE"
                         select new
                         {
                             master.ProjectName,

                         }).ToList();

            foreach (var t in query)
            {

                selectListItems.Add(new SelectListItem
                {
                    Value = t.ProjectName,
                    Text = t.ProjectName
                });
            }


            ViewBag.ProjectId = selectListItems;
            //Test phase//
            var selectListItemsTestPhase = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Select Test Phase" } };
            List<SwQcTestPhaseModel> listTestPhase = _projectManagerRepository.GetSwQcTestPhaseForSuppDemo() ??
                                                     new List<SwQcTestPhaseModel>();
            selectListItemsTestPhase.AddRange(listTestPhase.Select(p => new SelectListItem { Value = p.TestPhaseID.ToString(), Text = p.TestPhaseName }));
            ViewBag.CombinedIdsForTestPhase = selectListItemsTestPhase;

            var result = _projectManagerRepository.UpdateDbByExcel(swQcIssueExcelModel.SelectedProjectName, swQcIssueExcelModel.SoftVersionNo, swQcIssueExcelModel.ExcelFile, swQcIssueExcelModel.CombinedTestPhaseIds);

            ViewBag.Message = result == true ? "Updated Successfully" : "Update Failed";

            return View();
        }
        #endregion

        public ActionResult BTRCModelInformation()
        {
            ViewBag.PMSModels = _projectManagerRepository.GetModelsFromPMS();
            ViewBag.BTRCModels = _projectManagerRepository.GetBTRCModels();
            BTRCRegistrationVM vm = new BTRCRegistrationVM();
            return View(vm);
        }

        public ActionResult BTRCRegistration()
        {
            var btrcmodels = (from mod in _projectManagerRepository.GetBTRCModels()
                              select new SelectListItem()
                               {
                                   Value = mod.ModelID.ToString(),
                                   Text = mod.ProjectModel
                               }).ToList();
            ViewBag.BTRCModels = btrcmodels;
            return View();
        }
        [HttpPost]
        public ActionResult GetProjectMaster(string sdate, string edate, long pmid = 0)
        {
            BTRCRegistrationVM vm = new BTRCRegistrationVM();

            if (pmid > 0)
            {
                DateTime FromDate = DateTime.MinValue;
                DateTime ToDate = DateTime.MaxValue;
                DateTime.TryParseExact(sdate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out FromDate);
                DateTime.TryParseExact(edate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out ToDate);
                vm.StartDate = FromDate;
                vm.EndDate = ToDate;
                vm.ProjectMasterId = pmid;
                var data = _projectManagerRepository.GetProjectMasterInfo(pmid);
                vm.ProjectMaster = data;

                return PartialView("~/Views/ProjectManager/Partial/_ProjectMasterInfo.cshtml", vm);
            }
            else
                return View();
        }

        [HttpGet]
        public JsonResult GetLatestRBSYIMEIs(string sdate, string edate)
        {
            DateTime FromDate = DateTime.MinValue;
            DateTime ToDate = DateTime.MaxValue;
            DateTime.TryParseExact(sdate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out FromDate);
            DateTime.TryParseExact(edate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out ToDate);

            var data = _projectManagerRepository.GetLatestIMEIs(FromDate, ToDate);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetModelsBarCodeInv(string sdate, string edate)
        {
            DateTime FromDate = DateTime.MinValue;
            DateTime ToDate = DateTime.MaxValue;
            sdate = sdate + " 12:00:00 AM";
            edate = edate + " 11:59:59 PM";
            DateTime.TryParseExact(sdate, "yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out FromDate);
            DateTime.TryParseExact(edate, "yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out ToDate);

            var data = _projectManagerRepository.GetModelsFromBarCodeInv(FromDate, ToDate);
            ViewBag.BarCodeInvModels = data;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveBTRCData(string sdate, string edate, List<SelectListItem> models)
        {
            DateTime FromDate = DateTime.MinValue;
            DateTime ToDate = DateTime.MaxValue;
            sdate = sdate + " 12:00:00 AM";
            edate = edate + " 11:59:59 PM";
            DateTime.TryParseExact(sdate, "yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out FromDate);
            DateTime.TryParseExact(edate, "yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out ToDate);
            var result = _projectManagerRepository.SaveBTRCData(FromDate, ToDate, models);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveBTRCModelInfo(BTRCRegistrationVM vm)
        {
            ViewBag.PMSModels = _projectManagerRepository.GetModelsFromPMS();
            ViewBag.BTRCModels = _projectManagerRepository.GetBTRCModels();
            var result = _projectManagerRepository.SaveBTRCModelInformation(vm);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public DataTable GetFirstRow(DateTime sdate, DateTime edate, string modelname)
        {
            DataTable totalhistry = new DataTable();
            BTRCModel btrcmodelinfo = _projectManagerRepository.GetBTRCModel(modelname);

            if (btrcmodelinfo != null)
            {
                var cn = new SqlConnection(_connectionStringCellphone);

                cn.Open();

                totalhistry.Columns.Add("SI No.");
                totalhistry.Columns.Add("Brand");
                totalhistry.Columns.Add("Model Name");
                totalhistry.Columns.Add("Marketing Name");
                totalhistry.Columns.Add("Color");
                totalhistry.Columns.Add("Device Type");
                totalhistry.Columns.Add("App Ref#");

                totalhistry.Columns.Add("Country of Origin");
                totalhistry.Columns.Add("As per IMEI TAC");
                totalhistry.Columns.Add("No. Of SIM");

                totalhistry.Columns.Add("Battery Capacity");
                totalhistry.Columns.Add("Battery Capacity(Tested)");
                totalhistry.Columns.Add("Charger/Adapter Type");
                totalhistry.Columns.Add("Charger Output");

                totalhistry.Columns.Add("Processor");
                totalhistry.Columns.Add("RAM");
                totalhistry.Columns.Add("ROM");
                totalhistry.Columns.Add("NFC");
                totalhistry.Columns.Add("Bluetooth");
                totalhistry.Columns.Add("WLAN");
                totalhistry.Columns.Add("Data Speed");
                totalhistry.Columns.Add("SAR Value");
                totalhistry.Columns.Add("Rear Camera");
                totalhistry.Columns.Add("Front Camera");
                totalhistry.Columns.Add("Camera Resulution (In software)");
                totalhistry.Columns.Add("Radio Interface (2G/3G/LTE/4G)");
                totalhistry.Columns.Add("Supported Spectrum Bands for 2G");
                totalhistry.Columns.Add("Supported Spectrum Bands for 3G");
                totalhistry.Columns.Add("Supported Spectrum Bands for 4G");
                totalhistry.Columns.Add("MotherBoard");
                totalhistry.Columns.Add("MotherBoard/Chipset Name");
                totalhistry.Columns.Add("Operating System");
                totalhistry.Columns.Add("Shipment Mode");
                totalhistry.Columns.Add("Product Type (CBU/CKD/SKD)");
                totalhistry.Columns.Add("Unit Price$ (Import)");
                totalhistry.Columns.Add("Unit Price BDT (MRP)");
                totalhistry.Columns.Add("Marketing Period");
                totalhistry.Columns.Add("IMEI Tac 1");
                totalhistry.Columns.Add("IMEI Tac 2");
                totalhistry.Columns.Add("IMEI Tac 3");
                totalhistry.Columns.Add("IMEI Tac 4");
                totalhistry.Columns.Add("IMEI 1");
                totalhistry.Columns.Add("IMEI 2");
                totalhistry.Columns.Add("IMEI 3");
                totalhistry.Columns.Add("IMEI 4");
                totalhistry.Columns.Add("Serial No.");

                String sql;

                //if (modelname != "")
                //{
                //            sql = String.Format(@"SELECT top 5 im.[ID]
                //                  ,info.[Brand]
                //                  ,info.[ProjectMasterId]
                //                  ,info.[ProjectName]
                //                  ,info.[ProjectModel]
                //                  ,info.[MarketingName]
                //                  ,info.[Color]
                //                  ,info.[DeviceType]
                //                  ,info.[ApplicationRef]
                //                  ,info.[ContryOfOrigin]
                //                  ,info.[SupplierId]
                //                  ,info.[Manufacturer]
                //                  ,info.[SimSlotNumber]
                //                  ,info.[BatteryRating]
                //                  ,info.[BatteryCapacityTested]
                //                  ,info.[ChargerAdapterType]
                //                  ,info.[ChargerRating]
                //                  ,info.[ProcessorName]
                //                  ,info.[Ram]
                //                  ,info.[Rom]
                //                  ,info.[NFC]
                //                  ,info.[Bluetooth]
                //                  ,info.[WLAN]
                //                  ,info.[DataSpeed]
                //                  ,info.[SARValue]
                //                  ,info.[FrontCamera]
                //                  ,info.[BackCamera]
                //                  ,info.[CameraResulution]
                //                  ,info.[RadioInterface]
                //                  ,info.[SecondGen]
                //                  ,info.[ThirdGen]
                //                  ,info.[FourthGen]
                //                  ,info.[Motherboard]
                //                  ,info.[ChipsetName]
                //                  ,info.[OSName]
                //                  ,info.[ShipmentMode]
                //                  ,info.[SourcingType]
                //                  ,info.[UnitPrice]
                //                  ,info.[PriceBDT]
                //                  ,info.[MarketingPeriod]
                //                  ,im.[IMEITac1]
                //                  ,im.[IMEITac2]
                //                  ,im.[IMEITac3]
                //                  ,im.[IMEITac4]
                //                  ,im.[IMEI1]
                //                  ,im.[IMEI2]
                //                  ,im.[IMEI3]
                //                  ,im.[IMEI4]
                //                  ,info.[SerialNo]
                //                  ,info.[Added]
                //                  ,info.[AddedDate]
                //                  ,info.[Updated]
                //                  ,info.[UpdatedDate]
                //               FROM [CellPhoneProject].[dbo].[BTRCIMEIRegistration] im inner join [dbo].[BTRCModels] info
                //                 on im.BTRCModelId=info.ModelID
                //                where info.ProjectModel='{0}'", modelname);

                sql = String.Format(@"SELECT top 5 [BarCode] ,[BarCode2] ,[DateAdded], [Color]
               FROM [RBSYNERGY].[dbo].[tblBarCodeInv]  
               where Model='{0}' and DateAdded>='{1}' and DateAdded<='{2}' ", modelname, sdate, edate);

                //}
                //else
                //{
                //    sql = String.Format(@" select * from CellPhoneProject.dbo.SwQcIssueDetails where ProjectName='{0}' and IsApprovedForChina=1  and SoftwareVersionName='{1}' order by SoftwareVersionNo,IssueSerial asc", projectId.Trim(), softVersionName.Trim().Trim());

                //}

                SqlCommand cmd = new SqlCommand(sql, cn);
                var count = 1;
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        DataRow newRow = totalhistry.NewRow();
                        // totalhistry.Rows.Add((String)rdr["ProjectName"], (String)rdr["SoftwareVersionName"], (String)rdr["IssueScenario"]);

                        //long entityid = Int64.Parse(rdr["ID"].ToString());
                        newRow["SI No."] = count;
                        newRow["Brand"] = btrcmodelinfo.SerialNo;
                        newRow["Model Name"] = btrcmodelinfo.ProjectModel;
                        newRow["Marketing Name"] = btrcmodelinfo.MarketingName;
                        newRow["Color"] = rdr["Color"].ToString();
                        newRow["Device Type"] = btrcmodelinfo.DeviceType;
                        newRow["App Ref#"] = btrcmodelinfo.ApplicationRef;
                        newRow["Country of Origin"] = btrcmodelinfo.ContryOfOrigin;
                        newRow["As per IMEI TAC"] = btrcmodelinfo.Manufacturer;
                        newRow["No. Of SIM"] = btrcmodelinfo.SimSlotNumber;
                        newRow["Battery Capacity"] = btrcmodelinfo.BatteryRating;
                        newRow["Battery Capacity(Tested)"] = btrcmodelinfo.BatteryCapacityTested;
                        newRow["Charger/Adapter Type"] = btrcmodelinfo.ChargerAdapterType;
                        newRow["Charger Output"] = btrcmodelinfo.ChargerRating;
                        newRow["Processor"] = btrcmodelinfo.ProcessorName;
                        newRow["RAM"] = btrcmodelinfo.Ram;
                        newRow["ROM"] = btrcmodelinfo.Rom;
                        newRow["NFC"] = btrcmodelinfo.NFC;
                        newRow["Bluetooth"] = btrcmodelinfo.Bluetooth;
                        newRow["WLAN"] = btrcmodelinfo.WLAN;
                        newRow["Data Speed"] = btrcmodelinfo.DataSpeed;
                        newRow["SAR Value"] = btrcmodelinfo.SARValue;
                        newRow["Rear Camera"] = btrcmodelinfo.BackCamera;
                        newRow["Front Camera"] = btrcmodelinfo.FrontCamera;
                        newRow["Camera Resulution (In software)"] = btrcmodelinfo.CameraResulution;
                        newRow["Radio Interface (2G/3G/LTE/4G)"] = btrcmodelinfo.RadioInterface;
                        newRow["Supported Spectrum Bands for 2G"] = btrcmodelinfo.SecondGen;
                        newRow["Supported Spectrum Bands for 2G"] = btrcmodelinfo.ThirdGen;
                        newRow["Supported Spectrum Bands for 2G"] = btrcmodelinfo.FourthGen;
                        newRow["Motherboard"] = btrcmodelinfo.Motherboard;

                        newRow["MotherBoard/Chipset Name"] = btrcmodelinfo.ChipsetName;
                        newRow["Operating System"] = btrcmodelinfo.OSName;
                        newRow["Shipment Mode"] = btrcmodelinfo.ShipmentMode;
                        newRow["Product Type (CBU/CKD/SKD)"] = btrcmodelinfo.SourcingType;
                        newRow["Unit Price$ (Import)"] = btrcmodelinfo.UnitPrice;
                        newRow["Unit Price BDT (MRP)"] = btrcmodelinfo.PriceBDT;
                        newRow["Marketing Period"] = btrcmodelinfo.MarketingPeriod;
                        newRow["IMEI Tac 1"] = rdr["BarCode"].ToString().Substring(0, 8);
                        newRow["IMEI Tac 2"] = rdr["BarCode2"].ToString().Substring(0, 8);
                        newRow["IMEI Tac 3"] = "";
                        newRow["IMEI Tac 4"] = "";
                        newRow["IMEI 1"] = rdr["BarCode"].ToString();
                        newRow["IMEI 2"] = rdr["BarCode2"].ToString();
                        newRow["IMEI 3"] = "";
                        newRow["IMEI 4"] = "";
                        newRow["Serial No."] = btrcmodelinfo.SerialNo;
                        totalhistry.Rows.Add(newRow);
                        //UpdateBRTCData(entityid);
                        count++;
                    }
                }
                cn.Close();
            }
            return totalhistry;
        }

        public DataTable GetIMEIRecords(string modelname, string stdate, string eddate)
        {
            DataTable totalhistry = new DataTable();
            BTRCModel btrcmodelinfo = _projectManagerRepository.GetBTRCModel(modelname.Trim());
            if (btrcmodelinfo != null)
            {
                var cn =
                   new SqlConnection(
                       "Data Source=test;initial catalog=WCMS;persist security info=True;user id=test;password=test;MultipleActiveResultSets=True;App=EntityFramework");

                cn.Open();

                totalhistry.Columns.Add("SI No.");
                totalhistry.Columns.Add("Brand");
                totalhistry.Columns.Add("Model Name");
                totalhistry.Columns.Add("Marketing Name");
                totalhistry.Columns.Add("Color");
                totalhistry.Columns.Add("Device Type");
                totalhistry.Columns.Add("App Ref#");

                totalhistry.Columns.Add("Country of Origin");
                totalhistry.Columns.Add("As per IMEI TAC");
                totalhistry.Columns.Add("No. Of SIM");

                totalhistry.Columns.Add("Battery Capacity");
                totalhistry.Columns.Add("Battery Capacity(Tested)");
                totalhistry.Columns.Add("Charger/Adapter Type");
                totalhistry.Columns.Add("Charger Output");

                totalhistry.Columns.Add("Processor");
                totalhistry.Columns.Add("RAM");
                totalhistry.Columns.Add("ROM");
                totalhistry.Columns.Add("NFC");
                totalhistry.Columns.Add("Bluetooth");
                totalhistry.Columns.Add("WLAN");
                totalhistry.Columns.Add("Data Speed");
                totalhistry.Columns.Add("SAR Value");
                totalhistry.Columns.Add("Rear Camera");
                totalhistry.Columns.Add("Front Camera");
                totalhistry.Columns.Add("Camera Resulution (In software)");
                totalhistry.Columns.Add("Radio Interface (2G/3G/LTE/4G)");
                totalhistry.Columns.Add("Supported Spectrum Bands for 2G");
                totalhistry.Columns.Add("Supported Spectrum Bands for 3G");
                totalhistry.Columns.Add("Supported Spectrum Bands for 4G");
                totalhistry.Columns.Add("MotherBoard");
                totalhistry.Columns.Add("MotherBoard/Chipset Name");
                totalhistry.Columns.Add("Operating System");
                totalhistry.Columns.Add("Shipment Mode");
                totalhistry.Columns.Add("Product Type (CBU/CKD/SKD)");
                totalhistry.Columns.Add("Unit Price$ (Import)");
                totalhistry.Columns.Add("Unit Price BDT (MRP)");
                totalhistry.Columns.Add("Marketing Period");
                totalhistry.Columns.Add("IMEI Tac 1");
                totalhistry.Columns.Add("IMEI Tac 2");
                totalhistry.Columns.Add("IMEI Tac 3");
                totalhistry.Columns.Add("IMEI Tac 4");
                totalhistry.Columns.Add("IMEI 1");
                totalhistry.Columns.Add("IMEI 2");
                totalhistry.Columns.Add("IMEI 3");
                totalhistry.Columns.Add("IMEI 4");
                totalhistry.Columns.Add("Serial No.");

                string sql;
                var modelArray = modelname.Trim().Split(' ');
                var contModel = modelArray[0] + modelArray[1];// + modelArray[2];
                contModel = contModel.ToUpper();
                var model = Regex.Replace(modelname, @"\s+", "");
                model = model.ToUpper();
                sql = string.Format(@"SELECT [IMEI1] ,[IMEI2] ,[Color], [Model]
               FROM [WCMS].[dbo].[tblIMEIRecord]  
               where UPPER(REPLACE(Model, ' ', '')) = '{0}' AND CONVERT(date,AddedDate) BETWEEN '{1}' AND '{2}'  ", contModel, stdate, eddate);

                //}
                //else
                //{
                //    sql = String.Format(@" select * from CellPhoneProject.dbo.SwQcIssueDetails where ProjectName='{0}' and IsApprovedForChina=1  and SoftwareVersionName='{1}' order by SoftwareVersionNo,IssueSerial asc", projectId.Trim(), softVersionName.Trim().Trim());

                //}

                SqlCommand cmd = new SqlCommand(sql, cn);
                var count = 1;
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        DataRow newRow = totalhistry.NewRow();
                        // totalhistry.Rows.Add((String)rdr["ProjectName"], (String)rdr["SoftwareVersionName"], (String)rdr["IssueScenario"]);

                        //long entityid = Int64.Parse(rdr["ID"].ToString());
                        newRow["SI No."] = count;
                        newRow["Brand"] = "WALTON";//btrcmodelinfo.SerialNo;
                        newRow["Model Name"] = btrcmodelinfo.ProjectModel;
                        newRow["Marketing Name"] = btrcmodelinfo.MarketingName;
                        newRow["Color"] = rdr["Color"].ToString();
                        newRow["Device Type"] = btrcmodelinfo.DeviceType;
                        newRow["App Ref#"] = btrcmodelinfo.ApplicationRef;
                        newRow["Country of Origin"] = btrcmodelinfo.ContryOfOrigin;
                        newRow["As per IMEI TAC"] = btrcmodelinfo.Manufacturer;
                        newRow["No. Of SIM"] = btrcmodelinfo.SimSlotNumber;
                        newRow["Battery Capacity"] = btrcmodelinfo.BatteryRating;
                        newRow["Battery Capacity(Tested)"] = btrcmodelinfo.BatteryCapacityTested;
                        newRow["Charger/Adapter Type"] = btrcmodelinfo.ChargerAdapterType;
                        newRow["Charger Output"] = btrcmodelinfo.ChargerRating;
                        newRow["Processor"] = btrcmodelinfo.ProcessorName;
                        newRow["RAM"] = btrcmodelinfo.Ram;
                        newRow["ROM"] = btrcmodelinfo.Rom;
                        newRow["NFC"] = btrcmodelinfo.NFC;
                        newRow["Bluetooth"] = btrcmodelinfo.Bluetooth;
                        newRow["WLAN"] = btrcmodelinfo.WLAN;
                        newRow["Data Speed"] = btrcmodelinfo.DataSpeed;
                        newRow["SAR Value"] = btrcmodelinfo.SARValue;
                        newRow["Rear Camera"] = btrcmodelinfo.BackCamera;
                        newRow["Front Camera"] = btrcmodelinfo.FrontCamera;
                        newRow["Camera Resulution (In software)"] = btrcmodelinfo.CameraResulution;
                        newRow["Radio Interface (2G/3G/LTE/4G)"] = btrcmodelinfo.RadioInterface;
                        newRow["Supported Spectrum Bands for 2G"] = btrcmodelinfo.SecondGen;
                        newRow["Supported Spectrum Bands for 3G"] = btrcmodelinfo.ThirdGen;
                        newRow["Supported Spectrum Bands for 4G"] = btrcmodelinfo.FourthGen;
                        newRow["MotherBoard"] = btrcmodelinfo.Motherboard;

                        newRow["MotherBoard/Chipset Name"] = btrcmodelinfo.ChipsetName;
                        newRow["Operating System"] = btrcmodelinfo.OSName;
                        newRow["Shipment Mode"] = btrcmodelinfo.ShipmentMode;
                        newRow["Product Type (CBU/CKD/SKD)"] = btrcmodelinfo.SourcingType;
                        newRow["Unit Price$ (Import)"] = btrcmodelinfo.UnitPrice;
                        newRow["Unit Price BDT (MRP)"] = btrcmodelinfo.PriceBDT;
                        newRow["Marketing Period"] = btrcmodelinfo.MarketingPeriod;
                        newRow["IMEI Tac 1"] = rdr["IMEI1"].ToString().Substring(0, 8);
                        newRow["IMEI Tac 2"] = rdr["IMEI2"].ToString().Substring(0, 8);
                        newRow["IMEI Tac 3"] = "";
                        newRow["IMEI Tac 4"] = "";
                        newRow["IMEI 1"] = rdr["IMEI1"].ToString();
                        newRow["IMEI 2"] = rdr["IMEI2"].ToString();
                        newRow["IMEI 3"] = "";
                        newRow["IMEI 4"] = "";
                        newRow["Serial No."] = btrcmodelinfo.SerialNo;
                        totalhistry.Rows.Add(newRow);
                        //UpdateBRTCData(entityid);
                        count++;
                    }
                }
                cn.Close();
            }
            return totalhistry;
        }
        public static void UpdateBRTCData(long id)
        {
            try
            {
                var cn =
                       new SqlConnection(
                           "Data Source=test;initial catalog=CellPhoneProject;persist security info=True;user id=test;password=test;MultipleActiveResultSets=True;App=EntityFramework");
                cn.Open();
                var sql = String.Format(@"Update [CellPhoneProject].[dbo].[BTRCIMEIRegistration] set Exported=1 where ID={0}", id);
                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.ExecuteNonQuery();
                cn.Close();
                //return true;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                // false;
            }
        }

        public ActionResult GetExcel1(DataTable ds, string sdate, string edate, string models)
        {
            if (models == "")
                return null;
            DataTable dt = GetIMEIRecords(models, sdate, edate);
            var memoryStream = new MemoryStream();
            using (var excelPackage = new ExcelPackage(memoryStream))
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add(models);
                worksheet.Cells["A1"].LoadFromDataTable(dt, true, TableStyles.Light21);
                worksheet.Cells["A1:AN1"].Style.Font.Bold = true;
                worksheet.DefaultRowHeight = 18;


                worksheet.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                worksheet.Column(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Column(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.DefaultColWidth = 20;
                worksheet.Column(2).AutoFit();


                byte[] data = excelPackage.GetAsByteArray();//Session["DownloadExcel_FileManager"] as byte[];
                return File(data, "application/octet-stream", "Walton_MobileIMEIs_Part_last" + DateTime.Now.ToString("_yyyyMMdd_hhmmss") + ".xlsx");

                //Session["DownloadExcel_FileManager"] = excelPackage.GetAsByteArray();
                //if (Session["DownloadExcel_FileManager"] != null)
                //{
                //    byte[] data = excelPackage.GetAsByteArray();//Session["DownloadExcel_FileManager"] as byte[];
                //    return File(data, "application/octet-stream", "Walton_MobileIMEIs_Part_last" + DateTime.Now.ToString("_yyyyMMdd_hhmmss") + ".xlsx");
                //}
                //return Json("", JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult GetExcel(DataTable ds, string sdate, string edate, string models)
        {
            DateTime FromDate = DateTime.MinValue;
            DateTime ToDate = DateTime.MaxValue;
            if ((sdate != null) && (edate != null))
            {
                sdate = sdate + " 12:00:00 AM";
                edate = edate + " 11:59:59 PM";
            }

            DateTime.TryParseExact(sdate, "yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out FromDate);
            DateTime.TryParseExact(edate, "yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out ToDate);


            List<SelectListItem> exportablemodels = new List<SelectListItem>();
            if (models == "")
                exportablemodels = _projectManagerRepository.GetModelsFromBarCodeInv(FromDate, ToDate);

            //List<SelectListItem> models = new List<SelectListItem> { new SelectListItem() { Value = "252", Text = "Primo S7" } };

            // var temp = Server.MapPath("~/temp");
            //string fileName = String.Format(@"{0}\type1.txt", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            var root = Server.MapPath("~/");
            var temp = string.Format(@"{0}{1}\{2}", root, "Content", "UploadImage");
            var zipdest = string.Format(@"{0}{1}", root, "archive");
            Directory.EnumerateFiles(temp).ToList().ForEach(f => System.IO.File.Delete(f));


            List<string> selectedmodels = new List<string>();
            if (models == "")
            {
                foreach (var item in exportablemodels)
                {
                    selectedmodels.Add(item.Text);
                }
            }
            else
                selectedmodels = models.Split(',').ToList();

            List<ModelExcel> modelexcels = new List<ModelExcel>();

            foreach (var table in selectedmodels)
            {
                var modelname = table;
                DataTable dt = new DataTable();
                if ((sdate == null) || (edate == null))
                    dt = GetIMEIRecords(modelname, sdate, edate);
                else
                    dt = GetFirstRow(FromDate, ToDate, modelname);
                if (dt.Rows.Count > 0)
                {
                    ModelExcel item = new ModelExcel();
                    item.ModelName = modelname;
                    item.ImeiData = dt;
                    modelexcels.Add(item);
                }
            }

            bool isReadNotComplete = true;
            int lastreadedModelindex = 0;
            int lastrowinddex = 0;
            int thresholdlimit = 10000;
            int excelfilecount = 0;
            while (isReadNotComplete)
            {
                int currendatarowcount = 0;
                // var filename = "excelApp" + excelfilecount;

                Excel.Application excelApp = new Excel.Application();


                Excel.Workbook excelWorkBook = excelApp.Workbooks.Add("");
                //take a new file 
                for (int ii = lastreadedModelindex; ii < modelexcels.Count; ii++)
                {
                    if (currendatarowcount > thresholdlimit)
                    {
                        currendatarowcount = 0;
                        break;
                    }
                    //take a new tab
                    Excel.Worksheet excelWorkSheet = excelWorkBook.Sheets.Add();
                    excelWorkSheet.Name = modelexcels[ii].ModelName;
                    //freeze row//

                    #region Excel Column Design Area
                    //Adjust all row
                    excelWorkSheet.Rows.AutoFit();


                    //Column width and Hight//
                    //excelWorkSheet.Range["A5", "AT5"].Rows.RowHeight = 10;
                    excelWorkSheet.Columns[1].ColumnWidth = 5;
                    excelWorkSheet.Columns[2].ColumnWidth = 20;
                    excelWorkSheet.Columns[3].ColumnWidth = 30;
                    excelWorkSheet.Columns[4].ColumnWidth = 25;
                    excelWorkSheet.Columns[5].ColumnWidth = 10;
                    excelWorkSheet.Columns[6].ColumnWidth = 20;
                    excelWorkSheet.Columns[7].ColumnWidth = 15;
                    excelWorkSheet.Columns[8].ColumnWidth = 10;
                    excelWorkSheet.Columns[9].ColumnWidth = 30;
                    excelWorkSheet.Columns[10].ColumnWidth = 5;
                    excelWorkSheet.Columns[11].ColumnWidth = 15;
                    excelWorkSheet.Columns[12].ColumnWidth = 15;
                    excelWorkSheet.Columns[13].ColumnWidth = 10;
                    excelWorkSheet.Columns[14].ColumnWidth = 15;

                    excelWorkSheet.Columns[15].ColumnWidth = 10;
                    excelWorkSheet.Columns[16].ColumnWidth = 5;
                    excelWorkSheet.Columns[17].ColumnWidth = 5;
                    excelWorkSheet.Columns[18].ColumnWidth = 5;
                    excelWorkSheet.Columns[19].ColumnWidth = 10;
                    excelWorkSheet.Columns[20].ColumnWidth = 10;
                    excelWorkSheet.Columns[21].ColumnWidth = 10;
                    excelWorkSheet.Columns[22].ColumnWidth = 10;
                    excelWorkSheet.Columns[23].ColumnWidth = 10;
                    excelWorkSheet.Columns[24].ColumnWidth = 10;
                    excelWorkSheet.Columns[25].ColumnWidth = 10;
                    excelWorkSheet.Columns[26].ColumnWidth = 10;
                    excelWorkSheet.Columns[27].ColumnWidth = 10;
                    excelWorkSheet.Columns[28].ColumnWidth = 10;
                    excelWorkSheet.Columns[29].ColumnWidth = 10;
                    excelWorkSheet.Columns[30].ColumnWidth = 10;
                    excelWorkSheet.Columns[31].ColumnWidth = 10;
                    excelWorkSheet.Columns[32].ColumnWidth = 10;
                    excelWorkSheet.Columns[33].ColumnWidth = 10;
                    excelWorkSheet.Columns[34].ColumnWidth = 10;
                    excelWorkSheet.Columns[35].ColumnWidth = 10;
                    excelWorkSheet.Columns[36].ColumnWidth = 10;
                    excelWorkSheet.Columns[37].ColumnWidth = 20;
                    excelWorkSheet.Columns[38].ColumnWidth = 15;
                    excelWorkSheet.Columns[39].ColumnWidth = 15;
                    excelWorkSheet.Columns[40].ColumnWidth = 15;
                    excelWorkSheet.Columns[41].ColumnWidth = 15;
                    excelWorkSheet.Columns[42].ColumnWidth = 20;
                    excelWorkSheet.Columns[43].ColumnWidth = 20;
                    excelWorkSheet.Columns[44].ColumnWidth = 20;
                    excelWorkSheet.Columns[45].ColumnWidth = 20;
                    excelWorkSheet.Columns[46].ColumnWidth = 20;
                    /////////
                    excelWorkSheet.Range[excelWorkSheet.Cells[1, 1], excelWorkSheet.Cells[1, 35]].Merge();
                    excelWorkSheet.Range[excelWorkSheet.Cells[2, 1], excelWorkSheet.Cells[2, 35]].Merge();
                    excelWorkSheet.Range[excelWorkSheet.Cells[3, 1], excelWorkSheet.Cells[3, 35]].Merge();
                    excelWorkSheet.Range[excelWorkSheet.Cells[4, 1], excelWorkSheet.Cells[4, 35]].Merge();

                    //For Header Name//
                    excelWorkSheet.Cells[2, 1] = "Name of Organization:";

                    excelWorkSheet.get_Range("A2", "T2").Font.Bold = true;
                    excelWorkSheet.get_Range("A2", "T2").Font.Name = "Arial";
                    excelWorkSheet.get_Range("A2", "T2").Font.Size = 16;
                    excelWorkSheet.get_Range("A2", "T2").Font.Underline = true;
                    excelWorkSheet.get_Range("A2", "T2").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    excelWorkSheet.get_Range("A2", "T2").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                    excelWorkSheet.get_Range("A2", "T2").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;



                    excelWorkSheet.Cells[3, 1] = "Enlistment Validity:";
                    excelWorkSheet.get_Range("A3", "T3").Font.Bold = true;
                    excelWorkSheet.get_Range("A3", "T3").Font.Name = "Arial";
                    excelWorkSheet.get_Range("A3", "T3").Font.Size = 16;
                    excelWorkSheet.get_Range("A3", "T3").Font.Underline = true;
                    excelWorkSheet.get_Range("A3", "T3").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    excelWorkSheet.get_Range("A3", "T3").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                    excelWorkSheet.get_Range("A3", "T3").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;


                    excelWorkSheet.Range["A5", "AT5"].Application.ActiveWindow.SplitRow = 5;
                    excelWorkSheet.Range["A5", "AT5"].Rows.Application.ActiveWindow.FreezePanes = true;
                    excelWorkSheet.Rows[5].WrapText = true;

                    ////For Issue List Color Group 1//

                    excelWorkSheet.get_Range("A5", "AT5").Font.Bold = true;
                    excelWorkSheet.get_Range("A5", "AT5").Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.RosyBrown);
                    excelWorkSheet.get_Range("A5", "AT5").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    excelWorkSheet.get_Range("A5", "AT5").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                    excelWorkSheet.get_Range("A5", "AT5").Application.StandardFont = "Calibri";
                    excelWorkSheet.get_Range("A5", "AT5").Application.StandardFontSize = 11;
                    #endregion

                    //lastreadedModelindex = 0;
                    if (lastreadedModelindex != ii) lastrowinddex = 0;
                    bool IsColumnformated = false;
                    int excelrow = 0;

                    var memoryStream = new MemoryStream();
                    using (var excelPackage = new ExcelPackage(memoryStream))
                    {
                        var worksheet = excelPackage.Workbook.Worksheets.Add(modelexcels[ii].ModelName);
                        worksheet.Cells["A1"].LoadFromDataTable(modelexcels[ii].ImeiData, true, TableStyles.None);
                        worksheet.Cells["A1:AN1"].Style.Font.Bold = true;
                        worksheet.DefaultRowHeight = 18;


                        worksheet.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        worksheet.Column(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.Column(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        worksheet.DefaultColWidth = 20;
                        worksheet.Column(2).AutoFit();



                        byte[] data = excelPackage.GetAsByteArray();//Session["DownloadExcel_FileManager"] as byte[];
                        return File(data, "application/octet-stream", "Walton_MobileIMEIs_Part_last" + DateTime.Now.ToString("_yyyyMMdd_hhmmss") + ".xlsx");

                        //Session["DownloadExcel_FileManager"] = excelPackage.GetAsByteArray();
                        //if (Session["DownloadExcel_FileManager"] != null)
                        //{
                        //    byte[] data = excelPackage.GetAsByteArray();//Session["DownloadExcel_FileManager"] as byte[];
                        //    return File(data, "application/octet-stream", "Walton_MobileIMEIs_Part_last" + DateTime.Now.ToString("_yyyyMMdd_hhmmss") + ".xlsx");
                        //}
                        //return Json("", JsonRequestBehavior.AllowGet);
                    }
                    //for (int jj = lastrowinddex; jj < modelexcels[ii].ImeiData.Rows.Count; jj++)
                    //{

                    //    if (currendatarowcount < thresholdlimit)
                    //    {
                    //        //Excell Generation
                    //        lastreadedModelindex = ii;
                    //        lastrowinddex = jj;
                    //        #region Excel Data Writing Area
                    //        if (!IsColumnformated)
                    //        {
                    //            IsColumnformated = true;
                    //            for (int i = 1; i < modelexcels[ii].ImeiData.Columns.Count + 1; i++)
                    //            {
                    //                excelWorkSheet.Cells[5, i] = modelexcels[ii].ImeiData.Columns[i - 1].ColumnName;
                    //                excelWorkSheet.Cells[5, i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    //                excelWorkSheet.Cells[5, i].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    //                //border
                    //                //excelWorkSheet.Cells[5, i].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                    //                excelWorkSheet.Cells[5, i].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                    //                excelWorkSheet.Cells[5, i].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                    //                excelWorkSheet.Cells[5, i].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    //                excelWorkSheet.Cells[5, i].Borders.Weight = Excel.XlBorderWeight.xlThin;
                    //            }
                    //        }


                    //        //for (int j = 0; j < modelexcels[ii].ImeiData.Rows.Count; j++)
                    //        //{
                    //        for (int k = 0; k < modelexcels[ii].ImeiData.Columns.Count; k++)
                    //        {
                    //            //int rowindex=jj + 6- lastrowinddex;
                    //            excelWorkSheet.Cells[excelrow + 6, k + 1] = modelexcels[ii].ImeiData.Rows[jj].ItemArray[k].ToString();
                    //            excelWorkSheet.Cells[excelrow + 6, 42].NumberFormat = "0";
                    //            excelWorkSheet.Cells[excelrow + 6, 43].NumberFormat = "0";
                    //            excelWorkSheet.Cells[excelrow + 6, 44].NumberFormat = "0";
                    //            excelWorkSheet.Cells[excelrow + 6, 45].NumberFormat = "0";
                    //            //border
                    //            excelWorkSheet.Cells[excelrow + 6, k + 1].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                    //            excelWorkSheet.Cells[excelrow + 6, k + 1].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                    //            excelWorkSheet.Cells[excelrow + 6, k + 1].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                    //            excelWorkSheet.Cells[excelrow + 6, k + 1].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    //            excelWorkSheet.Cells[excelrow + 6, k + 1].Borders.Weight = Excel.XlBorderWeight.xlThin;
                    //        }

                    //        // }
                    //        #endregion

                    //        currendatarowcount++;
                    //        excelrow++;
                    //    }
                    //    else
                    //    {
                    //        lastreadedModelindex = ii;
                    //        lastrowinddex = jj;
                    //        string dd = "Walton_MobileIMEIs_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xlsx";
                    //        string files2 = temp + "\\" + dd;
                    //        excelWorkBook.SaveAs(temp + "\\" + dd);
                    //        //excelWorkBook.Close();
                    //        //excelApp.Quit();
                    //        break;
                    //    }

                    //}

                }

                //if (lastreadedModelindex == (modelexcels.Count - 1))// && lastrowinddex == (modelexcels[lastreadedModelindex].ImeiData.Rows.Count - 1)
                //    isReadNotComplete = false;
                //if (isReadNotComplete == false)
                //{
                //    string dd2 = "Walton_MobileIMEIs_Part_last" + DateTime.Now.ToString("_yyyyMMdd_hhmmss") + ".xlsx";
                //    //string files22 = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                //    string files22 = temp + "\\" + dd2;
                //    excelWorkBook.SaveAs(temp + "\\" + dd2);
                //    excelWorkBook.Close();
                //    excelApp.Quit();

                //}
                //File Closing
            }

            var archive = zipdest + "\\" + "Walton_MobileIMEIs_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".zip";

            //create a new archive
            ZipFile.CreateFromDirectory(temp, archive);

            try
            {
                string XlsPath = archive;
                FileInfo fileDet = new System.IO.FileInfo(XlsPath);
                Response.Clear();
                Response.Charset = "UTF-8";
                Response.ContentEncoding = Encoding.UTF8;
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(fileDet.Name));
                Response.AddHeader("Content-Length", fileDet.Length.ToString());
                Response.ContentType = "application/x-zip-compressed";
                Response.WriteFile(fileDet.FullName);
                Response.End();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            #region com

            //Creae an Excel application instance
            //var excelrowcounter = 0;

            //List<DataTable> modeldts=new List<DataTable>();
            //long totalrow=0;

            //int excelfileneeded=(int)totalrow/5000;
            ////List<Excel.Application> FinalExcels=new List<Excel.Application>();
            //for(int i=0; i<=excelfileneeded+1; i++)
            //{
            //Excel.Application excelApp = new Excel.Application();
            //Excel.Workbook excelWorkBook = excelApp.Workbooks.Add("");

            //string dd = "Walton_MobileIMEIs_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xlsx";
            //string files2 = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            //excelWorkBook.SaveAs(files2 + "\\" + dd);
            //excelWorkBook.Close();
            //excelApp.Quit();

            //}

            //foreach (var table in models)
            //{

            //    DataTable dt = new DataTable();
            //    var modelname = table.Text;
            //    dt = GetFirstRow(modelname);
            ////    if (excelrowcounter + dt.Rows.Count > 100000)
            ////    {
            ////       // worKsheeT = null;  
            ////       // celLrangE = null;  
            ////       // worKbooK = null; 
            ////       // Excel.Workbook newfile = excelApp.Workbooks.Add("");
            ////       // Excel.Worksheet excelWorkSheet = newfile.Sheets.Add();
            ////        string dd = "Walton_MobileIMEIs_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xlsx";
            ////string files2 = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            ////excelWorkBook.SaveAs(files2 + "\\" + dd);
            ////excelWorkBook.Close();
            ////excelApp.Quit();

            ////    }
            ////    else
            ////    {
            ////        //Add a new worksheet to workbook with the Datatable name

            ////    }

            //    Excel.Worksheet excelWorkSheet = excelWorkBook.Sheets.Add();

            //        excelWorkSheet.Name = table.Text;
            //        //freeze row//

            //        //Adjust all row
            //        excelWorkSheet.Rows.AutoFit();


            //        //Column width and Hight//
            //        //excelWorkSheet.Range["A5", "AT5"].Rows.RowHeight = 10;
            //        excelWorkSheet.Columns[1].ColumnWidth = 5;
            //        excelWorkSheet.Columns[2].ColumnWidth = 20;
            //        excelWorkSheet.Columns[3].ColumnWidth = 30;
            //        excelWorkSheet.Columns[4].ColumnWidth = 25;
            //        excelWorkSheet.Columns[5].ColumnWidth = 10;
            //        excelWorkSheet.Columns[6].ColumnWidth = 20;
            //        excelWorkSheet.Columns[7].ColumnWidth = 15;
            //        excelWorkSheet.Columns[8].ColumnWidth = 10;
            //        excelWorkSheet.Columns[9].ColumnWidth = 30;
            //        excelWorkSheet.Columns[10].ColumnWidth = 5;
            //        excelWorkSheet.Columns[11].ColumnWidth = 15;
            //        excelWorkSheet.Columns[12].ColumnWidth = 15;
            //        excelWorkSheet.Columns[13].ColumnWidth = 10;
            //        excelWorkSheet.Columns[14].ColumnWidth = 15;

            //        excelWorkSheet.Columns[15].ColumnWidth = 10;
            //        excelWorkSheet.Columns[16].ColumnWidth = 5;
            //        excelWorkSheet.Columns[17].ColumnWidth = 5;
            //        excelWorkSheet.Columns[18].ColumnWidth = 5;
            //        excelWorkSheet.Columns[19].ColumnWidth = 10;
            //        excelWorkSheet.Columns[20].ColumnWidth = 10;
            //        excelWorkSheet.Columns[21].ColumnWidth = 10;
            //        excelWorkSheet.Columns[22].ColumnWidth = 10;
            //        excelWorkSheet.Columns[23].ColumnWidth = 10;
            //        excelWorkSheet.Columns[24].ColumnWidth = 10;
            //        excelWorkSheet.Columns[25].ColumnWidth = 10;
            //        excelWorkSheet.Columns[26].ColumnWidth = 10;
            //        excelWorkSheet.Columns[27].ColumnWidth = 10;
            //        excelWorkSheet.Columns[28].ColumnWidth = 10;
            //        excelWorkSheet.Columns[29].ColumnWidth = 10;
            //        excelWorkSheet.Columns[30].ColumnWidth = 10;
            //        excelWorkSheet.Columns[31].ColumnWidth = 10;
            //        excelWorkSheet.Columns[32].ColumnWidth = 10;
            //        excelWorkSheet.Columns[33].ColumnWidth = 10;
            //        excelWorkSheet.Columns[34].ColumnWidth = 10;
            //        excelWorkSheet.Columns[35].ColumnWidth = 10;
            //        excelWorkSheet.Columns[36].ColumnWidth = 10;
            //        excelWorkSheet.Columns[37].ColumnWidth = 20;
            //        excelWorkSheet.Columns[38].ColumnWidth = 15;
            //        excelWorkSheet.Columns[39].ColumnWidth = 15;
            //        excelWorkSheet.Columns[40].ColumnWidth = 15;
            //        excelWorkSheet.Columns[41].ColumnWidth = 15;
            //        excelWorkSheet.Columns[42].ColumnWidth = 20;
            //        excelWorkSheet.Columns[43].ColumnWidth = 20;
            //        excelWorkSheet.Columns[44].ColumnWidth = 20;
            //        excelWorkSheet.Columns[45].ColumnWidth = 20;
            //        excelWorkSheet.Columns[46].ColumnWidth = 20;
            //        /////////
            //        excelWorkSheet.Range[excelWorkSheet.Cells[1, 1], excelWorkSheet.Cells[1, 35]].Merge();
            //        excelWorkSheet.Range[excelWorkSheet.Cells[2, 1], excelWorkSheet.Cells[2, 35]].Merge();
            //        excelWorkSheet.Range[excelWorkSheet.Cells[3, 1], excelWorkSheet.Cells[3, 35]].Merge();
            //        excelWorkSheet.Range[excelWorkSheet.Cells[4, 1], excelWorkSheet.Cells[4, 35]].Merge();

            //        //For Header Name//
            //        excelWorkSheet.Cells[2, 1] = "Name of Organization:";

            //        excelWorkSheet.get_Range("A2", "T2").Font.Bold = true;
            //        excelWorkSheet.get_Range("A2", "T2").Font.Name = "Arial";
            //        excelWorkSheet.get_Range("A2", "T2").Font.Size = 16;
            //        excelWorkSheet.get_Range("A2", "T2").Font.Underline = true;
            //        excelWorkSheet.get_Range("A2", "T2").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
            //        excelWorkSheet.get_Range("A2", "T2").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
            //        excelWorkSheet.get_Range("A2", "T2").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;

            //        excelWorkSheet.Cells[3, 1] = "Enlistment Validity:";
            //        excelWorkSheet.get_Range("A3", "T3").Font.Bold = true;
            //        excelWorkSheet.get_Range("A3", "T3").Font.Name = "Arial";
            //        excelWorkSheet.get_Range("A3", "T3").Font.Size = 16;
            //        excelWorkSheet.get_Range("A3", "T3").Font.Underline = true;
            //        excelWorkSheet.get_Range("A3", "T3").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
            //        excelWorkSheet.get_Range("A3", "T3").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
            //        excelWorkSheet.get_Range("A3", "T3").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;

            //        excelWorkSheet.Range["A5", "AT5"].Application.ActiveWindow.SplitRow = 5;
            //        excelWorkSheet.Range["A5", "AT5"].Rows.Application.ActiveWindow.FreezePanes = true;
            //        excelWorkSheet.Rows[5].WrapText = true;

            //        ////For Issue List Color Group 1//

            //        excelWorkSheet.get_Range("A5", "AT5").Font.Bold = true;
            //        excelWorkSheet.get_Range("A5", "AT5").Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.RosyBrown);
            //        excelWorkSheet.get_Range("A5", "AT5").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
            //        excelWorkSheet.get_Range("A5", "AT5").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
            //        excelWorkSheet.get_Range("A5", "AT5").Application.StandardFont = "Calibri";
            //        excelWorkSheet.get_Range("A5", "AT5").Application.StandardFontSize = 11;

            //        ////end Issue List Color Group 1


            //        using (dt)
            //        {

            //            for (int i = 1; i < dt.Columns.Count + 1; i++)
            //            {
            //                excelWorkSheet.Cells[5, i] = dt.Columns[i - 1].ColumnName;
            //                excelWorkSheet.Cells[5, i].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            //                excelWorkSheet.Cells[5, i].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;

            //                //border
            //                //excelWorkSheet.Cells[5, i].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            //                excelWorkSheet.Cells[5, i].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //                excelWorkSheet.Cells[5, i].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //                excelWorkSheet.Cells[5, i].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //                excelWorkSheet.Cells[5, i].Borders.Weight = Excel.XlBorderWeight.xlThin;
            //            }

            //            for (int j = 0; j < dt.Rows.Count; j++)
            //            {
            //                for (int k = 0; k < dt.Columns.Count; k++)
            //                {
            //                    excelWorkSheet.Cells[j + 6, k + 1] = dt.Rows[j].ItemArray[k].ToString();
            //                    excelWorkSheet.Cells[j + 6, 42].NumberFormat = "0";
            //                    excelWorkSheet.Cells[j + 6, 43].NumberFormat = "0";
            //                    excelWorkSheet.Cells[j + 6, 44].NumberFormat = "0";
            //                    excelWorkSheet.Cells[j + 6, 45].NumberFormat = "0";
            //                    //border
            //                    excelWorkSheet.Cells[j + 6, k + 1].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            //                    excelWorkSheet.Cells[j + 6, k + 1].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //                    excelWorkSheet.Cells[j + 6, k + 1].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //                    excelWorkSheet.Cells[j + 6, k + 1].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            //                    excelWorkSheet.Cells[j + 6, k + 1].Borders.Weight = Excel.XlBorderWeight.xlThin;
            //                }
            //                excelrowcounter += 1;
            //            }
            //        }
            //}


            //string dd = "Walton_MobileIMEIs_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xlsx";
            //string files2 = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            //excelWorkBook.SaveAs(files2 + "\\" + dd);
            //excelWorkBook.Close();
            //excelApp.Quit();

            //try
            //{
            //    string XlsPath = files2 + "\\" + dd;
            //    FileInfo fileDet = new System.IO.FileInfo(XlsPath);
            //    Response.Clear();
            //    Response.Charset = "UTF-8";
            //    Response.ContentEncoding = Encoding.UTF8;
            //    Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(fileDet.Name));
            //    Response.AddHeader("Content-Length", fileDet.Length.ToString());
            //    Response.ContentType = "application/ms-excel";
            //    Response.WriteFile(fileDet.FullName);
            //    Response.End();
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            //try
            //{
            //    var archive = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //    // create a new archive
            //    ZipFile.CreateFromDirectory(temp, archive);
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            #endregion

            return null;
        }
        public ActionResult RawMaterialInspection(string ProjectMasterId, string bomsTypes, string projectNames)
        {
            var models = new AssignProjectsViewModel();
            ViewBag.Projects = _projectManagerRepository.GetAllProjects();
            ViewBag.AllBOMType = _projectManagerRepository.AllBOMType();

            long proId;
            long.TryParse(ProjectMasterId, out proId);

            if (proId != 0)
            {
                models.ProjectMasterModel = _projectManagerRepository.GetProjectDetails(proId);
                models.ProjectMasterId = proId;

                models.ProjectMasterModelsForBomName = _projectManagerRepository.GetBomName(proId, bomsTypes, projectNames);
                List<SelectListItem> itemsBomName = models.ProjectMasterModelsForBomName.Select(model => new SelectListItem { Text = model.BOMName, Value = model.BOMName.ToString(CultureInfo.InvariantCulture) }).ToList();
                ViewBag.ProjectBomName = itemsBomName;
            }

            return View(models);
        }
        //  data: { proId: proId, bomsTypes: bomsTypes, projectNames: projectNames },
        [HttpPost]
        public JsonResult GetBomName(string proId, string bomsTypes, string projectNames)
        {
            var modelss = new AssignProjectsViewModel();

            long proIds;
            long.TryParse(proId, out proIds);
            var json = "";
            if (proIds != 0)
            {
                modelss.ProjectMasterModelsForBomName = _projectManagerRepository.GetBomName(proIds, bomsTypes, projectNames);

                List<SelectListItem> items1 = modelss.ProjectMasterModelsForBomName.Select(model => new SelectListItem { Text = model.BOMName, Value = model.BOMName.ToString(CultureInfo.InvariantCulture) }).ToList();
                json = JsonConvert.SerializeObject(items1);
                ViewBag.ProjectBomName = items1;
            }


            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public ActionResult AddFocClaimFromPm(int IsRemoved, string bomsType, string bomName, string BOMQuantity, string color, string remarks)
        {
            var model = new ProjectMasterModel();

            model.IsRemoved = IsRemoved;
            model.BOMType = bomsType;
            model.BOMName = bomName;
            model.ItemQuantity = BOMQuantity;
            model.Color = color;
            model.BomRemarks = remarks;
            return PartialView("~/Views/ProjectManager/Partial/_FocClaimFromPm.cshtml", model);
        }
        [HttpPost]
        public ActionResult RawMaterialInspection(List<ProjectMasterModel> issueList, List<ProjectMasterModel> issueList1, String focChk1)
        {
            var manager = new FileManager();

            //string[] values1 = Request.Form.GetValues("issueList1[]");
            // string values_joined = string.Join("; ", values1);

            //issueList = issueList.Where(x => x.IsRemoved == 0).ToList();
            //issueList1 = issueList1.Where(x => x.IsRemoved == 0).ToList();

            var Attachment = "";
            long pro_id = 0;
            foreach (var items in issueList)
            {
                if (items.FileId != null)
                {
                    var res = manager.Upload3(items.ProjectMasterId, Convert.ToInt64(items.ProjectPurchaseOrderFormId), items.IsRemoved,
                    "PmRawMaterial", "PmRawMaterialImage", items.FileId);

                    Console.Write("res  :" + res);

                    items.SupportingDocument = items.SupportingDocument == null ? res : items.SupportingDocument + "|" + res;

                    Attachment = items.SupportingDocument;
                }

                pro_id = items.ProjectMasterId;
            }

            _projectManagerRepository.SaveRawMaterialInspection(issueList, issueList1, pro_id, focChk1, Attachment);

            return RedirectToAction("RawMaterialInspection", new { ProjectMasterId = pro_id });

        }
        [HttpGet]
        public ActionResult RawMaterialInspectionList()
        {
            long userId = Convert.ToInt64(User.Identity.Name);

            ViewBag.RawMaterialInspectionListData = _projectManagerRepository.GetRawMaterialInspectionList();

            var fileManager = new FileManager();
            if (ViewBag.RawMaterialInspectionListData != null)
            {
                foreach (var model in ViewBag.RawMaterialInspectionListData)
                {
                    if (model.SupportingDocument != null)
                    {
                        var urls = model.SupportingDocument;

                        FilesDetail detail = new FilesDetail();
                        detail.FilePath = fileManager.GetFile(urls);
                        detail.Extention = fileManager.GetExtension(urls);
                        model.FilesDetails.Add(detail);
                    }
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult RawMaterialInspectionList(List<ProjectMasterModel> issueList1, String ProIds)
        {
            var manager = new FileManager();

            var Attachment = "";
            long pro_id = 0;
            long.TryParse(ProIds, out pro_id);

            var proDetails = _projectManagerRepository.GetRawDetails(pro_id);

            foreach (var items in issueList1)
            {
                if (items.FileId != null)
                {
                    var res = manager.Upload3(Convert.ToInt64(proDetails.ProjectMasterId), Convert.ToInt64(proDetails.ProjectPurchaseOrderFormId), 0,
                    "PmRawMaterial", "PmRawMaterialImage", items.FileId);

                    Console.Write("res  :" + res);

                    items.SupportingDocument = items.SupportingDocument == null ? res : items.SupportingDocument + "|" + res;

                    Attachment = items.SupportingDocument;
                }

                // pro_id = items.ProjectMasterId;
            }

            _projectManagerRepository.UpdateRawMaterialInspection(pro_id, Attachment);

            return RedirectToAction("RawMaterialInspectionList");

        }
        [HttpGet]
        public ActionResult FocClaimDetailsLotWise(long RawMaterialId = 0, long projectId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.Project = _commonRepository.GetProjectInfoByProjectId(projectId);
            var models = new AssignProjectsViewModel();

            models.ProjectMasterModelsForBomName = _projectManagerRepository.GetBomName(0, null, null);
            List<SelectListItem> itemsBomName = models.ProjectMasterModelsForBomName.Select(model => new SelectListItem { Text = model.BOMName, Value = model.BOMName.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.ProjectBomName = itemsBomName;

            ViewBag.GetBomDetails = _projectManagerRepository.GetBomDetails(RawMaterialId);
            ViewBag.RawMaterialId = RawMaterialId;
            return View();
        }

        public ActionResult QcDelayReportForPm(string ProjectName, string ProjectType, string StartDate, string EndDate, string EmployeeCode)
        {
            var vmSw = new VmAllIncentiveList();
            vmSw.ProjectMasterModels = _projectManagerRepository.GetProjectListForSwQcHead();

            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Select Project" } };

            var query = (from master in vmSw.ProjectMasterModels
                         select new
                         {
                             master.ProjectName

                         }).ToList();

            foreach (var t in query)
            {
                selectListItems.Add(new SelectListItem
                {
                    Value = t.ProjectName.Trim(),
                    Text = t.ProjectName.Trim()
                });
            }
            ViewBag.ProjectLists = selectListItems;
            //
            vmSw.CmnUserModels = _projectManagerRepository.GetPmUserList();
            var selectListItems1 = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Select Person" } };

            var query1 = (from master in vmSw.CmnUserModels
                          select new
                          {
                              master.UserFullName,
                              master.EmployeeCode

                          }).ToList();

            foreach (var t in query1)
            {
                selectListItems1.Add(new SelectListItem
                {
                    Value = t.EmployeeCode.Trim(),
                    Text = t.UserFullName.Trim()
                });
            }
            ViewBag.CmnUserModelsList = selectListItems1;
            //
            if (ProjectName != null && ProjectType != null && StartDate != null && EndDate != null)
            {
                ViewBag.GetQcDelayReport = _projectManagerRepository.GetQcDelayReport(ProjectName, ProjectType, StartDate, EndDate, EmployeeCode);
            }

            vmSw.ProjectName = ProjectName;
            vmSw.ProjectType = ProjectType;
            vmSw.StartDate = StartDate;
            vmSw.EndDate = EndDate;

            return View(vmSw);
        }

        [HttpPost]
        public JsonResult AddNewFoc(string rawMatIds, string bomsType, string bomName, string BOMQuantity, string color, string BomRemarks)
        {
            var modelss = new AssignProjectsViewModel();

            long rawIds;
            long.TryParse(rawMatIds, out rawIds);
            var json = "";
            if (rawIds != 0)
            {
                json = _projectManagerRepository.SaveNewFoc(rawIds, bomsType, bomName, BOMQuantity, color, BomRemarks);
            }
            return new JsonResult{ Data=json,JsonRequestBehavior = JsonRequestBehavior.AllowGet};
        }

        #region Finish Good

        public ActionResult FinishGoodVariant()
        {
            List<FinishGoodVariantModel> orderShipmentModels = _projectManagerRepository.GetShipmentDetailsForFinishGood();
            return View(orderShipmentModels);
        }
        public JsonResult GetFinishGoodDetails(string ProjectOrderShipmentId)
        {
            long proShipOrder;
            long.TryParse(ProjectOrderShipmentId, out proShipOrder);

            var finishGood = _projectManagerRepository.GetFinishGoodDetails(proShipOrder);

            return new JsonResult { Data = finishGood, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        #endregion

        public ActionResult InactiveAssignedProjectToQc()
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            //ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            //Get from Repository
            List<PmQcAssignModel> pmQcAssignModels = _projectManagerRepository.GetPmToQcHeadAssignModels(userId);
            List<CmnUserModel> list = _projectManagerRepository.GetActiveQc();

            //Create a ViewModel
            AssignMuliplePersonViewModel model = new AssignMuliplePersonViewModel();
            model.PmQcAssignModels = pmQcAssignModels;
            model.ddlAssignUsersList = list;


            List<PmQcAssignModel> pmQcAssignModels1 = _projectManagerRepository.GetQcHeadToQcAssignModels(userId);
            model.PmQcAssignModels1 = pmQcAssignModels1;


            return View(model);
        }

        public JsonResult UpdateInactiveAssignedProjectToQc(String ProjectMasterId, String SwQcInchargeAssignId)
        {
            long proId;
            long.TryParse(ProjectMasterId, out proId);

            long swQcHeadIds;
            long.TryParse(SwQcInchargeAssignId, out swQcHeadIds);

            var saveInactiveData = "0";

            if (proId != 0 && swQcHeadIds !=0)
            {
                saveInactiveData = _projectManagerRepository.UpdateInactiveAssignedProjectToQc(proId, swQcHeadIds); 
            }
            return Json(new { saveInactiveData }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateQcheadToQcAssignedProjectForInactive(String ProjectMasterId, String SwQcInchargeAssignId)
        {
            long proId;
            long.TryParse(ProjectMasterId, out proId);

            long swQcHeadIds;
            long.TryParse(SwQcInchargeAssignId, out swQcHeadIds);

            var saveInactiveData = "0";

            if (proId != 0 && swQcHeadIds != 0)
            {
                saveInactiveData = _projectManagerRepository.UpdateQcheadToQcAssignedProjectForInactive(proId, swQcHeadIds);
            }


            return Json(new { saveInactiveData }, JsonRequestBehavior.AllowGet);
        }

        #region China Qc Inspection Clearance
        public ActionResult ChinaQcInspectionClearance(string ProjectName, string ProjectMasterId, string Orders, string OrderQuantity)
        {
            var vmModel = new VmChinaQcInspectionsClearance();
            vmModel.ChinaQcInspectionsClearanceModels1 = _projectManagerRepository.GetProjectListForChinaQc();

            var items1 = new List<SelectListItem> { new SelectListItem { Value = "", Text = "SELECT PROJECT" } };
            items1 = vmModel.ChinaQcInspectionsClearanceModels1.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectName.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.Projects = items1;

            vmModel.ChinaQcInspectionsClearanceModels2 = _projectManagerRepository.GetProjectOrders(ProjectName);
            List<SelectListItem> items2 = vmModel.ChinaQcInspectionsClearanceModels2.Select(model => new SelectListItem { Text = model.Orders, Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.ProjectOrderLists = items2;


            vmModel.ChinaQcInspectionsClearanceModels3 = _projectManagerRepository.GetChinaInspectionDetails(ProjectMasterId);
            ViewBag.GetChinaInspectionDetails = vmModel.ChinaQcInspectionsClearanceModels3;

            vmModel.ChinaQcInspectionsClearanceModels4 = _projectManagerRepository.GetChinaInspectionProjectDetails(ProjectMasterId);
            ViewBag.GetChinaInspectionProjectDetails = vmModel.ChinaQcInspectionsClearanceModels4;

            var fileManager = new FileManager();

            if (vmModel.ChinaQcInspectionsClearanceModels4 != null)
            {
                foreach (var model in vmModel.ChinaQcInspectionsClearanceModels4)
                {
                    if (model.InspectionAttachment != null)
                    {
                        var urls = model.InspectionAttachment.Split('|').ToList();
                        for (int i = 0; i < urls.Count; i++)
                        {
                            FilesDetail detail = new FilesDetail();
                            detail.FilePath = fileManager.GetFile(urls[i]);
                            detail.Extention = fileManager.GetExtension(urls[i]);
                            model.FilesDetails.Add(detail);
                        }
                    }
                }
            }
            //Load Urls//
            vmModel.ProjectMasterId = Convert.ToInt64(ProjectMasterId);
            vmModel.ProjectName = ProjectName;
            vmModel.Orders = Orders;
            vmModel.OrderQuantity = Convert.ToInt64(OrderQuantity);

            return View(vmModel);
        }
        public JsonResult GetProjectOrders(string projectName)
        {
            var projectOrdersList = _projectManagerRepository.GetProjectOrders(projectName);
            List<SelectListItem> items = projectOrdersList.Select(model => new SelectListItem { Text = model.Orders, Value = model.ProjectMasterId.ToString(CultureInfo.InvariantCulture) }).ToList();
            var json = JsonConvert.SerializeObject(items);
           
            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        //GetProjectOrderQuantity
        public JsonResult GetProjectOrderQuantity(string projectMasterId)
        {
            var projectOrdersQty = _projectManagerRepository.GetProjectOrderQuantity(projectMasterId);
            var json = JsonConvert.SerializeObject(projectOrdersQty);

            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public ActionResult ChinaQcInspectionClearance(List<ChinaQcInspectionsClearanceModel> issueList)
        {
            var manager = new FileManager();
            var saveData = "";
            foreach (var swQc in issueList)
            {
                if (swQc.File.Count() > 0 && swQc.File != null)
                {
                    var res = manager.Upload1(issueList[0].ProjectMasterId, "ChinaQc",
                        "ChinaQcInspections", swQc.File);
                    swQc.InspectionAttachment = swQc.InspectionAttachment == null ? res : swQc.InspectionAttachment + "|" + res;
                }
            }
            saveData = _projectManagerRepository.SaveChinaQcInspectionClearanceDetails(issueList);
           // return RedirectToAction("ChinaQcInspectionClearance", new { ProjectName = issueList[0].ProjectName + ",ProjectMasterId=" + issueList[0].ProjectMasterId + ",Orders=" + issueList[0].Orders + ",OrderQuantity=" + issueList[0].OrderQuantity });
            return RedirectToAction("ChinaQcInspectionClearance");
        }

        public ActionResult ChinaQcInspectionClearanceApprovalDetails(string ids)
        {
            var vmModel = new VmChinaQcInspectionsClearance();
            vmModel.ChinaQcInspectionsClearanceModels1 = _projectManagerRepository.GetChinaApprovalLog(ids);
            vmModel.ChinaQcInspectionsClearanceModel2 = _projectManagerRepository.GetChinaApprovalStatus(ids);
            ViewBag.GetQcInspections = vmModel.ChinaQcInspectionsClearanceModel2;

            return View(vmModel);
        }

        [HttpPost]
        public ActionResult ChinaQcInspectionClearanceApprovalDetails(List<ChinaQcInspectionsClearanceModel> issueList)
        {
            var saveData = "";
            if (issueList[0].Id !=0)
            {
                saveData = _projectManagerRepository.SaveShipmentDeniedData(issueList[0].Id, issueList[0].ProjectMasterId, issueList[0].Remarks);
            }
            return RedirectToAction("ChinaQcInspectionClearanceApprovalDetails", new { ids = issueList[0].Id});
        }
        public JsonResult SaveChinaShipmentClearance(string ids)
        {
            var saveData = "";
            long proIds;
            long.TryParse(ids, out proIds);

            if (proIds>0)
            {
                saveData = _projectManagerRepository.SaveChinaShipmentClearance(proIds);
            }
            return new JsonResult { Data = saveData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion
    }

    public class ModelExcel
    {
        public string ModelName { get; set; }
        public DataTable ImeiData { get; set; }
    }
}
