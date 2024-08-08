using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject.Repo;

public class UserRepo: RepoBase<User>, IUserRepo
{
    private new readonly TicketContext _context;
    public UserRepo(TicketContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User> GetUserByEmailAddressAndPasswordHash(string email, string passwordHash)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(record => record.Email == email && record.Password == passwordHash);
        if (user is null)
        {
            throw new Exception("Email & password is not correct");
        }

        return user;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Set<User>().FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> GetUserById(string id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<bool> CheckEmailAddressExisted(string emailaddress)
    {
        return await _context.Users.AnyAsync(u => u.Email == emailaddress);
    }
}