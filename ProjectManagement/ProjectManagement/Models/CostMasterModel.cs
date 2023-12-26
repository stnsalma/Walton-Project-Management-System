using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.DAL.DbModel;

namespace ProjectManagement.Models
{
    public class CostMasterModel
    {
        public CostMasterModel()
        {
            CostProposals = new List<CostProposalModel>();
            CostMasterDetails = new List<CostMasterDetailModel>();
            CostSimulationLogs = new List<CostSimulationLogModel>();
        }
        public long Id { get; set; }
        public long CostManagementModelId { get; set; }
        public string ModelName { get; set; }
        public string PhaseNo { get; set; }
        public bool? IsCompleted { get; set; }
        public string ManagementProposal { get; set; }
        public System.DateTime AddedDate { get; set; }
        public long AddedBy { get; set; }
        public string AddedFrom { get; set; }
        public string COGS { get; set; }
        public string FinalMSRP { get; set; }
        public string FinalInvoice { get; set; }
        public string FinalGp { get; set; }

        public ICollection<CostProposalModel> CostProposals { get; set; }
        public ICollection<CostMasterDetailModel> CostMasterDetails { get; set; }
        public ICollection<CostSimulationLogModel> CostSimulationLogs { get; set; }

    }
}