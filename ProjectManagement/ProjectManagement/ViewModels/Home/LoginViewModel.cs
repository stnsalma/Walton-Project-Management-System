using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagement.ViewModels.Home
{
    public class LoginViewModel
    {
        public String username { get; set; }
        public String password { get; set; }
        public Boolean remember { get; set; }
        //public String CurrentDateTime { get; set; }
    }
}