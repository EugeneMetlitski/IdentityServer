using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.Models;

namespace IdentityServer4_Test
{
    public static class Configuration
    {
        public static IEnumerable<ApiResource> GetApis() =>
            new List<ApiResource>
            {
                new ApiResource("ApiOne"),
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
                }
            };
    }
}
