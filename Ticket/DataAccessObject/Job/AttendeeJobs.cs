/*using DataAccessObject.IRepo;

namespace DataAccessObject.Job;

public class AttendeeJobs
{
    private readonly IAttendeeRepo _attendeeRepo;
    private readonly IAttendeeDetailRepo _attendeeDetailRepo;

    public AttendeeJobs(IAttendeeRepo attendeeRepo, IAttendeeDetailRepo attendeeDetailRepo)
    {
        _attendeeRepo = attendeeRepo;
        _attendeeDetailRepo = attendeeDetailRepo;
    }

    private async Task DeleteAttendeeWithDetailsAsync(int attendeeId)
    {
        var attendee = await _attendeeRepo.GetAttendeeByIdAsync(attendeeId);

        if (attendee != null)
        {
            // Delete AttendeeDetails first
            foreach (var detail in attendee.AttendeeDetails.ToList())
            {
                await _attendeeDetailRepo.RemoveAsync(detail);
            }

            // Delete Attendee
            await _attendeeRepo.RemoveAsync(attendee);
        }
    }

    public async Task HandlePaymentCancellationAsync(int attendeeId)
    {
        await DeleteAttendeeWithDetailsAsync(attendeeId);
    }
}*/