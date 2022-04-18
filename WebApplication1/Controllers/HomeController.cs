using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
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

                    _context.users.Add(user);
                    await _context.SaveChangesAsync();
                    Helper.user = user;

                    Helper.player = "";

                    IQueryable<Tracks>[] tracks = new IQueryable<Tracks>[2];

                    tracks[0] = (from t in _context.tracks
                                 select t).OrderByDescending(t => t.Listens).Take(4);
                    if (Helper.user != null)
                    {
                        tracks[1] = (from t in _context.tracks
                                     join p in _context.resentlyPlayeds on t.TrackId equals p.TrackId
                                     where (p.UserId == Helper.user.UserID)
                                     select t).OrderByDescending(t => t).Take(4);
                    }
                    return View("/Views/Main/Main.cshtml", tracks);
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
         
            string UserName = Request.Form["UserName"];
            string Password =  Request.Form["Password"];

            if (ModelState.IsValid)
            {
                var userindb = await _context.users.FirstOrDefaultAsync(u => u.UserName == UserName);
                
                if(userindb == null)
                {
                    return View("Login", Helper.Errors.UserNotExist);
                }
                else
                {
                    if(userindb.Password != Password) return View("Login", Helper.Errors.PassworInCorrect);

                    Helper.user = userindb;
                    Helper.player = "";

                    IQueryable<Tracks>[] tracks = new IQueryable<Tracks>[2];

                    tracks[0] = (from t in _context.tracks
                                 select t).OrderByDescending(t => t.Listens).Take(4);
                    if (Helper.user != null)
                    {
                        tracks[1] = (from t in _context.tracks
                                     join p in _context.resentlyPlayeds on t.TrackId equals p.TrackId
                                     where (p.UserId == Helper.user.UserID)
                                     select t).OrderByDescending(t => t).Take(4);
                    }
                    return View("/Views/Main/Main.cshtml", tracks);
                }
            }
            return RedirectToAction("Login");
        }

        public IActionResult OrderBy(string searchId)
        {

            var tracks = from u in _context.tracks
                        select u;

            switch (searchId)
            {
                case "1":
                    tracks = tracks.OrderBy(u => u.TrackName);
                    break;
                case "2":
                    tracks = tracks.OrderBy(u => u.Artist);
                    break ;
                case "3":
                    tracks = tracks.OrderByDescending(u => u.Listens);
                    break;
            }
            Helper.player = "";
            return View("Index", tracks);
        }

        public IActionResult Search(string searchString)
        {

            var tracks = from u in _context.tracks
                         select u;

            if (!String.IsNullOrEmpty(searchString))
            {
                tracks = tracks.Where(s => s.Artist.Contains(searchString) || s.TrackName.Contains(searchString));
            }

            Helper.player = "";
            return View("Index", tracks);
        }

        public async Task<IActionResult> Player(string scr,int id)
        {
            //var result = "<source src = \"";
            //result += scr;
            //result += "\" type = \"audio/mpeg\">";
            Helper.player = scr;

            var tracks = from u in _context.tracks
                         select u;
            tracks = tracks.OrderByDescending(u => u);

            var track = await _context.tracks.FindAsync(id);

            track.Listens += 1;

            try
            {
                _context.tracks.Update(track);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return View("Index", tracks);
        }

        public async Task<IActionResult> Saved(int id)
        {
            var savedtrack = _context.savedTracks.FirstOrDefault(s => s.TrackId == id);

            var tracks = from u in _context.tracks
                         select u;
            tracks = tracks.OrderByDescending(u => u);

            if (savedtrack != null)
            {
                if(savedtrack.UserId == Helper.user.UserID) return View("Index", tracks);
            }
            SavedTracks saved = new SavedTracks()
            {
                UserId = Helper.user.UserID,
                TrackId = id
            };
            _context.savedTracks.Add(saved);
            await _context.SaveChangesAsync();
            
            return View("Index", tracks);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var tracks = from u in _context.tracks
                         select u;
            tracks = tracks.OrderByDescending(u => u);

            var track = _context.tracks.FirstOrDefault(s => s.TrackId == id);
            
            if (track != null)
            {
                _context.tracks.Remove(track);
                await _context.SaveChangesAsync();
            }

            return View("Index", tracks);
        }
        public IActionResult Index()
        {
            Helper.player = "";
            var tracks = from u in _context.tracks
                         select u;
            tracks = tracks.OrderByDescending(u => u);
            return View("Index",tracks);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}