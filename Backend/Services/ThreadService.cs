using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using RegisztracioTest.Data;
using RegisztracioTest.Dtos.Requests;
using RegisztracioTest.Dtos.Responses;
using RegisztracioTest.Model;
using RegisztracioTest.Services.IServices;

namespace RegisztracioTest.Services
{
    public class ThreadService : IThreadService
    {
        private readonly RegistrationDbContext _context;
        private readonly IServiceScopeFactory _scopeFactory;

        public ThreadService(RegistrationDbContext context, IServiceScopeFactory scopeFactory)
        {
            _context = context;
            _scopeFactory = scopeFactory;
        }

        public async Task<ThreadResponse> CreateThread(ThreadRequest request, int userId)
        {
            try
            {
                var category = await _context.Categories.FindAsync(request.CategoryId);
                if (category == null) throw new Exception("Category not found");

                var user = await _context.Users.FindAsync(userId);
                if (user == null) throw new Exception("User not found");

                var thread = new ForumThread
                {
                    Title = request.Title,
                    Content = request.Content,
                    CategoryId = request.CategoryId,
                    AuthorId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ForumThreads.Add(thread);
                await _context.SaveChangesAsync();

                if (request.TagIds != null && request.TagIds.Any())
                {
                    foreach (var tagId in request.TagIds)
                    {
                        var tag = await _context.Tags.FindAsync(tagId);
                        if (tag != null)
                        {
                            _context.ThreadTags.Add(new ThreadTag
                            {
                                ThreadId = thread.Id,
                                TagId = tagId
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                await UpdateUserLastActive(userId);
                return await GetThread(thread.Id);
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException is MySqlException mysqlEx)
                {
                    Console.WriteLine($"MySQL Error Code: {mysqlEx.ErrorCode}, Number: {mysqlEx.Number}");
                }
                throw new Exception($"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
        }

        public async Task<ThreadResponse> GetThread(int threadId)
        {
            var thread = await _context.ForumThreads
                .Include(t => t.Author)
                .Include(t => t.Category)
                .Include(t => t.ThreadTags)
                    .ThenInclude(tt => tt.Tag)
                .Include(t => t.Posts)
                    .ThenInclude(p => p.Author)
                .FirstOrDefaultAsync(t => t.Id == threadId);

            if (thread == null) throw new Exception("Thread not found");

            var lastPost = thread.Posts.OrderByDescending(p => p.CreatedAt).FirstOrDefault();

            return new ThreadResponse
            {
                Id = thread.Id,
                Title = thread.Title,
                Content = thread.Content,
                CreatedAt = thread.CreatedAt,
                UpdatedAt = thread.UpdatedAt,
                IsLocked = thread.IsLocked,
                IsPinned = thread.IsPinned,
                ViewCount = thread.ViewCount,
                PostCount = thread.Posts.Count,
                Author = new UserResponse
                {
                    Id = thread.Author!.Id,
                    Username = thread.Author.Username,
                    ProfileImages = thread.Author.ProfileImages,
                    LastActive = thread.Author.LastActive
                },
                Category = new CategoryResponse
                {
                    Id = thread.Category!.Id,
                    Name = thread.Category.Name,
                    Description = thread.Category.Description,
                    Icon = thread.Category.Icon,
                    Order = thread.Category.Order
                },
                Tags = thread.ThreadTags.Select(tt => new TagResponse
                {
                    Id = tt.Tag!.Id,
                    Name = tt.Tag.Name,
                    Color = tt.Tag.Color
                }).ToList(),
                LastPost = lastPost != null ? new PostResponse
                {
                    Id = lastPost.Id,
                    Content = lastPost.Content,
                    CreatedAt = lastPost.CreatedAt,
                    Author = new UserResponse
                    {
                        Id = lastPost.Author!.Id,
                        Username = lastPost.Author.Username,
                        ProfileImages = lastPost.Author.ProfileImages
                    }
                } : null
            };
        }

        public async Task<IEnumerable<ThreadResponse>> GetThreads(int categoryId, int page, int pageSize, string sortBy)
        {
            var query = _context.ForumThreads
                .Include(t => t.Author)
                .Include(t => t.Category)
                .Include(t => t.ThreadTags)
                    .ThenInclude(tt => tt.Tag)
                .Include(t => t.Posts)
                .Where(t => categoryId == 0 || t.CategoryId == categoryId);

            query = sortBy switch
            {
                "newest" => query.OrderByDescending(t => t.CreatedAt),
                "active" => query.OrderByDescending(t => t.Posts.Any() ? t.Posts.Max(p => p.CreatedAt) : t.CreatedAt),
                "popular" => query.OrderByDescending(t => t.ViewCount),
                _ => query.OrderByDescending(t => t.IsPinned).ThenByDescending(t => t.CreatedAt)
            };

            var threads = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return threads.Select(t => new ThreadResponse
            {
                Id = t.Id,
                Title = t.Title,
                Content = t.Content,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                IsLocked = t.IsLocked,
                IsPinned = t.IsPinned,
                ViewCount = t.ViewCount,
                PostCount = t.Posts.Count,
                Author = new UserResponse
                {
                    Id = t.Author!.Id,
                    Username = t.Author.Username,
                    ProfileImages = t.Author.ProfileImages,
                    LastActive = t.Author.LastActive
                },
                Category = new CategoryResponse
                {
                    Id = t.Category!.Id,
                    Name = t.Category.Name,
                    Description = t.Category.Description,
                    Icon = t.Category.Icon,
                    Order = t.Category.Order
                },
                Tags = t.ThreadTags.Select(tt => new TagResponse
                {
                    Id = tt.Tag!.Id,
                    Name = tt.Tag.Name,
                    Color = tt.Tag.Color
                }).ToList()
            });
        }

        public async Task<ThreadResponse> UpdateThread(int threadId, ThreadRequest request, int userId)
        {
            var thread = await _context.ForumThreads
                .Include(t => t.Author)
                .FirstOrDefaultAsync(t => t.Id == threadId);

            if (thread == null) throw new Exception("Thread not found");
            if (thread.AuthorId != userId) throw new Exception("Unauthorized");

            thread.Title = request.Title;
            thread.Content = request.Content;
            thread.CategoryId = request.CategoryId;
            thread.UpdatedAt = DateTime.UtcNow;

            var existingTags = _context.ThreadTags.Where(tt => tt.ThreadId == threadId);
            _context.ThreadTags.RemoveRange(existingTags);

            if (request.TagIds != null)
            {
                foreach (var tagId in request.TagIds)
                {
                    _context.ThreadTags.Add(new ThreadTag
                    {
                        ThreadId = threadId,
                        TagId = tagId
                    });
                }
            }

            await _context.SaveChangesAsync();
            await UpdateUserLastActive(userId);

            return await GetThread(threadId);
        }

        public async Task<bool> DeleteThread(int threadId, int userId)
        {
            var thread = await _context.ForumThreads.FindAsync(threadId);
            if (thread == null) return false;
            if (thread.AuthorId != userId) throw new Exception("Unauthorized");

            _context.ForumThreads.Remove(thread);
            await _context.SaveChangesAsync();

            await UpdateUserLastActive(userId);
            return true;
        }

        public async Task IncrementViewCount(int threadId)
        {
            var thread = await _context.ForumThreads.FindAsync(threadId);
            if (thread != null)
            {
                thread.ViewCount++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> TogglePinThread(int threadId, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

            var thread = await _context.ForumThreads.FindAsync(threadId);
            if (thread == null) return false;

            thread.IsPinned = !thread.IsPinned;
            await _context.SaveChangesAsync();
            await UpdateUserLastActive(userId);

            return true;
        }

        public async Task<bool> ToggleLockThread(int threadId, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("User not found");

            var thread = await _context.ForumThreads.FindAsync(threadId);
            if (thread == null) return false;

            thread.IsLocked = !thread.IsLocked;
            await _context.SaveChangesAsync();
            await UpdateUserLastActive(userId);

            return true;
        }

        public async Task<int> GetThreadCount(int categoryId = 0)
        {
            if (categoryId == 0) return await _context.ForumThreads.CountAsync();
            return await _context.ForumThreads.CountAsync(t => t.CategoryId == categoryId);
        }

        public async Task<IEnumerable<ThreadResponse>> SearchThreads(string query, int page, int pageSize)
        {
            var threads = await _context.ForumThreads
                .Include(t => t.Author)
                .Include(t => t.Category)
                .Include(t => t.ThreadTags)
                    .ThenInclude(tt => tt.Tag)
                .Include(t => t.Posts)
                .Where(t => t.Title.Contains(query) || t.Content.Contains(query))
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return threads.Select(t => new ThreadResponse
            {
                Id = t.Id,
                Title = t.Title,
                Content = t.Content,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                IsLocked = t.IsLocked,
                IsPinned = t.IsPinned,
                ViewCount = t.ViewCount,
                PostCount = t.Posts.Count,
                Author = new UserResponse
                {
                    Id = t.Author!.Id,
                    Username = t.Author.Username,
                    ProfileImages = t.Author.ProfileImages,
                    LastActive = t.Author.LastActive
                },
                Category = new CategoryResponse
                {
                    Id = t.Category!.Id,
                    Name = t.Category.Name,
                    Description = t.Category.Description,
                    Icon = t.Category.Icon,
                    Order = t.Category.Order
                },
                Tags = t.ThreadTags.Select(tt => new TagResponse
                {
                    Id = tt.Tag!.Id,
                    Name = tt.Tag.Name,
                    Color = tt.Tag.Color
                }).ToList()
            });
        }

        private async Task UpdateUserLastActive(int userId)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var scopedContext = scope.ServiceProvider.GetRequiredService<RegistrationDbContext>();
                var user = await scopedContext.Users.FindAsync(userId);
                if (user != null)
                {
                    user.LastActive = DateTime.UtcNow;
                    await scopedContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to update user LastActive: {ex.Message}");
            }
        }
    }
}