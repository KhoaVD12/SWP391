using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject.Repo
{
    public class EventRepo : RepoBase<Event>, IEventRepo
    {
        private readonly TicketContext _context;

        public EventRepo(TicketContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Event>> GetEvent()
        {
            return await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Staff)
                .Include(e => e.Ticket)
                .Include(e => e.Venue)
                .Include(e => e.Booths)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Event?> GetEventById(int id)
        {
            return await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Staff)
                .Include(e => e.Ticket)
                .Include(e => e.Venue)
                .Include(e => e.Booths)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<bool> DeleteEvent(int id)
        {
            var exist = await _context.Events.FindAsync(id);
            if (exist != null)
            {
                _context.Events.Remove(exist);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new Exception("Id Not Found");
            }
        }

        public async Task UpdateEvent(int id, Event e)
        {
            var exist = await _context.Events.FindAsync(id);
            if (exist != null)
            {
                exist.Title = e.Title;
                exist.StartDate = e.StartDate;
                exist.EndDate = e.EndDate;
                exist.VenueId = e.VenueId;
                exist.Description = e.Description;
                exist.OrganizerId = e.OrganizerId;
                exist.Status = e.Status;
                _context.Events.Update(exist);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Id Not Found");
            }
        }

        public async Task<bool> CheckExistByStartDateAndVenue(string inputString, int venueId)
        {
            return await _context.Events.AnyAsync(e =>
                e.StartDate.ToString() == inputString&&e.VenueId==venueId);
        }

        public async Task<IEnumerable<Event>> GetEventsByStatus(string status)
        {
            return await _context.Events.Where(e => e.Status == status)
                .ToListAsync();
        }

        public async Task<List<Event>> GetEventsByStaffIdAsync(int staffId)
        {
            return await _context.Events
                .Where(e => e.StaffId == staffId)
                .Include(e => e.Venue)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventByOrganizer(int organizerId)
        {
            return await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Staff)
                .Include(e => e.Ticket)
                .Include(e => e.Venue)
                .Where(e => e.OrganizerId == organizerId).ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByStatusAsync(string status)
        {
            return await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.Ticket)
                .Include(e => e.Booths)
                .Where(e => e.Status == status)
                .ToListAsync();
        }

        public async Task<bool> IsStaffAssignedToAnotherEventAsync(int staffId)
        {
            return await _context.Events
                .AnyAsync(e => e.StaffId == staffId);
        }

        public async Task<IEnumerable<Attendee>> GetAttendeesByEventAsync(int eventId)
        {
            return await _context.Attendees
                .Where(a => a.EventId == eventId)
                .Include(a => a.AttendeeDetails)
                .Include(a => a.Event)
                .ToListAsync();
        }
    }
}