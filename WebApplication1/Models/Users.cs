
using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "Required User Access Level")]
        public string UserAccessLevel { get; set; }
    }
}
