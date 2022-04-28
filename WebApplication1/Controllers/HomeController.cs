using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DataLayer;
using WebApplication1.Models;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbPlayList _context;
        private readonly Helper _helper;
        public HomeController(ILogger<HomeController> logger,DbPlayList context)
        {
            _context = context;

            _helper = new Helper(_context);
            Helper.playLists = _helper.PlayLists();
        }

        public static string hashPassword(string password)
        {
            SHA1 sHA1 = SHA1.Create();
            byte[] passwordbytes = Encoding.ASCII.GetBytes(password);
            byte[] encrypte_bytes = sHA1.ComputeHash(passwordbytes);
            return Convert.ToBase64String(encrypte_bytes);
        }
        public (IQueryable<Artists>, IQueryable<Genres>) LoadForFilter()
        {
            var artists = from a in _context.artists select a;
            var genres = from g in _context.genres select g;

            return (artists, genres);
        }
        public (IQueryable<Tracks>, (IQueryable<Artists>, IQueryable<Genres>)) LoadIndex()
        {
            var tracks = (from t in _context.tracks select t).OrderByDescending(t => t);

            return (tracks, LoadForFilter());
        }
        public IQueryable<Tracks>[] LoadMain()
        {
            IQueryable<Tracks>[] tracks = new IQueryable<Tracks>[2];

            tracks[0] = (from t in _context.tracks
                         select t).OrderByDescending(t => t.Listens).Take(4);
            if (Helper.user != null)
            {
                tracks[1] = (from t in _context.tracks
                             join rp in _context.resentlyPlayeds.OrderByDescending(rp => rp) on t.TrackId equals rp.Track.TrackId
                             where (rp.User.UserID == Helper.user.UserID)
                             select t).Take(4);
            }

            return tracks;
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
            string password = Request.Form["Password"];
            Users user = new()
            {
                UserName = Request.Form["UserName"],
                Password =  hashPassword(password),
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

                    return View("/Views/Main/Main.cshtml", LoadMain());
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

            Password = hashPassword(Password);

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

                    return View("/Views/Main/Main.cshtml", LoadMain());
                }
            }
            return RedirectToAction("Login");
        }

        public IActionResult Filter(int type)
        {
            int artistid = Convert.ToInt32(Request.Form["ArtistId"]);
            int genreid = Convert.ToInt32(Request.Form["GenreId"]);

            IQueryable<Tracks> tracks = null;

            switch (type)
            {
                case 1:
                    tracks = _context.tracks.Where(t => t.Artist.ArtistId == artistid);
                break;
                case 2:
                    tracks = _context.tracks.Where(t => t.Genre.GenreId == genreid);
                break;
            }
            Helper.player = "";
            return View("Index",(tracks, LoadForFilter()));
        }

        public IActionResult Search(string searchString)
        {

            var tracks = from t in _context.tracks select t;

            if (!String.IsNullOrEmpty(searchString))
            {
                tracks = tracks.Where(s => s.TrackName.Contains(searchString.Trim()));
            }

            Helper.player = "";
            return View("Index", (tracks, LoadForFilter()));
        }

        public async Task<IActionResult> Player(string scr,int id)
        {
            //var result = "<source src = \"";
            //result += scr;
            //result += "\" type = \"audio/mpeg\">";
            Helper.player = scr;

            await _helper.IncrementListen(id);

            await _helper.updateResentlyPlayed(id);

            return View("Index", LoadIndex());
        }

        public async Task<IActionResult> Saved(int id)
        {
            var savedtrack = _context.savedTracks.FirstOrDefault(s => s.Track.TrackId == id && s.User.UserID == Helper.user.UserID);

            if (savedtrack != null)
            {
                return View("Index", LoadIndex());
            }

            SavedTracks saved = new SavedTracks()
            {
                User = _context.users.Where(u => u.UserID == Helper.user.UserID).First(),
                Track = _context.tracks.Where(t => t.TrackId == id).First()
            };
            _context.savedTracks.Add(saved);
            await _context.SaveChangesAsync();
            
            return View("Index", LoadIndex());
        }

        public async Task<IActionResult> Delete(int id)
        {
            var track = _context.tracks.FirstOrDefault(s => s.TrackId == id);
            
            if (track != null)
            {
                _context.tracks.Remove(track);
                await _context.SaveChangesAsync();
            }

            return View("Index", LoadIndex());
        }
        public IActionResult Index()
        {
            Helper.player = "";
            return View("Index",LoadIndex());
        }

        public IActionResult Library()
        {
            return View("Views/Library/Index.cshtml");
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}