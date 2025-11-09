using Microsoft.AspNetCore.Mvc;
using RegisztracioTest.Dtos.AuthDto;
using RegisztracioTest.Model;
using RegisztracioTest.Services.IServices;

namespace RegisztracioTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginCodeController : ControllerBase
    {
        private readonly ICodeService _codeService;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public LoginCodeController(
            ICodeService codeService,
            IAuthService authService,
            IEmailService emailService)
        {
            _codeService = codeService ?? throw new ArgumentNullException(nameof(codeService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        // -------------------------------
        // KÓD KÜLDÉSE EMAILRE
        // -------------------------------
        [HttpPost("send-code")]
        public async Task<ActionResult<SendCodeResponseDto>> SendCode([FromBody] SendCodeRequestDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email))
            {
                return BadRequest(new SendCodeResponseDto
                {
                    Success = false,
                    Message = "Email cím megadása kötelező."
                });
            }

            // Ellenőrizzük, hogy létezik-e a felhasználó
            if (!await _authService.UserExistsAsync(dto.Email))
            {
                return BadRequest(new SendCodeResponseDto
                {
                    Success = false,
                    Message = "Email nincs regisztrálva."
                });
            }

            // Kód generálása
            var code = _codeService.GenerateCode();

            // Kód tárolása
            var stored = await _codeService.StoreCodeAsync(dto.Email, code);
            if (!stored)
            {
                return StatusCode(500, new SendCodeResponseDto
                {
                    Success = false,
                    Message = "Hiba történt a kód mentése során."
                });
            }

            try
            {
                // --- Email küldés ---
                await _emailService.SendEmailAsync(dto.Email, "Bejelentkezési kód", $"A kódod: {code}");
            }
            catch (Exception ex)
            {
                // Email küldési hiba logolása, de a kód mentve marad
                return StatusCode(500, new SendCodeResponseDto
                {
                    Success = false,
                    Message = $"A kód generálása sikeres, de az email küldés nem: {ex.Message}"
                });
            }

            return Ok(new SendCodeResponseDto
            {
                Success = true,
                Message = "Kód sikeresen generálva és elküldve emailben.",
                ExpiresIn = 60
            });
        }

        // -------------------------------
        // KÓD ELLENŐRZÉSE
        // -------------------------------
        [HttpPost("verify-code")]
        public async Task<ActionResult> VerifyCode([FromBody] string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest(new { message = "A kód megadása kötelező." });
            }

            // Ellenőrzés, visszaadjuk a felhasználó DTO-t
            var userDto = await _codeService.ValidateCodeAsync(code);
            if (userDto == null)
            {
                return BadRequest(new { message = "Érvénytelen vagy lejárt kód." });
            }

            // JWT NINCS, csak jelzés a sikeres ellenőrzésről
            return Ok(new
            {
                Success = true,
                Message = "Sikeres autentikáció."
            });
        }
    }
}
