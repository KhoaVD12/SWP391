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
    public class TicketRepo:RepoBase<Ticket>, ITicketRepo
    {
        private readonly TicketContext _context;
        public TicketRepo(TicketContext ticketContext):base(ticketContext)
        {
            _context = ticketContext;
        }
        public async Task<IEnumerable<Ticket>> GetTicketByEventId(int eventId)
        {
            return await _context.Tickets.Where(t => t.EventId == eventId).ToListAsync();
        }
        public async Task CreateTicket(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
        }
    }
}
