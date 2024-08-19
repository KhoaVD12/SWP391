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
    public class BoothRequestRepo:RepoBase<BoothRequest>, IBoothRequestRepo
    {
        private readonly TicketContext _context;
        public BoothRequestRepo(TicketContext context):base(context) 
        {
            _context = context;
        }
        public async Task CreateBoothRequest(BoothRequest request)
        {
            await _context.AddAsync(request);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<BoothRequest>> GetAllBoothRequest()
        {
            return await _context.BoothRequests.Include(r => r.Booth).Include(r => r.Sponsor).ToListAsync();
        }
        public async Task<bool> CheckExistByBoothId(int boothId)
        {
            return await _context.BoothRequests.AnyAsync(b => b.BoothId == boothId);
        }
        public async Task<bool> DeleteBoothRequest(int id)
        {
            var checkExist = await _context.BoothRequests.FindAsync(id);
            if (checkExist != null)
            {
                _context.BoothRequests.Remove(checkExist);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new Exception("Id Not Found");
            }
        }
        public async Task UpdateBoothRequest(int id, BoothRequest boothRequest)
        {
            var exist = await _context.BoothRequests.FindAsync(id);
            if(exist != null)
            {
                exist.SponsorId=boothRequest.SponsorId;
                exist.RequestDate=boothRequest.RequestDate;
                exist.Status=boothRequest.Status;
                exist.BoothId=boothRequest.BoothId;
                _context.BoothRequests.Update(exist);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Id Not Found");
            }
        }
        public async Task<BoothRequest> GetBoothRequestById(int id)
        {
            return await _context.Set<BoothRequest>().Where(b => b.Id == id).SingleOrDefaultAsync();
        }
    }
}
