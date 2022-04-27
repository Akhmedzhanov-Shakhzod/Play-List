using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class TracksInPlayList
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public PlayList PlayList { get; set; }
        [Required]
        public Tracks Track { get; set; }
    }
}
