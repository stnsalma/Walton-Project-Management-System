using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using ProjectManagement.DAL;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Models;

namespace ProjectManagement.Infrastructures.Repositories
{
    public class CpsdRepository:ICpsdRepository
    {
        private readonly CellPhoneProjectEntities _dbEntities;

        public CpsdRepository()
        {
            _dbEntities = new CellPhoneProjectEntities();
            _dbEntities.Configuration.LazyLoadingEnabled = false;
        }

        #region GET

        public List<ServiceToSalesRatioWarningMailModel> GetServiceToSalesRatioWarningMailModels()
        {
            string query = string.Format(@"select * from ServiceToSalesRatioWarningMails");
            var exe = _dbEntities.Database.SqlQuery<ServiceToSalesRatioWarningMailModel>(query).ToList();
            return exe;
        }

        public ServiceToSalesRatioWarningMailModel GetServiceToSalesRatioWarningMailModelById(long id)
        {
            var ratioModel = (from m in _dbEntities.ServiceToSalesRatioWarningMails
                where m.ServiceToSalesRatioWarningId == id
                select new ServiceToSalesRatioWarningMailModel
                {
                    ServiceToSalesRatioWarningId = m.ServiceToSalesRatioWarningId,
                    ProductCode = m.ProductCode,
                    Model = m.Model,
                    LaunchDate = m.LaunchDate,
                    ServiceToSalesRatio = m.ServiceToSalesRatio,
                    AddedBy = m.AddedBy,
                    AddedDate = m.AddedDate
                }).FirstOrDefault();
            return ratioModel;
        }
        #endregion

        #region UPDATE

