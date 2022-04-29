using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DataLayer;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class TracksController : Controller
    {
        private readonly DbPlayList _context;
        private readonly Helper _helper;

        public TracksController(DbPlayList context)
        {
            _context = context;

            _helper = new Helper(_context);
            Helper.playLists = _helper.PlayLists();
        }
        public (IQueryable<Artists>, IQueryable<Genres>) LoadForFilter()
        {
            var artists = (from a in _context.artists select a).OrderBy(a => a.ArtistName);
            var genres = (from g in _context.genres select g).OrderBy(g => g.GenreName);

            return (artists, genres);
        }
        public (IQueryable<Tracks>, (IQueryable<Artists>, IQueryable<Genres>)) LoadHomeIndex()
        {
            var tracks = (from t in _context.tracks select t).OrderByDescending(t => t);

            return (tracks, LoadForFilter());
        }

        public IActionResult Index()
        {
            return View("AddTrack",("",LoadForFilter()));
        }

        [HttpPost("FileUpload")]
        public async Task<IActionResult> AddTrack(List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {

                int Artistid = Convert.ToInt32(Request.Form["Artist"]);
                int Genreid = Convert.ToInt32(Request.Form["Genre"]);

                ////
                ////
                var trackndb = await _context.tracks.FirstOrDefaultAsync(u => u.Audio == "/files/tracks/" + files[0].FileName && u.Artist.ArtistId == Artistid);
                if(trackndb != null) return View("AddTrack", (Helper.Errors.TrackAlreadyExist,LoadForFilter()));
                ////
                ////

                var Artist = await _context.artists.Where(a => a.ArtistId == Artistid).FirstAsync();
                var Genre = await _context.genres.Where(a => a.GenreId == Genreid).FirstAsync();

                // путь к папке Files
                string pathaudio = "wwwroot/files/tracks/" + files[0].FileName;
                // сохраняем файл в папку Files в каталоге wwwroot
                using (var fileStream = new FileStream(pathaudio, FileMode.Create))
                {
                    await files[0].CopyToAsync(fileStream);
                }
                pathaudio = "/files/tracks/" + files[0].FileName;
                Tracks track = new Tracks();

                track.TrackName = files[0].FileName;
                
                track.Artist = Artist;
                track.Genre = Genre;

                track.Listens = 0;
                track.Audio = pathaudio;

                if (files.Count > 1)
                {
                    string pathpicture = Path.Combine("wwwroot/files/pictures", files[1].FileName);

                    using (var fileStream = new FileStream(pathpicture, FileMode.Create))
                    {
                        await files[1].CopyToAsync(fileStream);
                    }
                    pathpicture = Path.Combine("/files/pictures", files[1].FileName);
                    track.Picture = pathpicture;
                }
                else
                {
                    string pathpicture = "/files/pictures/default.png";
                    track.Picture = pathpicture;
                }

                _context.tracks.Add(track);
                await _context.SaveChangesAsync();

                Helper.player = "";


                return View("Views/Home/Index.cshtml",LoadHomeIndex());

                //string sss = "";
                //await using (var ms = new MemoryStream())
                //{
                //    files[0].CopyTo(ms);
                //    var fileBytes = ms.ToArray();

                //    string s = Convert.ToBase64String(fileBytes);

                //    foreach (var file in fileBytes)
                //        sss += file;
                //    act on the Base64 data

                //    track.Audio = fileBytes;
                //}
                //return Ok(new { count = files.Count, lengh = files[0].Length });
            }
            return View();
        }
    }
}
