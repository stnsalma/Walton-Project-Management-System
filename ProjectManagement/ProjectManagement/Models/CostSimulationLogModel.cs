using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class CostSimulationLogModel
    {
        public long Id { get; set; }
        public long? CostMasterId { get; set; }
        public string MSRP { get; set; }
        public string Invoice { get; set; }
        public string Rebate { get; set; }
        public string InvoiceAfterRebate { get; set; }
        public string COGS { get; set; }
        public string GP { get; set; }
        public string MSRP_IP_Percent { get; set; }
        public System.DateTime? AddedDate { get; set; }
        public string AddedBy { get; set; }

        public virtual CostMasterModel CostMaster { get; set; }
    }
}