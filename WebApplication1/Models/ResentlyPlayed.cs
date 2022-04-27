using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class ResentlyPlayed
    {
        // adding evvery new played track need to optimaze
        [Key]
        public int Id { get; set; }
        [Required]
        public Users User { get; set; }
        [Required]
        public Tracks Track { get; set; }
    }
}
