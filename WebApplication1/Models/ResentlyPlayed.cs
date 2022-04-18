using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class ResentlyPlayed
    {
        // adding evvery new played track need to optimaze
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int TrackId { get; set; }
    }
}
