using BusinessObject.Models.TicketDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.IService
{
    public interface ITicketService
    {
        Task <CreateTicketDTO> CreateTicket(CreateTicketDTO ticketDTO);
    }
}
