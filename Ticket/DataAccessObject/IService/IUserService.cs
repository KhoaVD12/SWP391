using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.IService
{
    public interface IUserService
    {
        Task<User> Login(string email, string password);
    }
}
