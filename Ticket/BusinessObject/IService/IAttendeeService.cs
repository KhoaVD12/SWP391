using BusinessObject.Models.AttendeeDto;
using BusinessObject.Responses;

namespace BusinessObject.IService;

public interface IAttendeeService
{
    Task<ServiceResponse<RegisterAttendeeDTO>> RegisterAttendeeAsync(RegisterAttendeeDTO registerAttendeeDto);
    Task<ServiceResponse<PaginationModel<AttendeeDto>>> GetAttendees(int page, int pageSize);
    Task<ServiceResponse<AttendeeDetailDto?>> GetAttendeeDetailsAsync(int id);
    Task<ServiceResponse<UpdateAttendeeDto>> UpdateAttendeeAsync(UpdateAttendeeDto updateAttendeeDto);
    Task<ServiceResponse<IEnumerable<AttendeeDto>>> GetAttendeesByEventAsync(int eventId);
    Task<ServiceResponse<IEnumerable<AttendeeDto>>> SearchAttendeesAsync(int eventId, string searchTerm);
    Task<ServiceResponse<string>> ExportAttendeesToCsvAsync(int eventId);
    Task<ServiceResponse<bool>> UpdateCheckInStatusAsync(int attendeeId, string status);
    Task<ServiceResponse<bool>> CheckInAttendeeByCodeAsync(string checkInCode);
    Task CleanupUnpaidAttendeesAsync(TimeSpan expirationPeriod);
    Task DeleteUnpaidAttendeeAsync(int attendeeId);
}