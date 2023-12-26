using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class AssemblyProductionEventModel
    {
        public AssemblyProductionEventModel()
        {
            CustomPrdAssemblyAndPackingDetailses=new List<CustomPrdAssemblyAndPackingDetails>();
        }

        public List<CustomPrdAssemblyAndPackingDetails> CustomPrdAssemblyAndPackingDetailses { get; set; }
        public long Id { get; set; }
        public Nullable<long> ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int OrderNumber { get; set; }
        public string PoCategory { get; set; }
        public Nullable<System.DateTime> MaterialReceiveDate { get; set; }
        public Nullable<System.DateTime> IqcCompleteDate { get; set; }
        public Nullable<System.DateTime> TrialProductionDate { get; set; }
        public Nullable<System.DateTime> SoftwareConfirmationDate { get; set; }
        public Nullable<System.DateTime> RnDClearanceDate { get; set; }
        public string AssemblyLineInformation { get; set; }
        public Nullable<System.DateTime> AssemblyProductionStartDate { get; set; }
        public Nullable<long> AssemblyQuantity { get; set; }
        public Nullable<long> AssemblyPerDayCapacity { get; set; }
        public Nullable<System.DateTime> AssemblyProductionEndDate { get; set; }
        public string Status { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}