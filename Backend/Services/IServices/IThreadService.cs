using RegisztracioTest.Dtos.Requests;
using RegisztracioTest.Dtos.Responses;

namespace RegisztracioTest.Services.IServices
{
    public interface IThreadService
    {
        Task<ThreadResponse> CreateThread(ThreadRequest request, int userId);
        Task<ThreadResponse> GetThread(int threadId);
        Task<IEnumerable<ThreadResponse>> GetThreads(int categoryId, int page, int pageSize, string sortBy);
        Task<ThreadResponse> UpdateThread(int threadId, ThreadRequest request, int userId);
        Task<bool> DeleteThread(int threadId, int userId);
        Task<bool> TogglePinThread(int threadId, int userId);
        Task<bool> ToggleLockThread(int threadId, int userId);
        Task IncrementViewCount(int threadId);
        Task<int> GetThreadCount(int categoryId = 0);
        Task<IEnumerable<ThreadResponse>> SearchThreads(string query, int page, int pageSize);
    }
}