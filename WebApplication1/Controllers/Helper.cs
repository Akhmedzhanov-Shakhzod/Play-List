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
        }   

        public static bool ValidateAdmin(string accesslevel)
        {
            return accesslevel == "ordinary";
        }
        public static Users user { get; set; }
        public static string player { get; set; }
    }
}
