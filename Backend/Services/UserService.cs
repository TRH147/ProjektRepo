using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RegisztracioTest.Data;
using RegisztracioTest.Dtos.AuthDto;
using RegisztracioTest.Dtos.FriendRequestDto;
using RegisztracioTest.Dtos.UploadProfileImageDto;
using RegisztracioTest.Dtos.UserDto;
using RegisztracioTest.Model;
using RegisztracioTest.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace RegisztracioTest.Services
{
    public class UserService : IUserService
    {
        private readonly RegistrationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly IMemoryCache _cache;
        private readonly IServiceScopeFactory _scopeFactory;
        private static readonly ConcurrentDictionary<string, bool> _usernameCache = new();
        private static readonly ConcurrentDictionary<string, bool> _emailCache = new();
        private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

        public UserService(
            RegistrationDbContext context,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            IMemoryCache cache,
            IServiceScopeFactory scopeFactory)
        {
            _context = context;
            _configuration = configuration;
            _environment = environment;
            _cache = cache;
            _scopeFactory = scopeFactory;
        }

        public async Task<UserReadDto> Register(UserCreateDto request)
        {
            if (await _context.Users.AsNoTracking().AnyAsync(u => u.Username == request.Username))
                throw new Exception("Ez a felhasználónév már foglalt.");

            if (await _context.UserDetails.AsNoTracking().AnyAsync(ud => ud.Email == request.Email))
                throw new Exception("Ez az email cím már regisztrálva van.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                LastActive = DateTime.UtcNow,
                ProfileImages = null
            };

            var userDetails = new UserDetails
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow,
                User = user
            };

            var userStats = new UserStats
            {
                Kills = 0,
                Death = 0,
                Score = 0,
                User = user
            };

            _context.Users.Add(user);
            _context.UserDetails.Add(userDetails);
            _context.UserStats.Add(userStats);

            await _context.SaveChangesAsync();

            _usernameCache.TryAdd(request.Username, false);
            _emailCache.TryAdd(request.Email, false);

            return new UserReadDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = userDetails.Email,
                ProfileImages = user.ProfileImages,
                CreatedAt = userDetails.CreatedAt,
                LastActive = user.LastActive
            };
        }

        public async Task<AuthResponseDto> Login(UserLoginDto request)
        {
            var user = await _context.Users
                .Include(u => u.Credential)
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user?.Credential == null)
                throw new Exception("Helytelen felhasználónév vagy jelszó.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Credential.PasswordHash))
                throw new Exception("Helytelen felhasználónév vagy jelszó.");

            user.LastActive = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = CreateToken(user, user.Credential.Email);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Credential.Email,
                Token = token
            };
        }

        public async Task<UserReadDto> GetUserProfile(int userId)
        {
            var cacheKey = $"user_profile_{userId}";

            if (_cache.TryGetValue(cacheKey, out UserReadDto cachedProfile))
            {
                _ = UpdateLastActiveAsync(userId);
                return cachedProfile;
            }

            var user = await _context.Users
                .Include(u => u.Credential)
                .Include(u => u.UserStats)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("A felhasználó nem található.");

            var profile = new UserReadDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Credential?.Email ?? string.Empty,
                ProfileImages = user.ProfileImages,
                CreatedAt = user.Credential?.CreatedAt ?? DateTime.MinValue,
                LastActive = user.LastActive
            };

            _cache.Set(cacheKey, profile, TimeSpan.FromSeconds(30));

            user.LastActive = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return profile;
        }

        public async Task<UserReadDto> GetUserProfileByUsername(string username)
        {
            var cacheKey = $"user_profile_username_{username}";

            if (_cache.TryGetValue(cacheKey, out UserReadDto cachedProfile))
            {
                return cachedProfile;
            }

            var user = await _context.Users
                .Include(u => u.Credential)
                .Include(u => u.UserStats)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                throw new Exception("A felhasználó nem található.");

            var profile = new UserReadDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Credential?.Email ?? string.Empty,
                ProfileImages = user.ProfileImages,
                CreatedAt = user.Credential?.CreatedAt ?? DateTime.MinValue,
                LastActive = user.LastActive
            };

            _cache.Set(cacheKey, profile, TimeSpan.FromSeconds(30));

            user.LastActive = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return profile;
        }

        public async Task<bool> UpdateUserProfile(int userId, string profileImageUrl)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.ProfileImages = profileImageUrl;
            user.LastActive = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _cache.Remove($"user_profile_{userId}");

            return true;
        }

        public async Task<string> UploadProfileImage(int userId, UploadProfileImageDto request)
        {
            if (request?.File == null || request.File.Length == 0)
                throw new Exception("No file uploaded");

            if (request.File.Length > 5 * 1024 * 1024)
                throw new Exception("File too large. Max size is 5MB");

            var fileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
            if (fileExtension != ".jpg" && fileExtension != ".jpeg" &&
                fileExtension != ".png" && fileExtension != ".gif" &&
                fileExtension != ".webp")
            {
                throw new Exception("Invalid file type. Allowed: jpg, jpeg, png, gif, webp");
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "profile-images");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{userId}_{Guid.NewGuid():N}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }

            var relativePath = $"/uploads/profile-images/{fileName}";

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.ProfileImages = relativePath;
                user.LastActive = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            _cache.Remove($"user_profile_{userId}");

            return relativePath;
        }

        public async Task<bool> ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = await _context.Users
                .Include(u => u.Credential)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user?.Credential == null)
                throw new Exception("User not found");

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Credential.PasswordHash))
                throw new Exception("Current password is incorrect");

            user.Credential.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.LastActive = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _cache.Remove($"user_profile_{userId}");

            return true;
        }

        public async Task<bool> UpdateEmail(int userId, string newEmail)
        {
            if (string.IsNullOrWhiteSpace(newEmail) || !newEmail.Contains('@'))
                throw new Exception("Invalid email format");

            var user = await _context.Users
                .Include(u => u.Credential)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user?.Credential == null)
                throw new Exception("User not found");

            var emailExists = await _context.UserDetails
                .AnyAsync(ud => ud.Email == newEmail && ud.Id != user.Credential.Id);

            if (emailExists)
                throw new Exception("Email already exists");

            user.Credential.Email = newEmail;
            user.LastActive = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _cache.Remove($"user_profile_{userId}");
            _emailCache.TryAdd(newEmail, false);

            return true;
        }

        public async Task<bool> UpdateUsername(int userId, string currentUsername, string newUsername)
        {
            if (string.IsNullOrWhiteSpace(newUsername))
                throw new Exception("Username cannot be empty");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId && u.Username == currentUsername);

            if (user == null)
                throw new Exception("Current username does not match or user not found");

            var usernameExists = await _context.Users
                .AnyAsync(u => u.Username == newUsername && u.Id != userId);

            if (usernameExists)
                throw new Exception("Username already exists");

            user.Username = newUsername;
            user.LastActive = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _cache.Remove($"user_profile_{userId}");
            _cache.Remove($"user_profile_username_{currentUsername}");
            _usernameCache.TryAdd(newUsername, false);

            return true;
        }

        public async Task<IEnumerable<UserReadDto>> SearchUsers(string searchTerm, int page, int pageSize)
        {
            var query = _context.Users
                .Include(u => u.Credential)
                .Where(u => string.IsNullOrEmpty(searchTerm) ||
                           u.Username.Contains(searchTerm) ||
                           (u.Credential != null && u.Credential.Email.Contains(searchTerm)))
                .OrderBy(u => u.Username);

            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserReadDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Credential != null ? u.Credential.Email : string.Empty,
                    ProfileImages = u.ProfileImages,
                    CreatedAt = u.Credential != null ? u.Credential.CreatedAt : DateTime.MinValue,
                    LastActive = u.LastActive
                })
                .ToListAsync();

            return users;
        }

        public async Task<IEnumerable<UserReadDto>> GetAllUsersAsync()
        {
            const string cacheKey = "all_users";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<UserReadDto> cachedUsers))
            {
                return cachedUsers;
            }

            var users = await _context.Users
                .Include(u => u.Credential)
                .OrderBy(u => u.Username)
                .Select(u => new UserReadDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Credential != null ? u.Credential.Email : string.Empty,
                    ProfileImages = u.ProfileImages,
                    CreatedAt = u.Credential != null ? u.Credential.CreatedAt : DateTime.MinValue,
                    LastActive = u.LastActive
                })
                .ToListAsync();

            _cache.Set(cacheKey, users, TimeSpan.FromSeconds(30));

            return users;
        }

        public async Task<bool> SendFriendRequest(int senderId, int receiverId)
        {
            if (senderId == receiverId)
                throw new Exception("Cannot send friend request to yourself");

            var existingRequest = await _context.FriendRequests
                .Where(fr =>
                    (fr.SenderId == senderId && fr.ReceiverId == receiverId) ||
                    (fr.SenderId == receiverId && fr.ReceiverId == senderId))
                .FirstOrDefaultAsync();

            if (existingRequest != null)
            {
                if (existingRequest.Status == "Pending")
                    throw new Exception("Friend request already pending");
                if (existingRequest.Status == "Accepted")
                    throw new Exception("Users are already friends");
                if (existingRequest.Status == "Rejected")
                {
                    existingRequest.Status = "Pending";
                    existingRequest.CreatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return true;
                }
            }

            var friendRequest = new FriendRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.FriendRequests.Add(friendRequest);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RespondToFriendRequest(int requestId, int userId, bool accept)
        {
            var friendRequest = await _context.FriendRequests
                .FirstOrDefaultAsync(fr => fr.Id == requestId && fr.ReceiverId == userId);

            if (friendRequest == null)
                throw new Exception("Friend request not found");

            friendRequest.Status = accept ? "Accepted" : "Rejected";
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<FriendRequestDto>> GetFriendRequests(int userId)
        {
            return await _context.FriendRequests
                .Include(fr => fr.Sender)
                .Include(fr => fr.Receiver)
                .Where(fr => fr.ReceiverId == userId && fr.Status == "Pending")
                .Select(fr => new FriendRequestDto
                {
                    Id = fr.Id,
                    SenderId = fr.SenderId,
                    SenderUsername = fr.Sender != null ? fr.Sender.Username : "Unknown",
                    ReceiverId = fr.ReceiverId,
                    ReceiverUsername = fr.Receiver != null ? fr.Receiver.Username : "Unknown",
                    Status = fr.Status,
                    CreatedAt = fr.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<FriendRequestDto>> GetFriends(int userId)
        {
            return await _context.FriendRequests
                .Include(fr => fr.Sender)
                .Include(fr => fr.Receiver)
                .Where(fr => (fr.SenderId == userId || fr.ReceiverId == userId) &&
                             fr.Status == "Accepted")
                .Select(fr => new FriendRequestDto
                {
                    Id = fr.Id,
                    SenderId = fr.SenderId,
                    SenderUsername = fr.Sender != null ? fr.Sender.Username : "Unknown",
                    ReceiverId = fr.ReceiverId,
                    ReceiverUsername = fr.Receiver != null ? fr.Receiver.Username : "Unknown",
                    Status = fr.Status,
                    CreatedAt = fr.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<bool> RemoveFriend(int userId, int friendId)
        {
            var friendship = await _context.FriendRequests
                .FirstOrDefaultAsync(fr =>
                    ((fr.SenderId == userId && fr.ReceiverId == friendId) ||
                     (fr.SenderId == friendId && fr.ReceiverId == userId)) &&
                    fr.Status == "Accepted");

            if (friendship == null)
                throw new Exception("Friendship not found");

            _context.FriendRequests.Remove(friendship);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CancelFriendRequest(int requestId, int userId)
        {
            var friendRequest = await _context.FriendRequests
                .FirstOrDefaultAsync(fr => fr.Id == requestId && fr.SenderId == userId && fr.Status == "Pending");

            if (friendRequest == null)
                throw new Exception("Friend request not found or you are not the sender");

            _context.FriendRequests.Remove(friendRequest);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CheckUsernameAvailability(string username)
        {
            if (_usernameCache.TryGetValue(username, out bool isAvailable))
            {
                return isAvailable;
            }

            var available = !await _context.Users.AnyAsync(u => u.Username == username);
            _usernameCache.TryAdd(username, available);

            _ = Task.Delay(_cacheDuration).ContinueWith(t =>
                _usernameCache.TryRemove(username, out bool _));

            return available;
        }

        public async Task<bool> CheckEmailAvailability(string email)
        {
            if (_emailCache.TryGetValue(email, out bool isAvailable))
            {
                return isAvailable;
            }

            var available = !await _context.UserDetails.AnyAsync(ud => ud.Email == email);
            _emailCache.TryAdd(email, available);

            _ = Task.Delay(_cacheDuration).ContinueWith(t =>
                _emailCache.TryRemove(email, out bool _));

            return available;
        }

        public async Task<IEnumerable<UserReadDto>> GetUsers(int page, int pageSize)
        {
            return await _context.Users
                .Include(u => u.Credential)
                .OrderBy(u => u.Username)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserReadDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Credential != null ? u.Credential.Email : string.Empty,
                    ProfileImages = u.ProfileImages,
                    CreatedAt = u.Credential != null ? u.Credential.CreatedAt : DateTime.MinValue,
                    LastActive = u.LastActive
                })
                .ToListAsync();
        }

        public async Task<UserReadDto> GetUserWithDetailsAsync(int id)
        {
            return await GetUserProfile(id);
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _context.Users
                    .Include(u => u.Credential)
                    .Include(u => u.UserStats)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    return false;

                var userEmail = user.Credential?.Email;
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var resetCodes = await _context.PasswordResetCodes
                        .Where(rc => rc.Email == userEmail)
                        .ToListAsync();
                    _context.PasswordResetCodes.RemoveRange(resetCodes);
                }

                var posts = await _context.Posts
                    .Where(p => p.AuthorId == userId)
                    .ToListAsync();
                _context.Posts.RemoveRange(posts);

                var threads = await _context.ForumThreads
                    .Where(t => t.AuthorId == userId)
                    .ToListAsync();
                _context.ForumThreads.RemoveRange(threads);

                var friendRequests = await _context.FriendRequests
                    .Where(fr => fr.SenderId == userId || fr.ReceiverId == userId)
                    .ToListAsync();
                _context.FriendRequests.RemoveRange(friendRequests);

                if (user.UserStats != null)
                    _context.UserStats.Remove(user.UserStats);

                if (user.Credential != null)
                    _context.UserDetails.Remove(user.Credential);

                _context.Users.Remove(user);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _cache.Remove($"user_profile_{userId}");
                _cache.Remove("all_users");

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task UpdateLastActiveAsync(int userId)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<RegistrationDbContext>();
                await db.Users
                    .Where(u => u.Id == userId)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(u => u.LastActive, DateTime.UtcNow));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to update last active for user {userId}: {ex.Message}");
            }
        }

        private string CreateToken(User user, string email)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? "ThisIsAVeryLongSecretKeyThatIsAtLeast64CharactersLongForHMACSHA512Algorithm1234567890"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}