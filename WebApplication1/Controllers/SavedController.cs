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

        public IQueryable<Tracks> LoadSavedTraks()
        {
            return (from t in _context.tracks
             join s in _context.savedTracks on t.TrackId equals s.Track.TrackId
             where (s.User.UserID == Helper.user.UserID)
             select t).OrderByDescending(t => t);
        }
        public (IQueryable<Artists>, IQueryable<Genres>) LoadForFilter()
        {
            var artists = (from a in _context.artists select a).OrderBy(a => a.ArtistName);
            var genres = (from g in _context.genres select g).OrderBy(g => g.GenreName);

            return (artists, genres);
        }
        public (IQueryable<Tracks>, (IQueryable<Artists>, IQueryable<Genres>)) LoadSaved()
        {
            return (LoadSavedTraks(), LoadForFilter());
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

        public IActionResult Filter(int type)
        {
            int artistid = Convert.ToInt32(Request.Form["ArtistId"]);
            int genreid = Convert.ToInt32(Request.Form["GenreId"]);

            IQueryable<Tracks> savedtracks = LoadSavedTraks();

            switch (type)
            {
                case 1:
                    savedtracks = savedtracks.Where(t => t.Artist.ArtistId == artistid);
                    break;
                case 2:
                    savedtracks = savedtracks.Where(t => t.Genre.GenreId == genreid);
                    break;
            }
            Helper.player = "";
            return View("Saved", (savedtracks, LoadForFilter()));
        }

        public IActionResult Search(string searchString)
        {

            var savedtracks = LoadSavedTraks();

            if (!String.IsNullOrEmpty(searchString))
            {
                savedtracks = savedtracks.Where(s => s.TrackName.Contains(searchString.Trim()));
            }

            Helper.player = "";
            return View("Saved", (savedtracks, LoadForFilter()));
        }
    }
}
