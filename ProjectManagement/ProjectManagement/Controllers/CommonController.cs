using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using ProjectManagement.DAL;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Infrastructures.Repositories;
using ProjectManagement.Models;
using ProjectManagement.Models.Common;
using ProjectManagement.Models.StausObjects;
using ProjectManagement.ViewModels.Commercial;
using ProjectManagement.ViewModels.Common;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Office.Interop.Excel;
using ProjectManagement.ViewModels.ProjectManager;
using DataTable = System.Data.DataTable;
using ZipFile = Ionic.Zip.ZipFile;


namespace ProjectManagement.Controllers
{
    [Authorize(
        Roles = "CM, CMHEAD, CMBTRC, SA, PM, PMHEAD, HWHEAD, HW, QC, QCHEAD, CMBTRC, MM, BRND, SPRHEAD, SPR, MKTHEAD, MKT, PS, PRD, WAR, WARHEAD,CPSD,CPSDHEAD,ASPM,ASPMHEAD,PRC,PRCHEAD,BDIQC,BDIQCHEAD,INV,INVHEAD,ACCNT,ACCNTHEAD,FIN,FINHEAD,CEO,AUD,AUDHEAD,SALES,SALESHEAD,COO,CBO,BIHEAD")]
    public class CommonController : Controller
    {
        // GET: Common
        private readonly ICommonRepository _commonRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICommercialRepository _commercialRepository;
        private readonly IGeneralIncidentRepository _generalIncidentRepository;
        private readonly IHardwareRepository _hardwareRepository;
        private readonly IMarketingRepository _marketingRepository;
        private readonly IIqcRepository _iqcRepository;
        private const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public CommonController(CommonRepository commonRepository, UserRepository userRepository,
            CommercialRepository commercialRepository, GeneralIncidentRepository generalIncidentRepository, HardwareRepository hardwareRepository, MarketingRepository marketingRepository,IqcRepository iqcRepository)
        {
            String useridentity = System.Web.HttpContext.Current.User.Identity.Name;
            var users = Convert.ToInt64(useridentity == "" ? "0" : useridentity);

            _commonRepository = commonRepository;
            _userRepository = userRepository;
            _commercialRepository = commercialRepository;
            _generalIncidentRepository = generalIncidentRepository;
            _hardwareRepository = hardwareRepository;
            _marketingRepository = marketingRepository;
            _iqcRepository = iqcRepository;
            ViewBag.ChinaQcInspectionCount = _commonRepository.GetChinaQcInspectionCount(users);
        }

        public JsonResult ProjectStatusForHw(long projectMasterId = 0)
        {
            var projectStatusHw = _commonRepository.GetProjectStatusForHw(projectMasterId);
            var json = new JavaScriptSerializer().Serialize(projectStatusHw);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProjectStatusForCm(long projectMasterId = 0)
        {
            return null;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Issues(int type = 0, string msg = null)
        {
            if (type > 0)
            {
                TempData["message"] = msg;
                TempData["messageType"] = type;
            }
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            IEnumerable<CommonIssueModel> models = _commonRepository.GetIssues();
            ViewBag.CreatedIssues = _commonRepository.GetIssuesCreatedByUserId(Convert.ToInt64(HttpContext.User.Identity.Name));
            return View(models);
        }

        [HttpGet]
        public ActionResult Create(long id = 0)
        {
            ViewBag.Projects = _commonRepository.GetAllProjects();
            ViewBag.UserList = _userRepository.GetAllUsers();
            List<CommonParamModel> componentList = _commonRepository.GetComponents();
            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(CommonIssueModel model)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            model.CreatorUserId = userId;
            if (ModelState.IsValid)
            {
                long returnId = _commonRepository.SaveIssue(model);
                if (returnId > 0) return RedirectToAction("Issues", new { type = 1, msg = "Issue Created Successfully" });
            }
            return RedirectToAction("Issues",
                new { type = 2, msg = "Your Form is Invalid to create a new Issue. Check every input field carefully" });
        }

        public ActionResult Forward(long id)
        {
            var model = new CommonIssueModel();
            model.CommonIssueId = id;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Forward(CommonIssueModel model)
        {
            if (model.CommonIssueId > 0 && !string.IsNullOrWhiteSpace(model.ReferenceRemarks) &&
                !string.IsNullOrWhiteSpace(model.CurrentlyWorkingRole))
            {
                bool result = _commonRepository.ReferedIssue(model);
                if (result) return RedirectToAction("Issues", new { type = 1, msg = "Issue Forwarded Successfully" });
            }
            return RedirectToAction("Issues", new { type = 2, msg = "Refer To and Remarks field is Required" });
        }


        [HttpGet]
        public ActionResult Solved(long id = 0)
        {
            var model = new CommonIssueModel { CommonIssueId = id };
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Solved(CommonIssueModel model)
        {

            if (model.CommonIssueId > 0)
            {
                long userId;
                long.TryParse(HttpContext.User.Identity.Name, out userId);
                model.SolverUserId = userId;
                model.Updated = userId;
                model.UpdatedDate = DateTime.Now;
                bool result = _commonRepository.SolveIssue(model);
                if (result) return RedirectToAction("Issues", new { type = 1, msg = "Issue Solved Successfully" });
            }
            return RedirectToAction("Issues",
                new { type = 2, msg = "Something goes wrong, Please contact with administrator" });
        }

        [HttpGet]
        public ActionResult Deny(long id)
        {
            var model = new CommonIssueModel();
            model.CommonIssueId = id;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Deny(CommonIssueModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.IgnoreComment) && model.CommonIssueId > 0)
            {

                bool result = _commonRepository.IgnoreIssue(model);
            }
            return RedirectToAction("Issues");
        }

        public ActionResult HardwareIssues()
        {
            var model = new VmHardwareIssueModel();
            ViewBag.Projects = _commercialRepository.GetAllProjects();
            model.HardwareIssueCustomModels = _commonRepository.GetHardwareIssueModels();
            return View(model);
        }

        [HttpGet]
        public ActionResult CommercialComment(long id, string comment)
        {
            return RedirectToAction("HardwareIssues");
        }

        [HttpGet]
        public ActionResult SupplierRating(long supplierId = 0, long projectMasterId = 0)
        {
            var model = new SupplierRatingModel();
            List<SupplierModel> supplierModels = _commercialRepository.GeTAllSuppliers();
            ViewBag.Suppliers = supplierModels;
            ViewBag.Projects = new List<ProjectMasterModel>();
            if (supplierId > 0 && projectMasterId > 0)
            {
                List<ProjectMasterModel> models = _commercialRepository.GetProjectBySupplierId(supplierId);
                ViewBag.Projects = models;
                //model = _commonRepository.GetSupplierRating(supplierId, projectMasterId);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult SupplierRating(SupplierRatingModel model)
        {
            bool result = _commonRepository.SaveSupplierRating(model);
            if (result)
            {
                TempData["message"] = "Supplier overall rating has been successfully saved";
                TempData["messageType"] = 1;
                return RedirectToAction("SupplierRating",
                    new { supplierId = model.SupplierId, projectMasterId = model.ProjectMasterId });
            }
            TempData["message"] = "Error Occured, Please Check your form carefully or Contact with Adminstrator";
            TempData["messageType"] = 2;
            List<SupplierModel> supplierModels = _commercialRepository.GeTAllSuppliers();
            ViewBag.Suppliers = supplierModels;
            if (model.SupplierId != null)
            {
                long supId = (long)model.SupplierId;
                List<ProjectMasterModel> models = _commercialRepository.GetProjectBySupplierId(supId);
                ViewBag.Projects = models;
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult GetSupplierProject(long supplierId)
        {
            List<ProjectMasterModel> models = _commercialRepository.GetProjectBySupplierId(supplierId);
            return new JsonResult { Data = models, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public JsonResult GetProjects()
        {
            List<ProjectMasterModel> models = _commercialRepository.GetProjectBySupplierId();
            return new JsonResult { Data = models, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public JsonResult GetAllProjects()
        {
            List<ProjectMasterModel> models = _commercialRepository.GetAllProjects();
            return new JsonResult { Data = models, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public JsonResult GetProjectInfoByProjectId(long projectId = 0)
        {
            ProjectMasterModel model = _commonRepository.GetProjectInfoByProjectId(projectId);
            MarketPriceModel marketPriceModel = _commonRepository.GetMarketPriceModelByProjectId(projectId);
            //if (marketPriceModel != null)
            //{
            //    if (model.FinalPrice == marketPriceModel.FinalPrice)
            //    {
            //        return new JsonResult { Data = marketPriceModel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //    }   
            //}
            return new JsonResult { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }




        [HttpPost]
        public string CreateOpinion(long partialPostProjectId, string OpinionText)
        {
            bool result = _commonRepository.SaveOpinion(partialPostProjectId, OpinionText);
            return result ? "y" : "n";
        }

        [HttpPost]
        public JsonResult GetRecentComments(long projectId)
        {
            List<OpinionModel> opinionModels = _commonRepository.GetOpinionsByProjectId(projectId);
            var json = JsonConvert.SerializeObject(opinionModels);
            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        [HttpGet]
        public JsonResult GetLatestHundredComment()
        {
            List<OpinionModel> models = _commonRepository.GetOpinionsByProjectId(0);
            models = models.OrderBy(i => i.OpinionId).ToList();
            var json = JsonConvert.SerializeObject(models);
            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult UserOpinion()
        {
            ViewBag.ProjectNames = _commonRepository.GetAllProjectNames();
            return View();
        }

        public ActionResult Status()
        {
            List<ProjectMasterModel> masters = _commercialRepository.GetAllProjectsForStatus();
            ViewBag.Projects = masters;
            return View();
        }
        public JsonResult GetData(long id)
        {
            ProjectDetailStatus statuses = _commonRepository.GetProjectStatus(id);
            string jsonStr = JsonConvert.SerializeObject(statuses);
            return new JsonResult { Data = jsonStr, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult StatusByModule()
        {
            List<ProjectMasterModel> masters = _commonRepository.GetAllProjects();
            ViewBag.Projects = masters;
            return View();
        }

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

        public ActionResult PostProductionIssue()
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            var vmPostProductionIssue = new VmPostProductionIssue();
            var projectMasterModels = new List<ProjectMasterModel>();
            if (HttpContext.User.IsInRole("QC") || HttpContext.User.IsInRole("QCHEAD"))
            {
                projectMasterModels = _commonRepository.GetPostProductionProjects(userId);
            }
            else if (HttpContext.User.IsInRole("MKT"))
            {
                projectMasterModels = _commonRepository.GetPostProductionProjects();
            }
            var listItems = new List<SelectListItem>
            {
                new SelectListItem {Text = "--Select--"}
            };
            foreach (var masterModel in projectMasterModels)
            {
                if (listItems.FindIndex(i => i.Value == masterModel.ProjectName) < 0)
                {
                    var item = new SelectListItem { Value = masterModel.ProjectName, Text = masterModel.ProjectName };
                    listItems.Add(item);
                }

            }
            //listItems = listItems.Distinct().ToList();
            ViewBag.Projects = listItems;
            vmPostProductionIssue.PostProductionIssueModels = _commonRepository.GetPostProductionIssuesByUser(userId);
            return View(vmPostProductionIssue);
        }

        [HttpPost]
        public ActionResult PostProductionIssue(VmPostProductionIssue model)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            var projectMasterModels = new List<ProjectMasterModel>();
            if (HttpContext.User.IsInRole("QC") || HttpContext.User.IsInRole("QCHEAD"))
            {
                projectMasterModels = _commonRepository.GetPostProductionProjects(userId);
            }
            else if (HttpContext.User.IsInRole("MKT"))
            {
                projectMasterModels = _commonRepository.GetPostProductionProjects();
            }
            var listItems = new List<SelectListItem>
            {
                new SelectListItem {Text = "--Select--"}
            };
            foreach (var masterModel in projectMasterModels)
            {
                if (listItems.FindIndex(i => i.Value == masterModel.ProjectName) < 0)
                {
                    var item = new SelectListItem { Value = masterModel.ProjectName, Text = masterModel.ProjectName };
                    listItems.Add(item);
                }

            }
            //listItems = listItems.Distinct().ToList();
            ViewBag.Projects = listItems;
            if (ModelState.IsValid)
            {
                bool save = _commonRepository.SavePostProductionIssue(model.PostProductionIssueModel);
                if (save)
                {
                    TempData["message"] = "Issue saved successfully..";
                    TempData["messageType"] = "1";
                    var postProductionIssueModel = new PostProductionIssueModel();
                    return RedirectToAction("PostProductionIssue");
                }
                TempData["message"] = "Error Occured....";
                TempData["messageType"] = "2";
            }

            return View(model);
        }

        public JsonResult GetPurchaseOrder(string projectName)
        {
            var poList = _commonRepository.GetProjectsByName(projectName);
            var listItems = new List<SelectListItem>();
            if (poList.Any())
            {
                listItems.AddRange(poList.Select(po => new SelectListItem { Value = po.OrderNuber.ToString(), Text = CommonConversion.AddOrdinal(po.OrderNuber) }));
            }
            var json = JsonConvert.SerializeObject(listItems);
            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetOrderNumbersByProjectName(string projectname)
        {
            var orders = _commonRepository.GetOrderNumbersByProjectName(projectname);
            var items = (from t in orders
                         let ordinal = CommonConversion.AddOrdinal(t.OrderNuber) + " Order"
                         select new SelectListItem
                         {
                             Text = ordinal,
                             Value = t.ProjectMasterId.ToString(CultureInfo.InvariantCulture)
                         }).ToList();

            string jsonStr = JsonConvert.SerializeObject(items);
            return new JsonResult { Data = jsonStr, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult PostProductionIssueNewsFeed(long swQcAllProjectIssueId = 0)
        {
            return View();
        }

        public ActionResult PostProductionIssueList()
        {
            if (HttpContext.User.IsInRole("MM"))
            {
                ViewBag.RoleCheck = "MM";
            }
            return View();
        }

        public JsonResult GetPostProductionIssueList(long swqcallprojectissueid = 0)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            var postProductionIssueModels = _commonRepository.GetPostProductionIssues(userId, swqcallprojectissueid);
            string jsonStr = JsonConvert.SerializeObject(postProductionIssueModels);
            return new JsonResult { Data = jsonStr, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult PostIssueComment(string message, long swqcallprojectissueid = 0)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            var postCommentModel = new PostCommentModel
            {
                SwQcAllProjectIssueId = swqcallprojectissueid,
                Message = message,
                CommentedBy = userId,
                CommentedDate = DateTime.Now
            };
            var latestcomment = _commonRepository.SaveIssuePostComment(postCommentModel);
            string jsonStr = JsonConvert.SerializeObject(latestcomment);
            return new JsonResult { Data = jsonStr, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetIssueCommentById(long swqcallprojectissueid = 0)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            var ppissuecomment = _commonRepository.GetPostCommentById(swqcallprojectissueid);
            string jsonStr = JsonConvert.SerializeObject(ppissuecomment);
            return new JsonResult { Data = jsonStr, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult ApporveComment(long swqcallprojectissueid, long postcommentid, long approve)
        {
            _commonRepository.UpdateCommentForApproval(swqcallprojectissueid, postcommentid, approve);
            return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult PhoneComparison()
        {
            var projectSelectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "--Select One--" } };
            var projects = _commonRepository.GetAllProjects();
            projectSelectListItems.AddRange(projects.Select(project => new SelectListItem { Value = project.ProjectMasterId.ToString(CultureInfo.InvariantCulture), Text = project.ProjectName }));
            ViewBag.Projects = projectSelectListItems;
            return View();
        }

        public ActionResult IncidentCreation()
        {
            var modelSelectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "Select model" } };
            var modelNames = _commonRepository.GetModelNamesHavingIssues();
            modelSelectListItems.AddRange(modelNames.Select(modelName => new SelectListItem { Value = modelName.ModelName.ToString(CultureInfo.InvariantCulture), Text = modelName.ModelName }));
            ViewBag.ModelNamesHavingIssues = modelSelectListItems;
            ViewBag.Diagnostic = _commonRepository.GetDiagnosticCodeFromOracleModels();
            return View();
        }

        public ActionResult SpecComparison()
        {
            var projectSelectListItems = new List<SelectListItem> { new SelectListItem { Value = "", Text = "--Select One--" } };
            var projects = _commonRepository.GetAllProjects();
            projectSelectListItems.AddRange(projects.Select(project => new SelectListItem { Value = project.ProjectMasterId.ToString(CultureInfo.InvariantCulture), Text = project.ProjectName }));
            ViewBag.Projects = projectSelectListItems;
            ViewBag.ForeignProjects = _marketingRepository.GetMkProjectSpecModels();
            return View();
        }

        public JsonResult Compare(long projectId, long otherProjectId)
        {
            var project = _commonRepository.GetProjectInfoByProjectId(projectId);
            var otherProject = _marketingRepository.GetMkProjectSpecModelById(otherProjectId);
            var json = new { project, otherProject };
            return Json(json);
        }

        public JsonResult GetSpec(string specname, string type)
        {
            var data = _commonRepository.GetSpec(specname, type);
            return Json(data);
        }

        #region SAMPLE TRACKING

        public ActionResult SampleTracking()
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            //Department
            var roles = _generalIncidentRepository.GetAllRoleModels();
            List<SelectListItem> roleDescriptions = roles.Select(role => new SelectListItem { Text = role.RoleDescription, Value = role.RoleName }).ToList();
            ViewBag.Roles = roleDescriptions;
            //Employees
            var users = _commonRepository.GetAllEmployee();
            List<SelectListItem> userList = users.Select(user => new SelectListItem { Text = user.UserFullName, Value = Convert.ToString(user.CmnUserId) }).ToList();
            ViewBag.Users = userList;
            //Projects
            var models = _commonRepository.GetOnlyModelName();
            List<SelectListItem> modelList = models.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectName }).ToList();
            ViewBag.Model = modelList;
            //Sample sent by me
            ViewBag.sample = _commonRepository.GetSampleTrackingByAddedId(userId);
            //sample sent to me
            ViewBag.SampleToMe = _commonRepository.GetSampleTrackingByEmployeeId(userId);
            //sample to my dept if you are a HEAD
            ViewBag.SampleToDept = _commonRepository.GetSampleTrackingByRole(ViewBag.UserInfo.RoleName);
            return View();
        }

        [HttpPost]
        public ActionResult SampleTracking(SampleTrackerModel sample)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            if (User.IsInRole("ASPM") || User.IsInRole("ASPMHEAD") || User.IsInRole("CPSD") || User.IsInRole("CPSDHEAD") || User.IsInRole("PM") || User.IsInRole("PMHEAD"))
            {
                sample.AddedBy = userId;
                sample.AddedDate = DateTime.Now;
                sample.ReturnQuantity = 0;
                _commonRepository.SaveSampleTracker(sample);
                MailSendFromPms mailSendFromPms = new MailSendFromPms();
                if (sample.SampleSentToDept != null && sample.SampleSentToPersonId == null)
                {
                    mailSendFromPms.SendMail(new List<string>(new[] { sample.SampleSentToDept }),
                new List<string>(new[] { "" }), "Sample Sent", "This is to inform you that " + sample.NumberOfSample + " samples sent by " + ViewBag.UserInfo.UserFullName
                + ". <br/> Project:" + sample.Model + "<br/> Additional info:" + sample.AdditionalInfo);
                }
                if (sample.SampleSentToPersonId != null)
                {
                    mailSendFromPms.SendMail(new List<long>(new[] { Convert.ToInt64(sample.SampleSentToPersonId) }),
                new List<string>(new[] { "" }), "Sample Sent", "This is to inform you that " + sample.NumberOfSample + " samples sent by " + ViewBag.UserInfo.UserFullName
                + ". <br/> Project:" + sample.Model + "<br/> Additional info:" + sample.AdditionalInfo);
                }
            }
            return RedirectToAction("SampleTracking");
        }

        public ActionResult SampleTrackerDetails(long id)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            //Department
            var roles = _generalIncidentRepository.GetAllRoleModels();
            List<SelectListItem> roleDescriptions = roles.Select(role => new SelectListItem { Text = role.RoleDescription, Value = role.RoleName }).ToList();
            ViewBag.Roles = roleDescriptions;
            //Employees
            var users = _commonRepository.GetAllEmployee();
            List<SelectListItem> userList = users.Select(user => new SelectListItem { Text = user.UserFullName, Value = Convert.ToString(user.CmnUserId) }).ToList();
            ViewBag.Users = userList;
            //Projects
            var models = _commonRepository.GetOnlyModelName();
            List<SelectListItem> modelList = models.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectName }).ToList();
            ViewBag.Model = modelList;
            //get sampel by id
            var sample = _commonRepository.GetSampleTrackerById(id);
            return View(sample);
        }

        [HttpPost]
        public ActionResult SampleTrackerDetails(SampleTrackerModel sample)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            sample.SampleSentToDept = sample.Role;
            sample.UpdatedBy = userId;
            sample.UpdatedDate = DateTime.Now;
            _commonRepository.UpdateSampleTracker(sample);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            if (sample.SampleSentToDept != null && sample.SampleSentToPersonId == null)
            {
                mailSendFromPms.SendMail(new List<string>(new[] { sample.SampleSentToDept }),
            new List<string>(new[] { "" }), "Sample Tracker Data Updated", "This is to inform you that sample tracker data updated by " + ViewBag.UserInfo.UserFullName
            + ". <br/> Project:" + sample.Model + "<br/> Additional info:" + sample.AdditionalInfo);
            }
            if (sample.SampleSentToPersonId != null)
            {
                mailSendFromPms.SendMail(new List<long>(new[] { Convert.ToInt64(sample.SampleSentToPersonId) }),
            new List<string>(new[] { "" }), "Sample Tracker Data Updated", "This is to inform you that sample tracker data updated by " + ViewBag.UserInfo.UserFullName
            + ". <br/> Project:" + sample.Model + "<br/> Tracker ID:" + sample.SampleTrackerId);
            }
            return RedirectToAction("SampleTrackerDetails", new { id = sample.SampleTrackerId });
        }

        public JsonResult SampleReceive(long id)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var model = _commonRepository.GetSampleTrackerById(id);
            model.ReceivedBy = userId;
            model.ReceiveDate = DateTime.Now;
            model.SampleSentToDept = model.Role;
            var sample = _commonRepository.UpdateSampleTracker(model);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<long>(new[] { Convert.ToInt64(sample.AddedBy) }),
        new List<string>(new[] { "" }), "Sample Received", "This is to inform you that , samples received by " + sample.ReceivedByName
        + ". <br/> Project:" + sample.Model + "<br/> Tracker ID:" + sample.SampleTrackerId);
            return Json(sample);
        }

        public JsonResult SampleReturn(long id, string remarks, int returnquantity)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var model = _commonRepository.GetSampleTrackerById(id);
            model.ReturnedBy = userId;
            model.ReturnDate = DateTime.Now;
            var prevReturnQuantity = model.ReturnQuantity ?? 0;
            model.ReturnQuantity = returnquantity + prevReturnQuantity;
            model.SampleSentToDept = model.Role;
            var sample = _commonRepository.UpdateSampleTracker(model);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<long>(new[] { Convert.ToInt64(sample.AddedBy) }),
        new List<string>(new[] { "" }), "Sample Returned", "This is to inform you that ," + returnquantity + " samples returned by " + sample.ReturnedByName
        + ". <br/> Project:" + sample.Model + "<br/> Tracker ID:" + sample.SampleTrackerId);
            var log = new SampleReturnLogModel
            {
                SampleTrackerId = id,
                ReturnQuantity = returnquantity,
                Remarks = remarks,
                AddedBy = userId,
                AddedDate = DateTime.Now
            };
            _commonRepository.SaveSampleReturnLog(log);
            return Json(sample);
        }

        public ActionResult SampleIssue()
        {
            //Projects
            var models = _commonRepository.GetOnlyModelName();
            long userId = Convert.ToInt64(User.Identity.Name);
            List<SelectListItem> modelList = models.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectName }).ToList();
            ViewBag.Model = modelList;
            ViewBag.MyIssues = _commonRepository.GetSampleIssueListByIssuerId(userId);
            //Employees
            var users = _commonRepository.GetAllEmployee();
            List<SelectListItem> userList = users.Select(user => new SelectListItem { Text = user.UserFullName, Value = Convert.ToString(user.CmnUserId) }).ToList();
            ViewBag.Users = userList;
            return View();
        }

        [HttpPost]
        public ActionResult SampleIssue(SampleTrackerModel sample)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var userInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            sample.SampleIssueDate = DateTime.Now;
            sample.SampleIssuedBy = userId;
            _commonRepository.SaveSampleTracker(sample);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<string>(new[] { "PMHEAD","PM" }),
        new List<string>(new[] { "" }), "Sample Requisition Request", "This is to inform you that ,"+sample.SampleIssueQuantity+" samples requisition issued by " + userInfo.UserFullName
        + ". <br/> Project:" + sample.Model + "<br/> Tracker ID:" + sample.SampleTrackerId);
            return RedirectToAction("SampleIssue");
        }

        public ActionResult AssignIssuedSamples()
        {
            var samples = _commonRepository.GetAllSampleTrackers();
            return View(samples);
        }

        public JsonResult SaveAssignSampleIssue(string remarks, int assignQuantity = 0, long id = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var userInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            var sample = _commonRepository.GetSampleTrackerById(id);
            sample.NumberOfSample = assignQuantity;
            sample.Remarks = remarks;
            sample.AddedBy = userId;
            sample.AddedDate = DateTime.Now;//==use as assign date==
            sample.SampleSentToPersonId = sample.SampleIssuedBy;
            var model = _commonRepository.UpdateSampleTracker(sample);
            var receiverInfo = _hardwareRepository.GetUserInfoByUserId(Convert.ToInt64(sample.SampleSentToPersonId));
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<long>(new[] { Convert.ToInt64(sample.SampleSentToPersonId) }),
        new List<string>(new[] { "SA" }), "Sample Sent", "This is to inform you that, "+assignQuantity+" samples of "+sample.Model+" sent by " + userInfo.UserFullName
        + " to " + receiverInfo.UserFullName + "." + "<br/> Tracker ID:" + sample.SampleTrackerId);
            return Json(model);
        }

        public JsonResult ReturnSampleToInventory(string remarks, long id = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var userInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            var sample = _commonRepository.GetSampleTrackerById(id);
            sample.InventoryReturnRemarks = remarks;
            sample.InventoryReturnDate = DateTime.Now;
            sample.InventoryReturnedBy = userId;
            var model = _commonRepository.UpdateSampleTracker(sample);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<long>(new[] { Convert.ToInt64(sample.SampleSentToPersonId) }),
        new List<string>(new[] { "SA" }), "Sample Returned to Inventory", "This is to inform you that, samples of " + sample.Model + " returned to inventory by " + userInfo.UserFullName + "." + "<br/> Tracker ID:" + sample.SampleTrackerId);
            return Json(model);
        }
        #endregion

        #region Project Close Penalty

        public ActionResult ProjectClosePenalty()
        {
            ViewBag.RunningPenalty = _commonRepository.GetRunningPenaltyModels();
            ViewBag.ClosedPenalty = _commonRepository.GetClosedPenaltyModels();
            return View();
        }
        #endregion


        #region PR_PO

        public ActionResult PrPoList()
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            List<PrPoViewModel> prPoViewModels = _commonRepository.GetPrPoData(userId);
            return View(prPoViewModels);
        }
        #endregion

