using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return await _context.Events.ToListAsync();
        }
        public async Task<Event> GetEventById(int id)
        {
            return await _context.Set<Event>().Where(e => e.Id == id).SingleOrDefaultAsync();
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
                exist.StartDate= e.StartDate;
                exist.EndDate= e.EndDate;
                exist.VenueId = e.VenueId;
                exist.Description = e.Description;
                exist.OrganizerId= e.OrganizerId;
                exist.Status = e.Status;
                _context.Events.Update(exist);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Id Not Found");
            }
        }
        public async Task<Event?> CheckExistByTitle(string inputString)
        {
            return await _context.Events.FirstOrDefaultAsync(e => e.Title == inputString);
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
