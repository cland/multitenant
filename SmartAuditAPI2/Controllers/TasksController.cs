using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartAuditAPI2.Data;

namespace MultiTenant.Controllers
{
    [Route("/{__tenant__=}/api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        public TasksController()
        {
            
        }
        public IActionResult Get()
        {
            var ti = HttpContext.GetMultiTenantContext()?.TenantInfo;
            if (ti == null)
            {
                return BadRequest("Missing tenant information");
            }
            var tenantInfoDTO = new
            {
                ti.Id,
                ti.Name,
                ti.Identifier,
                ti.ConnectionString
            };

            using var context = new ApplicationDbContext(ti);
            
            var rolesInDb = context.Roles.ToList();

            return new ObjectResult(rolesInDb);
        }
    } //end class
} //end namespace