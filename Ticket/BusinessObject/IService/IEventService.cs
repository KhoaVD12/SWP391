using BusinessObject.Models.EventDTO;
using BusinessObject.Models.VenueDTO;
using BusinessObject.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Responses;
using DataAccessObject.Entities;
using DataAccessObject.Enums;

namespace BusinessObject.IService
{
    public interface IEventService
    {
        public Task<ServiceResponse<PaginationModel<ViewEventDTO>>> GetAllEvents(int page, int pageSize, string search,
            string sort);

        public Task<ServiceResponse<ViewEventDTO>> GetEventById(int id);
        public Task<ServiceResponse<ViewEventDTO>> CreateEvent(CreateEventDTO eventDTO);
        Task<ServiceResponse<bool>> AssignStaffToEventAsync(int staffId, int eventId);
        Task<ServiceResponse<EventStaffDTO?>> GetEventByStaffAsync(int staffId);

        public Task<ServiceResponse<string>> DeleteEvent(int id);
        public Task<ServiceResponse<ViewEventDTO>> UpdateEvent(int id, UpdateEventDTO eventDTO);
        Task<ServiceResponse<bool>> ChangeEventStatus(int eventId, ChangeEventStatusDTO statusDTO);
        Task<ServiceResponse<PaginationModel<ViewEventDTO>>> GetEventsByStatus(string status, int page, int pageSize);
    }
}