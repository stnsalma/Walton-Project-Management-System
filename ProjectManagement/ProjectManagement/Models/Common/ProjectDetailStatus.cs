using System.Collections.Generic;

namespace ProjectManagement.Models.Common
{
    public class ProjectDetailStatus
    {
        public ProjectDetailStatus()
        {
            CommercialList = new List<DataObject>();
            ManagementList = new List<DataObject>();
            HardwareList = new List<DataObject>();
            SoftwareList = new List<DataObject>();
            ProjectManagerList = new List<DataObject>();
            CommonStatusObjects = new List<CommonStatusObject>();
        }
        public List<DataObject> CommercialList { get; set; }
        public List<DataObject> ManagementList { get; set; }
        public List<DataObject> HardwareList { get; set; }
        public List<DataObject> SoftwareList { get; set; }
        public List<DataObject> ProjectManagerList { get; set; }
        public List<CommonStatusObject> CommonStatusObjects { get; set; }
    }
}