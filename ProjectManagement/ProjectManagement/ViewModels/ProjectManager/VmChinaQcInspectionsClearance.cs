using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.ProjectManager
{
    public class VmChinaQcInspectionsClearance
    {
        public VmChinaQcInspectionsClearance() 
        {
            ChinaQcInspectionsClearanceModels1 = new List<ChinaQcInspectionsClearanceModel>();
            ChinaQcInspectionsClearanceModel1 = new ChinaQcInspectionsClearanceModel();

            ChinaQcInspectionsClearanceModels2 = new List<ChinaQcInspectionsClearanceModel>();
            ChinaQcInspectionsClearanceModel2 = new ChinaQcInspectionsClearanceModel();

            ChinaQcInspectionsClearanceModels3 = new List<ChinaQcInspectionsClearanceModel>();
            ChinaQcInspectionsClearanceModel3 = new ChinaQcInspectionsClearanceModel();

            ChinaQcInspectionsClearanceModels4 = new List<ChinaQcInspectionsClearanceModel>();
            ChinaQcInspectionsClearanceModel4 = new ChinaQcInspectionsClearanceModel();
        }

        public List<ChinaQcInspectionsClearanceModel> ChinaQcInspectionsClearanceModels1 { get; set; }
        public ChinaQcInspectionsClearanceModel ChinaQcInspectionsClearanceModel1 { get; set; }
        public List<ChinaQcInspectionsClearanceModel> ChinaQcInspectionsClearanceModels2 { get; set; }
        public ChinaQcInspectionsClearanceModel ChinaQcInspectionsClearanceModel2 { get; set; }
        public List<ChinaQcInspectionsClearanceModel> ChinaQcInspectionsClearanceModels3 { get; set; }
        public ChinaQcInspectionsClearanceModel ChinaQcInspectionsClearanceModel3 { get; set; }
        public List<ChinaQcInspectionsClearanceModel> ChinaQcInspectionsClearanceModels4 { get; set; }
        public ChinaQcInspectionsClearanceModel ChinaQcInspectionsClearanceModel4 { get; set; }
        public long? ProjectMasterId1 { get; set; }
        public long? ProjectMasterId2 { get; set; }
        public long? ProjectMasterId { get; set; }
        public string Orders { get; set; }
        public string ProjectName { get; set; }
        public long? OrderQuantity { get; set; }
    }
}