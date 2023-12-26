using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using AutoMapper;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Models;

namespace ProjectManagement.Infrastructures.Repositories
{
    public class WarehouseRepository:IWarehouseRepository
    {
        private readonly CellPhoneProjectEntities _dbeEntities;
        //  dbeEntities.Con

        public WarehouseRepository()
        {
            _dbeEntities = new CellPhoneProjectEntities();
            _dbeEntities.Configuration.LazyLoadingEnabled = false;
        }

        public WarehouseReturnImeiModel GetWarehouseReturnImeiByImei1(string imei1)
        {
            var model = new WarehouseReturnImeiModel();
            string sqlconnectionstring = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(sqlconnectionstring);
            conn.Open();
            string query = string.Format("select ddd.*,di.DealerName from tblDealerDistributionDetails ddd inner join tblDealerInfo di on ddd.DealerCode=di.DealerCode where ddd.BarCode='{0}'",imei1);
            var cmd = new SqlCommand(query, conn);
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                model = new WarehouseReturnImeiModel
                {
                    BarCode = Convert.ToString(rd["BarCode"]),
                    BarCode2 = Convert.ToString(rd["BarCode2"]),
                    DealerCode = Convert.ToString(rd["DealerCode"]),
                    Model = Convert.ToString(rd["Model"]),
                    DistributionDate = Convert.ToDateTime(rd["DistributionDate"]).ToShortDateString(),
                    DealerName = Convert.ToString(rd["DealerName"])
                };
            }
            conn.Close();
            return model;
        }

        public string DeleteImei(string imei)
        {
            try
            {
                string sqlconnectionstring = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
                SqlConnection conn = new SqlConnection(sqlconnectionstring);
                conn.Open();
                string query = string.Format("DELETE tblDealerDistributionDetails where BarCode='{0}'", imei);
                var cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public void SaveToReturnImeiLog(ReturnImeiLogModel model)
        {
            Mapper.CreateMap<ReturnImeiLogModel, ReturnImeiLog>();
            var save = Mapper.Map<ReturnImeiLog>(model);
            _dbeEntities.ReturnImeiLogs.Add(save);
            _dbeEntities.SaveChanges();
        }

        public DealerInfoModel GetDealerInfoByDealerCode(string dealercode)
        {
            var model = new DealerInfoModel();
            string sqlconnectionstring = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(sqlconnectionstring);
            conn.Open();
            string query = string.Format("select * from tblDealerInfo where DealerCode='{0}'", dealercode);
            var cmd = new SqlCommand(query, conn);
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                model = new DealerInfoModel
                {
                    DealerCode = Convert.ToString(rd["DealerCode"]),
                    DealerName = Convert.ToString(rd["DealerName"])
                };
            }
            conn.Close();
            return model;
        }

        public WarehouseReturnImeiModel CheckInvalidImei(string imei1)
        {
            var model = new WarehouseReturnImeiModel();
            string sqlconnectionstring = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(sqlconnectionstring);
            conn.Open();
            string query = string.Format("select * from tblBarCodeInv where BarCode='{0}'", imei1);
            var cmd = new SqlCommand(query, conn);
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                model = new WarehouseReturnImeiModel
                {
                    BarCode = Convert.ToString(rd["BarCode"]),
                    BarCode2 = Convert.ToString(rd["BarCode2"]),
                    Model = Convert.ToString(rd["Model"])
                };
            }
            conn.Close();
            return model;
        }

        public void SaveImeiModel(List<WarehouseReturnImeiModel> model)
        {
            string sqlconnectionstring = ConfigurationManager.ConnectionStrings["RbConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(sqlconnectionstring);
            conn.Open();
            foreach (var m in model)
            {
                string date = DateTime.ParseExact(m.DistributionDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                string query = string.Format("Insert into tblDealerDistributionDetails (DealerdistributionId,DealerCode,BarCode,BarCode2,Model,DistributionDate) values ('{0}','{1}','{2}','{3}','{4}','{5}')",
                    m.DealerdistributionId, m.DealerCode, m.BarCode, m.BarCode2, m.Model, date);
                var cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }
    }
}