using System;
//using System.Reflection;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Infrastructures.Repositories;
using ProjectManagement.Models;
using ProjectManagement.Models.AssignModels;
using ProjectManagement.ViewModels.ProjectManager;
using ProjectManagement.ViewModels.Software;
using Excel = Microsoft.Office.Interop.Excel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
//using OfficeOpenXml;
//using OfficeOpenXml.Style;

namespace ProjectManagement.Controllers
{
    [Authorize(Roles = "QCHEAD,QC,SA,ACCNT,PM,PMHEAD,ASPM,ASPMHEAD")]
    public class SoftwareController : Controller
    {
        private ISoftwareRepository _repository;
        private readonly ICommercialRepository _commercialRepository;
        private readonly IProjectManagerRepository _projectManagerRepository;
        private readonly ICommonRepository _commonRepository;
        static String _connectionStringCellphone = ConfigurationManager.ConnectionStrings["CellPhoneForExcel"].ConnectionString;
        public SoftwareController(SoftwareRepository repository, CommercialRepository commercialRepository, ProjectManagerRepository projectManagerRepository, CommonRepository commonRepository)
        {
            _repository = repository;
            _commercialRepository = commercialRepository;
            _projectManagerRepository = projectManagerRepository;
            _commonRepository = commonRepository;
            String useridentity = System.Web.HttpContext.Current.User.Identity.Name;
            var users = Convert.ToInt64(useridentity == "" ? "0" : useridentity);
            ViewBag.ChinaQcInspectionCount = _commonRepository.GetChinaQcInspectionCount(users);
        }
        // GET: Software
        public ActionResult Index(string Sorting_Order)
        {
            using (var db = new CellPhoneProjectEntities())
            {
                return View(db.SwQcBatteryAssignIssues.ToList());
            }
        }

        #region Incharhe Dashboard
        [Authorize(Roles = "QCHEAD,SA")]

        public ActionResult SwQcInchargeDashboard(VmSwQcSpecificationModified model)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.GetNewProjectStatus = _repository.GetNewProjectStatusForInchargeDashboard();
            ViewBag.GetAssignedProjectToQCStatus = _repository.GetAssignedProjectToQCStatusForInchargeDashboard();
            ViewBag.GetQCCompletedProjectStatus = _repository.GetQcCompletedProjectStatusForInchargeDashboard();
            ViewBag.GetQcAssignedProjectStatus = _repository.GetAssignedProjectToQCStatusForQcDashboard(userId);

            model.SwQcTestCounterModel = _repository.GetSwQcTestCountsForQcIncharge(userId);
            return View(model);
        }

        #endregion

        #region QC Dashboard
        [Authorize(Roles = "QC,SA")]
        public ActionResult SwQcDashboard(VmSwQcSpecificationModified model)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            ViewBag.GetQcAssignedProjectStatus = _repository.GetAssignedProjectToQCStatusForQcDashboard(userId);
            ViewBag.GetQcCompletedProjectStatus = _repository.GetQcCompletedProjectStatusForQcDashboard(userId);

            model.SwQcTestCounterModel = _repository.GetSwQcTestCountsForQc(userId);
            return View(model);
        }
        public ActionResult QcRecommendedProjectDetails(string StartDate, string EndDate)
        {
            var model = new SwQcIssueDetailModel();
            if (StartDate != null && EndDate != null)
            {
                ViewBag.QcRecommendedProjectDetails1 = _repository.QcRecommendedProjectDetails1(StartDate, EndDate);

            }
            model.StartDate = StartDate;
            model.EndDate = EndDate;

            return View(model);
        }
        #endregion

        #region Qc Issue List
        [Authorize(Roles = "QCHEAD,QC,SA")]
        public ActionResult SwQcSpecification(long projectId = 0, long AssignId = 0, string tabName = "", string projectType = "")
        {

            FileManager fileManager = new FileManager();

            var vmSwQcSpecification = new VmSwQcSpecificationModified();

            long userId = Convert.ToInt64(User.Identity.Name);

            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);


            vmSwQcSpecification.SwQcAssignModels = _repository.GetProjectListForQcInchargeToQcAssign(userId);
            if (projectId > 0)
            {
                vmSwQcSpecification.ProjectMasterModel = _repository.GetProjectMasterModel(projectId);
                vmSwQcSpecification.AssignId = _repository.GetAssignId(projectId, userId);
                vmSwQcSpecification.SwQcTabColorModels = _repository.GetAllTabColorAccordingToProject(projectId);
            }
            if (tabName == "Start Up")
            {
                vmSwQcSpecification.SwQcStartUpModels = _repository.GetStartUps(projectId, AssignId, "Start Up", projectType);
                if (vmSwQcSpecification.SwQcStartUpModels.Any())
                {

                    foreach (SwQcStartUpModel model in vmSwQcSpecification.SwQcStartUpModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                //old one// model.ScreenShotGetUrl1.Add(Path.GetFileName(urls[i]));                          
                                var last = urls[i].Split('-').Last();
                                model.ScreenShotGetUrl1.Add(Path.GetFileName(last));
                            }
                        }

                    }
                }
                vmSwQcSpecification.IsEdit = vmSwQcSpecification.SwQcStartUpModels.Any() && vmSwQcSpecification.SwQcStartUpModels[0].SwQcStartUpId > 0;
            }
            else if (tabName == "Call setting")
            {
                vmSwQcSpecification.SwQcCallSettingModels = _repository.GetCallSettings(projectId, AssignId, "Call setting", projectType);
                if (vmSwQcSpecification.SwQcCallSettingModels.Any())
                {
                    foreach (SwQcCallSettingModel model in vmSwQcSpecification.SwQcCallSettingModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                //old one// model.ScreenShotGetUrl1.Add(Path.GetFileName(urls[i]));                          
                                var last = urls[i].Split('-').Last();
                                model.ScreenShotGetUrl1.Add(Path.GetFileName(last));
                            }
                        }
                    }
                }
                vmSwQcSpecification.IsEdit = vmSwQcSpecification.SwQcCallSettingModels.Any() && vmSwQcSpecification.SwQcCallSettingModels[0].SwQcCallSettingId > 0;
            }
            else if (tabName == "Message")
            {
                vmSwQcSpecification.SwQcMessageModels = _repository.GetMessages(projectId, AssignId, "Message", projectType);
                if (vmSwQcSpecification.SwQcMessageModels.Any())
                {
                    foreach (SwQcMessageModel model in vmSwQcSpecification.SwQcMessageModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                //old one// model.ScreenShotGetUrl1.Add(Path.GetFileName(urls[i]));                          
                                var last = urls[i].Split('-').Last();
                                model.ScreenShotGetUrl1.Add(Path.GetFileName(last));
                            }
                        }
                    }
                }
                vmSwQcSpecification.IsEdit = vmSwQcSpecification.SwQcMessageModels.Any() && vmSwQcSpecification.SwQcMessageModels[0].SwQcMassageId > 0;
            }
            else if (tabName == "Tools Check")
            {
                vmSwQcSpecification.SwQcToolsCheckModels = _repository.GetTools(projectId, AssignId, "Tools Check", projectType);
                if (vmSwQcSpecification.SwQcToolsCheckModels.Any())
                {
                    foreach (SwQcToolsCheckModel model in vmSwQcSpecification.SwQcToolsCheckModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                var last = urls[i].Split('-').Last();
                                model.ScreenShotGetUrl1.Add(Path.GetFileName(last));
                            }
                        }
                    }
                }
                vmSwQcSpecification.IsEdit = vmSwQcSpecification.SwQcToolsCheckModels.Any() && vmSwQcSpecification.SwQcToolsCheckModels[0].SwQcToolsCheckId > 0;
            }
            else if (tabName == "Camera")
            {
                vmSwQcSpecification.SwQcCameraModels = _repository.GetCamera(projectId, AssignId, "Camera", projectType);
                if (vmSwQcSpecification.SwQcCameraModels.Any())
                {
                    foreach (SwQcCameraModel model in vmSwQcSpecification.SwQcCameraModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                var last = urls[i].Split('-').Last();
                                model.ScreenShotGetUrl1.Add(Path.GetFileName(last));
                            }
                        }
                    }
                }
                vmSwQcSpecification.IsEdit = vmSwQcSpecification.SwQcCameraModels.Any() && vmSwQcSpecification.SwQcCameraModels[0].SwQcCameraId > 0;
            }

            else if (tabName == "Display Loop")
            {
                vmSwQcSpecification.SwQcDisplayLoopModels = _repository.GetDisplayLoop(projectId, AssignId, "Display Loop", projectType);
                if (vmSwQcSpecification.SwQcDisplayLoopModels.Any())
                {
                    foreach (SwQcDisplayLoopModel model in vmSwQcSpecification.SwQcDisplayLoopModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                var last = urls[i].Split('-').Last();
                                model.ScreenShotGetUrl1.Add(Path.GetFileName(last));
                            }
                        }
                    }
                }
                vmSwQcSpecification.IsEdit = vmSwQcSpecification.SwQcDisplayLoopModels.Any() && vmSwQcSpecification.SwQcDisplayLoopModels[0].SwQcDisplayLoopId > 0;
            }
            else if (tabName == "Display")
            {
                vmSwQcSpecification.SwQcDisplayModels = _repository.GetDisplay(projectId, AssignId, "Display", projectType);
                if (vmSwQcSpecification.SwQcDisplayModels.Any())
                {
                    foreach (SwQcDisplayModel model in vmSwQcSpecification.SwQcDisplayModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                var last = urls[i].Split('-').Last();
                                model.ScreenShotGetUrl1.Add(Path.GetFileName(last));
                            }
                        }
                    }
                }
                vmSwQcSpecification.IsEdit = vmSwQcSpecification.SwQcDisplayModels.Any() && vmSwQcSpecification.SwQcDisplayModels[0].SwQcDisplayId > 0;
            }
            else if (tabName == "Settings")
            {
                vmSwQcSpecification.SwQcSettingModels = _repository.GetSetting(projectId, AssignId, "Settings", projectType);
                if (vmSwQcSpecification.SwQcSettingModels.Any())
                {
                    foreach (SwQcSettingModel model in vmSwQcSpecification.SwQcSettingModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                var last = urls[i].Split('-').Last();
                                model.ScreenShotGetUrl1.Add(Path.GetFileName(last));
                            }
                        }
                    }
                }
                vmSwQcSpecification.IsEdit = vmSwQcSpecification.SwQcSettingModels.Any() && vmSwQcSpecification.SwQcSettingModels[0].SwQcSettingsId > 0;
            }
            else if (tabName == "Multimedia")
            {
                vmSwQcSpecification.SwQcMultimediaModels = _repository.GetMultimedia(projectId, AssignId, "Multimedia", projectType);
                if (vmSwQcSpecification.SwQcMultimediaModels.Any())
                {
                    foreach (SwQcMultimediaModel model in vmSwQcSpecification.SwQcMultimediaModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                var last = urls[i].Split('-').Last();
                                model.ScreenShotGetUrl1.Add(Path.GetFileName(last));
                            }
                        }
                    }
                }
                vmSwQcSpecification.IsEdit = vmSwQcSpecification.SwQcMultimediaModels.Any() && vmSwQcSpecification.SwQcMultimediaModels[0].SwQcMultimediaId > 0;
            }
            else if (tabName == "Google Services")
            {
                vmSwQcSpecification.SwQcGoogleServiceModels = _repository.GetGoogleService(projectId, AssignId, "Google Services", projectType);
                if (vmSwQcSpecification.SwQcGoogleServiceModels.Any())
                {
                    foreach (SwQcGoogleServiceModel model in vmSwQcSpecification.SwQcGoogleServiceModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                var last = urls[i].Split('-').Last();
                                model.ScreenShotGetUrl1.Add(Path.GetFileName(last));
                            }
                        }
                    }
                }
                vmSwQcSpecification.IsEdit = vmSwQcSpecification.SwQcGoogleServiceModels.Any() && vmSwQcSpecification.SwQcGoogleServiceModels[0].SwQcGoogleServicesId > 0;
            }
            else if (tabName == "Storage check")
            {
                vmSwQcSpecification.SwQcStorageCheckModels = _repository.GetStorageCheck(projectId, AssignId, "Storage check", projectType);
                if (vmSwQcSpecification.SwQcStorageCheckModels.Any())
                {
                    foreach (SwQcStorageCheckModel model in vmSwQcSpecification.SwQcStorageCheckModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                var last = urls[i].Split('-').Last();
                                model.ScreenShotGetUrl1.Add(Path.GetFileName(last));
                            }
                        }
                    }
                }
                vmSwQcSpecification.IsEdit = vmSwQcSpecification.SwQcStorageCheckModels.Any() && vmSwQcSpecification.SwQcStorageCheckModels[0].SwQcStorageCheckId > 0;
            }
            else if (tabName == "Game")
            {
                vmSwQcSpecification.SwQcGameModels = _repository.GetGame(projectId, AssignId, "Game", projectType);
                if (vmSwQcSpecification.SwQcGameModels.Any())
                {
                    foreach (SwQcGameModel model in vmSwQcSpecification.SwQcGameModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                var last = urls[i].Split('-').Last();
                                model.ScreenShotGetUrl1.Add(Path.GetFileName(last));
                            }
                        }
                    }
                }
                vmSwQcSpecification.IsEdit = vmSwQcSpecification.SwQcGameModels.Any() && vmSwQcSpecification.SwQcGameModels[0].SwQcGameId > 0;
            }
            else if (tabName == "Testing App")
            {
                vmSwQcSpecification.SwQcTestingAppModels = _repository.GetTestingApp(projectId, AssignId, "Testing App", projectType);
                if (vmSwQcSpecification.SwQcTestingAppModels.Any())
                {
                    foreach (SwQcTestingAppModel model in vmSwQcSpecification.SwQcTestingAppModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                var last = urls[i].Split('-').Last();
                                model.ScreenShotGetUrl1.Add(Path.GetFileName(last));
                            }
                        }
                    }
                }
                vmSwQcSpecification.IsEdit = vmSwQcSpecification.SwQcTestingAppModels.Any() && vmSwQcSpecification.SwQcTestingAppModels[0].SwQcTestingAppId > 0;
            }
            else if (tabName == "File manager")
            {
                vmSwQcSpecification.SwQcFileManagerModels = _repository.GetFileManager(projectId, AssignId, "File manager", projectType);
                if (vmSwQcSpecification.SwQcFileManagerModels.Any())
                {
                    foreach (SwQcFileManagerModel model in vmSwQcSpecification.SwQcFileManagerModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                var last = urls[i].Split('-').Last();
                                model.ScreenShotGetUrl1.Add(Path.GetFileName(last));
                            }
                        }
                    }
                }
                vmSwQcSpecification.IsEdit = vmSwQcSpecification.SwQcFileManagerModels.Any() && vmSwQcSpecification.SwQcFileManagerModels[0].SwQcFileManagerId > 0;
            }
            else if (tabName == "Connectivity")
            {
                vmSwQcSpecification.SwQcConnectivityModels = _repository.GetConnectivity(projectId, AssignId, "Connectivity", projectType);
                if (vmSwQcSpecification.SwQcConnectivityModels.Any())
                {
                    foreach (SwQcConnectivityModel model in vmSwQcSpecification.SwQcConnectivityModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                var last = urls[i].Split('-').Last();
                                model.ScreenShotGetUrl1.Add(Path.GetFileName(last));
                            }
                        }
                    }
                }
                vmSwQcSpecification.IsEdit = vmSwQcSpecification.SwQcConnectivityModels.Any() && vmSwQcSpecification.SwQcConnectivityModels[0].SwQcConnectivityId > 0;
            }
            else if (tabName == "Shut down")
            {
                vmSwQcSpecification.SwQcShutDownModels = _repository.GetShutDown(projectId, AssignId, "Shut down", projectType);
                if (vmSwQcSpecification.SwQcShutDownModels.Any())
                {
                    foreach (SwQcShutDownModel model in vmSwQcSpecification.SwQcShutDownModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                var last = urls[i].Split('-').Last();
                                model.ScreenShotGetUrl1.Add(Path.GetFileName(last));
                            }
                        }
                    }
                }
                vmSwQcSpecification.IsEdit = vmSwQcSpecification.SwQcShutDownModels.Any() && vmSwQcSpecification.SwQcShutDownModels[0].SwQcShutDownId > 0;
            }
            return View(vmSwQcSpecification);
        }

        [HttpPost]
        public ActionResult SwQcSpecification(VmSwQcSpecificationModified swQcSpecification)
        {
            if (ModelState.IsValid)
            {
                long userId = Convert.ToInt64(User.Identity.Name);
                ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);

                var manager = new FileManager();

                if (!swQcSpecification.IsEdit)
                {
                    if (swQcSpecification.Tabname == "Start Up")
                    {
                        foreach (var model in swQcSpecification.SwQcStartUpModels)
                        {
                            model.SwQcUserId = userId;
                            model.Added = userId;
                            model.AddedDate = DateTime.Now;
                            //model.Updated = userId;
                            //model.UpdatedDate = DateTime.Now;
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;

                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;
                            HttpPostedFileBase userPostedFile = null;

                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);
                                // var ddd1 = Convert.ToInt64(uploadedFiles.AllKeys[i]);

                                //if (model.SwQcIssueId == Convert.ToInt64(ddd) &&
                                //    model.SwQcIssueId == Convert.ToInt64(ddd1))
                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "StartUp", model.File1);

                                        Console.Write("res  :" + res);

                                        model.UploadedFile = model.UploadedFile == null
                                            ? res
                                            : model.UploadedFile + "|" + res;
                                    }
                                }
                            }

                        }
                        bool isSaved = _repository.SaveSwQcStartUp(swQcSpecification.SwQcStartUpModels);
                    }
                    else if (swQcSpecification.Tabname == "Call setting")
                    {
                        foreach (var model in swQcSpecification.SwQcCallSettingModels)
                        {
                            model.SwQcUserId = userId;
                            model.Added = userId;
                            model.AddedDate = DateTime.Now;
                            //model.Updated = userId;
                            //model.UpdatedDate = DateTime.Now;
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;

                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;
                            HttpPostedFileBase userPostedFile = null;

                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "CallSetting", model.File1);

                                        Console.Write("res  :" + res);

                                        model.UploadedFile = model.UploadedFile == null
                                            ? res
                                            : model.UploadedFile + "|" + res;
                                    }
                                }
                            }

                        }
                        bool isSaved = _repository.SaveSwQcCallSetting(swQcSpecification.SwQcCallSettingModels);
                    }
                    else if (swQcSpecification.Tabname == "Message")
                    {
                        foreach (var model in swQcSpecification.SwQcMessageModels)
                        {

                            model.Added = userId;
                            model.AddedDate = DateTime.Now;
                            //model.Updated = userId;
                            //model.UpdatedDate = DateTime.Now;
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;

                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;
                            HttpPostedFileBase userPostedFile = null;

                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "Message", model.File1);

                                        Console.Write("res  :" + res);

                                        model.UploadedFile = model.UploadedFile == null
                                            ? res
                                            : model.UploadedFile + "|" + res;
                                    }
                                }
                            }
                        }
                        bool isSaved = _repository.SaveSwQcMessage(swQcSpecification.SwQcMessageModels);
                    }
                    else if (swQcSpecification.Tabname == "Tools Check")
                    {
                        foreach (var model in swQcSpecification.SwQcToolsCheckModels)
                        {

                            model.Added = userId;
                            model.AddedDate = DateTime.Now;
                            //model.Updated = userId;
                            //model.UpdatedDate = DateTime.Now;
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;
                            HttpPostedFileBase userPostedFile = null;

                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "ToolsCheck", model.File1);

                                        Console.Write("res  :" + res);

                                        model.UploadedFile = model.UploadedFile == null
                                            ? res
                                            : model.UploadedFile + "|" + res;
                                    }
                                }
                            }
                        }
                        bool isSaved = _repository.SaveSwQcTools(swQcSpecification.SwQcToolsCheckModels);
                    }
                    else if (swQcSpecification.Tabname == "Camera")
                    {
                        foreach (SwQcCameraModel model in swQcSpecification.SwQcCameraModels)
                        {
                            model.Added = userId;
                            model.AddedDate = DateTime.Now;
                            //model.Updated = userId;
                            //model.UpdatedDate = DateTime.Now;
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;
                            HttpPostedFileBase userPostedFile = null;

                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "Camera", model.File1);

                                        Console.Write("res  :" + res);

                                        model.UploadedFile = model.UploadedFile == null
                                            ? res
                                            : model.UploadedFile + "|" + res;
                                    }
                                }
                            }
                        }
                        bool isSave = _repository.SaveSwQcCamera(swQcSpecification.SwQcCameraModels);
                    }

                    else if (swQcSpecification.Tabname == "Display Loop")
                    {
                        foreach (SwQcDisplayLoopModel model in swQcSpecification.SwQcDisplayLoopModels)
                        {
                            model.Added = userId;
                            model.AddedDate = DateTime.Now;
                            //model.Updated = userId;
                            //model.UpdatedDate = DateTime.Now;
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;
                            HttpPostedFileBase userPostedFile = null;

                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "DisplayLoop", model.File1);

                                        Console.Write("res  :" + res);

                                        model.UploadedFile = model.UploadedFile == null
                                            ? res
                                            : model.UploadedFile + "|" + res;
                                    }
                                }
                            }
                        }
                        bool isSaved = _repository.SaveSwQcDisplayLoop(swQcSpecification.SwQcDisplayLoopModels);
                    }
                    else if (swQcSpecification.Tabname == "Display")
                    {
                        foreach (SwQcDisplayModel model in swQcSpecification.SwQcDisplayModels)
                        {
                            model.Added = userId;
                            model.AddedDate = DateTime.Now;
                            //model.Updated = userId;
                            //model.UpdatedDate = DateTime.Now;
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;
                            HttpPostedFileBase userPostedFile = null;

                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "Display", model.File1);

                                        Console.Write("res  :" + res);

                                        model.UploadedFile = model.UploadedFile == null
                                            ? res
                                            : model.UploadedFile + "|" + res;
                                    }
                                }
                            }
                        }
                        bool isSaved = _repository.SaveSwQcDisplay(swQcSpecification.SwQcDisplayModels);
                    }
                    else if (swQcSpecification.Tabname == "Settings")
                    {
                        foreach (var model in swQcSpecification.SwQcSettingModels)
                        {
                            model.Added = userId;
                            model.AddedDate = DateTime.Now;
                            //model.Updated = userId;
                            //model.UpdatedDate = DateTime.Now;
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;
                            HttpPostedFileBase userPostedFile = null;

                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "Settings", model.File1);

                                        Console.Write("res  :" + res);

                                        model.UploadedFile = model.UploadedFile == null
                                            ? res
                                            : model.UploadedFile + "|" + res;
                                    }
                                }
                            }
                        }
                        bool isSaved = _repository.SaveSwQcSetting(swQcSpecification.SwQcSettingModels);
                    }
                    else if (swQcSpecification.Tabname == "Multimedia")
                    {
                        foreach (var model in swQcSpecification.SwQcMultimediaModels)
                        {
                            model.Added = userId;
                            model.AddedDate = DateTime.Now;
                            //model.Updated = userId;
                            //model.UpdatedDate = DateTime.Now;
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;
                            HttpPostedFileBase userPostedFile = null;

                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "Multimedia", model.File1);

                                        Console.Write("res  :" + res);

                                        model.UploadedFile = model.UploadedFile == null
                                            ? res
                                            : model.UploadedFile + "|" + res;
                                    }
                                }
                            }
                        }
                        bool isSaved = _repository.SaveSwQcMultimedia(swQcSpecification.SwQcMultimediaModels);
                    }
                    else if (swQcSpecification.Tabname == "Google Services")
                    {
                        foreach (var model in swQcSpecification.SwQcGoogleServiceModels)
                        {
                            model.Added = userId;
                            model.AddedDate = DateTime.Now;
                            //model.Updated = userId;
                            //model.UpdatedDate = DateTime.Now;
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;
                            HttpPostedFileBase userPostedFile = null;

                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "GoogleServices", model.File1);

                                        Console.Write("res  :" + res);

                                        model.UploadedFile = model.UploadedFile == null
                                            ? res
                                            : model.UploadedFile + "|" + res;
                                    }
                                }
                            }
                        }
                        bool isSaved = _repository.SaveSwQcGoogleService(swQcSpecification.SwQcGoogleServiceModels);
                    }
                    else if (swQcSpecification.Tabname == "Storage check")
                    {
                        foreach (var model in swQcSpecification.SwQcStorageCheckModels)
                        {
                            model.Added = userId;
                            model.AddedDate = DateTime.Now;
                            //model.Updated = userId;
                            //model.UpdatedDate = DateTime.Now;
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;
                            HttpPostedFileBase userPostedFile = null;

                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "StorageCheck", model.File1);

                                        Console.Write("res  :" + res);

                                        model.UploadedFile = model.UploadedFile == null
                                            ? res
                                            : model.UploadedFile + "|" + res;
                                    }
                                }
                            }
                        }
                        bool isSaved = _repository.SaveSwQcStorageCheck(swQcSpecification.SwQcStorageCheckModels);
                    }
                    else if (swQcSpecification.Tabname == "Game")
                    {
                        foreach (var model in swQcSpecification.SwQcGameModels)
                        {
                            model.Added = userId;
                            model.AddedDate = DateTime.Now;
                            //model.Updated = userId;
                            //model.UpdatedDate = DateTime.Now;
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;
                            HttpPostedFileBase userPostedFile = null;

                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "Game", model.File1);

                                        Console.Write("res  :" + res);

                                        model.UploadedFile = model.UploadedFile == null
                                            ? res
                                            : model.UploadedFile + "|" + res;
                                    }
                                }
                            }
                        }
                        bool isSaved = _repository.SaveSwQcGame(swQcSpecification.SwQcGameModels);
                    }
                    else if (swQcSpecification.Tabname == "Testing App")
                    {
                        foreach (var model in swQcSpecification.SwQcTestingAppModels)
                        {
                            model.Added = userId;
                            model.AddedDate = DateTime.Now;
                            //model.Updated = userId;
                            //model.UpdatedDate = DateTime.Now;
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;
                            HttpPostedFileBase userPostedFile = null;

                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "TestingApp", model.File1);

                                        Console.Write("res  :" + res);

                                        model.UploadedFile = model.UploadedFile == null
                                            ? res
                                            : model.UploadedFile + "|" + res;
                                    }
                                }
                            }
                        }
                        bool isSaved = _repository.SaveSwQcTestingApp(swQcSpecification.SwQcTestingAppModels);
                    }
                    else if (swQcSpecification.Tabname == "File manager")
                    {
                        foreach (var model in swQcSpecification.SwQcFileManagerModels)
                        {
                            model.Added = userId;
                            model.AddedDate = DateTime.Now;
                            //model.Updated = userId;
                            //model.UpdatedDate = DateTime.Now;
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;
                            HttpPostedFileBase userPostedFile = null;

                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "FileManager", model.File1);

                                        Console.Write("res  :" + res);

                                        model.UploadedFile = model.UploadedFile == null
                                            ? res
                                            : model.UploadedFile + "|" + res;
                                    }
                                }
                            }
                        }
                        bool isSaved = _repository.SaveSwQcFileManager(swQcSpecification.SwQcFileManagerModels);
                    }
                    else if (swQcSpecification.Tabname == "Connectivity")
                    {
                        foreach (var model in swQcSpecification.SwQcConnectivityModels)
                        {
                            model.Added = userId;
                            model.AddedDate = DateTime.Now;
                            //model.Updated = userId;
                            //model.UpdatedDate = DateTime.Now;
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;
                            HttpPostedFileBase userPostedFile = null;

                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "Connectivity", model.File1);

                                        Console.Write("res  :" + res);

                                        model.UploadedFile = model.UploadedFile == null
                                            ? res
                                            : model.UploadedFile + "|" + res;
                                    }
                                }
                            }
                        }
                        bool isSaved = _repository.SaveSwQcConnectivity(swQcSpecification.SwQcConnectivityModels);
                    }
                    else if (swQcSpecification.Tabname == "Shut down")
                    {
                        foreach (var model in swQcSpecification.SwQcShutDownModels)
                        {
                            model.Added = userId;
                            model.AddedDate = DateTime.Now;
                            //model.Updated = userId;
                            //model.UpdatedDate = DateTime.Now;
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;
                            HttpPostedFileBase userPostedFile = null;

                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "Shutdown", model.File1);

                                        Console.Write("res  :" + res);

                                        model.UploadedFile = model.UploadedFile == null
                                            ? res
                                            : model.UploadedFile + "|" + res;
                                    }
                                }
                            }
                        }
                        bool isSaved = _repository.SaveSwQcShutDown(swQcSpecification.SwQcShutDownModels);
                    }
                    else if (swQcSpecification.Tabname == "")
                    {
                        //something esle will happen here
                    }

                }
                else
                {

                    if (swQcSpecification.Tabname == "Start Up")
                    {
                        foreach (var model in swQcSpecification.SwQcStartUpModels)
                        {
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            model.Updated = userId;
                            model.UpdatedDate = DateTime.Now;

                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;
                            //  IEnumerable<int> ints = uploadedFiles1.Select(int.Parse);
                            HttpPostedFileBase userPostedFile = null;
                            var urls = string.Empty;
                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);
                                //  var ddd1 = Convert.ToInt64(uploadedFiles.AllKeys[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "StartUp", model.File1);

                                        Console.Write("res  :" + res);
                                        urls = urls + res + "|";
                                        //model.UploadedFile =model.UploadedFile + res + "|";
                                        // model.UploadedFile = model.UploadedFile == null ? res : model.UploadedFile + "|" + res;

                                        //model.UploadedFile = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                        //    "SwQcIssues", "StartUp", model.File1);

                                        //model.UploadedFile = model.UploadedFile == null
                                        //  ? res
                                        //  : model.UploadedFile + "|" + res;
                                    }
                                }
                            }
                            model.UploadedFile = !string.IsNullOrWhiteSpace(urls) ? urls.TrimEnd('|') : model.UploadedFile;
                        }
                        bool isSaved = _repository.UpdateSwQcStartUp(swQcSpecification.SwQcStartUpModels);
                    }
                    else if (swQcSpecification.Tabname == "Call setting")
                    {
                        foreach (var model in swQcSpecification.SwQcCallSettingModels)
                        {
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            model.Updated = userId;
                            model.UpdatedDate = DateTime.Now;

                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;

                            HttpPostedFileBase userPostedFile = null;
                            var urls = string.Empty;
                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "CallSetting", model.File1);

                                        Console.Write("res  :" + res);
                                        urls = urls + res + "|";

                                    }
                                }
                            }
                            model.UploadedFile = !string.IsNullOrWhiteSpace(urls) ? urls.TrimEnd('|') : model.UploadedFile;
                        }
                        bool isSaved = _repository.UpdateSwQcCallSetting(swQcSpecification.SwQcCallSettingModels);
                    }
                    else if (swQcSpecification.Tabname == "Message")
                    {
                        foreach (var model in swQcSpecification.SwQcMessageModels)
                        {
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            model.Updated = userId;
                            model.UpdatedDate = DateTime.Now;

                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;

                            HttpPostedFileBase userPostedFile = null;
                            var urls = string.Empty;
                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "Message", model.File1);

                                        Console.Write("res  :" + res);
                                        urls = urls + res + "|";

                                    }
                                }
                            }
                            model.UploadedFile = !string.IsNullOrWhiteSpace(urls) ? urls.TrimEnd('|') : model.UploadedFile;
                        }
                        bool isSaved = _repository.UpdateSwQcMessage(swQcSpecification.SwQcMessageModels);
                    }
                    else if (swQcSpecification.Tabname == "Tools Check")
                    {
                        foreach (var model in swQcSpecification.SwQcToolsCheckModels)
                        {
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            model.Updated = userId;
                            model.UpdatedDate = DateTime.Now;

                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;

                            HttpPostedFileBase userPostedFile = null;
                            var urls = string.Empty;
                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "ToolsCheck", model.File1);

                                        Console.Write("res  :" + res);
                                        urls = urls + res + "|";

                                    }
                                }
                            }
                            model.UploadedFile = !string.IsNullOrWhiteSpace(urls) ? urls.TrimEnd('|') : model.UploadedFile;
                        }
                        bool isSaved = _repository.UpdateSwQcTools(swQcSpecification.SwQcToolsCheckModels);
                    }
                    else if (swQcSpecification.Tabname == "Camera")
                    {
                        foreach (var model in swQcSpecification.SwQcCameraModels)
                        {
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            model.Updated = userId;
                            model.UpdatedDate = DateTime.Now;

                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;

                            HttpPostedFileBase userPostedFile = null;
                            var urls = string.Empty;
                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "Camera", model.File1);

                                        Console.Write("res  :" + res);
                                        urls = urls + res + "|";

                                    }
                                }
                            }
                            model.UploadedFile = !string.IsNullOrWhiteSpace(urls) ? urls.TrimEnd('|') : model.UploadedFile;
                        }
                        bool isSaved = _repository.UpdateSwQcCamera(swQcSpecification.SwQcCameraModels);
                    }
                    else if (swQcSpecification.Tabname == "Display Loop")
                    {
                        foreach (var model in swQcSpecification.SwQcDisplayLoopModels)
                        {
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            model.Updated = userId;
                            model.UpdatedDate = DateTime.Now;

                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;

                            HttpPostedFileBase userPostedFile = null;
                            var urls = string.Empty;
                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "DisplayLoop", model.File1);

                                        Console.Write("res  :" + res);
                                        urls = urls + res + "|";

                                    }
                                }
                            }
                            model.UploadedFile = !string.IsNullOrWhiteSpace(urls) ? urls.TrimEnd('|') : model.UploadedFile;
                        }
                        bool isSaved = _repository.UpdateSwQcDisplayLoop(swQcSpecification.SwQcDisplayLoopModels);
                    }
                    else if (swQcSpecification.Tabname == "Display")
                    {
                        foreach (var model in swQcSpecification.SwQcDisplayModels)
                        {
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            model.Updated = userId;
                            model.UpdatedDate = DateTime.Now;

                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;

                            HttpPostedFileBase userPostedFile = null;
                            var urls = string.Empty;
                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "Display", model.File1);

                                        Console.Write("res  :" + res);
                                        urls = urls + res + "|";

                                    }
                                }
                            }
                            model.UploadedFile = !string.IsNullOrWhiteSpace(urls) ? urls.TrimEnd('|') : model.UploadedFile;
                        }
                        bool isSaved = _repository.UpdateSwQcDisplay(swQcSpecification.SwQcDisplayModels);
                    }
                    else if (swQcSpecification.Tabname == "Settings")
                    {
                        foreach (var model in swQcSpecification.SwQcSettingModels)
                        {
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            model.Updated = userId;
                            model.UpdatedDate = DateTime.Now;

                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;

                            HttpPostedFileBase userPostedFile = null;
                            var urls = string.Empty;
                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "Settings", model.File1);

                                        Console.Write("res  :" + res);
                                        urls = urls + res + "|";

                                    }
                                }
                            }
                            model.UploadedFile = !string.IsNullOrWhiteSpace(urls) ? urls.TrimEnd('|') : model.UploadedFile;
                        }
                        bool isSaved = _repository.UpdateSwQcSetting(swQcSpecification.SwQcSettingModels);
                    }
                    else if (swQcSpecification.Tabname == "Multimedia")
                    {
                        foreach (var model in swQcSpecification.SwQcMultimediaModels)
                        {
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            model.Updated = userId;
                            model.UpdatedDate = DateTime.Now;

                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;

                            HttpPostedFileBase userPostedFile = null;
                            var urls = string.Empty;
                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "Multimedia", model.File1);

                                        Console.Write("res  :" + res);
                                        urls = urls + res + "|";

                                    }
                                }
                            }
                            model.UploadedFile = !string.IsNullOrWhiteSpace(urls) ? urls.TrimEnd('|') : model.UploadedFile;
                        }
                        bool isSaved = _repository.UpdateSwQcMultimedia(swQcSpecification.SwQcMultimediaModels);
                    }
                    else if (swQcSpecification.Tabname == "Google Services")
                    {
                        foreach (var model in swQcSpecification.SwQcGoogleServiceModels)
                        {
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            model.Updated = userId;
                            model.UpdatedDate = DateTime.Now;

                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;

                            HttpPostedFileBase userPostedFile = null;
                            var urls = string.Empty;
                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "GoogleServices", model.File1);

                                        Console.Write("res  :" + res);
                                        urls = urls + res + "|";

                                    }
                                }
                            }
                            model.UploadedFile = !string.IsNullOrWhiteSpace(urls) ? urls.TrimEnd('|') : model.UploadedFile;
                        }
                        bool isSaved = _repository.UpdateSwQcGoogleService(swQcSpecification.SwQcGoogleServiceModels);
                    }
                    else if (swQcSpecification.Tabname == "Storage check")
                    {
                        foreach (var model in swQcSpecification.SwQcStorageCheckModels)
                        {
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            model.Updated = userId;
                            model.UpdatedDate = DateTime.Now;

                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;

                            HttpPostedFileBase userPostedFile = null;
                            var urls = string.Empty;
                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "StorageCheck", model.File1);

                                        Console.Write("res  :" + res);
                                        urls = urls + res + "|";

                                    }
                                }
                            }
                            model.UploadedFile = !string.IsNullOrWhiteSpace(urls) ? urls.TrimEnd('|') : model.UploadedFile;
                        }
                        bool isSaved = _repository.UpdateSwQcStorageCheck(swQcSpecification.SwQcStorageCheckModels);
                    }
                    else if (swQcSpecification.Tabname == "Game")
                    {
                        foreach (var model in swQcSpecification.SwQcGameModels)
                        {

                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            model.Updated = userId;
                            model.UpdatedDate = DateTime.Now;

                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;

                            HttpPostedFileBase userPostedFile = null;
                            var urls = string.Empty;
                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "Game", model.File1);

                                        Console.Write("res  :" + res);
                                        urls = urls + res + "|";

                                    }
                                }
                            }
                            model.UploadedFile = !string.IsNullOrWhiteSpace(urls) ? urls.TrimEnd('|') : model.UploadedFile;
                        }
                        bool isSaved = _repository.UpdateSwQcGame(swQcSpecification.SwQcGameModels);
                    }
                    else if (swQcSpecification.Tabname == "Testing App")
                    {
                        foreach (var model in swQcSpecification.SwQcTestingAppModels)
                        {
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            model.Updated = userId;
                            model.UpdatedDate = DateTime.Now;

                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;

                            HttpPostedFileBase userPostedFile = null;
                            var urls = string.Empty;
                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "TestingApp", model.File1);

                                        Console.Write("res  :" + res);
                                        urls = urls + res + "|";

                                    }
                                }
                            }
                            model.UploadedFile = !string.IsNullOrWhiteSpace(urls) ? urls.TrimEnd('|') : model.UploadedFile;
                        }
                        bool isSaved = _repository.UpdateSwQcTestingApp(swQcSpecification.SwQcTestingAppModels);
                    }
                    else if (swQcSpecification.Tabname == "File manager")
                    {
                        foreach (var model in swQcSpecification.SwQcFileManagerModels)
                        {
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            model.Updated = userId;
                            model.UpdatedDate = DateTime.Now;

                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;

                            HttpPostedFileBase userPostedFile = null;
                            var urls = string.Empty;
                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "FileManager", model.File1);

                                        Console.Write("res  :" + res);
                                        urls = urls + res + "|";

                                    }
                                }
                            }
                            model.UploadedFile = !string.IsNullOrWhiteSpace(urls) ? urls.TrimEnd('|') : model.UploadedFile;
                        }
                        bool isSaved = _repository.UpdateSwQcFileManager(swQcSpecification.SwQcFileManagerModels);
                    }
                    else if (swQcSpecification.Tabname == "Connectivity")
                    {
                        foreach (var model in swQcSpecification.SwQcConnectivityModels)
                        {
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            model.Updated = userId;
                            model.UpdatedDate = DateTime.Now;

                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;

                            HttpPostedFileBase userPostedFile = null;
                            var urls = string.Empty;
                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "Connectivity", model.File1);

                                        Console.Write("res  :" + res);
                                        urls = urls + res + "|";

                                    }
                                }
                            }
                            model.UploadedFile = !string.IsNullOrWhiteSpace(urls) ? urls.TrimEnd('|') : model.UploadedFile;
                        }
                        bool isSaved = _repository.UpdateSwQcConnectivity(swQcSpecification.SwQcConnectivityModels);
                    }
                    else if (swQcSpecification.Tabname == "Shut down")
                    {
                        foreach (var model in swQcSpecification.SwQcShutDownModels)
                        {
                            model.ProjectMasterId = swQcSpecification.ProjectMasterModel.ProjectMasterId;
                            model.Updated = userId;
                            model.UpdatedDate = DateTime.Now;

                            HttpFileCollectionBase uploadedFiles = Request.Files;

                            string[] uploadedFiles1 = Request.Files.AllKeys;

                            HttpPostedFileBase userPostedFile = null;
                            var urls = string.Empty;
                            for (int i = 0; i < uploadedFiles.Count; i++)
                            {

                                var convertUploadedFiles1 = Convert.ToInt64(uploadedFiles1[i]);

                                if (model.SwQcIssueId == Convert.ToInt64(convertUploadedFiles1))
                                {
                                    userPostedFile = uploadedFiles[i];
                                    model.File1 = userPostedFile;
                                    if (model.File1.ContentLength > 0 && model.File1 != null)
                                    {
                                        var res = manager.Upload(swQcSpecification.ProjectMasterModel.ProjectMasterId,
                                            "SwQcIssues", "Shutdown", model.File1);

                                        Console.Write("res  :" + res);
                                        urls = urls + res + "|";

                                    }
                                }
                            }
                            model.UploadedFile = !string.IsNullOrWhiteSpace(urls) ? urls.TrimEnd('|') : model.UploadedFile;
                        }
                        bool isSaved = _repository.UpdateSwQcShutDown(swQcSpecification.SwQcShutDownModels);
                    }
                    else if (swQcSpecification.Tabname == "")
                    {
                        //something esle will happen here
                    }

                }

                //ViewBag.ControllerVariable = notificationObject;
            }

            return RedirectToAction("SwQcSpecification", new { projectId = swQcSpecification.ProjectMasterModel.ProjectMasterId, AssignId = swQcSpecification.AssignId, tabName = swQcSpecification.Tabname, projectType = swQcSpecification.projectType });
        }

        //public String SwQcTabFunction(String projectMasterId)
        //{

        //    long projectId;
        //    long.TryParse(projectMasterId, out projectId);

        //    _repository.GetAllTabColorAccordingToProject(projectId);
        //    //  SwQcFieldTestViewModel viewModel = new SwQcFieldTestViewModel();
        //    return null;
        //}
        #endregion

        #region Project Load for Incharge, Details and Download

        [Authorize(Roles = "QCHEAD,SA,PM,PMHEAD")]
        public ActionResult SwInchargeProjectSubmit()
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            var joiningTableReturn = _repository.GetAllProjectForSendToQcInchargeToPmList();

            return View(joiningTableReturn);
        }
        public ActionResult DetailsOfSwQc(long projectId, long swqcInchargeId)
        {
            var fileManager = new FileManager();
            var vmSwInchargemodel = new VmSwInchargeViewModel();
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name);

            /////For Re-assign project to Qc////////////

            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            List<PmQcAssignModel> pmQcAssignModels = _repository.GetPmQcAssignModels();
            List<CmnUserModel> list = _repository.GetActiveQc();
            List<TestPhaseModel> testPhaseList = _repository.GetTestPhases();

            vmSwInchargemodel.PmQcAssignModels = pmQcAssignModels;
            vmSwInchargemodel.ddlAssignUsersList = list;
            vmSwInchargemodel.ddlTestPhasesList = testPhaseList;
            ///////////////////////////////////////

            if (projectId > 0)
            {
                vmSwInchargemodel.ProjectMasterModel = _repository.GetProjectMasterModel(projectId);
                vmSwInchargemodel.SwQcStartUpModels = _repository.GetStartUpsForDetails(projectId, swqcInchargeId);
                if (vmSwInchargemodel.SwQcStartUpModels.Any())
                {


                    foreach (SwQcStartUpModel model in vmSwInchargemodel.SwQcStartUpModels)
                    {

                        //if (model.UploadedFile != null)
                        //{
                        //    var urls = model.UploadedFile.Split('|').ToList();
                        //    for (int i = 0; i < urls.Count; i++)
                        //    {
                        //        model.ScreenShotGetUrl1.Add(GetFile1(urls[i]));
                        //    }
                        //}


                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                FilesDetail detail = new FilesDetail();
                                detail.FilePath = fileManager.GetFile(urls[i]);
                                detail.Extention = fileManager.GetExtension(urls[i]);

                                model.FilesDetails.Add(detail);
                                // model.ScreenShotGetUrl1.Add(fileManager.GetFile1(urls[i]));
                                // model.FileExtensions.Add(fileManager.GetExtension(urls[i]));


                            }

                        }

                    }
                }
                vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcStartUpModels.Any() &&
                                           vmSwInchargemodel.SwQcStartUpModels[0].SwQcStartUpId > 0;

                ///////////////SwQcCallSettingModel/////
                vmSwInchargemodel.SwQcCallSettingModels = _repository.GetCallSettingForDetails(projectId, swqcInchargeId);
                if (vmSwInchargemodel.SwQcCallSettingModels.Any())
                {


                    foreach (SwQcCallSettingModel model in vmSwInchargemodel.SwQcCallSettingModels)
                    {

                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                FilesDetailForCall detail = new FilesDetailForCall();
                                detail.FilePath = fileManager.GetFile(urls[i]);
                                detail.Extention = fileManager.GetExtension(urls[i]);

                                model.FilesDetails.Add(detail);

                            }

                        }

                    }
                }
                vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcCallSettingModels.Any() &&
                                           vmSwInchargemodel.SwQcCallSettingModels[0].SwQcCallSettingId > 0;

                ///////////////SwQcMessageModel/////
                vmSwInchargemodel.SwQcMessageModels = _repository.GetMessageForDetails(projectId, swqcInchargeId);
                if (vmSwInchargemodel.SwQcMessageModels.Any())
                {


                    foreach (SwQcMessageModel model in vmSwInchargemodel.SwQcMessageModels)
                    {

                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                FilesDetailForMessage detail = new FilesDetailForMessage();
                                detail.FilePath = fileManager.GetFile(urls[i]);
                                detail.Extention = fileManager.GetExtension(urls[i]);

                                model.FilesDetails.Add(detail);

                            }

                        }

                    }
                }
                vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcMessageModels.Any() &&
                                           vmSwInchargemodel.SwQcMessageModels[0].SwQcMassageId > 0;

                /////////////SwQcToolsCheckModel/////
                vmSwInchargemodel.SwQcToolsCheckModels = _repository.GetToolsForDetails(projectId, swqcInchargeId);
                if (vmSwInchargemodel.SwQcToolsCheckModels.Any())
                {
                    foreach (SwQcToolsCheckModel model in vmSwInchargemodel.SwQcToolsCheckModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                FilesDetailForTools detail = new FilesDetailForTools();
                                detail.FilePath = fileManager.GetFile(urls[i]);
                                detail.Extention = fileManager.GetExtension(urls[i]);

                                model.FilesDetails.Add(detail);

                            }

                        }

                    }
                }
                vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcToolsCheckModels.Any() &&
                                           vmSwInchargemodel.SwQcToolsCheckModels[0].SwQcToolsCheckId > 0;

                /////////////SwQcCameraModel/////
                vmSwInchargemodel.SwQcCameraModels = _repository.GetCameraForDetails(projectId, swqcInchargeId);
                if (vmSwInchargemodel.SwQcCameraModels.Any())
                {
                    foreach (SwQcCameraModel model in vmSwInchargemodel.SwQcCameraModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                FilesDetailForCamera detail = new FilesDetailForCamera();
                                detail.FilePath = fileManager.GetFile(urls[i]);
                                detail.Extention = fileManager.GetExtension(urls[i]);

                                model.FilesDetails.Add(detail);

                            }

                        }

                    }
                }
                vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcCameraModels.Any() &&
                                           vmSwInchargemodel.SwQcCameraModels[0].SwQcCameraId > 0;
                /////////////SwQcDisplayLoopModel/////
                vmSwInchargemodel.SwQcDisplayLoopModels = _repository.GetDisplayLoopForDetails(projectId, swqcInchargeId);
                if (vmSwInchargemodel.SwQcDisplayLoopModels.Any())
                {
                    foreach (SwQcDisplayLoopModel model in vmSwInchargemodel.SwQcDisplayLoopModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                FilesDetailForDisplayLoop detail = new FilesDetailForDisplayLoop();
                                detail.FilePath = fileManager.GetFile(urls[i]);
                                detail.Extention = fileManager.GetExtension(urls[i]);

                                model.FilesDetails.Add(detail);

                            }

                        }

                    }
                }
                vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcDisplayLoopModels.Any() &&
                                           vmSwInchargemodel.SwQcDisplayLoopModels[0].SwQcDisplayLoopId > 0;

                /////////////SwQcDisplayModel/////
                vmSwInchargemodel.SwQcDisplayModels = _repository.GetDisplayForDetails(projectId, swqcInchargeId);
                if (vmSwInchargemodel.SwQcDisplayModels.Any())
                {
                    foreach (SwQcDisplayModel model in vmSwInchargemodel.SwQcDisplayModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                FilesDetailForDisplay detail = new FilesDetailForDisplay();
                                detail.FilePath = fileManager.GetFile(urls[i]);
                                detail.Extention = fileManager.GetExtension(urls[i]);

                                model.FilesDetails.Add(detail);

                            }

                        }

                    }
                }
                vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcDisplayModels.Any() &&
                                           vmSwInchargemodel.SwQcDisplayModels[0].SwQcDisplayId > 0;
                /////////////SwQcSettingModel/////
                vmSwInchargemodel.SwQcSettingModels = _repository.GetSettingForDetails(projectId, swqcInchargeId);
                if (vmSwInchargemodel.SwQcSettingModels.Any())
                {
                    foreach (SwQcSettingModel model in vmSwInchargemodel.SwQcSettingModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                FilesDetailForSetting detail = new FilesDetailForSetting();
                                detail.FilePath = fileManager.GetFile(urls[i]);
                                detail.Extention = fileManager.GetExtension(urls[i]);

                                model.FilesDetails.Add(detail);

                            }

                        }

                    }
                }
                vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcSettingModels.Any() &&
                                           vmSwInchargemodel.SwQcSettingModels[0].SwQcSettingsId > 0;
                /////////////SwQcMultimediaModel/////
                vmSwInchargemodel.SwQcMultimediaModels = _repository.GetMultimediaForDetails(projectId, swqcInchargeId);
                if (vmSwInchargemodel.SwQcMultimediaModels.Any())
                {
                    foreach (SwQcMultimediaModel model in vmSwInchargemodel.SwQcMultimediaModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                FilesDetailForMultimedia detail = new FilesDetailForMultimedia();
                                detail.FilePath = fileManager.GetFile(urls[i]);
                                detail.Extention = fileManager.GetExtension(urls[i]);

                                model.FilesDetails.Add(detail);

                            }

                        }

                    }
                }
                vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcMultimediaModels.Any() &&
                                           vmSwInchargemodel.SwQcMultimediaModels[0].SwQcMultimediaId > 0;
                ///////////////SwQcGoogleServiceModel/////
                vmSwInchargemodel.SwQcGoogleServiceModels = _repository.GetGoogleServiceForDetails(projectId, swqcInchargeId);
                if (vmSwInchargemodel.SwQcGoogleServiceModels.Any())
                {
                    foreach (SwQcGoogleServiceModel model in vmSwInchargemodel.SwQcGoogleServiceModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                FilesDetailForGoogleService detail = new FilesDetailForGoogleService();
                                detail.FilePath = fileManager.GetFile(urls[i]);
                                detail.Extention = fileManager.GetExtension(urls[i]);

                                model.FilesDetails.Add(detail);

                            }

                        }

                    }
                }
                vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcGoogleServiceModels.Any() &&
                                           vmSwInchargemodel.SwQcGoogleServiceModels[0].SwQcGoogleServicesId > 0;

                ///////////////SwQcStorageCheckModel/////
                vmSwInchargemodel.SwQcStorageCheckModels = _repository.GetStorageCheckForDetails(projectId, swqcInchargeId);
                if (vmSwInchargemodel.SwQcStorageCheckModels.Any())
                {
                    foreach (SwQcStorageCheckModel model in vmSwInchargemodel.SwQcStorageCheckModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                FilesDetailForStorage detail = new FilesDetailForStorage();
                                detail.FilePath = fileManager.GetFile(urls[i]);
                                detail.Extention = fileManager.GetExtension(urls[i]);

                                model.FilesDetails.Add(detail);

                            }

                        }

                    }
                }
                vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcStorageCheckModels.Any() &&
                                           vmSwInchargemodel.SwQcStorageCheckModels[0].SwQcStorageCheckId > 0;
                ///////////////SwQcGameModel/////
                vmSwInchargemodel.SwQcGameModels = _repository.GetGameForDetails(projectId, swqcInchargeId);
                if (vmSwInchargemodel.SwQcGameModels.Any())
                {
                    foreach (SwQcGameModel model in vmSwInchargemodel.SwQcGameModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                FilesDetailForGame detail = new FilesDetailForGame();
                                detail.FilePath = fileManager.GetFile(urls[i]);
                                detail.Extention = fileManager.GetExtension(urls[i]);

                                model.FilesDetails.Add(detail);

                            }

                        }

                    }
                }
                vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcGameModels.Any() &&
                                           vmSwInchargemodel.SwQcGameModels[0].SwQcGameId > 0;

                ///////////////SwQcTestingAppModel/////
                vmSwInchargemodel.SwQcTestingAppModels = _repository.GetTestingAppForDetails(projectId, swqcInchargeId);
                if (vmSwInchargemodel.SwQcTestingAppModels.Any())
                {
                    foreach (SwQcTestingAppModel model in vmSwInchargemodel.SwQcTestingAppModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                FilesDetailForTesting detail = new FilesDetailForTesting();
                                detail.FilePath = fileManager.GetFile(urls[i]);
                                detail.Extention = fileManager.GetExtension(urls[i]);

                                model.FilesDetails.Add(detail);

                            }

                        }

                    }
                }
                vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcTestingAppModels.Any() &&
                                           vmSwInchargemodel.SwQcTestingAppModels[0].SwQcTestingAppId > 0;

                ///////////////SwQcFileManagerModel/////
                vmSwInchargemodel.SwQcFileManagerModels = _repository.GetFileManageForDetails(projectId, swqcInchargeId);
                if (vmSwInchargemodel.SwQcFileManagerModels.Any())
                {
                    foreach (SwQcFileManagerModel model in vmSwInchargemodel.SwQcFileManagerModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                FilesDetailForFileManager detail = new FilesDetailForFileManager();
                                detail.FilePath = fileManager.GetFile(urls[i]);
                                detail.Extention = fileManager.GetExtension(urls[i]);

                                model.FilesDetails.Add(detail);

                            }

                        }

                    }
                }
                vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcFileManagerModels.Any() &&
                                           vmSwInchargemodel.SwQcFileManagerModels[0].SwQcFileManagerId > 0;

                ///////////////SwQcConnectivityModel/////
                vmSwInchargemodel.SwQcConnectivityModels = _repository.GetConnectivityForDetails(projectId, swqcInchargeId);
                if (vmSwInchargemodel.SwQcConnectivityModels.Any())
                {
                    foreach (SwQcConnectivityModel model in vmSwInchargemodel.SwQcConnectivityModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                FilesDetailForConnectivity detail = new FilesDetailForConnectivity();
                                detail.FilePath = fileManager.GetFile(urls[i]);
                                detail.Extention = fileManager.GetExtension(urls[i]);

                                model.FilesDetails.Add(detail);

                            }

                        }

                    }
                }
                vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcConnectivityModels.Any() &&
                                           vmSwInchargemodel.SwQcConnectivityModels[0].SwQcConnectivityId > 0;
                ///////////////SwQcShutDownModel/////
                vmSwInchargemodel.SwQcShutDownModels = _repository.GetShutDownForDetails(projectId, swqcInchargeId);
                if (vmSwInchargemodel.SwQcShutDownModels.Any())
                {
                    foreach (SwQcShutDownModel model in vmSwInchargemodel.SwQcShutDownModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                FilesDetailForShutDown detail = new FilesDetailForShutDown();
                                detail.FilePath = fileManager.GetFile(urls[i]);
                                detail.Extention = fileManager.GetExtension(urls[i]);

                                model.FilesDetails.Add(detail);

                            }

                        }

                    }
                }
                vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcShutDownModels.Any() &&
                                           vmSwInchargemodel.SwQcShutDownModels[0].SwQcShutDownId > 0;

                ///////////////SwQcProjectWiseIssueViewModel/////
                vmSwInchargemodel.SwQcProjectWiseIssueViewModels = _repository.GetProjectWiseIssueViewModelsForDetails(projectId, swqcInchargeId);
                if (vmSwInchargemodel.SwQcProjectWiseIssueViewModels.Any())
                {
                    foreach (SwQcProjectWiseIssueViewModel model in vmSwInchargemodel.SwQcProjectWiseIssueViewModels)
                    {
                        if (model.UploadedFile != null)
                        {
                            var urls = model.UploadedFile.Split('|').ToList();
                            for (int i = 0; i < urls.Count; i++)
                            {
                                FilesDetailForSwQcProjectWise detail = new FilesDetailForSwQcProjectWise();
                                detail.FilePath = fileManager.GetFile(urls[i]);
                                detail.Extention = fileManager.GetExtension(urls[i]);

                                model.FilesDetails.Add(detail);

                            }

                        }

                    }
                }
                vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcProjectWiseIssueViewModels.Any() &&
                                           vmSwInchargemodel.SwQcProjectWiseIssueViewModels[0].SwQcProjectWiseIssueId > 0;
                ///////////////SwQcBatteryAssignIssueModel/////
                //vmSwInchargemodel.SwQcBatteryAssignIssueModelsList = _repository.GetSwQcBatteryForDetails(projectId);
                //if (vmSwInchargemodel.SwQcBatteryAssignIssueModelsList.Any())
                //{
                //    foreach (SwQcBatteryAssignIssueModel model in vmSwInchargemodel.SwQcBatteryAssignIssueModelsList)
                //    {
                //        model.ScreenShotGetUrl1 = fileManager.GetFile(model.ScreenShot1FilePath);
                //        model.ScreenShotGetUrl2 = fileManager.GetFile(model.ScreenShot2FilePath);
                //        model.VideoUploadGetUrl1 = fileManager.GetFile(model.VideoUpload1FilePath);
                //        model.VideoUploadGetUrl2 = fileManager.GetFile(model.VideoUpload2FilePath);

                //    }
                //}
                //vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcBatteryAssignIssueModelsList.Any() &&
                //                           vmSwInchargemodel.SwQcBatteryAssignIssueModelsList[0].SwQcBatteryAssignIssuesId > 0;
            }

            return View(vmSwInchargemodel);

        }
        public FileResult Download(string ImageName)
        {

            return File("" + ImageName, System.Net.Mime.MediaTypeNames.Application.Octet);


        }

        ///Details for Alls////
        //public ActionResult DetailsOfSwQcForAll(long projectId)
        //{
        //    var fileManager = new FileManager();
        //    var vmSwInchargemodel = new VmSwInchargeViewModel();
        //    long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name);

        //    /////For Re-assign project to Qc////////////

        //    ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
        //    List<PmQcAssignModel> pmQcAssignModels = _repository.GetPmQcAssignModels();
        //    List<CmnUserModel> list = _repository.GetActiveQc();
        //    List<TestPhaseModel> testPhaseList = _repository.GetTestPhases();

        //    vmSwInchargemodel.PmQcAssignModels = pmQcAssignModels;
        //    vmSwInchargemodel.ddlAssignUsersList = list;
        //    vmSwInchargemodel.ddlTestPhasesList = testPhaseList;
        //    ///////////////////////////////////////

        //    if (projectId > 0)
        //    {
        //        vmSwInchargemodel.ProjectMasterModel = _repository.GetProjectMasterModel(projectId);
        //        vmSwInchargemodel.SwQcStartUpModels = _repository.AllGetStartUpsForDetails(projectId);
        //        if (vmSwInchargemodel.SwQcStartUpModels.Any())
        //        {


        //            foreach (SwQcStartUpModel model in vmSwInchargemodel.SwQcStartUpModels)
        //            {

        //                //if (model.UploadedFile != null)
        //                //{
        //                //    var urls = model.UploadedFile.Split('|').ToList();
        //                //    for (int i = 0; i < urls.Count; i++)
        //                //    {
        //                //        model.ScreenShotGetUrl1.Add(GetFile1(urls[i]));
        //                //    }
        //                //}


        //                if (model.UploadedFile != null)
        //                {
        //                    var urls = model.UploadedFile.Split('|').ToList();
        //                    for (int i = 0; i < urls.Count; i++)
        //                    {
        //                        FilesDetail detail = new FilesDetail();
        //                        detail.FilePath = fileManager.GetFile(urls[i]);
        //                        detail.Extention = fileManager.GetExtension(urls[i]);

        //                        model.FilesDetails.Add(detail);
        //                        // model.ScreenShotGetUrl1.Add(fileManager.GetFile1(urls[i]));
        //                        // model.FileExtensions.Add(fileManager.GetExtension(urls[i]));


        //                    }

        //                }

        //            }
        //        }
        //        vmSwInchargemodel.IsEdit = vmSwInchargemodel.SwQcStartUpModels.Any() &&
        //                                   vmSwInchargemodel.SwQcStartUpModels[0].SwQcStartUpId > 0;

        //        ///////////////SwQcCallSettingModel/////
        //        vmSwInchargemodel.SwQcCallSettingModels = _repository.AllGetCallSettingForDetails(projectId);
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
        //        vmSwInchargemodel.SwQcMessageModels = _repository.AllGetMessageForDetails(projectId);
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
        //        vmSwInchargemodel.SwQcToolsCheckModels = _repository.AllGetToolsForDetails(projectId);
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
        //        vmSwInchargemodel.SwQcCameraModels = _repository.AllGetCameraForDetails(projectId);
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
        //        vmSwInchargemodel.SwQcDisplayLoopModels = _repository.AllGetDisplayLoopForDetails(projectId);
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
        //        vmSwInchargemodel.SwQcDisplayModels = _repository.AllGetDisplayForDetails(projectId);
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
        //        vmSwInchargemodel.SwQcSettingModels = _repository.AllGetSettingForDetails(projectId);
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
        //        vmSwInchargemodel.SwQcMultimediaModels = _repository.AllGetMultimediaForDetails(projectId);
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
        //        vmSwInchargemodel.SwQcGoogleServiceModels = _repository.AllGetGoogleServiceForDetails(projectId);
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
        //        vmSwInchargemodel.SwQcStorageCheckModels = _repository.AllGetStorageCheckForDetails(projectId);
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
        //        vmSwInchargemodel.SwQcGameModels = _repository.AllGetGameForDetails(projectId);
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
        //        vmSwInchargemodel.SwQcTestingAppModels = _repository.AllGetTestingAppForDetails(projectId);
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
        //        vmSwInchargemodel.SwQcFileManagerModels = _repository.AllGetFileManageForDetails(projectId);
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
        //        vmSwInchargemodel.SwQcConnectivityModels = _repository.AllGetConnectivityForDetails(projectId);
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
        //        vmSwInchargemodel.SwQcShutDownModels = _repository.AllGetShutDownForDetails(projectId);
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
        //        vmSwInchargemodel.SwQcProjectWiseIssueViewModels = _repository.AllGetProjectWiseIssueViewModelsForDetails(projectId);
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

        #region Re-Assign QcIncharge to QC

        [Authorize(Roles = "QCHEAD,SA")]

        [NotificationActionFilter(ReceiverRoles = "QCHEAD,QC,PM,MM,PS")]
        [HttpPost]

        public JsonResult QcInchargeToQcReAssignProject(string testPhaseId, string projectMasterId, string projectName, string swqcInchargeAId, string multiple1, string ApproxInchargeToQcDeliveryDate, string SwInchargeAssignToQcComment, string projectPmAssignId, string pmDate, string softwareName, string softwareNo)
        {
            var _dbEntities = new CellPhoneProjectEntities();
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name);
            var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);

            if (userId > 0)
            {
                long pMasterId;
                long.TryParse(projectMasterId, out pMasterId);
                long sWQcInId_id = 0;
                long.TryParse(swqcInchargeAId, out sWQcInId_id);

                //////----------MAIL Start-----------------------
                ////var swQcInchargeproInfo = _dbEntities.SwQcHeadAssignsFromPms.FirstOrDefault(i => i.ProjectMasterId == pMasterId && i.SwQcInchargeAssignId == sWQcInId_id);
                //////var tPhaseName = _dbEntities.TestPhases.FirstOrDefault(i => i.TestPhaseID == swQcInchargeproInfo.TestPhaseID);
                ////var proName = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == pMasterId);

                //////---------------ends-----------------

                var notificationObject = new NotificationObject
                {
                    ProjectId = pMasterId,
                    ToUser = "-1"
                };

                var multiple11 = multiple1.Split(',');

                if (multiple11 != null)
                {
                    List<long> ids = new List<long>();
                    string body = string.Empty;
                    string assignedQc = string.Empty;
                    foreach (var mId in multiple11)
                    {
                        notificationObject.ToUser = notificationObject.ToUser + mId + ",";
                        long qcIDs;
                        long.TryParse(mId, out qcIDs);
                        ids.Add(qcIDs);
                        var assignedUserName = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == qcIDs);
                        assignedQc = assignedQc + assignedUserName.UserFullName + ", ";
                    }


                    //    assignedQc = assignedQc.TrimEnd(',');
                    //    body = "This is to inform you that a QC Completed project has been re-assigned/assigned by <b>" + user.UserFullName + " </b> for testing of Software QC issues.<br/><br/><br/> <br/>" + "Project : <b>" + proName.ProjectName + "</b> <br/>Assigned to : " + assignedQc + "<br/>Sample Type - " + swQcInchargeproInfo.ProjectManagerSampleType + "<br/>Sample Quantity - " + swQcInchargeproInfo.ProjectManagerSampleNo;
                    //    var mailSendFromPms = new MailSendFromPms();
                    //    mailSendFromPms.SendMail(ids, new List<string>(new[] { "MM", "SA", "PS" }), "Software QC has been re-assigned for a QC Completed Project(" + proName.ProjectName + ")", body);
                }
                notificationObject.Message = "re-assigned for Checking Software Issues";

                long pSwQcInId;
                long.TryParse(swqcInchargeAId, out pSwQcInId);

                long pPrPmAssignId;
                long.TryParse(projectPmAssignId, out pPrPmAssignId);

                var sWQcInchargeToQCProjectReAssignResult = _repository.SaveQcInchargeToQcReAssignProject(testPhaseId, pMasterId, projectName, pSwQcInId, multiple1, ApproxInchargeToQcDeliveryDate, SwInchargeAssignToQcComment, pPrPmAssignId, pmDate, softwareName, softwareNo);

                ViewBag.ControllerVariable = notificationObject;
                return Json(sWQcInchargeToQCProjectReAssignResult, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = "Redirect", url = Url.Action("Software", "SwInchargeProjectSubmit") });
        }


        #endregion

        #region AssignMutipleQc

        [Authorize(Roles = "QCHEAD,SA")]
        public ActionResult AssignMuliplePerson(string ProjectName)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            //Get from Repository
            List<PmQcAssignModel> pmQcAssignModels = _repository.GetPmToQcHeadAssignModels();
            List<CmnUserModel> list = _repository.GetActiveQc();

            //Create a ViewModel
            AssignMuliplePersonViewModel model = new AssignMuliplePersonViewModel();
            model.PmQcAssignModels = pmQcAssignModels;
            model.ddlAssignUsersList = list;

            model.ProjectMasterModelsList = _repository.GetProjectListForSwQcHead();
            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Select One" } };

            var query = (from master in model.ProjectMasterModelsList
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
            ViewBag.CombinedIds = selectListItems;
            model.ProjectName = ProjectName;

            return View(model);
        }

        [NotificationActionFilter(ReceiverRoles = "QCHEAD,QC,PM,MM,PS")]
        [HttpPost]
        public JsonResult AssignMuliplePerson(String ProjectName, String ProjectMasterId, String SwQcInchargeAssignId, String ProjectPmAssignId, String SwInchargeAssignToQcComment, String[] multiple, String ApproxInchargeToQcDeliveryDate, String AccessoriesTestType)
        {
            var _dbEntities = new CellPhoneProjectEntities();

            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name; ;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);

            long sWQcInId_id = 0;
            long.TryParse(SwQcInchargeAssignId, out sWQcInId_id);
            long p_id = 0;
            long.TryParse(ProjectMasterId, out p_id);

            //----------MAIL Start-----------------------
            var swQcInchargeproInfo = _dbEntities.SwQcHeadAssignsFromPms.FirstOrDefault(i => i.ProjectMasterId == p_id && i.SwQcHeadAssignId == sWQcInId_id && i.Status != "RECOMMENDED");
            var tPhaseName = _dbEntities.SwQcTestPhases.FirstOrDefault(i => i.TestPhaseID == swQcInchargeproInfo.TestPhaseID);
            var proName = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == p_id);

            //---------------ends-----------------

            var notificationObject = new NotificationObject
            {

                ProjectId = p_id,
                //ToUser = multiple
            };
            if (multiple != null && AccessoriesTestType != "Os Requirement Analysis")
            {
                List<long> ids = new List<long>();
                string body = string.Empty;
                string assignedQc = string.Empty;
                foreach (string mId in multiple)
                {
                    notificationObject.ToUser = notificationObject.ToUser + mId + ",";
                    long qcIDs;
                    long.TryParse(mId, out qcIDs);
                    ids.Add(qcIDs);
                    var assignedUserName = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == qcIDs);
                    assignedQc = assignedQc + assignedUserName.UserFullName + ", ";
                }
                //mailSendFromPms.SendMail(new List<long>(new[] { pmUserIDCon }), new List<string>(new[] { "PMHEAD", "SA" }), "Project forwarded by Software QC Incharge",
                //  mailBody);
                assignedQc = assignedQc.TrimEnd(',');
                body = "This is to inform you that a project has been assigned by  <b>" + user.UserFullName + " </b> for testing of Software QC issues.<br/><br/><br/> <br/>" + "Project : <b>" + proName.ProjectName + "</b> <br/>Assigned to : " + assignedQc + "<br/>Sample Type - " + swQcInchargeproInfo.ProjectManagerSampleType + "<br/>Sample Quantity - " + swQcInchargeproInfo.ProjectManagerSampleNo + "<br/>Test Phase Name - " + tPhaseName.TestPhaseName
                    + "<br/>SoftwareVersion Name - " + swQcInchargeproInfo.SoftwareVersionName + "<br/>Software Version Number - " + swQcInchargeproInfo.SoftwareVersionNo;
                var mailSendFromPms = new MailSendFromPms();
                //var extendedRole = _dbEntities.CmnUsers.FirstOrDefault(i => i.ExtendedRoleName == "QCHEAD" && i.IsActive==true);

                mailSendFromPms.SendMail(ids, new List<string>(new[] { "MM", "PS","PMHEAD", "QCHEAD" }), "Software QC has been assigned for a New Project(" + proName.ProjectName + ")", body);
            }
           
            String insert = _repository.SaveAssignMuliplePerson(ProjectMasterId, SwQcInchargeAssignId, ProjectPmAssignId, SwInchargeAssignToQcComment, multiple, ApproxInchargeToQcDeliveryDate, AccessoriesTestType);
            if (AccessoriesTestType != "Os Requirement Analysis")
            {
                notificationObject.Message = "assigned for Checking Software Issues";
                ViewBag.ControllerVariable = notificationObject;
            }
            return new JsonResult { Data = "ok", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //  return Json(new { result = "Redirect", url = Url.Action("Software", "AssignMuliplePerson") });
        }

        [HttpPost]
        public JsonResult Edit()
        {
            if (ModelState.IsValid)
            {
                // Save  
                return Json(new { Ok = true });
            }

            return Json(new { Ok = false });
        }

        [HttpPost]
        public JsonResult ForwardFirstVersionIssueToSecondVersion(string objArr)
        {
            List<SwQcIssueDetailModel> results = JsonConvert.DeserializeObject<List<SwQcIssueDetailModel>>(objArr);
            //Console.Write("result :" + results);

            //var saveSwIncentive = "0";
            //if (results.Count != 0)
            //{
            //    saveSwIncentive = _repository.SaveSwIncentive_OthersAuto(results);
            //}

            //return Json(new { data = saveSwIncentive }, JsonRequestBehavior.AllowGet);

            var saveIncentive = "0";
            if (results.Count != 0)
            {
                saveIncentive = _repository.ForwardFirstVersionIssueToSecondVersion(results);
            }
            return new JsonResult { Data = saveIncentive, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        //OsRequirementAnalysis
        public ActionResult OsRequirementAnalysis()
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.OsRequirementAnalysisData = _repository.OsRequirementAnalysisData(userId);

            var fileManager = new FileManager();
            if (ViewBag.OsRequirementAnalysisData != null)
            {
                foreach (var model in ViewBag.OsRequirementAnalysisData)
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

            ViewBag.OsRequirementAnalysisDoneData = _repository.OsRequirementAnalysisDoneData(userId);

            if (ViewBag.OsRequirementAnalysisDoneData != null)
            {
                foreach (var model in ViewBag.OsRequirementAnalysisDoneData)
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
        public ActionResult OsRequirementAnalysis(List<ProjectMasterModel> issueList1, String proIds, String swQcAssignId, String swQcHeadUserId)
        {
            var manager = new FileManager();

            var Attachment = "";
            long proId = 0;
            long.TryParse(proIds, out proId);

            long swQcAssignIds = 0;
            long.TryParse(swQcAssignId, out swQcAssignIds);

            long swQcHeadUserIds = 0;
            long.TryParse(swQcHeadUserId, out swQcHeadUserIds);

            // var proDetails = _repository.GetOsRequirementData(proId, swQcAssignIds, swQcHeadUserIds);

            foreach (var items in issueList1)
            {
                if (items.FileId != null)
                {
                    var res = manager.Upload3(Convert.ToInt64(proIds), Convert.ToInt64(swQcAssignIds), Convert.ToInt64(swQcHeadUserIds),
                    "OsRequirementAnalysis", "OsRequirement", items.FileId);

                    Console.Write("res  :" + res);

                    items.SupportingDocument = items.SupportingDocument == null ? res : items.SupportingDocument + "|" + res;

                    Attachment = items.SupportingDocument;
                }
            }

            _repository.UpdateOsRequirementAnalysis(proId, swQcAssignIds, swQcHeadUserIds, Attachment);

            return RedirectToAction("OsRequirementAnalysis");

        }
        #endregion

        #region Qc Incharge to PM Assign

        [Authorize(Roles = "QCHEAD,SA")]

        [NotificationActionFilter(ReceiverRoles = "QCHEAD,PM,MM,PS")]
        [HttpPost]
        public JsonResult QcInchargeToPmProjectForward(string testPhaseId, string projectMasterId, string projectName, string swqcInchargeAId, string proComment, string projectPmAssignId, DateTime pmDate, string softwareName, string softwareNo, bool isFinals, DateTime swQcHeadToQcAssignTime)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var _dbEntities = new CellPhoneProjectEntities();

            if (userId > 0)
            {

                long pmUserIDCon;
                long.TryParse(projectPmAssignId, out pmUserIDCon);

                long pMasterId;
                long.TryParse(projectMasterId, out pMasterId);

                var notificationObject = new NotificationObject
                {
                    ProjectId = pMasterId,
                    ToUser = projectPmAssignId
                };

                notificationObject.Message = "forwarded QC report";

                long pSwQcInId;
                long.TryParse(swqcInchargeAId, out pSwQcInId);

                //var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);
                //var swQcInchargeproInfo = _dbEntities.SwQcInchargeAssigns.FirstOrDefault(i => i.ProjectMasterId == pMasterId && i.SwQcInchargeAssignId == pSwQcInId);
                //var tPhaseName = _dbEntities.TestPhases.FirstOrDefault(i => i.TestPhaseID == swQcInchargeproInfo.TestPhaseID);

                //var assignedUserName = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == swQcInchargeproInfo.Added);

                //string mailBody = "This is to inform you that a project has been forwarded to <b>" + assignedUserName.UserFullName + "</b>, by <b>" + user.UserFullName + "</b>, after a successful testing of Software QC issues. <br/><br/><br/><br/>" + "Project : <b>" + projectName + "</b> <br/>Sample Type - " + swQcInchargeproInfo.ProjectManagerSampleType + "<br/>Sample Quantity - " + swQcInchargeproInfo.ProjectManagerSampleNo + "<br/>Test Phase Name - " + tPhaseName.TestPhaseName;
                //MailSendFromPms mailSendFromPms = new MailSendFromPms();
                //mailSendFromPms.SendMail(new List<long>(new[] { pmUserIDCon }), new List<string>(new[] { "PMHEAD", "SA", "PS" }), "Project(" + projectName + ") has been forwarded by Software QC Incharge",
                //    mailBody);

                //string testPhaseId, string projectMasterId, string projectName, string swqcInchargeAId, string proComment, string projectPmAssignId, string pmDate, string softwareName, string softwareNo, bool isFinals)


                var sWQcInchargeSubmitToPmResult = _repository.SoftWareQcInchargeToPm(pMasterId, pmUserIDCon, testPhaseId, userId, pSwQcInId, projectName, proComment,
                    pmDate, softwareName, softwareNo, isFinals, swQcHeadToQcAssignTime);

                ViewBag.ControllerVariable = notificationObject;
                return Json(sWQcInchargeSubmitToPmResult, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "Redirect", url = Url.Action("Software", "SwInchargeProjectSubmit") });
        }
        [Authorize(Roles = "QCHEAD,SA")]
        public ActionResult SwQcHeadToPmProjectSubmit()
        {
            var vmSwInchargemodel = new VmSwQcHeadViewModel();

            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            //  var joiningTableReturn = _repository.GetCompletedProjectForQcHeadToPmSubmit();

            vmSwInchargemodel.SwQcAssignsFromQcHeadModels = _repository.GetCompletedProjectForQcHeadToPmSubmit();

            vmSwInchargemodel.SwQcAssignsFromQcHeadModels1 = _repository.GetCompletedFieldTestProjectForQcHeadToPmSubmit();

            var selectListItemsForRef = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT" } };
            List<CmnUserModel> list = _repository.GetInnoVationAssignedBy() ??
                                                     new List<CmnUserModel>();
            selectListItemsForRef.AddRange(list.Select(p => new SelectListItem { Value = p.RoleName, Text = p.RoleName }));
            ViewBag.ddlRolesForInno = selectListItemsForRef;

            List<SwQcNewInnovationModel> innoVat = _repository.GetNewInnovationForQcHead();
            if (innoVat != null)
            {
                ViewBag.GetInnovative = innoVat;

            }

            return View(vmSwInchargemodel);
        }
        [Authorize(Roles = "QCHEAD,SA")]
        public ActionResult DetailsOfSwQcsAllWork(string projectId, string swqcInchargeId, string pmAssignId, DateTime swQcHeadToQcAssignTime, string testPhaseId, DateTime PmToQcHeadAssignTime, string SoftwareVersionName, string SoftwareVersionNo)
        {
            var fileManager = new FileManager();
            var vmSwInchargemodel = new VmSwQcHeadViewModel();
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name);

            /////For Re-assign project to Qc////////////

            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            List<PmQcAssignModel> pmQcAssignModels = _repository.GetPmQcAssignModels();
            List<TestPhaseModel> testPhaseList = _repository.GetTestPhases();

            //vmSwInchargemodel.PmQcAssignModels = pmQcAssignModels;
            List<CmnUserModel> list = _repository.GetActiveQc();
            vmSwInchargemodel.ddlAssignUsersList = list;
            //vmSwInchargemodel.ddlTestPhasesList = testPhaseList;
            ///////////////////////////////////////

            if (projectId != null)
            {
                //  vmSwInchargemodel.ProjectMasterModel = _repository.GetProjectMasterModel(projectId);
                //  vmSwInchargemodel.SwQcIssueDetailModels = _repository.GetSwQcIssue(projectId, swqcInchargeId);
                vmSwInchargemodel.SwQcIssueDetailModels = _repository.GetSwQcIssueDetailsForQcHeadToPmForward(projectId, swqcInchargeId, pmAssignId, swQcHeadToQcAssignTime, testPhaseId, SoftwareVersionNo);
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

                vmSwInchargemodel.SwQcIssueDetailModels1 = _repository.GetSwQcCtsMonkeyOrCameraAutomationData(projectId, swqcInchargeId, pmAssignId, swQcHeadToQcAssignTime, testPhaseId, SoftwareVersionNo);
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


                vmSwInchargemodel.SwQcPersonalUseFindingsIssueDetailModels = _repository.GetPersonalUseFindingsForQcHead(projectId, swqcInchargeId, pmAssignId, swQcHeadToQcAssignTime, testPhaseId);
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

                //Earphone
                ViewBag.GetEarphoneDataForDetails = _repository.GetEarphoneDataForDetails(projectId, swqcInchargeId);

                //Battery
                ViewBag.GetSavedAccessoriesDataBattery = _repository.GetSavedBatteryDataForDetails(projectId, swqcInchargeId);

            }

            return View(vmSwInchargemodel);

        }
        #endregion

        #region QC to Incharge Final Project Submit

        [Authorize(Roles = "QCHEAD,QC,SA")]

        [NotificationActionFilter(ReceiverRoles = "QCHEAD,QC,MM,PS")]
        [HttpPost]
        public JsonResult QcToQcInchargeFinalProjectSubmit(string projectMasterId, string swQcUserId, string proStatus, string swQcInchargeAssignId)
        {
            var _dbEntities = new CellPhoneProjectEntities();
            long userId = Convert.ToInt64(User.Identity.Name);

            if (userId > 0)
            {
                long sPMasterId;
                long.TryParse(projectMasterId, out sPMasterId);

                long pSwQcInId;
                long.TryParse(swQcInchargeAssignId, out pSwQcInId);

                var notificationObject = new NotificationObject
                {
                    ProjectId = sPMasterId,
                    ToUser = "-1"
                };

                notificationObject.Message = "completed Checking Software Issues";


                long sQcUserId;
                long.TryParse(swQcUserId, out sQcUserId);

                //////////////////mail///////////////////////
                var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);
                var swQcInchargeproInfo = _dbEntities.SwQcInchargeAssigns.FirstOrDefault(i => i.ProjectMasterId == sPMasterId && i.SwQcInchargeAssignId == pSwQcInId);
                var tPhaseName = _dbEntities.TestPhases.FirstOrDefault(i => i.TestPhaseID == swQcInchargeproInfo.TestPhaseID);
                var proName = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == sPMasterId);

                var assignedUserName = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == swQcInchargeproInfo.SwQcInchargeUserId);

                string mailBody = "This is to inform you that a project has been completed by <b>" + user.UserFullName + "</b>, after a successful testing of Software QC issues.  <br/> <br/> <br/> <br/><br/>" + "Project : <b>" + proName.ProjectName
                    + "</b><br/>Sample Type - <b>" + swQcInchargeproInfo.ProjectManagerSampleType + "</b><br/>Sample Quantity - <b>" + swQcInchargeproInfo.ProjectManagerSampleNo + "</b><br/>Test Phase Name - <b>" + tPhaseName.TestPhaseName;
                MailSendFromPms mailSendFromPms = new MailSendFromPms();
                mailSendFromPms.SendMail(new List<string>(new[] { "QCHEAD" }), new List<string>(new[] { "MM", "PS" ,"QCHEAD","PMHEAD"}), "Project(" + proName.ProjectName + ") has been completed by Software QC Users",
                    mailBody);

                //////////////////mail///////////////////
                long sQcInchargeAssignId;
                long.TryParse(swQcInchargeAssignId, out sQcInchargeAssignId);
                var sWQcToQcInchargeFinalProject = _repository.SoftwareQcToQcInchargeProjectSubmit(sPMasterId, sQcUserId, proStatus, sQcInchargeAssignId);

                ViewBag.ControllerVariable = notificationObject;
                return Json(sWQcToQcInchargeFinalProject, JsonRequestBehavior.AllowGet);
            }


            return Json(new { result = "Redirect", url = Url.Action("Software", "SwQcSpecification") });
        }
        #endregion

        #region FieldTest old
        [Authorize(Roles = "HWHEAD,HW,SA")]
        [HttpGet]
        public ActionResult FieldTest()
        {

            var vmSwQcSpecification = new VmSwQcSpecificationModified();
            vmSwQcSpecification.CmnUserModels = _repository.GetActiveHw();
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            vmSwQcSpecification.ProjectMasterModelsList = _repository.GetProjectListForFieldTest();

            return View(vmSwQcSpecification);
        }

        [NotificationActionFilter(ReceiverRoles = "HWHEAD,HW,PM,MM,PS")]
        [HttpPost]
        public String FieldTest(String projectMasterId, String hwQcIncharge_userId, String[] ddlUsers, List<SwFieldTestDetailModel> details, String ComparedWith, String IssueOf, String COMMENT)
        {


            long pMasterId;
            long.TryParse(projectMasterId, out pMasterId);

            long phwQcInuId = Convert.ToInt64(User.Identity.Name);
            long.TryParse(hwQcIncharge_userId, out phwQcInuId);

            _repository.SaveFieldTest(pMasterId, phwQcInuId, ddlUsers, details, ComparedWith, IssueOf, COMMENT);

            return null;
        }

        //public String FieldTest(String projectMasterId, String pro_id, String swQcIncharge_id, String[] ddlUsers, List<SwFieldTestDetailModel> details, String ComparedWith, String IssueOf, String COMMENT)
        //{

        //    long pMasterId;
        //    long.TryParse(pro_id, out pMasterId);

        //    long pSwQcInId;
        //    long.TryParse(swQcIncharge_id, out pSwQcInId);

        //    _repository.SaveFieldTest(pMasterId, pSwQcInId, ddlUsers, details, ComparedWith, IssueOf, COMMENT);
        //    //  SwQcFieldTestViewModel viewModel = new SwQcFieldTestViewModel();
        //    return null;
        //}
        #endregion

        #region Field test report old
        [Authorize(Roles = "HWHEAD,SA")]
        public ActionResult ReportPrintFieldTest(long projectId = 0)
        {
            var vmSwQcSpecification = new VmSwQcSpecificationModified();
            //vmSwQcSpecification.CmnUserModels = _repository.GetActiveQc();

            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            vmSwQcSpecification.ProjectMasterModelsList = _repository.GetFieldTestCompletedProjectList();

            //long projectId = 0;
            if (projectId > 0)
            {
                //vmSwQcSpecification.ProjectMasterModel = _commercialRepository.GetProjectMasterModel(projectId);
                //vmSwQcSpecification.AssignId = _repository.GetAssignId(projectId, userId);
                vmSwQcSpecification.SwFieldTestReportViews = _repository.GetFieldTestProjectLisForPrintReport(projectId, userId);
            }

            return View(vmSwQcSpecification);
        }



        #endregion

        #region SwQcIssueDetails and Personal Findings
        [Authorize(Roles = "QCHEAD,QC,SA")]

        public ActionResult AddMoreIssues(string projectId)
        {

            // vmSwQcSpecification.CmnUserModels = _repository.GetActiveQc();
            var vmSwQcSpecification = new VmSwQcSpecificationModified();
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            vmSwQcSpecification.SwQcAssignModels = _repository.GetProjectListForQcInchargeToQcAssign(userId);
            vmSwQcSpecification.ProjectMasterModelsList = _repository.GetProjectListForFieldTest();
            vmSwQcSpecification.SwQcInchargeAssignModels = _repository.GetSwQcInchargeAssignModelsForFieldTest();
            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select One" } };

            var query = (from qcAssign in vmSwQcSpecification.SwQcAssignModels
                         join inchargeAssign in vmSwQcSpecification.SwQcInchargeAssignModels on qcAssign.SwQcInchargeAssignId equals inchargeAssign.SwQcInchargeAssignId
                         join master in vmSwQcSpecification.ProjectMasterModelsList on qcAssign.ProjectMasterId equals master.ProjectMasterId
                         where
                             qcAssign.Status == "ASSIGNED"
                          && qcAssign.SwQcUserId == @ViewBag.UserInfo.CmnUserId

                         select new
                         {
                             master.ProjectMasterId,
                             master.ProjectName,
                             master.OrderNuber,
                             master.OrderNumberOrdinal,
                             qcAssign.SwQcInchargeAssignId,
                             inchargeAssign.SwQcInchargeUserId,
                             qcAssign.SwQcAssignId,
                             qcAssign.SwQcUserId
                         }).ToList();


            foreach (var t in query)
            {
                String selectedValue = t.ProjectMasterId + "," + t.SwQcInchargeAssignId + "," + t.SwQcAssignId + "," + t.SwQcUserId;

                selectListItems.Add(new SelectListItem
                {
                    Value = selectedValue,
                    Text = t.ProjectName
                });

            }
            ViewBag.CombinedIds = selectListItems;



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

                    var match1 = obj1.Split(',');

                    for (var i = 0; i < match1.Length; i++)
                    {
                        Console.Write("<br /> Element " + i + " of the array is: " + match1[i]);

                        pro_id1 = match1[0];
                        swQcIncharge_id1 = match1[1];
                        swQcAssign_id1 = match1[2];

                    }
                    Console.Write("pro_id1 :" + pro_id1);
                    Console.Write("swQcIncharge_id1 :" + swQcIncharge_id1);
                    Console.Write("swQcAssign_id1 :" + swQcAssign_id1);


                    long pMasterId;
                    long.TryParse(pro_id1, out pMasterId);

                    long pSwQcInId;
                    long.TryParse(swQcIncharge_id1, out pSwQcInId);

                    long pSwQcAssignId;
                    long.TryParse(swQcAssign_id1, out pSwQcAssignId);

                    vmSwQcSpecification.SwQcProjectWiseIssueViewModels =
                        _repository.GetSwQcProjectWiseIssueViewModelss(pMasterId, pSwQcInId, pSwQcAssignId);

                    if (vmSwQcSpecification.SwQcProjectWiseIssueViewModels.Any())
                    {

                        foreach (
                            SwQcProjectWiseIssueViewModel model in vmSwQcSpecification.SwQcProjectWiseIssueViewModels)
                        {

                            if (model.UploadedFile != null)
                            {
                                var urls = model.UploadedFile.Split('|').ToList();
                                for (int i = 0; i < urls.Count; i++)
                                {
                                    //old one// model.ScreenShotGetUrl1.Add(Path.GetFileName(urls[i]));                          
                                    var last = urls[i].Split('-').Last();
                                    model.UploadedFileGetUrl1.Add(Path.GetFileName(last));
                                }
                            }

                        }
                    }

                }
            }
            vmSwQcSpecification.CombinedProjectId = projectId;

            //return RedirectToAction("SwQcSpecification", new { projectId = swQcSpecification.ProjectMasterModel.ProjectMasterId, AssignId = swQcSpecification.AssignId, tabName = swQcSpecification.Tabname, projectType = swQcSpecification.projectType });


            return View(vmSwQcSpecification);
        }

        [HttpPost]
        public ActionResult AddMoreIssues(List<SwQcProjectWiseIssueViewModel> issueList, String pro_id, String swQcIncharge_id, String swQcAssign_id, String projectId)
        {

            issueList = issueList.Where(x => x.IsRemoved == 0).ToList();
            long userId = Convert.ToInt64(User.Identity.Name);
            if (ModelState.IsValid)
            {
                long pMasterId;
                long.TryParse(pro_id, out pMasterId);

                long pSwQcInId;
                long.TryParse(swQcIncharge_id, out pSwQcInId);

                long pSwQcAssignId;
                long.TryParse(swQcAssign_id, out pSwQcAssignId);

                var manager = new FileManager();

                foreach (var swQcProWiseModel in issueList)
                {
                    if (swQcProWiseModel.File.Count() > 0 && swQcProWiseModel.File != null)
                    {
                        var res = manager.Upload1(pMasterId, "SwQCRaisedIssuesImage",
                            "QCRaisedIssuesImage", swQcProWiseModel.File);
                        swQcProWiseModel.UploadedFile = swQcProWiseModel.UploadedFile == null ? res : swQcProWiseModel.UploadedFile + "|" + res;

                    }

                }
                _repository.SaveProjectWiseIssues(issueList, pMasterId, pSwQcInId, pSwQcAssignId);

            }
            // return View();
            //return RedirectToAction("SwQcSpecification", new { projectId = swQcSpecification.ProjectMasterModel.ProjectMasterId, AssignId = swQcSpecification.AssignId, tabName = swQcSpecification.Tabname, projectType = swQcSpecification.projectType });


            return RedirectToAction("AddMoreIssues", new { projectId = pro_id + ',' + swQcIncharge_id + ',' + swQcAssign_id + ',' + userId });


        }
        [Authorize(Roles = "QCHEAD,QC,SA")]
        public ActionResult SwQcsTotalIssueDetails(string projectId)
        {
            var vmSwQcSpecification = new VmSwQcSpecificationModified();
            long userId = Convert.ToInt64(User.Identity.Name);

            long proId;
            long.TryParse(projectId, out proId);

            vmSwQcSpecification.SwQcAssignsFromQcHeadModels = _repository.GetProjectListForQcHeadToQcAssign(userId);
            vmSwQcSpecification.SwQcHeadAssignsFromPmModels = _repository.GetSwQcHeadAssignModelsFromPmForIssue();
            vmSwQcSpecification.ProjectMasterModelsList = _repository.GetProjectListForSwQc(userId);

            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);

            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select One" } };

            var query = (from qcAssign in vmSwQcSpecification.ProjectMasterModelsList
                         //where qcAssign.SwQcUserId == userId 
                         select new
                         {
                             qcAssign.ProjectMasterId,
                             qcAssign.ProjectName,
                             qcAssign.OrderNuber,
                             qcAssign.OrderNumberOrdinal,
                             qcAssign.SwQcHeadAssignId,
                             qcAssign.SwQcAssignId,
                             qcAssign.SwQcUserId
                         }).ToList();

            foreach (var t in query)
            {
                String selectedValue = t.ProjectMasterId + "," + t.SwQcHeadAssignId + "," + t.SwQcAssignId + "," + t.SwQcUserId;
                selectListItems.Add(new SelectListItem
                {
                    Value = selectedValue,
                    Text = t.ProjectName
                });
            }
            ViewBag.CombinedIds = selectListItems;

            ///////////////For load issues list/////////
            String pro_id1 = null;
            String swQcIncharge_id1 = null;
            String swQcAssign_id1 = null;
            String usersIds = null;
            string obj1 = null;
            obj1 = projectId;
            if (obj1 != null)
            {
                if (obj1 != "")
                {

                    //String pro_id1 = null;
                    //String swQcIncharge_id1 = null;
                    //String swQcAssign_id1 = null;
                    //String usersIds = null;

                    var match1 = obj1.Split(',');

                    for (var i = 0; i < match1.Length; i++)
                    {
                        Console.Write("<br /> Element " + i + " of the array is: " + match1[i]);

                        pro_id1 = match1[0];
                        swQcIncharge_id1 = match1[1];
                        swQcAssign_id1 = match1[2];
                        usersIds = match1[3];

                    }
                    Console.Write("pro_id1 :" + pro_id1);
                    Console.Write("swQcIncharge_id1 :" + swQcIncharge_id1);
                    Console.Write("swQcAssign_id1 :" + swQcAssign_id1);
                    Console.Write("usersIds :" + usersIds);

                    long pMasterId;
                    long.TryParse(pro_id1, out pMasterId);

                    long pSwQcInId;
                    long.TryParse(swQcIncharge_id1, out pSwQcInId);

                    long pSwQcAssignId;
                    long.TryParse(swQcAssign_id1, out pSwQcAssignId);

                    long pUsers;
                    long.TryParse(usersIds, out pUsers);
                    var fileManager = new FileManager();
                    //
                    //  var selectListItemsForRef = new List<SelectListItem>();
                    var selectListItemsForRef = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT" } };
                    List<SwQcTestPhaseModel> list = _repository.GetSwQcTestPhase() ??
                                                             new List<SwQcTestPhaseModel>();
                    selectListItemsForRef.AddRange(list.Select(p => new SelectListItem { Value = p.TestPhaseName, Text = p.TestPhaseName }));
                    ViewBag.ddlReferenceListForModal = selectListItemsForRef;
                    ////


                    vmSwQcSpecification.ProjectDetailsForSwQcModels = _repository.GetProjectDetailsForSwQc(userId, pMasterId, pSwQcInId);

                    vmSwQcSpecification.SwQcIssueDetailModels =
                        _repository.GetSwQcIssueDetails(pMasterId, pSwQcInId, pSwQcAssignId);

                    //if (vmSwQcSpecification.SwQcIssueDetailModels.Any())
                    //{

                    //    foreach (SwQcIssueDetailModel model in vmSwQcSpecification.SwQcIssueDetailModels)
                    //    {

                    //        if (model.UploadedFile != null)
                    //        {
                    //            var urls = model.UploadedFile.Split('|').ToList();
                    //            for (int i = 0; i < urls.Count; i++)
                    //            {
                    //                var last = urls[i].Split('-').Last();
                    //                model.UploadedFileGetUrl1.Add(Path.GetFileName(last));
                    //            }
                    //        }

                    //    }
                    //}
                    if (vmSwQcSpecification.SwQcIssueDetailModels.Any())
                    {
                        foreach (SwQcIssueDetailModel model in vmSwQcSpecification.SwQcIssueDetailModels)
                        {
                            if (model.UploadedFile != null)
                            {
                                var urls = model.UploadedFile.Split('|').ToList();
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

                }
            }
            var pro = pro_id1 + "," + swQcIncharge_id1 + "," + swQcAssign_id1 + "," + usersIds;
            vmSwQcSpecification.CombinedProjectId = pro;
            //return RedirectToAction("SwQcSpecification", new { projectId = swQcSpecification.ProjectMasterModel.ProjectMasterId, AssignId = swQcSpecification.AssignId, tabName = swQcSpecification.Tabname, projectType = swQcSpecification.projectType });
            return View(vmSwQcSpecification);
        }

        [HttpPost]
        public ActionResult SwQcsTotalIssueDetails(List<SwQcIssueDetailModel> issueList, List<SwQcIssueDetailModel> issueList1, String pro_id, string issueChk, string fileChk, String swQcIncharge_id, String swQcAssign_id, String projectId)
        {

            issueList = issueList.Where(x => x.IsRemoved == 0).ToList();
            long userId = Convert.ToInt64(User.Identity.Name);
            if (ModelState.IsValid)
            {
                long pMasterId;
                long.TryParse(pro_id, out pMasterId);

                long pSwQcInId;
                long.TryParse(swQcIncharge_id, out pSwQcInId);

                long pSwQcAssignId;
                long.TryParse(swQcAssign_id, out pSwQcAssignId);

                bool issuesChk;
                bool.TryParse(issueChk, out issuesChk);

                bool filesChk;
                bool.TryParse(fileChk, out filesChk);

                var manager = new FileManager();

                foreach (var swQcIssueDetails in issueList)
                {
                    if (swQcIssueDetails.File.Count() > 0 && swQcIssueDetails.File != null)
                    {
                        var res = manager.Upload3(pMasterId, pSwQcInId, pSwQcAssignId, "SwQcsTotalIssueDetails",
                            "SwQcsIssuesImage", swQcIssueDetails.File);
                        swQcIssueDetails.UploadedFile = swQcIssueDetails.UploadedFile == null ? res : swQcIssueDetails.UploadedFile + "|" + res;

                    }

                }

                foreach (var swQcIssueDetails in issueList1)
                {
                    if (swQcIssueDetails.File.Count() > 0 && swQcIssueDetails.File != null)
                    {
                        var res = manager.Upload3(pMasterId, pSwQcInId, pSwQcAssignId, "SwQcsTotalIssueDetails",
                            "SwQcsIssuesImage", swQcIssueDetails.File);
                        swQcIssueDetails.UploadedFile = swQcIssueDetails.UploadedFile == null ? res : swQcIssueDetails.UploadedFile + "|" + res;

                    }

                    if (swQcIssueDetails.RefernceModules != null)
                    {
                        //!string.IsNullOrWhiteSpace(cmBatteryVmModels[index].IqcCompleteBSmt) ? cmBatteryVmModels[index].IqcCompleteBSmt + ", " + queryProduction.ProjectName;

                        foreach (var resf in swQcIssueDetails.RefernceModules)
                        {
                            swQcIssueDetails.RefernceModules1 = swQcIssueDetails.RefernceModules1 == null ? resf : swQcIssueDetails.RefernceModules1 + "," + resf;

                        }

                    }


                }
                //  _repository.SaveProjectWiseIssues(issueList, pMasterId, pSwQcInId, pSwQcAssignId);
                _repository.SaveSwQcProjectIssueDetails(issueList, issueList1, pMasterId, issuesChk, filesChk, pSwQcInId, pSwQcAssignId);
            }

            //return RedirectToAction("SwQcSpecification", new { projectId = swQcSpecification.ProjectMasterModel.ProjectMasterId, AssignId = swQcSpecification.AssignId, tabName = swQcSpecification.Tabname, projectType = swQcSpecification.projectType });
            return RedirectToAction("SwQcsTotalIssueDetails", new { projectId = pro_id + ',' + swQcIncharge_id + ',' + swQcAssign_id + ',' + userId });
        }

        [Authorize(Roles = "QCHEAD,QC,SA")]
        public ActionResult SwQcPersonalUseFindingsIssueDetails(string projectId)
        {
            var vmSwQcSpecification = new VmSwQcSpecificationModified();
            long userId = Convert.ToInt64(User.Identity.Name);

            long proId;
            long.TryParse(projectId, out proId);

            vmSwQcSpecification.SwQcHeadAssignsFromPmModels = _repository.GetProjectListForMPVersionSwQc(userId);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);

            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select One" } };

            var query = (from qcAssign in vmSwQcSpecification.SwQcHeadAssignsFromPmModels
                         select new
                         {
                             qcAssign.ProjectMasterId,
                             qcAssign.ProjectName,
                             qcAssign.OrderNumber,
                             qcAssign.OrderNumberOrdinal,
                             qcAssign.SwQcHeadAssignId,
                             qcAssign.ProjectPmAssignId

                         }).ToList();

            foreach (var t in query)
            {
                String selectedValue = t.ProjectMasterId + "," + t.SwQcHeadAssignId + "," + t.ProjectPmAssignId + "," + userId;
                selectListItems.Add(new SelectListItem
                {
                    Value = selectedValue,
                    Text = t.ProjectName
                });
            }
            ViewBag.CombinedIds = selectListItems;

            ///////////////For load issues list/////////
            string obj1 = null;
            obj1 = projectId;
            if (obj1 != null)
            {
                if (obj1 != "")
                {

                    String pro_id1 = null;
                    String swQcIncharge_id1 = null;
                    String projectPmAssignId = null;

                    var match1 = obj1.Split(',');

                    for (var i = 0; i < match1.Length; i++)
                    {
                        Console.Write("<br /> Element " + i + " of the array is: " + match1[i]);

                        pro_id1 = match1[0];
                        swQcIncharge_id1 = match1[1];
                        projectPmAssignId = match1[2];

                    }
                    Console.Write("pro_id1 :" + pro_id1);
                    Console.Write("swQcIncharge_id1 :" + swQcIncharge_id1);
                    Console.Write("projectPmAssignId :" + projectPmAssignId);


                    long pMasterId;
                    long.TryParse(pro_id1, out pMasterId);

                    long pSwQcInId;
                    long.TryParse(swQcIncharge_id1, out pSwQcInId);

                    long pPmAssignId;
                    long.TryParse(projectPmAssignId, out pPmAssignId);


                    vmSwQcSpecification.ProjectDetailsForSwQcModels = _repository.GetProjectDetailsForSwQcPersonalFindings(userId, pMasterId, pSwQcInId);

                    vmSwQcSpecification.SwQcPersonalUseFindingsIssueDetailModels =
                        _repository.GetSwQcPersonalFindingIssueDetails(pMasterId, pSwQcInId, pPmAssignId);

                    if (vmSwQcSpecification.SwQcPersonalUseFindingsIssueDetailModels.Any())
                    {

                        foreach (SwQcPersonalUseFindingsIssueDetailModel model in vmSwQcSpecification.SwQcPersonalUseFindingsIssueDetailModels)
                        {

                            if (model.UploadedFile != null)
                            {
                                var urls = model.UploadedFile.Split('|').ToList();
                                for (int i = 0; i < urls.Count; i++)
                                {
                                    var last = urls[i].Split('-').Last();
                                    model.UploadedFileGetUrl1.Add(Path.GetFileName(last));
                                }
                            }

                        }
                    }

                }
            }
            vmSwQcSpecification.CombinedProjectId = projectId;
            //return RedirectToAction("SwQcSpecification", new { projectId = swQcSpecification.ProjectMasterModel.ProjectMasterId, AssignId = swQcSpecification.AssignId, tabName = swQcSpecification.Tabname, projectType = swQcSpecification.projectType });
            return View(vmSwQcSpecification);
        }

        [HttpPost]
        public ActionResult SwQcPersonalUseFindingsIssueDetails(List<SwQcPersonalUseFindingsIssueDetailModel> issueList, List<SwQcPersonalUseFindingsIssueDetailModel> issueList1, String pro_id, string issueChk, string fileChk, String swQcIncharge_id, String projectPmAssignId, String projectId)
        {
            issueList = issueList.Where(x => x.IsRemoved == 0).ToList();
            long userId = Convert.ToInt64(User.Identity.Name);
            if (ModelState.IsValid)
            {
                long pMasterId;
                long.TryParse(pro_id, out pMasterId);

                long pSwQcInId;
                long.TryParse(swQcIncharge_id, out pSwQcInId);

                long pPmAssignId;
                long.TryParse(projectPmAssignId, out pPmAssignId);

                bool issuesChk;
                bool.TryParse(issueChk, out issuesChk);

                bool filesChk;
                bool.TryParse(fileChk, out filesChk);

                var manager = new FileManager();

                foreach (var swQcIssueDetails in issueList)
                {
                    if (swQcIssueDetails.File.Count() > 0 && swQcIssueDetails.File != null)
                    {
                        var res = manager.Upload3(pMasterId, pSwQcInId, pPmAssignId, "SwQcPersonalUseFindingsIssueDetails",
                            "SwQcPersonalUseFindingsIssueImage", swQcIssueDetails.File);
                        swQcIssueDetails.UploadedFile = swQcIssueDetails.UploadedFile == null ? res : swQcIssueDetails.UploadedFile + "|" + res;

                    }

                }

                foreach (var swQcIssueDetails in issueList1)
                {
                    if (swQcIssueDetails.File.Count() > 0 && swQcIssueDetails.File != null)
                    {
                        var res = manager.Upload3(pMasterId, pSwQcInId, pPmAssignId, "SwQcPersonalUseFindingsIssueDetails",
                            "SwQcPersonalUseFindingsIssueDetails", swQcIssueDetails.File);
                        swQcIssueDetails.UploadedFile = swQcIssueDetails.UploadedFile == null ? res : swQcIssueDetails.UploadedFile + "|" + res;

                    }

                    if (swQcIssueDetails.RefernceModules != null)
                    {
                        foreach (var resf in swQcIssueDetails.RefernceModules)
                        {
                            swQcIssueDetails.RefernceModules1 = swQcIssueDetails.RefernceModules1 == null ? resf : swQcIssueDetails.RefernceModules1 + "," + resf;

                        }
                    }
                }

                _repository.SaveSwQcPersonalUseFindingsIssueDetails(issueList, issueList1, pMasterId, issuesChk, filesChk, pSwQcInId, pPmAssignId);
            }

            //return RedirectToAction("SwQcSpecification", new { projectId = swQcSpecification.ProjectMasterModel.ProjectMasterId, AssignId = swQcSpecification.AssignId, tabName = swQcSpecification.Tabname, projectType = swQcSpecification.projectType });
            return RedirectToAction("SwQcPersonalUseFindingsIssueDetails", new { projectId = pro_id + ',' + swQcIncharge_id + ',' + projectPmAssignId + ',' + userId });
        }
        [Authorize(Roles = "QCHEAD,SA")]
        public ActionResult SwQcPersonalUseFindingsApprove(string projectId)
        {
            var vmSwQcSpecification = new VmSwQcSpecificationModified();
            long userId = Convert.ToInt64(User.Identity.Name);

            long proId;
            long.TryParse(projectId, out proId);

            vmSwQcSpecification.SwQcHeadAssignsFromPmModels = _repository.GetProjectListForMPVersionSwQc(userId);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);

            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select One" } };

            var query = (from qcAssign in vmSwQcSpecification.SwQcHeadAssignsFromPmModels
                         select new
                         {
                             qcAssign.ProjectMasterId,
                             qcAssign.ProjectName,
                             qcAssign.OrderNumber,
                             qcAssign.OrderNumberOrdinal,
                             qcAssign.SwQcHeadAssignId,
                             qcAssign.ProjectPmAssignId

                         }).ToList();

            foreach (var t in query)
            {
                String selectedValue = t.ProjectMasterId + "," + t.SwQcHeadAssignId + "," + t.ProjectPmAssignId + "," + userId;
                selectListItems.Add(new SelectListItem
                {
                    Value = selectedValue,
                    Text = t.ProjectName
                });
            }
            ViewBag.CombinedIds = selectListItems;

            ///////////////For load issues list/////////
            string obj1 = null;
            obj1 = projectId;
            if (obj1 != null)
            {
                if (obj1 != "")
                {

                    String pro_id1 = null;
                    String swQcIncharge_id1 = null;
                    String projectPmAssignId = null;

                    var match1 = obj1.Split(',');

                    for (var i = 0; i < match1.Length; i++)
                    {
                        Console.Write("<br /> Element " + i + " of the array is: " + match1[i]);

                        pro_id1 = match1[0];
                        swQcIncharge_id1 = match1[1];
                        projectPmAssignId = match1[2];

                    }
                    Console.Write("pro_id1 :" + pro_id1);
                    Console.Write("swQcIncharge_id1 :" + swQcIncharge_id1);
                    Console.Write("projectPmAssignId :" + projectPmAssignId);


                    long pMasterId;
                    long.TryParse(pro_id1, out pMasterId);

                    long pSwQcInId;
                    long.TryParse(swQcIncharge_id1, out pSwQcInId);

                    long pPmAssignId;
                    long.TryParse(projectPmAssignId, out pPmAssignId);


                    vmSwQcSpecification.ProjectDetailsForSwQcModels = _repository.GetProjectDetailsForSwQcPersonalFindings(userId, pMasterId, pSwQcInId);

                    vmSwQcSpecification.SwQcPersonalUseFindingsIssueDetailModels =
                        _repository.GetSwQcPersonalFindingIssueDetails(pMasterId, pSwQcInId, pPmAssignId);

                    if (vmSwQcSpecification.SwQcPersonalUseFindingsIssueDetailModels.Any())
                    {

                        foreach (SwQcPersonalUseFindingsIssueDetailModel model in vmSwQcSpecification.SwQcPersonalUseFindingsIssueDetailModels)
                        {

                            if (model.UploadedFile != null)
                            {
                                var urls = model.UploadedFile.Split('|').ToList();
                                for (int i = 0; i < urls.Count; i++)
                                {
                                    var last = urls[i].Split('-').Last();
                                    model.UploadedFileGetUrl1.Add(Path.GetFileName(last));
                                }
                            }

                        }
                    }

                }
            }
            vmSwQcSpecification.CombinedProjectId = projectId;
            //return RedirectToAction("SwQcSpecification", new { projectId = swQcSpecification.ProjectMasterModel.ProjectMasterId, AssignId = swQcSpecification.AssignId, tabName = swQcSpecification.Tabname, projectType = swQcSpecification.projectType });
            return View(vmSwQcSpecification);
        }

        [HttpPost]
        public JsonResult SaveSwQcSubmittedProjectToQcHead(String projectMasterId)
        {
            var vmSwQcSpecification = new VmSwQcSpecificationModified();

            long userId = Convert.ToInt64(User.Identity.Name);

            string obj1 = null;
            obj1 = projectMasterId;
            if (obj1 != null)
            {
                if (obj1 != "")
                {

                    String proId1 = null;
                    String swQcInchargeId1 = null;
                    String swQcAssignId1 = null;

                    var match1 = obj1.Split(',');

                    for (var i = 0; i < match1.Length; i++)
                    {
                        Console.Write("<br /> Element " + i + " of the array is: " + match1[i]);

                        proId1 = match1[0];
                        swQcInchargeId1 = match1[1];
                        swQcAssignId1 = match1[2];

                    }
                    Console.Write("pro_id1 :" + proId1);
                    Console.Write("swQcIncharge_id1 :" + swQcInchargeId1);
                    Console.Write("swQcAssign_id1 :" + swQcAssignId1);


                    long pMasterId;
                    long.TryParse(proId1, out pMasterId);

                    long pSwQcInId;
                    long.TryParse(swQcInchargeId1, out pSwQcInId);

                    long pSwQcAssignId;
                    long.TryParse(swQcAssignId1, out pSwQcAssignId);


                    _repository.SaveSwQcSubmittedProjectToQcHead(pMasterId, pSwQcInId, pSwQcAssignId, userId);
                }
            }
            vmSwQcSpecification.CombinedProjectId = projectMasterId;

            // return View(vmSwQcSpecification);

            return new JsonResult { Data = "OK", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [Authorize(Roles = "QCHEAD,QC,SA")]
        public ActionResult SwQcsIssueDetailsSendToChaina(string projectId, string swVersionNo, string projectOrder, string moduleName, string testPhases)
        {
            var vmSwQcSpecification = new VmSwQcSpecificationModified();
            long userId = Convert.ToInt64(User.Identity.Name);

            //var proNames = projectId.Trim();

            int softVersionNo;
            int.TryParse(swVersionNo, out softVersionNo);

            int projectOrders;
            int.TryParse(projectOrder, out projectOrders);

            vmSwQcSpecification.ProjectMasterModelsList = _repository.GetProjectListForSwQcHead();

            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Select One" } };

            var query = (from master in vmSwQcSpecification.ProjectMasterModelsList
                         // where master.SwQcHeadStatus != "NEW" || master.SwQcHeadStatus != "INACTIVE"
                         select new
                         {
                             //  master.ProjectMasterId,
                             master.ProjectName,
                             //master.OrderNuber,
                             //master.SwQcHeadAssignId,
                             //master.SoftwareVersionName,
                             //master.SoftwareVersionNo
                         }).ToList();

            foreach (var t in query)
            {
                // String selectedValue = t.ProjectMasterId +  "," + t.SoftwareVersionNo + "," + t.OrderNuber+","+t.ProjectName;

                selectListItems.Add(new SelectListItem
                {
                    Value = t.ProjectName.Trim(),
                    Text = t.ProjectName.Trim()
                });
            }
            ViewBag.CombinedIds = selectListItems;

            var selectListItemsForRef = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT" } };
            List<SwQcTestPhaseModel> list = _repository.GetSwQcTestPhase() ??
                                                     new List<SwQcTestPhaseModel>();
            selectListItemsForRef.AddRange(list.Select(p => new SelectListItem { Value = p.TestPhaseName, Text = p.TestPhaseName }));
            ViewBag.ddlReferenceListForModal = selectListItemsForRef;
            ////
            ////ModuleList///
            vmSwQcSpecification.SwQcIssueCategoryModels = _repository.GetIssueCategory();

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
            //Test phase//
            var selectListItemsTestPhase = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Select Test Phase" } };
            List<SwQcTestPhaseModel> listTestPhase = _repository.GetSwQcTestPhaseForSupp() ??
                                                     new List<SwQcTestPhaseModel>();
            selectListItemsTestPhase.AddRange(listTestPhase.Select(p => new SelectListItem { Value = p.TestPhaseID.ToString(), Text = p.TestPhaseName }));
            ViewBag.CombinedIdsForTestPhase = selectListItemsTestPhase;
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

                        //pro_id1 = match1[0];
                        //swQcIncharge_id1 = match1[1];
                        //swQcAssign_id1 = match1[2];
                        projectName = match1[0];

                    }
                    //  Console.Write("pro_id1 :" + pro_id1);
                    // Console.Write("swQcIncharge_id1 :" + swQcIncharge_id1);
                    // Console.Write("swQcAssign_id1 :" + swQcAssign_id1);


                    //long pMasterId;
                    //long.TryParse(pro_id1, out pMasterId);

                    //long pSwQcInId;
                    //long.TryParse(swQcIncharge_id1, out pSwQcInId);

                    //long pSwQcAssignId;
                    //long.TryParse(swQcAssign_id1, out pSwQcAssignId);

                    vmSwQcSpecification.SwQcAssignsFromQcHeadModels = _repository.GetSwQcsAssignsInfo(projectName, projectOrders, softVersionNo, testPhases);

                    vmSwQcSpecification.SwQcIssueDetailModels = _repository.GetSwQcIssueDetailsForSupplier(projectName, moduleName, projectOrders, softVersionNo, testPhases);
                    var fileManager = new FileManager();
                    if (vmSwQcSpecification.SwQcIssueDetailModels.Any())
                    {
                        foreach (SwQcIssueDetailModel model in vmSwQcSpecification.SwQcIssueDetailModels)
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
        public JsonResult UpdateSwQcIssueDetailModelForApprove(string objArr)
        {
            SwQcIssueDetailModel results = JsonConvert.DeserializeObject<SwQcIssueDetailModel>(objArr);
            Console.Write("result :" + results);
            //  var approveStage1 = _repository.UpdateSwQcIssueDetailModelForApprove(approveStage);

            var saveIncentive = "0";

            if (results.SwQcIssueId != 0)
            {
                saveIncentive = _repository.UpdateSwQcIssueDetailModelForApprove(results);
            }


            return Json(new { data = saveIncentive }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateSwQcIssueDetailModelForDecline(string objArr)
        {
            SwQcIssueDetailModel results = JsonConvert.DeserializeObject<SwQcIssueDetailModel>(objArr);
            Console.Write("result :" + results);

            var saveIncentive = "0";

            if (results.SwQcIssueId != 0)
            {
                saveIncentive = _repository.UpdateSwQcIssueDetailModelForDecline(results);
            }
            return Json(new { data = saveIncentive }, JsonRequestBehavior.AllowGet);
        }
        //ForwardFirstVersionIssueToSecondVersion
        [HttpPost]
        public JsonResult UpdateNewInnovationModelForApprove(string objArr)
        {
            SwQcNewInnovationModel results = JsonConvert.DeserializeObject<SwQcNewInnovationModel>(objArr);
            Console.Write("result :" + results);

            var saveIncentive = "0";

            if (results.NewInnovationId != 0)
            {
                saveIncentive = _repository.UpdateNewInnovationModelForApprove(results);
            }


            return Json(new { data = saveIncentive }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdateNewInnovationModelForDecline(string objArr)
        {
            SwQcNewInnovationModel results = JsonConvert.DeserializeObject<SwQcNewInnovationModel>(objArr);
            Console.Write("result :" + results);

            var saveIncentive = "0";

            if (results.NewInnovationId != 0)
            {
                saveIncentive = _repository.UpdateNewInnovationModelForDecline(results);
            }
            return Json(new { data = saveIncentive }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdatePersonalFindingsForApprove(string objArr)
        {
            SwQcPersonalUseFindingsIssueDetailModel results = JsonConvert.DeserializeObject<SwQcPersonalUseFindingsIssueDetailModel>(objArr);
            Console.Write("result :" + results);

            var saveIncentive = "0";

            if (results.SwQcPrUseFindId != 0)
            {
                saveIncentive = _repository.UpdatePersonalFindingsForApprove(results);
            }


            return Json(new { data = saveIncentive }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdatePersonalFindingsForDecline(string objArr)
        {
            SwQcPersonalUseFindingsIssueDetailModel results = JsonConvert.DeserializeObject<SwQcPersonalUseFindingsIssueDetailModel>(objArr);
            Console.Write("result :" + results);

            var saveIncentive = "0";

            if (results.SwQcPrUseFindId != 0)
            {
                saveIncentive = _repository.UpdatePersonalFindingsForDecline(results);
            }
            return Json(new { data = saveIncentive }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateSwQcIssueDetailModelForSupplier(SwQcIssueDetailModel supplierUpdate)
        {

            //var supplierUpdate1 = _repository.UpdateSwQcIssueDetailModelForSupplier(supplierUpdate);

            //return Json(new { data = supplierUpdate1 }, JsonRequestBehavior.AllowGet);
            // return View(supplierUpdate);

            string supplierUpdate1 = null;
            bool isExist = false;
            if (supplierUpdate != null)
            {
                isExist = _repository.GetSupplierFeedbackData(supplierUpdate);

            }
            supplierUpdate1 = _repository.UpdateSwQcIssueDetailModelForSupplier(supplierUpdate);

            if (isExist)
            {
                supplierUpdate1 = "Already Supplier Feeadback Generated.";

            }
            //else
            //{
            //    supplierUpdate1 = _repository.UpdateSwQcIssueDetailModelForSupplier(supplierUpdate);
            //}

            //return Json(new { data = supplierUpdate1 + "," + isExist }, JsonRequestBehavior.AllowGet);
            return Json(new { data = supplierUpdate1 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AllApproveForChaina(string objArr)
        {
            List<SwQcIssueDetailModel> results = JsonConvert.DeserializeObject<List<SwQcIssueDetailModel>>(objArr);
            Console.Write("result :" + results);

            var saveIncentive = "0";

            if (results.Count != 0)
            {
                //  saveIncentive = _repository.UpdateSwQcIssueDetailModelForApprove(results);
                saveIncentive = _repository.AllApproveForChaina(results);
            }

            return Json(new { data = saveIncentive }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SwQcIssueDelete(SwQcIssueDetailModel supplierUpdate)
        {
            var vmSwQcSpecification = new VmSwQcSpecificationModified();
            var saveIncentive = _repository.SwQcIssueDelete(supplierUpdate);

            return new JsonResult { Data = saveIncentive, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult GetRefernceModule()
        {
            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT" } };
            // List<SelectListItem> list = new List<SelectListItem>();
            String phoneType = "Smart";
            List<String> moduleList = _repository.GetIssueModules(phoneType);
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
        [HttpPost]
        public JsonResult GetRefernceRole()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            List<String> moduleList = _repository.GetAllRoles();
            foreach (var module in moduleList)
            {
                list.Add(new SelectListItem
                {
                    Value = module,
                    Text = module
                });
            }
            return Json(new { list = list }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetSwQcProjectWiseIssueViewModels(string pro_id1, string swQcIncharge_id1, string swQcAssign_id1, string projectMasterId1)
        {
            var vmSwQcSpecification1 = new VmSwQcSpecificationModified();
            // if (ModelState.IsValid)
            // {
            long pMasterId;
            long.TryParse(pro_id1, out pMasterId);
            long pSwQcInId;
            long.TryParse(swQcIncharge_id1, out pSwQcInId);
            long pSwQcAssignId;
            long.TryParse(swQcAssign_id1, out pSwQcAssignId);

            var manager = new FileManager();
            //     var sWQcInchargeSubmitToPmResult = _repository.SoftWareQcInchargeToPm(pMasterId, proComment, userId, pSwQcInId);
            //    return Json(sWQcInchargeSubmitToPmResult, JsonRequestBehavior.AllowGet);
            //}
            //return Json(new { result = "Redirect", url = Url.Action("Software", "SwInchargeProjectSubmit") });
            vmSwQcSpecification1.SwQcProjectWiseIssueViewModels = _repository.GetSwQcProjectWiseIssueViewModelss(pMasterId, pSwQcInId, pSwQcAssignId);
            // var sWQcInchargeSubmitToPmResult = _repository.GetSwQcProjectWiseIssueViewModelss(pMasterId, pSwQcInId, pSwQcAssignId);
            return Json(vmSwQcSpecification1.SwQcProjectWiseIssueViewModels, JsonRequestBehavior.AllowGet);
            //  }
            //  return Json(new { result = "Redirect", url = Url.Action("Software", "AddMoreIssues") });
            //return vmSwQcSpecification1;
        }
        [Authorize(Roles = "QCHEAD,SA")]
        public ActionResult AssignForFieldTestFromQcHead(string projectId)
        {
            var vmSwQcSpecification = new AssignMuliplePersonViewModel();
            long userId = Convert.ToInt64(User.Identity.Name);

            List<CmnUserModel> list = _repository.GetActiveQc();
            vmSwQcSpecification.ddlAssignUsersList = list;

            List<SwQcTestPhaseModel> testPhaseList = _projectManagerRepository.GetSwQcTestPhasesForPm();
            ViewBag.ddlTestPhasesList = testPhaseList;

            vmSwQcSpecification.ProjectMasterModelsList = _repository.GetProjectListForFieldTestNew();

            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Select One" } };

            var query = (from master in vmSwQcSpecification.ProjectMasterModelsList
                         where master.SwQcHeadStatus != "INACTIVE"
                         select new
                         {
                             master.ProjectMasterId,
                             master.ProjectNames,
                             master.ProjectName,
                             master.OrderNuber,
                             master.SoftwareVersionName,
                             master.SoftwareVersionNo

                         }).ToList();

            foreach (var t in query)
            {
                String selectedValue = t.ProjectMasterId + "," + t.SoftwareVersionNo + "," + t.SoftwareVersionName + "," + t.OrderNuber + "," + t.ProjectNames;

                selectListItems.Add(new SelectListItem
                {
                    Value = selectedValue,
                    Text = t.ProjectName
                });
            }
            ViewBag.CombinedIds = selectListItems;
            vmSwQcSpecification.CombinedProjectId = projectId;


            string obj1 = null;
            obj1 = projectId;
            if (obj1 != null)
            {
                if (obj1 != "")
                {

                    String proId = null;
                    String softwareVerNo = null;
                    String proOrders = null;
                    String projectName = null;

                    var match1 = obj1.Split(',');

                    for (var i = 0; i < match1.Length; i++)
                    {
                        Console.Write("<br /> Element " + i + " of the array is: " + match1[i]);

                        proId = match1[0];
                        softwareVerNo = match1[1];
                        proOrders = match1[3];
                        projectName = match1[4];

                    }

                    long pMasterId;
                    long.TryParse(proId, out pMasterId);

                    long softwareVerNumber;
                    long.TryParse(softwareVerNo, out softwareVerNumber);

                    long proOrder;
                    long.TryParse(proOrders, out proOrder);

                    vmSwQcSpecification.PmQcAssignModels = _repository.GetProjectDetailsForQcFieldTest(pMasterId, softwareVerNumber, proOrder);
                    vmSwQcSpecification.SwQcAssignsFromQcHeadModels = _repository.GetAssignedProjectDetailsForQcFieldTest(pMasterId, softwareVerNumber, proOrder, projectName);
                }
            }

            return View(vmSwQcSpecification);
        }

        [NotificationActionFilter(ReceiverRoles = "QCHEAD,QC,PM,PS")]
        [HttpPost]
        public JsonResult AssignForFieldTestFromQcHead(String ProjectName, String ProjectMasterId, String ProjectPmAssignId, String SwInchargeAssignToQcComment, String[] multiple, String singleOne, String ApproxInchargeToQcDeliveryDate, String SoftwareVersionNo)
        {
            var _dbEntities = new CellPhoneProjectEntities();

            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name; ;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);

            long p_id = 0;
            long.TryParse(ProjectMasterId, out p_id);

            //----------MAIL Start-----------------------
            // var swQcInchargeproInfo = _dbEntities.SwQcHeadAssignsFromPms.FirstOrDefault(i => i.ProjectMasterId == p_id && i.SwQcHeadAssignId == sWQcInId_id && i.Status != "RECOMMENDED");
            // var tPhaseName = _dbEntities.SwQcTestPhases.FirstOrDefault(i => i.TestPhaseID == swQcInchargeproInfo.TestPhaseID);
            var proName = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == p_id);

            //---------------ends-----------------


            var notificationObject = new NotificationObject
            {
                ProjectId = p_id,
                //ToUser = multiple
            };
            if (multiple != null)
            {
                List<long> ids = new List<long>();
                string body = string.Empty;
                string assignedQc = string.Empty;
                foreach (string mId in multiple)
                {
                    notificationObject.ToUser = notificationObject.ToUser + mId + ",";
                    long qcIDs;
                    long.TryParse(mId, out qcIDs);
                    ids.Add(qcIDs);
                    var assignedUserName = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == qcIDs);
                    assignedQc = assignedQc + assignedUserName.UserFullName + ", ";
                }

                //assignedQc = assignedQc.TrimEnd(',');
                //body = "This is to inform you that a project has been assigned by  <b>" + user.UserFullName + " </b> for testing of Software QC issues.<br/><br/><br/> <br/>" + "Project : <b>" + proName.ProjectName + "</b> <br/>Assigned to : " + assignedQc + "<br/>Sample Type - " + swQcInchargeproInfo.ProjectManagerSampleType + "<br/>Sample Quantity - " + swQcInchargeproInfo.ProjectManagerSampleNo + "<br/>Test Phase Name - " + tPhaseName.TestPhaseName
                //    + "<br/>SoftwareVersion Name - " + swQcInchargeproInfo.SoftwareVersionName + "<br/>Software Version Number - " + swQcInchargeproInfo.SoftwareVersionNo;
                //var mailSendFromPms = new MailSendFromPms();
                //mailSendFromPms.SendMail(ids, new List<string>(new[] { "MM", "SA", "PS" }), "Software QC has been assigned for a New Project(" + proName.ProjectName + ")", body);


            }
            notificationObject.Message = "assigned for Checking Software Issues";

            String insert = _repository.SaveAssignForFieldTestFromQcHead(ProjectName, ProjectMasterId, ProjectPmAssignId, SwInchargeAssignToQcComment, multiple, singleOne, ApproxInchargeToQcDeliveryDate, SoftwareVersionNo);

            ViewBag.ControllerVariable = notificationObject;
            return new JsonResult { Data = "ok", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [Authorize(Roles = "QCHEAD,QC,SA")]
        public ActionResult SwQcNewInnovation()
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name; ;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);
            var vmSwQcSpecification = new VmSwQcSpecificationModified();

            vmSwQcSpecification.SwQcNewInnovationModels = _repository.GetSwQcNewInnovation();

            return View(vmSwQcSpecification);
        }

        [HttpPost]
        public ActionResult SwQcNewInnovation(List<SwQcNewInnovationModel> issueList)
        {
            issueList = issueList.Where(x => x.IsRemoved == 0).ToList();
            long userId = Convert.ToInt64(User.Identity.Name);
            if (ModelState.IsValid)
            {
                foreach (var swQcNewInno in issueList)
                {
                    if (swQcNewInno.RefernceModules != null)
                    {
                        foreach (var resf in swQcNewInno.RefernceModules)
                        {
                            swQcNewInno.RefernceModules1 = swQcNewInno.RefernceModules1 == null ? resf : swQcNewInno.RefernceModules1 + "," + resf;

                        }
                    }
                }
                _repository.SaveSwQcNewInnovation(issueList);

            }
            return RedirectToAction("SwQcNewInnovation");
        }

        //[HttpPost]
        //public JsonResult GetRefBy()
        //{
        //    List<SelectListItem> list = new List<SelectListItem>();
        //    List<CmnUserModel> users = _repository.GetActiveQc();
        //    foreach (var user in users)
        //    {
        //        list.Add(new SelectListItem
        //        {
        //            Value = user.CmnUserId.ToString(),
        //            Text = user.UserFullName + "_" + user.EmployeeCode
        //        });
        //    }
        //    return Json(new { list = list }, JsonRequestBehavior.AllowGet);
        //}
        #endregion

        #region Paused or Restart any project
        [Authorize(Roles = "QCHEAD,SA")]

        public ActionResult SwPauseOrRestartAssignedProject(VmSwQcSpecificationModified model)
        {
            long userId = Convert.ToInt64(User.Identity.Name);

            // long pMasterId;
            // long.TryParse(projectMasterId, out pMasterId);

            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            List<CmnUserModel> list = _repository.GetActiveQc();

            // ViewBag.delList = _repository.GetDeletedPersonNameList(projectId);
            model.CmnUserModels = list;
            // model.CmnUserModels = delList;

            // List<CmnUserModel> assLis = _repository.GetAssignQc(pMasterId);


            //model.CmnUserModels = assLis;

            // ViewBag.GetAssignsQCs = assLis;

            ViewBag.GetAssignedProjectToQCStatus = _repository.GetAssignedProjectToQCStatusForInchargeDashboard();
            // ViewBag.GetQCCompletedProjectStatus = _repository.GetQcCompletedProjectStatusForInchargeDashboard();
            return View(model);
        }

        [HttpPost]
        [NotificationActionFilter(ReceiverRoles = "QC,PM,MM,CM,PS")]
        public JsonResult SwPauseOrRestartAssignedProject(string projectMsterId, string projectHeadRemarks, string swqcInchargeId,
            string projectName, string pmAssignId)
        {
            var _dbEntities = new CellPhoneProjectEntities();
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name);

            if (userId > 0)
            {
                long pMasterId;
                long.TryParse(projectMsterId, out pMasterId);

                long pSwQcInId;
                long.TryParse(swqcInchargeId, out pSwQcInId);

                long pmHeadAssignId;
                long.TryParse(pmAssignId, out pmHeadAssignId);

                var swPauseOrRestartAssignedProjectResult = _repository.SaveSwPauseOrRestartAssignedProject(pMasterId, projectHeadRemarks, pSwQcInId, projectName, pmHeadAssignId);


                var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);
                var swQcInchargeproInfo = _dbEntities.SwQcHeadAssignsFromPms.FirstOrDefault(i => i.ProjectMasterId == pMasterId && i.SwQcHeadAssignId == pSwQcInId);
                var tPhaseName = _dbEntities.SwQcTestPhases.FirstOrDefault(i => i.TestPhaseID == swQcInchargeproInfo.TestPhaseID);

                var assignedUserName = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == swQcInchargeproInfo.Added);

                var pasueReason = _dbEntities.SwQcHeadStatusChangeActivities.OrderByDescending(i => i.ActivityId).FirstOrDefault(i => i.SwQcHeadAssignId == pSwQcInId);

                MailSendFromPms mailSendFromPms = new MailSendFromPms();
                mailSendFromPms.SendMail(new List<long>(new[] { assignedUserName.CmnUserId }), new List<string>(new[] { "PMHEAD", "MM", "CM","CMHEAD", "PS", "QCHEAD" }), "Project(" + projectName + ") has been paused by Software QC In charge",
                    "This is to inform you that, a project has been paused by Software QC In charge,  <b>" + user.UserFullName + "</b> because of an urgent issue. <br/><br/><br/><br/>Project : <b>" + projectName + "</b> <br/>Project Manager : " + assignedUserName.UserFullName
                    + "<br/>Sample Type - " + swQcInchargeproInfo.ProjectManagerSampleType + "<br/>Sample Quantity - " + swQcInchargeproInfo.ProjectManagerSampleNo + "<br/>Test Phase Name - " + tPhaseName.TestPhaseName + "<br/>Paused reason - " + pasueReason.PausedReason
                    + "<br/>Software Version Name - " + swQcInchargeproInfo.SoftwareVersionName + "<br/>Software Version No. - " + swQcInchargeproInfo.SoftwareVersionNo);

                var notificationObject = new NotificationObject
                {
                    Message = "paused a running project",
                    AdditionalMessage = projectHeadRemarks,
                    ProjectId = pMasterId,
                    ToUser = "-1"
                };
                ViewBag.ControllerVariable = notificationObject;
                return Json(swPauseOrRestartAssignedProjectResult, JsonRequestBehavior.AllowGet);
            }
            return new JsonResult { Data = "OK", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //return Json(new { result = "Redirect", url = Url.Action("Software", "SwPauseOrRestartAssignedProject") });
        }

        [HttpPost]
        [NotificationActionFilter(ReceiverRoles = "QC,PM,MM,CM,PS")]
        public JsonResult SwRestartAssignedProject(string projectMasterId, string swqcInchargeId, string projectName, string pmAssignId)
        {
            var _dbEntities = new CellPhoneProjectEntities();
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name);

            if (userId > 0)
            {
                long pMasterId;
                long.TryParse(projectMasterId, out pMasterId);

                long pSwQcInId;
                long.TryParse(swqcInchargeId, out pSwQcInId);

                long pmHeadAssignId;
                long.TryParse(pmAssignId, out pmHeadAssignId);

                var swRestartAssignedProjectResult = _repository.SaveSwRestartAssignedProject(pMasterId, pSwQcInId, projectName, pmHeadAssignId);

                var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);
                var swQcInchargeproInfo = _dbEntities.SwQcHeadAssignsFromPms.FirstOrDefault(i => i.ProjectMasterId == pMasterId && i.SwQcHeadAssignId == pSwQcInId);
                var tPhaseName = _dbEntities.SwQcTestPhases.FirstOrDefault(i => i.TestPhaseID == swQcInchargeproInfo.TestPhaseID);
                var assignedUserName = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == swQcInchargeproInfo.Added);

                MailSendFromPms mailSendFromPms = new MailSendFromPms();
                mailSendFromPms.SendMail(new List<long>(new[] { assignedUserName.CmnUserId }), new List<string>(new[] { "PMHEAD","CMHEAD", "MM", "CM", "PS", "QCHEAD" }), "Project(" + projectName + ") has been restarted by Software QC In charge",
                    "This is to inform you that, a paused project has been restarted by Software QC In charge, <b>" + user.UserFullName + "</b>. <br/><br/><br/><br/>Project : <b>" + projectName + "</b> <br/>Project Manager : " + assignedUserName.UserFullName
                    + "<br/>Sample Type - " + swQcInchargeproInfo.ProjectManagerSampleType + "<br/>Sample Quantity - " + swQcInchargeproInfo.ProjectManagerSampleNo + "<br/>Test Phase Name - " + tPhaseName.TestPhaseName
                    + "<br/>Software Version Name - " + swQcInchargeproInfo.SoftwareVersionName + "<br/>Software Version No. - " + swQcInchargeproInfo.SoftwareVersionNo);

                var notificationObject = new NotificationObject
                {
                    Message = "restart a paused project",
                    ProjectId = pMasterId,
                    ToUser = "-1"
                };

                ViewBag.ControllerVariable = notificationObject;
                return Json(swRestartAssignedProjectResult, JsonRequestBehavior.AllowGet);
            }
            return new JsonResult { Data = "OK", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //return Json(new { result = "Redirect", url = Url.Action("Software", "SwPauseOrRestartAssignedProject") });
        }

        [Authorize(Roles = "QCHEAD,SA")]

        public ActionResult SwPauseOrRestartQCCompletedProject(VmSwQcSpecificationModified model)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);

            //ViewBag.GetAssignedProjectToQCStatus = _repository.GetAssignedProjectToQCStatusForInchargeDashboard();
            ViewBag.GetQCCompletedProjectStatus = _repository.GetQcCompletedProjectStatusForInchargeDashboard();
            return View(model);
        }
        [HttpPost]
        [NotificationActionFilter(ReceiverRoles = "QC,PM,MM,CM,PS")]
        public JsonResult SwPauseOrRestartQCCompletedProject(string projectMsterId, string projectHeadRemarks, string swqcInchargeId, string projectName, string pmAssignId)
        {
            var _dbEntities = new CellPhoneProjectEntities();
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name);

            if (userId > 0)
            {
                long pMasterId;
                long.TryParse(projectMsterId, out pMasterId);

                long pSwQcInId;
                long.TryParse(swqcInchargeId, out pSwQcInId);

                long pmHeadAssignId;
                long.TryParse(pmAssignId, out pmHeadAssignId);

                var swPauseOrRestartAssignedProjectResult = _repository.SaveSwPauseOrRestartAssignedProject(pMasterId, projectHeadRemarks, pSwQcInId, projectName, pmHeadAssignId);

                //var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);
                //var swQcInchargeproInfo = _dbEntities.SwQcInchargeAssigns.FirstOrDefault(i => i.ProjectMasterId == pMasterId && i.SwQcInchargeAssignId == pSwQcInId);
                //var tPhaseName = _dbEntities.TestPhases.FirstOrDefault(i => i.TestPhaseID == swQcInchargeproInfo.TestPhaseID);

                //var assignedUserName = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == swQcInchargeproInfo.Added);


                //MailSendFromPms mailSendFromPms = new MailSendFromPms();
                //mailSendFromPms.SendMail(new List<long>(new[] { assignedUserName.CmnUserId }), new List<string>(new[] { "PMHEAD", "MM", "CM", "SA" }), "Project Restart by Software QC Incharge",
                //    "This is to inform you that, a project has been restart by Software QC Incharge, <b>" + user.UserFullName + "</b> . <br/>Project : <b>" + projectName + "</b> Project Manager : " + assignedUserName.UserFullName
                //    + "<br/>Sample Type - " + swQcInchargeproInfo.ProjectManagerSampleType + "<br/>Sample Quantity - " + swQcInchargeproInfo.ProjectManagerSampleNo + "<br/>Test Phase Name - " + tPhaseName.TestPhaseName + "<br/>Paused reason - " + swQcInchargeproInfo.PasuedReason);
                var notificationObject = new NotificationObject
                {
                    Message = "paused a running project",
                    AdditionalMessage = projectHeadRemarks,
                    ProjectId = pMasterId,
                    ToUser = "-1"
                };
                ViewBag.ControllerVariable = notificationObject;
                return Json(swPauseOrRestartAssignedProjectResult, JsonRequestBehavior.AllowGet);
            }
            return new JsonResult { Data = "OK", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            // return Json(new { result = "Redirect", url = Url.Action("Software", "SwPauseOrRestartQCCompletedProject") });
        }


        #endregion

        #region Report Dashboard for QCHEAD
        [Authorize(Roles = "QCHEAD,QC,SA,MM,CM")]

        public ActionResult ReportDashboard(VmSwInchargeViewModel model, string startValue = "", string endValue = "", string emplyCode = "", long projectId = 0, long swqcInchargeAsngId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);

            ViewBag.GetActiveQcList = _repository.GetActiveQc();
            ViewBag.GetAllProjectListDetailsForInchargeReport = _repository.GetAllProjectListDetailsForInchargeReport(startValue, endValue, emplyCode) ?? new List<SoftwareCustomModelForDashboard>();

            ViewBag.GetAllFieldTestListForInchargeReport = _repository.GetAllFieldTestListForInchargeReport(startValue, endValue, emplyCode) ?? new List<SoftwareCustomModelForDashboard>();


            return View(model);
        }

        [Authorize(Roles = "QCHEAD,QC,SA,MM,CM")]

        public ActionResult TodaysWorkStatus(VmSwInchargeViewModel model, string emplyCode = "")
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);

            ViewBag.GetActiveQcList = _repository.GetActiveQc();
            ViewBag.GetAllProjectPersonStatus = _repository.GetAllProjectPersonStatus(emplyCode) ?? new List<SoftwareCustomModelForDashboard>();

            return View(model);
        }
        public ActionResult DetailsOfSwQcTestCase(long projectId, string projectName, long swqcInchargeAsngId, string emplyCode, DateTime swQcHeadToQcAssignTime, long testPhaseId)
        {
            var fileManager = new FileManager();
            var vmSwInchargemodel = new VmSwQcHeadViewModel();
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name);

            /////For Re-assign project to Qc////////////

            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            List<PmQcAssignModel> pmQcAssignModels = _repository.GetPmQcAssignModels();
            List<TestPhaseModel> testPhaseList = _repository.GetTestPhases();

            //vmSwInchargemodel.PmQcAssignModels = pmQcAssignModels;
            List<CmnUserModel> list = _repository.GetActiveQc();
            vmSwInchargemodel.ddlAssignUsersList = list;
            //vmSwInchargemodel.ddlTestPhasesList = testPhaseList;
            ///////////////////////////////////////

            if (projectId != null)
            {

                vmSwInchargemodel.SwQcIssueDetailModels = _repository.GetSwQcIssueDetailsForReport(projectId, projectName, swqcInchargeAsngId, emplyCode, swQcHeadToQcAssignTime, testPhaseId);
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

                vmSwInchargemodel.SwQcIssueDetailModels1 = _repository.GetSwQcCtsMonkeyOrCameraAutomationDataForReport(projectId, projectName, swqcInchargeAsngId, emplyCode, swQcHeadToQcAssignTime, testPhaseId);
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


                vmSwInchargemodel.SwQcPersonalUseFindingsIssueDetailModels = _repository.GetPersonalUseFindingsForQcHeadForReport(projectId, swqcInchargeAsngId);
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

        #region com
        //public ActionResult DetailsOfSwQcTestCase(long projectId, long swqcInchargeAsngId, string emplyCode)
        //{
        //    var fileManager = new FileManager();
        //    var vmSwInchargemodel = new VmSwInchargeViewModel();
        //    long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name);
        //    ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
        //    if (projectId > 0)
        //    {
        //        vmSwInchargemodel.ProjectMasterModel = _repository.GetProjectMasterModel(projectId);
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

        //[HttpPost]
        //public JsonResult GetReportData(string startValue, string endValue, long userId = 0)
        //{
        //    var result = _repository.GetAllProjectListDetailsForInchargeReport(startValue, endValue);
        //    ViewBag.GetAllProjectListDetailsForInchargeReport = _repository.GetAllProjectListDetailsForInchargeReport(startValue, endValue);
        //    var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(result);
        //    return new JsonResult {Data = jsonData, JsonRequestBehavior = JsonRequestBehavior.AllowGet};
        //}
        #endregion

        #endregion

        #region Report Dashboard for QC
        [Authorize(Roles = "QC,SA,MM,CM")]
        public ActionResult QcReportDashboard(VmSwInchargeViewModel model, string startValue = "", string endValue = "", string emplyCode = "", long projectId = 0, long swqcInchargeAsngId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);

            ViewBag.GetAllProjectListDetailsForQc = _repository.GetAllProjectListDetailsForQc(startValue, endValue, userId);

            return View(model);
        }

        #endregion

        #region Qc person Delete or Newly assign person
        [Authorize(Roles = "QCHEAD,SA")]

        [NotificationActionFilter(ReceiverRoles = "QCHEAD,QC,PM,MM,PS")]
        [HttpPost]

        public JsonResult QCDeleteOrReassignByQcIncharge(string projectMasterId, string swqcInchargeId, string projectPmAssignId, string approxInchargeToQcDeliveryDate, string swInchargeDeleteQcComment, string swQcInchargeReassignToQcComment, string multideleteValue, string multiReassignValue)
        {
            var _dbEntities = new CellPhoneProjectEntities();
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name);
            var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);

            if (userId > 0)
            {
                long pMasterId;
                long.TryParse(projectMasterId, out pMasterId);

                long pSwQcInId;
                long.TryParse(swqcInchargeId, out pSwQcInId);

                long pPrPmAssignId;
                long.TryParse(projectPmAssignId, out pPrPmAssignId);


                //----------MAIL Start-----------------------
                var swQcInchargeproInfo = _dbEntities.SwQcHeadAssignsFromPms.FirstOrDefault(i => i.ProjectMasterId == pMasterId && i.SwQcHeadAssignId == pSwQcInId);
                var tPhaseName = _dbEntities.SwQcTestPhases.FirstOrDefault(i => i.TestPhaseID == swQcInchargeproInfo.TestPhaseID);
                var proName = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == pMasterId);

                //---------------ends-----------------

                var notificationObject1 = new NotificationObject();
                var notificationObject2 = new NotificationObject();

                if (multideleteValue.ToString().Trim() != "null")
                {
                    var multideleteValue1 = multideleteValue.Split(',');

                    notificationObject1 = new NotificationObject
                    {
                        Message = "deleted from the assigned project",
                        AdditionalMessage = swInchargeDeleteQcComment,
                        ProjectId = pMasterId,
                        // ToUser = "-1"
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
                        deletedQc = deletedQc + deletedUserName.UserFullName + ", ";
                    }

                    deletedQc = deletedQc.TrimEnd(',');
                    body = "This is to inform you that, <b>" + deletedQc + " </b> has/have been deleted by <b>" + user.UserFullName + "</b> for testing of Software QC issues.<br/><br/><br/> <br/>"
                        + "Project : <b>" + proName.ProjectName + "</b> <br/>Sample Type - " + swQcInchargeproInfo.ProjectManagerSampleType + "<br/>Sample Quantity - "
                        + swQcInchargeproInfo.ProjectManagerSampleNo + "<br/>Test Phase Name - " + tPhaseName.TestPhaseName + "<br/>Reason Of Deleting - " + swInchargeDeleteQcComment
                        + "<br/>Software Version Name - " + swQcInchargeproInfo.SoftwareVersionName + "<br/>Software Version No. - " + swQcInchargeproInfo.SoftwareVersionNo;

                    var mailSendFromPms = new MailSendFromPms();
                    mailSendFromPms.SendMail(ids, new List<string>(new[] { "MM", "PS", "QCHEAD","PMHEAD" }), "Software QC person has been deleted from a Project(" + proName.ProjectName + ")", body);

                }

                if (multiReassignValue.ToString().Trim() != "null")
                {
                    var multiReassignValue1 = multiReassignValue.Split(',');
                    notificationObject2 = new NotificationObject
                    {
                        Message = "newly assigned for Checking Software QC Issues",
                        ProjectId = pMasterId,
                    };

                    List<long> ids = new List<long>();
                    string body = string.Empty;
                    string deletedQc = string.Empty;

                    foreach (var mIdr1 in multiReassignValue1)
                    {
                        notificationObject2.ToUser = notificationObject2.ToUser + mIdr1 + ",";

                        long qcIDs;
                        long.TryParse(mIdr1, out qcIDs);
                        ids.Add(qcIDs);

                        var deletedUserName = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == qcIDs);
                        deletedQc = deletedQc + deletedUserName.UserFullName + ", ";
                    }

                    deletedQc = deletedQc.TrimEnd(',');
                    body = "This is to inform you that, <b>" + deletedQc + " </b> has/have been newly assigned by <b>" + user.UserFullName + "</b> for testing of Software QC issues.<br/><br/><br/> <br/>"
                        + "Project : <b>" + proName.ProjectName + "</b> <br/>Sample Type - " + swQcInchargeproInfo.ProjectManagerSampleType + "<br/>Sample Quantity - "
                        + swQcInchargeproInfo.ProjectManagerSampleNo + "<br/>Test Phase Name - " + tPhaseName.TestPhaseName
                        + "<br/>Software Version Name - " + swQcInchargeproInfo.SoftwareVersionName + "<br/>Software Version No. - " + swQcInchargeproInfo.SoftwareVersionNo;

                    var mailSendFromPms = new MailSendFromPms();
                    mailSendFromPms.SendMail(ids, new List<string>(new[] { "MM", "PS", "QCHEAD","PMHEAD" }), "Software QC person has been newly assigned for a Project(" + proName.ProjectName + ")", body);

                }
                var swQCDeleteOrNewAssignByQcIncharge = _repository.DeleteOrNewAssignQcByQcIncharge(pMasterId, pSwQcInId,
                    pPrPmAssignId, multideleteValue, multiReassignValue, approxInchargeToQcDeliveryDate,
                    swInchargeDeleteQcComment, swQcInchargeReassignToQcComment);

                ViewBag.ControllerVariable = notificationObject1;
                ViewBag.ControllerVariable = notificationObject2;

                return Json(swQCDeleteOrNewAssignByQcIncharge, JsonRequestBehavior.AllowGet);

            }

            // return Json(new { result = "Redirect", url = Url.Action("Software", "SwPauseOrRestartAssignedProject") });
            return new JsonResult { Data = "OK", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult DeletedUserIdList(string projectMasterId, string swqcInchargeId, string projectPmAssignId)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);

            long projectId;
            long.TryParse(projectMasterId, out projectId);

            long swqcInchargeIds;
            long.TryParse(swqcInchargeId, out swqcInchargeIds);

            long projectPmAssignIds;
            long.TryParse(projectPmAssignId, out projectPmAssignIds);

            var GetDeletedPersonNameLists = _repository.GetDeletedPersonNameList(projectId, swqcInchargeIds, projectPmAssignIds);



            return Json(GetDeletedPersonNameLists, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Zip All Doc
        public ActionResult GetAllSwFiles(string projectName, long projectId = 0, long swqcInchargeId = 0)
        {
            var swAllFilesModel = new List<SwQcAllFilesModel>();
            swAllFilesModel = _repository.GetAllFilesModel(projectId, swqcInchargeId);

            ViewBag.ProjectName = projectName;
            if (swAllFilesModel.Any())
            {


                foreach (SwQcAllFilesModel model in swAllFilesModel)
                {
                    model.ProjectMasterId = projectId;
                    model.ProjectName = projectName;

                    if (model.UploadedFile1 != null)
                    {
                        var urls = model.UploadedFile1.Split('|').ToList();
                        for (int i = 0; i < urls.Count; i++)
                        {
                            var last = urls[i].Split('\\').Last();
                            model.UploadedFiles1.Add(last);
                        }
                    }

                }

            }

            return View(swAllFilesModel);
        }

        [HttpPost]
        public FileResult GetAllSwFiles(List<string> files, List<SwQcAllFilesModel> swAllFilesModel)
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

            return File(archive, "application/zip", DateTime.Now.ToString("dd-MM-yyyy hh:mm ss") + " - SwQcIssues - " + swAllFilesModel[0].ProjectName + ".zip");
        }
        #endregion

        #region Post production

        /////For SwQc////
        public ActionResult AllProjectPostProductionIssuesForSwQc(string projectId)
        {
            AssignForPostProductionMuliplePersonViewModel model = new AssignForPostProductionMuliplePersonViewModel();
            long userId = Convert.ToInt64(User.Identity.Name);


            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);

            model.ProjectMasterModelsList = _repository.GetProjectMasterModelForPostProduction();


            List<SwQcPostProductionAssignModel> swQcPostProductionAssignModels =
                  _repository.GetProjectListForPostProductionIssueList(userId);


            model.swQcPostProductionAssignModels = swQcPostProductionAssignModels;



            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select One" } };

            var query = (from sppa in model.swQcPostProductionAssignModels
                         join master in model.ProjectMasterModelsList on sppa.ProjectMasterId equals master.ProjectMasterId
                         where
                             master.ProjectStatus == "APPROVED"
                         select new
                         {
                             master.ProjectMasterId,
                             master.ProjectName,
                             master.OrderNuber,
                             sppa.SwQcPostProductAssignId,
                             sppa.RoleName

                         }).ToList();


            foreach (var t in query)
            {
                String selectedValue = t.ProjectMasterId + "," + t.SwQcPostProductAssignId;

                selectListItems.Add(new SelectListItem
                {
                    Value = selectedValue,
                    Text = t.ProjectName
                });

            }
            ViewBag.CombinedIds = selectListItems;





            /////////////////For load issues list/////////

            string obj1 = null;
            obj1 = projectId;
            if (obj1 != null)
            {
                if (obj1 != "")
                {

                    String pro_id1 = null;

                    var match1 = obj1.Split(',');

                    for (var i = 0; i < match1.Length; i++)
                    {
                        Console.Write("<br /> Element " + i + " of the array is: " + match1[i]);

                        pro_id1 = match1[0];

                    }
                    Console.Write("pro_id1 :" + pro_id1);

                    long pMasterId1;
                    long.TryParse(pro_id1, out pMasterId1);

                    List<PostProductionIssueModel> swQcPostProductionAssignModelsForSearch =
                        _repository.GetProjectListForPostProductionIssueListAfterSearch(pMasterId1, userId);

                    model.AllProjectIssuesForSwQcModels = swQcPostProductionAssignModelsForSearch;

                    if (model.AllProjectIssuesForSwQcModels.Any())
                    {

                        foreach (
                            PostProductionIssueModel model1 in model.AllProjectIssuesForSwQcModels)
                        {

                            if (model1.Upload != null)
                            {
                                var urls = model1.Upload.Split('|').ToList();
                                for (int i = 0; i < urls.Count; i++)
                                {

                                    var last = urls[i].Split('-').Last();
                                    model1.UploadedFileGetUrl.Add(Path.GetFileName(last));
                                }
                            }

                        }
                    }

                }
            }

            model.CombinedProjectId = projectId;

            return View(model);

        }

        [NotificationActionFilter(ReceiverRoles = "QCHEAD,MKT,PM,MM,PS,PMHEAD,QC,CM")]
        [HttpPost]
        public ActionResult AllProjectPostProductionIssuesForSwQc(List<PostProductionIssueModel> issueList, String pro_id, String SwQcPostProductAssignId)
        {
            CellPhoneProjectEntities _dbEntities = new CellPhoneProjectEntities();
            issueList = issueList.Where(x => x.IsRemoved == 0).ToList();
            long userId = Convert.ToInt64(User.Identity.Name);
            if (ModelState.IsValid)
            {
                long pMasterId;
                long.TryParse(pro_id, out pMasterId);
                long swQcPostPro;
                long.TryParse(SwQcPostProductAssignId, out swQcPostPro);

                var pMHeadId = (from cmn1 in _dbEntities.CmnUsers
                                where cmn1.RoleName == "PMHEAD"
                                group cmn1 by new { cmn1.CmnUserId }
                                    into pmHeadGroup
                                    select new
                                    {
                                        pmHeadGroup.Key.CmnUserId,
                                    }
                 ).ToList();

                var pMId = (from sia in _dbEntities.SwQcInchargeAssigns
                            where sia.ProjectMasterId == pMasterId
                            group sia by new { sia.ProjectManagerUserId }
                                into pmGroup
                                select new
                                {
                                    pmGroup.Key.ProjectManagerUserId,
                                }).ToList();


                var qCHeadId = (from cmn2 in _dbEntities.CmnUsers
                                where cmn2.RoleName == "QCHEAD"
                                group cmn2 by new { cmn2.CmnUserId }
                                    into pmHeadGroup
                                    select new
                                    {
                                        pmHeadGroup.Key.CmnUserId,
                                    }).ToList();

                var qCIds = (from sa in _dbEntities.SwQcAssigns
                             where sa.ProjectMasterId == pMasterId && (sa.Status == "ASSIGNED" || sa.Status == "QCCOMPLETED" || sa.Status == "RECOMMENDED")
                             group sa by new { sa.SwQcUserId }
                                 into grp
                                 select new
                                 {
                                     grp.Key.SwQcUserId,


                                 }).ToList();

                var CMId = (from cmn3 in _dbEntities.CmnUsers
                            where cmn3.RoleName == "CM"
                            group cmn3 by new { cmn3.CmnUserId }
                                into cmHeadGroup
                                select new
                                {
                                    cmHeadGroup.Key.CmnUserId,
                                }).ToList();

                var hwHeadId = (from cmn3 in _dbEntities.CmnUsers
                                where cmn3.RoleName == "HWHEAD"
                                group cmn3 by new { cmn3.CmnUserId }
                                    into cmHeadGroup
                                    select new
                                    {
                                        cmHeadGroup.Key.CmnUserId,
                                    }).ToList();

                var hwIds = (from hw in _dbEntities.HwQcAssigns
                             where hw.ProjectMasterId == pMasterId
                             group hw by new { hw.HwQcUserId }
                                 into hwGroup
                                 select new
                                 {
                                     hwGroup.Key.HwQcUserId,
                                 }).ToList();

                var mMHeadId = (from cmn3 in _dbEntities.CmnUsers
                                where cmn3.RoleName == "MM"
                                group cmn3 by new { cmn3.CmnUserId }
                                    into cmHeadGroup
                                    select new
                                    {
                                        cmHeadGroup.Key.CmnUserId,
                                    }).ToList();
                var pSId = (from cmn3 in _dbEntities.CmnUsers
                            where cmn3.RoleName == "PS"
                            group cmn3 by new { cmn3.CmnUserId }
                                into cmHeadGroup
                                select new
                                {
                                    cmHeadGroup.Key.CmnUserId,
                                }).ToList();



                string allUsers = string.Empty;
                foreach (var v in pMHeadId)
                {

                    allUsers += v.CmnUserId + ",";
                }
                foreach (var v in pMId)
                {

                    allUsers += v.ProjectManagerUserId + ",";
                }
                foreach (var v in qCHeadId)
                {
                    allUsers += v.CmnUserId + ",";
                }
                foreach (var v in qCIds)
                {
                    allUsers += v.SwQcUserId + ",";
                }

                foreach (var v in CMId)
                {
                    allUsers += v.CmnUserId + ",";
                }

                foreach (var v in hwHeadId)
                {
                    allUsers += v.CmnUserId + ",";
                }

                foreach (var v in hwIds)
                {
                    allUsers += v.HwQcUserId + ",";
                }

                foreach (var v in mMHeadId)
                {
                    allUsers += v.CmnUserId + ",";
                }

                foreach (var v in pSId)
                {
                    allUsers += v.CmnUserId + ",";
                }


                allUsers = allUsers.TrimEnd(',');
                System.Console.Write(allUsers);

                List<string> uniques = allUsers.Split(',').Reverse().Distinct().Reverse().ToList();
                string allUsersList = string.Join(",", uniques);
                Console.WriteLine(allUsersList);


                var manager = new FileManager();

                foreach (var swQcProWiseModel in issueList)
                {
                    if (swQcProWiseModel.File.Count() > 0 && swQcProWiseModel.File != null)
                    {
                        var res = manager.Upload1(pMasterId, "SwQCPostProductionIssuesImage",
                            "QCPostProductionIssuesImage", swQcProWiseModel.File);
                        swQcProWiseModel.UploadedFile = swQcProWiseModel.UploadedFile == null ? res : swQcProWiseModel.UploadedFile + "|" + res;

                    }

                }

                foreach (var postIssues in issueList)
                {

                    var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);
                    var proName = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == pMasterId);
                    var multiple1 = allUsersList.Split(',');
                    if (allUsersList != null)
                    {
                        var notifications = new List<Notification>();
                        foreach (var uId in multiple1)
                        {
                            int uuid;
                            int.TryParse(uId, out uuid);
                            Notification notification = new Notification();
                            notification.ProjectMasterId = pMasterId;
                            notification.Message = "" + user.UserFullName + " created Software Issues in Post Production Phase. Project Name :" + proName.ProjectName + "";
                            notification.IsViewd = false;
                            notification.ViewerId = uuid;
                            notification.Role = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == uuid).RoleName;
                            notification.AddedBy = userId;
                            notification.Added = DateTime.Now;


                            notifications.Add(notification);

                        }
                        _dbEntities.Notifications.AddRange(notifications);
                        _dbEntities.SaveChanges();
                        string body = string.Empty;
                        //////////////////mail///////////////////////



                        body =
                            "This is to inform you that in Post Production Phase Software Issue has been found by <b>" +
                            user.UserFullName + "</b>.  <br/> <br/><br/>" + "Project : <b>"
                            + proName.ProjectName + "</b> <br/>Issue : <b>" + postIssues.IssueName +
                            "</b> <br/>Issue Details :<b>" + postIssues.Comment + "</b> <br/>Issue Type :<b>" +
                            postIssues.IssueType + "</b> <br/>Frequency :<b>" + postIssues.Frequency;
                        MailSendFromPms mailSendFromPms = new MailSendFromPms();
                        List<long> mailIdList = multiple1.Select(long.Parse).ToList();
                        mailSendFromPms.SendMail(mailIdList, new List<string>(new[] { " " }),
                            " Post production issues found (" + proName.ProjectName + ")",
                            body);

                        //////////////////mail///////////////////
                    }


                }
                _repository.SaveAllProjectIssuesForSwQcModels(issueList, pMasterId, swQcPostPro, allUsersList);

            }

            return RedirectToAction("AllProjectPostProductionIssuesForSwQc", new { projectId = pro_id + "," + SwQcPostProductAssignId });

        }

        [Authorize(Roles = "QCHEAD,SA")]
        public ActionResult AssignPostProductionMuliplePerson()
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            //Get from Repository
            List<SwQcPostProductionAssignModel> swQcPostProductionAssignModels = _repository.GetProjectListForPostProductionAssign();
            List<CmnUserModel> list = _repository.GetActiveQc();

            //Create a ViewModel
            AssignForPostProductionMuliplePersonViewModel model = new AssignForPostProductionMuliplePersonViewModel();
            model.swQcPostProductionAssignModels = swQcPostProductionAssignModels;
            model.ddlAssignUsersList = list;

            return View(model);
        }

        [NotificationActionFilter(ReceiverRoles = "QCHEAD,QC,PM,MM,PS")]
        [HttpPost]
        public JsonResult AssignPostProductionMuliplePerson(string ProjectMasterId, string swInchargeDeleteQcComment, string swQcInchargeAssignToQcComment, long sampleNumber, string multideleteValue, string multiAssignValue)
        {
            var _dbEntities = new CellPhoneProjectEntities();
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name);
            var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);

            if (userId > 0)
            {
                long pMasterId;
                long.TryParse(ProjectMasterId, out pMasterId);


                //----------MAIL Start-----------------------
                var proName = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == pMasterId);
                //---------------ends-----------------

                var notificationObject1 = new NotificationObject();
                var notificationObject2 = new NotificationObject();

                if (multideleteValue.ToString().Trim() != "null")
                {
                    var multideleteValue1 = multideleteValue.Split(',');

                    notificationObject1 = new NotificationObject
                    {
                        Message = "deleted from the Post Production project",
                        AdditionalMessage = swInchargeDeleteQcComment,
                        ProjectId = pMasterId,
                        // ToUser = "-1"
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
                        deletedQc = deletedQc + deletedUserName.UserFullName + ", ";
                    }

                    deletedQc = deletedQc.TrimEnd(',');
                    body = "This is to inform you that, <b>" + deletedQc + " </b> has/have been deleted by <b>" + user.UserFullName + "</b> for testing of Post Production project.<br/><br/><br/> <br/>"
                        + "Project : <b>" + proName.ProjectName + "</b><br/>Reason Of Deleting - " + swInchargeDeleteQcComment;

                    var mailSendFromPms = new MailSendFromPms();
                    mailSendFromPms.SendMail(ids, new List<string>(new[] { "MM", "PS", "QCHEAD","PMHEAD" }), "Software QC person has been deleted from a Post Proiduction Project(" + proName.ProjectName + ")", body);

                }

                if (multiAssignValue.ToString().Trim() != "null")
                {
                    var multiAssignValue1 = multiAssignValue.Split(',');
                    notificationObject2 = new NotificationObject
                    {
                        Message = "newly assigned for Checking Post Production project",
                        ProjectId = pMasterId,
                    };

                    List<long> ids = new List<long>();
                    string body = string.Empty;
                    string deletedQc = string.Empty;

                    foreach (var mIdr1 in multiAssignValue1)
                    {
                        notificationObject2.ToUser = notificationObject2.ToUser + mIdr1 + ",";

                        long qcIDs;
                        long.TryParse(mIdr1, out qcIDs);
                        ids.Add(qcIDs);

                        var deletedUserName = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == qcIDs);
                        deletedQc = deletedQc + deletedUserName.UserFullName + ", ";
                    }

                    deletedQc = deletedQc.TrimEnd(',');
                    body = "This is to inform you that, <b>" + deletedQc + " </b> has/have been newly assigned by <b>" + user.UserFullName + "</b> for testing of Post Production project.<br/><br/><br/> <br/>"
                        + "Project : <b>" + proName.ProjectName;

                    var mailSendFromPms = new MailSendFromPms();
                    mailSendFromPms.SendMail(ids, new List<string>(new[] { "MM", "PS", "QCHEAD","PMHEAD" }), "Software QC person has been newly assigned for a Post Production Project(" + proName.ProjectName + ")", body);

                }
                var swQCDeleteOrNewAssignByQcInchargeForPostProduction = _repository.DeleteOrNewAssignQcByQcInchargeForPostProduction(pMasterId, swInchargeDeleteQcComment, swQcInchargeAssignToQcComment, sampleNumber,
                    multideleteValue, multiAssignValue);

                ViewBag.ControllerVariable = notificationObject1;
                ViewBag.ControllerVariable = notificationObject2;

                return Json(swQCDeleteOrNewAssignByQcInchargeForPostProduction, JsonRequestBehavior.AllowGet);

            }

            return Json(new { result = "Redirect", url = Url.Action("Software", "AssignPostProductionMuliplePerson") });

        }
        public JsonResult DeletedPostProductionUserIdList(string projectMasterId, string swqcInchargeId, string projectPmAssignId)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);

            long projectId;
            long.TryParse(projectMasterId, out projectId);

            long swqcInchargeIds;
            long.TryParse(swqcInchargeId, out swqcInchargeIds);

            long projectPmAssignIds;
            long.TryParse(projectPmAssignId, out projectPmAssignIds);

            var GetDeletedPersonNameLists = _repository.GetDeletedPersonNameListForPostProduction(projectId);



            return Json(GetDeletedPersonNameLists, JsonRequestBehavior.AllowGet);
        }


        /////For MKT////
        public ActionResult AllProjectPostProductionIssuesForMKT(string projectName)
        {
            AssignForPostProductionMuliplePersonViewModel model = new AssignForPostProductionMuliplePersonViewModel();
            long userId = Convert.ToInt64(User.Identity.Name);

            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);
            model.ProjectName = projectName;
            model.ProjectMasterModelsList = _repository.GetProjectMasterModelForPostProduction();

            List<OrderNumberModel> orderNumberModels = new List<OrderNumberModel>();



            List<ProjectMasterModel> ddlOrderNumber = _repository.GetProjectOrderNumberList(projectName);
            model.DdlOrderNumber = ddlOrderNumber;
            foreach (var ddlOr in model.DdlOrderNumber)
            {
                var orderObj = new OrderNumberModel();
                orderObj.OrderNumber = ddlOr.OrderNuber.ToString();
                orderObj.OrderNumberOrdinal = CommonConversion.AddOrdinal(ddlOr.OrderNuber);

                orderNumberModels.Add(orderObj);
            }
            model.DdlOrderNumberModels = orderNumberModels;

            /////////////////For load issues list/////////

            string obj1 = null;
            obj1 = projectName;
            if (obj1 != null)
            {
                if (obj1 != "")
                {

                    String pro_name = null;

                    var match1 = obj1.Split(',');

                    for (var i = 0; i < match1.Length; i++)
                    {
                        Console.Write("<br /> Element " + i + " of the array is: " + match1[i]);

                        pro_name = match1[0];

                    }
                    Console.Write("pro_name :" + pro_name);

                    List<PostProductionIssueModel> swQcPostProductionAssignModelsForSearch =
                       _repository.GetProjectListForPostProductionIssueListAfterSearchForMK(pro_name, userId);

                    model.AllProjectIssuesForSwQcModels = swQcPostProductionAssignModelsForSearch;

                    if (model.AllProjectIssuesForSwQcModels.Any())
                    {

                        foreach (
                            PostProductionIssueModel model1 in model.AllProjectIssuesForSwQcModels)
                        {

                            if (model1.Upload != null)
                            {
                                var urls = model1.Upload.Split('|').ToList();
                                for (int i = 0; i < urls.Count; i++)
                                {

                                    var last = urls[i].Split('-').Last();
                                    model1.UploadedFileGetUrl.Add(Path.GetFileName(last));
                                }
                            }

                        }
                    }

                }
            }

            model.CombinedProjectId = projectName;

            return View(model);

        }

        [NotificationActionFilter(ReceiverRoles = "QCHEAD,MKT,PM,MM,PS,PMHEAD,QC,CM")]
        [HttpPost]
        public ActionResult AllProjectPostProductionIssuesForMKT(List<PostProductionIssueModel> issueList, String pro_id, String orderId)
        {
            CellPhoneProjectEntities _dbEntities = new CellPhoneProjectEntities();
            issueList = issueList.Where(x => x.IsRemoved == 0).ToList();
            long userId = Convert.ToInt64(User.Identity.Name);
            if (ModelState.IsValid)
            {
                long pMasterId;
                long.TryParse(pro_id, out pMasterId);
                long swQcPostPro;
                long.TryParse(orderId, out swQcPostPro);

                var pMHeadId = (from cmn1 in _dbEntities.CmnUsers
                                where cmn1.RoleName == "PMHEAD"
                                group cmn1 by new { cmn1.CmnUserId }
                                    into pmHeadGroup
                                    select new
                                    {
                                        pmHeadGroup.Key.CmnUserId,
                                    }
                 ).ToList();

                var pMId = (from sia in _dbEntities.SwQcInchargeAssigns
                            where sia.ProjectMasterId == pMasterId
                            group sia by new { sia.ProjectManagerUserId }
                                into pmGroup
                                select new
                                {
                                    pmGroup.Key.ProjectManagerUserId,
                                }).ToList();


                var qCHeadId = (from cmn2 in _dbEntities.CmnUsers
                                where cmn2.RoleName == "QCHEAD"
                                group cmn2 by new { cmn2.CmnUserId }
                                    into pmHeadGroup
                                    select new
                                    {
                                        pmHeadGroup.Key.CmnUserId,
                                    }).ToList();

                var qCIds = (from sa in _dbEntities.SwQcAssigns
                             where sa.ProjectMasterId == pMasterId && (sa.Status == "ASSIGNED" || sa.Status == "QCCOMPLETED" || sa.Status == "RECOMMENDED")
                             group sa by new { sa.SwQcUserId }
                                 into grp
                                 select new
                                 {
                                     grp.Key.SwQcUserId,


                                 }).ToList();

                var CMId = (from cmn3 in _dbEntities.CmnUsers
                            where cmn3.RoleName == "CM"
                            group cmn3 by new { cmn3.CmnUserId }
                                into cmHeadGroup
                                select new
                                {
                                    cmHeadGroup.Key.CmnUserId,
                                }).ToList();

                var hwHeadId = (from cmn3 in _dbEntities.CmnUsers
                                where cmn3.RoleName == "HWHEAD"
                                group cmn3 by new { cmn3.CmnUserId }
                                    into cmHeadGroup
                                    select new
                                    {
                                        cmHeadGroup.Key.CmnUserId,
                                    }).ToList();

                var hwIds = (from hw in _dbEntities.HwQcAssigns
                             where hw.ProjectMasterId == pMasterId
                             group hw by new { hw.HwQcUserId }
                                 into hwGroup
                                 select new
                                 {
                                     hwGroup.Key.HwQcUserId,
                                 }).ToList();

                var mMHeadId = (from cmn3 in _dbEntities.CmnUsers
                                where cmn3.RoleName == "MM"
                                group cmn3 by new { cmn3.CmnUserId }
                                    into cmHeadGroup
                                    select new
                                    {
                                        cmHeadGroup.Key.CmnUserId,
                                    }).ToList();
                var pSId = (from cmn3 in _dbEntities.CmnUsers
                            where cmn3.RoleName == "PS"
                            group cmn3 by new { cmn3.CmnUserId }
                                into cmHeadGroup
                                select new
                                {
                                    cmHeadGroup.Key.CmnUserId,
                                }).ToList();



                string allUsers = string.Empty;
                foreach (var v in pMHeadId)
                {

                    allUsers += v.CmnUserId + ",";
                }
                foreach (var v in pMId)
                {

                    allUsers += v.ProjectManagerUserId + ",";
                }
                foreach (var v in qCHeadId)
                {
                    allUsers += v.CmnUserId + ",";
                }
                foreach (var v in qCIds)
                {
                    allUsers += v.SwQcUserId + ",";
                }

                foreach (var v in CMId)
                {
                    allUsers += v.CmnUserId + ",";
                }

                foreach (var v in hwHeadId)
                {
                    allUsers += v.CmnUserId + ",";
                }

                foreach (var v in hwIds)
                {
                    allUsers += v.HwQcUserId + ",";
                }

                foreach (var v in mMHeadId)
                {
                    allUsers += v.CmnUserId + ",";
                }

                foreach (var v in pSId)
                {
                    allUsers += v.CmnUserId + ",";
                }


                allUsers = allUsers.TrimEnd(',');
                System.Console.Write(allUsers);

                List<string> uniques = allUsers.Split(',').Reverse().Distinct().Reverse().ToList();
                string allUsersList = string.Join(",", uniques);
                Console.WriteLine(allUsersList);

                var manager = new FileManager();

                foreach (var swQcProWiseModel in issueList)
                {
                    if (swQcProWiseModel.File.Count() > 0 && swQcProWiseModel.File != null)
                    {
                        var res = manager.Upload1(pMasterId, "SwQCPostProductionIssuesImage",
                            "QCPostProductionIssuesImage", swQcProWiseModel.File);
                        swQcProWiseModel.UploadedFile = swQcProWiseModel.UploadedFile == null ? res : swQcProWiseModel.UploadedFile + "|" + res;

                    }

                }

                foreach (var postIssues in issueList)
                {

                    var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);
                    var proName = _dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == pMasterId);
                    var multiple1 = allUsersList.Split(',');
                    if (allUsersList != null)
                    {
                        var notifications = new List<Notification>();
                        foreach (var uId in multiple1)
                        {
                            int uuid;
                            int.TryParse(uId, out uuid);
                            Notification notification = new Notification();
                            notification.ProjectMasterId = pMasterId;
                            notification.Message = "" + user.UserFullName + " created Software Issues in Post Production Phase. Project Name :" + proName.ProjectName + "";
                            notification.IsViewd = false;
                            notification.ViewerId = uuid;
                            notification.Role = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == uuid).RoleName;
                            notification.AddedBy = userId;
                            notification.Added = DateTime.Now;


                            notifications.Add(notification);

                        }
                        _dbEntities.Notifications.AddRange(notifications);
                        _dbEntities.SaveChanges();
                        string body = string.Empty;
                        //////////////////mail///////////////////////



                        body =
                            "This is to inform you that in Post Production Phase Software Issue has been found by <b>" +
                            user.UserFullName + "</b>.  <br/> <br/><br/>" + "Project : <b>"
                            + proName.ProjectName + "</b> <br/>Issue : <b>" + postIssues.IssueName +
                            "</b> <br/>Issue Details :<b>" + postIssues.Comment + "</b> <br/>Issue Type :<b>" +
                            postIssues.IssueType + "</b> <br/>Frequency :<b>" + postIssues.Frequency;
                        MailSendFromPms mailSendFromPms = new MailSendFromPms();
                        List<long> mailIdList = multiple1.Select(long.Parse).ToList();
                        mailSendFromPms.SendMail(mailIdList, new List<string>(new[] { " " }),
                            " Post production issues found (" + proName.ProjectName + ")",
                            body);

                        //////////////////mail///////////////////
                    }


                }


                _repository.SaveAllProjectIssuesForMKTModels(issueList, pMasterId, allUsersList);

            }

            return RedirectToAction("AllProjectPostProductionIssuesForMKT", new { projectId = pro_id + "," + orderId });

        }

        [Authorize(Roles = "QCHEAD,QC,SA,PM,PMHEAD,PS,CM,MM,MKT,HW,HWHEAD")]
        public ActionResult PostProductionReport(string projectId)
        {
            AssignForPostProductionMuliplePersonViewModel model = new AssignForPostProductionMuliplePersonViewModel();
            long userId = Convert.ToInt64(User.Identity.Name);
            FileManager fmanager = new FileManager();
            ViewBag.UserInfo = _repository.GetUserInfoByUserId(userId);

            model.ProjectMasterModelsList = _repository.GetProjectMasterModelForPostProduction();

            var selectListItems1 = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select One" } };

            var query1 = (from master in model.ProjectMasterModelsList

                          where
                              master.ProjectStatus == "APPROVED"
                          select new
                          {
                              master.ProjectMasterId,
                              master.ProjectName,
                              master.OrderNuber,

                          }).ToList();


            foreach (var t in query1)
            {
                String selectedValue1 = t.ProjectMasterId + "," + t.OrderNuber;

                selectListItems1.Add(new SelectListItem
                {
                    Value = selectedValue1,
                    Text = t.ProjectName
                });

            }
            ViewBag.CombinedIds = selectListItems1;

            /////////////////For load issues list/////////

            string obj1 = null;
            obj1 = projectId;
            if (obj1 != null)
            {
                if (obj1 != "")
                {

                    String pro_id1 = null;

                    var match1 = obj1.Split(',');

                    for (var i = 0; i < match1.Length; i++)
                    {
                        Console.Write("<br /> Element " + i + " of the array is: " + match1[i]);

                        pro_id1 = match1[0];

                    }
                    Console.Write("pro_id1 :" + pro_id1);

                    long pMasterId;
                    long.TryParse(pro_id1, out pMasterId);

                    List<PostProductionIssueModel> swQcPostProductionAssignModelsForSearch =
                       _repository.GetProjectListForPostProductionIssueListAfterSearchForAll(pMasterId, userId);

                    model.AllProjectIssuesForSwQcModels = swQcPostProductionAssignModelsForSearch;

                    if (model.AllProjectIssuesForSwQcModels.Any())
                    {

                        foreach (
                            PostProductionIssueModel model1 in model.AllProjectIssuesForSwQcModels)
                        {

                            if (model1.Upload != null)
                            {
                                var urls = model1.Upload.Split('|').ToList();
                                for (int i = 0; i < urls.Count; i++)
                                {
                                    FilesDetailForPostProduction postProduction = new FilesDetailForPostProduction();
                                    postProduction.FilePath = fmanager.GetFile(urls[i]);
                                    postProduction.Extention = fmanager.GetExtension(urls[i]);
                                    model1.FilesDetails.Add(postProduction);
                                }
                            }

                        }
                    }

                }
            }

            model.CombinedProjectId = projectId;

            return View(model);

        }

        #endregion

        #region comment
        //public ActionResult SwQcBattery(long projectId = 0, long AssignId = 0)
        //{
        //    long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name);
        //    var batteryViewModel = new SwBatteryViewModel
        //    {

        //        SwQcAssignModels = _repository.GetProjectListForSwQcBatteryList(userId)
        //    };

        //    batteryViewModel.AssignId = AssignId;
        //    var generator = new SwBatteryStaticListGenerator();
        //    if (projectId > 0)
        //    {
        //        batteryViewModel.ProjectMasterModel = _commercialRepository.GetProjectMasterModel(projectId);
        //        var data = _repository.GetSwQcBatteryForList(projectId);
        //        if (data.Any())
        //        {
        //            var fileManager = new FileManager();
        //            batteryViewModel.IsEdit = true;
        //            batteryViewModel.SwQcBatteryAssignIssueModelsList = data;

        //            foreach (SwQcBatteryAssignIssueModel model in batteryViewModel.SwQcBatteryAssignIssueModelsList)
        //            {
        //                model.ScreenShotGetUrl1 = fileManager.GetFile(model.ScreenShot1FilePath);
        //                model.ScreenShotGetUrl2 = fileManager.GetFile(model.ScreenShot2FilePath);
        //                model.VideoUploadGetUrl1 = fileManager.GetFile(model.VideoUpload1FilePath);
        //                model.VideoUploadGetUrl2 = fileManager.GetFile(model.VideoUpload2FilePath);
        //            }

        //        }
        //        else
        //        {
        //            batteryViewModel.IsEdit = false;
        //            batteryViewModel.SwQcBatteryAssignIssueModelsList = generator.GetStaticList(projectId);
        //        }
        //        batteryViewModel.IsEdit = batteryViewModel.SwQcBatteryAssignIssueModelsList.Any() && batteryViewModel.SwQcBatteryAssignIssueModelsList[0].SwQcBatteryAssignIssuesId > 0;

        //    }
        //    else
        //    {
        //        batteryViewModel.IsEdit = false;
        //        batteryViewModel.ProjectMasterModel = new ProjectMasterModel();
        //        batteryViewModel.SwQcBatteryAssignIssueModelsList = generator.GetStaticList(projectId);
        //    }
        //    return View(batteryViewModel);

        //}
        //[HttpPost]
        //public ActionResult SwQcBattery(SwBatteryViewModel models)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name);
        //        using (var dbEntities = new CellPhoneProjectEntities())
        //        {
        //            if (models.IsEdit)
        //            {
        //                var manager = new FileManager();
        //                foreach (SwQcBatteryAssignIssueModel model in models.SwQcBatteryAssignIssueModelsList)
        //                {
        //                    model.Added = userId;
        //                    model.AddedDate = DateTime.Now;
        //                    model.Updated = userId;
        //                    model.UpdatedDate = DateTime.Now;

        //                    if (model.ScreenShots1File != null)
        //                    {
        //                        model.ScreenShot1FilePath = manager.Upload(
        //                            models.ProjectMasterModel.ProjectMasterId, "SwQcIssues", "IssuesImages",
        //                            model.ScreenShots1File);

        //                    }
        //                    if (model.ScreenShots2File != null)
        //                    {
        //                        model.ScreenShot2FilePath = manager.Upload(
        //                            models.ProjectMasterModel.ProjectMasterId, "SwQcIssues", "IssuesImages",
        //                            model.ScreenShots2File);

        //                    }
        //                    if (model.VideoUpload1File != null)
        //                    {
        //                        model.VideoUpload1FilePath = manager.Upload(
        //                            models.ProjectMasterModel.ProjectMasterId, "SwQcIssues", "IssuesImages",
        //                            model.VideoUpload1File);
        //                    }
        //                    if (model.VideoUpload2File != null)
        //                    {
        //                        model.VideoUpload2FilePath = manager.Upload(
        //                            models.ProjectMasterModel.ProjectMasterId, "SwQcIssues", "IssuesImages",
        //                            model.VideoUpload2File);
        //                    }
        //                    var dbValues =
        //                        dbEntities.SwQcBatteryAssignIssues.FirstOrDefault(
        //                            i => i.SwQcBatteryAssignIssuesId == model.SwQcBatteryAssignIssuesId);
        //                    if (dbValues != null)
        //                    {
        //                        dbValues.SwQcBatteryAssignIssuesId = model.SwQcBatteryAssignIssuesId;
        //                        dbValues.ProjectMasterId = model.ProjectMasterId;
        //                        dbValues.CheckingOption = model.CheckingOption;
        //                        dbValues.Time = model.Time;
        //                        dbValues.Charging = model.Charging;
        //                        dbValues.Voltage = model.Voltage;
        //                        dbValues.Decreased = model.Decreased;
        //                        dbValues.IsIssueChecked = model.IsIssueChecked;
        //                        dbValues.Issues = model.Issues;
        //                        dbValues.IssueComment = model.IssueComment;
        //                        dbValues.ScreenShots1 = model.ScreenShot1FilePath;
        //                        dbValues.ScreenShots2 = model.ScreenShot2FilePath;
        //                        dbValues.VideoUpload1 = model.VideoUpload1FilePath;
        //                        dbValues.VideoUpload2 = model.VideoUpload2FilePath;


        //                        dbEntities.Entry(dbValues).State = EntityState.Modified;
        //                    }
        //                }
        //                dbEntities.SaveChanges();
        //                return RedirectToAction("SwQcBattery",
        //                    new { projectId = models.ProjectMasterModel.ProjectMasterId, AssignId = models.AssignId });
        //            }
        //            else
        //            {
        //                var dbBatteryIssues = models.SwQcBatteryAssignIssueModelsList;
        //                foreach (var swQcBatteryAssignIssueModel in dbBatteryIssues)
        //                {
        //                    var manager = new FileManager();

        //                    if (swQcBatteryAssignIssueModel.ScreenShots1File != null)
        //                    {
        //                        swQcBatteryAssignIssueModel.ScreenShot1FilePath = manager.Upload(
        //                            models.ProjectMasterModel.ProjectMasterId, "SwQcIssues", "IssuesImages",
        //                            swQcBatteryAssignIssueModel.ScreenShots1File);

        //                    }
        //                    if (swQcBatteryAssignIssueModel.ScreenShots2File != null)
        //                    {
        //                        swQcBatteryAssignIssueModel.ScreenShot2FilePath = manager.Upload(
        //                            models.ProjectMasterModel.ProjectMasterId, "SwQcIssues", "IssuesImages",
        //                            swQcBatteryAssignIssueModel.ScreenShots2File);

        //                    }
        //                    if (swQcBatteryAssignIssueModel.VideoUpload1File != null)
        //                    {
        //                        swQcBatteryAssignIssueModel.VideoUpload1FilePath = manager.Upload(
        //                            models.ProjectMasterModel.ProjectMasterId, "SwQcIssues", "IssuesImages",
        //                            swQcBatteryAssignIssueModel.VideoUpload1File);
        //                    }
        //                    if (swQcBatteryAssignIssueModel.VideoUpload2File != null)
        //                    {
        //                        swQcBatteryAssignIssueModel.VideoUpload2FilePath = manager.Upload(
        //                            models.ProjectMasterModel.ProjectMasterId, "SwQcIssues", "IssuesImages",
        //                            swQcBatteryAssignIssueModel.VideoUpload2File);
        //                    }
        //                    var swQcBatterys = new SwQcBatteryAssignIssue
        //                    {

        //                        ProjectMasterId = swQcBatteryAssignIssueModel.ProjectMasterId,
        //                        SwQcAssignId = models.AssignId,
        //                        ModuleName = swQcBatteryAssignIssueModel.ModuleName,
        //                        CheckingOption = swQcBatteryAssignIssueModel.CheckingOption,
        //                        Time = swQcBatteryAssignIssueModel.Time,
        //                        Charging = swQcBatteryAssignIssueModel.Charging,
        //                        Voltage = swQcBatteryAssignIssueModel.Voltage,
        //                        Decreased = swQcBatteryAssignIssueModel.Decreased,
        //                        IsIssueChecked = swQcBatteryAssignIssueModel.IsIssueChecked,
        //                        Issues = swQcBatteryAssignIssueModel.Issues,
        //                        IssueComment = swQcBatteryAssignIssueModel.IssueComment,
        //                        ScreenShots1 = swQcBatteryAssignIssueModel.ScreenShot1FilePath,
        //                        ScreenShots2 = swQcBatteryAssignIssueModel.ScreenShot2FilePath,
        //                        VideoUpload1 = swQcBatteryAssignIssueModel.VideoUpload1FilePath,
        //                        VideoUpload2 = swQcBatteryAssignIssueModel.VideoUpload2FilePath,
        //                        Added = userId,
        //                        AddedDate = DateTime.Now

        //                    };

        //                    try
        //                    {
        //                        dbEntities.SwQcBatteryAssignIssues.Add(swQcBatterys);


        //                    }
        //                    catch (Exception exception)
        //                    {
        //                        var ss = exception;
        //                    }


        //                }
        //                dbEntities.SaveChanges();
        //                //    return RedirectToAction("SwQcSpecification", new { projectId = swQcSpecification.ProjectMasterModel.ProjectMasterId, AssignId = swQcSpecification.AssignId, tabName = swQcSpecification.Tabname });

        //                return RedirectToAction("SwQcBattery",
        //                   new { projectId = models.ProjectMasterModel.ProjectMasterId, AssignId = models.AssignId });

        //            }////for else


        //        } ///for using


        //    }
        //    return View(models);




        //}
        //public JsonResult QcInchargeToQcReAssignProject(string projectMasterId, string projectName, string swqcInchargeAId, string[] multiple1, string ApproxInchargeToQcDeliveryDate, string SwInchargeAssignToQcComment, string projectPmAssignId)
        //{

        //    long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name);

        //    if (userId > 0)
        //    {
        //        long pMasterId;
        //        long.TryParse(projectMasterId, out pMasterId);


        //        var notificationObject = new NotificationObject
        //        {
        //            ProjectId = pMasterId,
        //        };
        //        //  var urls = model.UploadedFile.Split('|').ToList();
        //        // var multiple11 = multiple1.Split(',').ToList();

        //        foreach (string mId in multiple1)
        //        {
        //            notificationObject.ToUser = notificationObject.ToUser + mId + ",";
        //        }
        //        notificationObject.Message = "re-assigned for Checking Software Issues";

        //        long pSwQcInId;
        //        long.TryParse(swqcInchargeAId, out pSwQcInId);

        //        long pPrPmAssignId;
        //        long.TryParse(projectPmAssignId, out pPrPmAssignId);

        //        var sWQcInchargeToQCProjectReAssignResult = _repository.SaveQcInchargeToQcReAssignProject(pMasterId, projectName, pSwQcInId, multiple1, ApproxInchargeToQcDeliveryDate, SwInchargeAssignToQcComment, pPrPmAssignId);

        //        ViewBag.ControllerVariable = notificationObject;
        //        return Json(sWQcInchargeToQCProjectReAssignResult, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(new { result = "Redirect", url = Url.Action("Software", "SwInchargeProjectSubmit") });
        //}
        #endregion

        #region excel
        public static DataTable GetFirstRow(string projectId, string softVersionName, long testIds, int swVersionNo)
        {
            DataTable totalhistry = new DataTable();
            var cn = new SqlConnection(_connectionStringCellphone);

            //var cn =new SqlConnection("Data Source=test;initial catalog=CellPhoneProject;persist security info=True;user id=test;password=test;MultipleActiveResultSets=True;App=EntityFramework");
            //var cn =
            //    new SqlConnection(
            //        "Data Source=test;initial catalog=CellPhoneProject;persist security info=True;user id=test;password=test6;MultipleActiveResultSets=True;App=EntityFramework");


            cn.Open();

            //totalhistry.Columns.Add("Model");
            //totalhistry.Columns.Add("Software Version Name");
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
                //sql = String.Format(@" select * from CellPhoneProject.dbo.SwQcIssueDetails where ProjectName='{0}' and IsApprovedForChina=1 order by IssueSerial asc", projectId, softVersionName);
                //                sql = String.Format(@" 
                //                select distinct SwQcIssueId,IssueSerial,SoftwareVersionNo,SoftwareVersionName,IssueScenario,ExpectedOutcome,Frequency,IssueReproducePath,Attachment,IssueType,WaltonQcComDate,WaltonQcComment,WaltonQcStatus,
                //                FixedVersion,SupplierComDate,SupplierStatus,SupplierComment,WaltonPmComDate,WaltonPmComment
                //                from CellPhoneProject.dbo.SwQcIssueDetails sii where ProjectName='{0}' and (SoftwareVersionName is not null and SoftwareVersionName != '')
                //                and IsApprovedForChina=1 and SwQcIssueId in (select  top 1  SwQcIssueId from CellPhoneProject.dbo.SwQcIssueDetails where IssueSerial=sii.IssueSerial and ProjectName='{0}' and IsApprovedForChina=1 and (SoftwareVersionName is not null and SoftwareVersionName != '') order by SwQcIssueId desc)
                //                order by IssueSerial asc", projectId, softVersionName);
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
                //                sql = String.Format(@"select distinct SwQcIssueId,IssueSerial,SoftwareVersionNo,SoftwareVersionName,IssueScenario,ExpectedOutcome,Frequency,IssueReproducePath,Attachment,IssueType,WaltonQcComDate,WaltonQcComment,WaltonQcStatus,
                //                FixedVersion,SupplierComDate,SupplierStatus,SupplierComment,WaltonPmComDate,WaltonPmComment
                //                from CellPhoneProject.dbo.SwQcIssueDetails sii where ProjectName='{0}' and (SoftwareVersionName is not null and SoftwareVersionName != '')
                //                and IsApprovedForChina=1 and SwQcIssueId in (select  top 1  SwQcIssueId from CellPhoneProject.dbo.SwQcIssueDetails where IssueSerial=sii.IssueSerial and ProjectName='{0}' 
                //                and IsApprovedForChina=1 and (SoftwareVersionName is not null and SoftwareVersionName != '')  and 
                //                SoftwareVersionNo in (select  top 1  SoftwareVersionNo from CellPhoneProject.dbo.SwQcIssueDetails where IssueSerial=sii.IssueSerial and ProjectName='{0}' 
                //                and IsApprovedForChina=1 and (SoftwareVersionName is not null and SoftwareVersionName != '')  order by SoftwareVersionNo desc)
                //                order by SwQcIssueId desc)
                //                order by IssueSerial asc", projectId.Trim(), softVersionName);

            }
            else
            {
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

            cmd.CommandTimeout = 6000;
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
        public void GetExcel(DataTable ds, string projectId, int swVersionNo, long testPhaseIds)
        {
            var vms = new VmSwQcSpecificationModified();
            vms.SwQcHeadAssignsFromPmModels = _repository.GetProjectVersionName(projectId, swVersionNo, testPhaseIds);
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

                excelWorkSheet.Range["A2", "Q2"].Application.ActiveWindow.SplitRow = 2;
                //ok into test
               // excelWorkSheet.Range["A2", "Q2"].Rows.Application.ActiveWindow.FreezePanes = true;
                

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
                //ok into test
               // excelWorkSheet.Range["A1", "C1"].EntireColumn.Application.ActiveWindow.FreezePanes = true;
                

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
                //end Issue List Color Group 3

                DataTable dt = new DataTable();
                using (dt = GetFirstRow(projectId, swVersionsName, testIds, swVersionNo))
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

            //  ViewBag.GetExcelsNames = _repository.GetExcelsNames(projectId, swVersionNo, testPhaseIds);

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

        #region com2
        // public void GetExcel(DataTable ds, string projectId)
        // {
        //     var vms = new VmSwQcSpecificationModified();
        //     vms.SwQcHeadAssignsFromPmModels = _repository.GetProjectVersionName(projectId);

        //     var fileInfo = new FileInfo(Path.GetTempPath() + "\\" +
        //                         DateTime.Now.Ticks + ".xlsx");

        //     //  var xls = new ExcelPackage(fileInfo);
        //     //Creae an Excel application instance
        //     Excel.Application excelApp = new Excel.Application();

        //     Excel.Workbook excelWorkBook = excelApp.Workbooks.Add("");
        //     //excelWorkBook.SaveAs(projectId + DateTime.Now.ToString("yyyyMMdd_hhmmss"), Excel.XlSaveAsAccessMode.xlNoChange);
        //     // excelWorkBook.SaveAs(projectId + DateTime.Now.ToString("yyyyMMdd_hhmmss"));
        //     //excelWorkBook.SaveAs(@"C:\" + projectId + DateTime.Now.ToString("yyyyMMdd_hhmmss"));

        //     //string myFileName =
        //     //    Server.MapPath(@"~/Content/UploadImage/" + projectId + DateTime.Now.ToString("yyyyMMdd_hhmmss") +
        //     //                   ".xlsx");
        //     //excelWorkBook = excelApp.Workbooks.Add(myFileName);


        //     DataSet dss = new DataSet();
        //     //else
        //     //{
        //     //    Response.Write("This file does not exist.");
        //     //}

        //     // excelApp.DisplayAlerts = false;
        //     //excelWorkBook.SaveAs(projectId + DateTime.Now.ToString("yyyyMMdd_hhmmss"), Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing,
        //     //    Type.Missing, true, false, Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlLocalSessionChanges, Type.Missing,
        //     //    Type.Missing);

        //     foreach (var table in vms.SwQcHeadAssignsFromPmModels)
        //     {
        //         //Add a new worksheet to workbook with the Datatable name
        //         Excel.Worksheet excelWorkSheet = excelWorkBook.Sheets.Add();

        //         excelWorkSheet.Name = table.SoftwareVersionName;

        //         //Excel design//


        //         //freeze row//

        //         excelWorkSheet.Range["A2", "O2"].Application.ActiveWindow.SplitRow = 2;
        //         excelWorkSheet.Range["A2", "O2"].Rows.Application.ActiveWindow.FreezePanes = true;

        //         //Adjust all row
        //         excelWorkSheet.Rows.AutoFit();

        //         //Column width and Hight//
        //         excelWorkSheet.Range["A2", "O2"].Rows.RowHeight = 40;

        //         excelWorkSheet.Columns[1].ColumnWidth = 30;
        //         excelWorkSheet.Columns[2].ColumnWidth = 30;

        //         excelWorkSheet.Columns[4].ColumnWidth = 25;
        //         excelWorkSheet.Columns[5].ColumnWidth = 25;
        //         excelWorkSheet.Columns[9].ColumnWidth = 20;
        //         excelWorkSheet.Columns[10].ColumnWidth = 20;
        //         excelWorkSheet.Columns[13].ColumnWidth = 20;

        //         excelWorkSheet.Columns[3].ColumnWidth = 15;
        //         excelWorkSheet.Columns[6].ColumnWidth = 15;
        //         excelWorkSheet.Columns[7].ColumnWidth = 20;
        //         excelWorkSheet.Columns[8].ColumnWidth = 20;
        //         excelWorkSheet.Columns[11].ColumnWidth = 20;
        //         excelWorkSheet.Columns[12].ColumnWidth = 20;
        //         excelWorkSheet.Columns[14].ColumnWidth = 20;
        //         excelWorkSheet.Columns[15].ColumnWidth = 20;
        //         /////////

        //         //wrap text//
        //         excelWorkSheet.get_Range("A2", "B2").Style.WrapText = true;

        //         //freeze column//

        //         excelWorkSheet.Range["A1", "B1"].Application.ActiveWindow.SplitColumn = 2;
        //         excelWorkSheet.Range["A1", "B1"].EntireColumn.Application.ActiveWindow.FreezePanes = true;

        //         //Adjust all column
        //         excelWorkSheet.Columns.AutoFit();

        //         //For Header Name//
        //         excelWorkSheet.Cells[1, 2] = projectId;
        //         excelWorkSheet.get_Range("A1", "I1").Font.Bold = true;
        //         excelWorkSheet.get_Range("A1", "I1").Font.Name = "Calibri";
        //         excelWorkSheet.get_Range("A1", "I1").Font.Size = 32;
        //         excelWorkSheet.get_Range("A1", "I1").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
        //         excelWorkSheet.get_Range("A1", "I1").HorizontalAlignment =
        //Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
        //         //excelWorkSheet.get_Range("A1", "I1").Columns.AutoFit();



        //         //For Issue List Color Group 1//
        //         excelWorkSheet.get_Range("A2", "F2").Font.Bold = true;
        //         excelWorkSheet.get_Range("A2", "F2").Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.MediumPurple);
        //         excelWorkSheet.get_Range("A2", "F2").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
        //         excelWorkSheet.get_Range("A2", "F2").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
        //         // excelWorkSheet.get_Range("A2", "F2").Columns.AutoFit();
        //         excelWorkSheet.get_Range("A2", "F2").Application.StandardFont = "Calibri";
        //         excelWorkSheet.get_Range("A2", "F2").Application.StandardFontSize = 11;
        //         //end Issue List Color Group 1

        //         //For Issue List Color Group 2//
        //         excelWorkSheet.get_Range("G2", "J2").Font.Bold = true;
        //         excelWorkSheet.get_Range("G2", "J2").Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Green);
        //         excelWorkSheet.get_Range("G2", "J2").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
        //         excelWorkSheet.get_Range("G2", "J2").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
        //         //  excelWorkSheet.get_Range("G2", "J2").Columns.AutoFit();
        //         excelWorkSheet.get_Range("G2", "J2").Application.StandardFont = "Calibri";
        //         excelWorkSheet.get_Range("G2", "J2").Application.StandardFontSize = 11;
        //         //end Issue List Color Group 2

        //         //For Issue List Color Group 3//
        //         excelWorkSheet.get_Range("K2", "M2").Font.Bold = true;
        //         excelWorkSheet.get_Range("K2", "M2").Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Tomato);
        //         excelWorkSheet.get_Range("K2", "M2").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
        //         excelWorkSheet.get_Range("K2", "M2").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
        //         // excelWorkSheet.get_Range("K2", "M2").Columns.AutoFit();
        //         excelWorkSheet.get_Range("K2", "M2").Application.StandardFont = "Calibri";
        //         excelWorkSheet.get_Range("K2", "M2").Application.StandardFontSize = 11;
        //         //end Issue List Color Group 3

        //         //For Issue List Color Group 3//
        //         excelWorkSheet.get_Range("N2", "O2").Font.Bold = true;
        //         excelWorkSheet.get_Range("N2", "O2").Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DodgerBlue);
        //         excelWorkSheet.get_Range("N2", "O2").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
        //         excelWorkSheet.get_Range("N2", "O2").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
        //         //  excelWorkSheet.get_Range("N2", "O2").Columns.AutoFit();
        //         excelWorkSheet.get_Range("N2", "O2").Application.StandardFont = "Calibri";
        //         excelWorkSheet.get_Range("N2", "O2").Application.StandardFontSize = 11;
        //         //end Issue List Color Group 3



        //         DataTable dt = new DataTable();
        //         using (dt = GetFirstRow(projectId, excelWorkSheet.Name))
        //         {
        //             var qcF = "";
        //             var supF = "";
        //             for (int i = 1; i < dt.Columns.Count + 1; i++)
        //             {
        //                 //excelWorkSheet.Cells[1, i] = dt.Columns[i - 1].ColumnName;
        //                 excelWorkSheet.Cells[2, i] = dt.Columns[i - 1].ColumnName;
        //             }

        //             for (int j = 0; j < dt.Rows.Count; j++)
        //             {
        //                 for (int k = 0; k < dt.Columns.Count; k++)
        //                 {
        //                     //excelWorkSheet.Cells[j + 2, k + 1] = dt.Rows[j].ItemArray[k].ToString();
        //                     excelWorkSheet.Cells[j + 3, k + 1] = dt.Rows[j].ItemArray[k].ToString();

        //                     qcF = excelWorkSheet.Cells[2, 8].Value2;
        //                     supF = excelWorkSheet.Cells[2, 12].Value2;

        //                     if (dt.Rows[j].ItemArray[k].ToString() == "CRITICAL" || dt.Rows[j].ItemArray[k].ToString() == "NOT FIXED")  // CHECKING CONDITION WITH THE DATATABLE
        //                     {
        //                         // USE TO COLOR THE ROW AND THE COLUMN RANGE 
        //                         excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(255, 0, 0);
        //                         excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
        //                     }
        //                     else if (dt.Rows[j].ItemArray[k].ToString() == "MAJOR")
        //                     {
        //                         excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(142, 169, 219);
        //                         excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
        //                     }
        //                     else if (dt.Rows[j].ItemArray[k].ToString() == "MINOR")
        //                     {
        //                         excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(0, 176, 240);
        //                         excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
        //                     }
        //                     else if (dt.Rows[j].ItemArray[k].ToString() == "IMPROVED")
        //                     {
        //                         excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(255, 255, 0);
        //                         excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
        //                     }
        //                     else if (dt.Rows[j].ItemArray[k].ToString() == "OPTIMIZED")
        //                     {
        //                         excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(255, 242, 204);
        //                         excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
        //                     }
        //                     //|| dt.Rows[j].ItemArray[k].ToString() == "FIXED" || dt.Rows[j].ItemArray[k].ToString() == "SUPPLIER CAN'T FIXED"
        //                     else if (dt.Rows[j].ItemArray[k].ToString() == "FIXED" && qcF == "Walton QC Status")
        //                     {
        //                         excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(0, 176, 80);
        //                         excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
        //                         //for (int i = 1; i < dt.Columns.Count + 1; i++)
        //                         //{
        //                         //    //excelWorkSheet.Cells[2, i] = dt.Columns[i - 1].ColumnName;
        //                         //    if (dt.Columns[i - 1].ColumnName == "Supplier Status")
        //                         //    {
        //                         //        excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(0, 176, 240);
        //                         //        excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
        //                         //    }
        //                         //    if (dt.Columns[i - 1].ColumnName == "Walton QC Status")
        //                         //    {
        //                         //        excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(0, 176, 80);
        //                         //        excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
        //                         //    }

        //                         //}
        //                     }
        //                     else if (dt.Rows[j].ItemArray[k].ToString() == "FIXED" && supF == "Supplier Status")
        //                     {
        //                         excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(0, 176, 240);
        //                         excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
        //                     }
        //                     //else if (dt.Rows[j].ItemArray[k].ToString() == "FIXED")
        //                     //{
        //                     //    excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(0, 176, 80);
        //                     //    excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
        //                     //}

        //                     else if (dt.Rows[j].ItemArray[k].ToString() == "SUPPLIER CAN'T FIXED")
        //                     {
        //                         excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(0, 176, 240);
        //                         excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
        //                         //for (int i = 1; i < dt.Columns.Count + 1; i++)
        //                         //{
        //                         //    //excelWorkSheet.Cells[2, i] = dt.Columns[i - 1].ColumnName;
        //                         //    if (dt.Columns[i - 1].ColumnName == "Supplier Status")
        //                         //    {
        //                         //        excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(255, 0, 0);
        //                         //        excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
        //                         //    }
        //                         //    if (dt.Columns[i - 1].ColumnName == "Walton QC Status")
        //                         //    {
        //                         //        excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.Color.FromArgb(0, 176, 240);
        //                         //        excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;
        //                         //    }

        //                         //}
        //                     }


        //                 }
        //             }

        //         }
        //         dss.Tables.Add(dt);
        //     }
        //     //  //excelWorkBook.Save();

        //     ////  excelWorkBook.SaveAs(projectId + DateTime.Now.ToString("yyyyMMdd_hhmmss"));
        //     //  excelApp.DisplayAlerts = false;
        //     //  excelWorkBook.SaveAs(projectId + DateTime.Now.ToString("yyyyMMdd_hhmmss"));
        //     //  //excelWorkBook.SaveAs(projectId + DateTime.Now.ToString("yyyyMMdd_hhmmss"), Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, 
        //     //  //Type.Missing, true, false,
        //     //  //Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlLocalSessionChanges, Type.Missing, Type.Missing);
        //     //  excelWorkBook.Close();
        //     //  excelApp.Quit();
        //     //excelWorkBook.SaveAs(projectId + DateTime.Now.ToString("yyyyMMdd_hhmmss"));
        //     //excelWorkBook.Close();
        //     //excelApp.Quit();

        //     //  Response.Clear();
        //     //  Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //     //  excelWorkBook.Save();
        //     //  Response.Write(Response.OutputStream);
        //     ////  excelWorkBook.Close();
        //     //  Response.End();
        //     //var count = 0;
        //     //string tmpPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\EET_Commande_Fournisseur";
        //     //if (!Directory.Exists(tmpPath))
        //     //{
        //     //    Directory.CreateDirectory(tmpPath);
        //     //}
        //     //foreach (string file in Directory.GetFiles(tmpPath))
        //     //{
        //     //    if ((tmpPath + ".xls").Equals(file.Substring(tmpPath.Length + 1)))
        //     //        count++;
        //     //}
        //     //if (count > 0)
        //     //    excelWorkBook.SaveAs(string.Format("EET_Commande_Fournisseur\\{0}_({1}).xls", path, count), Excel.XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        //     //else
        //     //    excelWorkBook.SaveAs(string.Format("EET_Commande_Fournisseur\\{0}.xls", path) + path, Excel.XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

        //     //GC.Collect();
        //     //GC.WaitForPendingFinalizers();

        //     //new//
        //     //excelWorkBook.SaveAs(projectId + DateTime.Now.ToString("yyyyMMdd_hhmmss"));
        //     ////excelApp.DefaultSaveFormat = Excel.XlFileFormat.xlOpenXMLWorkbook;
        //     ////excelWorkBook.Save();
        //     //excelWorkBook.Close();
        //     //excelApp.Quit();
        //     //new//

        //     //string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), projectId);
        //     //Response.Clear();
        //     //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //     //Response.TransmitFile(path);
        //     //Response.End();

        //     //string filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),"Excels");
        //     ////var fileName =  projectId + DateTime.Now.ToString("yyyyMMdd_hhmmss")+".xlsx";
        //     //var fileName = projectId+"_" + DateTime.Now.ToString("yyyyMMdd_hhmmss");
        //     //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //     //Response.AppendHeader("Content-Disposition", "attachment; filename="+ fileName);
        //     //Response.Write(Response.OutputStream);
        //     //Response.End();


        //     //Response.ClearContent();
        //     // Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //     //// Response.AddHeader("content-disposition", "attachment;filename=Contact.xls");
        //     // //Response.AddHeader("Content-Type", "application/vnd.ms-excel");
        //     // Response.AddHeader("Content-Disposition", "attachment; filename=" + projectId+".xlsx");
        //     // Response.Write(Response.OutputStream);
        //     // Response.End();


        //     //    var path = Excel.Windows;
        //     //string fullPath = Path.Combine(path, fileName);
        //     //return fullPath;

        //     string dd = projectId + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xlsx";
        //     //string myFileName =
        //     //Server.MapPath(@"~/Content/UploadImage/" + projectId + DateTime.Now.ToString("yyyyMMdd_hhmmss") +
        //     //               ".xlsx");
        //     // var files1 = Directory.GetFiles(@"C:\Downloads\Excels");
        //     //var files2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Excels");
        //     //var ss=files2+"\"+dd;

        //     // var files2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyComputer), "Excels");
        //     string files2 = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        //     excelWorkBook.SaveAs(files2 + "\\" + dd);
        //     //excelWorkBook.SaveAs(files1 +"\\"+ projectId + DateTime.Now.ToString("yyyyMMdd_hhmmss") +
        //     //               ".xlsx");
        //     excelWorkBook.Close();
        //     excelApp.Quit();

        //     //const string initialPath = @"~/Content/UploadImage";
        //     //var path = Path.Combine(Server.MapPath(initialPath), dd);

        //     //string finalPath = @"C:\uploads" + @"\" + projectId + DateTime.Now.ToString("yyyyMMdd_hhmmss");

        //     //if (Directory.Exists(finalPath))
        //     //{
        //     //    //sfiles.GetFileName().Copy(path, finalPath + "\\" + dd, true);
        //     //    File(dd, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", dd+".xlsx");
        //     //}
        //     //else
        //     //{
        //     //    Directory.CreateDirectory(finalPath);
        //     //    //File.Copy(path, finalPath + "\\" + dd, true);
        //     //    File(dd, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", dd+".xlsx");
        //     //}
        //     //  return finalPath + "\\" + fileName;

        //     try
        //     {
        //         string XlsPath = Server.MapPath(files2 + "\\" + dd);
        //         FileInfo fileDet = new System.IO.FileInfo(XlsPath);
        //         Response.Clear();
        //         Response.Charset = "UTF-8";
        //         Response.ContentEncoding = Encoding.UTF8;
        //         Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.UrlEncode(fileDet.Name));
        //         Response.AddHeader("Content-Length", fileDet.Length.ToString());
        //         Response.ContentType = "application/ms-excel";
        //         Response.WriteFile(fileDet.FullName);
        //         Response.End();
        //     }
        //     catch (Exception ex)
        //     {
        //         throw ex;
        //     }

        // }
        #endregion
        #endregion

        #region Edit

        [HttpPost]
        public JsonResult EditSwQcIssueDetails(String ProjectMasterId, String SwQcIssueId, String IssueScenario,
            String ExpectedOutcome, String Result, String RefernceModule,
            String IssueReproducePath, String Attachment, String IssueType, String FilesUrl, String Frequency)
        {
            var _dbEntities = new CellPhoneProjectEntities();

            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name; ;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            var user = _dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userId);

            long proId = 0;
            long.TryParse(ProjectMasterId, out proId);
            long swIssueId = 0;
            long.TryParse(SwQcIssueId, out swIssueId);

            var saveIncentive = _repository.EditSwQcIssueDetails(proId, swIssueId, IssueScenario, ExpectedOutcome, Result, RefernceModule, IssueReproducePath, Attachment
                , IssueType, FilesUrl, Frequency);

            return new JsonResult { Data = saveIncentive, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
       
        [HttpPost]
        public JsonResult ForwardSwQcIssues(String SwQcIssueId, String ProjectName, String IssueSerial, String IssueScenario, String SoftwareVersionNo,
            String TestPhaseID, String WaltonQcStatus, String WaltonQcComment, String SupplierComment)
        {
            var _dbEntities = new CellPhoneProjectEntities();


            var saveIncentive = "";
            long SwQcIssueIds = 0;
            long.TryParse(SwQcIssueId, out SwQcIssueIds);

            int SoftwareVersionNos = 0;
            int.TryParse(SoftwareVersionNo, out SoftwareVersionNos);
            long IssueSerials = 0;
            long.TryParse(IssueSerial, out IssueSerials);
            long TestPhaseIDs = 0;
            long.TryParse(TestPhaseID, out TestPhaseIDs);

            if (IssueSerials != 0 && SoftwareVersionNos != 0)
            {
                saveIncentive = _repository.ForwardSwQcIssues(SwQcIssueIds, ProjectName, IssueSerials, IssueScenario, SoftwareVersionNos, TestPhaseIDs, WaltonQcStatus, WaltonQcComment, SupplierComment);
            }

            return new JsonResult { Data = saveIncentive, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion

        #region QcHead Issue Create

        public ActionResult QcHeadIssueDetails(string proName, string swVersionNo, string testPhaseNameId)
        {
            var vmSwQcSpecification = new VmSwQcSpecificationModified();
            long userId = Convert.ToInt64(User.Identity.Name);

            int softVersionNo;
            int.TryParse(swVersionNo, out softVersionNo);

            long testPhaseId;
            long.TryParse(testPhaseNameId, out testPhaseId);

            //Project Name
            vmSwQcSpecification.ProjectMasterModelsList = _repository.GetProjectListForQcHeadIssue();

            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Select Project" } };

            var query = (from master in vmSwQcSpecification.ProjectMasterModelsList
                         select new
                         {
                             master.ProjectName,
                             master.SwQcAssignId,
                             master.SwQcHeadAssignId,
                             master.ProjectMasterId
                         }).ToList();

            foreach (var t in query)
            {
                String selectedValue = t.ProjectMasterId + "," + t.SwQcHeadAssignId + "," + t.SwQcAssignId + "," + t.ProjectName;

                selectListItems.Add(new SelectListItem
                {
                    Value = selectedValue,
                    Text = t.ProjectName
                });
            }
            ViewBag.CombinedIds = selectListItems;
            //Test Phase Name
            vmSwQcSpecification.SwQcTestPhaseModels = _repository.GetTestPhasesForQcHeadIssue();

            var selectListItems1 = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Select Test Phase" } };

            var query1 = (from master in vmSwQcSpecification.SwQcTestPhaseModels
                          select new
                          {
                              master.TestPhaseName,
                              master.TestPhaseID,
                          }).ToList();

            foreach (var t in query1)
            {
                // String selectedValue = t.ProjectMasterId +  "," + t.SoftwareVersionNo + "," + t.OrderNuber+","+t.ProjectName;

                selectListItems1.Add(new SelectListItem
                {
                    Value = t.TestPhaseID.ToString(),
                    Text = t.TestPhaseName
                });
            }
            ViewBag.CombinedIdsForTestPhase = selectListItems1;
            //
            var selectListItemsForRef = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT" } };
            List<SwQcTestPhaseModel> list = _repository.GetSwQcTestPhase() ??
                                                     new List<SwQcTestPhaseModel>();
            selectListItemsForRef.AddRange(list.Select(p => new SelectListItem { Value = p.TestPhaseName, Text = p.TestPhaseName }));
            ViewBag.ddlReferenceListForModal = selectListItemsForRef;
            //issue load
            var fileManager = new FileManager();
            vmSwQcSpecification.SwQcIssueDetailModels =
                       _repository.GetSwQcIssuesForHead(proName, testPhaseNameId, softVersionNo);


            if (vmSwQcSpecification.SwQcIssueDetailModels != null)
            {
                foreach (SwQcIssueDetailModel model in vmSwQcSpecification.SwQcIssueDetailModels)
                {
                    if (model.UploadedFile != null)
                    {
                        var urls = model.UploadedFile.Split('|').ToList();
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

            //
            vmSwQcSpecification.CombinedProjectId = proName;
            vmSwQcSpecification.CombinedProjectIds = testPhaseNameId;
            vmSwQcSpecification.SoftwareVersionNumber = softVersionNo;

            return View(vmSwQcSpecification);
        }

        [HttpPost]
        public ActionResult QcHeadIssueDetails(List<SwQcIssueDetailModel> issueList, List<SwQcIssueDetailModel> issueList1, string issueChk, string fileChk, string proName, string pro_id, string swQcIncharge_id, string swQcAssign_id, string projectName, string TestPhaseId, string SoftwareVersionName)
        {
            //issueList,issueList1,issueChk
            issueList = issueList.Where(x => x.IsRemoved == 0).ToList();
            long userId = Convert.ToInt64(User.Identity.Name);
            if (ModelState.IsValid)
            {

                long pMasterId;
                long.TryParse(pro_id, out pMasterId);

                long pSwQcInId;
                long.TryParse(swQcIncharge_id, out pSwQcInId);

                long pSwQcAssignId;
                long.TryParse(swQcAssign_id, out pSwQcAssignId);

                long TestPhaseIds;
                long.TryParse(TestPhaseId, out TestPhaseIds);

                int SoftwareVersionNames;
                int.TryParse(SoftwareVersionName, out SoftwareVersionNames);


                bool issuesChk;
                bool.TryParse(issueChk, out issuesChk);

                bool filesChk;
                bool.TryParse(fileChk, out filesChk);

                var manager = new FileManager();

                foreach (var swQcIssueDetails in issueList)
                {
                    if (swQcIssueDetails.File.Count() > 0 && swQcIssueDetails.File != null)
                    {
                        var res = manager.Upload3(pMasterId, pSwQcInId, pSwQcAssignId, "QcHeadIssueDetails",
                            "SwQcsIssuesImage", swQcIssueDetails.File);
                        swQcIssueDetails.UploadedFile = swQcIssueDetails.UploadedFile == null ? res : swQcIssueDetails.UploadedFile + "|" + res;

                    }

                }

                foreach (var swQcIssueDetails in issueList1)
                {
                    if (swQcIssueDetails.File.Count() > 0 && swQcIssueDetails.File != null)
                    {
                        var res = manager.Upload3(pMasterId, pSwQcInId, pSwQcAssignId, "QcHeadIssueDetails",
                            "SwQcsIssuesImage", swQcIssueDetails.File);
                        swQcIssueDetails.UploadedFile = swQcIssueDetails.UploadedFile == null ? res : swQcIssueDetails.UploadedFile + "|" + res;

                    }

                    if (swQcIssueDetails.RefernceModules != null)
                    {

                        foreach (var resf in swQcIssueDetails.RefernceModules)
                        {
                            swQcIssueDetails.RefernceModules1 = swQcIssueDetails.RefernceModules1 == null ? resf : swQcIssueDetails.RefernceModules1 + "," + resf;

                        }

                    }


                }
                _repository.SaveIssueDetailsForQcHead(issueList, issueList1, issuesChk, filesChk, pMasterId, pSwQcInId, pSwQcAssignId, projectName, TestPhaseIds, SoftwareVersionNames);
            }
            var pro = "proName=" + proName + "&swVersionNo=" + SoftwareVersionName + "&testPhaseNameId=" + TestPhaseId;


            int SoftwareVersionNames1;
            int.TryParse(SoftwareVersionName, out SoftwareVersionNames1);
            var vmSwQcSpecification = new VmSwQcSpecificationModified();
            vmSwQcSpecification.CombinedProjectId = proName;
            vmSwQcSpecification.CombinedProjectIds = TestPhaseId;
            vmSwQcSpecification.SoftwareVersionNumber = SoftwareVersionNames1;
            //  return RedirectToAction("SwQcSpecification", new { projectId = swQcSpecification.ProjectMasterModel.ProjectMasterId, AssignId = swQcSpecification.AssignId, tabName = swQcSpecification.Tabname, projectType = swQcSpecification.projectType });

            // return RedirectToAction("QcHeadIssueDetails", new { proName = proName + ',' + SoftwareVersionNames1 + ',' + TestPhaseId });
            return RedirectToAction("QcHeadIssueDetails", new { proName = proName, swVersionNo = SoftwareVersionNames1, testPhaseNameId = TestPhaseId });

        }

        #endregion

        #region new innovation delete and edit
        [HttpPost]
        public JsonResult DeleteQcInnovation(SwQcNewInnovationModel supplierUpdate)
        {
            var vmSwQcSpecification = new VmSwQcSpecificationModified();
            //var saveIncentive = _repository.SwQcIssueDelete(supplierUpdate);
            var newInno = _repository.DeleteQcInnovation(supplierUpdate);

            return new JsonResult { Data = newInno, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult EditInnovationDetails(String newInnoId, String ProjectName, String AssignedBy1,
            String[] AssignedBy, String Description, String WorkType, DateTime EffectiveDate)
        {
            var _dbEntities = new CellPhoneProjectEntities();

            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name; ;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            long newInnoIds;
            long.TryParse(newInnoId, out newInnoIds);

            var AssignedBys = "";
            var AssignedBy2 = "";
            var saveIncentive = "";
            if (newInnoIds != 0)
            {
                if (AssignedBy != null)
                {
                    foreach (var resf in AssignedBy)
                    {
                        AssignedBys = AssignedBys == null ? resf : AssignedBys + "," + resf;

                    }
                }

                AssignedBy2 = AssignedBys.TrimStart(',');

                saveIncentive = _repository.EditInnovationDetails(newInnoIds, ProjectName, AssignedBy1, AssignedBy2, Description, WorkType, EffectiveDate);

            }

            return new JsonResult { Data = saveIncentive, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion

        #region Excel Supplier Feedback
        //Added By Fahad

        [HttpGet]
        public ActionResult ExcelUploaderQcSuppFeed()
        {
            var swQcIssueExcelModel = new SwQcIssueExcelModel();

            long userId = Convert.ToInt64(User.Identity.Name);

            var modelList = new List<ProjectMasterModel>();

            modelList = _repository.GetProjectListForSwQcHead();

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
            List<SwQcTestPhaseModel> listTestPhase = _repository.GetSwQcTestPhaseForSuppDemo() ??
                                                     new List<SwQcTestPhaseModel>();
            selectListItemsTestPhase.AddRange(listTestPhase.Select(p => new SelectListItem { Value = p.TestPhaseID.ToString(), Text = p.TestPhaseName }));
            ViewBag.CombinedIdsForTestPhase = selectListItemsTestPhase;

            return View();
        }

        [HttpPost]
        public ActionResult ExcelUploaderQcSuppFeed(SwQcIssueExcelModel swQcIssueExcelModel)
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
            List<SwQcTestPhaseModel> listTestPhase = _repository.GetSwQcTestPhaseForSuppDemo() ??
                                                     new List<SwQcTestPhaseModel>();
            selectListItemsTestPhase.AddRange(listTestPhase.Select(p => new SelectListItem { Value = p.TestPhaseID.ToString(), Text = p.TestPhaseName }));
            ViewBag.CombinedIdsForTestPhase = selectListItemsTestPhase;

            var result = _repository.UpdateDbByExcel(swQcIssueExcelModel.SelectedProjectName, swQcIssueExcelModel.SoftVersionNo, swQcIssueExcelModel.ExcelFile, swQcIssueExcelModel.CombinedTestPhaseIds);

            ViewBag.Message = result == true ? "Updated Successfully" : "Update Failed";

            return View();
        }
        #endregion

        #region Qc Incentive

        public ActionResult AllIncentiveList(string projectType)
        {
            var vmAllIncentive = new VmAllIncentiveList();

            var selectListItemsForType = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Select Project Type" } };
            List<ProjectTypeModel> list = _repository.GetProjectType() ??
                                                     new List<ProjectTypeModel>();
            selectListItemsForType.AddRange(list.Select(p => new SelectListItem { Value = p.TypeName, Text = p.TypeName }));
            ViewBag.ddlProjectType = selectListItemsForType;

            if (projectType != null)
            {
                vmAllIncentive.SwQcAllIncentiveListModels = _repository.GetAllIncentiveList(projectType);

            }

            return View(vmAllIncentive);
        }
        [HttpPost]
        public ActionResult AllIncentiveList(List<SwQcAllIncentiveListModel> swQcAllIncentiveListModels, string projectType)
        {
            if (swQcAllIncentiveListModels.Count != 0)
            {
                _repository.UpdateAllIncentiveList(swQcAllIncentiveListModels);

            }

            return RedirectToAction("AllIncentiveList", new { projectType = projectType });

        }

        public ActionResult All_QcMembersMonthlyIncentive(string months, string years, string roles, string persons)
        {
            var vmQcIncentive = new VmAllIncentiveList();
            //!= "DEPUTY" || roles != "QCHEAD"


            vmQcIncentive.CmnUserModels = _repository.GetQcUserList();

            var selectListItemsForRef = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT PERSON" } };
            vmQcIncentive.CmnUserModels = _repository.GetQcUserList() ??
                                                   new List<CmnUserModel>();
            selectListItemsForRef.AddRange(vmQcIncentive.CmnUserModels.Select(p => new SelectListItem { Value = p.EmployeeCode, Text = p.UserFullName + ' ' + '(' + p.EmployeeCode + ')' }));
            ViewBag.ddlUsers = selectListItemsForRef;

            //
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
            //
            List<SelectListItem> selectListItemsRole = new List<SelectListItem>();
            selectListItemsRole.Add(new SelectListItem() { Text = "SELECT ROLE", Value = "0" });
            selectListItemsRole.Add(new SelectListItem() { Text = "QC", Value = "QC" });
            selectListItemsRole.Add(new SelectListItem() { Text = "BRAND", Value = "BRAND" });
            selectListItemsRole.Add(new SelectListItem() { Text = "DEPUTY", Value = "DEPUTY" });
            selectListItemsRole.Add(new SelectListItem() { Text = "QCHEAD", Value = "QCHEAD" });
            ViewBag.ddlRoles = selectListItemsRole;

            vmQcIncentive.ProjectMasterModels = _repository.GetAllProjectName();
            List<SelectListItem> items = vmQcIncentive.ProjectMasterModels.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectName.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.Projects = items;

            //Penalties Projects
            List<SelectListItem> items2 = vmQcIncentive.ProjectMasterModels.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectName.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.ProjectsPenalties = items2;

            vmQcIncentive.EmployeeCode = persons;
            vmQcIncentive.Month = months;
            vmQcIncentive.Year = years;
            vmQcIncentive.RoleName = roles;
            if ((roles != null && !roles.Contains("QCHEAD")) && (roles.Contains("QC") || roles.Contains("BRAND")))
            {
                //--------------All Issue Load---------------//

                if (months != null && years != null && roles != null && persons != null)
                {
                    vmQcIncentive.SwQcIssueDetailModels1 = _repository.GetIssueDetailsForIncentive(months, years, roles, persons);
                    vmQcIncentive.SwQcNewInnovationModels = _repository.GetNewInnovationForIncentive(months, years, roles, persons);
                    vmQcIncentive.SwQcPersonalUseFindingsIssueDetailModels = _repository.GetPersonalUseIncentive(months, years, roles, persons);
                    vmQcIncentive.SwQcIssueDetailModels2 = _repository.GetCtsDataForIncentive(months, years, roles, persons);
                    vmQcIncentive.SwQcIssueDetailModels3 = _repository.GetFieldAssignByHeadDataForIncentive(months, years, roles, persons);

                    //penalties
                  //  vmQcIncentive.SwQcIssueDetailModelsPenalties = _repository.GetPenaltiesDataForIncentive(months, years, roles, persons);
                    vmQcIncentive.SwIncentive_PenaltiesForIssuesModels = _repository.GetPenaltiesForIssues(months, years, roles, persons);
                    //others
                    vmQcIncentive.SwQcIssueDetailModelsOthers = _repository.GetOthersDataForIncentive(months, years, roles, persons);

                    //new reward and penalties
                    if (roles.Trim() == "QC")
                    {
                        ViewBag.GetRewardAndPenaltiesQc = _repository.GetRewardAndPenaltiesQc(months, years);
                    }
                    else if (roles.Trim() == "BRAND")
                    {
                        ViewBag.GetRewardAndPenaltiesQc = _repository.GetRewardAndPenaltiesQc(months, years);
                    }

                }

            }

            return View(vmQcIncentive);
        }


        [HttpPost]
        public ActionResult AddProjectsForOthers(string incentiveType, string addedAmount, string personEmpCode, string projectNameForOthers,
         string monName, int monId, string yearName,
          long yearId, string othersAddedRemarks, string othersDeduct, string others_D_Remarks, string finalAmount)
        {

            var model = new Custom_Sw_IncentiveModel();


            model.IncentiveTypes = incentiveType;
            model.EmployeeCode = personEmpCode;
            model.MonNum = monId;
            model.Month = monName;
            model.Year = yearId.ToString();
            model.ProjectName = projectNameForOthers;

            // model.AddedAmount = Convert.ToDecimal(addedAmount);
            model.AddedAmount = addedAmount != "" ? Convert.ToDecimal(addedAmount) : 0;
            model.AddAmountRemarks = othersAddedRemarks;
            model.DeductionRemarks = others_D_Remarks;
            model.Deduction = othersDeduct != "" ? Convert.ToDecimal(othersDeduct) : 0;
            model.FinalAmount = Convert.ToDecimal(finalAmount);


            return PartialView("~/Views/Software/Partial/_SwQcOthersIncentive.cshtml", model);


        }
        [HttpPost]
        public ActionResult AddProjectsForOthersHead(string incentiveType, string addedAmount, string personEmpCode, string projectNameForOthers,
         string monName, int monId, string yearName,
          string yearId, string othersAddedRemarks, string othersDeduct, string others_D_Remarks, string finalAmount)
        {

            var model = new Custom_Sw_IncentiveModel();


            model.IncentiveTypes = incentiveType;
            model.EmployeeCode = personEmpCode;
            model.MonNum = monId;
            model.Month = monName;
            model.Year = yearId;
            model.ProjectName = projectNameForOthers;

            // model.AddedAmount = Convert.ToDecimal(addedAmount);
            model.AddedAmount = addedAmount != "" ? Convert.ToDecimal(addedAmount) : 0;
            model.AddAmountRemarks = othersAddedRemarks;
            model.DeductionRemarks = others_D_Remarks;
            model.Deduction = othersDeduct != "" ? Convert.ToDecimal(othersDeduct) : 0;
            model.FinalAmount = Convert.ToDecimal(finalAmount);


            return PartialView("~/Views/Software/Partial/_SwQcOthersIncentiveHead.cshtml", model);


        }
        [HttpPost]
        public ActionResult AddProjectsForPenalties(string penaltiesReason, string penaltiesTotal, string personEmpCode, string projectNameForPenalties,
         string monName, int monId, string yearName,
          long yearId, string assignedPersons, string ParticularPersonsPenalties, string penaltiesRemarks, string finalAmount)
        {

            var model = new Custom_Sw_IncentiveModel();

            model = new Custom_Sw_IncentiveModel
            {
                PenaltiesReason = penaltiesReason,
                EmployeeCode = personEmpCode,
                MonNum = monId,
                Month = monName,
                Year = yearId.ToString(),
                ProjectName = projectNameForPenalties,
                TotalPenalties = Convert.ToDecimal(penaltiesTotal),
                AssignedPersons = Convert.ToInt32(assignedPersons),
                ParticularPersonsPenalties = Convert.ToDecimal(ParticularPersonsPenalties),
                PenaltiesRemarks = penaltiesRemarks,
                FinalAmount = Convert.ToDecimal(finalAmount)
            };

            return PartialView("~/Views/Software/Partial/_SwQcPenaltiesForIncentive.cshtml", model);

        }
        [HttpPost]
        public ActionResult AddProjectsForPenaltiesHead(string penaltiesReason, string penaltiesTotal, string personEmpCode, string projectNameForPenalties,
         string monName, int monId, string yearName,
          string yearId, string assignedPersons, string ParticularPersonsPenalties, string penaltiesRemarks, string finalAmount)
        {

            var model = new Custom_Sw_IncentiveModel();

            model = new Custom_Sw_IncentiveModel
            {
                PenaltiesReason = penaltiesReason,
                EmployeeCode = personEmpCode,
                MonNum = monId,
                Month = monName,
                Year = yearId,
                ProjectName = projectNameForPenalties,
                TotalPenalties = Convert.ToDecimal(penaltiesTotal),
                AssignedPersons = Convert.ToInt32(assignedPersons),
                ParticularPersonsPenalties = Convert.ToDecimal(ParticularPersonsPenalties),
                PenaltiesRemarks = penaltiesRemarks,
                FinalAmount = Convert.ToDecimal(finalAmount)
            };

            return PartialView("~/Views/Software/Partial/_SwQcPenaltiesForIncentive.cshtml", model);


        }
        [HttpPost]
        public ActionResult AddDetailsForBrandIssue(string totalAmount, string BrandIssueAmountPercentage, string finalAmount, string barndRemarks,
         string monName, int monId, string yearName, long yearId, string personEmpCode)
        {

            var model = new Custom_Sw_IncentiveModel();

            model = new Custom_Sw_IncentiveModel
            {
                TotalAmountForBrand = Convert.ToDecimal(totalAmount),
                BrandIssueAmountPercentage = Convert.ToInt32(BrandIssueAmountPercentage),
                BrandFinalAmount = Convert.ToDecimal(finalAmount),
                BrandRemarks = barndRemarks,
                EmployeeCode = personEmpCode,
                MonNum = monId,
                Month = monName,
                Year = yearId.ToString(),
            };

            return PartialView("~/Views/Software/Partial/_SwQcBrandIssueForIncentive.cshtml", model);
        }

        [HttpPost]
        public ActionResult AddDetailsForBrandCost(string BrandCost, string BrandCostPercentage, string BrandCostPerPersonIncentive, string BrandCostAddedAmount,
            string BrandCostAddedRemarks, string BrandCostDeduction, string BrandCostDeductionRemarks, string finalAmount,
         string monName, int monId, string yearName, long yearId, string personEmpCode)
        {
            var model = new Custom_Sw_IncentiveModel();
            model = new Custom_Sw_IncentiveModel
            {
                BrandCost = Convert.ToDecimal(BrandCost),
                BrandCostPercentage = Convert.ToInt32(BrandCostPercentage),
                BrandCostPerPersonIncentive = Convert.ToDecimal(BrandCostPerPersonIncentive),
                BrandCostAddedAmount = Convert.ToDecimal(BrandCostAddedAmount),
                BrandCostAddedRemarks = BrandCostAddedRemarks,
                BrandCostDeduction = Convert.ToDecimal(BrandCostDeduction),
                BrandCostDeductionRemarks = BrandCostDeductionRemarks,
                BrandCostFinalAmount = Convert.ToDecimal(finalAmount),
                EmployeeCode = personEmpCode,
                MonNum = monId,
                Month = monName,
                Year = yearId.ToString(),
            };
            return PartialView("~/Views/Software/Partial/_SwQcBrandCostForIncentive.cshtml", model);
        }
        #endregion

        #region Issue Summary

        public ActionResult IssueSummary(string projectId, string waltonQcStatus)
        {
            var vmSwQcSpecification = new VmSwQcSpecificationModified();
            //long userId = Convert.ToInt64(User.Identity.Name);


            vmSwQcSpecification.ProjectMasterModelsList = _repository.GetProjectListForSwQcHead();

            var selectListItems = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Select Project" } };

            var query = (from master in vmSwQcSpecification.ProjectMasterModelsList
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
            ViewBag.CombinedIds = selectListItems;
            vmSwQcSpecification.CombinedProjectId = projectId;

            ////Walton Qc Status///
            vmSwQcSpecification.SwQcIssueStauses = _repository.GetIssueStatus();

            var selectListItemsForRef = new List<SelectListItem> { new SelectListItem { Value = "ALL", Text = "ALL" } };
            List<SwQcIssueDetailModel> list = _repository.GetIssueStatus() ??
                                                     new List<SwQcIssueDetailModel>();
            selectListItemsForRef.AddRange(list.Select(p => new SelectListItem { Value = p.WaltonQcStatus, Text = p.WaltonQcStatus }));
            ViewBag.QcAllStatus = selectListItemsForRef;
            //Test phase//
            var selectListItemsTestPhase = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "Select Test Phase" } };
            List<SwQcTestPhaseModel> listTestPhase = _repository.GetSwQcTestPhaseForSupp() ??
                                                     new List<SwQcTestPhaseModel>();
            selectListItemsTestPhase.AddRange(listTestPhase.Select(p => new SelectListItem { Value = p.TestPhaseID.ToString(), Text = p.TestPhaseName }));
            ViewBag.CombinedIdsForTestPhase = selectListItemsTestPhase;

            if (projectId != "0" && waltonQcStatus != null)
            {
                vmSwQcSpecification.SwQcIssueDetailModels = _repository.GetTotalProjectsIssue(projectId.Trim(), waltonQcStatus.Trim());
            }

            return View(vmSwQcSpecification);
        }

        #endregion

        #region Save Incentive
        [HttpPost]
        public JsonResult All_QcMembersMonthlyIncentive(string objIssueArr)
        {

            List<Custom_Sw_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Sw_IncentiveModel>>(objIssueArr);
            Console.Write("result :" + results);

            bool isExist = false;
            if (results.Count != 0)
            {
                isExist = _repository.GetAll_QcMembersMonthlyIncentiveData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }
            if (isExist)
            {
                TempData["Message2"] = "Generated";
                return Json("All_QcMembersMonthlyIncentive", "Software");
            }

            var SaveIncentive = "0";

            if (results.Count != 0)
            {
                SaveIncentive = _repository.SaveSwIncentive_Issue(results);
            }


            return Json(new { data = SaveIncentive }, JsonRequestBehavior.AllowGet);
        }
       
        [HttpPost]
        public JsonResult SaveSwIncentive_ExtraWork(string objExtraArr)
        {
            List<Custom_Sw_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Sw_IncentiveModel>>(objExtraArr);
            Console.Write("result :" + results);

            bool isExist = false;

            if (results.Count != 0)
            {
                isExist = _repository.GetExtraWorkData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message2"] = "Generated";
                return Json("All_QcMembersMonthlyIncentive", "Software");
            }
            var saveSwIncentive = "0";
            if (results.Count != 0)
            {
                saveSwIncentive = _repository.SaveSwIncentive_ExtraWork(results);
            }

            return Json(new { data = saveSwIncentive }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveSwIncentive_PersonalUse(string objPerUseArr)
        {
            List<Custom_Sw_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Sw_IncentiveModel>>(objPerUseArr);
            Console.Write("result :" + results);

            bool isExist = false;

            if (results.Count != 0)
            {
                isExist = _repository.GetPersonalUseData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message2"] = "Generated";
                return Json("All_QcMembersMonthlyIncentive", "Software");
            }
            var saveSwIncentive = "0";
            if (results.Count != 0)
            {
                saveSwIncentive = _repository.SaveSwIncentive_PersonalUse(results);
            }

            return Json(new { data = saveSwIncentive }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveSwIncentive_Cts(string objCtsArr)
        {
            List<Custom_Sw_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Sw_IncentiveModel>>(objCtsArr);
            Console.Write("result :" + results);

            bool isExist = false;

            if (results.Count != 0)
            {
                isExist = _repository.GetCtsIncentiveData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message2"] = "Generated";
                return Json("All_QcMembersMonthlyIncentive", "Software");
            }
            var saveSwIncentive = "0";
            if (results.Count != 0)
            {
                saveSwIncentive = _repository.SaveSwIncentive_Cts(results);
            }

            return Json(new { data = saveSwIncentive }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveSwIncentive_FieldByHead(string objFieldArr)
        {
            List<Custom_Sw_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Sw_IncentiveModel>>(objFieldArr);
            Console.Write("result :" + results);

            bool isExist = false;

            if (results.Count != 0)
            {
                isExist = _repository.GetFieldByHeadData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message2"] = "Generated";
                return Json("All_QcMembersMonthlyIncentive", "Software");
            }
            var saveSwIncentive = "0";
            if (results.Count != 0)
            {
                saveSwIncentive = _repository.SaveSwIncentive_FieldByHead(results);
            }

            return Json(new { data = saveSwIncentive }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveSwIncentive_OthersAuto(string objOthersArr)
        {
            List<Custom_Sw_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Sw_IncentiveModel>>(objOthersArr);
            Console.Write("result :" + results);

            //bool isExist = false;

            //if (results.Count != 0)
            //{
            //    isExist = _repository.GetOthersData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            //}

            //if (isExist)
            //{
            //    TempData["Message2"] = "Generated";
            //    return Json("All_QcMembersMonthlyIncentive", "Software");
            //}
            var saveSwIncentive = "0";
            if (results.Count != 0)
            {
                saveSwIncentive = _repository.SaveSwIncentive_OthersAuto(results);
            }

            return Json(new { data = saveSwIncentive }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveSwIncentive_Others(string objOthersArr)
        {
            List<Custom_Sw_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Sw_IncentiveModel>>(objOthersArr);
            Console.Write("result :" + results);

            //bool isExist = false;

            //if (results.Count != 0)
            //{
            //    isExist = _repository.GetOthersData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            //}

            //if (isExist)
            //{
            //    TempData["Message2"] = "Generated";
            //    return Json("All_QcMembersMonthlyIncentive", "Software");
            //}
            var saveSwIncentive = "0";
            if (results.Count != 0)
            {
                saveSwIncentive = _repository.SaveSwIncentive_Others(results);
            }

            return Json(new { data = saveSwIncentive }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveSwIncentive_OthersHead(string objOthersArr)
        {
            List<Custom_Sw_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Sw_IncentiveModel>>(objOthersArr);
            Console.Write("result :" + results);

            bool isExist = false;

            if (results.Count != 0)
            {
                isExist = _repository.GetOthersData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message2"] = "Generated";
                return Json("HeadDeputyMonthlyIncentive", "Software");
            }
            var saveSwIncentive = "0";
            if (results.Count != 0)
            {
                saveSwIncentive = _repository.SaveSwIncentive_Others(results);
            }

            return Json(new { data = saveSwIncentive }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveSwIncentive_Penalties(string objPenaltiesArr)
        {
            List<Custom_Sw_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Sw_IncentiveModel>>(objPenaltiesArr);
            Console.Write("result :" + results);

            //bool isExist = false;

            //if (results.Count != 0)
            //{
            //    isExist = _repository.GetPenaltiesData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            //}

            //if (isExist)
            //{
            //    TempData["Message2"] = "Generated";
            //    return Json("All_QcMembersMonthlyIncentive", "Software");
            //}
            var saveSwIncentive = "0";
            if (results.Count != 0)
            {
                saveSwIncentive = _repository.SaveSwIncentive_Penalties(results);
            }

            return Json(new { data = saveSwIncentive }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveSwIncentive_PenaltiesHead(string objPenaltiesArr)
        {
            List<Custom_Sw_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Sw_IncentiveModel>>(objPenaltiesArr);
            Console.Write("result :" + results);

            bool isExist = false;

            if (results.Count != 0)
            {
                isExist = _repository.GetPenaltiesData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message2"] = "Generated";
                return Json("HeadDeputyMonthlyIncentive", "Software");
            }
            var saveSwIncentive = "0";
            if (results.Count != 0)
            {
                saveSwIncentive = _repository.SaveSwIncentive_Penalties(results);
            }

            return Json(new { data = saveSwIncentive }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveSwIncentive_Incentive(string objFinalArr)
        {
            List<Custom_Sw_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Sw_IncentiveModel>>(objFinalArr);
            Console.Write("result :" + results);

            bool isExist = false;

            if (results.Count != 0)
            {
                isExist = _repository.GetIncentiveData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message2"] = "Generated";
                return Json("All_QcMembersMonthlyIncentive", "Software");
            }
            var saveSwIncentive = "0";
            if (results.Count != 0)
            {
                saveSwIncentive = _repository.SaveSwIncentive_Incentive(results);
            }

            return Json(new { data = saveSwIncentive }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveSwIncentive_AutoPenalties(string objAutoPenalArr)
        {
            List<Custom_Sw_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Sw_IncentiveModel>>(objAutoPenalArr);
            Console.Write("result :" + results);

            bool isExist = false;

            if (results.Count != 0)
            {
                isExist = _repository.GetPenaltiesData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);
            }

            if (isExist)
            {
                TempData["Message2"] = "Generated";
                return Json("All_QcMembersMonthlyIncentive", "Software");
            }
            var saveSwIncentive = "0";
            if (results.Count != 0)
            {
                saveSwIncentive = _repository.SaveSwIncentive_AutoPenalties(results);
            }

            return Json(new { data = saveSwIncentive }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveSwIncentive_AutoPenaltiesForIssues(string objAutoPenalArr)
        {
            List<SwIncentive_PenaltiesForIssuesModel> results = JsonConvert.DeserializeObject<List<SwIncentive_PenaltiesForIssuesModel>>(objAutoPenalArr);
            Console.Write("result :" + results);

            bool isExist = false;

            if (results.Count != 0)
            {
                isExist = _repository.GetPenaltiesDataForIssues(results[0].EmployeeCode, Convert.ToInt32(results[0].MonNum), Convert.ToInt32(results[0].Year));
            }
                
            if (isExist)
            {
                TempData["Message2"] = "Generated";
                return Json("All_QcMembersMonthlyIncentive", "Software");
            }
            var saveSwIncentive = "0";
            if (results.Count != 0)
            {
                saveSwIncentive = _repository.SaveSwIncentive_AutoPenaltiesForIssues(results);
            }

            return Json(new { data = saveSwIncentive }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveSwIncentive_IncentiveHead(string objFinalArr)
        {
            List<Custom_Sw_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Sw_IncentiveModel>>(objFinalArr);
            Console.Write("result :" + results);

            bool isExist = false;

            if (results.Count != 0)
            {
                isExist = _repository.GetIncentiveData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message2"] = "Generated";
                return Json("HeadDeputyMonthlyIncentive", "Software");
            }
            var saveSwIncentive = "0";

            if (results.Count != 0)
            {
                saveSwIncentive = _repository.SaveSwIncentive_Incentive(results);
            }

            var saveSwRewardOrPenaltiesIncentive = "0";
            foreach (var insResult in results)
            {
                if (insResult.RoleName == "DEPUTY")
                {
                    saveSwRewardOrPenaltiesIncentive = _repository.SaveQcAllMemberRewardsAndPenalties(insResult.MonNum,insResult.Year);
                }
            }
            return Json(new { data = saveSwIncentive }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveSwIncentive_BrandIssues(string objBrandIssueArr)
        {
            List<Custom_Sw_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Sw_IncentiveModel>>(objBrandIssueArr);
            Console.Write("result :" + results);

            bool isExist = false;

            if (results.Count != 0)
            {
                isExist = _repository.GetBrandIssuesData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message2"] = "Generated";
                return Json("All_QcMembersMonthlyIncentive", "Software");
            }
            var saveSwIncentive = "0";
            if (results.Count != 0)
            {
                saveSwIncentive = _repository.SaveSwIncentive_BrandIssue(results);
            }
          
            return Json(new { data = saveSwIncentive }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveSwIncentive_BrandCost(string objBrandCostArr)
        {
            List<Custom_Sw_IncentiveModel> results = JsonConvert.DeserializeObject<List<Custom_Sw_IncentiveModel>>(objBrandCostArr);
            Console.Write("result :" + results);

            bool isExist = false;

            if (results.Count != 0)
            {
                isExist = _repository.GetBrandCostData(results[0].EmployeeCode, results[0].MonNum, results[0].Year);

            }

            if (isExist)
            {
                TempData["Message2"] = "Generated";
                return Json("All_QcMembersMonthlyIncentive", "Software");
            }
            var saveSwIncentive = "0";
            if (results.Count != 0)
            {
                saveSwIncentive = _repository.SaveSwIncentive_BrandCost(results);
            }

            return Json(new { data = saveSwIncentive }, JsonRequestBehavior.AllowGet);
        }
        //HeadDeputyMonthlyIncentive

        public ActionResult HeadDeputyMonthlyIncentive(string EmpCode, string Month, string MonNum, string Year, string roles)
        {
            var vmQcIncentive = new VmAllIncentiveList();
            //!= "DEPUTY" || roles != "QCHEAD"
            int mons;
            int.TryParse(MonNum, out mons);

            //
            vmQcIncentive.ProjectMasterModels = _repository.GetAllProjectName();
            List<SelectListItem> items = vmQcIncentive.ProjectMasterModels.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectName.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.Projects = items;

            //Penalties Projects
            List<SelectListItem> items2 = vmQcIncentive.ProjectMasterModels.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectName.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.ProjectsPenalties = items2;
            //
            vmQcIncentive.EmployeeCode = EmpCode;
            vmQcIncentive.Month = Month;
            vmQcIncentive.MonNum = mons;
            vmQcIncentive.Year = Year;
            vmQcIncentive.RoleName = roles;

            if (EmpCode != null && mons != 0 && Year != null)
            {
                vmQcIncentive.QcHeadIncentiveList = _repository.GetHeadDeputyIncentiveList(EmpCode, mons, Year, roles);
                //penalties
                vmQcIncentive.SwQcIssueDetailModelsPenalties = _repository.GetPenaltiesDataForHeadIncentive(MonNum, Year, roles, EmpCode);

                //new reward and penalties

                if (roles == "DEPUTY" || roles == "QCHEAD")
                {
                    ViewBag.GetRewardAndPenaltiesDeputyAndHead = _repository.GetRewardAndPenaltiesDeputyAndHead(MonNum, Year, roles);
                }


            }

            return View(vmQcIncentive);
        }

        #endregion

        #region Incentive Sheet

        [HttpGet]
        public ActionResult All_QcMembersIncentiveSheet()
        {
            var customInc = new Custom_Sw_IncentiveModel();
            customInc.CmnUserModelsList = _repository.GetSwUserList();
            return View(customInc);
        }

        [HttpGet]
        [Authorize(Roles = "QCHEAD,SA,ACCNT")]
        public ActionResult All_QcMembersIncentiveReportTopSheet(string Month, string MonNum, string Year)
        {
            List<Custom_Sw_IncentiveModel> customInc = new List<Custom_Sw_IncentiveModel>();

            customInc = _repository.All_QcMembersIncentiveReportTopSheet(Month, MonNum, Year);

            ViewBag.GetPreparedUser = _repository.GetPreparedUserName();
            return View(customInc);
        }
        public ActionResult All_QcMembersIncentiveReportPerPerson(string EmpCode, string Month, string MonNum, string Year)
        {
            List<Custom_Sw_IncentiveModel> cmList = new List<Custom_Sw_IncentiveModel>();
            var pmIncentiveModels = _repository.GetSwIncentive_IssueForPrint(EmpCode, MonNum, Year);

            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Sw_IncentiveModel items = new Custom_Sw_IncentiveModel();
                items.ProjectName = customPmIncentiveModel.ProjectName;
                items.SoftwareVersionName = customPmIncentiveModel.SoftwareVersionName;
                items.SoftwareVersionNumber = customPmIncentiveModel.SoftwareVersionNumber;
                items.IncentiveClaimArea = customPmIncentiveModel.IncentiveClaimArea;
                items.AssignedPersons = customPmIncentiveModel.AssignedPersons;
                items.Critical = customPmIncentiveModel.Critical;
                items.Major = customPmIncentiveModel.Major;
                items.Minor = customPmIncentiveModel.Minor;
                items.BaseAmount = customPmIncentiveModel.BaseAmount;
                items.StartDate = customPmIncentiveModel.StartDate;
                items.EndDate = customPmIncentiveModel.EndDate;
                items.DateDiffWithHoliday = customPmIncentiveModel.DateDiffWithHoliday;
                items.Timeline = customPmIncentiveModel.Timeline;
                items.IssueAmount = customPmIncentiveModel.IssueAmount;
                items.AddedAmount = customPmIncentiveModel.AddedAmount;
                items.AddAmountRemarks = customPmIncentiveModel.AddAmountRemarks;
                items.Deduction = customPmIncentiveModel.Deduction;
                items.DeductionRemarks = customPmIncentiveModel.DeductionRemarks;
                items.TotalAmount = customPmIncentiveModel.TotalAmount;
                items.Percentage = customPmIncentiveModel.Percentage;
                items.ParticularPersonIncentive = customPmIncentiveModel.ParticularPersonIncentive;
                items.Types = customPmIncentiveModel.Types;
                cmList.Add(items);
            }


            pmIncentiveModels = _repository.GetSwIncentive_ExtraWorkForPrint(EmpCode, MonNum, Year);
            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Sw_IncentiveModel items = new Custom_Sw_IncentiveModel();
                items.ProjectName = customPmIncentiveModel.ProjectName;
                items.SoftwareVersionName = customPmIncentiveModel.SoftwareVersionName;
                items.SoftwareVersionNumber = customPmIncentiveModel.SoftwareVersionNumber;
                items.IncentiveClaimArea = customPmIncentiveModel.IncentiveClaimArea;
                items.AssignedPersons = customPmIncentiveModel.AssignedPersons;
                items.Critical = customPmIncentiveModel.Critical;
                items.Major = customPmIncentiveModel.Major;
                items.Minor = customPmIncentiveModel.Minor;
                items.BaseAmount = customPmIncentiveModel.BaseAmount;
                items.StartDate = customPmIncentiveModel.StartDate;
                items.EndDate = customPmIncentiveModel.EndDate;
                items.DateDiffWithHoliday = customPmIncentiveModel.DateDiffWithHoliday;
                items.Timeline = customPmIncentiveModel.Timeline;
                items.IssueAmount = customPmIncentiveModel.IssueAmount;
                items.AddedAmount = customPmIncentiveModel.AddedAmount;
                items.AddAmountRemarks = customPmIncentiveModel.AddAmountRemarks;
                items.Deduction = customPmIncentiveModel.Deduction;
                items.DeductionRemarks = customPmIncentiveModel.DeductionRemarks;
                items.TotalAmount = customPmIncentiveModel.TotalAmount;
                items.Percentage = customPmIncentiveModel.Percentage;
                items.ParticularPersonIncentive = customPmIncentiveModel.ParticularPersonIncentive;
                items.Types = customPmIncentiveModel.Types;
                cmList.Add(items);
            }
            pmIncentiveModels = _repository.GetSwIncentive_PersonalUseForPrint(EmpCode, MonNum, Year);
            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Sw_IncentiveModel items = new Custom_Sw_IncentiveModel();
                items.ProjectName = customPmIncentiveModel.ProjectName;
                items.SoftwareVersionName = customPmIncentiveModel.SoftwareVersionName;
                items.SoftwareVersionNumber = customPmIncentiveModel.SoftwareVersionNumber;
                items.IncentiveClaimArea = customPmIncentiveModel.IncentiveClaimArea;
                items.AssignedPersons = customPmIncentiveModel.AssignedPersons;
                items.Critical = customPmIncentiveModel.Critical;
                items.Major = customPmIncentiveModel.Major;
                items.Minor = customPmIncentiveModel.Minor;
                items.BaseAmount = customPmIncentiveModel.BaseAmount;
                items.StartDate = customPmIncentiveModel.StartDate;
                items.EndDate = customPmIncentiveModel.EndDate;
                items.DateDiffWithHoliday = customPmIncentiveModel.DateDiffWithHoliday;
                items.Timeline = customPmIncentiveModel.Timeline;
                items.IssueAmount = customPmIncentiveModel.IssueAmount;
                items.AddedAmount = customPmIncentiveModel.AddedAmount;
                items.AddAmountRemarks = customPmIncentiveModel.AddAmountRemarks;
                items.Deduction = customPmIncentiveModel.Deduction;
                items.DeductionRemarks = customPmIncentiveModel.DeductionRemarks;
                items.TotalAmount = customPmIncentiveModel.TotalAmount;
                items.ParticularPersonIncentive = customPmIncentiveModel.ParticularPersonIncentive;
                items.Types = customPmIncentiveModel.Types;
                cmList.Add(items);
            }
            pmIncentiveModels = _repository.GetSwIncentive_CtsForPrint(EmpCode, MonNum, Year);
            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Sw_IncentiveModel items = new Custom_Sw_IncentiveModel();
                items.ProjectName = customPmIncentiveModel.ProjectName;
                items.SoftwareVersionName = customPmIncentiveModel.SoftwareVersionName;
                items.SoftwareVersionNumber = customPmIncentiveModel.SoftwareVersionNumber;
                items.IncentiveClaimArea = customPmIncentiveModel.IncentiveClaimArea;
                items.AssignedPersons = customPmIncentiveModel.AssignedPersons;
                items.Critical = customPmIncentiveModel.Critical;
                items.Major = customPmIncentiveModel.Major;
                items.Minor = customPmIncentiveModel.Minor;
                items.BaseAmount = customPmIncentiveModel.BaseAmount;
                items.StartDate = customPmIncentiveModel.StartDate;
                items.EndDate = customPmIncentiveModel.EndDate;
                items.DateDiffWithHoliday = customPmIncentiveModel.DateDiffWithHoliday;
                items.Timeline = customPmIncentiveModel.Timeline;
                items.IssueAmount = customPmIncentiveModel.IssueAmount;
                items.AddedAmount = customPmIncentiveModel.AddedAmount;
                items.AddAmountRemarks = customPmIncentiveModel.AddAmountRemarks;
                items.Deduction = customPmIncentiveModel.Deduction;
                items.DeductionRemarks = customPmIncentiveModel.DeductionRemarks;
                items.TotalAmount = customPmIncentiveModel.TotalAmount;
                items.Percentage = customPmIncentiveModel.Percentage;
                items.ParticularPersonIncentive = customPmIncentiveModel.ParticularPersonIncentive;
                items.Types = customPmIncentiveModel.Types;
                cmList.Add(items);
            }
            pmIncentiveModels = _repository.GetSwIncentive_FieldByHeadForPrint(EmpCode, MonNum, Year);
            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Sw_IncentiveModel items = new Custom_Sw_IncentiveModel();
                items.ProjectName = customPmIncentiveModel.ProjectName;
                items.SoftwareVersionName = customPmIncentiveModel.SoftwareVersionName;
                items.SoftwareVersionNumber = customPmIncentiveModel.SoftwareVersionNumber;
                items.IncentiveClaimArea = customPmIncentiveModel.IncentiveClaimArea;
                items.AssignedPersons = customPmIncentiveModel.AssignedPersons;
                items.Critical = customPmIncentiveModel.Critical;
                items.Major = customPmIncentiveModel.Major;
                items.Minor = customPmIncentiveModel.Minor;
                items.BaseAmount = customPmIncentiveModel.BaseAmount;
                items.StartDate = customPmIncentiveModel.StartDate;
                items.EndDate = customPmIncentiveModel.EndDate;
                items.DateDiffWithHoliday = customPmIncentiveModel.DateDiffWithHoliday;
                items.Timeline = customPmIncentiveModel.Timeline;
                items.IssueAmount = customPmIncentiveModel.IssueAmount;
                items.AddedAmount = customPmIncentiveModel.AddedAmount;
                items.AddAmountRemarks = customPmIncentiveModel.AddAmountRemarks;
                items.Deduction = customPmIncentiveModel.Deduction;
                items.DeductionRemarks = customPmIncentiveModel.DeductionRemarks;
                items.TotalAmount = customPmIncentiveModel.TotalAmount;
                items.Percentage = customPmIncentiveModel.Percentage;
                items.ParticularPersonIncentive = customPmIncentiveModel.ParticularPersonIncentive;
                items.Types = customPmIncentiveModel.Types;
                cmList.Add(items);
            }
            pmIncentiveModels = _repository.GetSwIncentive_OthersForPrint(EmpCode, MonNum, Year);
            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Sw_IncentiveModel items = new Custom_Sw_IncentiveModel();
                items.ProjectName = customPmIncentiveModel.ProjectName;
                items.SoftwareVersionName = customPmIncentiveModel.SoftwareVersionName;
                items.SoftwareVersionNumber = customPmIncentiveModel.SoftwareVersionNumber;
                items.IncentiveClaimArea = customPmIncentiveModel.IncentiveClaimArea;
                items.AssignedPersons = customPmIncentiveModel.AssignedPersons;
                items.Critical = customPmIncentiveModel.Critical;
                items.Major = customPmIncentiveModel.Major;
                items.Minor = customPmIncentiveModel.Minor;
                items.BaseAmount = customPmIncentiveModel.BaseAmount;
                items.StartDate = customPmIncentiveModel.StartDate;
                items.EndDate = customPmIncentiveModel.EndDate;
                items.DateDiffWithHoliday = customPmIncentiveModel.DateDiffWithHoliday;
                items.Timeline = customPmIncentiveModel.Timeline;
                items.IssueAmount = customPmIncentiveModel.IssueAmount;
                items.AddedAmount = customPmIncentiveModel.AddedAmount;
                items.AddAmountRemarks = customPmIncentiveModel.AddAmountRemarks;
                items.Deduction = customPmIncentiveModel.Deduction;
                items.DeductionRemarks = customPmIncentiveModel.DeductionRemarks;
                items.TotalAmount = customPmIncentiveModel.TotalAmount;
                items.Percentage = customPmIncentiveModel.Percentage;
                items.ParticularPersonIncentive = customPmIncentiveModel.ParticularPersonIncentive;
                items.Types = customPmIncentiveModel.Types;
                cmList.Add(items);
            }
            pmIncentiveModels = _repository.GetSwIncentive_PenaltiesForIssuesForPrint(EmpCode, MonNum, Year);
            foreach (var customPmIncentiveModel in pmIncentiveModels)
            {
                Custom_Sw_IncentiveModel items = new Custom_Sw_IncentiveModel();
                items.ProjectName = customPmIncentiveModel.ProjectName;
                items.SoftwareVersionName = customPmIncentiveModel.SoftwareVersionName;
                items.SoftwareVersionNumber = customPmIncentiveModel.SoftwareVersionNumber;
                items.IncentiveClaimArea = customPmIncentiveModel.IncentiveClaimArea;
                items.AssignedPersons = customPmIncentiveModel.AssignedPersons;
                items.Critical = customPmIncentiveModel.Critical;
                items.Major = customPmIncentiveModel.Major;
                items.Minor = customPmIncentiveModel.Minor;
                items.BaseAmount = customPmIncentiveModel.TotalAmount;
                items.StartDate = customPmIncentiveModel.StartDate;
                items.EndDate = customPmIncentiveModel.EndDate;
                items.DateDiffWithHoliday = customPmIncentiveModel.DateDiffWithHoliday;
                items.Timeline = customPmIncentiveModel.Timeline;
                items.IssueAmount = customPmIncentiveModel.IssueAmount;
                items.AddedAmount = customPmIncentiveModel.AddedAmount;
                items.AddAmountRemarks = customPmIncentiveModel.AddAmountRemarks;
                items.Deduction = customPmIncentiveModel.Deduction;
                items.DeductionRemarks = customPmIncentiveModel.DeductionRemarks;
                items.TotalAmount = customPmIncentiveModel.TotalPenalties;
                items.Percentage = customPmIncentiveModel.Percentage;
                items.PenaltiesPercentage = Convert.ToString(customPmIncentiveModel.PenaltiesPercentage);
                items.ParticularPersonIncentive = customPmIncentiveModel.FinalAmount;
                items.Types = customPmIncentiveModel.Types;
                cmList.Add(items);
            }
            //pmIncentiveModels = _repository.GetSwIncentive_PenaltiesForPrint(EmpCode, MonNum, Year);
            //foreach (var customPmIncentiveModel in pmIncentiveModels)
            //{
            //    Custom_Sw_IncentiveModel items = new Custom_Sw_IncentiveModel();
            //    items.ProjectName = customPmIncentiveModel.ProjectName;
            //    items.SoftwareVersionName = customPmIncentiveModel.SoftwareVersionName;
            //    items.SoftwareVersionNumber = customPmIncentiveModel.SoftwareVersionNumber;
            //    items.IncentiveClaimArea = customPmIncentiveModel.IncentiveClaimArea;
            //    items.AssignedPersons = customPmIncentiveModel.AssignedPersons;
            //    items.Critical = customPmIncentiveModel.Critical;
            //    items.Major = customPmIncentiveModel.Major;
            //    items.Minor = customPmIncentiveModel.Minor;
            //    items.BaseAmount = customPmIncentiveModel.TotalAmount;
            //    items.StartDate = customPmIncentiveModel.StartDate;
            //    items.EndDate = customPmIncentiveModel.EndDate;
            //    items.DateDiffWithHoliday = customPmIncentiveModel.DateDiffWithHoliday;
            //    items.Timeline = customPmIncentiveModel.Timeline;
            //    items.IssueAmount = customPmIncentiveModel.IssueAmount;
            //    items.AddedAmount = customPmIncentiveModel.AddedAmount;
            //    items.AddAmountRemarks = customPmIncentiveModel.AddAmountRemarks;
            //    items.Deduction = customPmIncentiveModel.Deduction;
            //    items.DeductionRemarks = customPmIncentiveModel.DeductionRemarks;
            //    items.TotalAmount = customPmIncentiveModel.TotalPenalties;
            //    items.Percentage = customPmIncentiveModel.Percentage;
            //    items.PenaltiesPercentage = Convert.ToString(customPmIncentiveModel.PenaltiesPercentage);
            //    items.ParticularPersonIncentive = customPmIncentiveModel.FinalAmount;
            //    items.Types = customPmIncentiveModel.Types;
            //    cmList.Add(items);
            //}
            //pmIncentiveModels = _repository.GetSwIncentive_BrandIssuesForPrint(EmpCode, MonNum, Year);
            //foreach (var customPmIncentiveModel in pmIncentiveModels)
            //{
            //    Custom_Sw_IncentiveModel items = new Custom_Sw_IncentiveModel();
            //    items.ProjectName = customPmIncentiveModel.ProjectName;
            //    items.SoftwareVersionName = customPmIncentiveModel.SoftwareVersionName;
            //    items.SoftwareVersionNumber = customPmIncentiveModel.SoftwareVersionNumber;
            //    items.IncentiveClaimArea = customPmIncentiveModel.IncentiveClaimArea;
            //    items.AssignedPersons = customPmIncentiveModel.AssignedPersons;
            //    items.Critical = customPmIncentiveModel.Critical;
            //    items.Major = customPmIncentiveModel.Major;
            //    items.Minor = customPmIncentiveModel.Minor;
            //    items.BaseAmount = customPmIncentiveModel.BaseAmount;
            //    items.StartDate = customPmIncentiveModel.StartDate;
            //    items.EndDate = customPmIncentiveModel.EndDate;
            //    items.DateDiffWithHoliday = customPmIncentiveModel.DateDiffWithHoliday;
            //    items.Timeline = customPmIncentiveModel.Timeline;
            //    items.IssueAmount = customPmIncentiveModel.IssueAmount;
            //    items.AddedAmount = customPmIncentiveModel.AddedAmount;
            //    items.AddAmountRemarks = customPmIncentiveModel.AddAmountRemarks;
            //    items.Deduction = customPmIncentiveModel.Deduction;
            //    items.DeductionRemarks = customPmIncentiveModel.DeductionRemarks;
            //    items.TotalAmount = customPmIncentiveModel.TotalAmount;
            //    items.Percentage = customPmIncentiveModel.Percentage;
            //    items.ParticularPersonIncentive = customPmIncentiveModel.ParticularPersonIncentive;
            //    items.Types = customPmIncentiveModel.Types;
            //    cmList.Add(items);
            //}
            //pmIncentiveModels = _repository.GetSwIncentive_BrandCostForPrint(EmpCode, MonNum, Year);
            //foreach (var customPmIncentiveModel in pmIncentiveModels)
            //{
            //    Custom_Sw_IncentiveModel items = new Custom_Sw_IncentiveModel();
            //    items.ProjectName = customPmIncentiveModel.ProjectName;
            //    items.SoftwareVersionName = customPmIncentiveModel.SoftwareVersionName;
            //    items.SoftwareVersionNumber = customPmIncentiveModel.SoftwareVersionNumber;
            //    items.IncentiveClaimArea = customPmIncentiveModel.IncentiveClaimArea;
            //    items.AssignedPersons = customPmIncentiveModel.AssignedPersons;
            //    items.Critical = customPmIncentiveModel.Critical;
            //    items.Major = customPmIncentiveModel.Major;
            //    items.Minor = customPmIncentiveModel.Minor;
            //    items.BaseAmount = customPmIncentiveModel.BaseAmount;
            //    items.StartDate = customPmIncentiveModel.StartDate;
            //    items.EndDate = customPmIncentiveModel.EndDate;
            //    items.DateDiffWithHoliday = customPmIncentiveModel.DateDiffWithHoliday;
            //    items.Timeline = customPmIncentiveModel.Timeline;
            //    items.IssueAmount = customPmIncentiveModel.IssueAmount;
            //    items.AddedAmount = customPmIncentiveModel.AddedAmount;
            //    items.AddAmountRemarks = customPmIncentiveModel.AddAmountRemarks;
            //    items.Deduction = customPmIncentiveModel.Deduction;
            //    items.DeductionRemarks = customPmIncentiveModel.DeductionRemarks;
            //    items.TotalAmount = customPmIncentiveModel.TotalAmount;
            //    items.Percentage = customPmIncentiveModel.Percentage;
            //    items.ParticularPersonIncentive = customPmIncentiveModel.ParticularPersonIncentive;
            //    items.Types = customPmIncentiveModel.Types;
            //    cmList.Add(items);
            //}
            ////ViewBag.PmIncentiveForPrint = cmList;
            ViewBag.GetPreparedUser = _repository.GetPreparedUserName();
            ViewBag.GetTotalFinalIncentiveOfSw = _repository.GetTotalFinalIncentiveOfSw(EmpCode, MonNum, Year);

            return View(cmList);
        }
        public ActionResult NewRewardOrPenalties(string MonNum1, string Year)
        {
            var vmSwQc = new VmAllIncentiveList();

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

            if (MonNum1 != null && Year != null)
            {
                ViewBag.GetNinetyFiveProductionReward = _repository.GetNinetyFiveProductionReward(MonNum1, Year);
                ViewBag.GetPmClearanceVsLsdForReport = _repository.GetPmClearanceVsLsdForReport(MonNum1, Year);

               //  ViewBag.GetPoDateWisePenalties = _repository.GetPoDateWisePenalties(MonNum1, Year);
               // ViewBag.GetNinetyFiveSalesOutReward = _repository.GetNinetyFiveSalesOutReward(MonNum1, Year);
            }

            //ViewBag.PmIncentiveForPrint = cmList;
            //ViewBag.GetPreparedUser = _projectManagerRepository.GetPreparedUserName();
            //ViewBag.GetTotalFinalIncentiveOfPm = _projectManagerRepository.GetTotalFinalIncentiveOfPm(EmpCode, MonNum, Year);

            vmSwQc.Month = MonNum1;
            vmSwQc.Year = Year;
            return View(vmSwQc);
        }
        public ActionResult NewRewardOrPenaltiesAccountant(string MonNum1, string Year)
        {
            var vmSwQc = new VmAllIncentiveList();

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

            if (MonNum1 != null && Year != null)
            {
               // ViewBag.GetPoDateWisePenalties = _repository.GetPoDateWisePenaltiesAccountant(MonNum1, Year);
                ViewBag.GetNinetyFiveProductionReward = _repository.GetNinetyFiveProductionRewardAccountant(MonNum1, Year);
                ViewBag.GetPmClearanceVsLsdForReport = _repository.GetPmClearanceVsLsdForReport(MonNum1, Year);
               // ViewBag.GetNinetyFiveSalesOutReward = _repository.GetNinetyFiveSalesOutRewardAccountant(MonNum1, Year);
               // ViewBag.GetTotalRewardAndPenalties = _repository.GetTotalRewardAndPenalties(MonNum1, Year);
            }
            //ViewBag.PmIncentiveForPrint = cmList;
            //ViewBag.GetPreparedUser = _projectManagerRepository.GetPreparedUserName();
            //ViewBag.GetTotalFinalIncentiveOfPm = _projectManagerRepository.GetTotalFinalIncentiveOfPm(EmpCode, MonNum, Year);

            vmSwQc.Month = MonNum1;
            vmSwQc.Year = Year;
            return View(vmSwQc);
        }
        #endregion

        #region Penalties

        #region com
        //        public ActionResult All_QcMembersPenaltiesCalculation(string MonNum1, string Year, string projectName)
        //        {
        //            int mons;
        //            int.TryParse(MonNum1, out mons);

        //            var vmQcIncentive = new VmAllIncentiveList();
        //            vmQcIncentive.ProjectMasterModels = _repository.GetAllProjectNamesForPenalties();
        //            List<SelectListItem> items = new List<SelectListItem> { new SelectListItem { Value = "All", Text = "All" } };
        //            items.AddRange(vmQcIncentive.ProjectMasterModels.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectName.ToString(CultureInfo.InvariantCulture) }).ToList());
        //            ViewBag.ProjectsName = items;
        //            //
        //            List<SelectListItem> selectListItemsMonth = new List<SelectListItem>();
        //            selectListItemsMonth.Add(new SelectListItem() { Text = "SELECT MONTH", Value = "0" });
        //            selectListItemsMonth.Add(new SelectListItem() { Text = "January", Value = "1" });
        //            selectListItemsMonth.Add(new SelectListItem() { Text = "February", Value = "2" });
        //            selectListItemsMonth.Add(new SelectListItem() { Text = "March", Value = "3" });
        //            selectListItemsMonth.Add(new SelectListItem() { Text = "April", Value = "4" });
        //            selectListItemsMonth.Add(new SelectListItem() { Text = "May", Value = "5" });
        //            selectListItemsMonth.Add(new SelectListItem() { Text = "June", Value = "6" });
        //            selectListItemsMonth.Add(new SelectListItem() { Text = "July", Value = "7" });
        //            selectListItemsMonth.Add(new SelectListItem() { Text = "August", Value = "8" });
        //            selectListItemsMonth.Add(new SelectListItem() { Text = "September", Value = "9" });
        //            selectListItemsMonth.Add(new SelectListItem() { Text = "October", Value = "10" });
        //            selectListItemsMonth.Add(new SelectListItem() { Text = "November", Value = "11" });
        //            selectListItemsMonth.Add(new SelectListItem() { Text = "December", Value = "12" });

        //            ViewBag.ddlMonths = selectListItemsMonth;
        //            //
        //            List<SelectListItem> selectListItemsYear = new List<SelectListItem>();
        //            selectListItemsYear.Add(new SelectListItem() { Text = "SELECT YEAR", Value = "0" });
        //            selectListItemsYear.Add(new SelectListItem() { Text = "2019", Value = "2019" });
        //            selectListItemsYear.Add(new SelectListItem() { Text = "2020", Value = "2020" });
        //            selectListItemsYear.Add(new SelectListItem() { Text = "2021", Value = "2021" });
        //            selectListItemsYear.Add(new SelectListItem() { Text = "2022", Value = "2022" });
        //            selectListItemsYear.Add(new SelectListItem() { Text = "2023", Value = "2023" });
        //            selectListItemsYear.Add(new SelectListItem() { Text = "2024", Value = "2024" });
        //            selectListItemsYear.Add(new SelectListItem() { Text = "2025", Value = "2025" });
        //            selectListItemsYear.Add(new SelectListItem() { Text = "2026", Value = "2026" });
        //            selectListItemsYear.Add(new SelectListItem() { Text = "2027", Value = "2027" });
        //            selectListItemsYear.Add(new SelectListItem() { Text = "2028", Value = "2028" });
        //            selectListItemsYear.Add(new SelectListItem() { Text = "2029", Value = "2029" });
        //            selectListItemsYear.Add(new SelectListItem() { Text = "2030", Value = "2030" });

        //            ViewBag.ddlYears = selectListItemsYear;
        //            //vmQcIncentive.ProjectMastersForPenaltiesAll 
        //            // var newList = GlobalStrings.Append(localStrings)

        //            List<ProjectMasterModel> GlobalStrings = new List<ProjectMasterModel>();

        //            if (mons != 0 && Year != null && projectName == "All")
        //            {
        //                String connectionString1 = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
        //                List<ProjectMasterModel> totalhistry = new List<ProjectMasterModel>();

        //                string resultQuery1 = "";

        //                using (var connection = new SqlConnection(connectionString1))
        //                {
        //                    //120
        //                    connection.Open();
        //                    resultQuery1 = String.Format(@"select distinct Model as ProjectName,FORMAT(ReleaseDate, 'yyyy-MM-dd') as ReleaseDate 
        //                    from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice ttp where GETDATE()-ReleaseDate <= 200  and   
        //                    ReleaseDate in (select top 1 ReleaseDate from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice where ttp.Model=Model and 			
        //                    GETDATE()-(select top 1 ReleaseDate from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice where ttp.Model=Model order by ReleaseDate asc) <= 200 order by ReleaseDate asc)");

        //                    var command = new SqlCommand(resultQuery1, connection);
        //                    command.CommandTimeout = 6000;
        //                    SqlDataReader dataReader = command.ExecuteReader();


        //                    while (dataReader.Read())
        //                    {
        //                        var Model = (string)dataReader["ProjectName"];
        //                        String ReleaseDate1 = (string)dataReader["ReleaseDate"];

        //                        DateTime curntDate = DateTime.Now;

        //                        DateTime ReleaseDate = Convert.ToDateTime(ReleaseDate1);
        //                        DateTime endDate = ReleaseDate.AddDays(120);

        //                        DateTime startDate;

        //                        DateTime firstTime = ReleaseDate.AddDays(30);
        //                        var fM = firstTime.ToString("MM");
        //                        var fY = firstTime.ToString("yyyy");

        //                        DateTime secondTime = ReleaseDate.AddDays(60);
        //                        var sM = secondTime.ToString("MM");
        //                        var sY = secondTime.ToString("yyyy");

        //                        DateTime thirdTime = ReleaseDate.AddDays(90);
        //                        var tM = thirdTime.ToString("MM");
        //                        var tY = thirdTime.ToString("yyyy");

        //                        DateTime fourthTime = ReleaseDate.AddDays(120);
        //                        var foM = fourthTime.ToString("MM");
        //                        var foY = fourthTime.ToString("yyyy");


        //                        var curMon1 = mons.ToString();
        //                        var curMon2 = mons.ToString();
        //                        var curMon3 = mons.ToString();
        //                        var curMon4 = mons.ToString();

        //                        if (fM.StartsWith("0"))
        //                        {
        //                            curMon1 = 0 + curMon1;
        //                        }
        //                        if (sM.StartsWith("0"))
        //                        {
        //                            curMon2 = 0 + curMon2;
        //                        }
        //                        if (tM.StartsWith("0"))
        //                        {
        //                            curMon3 = 0 + curMon3;
        //                        }
        //                        if (foM.StartsWith("0"))
        //                        {
        //                            curMon4 = 0 + curMon4;
        //                        }
        //                        //end

        //                        if (ReleaseDate <= endDate && firstTime <= curntDate && (fM == curMon1 && fY == Year))
        //                        {
        //                            startDate = ReleaseDate;
        //                            var fNames = "30Days";
        //                            vmQcIncentive.ProjectMastersForPenaltiesAll = _repository.GetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);

        //                            GlobalStrings.AddRange(vmQcIncentive.ProjectMastersForPenaltiesAll);
        //                        }
        //                        if (secondTime > firstTime && secondTime <= curntDate && (sM == curMon2 && sY == Year))
        //                        {
        //                            startDate = firstTime.AddDays(1);
        //                            var fNames = "60Days";
        //                            vmQcIncentive.ProjectMastersForPenaltiesAll = _repository.GetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);
        //                            GlobalStrings.AddRange(vmQcIncentive.ProjectMastersForPenaltiesAll);
        //                        }

        //                        if (thirdTime > secondTime && thirdTime <= curntDate && (tM == curMon3 && tY == Year))
        //                        {
        //                            var fNames = "90Days";
        //                            startDate = secondTime.AddDays(1);
        //                            vmQcIncentive.ProjectMastersForPenaltiesAll = _repository.GetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);
        //                            GlobalStrings.AddRange(vmQcIncentive.ProjectMastersForPenaltiesAll);
        //                        }
        //                        if (fourthTime > thirdTime && fourthTime <= curntDate && (foM == curMon4 && foY == Year))
        //                        {
        //                            startDate = thirdTime.AddDays(1);
        //                            var fNames = "120Days";
        //                            vmQcIncentive.ProjectMastersForPenaltiesAll = _repository.GetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);
        //                            GlobalStrings.AddRange(vmQcIncentive.ProjectMastersForPenaltiesAll);
        //                        }

        //                    }
        //                }
        //            }
        //            else if (mons != 0 && Year != null && projectName != "All")
        //            {
        //                //vmQcIncentive.ProjectMastersForPenalties = _repository.GetAllServiceissueHistories(mons, years, projectName);
        //                String connectionString1 = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
        //                List<ProjectMasterModel> totalhistry = new List<ProjectMasterModel>();

        //                string resultQuery1 = "";

        //                using (var connection = new SqlConnection(connectionString1))
        //                {
        //                    connection.Open();
        //                    //120
        //                    resultQuery1 = String.Format(@"select distinct Model as ProjectName,FORMAT(ReleaseDate, 'yyyy-MM-dd') as ReleaseDate 
        //                from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice ttp where GETDATE()-ReleaseDate <= 200  and   Model='{0}' and
        //                ReleaseDate in (select top 1 ReleaseDate from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice where ttp.Model=Model and 			
        //                GETDATE()-(select top 1 ReleaseDate from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice where ttp.Model=Model order by ReleaseDate asc) <= 200 order by ReleaseDate asc)", projectName);

        //                    var command = new SqlCommand(resultQuery1, connection);
        //                    command.CommandTimeout = 6000;
        //                    SqlDataReader dataReader = command.ExecuteReader();

        //                    List<ProjectMasterModel> proLists = new List<ProjectMasterModel>();
        //                    while (dataReader.Read())
        //                    {
        //                        var Model = (string)dataReader["ProjectName"];
        //                        String ReleaseDate1 = (string)dataReader["ReleaseDate"];

        //                        DateTime curntDate = DateTime.Now;

        //                        DateTime ReleaseDate = Convert.ToDateTime(ReleaseDate1);
        //                        DateTime endDate = ReleaseDate.AddDays(120);

        //                        DateTime startDate;

        //                        DateTime firstTime = ReleaseDate.AddDays(30);
        //                        var fM = firstTime.ToString("MM");
        //                        var fY = firstTime.ToString("yyyy");

        //                        DateTime secondTime = ReleaseDate.AddDays(60);
        //                        var sM = secondTime.ToString("MM");
        //                        var sY = secondTime.ToString("yyyy");

        //                        DateTime thirdTime = ReleaseDate.AddDays(90);
        //                        var tM = thirdTime.ToString("MM");
        //                        var tY = thirdTime.ToString("yyyy");

        //                        DateTime fourthTime = ReleaseDate.AddDays(120);
        //                        var foM = fourthTime.ToString("MM");
        //                        var foY = fourthTime.ToString("yyyy");

        //                        //current month and year//
        //                        //var curMon = DateTime.Now.ToString("MM");
        //                        //int curMons;
        //                        //int.TryParse(curMon, out curMons);
        //                        //var curYear = DateTime.Now.ToString("yyyy");

        //                        var curMon1 = mons.ToString();
        //                        var curMon2 = mons.ToString();
        //                        var curMon3 = mons.ToString();
        //                        var curMon4 = mons.ToString();

        //                        if (fM.StartsWith("0"))
        //                        {
        //                            curMon1 = 0 + curMon1;
        //                        }
        //                        if (sM.StartsWith("0"))
        //                        {
        //                            curMon2 = 0 + curMon2;
        //                        }
        //                        if (tM.StartsWith("0"))
        //                        {
        //                            curMon3 = 0 + curMon3;
        //                        }
        //                        if (foM.StartsWith("0"))
        //                        {
        //                            curMon4 = 0 + curMon4;
        //                        }
        //                        //end

        //                        if (ReleaseDate <= endDate && firstTime <= curntDate && (fM == curMon1 && fY == Year))
        //                        {
        //                            startDate = ReleaseDate;
        //                            var fNames = "30Days";
        //                            vmQcIncentive.ProjectMastersForPenalties = _repository.SaveGetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);

        //                        }
        //                        if (secondTime > firstTime && secondTime <= curntDate && (sM == curMon2 && sY == Year))
        //                        {
        //                            startDate = firstTime.AddDays(1);
        //                            var fNames = "60Days";
        //                            vmQcIncentive.ProjectMastersForPenalties = _repository.SaveGetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);

        //                        }

        //                        if (thirdTime > secondTime && thirdTime <= curntDate && (tM == curMon3 && tY == Year))
        //                        {
        //                            var fNames = "90Days";
        //                            startDate = secondTime.AddDays(1);
        //                            vmQcIncentive.ProjectMastersForPenalties = _repository.SaveGetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);

        //                        }
        //                        if (fourthTime > thirdTime && fourthTime <= curntDate && (foM == curMon4 && foY == Year))
        //                        {
        //                            startDate = thirdTime.AddDays(1);
        //                            var fNames = "120Days";
        //                            vmQcIncentive.ProjectMastersForPenalties = _repository.SaveGetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);

        //                        }

        //                    }
        //                }
        //            }//end of else statement

        //            vmQcIncentive.Month = MonNum1;
        //            vmQcIncentive.Year = Year;
        //            ViewBag.AllProjectValues = GlobalStrings;
        //            return View(vmQcIncentive);
        //        }
        #endregion
        public ActionResult All_QcMembersPenaltiesCalculation(string MonNum1, string Year, string projectName, string penaltiesTypes)
        {
            int mons;
            int.TryParse(MonNum1, out mons);

            var vmQcIncentive = new VmAllIncentiveList();
            vmQcIncentive.ProjectMasterModels = _repository.GetAllProjectNamesForPenalties();
            List<SelectListItem> items = new List<SelectListItem> { new SelectListItem { Value = "All", Text = "All" } };
            items.AddRange(vmQcIncentive.ProjectMasterModels.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectName.ToString(CultureInfo.InvariantCulture) }).ToList());
            ViewBag.ProjectsName = items;
            //
            List<SelectListItem> selectListItemsMonth = new List<SelectListItem>();
            selectListItemsMonth.Add(new SelectListItem() { Text = "SELECT MONTH", Value = "0" });
            selectListItemsMonth.Add(new SelectListItem() { Text = "All", Value = "All" });
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
            List<ProjectMasterModel> GlobalStringsForPerProject = new List<ProjectMasterModel>();
            List<ProjectMasterModel> GlobalStrings = new List<ProjectMasterModel>();

            if (penaltiesTypes == "PenaltiesModels")
            {
                #region com
                //
                //                if ((MonNum1 == "All") && Year != null && projectName != "All")
                //                {

                //                    String connectionString1 = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
                //                    List<ProjectMasterModel> totalhistry = new List<ProjectMasterModel>();

                //                    string resultQuery1 = "";

                //                    using (var connection = new SqlConnection(connectionString1))
                //                    {
                //                        connection.Open();
                //                        //120
                //                        resultQuery1 = String.Format(@"select distinct Model as ProjectName,FORMAT(ReleaseDate, 'yyyy-MM-dd') as ReleaseDate 
                //                from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice ttp where GETDATE()-ReleaseDate <= 200  and   Model='{0}' and
                //                ReleaseDate in (select top 1 ReleaseDate from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice where ttp.Model=Model and 			
                //                GETDATE()-(select top 1 ReleaseDate from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice where ttp.Model=Model order by ReleaseDate asc) <= 200 order by ReleaseDate asc)", projectName);

                //                        var command = new SqlCommand(resultQuery1, connection);
                //                        command.CommandTimeout = 6000;
                //                        SqlDataReader dataReader = command.ExecuteReader();

                //                        List<ProjectMasterModel> proLists = new List<ProjectMasterModel>();
                //                        while (dataReader.Read())
                //                        {
                //                            var Model = (string)dataReader["ProjectName"];
                //                            String ReleaseDate1 = (string)dataReader["ReleaseDate"];

                //                            DateTime curntDate = DateTime.Now;

                //                            DateTime ReleaseDate = Convert.ToDateTime(ReleaseDate1);
                //                            DateTime endDate = ReleaseDate.AddDays(120);

                //                            DateTime startDate;

                //                            DateTime firstTime = ReleaseDate.AddDays(30);

                //                            DateTime secondTime = ReleaseDate.AddDays(60);

                //                            DateTime thirdTime = ReleaseDate.AddDays(90);

                //                            DateTime fourthTime = ReleaseDate.AddDays(120);
                //                            //end

                //                            if (ReleaseDate <= endDate && firstTime <= curntDate)
                //                            {
                //                                startDate = ReleaseDate;
                //                                var fNames = "30Days";
                //                                vmQcIncentive.ProjectMastersForPenalties = _repository.SaveGetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);
                //                                GlobalStringsForPerProject.AddRange(vmQcIncentive.ProjectMastersForPenalties);
                //                            }
                //                            if (secondTime > firstTime && secondTime <= curntDate)
                //                            {
                //                                startDate = firstTime.AddDays(1);
                //                                var fNames = "60Days";
                //                                vmQcIncentive.ProjectMastersForPenalties = _repository.SaveGetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);
                //                                GlobalStringsForPerProject.AddRange(vmQcIncentive.ProjectMastersForPenalties);
                //                            }

                //                            if (thirdTime > secondTime && thirdTime <= curntDate)
                //                            {
                //                                var fNames = "90Days";
                //                                startDate = secondTime.AddDays(1);
                //                                vmQcIncentive.ProjectMastersForPenalties = _repository.SaveGetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);
                //                                GlobalStringsForPerProject.AddRange(vmQcIncentive.ProjectMastersForPenalties);
                //                            }
                //                            if (fourthTime > thirdTime && fourthTime <= curntDate)
                //                            {
                //                                startDate = thirdTime.AddDays(1);
                //                                var fNames = "120Days";
                //                                vmQcIncentive.ProjectMastersForPenalties = _repository.SaveGetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);
                //                                GlobalStringsForPerProject.AddRange(vmQcIncentive.ProjectMastersForPenalties);
                //                            }

                //                        }
                //                    }
                //                }//end of else statement
                //                else if (MonNum1 != "All" && Year != null && projectName != "All")
                //                {
                //                    String connectionString1 = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
                //                    List<ProjectMasterModel> totalhistry = new List<ProjectMasterModel>();

                //                    string resultQuery1 = "";

                //                    using (var connection = new SqlConnection(connectionString1))
                //                    {
                //                        connection.Open();
                //                        //120
                //                        resultQuery1 = String.Format(@"select distinct Model as ProjectName,FORMAT(ReleaseDate, 'yyyy-MM-dd') as ReleaseDate 
                //                from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice ttp where GETDATE()-ReleaseDate <= 200  and   Model='{0}' and
                //                ReleaseDate in (select top 1 ReleaseDate from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice where ttp.Model=Model and 			
                //                GETDATE()-(select top 1 ReleaseDate from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice where ttp.Model=Model order by ReleaseDate asc) <= 200 order by ReleaseDate asc)", projectName);

                //                        var command = new SqlCommand(resultQuery1, connection);
                //                        command.CommandTimeout = 6000;
                //                        SqlDataReader dataReader = command.ExecuteReader();

                //                        List<ProjectMasterModel> proLists = new List<ProjectMasterModel>();
                //                        while (dataReader.Read())
                //                        {
                //                            var Model = (string)dataReader["ProjectName"];
                //                            String ReleaseDate1 = (string)dataReader["ReleaseDate"];

                //                            DateTime curntDate = DateTime.Now;

                //                            DateTime ReleaseDate = Convert.ToDateTime(ReleaseDate1);
                //                            DateTime endDate = ReleaseDate.AddDays(120);


                //                            DateTime startDate;
                //                            DateTime firstTime = ReleaseDate.AddDays(30);
                //                            var fM = firstTime.ToString("MM");
                //                            var fY = firstTime.ToString("yyyy");

                //                            DateTime secondTime = ReleaseDate.AddDays(60);
                //                            var sM = secondTime.ToString("MM");
                //                            var sY = secondTime.ToString("yyyy");


                //                            DateTime thirdTime = ReleaseDate.AddDays(90);
                //                            var tM = thirdTime.ToString("MM");
                //                            var tY = thirdTime.ToString("yyyy");
                //                            var tM1 = secondTime.ToString("MM");


                //                            DateTime fourthTime = ReleaseDate.AddDays(120);
                //                            var foM = fourthTime.ToString("MM");
                //                            var foY = fourthTime.ToString("yyyy");
                //                            var foM1 = secondTime.ToString("MM");

                //                            var curMon1 = mons.ToString();
                //                            var curMon2 = mons.ToString();
                //                            var curMon3 = mons.ToString();
                //                            var curMon4 = mons.ToString();

                //                            if (fM.StartsWith("0"))
                //                            {
                //                                curMon1 = 0 + curMon1;
                //                            }
                //                            if (sM.StartsWith("0"))
                //                            {
                //                                curMon2 = 0 + curMon2;
                //                            }
                //                            if (tM.StartsWith("0"))
                //                            {
                //                                curMon3 = 0 + curMon3;
                //                            }
                //                            if (foM.StartsWith("0"))
                //                            {
                //                                curMon4 = 0 + curMon4;
                //                            }


                //                            //end

                //                            if (ReleaseDate <= endDate && firstTime <= curntDate && Convert.ToInt32(fM) <= Convert.ToInt32(curMon1))
                //                            {
                //                                startDate = ReleaseDate;
                //                                var fNames = "30Days";
                //                                vmQcIncentive.ProjectMastersForPenalties = _repository.SaveGetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);
                //                                GlobalStringsForPerProject.AddRange(vmQcIncentive.ProjectMastersForPenalties);
                //                            }
                //                            if (secondTime > firstTime && secondTime <= curntDate && Convert.ToInt32(sM) <= Convert.ToInt32(curMon2) && sM == tM1)
                //                            {
                //                                startDate = firstTime.AddDays(1);
                //                                var fNames = "60Days";
                //                                vmQcIncentive.ProjectMastersForPenalties = _repository.SaveGetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);
                //                                GlobalStringsForPerProject.AddRange(vmQcIncentive.ProjectMastersForPenalties);
                //                            }

                //                            if (thirdTime > secondTime && thirdTime <= curntDate && Convert.ToInt32(tM) <= Convert.ToInt32(curMon3) && tM1 == foM1)
                //                            {
                //                                var fNames = "90Days";
                //                                startDate = secondTime.AddDays(1);
                //                                vmQcIncentive.ProjectMastersForPenalties = _repository.SaveGetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);
                //                                GlobalStringsForPerProject.AddRange(vmQcIncentive.ProjectMastersForPenalties);
                //                            }
                //                            if (fourthTime > thirdTime && fourthTime <= curntDate && Convert.ToInt32(foM) <= Convert.ToInt32(curMon4))
                //                            {
                //                                startDate = thirdTime.AddDays(1);
                //                                var fNames = "120Days";
                //                                vmQcIncentive.ProjectMastersForPenalties = _repository.SaveGetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);
                //                                GlobalStringsForPerProject.AddRange(vmQcIncentive.ProjectMastersForPenalties);
                //                            }

                //                        }
                //                    }//end using
                //                }
                //                else if ((MonNum1 != "All" && MonNum1 != "0") && Year != null && projectName == "All")
                //                {
                //                    //
                //                    vmQcIncentive.ProjectMastersForPenaltiesAll = _repository.GetDataFromPenaltiesTable(MonNum1, Year);
                //                    ViewBag.AllProjectValues = vmQcIncentive.ProjectMastersForPenaltiesAll;

                //                    //
                //                }

                #endregion
                //new

                if ((MonNum1 != "All" && MonNum1 != "0") && Year != null && projectName == "All")
                {
                    //
                    vmQcIncentive.ProjectMastersForPenaltiesAll = _repository.GetDataFromPenaltiesTable(MonNum1, Year);
                    ViewBag.AllProjectValues = vmQcIncentive.ProjectMastersForPenaltiesAll;

                    //
                }
                else if ((MonNum1 == "All" && MonNum1 != "0") && Year != null && projectName != "All")
                {
                    //
                    vmQcIncentive.ProjectMastersForPenaltiesAll = _repository.GetDataFromPenaltiesTablePerProject(MonNum1, Year, projectName);
                    // ViewBag.AllProjectValues = vmQcIncentive.ProjectMastersForPenaltiesAll;
                    GlobalStringsForPerProject.AddRange(vmQcIncentive.ProjectMastersForPenaltiesAll);
                    //
                }

            }
            else if (penaltiesTypes == "PenaltiesProcesses")
            {
                if ((MonNum1 != "All" && MonNum1 != "0") && Year != null && projectName == "All")
                {
                    String connectionString1 = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
                    List<ProjectMasterModel> totalhistry = new List<ProjectMasterModel>();

                    string resultQuery1 = "";

                    using (var connection = new SqlConnection(connectionString1))
                    {
                        connection.Open();
                        //120
                        resultQuery1 = String.Format(@"select distinct Model as ProjectName,FORMAT(ReleaseDate, 'yyyy-MM-dd') as ReleaseDate 
                from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice ttp where GETDATE()-ReleaseDate <= 200  and
                ReleaseDate in (select top 1 ReleaseDate from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice where ttp.Model=Model and 			
                GETDATE()-(select top 1 ReleaseDate from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice where ttp.Model=Model order by ReleaseDate asc) <= 200 order by ReleaseDate asc)", projectName);

                        var command = new SqlCommand(resultQuery1, connection);
                        command.CommandTimeout = 6000;
                        SqlDataReader dataReader = command.ExecuteReader();

                        List<ProjectMasterModel> proLists = new List<ProjectMasterModel>();
                        while (dataReader.Read())
                        {
                            var Model = (string)dataReader["ProjectName"];
                            String ReleaseDate1 = (string)dataReader["ReleaseDate"];

                            DateTime curntDate = DateTime.Now;

                            DateTime ReleaseDate = Convert.ToDateTime(ReleaseDate1);
                            DateTime endDate = ReleaseDate.AddDays(120);


                            DateTime startDate;
                            DateTime firstTime = ReleaseDate.AddDays(30);
                            var fM = firstTime.ToString("MM");
                            var fY = firstTime.ToString("yyyy");

                            DateTime secondTime = ReleaseDate.AddDays(60);
                            var sM = secondTime.ToString("MM");
                            var sY = secondTime.ToString("yyyy");


                            DateTime thirdTime = ReleaseDate.AddDays(90);
                            var tM = thirdTime.ToString("MM");
                            var tY = thirdTime.ToString("yyyy");
                            var tM1 = secondTime.ToString("MM");


                            DateTime fourthTime = ReleaseDate.AddDays(120);
                            var foM = fourthTime.ToString("MM");
                            var foY = fourthTime.ToString("yyyy");
                            var foM1 = thirdTime.ToString("MM");

                            var curMon1 = mons.ToString();
                            var curMon2 = mons.ToString();
                            var curMon3 = mons.ToString();
                            var curMon4 = mons.ToString();

                            //var curMon1 = fM;
                            //var curMon2 = sM;
                            //var curMon3 = tM;
                            //var curMon4 = foM;

                            if (fM.StartsWith("0"))
                            {
                                curMon1 = 0 + curMon1;
                            }
                            if (sM.StartsWith("0"))
                            {
                                curMon2 = 0 + curMon2;
                            }
                            if (tM.StartsWith("0"))
                            {
                                curMon3 = 0 + curMon3;
                            }
                            if (foM.StartsWith("0"))
                            {
                                curMon4 = 0 + curMon4;
                            }


                            //end
                            if (fM == curMon1 && fY == Year)
                            {
                                if (ReleaseDate <= endDate && firstTime <= curntDate && Convert.ToInt32(fM) <= Convert.ToInt32(curMon1))
                                {
                                    startDate = ReleaseDate;
                                    var fNames = "30Days";
                                    vmQcIncentive.ProjectMastersForPenalties = _repository.SaveGetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);
                                    //  GlobalStringsForPerProject.AddRange(vmQcIncentive.ProjectMastersForPenalties);
                                }
                            }
                            if (sM == curMon2 && sY == Year)
                            {
                                if (secondTime > firstTime && secondTime <= curntDate && Convert.ToInt32(sM) <= Convert.ToInt32(curMon2) && sM == tM1)
                                {
                                    startDate = firstTime.AddDays(1);
                                    var fNames = "60Days";
                                    vmQcIncentive.ProjectMastersForPenalties = _repository.SaveGetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);
                                    // GlobalStringsForPerProject.AddRange(vmQcIncentive.ProjectMastersForPenalties);
                                }
                            }

                            if (tM == curMon3 && tY == Year)
                            {
                                if (thirdTime > secondTime && thirdTime <= curntDate && Convert.ToInt32(tM) <= Convert.ToInt32(curMon3) && tM == foM1)  //tM1 == foM1
                                {
                                    var fNames = "90Days";
                                    startDate = secondTime.AddDays(1);
                                    vmQcIncentive.ProjectMastersForPenalties = _repository.SaveGetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);
                                    // GlobalStringsForPerProject.AddRange(vmQcIncentive.ProjectMastersForPenalties);
                                }
                            }

                            if (foM == curMon4 && foY == Year)
                            {
                                if (fourthTime > thirdTime && fourthTime <= curntDate && Convert.ToInt32(foM) <= Convert.ToInt32(curMon4))
                                {
                                    startDate = thirdTime.AddDays(1);
                                    var fNames = "120Days";
                                    vmQcIncentive.ProjectMastersForPenalties = _repository.SaveGetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);
                                    //GlobalStringsForPerProject.AddRange(vmQcIncentive.ProjectMastersForPenalties);
                                }
                            }
                        }
                    }//end using
                    //
                    vmQcIncentive.ProjectMastersForPenaltiesAll = _repository.GetDataFromPenaltiesTable(MonNum1, Year);
                    ViewBag.AllProjectValues = vmQcIncentive.ProjectMastersForPenaltiesAll;
                    //
                }//end of else statement
                else if ((MonNum1 != "All" && MonNum1 != "0") && Year != null && projectName != "All")
                {
                    String connectionString1 = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
                    List<ProjectMasterModel> totalhistry = new List<ProjectMasterModel>();

                    string resultQuery1 = "";

                    using (var connection = new SqlConnection(connectionString1))
                    {
                        connection.Open();
                        //120
                        resultQuery1 = String.Format(@"select distinct Model as ProjectName,FORMAT(ReleaseDate, 'yyyy-MM-dd') as ReleaseDate 
                from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice ttp where  Model='{0}' and GETDATE()-ReleaseDate <= 200  and
                ReleaseDate in (select top 1 ReleaseDate from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice where ttp.Model=Model and 			
                GETDATE()-(select top 1 ReleaseDate from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice where ttp.Model=Model order by ReleaseDate asc) <= 200 order by ReleaseDate asc)", projectName);

                        var command = new SqlCommand(resultQuery1, connection);
                        command.CommandTimeout = 6000;
                        SqlDataReader dataReader = command.ExecuteReader();

                        List<ProjectMasterModel> proLists = new List<ProjectMasterModel>();
                        while (dataReader.Read())
                        {
                            var Model = (string)dataReader["ProjectName"];
                            String ReleaseDate1 = (string)dataReader["ReleaseDate"];

                            DateTime curntDate = DateTime.Now;

                            DateTime ReleaseDate = Convert.ToDateTime(ReleaseDate1);
                            DateTime endDate = ReleaseDate.AddDays(120);


                            DateTime startDate;
                            DateTime firstTime = ReleaseDate.AddDays(30);
                            var fM = firstTime.ToString("MM");
                            var fY = firstTime.ToString("yyyy");

                            DateTime secondTime = ReleaseDate.AddDays(60);
                            var sM = secondTime.ToString("MM");
                            var sY = secondTime.ToString("yyyy");


                            DateTime thirdTime = ReleaseDate.AddDays(90);
                            var tM = thirdTime.ToString("MM");
                            var tY = thirdTime.ToString("yyyy");
                            var tM1 = secondTime.ToString("MM");


                            DateTime fourthTime = ReleaseDate.AddDays(120);
                            var foM = fourthTime.ToString("MM");
                            var foY = fourthTime.ToString("yyyy");
                            var foM1 = thirdTime.ToString("MM");

                            var curMon1 = mons.ToString();
                            var curMon2 = mons.ToString();
                            var curMon3 = mons.ToString();
                            var curMon4 = mons.ToString();

                            //var curMon1 = fM;
                            //var curMon2 = sM;
                            //var curMon3 = tM;
                            //var curMon4 = foM;

                            if (fM.StartsWith("0"))
                            {
                                curMon1 = 0 + curMon1;
                            }
                            if (sM.StartsWith("0"))
                            {
                                curMon2 = 0 + curMon2;
                            }
                            if (tM.StartsWith("0"))
                            {
                                curMon3 = 0 + curMon3;
                            }
                            if (foM.StartsWith("0"))
                            {
                                curMon4 = 0 + curMon4;
                            }


                            //end
                            if (fM == curMon1 && fY == Year)
                            {
                                if (ReleaseDate <= endDate && firstTime <= curntDate && Convert.ToInt32(fM) <= Convert.ToInt32(curMon1))
                                {
                                    startDate = ReleaseDate;
                                    var fNames = "30Days";
                                    vmQcIncentive.ProjectMastersForPenalties = _repository.SaveGetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);
                                    //  GlobalStringsForPerProject.AddRange(vmQcIncentive.ProjectMastersForPenalties);
                                }
                            }
                            if (sM == curMon2 && sY == Year)
                            {
                                if (secondTime > firstTime && secondTime <= curntDate && Convert.ToInt32(sM) <= Convert.ToInt32(curMon2) && sM == tM1)
                                {
                                    startDate = firstTime.AddDays(1);
                                    var fNames = "60Days";
                                    vmQcIncentive.ProjectMastersForPenalties = _repository.SaveGetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);
                                    // GlobalStringsForPerProject.AddRange(vmQcIncentive.ProjectMastersForPenalties);
                                }
                            }

                            if (tM == curMon3 && tY == Year)
                            {
                                if (thirdTime > secondTime && thirdTime <= curntDate && Convert.ToInt32(tM) <= Convert.ToInt32(curMon3) && tM == foM1)  //tM1 == foM1
                                {
                                    var fNames = "90Days";
                                    startDate = secondTime.AddDays(1);
                                    vmQcIncentive.ProjectMastersForPenalties = _repository.SaveGetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);
                                    // GlobalStringsForPerProject.AddRange(vmQcIncentive.ProjectMastersForPenalties);
                                }
                            }

                            if (foM == curMon4 && foY == Year)
                            {
                                if (fourthTime > thirdTime && fourthTime <= curntDate && Convert.ToInt32(foM) <= Convert.ToInt32(curMon4))
                                {
                                    startDate = thirdTime.AddDays(1);
                                    var fNames = "120Days";
                                    vmQcIncentive.ProjectMastersForPenalties = _repository.SaveGetAllModelsHistory(Model, ReleaseDate1, startDate, endDate, fNames, mons, Year);
                                    //GlobalStringsForPerProject.AddRange(vmQcIncentive.ProjectMastersForPenalties);
                                }
                            }
                           
                        }
                    }//end using
                    //
                    vmQcIncentive.ProjectMastersForPenaltiesAll = _repository.GetDataFromPenaltiesTable(MonNum1, Year);
                    ViewBag.AllProjectValues = vmQcIncentive.ProjectMastersForPenaltiesAll;
                    //
                }
            }




            vmQcIncentive.Month = MonNum1;
            vmQcIncentive.Year = Year;

            ViewBag.PerProjectValues = GlobalStringsForPerProject;
            return View(vmQcIncentive);
        }
        #endregion

        #region Accessories Test
        public ActionResult AccessoriesTest(string ProjectsDetails)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var vmSwQcSpecification = new VmSwQcSpecificationModified();

            var selectListItemsForRef = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT PROJECT" } };
            List<SwQcAssignsFromQcHeadModel> list = _repository.GetAccessoriesModel(userId) ??
                                                     new List<SwQcAssignsFromQcHeadModel>();
            selectListItemsForRef.AddRange(list.Select(p => new SelectListItem { Value = p.SwQcHeadAssignId.ToString() + ',' + p.SwQcAssignId.ToString() + ',' + p.ProjectType + ',' + p.ProjectMasterId.ToString() + ',' + p.TestPhaseID.ToString() + ',' + p.AccessoriesTestType.ToString(), Text = p.ProjectName + " (" + p.AccessoriesTestType + ")" }));
            ViewBag.GetAccessoriesModel = selectListItemsForRef;
            vmSwQcSpecification.ProjectsDetails = ProjectsDetails;
            FileManager fileManager = new FileManager();

            if (ProjectsDetails != null)
            {
                var proSplit = ProjectsDetails.Split(',');

                var swQcheadId = proSplit[0].Trim();
                var swQcAssignId = proSplit[1].Trim();
                ViewBag.GetSavedAccessoriesDataEarphone = _repository.GetSavedAccessoriesDataEarphone(swQcheadId, swQcAssignId);

                ViewBag.GetSavedAccessoriesDataBattery = _repository.GetSavedAccessoriesDataBattery(swQcheadId, swQcAssignId);

                vmSwQcSpecification.SwQcGlassProtectorTests = _repository.GetSavedGlassProtectorAndChargerData(swQcheadId, swQcAssignId);
                if (vmSwQcSpecification.SwQcGlassProtectorTests.Any())
                {
                    foreach (var model in vmSwQcSpecification.SwQcGlassProtectorTests)
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
            }

            return View(vmSwQcSpecification);
        }
        [HttpPost]
        public ActionResult AccessoriesTest(List<SwQcEarphoneTestModel> issueList, List<SwQcEarphoneTestModel> issueList1, List<SwQcBatteryTestModel> issueList3, List<SwQcBatteryTestModel> issueList4, String SwQcHeadAssignId, String SwQcAssignId, String ProjectType, String ProjectMasterId, String TestPhaseID, String AccessoriesTestType)
        {
            issueList = issueList.Where(x => x.IsRemoved == 0).ToList();
            issueList1 = issueList1.Where(x => x.IsRemoved == 0).ToList();
            issueList3 = issueList3.Where(x => x.IsRemoved == 0).ToList();
            issueList4 = issueList4.Where(x => x.IsRemoved == 0).ToList();

            long userId = Convert.ToInt64(User.Identity.Name);
            // if (ModelState.IsValid)
            //  {
            long swQcHeadAssignId;
            long.TryParse(SwQcHeadAssignId, out swQcHeadAssignId);

            long swQcAssignId;
            long.TryParse(SwQcAssignId, out swQcAssignId);

            long proId;
            long.TryParse(ProjectMasterId, out proId);

            long testId;
            long.TryParse(TestPhaseID, out testId);

            if (AccessoriesTestType == "Earphone")
            {
                _repository.SaveAccessoriesTest(issueList, issueList1, swQcHeadAssignId, swQcAssignId, ProjectType, proId, testId);
            }
            else if (AccessoriesTestType == "Battery")
            {
                _repository.SaveBatteryTest(issueList3, swQcHeadAssignId, swQcAssignId, ProjectType, proId, testId, AccessoriesTestType);
            }

            else if (AccessoriesTestType == "Glass Protector" || AccessoriesTestType == "Charger")
            {
                var manager = new FileManager();

                if (AccessoriesTestType == "Glass Protector")
                {
                    foreach (var glass in issueList4)
                    {
                        if (glass.File.Count() > 0 && glass.File != null)
                        {
                            var res = manager.Upload3(proId, swQcHeadAssignId, swQcAssignId, "SwQcGlassProtector",
                                "SwQcGlassProtectorImage", glass.File);
                            glass.Upload = glass.Upload == null ? res : glass.Upload + "|" + res;

                        }
                    }
                }
                if (AccessoriesTestType == "Charger")
                {
                    foreach (var glass in issueList4)
                    {
                        if (glass.File.Count() > 0 && glass.File != null)
                        {
                            var res = manager.Upload3(proId, swQcHeadAssignId, swQcAssignId, "SwQcCharger",
                                "SwQcChargerImage", glass.File);
                            glass.Upload = glass.Upload == null ? res : glass.Upload + "|" + res;

                        }
                    }
                }
                _repository.SaveGlassProtectorAndChargerTest(issueList4, swQcHeadAssignId, swQcAssignId, ProjectType, proId, testId, AccessoriesTestType);
            }

            // }

            return RedirectToAction("AccessoriesTest", new { ProjectsDetails = SwQcHeadAssignId + ',' + SwQcAssignId + ',' + ProjectType + ',' + ProjectMasterId + ',' + TestPhaseID + ',' + AccessoriesTestType });
        }

        [HttpPost]
        public JsonResult AccessoriesIssueDelete(SwQcEarphoneTestModel supplierUpdate)
        {
            var vmSwQcSpecification = new VmSwQcSpecificationModified();
            var saveAccess = _repository.SaveAccessoriesIssueDelete(supplierUpdate);

            return new JsonResult { Data = saveAccess, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult SaveEditAccessoriesData(String AccessId, String HeadphoneModel1, String MusicPlayerPlayback,
            String VideoPlayerPlayback, String VoiceCall, String VoiceCallController,
            String FmPlayback, String FmController, String Controller, String Remarks, String MusicBase,
            String YoutubePlayback, String YoutubeController, String VolumeController, String HighEndDevice, String MidRangeDevice, String LowerMidRangeDevice, String LowRangeDevice)
        {

            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);


            long AccessIds = 0;
            long.TryParse(AccessId, out AccessIds);
            var saveIncentive = "";
            if (AccessIds != 0)
            {
                saveIncentive = _repository.SaveEditAccessoriesData(AccessIds, HeadphoneModel1, MusicPlayerPlayback, VideoPlayerPlayback, VoiceCall, VoiceCallController, FmPlayback,
                    FmController, Controller, Remarks, MusicBase, YoutubePlayback, YoutubeController, VolumeController, HighEndDevice, MidRangeDevice, LowerMidRangeDevice, LowRangeDevice);

            }

            return new JsonResult { Data = saveIncentive, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult SaveAccessoriesSubmittedProjectToQcHead(String ProjectDetails)
        {
            var vmSwQcSpecification = new VmSwQcSpecificationModified();

            long userId = Convert.ToInt64(User.Identity.Name);


            var proDetails = ProjectDetails.Split(',');

            var swQcHeadId = proDetails[0].Trim();
            var swQcAssignId = proDetails[1].Trim();

            long swQcHeadIds;
            long.TryParse(swQcHeadId, out swQcHeadIds);

            long swQcAssignIds;
            long.TryParse(swQcAssignId, out swQcAssignIds);

            if (swQcHeadIds != 0)
            {
                _repository.SaveAccessoriesSubmittedProjectToQcHead(swQcHeadIds, swQcAssignIds);
            }

            return new JsonResult { Data = "OK", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult AccessoriesTestExcel(string ProjectName, string AllOrLatest, string ProjectType, string AccessoriesCategories)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var vmSwQcSpecification = new VmSwQcSpecificationModified();

            var selectListItemsForRef = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT PROJECT" } };
            List<SwQcHeadAssignsFromPmModel> list = _repository.GetAccessoriesModelForExcel() ??
                                                     new List<SwQcHeadAssignsFromPmModel>();
            selectListItemsForRef.AddRange(list.Select(p => new SelectListItem { Value = p.ProjectName + ',' + p.ProjectType, Text = p.ProjectName }));
            ViewBag.GetAccessoriesModelForExcel = selectListItemsForRef;


            List<SelectListItem> selectListAll = new List<SelectListItem>();
            selectListAll.Add(new SelectListItem() { Text = "SELECT", Value = "0" });
            selectListAll.Add(new SelectListItem() { Text = "All", Value = "All" });
            selectListAll.Add(new SelectListItem() { Text = "Latest", Value = "Latest" });
            ViewBag.AllOrLatest = selectListAll;

            List<SelectListItem> selectListCategories = new List<SelectListItem>();
            selectListCategories.Add(new SelectListItem() { Text = "SELECT", Value = "0" });
            selectListCategories.Add(new SelectListItem() { Text = "Battery", Value = "Battery" });
            selectListCategories.Add(new SelectListItem() { Text = "Earphone", Value = "Earphone" });
            selectListCategories.Add(new SelectListItem() { Text = "Charger", Value = "Charger" });
            selectListCategories.Add(new SelectListItem() { Text = "Glass Protector", Value = "Glass Protector" });

            ViewBag.AccessoriesCategories = selectListCategories;


            if ((ProjectName != null && AllOrLatest != null && AccessoriesCategories != null) && (ProjectName != "0" && AllOrLatest != "0" && AccessoriesCategories != "0"))
            {

                if (AccessoriesCategories.Trim() == "Earphone")
                {
                    ViewBag.GetAccessListForExportEarphone = _repository.GetAccessListForExportEarphone(ProjectName, AllOrLatest);

                }
                else if (AccessoriesCategories.Trim() == "Battery")
                {
                    ViewBag.GetAccessListForExportBattery = _repository.GetAccessListForExportBattery(ProjectName, AllOrLatest);

                }
                else if (AccessoriesCategories.Trim() == "Glass Protector" || AccessoriesCategories.Trim() == "Charger")
                {
                    ViewBag.GetAccessListForExportGlassProtectorAndCharger = _repository.GetAccessListForExportGlassProtectorAndCharger(ProjectName, AllOrLatest, AccessoriesCategories);

                }
            }
            vmSwQcSpecification.ProjectName = ProjectName;
            vmSwQcSpecification.ProjectType = ProjectType;
            vmSwQcSpecification.AllOrLatest = AllOrLatest;
            vmSwQcSpecification.AccessoriesCategories = AccessoriesCategories;

            return View(vmSwQcSpecification);
        }
        [HttpPost]
        public JsonResult BatteryIssueDelete(SwQcBatteryTestModel supplierUpdate)
        {
            var saveAccess = _repository.SaveBatteryIssueDelete(supplierUpdate);

            return new JsonResult { Data = saveAccess, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult SaveEditBatteryData(String BatteryId, String CheckPoints1, String BatterymAh,
            String HundredToNighty, String NightyToEighty, String EightyToSeventy,
            String SeventyToSixty, String SixtyToFifty, String FiftyToFourty, String FourtyToThirty, String ThirtyToTwenty,
            String TwentyToTen, String TenToZero, String AverageFullDischarge)
        {

            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);


            long BatteryIds = 0;
            long.TryParse(BatteryId, out BatteryIds);
            var saveIncentive = "";
            if (BatteryIds != 0)
            {
                saveIncentive = _repository.SaveEditBatteryData(BatteryIds, CheckPoints1, BatterymAh, HundredToNighty, NightyToEighty, EightyToSeventy, SeventyToSixty,
                    SixtyToFifty, FiftyToFourty, FourtyToThirty, ThirtyToTwenty, TwentyToTen, TenToZero, AverageFullDischarge);

            }

            return new JsonResult { Data = saveIncentive, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion

        #region Excel For Earphone
        public static DataTable GetEarphoneData(string ProjectName, string ProjectType, string AllOrLatest, string AccessoriesCategories)
        {
            DataTable totalhistry = new DataTable();

            var cn = new SqlConnection(_connectionStringCellphone);

            //var cn =
            //   new SqlConnection(
            //       "Data Source=test;initial catalog=CellPhoneProject;persist security info=True;user id=test;password=test;MultipleActiveResultSets=True;App=EntityFramework");


            //var cn =
            //    new SqlConnection(
            //        "Data Source=test;initial catalog=CellPhoneProject;persist security info=True;user id=test;password=test;MultipleActiveResultSets=True;App=EntityFramework");

            cn.Open();

            if (AccessoriesCategories.Trim() == "Earphone")
            {
                if (ProjectType != null && ProjectType == "Feature")
                {
                    totalhistry.Columns.Add("Headphone Model");
                    totalhistry.Columns.Add("Music Player Playback");
                    totalhistry.Columns.Add("Video Player Playback");
                    totalhistry.Columns.Add("Voice Call");
                    totalhistry.Columns.Add("Voice Call controller & Hold");
                    totalhistry.Columns.Add("FM Playback & Controller");
                    totalhistry.Columns.Add("Controller");
                    totalhistry.Columns.Add("Remarks");
                }
                else if (ProjectType != null)
                {
                    totalhistry.Columns.Add("Headphone Model");
                    totalhistry.Columns.Add("Music Player Playback");
                    totalhistry.Columns.Add("Video Player Playback");
                    totalhistry.Columns.Add("Music Base");
                    totalhistry.Columns.Add("Youtube Playback");
                    totalhistry.Columns.Add("Youtube Controller");
                    totalhistry.Columns.Add("Voice Call (Receive/Drop/Mute)");
                    totalhistry.Columns.Add("Voice Call controller");
                    totalhistry.Columns.Add("FM Playback");
                    totalhistry.Columns.Add("FM Controller");
                    totalhistry.Columns.Add("Volume Controller");
                    totalhistry.Columns.Add("Remarks");
                    totalhistry.Columns.Add("High End Device");
                    totalhistry.Columns.Add("Mid Range Device");
                    totalhistry.Columns.Add("Lower Mid Range Device");
                    totalhistry.Columns.Add("Low Range Device");
                }

                String sql = "";

                if ((ProjectName.Trim() != null && AllOrLatest.Trim() != null) && (ProjectName != "0" && AllOrLatest != "0"))
                {

                    if (AllOrLatest == "Latest")
                    {
                        sql = String.Format(@"select * from
                        (select SwQcHeadAssignId,ProjectName,ProjectType,HeadphoneModel,MusicPlayerPlayback,VideoPlayerPlayback,VoiceCall,VoiceCallController,FmPlayback,FmController,
                        Controller,Remarks,MusicBase,YoutubePlayback,YoutubeController,VolumeController,HighEndDevice,MidRangeDevice,LowerMidRangeDevice,LowRangeDevice
                        from CellPhoneProject.dbo.SwQcEarphoneTest
                        where ProjectName='{0}'
                        group by SwQcHeadAssignId,ProjectName,ProjectType,HeadphoneModel,MusicPlayerPlayback,VideoPlayerPlayback,VoiceCall,VoiceCallController,FmPlayback,FmController,
                        Controller,Remarks,MusicBase,YoutubePlayback,YoutubeController,FmController,VolumeController,HighEndDevice,MidRangeDevice,LowerMidRangeDevice,LowRangeDevice)s
                        where SwQcHeadAssignId=(select top 1 ss.SwQcHeadAssignId from CellPhoneProject.dbo.SwQcEarphoneTest ss where ss.ProjectName='{0}' order by ss.SwQcHeadAssignId desc)

                        order by s.SwQcHeadAssignId desc", ProjectName.Trim(), AllOrLatest.Trim());
                    }
                    else if (AllOrLatest == "All")
                    {
                        sql = String.Format(@"select SwQcHeadAssignId,ProjectName,ProjectType,HeadphoneModel,MusicPlayerPlayback,VideoPlayerPlayback,VoiceCall,VoiceCallController,FmPlayback,FmController,
                        Controller,Remarks,MusicBase,YoutubePlayback,YoutubeController,FmController,VolumeController,HighEndDevice,MidRangeDevice,LowerMidRangeDevice,LowRangeDevice
                        from CellPhoneProject.dbo.SwQcEarphoneTest
                        where ProjectName='{0}'
                        group by SwQcHeadAssignId,ProjectName,ProjectType,HeadphoneModel,MusicPlayerPlayback,VideoPlayerPlayback,VoiceCall,VoiceCallController,FmPlayback,FmController,
                        Controller,Remarks,MusicBase,YoutubePlayback,YoutubeController,FmController,VolumeController,HighEndDevice,MidRangeDevice,LowerMidRangeDevice,LowRangeDevice

                        order by SwQcHeadAssignId desc", ProjectName.Trim(), AllOrLatest.Trim());
                    }

                }


                SqlCommand cmd = new SqlCommand(sql, cn);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        if (ProjectType != null && ProjectType == "Feature")
                        {
                            DataRow newRow = totalhistry.NewRow();

                            newRow["Headphone Model"] = rdr["HeadphoneModel"].ToString();
                            newRow["Music Player Playback"] = rdr["MusicPlayerPlayback"].ToString();
                            newRow["Video Player Playback"] = rdr["VideoPlayerPlayback"].ToString();
                            newRow["Voice Call"] = rdr["VoiceCall"].ToString();
                            newRow["Voice Call controller & Hold"] = rdr["VoiceCallController"].ToString();
                            newRow["FM Playback & Controller"] = rdr["FmPlayback"].ToString();
                            newRow["Controller"] = rdr["Controller"].ToString();
                            newRow["Remarks"] = rdr["Remarks"].ToString();

                            totalhistry.Rows.Add(newRow);
                        }
                        else if (ProjectType != null)
                        {
                            DataRow newRow = totalhistry.NewRow();

                            newRow["Headphone Model"] = rdr["HeadphoneModel"].ToString();
                            newRow["Music Player Playback"] = rdr["MusicPlayerPlayback"].ToString();
                            newRow["Video Player Playback"] = rdr["VideoPlayerPlayback"].ToString();
                            newRow["Music Base"] = rdr["MusicBase"].ToString();
                            newRow["Youtube Playback"] = rdr["YoutubePlayback"].ToString();
                            newRow["Youtube Controller"] = rdr["YoutubeController"].ToString();
                            newRow["Voice Call (Receive/Drop/Mute)"] = rdr["VoiceCall"].ToString();
                            newRow["Voice Call Controller"] = rdr["VoiceCallController"].ToString();
                            newRow["FM Playback"] = rdr["FmPlayback"].ToString();
                            newRow["FM Controller"] = rdr["FmController"].ToString();
                            newRow["Volume Controller"] = rdr["VolumeController"].ToString();
                            newRow["Remarks"] = rdr["Remarks"].ToString();
                            newRow["High End Device"] = rdr["HighEndDevice"].ToString();
                            newRow["Mid Range Device"] = rdr["MidRangeDevice"].ToString();
                            newRow["Lower Mid Range Device"] = rdr["LowerMidRangeDevice"].ToString();
                            newRow["Low Range Device"] = rdr["LowRangeDevice"].ToString();

                            totalhistry.Rows.Add(newRow);
                        }

                    }
                }
            }//end Earphone


            //Stary Battery
            else if (AccessoriesCategories.Trim() == "Battery")
            {

                totalhistry.Columns.Add("Check points/Charging slots >");
                totalhistry.Columns.Add("100-90%");
                totalhistry.Columns.Add("90-80%");
                totalhistry.Columns.Add("80-70%");
                totalhistry.Columns.Add("70-60%");
                totalhistry.Columns.Add("60-50%");
                totalhistry.Columns.Add("50-40%");
                totalhistry.Columns.Add("40-30%");
                totalhistry.Columns.Add("30-20%");
                totalhistry.Columns.Add("20-10%");
                totalhistry.Columns.Add("10-0%");
                totalhistry.Columns.Add("Average full Discharge");

                String sql = "";

                if ((ProjectName.Trim() != null && AllOrLatest.Trim() != null && AccessoriesCategories.Trim() != null) && (ProjectName != "0" && AllOrLatest != "0" && AccessoriesCategories != "0"))
                {

                    if (AllOrLatest == "Latest")
                    {
                        sql = String.Format(@"select * from
                        (select SwQcHeadAssignId,ProjectName,ProjectType,CheckPoints,[BatterymAh],[HundredToNighty],[NightyToEighty],[EightyToSeventy],[SeventyToSixty],[SixtyToFifty],[FiftyToFourty],[FourtyToThirty],
                        [ThirtyToTwenty],[TwentyToTen],[TenToZero],[AverageFullDischarge]
                        from CellPhoneProject.dbo.[SwQcBatteryTest]
                        where ProjectName='{0}'
                        group by SwQcHeadAssignId,ProjectName,ProjectType,CheckPoints,[BatterymAh],[HundredToNighty],[NightyToEighty],[EightyToSeventy],[SeventyToSixty],[SixtyToFifty],[FiftyToFourty],[FourtyToThirty],
                        [ThirtyToTwenty],[TwentyToTen],[TenToZero],[AverageFullDischarge])s
                        where SwQcHeadAssignId=(select top 1 ss.SwQcHeadAssignId from CellPhoneProject.dbo.[SwQcBatteryTest] ss where ss.ProjectName='{0}' order by ss.SwQcHeadAssignId desc)
                        order by s.SwQcHeadAssignId desc", ProjectName.Trim(), AllOrLatest.Trim());
                    }
                    else if (AllOrLatest == "All")
                    {
                        sql = String.Format(@"select SwQcHeadAssignId,ProjectName,ProjectType,CheckPoints,[BatterymAh],[HundredToNighty],[NightyToEighty],[EightyToSeventy],[SeventyToSixty],[SixtyToFifty],[FiftyToFourty],[FourtyToThirty],
                        [ThirtyToTwenty],[TwentyToTen],[TenToZero],[AverageFullDischarge] from CellPhoneProject.dbo.[SwQcBatteryTest]
                        where ProjectName='{0}'
                        group by SwQcHeadAssignId,ProjectName,ProjectType,CheckPoints,[BatterymAh],[HundredToNighty],[NightyToEighty],[EightyToSeventy],[SeventyToSixty],[SixtyToFifty],[FiftyToFourty],[FourtyToThirty],
                        [ThirtyToTwenty],[TwentyToTen],[TenToZero],[AverageFullDischarge]
                        order by SwQcHeadAssignId asc", ProjectName.Trim(), AllOrLatest.Trim());
                    }

                }


                SqlCommand cmd = new SqlCommand(sql, cn);

                var BatterymAh = "";
                DataTable totalhistry1 = new DataTable();
                DataRow newRow2 = totalhistry1.NewRow();
                totalhistry1.Columns.Add("test");
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    int count = 0;

                    while (rdr.Read())
                    {
                        count += 1;
                        DataRow newRow = totalhistry.NewRow();

                        if (count == 1)
                        {
                            newRow2["test"] = rdr["BatterymAh"].ToString();
                            totalhistry1.Rows.Add(newRow2);
                            totalhistry.Merge(totalhistry1);
                        }

                        newRow["Check points/Charging slots >"] = rdr["CheckPoints"].ToString();
                        newRow["100-90%"] = rdr["HundredToNighty"].ToString();
                        newRow["90-80%"] = rdr["NightyToEighty"].ToString();
                        newRow["80-70%"] = rdr["EightyToSeventy"].ToString();
                        newRow["70-60%"] = rdr["SeventyToSixty"].ToString();
                        newRow["60-50%"] = rdr["SixtyToFifty"].ToString();
                        newRow["50-40%"] = rdr["FiftyToFourty"].ToString();
                        newRow["40-30%"] = rdr["FourtyToThirty"].ToString();
                        newRow["30-20%"] = rdr["ThirtyToTwenty"].ToString();
                        newRow["20-10%"] = rdr["TwentyToTen"].ToString();
                        newRow["10-0%"] = rdr["TenToZero"].ToString();
                        newRow["Average full Discharge"] = rdr["AverageFullDischarge"].ToString();
                        totalhistry.Rows.Add(newRow);

                    }

                }

            }//end Battery
            //Start glass
            else if (AccessoriesCategories.Trim() == "Glass Protector")
            {
                totalhistry.Columns.Add("Issue Sr.");
                totalhistry.Columns.Add("Issue Scenario");
                totalhistry.Columns.Add("Expected Outcome");
                totalhistry.Columns.Add("Walton QC Comment");

                String sql = "";

                if ((ProjectName.Trim() != null && AllOrLatest.Trim() != null && AccessoriesCategories.Trim() != null) && (ProjectName != "0" && AllOrLatest != "0" && AccessoriesCategories != "0"))
                {

                    if (AllOrLatest == "Latest")
                    {
                        sql = String.Format(@"select * from
                        (
	                        select *
	                        from CellPhoneProject.dbo.[SwQcGlassProtectorTest]
	                        where ProjectName='{0}'
                        )s
                        where SwQcHeadAssignId=(select top 1 ss.SwQcHeadAssignId from CellPhoneProject.dbo.[SwQcGlassProtectorTest] ss where ss.ProjectName={0} order by ss.SwQcHeadAssignId desc)
                        order by s.SwQcHeadAssignId desc", ProjectName.Trim(), AllOrLatest.Trim());
                    }
                    else if (AllOrLatest == "All")
                    {
                        sql = String.Format(@"select distinct *
                        from CellPhoneProject.dbo.[SwQcGlassProtectorTest]
                        where ProjectName='{0}' order by SwQcHeadAssignId desc", ProjectName.Trim(), AllOrLatest.Trim());
                    }

                }


                SqlCommand cmd = new SqlCommand(sql, cn);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        DataRow newRow = totalhistry.NewRow();

                        newRow["Issue Sr."] = rdr["IssueSerial"].ToString();
                        newRow["Issue Scenario"] = rdr["IssueScenario"].ToString();
                        newRow["Expected Outcome"] = rdr["ExpectedOutcome"].ToString();
                        newRow["Walton QC Comment"] = rdr["WaltonQcStatus"].ToString();
                        totalhistry.Rows.Add(newRow);
                    }
                }

            }//end glass
            //Start charger
            else if (AccessoriesCategories.Trim() == "Charger")
            {
                totalhistry.Columns.Add("Issue Sr.");
                totalhistry.Columns.Add("Issue Scenario");
                totalhistry.Columns.Add("Expected Outcome");
                totalhistry.Columns.Add("Walton QC Comment");

                String sql = "";

                if ((ProjectName.Trim() != null && AllOrLatest.Trim() != null && AccessoriesCategories.Trim() != null) && (ProjectName != "0" && AllOrLatest != "0" && AccessoriesCategories != "0"))
                {

                    if (AllOrLatest == "Latest")
                    {
                        sql = String.Format(@"select * from
                        (
	                        select *
	                        from CellPhoneProject.dbo.[SwQcChargerTest]
	                        where ProjectName='{0}'
                        )s
                        where SwQcHeadAssignId=(select top 1 ss.SwQcHeadAssignId from CellPhoneProject.dbo.[SwQcChargerTest] ss where ss.ProjectName={0} order by ss.SwQcHeadAssignId desc)
                        order by s.SwQcHeadAssignId desc", ProjectName.Trim(), AllOrLatest.Trim());
                    }
                    else if (AllOrLatest == "All")
                    {
                        sql = String.Format(@"select distinct *
                        from CellPhoneProject.dbo.[SwQcChargerTest]
                        where ProjectName='{0}' order by SwQcHeadAssignId desc", ProjectName.Trim(), AllOrLatest.Trim());
                    }

                }


                SqlCommand cmd = new SqlCommand(sql, cn);

                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        DataRow newRow = totalhistry.NewRow();

                        newRow["Issue Sr."] = rdr["IssueSerial"].ToString();
                        newRow["Issue Scenario"] = rdr["IssueScenario"].ToString();
                        newRow["Expected Outcome"] = rdr["ExpectedOutcome"].ToString();
                        newRow["Walton QC Comment"] = rdr["WaltonQcStatus"].ToString();
                        totalhistry.Rows.Add(newRow);
                    }
                }

            }//end glass
            return totalhistry;
        }

        public void GetExcelForEarphone(DataTable ds, string ProjectName, string ProjectType, string AllOrLatest, string AccessoriesCategories)
        {
            var vms = new VmSwQcSpecificationModified();

            //Creae an Excel application instance
            Excel.Application excelApp = new Excel.Application();

            Excel.Workbook excelWorkBook = excelApp.Workbooks.Add("");

            //Add a new worksheet to workbook with the Datatable name
            Excel.Worksheet excelWorkSheet = excelWorkBook.Sheets.Add();

            excelWorkSheet.Name = ProjectName;


            //Adjust all row
            excelWorkSheet.Rows.AutoFit();

            string dd = "";

            var proTypeList = ProjectType.Split(',');
            var proType = proTypeList[1].Trim();

            if (AccessoriesCategories.Trim() == "Earphone")
            {
                if (proType != null && proType == "Feature")
                {
                    //Column width and Hight//
                    excelWorkSheet.Range["A2", "P2"].Rows.RowHeight = 40;

                    excelWorkSheet.Columns[1].ColumnWidth = 20;
                    excelWorkSheet.Columns[2].ColumnWidth = 20;
                    excelWorkSheet.Columns[3].ColumnWidth = 20;
                    excelWorkSheet.Columns[4].ColumnWidth = 20;
                    excelWorkSheet.Columns[5].ColumnWidth = 20;
                    excelWorkSheet.Columns[6].ColumnWidth = 20;
                    excelWorkSheet.Columns[7].ColumnWidth = 20;
                    excelWorkSheet.Columns[8].ColumnWidth = 25;

                    /////////

                    //wrap text//
                    excelWorkSheet.get_Range("A2", "H2").Style.WrapText = true;
                    //Adjust all column
                    excelWorkSheet.Columns.AutoFit();

                    //For Issue List Color Group 1//
                    excelWorkSheet.get_Range("A2", "H2").Font.Bold = true;
                    // excelWorkSheet.get_Range("A2", "G2").Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Yellow);
                    excelWorkSheet.get_Range("A2", "H2").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    excelWorkSheet.get_Range("A2", "H2").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                    excelWorkSheet.get_Range("A2", "H2").Application.StandardFont = "Calibri";
                    excelWorkSheet.get_Range("A2", "H2").Application.StandardFontSize = 11;

                    excelWorkSheet.get_Range("A2", "H2").Borders.get_Item(Excel.XlBordersIndex.xlEdgeLeft).LineStyle = Excel.XlLineStyle.xlContinuous;
                    excelWorkSheet.get_Range("A2", "H2").Borders.get_Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle = Excel.XlLineStyle.xlContinuous;
                    excelWorkSheet.get_Range("A2", "H2").Borders.get_Item(Excel.XlBordersIndex.xlInsideHorizontal).LineStyle = Excel.XlLineStyle.xlContinuous;
                    excelWorkSheet.get_Range("A2", "H2").Borders.get_Item(Excel.XlBordersIndex.xlInsideVertical).LineStyle = Excel.XlLineStyle.xlContinuous;
                    excelWorkSheet.get_Range("A2", "H2").Borders.Weight = Excel.XlBorderWeight.xlThick;
                    //end Issue List Color Group 1

                    DataTable dt = new DataTable();
                    using (dt = GetEarphoneData(ProjectName, ProjectType, AllOrLatest, AccessoriesCategories))
                    {

                        for (int i = 1; i < dt.Columns.Count + 1; i++)
                        {

                            excelWorkSheet.Cells[2, i] = dt.Columns[i - 1].ColumnName;
                        }

                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            for (int k = 0; k < dt.Columns.Count; k++)
                            {
                                excelWorkSheet.Cells[j + 3, k + 1] = dt.Rows[j].ItemArray[k].ToString();
                                //excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;


                                excelWorkSheet.Cells[j + 3, k + 1].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                                excelWorkSheet.Cells[j + 3, k + 1].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                                excelWorkSheet.Cells[j + 3, k + 1].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                                excelWorkSheet.Cells[j + 3, k + 1].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                                excelWorkSheet.Cells[j + 3, k + 1].Borders.Weight = Excel.XlBorderWeight.xlThick;

                                excelWorkSheet.Cells[j + 3, k + 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                                excelWorkSheet.Cells[j + 3, k + 1].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;

                                excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightYellow);
                            }
                        }
                    }
                    //end
                }
                else if (proType != null)
                {
                    //Column width and Hight//
                    excelWorkSheet.Range["A2", "P2"].Rows.RowHeight = 40;

                    excelWorkSheet.Columns[1].ColumnWidth = 15;
                    excelWorkSheet.Columns[2].ColumnWidth = 15;
                    excelWorkSheet.Columns[3].ColumnWidth = 15;
                    excelWorkSheet.Columns[4].ColumnWidth = 15;
                    excelWorkSheet.Columns[5].ColumnWidth = 15;
                    excelWorkSheet.Columns[6].ColumnWidth = 15;
                    excelWorkSheet.Columns[7].ColumnWidth = 20;
                    excelWorkSheet.Columns[8].ColumnWidth = 15;
                    excelWorkSheet.Columns[9].ColumnWidth = 15;
                    excelWorkSheet.Columns[10].ColumnWidth = 15;
                    excelWorkSheet.Columns[11].ColumnWidth = 15;
                    excelWorkSheet.Columns[12].ColumnWidth = 20;
                    excelWorkSheet.Columns[13].ColumnWidth = 15;
                    excelWorkSheet.Columns[14].ColumnWidth = 15;
                    excelWorkSheet.Columns[15].ColumnWidth = 15;
                    excelWorkSheet.Columns[16].ColumnWidth = 15;
                    /////////

                    //wrap text//
                    excelWorkSheet.get_Range("A2", "P2").Style.WrapText = true;


                    //Adjust all column
                    excelWorkSheet.Columns.AutoFit();

                    //For Issue List Color Group 1//
                    excelWorkSheet.get_Range("A2", "P2").Font.Bold = true;
                    // excelWorkSheet.get_Range("A2", "G2").Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Yellow);
                    excelWorkSheet.get_Range("A2", "P2").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    excelWorkSheet.get_Range("A2", "P2").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                    excelWorkSheet.get_Range("A2", "P2").Application.StandardFont = "Calibri";
                    excelWorkSheet.get_Range("A2", "P2").Application.StandardFontSize = 11;

                    excelWorkSheet.get_Range("A2", "P2").Borders.get_Item(Excel.XlBordersIndex.xlEdgeLeft).LineStyle = Excel.XlLineStyle.xlContinuous;
                    excelWorkSheet.get_Range("A2", "P2").Borders.get_Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle = Excel.XlLineStyle.xlContinuous;
                    excelWorkSheet.get_Range("A2", "P2").Borders.get_Item(Excel.XlBordersIndex.xlInsideHorizontal).LineStyle = Excel.XlLineStyle.xlContinuous;
                    excelWorkSheet.get_Range("A2", "P2").Borders.get_Item(Excel.XlBordersIndex.xlInsideVertical).LineStyle = Excel.XlLineStyle.xlContinuous;
                    excelWorkSheet.get_Range("A2", "P2").Borders.Weight = Excel.XlBorderWeight.xlThick;
                    //end Issue List Color Group 1

                    DataTable dt = new DataTable();
                    using (dt = GetEarphoneData(ProjectName, ProjectType, AllOrLatest, AccessoriesCategories))
                    {

                        for (int i = 1; i < dt.Columns.Count + 1; i++)
                        {

                            excelWorkSheet.Cells[2, i] = dt.Columns[i - 1].ColumnName;
                        }

                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            for (int k = 0; k < dt.Columns.Count; k++)
                            {
                                excelWorkSheet.Cells[j + 3, k + 1] = dt.Rows[j].ItemArray[k].ToString();
                                //  excelWorkSheet.Cells[j + 3, k + 1].Font.Bold = true;


                                excelWorkSheet.Cells[j + 3, k + 1].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                                excelWorkSheet.Cells[j + 3, k + 1].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                                excelWorkSheet.Cells[j + 3, k + 1].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                                excelWorkSheet.Cells[j + 3, k + 1].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                                excelWorkSheet.Cells[j + 3, k + 1].Borders.Weight = Excel.XlBorderWeight.xlThick;

                                excelWorkSheet.Cells[j + 3, k + 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                                excelWorkSheet.Cells[j + 3, k + 1].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;

                                excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightYellow);
                            }
                        }
                    }
                    //end
                }

                //
                dd = ProjectName + "_Earphone_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xlsx";

            }

            else if (AccessoriesCategories.Trim() == "Battery")
            {

                //wrap text//
                excelWorkSheet.get_Range("B5", "M5").Style.WrapText = true;
                //For Header Name//
                excelWorkSheet.Cells[5, 2] = "Handset usages Time observation";
                excelWorkSheet.get_Range("B5", "M5").Font.Bold = true;
                excelWorkSheet.get_Range("B5", "M5").Font.Name = "Calibri";
                excelWorkSheet.get_Range("B5", "M5").Font.Size = 15;
                excelWorkSheet.get_Range("B5", "M5").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                //         excelWorkSheet.get_Range("B5", "M5").HorizontalAlignment =
                //Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

                //wrap text//
                excelWorkSheet.get_Range("B6", "M6").Style.WrapText = true;
                //For Project Name//
                excelWorkSheet.Cells[6, 2] = ProjectName;
                excelWorkSheet.get_Range("B6", "M6").Font.Bold = true;
                excelWorkSheet.get_Range("B6", "M6").Font.Name = "Calibri";
                excelWorkSheet.get_Range("B6", "M6").Font.Size = 10;
                excelWorkSheet.get_Range("B6", "M6").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                //         excelWorkSheet.get_Range("B6", "M6").HorizontalAlignment =
                //Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

                //Column width and Hight//
                excelWorkSheet.Range["B8", "M8"].Rows.RowHeight = 40;

                excelWorkSheet.Columns[2].ColumnWidth = 60;
                excelWorkSheet.Columns[3].ColumnWidth = 15;
                excelWorkSheet.Columns[4].ColumnWidth = 15;
                excelWorkSheet.Columns[5].ColumnWidth = 15;
                excelWorkSheet.Columns[6].ColumnWidth = 15;
                excelWorkSheet.Columns[7].ColumnWidth = 15;
                excelWorkSheet.Columns[8].ColumnWidth = 15;
                excelWorkSheet.Columns[9].ColumnWidth = 15;
                excelWorkSheet.Columns[10].ColumnWidth = 15;
                excelWorkSheet.Columns[11].ColumnWidth = 15;
                excelWorkSheet.Columns[12].ColumnWidth = 15;
                excelWorkSheet.Columns[13].ColumnWidth = 15;

                /////////

                //wrap text//
                excelWorkSheet.get_Range("B8", "M8").Style.WrapText = true;
                //Adjust all column
                excelWorkSheet.Columns.AutoFit();

                //For Issue List Color Group 1//
                excelWorkSheet.get_Range("B8", "M8").Font.Bold = true;
                excelWorkSheet.get_Range("B8", "M8").Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Silver);
                excelWorkSheet.get_Range("B8", "M8").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                excelWorkSheet.get_Range("B8", "M8").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                excelWorkSheet.get_Range("B8", "M8").Application.StandardFont = "Calibri";
                excelWorkSheet.get_Range("B8", "M8").Application.StandardFontSize = 11;

                excelWorkSheet.get_Range("B8", "M8").Borders.get_Item(Excel.XlBordersIndex.xlEdgeLeft).LineStyle = Excel.XlLineStyle.xlContinuous;
                excelWorkSheet.get_Range("B8", "M8").Borders.get_Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle = Excel.XlLineStyle.xlContinuous;
                excelWorkSheet.get_Range("B8", "M8").Borders.get_Item(Excel.XlBordersIndex.xlInsideHorizontal).LineStyle = Excel.XlLineStyle.xlContinuous;
                excelWorkSheet.get_Range("B8", "M8").Borders.get_Item(Excel.XlBordersIndex.xlInsideVertical).LineStyle = Excel.XlLineStyle.xlContinuous;
                excelWorkSheet.get_Range("B8", "M8").Borders.Weight = Excel.XlBorderWeight.xlThick;
                //end Issue List Color Group 1

                DataTable dt = new DataTable();
                using (dt = GetEarphoneData(ProjectName, ProjectType, AllOrLatest, AccessoriesCategories))
                {
                    for (int i = 1; i < dt.Columns.Count + 1; i++)
                    {

                        if (dt.Columns[i - 1].ColumnName != "test")
                            excelWorkSheet.Cells[8, i + 1] = dt.Columns[i - 1].ColumnName;

                    }

                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        for (int k = 0; k < dt.Columns.Count; k++)
                        {

                            if (k == 0 && j == 0)
                            {
                                var battaerymAh = dt.Rows[j].ItemArray[12].ToString();
                                excelWorkSheet.Cells[7, 2] = "Battery: " + battaerymAh + "mAh";

                                dt.Columns.Remove("test");

                                dt.Rows[0].Delete();
                                dt.AcceptChanges();

                            }
                            excelWorkSheet.Cells[j + 9, k + 2] = dt.Rows[j].ItemArray[k].ToString();

                            excelWorkSheet.Cells[j + 9, k + 2].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                            excelWorkSheet.Cells[j + 9, k + 2].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                            excelWorkSheet.Cells[j + 9, k + 2].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                            excelWorkSheet.Cells[j + 9, k + 2].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                            excelWorkSheet.Cells[j + 9, k + 2].Borders.Weight = Excel.XlBorderWeight.xlThin;

                        }
                        if (j == dt.Rows.Count - 1)
                        {
                            excelWorkSheet.Cells[j + 9 + j, 2] = "2 SIM active all time. ";
                            excelWorkSheet.Cells[j + 9 + j, 2].Font.Bold = true;

                            excelWorkSheet.Cells[j + 9 + j + 1, 2] = "Active adaptive brightness";
                            excelWorkSheet.Cells[j + 9 + j + 1, 2].Font.Bold = true;
                        }
                    }
                }
                //end
                //
                dd = ProjectName + "_Battery_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xlsx";
            }
            else if (AccessoriesCategories.Trim() == "Glass Protector" || AccessoriesCategories.Trim() == "Charger")
            {
                //Column width and Hight//
                excelWorkSheet.Range["A2", "D2"].Rows.RowHeight = 40;

                excelWorkSheet.Columns[1].ColumnWidth = 10;
                excelWorkSheet.Columns[2].ColumnWidth = 30;
                excelWorkSheet.Columns[3].ColumnWidth = 30;
                excelWorkSheet.Columns[4].ColumnWidth = 20;

                /////////

                //wrap text//
                excelWorkSheet.get_Range("A2", "C2").Style.WrapText = true;
                //Adjust all column
                excelWorkSheet.Columns.AutoFit();

                //For Issue List Color Group 1//
                excelWorkSheet.get_Range("A2", "C2").Font.Bold = true;
                excelWorkSheet.get_Range("A2", "C2").Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.DarkOrchid);
                excelWorkSheet.get_Range("A2", "C2").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                excelWorkSheet.get_Range("A2", "C2").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                excelWorkSheet.get_Range("A2", "C2").Application.StandardFont = "Calibri";
                excelWorkSheet.get_Range("A2", "C2").Application.StandardFontSize = 11;

                excelWorkSheet.get_Range("A2", "C2").Borders.get_Item(Excel.XlBordersIndex.xlEdgeLeft).LineStyle = Excel.XlLineStyle.xlContinuous;
                excelWorkSheet.get_Range("A2", "C2").Borders.get_Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle = Excel.XlLineStyle.xlContinuous;
                excelWorkSheet.get_Range("A2", "C2").Borders.get_Item(Excel.XlBordersIndex.xlInsideHorizontal).LineStyle = Excel.XlLineStyle.xlContinuous;
                excelWorkSheet.get_Range("A2", "C2").Borders.get_Item(Excel.XlBordersIndex.xlInsideVertical).LineStyle = Excel.XlLineStyle.xlContinuous;
                excelWorkSheet.get_Range("A2", "C2").Borders.Weight = Excel.XlBorderWeight.xlThick;
                //end Issue List Color Group 1

                //For Issue List Color Group 2//
                excelWorkSheet.get_Range("D2", "D2").Font.Bold = true;
                excelWorkSheet.get_Range("D2", "D2").Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Green);
                excelWorkSheet.get_Range("D2", "D2").Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                excelWorkSheet.get_Range("D2", "D2").HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                excelWorkSheet.get_Range("D2", "D2").Application.StandardFont = "Calibri";
                excelWorkSheet.get_Range("D2", "D2").Application.StandardFontSize = 11;

                excelWorkSheet.get_Range("D2", "D2").Borders.get_Item(Excel.XlBordersIndex.xlEdgeLeft).LineStyle = Excel.XlLineStyle.xlContinuous;
                excelWorkSheet.get_Range("D2", "D2").Borders.get_Item(Excel.XlBordersIndex.xlEdgeRight).LineStyle = Excel.XlLineStyle.xlContinuous;
                excelWorkSheet.get_Range("D2", "D2").Borders.get_Item(Excel.XlBordersIndex.xlInsideHorizontal).LineStyle = Excel.XlLineStyle.xlContinuous;
                excelWorkSheet.get_Range("D2", "D2").Borders.get_Item(Excel.XlBordersIndex.xlInsideVertical).LineStyle = Excel.XlLineStyle.xlContinuous;
                excelWorkSheet.get_Range("D2", "D2").Borders.Weight = Excel.XlBorderWeight.xlThick;
                //end Issue List Color Group 2
                DataTable dt = new DataTable();
                using (dt = GetEarphoneData(ProjectName, ProjectType, AllOrLatest, AccessoriesCategories))
                {

                    for (int i = 1; i < dt.Columns.Count + 1; i++)
                    {

                        excelWorkSheet.Cells[2, i] = dt.Columns[i - 1].ColumnName;
                    }

                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        for (int k = 0; k < dt.Columns.Count; k++)
                        {
                            excelWorkSheet.Cells[j + 3, k + 1] = dt.Rows[j].ItemArray[k].ToString();

                            excelWorkSheet.Cells[j + 3, k + 1].Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                            excelWorkSheet.Cells[j + 3, k + 1].Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                            excelWorkSheet.Cells[j + 3, k + 1].Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                            excelWorkSheet.Cells[j + 3, k + 1].Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                            excelWorkSheet.Cells[j + 3, k + 1].Borders.Weight = Excel.XlBorderWeight.xlThick;

                            excelWorkSheet.Cells[j + 3, k + 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            excelWorkSheet.Cells[j + 3, k + 1].VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;

                            excelWorkSheet.Cells[j + 3, k + 1].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                        }
                    }
                }
                //end

                if (AccessoriesCategories.Trim() == "Glass Protector")
                {
                    dd = ProjectName + "_GlassProtector_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xlsx";
                }
                else if (AccessoriesCategories.Trim() == "Charger")
                {
                    dd = ProjectName + "_Charger_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xlsx";
                }
            }
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
        #endregion

        #region Field Test New


        public ActionResult SwQcFieldTest(string ProjectsDetails)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var vmSwQcSpecification = new VmSwQcSpecificationModified();

            var selectListItemsForRef = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT PROJECT" } };
            List<SwQcAssignsFromQcHeadModel> list = _repository.GetFieldTestModel(userId) ??
                                                     new List<SwQcAssignsFromQcHeadModel>();
            selectListItemsForRef.AddRange(list.Select(p => new SelectListItem { Value = p.SwQcHeadAssignId.ToString() + ',' + p.SwQcAssignId.ToString() + ',' + p.ProjectType + ',' + p.ProjectMasterId.ToString() + ',' + p.TestPhaseID.ToString() + ',' + p.ProjectName, Text = p.ProjectName + "-->>(SV: " + p.SoftwareVersionNo + ")" + "-->>(AssignTime: " + p.SwQcAssignTimeByHead + ")" }));
            ViewBag.GetFieldTestModel = selectListItemsForRef;
            vmSwQcSpecification.ProjectsDetails = ProjectsDetails;
            var fileManager = new FileManager();
            if (ProjectsDetails != null)
            {
                var proSplit = ProjectsDetails.Split(',');

                var swQcheadId = proSplit[0].Trim();
                var swQcAssignId = proSplit[1].Trim();
                var proName = proSplit[5].Trim();

                ViewBag.GetFieldTestDetailsData = _repository.GetFieldTestDetailsData(swQcheadId, proName, swQcAssignId);
                ViewBag.GetFieldTestDetailsSavedData = _repository.GetFieldTestDetailsSavedData(swQcheadId, swQcAssignId);

                if (ViewBag.GetFieldTestDetailsSavedData != null)
                {
                    foreach (var model in ViewBag.GetFieldTestDetailsSavedData)
                    {
                        if (model.Attachment != null)
                        {
                            var urls = model.Attachment;

                            FilesDetail detail = new FilesDetail();
                            detail.FilePath = fileManager.GetFile(urls);
                            detail.Extention = fileManager.GetExtension(urls);
                            model.FilesDetails.Add(detail);

                        }
                    }
                }

            }
            vmSwQcSpecification.CurrDate = DateTime.Now;
            vmSwQcSpecification.ProjectsDetails = ProjectsDetails;

            return View(vmSwQcSpecification);
        }

        //public JsonResult SaveOrUpdateFieldTestData(string obj)

        [HttpPost]
        public ActionResult SwQcFieldTest(List<SwQcFieldTestDetailModel> issueList, List<SwQcFieldTestDetailModel> issueList1, String SoftwareVersionName1, String FrequencyBand33, String operatorStatus, String SwQcHeadAssignId, String SwQcAssignId, String ProjectType, String ProjectMasterId, String TestPhaseID, String ProjectName,
             String BenchmarkPhone1, String Route1, String Region1, String FieldTestResult1, String Remarks1, String Location1, String SpeedLimit1, String TRssiBars1, String BRssiBars1)
        {
            var manager = new FileManager();

            issueList = issueList.Where(x => x.IsRemoved == 0).ToList();

            long userId = Convert.ToInt64(User.Identity.Name);

            long swQcHeadAssignId;
            long.TryParse(SwQcHeadAssignId, out swQcHeadAssignId);

            long swQcAssignId;
            long.TryParse(SwQcAssignId, out swQcAssignId);

            long proId;
            long.TryParse(ProjectMasterId, out proId);

            long testId;
            long.TryParse(TestPhaseID, out testId);

            var Attachment = "";

            foreach (var items in issueList1)
            {
                if (items.FileId != null)
                {
                    var res = manager.Upload3(proId, swQcHeadAssignId, swQcAssignId,
                    "SwQcFieldTest", "SwQcFieldTestImage", items.FileId);

                    Console.Write("res  :" + res);

                    items.Attachment = items.Attachment == null ? res : items.Attachment + "|" + res;

                    Attachment = items.Attachment;
                }
            }
            foreach (var items in issueList)
            {
                if (items.IssueAttachmentIds != null)
                {
                    var res = manager.Upload3(proId, swQcHeadAssignId, swQcAssignId,
                    "SwQcFieldTest", "SwQcFieldTestImage", items.IssueAttachmentIds);

                    Console.Write("res  :" + res);

                    items.IssueAttachment = items.IssueAttachment == null ? res : items.IssueAttachment + "|" + res;

                }
            }


            if (swQcHeadAssignId != 0)
            {
                _repository.SaveOrUpdateFieldTestData(issueList, issueList1, SoftwareVersionName1, FrequencyBand33, operatorStatus, swQcHeadAssignId, swQcAssignId,
                    ProjectType, proId, testId, ProjectName, Attachment, BenchmarkPhone1, Route1, Region1,
                    FieldTestResult1, Remarks1, Location1, SpeedLimit1, TRssiBars1, BRssiBars1);
            }

            return RedirectToAction("SwQcFieldTest", new { ProjectsDetails = SwQcHeadAssignId + ',' + SwQcAssignId + ',' + ProjectType + ',' + ProjectMasterId + ',' + TestPhaseID + ',' + ProjectName });
        }

        [HttpPost]
        public JsonResult UpdateFieldOperatorData(String FieldTestId, String TRSSIbars, String BRSSIbars, String TCallDrop, String TNoiseInterference,
            String TLongMute, String BCallDrop, String BNoiseInterference, String BLongMute)
        {

            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            long FieldTestIds = 0;
            long.TryParse(FieldTestId, out FieldTestIds);
            var saveData = "";
            if (FieldTestIds != 0)
            {
                saveData = _repository.UpdateFieldOperatorData(FieldTestIds, TRSSIbars, BRSSIbars, TCallDrop, TNoiseInterference, TLongMute, BCallDrop, BNoiseInterference, BLongMute);

            }

            return new JsonResult { Data = saveData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult UpdateFieldRouteData(String FieldTestId, String BenchmarkPhone, String Route,
            String Region, String FrequencyBand, String FieldTestResult, String Remarks)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            long FieldTestIds = 0;
            long.TryParse(FieldTestId, out FieldTestIds);
            var saveData = "";
            if (FieldTestIds != 0)
            {
                saveData = _repository.UpdateFieldRouteData(FieldTestIds, BenchmarkPhone, Route, Region, FrequencyBand, FieldTestResult, Remarks);
            }
            return new JsonResult { Data = saveData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult UpdateFieldIssueData(String FieldTestId, String Issue, String ExpectedOutcome,
            String IssueType)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            long FieldTestIds = 0;
            long.TryParse(FieldTestId, out FieldTestIds);
            var saveData = "";
            if (FieldTestIds != 0)
            {
                saveData = _repository.UpdateFieldIssueData(FieldTestIds, Issue, ExpectedOutcome, IssueType);
            }
            return new JsonResult { Data = saveData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [HttpPost]
        public JsonResult DeleteFieldIssueData(String FieldTestId)
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);

            long FieldTestIds = 0;
            long.TryParse(FieldTestId, out FieldTestIds);
            var saveData = "";
            if (FieldTestIds != 0)
            {
                saveData = _repository.DeleteFieldIssueData(FieldTestIds);
            }
            return new JsonResult { Data = saveData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult FieldTestFinalSubmit(String ProjectsDetails)
        {
            var vmSwQcSpecification = new VmSwQcSpecificationModified();
            long userId = Convert.ToInt64(User.Identity.Name);

            if (ProjectsDetails != null)
            {
                if (ProjectsDetails != "")
                {
                    var proSplit = ProjectsDetails.Split(',');

                    var swQcheadId = proSplit[0].Trim();
                    var swQcAssignId = proSplit[1].Trim();
                    var proName = proSplit[5].Trim();

                    long swQcheadIds;
                    long.TryParse(swQcheadId, out swQcheadIds);

                    long swQcAssignIds;
                    long.TryParse(swQcAssignId, out swQcAssignIds);

                    _repository.FieldTestFinalSubmit(swQcheadIds, swQcAssignIds);
                }
            }
            vmSwQcSpecification.ProjectsDetails = ProjectsDetails;

            return new JsonResult { Data = "OK", JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        [Authorize(Roles = "QCHEAD,QC,SA,PM,PMHEAD")]
        public ActionResult SwQcFieldTestExcel(string ProjectsDetails)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var vmSwQcSpecification = new VmSwQcSpecificationModified();

            var selectListItemsForRef = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT PROJECT" } };
            List<SwQcAssignsFromQcHeadModel> list = _repository.GetFieldTestModelForPrint(userId) ??
                                                     new List<SwQcAssignsFromQcHeadModel>();
            selectListItemsForRef.AddRange(list.Select(p => new SelectListItem { Value = p.ProjectName, Text = p.ProjectName }));
            ViewBag.GetFieldTestModelForPrint = selectListItemsForRef;

            if (ProjectsDetails != null)
            {
                ViewBag.GetProjectDetailsForFieldDetails = _repository.GetProjectDetailsForFieldDetails(ProjectsDetails);

            }
            vmSwQcSpecification.ProjectsDetails = ProjectsDetails;
            return View(vmSwQcSpecification);
        }
        //SwQcFieldTestPrint
         [Authorize(Roles = "QCHEAD,QC,SA,PM,PMHEAD")]
        public ActionResult SwQcFieldTestPrint(string ProjectName, string SwQcHeadAssignId)
        {
            var vmSwQcSpecification = new VmSwQcSpecificationModified();


            long swQcheadIds;
            long.TryParse(SwQcHeadAssignId, out swQcheadIds);
            var fileManager = new FileManager();

            if (SwQcHeadAssignId != null)
            {
                ViewBag.GetDataForFieldTestPrint = _repository.GetDataForFieldTestPrint(swQcheadIds, ProjectName);
                if (ViewBag.GetDataForFieldTestPrint != null)
                {
                    foreach (var model in ViewBag.GetDataForFieldTestPrint)
                    {
                        if (model.Attachment != null)
                        {
                            var urls = model.Attachment;

                            FilesDetail detail = new FilesDetail();
                            detail.FilePath = fileManager.GetFile(urls);
                            detail.Extention = fileManager.GetExtension(urls);
                            model.FilesDetails.Add(detail);

                        }
                    }
                }
            }
            return View(vmSwQcSpecification);
        }
        #endregion

        #region others incentive
        public ActionResult SwQcOthersIncentive()
        {
            var vmQcIncentive = new VmAllIncentiveList();

            vmQcIncentive.CmnUserModels = _repository.GetQcUserList();

            var selectListItemsForRef = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT PERSON" } };
            vmQcIncentive.CmnUserModels = _repository.GetQcUserList() ??
                                                   new List<CmnUserModel>();
            selectListItemsForRef.AddRange(vmQcIncentive.CmnUserModels.Select(p => new SelectListItem { Value = p.EmployeeCode, Text = p.UserFullName + ' ' + '(' + p.EmployeeCode + ')' }));
            ViewBag.ddlUsers = selectListItemsForRef;

            var selectListItemsForTestPhase = new List<SelectListItem> { new SelectListItem { Value = "0", Text = "SELECT" } };
            List<SwQcTestPhaseModel> list = _repository.GetTestPhasesForQcHeadIssue() ??
                                                     new List<SwQcTestPhaseModel>();
            selectListItemsForTestPhase.AddRange(list.Select(p => new SelectListItem { Value = p.TestPhaseID.ToString(), Text = p.TestPhaseName }));
            ViewBag.ddlTestPhase = selectListItemsForTestPhase;
            ////

            vmQcIncentive.ProjectMasterModels = _repository.GetAllProjectName();
            List<SelectListItem> items = vmQcIncentive.ProjectMasterModels.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectName.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.Projects = items;

            ViewBag.GetOthersDetails = _repository.GetOthersDetails();

            return View(vmQcIncentive);
        }

        [HttpPost]
        public ActionResult AddProjectsForOthersField(string incentiveType, string projectNameForOthers, int testPhaseNameVal, string testPhaseName,
         int critical, int major, int minor, String[] sltPersonVal, DateTime effectiveMonth, string remarks)
        {

            var model = new Custom_Sw_IncentiveModel();

            model.IncentiveTypes = incentiveType;
            model.ProjectName = projectNameForOthers;
            model.TestPhaseId = testPhaseNameVal;
            model.TestPhaseName = testPhaseName;
            model.Critical = critical;
            model.Major = major;
            model.Minor = minor;

            var ssCount = sltPersonVal.Count();

            model.AssignedPersons = ssCount;

            foreach (var sp in sltPersonVal)
            {
                model.AssignedEmployees = model.AssignedEmployees + sp + ",";
            }
            var ss = model.AssignedEmployees.TrimEnd(',');
            model.AssignedEmployees = ss;
            model.EffectiveMonth = effectiveMonth;
            model.AddAmountRemarks = remarks;

            //model.AddedAmount = addedAmount != "" ? Convert.ToDecimal(addedAmount) : 0;
            //model.Deduction = othersDeduct != "" ? Convert.ToDecimal(othersDeduct) : 0;


            return PartialView("~/Views/Software/Partial/_SwQcOthersIncentiveField.cshtml", model);

        }

        #endregion

        #region Qc Work Progress
        public ActionResult QcWorkProgress()
        {
            ViewBag.GetRunningProjectForSwQcWork = _repository.GetRunningProjectForSwQcWork();
            ViewBag.GetRunningProjectCountForSw = _repository.GetRunningProjectCountForSw();
            ViewBag.GetRunningProjectForFtQcWork = _repository.GetRunningProjectForFtQcWork();
            ViewBag.GetRunningProjectCountForFt = _repository.GetRunningProjectCountForFt();

            ViewBag.GetNewProjectForSw = _repository.GetNewProjectForSw();
            ViewBag.GetNewProjectForSwCount = _repository.GetNewProjectForSwCount();
            ViewBag.GetNewProjectForFt = _repository.GetNewProjectForFt();
            ViewBag.GetNewProjectForFtCount = _repository.GetNewProjectForFtCount();

            ViewBag.GetAgentProgress = _repository.GetAgentProgress();

            return View();
        }
        #endregion

        #region Aftersales Issue Handling
        public ActionResult AftersalesPmIssueVerificationForm()
        {
            var fileManager = new FileManager();
            var vmModel = new VmAftersalesPmIssueVerification();
            vmModel.AftersalesPmIssueVerificationModels = _repository.GetAftersalesIssuesForVerification();
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
            return View(vmModel);
        }
        [Authorize(Roles = "QCHEAD,QC,SA,ACCNT,PM,PMHEAD,ASPM,ASPMHEAD")]
        public ActionResult DetailsOfAftersalesPmIssue(string issueIds)
        {
            long issueIdss;
            long.TryParse(issueIds, out issueIdss);


            var vmModel = new VmAftersalesPmIssueVerification();
            vmModel.AftersalesPm_ValidationReportModels = _repository.GetLogHistory(issueIdss);
            ViewBag.ValidationAndRootCauseAnalysis = _repository.ValidationAndRootCauseAnalysisReport(issueIdss);

            vmModel.AftersalesPm_SupplierFeedBackModels = _repository.GetSupplierFeedBackHistory(issueIdss);
          //  ViewBag.GetSupplierFeedBackHistory = vmModel.AftersalesPm_SupplierFeedBackModels;
          var fileManager = new FileManager();
            foreach (var model in vmModel.AftersalesPm_SupplierFeedBackModels)
            {
                if (model.Attachment != null)
                {
                    var urls = model.Attachment.Split('|').ToList();
                    for (int i = 0; i < urls.Count; i++)
                    {
                        FilesDetail detail = new FilesDetail();
                        detail.FilePath = fileManager.GetFile(urls[i]);
                        detail.Extention = fileManager.GetExtension(urls[i]);
                        model.FilesDetails.Add(detail);
                    }
                }
            }
           
            return View(vmModel);
        }
        [HttpPost]
        public JsonResult UpdateIssueVerificationStatus(String issueIds )
        {
            long ids;
            long.TryParse(issueIds, out ids);

            var saveIncentive = "0";

            if (ids != 0)
            {
                saveIncentive = _repository.UpdateIssueVerificationStatus(ids);
            }
            return Json(new { data = saveIncentive }, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult ForwardFirstVersionIssueToSecondVersion(string obj)
        //{
        //    List<SwQcIssueDetailModel> results = JsonConvert.DeserializeObject<List<SwQcIssueDetailModel>>(obj);
          

        [HttpPost]
        public JsonResult SaveDataIntoValidationReportTable(String IssueVerificationId, String ModelName, String SoftwareVersionName, String SoftwareVersionNo, String IssueDetails,
            String IssueOrRequirement, String NumberOfMpHsCheck, String FoundInGoldenHs, String FoundInMpHs, String ValidationResult, String Remarks)
        {
            var _dbEntities = new CellPhoneProjectEntities();
            var manager = new FileManager();

            var saveIncentive = "";
            long Ids = 0;
            long.TryParse(IssueVerificationId, out Ids);

            int SoftVersionNo = 0;
            int.TryParse(SoftwareVersionNo, out SoftVersionNo);

            int NoOfMpHsCheck = 0;
            int.TryParse(NumberOfMpHsCheck, out NoOfMpHsCheck);

            var Attachment = "";
            var supportingDocument = "";

            var query = _dbEntities.AftersalesPm_IssueVerification.FirstOrDefault(i => i.Id == Ids);

            long proIds = Convert.ToInt64(query.ProjectMasterId);

            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                if (file != null)
                {
                    //var res = manager.DocManagementUpload(ModelName, "VerificationIssueByQc", file);
                    var res = manager.Upload(proIds, ModelName, "VerificationIssueByQc", file);
                    Console.Write("res  :" + res);
                    supportingDocument = supportingDocument == null ? res : supportingDocument + "|" + res;
                    Attachment = supportingDocument;
                }
            }

            if (Ids > 0)
            {
                saveIncentive = _repository.SaveDataIntoValidationReportTable(Ids, ModelName, SoftwareVersionName, SoftVersionNo, IssueDetails, IssueOrRequirement, NoOfMpHsCheck, FoundInGoldenHs, FoundInMpHs,
                    ValidationResult, Remarks,Attachment);
            }

            return new JsonResult { Data = saveIncentive, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        [HttpPost]
        public JsonResult SaveQcFeedback(String IssueVerificationId, String Remarks, String QcStatus)
        {
            var _dbEntities = new CellPhoneProjectEntities();
            var manager = new FileManager();

            var saveIncentive = "";
            long Ids = 0;
            long.TryParse(IssueVerificationId, out Ids);
           
            if (Ids > 0)
            {
                saveIncentive = _repository.SaveQcFeedback(Ids, Remarks, QcStatus);
            }

            return new JsonResult { Data = saveIncentive, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion
    }
}