using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Infrastructures.Interfaces;
using System.Web.Mvc;
using ProjectManagement.Infrastructures.Repositories;
using ProjectManagement.Models;

namespace ProjectManagement.Controllers
{
    [Authorize(Roles = "MKT,MKTHEAD,CM,CMHEAD,SA,PS")]
    public class MarketingController : Controller
    {
        private readonly IMarketingRepository _marketingRepository;

        public MarketingController(MarketingRepository repository)
        {
            _marketingRepository = repository;
        }
        // GET: Marketing
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Spec(long id = 0)
        {
            ViewBag.Brands = _marketingRepository.GetMkOtherBrands();
            var model = _marketingRepository.GetMkProjectSpecModelById(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Spec(MkProjectSpecModel model)
        {
            long userId = Convert.ToInt64(User.Identity.Name);

            if (model.Id > 0)
            {
                model.UpdatedBy = userId;
                model.UpdatedDate = DateTime.Now;
                _marketingRepository.UpdateMkProjectSpec(model);
                return RedirectToAction("Spec", new { id = model.Id });
            }
            model.AddedBy = userId;
            model.AddedDate = DateTime.Now;
            _marketingRepository.SaveMkProjectSpec(model);
            return RedirectToAction("AllModelList");
        }

        public ActionResult AllModelList()
        {
            var model = _marketingRepository.GetMkProjectSpecModels();
            return View(model);
        }

        public JsonResult CommImportPrice(string commercialImportPrice, long id = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var model = _marketingRepository.GetMkProjectSpecModelById(id);
            if (model.Id > 0)
            {
                model.CommercialImportPrice = commercialImportPrice;
                model.UpdatedBy = userId;
                model.UpdatedDate = DateTime.Now;
                _marketingRepository.UpdateMkProjectSpec(model);
            }
            return Json(true);
        }

        public ActionResult MarketOrderQuantityDetails(long id = 0)
        {
            var tempMessage = TempData["Duplicate"] == null ? "blank" : TempData["Duplicate"].ToString();
            ViewBag.Message = tempMessage;
            ViewBag.ForeignModels = _marketingRepository.GetMkProjectSpecModels();
            if (id > 0)
            {
                var model = _marketingRepository.GetMarketOrderQuantityDetailById(id);
                return View(model);
            }
            return View();
        }

        [HttpPost]
        public ActionResult MarketOrderQuantityDetails(MkMarketOrderQuantityDetailModel model)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            if (model.Id > 0)
            {
                model.UpdatedBy = userId;
                model.UpdatedDate = DateTime.Now;
                var v = _marketingRepository.UpdateMarketOrderQuantityDetail(model);
                return RedirectToAction("MarketOrderQuantityDetails", new { id = v.Id });
            }
            if (model.Id == 0)
            {
                if (_marketingRepository.SameOrderCheck(model.MkProjectSpecId, model.OrderNumber) == null)
                {
                    model.AddedBy = userId;
                    model.AddedDate = DateTime.Now;
                    var v = _marketingRepository.SaveMarketOrderQuantityDetail(model);
                    return RedirectToAction("MarketOrderQuantityDetails", new { id = v.Id });
                }
                TempData["Duplicate"] = "this order already exists.";
                return RedirectToAction("MarketOrderQuantityDetails");
            }
            TempData["Duplicate"] = "Something went wrong.";
            return RedirectToAction("MarketOrderQuantityDetails");
        }

        public ActionResult MarketOrderList()
        {
            var model = _marketingRepository.GetMarketOrderQuantityDetailModels();
            return View(model);
        }
    }
}