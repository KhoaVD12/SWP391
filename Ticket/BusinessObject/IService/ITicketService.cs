﻿using BusinessObject.Models.TicketDTO;
using BusinessObject.Responses;

namespace BusinessObject.IService
{
    public interface ITicketService
    {
        public Task<ServiceResponse<PaginationModel<ViewTicketDTO>>> GetAllTickets(int page, int pageSize);
        public Task<ServiceResponse<ViewTicketDTO>> GetTicketById(int id);
        public Task<ServiceResponse<IEnumerable<ViewTicketDTO>>> GetTicketByEventId(int eventId);
        public Task<ServiceResponse<ViewTicketDTO>> CreateTicket(CreateTicketDTO ticketDTO);
        public Task<ServiceResponse<bool>> DeleteTicket(int id);
        public Task<ServiceResponse<ViewTicketDTO>> UpdateTicket(int id, CreateTicketDTO ticketDTO);
    }
}
