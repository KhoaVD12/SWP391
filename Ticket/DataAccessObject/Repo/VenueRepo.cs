using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.Repo
{
    public class VenueRepo : RepoBase<Venue>, IVenueRepo
    {
        private readonly TicketContext _context;
        public VenueRepo(TicketContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Venue> CreateVenue(Venue venue)
        {
            await _context.Venues.AddAsync(venue);
            await _context.SaveChangesAsync();
            return venue;
        }
    }
}
