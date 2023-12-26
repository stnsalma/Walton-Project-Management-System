using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Data.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Models;

namespace ProjectManagement.Infrastructures.Repositories
{
    public class GeneralIncidentRepository : IGeneralIncidentRepository
    {
        private readonly CellPhoneProjectEntities _dbeEntities;
        //  dbeEntities.Con

        public GeneralIncidentRepository()
        {
            _dbeEntities = new CellPhoneProjectEntities();
            _dbeEntities.Configuration.LazyLoadingEnabled = false;
        }
        #region GET
        public List<GeneralIncidentCategoryModel> GetGeneralIncidentCategories()
        {
            var category = (from cat in _dbeEntities.GeneralIncidentCategories
                            select new GeneralIncidentCategoryModel
                                {
                                    GeneralIncidentCategoryId = cat.GeneralIncidentCategoryId,
                                    GeneralIncidentCategoryName = cat.GeneralIncidentCategoryName
                                }).ToList();
            return category;
        }

        public List<CmnRoleModel> GetAllRoleModels()
        {
            var roles = (from role in _dbeEntities.CmnRoles
                         where role.IsHead == true
                         select new CmnRoleModel
                         {
                             CmnRoleId = role.CmnRoleId,
                             RoleName = role.RoleName,
                             RoleDescription = role.RoleDescription
                         }).ToList();
            return roles;
        }

        public List<GeneralIncidentCategoryModel> GetaGeneralIncidentCategoryModels()
        {
            var cats = (from cat in _dbeEntities.GeneralIncidentCategories
                select new GeneralIncidentCategoryModel
                {
                    GeneralIncidentCategoryId = cat.GeneralIncidentCategoryId,
                    GeneralIncidentCategoryName = cat.GeneralIncidentCategoryName,
                    AddedBy = cat.AddedBy,
                    AddedDate = cat.AddedDate
                }).ToList();
            return cats;
        } 

        public List<CmnUserModel> GetUserModels(long userId)
        {
            var roleName = (from role in _dbeEntities.CmnUsers
                            where role.CmnUserId == userId
                            select new CmnUserModel
                            {
                                RoleName = role.RoleName
                            }).FirstOrDefault();
            var roledescription = (from r in _dbeEntities.CmnRoles
                                   where r.RoleName == roleName.RoleName
                                   select new CmnRoleModel
                                   {
                                       RoleDescription = r.RoleDescription
                                   }).FirstOrDefault();
            var users = String.Format(@"select * from CmnUsers cu inner join CmnRoles cr
on cu.RoleName=cr.RoleName where cr.RoleDescription='{0}' and cu.CmnUserId!={1}", roledescription.RoleDescription, userId);
            var exe = _dbeEntities.Database.SqlQuery<CmnUserModel>(users).ToList();
            return exe;
        }

        public List<GeneralIncidentModel> GetGeneralIncidentByAddedBy(long addedby)
        {
            var incidents = (from i in _dbeEntities.GeneralIncidents
                             where i.AddedBy == addedby
                             select new GeneralIncidentModel
                             {
                                 GeneralIncidentId = i.GeneralIncidentId,
                                 GeneralIncidentCategories = i.GeneralIncidentCategories,
                                 GeneralIncidentTitle = i.GeneralIncidentTitle,
                                 GeneralIncidentDetails = i.GeneralIncidentDetails,
                                 AddedDate = i.AddedDate,
                                 UpadatedDate = i.UpadatedDate,
                                 DiscloseDate = i.DiscloseDate,
                                 Status = i.Status,
                                 RefferedRole = i.RefferedRole
                             }).Take(40).OrderByDescending(i => i.AddedDate).ToList();
            //incidents = incidents.OrderByDescending(i => i.Status).ToList();
            return incidents;
        }

        public List<GeneralIncidentModel> GetIncidentsForAssign()
        {
            var incidents = (from i in _dbeEntities.GeneralIncidents
                             where i.DiscloseDate == null
                             select new GeneralIncidentModel
                             {
                                 GeneralIncidentId = i.GeneralIncidentId,
                                 GeneralIncidentCategories = i.GeneralIncidentCategories,
                                 GeneralIncidentTitle = i.GeneralIncidentTitle,
                                 GeneralIncidentDetails = i.GeneralIncidentDetails,
                                 AddedDate = i.AddedDate,
                                 UpadatedDate = i.UpadatedDate,
                                 DiscloseDate = i.DiscloseDate,
                                 RefferedRole = i.RefferedRole,
                                 Status = i.Status,
                                 AddedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == i.AddedBy).Select(x => x.UserFullName).FirstOrDefault()
                             }).ToList();
            var notsolvedincident = new List<GeneralIncidentModel>();
            foreach (var g in incidents)
            {
                //var solution = _dbeEntities.GeneralIncidentSolutions.Where(x => x.GeneralIncidentId == g.GeneralIncidentId).ToList();
                //var assigned = _dbeEntities.GeneralIncidentAssigns.Where(x => x.GeneralIncidentId == g.GeneralIncidentId).ToList();
                //var lastassign =
                //    _dbeEntities.GeneralIncidentAssigns.Where(x => x.GeneralIncidentId == g.GeneralIncidentId)
                //        .OrderByDescending(x => x.AssignDate)
                //        .FirstOrDefault();
                if (g.Status=="NEW" || g.Status=="FORWARDED" || g.Status=="PARTIALSUBMIT")
                {
                    notsolvedincident.Add(g);
                }
            }
            return notsolvedincident;
        }

