using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject.Repo;

public class AttendeeRepo   : RepoBase<Attendee>, IAttendeeRepo
{
    private readonly TicketContext _context;

    public AttendeeRepo(TicketContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Attendee?> GetAttendeeByEventAndEmailAsync(int eventId, string email)
    {
        return await _context.Attendees
            .Include(a => a.AttendeeDetails)
            .FirstOrDefaultAsync(a => a.EventId == eventId && a.AttendeeDetails.Any(ad => ad.Email == email));
    }

    public async Task<IEnumerable<Attendee>> GetAttendeesByEventAsync(int eventId)
    {
        return await _context.Attendees
            .Include(a => a.AttendeeDetails)
            .Where(a => a.EventId == eventId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Attendee>> SearchAttendeesAsync(int eventId, string searchTerm)
    {
        return await _context.Attendees
            .Include(a => a.AttendeeDetails)
            .Where(a => a.EventId == eventId && 
                        (a.AttendeeDetails.Any(ad => ad.Name.Contains(searchTerm) || 
                                                     ad.Email.Contains(searchTerm)) ||
                         a.CheckInStatus.Contains(searchTerm)))
            .ToListAsync();
    }

    public async Task<bool> UpdateCheckInStatusAsync(int attendeeId, string status)
    {
        var attendee = await _context.Attendees.FindAsync(attendeeId);
        if (attendee == null)
        {
            return false;
        }

        attendee.CheckInStatus = status;
        _context.Attendees.Update(attendee);
        await _context.SaveChangesAsync();
        return true;
    }
}