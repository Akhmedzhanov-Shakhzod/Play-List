using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataLayer;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class SavedController : Controller
    {
        private readonly DbPlayList _context;
        private readonly Helper _helper;

        public SavedController(DbPlayList context)
        {
            _context = context;

            _helper = new Helper(_context);
            Helper.playLists = _helper.PlayLists();
        }

        public IOrderedQueryable<Tracks> LoadSaved()
        {
            var savedtracks = (from t in _context.tracks
                               join s in _context.savedTracks on t.TrackId equals s.TrackId
                               where (s.UserId == Helper.user.UserID)
                               select t).OrderByDescending(t => t);
            return savedtracks;
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
        public IActionResult Saved()
        {
            Helper.player = "";

            return View("Saved", LoadSaved());
        }

        public async Task<IActionResult> UnSaved(int id)
        {
            Helper.player = "";
            var savedtrack = _context.savedTracks.FirstOrDefault(s => s.TrackId == id);

            if (savedtrack != null)
            {
                _context.savedTracks.Remove(savedtrack);
                await _context.SaveChangesAsync();
            }
            return View("Saved", LoadSaved());
        }

        public async Task<IActionResult> Player(string scr, int id)
        {
            Helper.player = scr;
            var track = await _context.tracks.FindAsync(id);

            await _helper.IncrementListen(id);

            return View("Saved", LoadSaved());
        }

        public IActionResult OrderBy(string searchId)
        {
            var savedtracks = LoadSaved();
            switch (searchId)
            {
                case "1":
                    savedtracks = savedtracks.OrderBy(u => u.TrackName);
                    break;
                case "2":
                    savedtracks = savedtracks.OrderBy(u => u.Artist);
                    break;
                case "3":
                    savedtracks = savedtracks.OrderBy(u => u.Listens);
                    break;
            }
            Helper.player = "";
            return View("Saved", savedtracks);
        }

        public IActionResult Search(string searchString)
        {

            var savedtracks = LoadSaved();

            if (!String.IsNullOrEmpty(searchString))
            {
                savedtracks = (IOrderedQueryable<Tracks>)savedtracks.Where(s => s.Artist.Contains(searchString) || s.TrackName.Contains(searchString));
            }

            Helper.player = "";
            return View("Saved", savedtracks);
        }
    }
}
