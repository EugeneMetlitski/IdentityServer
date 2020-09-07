using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<string> Secret()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            // This will have the information about claims
            var claims = User.Claims.ToList();
            var _access_token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            // id_token is primarily about our authentication state, e.g. user id is
            // authentication information, rather than personal user information such as email. 
            // It's used to identify who is authenticated and how they are authenticated.
            var _id_token = new JwtSecurityTokenHandler().ReadJwtToken(idToken);

            var result = await GetSecret(accessToken);

            return "access_token:\n" +
                   _access_token.ToString() +
                   "\nid_token\n" +
                   _id_token.ToString();
        }

        public async Task<string> GetSecret(string accessToken)
        {
            var apiClient = _httpClientFactory.CreateClient();

            apiClient.SetBearerToken(accessToken);

            var response = await apiClient.GetAsync("https://localhost:5201/secret");

            var content = await response.Content.ReadAsStringAsync();

            return content;
        }
    }
}
