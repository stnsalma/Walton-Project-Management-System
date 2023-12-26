using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class JigsAndFixtureModel
    {
        public long JigsFixtureId { get; set; }
        public long? ProjectMasterId { get; set; }
        public string JigsAndFixtureName { get; set; }
        public string Type { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Price { get; set; }
        public string Quantity { get; set; }
        public string UnitPrice { get; set; }
    }
}