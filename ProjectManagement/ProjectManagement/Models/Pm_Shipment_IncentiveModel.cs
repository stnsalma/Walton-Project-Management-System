using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class Pm_Shipment_IncentiveModel
    {
        public long Id { get; set; }
        public Nullable<long> ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public string EmployeeCode { get; set; }
        public Nullable<System.DateTime> ApproxShipmentDate { get; set; }
        public Nullable<System.DateTime> ChainaInspectionDate { get; set; }
        public Nullable<int> NoOfdays { get; set; }
        public Nullable<long> ProjectManagerUserId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> DeductionAmount { get; set; }
        public Nullable<decimal> FinalAmount { get; set; }
        public string D_Remarks { get; set; }
        public string Remarks { get; set; }
        public string Month { get; set; }
        public Nullable<int> MonNum { get; set; }
        public Nullable<long> Year { get; set; }
        public Nullable<int> OrderNumber { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<long> Updated { get; set; } 
        public Nullable<System.DateTime> ProjectManagerClearanceDate { get; set; }
        public DateTime? FlightDepartureDate { get; set; }
    }
}