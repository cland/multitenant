using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartAuditAPI2.Data;
using SmartAuditAPI2.Dtos;
using SmartAuditAPI2.Model;

namespace SmartAuditAPI2.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private ApplicationDbContext context;
        private UserManager<IdentityUser> userManager;
        private readonly IMapper _mapper;
        public ValuesController(UserManager<IdentityUser> userManager ,ApplicationDbContext context,IMapper _mapper)
        {
            this.context = context;
            this._mapper = _mapper;
            this.userManager = userManager;
        }

        // GET api/values
        [HttpGet]
        [Authorize(Roles = SystemRoles.Role_Admin)]
        public async Task<ActionResult<List<IdentityUser>>> Get()
        {
            List<UserDTO> results = new List<UserDTO>() { };
            var users = context.Users;//.Select(_mapper.Map<ApplicationUser,UserDTO>);
            foreach(var u in users)
            {
                var roles = await userManager.GetRolesAsync(u);
                var user = _mapper.Map<IdentityUser, UserDTO>(u);
                user.roles = roles.ToList();
                results.Add(user);
            }
            return Ok(results);//   .Select(u => u.UserName).ToArray();
            //return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // GET api/status
        [HttpGet]
        [Route("status")]
        [AllowAnonymous]
        public ActionResult<string> GetStatus()
        {
            return "API running...";
        }

        // POST api/values
        [HttpPost]
        public ActionResult Post([FromBody] Person person)
        {
            return Ok(new {
                name = person.Firstname + " " + person.Lastname
            });
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
