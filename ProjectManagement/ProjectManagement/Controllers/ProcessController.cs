using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Infrastructures.Repositories;

namespace ProjectManagement.Controllers
{
    [Authorize(Roles = "PRC,PRCHEAD")]
    public class ProcessController : Controller
    {
        private readonly IProcessRepository _processRepository;
        private readonly CellPhoneProjectEntities _dbEntities;

        public ProcessController(ProcessRepository processRepository)
        {
            _processRepository = processRepository;
            _dbEntities.Configuration.LazyLoadingEnabled = false;
        }
        //
        // GET: /Process/
        public ActionResult Index()
        {
            return View();
        }
	}
}