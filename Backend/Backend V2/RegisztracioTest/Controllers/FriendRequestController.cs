using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegisztracioTest.Services.IServices;

namespace RegisztracioTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendRequestController : ControllerBase
    {
        private readonly IFriendRequestService _service;

        public FriendRequestController(IFriendRequestService service)
        {
            _service = service;
        }

        [HttpPost("send-request")]
        public async Task<IActionResult> SendRequest(int senderId, int receiverId)
        {
            try
            {
                var result = await _service.SendRequestAsync(senderId, receiverId);
                return Ok(new { success = true, request = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptRequest(int requestId)
        {
            var success = await _service.AcceptRequestAsync(requestId);
            if (!success) return NotFound();
            return Ok(new { success = true });
        }

        [HttpPost("reject")]
        public async Task<IActionResult> RejectRequest(int requestId)
        {
            var success = await _service.RejectRequestAsync(requestId);
            if (!success) return NotFound();
            return Ok(new { success = true });
        }

        [HttpGet("pending/{userId}")]
        public async Task<IActionResult> GetPendingRequests(int userId)
        {
            var list = await _service.GetPendingRequestsAsync(userId);

            if (list == null || !list.Any())
            {
                return Ok(new
                {
                    Success = true,
                    Message = "Nincs megjeleníthető barátfelkérés."
                });
            }

            return Ok(new
            {
                Success = true,
                PendingRequests = list
            });
        }

        [HttpGet("friends/{userId}")]
        public async Task<IActionResult> GetFriends(int userId)
        {
            var list = await _service.GetFriendsAsync(userId);

            if (list == null || !list.Any())
            {
                return Ok(new
                {
                    Success = true,
                    Message = "Nincs még felhasználó a barátlistádon."
                });
            }

            // Csak a barát nevét adjuk vissza
            var friends = list.Select(f =>
                f.SenderId == userId ? f.ReceiverUsername : f.SenderUsername
            ).ToList();

            return Ok(new
            {
                Success = true,
                Friends = friends
            });
        }
    }
}
