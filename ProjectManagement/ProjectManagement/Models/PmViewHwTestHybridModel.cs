using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PmViewHwTestHybridModel : IEnumerable
    {
        public long ProjectMasterId { get; set; }
        public long HwQcInchargeAssignId { get; set; }
        public string ProjectName { get; set; }
        public string SupplierName { get; set; }
        public string ProjectType { get; set; }
        public string Chipset_Vendor { get; set; }
        public string Chipset_Speed { get; set; }
        public string Chipset_Core { get; set; }
        public string FlashIC_ROM { get; set; }
        public string FlashIC_RAM { get; set; }
        public string FrontCamera_MPSW { get; set; }
        public string BackCamera_MPSW { get; set; }
        public string Remark { get; set; }
        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}