        #region GANTT

        public ActionResult ProjectProgressGanttChart()
        {
            ViewBag.ProjectNames = _commonRepository.GetAllProjectNames();
            return View();
        }

        public JsonResult GetDataForGantt(long id)
        {
            return Json(true);
        }
        #endregion

        #region Independent HW test Asign
        public JsonResult SaveHwTest(string hwTestName)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var hwtest = new HwTestMasterModel
            {
                HwTestName = hwTestName,
                AddedBy = userId,
                AddedDate = DateTime.Now
            };
            var json = _commonRepository.SaveHwTestMaster(hwtest);
            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion

        #region Hashtag and comment

        public ActionResult Discussion(string str, string tag)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            var model = _commonRepository.GetDiscussions();
            ViewBag.Files = _commonRepository.GetDiscussionFileUploadModels(model);
            ViewBag.Replies = _commonRepository.GetDiscussionReplies(model);
            if (str != null)
            {
                model = _commonRepository.GetDiscussionByMention(str);
                ViewBag.Files = _commonRepository.GetDiscussionFileUploadModels(model);
                ViewBag.Replies = _commonRepository.GetDiscussionReplies(model);
            }
            if (tag != null)
            {
                model = _commonRepository.GetDiscussionByHashTag(tag);
                ViewBag.Files = _commonRepository.GetDiscussionFileUploadModels(model);
                ViewBag.Replies = _commonRepository.GetDiscussionReplies(model);
            }
            return View(model);
        }

        public ActionResult DiscussionPartialView()
        {
            var model = _commonRepository.GetDiscussions();
            ViewBag.Files = _commonRepository.GetDiscussionFileUploadModels(model);
            return PartialView("_DiscussionPartialView", model);
        }

        public JsonResult GetUserNameForMention()
        {
            var json = _commonRepository.GetAllEmployee();
            return Json(json);
        }

