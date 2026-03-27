using Microsoft.EntityFrameworkCore;
using RegisztracioTest.Data;
using RegisztracioTest.Model;
using RegisztracioTest.Services.IServices;

namespace RegisztracioTest.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly RegistrationDbContext _context;

        public PasswordResetService(
            IUserService userService,
            IEmailService emailService,
            RegistrationDbContext context)
        {
            _userService = userService;
            _emailService = emailService;
            _context = context;
        }

        public async Task SendResetCodeAsync(string email)
        {
            var userExists = !await _userService.CheckEmailAvailability(email);
            if (!userExists)
                throw new InvalidOperationException("Ez az email cím nem található.");

            var code = new Random().Next(100000, 999999).ToString();
            var expiration = DateTime.UtcNow.AddMinutes(5);

            var resetCode = new PasswordResetCode
            {
                Email = email,
                Code = code,
                Expiration = expiration
            };

            _context.PasswordResetCodes.Add(resetCode);
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(email, "Jelszó visszaállítási kód",
                $"A jelszó visszaállítási kódod: {code}");
        }

        public async Task<bool> ResetPasswordAsync(string code, string newPassword)
        {
            var resetEntry = await _context.PasswordResetCodes
                .FirstOrDefaultAsync(r => r.Code == code && r.Expiration > DateTime.UtcNow);

            if (resetEntry == null)
                return false;

            var email = resetEntry.Email;

            var userExists = !await _userService.CheckEmailAvailability(email);
            if (!userExists)
                return false;

            var users = await _userService.SearchUsers(email, 1, 1);
            var user = users.FirstOrDefault(u => u.Email == email);

            if (user == null)
                return false;

            var userDetails = await _context.UserDetails
                .FirstOrDefaultAsync(u => u.UserId == user.Id);

            if (userDetails == null)
                return false;

            userDetails.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            _context.UserDetails.Update(userDetails);

            _context.PasswordResetCodes.Remove(resetEntry);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}