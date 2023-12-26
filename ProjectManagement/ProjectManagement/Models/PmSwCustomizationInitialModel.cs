using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PmSwCustomizationInitialModel
    {


        public long PmSwCustomizationInitialId { get; set; }
        public string PmSwCustomizationMenu { get; set; }
        public string PmSwCustomizationPath { get; set; }
        public string PmSwCustomizationDefaultSetting { get; set; }
        public bool? IsSmartPhone { get; set; }
        public bool? IsFeaturePhone { get; set; }
        public bool? IsTablet { get; set; }
        public bool? IsWindowsTablet { get; set; }
        public bool? IsPowerBank { get; set; }

        public long ProjectPmAssignId { get; set; }
        public long AssignUserId { get; set; }

        public long ProjectMasterId { get; set; }
    }
}