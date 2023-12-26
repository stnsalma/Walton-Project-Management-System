using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class CustomPrdAssemblyAndPackingDetails
    {
        public long Id { get; set; }
        public Nullable<long> ProjectId { get; set; }
        public bool? IsCkd { get; set; }
        public bool? IsCharger { get; set; }
        public string ProjectName { get; set; }
        public string OrderNumber { get; set; }
        public string PoCategory { get; set; }
        public Nullable<System.DateTime> MaterialReceiveDate { get; set; }
        public Nullable<System.DateTime> IqcCompleteDate { get; set; }
        public Nullable<System.DateTime> TrialProductionDate { get; set; }
        public Nullable<System.DateTime> SoftwareConfirmationDate { get; set; }
        public Nullable<System.DateTime> RnDClearanceDate { get; set; }
        public string AssemblyLineInformation { get; set; }
        public Nullable<System.DateTime> AssemblyProductionStartDate { get; set; }
        public string AssemblyQuantity { get; set; }
        public string AssemblyPerDayCapacity { get; set; }
        public Nullable<System.DateTime> AssemblyProductionEndDate { get; set; }
        public string PackingLineInformation { get; set; }
        public Nullable<System.DateTime> PackingProductionStartDate { get; set; }
        public string PackingQuantity { get; set; }
        public string PackingPerDayCapacity { get; set; }
        public Nullable<System.DateTime> PackingProductionEndDate { get; set; }
        public string Status { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public DateTime ProductionDate { get; set; }

        public string AssemblyLine { get; set; }
        public string PackingLine { get; set; }
        public string Remarks { get; set; }
        public DateTime ProductionRemarksDate { get; set; }
    }
}