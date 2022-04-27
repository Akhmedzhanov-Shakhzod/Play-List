using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class SavedTracks
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public Users User { get; set; }
        [Required]
        public Tracks Track { get; set; }
    }
}
