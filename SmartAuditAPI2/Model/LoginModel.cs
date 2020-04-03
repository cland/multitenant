using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAuditAPI2.Model
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
    public class Person
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}
