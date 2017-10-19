using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace David.WebSite.Models.API.Login
{
    public class LoginRequest
    {
        public string account { get; set; }
        public string password { get; set; }
        public string code { get; set; }
        public string key { get; set; }
        public bool remember { get; set; }
    }
}