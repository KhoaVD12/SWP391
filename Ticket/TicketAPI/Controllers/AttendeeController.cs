using System.Text;
using BusinessObject.IService;
using BusinessObject.Models.AttendeeDto;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace TicketAPI.Controllers;

[ApiController]
[EnableCors("Allow")]
[Route("api/[controller]")]
public class AttendeeController : ControllerBase
{
    private readonly IAttendeeService _attendeeService;

    public AttendeeController(IAttendeeService attendeeService)
    {
        _attendeeService = attendeeService;
    }

    [HttpPost("register")]
    public async Task<ActionResult> RegisterAttendee([FromBody] RegisterAttendeeDTO registerAttendeeDto)
    {
        var result = await _attendeeService.RegisterAttendeeAsync(registerAttendeeDto);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetEvent([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var result = await _attendeeService.GetAttendees(page, pageSize);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAttendee(int id, [FromBody] UpdateAttendeeDto updateAttendeeDto)
    {
        if (id != updateAttendeeDto.Id)
        {
            return BadRequest(new { success = false, message = "Attendee ID mismatch" });
        }

        var result = await _attendeeService.UpdateAttendeeAsync(updateAttendeeDto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAttendeeDetails(int id)
    {
        var attendee = await _attendeeService.GetAttendeeDetailsAsync(id);
        if (attendee == null)
        {
            return NotFound(new { success = false, message = "Attendee not found" });
        }

        return Ok(attendee);
    }

    [HttpGet("event/{eventId}/attendees")]
    public async Task<IActionResult> GetAttendeesByEvent(int eventId)
    {
        var result = await _attendeeService.GetAttendeesByEventAsync(eventId);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPut("{attendeeId}/checkin")]
    public async Task<IActionResult> UpdateCheckInStatus(int attendeeId, [FromBody] string status)
    {
        var result = await _attendeeService.UpdateCheckInStatusAsync(attendeeId, status);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}