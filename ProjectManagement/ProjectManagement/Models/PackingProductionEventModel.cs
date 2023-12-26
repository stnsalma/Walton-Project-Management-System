using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PackingProductionEventModel
    {
        public long Id { get; set; }
        public Nullable<long> ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int OrderNumber { get; set; }
        public Nullable<System.DateTime> MaterialReceiveDate { get; set; }
        public Nullable<System.DateTime> IqcCompleteDate { get; set; }
        public Nullable<System.DateTime> TrialProductionDate { get; set; }
        public Nullable<System.DateTime> SoftwareConfirmationDate { get; set; }
        public Nullable<System.DateTime> RnDClearanceDate { get; set; }
        public string PackingLineInformation { get; set; }
        public Nullable<System.DateTime> PackingProductionStartDate { get; set; }
        public Nullable<long> PackingQuantity { get; set; }
        public Nullable<long> PackingPerDayCapacity { get; set; }
        public Nullable<System.DateTime> PackingProductionEndDate { get; set; }
        public string Status { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}