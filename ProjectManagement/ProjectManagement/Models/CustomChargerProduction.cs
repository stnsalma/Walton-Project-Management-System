using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class CustomChargerProduction
    {
        public DateTime? ProductionDate { get; set; }
        public long Id { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string OrderNumber { get; set; }
        public string PoCategory { get; set; }
        public DateTime? MaterialReceiveStartDateSmt { get; set; }
        public DateTime? MaterialReceiveEndDateSmt { get; set; }
        public DateTime? IqcCompleteStartDateSmt { get; set; }
        public DateTime? IqcCompleteEndDateSmt { get; set; }
        public DateTime? TrialProductionStartDateSmt { get; set; }
        public DateTime? TrialProductionEndDateSmt { get; set; }
        public DateTime? SmtMassProductionStartDateSmt { get; set; }
        public DateTime? SmtMassProductionEndDateSmt { get; set; }
        public long ChargerSmtTotalQuantity { get; set; }
        public long ChargerSmtPerDayCapacity { get; set; }
        public string SmtAllLineCapacity { get; set; }
        public string SmtAllLineNumber { get; set; }

        public DateTime? MaterialReceiveStartDateHousing { get; set; }
        public DateTime? MaterialReceiveEndDateHousing { get; set; }
        public DateTime? IqcCompleteStartDateHousing { get; set; }
        public DateTime? IqcCompleteEndDateHousing { get; set; }
        public DateTime? TrialProductionStartDateHousing { get; set; }
        public DateTime? TrialProductionEndDateHousing { get; set; }
        public DateTime? HousingReliabilityStartDateHousing { get; set; }
        public DateTime? HousingReliabilityEndtDateHousing { get; set; }
        public DateTime? HousingMassProStartDateHousing { get; set; }
        public DateTime? HousingMassProEndtDateHousing { get; set; }
        public long ChargerHousingTotalQuantity { get; set; }
        public long ChargerHousingPerDayCapacity { get; set; }
        public string HousingAllLineCapacity { get; set; }
        public string HousingAllLineNumber { get; set; }

        public DateTime? MaterialReceiveStartDateAssembly { get; set; }
        public DateTime? MaterialReceiveEndDateAssembly { get; set; }
        public DateTime? IqcCompleteStartDateAssembly { get; set; }
        public DateTime? IqcCompleteEndDateAssembly { get; set; }
        public DateTime? TrialProductionStartDateAssembly { get; set; }
        public DateTime? TrialProductionEndDateAssembly { get; set; }
        public DateTime? RnDConfirmStartDateAssembly { get; set; }
        public DateTime? RnDConfirmEndDateAssembly { get; set; }
        public DateTime? AssembStartDateAssembly { get; set; }
        public DateTime? AssembEndDateAssembly { get; set; }
        public long ChargerAssemblyTotalQuantity { get; set; }
        public long ChargerAssemblyPerDayCapacity { get; set; }
        public string AssemblyAllLineCapacity { get; set; }
        public string AssemblyAllLineNumber { get; set; }

        public bool SmtChk { get; set; }
        public bool HouseChk { get; set; }
        public bool AssemblyChk { get; set; }
      
        public string Status { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string SmtTotalProduction { get; set; }
        public string HousingTotalProduction { get; set; }
        public string AssemblyTotalProduction { get; set; }

    }
}