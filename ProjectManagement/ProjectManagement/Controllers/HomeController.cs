using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using ProjectManagement.DAL;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Infrastructures.Repositories;
using ProjectManagement.Models;
using ProjectManagement.ViewModels.Home;

namespace ProjectManagement.Controllers
{
    //Login And Common Routing controller 
    public class 
        HomeController : Controller
    {
        private readonly IHomeRepository _homeRepository;
        private readonly ICommonRepository _commonRepository;
        //Inject HomeRepository into HomeController
        public HomeController(HomeRepository homeRepository, CommonRepository commonRepository)
        {
            _homeRepository = homeRepository;
            _commonRepository = commonRepository;
            String useridentity = System.Web.HttpContext.Current.User.Identity.Name;
            var users = Convert.ToInt64(useridentity == "" ? "0" : useridentity);
            ViewBag.ChinaQcInspectionCount = _commonRepository.GetChinaQcInspectionCount(users);
        }

        #region CompleteLogin
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            String userIdentity = System.Web.HttpContext.Current.User.Identity.Name;
            long userId = Convert.ToInt64(userIdentity == "" ? "0" : userIdentity);
            var tuple = _homeRepository.GetUserRedirectionDetailsAfterAuthentication();
            if (!(tuple.Item1 == "Login" && tuple.Item2 == "Home"))
            {
                return RedirectToAction(tuple.Item1, tuple.Item2);
            }
            FormsAuthentication.SignOut();
            // String Name =Page.User.Identity.Name;
            // HttpContext.Current.Page.User.Identity.Name = null;
            Session.Clear();
            Session.Abandon();
            var cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "")
            {
                Expires = DateTime.Now.AddYears(-1)
            };
            Response.Cookies.Add(cookie1);
            var cookie2 = new HttpCookie("ASP.NET_SessionId", "") { Expires = DateTime.Now.AddYears(-1) };
            Response.Cookies.Add(cookie2);

