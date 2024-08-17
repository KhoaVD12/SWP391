using System.Data.Common;
using AutoMapper;
using BusinessObject.Commons;
using BusinessObject.IService;
using BusinessObject.Models;
using BusinessObject.Models.UserDTO;
using BusinessObject.Responses;
using BusinessObject.Ultils;
using DataAccessObject.Enums;
using DataAccessObject.IRepo;

namespace BusinessObject.Service;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepo _repo;
    private readonly IMapper _mapper;
    private readonly AppConfiguration _configuration;

    public AuthenticationService(IUserRepo repo, AppConfiguration configuration, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<TokenResponse<string>> LoginAsync(LoginResquestDto loginForm)
    {
        var response = new TokenResponse<string>();
        try
        {
            if (loginForm == null)
            {
                throw new ArgumentNullException(nameof(loginForm));
            }

            var passHash = HashPass.HashWithSHA256(loginForm.Password);
            var user = await _repo.GetUserByEmailAddressAndPasswordHash(loginForm.Email, passHash);
            if (user == null)
            {
                response.Success = false;
                response.Message = "Invalid email or password.";
                return response;
            }

            var auth = user.Role.ToString();
            var token = user.GenerateJsonWebToken(_configuration, _configuration.JWTSection.Key, DateTime.Now);
            response.DataToken = token;
            response.Success = true;
            response.Message = "Login successful.";
            response.Role = auth;
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
            response.Message = "An error occurred.";
            response.ErrorMessages = new List<string> { ex.Message };
        }

        return response;
    }

    public async Task<TokenResponse<ResetPassDTO>> ResetPass(ResetPassDTO dto)
    {
        var response = new TokenResponse<ResetPassDTO>();
        try
        {
            var userAccount = await _repo.GetUserByEmailAsync(dto.Email);
            if (userAccount == null)
            {
                response.Success = false;
                response.Message = "Email not found";
                return response;
            }

            if (dto.Password != dto.ConfirmPassword)
            {
                response.Success = false;
                response.Message = "Password and Confirm Password do not match.";
                return response;
            }

            userAccount.Password = HashPass.HashWithSHA256(dto.Password);
            await _repo.UpdateAsync(userAccount);


            _mapper.Map<ResetPassDTO>(userAccount);
            response.Success = true;
            response.Message = "Password reset successfully.";
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