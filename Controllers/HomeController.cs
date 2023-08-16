using JsonParser.Classes;
using JsonParser.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using telemetry_parser;

namespace JsonParser.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly TelemetrieParserConfig _configuration;

        public static int numer;

        static HomeController()
        {
            numer = 13;
        }

        public HomeController(ILogger<HomeController> logger, IOptions<TelemetrieParserConfig> conf)
        {
            _logger = logger;
            _configuration = conf.Value;

            var b = new HomeController(null, null);
 
        }

        public async Task<IActionResult> Index()
        {
            TelemetryParser p = new TelemetryParser();
            await p.Parse(_configuration);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}