        public JsonResult Upload(int id)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            var manager = new FileManager();
            var moduleDirectory = "Discussion";
            var userDirectory = "CMN";
            for (int i = 0; i < Request.Files.Count; i++)
            {
                //HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                var model = new DiscussionFileUploadModel
                {
                    DiscussionId = id,
                    AddedBy = userId,
                    AddedDate = DateTime.Now,
                    FileUploadPath = manager.IncidentUpload(userDirectory, moduleDirectory, Request.Files[i])
                };
                _commonRepository.UploadDiscussionFile(model);
            }
            return Json(true);
        }

        public JsonResult SaveDiscussion(string comment)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            var users = _commonRepository.GetAllEmployee();
            string[] commentArray = comment.Split(' ');
            foreach (var s in commentArray)
            {
                if (s.Contains("#"))
                {
                    var hash = new HashtagModel
                    {
                        HashtagName = s,
                        AddedBy = userId,
                        AddedDate = DateTime.Now
                    };
                    _commonRepository.SaveHashtag(hash);
                    var hashToAscii = s.Replace("#", "%23");
                    comment = comment.Replace(s, "<a href=" + "\"" + "Discussion?tag=" + hashToAscii + "\"" + ">" + s + "</a>");
                }
            }
            foreach (var u in users)
            {
                if (comment.Contains(u.UserFullName))
                {
                    comment = comment.Replace("@" + u.UserFullName, "<a href=" + "\"" + "Discussion?str=" + u.UserFullName + "\"" + ">@" + u.UserFullName + "</a>");
                }
            }
            var model = new DiscussionModel
            {
                Comment = comment,
                AddedBy = userId,
                AddedDate = DateTime.Now
            };
            model = _commonRepository.SaveDiscussion(model);
            return Json(model);
        }

        public JsonResult GetHashtags(string str)
        {
            var json = _commonRepository.GetHashtagByString(str);
            return Json(json);
        }

        public JsonResult GetHashtagsToArr(string str)
        {
            var json = _commonRepository.GetHashtagByStringToArr(str);
            return Json(json);
        }

        public JsonResult NewCommentCheck()
        {
            var json = _commonRepository.CommentCount();
            return Json(json);
        }

        public JsonResult TopTrendingTagCheck()
        {
            var json = _commonRepository.GetTopHashtag();
            return Json(json);
        }

        public ActionResult DownloadDiscussionFile(long id = 0)
        {
            var model = _commonRepository.GetFileUploadModelById(id);
            var manager = new FileManager();
            var path = manager.GetDiscussionFile(model.FileUploadPath);
            var apppath = HttpContext.Server.MapPath(path);//HttpRuntime.AppDomainAppPath;
            string fileName = Path.GetFileName(path);
            var extension = Path.GetExtension(fileName);
            if (!string.IsNullOrEmpty(extension))
            {
                string ext = extension.Remove(0, 1);
                return File(apppath, "application/" + ext, fileName);
            }

            return new EmptyResult();
        }

        public JsonResult LoadMoreDiscussion(long id)
        {
            var loads = _commonRepository.LoadMoreDiscussions(id);
            var files = _commonRepository.GetDiscussionFileUploadModels(loads);
            var replies = _commonRepository.GetDiscussionReplyByModels(loads);
            var json = new ArrayList { loads, files, replies };
            return Json(json);
        }

        public JsonResult PostDiscussionReply(long id, string reply)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            var model = new DiscussionReplyModel
            {
                DiscussionId = id,
                Reply = reply,
                AddedBy = userId,
                AddedDate = DateTime.Now
            };
            var m = _commonRepository.SaveDiscussionReply(model);
            return Json(m);
        }
        #endregion

        #region Doc Management

        public ActionResult DocManagement()
        {
            ViewBag.ProjectNames = _commonRepository.GetAllProjectNames();
            return View();
        }

        public JsonResult GetFolderByProjectName(string projectname, long parentfolder)
        {
            var getFolders = _commonRepository.GetFolderModelsByProjectAndParent(projectname, parentfolder);
            var getfiles = _commonRepository.GetFileUploadModels(projectname, parentfolder);
            foreach (var v in getfiles)
            {
                v.DocFilePath = Path.GetFileName(v.DocFilePath);
            }
            return Json(new { Files = getfiles, Folders = getFolders }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddFolder(string foldername, string projectname, long parentfolder)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            var model = new FolderModel
            {
                ProjectName = projectname,
                FolderName = foldername,
                Parent = parentfolder,
                AddedBy = userId,
                AddedDate = DateTime.Now,
                UpdatedBy = userId,
                UpdatedDate = DateTime.Now
            };
            var data = _commonRepository.SaveFolderModel(model);
            return Json(data);
        }

        public JsonResult BrowseBack(string projectname, long folderid)
        {
            var getFolders = _commonRepository.BrowseBack(projectname, folderid);
            var getfiles = _commonRepository.BrowseBackFiles(projectname, folderid);
            foreach (var v in getfiles)
            {
                v.DocFilePath = Path.GetFileName(v.DocFilePath);
            }
            return Json(new { Files = getfiles, Folders = getFolders }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UploadFilesInFolder(string projectname, long folderid)
        {
            var model = new List<DocManagementFileUploadModel>();
            long userId = Convert.ToInt64(User.Identity.Name);
            var moduleDirectory = "Files";
            var userDirectory = "DocManager";
            var manager = new FileManager();
            HttpFileCollectionBase files = Request.Files;
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i];
                if (file != null)
                {
                    double filesize = file.ContentLength / 1000;
                    filesize = Math.Round(filesize);
                    var fileName = Path.GetFileName(file.FileName);
                    var duplicatename = _commonRepository.DuplicateFileCheck(projectname, fileName);
                    if (duplicatename == false)
                    {
                        var fileupload = new DocManagementFileUploadModel
                        {
                            FolderId = folderid,
                            ProjectName = projectname,
                            AddedBy = userId,
                            AddedDate = DateTime.Now,
                            Size = Convert.ToInt64(filesize),
                            DocFilePath = manager.DocManagementUpload(userDirectory, moduleDirectory, file)
                        };
                        var data = _commonRepository.SaveFileUploadModels(fileupload);
                        data.DocFilePath = Path.GetFileNameWithoutExtension(data.DocFilePath);
                        model.Add(data);
                    }
                }
            }
            return Json(model);
        }

        public ActionResult DownloadFile(long id = 0)
        {
            var model = _commonRepository.GetFileById(id);
            var manager = new FileManager();
            var path = manager.GetFile(model.DocFilePath);
            var apppath = HttpContext.Server.MapPath(path);//HttpRuntime.AppDomainAppPath;
            string fileName = Path.GetFileName(path);
            var extension = Path.GetExtension(fileName);
            if (extension != null)
            {
                string ext = extension.Remove(0, 1);
                return File(apppath, "application/" + ext, fileName);
            }

            return new EmptyResult();
        }
        #endregion

        public ActionResult UnProducedIMEI(string modelname, string order)
        {
            if (modelname == null)
                modelname = "";
            if (order == null)
                order = "";
            //var data = _commonRepository.GetProduced_UnProducedIMEIs(modelname, order);
            Produced_UnProducedIMEIViewModel data1 = _commonRepository.GetProductionInformation(modelname, order);
            var models = data1.Models;
            ViewBag.Projects = models.OrderBy(x=>x.ProjectModel);
            if (modelname != null)
                ViewBag.Orders = data1.Orders.Where(x => x.ProjectModel == modelname).ToList();
            //else
            //    ViewBag.Orders = data.Orders;
            return View(data1);
        }
        public JsonResult GetOrdersfromModel(string modelname)
        {
            var data = _commonRepository.GetOrdersfromModel(modelname);
            ViewBag.Orders = data;
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #region Service Trends


        public ActionResult ServiceTrend(ServiceTrendsViewModel vm)
        {
            var services = new ServiceTrendsViewModel();
            List<ProjectMasterModel> models = _commonRepository.GetOnlyModelName().ToList();
            services = _commonRepository.GetServiceLog(vm.ModelName);
            ViewBag.Models = models;
            return View(services);
        }

        public JsonResult GetMonthlyServiceEntry(string modelName)
        {
            List<PerMonthServiceEntryModel> data = _commonRepository.GetPerMonthServiceEntry(modelName);
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetMajorProblemChartData(string modelName)
        {
            MajorProblem data = _commonRepository.GetMajorProblemsChartData(modelName);
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion

        public ActionResult DailySalesInvoices(DailySalesInvoicesViewModel vm)
        {
            var invoices = new DailySalesInvoicesViewModel();
            var invdate = DateTime.Today.ToString("yyyy-MM-dd");
            //if(vm.InvoiceDate!=null)
            //  invdate = DateTime.Parse(vm.InvoiceDate.ToString()).ToString("yyyy-MM-dd");

            invoices = _commonRepository.DailySalesInvoices(invdate);
            return View(invoices);
        }
        public ActionResult ModelColorWiseDailySales(string id, string date)
        {
            var data = new List<ModelColorWiseDailySalesViewModel>();
            data = _commonRepository.GetColorWiseActivatedModelNumber(id, date);
            return View(data);
        }
        public ActionResult ModelWIseDailySalesByDealerType(string id, string date)
        {
            var data = new List<ModelWIseDailySalesByDealerTypeViewModel>();
            data = _commonRepository.GetModelWiseADealerType(id, date);
            return View(data);
        }
        public ActionResult RemainingMarketStockDealerWise(string modelname, string type, string date)
        {
            var data = new RemainingMarketStockDealerWiseViewModel();
            data = _commonRepository.GetRemainingStock(type, modelname, date);
            return View(data);
        }
        public ActionResult HighChartGraphforDailySales(string id, string date)
        {
            //var data = new HighChartGraphforDailySalesViewModel();
            //data = _commonRepository.GetHighChartGraphforDailySales(id, date);
            ViewBag.Id = id.ToString();
            ViewBag.InvDate = date.ToString();
            return View();
        }
        public JsonResult HighChartGraphforDailySalesData(string id, string date)
        {
            var data = new ChartGraphforDailySalesViewModel();
            data = _commonRepository.GetHighChartGraphforDailySales(id, date);
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public ActionResult DailySalesInvoiceNewMajorMinorIssues(NewMajorMinorIssuesViewModel vm)
        {
            var data = new NewMajorMinorIssuesViewModel();
            List<ProjectMasterModel> models = _commonRepository.GetOnlyModelName().ToList();
            List<Order> orders = _commonRepository.GetOrdersOfModel(vm.ModelName);
            data = _commonRepository.MajorMinorIssuesViewModel(vm.ModelName, vm.Order);
            ViewBag.Models = models;
            ViewBag.Orders = orders;
            return View(data);
        }

        public JsonResult MajorProblemData(string modelname, string order)
        {
            List<PieChartDataForIssueName> data = new List<PieChartDataForIssueName>();
            data = _commonRepository.GetMajorIssueChartsByMonthWiseServiceQuantity(modelname, order);
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult SpareUsedData(string modelname, string order)
        {
            List<PieChartDataForSpare> data = new List<PieChartDataForSpare>();
            data = _commonRepository.GetSpareChartsByMonthWiseServiceQuantity(modelname, order);
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult TotalReceiveData(string modelname, string order)
        {
            List<HighChartDataListForTotalReceive> data = new List<HighChartDataListForTotalReceive>();
            data = _commonRepository.GetTotalReceiveData(modelname, order);
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult OrderFromLauncingDate(string modelname, string order)
        {
            HighChartDataListByOrderFromLauncingDate data = new HighChartDataListByOrderFromLauncingDate();
            data = _commonRepository.GetOrderFromLauncingDate(modelname, order).FirstOrDefault();
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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
                GridView gv1 = new GridView();
                //gv1.HeaderStyle.BackColor = System.Drawing.Color.Blue;
                //gv1.HeaderStyle.ForeColor = System.Drawing.Color.White;
                gv1.HeaderStyle.Font.Bold = true;
                gv1.DataSource = dTable;
                gv1.DataBind();
                Response.Clear();
                Response.AddHeader("content-disposition", "attachment;filename=" + excelFileNames);
                Response.ContentType = "application/vnd.ms-excel";
                StringWriter sw1 = new StringWriter();
                HtmlTextWriter hw1 = new HtmlTextWriter(sw1);
                gv1.RenderControl(hw1);
                Response.Output.Write(sw1.ToString());
                Response.End();

                connection.Close();
            }

        }
        public void exportIMEI_Click(string model1, string orders1)
        {
            var model = model1;
            var orders = orders1;

            if (model.Contains("(") && model.Contains(")"))
            {
                var input = model;
                model = input.Replace('+', ' ');
            }
            orders = orders.Replace("+", " ");

            var excelFileNames = @"IMEI_" + model + "_" + DateTime.Now.ToString("yyyy-MM-dd hhmmss.mmm") + ".xls";
            String connectionString = ConfigurationManager.ConnectionStrings["WSMSConnectionString"].ConnectionString;


            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string getSmartPhoneDailySales1 = "";
                string getSmartPhoneDailySales2 = "";
                string getSmartPhoneDailySales3 = "";
                string getSmartPhoneDailySales4 = "";
                string getSmartPhoneDailySales5 = "";
                string getSmartPhoneDailySales6 = "";
                string getSmartPhoneDailySales7 = "";
                string getSmartPhoneDailySales8 = "";
                string getSmartPhoneDailySales9 = "";

                if (orders == "ALL")
                {

                    getSmartPhoneDailySales1 =
                        string.Format(
                            @"  select kk.TenDaysIMEI from
                            (Select distinct IME as TenDaysIMEI from WSMS.dbo.ServiceMaster sm 
                            join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                            where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                            from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                            and (sm.IME in (select BarCode from RBSYNERGY.dbo.tblBarCodeInv))
                            and ((sm.ServicePlaceDate>=RegistrationDate and sm.ServicePlaceDate<=DATEADD(DAY, 10, RegistrationDate)) 
                            or sm.ServiceType='StockFaulty')
                            and sm.IME is not null and sm.Model='{0}'  
                            and sm.Servicepointid not in (69)

                            union

                            Select distinct IME as TenDaysIMEI from WSMS.dbo.ServiceMaster sm 
                            join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                            where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                            from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                            and (sm.IME in (select BarCode2 from RBSYNERGY.dbo.tblBarCodeInv))
                            and ((sm.ServicePlaceDate>=RegistrationDate and sm.ServicePlaceDate<=DATEADD(DAY, 10, RegistrationDate)) 
                            or sm.ServiceType='StockFaulty')
                            and sm.IME is not null and sm.Model='{0}' 
                            and sm.Servicepointid not in (69))as kk", model);
                }
                else
                {
                    getSmartPhoneDailySales1 =
                        string.Format(
                            @"select kk.TenDaysIMEI from
                        (Select distinct IME as TenDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and (sm.IME in (select BarCode from RBSYNERGY.dbo.tblBarCodeInv where Updatedby='{1}') )
                        and ((sm.ServicePlaceDate>=RegistrationDate and sm.ServicePlaceDate<=DATEADD(DAY, 10, RegistrationDate)) 
                        or sm.ServiceType='StockFaulty')
                        and sm.IME is not null and sm.Model='{0}'  
                        and sm.Servicepointid not in (69)

                        union 

                        Select distinct IME as TenDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and ( sm.IME in (select BarCode2 from RBSYNERGY.dbo.tblBarCodeInv where Updatedby='{1}'))
                        and ((sm.ServicePlaceDate>=RegistrationDate and sm.ServicePlaceDate<=DATEADD(DAY, 10, RegistrationDate)) 
                        or sm.ServiceType='StockFaulty')
                        and sm.IME is not null and sm.Model='{0}'  
                        and sm.Servicepointid not in (69)) as kk",
                            model, orders);
                }
                if (orders == "ALL")
                {

                    getSmartPhoneDailySales2 =
                        string.Format(
                            @"select kk.TwentyDaysIMEI from
                        (Select distinct IME as TwentyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and (sm.IME in (select BarCode from RBSYNERGY.dbo.tblBarCodeInv)) 
                        and (sm.ServicePlaceDate>=DATEADD(DAY, 11, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 20, RegistrationDate)) 
                        and sm.IME is not null and sm.Model='{0}'  
                        and sm.Servicepointid not in (69)

                        union

                        Select distinct IME as TwentyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and (sm.IME in (select BarCode2 from RBSYNERGY.dbo.tblBarCodeInv))
                        and (sm.ServicePlaceDate>=DATEADD(DAY, 11, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 20, RegistrationDate))
                        and sm.IME is not null and sm.Model='{0}' 
                        and sm.Servicepointid not in (69))as kk", model);
                }
                else
                {
                    getSmartPhoneDailySales2 =
                        string.Format(
                            @"select kk.TwentyDaysIMEI from
                        (Select distinct IME as TwentyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and (sm.IME in (select BarCode from RBSYNERGY.dbo.tblBarCodeInv where Updatedby='{1}') )
                        and (sm.ServicePlaceDate>=DATEADD(DAY, 11, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 20, RegistrationDate))
                        and sm.IME is not null and sm.Model='{0}'  
                        and sm.Servicepointid not in (69)

                        union 

                        Select distinct IME as TwentyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and ( sm.IME in (select BarCode2 from RBSYNERGY.dbo.tblBarCodeInv where Updatedby='{1}'))
                        and (sm.ServicePlaceDate>=DATEADD(DAY, 11, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 20, RegistrationDate))
                        and sm.IME is not null and sm.Model='{0}'  
                        and sm.Servicepointid not in (69)) as kk",
                            model, orders);
                }

                if (orders == "ALL")
                {

                    getSmartPhoneDailySales3 =
                        string.Format(
                                                @"select kk.FourtyDaysIMEI from
                    (Select distinct IME as FourtyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                    join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                    where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                    from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                    and (sm.IME in (select BarCode from RBSYNERGY.dbo.tblBarCodeInv)) 
                    and (sm.ServicePlaceDate>=DATEADD(DAY, 21, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 40, RegistrationDate)) 
                    and sm.IME is not null and sm.Model='{0}'  
                    and sm.Servicepointid not in (69)

                    union

                    Select distinct IME as FourtyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                    join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                    where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                    from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                    and (sm.IME in (select BarCode2 from RBSYNERGY.dbo.tblBarCodeInv))
                    and (sm.ServicePlaceDate>=DATEADD(DAY, 21, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 40, RegistrationDate)) 
                    and sm.IME is not null and sm.Model='{0}' 
                    and sm.Servicepointid not in (69))as kk", model);
                }
                else
                {
                    getSmartPhoneDailySales3 =
                        string.Format(
                            @"select kk.FourtyDaysIMEI from
                        (Select distinct IME as FourtyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and (sm.IME in (select BarCode from RBSYNERGY.dbo.tblBarCodeInv where Updatedby='{1}') )
                        and (sm.ServicePlaceDate>=DATEADD(DAY, 21, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 40, RegistrationDate)) 
                        and sm.IME is not null and sm.Model='{0}'  
                        and sm.Servicepointid not in (69)

                        union 

                        Select distinct IME as FourtyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and ( sm.IME in (select BarCode2 from RBSYNERGY.dbo.tblBarCodeInv where Updatedby='{1}'))
                        and (sm.ServicePlaceDate>=DATEADD(DAY, 21, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 40, RegistrationDate)) 
                        and sm.IME is not null and sm.Model='{0}'  
                        and sm.Servicepointid not in (69)) as kk",
                            model, orders);
                }

                if (orders == "ALL")
                {

                    getSmartPhoneDailySales4 =
                        string.Format(
                         @"select kk.SixtyDaysIMEI from
                    (Select distinct IME as SixtyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                    join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                    where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                    from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                    and (sm.IME in (select BarCode from RBSYNERGY.dbo.tblBarCodeInv)) 
                     and (sm.ServicePlaceDate>=DATEADD(DAY, 41, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 60, RegistrationDate)) 
                    and sm.IME is not null and sm.Model='{0}'  
                    and sm.Servicepointid not in (69)

                    union

                    Select distinct IME as SixtyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                    join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                    where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                    from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                    and (sm.IME in (select BarCode2 from RBSYNERGY.dbo.tblBarCodeInv))
                     and (sm.ServicePlaceDate>=DATEADD(DAY, 41, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 60, RegistrationDate)) 
                    and sm.IME is not null and sm.Model='{0}' 
                    and sm.Servicepointid not in (69))as kk", model);
                }
                else
                {
                    getSmartPhoneDailySales4 =
                        string.Format(
                            @"select kk.SixtyDaysIMEI from
                        (Select distinct IME as SixtyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and (sm.IME in (select BarCode from RBSYNERGY.dbo.tblBarCodeInv where Updatedby='{1}') )
                        and (sm.ServicePlaceDate>=DATEADD(DAY, 41, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 60, RegistrationDate)) 
                        and sm.IME is not null and sm.Model='{0}'  
                        and sm.Servicepointid not in (69)

                        union 

                        Select distinct IME as SixtyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and ( sm.IME in (select BarCode2 from RBSYNERGY.dbo.tblBarCodeInv where Updatedby='{1}'))
                        and (sm.ServicePlaceDate>=DATEADD(DAY, 41, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 60, RegistrationDate)) 
                        and sm.IME is not null and sm.Model='{0}'  
                        and sm.Servicepointid not in (69)) as kk",
                            model, orders);
                }
                if (orders == "ALL")
                {

                    getSmartPhoneDailySales5 =
                        string.Format(
                         @"select kk.NinetyDaysIMEI from
                    (Select distinct IME as NinetyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                    join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                    where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                    from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                    and (sm.IME in (select BarCode from RBSYNERGY.dbo.tblBarCodeInv)) 
                    and (sm.ServicePlaceDate>=DATEADD(DAY, 61, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 90, RegistrationDate))
                    and sm.IME is not null and sm.Model='{0}'  
                    and sm.Servicepointid not in (69)

                    union

                    Select distinct IME as NinetyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                    join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                    where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                    from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                    and (sm.IME in (select BarCode2 from RBSYNERGY.dbo.tblBarCodeInv))
                    and (sm.ServicePlaceDate>=DATEADD(DAY, 61, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 90, RegistrationDate)) 
                    and sm.IME is not null and sm.Model='{0}' 
                    and sm.Servicepointid not in (69))as kk", model);
                }
                else
                {
                    getSmartPhoneDailySales5 =
                        string.Format(
                            @"select kk.NinetyDaysIMEI from
                        (Select distinct IME as NinetyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and (sm.IME in (select BarCode from RBSYNERGY.dbo.tblBarCodeInv where Updatedby='{1}') )
                         and (sm.ServicePlaceDate>=DATEADD(DAY, 61, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 90, RegistrationDate)) 
                        and sm.IME is not null and sm.Model='{0}'  
                        and sm.Servicepointid not in (69)

                        union 

                        Select distinct IME as NinetyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and ( sm.IME in (select BarCode2 from RBSYNERGY.dbo.tblBarCodeInv where Updatedby='{1}'))
                         and (sm.ServicePlaceDate>=DATEADD(DAY, 61, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 90, RegistrationDate)) 
                        and sm.IME is not null and sm.Model='{0}'  
                        and sm.Servicepointid not in (69)) as kk",
                            model, orders);
                }

                if (orders == "ALL")
                {

                    getSmartPhoneDailySales6 =
                        string.Format(
                         @"select kk.OneEightyDaysIMEI from
                    (Select distinct IME as OneEightyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                    join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                    where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                    from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                    and (sm.IME in (select BarCode from RBSYNERGY.dbo.tblBarCodeInv)) 
                     and (sm.ServicePlaceDate>=DATEADD(DAY, 91, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 180, RegistrationDate)) 
                    and sm.IME is not null and sm.Model='{0}'  
                    and sm.Servicepointid not in (69)

                    union

                    Select distinct IME as OneEightyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                    join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                    where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                    from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                    and (sm.IME in (select BarCode2 from RBSYNERGY.dbo.tblBarCodeInv))
                   and (sm.ServicePlaceDate>=DATEADD(DAY, 91, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 180, RegistrationDate)) 
                    and sm.IME is not null and sm.Model='{0}' 
                    and sm.Servicepointid not in (69))as kk", model);
                }
                else
                {
                    getSmartPhoneDailySales6 =
                        string.Format(
                            @"select kk.OneEightyDaysIMEI from
                        (Select distinct IME as OneEightyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and (sm.IME in (select BarCode from RBSYNERGY.dbo.tblBarCodeInv where Updatedby='{1}') )
                        and (sm.ServicePlaceDate>=DATEADD(DAY, 91, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 180, RegistrationDate)) 
                        and sm.IME is not null and sm.Model='{0}'  
                        and sm.Servicepointid not in (69)

                        union 

                        Select distinct IME as OneEightyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and ( sm.IME in (select BarCode2 from RBSYNERGY.dbo.tblBarCodeInv where Updatedby='{1}'))
                        and (sm.ServicePlaceDate>=DATEADD(DAY, 91, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 180, RegistrationDate)) 
                        and sm.IME is not null and sm.Model='{0}'  
                        and sm.Servicepointid not in (69)) as kk",
                            model, orders);
                }

                if (orders == "ALL")
                {

                    getSmartPhoneDailySales7 =
                        string.Format(
                             @"select kk.TwoSeventyDaysIMEI from
                        (Select distinct IME as TwoSeventyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and (sm.IME in (select BarCode from RBSYNERGY.dbo.tblBarCodeInv)) 
                          and (sm.ServicePlaceDate>=DATEADD(DAY, 181, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 270, RegistrationDate)) 
                        and sm.IME is not null and sm.Model='{0}'  
                        and sm.Servicepointid not in (69)

                        union

                        Select distinct IME as TwoSeventyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and (sm.IME in (select BarCode2 from RBSYNERGY.dbo.tblBarCodeInv))
                         and (sm.ServicePlaceDate>=DATEADD(DAY, 181, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 270, RegistrationDate)) 
                        and sm.IME is not null and sm.Model='{0}' 
                        and sm.Servicepointid not in (69))as kk", model);
                }
                else
                {
                    getSmartPhoneDailySales7 =
                        string.Format(
                            @"select kk.TwoSeventyDaysIMEI from
                        (Select distinct IME as TwoSeventyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and (sm.IME in (select BarCode from RBSYNERGY.dbo.tblBarCodeInv where Updatedby='{1}') )
                        and (sm.ServicePlaceDate>=DATEADD(DAY, 181, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 270, RegistrationDate)) 
                        and sm.IME is not null and sm.Model='{0}'  
                        and sm.Servicepointid not in (69)

                        union 

                        Select distinct IME as TwoSeventyDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and ( sm.IME in (select BarCode2 from RBSYNERGY.dbo.tblBarCodeInv where Updatedby='{1}'))
                         and (sm.ServicePlaceDate>=DATEADD(DAY, 181, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 270, RegistrationDate)) 
                        and sm.IME is not null and sm.Model='{0}'  
                        and sm.Servicepointid not in (69)) as kk",
                            model, orders);
                }

                if (orders == "ALL")
                {

                    getSmartPhoneDailySales8 =
                        string.Format(
                             @"select kk.ThreeSixtyFiveDaysIMEI from
                        (Select distinct IME as ThreeSixtyFiveDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and (sm.IME in (select BarCode from RBSYNERGY.dbo.tblBarCodeInv)) 
                        and (sm.ServicePlaceDate>=DATEADD(DAY, 271, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 365, RegistrationDate)) 
                        and sm.IME is not null and sm.Model='{0}'  
                        and sm.Servicepointid not in (69)

                        union

                        Select distinct IME as ThreeSixtyFiveDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and (sm.IME in (select BarCode2 from RBSYNERGY.dbo.tblBarCodeInv))
                        and (sm.ServicePlaceDate>=DATEADD(DAY, 271, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 365, RegistrationDate)) 
                        and sm.IME is not null and sm.Model='{0}' 
                        and sm.Servicepointid not in (69))as kk", model);
                }
                else
                {
                    getSmartPhoneDailySales8 =
                        string.Format(
                            @"select kk.ThreeSixtyFiveDaysIMEI from
                        (Select distinct IME as ThreeSixtyFiveDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and (sm.IME in (select BarCode from RBSYNERGY.dbo.tblBarCodeInv where Updatedby='{1}') )
                       and (sm.ServicePlaceDate>=DATEADD(DAY, 271, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 365, RegistrationDate)) 
                        and sm.IME is not null and sm.Model='{0}'  
                        and sm.Servicepointid not in (69)

                        union 

                        Select distinct IME as ThreeSixtyFiveDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and ( sm.IME in (select BarCode2 from RBSYNERGY.dbo.tblBarCodeInv where Updatedby='{1}'))
                        and (sm.ServicePlaceDate>=DATEADD(DAY, 271, RegistrationDate) and sm.ServicePlaceDate<=DATEADD(DAY, 365, RegistrationDate))  
                        and sm.IME is not null and sm.Model='{0}'  
                        and sm.Servicepointid not in (69)) as kk",
                            model, orders);
                }


                if (orders == "ALL")
                {

                    getSmartPhoneDailySales9 =
                        string.Format(
                             @"select kk.RestOftheDaysIMEI from
                        (Select distinct IME as RestOftheDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and (sm.IME in (select BarCode from RBSYNERGY.dbo.tblBarCodeInv)) 
                        and (sm.ServicePlaceDate>=DATEADD(DAY, 366, RegistrationDate) and sm.ServicePlaceDate<=GETDATE()) 
                        and sm.IME is not null and sm.Model='{0}'  
                        and sm.Servicepointid not in (69)

                        union

                        Select distinct IME as RestOftheDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and (sm.IME in (select BarCode2 from RBSYNERGY.dbo.tblBarCodeInv))
                        and (sm.ServicePlaceDate>=DATEADD(DAY, 366, RegistrationDate) and sm.ServicePlaceDate<=GETDATE()) 
                        and sm.IME is not null and sm.Model='{0}' 
                        and sm.Servicepointid not in (69))as kk", model);
                }
                else
                {
                    getSmartPhoneDailySales9 =
                        string.Format(
                            @"select kk.RestOftheDaysIMEI from
                        (Select distinct IME as RestOftheDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and (sm.IME in (select BarCode from RBSYNERGY.dbo.tblBarCodeInv where Updatedby='{1}') )
                        and (sm.ServicePlaceDate>=DATEADD(DAY, 366, RegistrationDate) and sm.ServicePlaceDate<=GETDATE()) 
                        and sm.IME is not null and sm.Model='{0}'  
                        and sm.Servicepointid not in (69)

                        union 

                        Select distinct IME as RestOftheDaysIMEI from WSMS.dbo.ServiceMaster sm 
                        join RBSYNERGY.dbo.tblProductRegistration tpr on tpr.ProductID=sm.IME
                        where  tpr.RegistrationDate>= (select Min(tp.ReleaseDate)
                        from RBSYNERGY.dbo.tblCellPhoneDepriciationPrice tp where tp.Model=sm.Model)
                        and ( sm.IME in (select BarCode2 from RBSYNERGY.dbo.tblBarCodeInv where Updatedby='{1}'))
                        and (sm.ServicePlaceDate>=DATEADD(DAY, 366, RegistrationDate) and sm.ServicePlaceDate<=GETDATE()) 
                        and sm.IME is not null and sm.Model='{0}'  
                        and sm.Servicepointid not in (69)) as kk",
                            model, orders);
                }
                var command1 = new SqlCommand(getSmartPhoneDailySales1, connection);
                command1.CommandTimeout = 2000;

                var command2 = new SqlCommand(getSmartPhoneDailySales2, connection);
                command2.CommandTimeout = 2000;

                var command3 = new SqlCommand(getSmartPhoneDailySales3, connection);
                command3.CommandTimeout = 2000;

                var command4 = new SqlCommand(getSmartPhoneDailySales4, connection);
                command4.CommandTimeout = 2000;

                var command5 = new SqlCommand(getSmartPhoneDailySales5, connection);
                command5.CommandTimeout = 2000;

                var command6 = new SqlCommand(getSmartPhoneDailySales6, connection);
                command6.CommandTimeout = 2000;

                var command7 = new SqlCommand(getSmartPhoneDailySales7, connection);
                command7.CommandTimeout = 2000;

                var command8 = new SqlCommand(getSmartPhoneDailySales8, connection);
                command8.CommandTimeout = 2000;

                var command9 = new SqlCommand(getSmartPhoneDailySales9, connection);
                command9.CommandTimeout = 2000;


                DataSet ds = new DataSet("ServiceIMEIQuantity");
                DataTable dTable1 = new DataTable();
                DataTable dTable2 = new DataTable();
                DataTable dTable3 = new DataTable();
                DataTable dTable4 = new DataTable();

                DataTable dTable5 = new DataTable();

                DataTable dTable6 = new DataTable();

                DataTable dTable7 = new DataTable();

                DataTable dTable8 = new DataTable();

                DataTable dTable9 = new DataTable();


                using (SqlDataReader reader = command1.ExecuteReader())
                {

                    if (dTable1 != null)
                    {

                        dTable1.Columns.Add("TenDaysIMEI");


                        while (reader.Read())
                        {
                            DataRow newRow = dTable1.NewRow();

                            newRow["TenDaysIMEI"] = Convert.ToDouble(reader["TenDaysIMEI"]);

                            dTable1.Rows.Add(newRow);

                        }
                        DataRow newRow1 = dTable1.NewRow();
                        dTable1.Rows.Add(newRow1);

                        DataRow newRow2 = dTable1.NewRow();
                        dTable1.Rows.Add(newRow2);

                        DataRow newRow3 = dTable1.NewRow();
                        dTable1.Rows.Add(newRow3);
                    }
                }

                using (SqlDataReader reader = command2.ExecuteReader())
                {

                    if (dTable2 != null)
                    {

                        dTable2.Columns.Add("TwentyDaysIMEI");


                        while (reader.Read())
                        {
                            DataRow newRow = dTable2.NewRow();

                            newRow["TwentyDaysIMEI"] = Convert.ToDouble(reader["TwentyDaysIMEI"]);

                            dTable2.Rows.Add(newRow);

                        }
                        DataRow newRow1 = dTable2.NewRow();
                        dTable2.Rows.Add(newRow1);

                        DataRow newRow2 = dTable2.NewRow();
                        dTable2.Rows.Add(newRow2);

                        DataRow newRow3 = dTable2.NewRow();
                        dTable2.Rows.Add(newRow3);
                    }
                }

                using (SqlDataReader reader = command3.ExecuteReader())
                {

                    if (dTable3 != null)
                    {

                        dTable3.Columns.Add("FourtyDaysIMEI");


                        while (reader.Read())
                        {
                            DataRow newRow = dTable3.NewRow();

                            newRow["FourtyDaysIMEI"] = Convert.ToDouble(reader["FourtyDaysIMEI"]);

                            dTable3.Rows.Add(newRow);

                        }
                        DataRow newRow1 = dTable3.NewRow();
                        dTable3.Rows.Add(newRow1);

                        DataRow newRow2 = dTable3.NewRow();
                        dTable3.Rows.Add(newRow2);

                        DataRow newRow3 = dTable3.NewRow();
                        dTable3.Rows.Add(newRow3);
                    }
                }

                using (SqlDataReader reader = command4.ExecuteReader())
                {

                    if (dTable4 != null)
                    {

                        dTable4.Columns.Add("SixtyDaysIMEI");


                        while (reader.Read())
                        {
                            DataRow newRow = dTable4.NewRow();

                            newRow["SixtyDaysIMEI"] = Convert.ToDouble(reader["SixtyDaysIMEI"]);

                            dTable4.Rows.Add(newRow);

                        }
                        DataRow newRow1 = dTable4.NewRow();
                        dTable4.Rows.Add(newRow1);

                        DataRow newRow2 = dTable4.NewRow();
                        dTable4.Rows.Add(newRow2);

                        DataRow newRow3 = dTable4.NewRow();
                        dTable4.Rows.Add(newRow3);
                    }
                }

                using (SqlDataReader reader = command5.ExecuteReader())
                {

                    if (dTable5 != null)
                    {

                        dTable5.Columns.Add("NinetyDaysIMEI");


                        while (reader.Read())
                        {
                            DataRow newRow = dTable5.NewRow();

                            newRow["NinetyDaysIMEI"] = Convert.ToDouble(reader["NinetyDaysIMEI"]);

                            dTable5.Rows.Add(newRow);

                        }
                        DataRow newRow1 = dTable5.NewRow();
                        dTable5.Rows.Add(newRow1);

                        DataRow newRow2 = dTable5.NewRow();
                        dTable5.Rows.Add(newRow2);

                        DataRow newRow3 = dTable5.NewRow();
                        dTable5.Rows.Add(newRow3);
                    }
                }
                using (SqlDataReader reader = command6.ExecuteReader())
                {

                    if (dTable6 != null)
                    {

                        dTable6.Columns.Add("OneEightyDaysIMEI");


                        while (reader.Read())
                        {
                            DataRow newRow = dTable6.NewRow();

                            newRow["OneEightyDaysIMEI"] = Convert.ToDouble(reader["OneEightyDaysIMEI"]);

                            dTable6.Rows.Add(newRow);

                        }
                        DataRow newRow1 = dTable6.NewRow();
                        dTable6.Rows.Add(newRow1);

                        DataRow newRow2 = dTable6.NewRow();
                        dTable6.Rows.Add(newRow2);

                        DataRow newRow3 = dTable6.NewRow();
                        dTable6.Rows.Add(newRow3);
                    }
                }

                using (SqlDataReader reader = command7.ExecuteReader())
                {

                    if (dTable7 != null)
                    {

                        dTable7.Columns.Add("TwoSeventyDaysIMEI");


                        while (reader.Read())
                        {
                            DataRow newRow = dTable7.NewRow();

                            newRow["TwoSeventyDaysIMEI"] = Convert.ToDouble(reader["TwoSeventyDaysIMEI"]);

                            dTable7.Rows.Add(newRow);

                        }
                        DataRow newRow1 = dTable7.NewRow();
                        dTable7.Rows.Add(newRow1);

                        DataRow newRow2 = dTable7.NewRow();
                        dTable7.Rows.Add(newRow2);

                        DataRow newRow3 = dTable7.NewRow();
                        dTable7.Rows.Add(newRow3);
                    }
                }
                using (SqlDataReader reader = command8.ExecuteReader())
                {

                    if (dTable8 != null)
                    {

                        dTable8.Columns.Add("ThreeSixtyFiveDaysIMEI");


                        while (reader.Read())
                        {
                            DataRow newRow = dTable8.NewRow();

                            newRow["ThreeSixtyFiveDaysIMEI"] = Convert.ToDouble(reader["ThreeSixtyFiveDaysIMEI"]);

                            dTable8.Rows.Add(newRow);

                        }
                        DataRow newRow1 = dTable8.NewRow();
                        dTable8.Rows.Add(newRow1);

                        DataRow newRow2 = dTable8.NewRow();
                        dTable8.Rows.Add(newRow2);

                        DataRow newRow3 = dTable8.NewRow();
                        dTable8.Rows.Add(newRow3);
                    }
                }

                using (SqlDataReader reader = command9.ExecuteReader())
                {

                    if (dTable9 != null)
                    {

                        dTable9.Columns.Add("RestOftheDaysIMEI");


                        while (reader.Read())
                        {
                            DataRow newRow = dTable9.NewRow();

                            newRow["RestOftheDaysIMEI"] = Convert.ToDouble(reader["RestOftheDaysIMEI"]);

                            dTable9.Rows.Add(newRow);

                        }
                        DataRow newRow1 = dTable9.NewRow();
                        dTable9.Rows.Add(newRow1);

                        DataRow newRow2 = dTable9.NewRow();
                        dTable9.Rows.Add(newRow2);

                        DataRow newRow3 = dTable9.NewRow();
                        dTable9.Rows.Add(newRow3);
                    }
                }
                ///////////////test//////////
                ds.Tables.Add(dTable1);
                ds.Tables.Add(dTable2);
                ds.Tables.Add(dTable3);
                ds.Tables.Add(dTable4);
                ds.Tables.Add(dTable5);
                ds.Tables.Add(dTable6);
                ds.Tables.Add(dTable7);
                ds.Tables.Add(dTable8);
                ds.Tables.Add(dTable9);

                GridView gv1 = new GridView();
                gv1.HeaderStyle.BackColor = System.Drawing.Color.Blue;
                gv1.HeaderStyle.ForeColor = System.Drawing.Color.White;
                gv1.HeaderStyle.Font.Bold = true;
                gv1.HeaderStyle.Font.Size = 16;
                gv1.DataSource = dTable1;
                gv1.DataBind();
                Response.Clear();
                Response.AddHeader("content-disposition", "attachment;filename=" + excelFileNames);
                Response.ContentType = "application/vnd.ms-excel";
                StringWriter sw1 = new StringWriter();
                HtmlTextWriter hw1 = new HtmlTextWriter(sw1);
                gv1.RenderControl(hw1);

                GridView gv2 = new GridView();
                gv2.HeaderStyle.BackColor = System.Drawing.Color.Green;
                gv2.HeaderStyle.ForeColor = System.Drawing.Color.White;
                gv2.HeaderStyle.Font.Bold = true;
                gv2.HeaderStyle.Font.Size = 16;
                gv2.DataSource = dTable2;
                gv2.DataBind();
                Response.Clear();
                gv2.RenderControl(hw1);

                GridView gv3 = new GridView();
                gv3.HeaderStyle.BackColor = System.Drawing.Color.Gray;
                gv3.HeaderStyle.ForeColor = System.Drawing.Color.White;
                gv3.HeaderStyle.Font.Bold = true;
                gv3.HeaderStyle.Font.Size = 16;
                gv3.DataSource = dTable3;
                gv3.DataBind();
                Response.Clear();
                gv3.RenderControl(hw1);

                GridView gv4 = new GridView();
                gv4.HeaderStyle.BackColor = System.Drawing.Color.IndianRed;
                gv4.HeaderStyle.ForeColor = System.Drawing.Color.White;
                gv4.HeaderStyle.Font.Bold = true;
                gv4.HeaderStyle.Font.Size = 16;
                gv4.DataSource = dTable4;
                gv4.DataBind();
                Response.Clear();
                gv4.RenderControl(hw1);

                GridView gv5 = new GridView();
                gv5.HeaderStyle.BackColor = System.Drawing.Color.LightSlateGray;
                gv5.HeaderStyle.ForeColor = System.Drawing.Color.White;
                gv5.HeaderStyle.Font.Bold = true;
                gv5.HeaderStyle.Font.Size = 16;
                gv5.DataSource = dTable5;
                gv5.DataBind();
                Response.Clear();
                gv5.RenderControl(hw1);


                GridView gv6 = new GridView();
                gv6.HeaderStyle.BackColor = System.Drawing.Color.LimeGreen;
                gv6.HeaderStyle.ForeColor = System.Drawing.Color.White;
                gv6.HeaderStyle.Font.Bold = true;
                gv6.HeaderStyle.Font.Size = 16;
                gv6.DataSource = dTable6;
                gv6.DataBind();
                Response.Clear();
                gv6.RenderControl(hw1);

                GridView gv7 = new GridView();
                gv7.HeaderStyle.BackColor = System.Drawing.Color.MediumBlue;
                gv7.HeaderStyle.ForeColor = System.Drawing.Color.White;
                gv7.HeaderStyle.Font.Bold = true;
                gv7.HeaderStyle.Font.Size = 16;
                gv7.DataSource = dTable7;
                gv7.DataBind();
                Response.Clear();
                gv7.RenderControl(hw1);

                GridView gv8 = new GridView();
                gv8.HeaderStyle.BackColor = System.Drawing.Color.LightYellow;
                gv8.HeaderStyle.ForeColor = System.Drawing.Color.Black;
                gv8.HeaderStyle.Font.Bold = true;
                gv8.HeaderStyle.Font.Size = 16;
                gv8.DataSource = dTable8;
                gv8.DataBind();
                Response.Clear();
                gv8.RenderControl(hw1);

                GridView gv9 = new GridView();
                gv9.HeaderStyle.BackColor = System.Drawing.Color.BlueViolet;
                gv9.HeaderStyle.ForeColor = System.Drawing.Color.White;
                gv9.HeaderStyle.Font.Bold = true;
                gv9.HeaderStyle.Font.Size = 16;
                gv9.DataSource = dTable9;
                gv9.DataBind();
                Response.Clear();
                gv9.RenderControl(hw1);

                string headerTable = "<p style='color:red;font-weight:bold;font-size:20;'>Model : </p>" + "<p style='color:green;font-weight:bold;font-size:20;'>" + model + "</p>" + "<br/>" + "<br/>";
                Response.Write(headerTable);

                Response.Write("<meta http-equiv=Content-Type content=\"text/html; charset=utf-8\">" + Environment.NewLine);
                Response.Output.Write(sw1.ToString());
                Response.End();

                connection.Close();
            }
        }

        public ActionResult SyncWSMTdatabase()
        {
            var viewmodel = new WSMTSyncVm();
            List<RBSYProductModel> models = _commonRepository.GetRBSYProductModels().ToList();
            viewmodel.WSMTHandsets = _commonRepository.GetWSMTModels();
            ViewBag.RBSYModels = models;
            return View(viewmodel);
        }

        public JsonResult SyncBomswithWSMT(WSMTSyncVm vm)
        {
            var response = new ResponseMessage();
            response = _commonRepository.SyncWSMTBoms(vm);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRBSYModels()
        {
            var data = _commonRepository.GetRBSYProductModels().ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWSMTHandsets()
        {
            var data = _commonRepository.GetWSMTModels();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BOMReport(BOMReportVm vm)
        {
            var viewmodel = new BOMReportVm();
            List<WSMTHandset> models = _commonRepository.GetWSMTHandsets();
            viewmodel = _commonRepository.GetWSMTBomReportData(vm);
            ViewBag.Models = models;
            return View(viewmodel);
        }
        public ActionResult PurchaseOrderComment()
        {
            string columnName = string.Empty;
            if (HttpContext.User.IsInRole("CM") || HttpContext.User.IsInRole("CMHEAD")) columnName = "InchargeComment";
            if (HttpContext.User.IsInRole("ASPM")) columnName = "AfterSalesPmComment";
            if (HttpContext.User.IsInRole("HWHEAD")) columnName = "QcComment";
            if (HttpContext.User.IsInRole("PROC")) columnName = "ProcessTeamComment";
            var dataList = _commonRepository.GetProjectPurchaseFormData(columnName);
            return View(dataList);
        }

        public ActionResult InsertPurchaseOrderComment(long masterId, string comment)
        {
            var data = _commonRepository.InsertPurchaseOrderComment(masterId, comment);

            return RedirectToAction("PurchaseOrderComment");
        }

        public ActionResult ProjectEventDates()
        {
            var model = _commonRepository.GetAllProjectEventDates();
            return View(model);
        }

        public ActionResult ProjectVariant()
        {
            List<ProjectMasterModel> masters = _commonRepository.GetAllProjects();
            ViewBag.Projects = masters;
            return View();
        }

        public JsonResult CheckExistingVariant(long id = 0)
        {
            var variants = _commonRepository.GetProjectVariantModelsByProjectId(id);
            var projectDetail = _commonRepository.GetProjectInfoByProjectId(id);
            var json = new { Variant = variants, Project = projectDetail };
            return Json(json);
        }

        public JsonResult SaveUpdateProjectVariant(string projectName, string productModel, string prefix,
            string ramRomVariant, string suffix, string variantQuantity, DateTime addedByDate, string variantName, long addedBy = 0, long variantId = 0, long projectId = 0)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            var projectDetail = _commonRepository.GetProjectInfoByProjectId(projectId);
            var model = new ProjectVariantModel
            {
                Id = variantId,
                ProjectId = projectId,
                ProjectName = projectDetail.ProjectName,
                OrderNumber = projectDetail.OrderNuber,
                ProjectModel = productModel,
                Prefix = prefix,
                ProjectVariantName = variantName,
                VariantByRamRom = ramRomVariant,
                ProjectVariantQuantity = variantQuantity,
                TotalOrderQuantity = Convert.ToString(projectDetail.OrderQuantity),
                Suffix = suffix
            };
            if (variantId == 0)
            {
                model.AddedBy = userId;
                model.AddedDate = DateTime.Now;
            }
            else
            {
                model.AddedBy = addedBy;
                model.AddedDate = addedByDate;
                model.UpdatedBy = userId;
                model.UpdatedDate = DateTime.Now;
            }
            model = _commonRepository.SaveUpdateProjectVariant(model);
            return Json(model);
        }

        public JsonResult RemoveProjectVariant(long variantId = 0)
        {
            try
            {
                _commonRepository.RemoveProjectVariant(variantId);
                return Json(true);
            }
            catch (Exception ex)
            {

                return Json(ex);
            }
        }

        public JsonResult LockVariant(long variantId = 0)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            var variant = _commonRepository.GetProjectVariantModelById(variantId);
            variant.IsLocked = true;
            variant = _commonRepository.SaveUpdateProjectVariant(variant);
            var orderdetail = new ProjectOrderQuantityDetailModel
            {
                ProjectMasterId = variant.ProjectId,
                ProjectModel = variant.ProjectVariantName,
                OrderQuantity = Convert.ToInt64(variant.ProjectVariantQuantity),
                AddedBy = userId,
                AddedDate = DateTime.Now
            };
            _commonRepository.SaveLockedVariantToOrderQuantityDetailModel(orderdetail);
            return Json(variant);
        }

        public JsonResult UpdateProjectModelInProjectMaster(string projectModel, long projectId = 0)
        {
            if (projectId > 0)
            {
                try
                {
                    _commonRepository.UpdateProjectModelInProjectMaster(projectModel, projectId);
                    return Json("updated");
                }
                catch (Exception ex)
                {
                    return Json(ex);
                }
            }
            return Json("No action");
        }

        public ActionResult UnproducedAverageDetails(string type)
        {
            var unproduced = new List<rbsBarCodeInv>();
            var model = _commonRepository.SixMonthsUnproducedAverageQty();
            if (type == "Smart")
            {
                foreach (var v in model)
                {
                    var parts = v.ProjectModel.Split(' ');
                    if (parts[0].ToLower() == "primo" || parts[0].ToLower() == "orbit")
                    {
                        unproduced.Add(v);
                    }
                }
            }
            if (type == "Feature")
            {
                foreach (var v in model)
                {
                    var parts = v.ProjectModel.Split(' ');
                    if (parts[0].ToLower() == "olvio" || parts[0].ToLower() == "axino")
                    {
                        unproduced.Add(v);
                    }
                }
            }
            return View(unproduced);
        }

        #region ProjectPoFeedback
        public ActionResult ProjectPoFeedback(long id = 0)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            var v = _commonRepository.GetPoFeedbackById(id);
            ViewBag.projects = _commonRepository.GetAllProjects();
            ViewBag.OnBehalfOf = _commonRepository.GetRoleDescriptions();
            ViewBag.Status = TempData["Status"] == null ? "blank" : TempData["Status"].ToString();
            return View(v);
        }

        [HttpPost]
        public ActionResult ProjectPoFeedback(ProjectPoFeedbackModel model)
        {
            //File Upload
            var manager = new FileManager();
            var moduleDirectory = "PO Feedback";
            var userDirectory = "CMN";
            model.FileUploadPath = manager.IncidentUpload(userDirectory, moduleDirectory,
                model.FileUpload);
            //---0---
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            var duplicate = _commonRepository.DuplicatePoFeedbackBySamePerson(model.ProjectId, userId);

            if (model.Id > 0)
            {
                model.UpdatedBy = userId;
                model.UpdatedDate = DateTime.Now;
                _commonRepository.SaveUpdatePoFeedBackModel(model);
                TempData["Status"] = "Updated";
                return RedirectToAction("ProjectPoFeedback");
            }
            if (!duplicate.Any())
            {
                model.AddedBy = userId;
                model.AddedDate = DateTime.Now;
                _commonRepository.SaveUpdatePoFeedBackModel(model);
                TempData["Status"] = "Saved";
                return RedirectToAction("ProjectPoFeedback");
            }

            TempData["Status"] = "Already exist!";
            return RedirectToAction("ProjectPoFeedback");
        }

        public ActionResult ProjectPoFeedbacks()
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            ViewBag.Feedbacks = _commonRepository.GetPoFeedbackByUserId(userId);
            return View();
        }

        public ActionResult PoFeedbackDashboard()
        {
            ViewBag.projects = _commonRepository.GetAllProjects();
            var model = _commonRepository.GetAllProjectPoFeedbackModels();
            return View(model);
        }

        public JsonResult PoFeedbacksByProjectId(long projectId = 0)
        {
            var v = _commonRepository.GetPoFeedbackByProjectId(projectId);
            foreach (var x in v)
            {
                //x.FileUploadPath = "<a href='/Common/DownloadPoFeedbackFile/" + x.Id + "'>" +
                //                   Path.GetFileNameWithoutExtension(x.FileUploadPath) + "</a>";
                x.FileUploadPath = Path.GetFileNameWithoutExtension(x.FileUploadPath);
            }
            return Json(v);
        }

        public ActionResult DownloadPoFeedbackFile(long id = 0)
        {
            var model = _commonRepository.GetPoFeedbackById(id);
            var manager = new FileManager();
            var path = manager.GetPoFeedbackFile(model.FileUploadPath);
            var apppath = HttpContext.Server.MapPath(path);//HttpRuntime.AppDomainAppPath;
            string fileName = Path.GetFileName(path);
            var extension = Path.GetExtension(fileName);
            if (!string.IsNullOrEmpty(extension))
            {
                string ext = extension.Remove(0, 1);
                return File(apppath, "application/" + ext, fileName);
            }

            return Content("<!DOCTYPE html><html><body><p>" + fileName + "</p></body></html>");
        }

        public JsonResult SaveSourcingComment(string sourcingComment, string sourcingAllowReorder, long id = 0)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            var userInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            if (id > 0)
            {
                var model = _commonRepository.GetPoFeedbackById(id);
                if (model != null)
                {
                    model.SourcingComment = sourcingComment;
                    model.SourcingCommentDate = DateTime.Now;
                    model.SourcingCommentBy = userId;
                    model.SourcingAllowReorder = sourcingAllowReorder;
                    _commonRepository.SaveUpdatePoFeedBackModel(model);
                    return Json(new { message = "Saved", user = userInfo });
                }
            }
            return Json(new { message = "Something went wrong with the ID", user = "" });
        }

        public ActionResult FullPoFeedback()
        {
            return View();
        }
        #endregion

        #region Process Cost

        public ActionResult ImportMonthWiseProcessCost()
        {
            var orderQuantityDetail = _commonRepository.GetProjectOrderQuantityDetailModels();
            var variantnames=orderQuantityDetail.Select(x => x.ProjectModel).ToList();
            ViewBag.Variants = variantnames.GroupBy(x => x).ToList();
            var v = _commonRepository.GetProcessCostMonthWiseModels();
            foreach (var p in v)
            {
                p.Month = _commonRepository.MonthNumberToName(Convert.ToInt16(p.Month));
            }
            ViewBag.ProcessCost = v;
            return View();
        }

        [HttpPost]
        public ActionResult ImportMonthWiseProcessCost(ProcessCostMonthWiseModel model)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            var v = model.FileUpload;
            if (v != null)
            {
                var ext = Path.GetExtension(v.FileName);
                if (ext == ".xlsx" || ext==".xls")
                {
                    var target = new MemoryStream();
                    model.FileUpload.InputStream.CopyTo(target);
                    byte[] data = target.ToArray();
                    using(var stream=new MemoryStream(data))
                    using (var excelPackage = new ExcelPackage(stream))
                    {
                        var worksheet = excelPackage.Workbook.Worksheets[1];
                        //loop all rows
                        for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                        {
                            if (!_commonRepository.DuplicateProcessCostCheckerByVariantName(worksheet.Cells[i, 1].Value.ToString()))
                            {
                                var process = new ProcessCostMonthWiseModel
                                {
                                    VariantName = worksheet.Cells[i, 1].Value.ToString(),
                                    ProcessCost = worksheet.Cells[i, 2].Value.ToString(),
                                    Month = model.Month,
                                    Year = model.Year,
                                    AddedDate = DateTime.Now,
                                    AddedBy = Convert.ToString(userId)
                                };
                                _commonRepository.SaveProcessCost(process);   
                            }
                        }
                    }
                }
            }
            return RedirectToAction("ImportMonthWiseProcessCost");
        }
        #endregion

        #region FOC Claim

        public ActionResult FocClaimForm()
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            ViewBag.Projects = _iqcRepository.GetAllVariants();
            var focClaims = _commonRepository.GetFocClaimAddedBy(userId);
            return View(focClaims);
        }

        public JsonResult GetBomByProjectModel(long id=0)
        {
            var variant = _iqcRepository.GetVariantById(id);
            var json = _iqcRepository.GetBomByProjectModel(variant.ProjectModel);
            var v = _commonRepository.GetBomDescriptionByIdThenProjectModel(id);
            return Json(json);
        }

        public JsonResult GetBomDescriptionByIdThenProjectModel(long id = 0)
        {
            var json = _commonRepository.GetBomDescriptionByIdThenProjectModel(id);
            return Json(json);
        }

        public JsonResult GetSpareDescriptionByDescription(string description)
        {
            var json = _commonRepository.GetSpareDescriptionByDescription(description);
            return Json(json);
        }

        public JsonResult SaveFocClaim( string desc, string spareDesc, string focClaimQuantity,
            long orderQuantityDetailId = 0)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            var model = new FocClaimModel();
            model.OrderQuantityDetailId = orderQuantityDetailId;
            model.Description = desc;
            model.SpareDescription = spareDesc;
            model.ClaimQuantity = focClaimQuantity;
            model.ClaimDate = DateTime.Now;
            model.ClaimedBy = userId;
            model = _commonRepository.SaveFocClaimModel(model);
            model.StrClaimDate = model.ClaimDate.ToString();
            return Json(model);
        }

        public ActionResult FocClaimReceive()
        {
            var model = _commonRepository.GetAllFocClaims();
            return View(model);
        }

        public JsonResult SaveFocClaimReceive(string receiveQuantity, long id = 0)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            var model = _commonRepository.GetFocClaimById(id);
            model.ReceiveQuantity = receiveQuantity;
            model.ReceivedBy = userId;
            model.ReceivedDate = DateTime.Now;
            model = _commonRepository.UpdateFocClaimModel(model);
            model.StrReceivedDate = Convert.ToString(model.ReceivedDate);
            model.StrClaimDate = model.ClaimDate.ToString();
            return Json(model);
        }
        #endregion

        #region Variant creation for project manager

        public ActionResult CreateProjectVariant()
        {
            ViewBag.projects = _commonRepository.GetAllProjects();
            return View();
        }

        public JsonResult CheckProjectInfoAndExistingVariant(long projectId = 0)
        {
            var projectDetail = _commonRepository.GetProjectInfoByProjectId(projectId);
            var orderQuantityDetails = _commonRepository.GetOrderQuantityDetailByProjectId(projectId);
            return Json(new {projectDetail, orderQuantityDetails});
        }

        public JsonResult SaveProjectVariant(string variantName,string ramVendor,string romVendor, long variantQuantity=0, long projectId = 0)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            try
            {
                var model = new ProjectOrderQuantityDetailModel
                {
                    ProjectMasterId = projectId,
                    ProjectModel = variantName.Trim().Replace("\t",String.Empty),
                    OrderQuantity = variantQuantity,
                    IsActive = true,
                    AddedBy = userId,
                    AddedDate = DateTime.Now,
                    RamVendor = ramVendor,
                    RomVendor = romVendor
                };
                var variantExists = _commonRepository.VariantAlreadyExists(variantName, projectId);
                if (!variantExists)
                {
                    var json = _commonRepository.SaveUpdateProjectVariantInOrderQuantityDetail(model);
                    //===email notification====
                    MailSendFromPms mailSendFromPms = new MailSendFromPms();
                    var addedByDetails = _hardwareRepository.GetUserInfoByUserId(userId);
                    var project = _commonRepository.GetProjectInfoByProjectId(projectId);
                    //List<long> to = new List<long> { 10003, 1 };//10003=Raihan sir, 1 = Super Admin --> my email
                    //if (userId != 17771)//17771=Pranto bro PMHEAD
                    //{
                    //    to.Add(17771);
                    //}
                    mailSendFromPms.SendMail(new List<string>(new[] { "PM", "PMHEAD", "CMHEAD", "CM" }), new List<string>(new[] { "PS" }), "New variant has been created",
                    "This is to inform you that, new variant <b>" + variantName + " </b> has been ADDED by <b>" + addedByDetails.UserFullName + ".<br/><br/><br/><br/>"
                               + "Project : <b>" + project.ProjectName
                               + ", Order No : <b>" + project.OrderNuber + "</b> <br/>Variant Quantity - "
                              + variantQuantity + "<br/>Total Order Quantity - " + project.OrderQuantity);
                    //=========================
                    return Json(json);
                }
                return Json("Variant already exists!!!");
            }
            catch (Exception ex)
            {
                return Json(ex.ToString());
            }
        }

        public JsonResult UpdateProjectVariant(string variantName, string ramVendor, string romVendor, string addedByDate, long variantQuantity = 0, long variantId = 0,long projectId = 0, long addedBy=0)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            var prevVariantInfo = _commonRepository.GetOrderQuantityDetailById(variantId);
            DateTime dt;
            DateTime.TryParseExact(addedByDate, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
            var model = new ProjectOrderQuantityDetailModel
            {
                Id=variantId,
                ProjectMasterId = projectId,
                ProjectModel = variantName.Trim().Replace("\t", String.Empty),
                OrderQuantity = variantQuantity,
                IsActive = true,
                AddedBy = addedBy,
                AddedDate = dt,
                UpdatedBy = userId,
                UpdatedDate = DateTime.Now,
                RamVendor = ramVendor,
                RomVendor = romVendor
            };
            var json = _commonRepository.SaveUpdateProjectVariantInOrderQuantityDetail(model);
            //===email notification====
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            var addedByDetails = _hardwareRepository.GetUserInfoByUserId(userId);
            var project = _commonRepository.GetProjectInfoByProjectId(projectId);
            //List<long> to = new List<long> { 10003, 1 };//10003=Raihan sir, 1 = Super Admin
            //if (userId != 13038)//13038=Tanvir Vai PMHEAD
            //{
            //    to.Add(13038);
            //}
            if (prevVariantInfo.ProjectModel != variantName)
            {
                mailSendFromPms.SendMail(new List<string>(new[] { "PM", "PMHEAD", "CMHEAD", "CM" }), new List<string>(new[] { "PS" }), "Variant name has been updated",
            "This is to inform you that, variant name updated to <b>" + variantName + " </b> by <b>" + addedByDetails.UserFullName + " from <b>"+prevVariantInfo.ProjectModel+"</b>.<br/><br/><br/><br/>"
                       + "Project : <b>" + project.ProjectName + "</b> " 
                       + ", Order No : <b>" + project.OrderNuber + "</b> " 
                       + "<br/>Variant Quantity - " + variantQuantity + "<br/>Total Order Quantity - " + project.OrderQuantity);
            }
            if (prevVariantInfo.OrderQuantity != variantQuantity)
            {
                mailSendFromPms.SendMail(new List<string>(new[] { "PM", "PMHEAD", "CMHEAD", "CM" }), new List<string>(new[] { "PS" }), "Variant quantity has been updated",
            "This is to inform you that, variant quantity of variant <b>" + variantName + " </b> has been updated by <b>" + addedByDetails.UserFullName + " from <b>" + prevVariantInfo.OrderQuantity + "</b> to <b>"+variantQuantity+"</b>.<br/><br/><br/><br/>"
                       + "Project : <b>" + project.ProjectName  
                       + ", Order No : <b>" + project.OrderNuber  
                       + "<br/>Total Order Quantity - " + project.OrderQuantity);
            }
            //=========================
            return Json(json);
        }

        public ActionResult AllVariants()
        {
            var model = _commonRepository.GetOrderQuantityDetails();
            return View(model);
        }
        #endregion

        #region LC Opening Permission Dashboard

        public ActionResult LcPermissionDashboard()
        {
            ViewBag.ProjectWithOrder = _commonRepository.GetAllProjects();
            ViewBag.ProjectModel = _commonRepository.GetAllProjectModels();
            ViewBag.AllLcPermissions = _commonRepository.GeAllPipelineLcPermissions();
            return View();
        }

        public JsonResult GetLcPermissionsByProjectId(long projectId = 0)
        {
            var chartData = new List<PriceEvolutionGraph>();
            var lcPer = _commonRepository.GetLcPermissionsByProjectId(projectId);
            foreach (var lp in lcPer)
            {
                var cd = new PriceEvolutionGraph();
                cd.value = Math.Round(Convert.ToDouble(lp.LcAmount) / Convert.ToDouble(lp.OrderQuantity),2);//avg
                cd.date = lp.OpeningDate.ToString();
                cd.date = Convert.ToDateTime(cd.date).ToString("yyyy-MM-dd");
                chartData.Add(cd);
            }
            return Json(chartData);
        }
        public JsonResult GetLcPermissionsByProjectModel(string projectModel)
        {
            var chartData = new List<PriceEvolutionGraph>();
            var infos = _commonRepository.GetProjectInfoByProjectModel(projectModel);
            var lcPer = _commonRepository.GetLcPermissionByProjectModel(projectModel);
            foreach (var lp in lcPer)
            {
                var cd = new PriceEvolutionGraph();
                cd.value = Math.Round(Convert.ToDouble(lp.LcAmount) / Convert.ToDouble(lp.OrderQuantity), 2);//avg
                cd.date = lp.OpeningDate.ToString();
                cd.date = Convert.ToDateTime(cd.date).ToString("yyyy-MM-dd");
                chartData.Add(cd);
            }
            var orders = _commonRepository.GetOrderNumbersByProjectModel(projectModel);
            return Json(new{chartData,orders,infos,lcPer});
        }

        public ActionResult LcOpeningApproval()
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var lcOpeningPermissionModels = _commercialRepository.GetLcPermissionList();
            var otherLcOpening = _commercialRepository.GetLcPermissionOtherProductList();
            ViewBag.OtherProductPermission = otherLcOpening;
            if (User.IsInRole("MM"))
            {
                lcOpeningPermissionModels = lcOpeningPermissionModels.Where(x => x.ApprovedBy == null && x.AcknowledgedBy!=null && x.TotalAmount>10000).ToList();
                ViewBag.OtherProductPermission = otherLcOpening.Where(x => x.ApprovedBy == null && x.AcknowledgedBy != null && x.TotalAmount > 10000).ToList();
            }
            if (User.IsInRole("PS"))
            {
                lcOpeningPermissionModels = lcOpeningPermissionModels.Where(x => x.AcknowledgedBy == null && x.TotalAmount > 5000).ToList();
                ViewBag.OtherProductPermission = otherLcOpening.Where(x => x.AcknowledgedBy == null && x.TotalAmount > 5000).ToList();
            }
            if (User.IsInRole("CEO"))
            {
                lcOpeningPermissionModels = lcOpeningPermissionModels.Where(x => x.CeoApprovalBy == null).ToList();
                ViewBag.OtherProductPermission = otherLcOpening.Where(x => x.CeoApprovalBy == null).ToList();
            }
            if (User.IsInRole("BIHEAD"))
            {
                lcOpeningPermissionModels = lcOpeningPermissionModels.Where(x => x.BiApprovalBy == null && x.ApprovedBy == null && x.TotalAmount > 10000).ToList();
                ViewBag.OtherProductPermission = otherLcOpening.Where(x => x.BiApprovalBy == null && x.ApprovedBy == null && x.TotalAmount > 10000).ToList();
            }
            if (User.IsInRole("CMHEAD"))
            {
                lcOpeningPermissionModels = lcOpeningPermissionModels.Where(x => x.SourcingApprovalBy == null).ToList();
                ViewBag.OtherProductPermission = otherLcOpening.Where(x => x.SourcingApprovalBy == null).ToList();
            }
            if (User.IsInRole("FINHEAD"))
            {
                lcOpeningPermissionModels = lcOpeningPermissionModels.Where(x => x.FinanceApprovalBy == null).ToList();
                ViewBag.OtherProductPermission = otherLcOpening.Where(x => x.FinanceApprovalBy == null).ToList();
            }
            if (User.IsInRole("ACCNTHEAD"))
            {
                lcOpeningPermissionModels = lcOpeningPermissionModels.Where(x => x.AccountsApprovalBy == null).ToList();
                ViewBag.OtherProductPermission = otherLcOpening.Where(x => x.AccountsApprovalBy == null).ToList();
            }
            return View(lcOpeningPermissionModels);
        }

        public JsonResult ApproveLc(string remarks, long id = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var model = _commonRepository.GetLcOpeningPermissionById(id);
            if (User.IsInRole("CMHEAD"))
            {
                model.SourcingRemarks = remarks;
                model.SourcingApprovalBy = userId;
                model.SourcingApprovalDate = DateTime.Now;
            }
            if (User.IsInRole("ACCNT") || User.IsInRole("ACCNTHEAD"))
            {
                model.AccountsApprovalBy = userId;
                model.AccountsRemarks = remarks;
                model.AccountsApprovalDate = DateTime.Now;
            }
            if (User.IsInRole("FIN") || User.IsInRole("FINHEAD"))
            {
                model.FinanceApprovalBy = userId;
                model.FinanceRemarks = remarks;
                model.FinanceApprovalDate = DateTime.Now;
            }
            if (User.IsInRole("BIHEAD"))
            {
                model.BiApprovalBy = userId;
                model.BiRemarks = remarks;
                model.BiApprovalDate = DateTime.Now;
            }
            if (User.IsInRole("CEO"))
            {
                model.CeoApprovalBy = userId;
                model.CeoRemarks = remarks;
                model.CeoApprovalDate = DateTime.Now;
                if (model.TotalAmount <= 5000)
                {
                    //auto BI approval
                    model.BiApprovalBy = 20145;//rezaul vai
                    model.BiApprovalDate = DateTime.Now;
                    model.BiRemarks = "LC equal or less than 5000 USD gets auto approved by CBO";
                    //auto PS approval
                    model.AcknowledgedBy = 10001;//adnan vai
                    model.AcknowledgeDate = DateTime.Now;
                    model.AcknowledgeRemarks = "LC equal or less than 5000 USD gets auto approved by CBO";
                    //auto MM approval
                    model.ApprovedByRemarks = "LC equal or less than 5000 USD gets auto approved by CBO";
                    model.ApprovedBy = 36;//ovee sir
                    model.ApprovedDate = DateTime.Now;
                    model.IsApproved = true;
                }
            }
            if (User.IsInRole("PS"))
            {
                model.AcknowledgedBy = userId;
                model.AcknowledgeRemarks = remarks;
                model.AcknowledgeDate = DateTime.Now;
                if (model.TotalAmount <= 10000)
                {
                    //auto BI approval
                    model.BiApprovalBy = 20145;//rezaul vai
                    model.BiApprovalDate = DateTime.Now;
                    model.BiRemarks = "LC equal or less than 10000 USD gets auto approved by acknowledger";
                    //auto MM approval
                    model.ApprovedByRemarks = "LC equal or less than 10000 USD gets auto approved by acknowledger";
                    model.ApprovedBy = 36;
                    model.ApprovedDate = DateTime.Now;
                    model.IsApproved = true;
                }
            }
            if (User.IsInRole("MM"))
            {
                model.ApprovedByRemarks = remarks;
                model.ApprovedBy = userId;
                model.ApprovedDate = DateTime.Now;
                model.IsApproved = true;
            }
            model = _commonRepository.SaveLcOpeningPermission(model);
            //===email notification====
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            var addedByDetails = _hardwareRepository.GetUserInfoByUserId(userId);
            var project = _commonRepository.GetProjectInfoByProjectId(model.ProjectMasterId??0);
            mailSendFromPms.SendMail(new List<string>(new[] { "CM","CMHEAD","ACCNT","ACCNTHEAD","FIN","FINHEAD","CEO","MM","PS","BIHEAD" }), new List<string>(new[] { "" }), "LC Opening Approved for "+project.ProjectName+"(Order "+project.OrderNuber+")",
            "This is to inform you that, LC opening approved by <b>" + addedByDetails.UserFullName + "</b>.<br/>"
                       + "Project : " + project.ProjectName + ",<br/>"
                       + "Order No : " + project.OrderNuber + ",<br/>"
                       + "LC Permission ID : " + model.Id + ",<br/>"
                       + "LC Value : " + model.LcAmount + ",<br/>"
                       + "LC Order Quantity : " + model.OrderQuantity);
            //=========================
            return Json("success");
        }

        public ActionResult LcOpeningApprovalDetails(long id=0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var lcVm = new VmProjectLc();
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            var foreCasts = new List<SalesForecastingReport>();
            var lc = _commonRepository.GetLcOpeningPermissionById(id);//lc opening details
            lcVm.LcOpeningPermissionModel = lc;
            lcVm.LcOpeningPermissionFileModels = _commonRepository.GetLcOpeningPermissionFilesByLcId(id);//lc files
            if (lc!=null && lc.ProjectMasterId != null)
            {
                var master = _commonRepository.GetProjectInfoByProjectId((long) lc.ProjectMasterId);//Project Master
                ViewBag.Master = master;
                var accessoriesPrice = _commonRepository.GetAccessoriesPricesByProjectId((long)lc.ProjectMasterId);//Accessories Price
                ViewBag.AccPrice = accessoriesPrice;
                ViewBag.JigsPrice = _commonRepository.GetJigsPriceByProjectName(master.ProjectName);//Jigs Price
                var masterPoVariant = _commonRepository.GetMasterPoVariantByProjectName(lc.Model);//all PO with variant
                ViewBag.MasterPoVariant = masterPoVariant;
                var allVariants = masterPoVariant.GroupBy(x => x.VariantName).ToList();//all variants
                var totalOrderQuantity = 0;
                foreach (var m in masterPoVariant)
                {
                    totalOrderQuantity = (int) (totalOrderQuantity + m.VariantQuantity);
                }
                ViewBag.TotalOrderQuantity = totalOrderQuantity;
                ViewBag.Po = _commonRepository.GetProjectPurchaseOrderByProjectId((long) lc.ProjectMasterId);// Purchase Order
                if (ViewBag.Master.OrderNuber>1)
                {
                    ViewBag.ProjectAge = _commonRepository.GetProjectAge(lc.Model);
                }
                else
                {
                    ViewBag.ProjectAge = "Project age is not applicable for 1st order.";
                }
                foreach (var v in allVariants)
                {
                    var forCast=
                    _commonRepository.GetSalesForecastingReportByVariantName(v.Key);//Sales Forecast by Variant
                    foreCasts.Add(forCast);
                }
                var priceRange = _commonRepository.GetPriceRangeByFinalPrice(master.FinalPrice, master.ProjectType.ToLower());//get price range for this project
                ViewBag.PriceRange = priceRange;
                if(priceRange!=null)
                ViewBag.RelevantModelForecast =
                    _commonRepository.GetSalesForecastForRelevantModelByPriceRange(priceRange.StartingRange,priceRange.FinishingRange,master.ProjectType);
            }
            ViewBag.VariantWiseForeCast = foreCasts;
            if (lc != null) ViewBag.RelevantModel = _commonRepository.GetRelevantModelByProjectId(lc.ProjectMasterId);
            if (lc != null)
                ViewBag.ProjectOrderPerformance = _commonRepository.GetProjectOrderPerformanceSumByModel(lc.Model);
            if (lc != null)
                ViewBag.SupplierPerformance =
                    _commonRepository.GetSupplierKpiPerformanceByProjectId(lc.ProjectMasterId);
            if (lc != null) ViewBag.ProjectImages = _commonRepository.GetProjectImages(lc.ProjectMasterId);
            return View(lcVm);
        }

        public JsonResult GetLcGraph(string projectName)
        {
            var lc = _commonRepository.GetProjectLcByProjectName(projectName);
            foreach (var v in lc)
            {
                v.StrLcOpeningDate = v.OpeningDate.ToString();
                v.StrLcOpeningDate = Convert.ToDateTime(v.StrLcOpeningDate).ToString("yyyy-MM-dd");
            }
            var totalLc = _commonRepository.GetMonthWiseTotalLcValueFromOracle();
            var approvedLc = _commonRepository.GetMonthWiseApprovedlLcValue();
            var dataForPriceGraph = _commonRepository.GetProjectListByProjectName(projectName);
            return Json(new { lc, totalLc, dataForPriceGraph,approvedLc });
        }

        public JsonResult ApproveOtherLc(string remarks, long id = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var model = _commonRepository.GetLcOpeningPermissionOtherProductById(id);
            if (User.IsInRole("CMHEAD"))
            {
                model.SourcingRemarks = remarks;
                model.SourcingApprovalBy = userId;
                model.SourcingApprovalDate = DateTime.Now;
            }
            if (User.IsInRole("ACCNT") || User.IsInRole("ACCNTHEAD"))
            {
                model.AccountsApprovalBy = userId;
                model.AccountsRemarks = remarks;
                model.AccountsApprovalDate = DateTime.Now;
            }
            if (User.IsInRole("FIN") || User.IsInRole("FINHEAD"))
            {
                model.FinanceApprovalBy = userId;
                model.FinanceRemarks = remarks;
                model.FinanceApprovalDate = DateTime.Now;
            }
            if (User.IsInRole("CEO"))
            {
                model.CeoApprovalBy = userId;
                model.CeoRemarks = remarks;
                model.CeoApprovalDate = DateTime.Now;
                if (model.TotalAmount <= 5000)
                {
                    //auto BI approval
                    model.BiApprovalBy = 20145;//rezaul vai
                    model.BiApprovalDate = DateTime.Now;
                    model.BiRemarks = "LC equal or less than 5000 USD gets auto approved by CBO";
                    //auto PS approval
                    model.AcknowledgedBy = 10001;//adnan vai
                    model.AcknowledgeDate = DateTime.Now;
                    model.AcknowledgeRemarks = "LC equal or less than 5000 USD gets auto approved by CBO";
                    //auto MM approval
                    model.ApprovedByRemarks = "LC equal or less than 5000 USD gets auto approved by CBO";
                    model.ApprovedBy = 36;//ovee sir
                    model.ApprovedDate = DateTime.Now;
                    model.IsApproved = true;
                }
            }
            if (User.IsInRole("BIHEAD"))
            {
                model.BiApprovalBy = userId;
                model.BiRemarks = remarks;
                model.BiApprovalDate = DateTime.Now;
            }
            if (User.IsInRole("PS"))
            {
                model.AcknowledgedBy = userId;
                model.AcknowledgeRemarks = remarks;
                model.AcknowledgeDate = DateTime.Now;
                if (model.TotalAmount <= 10000)
                {
                    //auto BI approval
                    model.BiApprovalBy = 20145;//rezaul vai
                    model.BiApprovalDate = DateTime.Now;
                    model.BiRemarks = "LC equal or less than 10000 USD gets auto approved by acknowledger";
                    //auto MM approval
                    model.ApprovedByRemarks = "LC equal or less than 10000 USD gets auto approved by acknowledger";
                    model.ApprovedBy = 36;//ovee sir
                    model.ApprovedDate = DateTime.Now;
                    model.IsApproved = true;
                }
            }
            if (User.IsInRole("MM"))
            {
                model.ApprovedByRemarks = remarks;
                model.ApprovedBy = userId;
                model.ApprovedDate = DateTime.Now;
                model.IsApproved = true;
            }
            model = _commonRepository.SaveLcOpeningPermissionOtherProduct(model);
            //===email notification====
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            var addedByDetails = _hardwareRepository.GetUserInfoByUserId(userId);
            mailSendFromPms.SendMail(new List<string>(new[] { "CM", "CMHEAD", "ACCNT", "ACCNTHEAD", "FIN", "FINHEAD", "CEO", "MM", "PS","BIHEAD" }), new List<string>(new[] { "" }), "LC Opening Approved for " + model.Product,
            "This is to inform you that, LC opening approved by <b>" + addedByDetails.UserFullName + ".<br/>"
                       + "Product Type : <b>" + model.ProductType
                       + ", Product Name : <b>" + model.Product);
            //=========================
            return Json(model);
        }

        public ActionResult LcOpeningOtherProductPrint(long id = 0)
        {
            var v = _commonRepository.GetLcOpeningPermissionOtherProductById(id);
            return View(v);
        }

        public ActionResult AddFilesPartial()
        {
            return PartialView("_AddFiles");
        }

        public ActionResult AddOtherFilesPartial()
        {
            return PartialView("_LcOpeningPermissionOtherFile");
        }

        public ActionResult DownloadLcPermissionFile(long id = 0)
        {
            var model = _commonRepository.GetOpeningPermissionFileById(id);
            var manager = new FileManager();
            var path = manager.GetPoFeedbackFile(model.FilePath);
            var apppath = HttpContext.Server.MapPath(path);//HttpRuntime.AppDomainAppPath;
            string fileName = Path.GetFileName(path);
            var extension = Path.GetExtension(fileName);
            if (!string.IsNullOrEmpty(extension))
            {
                string ext = extension.Remove(0, 1);
                return File(apppath, "application/" + ext, fileName);
            }

            return new EmptyResult();
        }

        public ActionResult OtherProductLcOpeningApprovalDetails(long id=0)
        {
            var otherProductLc = _commonRepository.GetLcOpeningPermissionOtherProductById(id);
            ViewBag.ServiceToSalesRatiosOfRelevantModels =
                _commonRepository.GetServiceToSalesRatiosBySplitProjectName(otherProductLc.OtherProductLcForTheProject+","+
                    otherProductLc.RelevantWaltonProjects);
            ViewBag.InvoiceVsSpare =
                _commonRepository.GetTblActivatedInvoiceValueVsSpareValues(otherProductLc.OtherProductLcForTheProject + "," + otherProductLc.RelevantWaltonProjects);
            ViewBag.ServiceToSalesRatioForFocClaim =
                _commonRepository.GetOrderWiseDailyServiceToSalesRatios(otherProductLc.OtherProductLcForTheProject + "," + otherProductLc.RelevantWaltonProjects);
            return View(otherProductLc);
        }
        #endregion

        #region variant and project final closing

        public JsonResult CloseVariant(string remarks,long variantId = 0)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            var variantInfo = _commonRepository.GetOrderQuantityDetailById(variantId);
            variantInfo.VariantClosingBy = userId;
            variantInfo.VariantClosingDate = DateTime.Now;
            variantInfo.ClosingRemarks = remarks;
            variantInfo = _commonRepository.SaveUpdateProjectVariantInOrderQuantityDetail(variantInfo);
            var projectId = variantInfo.ProjectMasterId ?? 0;
            var variantInfoList = _commonRepository.GetOrderQuantityDetailByProjectId(projectId);
            var noOfVariantClosed = variantInfoList.Count(v => v.VariantClosingDate != null);
            var project = _commonRepository.GetProjectInfoByProjectId(projectId);
            if (variantInfoList.Count == noOfVariantClosed)
            {
                project.IsFinallyClosed = true;
                _commercialRepository.UpdateProject(project, userId);
            }
            //===email notification====
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<string>(new[] { "CM", "CMHEAD", "PM", "PMHEAD" }), new List<string>(new[] { "MM", "PS", "SA" }), "Variant "+variantInfo.ProjectModel+" of project " + project.ProjectName + "(Order " + project.OrderNuber + ") closed",
            "This is to inform you that, Variant "+variantInfo.ProjectModel+" of project " + project.ProjectName + "(Order " + project.OrderNuber + ") closed "+variantInfo.VariantClosingByName+".<br/>"+ "Tolal Order Quantity : <b>" + project.OrderQuantity+ ", Variant Quantity : <b>" + variantInfo.OrderQuantity);
            //=========================
            return Json(variantInfo);
        }
        #endregion

        #region SWOT analysis

        public ActionResult SwotAnalysis()
        {
            ViewBag.Projects = _commonRepository.GetSwotPendingProjects();
            return View();
        }

        public JsonResult GetSwotAnalysis(long projectId = 0,long multiplier=0)
        {
            var v = _commonRepository.GetSwotAnalysis(projectId,multiplier);
            return Json(v);
        }

        public JsonResult SaveOpportunity(string opportunity, long projectId = 0)
        {
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
            var v = _commonRepository.GetProjectInfoByProjectId(projectId);
            v.SwotAnalysisBy = userId;
            v.SwotAnalysisDate = DateTime.Now;
            v.SwotOpportunityRemarks = opportunity;
            v.ProjectStatus = "NEW";
            _commercialRepository.UpdateProject(v, userId);
            //===email notification====
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            var addedByDetails = _hardwareRepository.GetUserInfoByUserId(userId);
            var project = _commonRepository.GetProjectInfoByProjectId(projectId);
            
            mailSendFromPms.SendMail(new List<string>{"MM","PS","CM","CMHEAD","SA"}, new List<string>(new[] { "" }), "SWOT Analysis completed",
            "This is to inform you that, SWOT analysis of <b>" + project.ProjectName + " </b> has been added by <b>" + addedByDetails.UserFullName + "</b> and ready for <b>management approval</b>.");
            //=========================
            return Json("Success");
        }
        #endregion

        #region IMEI excel export for Milon vai

        public ActionResult BarcodeExcelExport()
        {
            ViewBag.ProductModels = _commonRepository.GetProductModelsFromProductMaster();
            ViewBag.Exception = Convert.ToString(TempData["exception"]);
            return View();
        }

        public ActionResult ExportExcel(DateTime startDate, DateTime endDate, string cookieValue, string projects)
        {
            try
            {
                var memoryStream = new MemoryStream();
                var zip = new ZipFile();//Install-Package DotNetZip -Version 1.15.0
                var arrProject = projects.Split(',');
                foreach (var pro in arrProject)
                {
                    var v = _commonRepository.GeTblBarCodeInvByDateRangeAndProductModel(startDate, endDate,pro);
                    //Construct DataTable
                    var dt = new DataTable();
                    //dt.Columns.Add("Model", typeof(string));
                    //dt.Columns.Add("Color", typeof(string));
                    dt.Columns.Add("Sl No", typeof(string));
                    dt.Columns.Add("Brand", typeof(string));
                    dt.Columns.Add("IMEI TAC 1", typeof(string));
                    dt.Columns.Add("IMEI TAC 2", typeof(string));
                    dt.Columns.Add("IMEI TAC 3", typeof(string));
                    dt.Columns.Add("IMEI TAC 4", typeof(string));
                    dt.Columns.Add("IMEI 1", typeof(string));
                    dt.Columns.Add("IMEI 2", typeof(string));
                    dt.Columns.Add("IMEI 3", typeof(string));
                    dt.Columns.Add("IMEI 4", typeof(string));
                    //Load data to DataTable
                    var counter = 1;
                    var serialNo = 0;
                    if (v.Count <= 7000)
                    {
                        foreach (var item in v)
                        {
                            serialNo = serialNo + 1;
                            var row = dt.NewRow();
                            //row["Model"] = item.Model;
                            row["Sl No"] = serialNo;
                            row["Brand"] = "Walton Mobile";
                            row["IMEI TAC 1"] = item.BarCode.Substring(0,8);
                            row["IMEI TAC 2"] = "";
                            row["IMEI TAC 3"] = "";
                            row["IMEI TAC 4"] = "";
                            //row["Color"] = item.Color;
                            row["IMEI 1"] = item.BarCode;
                            row["IMEI 2"] = item.BarCode2;
                            row["IMEI 3"] = "";
                            row["IMEI 4"] = "";
                            dt.Rows.Add(row);
                        }
                        //transfer data from DataTable to worksheet
                        using (var package = new ExcelPackage())
                        {
                            var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                            worksheet.Cells["A1"].LoadFromDataTable(dt, PrintHeaders: true);
                            for (var col = 1; col < dt.Columns.Count + 1; col++)
                            {
                                //if (col > 2)
                                //{
                                //    worksheet.Column(col).Style.Numberformat.Format = "#";
                                //}
                                worksheet.Column(col).AutoFit();
                            }
                            zip.AddEntry(pro + ".xlsx", package.GetAsByteArray());
                        }
                    }
                    else
                    {
                        while (counter < v.Count)
                        {
                            var filePartCounter = 1;
                            foreach (tblBarCodeInv item in v)
                            {
                                serialNo = serialNo + 1;
                                var row = dt.NewRow();
                                //row["Model"] = item.Model;
                                row["Sl No"] = serialNo;
                                row["Brand"] = "Walton Mobile";
                                row["IMEI TAC 1"] = item.BarCode.Substring(0,8);
                                row["IMEI TAC 2"] = "";
                                row["IMEI TAC 3"] = "";
                                row["IMEI TAC 4"] = "";
                                //row["Color"] = item.Color;
                                row["IMEI 1"] = item.BarCode;
                                row["IMEI 2"] = item.BarCode2;
                                row["IMEI 3"] = "";
                                row["IMEI 4"] = "";
                                dt.Rows.Add(row);
                                if (counter%7000 == 0)
                                {
                                    //transfer data from DataTable to worksheet
                                    using (var package = new ExcelPackage())
                                    {
                                        var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                                        worksheet.Cells["A1"].LoadFromDataTable(dt, PrintHeaders: true);
                                        dt.Clear();
                                        for (var col = 1; col < dt.Columns.Count + 1; col++)
                                        {
                                            //if (col > 2)
                                            //{
                                            //    worksheet.Column(col).Style.Numberformat.Format = "#";
                                            //}
                                            worksheet.Column(col).AutoFit();
                                        }
                                        zip.AddEntry(pro + " (" + filePartCounter + ").xlsx", package.GetAsByteArray());
                                        filePartCounter++;
                                    }
                                }
                                if (v.Count==counter)
                                {
                                    //transfer data from DataTable to worksheet
                                    using (var package = new ExcelPackage())
                                    {
                                        var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                                        worksheet.Cells["A1"].LoadFromDataTable(dt, PrintHeaders: false);
                                        dt.Clear();
                                        for (var col = 1; col < dt.Columns.Count + 1; col++)
                                        {
                                            //if (col > 2)
                                            //{
                                            //    worksheet.Column(col).Style.Numberformat.Format = "#";
                                            //}
                                            worksheet.Column(col).AutoFit();
                                        }
                                        zip.AddEntry(pro + " (" + filePartCounter + ").xlsx", package.GetAsByteArray());
                                    }
                                }
                                counter++;
                            }
                        }
                    }
                }
                zip.Save(memoryStream);
                //return File(package.GetAsByteArray(), XlsxContentType, "report " + startDate.ToShortDateString() + "-" + endDate.ToShortDateString() + ".xlsx");
                HttpContext.Response.Cookies.Add(new HttpCookie("imei", cookieValue));
                return File(memoryStream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Zip, "IMEI of "+ projects+ " from " + startDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture) + " to " + endDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture) + ".zip");
            }
            catch (Exception ex)
            {
                TempData["exception"] = ex;
                return RedirectToAction("BarcodeExcelExport");
            }
            
        }
        #endregion

        #region SampleSentByMeExcelExport

        public ActionResult SampleSentToAndSentByMeExcelExport(DateTime fromDate, DateTime toDate)
        {
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
            try
            {
                var v = _commonRepository.GetSampleTrackingByAddedIdAndSentIdAndDateRange(userId,fromDate, toDate);
                //Construct DataTable
                var dt = new DataTable();
                dt.Columns.Add("SampleTrackerId", typeof(long));
                dt.Columns.Add("Model", typeof(string));
                dt.Columns.Add("SampleIssuedByName", typeof(string));
                dt.Columns.Add("SampleIssuedByDept", typeof(string));
                dt.Columns.Add("SampleIssueQuantity", typeof(string));
                dt.Columns.Add("SampleIssueDate", typeof(string));
                dt.Columns.Add("SampleIssuePurpose", typeof(string));
                dt.Columns.Add("SampleSentBy", typeof(string));
                dt.Columns.Add("SampleSentByDept", typeof(string));
                dt.Columns.Add("SampleSentDate", typeof(string));
                dt.Columns.Add("SampleSentToPersonName", typeof(string));
                dt.Columns.Add("SampleSentToDept", typeof(string));
                dt.Columns.Add("SampleSentQuantity", typeof(string));
                dt.Columns.Add("SampleSenderRemarks", typeof(string));
                dt.Columns.Add("ReceivedByName", typeof(string));
                dt.Columns.Add("ReceiveDate", typeof(string));
                dt.Columns.Add("ReturnedByName", typeof(string));
                dt.Columns.Add("ReturnQuantity", typeof(string));
                dt.Columns.Add("ReturnDate", typeof(string));
                dt.Columns.Add("InventoryReturnedByName", typeof(string));
                dt.Columns.Add("InventoryReturnQuantity", typeof(string));
                dt.Columns.Add("InventoryReturnDate", typeof(string));
                //Load data to DataTable
                foreach (var item in v)
                {
                    var row = dt.NewRow();
                    row["SampleTrackerId"] = item.SampleTrackerId;
                    row["Model"] = item.Model;
                    row["SampleIssuedByName"] = item.SampleIssuedByName;
                    row["SampleIssuedByDept"] = item.SampleIssuedByDept;
                    row["SampleIssueQuantity"] = item.SampleIssueQuantity;
                    row["SampleIssueDate"] = item.SampleIssueDate;
                    row["SampleIssuePurpose"] = item.SampleIssuePurpose;
                    row["SampleSentBy"] = item.AddedByName;
                    row["SampleSentByDept"] = item.AddedByDept;
                    row["SampleSentDate"] = item.AddedDate.ToString();
                    row["SampleSentToPersonName"] = item.SampleSentToPersonName;
                    row["SampleSentToDept"] = item.SampleSentToDept;
                    row["SampleSentQuantity"] = item.NumberOfSample.ToString();
                    row["SampleSenderRemarks"] = item.Remarks;
                    row["ReceivedByName"] = item.ReceivedByName;
                    row["ReceiveDate"] = item.ReceiveDate.ToString();
                    row["ReturnedByName"] = item.ReturnedByName;
                    row["ReturnQuantity"] = item.ReturnQuantity.ToString();
                    row["ReturnDate"] = item.ReturnDate.ToString();
                    row["InventoryReturnedByName"] = item.InventoryReturnedByName;
                    row["InventoryReturnQuantity"] = item.InventoryReturnQuantity;
                    row["InventoryReturnDate"] = item.InventoryReturnDate.ToString();
                    dt.Rows.Add(row);
                }
                //transfer data from DataTable to worksheet
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                    worksheet.Cells["A1"].LoadFromDataTable(dt, PrintHeaders: true);
                    for (var col = 1; col < dt.Columns.Count + 1; col++)
                    {
                        worksheet.Column(col).AutoFit();
                    }
                    //HttpContext.Response.Cookies.Add(new HttpCookie("imei", cookieValue));
                    return File(package.GetAsByteArray(), XlsxContentType, "report " + fromDate.ToShortDateString() + "-" + toDate.ToShortDateString() + ".xlsx");
                }
            }
            catch (Exception ex)
            {
                TempData["exception"] = ex;
                return RedirectToAction("SampleTracking");
            }  
        }
        #endregion

        #region ColorWiseVariantQuantity

        public ActionResult ColorWiseVariantQuantity()
        {
            ViewBag.Variants = _commonRepository.GetAllVariantsWithOrderNumber();
            return View();
        }

        public JsonResult GetVariantQuantityAndAssignedQuantiy(long id = 0)
        {
            var quantity = _commonRepository.GetOrderQuantityDetailById(id);
            var colors = _commonRepository.GetColorWiseVariantQuantityByVariantId(id);
            var result = new {Quantity = quantity, Colors = colors};
            return Json(result);
        }

        public JsonResult SaveColorWiseVariantQuantity(string color,long quantity=0,long id=0)
        {
            long userId = Convert.ToInt64(System.Web.HttpContext.Current.User.Identity.Name == "" ? "0" : System.Web.HttpContext.Current.User.Identity.Name);
            var m=new ColorWiseVariantQuantityModel
            {
                VariantId = id,
                Color = color,
                Quantity = quantity,
                AddedBy = userId, 
                AddedDate = DateTime.Now
            };
            var v = _commonRepository.SaveColorWiseVariantQuantity(m);
            return Json(v);
        }
        #endregion

        #region

        public ActionResult ProjectWiseAssignedPm()
        {
            var v = _commonRepository.GetPmAssignModels();
            return View(v);
        }
        #endregion

        public ActionResult RejectedProjectList()
        {
            var rejects = _commonRepository.GetRejectedProjectList();
            return View(rejects);
        }

        #region China Qc Inspection Clearance Approval

        public ActionResult ChinaQcInspectionClearanceApproval()
        {
            var vmModel = new VmChinaQcInspectionsClearance();
            vmModel.ChinaQcInspectionsClearanceModels4 = _commonRepository.GetChinaQcInspectionClearanceDetails();
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

            vmModel.ChinaQcInspectionsClearanceModels1 = _commonRepository.GetChinaQcInspectionClearanceApprovalDetails();
            if (vmModel.ChinaQcInspectionsClearanceModels1 != null)
            {
                foreach (var model in vmModel.ChinaQcInspectionsClearanceModels1)
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
            return View(vmModel);
        }
        public JsonResult SaveChinaShipmentClearance(string ids, string proIds, string sStatus, string remarks)
        {
            var saveData = "";

            long ids1;
            long.TryParse(ids, out ids1);

            long prIds;
            long.TryParse(proIds, out prIds);

            if (ids1 > 0)
            {
                saveData = _commonRepository.SaveChinaShipmentClearance(ids1, prIds, sStatus, remarks);
            }
            return new JsonResult { Data = saveData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion
        #region Wpms All Projects Details

        public ActionResult WpmsAllProjects(string ProjectName, string Orders, string ProStatus, string InitialApproval)
        {
            var vmModel = new VmWpmsAllProjectDetails();

            vmModel.WpmsAllProjectDetailsModels1 = _commonRepository.GetAllModels();
            var items1 = new List<SelectListItem> { new SelectListItem { Value = "ALL", Text = "ALL" } };
            items1 = vmModel.WpmsAllProjectDetailsModels1.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectName.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.GetAllModels = items1;

           //  vmModel.ChinaQcInspectionsClearanceModels1 = _projectManagerRepository.GetProjectListForChinaQc();
           // var items1 = new List<SelectListItem> { new SelectListItem { Value = "", Text = "SELECT PROJECT" } };
           // items1 = vmModel.ChinaQcInspectionsClearanceModels1.Select(model => new SelectListItem { Text = model.ProjectName, Value = model.ProjectName.ToString(CultureInfo.InvariantCulture) }).ToList();
           //ViewBag.Projects = items1;


            vmModel.WpmsAllProjectDetailsModels2 = _commonRepository.GetProjectOrders(ProjectName);
            var items2 = new List<SelectListItem> { new SelectListItem { Value = "ALL", Text = "ALL" } };
            items2 = vmModel.WpmsAllProjectDetailsModels2.Select(model => new SelectListItem { Text = model.Orders, Value = model.Orders.ToString(CultureInfo.InvariantCulture) }).ToList();        
            ViewBag.ProjectOrderLists = items2;

            vmModel.WpmsAllProjectDetailsModels3 = _commonRepository.GetProjectSpec(ProjectName, Orders, ProStatus, InitialApproval);
            ViewBag.GetProjectSpec = vmModel.WpmsAllProjectDetailsModels3;


            vmModel.WpmsAllProjectDetailsModels4 = _commonRepository.GetAllProStatus();
            var items4 = new List<SelectListItem> { new SelectListItem { Value = "ALL", Text = "ALL" } };
            items4 = vmModel.WpmsAllProjectDetailsModels4.Select(model => new SelectListItem { Text = model.ProjectStatus, Value = model.ProjectStatus.ToString(CultureInfo.InvariantCulture) }).ToList();
            ViewBag.GetAllProStatus = items4;

            //
            List<SelectListItem> items5 = new List<SelectListItem>();
            items5.Add(new SelectListItem() { Text = "ALL", Value = "ALL" });
            items5.Add(new SelectListItem() { Text = "Action Not taken By BI", Value = "Action Not taken By BI" });
            items5.Add(new SelectListItem() { Text = "Action Not taken By Management Coordinator", Value = "Action Not taken By Management Coordinator" });
            items5.Add(new SelectListItem() { Text = "Action Not taken By MD", Value = "Action Not taken By MD" });
            items5.Add(new SelectListItem() { Text = "SWOT analysis Pending", Value = "SWOT analysis Pending" });
            ViewBag.GetAllProInitialAppoval = items5;
            //

            vmModel.ProjectName = ProjectName;
            vmModel.Orders = Orders;
            vmModel.ProjectStatus = ProStatus;
            vmModel.InitialApprovalPendings = InitialApproval;
            return View(vmModel);
        }
        public JsonResult GetProjectOrders(string projectName)
        {
            var projectOrdersList = _commonRepository.GetProjectOrders(projectName);
            var items2 = new List<SelectListItem> { new SelectListItem { Value = "ALL", Text = "ALL" } };
            items2 = projectOrdersList.Select(model => new SelectListItem { Text = model.Orders, Value = model.Orders.ToString(CultureInfo.InvariantCulture) }).ToList();
            var json = JsonConvert.SerializeObject(items2);

            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion
    }
    public class PriceEvolutionGraph
    {
        public string date { get; set; }
        public double? value { get; set; }
    }
}
