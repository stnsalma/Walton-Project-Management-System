using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.Commercial
{
    public class VmProjectLc
    {
        public VmProjectLc()
        {
            ProjectMasterModel = new ProjectMasterModel();
            ProjectLcModel = new ProjectLcModel();
            PermissionModel=new LcOpeningPermissionModel();
            LcOpeningPermissionFileModels=new List<LcOpeningPermissionFileModel>();
            LcOpeningPermissionFileModel=new LcOpeningPermissionFileModel();
            LcOpeningPermissionModel=new LcOpeningPermissionModel();
        }
        public ProjectMasterModel ProjectMasterModel { get; set; }
        public ProjectLcModel ProjectLcModel { get; set; }
        public string PoNumber { get; set; }
        public bool Lc1 { get; set; }
        public bool Lc2 { get; set; }
        public LcOpeningPermissionModel PermissionModel { get; set; }
        public List<LcOpeningPermissionFileModel> LcOpeningPermissionFileModels { get; set; }
        public LcOpeningPermissionFileModel LcOpeningPermissionFileModel { get; set; }
        public LcOpeningPermissionModel LcOpeningPermissionModel { get; set; }
    }
}