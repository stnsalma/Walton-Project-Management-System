namespace ProjectManagement.ViewModels.CostManagement
{
    public class VmCostManTBD
    {
        public long CostMasterId { get; set; }
        public long CostManagementModelId { get; set; }
        public long ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public int? OrderNumber { get; set; }
        public string ProjectType { get; set; }
        public string OperatingSystem { get; set; }
        public decimal? DisplaySize { get; set; }
        public string CpuName { get; set; }
        public string ChipsetName { get; set; }
        public string Ram { get; set; }
        public string Rom { get; set; }
        public string SupplierName { get; set; }
        public decimal ProposedPrice { get; set; }

        public string PreviousPrices { get; set; }
        public int ProposalCount { get; set; }

    }
}