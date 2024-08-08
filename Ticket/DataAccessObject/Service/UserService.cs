using BusinessObject;
using DataAccessObject.IRepo;
using DataAccessObject.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.Service
{
    public class UserService: IUserService
    {
        private readonly IUserRepo _repo;
        public UserService(IUserRepo repo)
        {
            _repo = repo;
        }

        public async Task<User> Login(string email, string password)
        {
            return await _repo.Login(email, password);
        }
    }
}
