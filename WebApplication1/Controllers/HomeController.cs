using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Validate()
        {

            var userName = Request.Form["Username"];
            var password = Request.Form["Password"];


            return View("Index");
        }
        public IActionResult Registrate()
        {
            return View("Index");
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}