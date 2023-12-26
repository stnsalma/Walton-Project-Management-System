using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;

namespace ProjectManagement.Infrastructures.Helper
{
    public class MemberAuthorization : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            try
            {
                //if (new HttpRequestWrapper(System.Web.HttpContext.Current.Request).IsAjaxRequest())
                //{
                //    return true;
                //}

                //var controller = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
                //var action = httpContext.Request.RequestContext.RouteData.Values["action"].ToString();
                //if (action.ToLower() == "login" && controller.ToLower() == "auth") return true;
                //if (action.ToLower() == "logoff" && controller.ToLower() == "auth") return true;
                //if (action.ToLower() == "changesuccess" && controller.ToLower() == "auth") return true;


                var user = HttpContext.Current.User.Identity.Name;
                return !string.IsNullOrWhiteSpace(user);


                //if (action.ToLower() == "changepassword" && controller.ToLower() == "auth") return true;
                //if (action.ToLower() == "authfailed" && controller.ToLower() == "auth") return true;
                //if (action.ToLower() == "index" && controller.ToLower() == "dashboard") return true;
                //if (httpContext.Request.RequestContext.HttpContext.Session["permissions"] == null) return false;
                //var filterContext = new AuthorizationContext();

                //if(filterContext.IsChildAction)

                //if (Authorize(permissions, controller, action))
                //    return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //private static bool Authorize(List<PermissionModel> permissions, string controller, string action)
        //{
        //    int index = -1;
        //    if (!permissions.Any()) return false;
        //    foreach (var permissionModel in permissions)
        //    {
        //        index = permissionModel.SubMenus.FindIndex(i =>
        //            string.Equals(i.ControllerName, controller, StringComparison.CurrentCultureIgnoreCase) && string.Equals(i.ActionName, action, StringComparison.CurrentCultureIgnoreCase));
        //        if (index >= 0) break;
        //    }

        //    return index >= 0;
        //}

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            try
            {
                var user = HttpContext.Current.User.Identity.Name;
                //return !string.IsNullOrWhiteSpace(user);

                if (string.IsNullOrWhiteSpace(user))
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary  
                        {  
                            { "controller", "Home" },  
                            { "action", "Logout" }  
                        });
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary  
                        {  
                            { "controller", "Auth" },  
                            { "action", "AuthFailed" }  
                        });
                }


                //var actionName = filterContext.ActionDescriptor.ActionName.ToLower();
                //var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();
                //if (actionName == "LogOff" && controllerName == "Account")
                //{
                //    filterContext.Result = new RedirectResult("~/Account/LogOff");
                //}
                //else if (actionName == "Login" && controllerName == "Account")
                //{
                //    filterContext.Result = new RedirectResult("~/Account/Login");
                //}

                //else if (actionName == "authfailed" && controllerName == "Account")
                //{
                //    filterContext.Result = new RedirectResult("~/Account/AuthFailed");
                //}
                //else if (actionName == "lostpassword" && controllerName == "login")
                //{
                //    filterContext.Result = new RedirectResult("~/Account/LostPassword");
                //}
                //else
                //{
                //    if (filterContext.RequestContext.HttpContext.Session == null) return;
                //    var logInInfo = (User)filterContext.RequestContext.HttpContext.Session["user"];
                //    //if (filterContext.HttpContext.Request.IsAjaxRequest())
                //    //{
                //    //    var response = new ReturnArgs { };
                //    //    if (logInInfo == null)
                //    //    {
                //    //        response.Response = new ResponseMessage
                //    //        {
                //    //            MessageType = (int)MesssageType.UnAuthorized,
                //    //            Message = "You are not logged in"
                //    //        };
                //    //        filterContext.Result = logInInfo == null ? new RedirectResult("~/Account/Login") : new RedirectResult("~/Account/AuthFailed");
                //    //    }
                //    //    else
                //    //    {
                //    //        //response.Response = new ResponseMessage
                //    //        //{
                //    //        //    MessageType = (int)MesssageType.UnAuthorized,
                //    //        //    Message = "You don not have the permission for this operation."
                //    //        //};
                //    //        //filterContext.Result = new JsonResult
                //    //        //{
                //    //        //    Data = response,
                //    //        //    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                //    //        //}; ;
                //    //        base.HandleUnauthorizedRequest(filterContext);
                //    //    }
                //    //}
                //    //else
                //    //    filterContext.Result = logInInfo == null ? new RedirectResult("~/Account/Login") : new RedirectResult("~/Account/AuthFailed");
                //    var httpContext = filterContext.HttpContext;
                //    var request = httpContext.Request;
                //    var response = httpContext.Response;
                //    if (request.IsAjaxRequest())
                //    {

                //        response.StatusCode = (int)HttpStatusCode.Accepted;
                //        response.SuppressFormsAuthenticationRedirect = true;
                //        response.End();

                //        filterContext.Result = new JsonResult
                //        {
                //            Data = response,
                //            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                //        };
                //        filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.Url.AbsoluteUri);
                //    }
                //    else
                //        base.HandleUnauthorizedRequest(filterContext);
                //}
            }
            catch (Exception ex)
            {
                //Logger.SaveLogger(ex.Message, "AuthorizeCore");
                //throw ex;
            }
        }
    }
}