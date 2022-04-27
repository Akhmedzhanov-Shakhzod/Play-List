using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Tracks
    {
        [Key]
        public int TrackId { get; set; }
        [Required]
        public string TrackName { get; set; }
        [Required]
        public Artists Artist { get; set; }
        [Required]
        public Genres Genre { get; set; } 
        [Required]
        public int Listens { get; set; }
        [Required]
        public string Audio { get; set; }
        [Required]
        public string Picture { get; set; }
    }
}
