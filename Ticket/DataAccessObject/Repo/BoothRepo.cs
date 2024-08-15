using DataAccessObject.Entities;
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
            var exist = await _context.Booths.FindAsync(id);
            if (exist != null)
            {
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
            var exist = await _context.Booths.FindAsync(id);
            if(exist != null)
            {
                exist.Status = booth.Status;
                exist.EventId=booth.EventId;
                exist.Location = booth.Location;
                exist.SponsorId = booth.SponsorId;
                exist.Name = booth.Name;
                exist.Description = booth.Description;
                _context.Booths.Update(exist);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Id Not Found");
            }
        }
        public async Task<bool> CheckExistByName(string inputString)
        {
            return await _context.Booths.AnyAsync(e => e.Name == inputString);
        }
    }
}
