using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Production
{
    public class ChargerProductionViewModel
    {
        public DateTime? ProductionDate { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string OrderNumber { get; set; }
        public string PoCategory { get; set; }
        public string MaterialReceiveStartDateSmt { get; set; }
        public string MaterialReceiveEndDateSmt { get; set; }
        public string IqcCompleteStartDateSmt { get; set; }
        public string IqcCompleteEndDateSmt { get; set; }
        public string TrialProductionStartDateSmt { get; set; }
        public string TrialProductionEndDateSmt { get; set; }
        public string SmtMassProductionStartDateSmt { get; set; }
        public string SmtMassProductionEndDateSmt { get; set; }

        public string MetarialReceiveSmt { get; set; }
        public string IqcCompleteSmt { get; set; }
        public string TrialProductionSmt { get; set; }
        public string SmtMassProduction { get; set; }

        public string MaterialReceiveStartDateHousing { get; set; }
        public string MaterialReceiveEndDateHousing { get; set; }
        public string IqcCompleteStartDateHousing { get; set; }
        public string IqcCompleteEndDateHousing { get; set; }
        public string TrialProductionStartDateHousing { get; set; }
        public string TrialProductionEndDateHousing { get; set; }
        public string HousingReliabilityStartDateHousing { get; set; }
        public string HousingReliabilityEndtDateHousing { get; set; }
        public string HousingMassProStartDateHousing { get; set; }
        public string HousingMassProEndtDateHousing { get; set; }

        public string MetarialReceiveHousing { get; set; }
        public string IqcCompleteHousing { get; set; }
        public string TrialProductionHousing { get; set; }
        public string HousingReliability { get; set; }
        public string HousingMassProduction { get; set; }


        public string MaterialReceiveStartDateAssembly { get; set; }
        public string MaterialReceiveEndDateAssembly { get; set; }
        public string IqcCompleteStartDateAssembly { get; set; }
        public string IqcCompleteEndDateAssembly { get; set; }
        public string TrialProductionStartDateAssembly { get; set; }
        public string TrialProductionEndDateAssembly { get; set; }
        public string RnDConfirmStartDateAssembly { get; set; }
        public string RnDConfirmEndDateAssembly { get; set; }
        public string AssembStartDateAssembly { get; set; }
        public string AssembEndDateAssembly { get; set; }

        public string MetarialReceiveAssembly { get; set; }
        public string IqcCompleteAssembly { get; set; }
        public string TrialProductionAssembly { get; set; }
        public string RnDConfirmAssembly { get; set; }
        public string AssemblyDate { get; set; }


        public string Status { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public long SmtTotalProduction { get; set; }
        public long HousingTotalProduction { get; set; }
        public long AssemblyTotalProduction { get; set; }
    }
}