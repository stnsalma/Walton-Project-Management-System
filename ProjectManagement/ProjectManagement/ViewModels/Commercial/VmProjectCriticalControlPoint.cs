using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Commercial
{
    public class VmProjectCriticalControlPoint
    {
        public VmProjectCriticalControlPoint()
        {
            ProjectMasterModel = new ProjectMasterModel();
            ProjectCriticalControlPointModel=new ProjectCriticalControlPointModel();
        }
        public ProjectMasterModel ProjectMasterModel { get; set; }
        public ProjectCriticalControlPointModel ProjectCriticalControlPointModel { get; set; }
        
    }
}