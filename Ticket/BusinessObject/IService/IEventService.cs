using BusinessObject.Models.EventDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Responses;

namespace BusinessObject.IService
{
    public interface IEventService
    {
        public Task<IEnumerable<ViewEventDTO>> GetAllEvents();
        public Task<ViewEventDTO> GetEventById(int id);
        public Task<ServiceResponse<CreateEventDTO>> CreateEvent(CreateEventDTO eventDTO);
        public Task<bool> DeleteEvent(int id);
        public Task<ViewEventDTO> UpdateEvent(int id, ViewEventDTO eventDTO);
    }
}
