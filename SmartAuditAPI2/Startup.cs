﻿
using System.Text;
using AutoMapper;
using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using SmartAuditAPI2.Data;


namespace SmartAuditAPI2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddAutoMapper(typeof(Startup));

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var signinKey = Configuration.GetSection("Config")["signinkey"];

            services.AddMultiTenant().
                WithConfigurationStore().
                WithRouteStrategy();

            // Register the db context, but do not specify a provider/connection string since
            // these vary by tenant.
            services.AddDbContext<ApplicationDbContext>();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            )
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = "http://mylab.sytes.net",
                    ValidIssuer = "http://mylab.sytes.net",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signinKey))
                };
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
            });
        }// End ConfigureServices method

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(); // from old config, remove ???
            if (env.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseMultiTenant();

            var ti = new TenantInfo("finbuckle", "finbuckle-id1", "finbuckle-name", "Server=localhost;Port=5432;Uid=postgres;Pwd=pwd123;Database=Tenant1", null );
            SeedDatabase.Initialize(app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider,ti);
            ti = new TenantInfo("megacorp", "megacorp-id2", "megacorp-name", "Server=localhost;Port=5432;Uid=postgres;Pwd=pwd123;Database=Tenant2", null);
            SeedDatabase.Initialize(app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider, ti);
            ti = new TenantInfo("initech", "initech-id3", "Initech LLC", "Server=localhost;Port=5432;Uid=postgres;Pwd=pwd123;Database=Tenant3", null);
            SeedDatabase.Initialize(app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider, ti);

            app.UseAuthentication();
            app.UseHttpsRedirection();
        } //end Configure method
        
    } //end class
} //end namespace
