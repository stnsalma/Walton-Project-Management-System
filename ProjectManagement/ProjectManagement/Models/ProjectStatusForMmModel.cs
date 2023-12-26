using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectStatusForMmModel
    {
        #region ProjectInfo

        public long ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        #endregion



        #region MM

        public string InitialApproval { get; set; }
        public DateTime? InitialApprovalDate { get; set; }
        public string FinalApproval { get; set; }
        public DateTime? FinalApprovalDate { get; set; }

        #endregion

    }
}