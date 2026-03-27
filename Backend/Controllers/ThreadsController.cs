using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegisztracioTest.Dtos.Requests;
using RegisztracioTest.Dtos.Responses;
using RegisztracioTest.Services.IServices;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using RegisztracioTest.Model;
using RegisztracioTest.Data;

namespace RegisztracioTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThreadsController : ControllerBase
    {
        private readonly IThreadService _threadService;
        private readonly RegistrationDbContext _context;

        public ThreadsController(IThreadService threadService, RegistrationDbContext context)
        {
            _threadService = threadService;
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<Dtos.Responses.PaginationResponse<ThreadResponse>>> GetThreads(
            [FromQuery] int categoryId = 0,
            [FromQuery] string tag = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5,
            [FromQuery] string sortBy = "newest")
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 50) pageSize = 5;

                var query = _context.ForumThreads.AsQueryable();

                if (categoryId > 0)
                {
                    query = query.Where(t => t.CategoryId == categoryId);
                }

                if (!string.IsNullOrEmpty(tag))
                {
                    query = query.Where(t => t.ThreadTags.Any(tt => tt.Tag.Name == tag));
                    Console.WriteLine($"Filtering by tag: {tag}");
                }

                var totalCount = await query.CountAsync();
                Console.WriteLine($"Total threads matching filters: {totalCount}");

                query = sortBy.ToLower() switch
                {
                    "newest" => query.OrderByDescending(t => t.CreatedAt),
                    "oldest" => query.OrderBy(t => t.CreatedAt),
                    "mostreplies" => query.OrderByDescending(t => t.Posts.Count),
                    "mostviews" => query.OrderByDescending(t => t.ViewCount),
                    _ => query.OrderByDescending(t => t.CreatedAt)
                };

                var threads = await query
                    .Include(t => t.Author)
                    .Include(t => t.Category)
                    .Include(t => t.ThreadTags)
                        .ThenInclude(tt => tt.Tag)
                    .Include(t => t.Posts)
                        .ThenInclude(p => p.Author)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                Console.WriteLine($"Returning {threads.Count} threads for page {page}");

                var threadResponses = threads.Select(t => new ThreadResponse
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
                        Id = t.Author.Id,
                        Username = t.Author.Username,
                        ProfileImages = t.Author.ProfileImages,
                        LastActive = t.Author.LastActive
                    },
                    Category = new CategoryResponse
                    {
                        Id = t.Category.Id,
                        Name = t.Category.Name,
                        Description = t.Category.Description,
                        Icon = t.Category.Icon,
                        Order = t.Category.Order,
                        ThreadCount = t.Category.Threads.Count,
                        CreatedAt = t.Category.CreatedAt
                    },
                    Tags = t.ThreadTags.Select(tt => new TagResponse
                    {
                        Id = tt.Tag.Id,
                        Name = tt.Tag.Name,
                        Color = tt.Tag.Color
                    }).ToList(),
                    LastPost = t.Posts
                        .OrderByDescending(p => p.CreatedAt)
                        .Select(p => new PostResponse
                        {
                            Id = p.Id,
                            Content = p.Content,
                            CreatedAt = p.CreatedAt,
                            Author = new UserResponse
                            {
                                Id = p.Author.Id,
                                Username = p.Author.Username,
                                ProfileImages = p.Author.ProfileImages
                            }
                        })
                        .FirstOrDefault()
                }).ToList();

                var response = new Dtos.Responses.PaginationResponse<ThreadResponse>(
                    threadResponses,
                    page,
                    pageSize,
                    totalCount
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting threads: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "An error occurred while fetching threads" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetThread(int id)
        {
            try
            {
                var thread = await _context.ForumThreads
                    .Include(t => t.Author)
                    .Include(t => t.Category)
                    .Include(t => t.ThreadTags)
                        .ThenInclude(tt => tt.Tag)
                    .Include(t => t.Posts)
                        .ThenInclude(p => p.Author)
                        .ThenInclude(a => a.UserStats)
                    .Include(t => t.Posts)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (thread == null)
                {
                    return NotFound(new { message = "Thread not found" });
                }

                thread.ViewCount++;
                await _context.SaveChangesAsync();

                var response = new ThreadResponse
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
                        Id = thread.Author.Id,
                        Username = thread.Author.Username,
                        ProfileImages = thread.Author.ProfileImages,
                        LastActive = thread.Author.LastActive,
                        PostCount = await _context.Posts.CountAsync(p => p.AuthorId == thread.AuthorId),
                        ThreadCount = await _context.ForumThreads.CountAsync(t => t.AuthorId == thread.AuthorId)
                    },
                    Category = new CategoryResponse
                    {
                        Id = thread.Category.Id,
                        Name = thread.Category.Name,
                        Description = thread.Category.Description,
                        Icon = thread.Category.Icon,
                        Order = thread.Category.Order,
                        ThreadCount = thread.Category.Threads.Count,
                        CreatedAt = thread.Category.CreatedAt
                    },
                    Tags = thread.ThreadTags.Select(tt => new TagResponse
                    {
                        Id = tt.Tag.Id,
                        Name = tt.Tag.Name,
                        Color = tt.Tag.Color
                    }).ToList(),
                    LastPost = thread.Posts
                        .OrderByDescending(p => p.CreatedAt)
                        .Select(p => new PostResponse
                        {
                            Id = p.Id,
                            Content = p.Content,
                            CreatedAt = p.CreatedAt,
                            ParentPostId = p.ParentPostId,
                            Author = new UserResponse
                            {
                                Id = p.Author.Id,
                                Username = p.Author.Username,
                                ProfileImages = p.Author.ProfileImages,
                                LastActive = p.Author.LastActive
                            }
                        })
                        .FirstOrDefault()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting thread: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while fetching the thread" });
            }
        }

        [HttpGet("tags")]
        public async Task<IActionResult> GetTags()
        {
            try
            {
                var tags = await _context.Tags
                    .Select(t => new
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Color = t.Color,
                        ThreadCount = _context.ThreadTags.Count(tt => tt.TagId == t.Id)
                    })
                    .OrderBy(t => t.Name)
                    .ToListAsync();

                return Ok(tags);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching tags: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while fetching tags" });
            }
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _context.Categories
                    .Select(c => new
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        Order = c.Order,
                        Icon = c.Icon,
                        ThreadCount = _context.ForumThreads.Count(t => t.CategoryId == c.Id)
                    })
                    .OrderBy(c => c.Order)
                    .ThenBy(c => c.Name)
                    .ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching categories: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while fetching categories" });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateThread(ThreadRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                Console.WriteLine($"🔍 Controller - User claims:");
                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"  {claim.Type}: {claim.Value}");
                }

                Console.WriteLine($"🔍 Extracted userId from claims: {userIdClaim}");

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    Console.WriteLine($"❌ Cannot parse user ID from claims. Raw value: {userIdClaim}");
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                Console.WriteLine($"🔍 Parsed userId: {userId}");

                var category = await _context.Categories.FindAsync(request.CategoryId);
                if (category == null)
                {
                    return BadRequest(new { message = "Category not found" });
                }

                var author = await _context.Users.FindAsync(userId);
                if (author == null)
                {
                    return BadRequest(new { message = "User not found" });
                }

                var thread = new ForumThread
                {
                    Title = request.Title,
                    Content = request.Content,
                    CategoryId = request.CategoryId,
                    AuthorId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                if (request.TagIds != null && request.TagIds.Any())
                {
                    var tags = await _context.Tags
                        .Where(t => request.TagIds.Contains(t.Id))
                        .ToListAsync();

                    foreach (var tag in tags)
                    {
                        thread.ThreadTags.Add(new ThreadTag
                        {
                            Tag = tag
                        });
                    }
                }

                _context.ForumThreads.Add(thread);
                await _context.SaveChangesAsync();

                await _context.Entry(thread)
                    .Reference(t => t.Author)
                    .LoadAsync();
                await _context.Entry(thread)
                    .Reference(t => t.Category)
                    .LoadAsync();
                await _context.Entry(thread)
                    .Collection(t => t.ThreadTags)
                    .Query()
                    .Include(tt => tt.Tag)
                    .LoadAsync();

                var response = new ThreadResponse
                {
                    Id = thread.Id,
                    Title = thread.Title,
                    Content = thread.Content,
                    CreatedAt = thread.CreatedAt,
                    Author = new UserResponse
                    {
                        Id = thread.Author.Id,
                        Username = thread.Author.Username,
                        ProfileImages = thread.Author.ProfileImages,
                        LastActive = thread.Author.LastActive
                    },
                    Category = new CategoryResponse
                    {
                        Id = thread.Category.Id,
                        Name = thread.Category.Name,
                        Description = thread.Category.Description,
                        Icon = thread.Category.Icon,
                        Order = thread.Category.Order
                    },
                    Tags = thread.ThreadTags.Select(tt => new TagResponse
                    {
                        Id = tt.Tag.Id,
                        Name = tt.Tag.Name,
                        Color = tt.Tag.Color
                    }).ToList()
                };

                return CreatedAtAction(nameof(GetThread), new { id = thread.Id }, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Controller error: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateThread(int id, ThreadRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                var thread = await _context.ForumThreads
                    .Include(t => t.ThreadTags)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (thread == null)
                {
                    return NotFound(new { message = "Thread not found" });
                }

                if (thread.AuthorId != userId && !User.IsInRole("Admin"))
                {
                    return Unauthorized(new { message = "You can only edit your own threads" });
                }

                thread.Title = request.Title;
                thread.Content = request.Content;
                thread.CategoryId = request.CategoryId;
                thread.UpdatedAt = DateTime.UtcNow;

                if (request.TagIds != null)
                {
                    thread.ThreadTags.Clear();

                    var tags = await _context.Tags
                        .Where(t => request.TagIds.Contains(t.Id))
                        .ToListAsync();

                    foreach (var tag in tags)
                    {
                        thread.ThreadTags.Add(new ThreadTag
                        {
                            Tag = tag
                        });
                    }
                }

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating thread: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteThread(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                var thread = await _context.ForumThreads.FindAsync(id);
                if (thread == null)
                {
                    return NotFound(new { message = "Thread not found" });
                }

                if (thread.AuthorId != userId && !User.IsInRole("Admin"))
                {
                    return Unauthorized(new { message = "You can only delete your own threads" });
                }

                _context.ForumThreads.Remove(thread);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting thread: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/pin")]
        public async Task<IActionResult> TogglePinThread(int id)
        {
            try
            {
                var thread = await _context.ForumThreads.FindAsync(id);
                if (thread == null)
                {
                    return NotFound(new { message = "Thread not found" });
                }

                thread.IsPinned = !thread.IsPinned;
                await _context.SaveChangesAsync();

                return Ok(new { pinned = thread.IsPinned });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error toggling pin: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/lock")]
        public async Task<IActionResult> ToggleLockThread(int id)
        {
            try
            {
                var thread = await _context.ForumThreads.FindAsync(id);
                if (thread == null)
                {
                    return NotFound(new { message = "Thread not found" });
                }

                thread.IsLocked = !thread.IsLocked;
                await _context.SaveChangesAsync();

                return Ok(new { locked = thread.IsLocked });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error toggling lock: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchThreads(
            [FromQuery] string query,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    return BadRequest(new { message = "Search query is required" });

                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 50) pageSize = 20;

                var searchQuery = _context.ForumThreads
                    .Where(t => t.Title.Contains(query) || t.Content.Contains(query))
                    .AsQueryable();

                var totalCount = await searchQuery.CountAsync();

                var threads = await searchQuery
                    .Include(t => t.Author)
                    .Include(t => t.Category)
                    .Include(t => t.ThreadTags)
                        .ThenInclude(tt => tt.Tag)
                    .OrderByDescending(t => t.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var threadResponses = threads.Select(t => new ThreadResponse
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
                        Id = t.Author.Id,
                        Username = t.Author.Username,
                        ProfileImages = t.Author.ProfileImages,
                        LastActive = t.Author.LastActive
                    },
                    Category = new CategoryResponse
                    {
                        Id = t.Category.Id,
                        Name = t.Category.Name,
                        Description = t.Category.Description,
                        Icon = t.Category.Icon,
                        Order = t.Category.Order
                    },
                    Tags = t.ThreadTags.Select(tt => new TagResponse
                    {
                        Id = tt.Tag.Id,
                        Name = tt.Tag.Name,
                        Color = tt.Tag.Color
                    }).ToList()
                }).ToList();

                var response = new Dtos.Responses.PaginationResponse<ThreadResponse>(
        threadResponses,
        page,
        pageSize,
        totalCount
    );

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching threads: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while searching threads" });
            }
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetThreadCount([FromQuery] int categoryId = 0)
        {
            try
            {
                var query = _context.ForumThreads.AsQueryable();

                if (categoryId > 0)
                {
                    query = query.Where(t => t.CategoryId == categoryId);
                }

                var count = await query.CountAsync();
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting thread count: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while getting thread count" });
            }
        }

        [Authorize]
        [HttpPost("{threadId}/posts")]
        public async Task<ActionResult<PostResponse>> CreatePost(int threadId, PostRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                var thread = await _context.ForumThreads
                    .Include(t => t.Author)
                    .FirstOrDefaultAsync(t => t.Id == threadId);

                if (thread == null)
                {
                    return NotFound(new { message = "Thread not found" });
                }

                if (thread.IsLocked)
                {
                    return BadRequest(new { message = "Thread is locked and cannot be posted to" });
                }

                if (request.ParentPostId.HasValue)
                {
                    var parentPost = await _context.Posts
                        .FirstOrDefaultAsync(p => p.Id == request.ParentPostId.Value && p.ThreadId == threadId);

                    if (parentPost == null)
                    {
                        return BadRequest(new { message = "Parent post not found or does not belong to this thread" });
                    }
                }

                var author = await _context.Users.FindAsync(userId);
                if (author == null)
                {
                    return BadRequest(new { message = "User not found" });
                }

                var post = new Post
                {
                    Content = request.Content,
                    ThreadId = threadId,
                    AuthorId = userId,
                    ParentPostId = request.ParentPostId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Posts.Add(post);

                thread.UpdatedAt = DateTime.UtcNow;

                author.LastActive = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await _context.Entry(post)
                    .Reference(p => p.Author)
                    .LoadAsync();

                var response = new PostResponse
                {
                    Id = post.Id,
                    Content = post.Content,
                    CreatedAt = post.CreatedAt,
                    VoteScore = 0,
                    ParentPostId = post.ParentPostId,
                    Author = new UserResponse
                    {
                        Id = post.Author.Id,
                        Username = post.Author.Username,
                        ProfileImages = post.Author.ProfileImages,
                        LastActive = post.Author.LastActive
                    },
                    Replies = new List<PostResponse>()
                };

                return CreatedAtAction(nameof(GetThread), new { id = threadId }, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error creating post: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "An error occurred while creating the post" });
            }
        }

        [HttpGet("{threadId}/posts")]
        public async Task<ActionResult<List<PostResponse>>> GetThreadPosts(int threadId)
        {
            try
            {
                var threadExists = await _context.ForumThreads.AnyAsync(t => t.Id == threadId);
                if (!threadExists)
                {
                    return NotFound(new { message = "Thread not found" });
                }

                var posts = await _context.Posts
                    .Where(p => p.ThreadId == threadId)
                    .Include(p => p.Author)
                    .Include(p => p.Replies)
                        .ThenInclude(r => r.Author)
                    .OrderBy(p => p.CreatedAt) 
                    .ToListAsync();

                var postResponses = posts.Select(p => new PostResponse
                {
                    Id = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    ParentPostId = p.ParentPostId,
                    Author = new UserResponse
                    {
                        Id = p.Author.Id,
                        Username = p.Author.Username,
                        ProfileImages = p.Author.ProfileImages,
                        LastActive = p.Author.LastActive
                    },
                    Replies = p.Replies.Select(r => new PostResponse
                    {
                        Id = r.Id,
                        Content = r.Content,
                        CreatedAt = r.CreatedAt,
                        ParentPostId = r.ParentPostId,
                        Author = new UserResponse
                        {
                            Id = r.Author.Id,
                            Username = r.Author.Username,
                            ProfileImages = r.Author.ProfileImages,
                            LastActive = r.Author.LastActive
                        }
                    }).ToList()
                }).ToList();

                var topLevelPosts = postResponses.Where(p => p.ParentPostId == null).ToList();

                return Ok(topLevelPosts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching thread posts: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while fetching posts" });
            }
        }
    }
}