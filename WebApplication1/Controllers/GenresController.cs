using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataLayer;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class GenresController : Controller
    {
        private readonly DbPlayList _context;
        private readonly Helper _helper;

        public GenresController(ILogger<HomeController> logger, DbPlayList context)
        {
            _context = context;

            Helper.info = "";

            _helper = new Helper(_context);
            Helper.playLists = _helper.PlayLists();
        }

        public (List<Genres>, List<int>) LoadIndex()
        {
            var genres = (from a in _context.genres
                           select a).ToList();

            var genresCountOfTracks = new List<int>();

            foreach (var genre in genres)
            {
                var count = _context.tracks.Count(t => t.Genre.GenreId == genre.GenreId);
                genresCountOfTracks.Add(count);
            }
            return (genres, genresCountOfTracks);
        }

        public List<Tracks> LoadEdit(int genreid)
        {
            return (from t in _context.tracks
                    where t.Genre.GenreId == genreid
                    select t).ToList();
        }
        public IActionResult Index()
        {
            return View("Index", LoadIndex());
        }


        public IActionResult Edit(int genreid)
        {
            return View("Edit", LoadEdit(genreid));
        }

        public async Task<IActionResult> Delete(int trackid, int genreid)
        {
            var track = _context.tracks.FirstOrDefault(s => s.TrackId == trackid);

            if (track != null)
            {
                _context.tracks.Remove(track);
                await _context.SaveChangesAsync();
            }

            return View("Edit", LoadEdit(genreid));
        }

        public async Task<IActionResult> AddGenre(string GenreName)
        {
            var genreindb = _context.genres.Where(g => g.GenreName == GenreName).FirstOrDefault();

            if (genreindb != null)
            {
                Helper.info = $"{GenreName} alredy has in list watch carefully :) ";
                return View("Index", LoadIndex());
            }

            var genre = new Genres()
            {
                GenreName = GenreName
            };

            _context.genres.Add(genre);
            await _context.SaveChangesAsync();

            return View("Index", LoadIndex());
        }

        public async Task<IActionResult> DeleteGenre(int genreid)
        {
            var genre = _context.genres.FirstOrDefault(g => g.GenreId == genreid);

            if (genre != null)
            {
                _context.genres.Remove(genre);
                await _context.SaveChangesAsync();
            }
            return View("Index", LoadIndex());
        }
    }
}
