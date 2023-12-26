using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Commercial
{
    public class VmProjectBtrcNoc
    {
        public VmProjectBtrcNoc()
        {
            ProjectMasterModel = new ProjectMasterModel();
            ProjectBtrcNocModel = new ProjectBtrcNocModel();
        }
        public ProjectMasterModel ProjectMasterModel { get; set; }
        public ProjectBtrcNocModel ProjectBtrcNocModel { get; set; }

        public HttpPostedFileBase File { get; set; }
    }
}