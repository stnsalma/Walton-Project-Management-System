using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Common
{
    public class WSMTAltBom
    {
        public long ID { get; set; }
        public long BOM_Id { get; set; }
        public long Handset_Id { get; set; }
        public string PartNumber { get; set; }
        public long OraclePartID { get; set; }
        public string OracleItemCode { get; set; }
        public string Manufacturer_PartNumber { get; set; }
        public string Description { get; set; }
 
        public string Value { get; set; }
        public string Manufacturer { get; set; }
        
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}