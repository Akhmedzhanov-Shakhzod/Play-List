using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DataLayer;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class PlayListController : Controller
    {
        private readonly DbPlayList _context;
        private readonly Helper _helper;

        public PlayListController(ILogger<HomeController> logger, DbPlayList context)
        {
            _context = context;

            _helper = new Helper(_context);
            Helper.playLists = _helper.PlayLists();
        }

        public IQueryable<Tracks> LoadIndex()
        {
            var tracks = from t in _context.tracks
                         select t;
            tracks = tracks.OrderBy(t => t.Artist).ThenBy(t => t.TrackName);

            Helper.countUserPlaylist  = (from p in _context.playLists
                                         where (p.Author == Helper.user.UserName)
                                         select p).Count() + 1;

            return tracks;
        }

        public (PlayList,IQueryable<Tracks>) LoadEditor(int id)
        {
            var playlist = (from p in _context.playLists
                            where (p.Id == id)
                            select p).ToList();

            var tracks = from tp in _context.tracksInPlayList
                         join t in _context.tracks
                         on tp.Track.TrackId equals t.TrackId
                         join p in _context.playLists
                         on tp.PlayList.Id equals p.Id
                         where p.Author == Helper.user.UserName && p.Id == id
                         select t;

            var tracksInPlayList = (playlist[0], tracks);
            return tracksInPlayList;
        }

        public IActionResult Index()
        {
            return View("Index",LoadIndex());
        }
        
        public async Task<IActionResult> Create(List<IFormFile> Image, List<int> Tracks)
        {
            string name = Request.Form["Name"];
            Helper.isPlaylistExist = false;

            ////
            ///
            var playlistindb = _context.playLists.FirstOrDefault(p => p.Name == name && p.Author == Helper.user.UserName);
            if (playlistindb != null)
            {
                Helper.isPlaylistExist = true;
                return View("Index", LoadIndex());
            }
            ////
            ///
            PlayList playList = new PlayList()
            {
                Name = name,
                Author = Helper.user.UserName
            };
            if (Image.Count > 0)
            {
                string pathpicture = Path.Combine("wwwroot/files/pictures", Image[0].FileName);

                using (var fileStream = new FileStream(pathpicture, FileMode.Create))
                {
                    await Image[0].CopyToAsync(fileStream);
                }
                pathpicture = Path.Combine("/files/pictures", Image[0].FileName);
                playList.Picture = pathpicture;
            }
            else
            {
                string pathpicture = "/files/pictures/default-for-playlist.jpg";
                playList.Picture = pathpicture;
            }

            _context.playLists.Add(playList);
            await _context.SaveChangesAsync();

            foreach(var trackid in Tracks)
            {
                var playlistWithTracks = new TracksInPlayList()
                {
                    Track = (Tracks)_context.tracks.Single(t => t.TrackId == trackid),
                    PlayList = (PlayList)_context.playLists.Single(p => p.Id == playList.Id)
                };
                _context.tracksInPlayList.Add(playlistWithTracks);
                await _context.SaveChangesAsync();
            }
            return View("Index", LoadIndex());
        }

        public IActionResult EditPage(int id)
        {
            Helper.player = "";

            return View("Edit", LoadEditor(id));
        }

        public async Task<IActionResult> Edit(int playlistid)
        {
            string name = Request.Form["Name"];
            Helper.isPlaylistExist = false;

            ////
            ///
            var playlistindb = _context.playLists.FirstOrDefault(p => p.Name == name && p.Author == Helper.user.UserName);
            if (playlistindb != null)
            {
                Helper.isPlaylistExist = true;
                return View("Edit", LoadEditor(playlistid));
            }
            ////
            ///

            var playlist = await _context.playLists.FirstOrDefaultAsync(p => p.Id == playlistid);

            playlist.Name = name;

            _context.playLists.Update(playlist);
            await _context.SaveChangesAsync();

            return View("Edit", LoadEditor(playlistid));
        }
        public async Task<IActionResult> Delete(int id,int playlistid)
        {

            var track = _context.tracksInPlayList.FirstOrDefault(t => t.PlayList.Id == playlistid && t.Track.TrackId == id);

            if (track != null)
            {
                _context.tracksInPlayList.Remove(track);
                await _context.SaveChangesAsync();
            }

            return View("Edit", LoadEditor(playlistid));
        }
        public async Task<IActionResult> DeletePlayList(int playlistid)
        {

            var track = _context.playLists.FirstOrDefault(p => p.Id == playlistid);

            _context.playLists.Remove(track);
            await _context.SaveChangesAsync();

            return View("Index",LoadIndex());
        }
        public async Task<IActionResult> Player(string scr, int trackid,int playlistid)
        {
            //var result = "<source src = \"";
            //result += scr;
            //result += "\" type = \"audio/mpeg\">";
            Helper.player = scr;

            await _helper.IncrementListen(trackid);

            await _helper.updateResentlyPlayed(trackid);

            return View("Edit", LoadEditor(playlistid));
        }
    }
}
