using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.ProjectManager
{
    public class ProjectAcknowledgementViewModel
    {
        public long PlanId { get; set; }
        public string ProjectName { get; set; }
        public List<PMAcknowledgementModel> SMTAcknowledgements { get; set; }
        public List<PMAcknowledgementModel> HousingAcknowledgements { get; set; }
        public List<PMAcknowledgementModel> BatteryAcknowledgements { get; set; }
        public List<PMAcknowledgementModel> AssemblyAcknowledgements { get; set; }
       
    }
}