using System.ComponentModel.DataAnnotations;

namespace Task.Model
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public bool? IsAdmin { get; set; }
        [Required]
        public int? Age { get; set; }
        [Required]
        public string? Hobbies { get; set; }
    }
}
