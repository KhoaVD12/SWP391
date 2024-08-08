using BusinessObject;
using DataAccessObject.IRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.Repo
{
    public class UserRepo: IUserRepo
    {
        private IBaseRepo<User> baseRepo;
        public UserRepo(IBaseRepo<User> repo)
        {
            baseRepo= repo;
        }

        public async Task<User> Login(string email, string password)
        {
            try
            {
                return await baseRepo.Get()
                    .Include(p=>p.BoothRequests)
                    .Include(p=>p.Events)
                    .Where(u => u.Email == email && u.Password == password)
                    .FirstOrDefaultAsync();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
