using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Infrastructures.Repositories;
using ProjectManagement.Models;

namespace ProjectManagement.Controllers
{
    [Authorize(Roles = "CPSD,CPSDHEAD,ASPM,ASPMHEAD")]
    public class CpsdController : Controller
    {
        private CellPhoneProjectEntities db = new CellPhoneProjectEntities();
        private ICpsdRepository _repository;
        private IHardwareRepository _hwRepository;

        public CpsdController(CpsdRepository repository,HardwareRepository hardwareRepository)
        {
            _repository = repository;
            _hwRepository = hardwareRepository;
        }
        // GET: Cpsd
        public ActionResult ServiceToSalesRatioMonitor()
        {
            ViewBag.ServiceToSalesRatioWarningMail = _repository.GetServiceToSalesRatioWarningMailModels();
            return View();
        }

       
        public JsonResult ServiceToSalesRatioSolved(string solution, long id = 0)
        {
            long userId = Convert.ToInt64(User.Identity.Name);
            var userInfo = _hwRepository.GetUserInfoByUserId(userId);
            var ratioById = _repository.GetServiceToSalesRatioWarningMailModelById(id);
            _repository.UpdateServiceToSalesRatioMonitor(solution,userInfo.UserFullName, id);
            return Json(new
            {
                redirectUrl = Url.Action("ServiceToSalesRatioMonitor", "Cpsd"),
                isRedirect = true
            });
        }
    }
}