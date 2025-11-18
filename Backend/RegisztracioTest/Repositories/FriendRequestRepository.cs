using RegisztracioTest.Data;
using RegisztracioTest.Model;
using RegisztracioTest.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace RegisztracioTest.Repositories
{
    public class FriendRequestRepository : IFriendRequestRepository
    {
        private readonly RegistrationDbContext _context;

        public FriendRequestRepository(RegistrationDbContext context)
        {
            _context = context;
        }

        public async Task<FriendRequest> CreateAsync(FriendRequest request)
        {
            await _context.FriendRequests.AddAsync(request);
            await _context.SaveChangesAsync();
            return request; // Vissza kell adni a létrehozott entitást
        }

        public async Task<FriendRequest> UpdateAsync(FriendRequest request)
        {
            _context.FriendRequests.Update(request);
            await _context.SaveChangesAsync();
            return request; // Vissza kell adni a frissített entitást
        }

        public async Task DeleteAsync(FriendRequest request)
        {
            _context.FriendRequests.Remove(request);
            await _context.SaveChangesAsync();
        }

        public async Task<FriendRequest?> GetByIdAsync(int id)
        {
            return await _context.FriendRequests.FindAsync(id);
        }

        public async Task<List<FriendRequest>> GetPendingRequestsAsync(int userId)
        {
            return await _context.FriendRequests
                                 .Where(f => f.ReceiverId == userId && f.Status == "Pending")
                                 .ToListAsync();
        }

        public async Task<List<FriendRequest>> GetFriendsAsync(int userId)
        {
            return await _context.FriendRequests
                                 .Where(f => (f.ReceiverId == userId || f.SenderId == userId) && f.Status == "Accepted")
                                 .ToListAsync();
        }
    }
}