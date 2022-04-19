using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DataLayer;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class SavedController : Controller
    {
        private readonly DbPlayList _context;


        public SavedController(DbPlayList context)
        {
            _context = context;

            Helper helper = new Helper(_context);
            Helper.playLists = helper.PlayLists();
        }
        public IActionResult Saved()
        {
            Helper.player = "";
            var savedtracks = (from t in _context.tracks
                               join s in _context.savedTracks on t.TrackId equals s.TrackId
                               where (s.UserId == Helper.user.UserID)
                               select t).OrderByDescending(t => t);

            return View("Saved", savedtracks);
        }

        public async Task<IActionResult> UnSaved(int id)
        {
            Helper.player = "";
            var savedtrack = _context.savedTracks.FirstOrDefault(s => s.TrackId == id);

            var savedtracks = (from t in _context.tracks
                                join s in _context.savedTracks on t.TrackId equals s.TrackId
                                where (s.UserId == Helper.user.UserID)
                                select t).OrderByDescending(t => t);

            if (savedtrack != null)
            {

                _context.savedTracks.Remove(savedtrack);
                await _context.SaveChangesAsync();
            }
            return View("Saved", savedtracks);
        }

        public async Task<IActionResult> Player(string scr, int id)
        {
            Helper.player = scr;

            var savedtracks = (from t in _context.tracks
                               join s in _context.savedTracks on t.TrackId equals s.TrackId
                               where (s.UserId == Helper.user.UserID)
                               select t).OrderByDescending(t => t);

            var track = await _context.tracks.FindAsync(id);

            track.Listens += 1;

            try
            {
                _context.Update(track);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return View("Saved", savedtracks);
        }

        public IActionResult OrderBy(string searchId)
        {

            var savedtracks = (from t in _context.tracks
                               join s in _context.savedTracks on t.TrackId equals s.TrackId
                               where (s.UserId == Helper.user.UserID)
                               select t).OrderByDescending(t => t);

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

            var savedtracks = (from t in _context.tracks
                               join s in _context.savedTracks on t.TrackId equals s.TrackId
                               where (s.UserId == Helper.user.UserID)
                               select t).OrderByDescending(t => t);

            if (!String.IsNullOrEmpty(searchString))
            {
                savedtracks = (IOrderedQueryable<Tracks>)savedtracks.Where(s => s.Artist.Contains(searchString) || s.TrackName.Contains(searchString));
            }

            Helper.player = "";
            return View("Saved", savedtracks);
        }
    }
}
