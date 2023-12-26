using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectStatusForPmModel
    {
        #region ProjectInfo

        public long ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        #endregion

        #region PM

        public string PmAssigned { get; set; }
        public DateTime? PmAssignedDate { get; set; }
        public string PmForwardToSw { get; set; }
        public DateTime? PmForwardToSwDate { get; set; }
        public string PmForwardToHwForScreening { get; set; }
        public DateTime? PmForwardToHwForScreeningDate { get; set; }
        public string PmForwardToHwForRunning { get; set; }
        public DateTime? PmForwardToHwForRunningDate { get; set; }
        public string PmForardToHwForFinished { get; set; }
        public DateTime? PmForardToHwForFinishedDate { get; set; }

        #endregion
    }
}