        public List<GeneralIncidentModel> GetAssignedIncidentForMe(long id)
        {
            var incident = new List<GeneralIncidentModel>();
            var assigned = (from a in _dbeEntities.GeneralIncidentAssigns
                            where a.AssignedTo == id
                            select new GeneralIncidentAssignModel
                            {
                                GeneralIncidentId = a.GeneralIncidentId
                            }).ToList();
            foreach (var g in assigned)
            {
                var solution = _dbeEntities.GeneralIncidentSolutions.Where(x => x.GeneralIncidentId == g.GeneralIncidentId).ToList();
                var lastassign =
                    _dbeEntities.GeneralIncidentAssigns.Where(x => x.GeneralIncidentId == g.GeneralIncidentId)
                        .OrderByDescending(x => x.AssignDate)
                        .FirstOrDefault();
                var v = (from i in _dbeEntities.GeneralIncidents
                         where i.GeneralIncidentId == g.GeneralIncidentId
                         select new GeneralIncidentModel
                         {
                             GeneralIncidentId = i.GeneralIncidentId,
                             GeneralIncidentCategories = i.GeneralIncidentCategories,
                             GeneralIncidentTitle = i.GeneralIncidentTitle,
                             GeneralIncidentDetails = i.GeneralIncidentDetails,
                             AddedDate = i.AddedDate,
                             UpadatedDate = i.UpadatedDate,
                             DiscloseDate = i.DiscloseDate,
                             Status = i.Status,
                             RefferedRole = i.RefferedRole,
                             AddedByName =
                                 _dbeEntities.CmnUsers.Where(x => x.CmnUserId == i.AddedBy)
                                     .Select(x => x.UserFullName)
                                     .FirstOrDefault()
                         }).FirstOrDefault();
                if (lastassign != null && (!solution.Any() && lastassign.AssignedTo==id))
                {
                    incident.Add(v);   
                }  
            }
            return incident;
        }

