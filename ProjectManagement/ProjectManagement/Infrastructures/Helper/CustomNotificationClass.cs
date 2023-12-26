using System.Collections.Generic;

namespace ProjectManagement.Infrastructures.Helper
{
    public class CustomNotificationClass
    {
        public long ProjectId { get; set; }
        public string ToUser { get; set; }
        public long FromUser { get; set; }
        public string Message { get; set; }
        public string AdditionalMessage { get; set; }
    }
}