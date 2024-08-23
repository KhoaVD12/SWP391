using BusinessObject.Models.EventDTO;
using BusinessObject.Responses;

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
        public Task<ServiceResponse<PaginationModel<ViewOrganizerEventDTO>>> GetEventByOrganizer(int organizerId, int page, int pageSize);
    }
}