using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.MaterialWastage
{
    public class AddMaterialViewModel
    {
        public long Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemDetail { get; set; }
        public double BomUnit { get; set; }
        public double? WastagePercentage { get; set; }
        public int AssemblyMaterialFault { get; set; }
        public int AssemblyProcessFault { get; set; }
        public int RepairMaterialFault { get; set; }
        public int RepairProcessFault { get; set; }
        public int TotalWastageFault { get; set; }
        public string BOMType { get; set; }
        public string MonthName { get; set; }
        public int MonthNumber { get; set; }
        public int YearNumber { get; set; }
        public System.DateTime AddedDate { get; set; }
        public long AddedBy { get; set; }

        public List<MaterialWastageItemModel> MaterialWastageItemModels { get; set; }
    }
}