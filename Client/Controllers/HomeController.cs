using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [Route("/")]
        public async Task<IActionResult> Index()
        {
            // ********** Retrieve access token **********

            var serverClient = _httpClientFactory.CreateClient();

            // Get all the information that we need from '/.well-known/openid-configuration' page on identity server
            var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync("https://localhost:5001");

            // Request our access token (it includes the flow we are using in the name 'RequestClientCredentialsTokenAsync')
            var tokenResponse = await serverClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,
                    ClientId = "client_id",
                    ClientSecret = "client_secret",
                    Scope = "ApiOne"
                });

            // ********** Retrieve secret data **********

            var apiClient = _httpClientFactory.CreateClient();

            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var response =  await apiClient.GetAsync("https://localhost:5201/secret");

            var content = await response.Content.ReadAsStringAsync();

            return Ok(new
            {
                access_token = tokenResponse.AccessToken,
                message = content
            });
        }
    }
}
