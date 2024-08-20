using DataAccessObject.Entities;

namespace DataAccessObject.IRepo
{
    public interface IEventRepo : IGenericRepo<Event>
    {

        public Task<IEnumerable<Event>> GetEvent();
        public Task<Event?> GetEventById(int id);
        public Task<bool> DeleteEvent(int id);
        public Task UpdateEvent(int id, Event e);
        Task<Event?> CheckExistByTitle(string inputString);
        Task<IEnumerable<Event>> GetEventsByStatus(string status);
        Task<List<Event>> GetEventsByStaffIdAsync(int staffId);
    }
}
