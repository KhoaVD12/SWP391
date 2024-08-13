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
        Task<IEnumerable<Ticket>> GetTicketByEventId(int eventId);
        Task CreateTicket(Ticket ticket);
    }
}
