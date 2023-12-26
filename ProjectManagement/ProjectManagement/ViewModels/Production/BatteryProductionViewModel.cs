using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Production
{
    public class BatteryProductionViewModel
    {
        public DateTime? ProductionDate { get; set; }
        public string SmtMaterialProductionValue { get; set; }
        public string SmtIqcProductionValue { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int? OrderNumber { get; set; }
        public string PoCategory { get; set; }
        public DateTime? MaterialReceiveStartDateBSmt { get; set; }
        public DateTime? MaterialReceiveEndDateBSmt { get; set; }
        public DateTime? IqcCompleteStartDateBSmt { get; set; }
        public DateTime? IqcCompleteEndDateBSmt { get; set; }
        public DateTime? TrialProductionStartDateBSmt { get; set; }
        public DateTime? TrialProductionEndDateBSmt { get; set; }
        public DateTime? SmtMassProductionStartDateBSmt { get; set; }
        public long? TotalQuantityBSmt { get; set; }
        public DateTime? SmtMassProductionEndDateBSmt { get; set; }
        public string StatusBSmt { get; set; }


        public string MetarialReceiveBSmt { get; set; }
        public string IqcCompleteBSmt { get; set; }
        public string TrialProductionBSmt { get; set; }
        public string BSmtMassProduction { get; set; }

        public DateTime? MaterialReceiveStartDateBHousing { get; set; }
        public DateTime? MaterialReceiveEndDateBHousing { get; set; }
        public DateTime? IqcCompleteStartDateBHousing { get; set; }
        public DateTime? IqcCompleteEndDateBHousing { get; set; }
        public DateTime? TrialProductionStartDateBHousing { get; set; }
        public DateTime? TrialProductionEndDateBHousing { get; set; }
        public DateTime? HousingReliabilityTestStartDateBHousing { get; set; }
        public DateTime? HousingReliabilityTestEndDateBHousing { get; set; }
        public DateTime? HousingMassProductionStartDateBHousing { get; set; }
        public long? TotalQuantity { get; set; }
        public DateTime? HousingMassProductionEndDateBHousing { get; set; }
        public string StatusBHousing { get; set; }

        public string MetarialReceiveBHousing { get; set; }
        public string IqcCompleteBHousing { get; set; }
        public string TrialProductionBHousing { get; set; }
        public string HousingBReliability { get; set; }
        public string HousingBMassProduction { get; set; }


        public DateTime? MaterialReceiveStartDateBattery { get; set; }
        public DateTime? MaterialReceiveEndDateBattery { get; set; }
        public DateTime? IqcCompleteStartDateBattery { get; set; }
        public DateTime? IqcCompleteEndDateBattery { get; set; }
        public DateTime? TrialProductionStartDateBattery { get; set; }
        public DateTime? TrialProductionEndDateBattery { get; set; }
        public DateTime? BatteryReliabilityTestStartDate { get; set; }
        public DateTime? BatteryReliabilityTestEndDate { get; set; }
        public DateTime? BatteryMassProductionStartDate { get; set; }
        public long? TotalQuantityBattery { get; set; }
        public DateTime? BatteryMassProductionEndDate { get; set; }
        public DateTime? BatteryAgingTestStartDate { get; set; }
        public DateTime? BatteryAgingTestEndDate { get; set; }
        public string StatusBattery { get; set; }

        public string BatteryMetarialReceive { get; set; }
        public string BatteryIqcComplete { get; set; }
        public string BatteryTrialProduction { get; set; }
        public string BatteryReliability { get; set; }
        public string BatteryMassProduction { get; set; }
        public string BatteryAging { get; set; }

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

        public string MetarialReceiveAssembly { get; set; }
        public string IqcCompleteAssembly { get; set; }
        public string TrialProductionAssembly { get; set; }
        public string SoftwareConfirmationAssembly { get; set; }
        public string RnDConfirmAssembly { get; set; }
        public string AssemblyMassProduction { get; set; }
        public string PackingMassProduction { get; set; }
        public string ProductionRemarks { get; set; }
        public DateTime ProductionRemarksDate { get; set; }
        public bool? IsCkd { get; set; }
        public bool? IsCharger { get; set; }

        //All line//
        public string SmtLineOne { get; set; }
        public string SmtLineTwo { get; set; }

        public string HousingLineOne { get; set; }
        public string HousingLineTwo { get; set; }
        public string HousingLineThree { get; set; }
        public string HousingLineFour { get; set; }
        public string HousingLineFive { get; set; }
        public string HousingLineSix { get; set; }

        public string BatteryLineOne { get; set; }
        public string BatteryLineTwo { get; set; }

        public string AssemblyLineOne { get; set; }
        public string AssemblyLineTwo { get; set; }
        public string AssemblyLineThree { get; set; }
        public string AssemblyLineFour { get; set; }
        public string AssemblyLineFive { get; set; }

        public string PackingLine1 { get; set; }
        public string PackingLine2 { get; set; }

    }
}