using RegisztracioTest.Services.IServices;
using Microsoft.EntityFrameworkCore;
using RegisztracioTest.Data;
using RegisztracioTest.Dtos.Requests;
using RegisztracioTest.Dtos.Responses;
using RegisztracioTest.Model;
using Microsoft.Extensions.DependencyInjection;

namespace RegisztracioTest.Services
{
    public class PostService : IPostService
    {
        private readonly RegistrationDbContext _context;
        private readonly IServiceScopeFactory _scopeFactory;

        public PostService(RegistrationDbContext context, IServiceScopeFactory scopeFactory)
        {
            _context = context;
            _scopeFactory = scopeFactory;
        }

        public async Task<PostResponse> CreatePost(int threadId, PostRequest request, int userId)
        {
            var thread = await _context.ForumThreads.FindAsync(threadId);
            if (thread == null)
                throw new Exception("Thread not found");

            if (thread.IsLocked)
                throw new Exception("Thread is locked and cannot be posted to");

            if (request.ParentPostId.HasValue)
            {
                var parentPost = await _context.Posts.FindAsync(request.ParentPostId.Value);
                if (parentPost == null || parentPost.ThreadId != threadId)
                    throw new Exception("Parent post not found or does not belong to this thread");
            }

            var post = new Post
            {
                Content = request.Content,
                ThreadId = threadId,
                AuthorId = userId,
                ParentPostId = request.ParentPostId,
                CreatedAt = DateTime.UtcNow,
                IsPinned = false
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            await UpdateUserLastActive(userId);

            thread.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetPost(post.Id);
        }

        public async Task<PostResponse> GetPost(int postId)
        {
            var post = await _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Replies)
                    .ThenInclude(r => r.Author)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                throw new Exception("Post not found");

            var replies = await GetReplies(post.Id);

            return new PostResponse
            {
                Id = post.Id,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                IsEdited = post.IsEdited,
                VoteScore = post.VoteScore,
                ParentPostId = post.ParentPostId,
                IsPinned = post.IsPinned,
                Author = new UserResponse
                {
                    Id = post.Author!.Id,
                    Username = post.Author.Username,
                    ProfileImages = post.Author.ProfileImages,
                    LastActive = post.Author.LastActive
                },
                Replies = replies
            };
        }

        private async Task<List<PostResponse>> GetReplies(int parentPostId)
        {
            var replies = await _context.Posts
                .Where(p => p.ParentPostId == parentPostId)
                .Include(p => p.Author)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();

            var responseReplies = new List<PostResponse>();

            foreach (var reply in replies)
            {
                var nestedReplies = await GetReplies(reply.Id);

                responseReplies.Add(new PostResponse
                {
                    Id = reply.Id,
                    Content = reply.Content,
                    CreatedAt = reply.CreatedAt,
                    UpdatedAt = reply.UpdatedAt,
                    IsEdited = reply.IsEdited,
                    VoteScore = reply.VoteScore,
                    ParentPostId = reply.ParentPostId,
                    IsPinned = reply.IsPinned,
                    Author = new UserResponse
                    {
                        Id = reply.Author!.Id,
                        Username = reply.Author.Username,
                        ProfileImages = reply.Author.ProfileImages,
                        LastActive = reply.Author.LastActive
                    },
                    Replies = nestedReplies
                });
            }

            return responseReplies;
        }

        public async Task<IEnumerable<PostResponse>> GetPostsByThread(int threadId, int page, int pageSize)
        {
            var posts = await _context.Posts
                .Where(p => p.ThreadId == threadId && p.ParentPostId == null)
                .Include(p => p.Author)
                .Include(p => p.Replies)
                    .ThenInclude(r => r.Author)
                .OrderByDescending(p => p.IsPinned)
                .ThenBy(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var postResponses = new List<PostResponse>();

            foreach (var post in posts)
            {
                var replies = await GetReplies(post.Id);

                postResponses.Add(new PostResponse
                {
                    Id = post.Id,
                    Content = post.Content,
                    CreatedAt = post.CreatedAt,
                    UpdatedAt = post.UpdatedAt,
                    IsEdited = post.IsEdited,
                    VoteScore = post.VoteScore,
                    ParentPostId = post.ParentPostId,
                    IsPinned = post.IsPinned,
                    Author = new UserResponse
                    {
                        Id = post.Author!.Id,
                        Username = post.Author.Username,
                        ProfileImages = post.Author.ProfileImages,
                        LastActive = post.Author.LastActive
                    },
                    Replies = replies
                });
            }

            return postResponses;
        }

        public async Task<PostResponse> UpdatePost(int postId, PostRequest request, int userId)
        {
            var post = await _context.Posts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                throw new Exception("Post not found");

            if (post.AuthorId != userId)
                throw new Exception("Unauthorized");

            post.Content = request.Content;
            post.UpdatedAt = DateTime.UtcNow;
            post.IsEdited = true;

            await _context.SaveChangesAsync();

            await UpdateUserLastActive(userId);

            var thread = await _context.ForumThreads.FindAsync(post.ThreadId);
            if (thread != null)
            {
                thread.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return await GetPost(postId);
        }

        public async Task<bool> DeletePost(int postId, int userId)
        {
            var post = await _context.Posts
                .Include(p => p.Replies)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null) return false;

            if (post.AuthorId != userId)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Username == user.Username);
                    if (admin == null)
                        throw new Exception("Unauthorized");
                }
                else
                {
                    throw new Exception("User not found");
                }
            }

