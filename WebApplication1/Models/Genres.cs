using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Genres
    {
        [Key]
        public int GenreId { get; set; }
        [Required]
        public string GenreName { get; set; }

    }
}
