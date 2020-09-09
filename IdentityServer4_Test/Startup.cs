using IdentityServer.Configurations;
using IdentityServer.Data;
using IdentityServer.Models;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // AddIdentity registers the services
            services.AddIdentity<IdentityUser, IdentityRole>(config =>
            {
                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication()
                .AddOpenIdConnect("oidc", "MYP Provider", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                    options.SaveTokens = true;

                    options.Authority = "https://localhost:5001/";
                    options.ClientId = "interactive.confidential";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code";

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                });

            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "IdentityServer.Cookie";
                config.LoginPath = "/Auth/Login";
            });

            UseInMemoryDb(services); // use in memory data
            //UseDb(services); // use the database

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }


        #region helper-functions
        
        private void UseInMemoryDb(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(config =>
            {
                config.UseInMemoryDatabase("Memory");
            });

            var builder = services.AddIdentityServer()
                .AddTestUsers(Models.TestUsers.Users)
                //.AddAspNetIdentity<IdentityUser>()
                .AddInMemoryApiScopes(Configuration.ApiScopes)
                .AddInMemoryIdentityResources(Configuration.IdentityResources)
                .AddInMemoryClients(Configuration.Clients);

            builder.AddDeveloperSigningCredential();
        }

        private void UseDb(IServiceCollection services)
        {
            var dbConnectionString = _config.GetConnectionString("SqlServer");
            var migrationAssembly = typeof(Startup).Assembly.GetName().Name;

            services.AddDbContext<AppDbContext>(config =>
            {
                config.UseSqlServer(dbConnectionString);
            });

            var builder = services.AddIdentityServer()
                .AddTestUsers(Models.TestUsers.Users)
                //.AddAspNetIdentity<IdentityUser>()

                // Get the clients, api, identity resources... The static configuration information.
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(
                        dbConnectionString,
                        sql => sql.MigrationsAssembly(migrationAssembly));
                })
                // For temporary operational data such as authorization codes and refresh tokens.
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(
                        dbConnectionString,
                        sql => sql.MigrationsAssembly(migrationAssembly));
                });

            builder.AddDeveloperSigningCredential();
        }

        #endregion
    }
}
