using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MvcClient
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config =>
                {
                    config.DefaultScheme = "Cookie";
                    config.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Cookie")
                .AddOpenIdConnect("oidc", config =>
                {
                    config.Authority = "https://localhost:5001";
                    // Identifying the client
                    config.ClientId = "client_id_mvc";
                    config.ClientSecret = "client_secret_mvc";
                    // Indicate to save token in our cookies
                    config.SaveTokens = true;
                    // Indicate the authentication flow to use (access token and id token retrieval)
                    config.ResponseType = "code";

                    // Configure mapping between the user end point to our cookie for claims.
                    config.ClaimActions.DeleteClaim("amr"); // remove amr from cookie session
                    config.ClaimActions.MapUniqueJsonKey("RawCoding.Grandma", "rc.grandma");

                    // After we get the id_token, make another round trip to get
                    // our claims. This will make the id_token smaller and is why
                    // the endpoint for getting claims exists. If don't want to
                    // make another round trip, set the IdentityServer to inject
                    // all the claims into the id_token.
                    config.GetClaimsFromUserInfoEndpoint = true;

                    // Clear the scope, so that we will not ask for profile scope,
                    // which is optional by openid but is included in Microsoft
                    // Identity library (it bloats the cookies).
                    config.Scope.Clear();
                    // Ask for openid scope which is mandatory in openid specs.
                    config.Scope.Add("openid");
                    // Request IdentityServer for custom scope
                    config.Scope.Add("rc.scope");

                    // Added this when calling ApiOne after getting token
                    config.Scope.Add("ApiOne");
                    config.Scope.Add("ApiTwo");
                });

            // This is for being able to call ApiOne
            services.AddHttpClient();

            services.AddControllersWithViews();
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
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
