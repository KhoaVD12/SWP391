using BusinessObject.Models.EventDTO;
using BusinessObject.Models.TicketDTO;
using BusinessObject.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.IService
{
    public interface ITicketService
    {
        public Task<ServiceResponse<PaginationModel<ViewTicketDTO>>> GetAllTickets(int page, int pageSize, string sort);
        public Task<ServiceResponse<ViewTicketDTO>> GetTicketById(int id);
        public Task<ServiceResponse<ViewTicketDTO>> CreateTicket(CreateTicketDTO ticketDTO);
        public Task<ServiceResponse<bool>> DeleteTicket(int id);
        public Task<ServiceResponse<ViewTicketDTO>> UpdateTicket(int id, CreateTicketDTO ticketDTO);
    }
}
