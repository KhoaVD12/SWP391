using DataAccessObject.Entities;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace DataAccessObject.Job;

public class DeleteExpiredEntitiesJob : IJob
{
    private readonly TicketContext _context;

    public DeleteExpiredEntitiesJob(TicketContext context)
    {
        _context = context;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await DeleteExpiredEventsAsync();
        await DeleteExpiredTicketsAsync();
    }

    private async Task DeleteExpiredTicketsAsync()
    {
        var currentDateTime = DateTime.UtcNow;

        var expiredTickets = await _context.Tickets
            .Where(t => t.TicketSaleEndDate < currentDateTime)
            .ToListAsync();

        if (expiredTickets.Any())
        {
            _context.Tickets.RemoveRange(expiredTickets);
            await _context.SaveChangesAsync();
        }
    }

    private async Task DeleteExpiredEventsAsync()
    {
        var currentDateTime = DateTime.UtcNow;

        var expiredEvents = await _context.Events
            .Where(e => e.StartDate < currentDateTime)
            .ToListAsync();

        if (expiredEvents.Any())
        {
            _context.Events.RemoveRange(expiredEvents);
            await _context.SaveChangesAsync();
        }
    }
}