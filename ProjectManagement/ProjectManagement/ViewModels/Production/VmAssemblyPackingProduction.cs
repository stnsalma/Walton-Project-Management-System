using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Production
{
    public class VmAssemblyPackingProduction
    {
        public VmAssemblyPackingProduction()
        {
            ProjectMasterModelsList = new List<ProjectMasterModel>();
            ProjectMasterModel = new ProjectMasterModel();
            AssemblyProductionEventModels=new List<AssemblyProductionEventModel>();
            AssemblyProductionEventModel=new AssemblyProductionEventModel();
            PackingProductionEventModel=new PackingProductionEventModel();
            PackingProductionEventModels=new List<PackingProductionEventModel>();
            CustomPrdAssembly=new List<CustomPrdAssemblyAndPackingDetails>();
        }

        public List<CustomPrdAssemblyAndPackingDetails> CustomPrdAssembly { get; set; }
        public List<AssemblyProductionEventModel> AssemblyProductionEventModels { get; set; }
        public AssemblyProductionEventModel AssemblyProductionEventModel { get; set; }
        public List<PackingProductionEventModel> PackingProductionEventModels { get; set; }
        public PackingProductionEventModel PackingProductionEventModel { get; set; }
        public List<ProjectMasterModel> ProjectMasterModelsList { get; set; }
        public ProjectMasterModel ProjectMasterModel { get; set; }
        public long Id { get; set; }
        public Nullable<long> ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int? OrderNuber { get; set; }
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

        /// <summary>
        /// ///////////////////////
        /// </summary>
        public string PackingLineInformation { get; set; }
        public Nullable<System.DateTime> PackingProductionStartDate { get; set; }
        public Nullable<long> PackingQuantity { get; set; }
        public Nullable<long> PackingPerDayCapacity { get; set; }
        public Nullable<System.DateTime> PackingProductionEndDate { get; set; }
    }
}