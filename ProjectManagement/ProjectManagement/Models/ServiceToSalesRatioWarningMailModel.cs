using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ServiceToSalesRatioWarningMailModel
    {
        public long ServiceToSalesRatioWarningId { get; set; }
        public string ProductCode { get; set; }
        public string Model { get; set; }
        public DateTime? LaunchDate { get; set; }
        public decimal? ServiceToSalesRatio { get; set; }
        public string AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? IsSolved { get; set; }
        public string Solution { get; set; }
        public DateTime? SolutionDate { get; set; }
        public long? SolutionBy { get; set; }
        public long? ClosedBy { get; set; }
        public DateTime? ClosingDate { get; set; }
    }
}