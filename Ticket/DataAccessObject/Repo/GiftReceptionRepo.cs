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
    public class GiftReceptionRepo:RepoBase<GiftReception>,IGiftReceptionRepo
    {
        private readonly TicketContext _context;
        public GiftReceptionRepo(TicketContext context):base(context) 
        {
            _context = context;
        }

        public async Task<bool> CheckAttendeeExist(int attendeeId)
        {
            return await _context.Attendees.AnyAsync(a => a.Id == attendeeId);
        }

        public async Task<bool> CheckGiftExist(int giftId)
        {
            return await _context.Gifts.AnyAsync(a => a.Id == giftId);
        }

        public async Task CreateGiftReception(GiftReception reception)
        {
            await _context.GiftReceptions.AddAsync(reception);
            await _context.SaveChangesAsync();
        }

        public async Task<GiftReception> GetReceptionByAttendeeId(int attendeeId)
        {
            return await _context.Set<GiftReception>().Where(g => g.AttendeeId == attendeeId).SingleOrDefaultAsync();
        }

        public async Task<GiftReception> GetReceptionByGiftId(int giftId)
        {
            return await _context.Set<GiftReception>().Where(g => g.GiftId == giftId).SingleOrDefaultAsync();
        }

        public async Task<GiftReception> GetReceptionById(int id)
        {
            return await _context.Set<GiftReception>().Where(g => g.Id == id).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<GiftReception>> GetReceptions()
        {
            return await _context.GiftReceptions.ToListAsync();
        }
    }
}
