using Microsoft.EntityFrameworkCore;
using RegisztracioTest.Data;
using RegisztracioTest.Model;
using RegisztracioTest.Repositories.IRepositories;

namespace RegisztracioTest.Repositories
{
    public class Userrepository : IUserRepository
    {
        private readonly RegistrationDbContext _context;

        public Userrepository(RegistrationDbContext context)
        {
            _context = context;
        }

        // -----------------------
        // ID alapján lekérdezés
        // -----------------------
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        // -----------------------
        // E-mail alapján lekérdezés
        // -----------------------
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.UserDetails.AnyAsync(d => d.Email == email);
        }

        // -----------------------
        // Felhasználónév alapján lekérdezés
        // -----------------------
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        // -----------------------
        // Új felhasználó létrehozása
        // -----------------------
        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // -----------------------
        // Felhasználó frissítése
        // -----------------------
        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // -----------------------
        // Felhasználó törlése
        // -----------------------
        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        // -----------------------
        // Minden felhasználó lekérdezése (admin)
        // -----------------------
        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        // -----------------------
        // E-mail már létezik-e
        // -----------------------
        public async Task<User?> GetByEmailAsync(string email)
        {
            var detail = await _context.UserDetails
                                       .Include(d => d.User)
                                       .FirstOrDefaultAsync(d => d.Email == email);

            return detail?.User;
        }

        // -----------------------
        // Felhasználónév már létezik-e
        // -----------------------
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
    }
}
