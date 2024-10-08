﻿using DataAccessObject.Entities;

namespace DataAccessObject.IRepo
{
    public interface IUserRepo  : IGenericRepo<User>
    {
        Task<User> GetUserByEmailAddressAndPasswordHash(string email, string passwordHash);
        Task<User?> GetByEmailAsync(string email);
        Task<User> GetUserById(string id);
        Task<bool> CheckEmailAddressExisted(string emailaddress);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByRoleAsync(string role);
        Task<User?> GetUserById(int id);
        Task<IEnumerable<User?>> GetAllUsers();
        Task<IEnumerable<User?>> GetAllUsersSponsor();
        Task<IEnumerable<User?>> GetAllUsersStaff();
        Task<IEnumerable<User?>> GetAllUsersOrganizer();
        Task UpdateUser(User user);
        Task<User> GetUserByStatus(string role);
    }
}
