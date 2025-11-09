using Microsoft.EntityFrameworkCore;
using RegisztracioTest.Data;
using RegisztracioTest.Repositories.IRepositories;
using RegisztracioTest.Services.IServices;

namespace RegisztracioTest.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IPasswordResetRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly RegistrationDbContext _context;

        public PasswordResetService(IPasswordResetRepository repository, IUserRepository userRepository, IEmailService emailService, RegistrationDbContext context)
        {
            _repository = repository;
            _userRepository = userRepository;
            _emailService = emailService;
            _context = context;
        }

        public async Task SendResetCodeAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new InvalidOperationException("Ez az email cím nem található.");

            // Generálunk egy 6 jegyű kódot
            var code = new Random().Next(100000, 999999).ToString();
            var expiration = DateTime.Now.AddMinutes(5);

            await _repository.CreateCodeAsync(email, code, expiration);

            // Küldés emailben
            await _emailService.SendEmailAsync(email, "Jelszó visszaállítási kód", $"A jelszó visszaállítási kódod: {code}");
        }

        public async Task<bool> ResetPasswordAsync(string email, string code, string newPassword)
        {
            // Validáljuk a kódot
            var valid = await _repository.ValidateCodeAsync(email, code);
            if (!valid) return false;

            // Lekérjük a user-t
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return false;

            // Lekérjük a UserDetails-t, ahol a jelszó van tárolva
            var userDetails = await _context.UserDetails.FirstOrDefaultAsync(u => u.UserId == user.Id);
            if (userDetails == null) return false;

            // Frissítjük a jelszót (hash)
            userDetails.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            // Mentés
            _context.UserDetails.Update(userDetails);
            await _context.SaveChangesAsync();

            // Töröljük a reset kódot
            await _repository.DeleteCodeAsync(email, code);

            return true;
        }
    }
}
