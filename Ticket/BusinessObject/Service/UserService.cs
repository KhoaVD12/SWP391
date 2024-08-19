using System.Data.Common;
using AutoMapper;
using BusinessObject.IService;
using BusinessObject.Models.UserDTO;
using BusinessObject.Responses;
using BusinessObject.Ultils;
using DataAccessObject.Entities;
using DataAccessObject.Enums;
using DataAccessObject.IRepo;

namespace BusinessObject.Service;

public class UserService : IUserService
{
    private readonly IUserRepo _userRepo;
    private readonly IMapper _mapper;

    public UserService(IUserRepo userRepo, IMapper mapper)
    {
        _userRepo = userRepo;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<PaginationModel<UserDTO>>> GetAllUsers(int page, int pageSize, string search,
        string sort)
    {
        var response = new ServiceResponse<PaginationModel<UserDTO>>();

        try
        {
            var users = await _userRepo.GetAllUsers();
            if (!string.IsNullOrEmpty(search))
            {
                users = users
                    .Where(u => u != null && (u.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                              u.Email.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            users = sort.ToLower() switch
            {
                "name" => users.OrderBy(u => u?.Name),
                "email" => users.OrderBy(u => u?.Email),
                "role" => users.OrderBy(u => u?.Role),
                "status" => users.OrderBy(u => u?.Status),
                _ => users.OrderBy(u => u?.Id).ToList()
            };
            var userDtOs = _mapper.Map<IEnumerable<UserDTO>>(users);

            var paginationModel =
                await Pagination.GetPaginationEnum(userDtOs, page, pageSize);
            response.Data = paginationModel;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Failed to retrieve users: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<CreateUserDto>> CreateUserAsync(CreateUserDto userObject)
    {
        var response = new ServiceResponse<CreateUserDto>();
        try
        {
            // Check if email already exists
            var userAccount = await _userRepo.CheckEmailAddressExisted(userObject.Email);

            // Prevent creation of additional admin accounts
            if (userObject.Role == Roles.ADMIN)
            {
                var existingAdmin = await _userRepo.GetUserByRoleAsync(Roles.ADMIN);
                if (existingAdmin != null)
                {
                    response.Success = false;
                    response.Message = "An admin already exists and cannot create another admin.";
                    return response;
                }
            }

            // Map the DTO to the User entity
            var userAccountRegister = _mapper.Map<User>(userObject);
            userAccountRegister.Password = HashPass.HashWithSHA256(userObject.Password);
            userAccountRegister.Status = Status.ACTIVE; // Default status

            // Save the user
            await _userRepo.AddAsync(userAccountRegister);

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
            response.ErrorMessages = new List<string> { e.Message };
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = "An error occurred.";
            response.ErrorMessages = new List<string> { e.Message };
        }

        return response;
    }

    public async Task<ServiceResponse<PaginationModel<UserDTO>>> GetAllUsersByStaff(int page, int pageSize,
        string search, string sort)
    {
        var response = new ServiceResponse<PaginationModel<UserDTO>>();

        try
        {
            var users = await _userRepo.GetAllUsersStaff();
            if (!string.IsNullOrEmpty(search))
            {
                users = users
                    .Where(u => u != null && (u.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                              u.Email.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            users = sort.ToLower() switch
            {
                "name" => users.OrderBy(u => u?.Name),
                "email" => users.OrderBy(u => u?.Email),
                "status" => users.OrderBy(u => u?.Status),
                _ => users.OrderBy(u => u?.Id).ToList()
            };
            var userDtOs = _mapper.Map<IEnumerable<UserDTO>>(users);

            var paginationModel =
                await Pagination.GetPaginationEnum(userDtOs, page, pageSize); // Adjust pageSize as needed

            response.Data = paginationModel;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Failed to retrieve staff users: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<PaginationModel<UserDTO>>> GetAllUsersBySponsor(int page, int pageSize,
        string search, string sort)
    {
        var response = new ServiceResponse<PaginationModel<UserDTO>>();

        try
        {
            var users = await _userRepo.GetAllUsersSponsor();
            if (!string.IsNullOrEmpty(search))
            {
                users = users
                    .Where(u => u != null && (u.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                              u.Email.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            users = sort.ToLower() switch
            {
                "name" => users.OrderBy(u => u?.Name),
                "email" => users.OrderBy(u => u?.Email),
                "status" => users.OrderBy(u => u?.Status),
                _ => users.OrderBy(u => u?.Id).ToList()
            };
            var userDtOs = _mapper.Map<IEnumerable<UserDTO>>(users);

            var paginationModel =
                await Pagination.GetPaginationEnum(userDtOs, page, pageSize);

            response.Data = paginationModel;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Failed to retrieve staff users: {ex.Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<PaginationModel<UserDTO>>> GetAllUsersByOrganizer(int page, int pageSize,
        string search, string sort)
    {
        var response = new ServiceResponse<PaginationModel<UserDTO>>();

        try
        {
            var users = await _userRepo.GetAllUsersOrganizer();
            if (!string.IsNullOrEmpty(search))
            {
                users = users
                    .Where(u => u != null && (u.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                              u.Email.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            users = sort.ToLower() switch
            {
                "name" => users.OrderBy(u => u?.Name),
                "email" => users.OrderBy(u => u?.Email),
                "status" => users.OrderBy(u => u?.Status),
                _ => users.OrderBy(u => u?.Id).ToList()
            };
            var userDtOs = _mapper.Map<IEnumerable<UserDTO>>(users);

            var paginationModel =
                await Pagination.GetPaginationEnum(userDtOs, page, pageSize);

            response.Data = paginationModel;
            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Failed to retrieve staff users: {ex.Message}";
        }

        return response;
    }


    public async Task<ServiceResponse<UserDTO>> GetUserById(int id)
    {
        var serviceResponse = new ServiceResponse<UserDTO>();

        try
        {
            var user = await _userRepo.GetUserById(id);
            if (user == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "User not found";
            }
            else
            {
                var userDto = _mapper.Map<UserDTO>(user);
                serviceResponse.Data = userDto;
                serviceResponse.Success = true;
            }
        }
        catch (Exception ex)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = ex.Message;
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<string>> UpdateUser(UserUpdateDTO userUpdate)
    {
        var serviceResponse = new ServiceResponse<string>();

        try
        {
            var userEntity = _mapper.Map<User>(userUpdate);
            await _userRepo.UpdateUser(userEntity);

            serviceResponse.Success = true;
            serviceResponse.Message = "User updated successfully";
        }
        catch (Exception ex)
        {
            serviceResponse.Success = false;
            serviceResponse.Message = $"Failed to update user: {ex.Message}";
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<UserDTO>> ChangeStatus(int userId, UserStatusDTO statusReq)
    {
        var result = new ServiceResponse<UserDTO>();
        try
        {
            var user = await _userRepo.GetUserById(userId);
            if (user == null)
            {
                result.Success = false;
                result.Message = "User not found";
                return result;
            }

            user.Status = statusReq.Status;
            await _userRepo.UpdateAsync(user);

            result.Success = true;
            result.Data = new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Status = user.Status
            };
            result.Message = "Status change successfully!";
        }
        catch (Exception e)
        {
            result.Success = false;
            result.Message = e.InnerException != null
                ? e.InnerException.Message + "\n" + e.StackTrace
                : e.Message + "\n" + e.StackTrace;
        }

        return result;
    }
}