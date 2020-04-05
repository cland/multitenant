using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
            using var context = new ApplicationDbContext(tenantInfo);   //SUCCESS
            

            //ISSUE 2: If i seed OnModelCreating, IdenityUser requires TenantId yet I can't add that when creating the user.
            context.Database.EnsureCreated();

            IPasswordHasher<IdentityUser> passwordHasher = new PasswordHasher<IdentityUser>();
            //ISSUE 1: Setting the UserManager object from the injected service provider is using an ApplicationDbContext without a TenantInfo             
            //resulting in ConnectionString being null.
            var userManager = new UserManager<IdentityUser>(
                new UserStore<IdentityUser>(context),
                null,   //OptionsAccessor
                passwordHasher,   // IPasswordHasher<IdentityUser> passwordHasher
                null,   // userValidators
                null,   // passwordValidators
                null,   // keyNormalizer
                null,   // IdentityErrorDescriver errors
                serviceProvider,  //IServiceProvider services
                null    // ILogger logger
                ); //serviceProvider.GetRequiredService<UserManager<IdentityUser>>();            

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context), null, null, null, null);

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
                        SystemRoles.Role_Admin.ToString(),
                        SystemRoles.Role_User.ToString()
                    };

                    //IdentityResult roleResult;
                    bool adminRoleExists = await roleManager.RoleExistsAsync(roles[0]);
                    if (!adminRoleExists)
                    {
                        foreach(var r in roles)
                        {
                            bool thisRoleExists = await roleManager.RoleExistsAsync(r);
                            if (! thisRoleExists)
                            {
                                await roleManager.CreateAsync(new IdentityRole(r));
                            }
                        }
                    }
                    //await userManager.AddToRoleAsync(user, SystemRoles.Role_Admin.ToString());
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
