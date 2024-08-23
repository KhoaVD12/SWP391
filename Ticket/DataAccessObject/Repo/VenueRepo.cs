using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject.Repo
{
    public class VenueRepo : RepoBase<Venue>, IVenueRepo
    {
        private readonly TicketContext _context;
        public VenueRepo(TicketContext context) : base(context)
        {
            _context = context;
        }
        public async Task CreateVenue(Venue venue)
        {
            await _context.Venues.AddAsync(venue);
            await _context.SaveChangesAsync();
            
        }
        public async Task<IEnumerable<Venue>> GetAllVenues()
        {
            return await _context.Venues.ToListAsync();
        }
        public async Task<bool> DeleteVenue(int id)
        {
            var exist = await _context.Venues.FindAsync(id);
            if (exist != null)
            {
                _context.Venues.Remove(exist);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new Exception("Id not found");
            }
        }
        public async Task UpdateVenue(int id, Venue newVenue)
        {
            var exist = await _context.Venues.FindAsync(id);
            if (exist != null)
            {
                exist.Description = newVenue.Description;
                exist.Name = newVenue.Name;
                exist.Status = newVenue.Status;
                _context.Venues.Update(exist);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Id Not Found");
            }
        }
        public async Task<Venue> GetVenueById(int id)
        {
            return await _context.Set<Venue>().Where(v => v.Id == id).SingleOrDefaultAsync();
        }
        
    }
}
