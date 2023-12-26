using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class TeamKpiPercentageListModel
    {

        public long ProjectMasterId { get; set; }
        public string SourcingType { get; set; }
        public string ShipmentType { get; set; }
        public int OrderNumber { get; set; }
        public string IsFinalShipment { get; set; }
        public DateTime? PoDate { get; set; }
        public DateTime? WarehouseEntryDate { get; set; }
        public DateTime? ProjectManagerClearanceDate { get; set; }
        public string KpiName { get; set; }
        public string Target1 { get; set; }
        public int Target { get; set; }
        public int NoOfTimeInspection { get; set; }
        public int Weight { get; set; }
        public int TotalList { get; set; }
        public decimal? TotalAverageAchievement { get; set; }
        public decimal? TotalAverageScore { get; set; }
        public int? TotalAverageScorePercent { get; set; }
       // public decimal GrandTotalAverageScore { get; set; }
        public decimal? TotalAverageScoreForFeature { get; set; }
        public decimal? TotalAverageScoreForSmart { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public int DaysPassed { get; set; }
        public string TotalDays { get; set; }
        public decimal Achievement { get; set; }
        public decimal Score { get; set; }
        public string Month { get; set; }
        //public string Month { get; set; }
        public int MonthNum { get; set; }
        public int? MonthDiff { get; set; }
        public int Year { get; set; }
        public bool? IsActive { get; set; }
        //public string Year { get; set; }


        //new add for kpi
        public string Department { get; set; }
        public string Section { get; set; }
        public string LineManager { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public string Status { get; set; }
        public decimal? ServiceLength { get; set; }
        public int? ServiceLength1 { get; set; }
        public string EmployeeCode { get; set; }
        public string KpiFor { get; set; }
        public string RoleName { get; set; }
       
        public decimal? YearKpiAchievement { get; set; }
        public decimal? YearKpiScore { get; set; }

        public decimal? YearlyKpiAchievement { get; set; }
        public decimal? YearlyKpiScore { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        
    }
}