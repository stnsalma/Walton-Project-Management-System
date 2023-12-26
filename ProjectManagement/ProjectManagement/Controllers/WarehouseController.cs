using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Infrastructures.Repositories;
using ProjectManagement.Models;

namespace ProjectManagement.Controllers
{
    [Authorize(Roles = "WAR,WARHEAD")]
    public class WarehouseController:Controller
    {
        private CellPhoneProjectEntities db = new CellPhoneProjectEntities();
        private IWarehouseRepository _repository;

        public WarehouseController(WarehouseRepository repository)
        {
            _repository = repository;
        }


        [Authorize(Roles = "BAAL")]
        public ActionResult DeleteImei()
        {
            return View();
        }

        public JsonResult CheckImei(string imei1)
        {
            var model = _repository.GetWarehouseReturnImeiByImei1(imei1);
            return new JsonResult { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult ReturnImei(List<WarehouseReturnImeiModel> imeilist)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var json = "";
            foreach (var i in imeilist)
            {
                var model = new ReturnImeiLogModel
                {
                    IMEI = i.BarCode,
                    IMEI2 = i.BarCode2,
                    Model = i.Model,
                    DistributorName = i.DealerName,
                    DistributionDate = i.DistributionDate,
                    DealerCode = i.DealerCode,
                    AddedBy = userId,
                    AddedDate = DateTime.Now
                };
                _repository.SaveToReturnImeiLog(model);
                json = _repository.DeleteImei(i.BarCode);
            }
            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize(Roles = "BAAL")]
        public ActionResult InsertImei()
        {
            return View();
        }

        public JsonResult GetDealerInfo(string dealercode)
        {
            var model = _repository.GetDealerInfoByDealerCode(dealercode);
            return Json(model);
        }

        public JsonResult IsExist(string imei1)
        {
            var model = _repository.GetWarehouseReturnImeiByImei1(imei1);
            if (model.BarCode != null)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }

        public JsonResult CheckInvalidImei(string imei1)
        {
            var model = _repository.CheckInvalidImei(imei1);
            return Json(model);
        }

        public JsonResult SaveImei(List<WarehouseReturnImeiModel> imeilist, string dealercode, string distdate)
        {
            foreach (var m in imeilist)
            {
                m.DealerdistributionId = Guid.NewGuid();
                m.DealerCode = dealercode;
                m.DistributionDate = distdate;
            }
            _repository.SaveImeiModel(imeilist);
            return Json(true);
        }
    }
}