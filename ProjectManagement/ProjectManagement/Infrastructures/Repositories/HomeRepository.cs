using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using ProjectManagement.DAL;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Models;

namespace ProjectManagement.Infrastructures.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly CellPhoneProjectEntities _dbContext;
        public HomeRepository()
        {
            _dbContext = new CellPhoneProjectEntities();
            _dbContext.Configuration.LazyLoadingEnabled = false;
        }
        private string Encrypt(string password)
        {
            string EncrptKey = "@dval3L4cT@H";
            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byKey = System.Text.Encoding.UTF8.GetBytes(EncrptKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(password);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        private string Decrypt(string encryptedPassword)
        {
            encryptedPassword = encryptedPassword.Replace(" ", "+");
            string DecryptKey = "@dval3L4cT@H";
            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byte[] inputByteArray = new byte[encryptedPassword.Length];

            byKey = Encoding.UTF8.GetBytes(DecryptKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByteArray = Convert.FromBase64String(encryptedPassword.Replace(" ", "+"));
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            var encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }

        ///<summary>
        ///This for userlogin After User login he will be redirected to his respective page .IF not then his login page
        ///</summary>
        public Tuple<string, string> GetUserRedirectionDetailsAfterAuthentication()
        {
            List<String> rolesList = new List<string>();
            List<String> roleNames = _dbContext.CmnRoles.Select(x => x.RoleName).ToList();

            String controllerName = "Home";
            String actionName = "Login";
            foreach (var name in roleNames)
            {
                if (HttpContext.Current.User.IsInRole(name))
                    rolesList.Add(name);
            }
            if (rolesList.Count != 0)
            {
                if (HttpContext.Current.User.IsInRole(("MM")))
                {
                    actionName = "Index";
                    controllerName = "Management";

                }
                else if (HttpContext.Current.User.IsInRole(("ACCNT")))
                {
                    actionName = "CmIncentiveSheet";
                    controllerName = "Commercial";

                }
                else if (HttpContext.Current.User.IsInRole("CM") || HttpContext.Current.User.IsInRole("CMHEAD"))
                {
                    actionName = "Index";
                    controllerName = "Commercial";

                }
                else if (HttpContext.Current.User.IsInRole(("CMBTRC")))
                {
                    actionName = "BtrcNocRequests";
                    controllerName = "Commercial";

                }
                else if (HttpContext.Current.User.IsInRole(("PMHEAD")))
                {
                    actionName = "HeadOfProject";
                    controllerName = "ProjectManager";

                }
                else if (HttpContext.Current.User.IsInRole(("PM")))
                {
                    actionName = "ProjectManagerOfProjectDashboard";
                    controllerName = "ProjectManager";

                }
                //else if (HttpContext.Current.User.IsInRole(("CMBTRC")))
                //{
                //    actionName = "ProjectManagerOfProjectDashboard";
                //    controllerName = "ProjectManager";

                //}
                else if (HttpContext.Current.User.IsInRole(("HWHEAD")))
                {
                    actionName = "HwQcInchargeDashboard";
                    controllerName = "Hardware";

                }
                else if (HttpContext.Current.User.IsInRole(("HW")))
                {
                    actionName = "HwQcDashboard";
                    controllerName = "Hardware";

                }
                else if (HttpContext.Current.User.IsInRole(("QCHEAD")))
                {
                    actionName = "SwQcInchargeDashboard";
                    controllerName = "Software";

                }
                else if (HttpContext.Current.User.IsInRole(("QC")))
                {
                    actionName = "SwQcDashboard";
                    controllerName = "Software";

                }
                else if (HttpContext.Current.User.IsInRole(("SA")))
                {
                    actionName = "CreateUser";
                    controllerName = "Home";

                }
                else if (HttpContext.Current.User.IsInRole("PS"))
                {
                    actionName = "Index";
                    controllerName = "Management";
                }
                else if (HttpContext.Current.User.IsInRole("MKT") || HttpContext.Current.User.IsInRole("MKTHEAD"))
                {
                    actionName = "PostProductionIssue";
                    controllerName = "Common";
                }
                else if (HttpContext.Current.User.IsInRole("PRD"))
                {
                    actionName = "Index";
                    controllerName = "Production";
                }
                else if (HttpContext.Current.User.IsInRole("SPR") || HttpContext.Current.User.IsInRole("SPRHEAD"))
                {
                    actionName = "SpareOrder";
                    controllerName = "Spare";
                }
                else if (HttpContext.Current.User.IsInRole("CPSD") || HttpContext.Current.User.IsInRole("CPSDHEAD"))
                {
                    actionName = "ServiceToSalesRatioMonitor";
                    controllerName = "Cpsd";
                }
                else if (HttpContext.Current.User.IsInRole("MKTHEAD") || HttpContext.Current.User.IsInRole("MKT") || HttpContext.Current.User.IsInRole("ADMIN") || HttpContext.Current.User.IsInRole("ADMHEAD"))
                {
                    actionName = "CreateGeneralIncident";
                    controllerName = "GeneralIncident";
                }
                else if (HttpContext.Current.User.IsInRole("WAR") || HttpContext.Current.User.IsInRole("WARHEAD"))
                {
                    actionName = "CreateGeneralIncident";
                    controllerName = "GeneralIncident";
                }
                else if (HttpContext.Current.User.IsInRole("ASPM") || HttpContext.Current.User.IsInRole("ASPMHEAD"))
                {
                    actionName = "CreateGeneralIncident";
                    controllerName = "GeneralIncident";
                }
                else if (HttpContext.Current.User.IsInRole("BDIQC") || HttpContext.Current.User.IsInRole("BDIQCHEAD"))
                {
                    actionName = "BdIqc";
                    controllerName = "Iqc";
                }
                else if (HttpContext.Current.User.IsInRole("PRC") || HttpContext.Current.User.IsInRole("PRCHEAD"))
                {
                    actionName = "ProjectPoFeedback";
                    controllerName = "Common";
                }
                else if (HttpContext.Current.User.IsInRole("FIQC") || HttpContext.Current.User.IsInRole("FIQCHEAD"))
                {
                    actionName = "ForeignIqc";
                    controllerName = "Iqc";
                }
                else if (HttpContext.Current.User.IsInRole("INV") || HttpContext.Current.User.IsInRole("INVHEAD"))
                {
                    actionName = "RawMaterialInspectionList";
                    controllerName = "ProjectManager";
                }
                else if (HttpContext.Current.User.IsInRole("ACCNT") || HttpContext.Current.User.IsInRole("ACCNTHEAD"))
                {
                    actionName = "LcOpeningApproval";
                    controllerName = "Common";
                }
                else if (HttpContext.Current.User.IsInRole("FIN") || HttpContext.Current.User.IsInRole("FINHEAD"))
                {
                    actionName = "LcOpeningApproval";
                    controllerName = "Common";
                }
                else if (HttpContext.Current.User.IsInRole("CEO"))
                {
                    actionName = "LcOpeningApproval";
                    controllerName = "Common";
                }
                else if (HttpContext.Current.User.IsInRole("AUD") || HttpContext.Current.User.IsInRole("AUDHEAD"))
                {
                    actionName = "LcList";
                    controllerName = "Audit";
                }
                else if (HttpContext.Current.User.IsInRole("SALES") || HttpContext.Current.User.IsInRole("SALESHEAD"))
                {
                    actionName = "ProjectPoFeedback";
                    controllerName = "Common";
                }
                else if (HttpContext.Current.User.IsInRole("CBO"))
                {
                    actionName = "LcOpeningApproval";
                    controllerName = "Common";
                }
                else if (HttpContext.Current.User.IsInRole("COO"))
                {
                    actionName = "LcOpeningApproval";
                    controllerName = "Common";
                }
                else if (HttpContext.Current.User.IsInRole("BIHEAD"))
                {
                    actionName = "Index";
                    controllerName = "Management";
                }
                else if (HttpContext.Current.User.IsInRole("WHEAD"))
                {
                    actionName = "PendingApprovals";
                    controllerName = "MaterialWastage";
                }
                else
                {
                    controllerName = "Home";
                    actionName = "Home";
                }
            }
            else
            {
                controllerName = "Home";
                actionName = "Login";
            }

            return new Tuple<String, String>(actionName, controllerName);
        }

        public int AuthorizedUserByUserNamePassword(string userName, string password, bool rememberMe)
        {
            var user = _dbContext.CmnUsers.FirstOrDefault(i => i.UserName == userName && i.IsActive);
            if (user != null)
            {
                string ip = HttpContext.Current.Request.UserHostAddress;
                String currentDateTime = Convert.ToString(DateTime.Now);
                user.LastLoginDateTime = currentDateTime;
                user.LastLoginIpAddress = ip;
                user.IsRememberMailSend = false;
                _dbContext.CmnUsers.AddOrUpdate(user);
                //login tracker
                var loginTracker = new UserLoginTracker
                {
                    CmnUserId = user.CmnUserId,
                    UserName = user.UserName,
                    UserFullName = user.UserFullName,
                    LoginDateTime = DateTime.Now,
                    IpAddress = ip,
                    Role=user.RoleName,
                    ExtendedRole=user.ExtendedRoleName
                };
                _dbContext.UserLoginTrackers.Add(loginTracker);
                _dbContext.SaveChanges();
            }

            string originalPassword = Encrypt(password);
            
            if (user != null && originalPassword == user.Password)
            {
                var lastPassUpdatedBefore = user.LastPasswordUpdateDate == null ? 30 : (DateTime.Now - Convert.ToDateTime(user.LastPasswordUpdateDate)).TotalDays;
                if (lastPassUpdatedBefore > 29)
                {
                    return 3;
                }
                string extendedRole = user.ExtendedRoleName;
                string roleName = user.RoleName;
                roleName = roleName + "," + extendedRole;

                user.RoleName = roleName;

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1, // Ticket version
                    Convert.ToString(user.CmnUserId), // Username associated with ticket
                    DateTime.Now, // Date/time issued
                    DateTime.Now.AddMinutes(60), // Date/time to expire
                    rememberMe, // "true" for a persistent user cookie
                    user.RoleName, // User-data, in this case the roles
                    FormsAuthentication.FormsCookiePath); // Path cookie valid for
                

                // Encrypt the cookie using the machine key for secure transport,
                string hash = FormsAuthentication.Encrypt(ticket);
                HttpCookie cookie = new HttpCookie(
                    FormsAuthentication.FormsCookieName, // Name of auth cookie
                    hash); // Hashed ticket

                // Set the cookie's expiration time to the tickets expiration time
                if (ticket.IsPersistent) cookie.Expires = ticket.Expiration;

                // Add the cookie to the list for outgoing response
                HttpContext.Current.Response.Cookies.Add(cookie);
                return 1;
            }


            return 0;
        }

        public long CreateUser(CmnUserModel model)
        {
            try
            {
                var encryptedPassword = Encrypt(model.Password);
                if (!string.IsNullOrWhiteSpace(encryptedPassword))
                {
                    model.Password = encryptedPassword;
                    CmnUser cmnUser = GenericMapper<CmnUserModel, CmnUser>.GetDestination(model);
                    var result = GenereticRepo<CmnUser>.Add(_dbContext, cmnUser, 0);
                    return result.CmnUserId;
                }
            }
            catch (Exception)
            {
                return 0;
            }
            return 0;
        }

        public bool DeleteUser(CmnUser user)
        {
            throw new NotImplementedException();
        }

        public bool UpdateUser(CmnUserModel user)
        {
            try
            {
                CmnUser cmuser = GenericMapper<CmnUserModel, CmnUser>.GetDestination(user);
                GenereticRepo<CmnUser>.Update(_dbContext, cmuser);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public CmnUser GetUserByUserName(string username)
        {
            var v = _dbContext.CmnUsers.FirstOrDefault(x => x.UserName == username);
            return v;
        }

        public void ResetPassword(CmnUser user)
        {
            user.Password = Encrypt(user.Password);
            _dbContext.CmnUsers.AddOrUpdate(user);
            _dbContext.SaveChanges();
        }

        public bool ChagePassword(CmnUserModel model)
        {
            try
            {
                String encrypedPassword = Encrypt(model.Password);
                long userId = model.CmnUserId;
                String query = String.Format("select * from CmnUsers where CmnUserid={0}", userId);
                CmnUser user = GenereticRepo<CmnUser>.Get(_dbContext, query);
                user.Password = encrypedPassword;
                user.Updated = model.Updated;
                user.UpdatedDate = model.UpdatedDate;
                GenereticRepo<CmnUser>.Update(_dbContext, user);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string CheckUserNameExist(string userName)
        {
            String query = String.Format("select * from CmnUsers where UserName='{0}'", userName);
            var id = GenereticRepo<CmnUser>.Get(_dbContext, query);
            if (id == null)//Not exists
            {
                return "";
            }
            return "Already Exist";
        }

        public List<string> GetAllRoles()
        {
            List<String> roleList = new List<string>();
            List<CmnRole> roles = GenereticRepo<CmnRole>.GetList(_dbContext);
            foreach (var role in roles)
            {
                roleList.Add(role.RoleName);
            }

            return roleList;
        }

        public CmnUserModel GetUser(long userId)
        {
            CmnUser user = GenereticRepo<CmnUser>.GetById(_dbContext, userId);
            CmnUserModel userModel = GenericMapper<CmnUser, CmnUserModel>.GetDestination(user);
            return userModel;
        }

        public string CheckOldPassword(long userId, string password)
        {
            try
            {
                var cmnUser = _dbContext.CmnUsers.OrderByDescending(i => i.CmnUserId)
                    .FirstOrDefault(i => i.CmnUserId == userId);
                if (cmnUser != null)
                {
                    var oldPass = cmnUser.Password;
                    string encryptedOldPassword = Encrypt(password);
                    if (oldPass == encryptedOldPassword) return string.Empty;
                    return "Old Password Does not match";
                }
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            return "Something goes wrong, Contact with administrator";
        }

        public List<CmnUserModel> GetAllUser()
        {
            var models=new List<CmnUserModel>();
            List<CmnUser> users = GenereticRepo<CmnUser>.GetList(_dbContext);
            models = GenericMapper<CmnUser, CmnUserModel>.GetDestinationList(users);
            return models;
        }
    }
}