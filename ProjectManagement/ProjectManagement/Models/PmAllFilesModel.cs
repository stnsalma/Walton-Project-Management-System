using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class PmAllFilesModel
    {
        public long ProjectMasterId { get; set; }
        public string ProjectName { get; set; }
        public string ImageUpload1 { get; set; }
        public string VideoUpload1 { get; set; }
        public string pmGbImageUploadPath { get; set; }
        public string PmFinishingImageUploadPath { get; set; }
        public string PmLogoTypeImageUploadPath { get; set; }
        public string PmModelPrintImageUploadPath { get; set; }
        public string pmLabelImageUploadPath { get; set; }
        public string PmPhnAccessoriesEarphone { get; set; }
        public string PmPhnAccessoriesUSBCable { get; set; }
        public string PmPhnAccessoriesCharger { get; set; }
        public string PmPhnAccessoriesOTGCable { get; set; }
        public string PmPhnAccessoriesBackCover { get; set; }
        public string PmPhnAccessoriesFlipCover { get; set; }
        public string PmScreenProtectorImageUploadPath { get; set; }
        public string WalpaperUpload1 { get; set; }
        public string WalpaperUpload2 { get; set; }
        public string WalpaperUpload3 { get; set; }
        public string WalpaperUpload4 { get; set; }
        public string WalpaperUpload5 { get; set; }
        public string WalpaperUpload6 { get; set; }
        public string WalpaperUpload7 { get; set; }
    }
}