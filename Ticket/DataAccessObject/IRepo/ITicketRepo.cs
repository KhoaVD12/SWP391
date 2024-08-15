using DataAccessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.IRepo
{
    public interface ITicketRepo:IGenericRepo<Ticket>
    {
        Task<Ticket> GetTicketById(int id);
        Task<IEnumerable<Ticket>> GetTicket();
        Task CreateTicket(Ticket ticket);
        Task<bool> DeleteTicket(int id);
        Task UpdateTicket(int id, Ticket ticket);
    }
}
