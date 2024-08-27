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
    public class GiftRepo:RepoBase<Gift>, IGiftRepo
    {
        private readonly TicketContext _context;
        public GiftRepo(TicketContext context):base(context) 
        {
            _context = context;
        }
        public async Task<IEnumerable<Gift>> GetAllGifts()
        {
            return await _context.Gifts.Include(g => g.Booth)
                .ThenInclude(b => b.BoothRequests)
                .ThenInclude(br => br.Sponsor).ToListAsync();
        }
        public async Task<IEnumerable<Gift>> GetGiftsBySponsorId(int sponsorId)
        {
            return await _context.Gifts.Include(g => g.Booth)
                .Where(g => g.Booth.SponsorId == sponsorId)
                .ToListAsync();
        }
        public async Task<Booth> GetBoothWithSponsor(int boothId)
        {
            return await _context.Booths
            .Include(b => b.BoothRequests)
            .ThenInclude(br => br.Sponsor)
            .FirstOrDefaultAsync(b => b.Id ==boothId);
        }
        public async Task<Gift> GetGiftById(int id)
        {
            return await _context.Set<Gift>().Include(g => g.Booth).Where(g => g.Id == id).SingleOrDefaultAsync();
        }
        public async Task<IEnumerable<Booth>> GetOpenBooth()
        {
            return await _context.Booths.Where(b => b.Status == BoothStatus.Opened.ToString()).ToListAsync();
        }
        public async Task<bool> CheckExistByNameAndBooth(string inputString, int boothId)
        {
            return await _context.Gifts.AnyAsync(e => e.Name.ToLower().Trim() == inputString.ToLower().Trim() && e.BoothId==boothId);
        }
        public async Task<IEnumerable<Gift>> GetGiftByBoothId(int boothId)
        {
            return await _context.Set<Gift>().Where(g => g.BoothId == boothId).ToListAsync();
        }
        public async Task CreateGift(Gift gift)
        {
            await _context.Gifts.AddAsync(gift);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> DeleteGift(int id)
        {
            var exist = await _context.Gifts.Include(g=>g.GiftReceptions).FirstOrDefaultAsync(g=>g.Id==id);
            if(exist != null)
            {
                if (exist.GiftReceptions.Any())
                {
                    throw new Exception("You have GiftReceptions with this gift");
                }
                _context.Gifts.Remove(exist);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new Exception("Id not found");
            }
        }
        public async Task UpdateGift(int id, Gift gift)
        {
            var exist = await _context.Gifts.FindAsync(id);
            if(exist != null)
            {
                exist.Quantity = gift.Quantity;
                exist.BoothId = gift.BoothId;
                exist.Description = gift.Description;
                exist.Name = gift.Name;
                _context.Gifts.Update(exist);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Id Not Found");
            }
        }
    }
}
