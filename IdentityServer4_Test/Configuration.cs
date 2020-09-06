using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.Models;

namespace IdentityServer4_Test
{
    public static class Configuration
    {
        // There are 2 different type of scopes, one comes in the form of
        // resource end point, such as Apis as a resource you are trying to access.
        // Second resource is stuff like claims, e.g. email, telephone number are
        // another type of scope.

        // Register the scopes that can belong to a user
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                // Profile resource bloats the access_token, but is required if
                // client is using .Net OpenIdConnect NugetPackage to connect
                // to the IdentityServer.
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiResource> GetApis() =>
            new List<ApiResource>
            {
                new ApiResource("ApiOne"),
                new ApiResource("ApiTwo"),
            };

        public static IEnumerable<Client> GetClients() =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "client_id",
                    ClientSecrets = { new Secret("client_secret".ToSha256()) },
                    // How is it going to retrieve the access token (flow of access token requests)
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // What can this access token be used for
                    AllowedScopes = { "ApiOne" }
                },
                new Client
                {
                    ClientId = "client_id_mvc",
                    ClientSecrets = { new Secret("client_secret_mvc".ToSha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "https://localhost:44348/signin-oidc" },
                    AllowedScopes =
                    {
                        "ApiOne",
                        "ApiTwo",
                        IdentityServer4.IdentityServerConstants.StandardScopes.OpenId, // "openid"
                        IdentityServer4.IdentityServerConstants.StandardScopes.Profile // "profile"
                    }
                }
            };
    }
}
