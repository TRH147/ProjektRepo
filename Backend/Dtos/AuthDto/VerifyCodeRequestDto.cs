using System.ComponentModel.DataAnnotations;

namespace RegisztracioTest.Dtos.AuthDto;

public class VerifyCodeRequestDto
{
    [Required(ErrorMessage = "Code is required")]
    public string Code { get; set; } = string.Empty;
}