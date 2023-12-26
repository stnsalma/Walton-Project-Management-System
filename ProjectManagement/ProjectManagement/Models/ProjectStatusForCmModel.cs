using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class ProjectStatusForCmModel
    {
        #region ProjectInfo

        public long ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        #endregion

        #region CM

        public string ScreeeningForward { get; set; }
        public DateTime? ScreeeningForwardDate { get; set; }
        public string PoCreated { get; set; }
        public DateTime? PoCreationDate { get; set; }

        #endregion
    }
}