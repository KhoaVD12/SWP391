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
        Task<Venue> CreateVenue(Venue venue);
    }
}
