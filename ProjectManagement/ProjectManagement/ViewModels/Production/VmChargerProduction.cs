using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Production
{
    public class VmChargerProduction
    {

        public VmChargerProduction() 
        {
            ProjectMasterModels=new List<ProjectMasterModel>();
            ProjectMasterModel=new ProjectMasterModel();
            ChargerAssemblyModels=new List<ChargerAssemblyModel>();
            ChargerAssemblyModel=new ChargerAssemblyModel();
            ChargerHousingModels=new List<ChargerHousingModel>();
            ChargerHousingModel=new ChargerHousingModel();
            ChargerSmtModels=new List<ChargerSMTModel>();
            ChargerSmtModel=new ChargerSMTModel();
        }

        public List<ProjectMasterModel> ProjectMasterModels { get; set; }
        public ProjectMasterModel ProjectMasterModel { get; set; }
        public List<ChargerAssemblyModel> ChargerAssemblyModels { get; set; }
        public ChargerAssemblyModel ChargerAssemblyModel { get; set; }
        public List<ChargerHousingModel> ChargerHousingModels { get; set; }
        public ChargerHousingModel ChargerHousingModel { get; set; }
        public List<ChargerSMTModel> ChargerSmtModels { get; set; }
        public ChargerSMTModel ChargerSmtModel { get; set; }
        public long Id { get; set; }
        public Nullable<long> ProjectId { get; set; }
        public string ProjectName { get; set; }
        public Nullable<int> OrderNumber { get; set; }
        public string PoCategory { get; set; }
        public Nullable<System.DateTime> MaterialReceiveStartDate { get; set; }
        public Nullable<System.DateTime> MaterialReceiveEndDate { get; set; }
        public Nullable<System.DateTime> IqcCompleteStartDate { get; set; }
        public Nullable<System.DateTime> IqcCompleteEndDate { get; set; }
        public Nullable<System.DateTime> TrialProductionStartDate { get; set; }
        public Nullable<System.DateTime> TrialProductionEndDate { get; set; }
        public Nullable<System.DateTime> RandDConfirmationStartDate { get; set; }
        public Nullable<System.DateTime> RandDConfirmationEndDate { get; set; }
        public Nullable<System.DateTime> AssemblyProductionStartDate { get; set; }
        public Nullable<System.DateTime> AssemblyProductionEndDate { get; set; }
        public Nullable<System.DateTime> HousingReliabilityTestStartDate { get; set; }
        public Nullable<System.DateTime> HousingReliabilityTestEndDate { get; set; }
        public Nullable<System.DateTime> HousingMassProductionStartDate { get; set; }
        public Nullable<System.DateTime> HousingMassProductionEndDate { get; set; }
        public Nullable<System.DateTime> SmtMassProductionStartDate { get; set; }
        public Nullable<System.DateTime> SmtMassProductionEndDate { get; set; }
        public string Status { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public Nullable<long> Updated { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}