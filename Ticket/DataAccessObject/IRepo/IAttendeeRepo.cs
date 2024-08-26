using DataAccessObject.Entities;

namespace DataAccessObject.IRepo;

public interface IAttendeeRepo  : IGenericRepo<Attendee>
{
    Task<Attendee?> GetAttendeeByEventAndEmailAsync(int eventId, string email);
    Task<IEnumerable<Attendee>> GetAttendees();
    Task<IEnumerable<Attendee>> GetAttendeesByEventAsync(int eventId);
    Task<IEnumerable<Attendee>> SearchAttendeesAsync(int eventId, string searchTerm);
    Task<bool> UpdateCheckInStatusAsync(int attendeeId, string status);
    Task<Attendee> GetAttendeeByCheckInCodeAsync(string checkInCode);
    Task<Attendee?> GetAttendeeByIdAsync(int id);
    Task<List<Attendee>> GetUnpaidAttendeesAsync(TimeSpan expirationPeriod);
}