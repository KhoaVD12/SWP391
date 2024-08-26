using DataAccessObject.Entities;
using DataAccessObject.IRepo;

namespace DataAccessObject.Repo;

public class AttendeeDetailRepo : RepoBase<AttendeeDetail>, IAttendeeDetailRepo
{
    public AttendeeDetailRepo(TicketContext context) : base(context)
    {
    }
}