        public List<GeneralIncidentAssignModel> GetGeneralIncidentAssignModels(long incidentId)
        {
            var assigned = (from a in _dbeEntities.GeneralIncidentAssigns
                            where a.GeneralIncidentId == incidentId
                            select new GeneralIncidentAssignModel
                            {
                                AssignIncidentId = a.AssignIncidentId,
                                GeneralIncidentId = a.GeneralIncidentId,
                                AssignRemarks = a.AssignRemarks,
                                AssignByRole = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == a.AssignedBy).Select(x => x.RoleName).FirstOrDefault(),
                                AssignDate = a.AssignDate,
                                AssignedBy = a.AssignedBy,
                                AssignedByName =  _dbeEntities.CmnUsers.Where(x => x.CmnUserId == a.AssignedBy)
                                     .Select(x => x.UserFullName)
                                     .FirstOrDefault(),
                                AssignedTo = a.AssignedTo,
                                AssignedToRole = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == a.AssignedTo).Select(x => x.RoleName).FirstOrDefault(),
                                AssignedToName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == a.AssignedTo)
                                     .Select(x => x.UserFullName)
                                     .FirstOrDefault()
                            }).ToList();
            return assigned;
        } 

        public string GetRoleDescriptionByRoleName(string role)
        {
            var roledesc =
                _dbeEntities.CmnRoles.Where(x => x.RoleName == role).Select(x => x.RoleDescription).FirstOrDefault();
            return roledesc;
        }

        public GeneralIncidentModel GetGeneralIncidentByIncidentId(long incidentId)
        {
            var incident = (from i in _dbeEntities.GeneralIncidents
                            where i.GeneralIncidentId == incidentId
                            select new GeneralIncidentModel
                            {
                                GeneralIncidentId = i.GeneralIncidentId,
                                GeneralIncidentCategories = i.GeneralIncidentCategories,
                                GeneralIncidentTitle = i.GeneralIncidentTitle,
                                GeneralIncidentDetails = i.GeneralIncidentDetails,
                                AddedDate = i.AddedDate,
                                UpadatedDate = i.UpadatedDate,
                                DiscloseDate = i.DiscloseDate,
                                DisclosedBy = i.DisclosedBy,
                                DisclosedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == i.DisclosedBy).Select(x => x.UserFullName).FirstOrDefault(),
                                DiscloseRemark = i.DiscloseRemark,
                                AddedBy = i.AddedBy,
                                RefferedRole = i.RefferedRole,
                                Status = i.Status,
                                ModelName = i.ModelName,
                                Issues = i.Issues,
                                FileUploadPath = i.FileUploadPath,
                                AddedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == i.AddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                                SubmittedDate = i.SubmittedDate,
                                SubmitRemark = i.SubmitRemark,
                                SubmittedBy = i.SubmittedBy,
                                SubmittedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == i.SubmittedBy).Select(x => x.UserFullName).FirstOrDefault()
                            }).FirstOrDefault();
            return incident;
        }

        public List<GeneralIncidentLogModel> GetGeneralIncidentLogModels(long incidentId)
        {
            var incidentlog = (from i in _dbeEntities.GeneralIncidentLogs
                               where i.GeneralIncidentId == incidentId
                               select new GeneralIncidentLogModel
                               {
                                   LogId = i.LogId,
                                   GeneralIncidentId = i.GeneralIncidentId,
                                   GeneralIncidentTitle = i.GeneralIncidentTitle,
                                   GeneralIncidentCategories = i.GeneralIncidentCategories,
                                   GeneralIncidentDetails = i.GeneralIncidentDetails,
                                   AddedBy = i.AddedBy,
                                   AddedDate = i.AddedDate,
                                   ForwaredBy = i.ForwaredBy,
                                   ForwardRemark = i.ForwardRemark,
                                   ForwardedDate = i.ForwardedDate,
                                   RefferedRole = i.RefferedRole,
                                   Status = i.Status,
                                   ModelName = i.ModelName,
                                   Issues = i.Issues,
                                   ForwardByRole = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == i.ForwaredBy).Select(x => x.RoleName).FirstOrDefault(),
                                   ForwardByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == i.ForwaredBy).Select(x => x.UserFullName).FirstOrDefault()
                               }).ToList();
            foreach (var logModel in incidentlog)
            {
                var refferedNames = string.Empty;
                var names = _dbeEntities.CmnUsers.Where(i => i.RoleName == logModel.RefferedRole && i.IsActive).ToList();
                foreach (var name in names)
                {
                    refferedNames = !string.IsNullOrWhiteSpace(refferedNames) ? refferedNames + ", " + name.UserFullName : name.UserFullName;
                }
                refferedNames = refferedNames.TrimEnd(',');
                logModel.RefferedRole = refferedNames;
            }
            return incidentlog;
        }

        public List<GeneralIncidentModel> GetGeneralIncidentForDisclose()
        {
            var incidents = (from i in _dbeEntities.GeneralIncidents
                             where i.DiscloseDate == null
                             select new GeneralIncidentModel
                             {
                                 GeneralIncidentId = i.GeneralIncidentId,
                                 GeneralIncidentCategories = i.GeneralIncidentCategories,
                                 GeneralIncidentTitle = i.GeneralIncidentTitle,
                                 GeneralIncidentDetails = i.GeneralIncidentDetails,
                                 AddedDate = i.AddedDate,
                                 UpadatedDate = i.UpadatedDate,
                                 DiscloseDate = i.DiscloseDate,
                                 RefferedRole = i.RefferedRole,
                                 Status = i.Status,
                                 ModelName = i.ModelName,
                                 Issues = i.Issues,
                                 AddedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == i.AddedBy).Select(x => x.UserFullName).FirstOrDefault()
                             }).ToList();
            var solvedincident = new List<GeneralIncidentModel>();
            foreach (var g in incidents)
            {
                var solution = _dbeEntities.GeneralIncidentSolutions.Where(x => x.GeneralIncidentId == g.GeneralIncidentId).ToList();
                if (solution.Any())
                {
                    solvedincident.Add(g);
                }
            }
            return solvedincident;
        }

        public GeneralIncidentSolutionModel GetIncidentSolutionByIncidentId(long incidentId)
        {
            var solution = (from s in _dbeEntities.GeneralIncidentSolutions
                            where s.GeneralIncidentId == incidentId
                            select new GeneralIncidentSolutionModel
                                {
                                    SolutionId = s.SolutionId,
                                    GeneralIncidentId = s.GeneralIncidentId,
                                    Solution = s.Solution,
                                    AddedBy = s.AddedBy,
                                    AddedByName = s.AddedByName,
                                    AddedDate = s.AddedDate,
                                    AddedRole = s.AddedRole,
                                    DenyDate = s.DenyDate,
                                    DenyRemark = s.DenyRemark
                                }).FirstOrDefault();
            return solution;
        }

        public GeneralIncidentDashboardModel GetGeneralIncidentDashboardCounter()
        {
            string query = string.Format(@"select (select count(*) from GeneralIncidents) as TotalIncidents,
(select count(*)  from GeneralIncidents gi inner join GeneralIncidentSolutions gis on gi.GeneralIncidentId=gis.GeneralIncidentId where gi.DiscloseDate is null) as DisclosePending,
(select count(*) from GeneralIncidents where DiscloseDate is not null) as Disclosed");
            var exe = _dbeEntities.Database.SqlQuery<GeneralIncidentDashboardModel>(query).FirstOrDefault();
            return exe;
        }
        
        public GeneralIncidentDashboardModel GetGeneralIncidentDashboardCounterByReferredRole(string referredRole)
        {
            string query = string.Format(@"select (select count(*) from GeneralIncidents where RefferedRole='{0}') as TotalIncidents,
(select count(*)  from GeneralIncidents gi inner join GeneralIncidentSolutions gis on gi.GeneralIncidentId=gis.GeneralIncidentId where gi.DiscloseDate is null and gi.RefferedRole='{0}') as DisclosePending,
(select count(*) from GeneralIncidents where DiscloseDate is not null and RefferedRole='{0}') as Disclosed",referredRole);
            var exe = _dbeEntities.Database.SqlQuery<GeneralIncidentDashboardModel>(query).FirstOrDefault();
            return exe;
        }

        public List<GeneralIncidentModel> GetSolutionPendingIncidents()
        {
            string query = string.Format(@"SELECT gi.*,(select UserFullName from CmnUsers where CmnUserId=gi.AddedBy) AddedByName FROM GeneralIncidents gi where (select count(*) from GeneralIncidentSolutions gis where gis.GeneralIncidentId=gi.GeneralIncidentId)=0");
            var exe = _dbeEntities.Database.SqlQuery<GeneralIncidentModel>(query).ToList();
            return exe;
        }

        public List<GeneralIncidentModel> GetIncidentSolvedByMe(long userId)
        {
            var role = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == userId).Select(x => x.RoleName).FirstOrDefault();
            string query = string.Format(@"select * from GeneralIncidents gi inner join GeneralIncidentSolutions gis on gi.GeneralIncidentId=gis.GeneralIncidentId where gis.AddedBy='{0}'", userId);
            var exe = _dbeEntities.Database.SqlQuery<GeneralIncidentModel>(query).ToList();
            return exe;
        }

        public class ModelNamesModel
        {
            public string ModelName { get; set; }
        }

        public List<ModelNamesModel> GetModelses()
        {
            var model = new List<ModelNamesModel>();
            string sqlconnectionstring = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(sqlconnectionstring);
            conn.Open();
            string query = string.Format("select ProductModel from tblProductMaster where ProductType='Cell Phone' and dateadded>'2016-01-01'order by ProductModel");
            var cmd = new SqlCommand(query,conn);
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                var mod = new ModelNamesModel
                {
                    ModelName = (string) rd["ProductModel"]
                };
                model.Add(mod);
            }
            conn.Close();
            return model;
        }

        public List<GeneralIncidentModel> GetGeneralIncidentModelsByRole(string referredRole)
        {
            var i = new List<GeneralIncidentModel>();
            var log = (from g in _dbeEntities.GeneralIncidentLogs where g.RefferedRole==referredRole group g.GeneralIncidentId by g.GeneralIncidentId into a select new GeneralIncidentLogModel
            {
                GeneralIncidentId = a.Key
            }).ToList();
            foreach (var l in log)
            {
                var j = (from g in _dbeEntities.GeneralIncidents where g.GeneralIncidentId==l.GeneralIncidentId select new GeneralIncidentModel
                {
                    GeneralIncidentId = g.GeneralIncidentId,
                    RefferedRole = g.RefferedRole,
                    GeneralIncidentTitle = g.GeneralIncidentTitle,
                    GeneralIncidentCategories = g.GeneralIncidentCategories,
                    GeneralIncidentDetails = g.GeneralIncidentDetails,
                    AddedBy = g.AddedBy,
                    AddedDate = g.AddedDate,
                    UpadatedDate = g.UpadatedDate,
                    UpdatedBy = g.UpdatedBy,
                    DiscloseDate = g.DiscloseDate,
                    DisclosedBy = g.DisclosedBy,
                    DiscloseRemark = g.DiscloseRemark,
                    ReassignId = g.ReassignId,
                    ReassignRemark = g.ReassignRemark,
                    ReassignedBy = g.ReassignedBy,
                    ReassignDate = g.ReassignDate,
                    Status = g.Status,
                    FileUploadPath = g.FileUploadPath,
                    ModelName = g.ModelName,
                    Issues = g.Issues,
                    SubmittedBy = g.SubmittedBy,
                    SubmittedDate = g.SubmittedDate,
                    SubmitRemark = g.SubmitRemark,
                    AddedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == g.AddedBy).Select(x => x.UserFullName).FirstOrDefault()
                }).FirstOrDefault();
                i.Add(j);
            }

            return i;
        } 

        public class WsmsIssuesModel
        {
            public int IssueID { get; set; }
            public string IssueName { get; set; }
            public string IssueType { get; set; }
        }

        public List<WsmsIssuesModel> GetWsmsIssuesModels()
        {
            var model = new List<WsmsIssuesModel>();
            string sqlconnectionstring = ConfigurationManager.ConnectionStrings["WSMSConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(sqlconnectionstring);
            conn.Open();
            string query = string.Format("select IssueID,IssueName,IssueType from Issues");
            var cmd = new SqlCommand(query, conn);
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                var mod = new WsmsIssuesModel
                {
                    IssueID = (int) rd["IssueID"],
                    IssueName = (string) rd["IssueName"],
                    IssueType = (string) rd["IssueType"]
                };
                model.Add(mod);
            }
            conn.Close();
            return model;
        }
        #endregion

        #region SET

        public void SaveGeneralIncidentCategory(GeneralIncidentModel model)
        {
            Mapper.CreateMap<GeneralIncidentModel, GeneralIncident>();
            var save = Mapper.Map<GeneralIncident>(model);
            _dbeEntities.GeneralIncidents.Add(save);
            _dbeEntities.SaveChanges();
            long incidentId = (from i in _dbeEntities.GeneralIncidents where i.AddedBy == model.AddedBy orderby i.AddedDate descending select i.GeneralIncidentId).FirstOrDefault();
            var log = new GeneralIncidentLogModel
            {
                GeneralIncidentId = incidentId,
                GeneralIncidentTitle = model.GeneralIncidentTitle,
                GeneralIncidentCategories = model.GeneralIncidentCategories,
                GeneralIncidentDetails = model.GeneralIncidentDetails,
                AddedBy = model.AddedBy,
                AddedDate = model.AddedDate,
                RefferedRole = model.RefferedRole,
                Status = model.Status,
                FileUploadPath = model.FileUploadPath,
                ModelName = model.ModelName,
                Issues = model.Issues,
                UpadatedBy = model.UpdatedBy,
                UpdatedDate = model.UpadatedDate
            };
            Mapper.CreateMap<GeneralIncidentLogModel, GeneralIncidentLog>();
            var save2 = Mapper.Map<GeneralIncidentLog>(log);
            _dbeEntities.GeneralIncidentLogs.Add(save2);
            _dbeEntities.SaveChanges();
        }

        public void SaveGeneralIncidentSolutionModel(GeneralIncidentSolutionModel model)
        {
            var solution = _dbeEntities.GeneralIncidentSolutions.Where(x => x.GeneralIncidentId == model.GeneralIncidentId).ToList();
            if (!solution.Any())
            {
                model.AddedRole = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == model.AddedBy).Select(x => x.RoleName).FirstOrDefault();
                model.AddedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == model.AddedBy).Select(x => x.UserFullName).FirstOrDefault();
                Mapper.CreateMap<GeneralIncidentSolutionModel, GeneralIncidentSolution>();
                var save = Mapper.Map<GeneralIncidentSolution>(model);
                _dbeEntities.GeneralIncidentSolutions.Add(save);
                _dbeEntities.SaveChanges();
                string updateStat = string.Format(@"update GeneralIncidents set Status='{0}' where GeneralIncidentId={1}", "PARTIALSUBMIT", model.GeneralIncidentId);
                _dbeEntities.Database.ExecuteSqlCommand(updateStat);
                updateStat = string.Format(@"update GeneralIncidentAssigns set Status='{0}' where GeneralIncidentId={1}", "SUBMITTED", model.GeneralIncidentId);
                _dbeEntities.Database.ExecuteSqlCommand(updateStat);
            }
        }

        public void ForwardIncident(string remark, string forwardrole, long userId = 0, long incidentid = 0)
        {
            var incident = _dbeEntities.GeneralIncidents.FirstOrDefault(x => x.GeneralIncidentId == incidentid);
            if (incident != null)
            {
                var incidentlog = new GeneralIncidentLog
                {
                    GeneralIncidentId = incidentid,
                    GeneralIncidentCategories = incident.GeneralIncidentCategories,
                    GeneralIncidentTitle = incident.GeneralIncidentTitle,
                    GeneralIncidentDetails = incident.GeneralIncidentDetails,
                    AddedBy = incident.AddedBy,
                    AddedDate = incident.AddedDate,
                    ForwaredBy = userId,
                    ForwardRemark = remark,
                    ForwardedDate = DateTime.Now,
                    RefferedRole = forwardrole,
                    Status = "FORWARDED",
                    FileUploadPath = incident.FileUploadPath,
                    ModelName = incident.ModelName,
                    Issues = incident.Issues,
                    UpadatedBy = incident.UpdatedBy,
                    UpdatedDate = incident.UpadatedDate
                };
                _dbeEntities.GeneralIncidentLogs.Add(incidentlog);
            }
            incident.Status = "FORWARDED";
            incident.RefferedRole = forwardrole;
            incident.UpdatedBy = userId;
            incident.UpadatedDate = DateTime.Now;
            _dbeEntities.GeneralIncidents.AddOrUpdate(incident);
            _dbeEntities.SaveChanges();
        }

        public void ReassignIncident(string remark, string reassignrole, long userId = 0, long incidentid = 0)
        {
            //CREATE COPY OF INCIDENT FOR REASSIGN
            var incident = _dbeEntities.GeneralIncidents.FirstOrDefault(x => x.GeneralIncidentId == incidentid);
            var incidentReassign = new GeneralIncident
            {
                GeneralIncidentTitle = incident.GeneralIncidentTitle,
                GeneralIncidentDetails = incident.GeneralIncidentDetails,
                ReassignId = incidentid,
                GeneralIncidentCategories = incident.GeneralIncidentCategories,
                ReassignedBy = userId,
                ReassignDate = DateTime.Now,
                ReassignRemark = remark,
                AddedBy = userId,
                AddedDate = DateTime.Now,
                RefferedRole = reassignrole,
                Status = "NEW",
                FileUploadPath = incident.FileUploadPath
            };
            _dbeEntities.GeneralIncidents.Add(incidentReassign);
            _dbeEntities.SaveChanges();


            var newincident = _dbeEntities.GeneralIncidents.FirstOrDefault(i => i.AddedBy==userId);
            //ENTRY IN LOG TABLE
            var log = new GeneralIncidentLogModel
            {
                GeneralIncidentId = newincident.GeneralIncidentId,
                GeneralIncidentTitle = newincident.GeneralIncidentTitle,
                GeneralIncidentCategories = newincident.GeneralIncidentCategories,
                GeneralIncidentDetails = newincident.GeneralIncidentDetails,
                AddedBy = newincident.AddedBy,
                AddedDate = newincident.AddedDate,
                RefferedRole=newincident.RefferedRole,
                FileUploadPath = newincident.FileUploadPath,
                Status = "REASSIGNED"
            };
            Mapper.CreateMap<GeneralIncidentLogModel, GeneralIncidentLog>();
            var save2 = Mapper.Map<GeneralIncidentLog>(log);
            _dbeEntities.GeneralIncidentLogs.Add(save2);
            _dbeEntities.SaveChanges();

            //UPDATE GENERAL INCIDENT FOR CLOSING
            incident.GeneralIncidentId = incidentid;
            incident.DiscloseDate = DateTime.Now;
            incident.Status = "REASSIGNED";
            _dbeEntities.GeneralIncidents.AddOrUpdate(incident);
            _dbeEntities.SaveChanges();
        }

        public void SaveGeneralIncidentAssignModel(GeneralIncidentAssignModel model)
        {
            Mapper.CreateMap<GeneralIncidentAssignModel, GeneralIncidentAssign>();
            var save = Mapper.Map<GeneralIncidentAssign>(model);
            _dbeEntities.GeneralIncidentAssigns.Add(save);
            _dbeEntities.SaveChanges();
            string updateStat = string.Format(@"update GeneralIncidents set Status='{0}' where GeneralIncidentId={1}","ASSIGNED",model.GeneralIncidentId);
            _dbeEntities.Database.ExecuteSqlCommand(updateStat);
        }

        public List<GeneralIncidentModel> GetDisclosedIncidents()
        {
            var incident = (from i in _dbeEntities.GeneralIncidents
                where i.Status == "DISCLOSED"
                select new GeneralIncidentModel
                {
                    GeneralIncidentId = i.GeneralIncidentId,
                    GeneralIncidentCategories = i.GeneralIncidentCategories,
                    GeneralIncidentTitle = i.GeneralIncidentTitle,
                    GeneralIncidentDetails = i.GeneralIncidentDetails,
                    AddedDate = i.AddedDate,
                    UpadatedDate = i.UpadatedDate,
                    DiscloseDate = i.DiscloseDate,
                    DisclosedBy = i.DisclosedBy,
                    DisclosedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == i.DisclosedBy).Select(x => x.UserFullName).FirstOrDefault(),
                    DiscloseRemark = i.DiscloseRemark,
                    AddedBy = i.AddedBy,
                    RefferedRole = i.RefferedRole,
                    Status = i.Status,
                    ModelName = i.ModelName,
                    Issues = i.Issues,
                    FileUploadPath = i.FileUploadPath,
                    AddedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == i.AddedBy).Select(x => x.UserFullName).FirstOrDefault(),
                    SubmittedDate = i.SubmittedDate,
                    SubmitRemark = i.SubmitRemark,
                    SubmittedBy = i.SubmittedBy,
                    SubmittedByName = _dbeEntities.CmnUsers.Where(x => x.CmnUserId == i.SubmittedBy).Select(x => x.UserFullName).FirstOrDefault()
                }).ToList();
            return incident;
        } 
        #endregion

        #region UPDATE

        public void DiscloseIncident(string remark, long incidentid, long disclosedBy)
        {
            var model = _dbeEntities.GeneralIncidents.FirstOrDefault(x => x.GeneralIncidentId == incidentid);
            model.DiscloseDate = DateTime.Now;
            model.DisclosedBy = disclosedBy;
            model.Status = "DISCLOSED";
            model.DiscloseRemark = remark;
            _dbeEntities.GeneralIncidents.AddOrUpdate(model);
            _dbeEntities.SaveChanges();
        }

        public void UpdateGeneralIncidentModel(string remark, long incidentId, long userId)
        {
            var model = _dbeEntities.GeneralIncidents.FirstOrDefault(x => x.GeneralIncidentId == incidentId);
            model.SubmittedBy = userId;
            model.SubmittedDate = DateTime.Now;
            model.SubmitRemark = remark;
            model.Status = "SUBMITTED";
            _dbeEntities.GeneralIncidents.AddOrUpdate(model);
            _dbeEntities.SaveChanges();
        }
        #endregion
    }
}