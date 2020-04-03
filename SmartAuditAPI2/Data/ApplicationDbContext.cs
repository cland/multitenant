using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
            #region "Seed Data"
            builder.Entity<IdentityRole>().HasData(
                new { Id = "1", TenantId = TenantInfo.Identifier, Name = SystemRoles.Role_Admin, NormalizedName=SystemRoles.Role_Admin.ToUpper()},                
                new { Id = "2", TenantId = TenantInfo.Identifier, Name = SystemRoles.Role_User, NormalizedName = SystemRoles.Role_User.ToUpper() }
            );
            #endregion
        }

    }
}
