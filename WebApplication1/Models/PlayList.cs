using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class PlayList
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Picture { get; set; }
    }
}
