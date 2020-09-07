using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Client
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
                    config.ClientId = "client_1"; // identify the client
                    config.ClientSecret = "secret";
                    config.SaveTokens = true; // indicate to save token in our cookies
                    config.ResponseType = "code"; // indicate the authentication flow 

                    ConfigureCookieRequest(config);

                    // Added this line to be able to call the Api, after having gotten
                    // the token from IdentityServer
                    config.Scope.Add("Api");
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

        private static void ConfigureCookieRequest(OpenIdConnectOptions config)
        {
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
            config.Scope.Add("api.scope");
        }
    }
}
