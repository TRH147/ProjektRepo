using System.ComponentModel.DataAnnotations;

namespace RegisztracioTest.Dtos.AuthDto;

public class SendCodeRequestDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;
}