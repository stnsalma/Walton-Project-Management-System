using System;

namespace ProjectManagement.Models
{
    public class NotificationModel
    {
        public long Id { get; set; }
        public long? ProjectMasterId { get; set; }
        public string Message { get; set; }
        public string AdditionalMessage { get; set; }
        public string Role { get; set; }
        public bool? IsViewd { get; set; }
        public int? ViewerId { get; set; }
        public DateTime? Added { get; set; }
        public long? AddedBy { get; set; }
        public string NotificationTime { get; set; }
    }
}