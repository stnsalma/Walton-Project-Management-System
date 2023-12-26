using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Ajax.Utilities;
using Microsoft.Office.Interop.Excel;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Infrastructures.Repositories;
using ProjectManagement.Models;
using System.Web.Script.Serialization;

namespace ProjectManagement.Controllers
{
    [Authorize]
    public class GeneralIncidentController : Controller
    {
        private CellPhoneProjectEntities _dbeEntities;
        private CellPhoneProjectEntities db = new CellPhoneProjectEntities();
        private IGeneralIncidentRepository _repository;
        private readonly HardwareRepository _hardwareRepository;

        public GeneralIncidentController(GeneralIncidentRepository repository)
        {
            _repository = repository;
            _hardwareRepository = new HardwareRepository();
        }
        // GET: GeneralIncident
        public ActionResult CreateGeneralIncident()
        {
            string roleDescription = "";
            long userId = Convert.ToInt64(User.Identity.Name);
            var roles = _repository.GetAllRoleModels();
            List<SelectListItem>items = roles.Select(role => new SelectListItem {Text = role.RoleDescription, Value = role.RoleName}).ToList();
            ViewBag.Roles = items;
            ViewBag.categories = _repository.GetaGeneralIncidentCategoryModels();
            var incidents = _repository.GetGeneralIncidentByAddedBy(userId);
            for (var i = 0; i < incidents.Count; i++)
            {
                //string[] rolename = incidents[i].GeneralIncidentCategories.Split(',');
                //for (var j = 0; j < rolename.Length; j++)
                //{
                //    string role = rolename[j];
                //    string comma = ",";
                //    roleDescription = roleDescription + _repository.GetRoleDescriptionByRoleName(role) + (j == rolename.Length - 1 ? "" : comma);
                //}
                roleDescription = _repository.GetRoleDescriptionByRoleName(incidents[i].RefferedRole);
                incidents[i].RefferedRole = roleDescription;
            }
            ViewBag.incidents = incidents;
            ViewBag.models = _repository.GetModelses();
            ViewBag.issues = _repository.GetWsmsIssuesModels();
            return View();
        }

        [HttpPost]
        public ActionResult CreateGeneralIncident(GeneralIncidentModel model)
        {
            var manager = new FileManager();
            var moduleDirectory = "GeneralIncident";
            var userDirectory = "CMN";
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            model.AddedBy = userId;
            model.AddedDate = DateTime.Now;
            model.Status = "NEW";
            model.FileUploadPath = manager.IncidentUpload(userDirectory, moduleDirectory,
                model.FileUpload);
            _repository.SaveGeneralIncidentCategory(model);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<string>(new[] {model.RefferedRole}),
                new List<string>(new[] {""}), "New Incident Created", "This is to inform you that incident '"+model.GeneralIncidentTitle+"' has created by "+ViewBag.UserInfo.UserFullName);
            return RedirectToAction("CreateGeneralIncident");
        }

