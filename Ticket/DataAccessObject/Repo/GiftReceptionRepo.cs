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
        public async Task<bool> CheckMaxGiftQuantity(int giftId)
        {
            var currentNumberOfGiftReceptions=await _context.GiftReceptions.Where(a => a.GiftId == giftId).CountAsync();
            var giftMaxQuantity = await _context.Gifts
                                        .Where(g => g.Id == giftId)
                                        .Select(g => g.Quantity)
                                        .FirstOrDefaultAsync();
            return currentNumberOfGiftReceptions >= giftMaxQuantity;
        }

        public async Task CreateGiftReception(GiftReception reception)
        {
            await _context.GiftReceptions.AddAsync(reception);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<GiftReception>> GetReceptionByAttendeeId(int attendeeId)
        {
            return await _context.GiftReceptions.Include(g => g.Gift).Where(g => g.AttendeeId == attendeeId).ToListAsync();
        }

        public async Task<IEnumerable<GiftReception>> GetReceptionByGiftId(int giftId)
        {
            return await _context.GiftReceptions.Include(g => g.Gift).Where(g => g.GiftId == giftId).ToListAsync();
        }
        public async Task<IEnumerable<GiftReception>> GetReceptionByBoothId(int boothId)
        {
            return await _context.GiftReceptions.Include(g => g.Gift).Where(g => g.Gift.BoothId == boothId).ToListAsync();
        }

        public async Task<GiftReception> GetReceptionById(int id)
        {
            return await _context.Set<GiftReception>().Include(g => g.Gift).Where(g => g.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<GiftReception>> GetReceptions()
        {
            return await _context.GiftReceptions.Include(g=>g.Gift).ToListAsync();
        }
        public async Task<bool> DeleteGiftReception(int id)
        {
            var exist = await _context.GiftReceptions.FirstOrDefaultAsync(g => g.Id == id);
            if (exist != null)
            {
                _context.GiftReceptions.Remove(exist);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new Exception("Id not found");
            }
        }
        public async Task UpdateReception(int id, GiftReception reception)
        {
            var exist = await _context.GiftReceptions.FirstOrDefaultAsync(g => g.Id == id);
            if (exist != null)
            {
                exist.AttendeeId= reception.AttendeeId;
                exist.GiftId= reception.GiftId;
                exist.ReceptionDate= reception.ReceptionDate;
                _context.GiftReceptions.Update(exist);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Id Not found");
            }
        }
        public async Task<bool> CheckExistByAttendeeIdAndGiftId(int attendeeId, int giftId)
        {
            return await _context.GiftReceptions.AnyAsync(g => g.AttendeeId == attendeeId && g.GiftId == giftId);
        }
    }
}
