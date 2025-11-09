using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegisztracioTest.Model
{
    public class UserDetails
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Kapcsolat a User-hez
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
