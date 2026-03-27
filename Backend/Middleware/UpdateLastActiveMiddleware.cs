using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RegisztracioTest.Data;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RegisztracioTest.Middleware
{
    public class UpdateLastActiveMiddleware
    {
        private readonly RequestDelegate _next;

        public UpdateLastActiveMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Update LastActive BEFORE processing the request
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    await UpdateUserLastActive(context, userId);
                }
            }

            // Then process the request
            await _next(context);
        }

        private async Task UpdateUserLastActive(HttpContext context, int userId)
        {
            try
            {
                // Create a new scope to avoid DbContext conflicts
                using (var scope = context.RequestServices.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<RegistrationDbContext>();

                    var user = await dbContext.Users.FindAsync(userId);
                    if (user != null)
                    {
                        user.LastActive = DateTime.UtcNow;
                        await dbContext.SaveChangesAsync();

                        // Optional: log for debugging
                        Console.WriteLine($"✅ Updated LastActive for user {user.Username} to {user.LastActive}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Don't crash the request if this fails
                Console.WriteLine($"⚠️ Failed to update LastActive: {ex.Message}");
            }
        }
    }
}