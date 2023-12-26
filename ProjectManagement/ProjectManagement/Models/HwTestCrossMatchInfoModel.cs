using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HwTestCrossMatchInfoModel
    {
        public long? HwTestCrossMAtchInfoId { get; set; }
        public Nullable<long> HwQcAssignId { get; set; }
        public Nullable<long> HwQcInchargeAssignId { get; set; }
        public string TpLcdOpening { get; set; }
        public string SteelShielding { get; set; }
        public string Overall { get; set; }
        public string Recommendation { get; set; }
        public string Comment { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    
    }
}