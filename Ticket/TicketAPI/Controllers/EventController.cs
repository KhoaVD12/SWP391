using BusinessObject.IService;
using BusinessObject.Models.EventDTO;
using BusinessObject.Responses;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace TicketAPI.Controllers
{
    [EnableCors("Allow")]
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
        [ProducesResponseType(typeof(ServiceResponse<CreateEventDTO>), StatusCodes.Status200OK)]
        [SwaggerResponse(200, "Create a new event", typeof(ServiceResponse<CreateEventDTO>))]
        public async Task<IActionResult> CreateEvent([FromForm] CreateEventDTO eventDto)

        {
            var result = await _eventService.CreateEvent(eventDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetEvent([FromQuery] int page = 1, [FromQuery] int pageSize = 5,
            [FromQuery] string search = "", [FromQuery] string sort = "")
        {
            var result = await _eventService.GetAllEvents(page, pageSize, search, sort);
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
        public async Task<IActionResult> UpdateEvent(int id, CreateEventDTO eventDTO)
        {
            var result = await _eventService.UpdateEvent(id, eventDTO);
            return Ok(result);
        }
    }
}