using DataAccessObject.Entities;
using DataAccessObject.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BusinessObject.Service;

public class PaymentCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public PaymentCleanupService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // Adjust frequency as needed

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TicketContext>();

                var expirationPeriod = TimeSpan.FromHours(24); // Define your expiration period

                // Fetch attendees with pending payments that are expired
                var attendeesToRemove = await context.Attendees
                    .Where(a => a.PaymentStatus == PaymentStatus.PENDING && a.IsPaymentExpired(expirationPeriod))
                    .ToListAsync(stoppingToken);

                context.Attendees.RemoveRange(attendeesToRemove);
                await context.SaveChangesAsync(stoppingToken);
            }
        }
    }
}