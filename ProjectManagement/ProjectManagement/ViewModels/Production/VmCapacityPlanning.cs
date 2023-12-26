using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Antlr.Runtime.Misc;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Production
{
    public class VmCapacityPlanning
    {
        public VmCapacityPlanning()
        {
            ProShiftModels=new List<Pro_Shift_Model>();
            ProShiftModel=new Pro_Shift_Model();
            ProShiftModels1 = new List<Pro_Shift_Model>();
            ProShiftModel1 = new Pro_Shift_Model();
            CapacityPlanningModels=new ListStack<Pro_CapacityPlanning_Model>();
            CapacityPlanningModel=new Pro_CapacityPlanning_Model();
        }
        public List<Pro_Shift_Model> ProShiftModels { get; set; }
        public Pro_Shift_Model ProShiftModel { get; set; }
        public List<Pro_Shift_Model> ProShiftModels1 { get; set; }
        public Pro_Shift_Model ProShiftModel1 { get; set; }

        public List<Pro_CapacityPlanning_Model> CapacityPlanningModels { get; set; }
        public Pro_CapacityPlanning_Model CapacityPlanningModel { get; set; }

        public List<Pro_Type_Model> ProTypeModels { get; set; }
        public Pro_Type_Model ProTypeModel { get; set; }

        public string ProductionType { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public string ProductFamily { get; set; }
        public string AllShift { get; set; }
        public string PhoneType { get; set; }
        public string Month { get; set; }
        public string MonNum1 { get; set; }
        public int? MonNum { get; set; }
        public int? Year { get; set; }
        public string Year1 { get; set; }
        public string currentDate { get; set; }
        public string forwardedDate { get; set; }
    }
}