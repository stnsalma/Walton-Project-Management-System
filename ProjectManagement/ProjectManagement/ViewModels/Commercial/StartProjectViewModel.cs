using System.ComponentModel.DataAnnotations;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Commercial
{
    public class StartProjectViewModel
    {
        public StartProjectViewModel()
        {
            ProjectMasterModel = new ProjectMasterModel();
            ProjectPriceModel = new ProjectPriceModel();
        }
        public ProjectMasterModel ProjectMasterModel { get; set; }
        
        public decimal ApproximatePrice { get; set; }

        public ProjectPriceModel ProjectPriceModel { get; set; }
        public long ProjectDropdownId { get; set; }

    }
}