        public ActionResult GeneralInciedntForAssign()
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            var incidents = _repository.GetIncidentsForAssign();
            var rolewiseincidents = incidents.Where(t => User.IsInRole(t.RefferedRole)).ToList();
            for (var i = 0; i < rolewiseincidents.Count; i++)
            {
                rolewiseincidents[i].RoleDescription = _repository.GetRoleDescriptionByRoleName(rolewiseincidents[i].RefferedRole);
            }
            ViewBag.Roles = _repository.GetAllRoleModels();
            ViewBag.assign = _repository.GetAssignedIncidentForMe(userId);
            return View(rolewiseincidents);
        }

        public ActionResult GeneralIncidentDetails(long incidentId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            var incident = _repository.GetGeneralIncidentByIncidentId(incidentId);
            incident.RoleDescription = _repository.GetRoleDescriptionByRoleName(incident.RefferedRole);
            ViewBag.incident = incident;
            ViewBag.incidentlog = _repository.GetGeneralIncidentLogModels(incidentId);
            ViewBag.Roles = _repository.GetAllRoleModels();
            ViewBag.users = _repository.GetUserModels(userId);
            ViewBag.solution = _repository.GetIncidentSolutionByIncidentId(incidentId);
            ViewBag.assigned = _repository.GetGeneralIncidentAssignModels(incidentId);
            return View();
        }

        public JsonResult PostSolution(string solution, string type, long incidentid = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            var sol = new GeneralIncidentSolutionModel
            {
                GeneralIncidentId = incidentid,
                Solution = solution,
                AddedBy = userId,
                AddedDate = DateTime.Now,
                DenyDate = type == "deny" ? DateTime.Now : (DateTime?)null
            };
            _repository.SaveGeneralIncidentSolutionModel(sol);
            var model = _repository.GetGeneralIncidentByIncidentId(incidentid);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<string>(new[] { model.RefferedRole }),
                new List<string>(new[] { "" }), "Solution suggested", "This is to inform you that solution for incident '" + model.GeneralIncidentTitle + "' has been given by " + ViewBag.UserInfo.UserFullName);
            return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult ForwardIncident(string remark, string forwardrole, long incidentId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            _repository.ForwardIncident(remark, forwardrole, userId, incidentId);
            var model = _repository.GetGeneralIncidentByIncidentId(incidentId);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<string>(new[] { forwardrole }),
                new List<string>(new[] { "" }), "Incident Forwarded", "This is to inform you that incident '" + model.GeneralIncidentTitle + "' forwarded by " + ViewBag.UserInfo.UserFullName);
            return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //[Authorize(Roles = "MM,PS")]
        public ActionResult GeneralIncidentDisclose()
        {
            _dbeEntities=new CellPhoneProjectEntities();
            long userId = Convert.ToInt64(User.Identity.Name);
            var cmnUser = _dbeEntities.CmnUsers.FirstOrDefault(x => x.CmnUserId == userId);
            var cmnRoles = _dbeEntities.CmnRoles.FirstOrDefault(x => x.RoleName == cmnUser.RoleName);
            var counter = new GeneralIncidentDashboardModel();
            var incidents = new List<GeneralIncidentModel>();
            if (User.IsInRole("PS") || User.IsInRole("MM"))
            {
                counter = _repository.GetGeneralIncidentDashboardCounter();
                counter.SolutionPending = counter.TotalIncidents - counter.DisclosePending - counter.Disclosed;
                incidents = _repository.GetGeneralIncidentForDisclose();
            }
            else
            {
                if (cmnRoles != null && cmnRoles.IsHead == true)
                {
                    counter = _repository.GetGeneralIncidentDashboardCounterByReferredRole(cmnUser.RoleName);
                    counter.SolutionPending = counter.TotalIncidents - counter.DisclosePending - counter.Disclosed;
                    incidents = _repository.GetGeneralIncidentModelsByRole(cmnUser.RoleName);
                }
            }
            
            ViewBag.counter = counter;
            return View(incidents);
        }

        public JsonResult DiscloseIncident(string remark, long incidentid = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            _repository.DiscloseIncident(remark, incidentid, userId);
            var model = _repository.GetGeneralIncidentByIncidentId(incidentid);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<string>(new[] { model.RefferedRole }),
                new List<string>(new[] { "" }), "Incident Disclosed", "This is to inform you that incident '" + model.GeneralIncidentTitle + "' disclosed by " + ViewBag.UserInfo.UserFullName);
            return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult ReassignIncident(string remark, string reassigndrole, long incidentId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            _repository.ReassignIncident(remark, reassigndrole, userId, incidentId);
            return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult SolutionPendingIncident()
        {
            List<GeneralIncidentModel> model = _repository.GetSolutionPendingIncidents();
            for (var i = 0; i < model.Count; i++)
            {
                model[i].RoleDescription = _repository.GetRoleDescriptionByRoleName(model[i].RefferedRole);
            }
            return View(model);
        }

        public ActionResult IncidentSolvedDeniedByMe()
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.incidents = _repository.GetIncidentSolvedByMe(userId);
            return View();
        }

        public JsonResult AssignIncident(string remark, long incidentId = 0, long id = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            var assign = new GeneralIncidentAssignModel
            {
                GeneralIncidentId = incidentId,
                AssignedBy = userId,
                AssignedTo = id,
                AssignRemarks = remark,
                AssignDate = DateTime.Now,
                Status = "NEW"
            };
            _repository.SaveGeneralIncidentAssignModel(assign);
            var model = _repository.GetGeneralIncidentByIncidentId(incidentId);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<long>(new[] { id }),
                new List<string>(new[] { "" }), "Incident Assigned", "You have been assigned for incident - '" + model.GeneralIncidentTitle + "' by " + ViewBag.UserInfo.UserFullName);
            return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult SubmitIncident(string remark, long incidentId = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _hardwareRepository.GetUserInfoByUserId(userId);
            _repository.UpdateGeneralIncidentModel(remark, incidentId, userId);
            var model = _repository.GetGeneralIncidentByIncidentId(incidentId);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<string>(new[] { "MM","PS" }),
                new List<string>(new[] { "" }), "Incident Submitted", "Incident - '" + model.GeneralIncidentTitle + "' submitted by " + ViewBag.UserInfo.UserFullName);
            return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult DisclosedGeneralIncidents()
        {
            var model = _repository.GetDisclosedIncidents();
            for (var i = 0; i < model.Count; i++)
            {
                model[i].RoleDescription = _repository.GetRoleDescriptionByRoleName(model[i].RefferedRole);
            }
            return View(model);
        }

        public ActionResult DownloadFile(long incidentid = 0)
        {
            var model = _repository.GetGeneralIncidentByIncidentId(incidentid);
            var manager = new FileManager();
            var path = manager.GetFile(model.FileUploadPath);
            var apppath = HttpContext.Server.MapPath(path);//HttpRuntime.AppDomainAppPath;
            string fileName = Path.GetFileName(path);
            var extension = Path.GetExtension(fileName);
            if (extension != null)
            {
                string ext = extension.Remove(0,1);
                return File(apppath, "application/" + ext, fileName);
            }

            return new EmptyResult();
        }
    }
}