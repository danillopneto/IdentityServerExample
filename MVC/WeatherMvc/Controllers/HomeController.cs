using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using WeatherMvc.Models;
using WeatherMvc.Services;
using WeatherMVC.Models;

namespace WeatherMvc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ITokenService _tokenService;

        public HomeController(ITokenService tokenService, ILogger<HomeController> logger)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _logger = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Weather()
        {
            using var client = new HttpClient();

            var tokenResponse = await _tokenService.GetToken("weatherapi.read");

            client.SetBearerToken(tokenResponse.AccessToken);

            var result = client
              .GetAsync("https://localhost:5445/weatherforecast")
              .Result;

            if (result.IsSuccessStatusCode)
            {
                var model = result.Content.ReadAsStringAsync().Result;
                var data = JsonConvert.DeserializeObject<List<WeatherData>>(model);
                return View(data);
            }
            else
            {
                throw new Exception("Unable to get content");
            }
        }
    }
}