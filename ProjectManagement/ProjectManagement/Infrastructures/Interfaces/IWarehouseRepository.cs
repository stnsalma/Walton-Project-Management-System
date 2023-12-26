using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.Infrastructures.Interfaces
{
    interface IWarehouseRepository
    {
        WarehouseReturnImeiModel GetWarehouseReturnImeiByImei1(string imei1);

        string DeleteImei(string imei);
        void SaveToReturnImeiLog(ReturnImeiLogModel model);
        DealerInfoModel GetDealerInfoByDealerCode(string dealercode);
        WarehouseReturnImeiModel CheckInvalidImei(string imei1);
        void SaveImeiModel(List<WarehouseReturnImeiModel> model);
    }
}