            return View();

        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model)
        {
            //var dt=DateTime.ParseExact(model.CurrentDateTime, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            //if (dt.ToShortDateString() != DateTime.Now.ToShortDateString())
            //{
            //    FormsAuthentication.SignOut();
            //    Session.Clear();
            //    Session.Abandon();
            //    HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            //    cookie1.Expires = DateTime.Now.AddYears(-1);
            //    Response.Cookies.Add(cookie1);
            //    HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            //    cookie2.Expires = DateTime.Now.AddYears(-1);
            //    Response.Cookies.Add(cookie2);
            //    return RedirectToAction("Login");
            //}
            if (ModelState.IsValid)
            {
                int isAuthenticatedUser = _homeRepository.AuthorizedUserByUserNamePassword(model.username, model.password, model.remember);
                if (isAuthenticatedUser == 3)
                {
                    TempData["user"] = model;
                    return RedirectToAction("ResetPassword", "Home");
                }
            }

            return RedirectToAction("Login");

        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);
            return RedirectToAction("Login");
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult NoCookie()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Ooops()
        {
            return View();
        }
        #endregion

        //Authentication Required for usermanagement

        #region UserManagement

        [HttpGet]
        [Authorize(Roles = "SA")]
        public ActionResult CreateUser(long id = 0)
        {
            var model = new CmnUserModel();
            if (id > 0)
            {
                model = _homeRepository.GetUser(id);
            }
            model.RoleList = _homeRepository.GetAllRoles();
            return View(model);
        }
        [HttpPost]
        public ActionResult CreateUser(CmnUserModel model)
        {
            if (model.RoleList.Any())
            {
                model.RoleName = string.Join(",", model.RoleList);
            }
            if (model.CmnUserId > 0)
            {
                bool isUpdated = _homeRepository.UpdateUser(model);
                return RedirectToAction("CreateUser");
            }
            model.IsActive = true;
            model.Added = Convert.ToInt64(User.Identity.Name);
            var res = _homeRepository.CreateUser(model);
            return RedirectToAction("CreateUser");
        }

        [HttpGet]
        [Authorize(Roles = "SA")]
        public ActionResult Users()
        {
            List<CmnUserModel> userModels = _homeRepository.GetAllUser();
            return View(userModels);
        }
        [HttpGet]
        public ActionResult UpdateUser()
        {
            var manager = new FileManager();
            long userId = Convert.ToInt64(User.Identity.Name);
            ViewBag.UserInfo = _homeRepository.GetUser(userId);
            var user = new CmnUserModel();
            if (userId != 0)
            {
                user = _homeRepository.GetUser(userId);
                user.WebServerUrl = "~" + manager.GetFile(user.ProfilePictureUrl);
            }
            var vmUpdateUserProfile = new VmUpdateUserProfile();
            vmUpdateUserProfile.CmnUserModel = user;
            return View(vmUpdateUserProfile);
        }

        [HttpGet]
        public ActionResult EditUser(long id)
        {
           return RedirectToAction("CreateUser", new {id = id});
        }
        [HttpPost]
        //  [ValidateAntiForgeryToken]
        public ActionResult UpdateUser(VmUpdateUserProfile user)
        {
            try
            {
                var manager = new FileManager();
                long cmnUserId = user.CmnUserModel.CmnUserId;
                string userDirectory = user.CmnUserModel.RoleName;
                if (user.CmnUserModel.ProfilePictureFile != null)
                {
                    user.CmnUserModel.ProfilePictureUrl = manager.UserProfilePictureUpload(cmnUserId, userDirectory, user.CmnUserModel.ProfilePictureFile);

                }

                user.CmnUserModel.WebServerUrl = manager.GetFile(user.CmnUserModel.ProfilePictureUrl);
                user.CmnUserModel.Added = Convert.ToInt64(User.Identity.Name);
                user.CmnUserModel.AddedDate = DateTime.Now;
                user.CmnUserModel.Updated = Convert.ToInt64(User.Identity.Name);
                user.CmnUserModel.UpdatedDate = DateTime.Now;

                _homeRepository.UpdateUser(user.CmnUserModel);

                user.CmnUserModel.IsUserInfoUpdated = true;
                user.CmnUserModel.RoleList = _homeRepository.GetAllRoles();
                return RedirectToAction("UpdateUser");
            }
            catch (Exception exception)
            {
            }
            ModelState.Clear();
            var model = new VmUpdateUserProfile();
            model.CmnUserModel.RoleList = _homeRepository.GetAllRoles();
            return RedirectToAction("UpdateUser");

        }
        [HttpPost]
        //   [ValidateAntiForgeryToken]
        public ActionResult ChangeUserPassword(VmUpdateUserProfile model)
        {
            try
            {
                var cmnUserModel = new CmnUserModel
                {
                    CmnUserId = Convert.ToInt64(User.Identity.Name),
                    Password = model.NewPassword,
                    IsPasswordUpdated = true,
                    Updated = Convert.ToInt64(User.Identity.Name),
                    UpdatedDate = DateTime.Now
                };
                var res = _homeRepository.ChagePassword(cmnUserModel);
                if (res)
                {
                    ModelState.Clear();
                    return View("UpdateUser");
                }
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, "There is something wrong with your password");
                return View("UpdateUser");
            }
            //ValidationSummery
            ModelState.AddModelError(string.Empty, "There is something wrong with your password");
            return View("UpdateUser");
        }


        public JsonResult CheckUserName(String userName)
        {
            string result = _homeRepository.CheckUserNameExist(userName);
            if (result == string.Empty) return Json(true, JsonRequestBehavior.AllowGet);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckPasswordChange(String oldPassword)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            string result = _homeRepository.CheckOldPassword(userId, oldPassword);
            if (result == string.Empty) return Json(true, JsonRequestBehavior.AllowGet);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public FileContentResult UserPhotos(long uuId = 0)
        {
            if (User.Identity.IsAuthenticated)
            {
                String userId = User.Identity.Name;
                string fileName;
                long uId;
                long.TryParse(userId, out uId);
                if (uuId > 0) uId = uuId;
                FileContentResult result = _commonRepository.GetProfilePicture(uId);
                return result;
            }
            else
            {
                string fileName = HttpContext.Server.MapPath(@"~/assets/layouts/layout4/img/av.png");
                var fileInfo = new FileInfo(fileName);
                long imageFileLength = fileInfo.Length;
                var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                var br = new BinaryReader(fs);
                byte[] imageData = br.ReadBytes((int)imageFileLength);
                return File(imageData, "image/png");
            }
        }

        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            var user = TempData["user"];
            if (user  == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View(user);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ResetPassword(LoginViewModel model)
        {
            var user = _homeRepository.GetUserByUserName(model.username);
            user.Password = model.password;
            user.LastPasswordUpdateDate = DateTime.Now;
            _homeRepository.ResetPassword(user);
            return RedirectToAction("Login", "Home");
        }

        #endregion

        #region Notification

        public JsonResult GetNotification()
        {
            long id = 0;
            var contextName = HttpContext.User.Identity.Name;
            long.TryParse(contextName, out id);
            using (var dbEntities = new CellPhoneProjectEntities())
            {
                List<Notification> notifications = dbEntities.Notifications.Where(i => i.ViewerId == id).OrderByDescending(i => i.Added).ToList();
                List<NotificationModel> notificationModels = GenericMapper<Notification, NotificationModel>.GetDestinationList(notifications);
                if (notificationModels.Any())
                {
                    foreach (var model in notificationModels)
                    {
                        if (model !=null && model.Added !=null)
                        {
                            var span = (TimeSpan)(DateTime.Now - model.Added);
                            model.NotificationTime = CommonConversion.ToPrettyFormat(span);
                        }
                    }
                }
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(notificationModels);
                return new JsonResult { Data = jsonData, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public JsonResult ViewNotification(long id = 0)
        {
            if (id > 0)
            {
                using (var dbEntities = new CellPhoneProjectEntities())
                {
                    try
                    {
                        var dbNotification = dbEntities.Notifications.FirstOrDefault(i => i.Id == id);
                        if (dbNotification != null)
                        {
                            dbNotification.IsViewd = true;
                            dbEntities.Entry(dbNotification).State = EntityState.Modified;
                            dbEntities.SaveChanges();
                        }
                    }

                    catch (Exception)
                    {
                        return new JsonResult { Data = false, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                }
                return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return new JsonResult { Data = false, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult ViewAllNotification(long id)
        {
            if (id > 0)
            {
                using (var dbEntities = new CellPhoneProjectEntities())
                {
                    try
                    {
                        var dbNotification = dbEntities.Notifications.Where(i => i.ViewerId == id).ToList();
                        if (dbNotification.Any())
                        {
                            foreach (var notification in dbNotification)
                            {
                                notification.IsViewd = true;
                                dbEntities.Entry(notification).State = EntityState.Modified;
                            }
                            
                            dbEntities.SaveChanges();
                        }
                    }

                    catch (Exception)
                    {
                        return new JsonResult { Data = false, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                }
                return new JsonResult { Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            return new JsonResult { Data = false, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        #endregion


        public ActionResult Home()
        {
            return View();
        }
    }
}