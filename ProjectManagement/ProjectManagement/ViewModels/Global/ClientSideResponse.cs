using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Global
{
    public class ClientSideResponse
    {
        public int MessageType { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ReturnValue { get; set; }
    }
}