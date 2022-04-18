﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DataLayer;
using WebApplication1.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WebApplication1.Controllers
{
    public class TracksController : Controller
    {
        private readonly DbPlayList _context;

        public TracksController(DbPlayList context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View("AddTrack");
        }
        [HttpPost("FileUpload")]
        public async Task<IActionResult> AddTrack(List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                ////
                ////
                var trackndb = await _context.tracks.FirstOrDefaultAsync(u => u.Audio == "/files/tracks/" + files[0].FileName);
                if(trackndb != null) return View("AddTrack", Helper.Errors.TrackAlreadyExist);
                ////
                ////

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
                track.Artist = Request.Form["Artist"];
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
                var tracks = from u in _context.tracks
                             select u;
                tracks = tracks.OrderByDescending(u => u);

                return View("Views/Home/Index.cshtml",tracks);

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
