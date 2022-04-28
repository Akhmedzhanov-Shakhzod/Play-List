using Microsoft.AspNetCore.Mvc;
using WebApplication1.DataLayer;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UsersController : Controller
    {
        private readonly DbPlayList _context;
        private readonly Helper _helper;
        public UsersController(ILogger<HomeController> logger, DbPlayList context)
        {
            _context = context;

            _helper = new Helper(_context);
            Helper.playLists = _helper.PlayLists();
        }
        public List<Users> LoadUsers()
        {
            return (from u in _context.users select u).OrderByDescending(u => u.UserAccessLevel).ToList();
        }
        public List<IQueryable<PlayList>> LoadUsersPlayLists()
        {
            var list = new List<IQueryable<PlayList>>();
            foreach(var u in LoadUsers())
            {
                var playlist = from up in _context.playLists
                               where up.Author == u.UserName
                               select up;
                list.Add(playlist);
            }
            return list;
        }

        public IQueryable<Tracks> LoadTracksFromPlayList(int playlistId)
        {
            return from t in _context.tracks
                    join tp in _context.tracksInPlayList on t.TrackId equals tp.Track.TrackId
                    where tp.PlayList.Id == playlistId
                    select t;
        }
        public IActionResult Index()
        {
            return View("Index", (LoadUsers(),LoadUsersPlayLists()));
        }

        public IActionResult UserPlaylist(int playlistId)
        {
            var playlist = _context.playLists.Find(playlistId);
            return View("UserPlayList",(playlist,LoadTracksFromPlayList(playlistId)));
        }
        public async Task<IActionResult> DeletePlayList(int playlistId)
        {
            var playlist = _context.playLists.Find(playlistId);
            _context.playLists.Remove(playlist);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DeleteFromPlaylist(int trackId, int playlistId)
        {
            var trackInPlayList = _context.tracksInPlayList.First(tp => tp.Track.TrackId == trackId && tp.PlayList.Id == playlistId);

            _context.tracksInPlayList.Remove(trackInPlayList);
            await _context.SaveChangesAsync();

            var playlist = _context.playLists.Find(playlistId);
            return View("UserPlayList", (playlist, LoadTracksFromPlayList(playlistId)));
        }

    }
}
