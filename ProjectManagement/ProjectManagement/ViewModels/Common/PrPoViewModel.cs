using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Common
{
    public class PrPoViewModel
    {
        [DisplayName("COMPANY ID")]
        public string CompanyId { get; set; }
        [DisplayName("COMPANY")]
        public string Company { get; set; }
        [DisplayName("PR NUMBER")]
        public string PrNumber { get; set; }
        [DisplayName("PR CREATOR")]
        public string PrCreatorName { get; set; }
        [DisplayName("PR CREATION DATE")]
        public string PrCreationDate { get; set; }
        [DisplayName("PO NUMBER")]
        public string PoNumber { get; set; }
        [DisplayName("PO CREATOR")]
        public string PoCreator { get; set; }
        [DisplayName("PO CREATOR ID")]
        public string PoCreatorId { get; set; }
        [DisplayName("PO CREATOION D.")]
        public string PoCreationDate { get; set; }
        [DisplayName("STATUS")]
        public string PoStatus { get; set; }
    }
}