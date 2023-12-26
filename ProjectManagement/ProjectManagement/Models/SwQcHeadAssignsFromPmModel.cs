using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace ProjectManagement.Models
{
    public class SwQcHeadAssignsFromPmModel
    {
        public SwQcHeadAssignsFromPmModel()
        {
            FilesDetails = new List<FilesDetail>();
        }
        public List<FilesDetail> FilesDetails { get; set; }
        public long SwQcHeadAssignId { get; set; }
        public long ProjectPmAssignId { get; set; }
        public long ProjectMasterId { get; set; }
        public long ProjectOrderShipmentId { get; set; }
        public long ProjectManagerUserId { get; set; }
        public long SwQcHeadUserId { get; set; }
        public int? OrderNumber { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public System.DateTime PmToQcHeadAssignTime { get; set; }
        public string PmToQcHeadAssignComment { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? PmProjectFinishTime { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? SwQcHeadToQcAssignTime { get; set; }
        public string SwQcHeadToQcAssignComment { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? SwQcFinishedTime { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? SwQcHeadToPmSubmitTime { get; set; }
        public string SwQcHeadToPmForwardComment { get; set; }
        public string AssignCatagory { get; set; }
        public string PriorityFromPm { get; set; }
        public string ProjectManagerSampleType { get; set; }
        public int ProjectManagerSampleNo { get; set; }
        public string SoftwareVersionName { get; set; }
        public string SoftVersionName { get; set; }
        public int? SoftwareVersionNo { get; set; }
        public long? TestPhaseID { get; set; }
        public bool? IsFinalPhaseMP { get; set; }
        public string IsFinalPhaseMPs { get; set; }
        public string Status { get; set; }
        public string SwQcHeadAssignStatus { get; set; }
        public long? Added { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? AddedDate { get; set; }
        public long? Updated { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string TestPhaseName { get; set; }
        public int? OrderNuber { get; set; }
        public string OrderNumberOrdinal { get; set; }
        public string SwQcHeadStatus { get; set; }
        public string FieldTestFrom { get; set; }
        public string FieldTestID { get; set; }
        public string AccessoriesTestType { get; set; }
        public string AssignedPerson { get; set; }
        public string DaysDiff { get; set; }
        public int FeatureCount { get; set; }
        public int SmartCount { get; set; }
        public int UpcomingSwCount { get; set; }
        public int UpcomingFtCount { get; set; }
        public int WorkStatusSw { get; set; }
        public int WorkStatusFt { get; set; }
        public string RowNum { get; set; }
        public string UserFullName { get; set; }
        public string SupportingDocument { get; set; }

        public int? ForwardedForFullSwCheck { get; set; }
        public int? FullSoftwareConfirmed { get; set; }
        public int? ForwardedForFOTATest { get; set; }
        public int? FOTATestConfirmed { get; set; }
    }
}