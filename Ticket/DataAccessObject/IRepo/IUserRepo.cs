using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject.IRepo
{
    public interface IUserRepo
    {
        Task<User> Login(string email, string password);
    }
}
