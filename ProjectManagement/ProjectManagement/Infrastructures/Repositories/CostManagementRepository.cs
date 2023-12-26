using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using AutoMapper;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.Office.Interop.Excel;
using ProjectManagement.DAL;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Infrastructures.Helper;
using ProjectManagement.Infrastructures.Interfaces;
using ProjectManagement.Models;
using ProjectManagement.ViewModels.CostManagement;

namespace ProjectManagement.Infrastructures.Repositories
{
    public class CostManagementRepository:ICostManagementRepository
    {
        private readonly CellPhoneProjectEntities _cellPhoneProjectEntities;
        public CostManagementRepository()
        {
            _cellPhoneProjectEntities = new CellPhoneProjectEntities();
        }

        public List<VmCostManTBD> GetPendingPriceProposals()
        {
            var userId = Convert.ToInt64(HttpContext.Current.User.Identity.Name);
            List<VmCostManTBD> pendingList = (from costMaster in _cellPhoneProjectEntities.CostMasters
                join costManagementModel in _cellPhoneProjectEntities.CostManagementModels on costMaster
                    .CostManagementModelId equals costManagementModel.Id
                join orderQuantityDetail in _cellPhoneProjectEntities.ProjectOrderQuantityDetails on costManagementModel
                    .OrderQuantityDetailsId equals orderQuantityDetail.Id
                join master in _cellPhoneProjectEntities.ProjectMasters on orderQuantityDetail.ProjectMasterId equals
                    master.ProjectMasterId
                where costMaster.IsCompleted == null || costMaster.IsCompleted == false
                select new VmCostManTBD
                {
                    CostMasterId = costMaster.Id,
                    CostManagementModelId = costManagementModel.Id,
                    ChipsetName = master.ChipsetName,
                    CpuName = master.CpuName,
                    DisplaySize = master.DisplaySize,
                    OperatingSystem = master.OsName,
                    OrderNumber = master.OrderNuber,
                    ProjectMasterId = master.ProjectMasterId,
                    ProjectName = master.ProjectName,
                    ProjectType = master.ProjectType,
                    Ram = master.Ram,
                    Rom = master.Rom,
                    SupplierName = master.SupplierName
                    
                }).ToList();

            foreach (var vmCost in pendingList)
            {
                List<CostProposal> proposals = _cellPhoneProjectEntities.CostProposals
                    .Where(i => i.CostMasterId == vmCost.CostMasterId && i.ProposedBy == userId).ToList();
                foreach (var proposal in proposals)
                {
                    vmCost.PreviousPrices += WalCrypto.Crypto.Decrypt(proposal.PriceProposal, WalCrypto.Crypto.GetKey()) + ", ";

                }

                if (vmCost.PreviousPrices != null && vmCost.PreviousPrices.EndsWith(","))
                {
                    vmCost.PreviousPrices = vmCost.PreviousPrices.Substring(0, vmCost.PreviousPrices.Length - 1);
                }
                vmCost.ProposalCount = proposals.Count;
            }

            return pendingList;
        }

        public AjaxResponseModel SavePriceProposal(long cmId, decimal price)
        {
            var returnModel = new AjaxResponseModel();
            
            try
            {
                var userInfo = Convert.ToInt64(HttpContext.Current.User.Identity.Name);
                CmnUser user = _cellPhoneProjectEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == userInfo);
                CostProposal proposal = new CostProposal
                {
                    CostMasterId = cmId,
                    PriceProposal = WalCrypto.Crypto.Encrypt(price.ToString(CultureInfo.InvariantCulture),
                        WalCrypto.Crypto.GetKey()),
                    ProposedBy = userInfo,
                    ProposedDate = DateTime.Now,
                    IsCancelled = false
                };
                _cellPhoneProjectEntities.CostProposals.Add(proposal);
                _cellPhoneProjectEntities.SaveChanges();

                returnModel.Id = 1;
                returnModel.Message = "Price Proposal Success !!!";
            }
            catch (Exception e)
            {
                string msg = e.Message;
                if (e.InnerException != null && e.InnerException.InnerException != null)
                {
                    msg = msg + ". " + e.InnerException.InnerException.Message;
                }

                returnModel.Id = 0;
                returnModel.Message = msg;
            }


            return returnModel;
        }

