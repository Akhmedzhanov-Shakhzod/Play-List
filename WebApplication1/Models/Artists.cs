using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Artists
    {
        [Key]
        public int ArtistId { get; set; }
        [Required]
        public string ArtistName { get; set; }

        public Artists(string artistName)
        {
            ArtistName = artistName;
        }
    }
}
