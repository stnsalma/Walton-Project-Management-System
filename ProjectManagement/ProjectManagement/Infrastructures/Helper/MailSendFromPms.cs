using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using ProjectManagement.DAL.DbModel;

namespace ProjectManagement.Infrastructures.Helper
{
    public class MailSendFromPms
    {
        public string SendMail(List<long> toIdList, List<long> ccIdList, string subject, string body = null)
        {
            using (var dbEntities = new CellPhoneProjectEntities())
            {
                if (toIdList == null || !toIdList.Any() || string.IsNullOrWhiteSpace(subject)) return "Error email";
                List<string> toEmailIdList = dbEntities.CmnUsers.Where(i => toIdList.Contains(i.CmnUserId) && i.IsActive).Select(x => x.Email).ToList();
                toEmailIdList = FilterEmails(toEmailIdList);
                List<string> ccEmailAdresses = dbEntities.CmnUsers.Where(i => ccIdList.Contains(i.CmnUserId) && i.IsActive).Select(x => x.Email).ToList();
                ccEmailAdresses = FilterEmails(ccEmailAdresses);

                return ConstructAndSendEmail(toEmailIdList, ccEmailAdresses, subject, body);
            }
        }

        public string SendMail(List<string> toRoleListList, List<long> ccIdList, string subject, string body = null)
        {
            using (var dbEntities = new CellPhoneProjectEntities())
            {
                if (toRoleListList == null || !toRoleListList.Any() || string.IsNullOrWhiteSpace(subject)) return "Error email";
                List<string> toEmailIdList = dbEntities.CmnUsers.Where(i => toRoleListList.Contains(i.RoleName) && i.IsActive).Select(x => x.Email).ToList();
                toEmailIdList = FilterEmails(toEmailIdList);
                List<string> ccEmailAdresses = ccIdList!= null ? dbEntities.CmnUsers.Where(i => ccIdList.Contains(i.CmnUserId) && i.IsActive).Select(x => x.Email).ToList():new List<string>();
                ccEmailAdresses = FilterEmails(ccEmailAdresses);
                return ConstructAndSendEmail(toEmailIdList, ccEmailAdresses, subject, body);
            }
        }
        public string SendMail(List<string> toIdList, List<string> ccIdList, string subject, string body = null)
        {
            using (var dbEntities = new CellPhoneProjectEntities())
            {
                if (toIdList == null || !toIdList.Any() || string.IsNullOrWhiteSpace(subject)) return "Error email";
                List<string> toEmailIdList = dbEntities.CmnUsers.Where(i => toIdList.Contains(i.RoleName) && i.IsActive).Select(x => x.Email).ToList();
                toEmailIdList = FilterEmails(toEmailIdList);
                List<string> ccEmailAdresses = dbEntities.CmnUsers.Where(i => ccIdList.Contains(i.RoleName) && i.IsActive).Select(x => x.Email).ToList();
                ccEmailAdresses = FilterEmails(ccEmailAdresses);

                return ConstructAndSendEmail(toEmailIdList, ccEmailAdresses, subject, body);
            }
        }
        public string SendMail(List<long> toIdList, List<string> ccIdList, string subject, string body = null)
        {
            using (var dbEntities = new CellPhoneProjectEntities())
            {
                if (toIdList == null || !toIdList.Any() || string.IsNullOrWhiteSpace(subject)) return "Error email";
                List<string> toEmailIdList = dbEntities.CmnUsers.Where(i => toIdList.Contains(i.CmnUserId) && i.IsActive).Select(x => x.Email).ToList();
                toEmailIdList = FilterEmails(toEmailIdList);
                List<string> ccEmailAdresses = dbEntities.CmnUsers.Where(i => ccIdList.Contains(i.RoleName) && i.IsActive).Select(x => x.Email).ToList();
                ccEmailAdresses = FilterEmails(ccEmailAdresses);

                return ConstructAndSendEmail(toEmailIdList, ccEmailAdresses, subject, body);
                 
            }
        }
        public string SendMailWithSpecificEmailList(List<string> toEmailList, List<string> ccEmailList, string subject, string body = null)
        {
            using (var dbEntities = new CellPhoneProjectEntities())
            {
                return ConstructAndSendEmail(toEmailList, ccEmailList, subject, body);

            }
        }
        private List<string> FilterEmails(List<string> toEmailIdList)
        {
            if (toEmailIdList.Any())
            {
                return toEmailIdList.Where(emailAddress => !string.IsNullOrWhiteSpace(emailAddress) && Regex.IsMatch(emailAddress, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase)).Distinct().ToList();
            }
            return new List<string>();
        }
        public string PopulateBody(string body)
        {
            string template;
            string filePath =
                string.Format(
                    @"~\Infrastructures\Helper\EmailTemplate.html");
            using (var reader = new StreamReader(HttpContext.Current.Server.MapPath(filePath)))
            {
                template = reader.ReadToEnd();
            }
            template = template.Replace("{body}", body);
            return template;
        }
        public string ConstructAndSendEmail(IEnumerable<string> toEmailIdList, List<string> ccEmailAdresses, string subject, string body)
        {
            
            try
            {
                var mailMessage = new MailMessage { From = new MailAddress("test") };

                foreach (var to in toEmailIdList)
                {
                    mailMessage.To.Add(to);
                }
                if (ccEmailAdresses.Any())
                {
                    foreach (var cc in ccEmailAdresses)
                    {
                        mailMessage.CC.Add(cc);
                    }
                }

                mailMessage.Subject = subject;
                mailMessage.Body = PopulateBody(body);
                mailMessage.IsBodyHtml = true;
                mailMessage.Bcc.Add("atiqul19846@waltonbd.com");
                var smtp = new SmtpClient
                {
                    UseDefaultCredentials = false,
                    Host = "test",
                    Port = 25,
                    Credentials = new System.Net.NetworkCredential("test", "test")
                    
                };
                smtp.Send(mailMessage);
                return "email sent";
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }
    }
}