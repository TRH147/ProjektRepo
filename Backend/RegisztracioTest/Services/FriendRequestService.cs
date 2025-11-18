using RegisztracioTest.Dtos.FriendRequestDto;
using RegisztracioTest.Model;
using RegisztracioTest.Repositories.IRepositories;
using RegisztracioTest.Services.IServices;

namespace RegisztracioTest.Services
{
    public class FriendRequestService : IFriendRequestService
    {
        private readonly IFriendRequestRepository _repo;
        private readonly IUserRepository _userRepository;

        public FriendRequestService(IFriendRequestRepository repo, IUserRepository userRepository)
        {
            _repo = repo;
            _userRepository = userRepository;
        }

        // -------------------------------
        // Barátfelkérés küldése
        // -------------------------------
        public async Task<FriendRequestDto> SendRequestAsync(int senderId, int receiverId)
        {
            // Ellenőrzés, hogy már vannak-e barátok
            var existingFriend = await _repo.GetFriendsAsync(senderId);
            if (existingFriend.Any(f => (f.SenderId == senderId && f.ReceiverId == receiverId) ||
                                        (f.SenderId == receiverId && f.ReceiverId == senderId)))
            {
                throw new InvalidOperationException("Már barátok egymással.");
            }

            // Ellenőrzés, hogy már van-e függőben lévő kérés
            var pendingRequest = await _repo.GetPendingRequestsAsync(senderId);
            if (pendingRequest.Any(f => (f.SenderId == senderId && f.ReceiverId == receiverId) ||
                                        (f.SenderId == receiverId && f.ReceiverId == senderId)))
            {
                throw new InvalidOperationException("Már van egy függőben lévő barátfelkérés erre a felhasználóra.");
            }

            var request = new FriendRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            await _repo.CreateAsync(request);

            var sender = await _userRepository.GetByIdAsync(senderId);
            var receiver = await _userRepository.GetByIdAsync(receiverId);

            return new FriendRequestDto
            {
                Id = request.Id,
                SenderId = senderId,
                SenderUsername = sender?.Username ?? "Ismeretlen",
                ReceiverId = receiverId,
                ReceiverUsername = receiver?.Username ?? "Ismeretlen",
                Status = request.Status,
                CreatedAt = request.CreatedAt
            };
        }

        // -------------------------------
        // Barátfelkérés elfogadása
        // -------------------------------
        public async Task<bool> AcceptRequestAsync(int requestId)
        {
            var request = await _repo.GetByIdAsync(requestId);
            if (request == null) return false;

            request.Status = "Accepted";
            await _repo.UpdateAsync(request);
            return true;
        }

        // -------------------------------
        // Barátfelkérés elutasítása
        // -------------------------------
        public async Task<bool> RejectRequestAsync(int requestId)
        {
            var request = await _repo.GetByIdAsync(requestId);
            if (request == null) return false;

            request.Status = "Rejected";
            await _repo.UpdateAsync(request);
            return true;
        }

        // -------------------------------
        // Függőben lévő kérések lekérése
        // -------------------------------
        public async Task<List<FriendRequestDto>> GetPendingRequestsAsync(int userId)
        {
            var requests = await _repo.GetPendingRequestsAsync(userId);

            var result = new List<FriendRequestDto>();
            foreach (var r in requests)
            {
                var sender = await _userRepository.GetByIdAsync(r.SenderId);
                var receiver = await _userRepository.GetByIdAsync(r.ReceiverId);

                result.Add(new FriendRequestDto
                {
                    Id = r.Id,
                    SenderId = r.SenderId,
                    SenderUsername = sender?.Username ?? "Ismeretlen",
                    ReceiverId = r.ReceiverId,
                    ReceiverUsername = receiver?.Username ?? "Ismeretlen",
                    Status = r.Status,
                    CreatedAt = r.CreatedAt
                });
            }

            return result;
        }

        // -------------------------------
        // Elfogadott barátok lekérése
        // -------------------------------
        public async Task<List<FriendRequestDto>> GetFriendsAsync(int userId)
        {
            var friends = await _repo.GetFriendsAsync(userId);

            var result = new List<FriendRequestDto>();
            foreach (var f in friends)
            {
                var sender = await _userRepository.GetByIdAsync(f.SenderId);
                var receiver = await _userRepository.GetByIdAsync(f.ReceiverId);

                result.Add(new FriendRequestDto
                {
                    Id = f.Id,
                    SenderId = f.SenderId,
                    SenderUsername = sender?.Username ?? "Ismeretlen",
                    ReceiverId = f.ReceiverId,
                    ReceiverUsername = receiver?.Username ?? "Ismeretlen",
                    Status = f.Status,
                    CreatedAt = f.CreatedAt
                });
            }

            return result;
        }
    }
}
