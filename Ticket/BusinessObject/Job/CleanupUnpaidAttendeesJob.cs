using BusinessObject.IService;
using Quartz;

namespace BusinessObject.Job;

public class CleanupUnpaidAttendeesJob : IJob
{
    private readonly IAttendeeService _attendeeService;

    public CleanupUnpaidAttendeesJob(IAttendeeService attendeeService)
    {
        _attendeeService = attendeeService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var expirationPeriod = TimeSpan.FromHours(24); // Define expiration period, e.g., 24 hours
            await _attendeeService.CleanupUnpaidAttendeesAsync(expirationPeriod);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CleanupUnpaidAttendeesJob: {ex.Message}");
        }
    }
}