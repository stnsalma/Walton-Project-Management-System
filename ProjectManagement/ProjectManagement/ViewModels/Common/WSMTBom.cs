using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Common
{
    public class WSMTBom
    {
        public long ID { get; set; }
        public string PartNumber { get; set; }
        public string Name { get; set; }
        public string Name_Cn { get; set; }
        public long OrcalePartID { get; set; }
        public string OracleItemCode { get; set; }
        public string Manufacturer_PartNumber { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public int QTY { get; set; }
        public int Total_Qty { get; set; }
        public int MOQ { get; set; }
        public int MPQ { get; set; }
        public int Number_Of_Reel { get; set; }
        public string Value { get; set; }
        public string Manufacturer { get; set; }
        public long Handset_Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

    }
}