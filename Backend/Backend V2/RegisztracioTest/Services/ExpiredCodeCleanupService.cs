using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RegisztracioTest.Data;
using RegisztracioTest.Model;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RegisztracioTest.Services
{
    public class ExpiredCodeCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ExpiredCodeCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // 5 percenként

                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<RegistrationDbContext>();

                    var expiredCodes = await context.LoginCodes
                        .Where(c => c.ExpiresAt <= DateTime.Now)
                        .ToListAsync(stoppingToken);

                    if (expiredCodes.Any())
                    {
                        context.LoginCodes.RemoveRange(expiredCodes);
                        await context.SaveChangesAsync(stoppingToken);
                    }
                }
            }
        }
    }
}
