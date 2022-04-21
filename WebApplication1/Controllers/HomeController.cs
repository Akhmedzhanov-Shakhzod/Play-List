﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DataLayer;
using WebApplication1.Models;
using System.Security.Cryptography;
using System.Text;

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

            Helper helper = new Helper(_context);
            Helper.playLists = helper.PlayLists();
        }

        public static string hashPassword(string password)
        {
            SHA1 sHA1 = SHA1.Create();
            byte[] passwordbytes = Encoding.ASCII.GetBytes(password);
            byte[] encrypte_bytes = sHA1.ComputeHash(passwordbytes);
            return Convert.ToBase64String(encrypte_bytes);
        }
        public IQueryable<Tracks> LoadIndex()
        {
            var tracks = from t in _context.tracks
                         select t;
            tracks = tracks.OrderByDescending(t => t);
            return tracks;
        }
        public IQueryable<Tracks>[] LoadMain()
        {
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

            return tracks;
        }

        public IQueryable<Users> LoadUsers()
        {
            return _context.users.Select(u => u).OrderByDescending(u => u.UserName);
        }
        public void updateResentlyPlayed(int id)
        {
            if (Helper.user != null)
            {
                var played = (from p in _context.resentlyPlayeds
                              where (p.UserId == Helper.user.UserID)
                              select p).ToList();

                ResentlyPlayed resentlyPlayed = new ResentlyPlayed()
                {
                    TrackId = id,
                    UserId = Helper.user.UserID
                };
                _context.resentlyPlayeds.Add(resentlyPlayed);
                _context.SaveChanges();

                if (played != null)
                {
                    foreach (var item in played)
                    {
                        if (item.TrackId == resentlyPlayed.TrackId)
                        {
                            _context.resentlyPlayeds.Remove(item);
                            _context.SaveChanges();
                        }
                    }
                }
            }
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
            Users user = new Users()
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

        public IActionResult OrderBy(string searchId)
        {

            var tracks = LoadIndex();

            switch (searchId)
            {
                case "1":
                    tracks = tracks.OrderBy(u => u.TrackName);
                    break;
                case "2":
                    tracks = tracks.OrderBy(u => u.Artist);
                    break ;
                case "3":
                    tracks = tracks.OrderBy(u => u.Listens);
                    break;
            }
            Helper.player = "";
            return View("Index", tracks);
        }

        public IActionResult Search(string searchString)
        {

            var tracks = LoadIndex();

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

            var track = await _context.tracks.FindAsync(id);

            track.Listens += 1;
            await _context.SaveChangesAsync();

            updateResentlyPlayed(id);

            return View("Index", LoadIndex());
        }

        public async Task<IActionResult> Saved(int id)
        {
            var savedtrack = _context.savedTracks.FirstOrDefault(s => s.TrackId == id);

            if (savedtrack != null)
            {
                if(savedtrack.UserId == Helper.user.UserID) return View("Index", LoadIndex());
            }
            SavedTracks saved = new SavedTracks()
            {
                UserId = Helper.user.UserID,
                TrackId = id
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
        public IActionResult Users()
        {
            return View("Users",LoadUsers());
        }
        public IActionResult Privacy()
        {
            return View();
        }
    }
}