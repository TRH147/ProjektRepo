using System.Text.Json.Serialization;

namespace RegisztracioTest.Dtos.AuthDto;

public class SendCodeResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    [JsonIgnore]
    public string Code { get; set; } = string.Empty; 
    public int ExpiresIn { get; set; } // másodpercben
}