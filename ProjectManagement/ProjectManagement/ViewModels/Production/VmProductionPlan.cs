using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Production
{
    public class VmProductionPlan
    {
        public DateTime ProductionDate { get; set; }
        public CustomPrdAssemblyAndPackingDetails AssemblyAndPackingDetailse { get; set; }
    }
}