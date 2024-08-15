using DataAccessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.IRepo
{
    public interface IVenueRepo : IGenericRepo<Venue>
    {
        Task CreateVenue(Venue venue);
        Task<IEnumerable<Venue>> GetAllVenues();
        Task<bool> DeleteVenue(int id);
        Task UpdateVenue(Venue newVenue);
        Task<Venue> GetVenueById(int id);
    }
}
