using System.ComponentModel.DataAnnotations;

namespace RegisztracioTest.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? ProfileImages { get; set; }

        public UserStats UserStats { get; set; }

        // Navigáció a hitelesítéshez
        public UserDetails Credential { get; set; }
    }
}
