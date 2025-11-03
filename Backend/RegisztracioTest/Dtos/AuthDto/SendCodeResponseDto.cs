namespace RegisztracioTest.Dtos.AuthDto;

public class SendCodeResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // FIGYELEM: Éles környezetben töröld!
    public int ExpiresIn { get; set; } // másodpercben
}