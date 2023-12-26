using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class CostProposalModel
    {
        public long Id { get; set; }
        public long CostMasterId { get; set; }
        public string PriceProposal { get; set; }
        public long ProposedBy { get; set; }
        public System.DateTime ProposedDate { get; set; }
        public bool? IsCancelled { get; set; }
        public System.DateTime? CancelledDate { get; set; }
        public System.DateTime? UpdatedDate { get; set; }

        public virtual CostMasterModel CostMaster { get; set; }


        public string ProposalByName { get; set; }
        public string RoleDetailName { get; set; }
    }
}