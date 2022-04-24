
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataLayer;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class MainController : Controller
    {
        private readonly DbPlayList _context;
        private readonly Helper _helper;

        public MainController(DbPlayList context)
        {
            _context = context;

            _helper = new Helper(_context);
            Helper.playLists = _helper.PlayLists();
        }
        public IQueryable<Tracks>[] LoadMain()
        {
            IQueryable<Tracks>[] tracks = new IQueryable<Tracks>[2];

            tracks[0] = (from t in _context.tracks
                         select t).OrderByDescending(t => t.Listens).Take(4);
            if (Helper.user != null)
            {
                tracks[1] = (from t in _context.tracks
                             join rp in _context.resentlyPlayeds.OrderByDescending(rp => rp.Id) on t.TrackId equals rp.TrackId
                             where (rp.UserId == Helper.user.UserID)
                             select t).Take(4);
            }
            return tracks;
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
        public IActionResult Index()
        {
            Helper.player = "";

            return View("Main",LoadMain());
        }
        public async Task<IActionResult> Player(string scr, int id)
        {

            Helper.player = scr;

            await _helper.IncrementListen(id);

            updateResentlyPlayed(id);

            return View("Main", LoadMain());
        }
    }
}
