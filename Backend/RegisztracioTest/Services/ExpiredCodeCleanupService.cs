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
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // ellenőrzés gyakorisága

                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<RegistrationDbContext>();

                    var now = DateTime.Now;
                    var expiredCodes = await context.LoginCodes
                        .Where(c => c.CreatedAt <= now.AddMinutes(-5)) // létrehozás óta eltelt 5 perc
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
