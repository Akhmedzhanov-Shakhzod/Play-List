using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DataLayer;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly DbPlayList _context;

        public HomeController(ILogger<HomeController> logger,DbPlayList context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult LogOut()
        {
            Helper.user = null;
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrate()
        {
            Users user = new Users()
            {
                UserName = Request.Form["UserName"],
                Password =  Request.Form["Password"],
                UserAccessLevel = Request.Form["UserAccessLevel"]
            };

            if (ModelState.IsValid)
            {
                var userindb = await _context.users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
             
                if (userindb == null)
                {
                    user.UserAccessLevel = Helper.ValidateAdmin(user.UserAccessLevel) ? "admin" : "user";
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    Helper.user = user;
                    return View("Index", _context);
                }
                else
                {
                    return View("Login",userindb.UserName);
                }
            }
            return RedirectToAction("Login");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Validate()
        {
            Users user = new Users()
            {
                UserName = Request.Form["UserName"],
                Password =  Request.Form["Password"],
            };

            if (ModelState.IsValid)
            {
                var userindb = await _context.users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
                
                if(userindb == null)
                {
                    return View("Login", Helper.Errors.UserNotExist);
                }
                else
                {
                    if(userindb.Password != user.Password) return View("Login", Helper.Errors.PassworInCorrect);

                    user.UserAccessLevel = userindb.UserAccessLevel;
                    Helper.user = user;
                    return View("Index", _context);
                }
            }
            return RedirectToAction("Login");
        }
        public IActionResult Index()
        {
            return View("Index",_context);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}