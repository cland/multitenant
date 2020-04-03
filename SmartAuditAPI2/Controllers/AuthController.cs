using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartAuditAPI2.Data;
using SmartAuditAPI2.Dtos;
using SmartAuditAPI2.Model;

namespace SmartAuditAPI2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private UserManager<IdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private readonly IMapper _mapper;
        public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole>roleManager,IMapper _mapper, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this._mapper = _mapper;
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        [Route("insertuser")]
        [HttpPost]
        [Authorize(Roles = SystemRoles.Role_Admin )]
        public async Task<ActionResult> InsertUser([FromBody]UserDTO newUser)
        {
            var user = _mapper.Map<UserDTO, IdentityUser>(newUser);
            var result = await userManager.CreateAsync(user, newUser.Secret); 
            if (result.Succeeded)
            {
                await userManager.AddToRolesAsync(user, newUser.roles.ToArray());
                
                //var roleCheck = await roleManager.RoleExistsAsync("Admin");
                //if(roleCheck) await userManager.AddToRoleAsync(user, "Admin");                
                //await userManager.AddToRolesAsync(user,new []{
                //    "NInspector", "OfficeAdmin"
                //});
                //var mobileNoClaim = new Claim("MobileNo", "0823333", ClaimValueTypes.String);
            }
            var userroles = await userManager.GetRolesAsync(user);
            var userDto = _mapper.Map<UserDTO>(user);
            userDto.roles = userroles.ToList();
            return Ok(userDto);
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.Username);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var roles = await userManager.GetRolesAsync(user);
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName ),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Aud,user.UserName),
                    new Claim(JwtRegisteredClaimNames.Iat,DateTime.UtcNow.Ticks.ToString())
                    //,          new Claim("Office",(user.Office==null?"":user.Office))
                };
                foreach(var r in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, r.ToString()));
                }

                var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("Config")["signinkey"]));

                var token = new JwtSecurityToken(
                    issuer: "http://mylab.sytes.net",
                    audience: "http://mylab.sytes.net",
                    expires: DateTime.UtcNow.AddHours(1),
                    claims: claims,
                    signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(
                        signinKey, 
                        SecurityAlgorithms.HmacSha256)
                    );
                return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo,
                        location = "somelocation",
                        roles = roles.ToList()
                });
            }
            return Unauthorized();
        }
    }
}
