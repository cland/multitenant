using SmartAuditAPI2.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAuditAPI2.Dtos
{
    public class UserDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Office { get; set; }
        public string Secret { get; set; }
        public List<string> roles;
    }
}
