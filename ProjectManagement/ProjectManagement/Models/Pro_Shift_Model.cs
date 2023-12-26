using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class Pro_Shift_Model
    {
        public int IsRemoved { get; set; }
        public long Id { get; set; }
        public DateTime? TotalDays { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public int? TotalCategory1 { get; set; }
        public int? TotalTeam { get; set; }
        public int? Percentage { get; set; }
        public int? DaysCount { get; set; }
        public string Team { get; set; }
        public string Holidays { get; set; }
        public string LineType { get; set; }
        public string AssemblyCategory { get; set; }
        public string HousingCategory { get; set; }
        public string SmtCategory { get; set; }
        public string CategoryName { get; set; }
        public string ProjectName { get; set; }
        public string ProductFamily { get; set; }
        public string ProjectType { get; set; }
        public string ProductName { get; set; }
        public List<string> Category { get; set; }
        public bool IsActive { get; set; }
        public string AllShift { get; set; }
        public string AllDays { get; set; }
        public string PhoneType { get; set; }
        public string ProductionType { get; set; }
        public string Month { get; set; }
        public int? ShiftPerDay { get; set; }
        public int? ProductionDaysPerMonth { get; set; }
        public int? MonNum { get; set; }
        public int? Year { get; set; }
        public string Line { get; set; }
        public string Shift_1 { get; set; }
        public string Shift11 { get; set; }
        public string Shift_2 { get; set; }
        public string Shift_3 { get; set; }
        public string Shift_4 { get; set; }
        public string Shift_5 { get; set; }
        public string Shift_6 { get; set; }
        public string Shift_7 { get; set; }
        public string Product { get; set; }
        public decimal? HoursPerShift { get; set; }
        public decimal? ChangeOverTime { get; set; }
        public int? TotalShift { get; set; }
        public long? Added { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}