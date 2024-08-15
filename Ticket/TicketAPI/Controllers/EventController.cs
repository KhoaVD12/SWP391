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
        public async Task<IActionResult> CreateEvent([FromForm]CreateEventDTO eventDto)

        {
            var result = await _eventService.CreateEvent(eventDto);
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetEvent(int page, int pageSize, string search, string sort)
        {
            var result = await _eventService.GetAllEvents(page,pageSize,search,sort);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var result = await _eventService.GetEventById(id);
            return Ok(result);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var result = await _eventService.DeleteEvent(id);
            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateEvent(int id, ViewEventDTO eventDTO)
        {
            var result = await _eventService.UpdateEvent(id, eventDto);
            return Ok(result);
        }
    }
}