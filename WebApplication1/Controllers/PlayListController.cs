using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataLayer;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class PlayListController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly DbPlayList _context;

        public PlayListController(ILogger<HomeController> logger, DbPlayList context)
        {
            _context = context;
            _logger = logger;

            Helper helper = new Helper(_context);
            Helper.playLists = helper.PlayLists();
        }

        public IQueryable<Tracks> Load()
        {
            var tracks = from t in _context.tracks
                         select t;
            tracks = tracks.OrderBy(t => t.Artist).ThenBy(t => t.TrackName);

            Helper.countUserPlaylist  = (from p in _context.playLists
                                         where (p.Author == Helper.user.UserName)
                                         select p).Count() + 1;

            return tracks;
        }

        public IActionResult Index()
        {
            return View("Index",Load());
        }
        
        public async Task<IActionResult> Create(IFormFile Image, List<int> Tracks)
        {
            string name = Request.Form["Name"];
            Helper.isPlaylistExist = false;

            ////
            ///
            var playlistindb = _context.playLists.FirstOrDefault(p => p.Name == name && p.Author == Helper.user.UserName);
            if (playlistindb != null)
            {
                Helper.isPlaylistExist = true;
                return View("Index", Load());
            }
            ////
            ///
            PlayList playList = new PlayList()
            {
                Name = name,
                Author = Helper.user.UserName
            };
            if (Image != null)
            {
                string pathpicture = Path.Combine("wwwroot/files/pictures", Image.FileName);

                using (var fileStream = new FileStream(pathpicture, FileMode.Create))
                {
                    await Image.CopyToAsync(fileStream);
                }
                pathpicture = Path.Combine("/files/pictures", Image.FileName);
                playList.Picture = pathpicture;
            }
            else
            {
                string pathpicture = "/files/pictures/default-for-playlist.png";
                playList.Picture = pathpicture;
            }

            _context.playLists.Add(playList);
            await _context.SaveChangesAsync();

            foreach(var trackid in Tracks)
            {
                var playlistWithTracks = new TracksInPlayList()
                {
                    TrackId = trackid,
                    PlayListId = playList.Id
                };
                _context.tracksInPlayList.Add(playlistWithTracks);
                await _context.SaveChangesAsync();
            }
            return View("Index", Load());
        }


    }
}
