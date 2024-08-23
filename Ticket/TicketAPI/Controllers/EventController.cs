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

        /// <summary>
        /// Create a new event.
        /// </summary>
        /// <param name="eventDto">Event details.</param>
        /// <returns>A newly created event.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/Event
        ///     {
        ///        "title": "Sample Event",
        ///        "startDate": "2024-08-15T00:00:00Z",
        ///        "endDate": "2024-08-16T00:00:00Z",
        ///        "organizerId": 1,
        ///        "venueId": 1,
        ///        "description": "Event Description",
        ///        "imageUrl": "example.jpg"
        ///     }
        /// </remarks>
        [HttpPost]
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

        /// <summary>
        /// Get a list of events with pagination, search, and sort options.
        /// </summary>                                                                                                  
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Number of events per page.</param>
        /// <param name="search">Search keyword.</param>
        /// <param name="sort">Sort by (e.g., "startdate", "enddate").</param>
        /// <returns>A paginated list of events.</returns>
        [HttpGet]
        public async Task<IActionResult> GetEvent([FromQuery] int page = 1, [FromQuery] int pageSize = 5,
            [FromQuery] string search = "", [FromQuery] string sort = "")
        {
            var result = await _eventService.GetAllEvents(page, pageSize, search, sort);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get an event by its ID.
        /// </summary>
        /// <param name="id">Event ID.</param>
        /// <returns>An event.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var result = await _eventService.GetEventById(id);
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Assigns a staff member to an event.
        /// </summary>
        /// <param name="staffId">The ID of the staff member.</param>
        /// <param name="eventId">The ID of the event.</param>
        /// <returns>A response indicating the success or failure of the assignment.</returns>
        [HttpPost("{eventId}/staff/{staffId}")]
        public async Task<IActionResult> AssignStaffToEvent(int staffId, int eventId)
        {
            var response = await _eventService.AssignStaffToEventAsync(staffId, eventId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Retrieves the events assigned to a specific staff member.
        /// </summary>
        /// <param name="staffId">The ID of the staff.</param>
        /// <returns>The events assigned to the staff member.</returns>
        [HttpGet("{staffId}/events")]
        public async Task<IActionResult> GetEventsByStaff(int staffId)
        {
            var response = await _eventService.GetEventByStaffAsync(staffId);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }


        /// <summary>
        /// Delete an event by its ID.
        /// </summary>
        /// <param name="id">Event ID.</param>
        /// <returns>A response indicating whether the event was deleted.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var result = await _eventService.DeleteEvent(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }


        /// <summary>
        /// Update an existing event.
        /// </summary>
        /// <param name="id">Event ID.</param>
        /// <param name="eventDto">Event details to update.</param>
        /// <returns>The updated event.</returns>
        /// <remarks>
        /// Example request:
        /// 
        ///     PUT /api/Event/1
        ///     {
        ///        "title": "Updated Event",
        ///        "startDate": "2024-08-17T00:00:00Z",
        ///        "endDate": "2024-08-18T00:00:00Z",
        ///        "venueId": 2,
        ///        "description": "Updated Description"
        ///     }
        /// </remarks>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ServiceResponse<UpdateEventDTO>), StatusCodes.Status200OK)]
        [SwaggerResponse(200, "Update event", typeof(ServiceResponse<UpdateEventDTO>))]
        public async Task<IActionResult> UpdateEvent(int id, [FromForm] UpdateEventDTO eventDto)
        {
            var result = await _eventService.UpdateEvent(id, eventDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Change the status of an event by admin.
        /// </summary>
        /// <param name="eventId">ID of the event to update.</param>
        /// <param name="statusDto">Event status details.</param>
        /// <returns>A response indicating the status change.</returns>
        [HttpPost("{eventId}/status")]
        public async Task<IActionResult> ChangeEventStatus(int eventId, [FromBody] ChangeEventStatusDTO statusDto)
        {
            var result = await _eventService.ChangeEventStatus(eventId, statusDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetEventsByStatus(string status, [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5)
        {
            var result = await _eventService.GetEventsByStatus(status, page, pageSize);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}