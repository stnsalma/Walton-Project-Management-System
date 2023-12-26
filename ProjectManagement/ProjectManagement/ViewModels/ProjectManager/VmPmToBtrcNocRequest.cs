using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManagement.DAL.DbModel;
using ProjectManagement.Models;

namespace ProjectManagement.ViewModels.ProjectManager
{
    public class VmPmToBtrcNocRequest
    {



        public VmPmToBtrcNocRequest()
        {
        
            ProjectBtrcNocModel = new ProjectBtrcNocModel();
            ProjectBtrcNocDocuments = new ProjectBtrcNocDocumentModel();
        
        }

        public ProjectBtrcNocModel ProjectBtrcNocModel { get; set; }

        public ProjectBtrcNocDocumentModel ProjectBtrcNocDocuments { get; set; }

        public List<ProjectMasterModel> ProjectMasterModel { get; set; }


        public long  ProjectMasterId { get; set; }
        public List<FileShowModel> FilesWebServerPaths { get; set; } 
    }
}