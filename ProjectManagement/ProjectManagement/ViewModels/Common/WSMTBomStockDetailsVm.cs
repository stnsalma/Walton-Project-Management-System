using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Common
{
    public class WSMTBomStockDetailsVm
    {
        public long ID { get; set; }
        public long Handset_Id { get; set; }
        public bool IsMainBom { get; set; }
        public string PartNumber { get; set; }
        public string Name { get; set; }
        public string Name_Cn { get; set; }
        public long OrcalePartID { get; set; }
        public string OracleItemCode { get; set; }
        public string Manufacturer { get; set; }
        public string Manufacturer_PartNumber { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public long CurrentStock { get; set; }
       
    }
}