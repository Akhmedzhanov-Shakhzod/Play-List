
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataLayer;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ArtistsController : Controller
    {
        private readonly DbPlayList _context;
        private readonly Helper _helper;

        public ArtistsController(ILogger<HomeController> logger,DbPlayList context)
        {
            _context = context;

            Helper.info = "";

            _helper = new Helper(_context);
            Helper.playLists = _helper.PlayLists();
        }

        public (List<Artists>, List<int>) LoadIndex()
        {
            var artists = (from a in _context.artists
                           select a).ToList();

            var artistsCountOfTracks = new List<int>();

            foreach (var artist in artists)
            {
                var count = _context.tracks.Count(t => t.Artist.ArtistId == artist.ArtistId);
                artistsCountOfTracks.Add(count);
            }
            return (artists, artistsCountOfTracks);
        }

        public List<Tracks> LoadEdit(int artistid)
        {
            return (from t in _context.tracks
                          where t.Artist.ArtistId == artistid
                          select t).ToList();
        }
        public IActionResult Index()
        {
            return View("Index",LoadIndex());
        }


        public IActionResult Edit(int artistid)
        {
            return View("Edit",LoadEdit(artistid));
        }

        public async Task<IActionResult> Delete(int trackid,int artistid)
        {
            var track = _context.tracks.FirstOrDefault(s => s.TrackId == trackid);

            if (track != null)
            {
                _context.tracks.Remove(track);
                await _context.SaveChangesAsync();
            }

            return View("Edit", LoadEdit(artistid));
        }

        public async Task<IActionResult> AddArtist(string ArtistName)
        {
            var artistindb = _context.artists.Where(a => a.ArtistName == ArtistName).FirstOrDefault();

            if (artistindb != null)
            {
                Helper.info = $"{ArtistName} alredy has in list watch carefully :) ";
                return View("Index", LoadIndex());
            }

            var artist = new Artists()
            {
                ArtistName = ArtistName
            };

            _context.artists.Add(artist);
            await _context.SaveChangesAsync();

            return View("Index", LoadIndex());
        }

        public async Task<IActionResult> DeleteArtist(int artistid)
        {
            var artist = _context.artists.FirstOrDefault(a => a.ArtistId == artistid);

            if (artist != null)
            {
                _context.artists.Remove(artist);
                await _context.SaveChangesAsync();
            }
            return View("Index", LoadIndex());
        }
    }
}
