using System.Data.Common;
using AutoMapper;
using BusinessObject.Commons;
using BusinessObject.IService;
using BusinessObject.Models;
using BusinessObject.Models.UserDTO;
using BusinessObject.Responses;
using BusinessObject.Ultils;
using DataAccessObject.Entities;
using DataAccessObject.Enums;
using DataAccessObject.IRepo;
using Microsoft.Extensions.Options;

namespace BusinessObject.Service;

public class UserService : IUserService
{
    private readonly IUserRepo _repo;
    private readonly IMapper _mapper;
    private readonly AppConfiguration _configuration;

    public UserService(IUserRepo repo, IOptions<AppConfiguration> configuration, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
        _configuration = configuration.Value;
    }

    public async Task<ServiceResponse<string>> LoginAsync(LoginResquestDto loginform)
    {
        var response = new ServiceResponse<string>();
        try
        {
            var passHash = HashPass.HashWithSHA256(loginform.Password);
            var user = await _repo.GetUserByEmailAddressAndPasswordHash(loginform.Email, passHash);
            if (user == null)
            {
                response.Success = false;
                response.Message = "Invalid username or password";
                return response;
            }

            var auth = user.Role;
            var token = user.GenerateJsonWebToken(_configuration, _configuration.JWTSection.Key,
                DateTime.Now);
            response.Data = token;
            response.Success = true;
            response.Message = "Login successful.";
            return response;
        }
        catch (DbException ex)
        {
            response.Success = false;
            response.Message = "Database error occurred.";
            response.ErrorMessages = new List<string> { ex.Message };
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error";
            response.ErrorMessages = new List<string> { ex.Message };
        }

        return response;
    }

    public async Task<ServiceResponse<CreateUserDto>> CreateStaff(CreateUserDto userObject)
    {
        var response = new ServiceResponse<CreateUserDto>();
        try
        {
            // Check if email already exists
            var userAccount = await _repo.CheckEmailAddressExisted(userObject.Email);
            if (userAccount != null)
            {
                response.Success = false;
                response.Message = "Email is already existed";
                return response;
            }

            // Prevent creation of additional admin accounts
            if (userObject.Role == Role.Admin)
            {
                var existingAdmin = await _repo.GetUserByRoleAsync(Role.Admin);
                if (existingAdmin != null)
                {
                    response.Success = false;
                    response.Message = "An Admin already exists and cannot create another Admin.";
                    return response;
                }
            }

            // Map the DTO to the User entity
            var userAccountRegister = _mapper.Map<User>(userObject);
            userAccountRegister.Password = HashPass.HashWithSHA256(userObject.Password);
            userAccountRegister.Status = "Active"; // Default status

            // Save the user
            await _repo.AddAsync(userAccountRegister);

            // Prepare the response
            var accountRegisteredDto = _mapper.Map<CreateUserDto>(userAccountRegister);
            response.Success = true;
            response.Data = accountRegisteredDto;
            response.Message = "User created successfully.";
        }
        catch (DbException e)
        {
            response.Success = false;
            response.Message = "Database error occurred.";
            response.ErrorMessages = new List<string> { e.Message, e.InnerException?.Message };
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = "An error occurred.";
            response.ErrorMessages = new List<string> { e.Message, e.InnerException?.Message };
        }

        return response;
    }
}