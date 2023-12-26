using System.Collections.Generic;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Software
{
    public class SwBatteryViewModel
    {
        public SwBatteryViewModel()
        {
            ProjectMasterModel = new ProjectMasterModel();
            ProjectMasterModelsList = new List<ProjectMasterModel>();
            SwQcBatteryAssignIssueModelsList = new List<SwQcBatteryAssignIssueModel>();
        }
        public bool IsEdit { get; set; }
        public long AssignId { get; set; }
        public ProjectMasterModel ProjectMasterModel { get; set; }
        public List<ProjectMasterModel> ProjectMasterModelsList { get; set; }
        public List<SwQcBatteryAssignIssueModel> SwQcBatteryAssignIssueModelsList { get; set; }
        public List<SwQcAssignModel> SwQcAssignModels { get; set; }
    }
}