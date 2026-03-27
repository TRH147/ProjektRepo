using RegisztracioTest.Dtos.Requests;
using RegisztracioTest.Dtos.Responses;

namespace RegisztracioTest.Services.IServices
{
    public interface IPostService
    {
        Task<PostResponse> CreatePost(int threadId, PostRequest request, int userId);
        Task<PostResponse> GetPost(int postId);
        Task<IEnumerable<PostResponse>> GetPostsByThread(int threadId, int page, int pageSize);
        Task<PostResponse> UpdatePost(int postId, PostRequest request, int userId);
        Task<bool> DeletePost(int postId, int userId);
        Task<IEnumerable<PostResponse>> SearchPosts(string query, int page, int pageSize);

        Task<bool> PinPost(int postId, int userId);
        Task<bool> UnpinPost(int postId, int userId);
        Task<bool> TogglePinPost(int postId, int userId);

        Task<int> GetPostCount(int threadId);
        Task<IEnumerable<PostResponse>> GetUserPosts(int userId, int page, int pageSize);
    }
}