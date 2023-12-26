using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Software
{
    public class SwFieldTestReportView
    {
        ///ProjectMaster model/////
        public long ProjectMasterId { get; set; }
        public long ProjectTypeId { get; set; }
        public string ProjectName { get; set; }
        public string SupplierName { get; set; }
        public string SupplierModelName { get; set; }
        public int? NumberOfSample { get; set; }
        public string ProjectType { get; set; }
        public string OsName { get; set; }
        public string OsVersion { get; set; }
        ////////////SwQcFieldTest/////////
        public long SwFieldTestId { get; set; }
        public Nullable<long> SwQcInchargeAssignId { get; set; }
        public string IssueOf { get; set; }
        public string ComparedWith { get; set; }
        public Nullable<long> Added { get; set; }
        public Nullable<System.DateTime> AddedDate { get; set; }
        public string FieldTestAssignCommentFromIncharge { get; set; }

        ////////////CmnUsers/////////
        public long CmnUserId { get; set; }
        public string UserFullName { get; set; }
        public string UserName { get; set; }
        public string EmployeeCode { get; set; }

        ////////////ProjectPmAssigns/////////

        public long ProjectPmAssignId { get; set; }
        public long ProjectOrderShipmentId { get; set; }
        public System.DateTime AssignDate { get; set; }
        public long AssignUserId { get; set; }
        public long ProjectManagerUserId { get; set; }
        public string ProjectHeadRemarks { get; set; }
    
         //////SwQcInchargeAssign/////

        public long HwQcInchargeAssignId { get; set; }
        public long HwQcInchargeUserId { get; set; }
        public long? HwQcInchargeAssignedBy { get; set; }
        public DateTime? HwQcInchargeAssignDate { get; set; }
        public long? ReceivedSampleQuantity { get; set; }
        public long? ReturnedSampleQuantuty { get; set; }
        public string IsScreeningTest { get; set; }
        public string IsRunningTest { get; set; }
        public string IsFinishedGoodTest { get; set; }
        public string TestPhase { get; set; }
        public DateTime? SampleSetReceiveDate { get; set; }
        public string ProjectManagerAssignComment { get; set; }
        public string ProjectManagerSampleType { get; set; }
        public int? ProjectManagerSampleNo { get; set; }
        public DateTime? ApproxPmToHwDeliveryDate { get; set; }
        public string PriorityFromPm { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }


        ////////// SwFieldTestDetail///////
  
        public long SwFieldTestDetailId { get; set; }
        public Nullable<System.DateTime> TestDate { get; set; }
        public string Location { get; set; }
        public string Severity { get; set; }
        public string Description { get; set; }
        public string Condition_Op_TT_dbm { get; set; }
        public string Condition_Op_TT_Bar { get; set; }
        public string Condition_Op_RB_dbm { get; set; }
        public string Condition_Op_RB_Bar { get; set; }
        public string Condition_Op_BL_dbm { get; set; }
        public string Condition_Op_BL_Bar { get; set; }
        public string Condition_Op_AT_dbm { get; set; }
        public string Condition_Op_AT_Bar { get; set; }
        public string Ref_Op_TT_dbm { get; set; }
        public string Ref_Op_TT_Bar { get; set; }
        public string Ref_Op_RB_dbm { get; set; }
        public string Ref_Op_RB_Bar { get; set; }
        public string Ref_Op_BL_dbm { get; set; }
        public string Ref_Op_BL_Bar { get; set; }
        public string Ref_Op_AT_dbm { get; set; }
        public string Ref_Op_AT_Bar { get; set; }
        public string Remarks { get; set; }


    }
}