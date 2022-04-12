
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace WebApplication1.Models
{
    public class Users
    {
        [Key]
        public int UserID { get; set; }
        [Required(ErrorMessage = "Required Username")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Required Password")]
        public string Password { get; set; }
    }

    public class EFPlayList : DbContext
    {
        public DbSet<Users> users { get; set; }

    }
}
