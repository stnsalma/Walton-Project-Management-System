using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Production
{
    public class VmBatteryProduction
    {
        public VmBatteryProduction() 
        {
            ProjectMasterModels=new List<ProjectMasterModel>();
            ProjectMasterModel=new ProjectMasterModel();

            BatteryModel=new BatteryModel();
            BatteryModels=new List<BatteryModel>();
            BatterySmtModel=new BatterySMTModel();
            BatterySmtModels=new List<BatterySMTModel>();
            BatteryHousingModel=new BatteryHousingModel();
            BatteryHousingModels=new List<BatteryHousingModel>();
            BatteryAssemblyAndPackingModel=new BatteryAssemblyAndPackingModel();
            BatteryAssemblyAndPackingModels=new List<BatteryAssemblyAndPackingModel>();
            CustomBatteryProductions=new List<CustomBatteryProduction>();
        }
        public List<CustomBatteryProduction> CustomBatteryProductions { get; set; } 
        public List<ProjectMasterModel> ProjectMasterModels { get; set; }
        public ProjectMasterModel ProjectMasterModel { get; set; }
        public List<BatteryModel> BatteryModels { get; set; }
        public BatteryModel BatteryModel { get; set; }
        public BatterySMTModel BatterySmtModel { get; set; }
        public List<BatterySMTModel> BatterySmtModels { get; set; }
        public BatteryHousingModel BatteryHousingModel { get; set; }
        public List<BatteryHousingModel> BatteryHousingModels { get; set; }
        public List<BatteryAssemblyAndPackingModel> BatteryAssemblyAndPackingModels { get; set; }
        public BatteryAssemblyAndPackingModel BatteryAssemblyAndPackingModel { get; set; }
        public long Id { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int? OrderNumber { get; set; }
        public string PoCategory { get; set; }
       
    }
}