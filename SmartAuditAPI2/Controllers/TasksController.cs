using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartAuditAPI2.Data;

namespace MultiTenant.Controllers
{
    [Route("/{__tenant__=}/api/[controller]")]
    [Authorize(Roles = SystemRoles.Role_User)]
    [ApiController]
    public class TasksController : ControllerBase
    {        
        public IActionResult Get()
        {
            var ti = HttpContext.GetMultiTenantContext()?.TenantInfo;
            if (ti == null)return BadRequest("Missing tenant information");
            using var context = new ApplicationDbContext(ti);
            var identity = User.Identity as ClaimsIdentity;
            List<Claim> claims = identity.Claims.ToList();
            var data = new
            {
                
                Identity = identity.FindFirst("aud").Value.ToString(),
                tenantInfo = new {
                    ti.Id,
                    ti.Name,
                    ti.Identifier,
                    ti.ConnectionString
                }                
            };

            return new ObjectResult(data);
        }
    } //end class
} //end namespace