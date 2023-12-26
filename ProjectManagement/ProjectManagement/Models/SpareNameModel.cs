using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class SpareNameModel
    {
        public long SpareId { get; set; }
        public string SparePartsName { get; set; }
        public string ProposedImportRatio { get; set; }
        public string SpareType { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        
    }
}