using BusinessObject.IService;
using BusinessObject.Models.EventDTO;
using Microsoft.AspNetCore.Mvc;

namespace TicketAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }
        [HttpPost("Event")]
        public async Task<IActionResult> CreateEvent(CreateEventDTO eventDTO)
        {
            var result = await _eventService.CreateEvent(eventDTO);
            return Ok(result);
        }
        [HttpGet("Event")]
        public async Task<IActionResult> GetEvent()
        {
            var result = await _eventService.GetAllEvents();
            return Ok(result);
        }
        [HttpGet("Event/{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var result = await _eventService.GetEventById(id);
            return Ok(result);
        }
        [HttpDelete("Event")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var result = await _eventService.DeleteEvent(id);
            return Ok(result);
        }
        [HttpPut("Event")]
        public async Task<IActionResult> UpdateEvent(int id, ViewEventDTO eventDTO)
        {
            var result = await _eventService.UpdateEvent(id, eventDTO);
            return Ok(result);
        }
    }
}
