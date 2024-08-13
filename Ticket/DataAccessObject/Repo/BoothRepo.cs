using DataAccessObject.Entities;
using DataAccessObject.IRepo;
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
    }
}
