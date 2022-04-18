using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class PlayListController : Controller
    {
        public IActionResult Index()
        {
            return View("Index");
        }
    }
}
