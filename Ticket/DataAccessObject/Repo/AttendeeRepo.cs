using DataAccessObject.Entities;
using DataAccessObject.Enums;
using DataAccessObject.IRepo;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject.Repo;

public class AttendeeRepo : RepoBase<Attendee>, IAttendeeRepo
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

    public async Task<IEnumerable<Attendee>> GetAttendees()
    {
        return await _context.Attendees
            .Include(a => a.AttendeeDetails)
            .ToListAsync();
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
                         a.CheckInStatus.ToString().Contains(searchTerm)))
            .ToListAsync();
    }

    public async Task<bool> UpdateCheckInStatusAsync(int attendeeId, string status)
    {
        var attendee = await _context.Attendees.FindAsync(attendeeId);
        if (attendee == null)
        {
            return false;
        }

        attendee.CheckInStatus = CheckInStatus.CheckedIn;
        _context.Attendees.Update(attendee);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Attendee> GetAttendeeByCheckInCodeAsync(string checkInCode)
    {
        return await _context.Attendees
            .FirstOrDefaultAsync(a => a.CheckInCode == checkInCode);
    }

    public async Task<Attendee?> GetAttendeeByIdAsync(int id)
    {
        return await _context.Attendees
            .Include(a => a.AttendeeDetails)
            .Include(a => a.Transactions)
            .ThenInclude(t => t.PaymentMethodNavigation)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<Attendee>> GetUnpaidAttendeesAsync(TimeSpan expirationPeriod)
    {
        var now = DateTime.UtcNow;
        var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(now, localTimeZone);

        return await _context.Attendees
            .Where(a => a.PaymentStatus == PaymentStatus.PENDING &&
                        localDateTime - a.RegistrationDate > expirationPeriod)
            .ToListAsync();
    }
}