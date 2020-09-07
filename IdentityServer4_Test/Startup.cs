using IdentityServer.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "IdentityServer.Cookie";
                config.LoginPath = "/Auth/Login";
            });

            //UseUnMemoryDb(services); // use in memory data
            UseDb(services); // use the database

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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private void UseUnMemoryDb(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(config =>
            {
                config.UseInMemoryDatabase("Memory");
            });

            services.AddIdentityServer()
                // AddAspNetIdentity helps IdentityServer4 understand the Microsoft's
                // Identity model for the user and how to query for this model. The
                // AddIdentity function above is from Microsoft's Identity package.
                .AddAspNetIdentity<IdentityUser>()
                .AddInMemoryApiResources(Configuration.GetApis())
                .AddInMemoryIdentityResources(Configuration.GetIdentityResources())
                .AddInMemoryClients(Configuration.GetClients())
                .AddDeveloperSigningCredential();
        }

        private void UseDb(IServiceCollection services)
        {
            var dbConnectionString = _config.GetConnectionString("SqlServer");
            var migrationAssembly = typeof(Startup).Assembly.GetName().Name;

            services.AddDbContext<AppDbContext>(config =>
            {
                config.UseSqlServer(dbConnectionString);
            });

            services.AddIdentityServer()
                .AddAspNetIdentity<IdentityUser>()

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
                })

                .AddDeveloperSigningCredential();
        }
    }
}
