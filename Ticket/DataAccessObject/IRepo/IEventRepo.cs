using DataAccessObject.Entities;

namespace DataAccessObject.IRepo
{
    public interface IEventRepo : IGenericRepo<Event>
    {

        public Task<IEnumerable<Event>> GetEvent();
        public Task<Event?> GetEventById(int id);
        public Task<bool> DeleteEvent(int id);
        public Task UpdateEvent(int id, Event e);
        Task<bool> CheckExistByStartDateAndVenue(string inputString, int venueId);
        Task<List<Event>> GetEventsByStaffIdAsync(int staffId);
        Task<IEnumerable<Event>> GetEventByOrganizer(int organizerId);
        Task<IEnumerable<Event>> GetEventsByStatusAsync(string status);
        Task<bool> IsStaffAssignedToAnotherEventAsync(int staffId);
    }
}
