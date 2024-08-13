using System.Text;
using AutoMapper;
using BusinessObject.IService;
using BusinessObject.Models.AttendeeDto;
using BusinessObject.Responses;
using BusinessObject.Ultils;
using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace BusinessObject.Service;

public class AttendeeService : IAttendeeService
{
    private readonly IConfiguration _configuration;
    private readonly IAttendeeRepo _attendeeRepo;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;

    public AttendeeService(IMapper mapper, IAttendeeRepo attendeeRepo, IConfiguration configuration,
        IMemoryCache memoryCache)
    {
        _mapper = mapper;
        _attendeeRepo = attendeeRepo;
        _configuration = configuration;
        _memoryCache = memoryCache;
    }

    public async Task<ServiceResponse<RegisterAttendeeDTO>> RegisterAttendeeAsync(
        RegisterAttendeeDTO registerAttendeeDto)
    {
        var response = new ServiceResponse<RegisterAttendeeDTO>();

        try
        {
            var existingAttendee = await _attendeeRepo.GetAttendeeByEventAndEmailAsync(registerAttendeeDto.EventId,
                registerAttendeeDto.AttendeeDetails.First().Email);
            if (existingAttendee != null)
            {
                response.Success = false;
                response.Message = "Attendee already registered for this event.";
                return response;
            }

            // Map DTO to entity
            var attendee = _mapper.Map<Attendee>(registerAttendeeDto);
            attendee.RegistrationDate = DateTime.UtcNow;
            attendee.CheckInStatus = "Pending";

            // Save to the database
            await _attendeeRepo.AddAsync(attendee);

            // Generate check-in code
            string checkInCode = GenerateCheckInCode();

            string formattedDate = attendee.RegistrationDate.ToString("f");

            // Send email
            foreach (var attendeeDetail in registerAttendeeDto.AttendeeDetails)
            {
                await SendEmail.SendRegistrationEmail(_memoryCache, attendeeDetail.Email, attendeeDetail.Name,
                    attendee.RegistrationDate, checkInCode);
            }

            // Return success response
            response.Data = registerAttendeeDto;
            response.Success = true;
            response.Message = "Attendee registered and email sent successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error registering attendee.";
            response.ErrorMessages = [ex.Message];
        }

        return response;
    }

    public async Task<AttendeeDetailDto?> GetAttendeeDetailsAsync(int id)
    {
        var attendee = await _attendeeRepo.GetByIdAsync(id);
        if (attendee == null) return null;

        var attendeeDetails = _mapper.Map<AttendeeDetailDto>(attendee);
        return attendeeDetails;
    }

    public async Task<ServiceResponse<UpdateAttendeeDto>> UpdateAttendeeAsync(UpdateAttendeeDto updateAttendeeDto)
    {
        var response = new ServiceResponse<UpdateAttendeeDto>();

        try
        {
            var attendee = await _attendeeRepo.GetByIdAsync(updateAttendeeDto.Id);
            if (attendee == null)
            {
                response.Success = false;
                response.Message = "Attendee not found.";
                return response;
            }

            _mapper.Map(updateAttendeeDto, attendee);
            await _attendeeRepo.UpdateAsync(attendee);

            response.Success = true;
            response.Message = "Attendee updated successfully.";
            response.Data = updateAttendeeDto;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<IEnumerable<AttendeeDto>>> GetAttendeesByEventAsync(int eventId)
    {
        var attendees = await _attendeeRepo.GetAttendeesByEventAsync(eventId);
        var attendeeDtos = _mapper.Map<IEnumerable<AttendeeDto>>(attendees);

        return new ServiceResponse<IEnumerable<AttendeeDto>>
        {
            Data = attendeeDtos,
            Success = true,
            Message = "Attendees retrieved successfully."
        };
    }

    public async Task<ServiceResponse<IEnumerable<AttendeeDto>>> SearchAttendeesAsync(int eventId, string searchTerm)
    {
        var attendees = await _attendeeRepo.SearchAttendeesAsync(eventId, searchTerm);
        var attendeeDtos = _mapper.Map<IEnumerable<AttendeeDto>>(attendees);

        return new ServiceResponse<IEnumerable<AttendeeDto>>
        {
            Data = attendeeDtos,
            Success = true,
            Message = "Search results retrieved successfully."
        };
    }

    public async Task<ServiceResponse<string>> ExportAttendeesToCsvAsync(int eventId)
    {
        var attendees = await _attendeeRepo.GetAttendeesByEventAsync(eventId);
        var csv = new StringBuilder();

        csv.AppendLine("Name,Email,Phone,CheckInStatus");

        foreach (var attendee in attendees)
        {
            var detail = attendee.AttendeeDetails.FirstOrDefault();
            csv.AppendLine($"{detail.Name},{detail.Email},{detail.Phone},{attendee.CheckInStatus}");
        }

        return new ServiceResponse<string>
        {
            Data = csv.ToString(),
            Success = true,
            Message = "CSV generated successfully."
        };
    }

    public async Task<ServiceResponse<bool>> UpdateCheckInStatusAsync(int attendeeId, string status)
    {
        var success = await _attendeeRepo.UpdateCheckInStatusAsync(attendeeId, status);
        return new ServiceResponse<bool>
        {
            Data = success,
            Success = success,
            Message = success ? "Check-in status updated successfully." : "Attendee not found."
        };
    }

    private string GenerateCheckInCode()
    {
        return Guid.NewGuid().ToString()[..8].ToUpper();
    }
}