using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models.ManagementDashboard
{
    public class PieSlideDataModel
    {
        public long Id { get; set; }
        public string CreatorName { get; set; }
        public string ProfilePicture { get; set; }
        public string Description { get; set; }
        public DateTime AddedDate { get; set; }
        public long AddedBy { get; set; }
        public string FlagStatus { get; set; }
        public string WorkingRole { get; set; }
        public string Component { get; set; }
    }
}