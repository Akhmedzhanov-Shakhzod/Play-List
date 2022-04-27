
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
                             join rp in _context.resentlyPlayeds.OrderByDescending(rp => rp) on t.TrackId equals rp.Track.TrackId
                             where (rp.User.UserID == Helper.user.UserID)
                             select t).Take(4);
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

            await _helper.IncrementListen(id);

            await _helper.updateResentlyPlayed(id);

            return View("Main", LoadMain());
        }
    }
}
