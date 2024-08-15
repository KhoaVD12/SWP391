using BusinessObject.Models.TicketDTO;
using BusinessObject.Responses;

namespace BusinessObject.IService
{
    public interface ITicketService
    {
        public Task<ServiceResponse<PaginationModel<ViewTicketDTO>>> GetAllTickets(int page, int pageSize);
        public Task<ServiceResponse<ViewTicketDTO>> GetTicketById(int id);
        public Task<ServiceResponse<CreateTicketDTO>> CreateTicket(CreateTicketDTO ticketDTO);
        public Task<ServiceResponse<bool>> DeleteTicket(int id);
        public Task<ServiceResponse<ViewTicketDTO>> UpdateTicket(int id, ViewTicketDTO ticketDTO);
    }
}
