using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegisztracioTest.Services.IServices;
using System.Security.Claims;

namespace RegisztracioTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class FriendRequestController : ControllerBase
    {
        private readonly IUserService _userService;

        public FriendRequestController(IUserService userService) 
        {
            _userService = userService;
        }

        [HttpPost("send/{receiverId}")]
        public async Task<IActionResult> SendFriendRequest(int receiverId)
        {
            try
            {
                var senderId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var result = await _userService.SendFriendRequest(senderId, receiverId);

                return Ok(new
                {
                    success = true,
                    message = "A barátfelkérés sikeresen elküldve!",
                    friendRequestId = result 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("accept/{requestId}")]
        public async Task<IActionResult> AcceptFriendRequest(int requestId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var success = await _userService.RespondToFriendRequest(requestId, userId, true);

                if (!success)
                    return NotFound(new { success = false, message = "A barátfelkérés nem található." });

                return Ok(new
                {
                    success = true,
                    message = "A barátfelkérés sikeresen elfogadva!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("reject/{requestId}")]
        public async Task<IActionResult> RejectFriendRequest(int requestId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var success = await _userService.RespondToFriendRequest(requestId, userId, false);

                if (!success)
                    return NotFound(new { success = false, message = "A barátfelkérés nem található." });

                return Ok(new
                {
                    success = true,
                    message = "A barátfelkérés sikeresen elutasítva!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRequests()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var requests = await _userService.GetFriendRequests(userId);

                if (requests == null || !requests.Any())
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Nincs megjeleníthető barátfelkérés.",
                        pendingRequests = new List<object>()
                    });
                }

                var pendingRequestsList = new List<object>();

                foreach (var request in requests)
                {
                    var senderUser = await _userService.GetUserProfile(request.SenderId);

                    pendingRequestsList.Add(new
                    {
                        id = request.Id,
                        senderId = request.SenderId,
                        senderUsername = request.SenderUsername,
                        senderProfileImage = senderUser?.ProfileImages,
                        receiverId = request.ReceiverId,
                        receiverUsername = request.ReceiverUsername,
                        status = request.Status,
                        createdAt = request.CreatedAt
                    });
                }

                return Ok(new
                {
                    success = true,
                    pendingRequests = pendingRequestsList
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("friends")]
        public async Task<IActionResult> GetFriends()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var friends = await _userService.GetFriends(userId);

                if (friends == null || !friends.Any())
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Nincs még felhasználó a barátlistádon.",
                        friends = new List<object>()
                    });
                }

                var friendList = new List<object>();

                foreach (var f in friends)
                {
                    var isUserSender = f.SenderId == userId;
                    var friendId = isUserSender ? f.ReceiverId : f.SenderId;

                    var friendUser = await _userService.GetUserProfile(friendId);

                    friendList.Add(new
                    {
                        friendId = friendId,
                        friendUsername = isUserSender ? f.ReceiverUsername : f.SenderUsername,
                        friendProfileImage = friendUser?.ProfileImages,
                        friendshipId = f.Id,
                        createdAt = f.CreatedAt
                    });
                }

                return Ok(new
                {
                    success = true,
                    friends = friendList
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("cancel/{requestId}")]
        public async Task<IActionResult> CancelFriendRequest(int requestId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var success = await _userService.CancelFriendRequest(requestId, userId);

                if (!success)
                    return NotFound(new { success = false, message = "A barátfelkérés nem található." });

                return Ok(new
                {
                    success = true,
                    message = "Barátfelkérés sikeresen visszavonva!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("remove/{friendId}")]
        public async Task<IActionResult> RemoveFriend(int friendId)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var success = await _userService.RemoveFriend(userId, friendId);

                if (!success)
                    return NotFound(new { success = false, message = "Barátság nem található." });

                return Ok(new
                {
                    success = true,
                    message = "Barátság sikeresen törölve!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}