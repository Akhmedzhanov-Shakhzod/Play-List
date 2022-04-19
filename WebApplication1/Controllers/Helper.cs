using WebApplication1.DataLayer;
using WebApplication1.Models;



namespace WebApplication1.Controllers
{
    public class Helper
    {
        public static class Errors
        {
            public const string UserNotExist = "This user does not exist";
            public const string PassworInCorrect = "Incorrect password";
            public static string TrackAlreadyExist = "This track already exists";
            public static string PlayListAlreadyExist = "This playlist already exists";
        }   

        public static bool ValidateAdmin(string accesslevel)
        {
            return accesslevel == "ordinary";
        }
        public static Users user { get; set; }
        public static IQueryable<PlayList>? playLists { get; set; }
        public static string player { get; set; }
        public static int countUserPlaylist { get; set; }
        public static bool isPlaylistExist { get; set; } = false;
        public static bool isEditMode { get; set; }


        private readonly DbPlayList _context;

        public Helper(DbPlayList context)
        {
            _context = context;
        }
        public IQueryable<PlayList>? PlayLists()
        {
            try
            {
                return from p in _context.playLists
                       where p.Author == Helper.user.UserName
                       select p;
            }
            catch
            {
                return null;
            }
        }

    }
}
