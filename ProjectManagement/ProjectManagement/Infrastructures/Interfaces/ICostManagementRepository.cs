using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Models;
using ProjectManagement.ViewModels.CostManagement;

namespace ProjectManagement.Infrastructures.Interfaces
{
    public interface ICostManagementRepository
    {
        List<VmCostManTBD> GetPendingPriceProposals();
        AjaxResponseModel SavePriceProposal(long cmId, decimal price);
        List<VmCostManTBD> GetManagementPendingList();
        CostMasterModel GetCostMasterModel(long costMasterId);
        CostSimulationLog GetSimulation(decimal msrp, decimal perUnitCost);
        CostSimulationLog GetSimulatedMsrp(decimal gp, decimal perUnitCost);
        bool SaveSimulation(string txtMsrp, string txtInvicePrice, string txtCogs, string txtGp, string costMasterId);
    }
}
