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
            return (from t in _context.tracks
                    join s in _context.savedTracks on t.TrackId equals s.Track.TrackId
                    where (s.User.UserID == Helper.user.UserID)
                    select t).OrderByDescending(t => t);
        }
        public IActionResult Saved()
        {
            Helper.player = "";

            return View("Saved", LoadSaved());
        }

        public async Task<IActionResult> UnSaved(int id)
        {
            Helper.player = "";
            var savedtrack = _context.savedTracks.FirstOrDefault(s => s.Track.TrackId == id);

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
            await _helper.updateResentlyPlayed(id);

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
                savedtracks = (IOrderedQueryable<Tracks>)savedtracks.Where(s => s.Artist.ArtistName.Contains(searchString) || s.TrackName.Contains(searchString));
            }

            Helper.player = "";
            return View("Saved", savedtracks);
        }
    }
}
