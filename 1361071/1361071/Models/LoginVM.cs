using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _1361071.Models
{
    public class LoginVM
    {
        public string Username { get; set; }
        public string RawPWD { get; set; }
        public bool Remember { get; set; }
    }
}