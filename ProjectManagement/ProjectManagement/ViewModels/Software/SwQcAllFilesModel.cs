using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Software
{
    public class SwQcAllFilesModel
    {

        public SwQcAllFilesModel()
        {
            UploadedFiles1=new List<string>();
            
        }
        public long ProjectMasterId { get; set; }
        public List<string> UploadedFiles1 { get; set; }

        public string UploadedFile1 { get; set; }
      
        public string ProjectName { get; set; }
    }
}