using BusinessObject.Models.EventDTO;
using BusinessObject.Models.VenueDTO;
using BusinessObject.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.IService
{
    public interface IEventService
    {
        public Task<ServiceResponse<PaginationModel<ViewEventDTO>>> GetAllEvents(int page, int pageSize, string search, string sort);
        public Task<ServiceResponse<ViewEventDTO>> GetEventById(int id);
        public Task<ServiceResponse<CreateEventDTO>> CreateEvent(CreateEventDTO eventDTO);
        public Task<ServiceResponse<bool>> DeleteEvent(int id);
        public Task<ServiceResponse<ViewEventDTO>> UpdateEvent(int id, ViewEventDTO eventDTO);
    }
}
