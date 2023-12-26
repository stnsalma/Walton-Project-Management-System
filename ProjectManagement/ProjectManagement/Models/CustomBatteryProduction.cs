using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class CustomBatteryProduction
    {
        public string ActiveStatus { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? ProductionDate { get; set; }
        public string SmtMaterialProductionValue { get; set; }
        public string SmtIqcProductionValue { get; set; }
        public long? AsmId { get; set; }
        public long? BhId { get; set; }
        public long? BbId { get; set; }
        public long? SmtId { get; set; }
        public long? AsmPlanId { get; set; }
        public long? BbPlanId { get; set; }
        public long? BhPlanId { get; set; }
        public long? SmtPlanId { get; set; }
        public long? AsmProjectId { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string AsmProjectName { get; set; }
        public string OrderNumber { get; set; }
        public string PoCategory { get; set; }
        public DateTime? MaterialReceiveStartDateBSmt { get; set; }
        public DateTime? MaterialReceiveEndDateBSmt { get; set; }
        public DateTime? IqcCompleteStartDateBSmt { get; set; }
        public DateTime? IqcCompleteEndDateBSmt { get; set; }
        public DateTime? TrialProductionStartDateBSmt { get; set; }
        public DateTime? TrialProductionEndDateBSmt { get; set; }
        public DateTime? SmtMassProductionStartDateBSmt { get; set; }
        public string TotalQuantityBSmt { get; set; }
        public DateTime? SmtMassProductionEndDateBSmt { get; set; }
        public string StatusBSmt { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public long BatterySmtTotalQuantity { get; set; }
        public long BatterySmtPerDayCapacity { get; set; }
        public string SmtAllLineCapacity { get; set; }
        public string SmtAllLineNumber { get; set; }
        public string SmtTrialLine { get; set; }

        public DateTime? MaterialReceiveStartDateBHousing { get; set; }
        public DateTime? MaterialReceiveEndDateBHousing { get; set; }
        public DateTime? IqcCompleteStartDateBHousing { get; set; }
        public DateTime? IqcCompleteEndDateBHousing { get; set; }
        public DateTime? TrialProductionStartDateBHousing { get; set; }
        public DateTime? TrialProductionEndDateBHousing { get; set; }
        public DateTime? HousingReliabilityTestStartDateBHousing { get; set; }
        public DateTime? HousingReliabilityTestEndDateBHousing { get; set; }
        public DateTime? HousingMassProductionStartDateBHousing { get; set; }
        public string TotalQuantity { get; set; }
        public DateTime? HousingMassProductionEndDateBHousing { get; set; }
        public string StatusBHousing { get; set; }
        public long BatteryHousingTotalQuantity { get; set; }
        public long BatteryHousingPerDayCapacity { get; set; }
        public string HousingAllLineCapacity { get; set; }
        public string HousingAllLineNumber { get; set; }
        public string HousingTrialLine { get; set; }

        public DateTime? MaterialReceiveStartDateBattery { get; set; }
        public DateTime? MaterialReceiveEndDateBattery { get; set; }
        public DateTime? IqcCompleteStartDateBattery { get; set; }
        public DateTime? IqcCompleteEndDateBattery { get; set; }
        public DateTime? TrialProductionStartDateBattery { get; set; }
        public DateTime? TrialProductionEndDateBattery { get; set; }
        public DateTime? BatteryReliabilityTestStartDate { get; set; }
        public DateTime? BatteryReliabilityTestEndDate { get; set; }
        public DateTime? BatteryMassProductionStartDate { get; set; }
        public string TotalQuantityBattery { get; set; }
        public DateTime? BatteryMassProductionEndDate { get; set; }
        public DateTime? BatteryAgingTestStartDate { get; set; }
        public DateTime? BatteryAgingTestEndDate { get; set; }
        public string StatusBattery { get; set; }
        public long BatteryTotalQuantity { get; set; }
        public long BatteryPerDayCapacity { get; set; }
        public string BatteryAllLineCapacity { get; set; }
        public string BatteryAllLineNumber { get; set; }
        public string BatteryTrialLine { get; set; }

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
        public string TotalQuantityBAssembly { get; set; }
        public DateTime? AssemblyMassProductionEndDateBAssembly { get; set; }
        public DateTime? PackingMassProductionStartDateBAssembly { get; set; }
        public string TotalQuantityBPacking { get; set; }
        public DateTime? PackingMassProductionEndDateBAssembly { get; set; }
        public string StatusAssembAndPack { get; set; }
        public long BatteryAssemblyTotalQuantity { get; set; }
        public long BatteryAssemblyPerDayCapacity { get; set; }
        public string AssemblyAllLineCapacity { get; set; }
        public string AssemblyAllLineNumber { get; set; }

        public long BatteryPackingTotalQuantity { get; set; }
        public long BatteryPackingPerDayCapacity { get; set; }
        public string PackingAllLineCapacity { get; set; }
        public string PackingAllLineNumber { get; set; }
        public string AssemblyTrialLine { get; set; }

        public string AssemblyLine { get; set; }
        public bool SmtChk { get; set; }
        public bool HouseChk { get; set; }
        public bool BatteryChk { get; set; }
        public bool AssemblyChk { get; set; }

        public string AllSmtPro { get; set; }
        public string LineNumber { get; set; }
        public string ProductionRemarks { get; set; }
        public DateTime ProductionRemarksDate { get; set; }
        public string TrialLineNumber { get; set; }
        public string TrialLineInfoSmt { get; set; }
    }
}