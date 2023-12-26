using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Infrastructures.Helper
{
    public class NotificationObject
    {
        public long ModelId { get; set; }
        public long ProjectId { get; set; }
        public string AdditionalInformation { get; set; }
        public string MessageFromController { get; set; }


        public string ToUser { get; set; }
        public long FromUser { get; set; }
        public string Message { get; set; }
        public string AdditionalMessage { get; set; }
    }
}