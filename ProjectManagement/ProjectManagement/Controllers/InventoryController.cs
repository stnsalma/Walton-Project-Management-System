using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Infrastructures.Repositories;

namespace ProjectManagement.Controllers
{
    [Authorize(Roles = "INV,INVHEAD")]
    public class InventoryController : Controller
    {
        private readonly IInventoryRepository _inventoryRepository;
        public readonly ICommonRepository _CommonRepository;
        public InventoryController(InventoryRepository inventoryRepository,CommonRepository commonRepository)
        {
            _CommonRepository = commonRepository;
            _inventoryRepository = inventoryRepository;
        }
        //
        // GET: /Inventory/
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult SaveReceiveInfo(string receiveQuantity, string receiveRemarks, long id = 0)
        {
            var response = _inventoryRepository.SaveFocClaimBomDetailModel(receiveQuantity, receiveRemarks, id);
            return Json(response);
        }

        public ActionResult ReceiveReturnedSamples()
        {
            var sample = _CommonRepository.GetSampleTrackerToReceive();
            return View(sample);
        }

        public JsonResult ReceiveReturnedSample(string remarks, long id = 0)
        {
            long userId;
            long.TryParse(HttpContext.User.Identity.Name, out userId);
            var sample = _CommonRepository.GetSampleTrackerById(id);
            sample.InventoryReceiveDate = DateTime.Now;
            sample.InventoryReceivedBy = userId;
            sample.InventoryReceiveRemarks = remarks;
            sample = _CommonRepository.UpdateSampleTracker(sample);
            MailSendFromPms mailSendFromPms = new MailSendFromPms();
            mailSendFromPms.SendMail(new List<long>(new[] { Convert.ToInt64(sample.InventoryReturnedBy) }),
        new List<string>(new[] { "SA" }), "Sample Rceived by Inventory", "This is to inform you that, samples of " + sample.Model + " received by " + sample.InventoryReceivedByName + ".");
            return Json(sample);
        }
	}
}