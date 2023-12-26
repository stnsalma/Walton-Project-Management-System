using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Production
{
    public class ProductPlanModel
    {
        public DateTime ProductionDate { get; set; }
        public string MetarialReceive { get; set; }
        public string IqcComplete { get; set; }
        public string TrialProduction { get; set; }
        public string SoftwareConfirmation { get; set; }
        public string RnDClearance { get; set; }
        public string AssemblyProductionStart { get; set; }
        public string AssemblyLine1 { get; set; }
        public string AssemblyLine2 { get; set; }
        public string AssemblyLine3 { get; set; }
        public string AssemblyLine4 { get; set; }
        public string AssemblyLine5 { get; set; }
        public string AssemblyLine6 { get; set; }
        public string AssemblyLine7 { get; set; }
        public string AssemblyProductionEnd { get; set; }
        public string PackingProductionStart { get; set; }
        public string PackingLine1 { get; set; }
        public string PackingLine2 { get; set; }
        public string PackingLine3 { get; set; }
        public string PackingLine4 { get; set; }
        public string PackingLine5 { get; set; }
        public string PackingLine6 { get; set; }
        public string PackingLine7 { get; set; }
        public string PackingProductionEnd { get; set; }
        public string ProductionRemarks { get; set; }

    }
}