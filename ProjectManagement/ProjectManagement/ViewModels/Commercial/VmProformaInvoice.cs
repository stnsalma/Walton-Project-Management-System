using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Commercial
{
    public class VmProformaInvoice
    {
        public VmProformaInvoice()
        {
            ProjectMasterModel = new ProjectMasterModel();
            ProjectProformaInvoiceModel = new ProjectProformaInvoiceModel();
        }
        public ProjectMasterModel ProjectMasterModel { get; set; }
        public ProjectProformaInvoiceModel ProjectProformaInvoiceModel { get; set; }
    }
}