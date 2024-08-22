using DataAccessObject.Entities;
using DataAccessObject.Enums;
using DataAccessObject.IRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.Repo
{
    public class BoothRepo:RepoBase<Booth>, IBoothRepo
    {
        private readonly TicketContext _context;
        public BoothRepo(TicketContext ticketContext):base(ticketContext)
        {
            _context = ticketContext;
        }
        public async Task CreateBooth(Booth booth)
        {
            await _context.AddAsync(booth);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Booth>> GetBooth()
        {
            return await _context.Booths.ToListAsync();
        }
        public async Task<Booth> GetBoothById(int id)
        {
            return await _context.Set<Booth>().Where(b => b.Id == id).SingleOrDefaultAsync();
        }
        public async Task<bool> DeleteBooth(int id)
        {
            var exist = await _context.Booths.Include(b=>b.BoothRequests).FirstOrDefaultAsync(b=>b.Id==id);
            if (exist != null)
            {
                _context.BoothRequests.RemoveRange(exist.BoothRequests);
                _context.Booths.Remove(exist);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new Exception("Id Not Found");
            }
        }
        public async Task UpdateBooth(int id, Booth booth)
        {
            var exist = await _context.Booths.Include(b => b.BoothRequests).FirstOrDefaultAsync(b => b.Id == id);
            if (exist == null)
            {
                throw new Exception("Id Not Found");
            }

            // Prevent updates if the booth status is "Closed"
            if (exist.Status.Equals(BoothStatus.Closed.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Cannot update a booth with status 'Closed'.");
            }

            // Allow updates if the booth status is "Pending" or "Opened"
            exist.EventId = booth.EventId;
            exist.Location = booth.Location;
            exist.SponsorId = booth.SponsorId;
            exist.Name = booth.Name;
            exist.Description = booth.Description;

            if (booth.Status.Equals(BoothStatus.Opened.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var boothRequest = exist.BoothRequests.FirstOrDefault();
                if (boothRequest != null && boothRequest.Status.Equals(BoothRequestStatus.Approved.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    exist.Status = BoothStatus.Opened.ToString();
                }
                else
                {
                    throw new Exception("Booth cannot be opened as the associated request is not approved.");
                }
            }
            else if (booth.Status.Equals(BoothStatus.Closed.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                exist.Status = BoothStatus.Closed.ToString();
            }
            else if (!booth.Status.Equals(BoothStatus.Pending.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Invalid Status");
            }

            _context.Booths.Update(exist);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> CheckExistByName(string inputString)
        {
            return await _context.Booths.AnyAsync(e => e.Name == inputString);
        }
        public async Task<bool> CheckEventExist(int eventId)
        {
            return await _context.Events.AnyAsync(e => e.Id == eventId);
        }
        public async Task<bool> CheckSponsorExist(int sponsorId)
        {
            return await _context.Users.AnyAsync(u => u.Id == sponsorId && u.Role == "Sponsor");
        }
        public async Task<bool> CheckBoothExist(int eventId, int sponsorId)
        {
            return await _context.Booths
                .AnyAsync(b => b.EventId == eventId && b.SponsorId == sponsorId);
        }
    }
}