        public void UpdateServiceToSalesRatioMonitor(string solution,string submittedBy, long id)
        {
            var ssrs = (from m in _dbEntities.ServiceToSalesRatioWarningMails
                              where m.ServiceToSalesRatioWarningId == id
                              select new ServiceToSalesRatioWarningMailModel
                              {
                                  ServiceToSalesRatioWarningId = m.ServiceToSalesRatioWarningId,
                                  ProductCode = m.ProductCode,
                                  Model = m.Model,
                                  LaunchDate = m.LaunchDate,
                                  ServiceToSalesRatio = m.ServiceToSalesRatio,
                                  AddedBy = m.AddedBy,
                                  AddedDate = m.AddedDate
                              }).FirstOrDefault();
            string query = "";
            if (HttpContext.Current.User.IsInRole("ASPM") || HttpContext.Current.User.IsInRole("ASPMHEAD"))
            {
                query = string.Format(@"UPDATE ServiceToSalesRatioWarningMails SET Solution='{1}',SolutionDate=GETDATE(),SolutionBy={2} WHERE ServiceToSalesRatioWarningId={0}", id, solution, HttpContext.Current.User.Identity.Name);
                var user = _dbEntities.CmnUsers.Where(x => x.RoleName == "ASPM").ToList();
                foreach (var l in user)
                {
                    var noti = new Notification
                    {
                        Message = "Service to sales ratio solution of Model " + ssrs.Model + " has been submitted by "+submittedBy ,
                        AdditionalMessage = "Submitted on " + ssrs.AddedDate,
                        Role = "ASPM",
                        ViewerId = Convert.ToInt32(l.CmnUserId)
                    };
                    _dbEntities.Notifications.Add(noti);
                    _dbEntities.SaveChanges();
                }
                user = _dbEntities.CmnUsers.Where(x => x.RoleName == "ASPMHEAD").ToList();
                foreach (var l in user)
                {
                    var noti = new Notification
                    {
                        Message = "Service to sales ratio solution of Model " + ssrs.Model + " has been submitted by " + submittedBy,
                        AdditionalMessage = "Submitted on " + ssrs.AddedDate,
                        Role = "ASPMHEAD",
                        ViewerId = Convert.ToInt32(l.CmnUserId)
                    };
                    _dbEntities.Notifications.Add(noti);
                    _dbEntities.SaveChanges();
                }
                user = _dbEntities.CmnUsers.Where(x => x.RoleName == "ASPM").ToList();
                foreach (var l in user)
                {
                    var noti = new Notification
                    {
                        Message = "Service to sales ratio solution of Model " + ssrs.Model + " has been submitted by " + submittedBy,
                        AdditionalMessage = "Submitted on " + ssrs.AddedDate,
                        Role = "ASPM",
                        ViewerId = Convert.ToInt32(l.CmnUserId)
                    };
                    _dbEntities.Notifications.Add(noti);
                    _dbEntities.SaveChanges();
                }
                user = _dbEntities.CmnUsers.Where(x => x.RoleName == "ASPMHEAD").ToList();
                foreach (var l in user)
                {
                    var noti = new Notification
                    {
                        Message = "Service to sales ratio solution of Model " + ssrs.Model + " has been submitted by " + submittedBy,
                        AdditionalMessage = "Submitted on " + ssrs.AddedDate,
                        Role = "ASPMHEAD",
                        ViewerId = Convert.ToInt32(l.CmnUserId)
                    };
                    _dbEntities.Notifications.Add(noti);
                    _dbEntities.SaveChanges();
                }
            }
            if (HttpContext.Current.User.IsInRole("CPSD") || HttpContext.Current.User.IsInRole("CPSDHEAD") || HttpContext.Current.User.IsInRole("ASPM") || HttpContext.Current.User.IsInRole("ASPMHEAD"))
            {
                query = string.Format(@"UPDATE ServiceToSalesRatioWarningMails SET IsSolved=1,ClosedBy={1},ClosingDate=GETDATE() WHERE ServiceToSalesRatioWarningId={0}", id, HttpContext.Current.User.Identity.Name);
                var user = _dbEntities.CmnUsers.Where(x => x.RoleName == "ASPM").ToList();
                foreach (var l in user)
                {
                    var noti = new Notification
                    {
                        Message = "Service to sales ratio issue of Model " + ssrs.Model + " has been closed by " + submittedBy,
                        AdditionalMessage = "Submitted on " + ssrs.AddedDate,
                        Role = "ASPM",
                        ViewerId = Convert.ToInt32(l.CmnUserId)
                    };
                    _dbEntities.Notifications.Add(noti);
                    _dbEntities.SaveChanges();
                }
                user = _dbEntities.CmnUsers.Where(x => x.RoleName == "ASPMHEAD").ToList();
                foreach (var l in user)
                {
                    var noti = new Notification
                    {
                        Message = "Service to sales ratio issue of Model " + ssrs.Model + " has been closed by " + submittedBy,
                        AdditionalMessage = "Submitted on " + ssrs.AddedDate,
                        Role = "ASPMHEAD",
                        ViewerId = Convert.ToInt32(l.CmnUserId)
                    };
                    _dbEntities.Notifications.Add(noti);
                    _dbEntities.SaveChanges();
                }
                user = _dbEntities.CmnUsers.Where(x => x.RoleName == "ASPM").ToList();
                foreach (var l in user)
                {
                    var noti = new Notification
                    {
                        Message = "Service to sales ratio issue of Model " + ssrs.Model + " has been closed by " + submittedBy,
                        AdditionalMessage = "Submitted on " + ssrs.AddedDate,
                        Role = "ASPM",
                        ViewerId = Convert.ToInt32(l.CmnUserId)
                    };
                    _dbEntities.Notifications.Add(noti);
                    _dbEntities.SaveChanges();
                }
                user = _dbEntities.CmnUsers.Where(x => x.RoleName == "ASPMHEAD").ToList();
                foreach (var l in user)
                {
                    var noti = new Notification
                    {
                        Message = "Service to sales ratio issue of Model " + ssrs.Model + " has been closed by " + submittedBy,
                        AdditionalMessage = "Submitted on " + ssrs.AddedDate,
                        Role = "ASPMHEAD",
                        ViewerId = Convert.ToInt32(l.CmnUserId)
                    };
                    _dbEntities.Notifications.Add(noti);
                    _dbEntities.SaveChanges();
                }
            }
             _dbEntities.Database.ExecuteSqlCommand(query);
        }
        #endregion
    }
}