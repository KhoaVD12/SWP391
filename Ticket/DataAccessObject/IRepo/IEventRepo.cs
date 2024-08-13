using DataAccessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.IRepo
{
    public interface IEventRepo : IGenericRepo<Event>
    {

        public Task<IEnumerable<Event>> GetEvent();
        public Task<Event> GetEventById(int id);
        public Task<bool> DeleteEvent(int id);
        public Task UpdateEvent(Event e);
        Task<bool> CheckExistByTitle(string inputString);
    }
}