        public List<VmCostManTBD> GetManagementPendingList()
        {
            List<VmCostManTBD> pendingList = (from costMaster in _cellPhoneProjectEntities.CostMasters
                                              join costManagementModel in _cellPhoneProjectEntities.CostManagementModels on costMaster
                                                  .CostManagementModelId equals costManagementModel.Id
                                              join orderQuantityDetail in _cellPhoneProjectEntities.ProjectOrderQuantityDetails on costManagementModel
                                                  .OrderQuantityDetailsId equals orderQuantityDetail.Id
                                              join master in _cellPhoneProjectEntities.ProjectMasters on orderQuantityDetail.ProjectMasterId equals
                                                  master.ProjectMasterId
                                              where costMaster.IsCompleted == null || costMaster.IsCompleted == false
                                              select new VmCostManTBD
                                              {
                                                  CostMasterId = costMaster.Id,
                                                  CostManagementModelId = costManagementModel.Id,
                                                  ChipsetName = master.ChipsetName,
                                                  CpuName = master.CpuName,
                                                  DisplaySize = master.DisplaySize,
                                                  OperatingSystem = master.OsName,
                                                  OrderNumber = master.OrderNuber,
                                                  ProjectMasterId = master.ProjectMasterId,
                                                  ProjectName = master.ProjectName,
                                                  ProjectType = master.ProjectType,
                                                  Ram = master.Ram,
                                                  Rom = master.Rom,
                                                  SupplierName = master.SupplierName

                                              }).ToList();

            

            return pendingList;
        }

        public CostMasterModel GetCostMasterModel(long costMasterId)
        {
            CostMaster costMaster = _cellPhoneProjectEntities.CostMasters.FirstOrDefault(i => i.Id == costMasterId);

            Mapper.Initialize(config =>
            {
                config.CreateMap<CostMaster, CostMasterModel>();
                config.CreateMap<CostMasterDetail, CostMasterDetailModel>();
                config.CreateMap<CostProposal, CostProposalModel>();
                config.CreateMap<CostSimulationLog, CostSimulationLogModel>();
            });

            var model = Mapper.Map<CostMaster, CostMasterModel>(costMaster);

            foreach (var masterDetail in model.CostMasterDetails)
            {
                masterDetail.COGS = Convert.ToDecimal(WalCrypto.Crypto.Decrypt(masterDetail.COGS, WalCrypto.Crypto.GetKey())).ToString("0.00");
                masterDetail.GP = Convert.ToDecimal(WalCrypto.Crypto.Decrypt(masterDetail.GP, WalCrypto.Crypto.GetKey())).ToString("0.00");
                masterDetail.Invoice = Convert.ToDecimal(WalCrypto.Crypto.Decrypt(masterDetail.Invoice, WalCrypto.Crypto.GetKey())).ToString("0.00");
                masterDetail.InvoiceAfterRebate = Convert.ToDecimal(WalCrypto.Crypto.Decrypt(masterDetail.InvoiceAfterRebate, WalCrypto.Crypto.GetKey())).ToString("0.00");
                masterDetail.MSRP = Convert.ToDecimal(WalCrypto.Crypto.Decrypt(masterDetail.MSRP, WalCrypto.Crypto.GetKey())).ToString("0.00");
                masterDetail.MSRP_IP_Percent = Convert.ToDecimal(WalCrypto.Crypto.Decrypt(masterDetail.MSRP_IP_Percent, WalCrypto.Crypto.GetKey())).ToString("0.00");
                masterDetail.Rebate = Convert.ToDecimal(WalCrypto.Crypto.Decrypt(masterDetail.Rebate, WalCrypto.Crypto.GetKey())).ToString("0.00");

            }

            foreach (var logModel in model.CostSimulationLogs)
            {
                logModel.COGS = Convert.ToDecimal(WalCrypto.Crypto.Decrypt(logModel.COGS, WalCrypto.Crypto.GetKey())).ToString("0.00");
                logModel.GP = Convert.ToDecimal(WalCrypto.Crypto.Decrypt(logModel.GP, WalCrypto.Crypto.GetKey())).ToString("0.00");
                logModel.Invoice = Convert.ToDecimal(WalCrypto.Crypto.Decrypt(logModel.Invoice, WalCrypto.Crypto.GetKey())).ToString("0.00");
                logModel.InvoiceAfterRebate = Convert.ToDecimal(WalCrypto.Crypto.Decrypt(logModel.InvoiceAfterRebate, WalCrypto.Crypto.GetKey())).ToString("0.00");
                logModel.MSRP = Convert.ToDecimal(WalCrypto.Crypto.Decrypt(logModel.MSRP, WalCrypto.Crypto.GetKey())).ToString("0.00");
                logModel.MSRP_IP_Percent = Convert.ToDecimal(WalCrypto.Crypto.Decrypt(logModel.MSRP_IP_Percent, WalCrypto.Crypto.GetKey())).ToString("0.00");
                logModel.Rebate = Convert.ToDecimal(WalCrypto.Crypto.Decrypt(logModel.Rebate, WalCrypto.Crypto.GetKey())).ToString("0.00");

            }


            foreach (var proposal in model.CostProposals)
            {
                proposal.PriceProposal = WalCrypto.Crypto.Decrypt(proposal.PriceProposal, WalCrypto.Crypto.GetKey());
                var user = _cellPhoneProjectEntities.CmnUsers.FirstOrDefault(i => i.CmnUserId == proposal.ProposedBy);
                if (user == null) continue;
                {
                    proposal.ProposalByName = user.UserFullName;
                    proposal.RoleDetailName = _cellPhoneProjectEntities.CmnRoles
                        .FirstOrDefault(i => i.RoleName == user.RoleName).RoleDescription;
                }
            }
            var a = model.CostProposals.GroupBy(l => new { l.ProposalByName, l.RoleDetailName})
                .Select(g => new { g.Key.ProposalByName, g.Key.RoleDetailName,  Prices = string.Join(", ", g.Select(i => i.PriceProposal)) });

            model.CostProposals = a.Select(x => new CostProposalModel
            {
                ProposalByName = x.ProposalByName,
                RoleDetailName = x.RoleDetailName,
                PriceProposal = x.Prices
            }).ToList();
            return model;
        }

