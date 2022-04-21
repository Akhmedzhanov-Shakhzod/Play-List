using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.DataLayer;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers
{
    public class MainController : Controller
    {
        private readonly DbPlayList _context;
        public MainController(DbPlayList context)
        {
            _context = context;

            Helper helper = new Helper(_context);
            Helper.playLists = helper.PlayLists();
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
        public IActionResult Index()
        {
            Helper.player = "";

            return View("Main",LoadMain());
        }
        public async Task<IActionResult> Player(string scr, int id)
        {

            Helper.player = scr;

            IQueryable<Tracks>[] tracks = new IQueryable<Tracks>[2];

            tracks[0] = (from t in _context.tracks
                         select t).OrderByDescending(t => t.Listens).Take(4);

            var track = await _context.tracks.FindAsync(id);

            track.Listens += 1;
            await _context.SaveChangesAsync();

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
                await _context.SaveChangesAsync();

                if (played != null)
                {
                    foreach (var item in played)
                    {
                        if (item.TrackId == resentlyPlayed.TrackId)
                        {
                            _context.resentlyPlayeds.Remove(item);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                tracks[1] = (from t in _context.tracks
                             join p in _context.resentlyPlayeds.OrderByDescending(p => p) on t.TrackId equals p.TrackId
                             where (p.UserId == Helper.user.UserID)
                             select t).Take(4);
            }

            return View("Main", tracks);
        }
    }
}
