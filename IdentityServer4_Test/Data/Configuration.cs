using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.Models;

namespace IdentityServer.Data
{
    public static class Configuration
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() =>

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

        public static IEnumerable<ApiResource> GetApis() =>
            new List<ApiResource>
            {
                new ApiResource("Api", new string[] { "rc.api.grandma" }),
            };

        public static IEnumerable<Client> GetClients() =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "client_1",
                    ClientSecrets = { new Secret("secret".ToSha256()) },
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
                },
                //new Client
                //{
                //    ClientId = "client_id",
                //    ClientSecrets = { new Secret("client_secret".ToSha256()) },
                //    // How is it going to retrieve the access token (flow of access token requests)
                //    AllowedGrantTypes = GrantTypes.ClientCredentials,
                //    // What can this access token be used for
                //    AllowedScopes = { "Api" },
                //},
            };
    }
}
