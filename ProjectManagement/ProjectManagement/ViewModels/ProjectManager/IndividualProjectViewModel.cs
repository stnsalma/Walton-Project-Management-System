using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.ProjectManager
{
    public class IndividualProjectViewModel
    {

        public IndividualProjectViewModel()
        {
            
            PmBootImageAnimationModel = new PmBootImageAnimationModel();

            PmWalpaperModel = new PmWalpaperModel();

            PmGiftBoxModel = new PmGiftBoxModel();

            PmIdModel = new PmIdModel();
            PmLabelsModel = new PmLabelsModel();
            PmScreenProtectorModel = new PmScreenProtectorModel();
            PmSwCustomizationModel = new PmSwCustomizationModel();
            PmServiceDocumentsModel = new PmServiceDocumentsModel();

            PmPhnAccessoriesModel = new PmPhnAccessoriesModel();

            PmPhnCameraModel = new PmPhnCameraModel();

            ProjectMasterModel = new ProjectMasterModel();

            PmPhnColorModel = new PmPhnColorModel();

            PmViewHwTestHybridModel = new PmViewHwTestHybridModel();
            PmSwCustomizationInitialModels=new List<PmSwCustomizationInitialModel>();

        }

       
        public PmBootImageAnimationModel PmBootImageAnimationModel { get; set; }

        public PmWalpaperModel PmWalpaperModel { get; set; }

        public PmGiftBoxModel PmGiftBoxModel { get; set; }

        public PmIdModel PmIdModel { get; set; }

        public PmLabelsModel PmLabelsModel { get; set; }

        public PmScreenProtectorModel PmScreenProtectorModel { get; set; }

        public PmSwCustomizationModel PmSwCustomizationModel { get; set; }

        public PmServiceDocumentsModel PmServiceDocumentsModel { get; set; }

        public ProjectMasterModel ProjectMasterModel { get; set; }

        public PmPhnAccessoriesModel PmPhnAccessoriesModel { get; set; }

        public PmPhnCameraModel PmPhnCameraModel { get; set; }

        public PmPhnColorModel PmPhnColorModel { get; set; }

      

        public List<PmSwCustomizationInitialModel> PmSwCustomizationInitialModels { get; set; }
        public List<PmSwCustomizationFinalModel> PmSwCustomizationFinalModels { get; set; }


        public PmViewHwTestHybridModel PmViewHwTestHybridModel { get; set; }

    }
}