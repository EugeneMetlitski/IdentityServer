using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApiOne
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", config =>
                {
                    // Tell api where to pass tokens to validate
                    config.Authority = "https://localhost:5001";

                    // Identify what resources trying to pass this token for authentication,
                    // identifying that it's this api that is trying to validate the token
                    config.Audience = "ApiOne";

                    // Points to the '/.well-known/openid-configuration' route
                    //config.MetadataAddress = "";
                });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
