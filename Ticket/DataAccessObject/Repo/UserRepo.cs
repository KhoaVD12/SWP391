using DataAccessObject.Entities;
using DataAccessObject.Enums;
using DataAccessObject.IRepo;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject.Repo;

public class UserRepo: RepoBase<User>, IUserRepo
{
    private readonly TicketContext _context;
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

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserByRoleAsync(Role role)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Role == role);
    }

    public async Task<User?> GetUserById(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public Task<IEnumerable<User?>> GetAllUsers()
    {
        return Task.FromResult<IEnumerable<User?>>(_context.Users.ToList());
    }

    public async Task<IEnumerable<User?>> GetAllUsersSponsor()
    {
        return await _context.Users
            .Where(u => u.Role == Role.Sponsor)
            .ToListAsync();
    }

    public async Task<IEnumerable<User?>> GetAllUsersStaff()
    {
        return await _context.Users
            .Where(u => u.Role == Role.Staff)
            .ToListAsync();
    }

    public async Task<IEnumerable<User?>> GetAllUsersOrganizer()
    {
        return await _context.Users
            .Where(u => u.Role == Role.Organizer)
            .ToListAsync();
    }

    public async Task UpdateUser(User user)
    {
        var existingUser = await _context.Users.FindAsync(user.Id);
        if (existingUser == null)
        {
            throw new KeyNotFoundException("User not found");
        }

        // Update the properties
        existingUser.Name = user.Name;
        existingUser.Email = user.Email;

        // Do not update the password to avoid setting it to null
        // existingUser.Password remains unchanged

        _context.Users.Update(existingUser);
        await _context.SaveChangesAsync();
    }
}