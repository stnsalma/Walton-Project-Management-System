using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.Models
{
    public class HashtagModel
    {
        public long HashtagId { get; set; }
        public string HashtagName { get; set; }
        public long? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
    }
}