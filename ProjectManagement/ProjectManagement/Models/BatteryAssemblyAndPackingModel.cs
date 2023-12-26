using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class BatteryAssemblyAndPackingModel
    {
        public long Id { get; set; }
        public long PlanId { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int? OrderNumber { get; set; }
        public string PoCategory { get; set; }
        public DateTime? MaterialReceiveStartDateBAssembly { get; set; }
        public DateTime? MaterialReceiveEndDateBAssembly { get; set; }
        public DateTime? IqcCompleteStartDateBAssembly { get; set; }
        public DateTime? IqcCompleteEndDateBAssembly { get; set; }
        public DateTime? TrialProductionStartDateBAssembly { get; set; }
        public DateTime? TrialProductionEndDateBAssembly { get; set; }
        public DateTime? SoftwareConfirmationStartDateBAssembly { get; set; }
        public DateTime? SoftwareConfirmationEndDateBAssembly { get; set; }
        public DateTime? RandDConfirmationStartDateBAssembly { get; set; }
        public DateTime? RandDConfirmationEndDateBAssembly { get; set; }
        public DateTime? AssemblyMassProductionStartDateBAssembly { get; set; }
        public long? TotalQuantityBAssembly { get; set; }
        public DateTime? AssemblyMassProductionEndDateBAssembly { get; set; }
        public DateTime? PackingMassProductionStartDateBAssembly { get; set; }
        public long? TotalQuantityBPacking { get; set; }
        public DateTime? PackingMassProductionEndDateBAssembly { get; set; }
        public string StatusAssembAndPack { get; set; }
        public bool? IsActive { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}