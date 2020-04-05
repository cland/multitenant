using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SmartAuditAPI2.Data
{
    public class ApplicationDbContext : MultiTenantIdentityDbContext

    {
        public ApplicationDbContext(TenantInfo tenantInfo) : base(tenantInfo)
        {
        }

        public ApplicationDbContext(TenantInfo tenantInfo, DbContextOptions<ApplicationDbContext> options)
            : base(tenantInfo, options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            //optionsBuilder.UseSqlite(ConnectionString);            
            optionsBuilder.UseNpgsql(ConnectionString);
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            #region "Seed Roles"
            builder.Entity<IdentityRole>().HasData(
                new { Id = "1", TenantId = TenantInfo.Identifier, Name = SystemRoles.Role_Admin, NormalizedName = SystemRoles.Role_Admin.ToUpper() },
                new { Id = "2", TenantId = TenantInfo.Identifier, Name = SystemRoles.Role_User, NormalizedName = SystemRoles.Role_User.ToUpper() }
            );
            #endregion

            #region "Seed Users"
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            string ADMIN_ID = Guid.NewGuid().ToString();
            string ROLE_ID = "1";
            //var hasher = new PasswordHasher();
            builder.Entity<IdentityUser>().HasData(new IdentityUser
            {
                Id = ADMIN_ID,
                UserName = "admin@myemail.com",
                NormalizedUserName = "ADMIN@MYEMAIL.COM",
                Email = "admin@myemail.com",
                NormalizedEmail = "ADMIN@MYEMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = HashPassword("P@ssw0rd1"),
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
                
            });
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = ROLE_ID,
                UserId = ADMIN_ID
            });
            #endregion
        }
        public static string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        } //end method HashPassword
    } //end class
} //end Namespace
