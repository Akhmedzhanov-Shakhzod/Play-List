using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.DataLayer
{
    public class DbPlayList: DbContext
    {
        public DbPlayList(DbContextOptions options)
            :base(options)
        {
        }
        public DbSet<Users> users { get; set; } 
        public DbSet<Tracks> tracks { get; set; } 
        public DbSet<SavedTracks> savedTracks { get; set; }
        public DbSet<ResentlyPlayed> resentlyPlayeds { get; set; }
    }
}
