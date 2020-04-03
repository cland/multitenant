using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAuditAPI2.Data
{
    public class SeedDatabase
    {
        public async static void Initialize(IServiceProvider serviceProvider, TenantInfo tenantInfo)
        {
            //var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            using var context = new ApplicationDbContext(tenantInfo);
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            context.Database.EnsureCreated();

            //await CreateUserRoles(serviceProvider);            

            if (!context.Users.Any())
            {
                IdentityUser user = new IdentityUser()
                {
                    Email = "admin@testemail.co.zw",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = "admin"
                };
                var result = await userManager.CreateAsync(user, "P@ssword123");
                if (result.Succeeded)
                {
                    string[] roles = new string[] {
                        SystemRoles.Role_SystemAdmin.ToString(),
                        SystemRoles.Role_Admin.ToString(),
                        SystemRoles.Role_Inspector.ToString(),
                        SystemRoles.Role_User.ToString()
                    };
                    //await userManager.AddToRoleAsync(user, "Admin");
                    await userManager.AddToRolesAsync(user, roles);
                }
            }


        } //end initialize

        private static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                        
            IdentityResult roleResult;

            //Adding Admin Roles multiple
            string[] roles = new string[] {
                SystemRoles.Role_Admin,                
                SystemRoles.Role_User
            };
            int i = 0;
            foreach (var item in roles)
            {
                var rolesCheck = await RoleManager.RoleExistsAsync(item);
                if (!rolesCheck)
                {
                    //create the roles and seed them to the database
                    roleResult = await RoleManager.CreateAsync(new IdentityRole {
                        Name = item, NormalizedName = item
                    });
                }
                i++;
            }

            //Assign Admin role to the main User here we have given our newly registered
            //login id for Admin management
         //   ApplicationUser user = await UserManager.FindByEmailAsync("xxx@gmail.com");
         //   var User = new ApplicationUser();
         //   await UserManager.AddToRoleAsync(user, "Admin");
        }

    } //end class
} //end namespace
