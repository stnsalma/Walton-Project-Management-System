using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Infrastructures.Repositories;
using ProjectManagement.Models;
using ProjectManagement.ViewModels.CostManagement;

namespace ProjectManagement.Controllers
{
    public class CostManagementController : Controller
    {

        private readonly ICostManagementRepository _costManagementRepository;

        public CostManagementController(CostManagementRepository costManagementRepository)
        {
            _costManagementRepository = costManagementRepository;
        }

        // GET: CostManagement
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PendingPricingList()
        {
            var pendingList = _costManagementRepository.GetPendingPriceProposals();
            return View(pendingList);
        }

        public JsonResult AddPrice(string costMasterId, string proposedPrice)
        {
            long cmId;
            decimal price;
            long.TryParse(costMasterId, out cmId);
            decimal.TryParse(proposedPrice, out price);
            AjaxResponseModel responseModel = _costManagementRepository.SavePriceProposal(cmId, price);
            return new JsonResult { Data = responseModel, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ManagementPricingPending()
        {
            List<VmCostManTBD> models = _costManagementRepository.GetManagementPendingList();
            return View(models);
        }
        

        public ActionResult CostDetailInfo(long id)
        {
            CostMasterModel model = _costManagementRepository.GetCostMasterModel(costMasterId: id);
            return View(model);
        }


        public JsonResult CostSimulation(string simulationType, string simulationValue, string costMasterCogs)
        {
            decimal decSimulationValue, cogs;
            decimal.TryParse(simulationValue, out decSimulationValue);
            decimal.TryParse(WalCrypto.Crypto.Decrypt(costMasterCogs, WalCrypto.Crypto.GetKey()), out cogs);
            if (decSimulationValue > 0)
            {
                if (simulationType.ToLower().Contains("msrp"))
                {
                    CostSimulationLog simulationLog = _costManagementRepository.GetSimulation(msrp: decSimulationValue, perUnitCost: cogs);
                    return new JsonResult { Data = simulationLog, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }

                if (simulationType.ToLower().Contains("gp"))
                {
                    CostSimulationLog simulationLog = _costManagementRepository.GetSimulatedMsrp(gp: decSimulationValue, perUnitCost: cogs);
                    return new JsonResult { Data = simulationLog, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }

            return new JsonResult { Data = new CostSimulationLog{Id = -1}, JsonRequestBehavior = JsonRequestBehavior.AllowGet };;
        }

        public JsonResult SaveSimulation(string txtMsrp, string txtInvicePrice, string txtCogs, string txtGp, string costMasterId)
        {
            bool result = _costManagementRepository.SaveSimulation(txtMsrp, txtInvicePrice, txtCogs, txtGp, costMasterId);
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}