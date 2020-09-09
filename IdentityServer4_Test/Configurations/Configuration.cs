using System.Collections.Generic;
using IdentityServer4.Models;

namespace IdentityServer.Configurations
{
    public static class Configuration
    {
        public static IEnumerable<IdentityResource> IdentityResources =>

            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                {
                    Name = "api.scope", UserClaims =
                    {
                        "rc.grandma",
                        "firstname.read",
                        "firstname.write"
                    }
                }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("Api", "rc.api.grandma")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "client_1",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "https://localhost:44348/signin-oidc" },
                    AllowedScopes =
                    {
                        "Api",
                        IdentityServer4.IdentityServerConstants.StandardScopes.OpenId, // "openid"
                        IdentityServer4.IdentityServerConstants.StandardScopes.Profile, // "profile"
                        "api.scope"
                    },

                    // Tell IdentityServer4 to load all the claims into id token
                    //AlwaysIncludeUserClaimsInIdToken = true,

                    // Turn off consent form at this point, later will enable it
                    RequireConsent = false
                }
            };
    }
}
