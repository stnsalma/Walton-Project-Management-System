﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Common
{
    public class ResponseMessage
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public long ReturnId { get; set; }
    }
}