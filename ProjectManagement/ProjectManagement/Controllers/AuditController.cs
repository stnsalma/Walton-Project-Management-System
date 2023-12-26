using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Infrastructures.Repositories;
using ProjectManagement.Models;

namespace ProjectManagement.Controllers
{
    [Authorize(Roles = "AUD,AUDHEAD,SA,BIHEAD")]
    public class AuditController : Controller
    {
        private readonly ICommercialRepository _commercialRepository;

        public AuditController(CommercialRepository commercialRepository)
        {
            _commercialRepository = commercialRepository;
        }
        
        public ActionResult LcList(int type = 0, string msg = null)
        {
            List<ProjectLcModel> lcModels = _commercialRepository.GetProjectLcModels();
            return View(lcModels);
        }

        public ActionResult PoList()
        {
            var poList = _commercialRepository.GetAllPoList();
            return View(poList);
        }
	}
}