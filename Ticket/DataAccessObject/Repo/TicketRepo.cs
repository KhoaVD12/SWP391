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
        public async Task<Ticket> GetTicketById(int id)
        {
            return await _context.Set<Ticket>().Where(t => t.Id == id).SingleOrDefaultAsync();
        }
        public async Task<IEnumerable<Ticket>> GetTicket()
        {
            return await _context.Tickets.ToListAsync();
        }
        public async Task CreateTicket(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> DeleteTicket(int id)
        {
            var exist=await _context.Tickets.FindAsync(id);
            if(exist!=null)
            {
                _context.Tickets.Remove(exist);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new Exception("Id Not Found");
            }
        }
        public async Task UpdateTicket(int id, Ticket ticket)
        {
            var exist = await _context.Tickets.FindAsync(id);
            if(exist!=null)
            {
                exist.TicketSaleEndDate = ticket.TicketSaleEndDate;
                exist.EventId = ticket.EventId;
                exist.Price = ticket.Price;
                exist.Quantity = ticket.Quantity;
                _context.Tickets.Update(exist);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Id Not Found");
            }
        }
    }
}
