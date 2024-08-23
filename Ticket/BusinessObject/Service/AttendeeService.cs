using System.ComponentModel.DataAnnotations;
using System.Text;
using AutoMapper;
using BusinessObject.IService;
using BusinessObject.Models.AttendeeDto;
using BusinessObject.Responses;
using BusinessObject.Ultils;
using DataAccessObject.Entities;
using DataAccessObject.Enums;
using DataAccessObject.IRepo;

namespace BusinessObject.Service;

public class AttendeeService : IAttendeeService
{
    private readonly IAttendeeRepo _attendeeRepo;
    private readonly IEventRepo _eventRepo;
    private readonly IMapper _mapper;

    public AttendeeService(IMapper mapper, IAttendeeRepo attendeeRepo, IEventRepo eventRepo)
    {
        _mapper = mapper;
        _attendeeRepo = attendeeRepo;
        _eventRepo = eventRepo;
    }

    public async Task<ServiceResponse<RegisterAttendeeDTO>> RegisterAttendeeAsync(
        RegisterAttendeeDTO registerAttendeeDto)
    {
        var response = new ServiceResponse<RegisterAttendeeDTO>();

        try
        {
            // Check if the attendee is already registered for the event
            var existingAttendee = await _attendeeRepo.GetAttendeeByEventAndEmailAsync(
                registerAttendeeDto.EventId,
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
            attendee.PaymentStatus = PaymentStatus.PENDING; // Set default status
            attendee.CheckInCode = null; // Ensure CheckInCode is null initially

            // Save to the database
            await _attendeeRepo.AddAsync(attendee);

            response.Data = registerAttendeeDto;
            response.Success = true;
            response.Message = "Attendee registered successfully. Proceed to payment.";
            response.Id = attendee.Id;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error registering attendee.";
            response.ErrorMessages = new List<string> { ex.Message };
            Console.WriteLine(ex.ToString()); // Log detailed exception information
        }

        return response;
    }

    public async Task<ServiceResponse<PaginationModel<AttendeeDto>>> GetAttendees(int page, int pageSize)
    {
        var response = new ServiceResponse<PaginationModel<AttendeeDto>>();
        try
        {
            if (page <= 0)
            {
                page = 1;
            }

            var attendees = await _attendeeRepo.GetAttendees();

            var map = _mapper.Map<IEnumerable<AttendeeDto>>(attendees);

            var paging = await Pagination.GetPaginationEnum(map, page, pageSize);

            response.Data = paging;
            response.Success = true;
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = e.InnerException != null
                ? e.InnerException.Message + "\n" + e.StackTrace
                : e.Message + "\n" + e.StackTrace;
        }

        return response;
    }

    public async Task<ServiceResponse<AttendeeDetailDto?>> GetAttendeeDetailsAsync(int id)
    {
        var response = new ServiceResponse<AttendeeDetailDto?>();
        try
        {
            var attendee = await _attendeeRepo.GetAttendeeByIdAsync(id);
            if (attendee == null)
            {
                response.Success = false;
                response.Message = "Attendee not found.";
                response.Data = null;
                return response;
            }

            var attendeeDetails = _mapper.Map<AttendeeDetailDto>(attendee);
            response.Success = true;
            response.Message = " Retrieve attendee successfully";
            response.Data = attendeeDetails;
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = $"An error occurred: {e.Message}";
            response.Data = null;
        }

        return response;
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
        var response = new ServiceResponse<IEnumerable<AttendeeDto>>();

        try
        {
            // Check if the event exists
            var eventEntity = await _eventRepo.GetEventById(eventId);
            if (eventEntity == null)
            {
                response.Success = false;
                response.Message = "Event not found.";
                return response;
            }

            // Get attendees for the event
            var attendees = await _attendeeRepo.GetAttendeesByEventAsync(eventId);
            if (!attendees.Any())
            {
                response.Success = false;
                response.Message = "No attendees found for this event.";
                return response;
            }

            var attendeeDtos = _mapper.Map<IEnumerable<AttendeeDto>>(attendees);

            // Return the list of attendees
            response.Data = attendeeDtos;
            response.Success = true;
            response.Message = "Attendees retrieved successfully.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error retrieving attendees.";
            response.ErrorMessages = [ex.Message];
        }

        return response;
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

    public async Task<ServiceResponse<bool>> CheckInAttendeeByCodeAsync(string checkInCode)
    {
        var response = new ServiceResponse<bool>();

        // Find the attendee by check-in code
        var attendee = await _attendeeRepo.GetAttendeeByCheckInCodeAsync(checkInCode);
        if (attendee == null)
        {
            response.Success = false;
            response.Message = "Attendee not found";
            return response;
        }

        attendee.CheckInStatus = CheckInStatus.CheckedIn;
        await _attendeeRepo.UpdateAsync(attendee);

        response.Data = true;
        response.Success = true;
        response.Message = "Attendee checked in successfully.";
        return response;
    }
}