            if (post.Replies.Any())
            {
                post.Content = "[Deleted]";
                post.AuthorId = 0;
            }
            else
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();

            await UpdateUserLastActive(userId);

            return true;
        }

        public async Task<IEnumerable<PostResponse>> SearchPosts(string query, int page, int pageSize)
        {
            var posts = await _context.Posts
                .Where(p => p.Content.Contains(query))
                .Include(p => p.Author)
                .Include(p => p.Thread)
                .OrderByDescending(p => p.IsPinned)
                .ThenByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return posts.Select(post => new PostResponse
            {
                Id = post.Id,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                IsEdited = post.IsEdited,
                VoteScore = post.VoteScore,
                ParentPostId = post.ParentPostId,
                IsPinned = post.IsPinned,
                Author = new UserResponse
                {
                    Id = post.Author!.Id,
                    Username = post.Author.Username,
                    ProfileImages = post.Author.ProfileImages,
                    LastActive = post.Author.LastActive
                }
            });
        }

        public async Task<int> GetPostCount(int threadId)
        {
            return await _context.Posts.CountAsync(p => p.ThreadId == threadId);
        }

        public async Task<IEnumerable<PostResponse>> GetUserPosts(int userId, int page, int pageSize)
        {
            var posts = await _context.Posts
                .Where(p => p.AuthorId == userId)
                .Include(p => p.Author)
                .Include(p => p.Thread)
                .OrderByDescending(p => p.IsPinned)
                .ThenByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return posts.Select(post => new PostResponse
            {
                Id = post.Id,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                IsEdited = post.IsEdited,
                VoteScore = post.VoteScore,
                ParentPostId = post.ParentPostId,
                IsPinned = post.IsPinned,
                Author = new UserResponse
                {
                    Id = post.Author!.Id,
                    Username = post.Author.Username,
                    ProfileImages = post.Author.ProfileImages,
                    LastActive = post.Author.LastActive
                }
            });
        }

        public async Task<bool> PinPost(int postId, int userId)
        {
            if (!await IsUserAdmin(userId))
                throw new Exception("Unauthorized: Only admins can pin posts");

            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
                throw new Exception("Post not found");

            post.IsPinned = true;
            await _context.SaveChangesAsync();

            var thread = await _context.ForumThreads.FindAsync(post.ThreadId);
            if (thread != null)
            {
                thread.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            await UpdateUserLastActive(userId);
            return true;
        }

        public async Task<bool> UnpinPost(int postId, int userId)
        {
            if (!await IsUserAdmin(userId))
                throw new Exception("Unauthorized: Only admins can unpin posts");

            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
                throw new Exception("Post not found");

            post.IsPinned = false;
            await _context.SaveChangesAsync();

            var thread = await _context.ForumThreads.FindAsync(post.ThreadId);
            if (thread != null)
            {
                thread.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            await UpdateUserLastActive(userId);
            return true;
        }

        public async Task<bool> TogglePinPost(int postId, int userId)
        {
            if (!await IsUserAdmin(userId))
                throw new Exception("Unauthorized: Only admins can toggle pin status");

            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
                throw new Exception("Post not found");

            post.IsPinned = !post.IsPinned;
            await _context.SaveChangesAsync();

            var thread = await _context.ForumThreads.FindAsync(post.ThreadId);
            if (thread != null)
            {
                thread.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            await UpdateUserLastActive(userId);
            return true;
        }

        private async Task<bool> IsUserAdmin(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Username == user.Username);
            return admin != null;
        }

        private async Task UpdateUserLastActive(int userId)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var scopedContext = scope.ServiceProvider.GetRequiredService<RegistrationDbContext>();
                    var user = await scopedContext.Users.FindAsync(userId);
                    if (user != null)
                    {
                        user.LastActive = DateTime.UtcNow;
                        await scopedContext.SaveChangesAsync();
                        Console.WriteLine($"Updated LastActive for user ID: {userId}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to update user LastActive: {ex.Message}");
            }
        }
    }
}