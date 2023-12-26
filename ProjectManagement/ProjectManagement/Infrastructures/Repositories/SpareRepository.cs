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
    public class SpareRepository : ISpareRepository
    {
        private readonly CellPhoneProjectEntities _dbEntities;

        public SpareRepository()
        {
            _dbEntities = new CellPhoneProjectEntities();
            _dbEntities.Configuration.LazyLoadingEnabled = false;
        }

        #region GET
        public List<ProjectPurchaseOrderFormModel> GetProjectsWithPo()
        {
            string query = string.Format(@"select ppf.ProjectMasterId,pm.ProjectName,pm.OrderNuber,ppf.PurchaseOrderNumber,ppf.PoDate,ppf.AddedDate from ProjectMasters 
                                           pm inner join ProjectPurchaseOrderForms ppf on pm.ProjectMasterId=ppf.ProjectMasterId");
            var exe = _dbEntities.Database.SqlQuery<ProjectPurchaseOrderFormModel>(query).ToList();
            return exe;
        }

        public List<ProjectMasterModel> GetAllProjectNamesWithPo()
        {
            string query = string.Format(@"select pm.projectname,pm.ProjectType from ProjectMasters pm
                                           inner join ProjectPurchaseOrderForms ppf on pm.ProjectMasterId=ppf.ProjectMasterId
										   where IsActive=1 group by pm.ProjectName,pm.ProjectType order by pm.ProjectName");
            var allProjectNames = _dbEntities.Database.SqlQuery<ProjectMasterModel>(query).ToList();
            return allProjectNames;
        }

        public List<SpareNameModel> GetSpareNameModels(string sparetype)
        {
            string query = string.Format(@"select * from SpareNames where SpareType='{0}'", sparetype);
            var exe = _dbEntities.Database.SqlQuery<SpareNameModel>(query).ToList();
            return exe;
        }

        public List<SpareNameModel> GetAllSpareNames()
        {
            string query = string.Format(@"select * from SpareNames");
            var exe = _dbEntities.Database.SqlQuery<SpareNameModel>(query).ToList();
            return exe;
        }

        public SpareNameModel GetSpareNameById(long spareId)
        {
            string query = string.Format(@"select * from SpareNames where SpareId={0}",spareId);
            var exe = _dbEntities.Database.SqlQuery<SpareNameModel>(query).FirstOrDefault();
            return exe;
        }

        public List<SpareOrderModel> GetSpareOrderByPorjectId(long projectId)
        {
            string query = string.Format(@"select sp.*,ppf.PiDate,ppf.IsSpareSubmittedDate,ppf.IsSpareConfirmedDate from SpareOrders sp
                                           inner join ProjectPurchaseOrderForms ppf on sp.ProjectMasterId=ppf.ProjectMasterId
                                           where sp.ProjectMasterId={0}", projectId);
            var exe = _dbEntities.Database.SqlQuery<SpareOrderModel>(query).ToList();
            return exe;
        }

        public SpareOrderModel GetLastSapreOrder(string projectName, string orderNumber)
        {
            string query = string.Format(@"select * from SpareOrders where ProjectName='{0}' and OrderNumber='{1}' order by AddedDate desc", projectName, orderNumber);
            var exe = _dbEntities.Database.SqlQuery<SpareOrderModel>(query).FirstOrDefault();
            return exe;
        }

        public ProjectPurchaseOrderFormModel GetProjectPurchaseOrderFormById(long projectid)
        {
            string query = string.Format(@"select * from ProjectPurchaseOrderForms where ProjectMasterId={0}", projectid);
            var exe = _dbEntities.Database.SqlQuery<ProjectPurchaseOrderFormModel>(query).FirstOrDefault();
            return exe;
        }

        public List<ProjectMasterModel> GetOrderNumbersByProjectNameWithPo(string projectname)
        {
            string query = string.Format(@"select distinct pm.projectmasterid,pm.OrderNuber from ProjectMasters pm inner join ProjectPurchaseOrderForms ppf on pm.ProjectMasterId=ppf.ProjectMasterId  where ProjectName = '{0}'", projectname);
            var execute = _dbEntities.Database.SqlQuery<ProjectMasterModel>(query).ToList();
            return execute;
        }

        public List<SpareOrderByMultipleModelModel> GetSpareOrderByMultipleModelModels()
        {
            string query = string.Format("select * from SpareOrderByMultipleModels");
            var exe = _dbEntities.Database.SqlQuery<SpareOrderByMultipleModelModel>(query).ToList();
            return exe;
        }

        #endregion

        #region SET
        public SpareNameModel SaveSpareName(SpareNameModel model)
        {
            Mapper.CreateMap<SpareNameModel, SpareName>();
            var sparename = Mapper.Map<SpareName>(model);
            _dbEntities.SpareNames.Add(sparename);
            _dbEntities.SaveChanges();
            string query = string.Format(@"select * from SpareNames where SpareType='{0}' order by AddedDate desc", model.SpareType);
            var exe = _dbEntities.Database.SqlQuery<SpareNameModel>(query).FirstOrDefault();
            return exe;
        }

        public void SaveSpareOrder(SpareOrderModel model)
        {
            Mapper.CreateMap<SpareOrderModel, SpareOrder>();
            var spareorder = Mapper.Map<SpareOrder>(model);
            _dbEntities.SpareOrders.Add(spareorder);
            _dbEntities.SaveChanges();
        }

        public SpareOrderByMultipleModelModel SaveSpareOrderByMultipleModels(SpareOrderByMultipleModelModel model)
        {
            Mapper.CreateMap<SpareOrderByMultipleModelModel, SpareOrderByMultipleModel>();
            var multiplespareorder = Mapper.Map<SpareOrderByMultipleModel>(model);
            _dbEntities.SpareOrderByMultipleModels.Add(multiplespareorder);
            _dbEntities.SaveChanges();
            string query = string.Format(@"select top 1 * from SpareOrderByMultipleModels order by AddedDate desc");
            var exe = _dbEntities.Database.SqlQuery<SpareOrderByMultipleModelModel>(query).FirstOrDefault();
            return exe;
        }

        #endregion

        #region UPDATE

        public void SubmitSpareOrderToCommercial(long projectid, string pidate, string remark,long spareSubmittedBy)
        {
            string query = string.Format(@"update ProjectPurchaseOrderForms  set IsSpareSubmittedDate=GETDATE(),PiDate='{1}',IsSpareSubmittedRemark='{2}',SpareSubmittedBy={3} where ProjectMasterId={0}", projectid, pidate, remark,spareSubmittedBy);
            _dbEntities.Database.ExecuteSqlCommand(query);
        }

        public int UpdateSpareOrder(long spareorderId, long spareId, string spareName, string quantity, string pir, string remarks, long userId)
        {
            string query =
                string.Format(
                    @"UPDATE SpareOrders SET SpareId={1},SparePartsName='{2}',Quantity='{3}',ProposedImportRatio='{4}', Remarks='{6}',UpdatedBy={5},UpdatedDate=GETDATE() WHERE SpareOrderId={0}", spareorderId, spareId, spareName, quantity, pir, userId, remarks);
            var exe = _dbEntities.Database.ExecuteSqlCommand(query);
            return exe;
        }

        public bool UpdateSpareName(SpareNameModel spare)
        {
            try
            {
                Mapper.CreateMap<SpareNameModel, SpareName>();
                var sparename = Mapper.Map<SpareName>(spare);
                _dbEntities.SpareNames.AddOrUpdate(sparename);
                _dbEntities.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}