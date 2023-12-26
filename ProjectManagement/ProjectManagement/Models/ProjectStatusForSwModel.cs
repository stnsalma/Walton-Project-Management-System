using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectStatusForSwModel
    {
        #region ProjectInfo

        public long ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        #endregion

        #region SW

        public string QcAssigned { get; set; }
        public DateTime? QcAssignDate { get; set; }
        public string QcSubmitted { get; set; }
        public DateTime? QcSubmittedDate { get; set; }
        public string QcForwarded { get; set; }
        public DateTime? QcForwardDate { get; set; }
        #endregion
    }
}