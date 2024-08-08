using DataAccessObject.Entities;

namespace DataAccessObject.IRepo
{
    public interface IUserRepo  : IGenericRepo<User>
    {
        Task<User> GetUserByEmailAddressAndPasswordHash(string email, string passwordHash);
        Task<User?> GetByEmailAsync(string email);
        Task<User> GetUserById(string id);
        Task<bool> CheckEmailAddressExisted(string emailaddress);
    }
}
