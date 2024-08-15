using System.Text;
using BusinessObject.IService;
using BusinessObject.Models.AttendeeDto;
using Microsoft.AspNetCore.Mvc;

namespace TicketAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendeeController : ControllerBase
{
    private readonly IAttendeeService _attendeeService;

    public AttendeeController(IAttendeeService attendeeService)
    {
        _attendeeService = attendeeService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAttendee([FromBody] RegisterAttendeeDTO registerAttendeeDto)
    {
        var result = await _attendeeService.RegisterAttendeeAsync(registerAttendeeDto);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPut("attendees/{id}")]
    public async Task<IActionResult> UpdateAttendee(int id, UpdateAttendeeDto updateAttendeeDto)
    {
        if (id != updateAttendeeDto.Id)
        {
            return BadRequest("Attendee ID mismatch");
        }

        var result = await _attendeeService.UpdateAttendeeAsync(updateAttendeeDto);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return NoContent();
    }

    [HttpGet("attendees/{id}")]
    public async Task<IActionResult> GetAttendeeDetails(int id)
    {
        var attendee = await _attendeeService.GetAttendeeDetailsAsync(id);
        if (attendee == null)
        {
            return NotFound();
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

    [HttpGet("event/{eventId}/attendees/export")]
    public async Task<IActionResult> ExportAttendeesToCsv(int eventId)
    {
        var result = await _attendeeService.ExportAttendeesToCsvAsync(eventId);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return File(Encoding.UTF8.GetBytes(result.Data), "text/csv", "attendees.csv");
    }

    [HttpPut("attendee/{attendeeId}/checkin")]
    public async Task<IActionResult> UpdateCheckInStatus(int attendeeId, [FromBody] string status)
    {
        var result = await _attendeeService.UpdateCheckInStatusAsync(attendeeId, status);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("checkin/qr")]
    public async Task<IActionResult> CheckInByQrCode([FromBody] string qrCode)
    {
        var result = await _attendeeService.CheckInAttendeeByCodeAsync(qrCode);
        if (!result.Success)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Message);
    }
}