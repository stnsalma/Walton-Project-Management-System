using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectStatusForHwModel
    {
        #region ProjectInfo

        public long ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        #endregion

        #region HW

        public DateTime? ScreeningSampleSetSentDate { get; set; }
        public DateTime? RunningSampleSetSentDate { get; set; }
        public DateTime? FinishedSampleSetSentDate { get; set; }
        public DateTime? ScreeningSampleSetReceiveDate { get; set; }
        public DateTime? RunningSampleSetReceiveDate { get; set; }
        public DateTime? RunningSampleReceivedDate { get; set; }
        public DateTime? FinishedSampleReceivedDate { get; set; }
        public DateTime? ScreeningAssignedDate { get; set; }
        public DateTime? RunningAssignedDate { get; set; }
        public DateTime? FinishedAssignedDate { get; set; }
        public DateTime? ScreeningSubmittedDate { get; set; }
        public DateTime? RunningSubmittedDate { get; set; }
        public DateTime? FinishedGoodsSubmittedDate { get; set; }
        public DateTime? ScreeningCheckedDate { get; set; }
        public DateTime? RunningCheckedDate { get; set; }
        public DateTime? FinishedCheckedDate { get; set; }
        public DateTime? ScreeningForwardedDate { get; set; }
        public DateTime? RunningForwardedDate { get; set; }
        public DateTime? FinishedForwardedDate { get; set; }

        #endregion
    }
}