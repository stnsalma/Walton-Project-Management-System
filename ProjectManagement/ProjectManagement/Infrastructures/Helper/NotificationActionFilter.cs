using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagement.DAL.DbModel;

namespace ProjectManagement.Infrastructures.Helper
{

    public class NotificationActionFilter : ActionFilterAttribute
    {
        public string ReceiverRoles { get; set; }
        public string MessageHeader { get; set; }
        public static bool In<T>(T source, params T[] list)
        {
            if (null == source) throw new ArgumentNullException("source");
            return list.Contains(source);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {

            var customMassage = string.Empty;

            long senderid;
            var identity = HttpContext.Current.User.Identity.Name;
            long.TryParse(identity, out senderid);
            var receiverRoles = ReceiverRoles;
            var roleList = new string[1000];
            if (receiverRoles != null)
            {
                roleList = receiverRoles.Split(',');
                roleList = roleList.Distinct().ToArray();
            }
            if (senderid > 0 && roleList.Any())
            {
                using (var dbEntities = new CellPhoneProjectEntities())
                {
                    var cmnUser = dbEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == senderid);
                    if (cmnUser != null)
                    {
                        NotificationObject notificationObject = filterContext.Controller.ViewBag.ControllerVariable;
                        if (In(cmnUser.RoleName, new[] { "CM", "CMBTRC", "CMHEAD" }) && !string.Equals(MessageHeader, "Incentive"))
                        {
                         
                            if (notificationObject != null)
                            {
                                var projectMaster = dbEntities.ProjectMasters.FirstOrDefault(i => i.ProjectMasterId == notificationObject.ProjectId);
                                if (projectMaster != null)
                                {
                                    var projectName = projectMaster.ProjectName;
                                    string userName = cmnUser.UserFullName;
                                    if (!string.IsNullOrWhiteSpace(notificationObject.MessageFromController))
                                    {
                                        customMassage = notificationObject.MessageFromController + " by " + userName + ", Project Name: " + projectName;
                                    }
                                    else
                                    {
                                        if (notificationObject.ModelId > 0)
                                        {
                                            customMassage = MessageHeader + " has been updated by " + userName + ", Project Name: " + projectName;
                                        }
                                        else if (notificationObject.ModelId == 0)
                                        {
                                            customMassage = MessageHeader + " has been created by " + userName + ", Project Name: " + projectName;
                                        }

                                    }
                                    foreach (var s in roleList)
                                    {
                                        var viewerIds = dbEntities.CmnUsers.Where(i => i.RoleName == s && i.CmnUserId != senderid && i.IsActive).Select(i => i.CmnUserId).ToList();
                                        if (viewerIds.Any())
                                        {
                                            foreach (var viewerId in viewerIds)
                                            {
                                                var notification = new Notification
                                                {
                                                    IsViewd = false,
                                                    Role = s,
                                                    Message = customMassage,
                                                    AdditionalMessage = notificationObject.AdditionalInformation,
                                                    ViewerId = (int?)viewerId,
                                                    Added = DateTime.Now,
                                                    ProjectMasterId = notificationObject.ProjectId,
                                                    AddedBy = senderid
                                                };
                                                dbEntities.Notifications.Add(notification);
                                            }
                                        }

                                    }
                                    if (string.Equals(MessageHeader, "Purchase Order"))
                                    {
                                        var viewerIds = dbEntities.CmnUsers.Where(i => i.RoleName == "PMHEAD" && i.CmnUserId != senderid && i.IsActive).Select(i => i.CmnUserId).ToList();
                                        if (viewerIds.Any())
                                        {
                                            foreach (var viewerId in viewerIds)
                                            {
                                                var notification = new Notification
                                                {
                                                    IsViewd = false,
                                                    Role = "PMHEAD",
                                                    Message = "You have a new project to assign. Project name is : " + projectName,
                                                    AdditionalMessage = notificationObject.AdditionalInformation,
                                                    ViewerId = (int?)viewerId,
                                                    Added = DateTime.Now,
                                                    ProjectMasterId = notificationObject.ProjectId,
                                                    AddedBy = senderid
                                                };
                                                dbEntities.Notifications.Add(notification);
                                            }
                                        }
                                    }
                                    
                                }
                                dbEntities.SaveChanges();

                            }
                        }
                        else if (In(cmnUser.RoleName, new[] { "CMHEAD" }) && string.Equals(MessageHeader, "Incentive"))
                        {
                            if (string.Equals(MessageHeader, "Incentive"))
                            {
                                foreach (var roleL in roleList)
                                {
                                    var viewerIds = dbEntities.CmnUsers.Where(i => i.RoleName.Contains(roleL) && i.CmnUserId != senderid && i.IsActive).Select(i => i.CmnUserId).ToList();
                                    if (viewerIds.Any())
                                    {
                                        foreach (var viewerId in viewerIds)
                                        {
                                            var notification = new Notification
                                            {
                                                IsViewd = false,
                                                Role = roleL,
                                                Message = "This month's incentive for commercial team has been generated by " + cmnUser.UserFullName,
                                                AdditionalMessage = notificationObject.AdditionalInformation,
                                                ViewerId = (int?)viewerId,
                                                Added = DateTime.Now,
                                                //ProjectMasterId = notificationObject.ProjectId,
                                                AddedBy = senderid
                                            };
                                            dbEntities.Notifications.Add(notification);
                                        }
                                    }
                                }
                                dbEntities.SaveChanges();
                            }
                        }

                        else if (In(cmnUser.RoleName, new[] { "PMHEAD" }) && string.Equals(MessageHeader, "Incentive"))
                        {
                            if (string.Equals(MessageHeader, "Incentive"))
                            {
                                foreach (var roleL in roleList)
                                {
                                    var viewerIds = dbEntities.CmnUsers.Where(i => i.RoleName.Contains(roleL) && i.CmnUserId != senderid && i.IsActive).Select(i => i.CmnUserId).ToList();
                                    if (viewerIds.Any())
                                    {
                                        foreach (var viewerId in viewerIds)
                                        {
                                            var notification = new Notification
                                            {
                                                IsViewd = false,
                                                Role = roleL,
                                                Message = "This month's incentive for project manager team has been generated by " + cmnUser.UserFullName,
                                                AdditionalMessage = notificationObject.AdditionalInformation,
                                                ViewerId = (int?)viewerId,
                                                Added = DateTime.Now,
                                                //ProjectMasterId = notificationObject.ProjectId,
                                                AddedBy = senderid
                                            };
                                            dbEntities.Notifications.Add(notification);
                                        }
                                    }
                                }
                                dbEntities.SaveChanges();
                            }
                        }
                    }
                    else if (In(cmnUser.RoleName, new[] { "HW", "HWHEAD", "PM", "PMHEAD", "QC", "QCHEAD", "MM" }))
                    {
                        NotificationObject notificationObject = filterContext.Controller.ViewBag.ControllerVariable;
                        if (notificationObject != null)
                        {
                            long pid = notificationObject.ProjectId;
                            string[] tUsers = notificationObject.ToUser != null ? notificationObject.ToUser.TrimEnd(',').Split(',') : new string[0];
                            var projectMaster = dbEntities.ProjectMasters.FirstOrDefault(x => x.ProjectMasterId == pid);
                            if (tUsers.Any())
                            {
                                foreach (var tUser in tUsers)
                                {
                                    long tId = 0;
                                    long.TryParse(tUser, out tId);
                                    var dbUser = dbEntities.CmnUsers.FirstOrDefault(x => x.CmnUserId == tId);
                                    if (projectMaster != null)
                                    {

                                        string notifyerName = cmnUser.UserFullName;
                                        string projectName = projectMaster.ProjectName;
                                        string customNotificationMessage = string.Empty;
                                        if (tId > 0 && dbUser != null)
                                        {
                                            customNotificationMessage = dbUser.UserFullName + " has been " + notificationObject.Message + ", by " + notifyerName + ", Project Name: " + projectName;
                                        }
                                        else if (tId < 0)
                                        {
                                            customNotificationMessage = notifyerName + " " + notificationObject.Message + ", Project Name: " + projectName;
                                        }
                                        foreach (var s in roleList)
                                        {
                                            var viewerIds = dbEntities.CmnUsers.Where(i => i.RoleName == s && i.CmnUserId != senderid && i.IsActive).Select(i => i.CmnUserId).ToList();
                                            if (viewerIds.Any())
                                            {
                                                bool isChanged = false;
                                                foreach (var viewerId in viewerIds)
                                                {
                                                    if (viewerId == tId)
                                                    {
                                                        isChanged = true;
                                                        customNotificationMessage = "You've been " + notificationObject.Message + ", by " + notifyerName + ", Project Name: " + projectName;
                                                    }

                                                    var notification = new Notification
                                                    {
                                                        IsViewd = false,
                                                        Role = s,
                                                        Message = customNotificationMessage,
                                                        AdditionalMessage = notificationObject.AdditionalMessage,
                                                        ViewerId = (int?)viewerId,
                                                        Added = DateTime.Now,
                                                        ProjectMasterId = notificationObject.ProjectId,
                                                        AddedBy = senderid
                                                    };
                                                    dbEntities.Notifications.Add(notification);
                                                    if (isChanged && tId > 0)
                                                    {
                                                        isChanged = false;
                                                        customNotificationMessage = dbUser.UserFullName + " has been " + notificationObject.Message + ", by " + notifyerName + ", Project Name: " + projectName;
                                                    }
                                                    else if (isChanged && tId < 0)
                                                    {
                                                        isChanged = false;
                                                        customNotificationMessage = notifyerName + " " + notificationObject.Message + ", Project Name: " + projectName;
                                                    }

                                                }
                                            }
                                        }
                                        dbEntities.SaveChanges();
                                    }
                                    else if (notificationObject.ProjectId < 0)
                                    {

                                    }
                                }
                            }

                        }

                    }
                    else if (In(cmnUser.RoleName, new[] { "QC, QCHEAD" })) { }
                    else if (In(cmnUser.RoleName, new[] { "PM, PMHEAD" })) { }
                    else if (In(cmnUser.RoleName, new[] { "MM" })) { }
                }
            }
        }
    }
}