        public CostSimulationLog GetSimulation(decimal msrp, decimal perUnitCost)
        {
            decimal invoicePrice = (90 * msrp) / 100;
            decimal invoiceAfterRebate = invoicePrice;
            decimal gp = ((invoiceAfterRebate - perUnitCost) / invoiceAfterRebate) * 100;
            decimal mrp = msrp;


            CostSimulationLog log = new CostSimulationLog
            {
                Invoice = invoicePrice.ToString("0.00"),
                InvoiceAfterRebate = invoiceAfterRebate.ToString("0.00"),
                MSRP = mrp.ToString("0.00"),
                COGS = perUnitCost.ToString("0.00"),
                GP = gp.ToString("0.00"),
                MSRP_IP_Percent = 10.ToString(),
                Rebate = 0.ToString()
            };

            return log;
        }

        public CostSimulationLog GetSimulatedMsrp(decimal gp, decimal perUnitCost)
        {
            decimal gpp = gp;
            decimal costing = perUnitCost;
            decimal invoicePrice = (100 * costing) / (100 - gp);
            decimal initalMsrp = invoicePrice + ((decimal)0.10 * invoicePrice);
            CostSimulationLog log = new CostSimulationLog
            {
                Invoice = invoicePrice.ToString("0.00"),
                InvoiceAfterRebate = invoicePrice.ToString("0.00"),
                MSRP = initalMsrp.ToString("0.00"),
                COGS = perUnitCost.ToString("0.00"),
                GP = gp.ToString("0.00"),
                MSRP_IP_Percent = 10.ToString(),
                Rebate = 0.ToString()
            };

            return log;
        }

        public bool SaveSimulation(string txtMsrp, string txtInvicePrice, string txtCogs, string txtGp, string costMasterId)
        {
            try
            {
                CostSimulationLog log = new CostSimulationLog
                {
                    MSRP = WalCrypto.Crypto.Encrypt(txtMsrp, WalCrypto.Crypto.GetKey()),
                    Invoice = WalCrypto.Crypto.Encrypt(txtInvicePrice, WalCrypto.Crypto.GetKey()),
                    COGS = WalCrypto.Crypto.Encrypt(txtCogs, WalCrypto.Crypto.GetKey()),
                    GP = WalCrypto.Crypto.Encrypt(txtGp, WalCrypto.Crypto.GetKey()),
                    InvoiceAfterRebate = WalCrypto.Crypto.Encrypt(txtInvicePrice, WalCrypto.Crypto.GetKey()),
                    CostMasterId = Convert.ToInt64(costMasterId),
                    MSRP_IP_Percent = WalCrypto.Crypto.Encrypt(10.ToString(), WalCrypto.Crypto.GetKey()),
                    Rebate = WalCrypto.Crypto.Encrypt(0.ToString(), WalCrypto.Crypto.GetKey()),
                    AddedBy = HttpContext.Current.User.Identity.Name,
                    AddedDate = DateTime.Now
                };

                _cellPhoneProjectEntities.CostSimulationLogs.Add(log);
                _cellPhoneProjectEntities.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}