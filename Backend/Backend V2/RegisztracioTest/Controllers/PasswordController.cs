using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegisztracioTest.Dtos.ForgotPasswordRequestDto;
using RegisztracioTest.Dtos.ResetPasswordDto;
using RegisztracioTest.Services.IServices;

namespace RegisztracioTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly IPasswordResetService _resetService;

        public PasswordController(IPasswordResetService resetService)
        {
            _resetService = resetService;
        }

        [HttpPost("forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
        {
            try
            {
                await _resetService.SendResetCodeAsync(dto.Email);
                return Ok(new { message = "A visszaállítási kód elküldve az email címedre." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var success = await _resetService.ResetPasswordAsync(dto.Email, dto.Code, dto.NewPassword);
            if (!success)
                return BadRequest(new { message = "Érvénytelen kód vagy email." });

            return Ok(new { message = "A jelszó sikeresen megváltoztatva." });
        }
    }
}
