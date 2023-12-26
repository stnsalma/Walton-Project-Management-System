using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Antlr.Runtime.Misc;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Commercial
{
    public class VmProjectPurchaseOrder
    {
        public VmProjectPurchaseOrder()
        {
            ProjectPurchaseOrderFormModel = new ProjectPurchaseOrderFormModel();
            ProjectPurchaseOrderHandsetModels = new List<ProjectPurchaseOrderHandsetModel>();
            ProjectPurchaseOrderConditionModels = new ListStack<ProjectPurchaseOrderConditionModel>();
            JigsAndFixtureModels = new ListStack<JigsAndFixtureModel>();
            JigsAndFixtureModel=new JigsAndFixtureModel();
        }
        public ProjectPurchaseOrderFormModel ProjectPurchaseOrderFormModel { get; set; }
        public List<ProjectPurchaseOrderHandsetModel> ProjectPurchaseOrderHandsetModels { get; set; }
        public List<ProjectPurchaseOrderConditionModel> ProjectPurchaseOrderConditionModels { get; set; }
        public List<JigsAndFixtureModel> JigsAndFixtureModels { get; set; }
        public JigsAndFixtureModel JigsAndFixtureModel { get; set; }
        public string PrintRequired { get; set; }
        public bool IsReorder { get; set; }
        public long PrintFormId { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ApproximateFinishDateForReorder { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ApproximateShipmentDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ApproximatePoDate { get; set; }
        public decimal? ApproximatePrice { get; set; }
        public decimal? FinalPrice { get; set; }
        public bool SendSmtCapacityWarningMail { get; set; }

    }
}