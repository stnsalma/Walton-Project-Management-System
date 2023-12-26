using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Infrastructures.Repositories;
using ProjectManagement.Models;
using ProjectManagement.ViewModels.Spare;

namespace ProjectManagement.Controllers
{
    [Authorize(Roles = "SPR,SPRHEAD")]
    public class SpareController : Controller
    {
        private ISpareRepository _repository;
        private readonly ICommonRepository _commonRepository;
        private readonly ICommercialRepository _commercialRepository;
        private readonly IHardwareRepository _hardwareRepository;

        public SpareController(SpareRepository repository, CommercialRepository commercialRepository, HardwareRepository hardwareRepository)
        {
            _repository = repository;
            _commercialRepository = commercialRepository;
            _hardwareRepository = hardwareRepository;
        }

        // GET: Sapre
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SpareOrder()
        {
            var model = new SpareViewModel();
            //model.SpareNameModels = _repository.GetSpareNameModels();
            ViewBag.ProjectNamesWithPo = _repository.GetAllProjectNamesWithPo();
            return View(model);
        }

        public JsonResult LoadDefaultSpare(string sparetype)
        {
            var spares = _repository.GetSpareNameModels(sparetype);
            return new JsonResult { Data = spares, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult PostSpareName(string spareName, string pir, string sparetype)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);

            var model = new SpareNameModel
            {
                SparePartsName = spareName,
                ProposedImportRatio = pir,
                AddedBy = userId,
                AddedDate = DateTime.Now,
                SpareType = sparetype
            };

            var json = _repository.SaveSpareName(model);
            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult UpdateSpareOrder(string spareName, string quantity, string pir, string remarks, long spareOrderId = 0,
            long spareId = 0)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            var data = _repository.UpdateSpareOrder(spareOrderId, spareId, spareName, quantity, pir, remarks, userId);
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetSpareNameModels(string sparetype)
        {
            var model = _repository.GetSpareNameModels(sparetype);
            return new JsonResult { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetSpareOrderByProjectAndOrder(long projectId = 0)
        {
            var spareorder = _repository.GetSpareOrderByPorjectId(projectId);
            string jsonStr = JsonConvert.SerializeObject(spareorder);
            return new JsonResult { Data = jsonStr, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public JsonResult SubmitSpare(string piDate, string remark, long projectmasterid = 0)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            _repository.SubmitSpareOrderToCommercial(projectmasterid, piDate, remark,userId);
            return Json(new
            {
                redirectUrl = Url.Action("SpareOrder", "Spare"),
                isRedirect = true
            });
        }

        public JsonResult PostSpareOrder(string projectName, string orderNumber, string handsetquantity, List<SpareOrderListObject> orderListObjects, long projectmasterid = 0)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            try
            {
                foreach (var item in orderListObjects)
                {
                    var spareorder = new SpareOrderModel
                    {
                        ProjectName = projectName,
                        OrderNumber = orderNumber,
                        SpareId = item.SpareId,
                        SparePartsName = item.SpareName,
                        Quantity = item.Quantity,
                        ProposedImportRatio = item.ProposedImportRatio,
                        AddedBy = userId,
                        AddedDate = DateTime.Now,
                        HandsetQuantity = handsetquantity,
                        ProjectMasterId = projectmasterid,
                        Remarks = item.Remarks
                    };
                    _repository.SaveSpareOrder(spareorder);
                }

                //var jsonstr = _repository.GetLastSapreOrder(projectName, orderNumber);
                return new JsonResult {Data = true, JsonRequestBehavior = JsonRequestBehavior.AllowGet};
            }
            catch (Exception ex)
            {
                return new JsonResult { Data = ex, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        public JsonResult CheckSubmission(long projectid = 0)
        {
            var po = _repository.GetProjectPurchaseOrderFormById(projectid);
            return new JsonResult { Data = po, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult MultipleModelSpareOrder()
        {
            var model = new SpareViewModel();
            model.SpareNameModels = _repository.GetSpareNameModels("smart");
            ViewBag.ProjectNamesWithPo = _repository.GetAllProjectNamesWithPo();
            ViewBag.SpareOrderByMultipleModel = _repository.GetSpareOrderByMultipleModelModels();
            return View(model);
        }

        public JsonResult GetOrderNumbersByProjectNameWithPo(string projectname)
        {
            var orders = _repository.GetOrderNumbersByProjectNameWithPo(projectname);
            var items = (from t in orders
                         let ordinal = CommonConversion.AddOrdinal(t.OrderNuber) + " Order"
                         select new SelectListItem
                         {
                             Text = ordinal,
                             Value = t.ProjectMasterId.ToString(CultureInfo.InvariantCulture)
                         }).ToList();

            string jsonStr = JsonConvert.SerializeObject(items);
            return new JsonResult { Data = jsonStr, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult PostSpareForMultipleModel(string projectnames, string sparename, string orderquantity, string materialcode, string price, string amount)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            var spareorderByMultipleModel = new SpareOrderByMultipleModelModel
            {
                ModelNames = projectnames,
                SpareName = sparename,
                OrderQuantity = Convert.ToInt64(orderquantity),
                MaterialCode = materialcode,
                Price = Convert.ToDouble(price),
                Amount = Convert.ToDouble(amount),
                AddedBy = userId,
                AddedDate = DateTime.Now
            };
            var json = _repository.SaveSpareOrderByMultipleModels(spareorderByMultipleModel);
            return new JsonResult { Data = json, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult SpareList()
        {
            var spares = _repository.GetAllSpareNames();
            return View(spares);
        }

        public JsonResult UpdateSpare(string pir, long spareId = 0)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            if (spareId != 0)
            {
                var spare = _repository.GetSpareNameById(spareId);
                spare.ProposedImportRatio = pir;
                spare.UpdatedBy = userId;
                spare.UpdatedDate = DateTime.Now;
                var update = _repository.UpdateSpareName(spare);
                return Json(update);
            }
            return Json("Something went wrong!!!");
        }
    }
}