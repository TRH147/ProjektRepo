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

            if (!await _authService.UserExistsAsync(dto.Email))
            {
                return BadRequest(new SendCodeResponseDto
                {
                    Success = false,
                    Message = "Email nincs regisztrálva."
                });
            }

            var code = _codeService.GenerateCode();

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
                await _emailService.SendEmailAsync(dto.Email, "Bejelentkezési kód", $"A kódod: {code}");
            }
            catch (Exception ex)
            {
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


        [HttpPost("verify-code")]
        public async Task<ActionResult> VerifyCode([FromBody] VerifyCodeRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.Code))
            {
                return BadRequest(new { message = "A kód megadása kötelező." });
            }

            var userDto = await _codeService.ValidateCodeAsync(request.Code);
            if (userDto == null)
            {
                return BadRequest(new { message = "Érvénytelen vagy lejárt kód." });
            }

            return Ok(new
            {
                Success = true,
                Message = "Sikeres autentikáció.",
                User = userDto
            });
        }
    }
}
