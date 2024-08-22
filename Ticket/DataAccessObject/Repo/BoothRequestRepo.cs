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
            var exist = await _context.BoothRequests.Include(b => b.Booth).FirstOrDefaultAsync(b => b.Id == id);
            if (exist == null)
            {
                throw new Exception("Id Not Found");
            }

            exist.SponsorId = boothRequest.SponsorId;
            exist.RequestDate = boothRequest.RequestDate;
            exist.BoothId = boothRequest.BoothId;

            if (boothRequest.Status.Equals(BoothRequestStatus.Approved.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                exist.Status = BoothRequestStatus.Approved.ToString();

                // Update booth status only if it's not "Closed"
                if (!exist.Booth.Status.Equals(BoothStatus.Closed.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    exist.Booth.Status = BoothStatus.Opened.ToString();
                }
            }
            else if (boothRequest.Status.Equals(BoothRequestStatus.Rejected.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                exist.Status = BoothRequestStatus.Rejected.ToString();

                // Update booth status only if it's not "Closed"
                if (!exist.Booth.Status.Equals(BoothStatus.Closed.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    exist.Booth.Status = BoothStatus.Closed.ToString();
                }
            }
            else
            {
                throw new Exception("Invalid Status");
            }

            _context.BoothRequests.Update(exist);
            await _context.SaveChangesAsync();
        }
        public async Task<BoothRequest> GetBoothRequestById(int id)
        {
            return await _context.Set<BoothRequest>().Where(b => b.Id == id).SingleOrDefaultAsync();
        }
        public async Task<IEnumerable<BoothRequest>> GetBoothRequestBySponsorId(int sponsorId)
        {
            return await _context.BoothRequests.Include(r => r.Booth).Include(r => r.Sponsor).Where(b => b.SponsorId == sponsorId).ToListAsync();
        }
        public async Task<IEnumerable<BoothRequest>> GetBoothRequestByEventId(int eventId)
        {
            return await _context.BoothRequests.Include(b=>b.Booth).Include(r => r.Sponsor).Where(b => b.Booth.EventId == eventId).ToListAsync();
        }
    }
}
