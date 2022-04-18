using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class SavedTracks
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int TrackId { get; set; }
    }
}
