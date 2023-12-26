using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.AssignModels
{
    public class PmQcAssignModel
    {
        public PmQcAssignModel()
        {
            FilesDetails = new List<FilesDetail>();
        }
        public List<FilesDetail> FilesDetails { get; set; }
        public string AccessoriesTestType { get; set; }
        public long ProjectPmAssignId { get; set; }
        public long ProjectMasterId { get; set; }
        public long? SwQcAssignId { get; set; }
        public string ProjectName { get; set; }
        public String TypeName { get; set; }
        public long AssignUserId { get; set; }
        public String AssignUserName { get; set; }
        public DateTime AssignDate { get; set; }
        public long ProjectManagerUserId { get; set; }
        public String ProjectManagerUserName { get; set; }
        public long SwQcInchargeAssignId { get; set; }
        public String QcInchargeUserName { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime ProjectManagerAssignToQcInTime { get; set; }

        ////ProjectMasterModel////
        public long ProjectTypeId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierModelName { get; set; }
        public int? NumberOfSample { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? ApproxProjectFinishDate { get; set; }
        public string SupplierTrustLevel { get; set; }
        public bool? IsScreenTestComplete { get; set; }
        public bool? IsApproved { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ApproxProjectOrderDate { get; set; }
        public DateTime? ApproxShipmentDate { get; set; }
        public bool? IsNew { get; set; }
        public bool? IsProjectManagerAssigned { get; set; }
        public string ProjectType { get; set; }
        public bool? IsReorder { get; set; }
        public string OsName { get; set; }
        public string OsVersion { get; set; }
        public decimal? DisplaySize { get; set; }
        public string DisplayName { get; set; }
        public string ProcessorName { get; set; }
        public decimal? ProcessorClock { get; set; }
        public string Chipset { get; set; }
        public string FrontCamera { get; set; }
        public string BackCamera { get; set; }
        public string Ram { get; set; }
        public string Rom { get; set; }
        public string Battery { get; set; }
        public int? SimSlotNumber { get; set; }
        public string SlotType { get; set; }
        public string ProjectStatus { get; set; }
        public string ManagentComment { get; set; }
  
        ////custom property///
        public int? OrderNuber { get; set; }
        public string OrderNumberOrdinal { get; set; }
        public string ProjectManagerAssignComment { get; set; }
        public string TestPhaseName { get; set; }
        public int? ProjectManagerSampleNo { get; set; }

        //New addition 2019-02-23/// 
        public int? SoftwareVersionNo { get; set; }
        public string SoftwareVersionName { get; set; }
        public string PmToQcHeadAssignComment { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? PmToQcHeadAssignTime { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss tt}", ApplyFormatInEditMode = true)]
        public DateTime? SwQcHeadToQcAssignTime { get; set; }
        public long SwQcHeadUserId { get; set; }
        public string SourcingType { get; set; }
        public long SwQcHeadAssignId { get; set; }
        public string SupportingDocument { get; set; }
        public string Status { get; set; }
        //end//
    }
}