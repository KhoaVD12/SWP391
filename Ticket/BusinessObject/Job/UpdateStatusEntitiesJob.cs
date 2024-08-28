using DataAccessObject.Entities;
using DataAccessObject.Enums;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace BusinessObject.Job;

public class UpdateStatusEntitiesJob : IJob
{
    private readonly TicketContext _context;

    public UpdateStatusEntitiesJob(TicketContext context)
    {
        _context = context;
    }


    public async Task Execute(IJobExecutionContext context)
    {
        await UpdateEventStatusAsync();
    }

    private async Task UpdateEventStatusAsync()
    {
        var now = DateTime.UtcNow;
        var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(now, localTimeZone);


        var activeEvents = await _context.Events
            .Where(e => e.StartDate <= localDateTime && e.EndDate > localDateTime &&
                        e.Status != EventStatus.ONGOING && e.Status != EventStatus.CANCEL && e.Status != EventStatus.PENDING)
            .ToListAsync();

        var endedEvents = await _context.Events
            .Where(e => e.EndDate <= localDateTime && e.Status != EventStatus.ENDED && e.Status != EventStatus.CANCEL && e.Status != EventStatus.PENDING)
            .ToListAsync();

        if (activeEvents.Count != 0)
        {
            foreach (var eventItem in activeEvents)
            {
                eventItem.Status = EventStatus.ONGOING; // Set status to "OnGoing"
            }
        }

        if (endedEvents.Count != 0)
        {
            foreach (var eventItem in endedEvents)
            {
                eventItem.Status = EventStatus.ENDED; // Set status to "Ended"
            }
        }

        await _context.SaveChangesAsync();
    }
}