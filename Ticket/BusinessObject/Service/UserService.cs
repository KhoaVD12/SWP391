using System.Data.Common;
using AutoMapper;
using BusinessObject.Commons;
using BusinessObject.IService;
using BusinessObject.Models;
using BusinessObject.Models.UserDTO;
using BusinessObject.Responses;
using BusinessObject.Ultils;
using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using Microsoft.Extensions.Options;

namespace BusinessObject.Service
{
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
                var token = user.GenerateJsonWebToken(_configuration, _configuration.JWTSection.SecretKey,
                    DateTime.Now);
                response.Data = token;
                response.Success = true;
                response.Role = auth;
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
                var userAccount = await _repo.CheckEmailAddressExisted(userObject.Email);
                if (userAccount == null)
                {
                    response.Success = false;
                    response.Message = "Email is already existed";
                    return response;
                }

                var userAccountRegister = _mapper.Map<User>(userObject);
                userAccountRegister.Password = HashPass.HashWithSHA256(userObject.Password);

                userAccountRegister.Status = "Active";
                userAccountRegister.Role = "Staff";

                await _repo.AddAsync(userAccountRegister);
                var accountRegistedDto = _mapper.Map<CreateUserDto>(userAccountRegister);
                response.Success = true;
                response.Data = accountRegistedDto;
                response.Message = "Register successfully.";
            }
            catch (DbException e)
            {
                response.Success = false;
                response.Message = "Database error occurred.";
                response.ErrorMessages = new List<string> { e.Message };
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = "Error";
                response.ErrorMessages = new List<string> { e.Message };
            }

            return response;
        }
    }
}