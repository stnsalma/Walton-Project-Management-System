using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectEventDates
    {
        public long ProjectMasterId { get; set; }
        public int OrderNumber { get; set; }
        public string ProjectName { get; set; }
        public DateTime? ShipmentTakenDate { get; set; }
        public DateTime? MaterialsArrivalDate { get; set; }
        public DateTime? MarketClearanceDate { get; set; }
        public DateTime? PoDate { get; set; }
        public int? PoWiseShipmentNumber { get; set; }
        public string ShipmentNoOrdinal { get; set; }
        public string PoOrdinal { get; set; }
    }
}